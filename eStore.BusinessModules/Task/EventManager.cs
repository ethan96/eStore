using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using Amib.Threading;
using System.Threading;
using System.Diagnostics;
using eStore.POCOS.DAL;
using eStore.POCOS;

namespace eStore.BusinessModules.Task
{
    public  class EventManager
    {
        private EventManager(String storeID) { _storeID = storeID; }
        private String _storeID;

        private static Dictionary<String, EventManager> _eventManagers = new Dictionary<string, EventManager>();
        private static bool? _productionMode;
        private static bool _primary = false;
        /// <summary>
        /// This property returns true if current server is a primary production server.
        /// </summary>
        private static bool primaryProduction
        {
            get 
            {
                if (_productionMode.HasValue == false)
                {
                    //get productionMode value
                    String testingMode = System.Configuration.ConfigurationManager.AppSettings.Get("TestingMode");
                    if (!String.IsNullOrEmpty(testingMode) && testingMode.ToLower().Equals("false"))
                        _productionMode = true;
                    else
                        _productionMode = false;
                    //get primary flag
                    String clusterRole = System.Configuration.ConfigurationManager.AppSettings.Get("ClusterRole");
                    if (!String.IsNullOrEmpty(clusterRole) && clusterRole.ToLower().Equals("primary"))
                        _primary = true;
                    else
                        _primary = false;
                }
                return (_productionMode.GetValueOrDefault() && _primary);
            }
        }

        /// <summary>
        /// This method is for singleton practice.
        /// </summary>
        /// <param name="storeID"></param>
        /// <returns></returns>
        public static EventManager getInstance(String storeID)
        {
            EventManager manager = null;
            if (_eventManagers.ContainsKey(storeID) == false)
            {
                manager = new EventManager(storeID);
                _eventManagers.Add(storeID, manager);

                if (primaryProduction)    //only start these thread
                {
                    //start abandoned cart event publisher
                    AbandonedCartEventPublisher routinePublisher = null;
                    POCOS.Store store = CachePool.getInstance().getStore(storeID);
                    if (store == null)
                        routinePublisher = new AbandonedCartEventPublisher(storeID, "http://buy.advantech.com");
                    else
                        routinePublisher = new AbandonedCartEventPublisher(storeID, @"http://"+store.StoreURL);
                    manager.QueueCommonTask(routinePublisher);

                    //resume not-finished tasks
                    TaskHelper helper = new TaskHelper();
                    List<POCOS.Task> tasks = helper.getPendingTasks(storeID);
                    //Converting pending tasks to EventPublishers
                    foreach (POCOS.Task task in tasks)
                    {
                        TaskBase restoredTask = null;
                        if (!String.IsNullOrEmpty(task.ClassName))
                            restoredTask = manager.deserializeTask(task.SerializedEntity, task.ClassName);

                        if (restoredTask != null)
                        {
                            restoredTask.eventInfo = task;
                            manager.PublishNewEvent(restoredTask, true);
                        }
                    }
                }
            }
            else
                manager = _eventManagers[storeID];

            return manager;
        }

        private IWorkItemsGroup _eventPublisherGroup;
        private IWorkItemsGroup EventPublisherGroup
        {
            get
            {
                if (_eventPublisherGroup == null)
                {
                    //create a thread pool with 20 max threads and min 3 threads
                    _eventPublisherGroup = new SmartThreadPool(600000 /*10 minutes idle time*/, 20, 3);
                    _eventPublisherGroup.Name = "Event Publisher Thread Manager";
                }
                return _eventPublisherGroup;
            }
        }

        /// <summary>
        /// This retrialItemsGroup is a thread pool executing delay before resumming failed event publisher to 
        /// retry
        /// </summary>
        private IWorkItemsGroup _genericWorkerGroup;
        private IWorkItemsGroup GenericWorkerGroup
        {
            get
            {
                if (_genericWorkerGroup == null)
                {
                    //create a thread pool with 20 max threads and min 3 threads
                    _genericWorkerGroup = new SmartThreadPool(600000 /*10 minutes idle time*/, 40, 3);
                    _genericWorkerGroup.Name = "Generic Worker Thread Manager";
                }
                return _genericWorkerGroup;
            }
        }

