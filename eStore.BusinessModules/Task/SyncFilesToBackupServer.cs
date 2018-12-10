using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32.TaskScheduler;

namespace eStore.BusinessModules.Task
{
    public class SyncFilesToBackupServer : TaskBase
    {
        public SyncFilesToBackupServer()
        { }
        private string _taskName;
        public SyncFilesToBackupServer(string taskName)
        {
            _taskName = taskName;
        }
        const int delaytime = 30;
        public override object execute(object obj)
        {
            if (string.IsNullOrEmpty(_taskName))
                return null;
            try
            {
                using (TaskService tasksrvc = new TaskService("172.21.1.19", "APIAgent", "AUS", "thu0731"))
                {

                    Microsoft.Win32.TaskScheduler.Task task = tasksrvc.FindTask(_taskName);
                    TimeTrigger trigger = (TimeTrigger)task.Definition.Triggers.FirstOrDefault(x => x.TriggerType == TaskTriggerType.Time);
                    if (trigger == null)
                    {
                        trigger = new TimeTrigger(DateTime.Now.AddMinutes(delaytime));
                        task.Definition.Triggers.Add(trigger);
                        tasksrvc.RootFolder.RegisterTaskDefinition(_taskName, task.Definition, TaskCreation.CreateOrUpdate, tasksrvc.UserName);
                    }
                    else
                    {
                        if (trigger.StartBoundary < DateTime.Now)
                        {
                            trigger.StartBoundary = DateTime.Now.AddMinutes(delaytime);
                            tasksrvc.RootFolder.RegisterTaskDefinition(_taskName, task.Definition, TaskCreation.CreateOrUpdate, tasksrvc.UserName);
                        }
                    }

                    //task.Run();
                }

            }
            catch (Exception ex)
            {

                eStore.Utilities.eStoreLoger.Error("SyncFilesToBackupServer Error", "", "", "", ex);
            }
          
            return null;
        }
    }
}