        /// <summary>
        /// This function is to register a new event publisher to be performed
        /// </summary>
        /// <param name="eventPublisher"></param>
        /// <returns></returns>
        public bool PublishNewEvent(TaskBase eventPublisher, bool resuming = false)
        {
            bool status = true;

            if (_productionMode.GetValueOrDefault()== true && eventPublisher.PreProcess() )
            {
                if (!resuming)
                    prepareToRun(eventPublisher);

                IWorkItemResult rlt = EventPublisherGroup.QueueWorkItem(eventPublisher.execute,
                                                                                                        eventPublisher, this.OnPublishCompleted,
                                                                                                        WorkItemPriority.Normal);
                if (rlt.Exception != null)
                {
                    setEventPublishStatus(eventPublisher, false);
                    status = false;
                }
            }
            else
                status = false;

            return status;
        }


        /// <summary>
        /// This function is to register a new common task to be executed
        /// </summary>
        /// <param name="eventPublisher"></param>
        /// <returns></returns>
        public bool QueueCommonTask(TaskBase commonTask)
        {
            bool status = true;

            if (commonTask.PreProcess())
            {
                IWorkItemResult rlt = EventPublisherGroup.QueueWorkItem(commonTask.execute,
                                                                                                        commonTask, this.OnCommonTaskCompleted,
                                                                                                        WorkItemPriority.Normal);
                if (rlt.Exception != null)
                {
                    //setEventPublishStatus(eventPublisher, false);
                    status = false;
                }
            }
            else
                status = false;

            return status;
        }


        /// <summary>
        /// This is a callback function for thread pool manager to inform Event manager the complete of 
        /// EventPublisher execution.
        /// </summary>
        /// <param name="workItemResult"></param>
        public void OnPublishCompleted(IWorkItemResult workItemResult)
        {
            if (workItemResult.State != null && workItemResult.State is EventPublisher)
            {
                EventPublisher eventPublisher = (EventPublisher)workItemResult.State;

                if (workItemResult.Exception != null || workItemResult.IsCanceled)   //failed
                    setEventPublishStatus(eventPublisher, false);
                else
                {
                    if (eventPublisher.Status == TaskStatus.Completed)
                        setEventPublishStatus(eventPublisher, true);
                    else
                        setEventPublishStatus(eventPublisher, false);
                }
            }
        }

        /// <summary>
        /// This is a callback function for handling the completion of common task execution.
        /// </summary>
        /// <param name="workItemResult"></param>
        public void OnCommonTaskCompleted(IWorkItemResult workItemResult)
        {
            if (workItemResult.State != null)
            {
                if (workItemResult.State is DeferAgent) //defer for certain time before re-execute a failed task
                {
                    DeferAgent deferAgent = (DeferAgent)workItemResult.State;

                    if (deferAgent.Status == TaskStatus.Completed)
                    {
                        if (deferAgent.DeferredTask is EventPublisher)
                            PublishNewEvent(deferAgent.DeferredTask, true);  //resumbit event publisher to publish event
                        else
                            QueueCommonTask(deferAgent.DeferredTask);
                    }
                }
                else  //common task
                {
                    TaskBase task = (TaskBase)workItemResult.State;
                    if (task.NeedRerun)
                    {
                        if (task.DeferDuration > 0)
                        {
                            DeferAgent agent = new DeferAgent(task, task.DeferDuration);
                            GenericWorkerGroup.QueueWorkItem(agent.execute, agent, this.OnCommonTaskCompleted, WorkItemPriority.Normal);
                        }
                        else
                        {
                            GenericWorkerGroup.QueueWorkItem(task.execute, task, this.OnCommonTaskCompleted, WorkItemPriority.Normal);
                        }
                        //clean up re-run flag to make sure no running forever situation.
                        task.NeedRerun = false;
                    }
                }
            }
        }

        /// <summary>
        /// This function is to convert eventPublisher details to a storable event instance and
        /// save the event in case server is unexpectedly down.  EventManager will be able
        /// to restore and resume the not-yet published events.
        /// </summary>
        /// <param name="eventPublisher"></param>
        /// <returns></returns>
        private void prepareToRun(TaskBase eventPublisher)
        {
            POCOS.Task eventInfo = new POCOS.Task();
            try
            {
                eventInfo.ID = eventPublisher.ID;
                eventInfo.StoreID = _storeID;
                eventInfo.Name = eventPublisher.Name;
                eventInfo.StartAt = eventPublisher.StartAt;
                eventInfo.EndAt = eventPublisher.EndAt;
                eventInfo.AttemptTimes = eventPublisher.AttemptTimes;
                eventInfo.Status = "Waiting to be published";
                eventInfo.ClassName = eventPublisher.GetType().FullName;
                eventInfo.SerializedEntity = serializeTask(eventPublisher);
                eventInfo.save();
                //store event along with eventPublisher to later reference
                eventPublisher.eventInfo = eventInfo;
            }
            catch (Exception e)
            {
                eStore.Utilities.eStoreLoger.Error("Failed at prepare for event publish : " + e.StackTrace);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventPublisher"></param>
        /// <param name="status">True : success, False : fail</param>
        private void setEventPublishStatus(TaskBase eventPublisher, bool status /* true : success, false : fail*/)
        {
            if (status) //success
            {
                eventPublisher.OnCompleted();
                if (eventPublisher.eventInfo != null)
                {
                    POCOS.Task eventInfo = (POCOS.Task)eventPublisher.eventInfo;
                    eventInfo.Status = "Success";
                    eventInfo.EndAt = DateTime.Now;
                    eventInfo.AttemptTimes++;
                    //remove event from physical queue
                    eventInfo.delete();
                }
            }
            else
            {
                eventPublisher.OnFailed();
                if (eventPublisher.eventInfo != null)
                {
                    POCOS.Task eventInfo = (POCOS.Task)eventPublisher.eventInfo;
                    eventInfo.Status = "Fail";
                    eventInfo.EndAt = DateTime.Now;
                    eventInfo.AttemptTimes++;
                    eventInfo.save();

                    //put event publsiher to retry queue.  The failed event publisher will be re-executed after 1 minute defer
                    if (eventInfo.AttemptTimes < 6)
                    {
                        DeferAgent agent = new DeferAgent(eventPublisher);
                        GenericWorkerGroup.QueueWorkItem(agent.execute, agent, this.OnCommonTaskCompleted, WorkItemPriority.Normal);
                    }
                }
            }
        }

        /// <summary>
        /// This function is to serialize Task for save
        /// </summary>
        /// <param name="eventPublisher"></param>
        /// <returns></returns>
        private string serializeTask(TaskBase task)
        {
            StringWriter stringWriter = new StringWriter();
            XmlSerializer xmlSerializer = new XmlSerializer(task.GetType());
            xmlSerializer.Serialize(stringWriter, task);
            string serializedXML = stringWriter.ToString();
            return serializedXML;
        }


        /// <summary>
        /// This function is to reconstruct Task from serialized string
        /// </summary>
        /// <param name="serializedXML"></param>
        /// <param name="targetClass"></param>
        /// <returns></returns>
        private TaskBase deserializeTask(String serializedXML, String targetClass)
        {
            try
            {
                Type classType = Type.GetType(targetClass);
                XmlSerializer xmlSerializer = new XmlSerializer(classType);
                StringReader rd = new StringReader(serializedXML);
                Object eventPublisher = xmlSerializer.Deserialize(rd);
                if (eventPublisher != null)
                {
                    return (TaskBase)eventPublisher;
                }
                else
                    return null;
            }
            catch (Exception)
            {
                return null;
            }
        }


        private static DateTime logFileDate = DateTime.MinValue;
        private static String logFileName = "d:\\SmartThreadPoollog.txt";

        private static void writeLog(string logstr)
        {
            //generate new file if the logging date is not the same
            if (logFileDate != DateTime.Now.Date)
            {
                logFileDate = DateTime.Now.Date;
                logFileName = String.Format("d:\\eStoreSmartThreadPoollog\\{0}.txt", logFileDate.ToString("yyyyMMdd"));
            }

            //create a writer and open the file
            StreamWriter sw;
            sw = File.AppendText(logFileName);
            // write a line of text to the file
            sw.WriteLine(DateTime.Now + "--:" + logstr);
            // close the stream
            sw.Close();
        }
    }
}
