using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Timers;

namespace eStore.BusinessModules.Task
{
    public class TaskScheduler
    {
        private Timer myTimer;

        private List<ScheduledTask> _scheduledTasks;
        private List<ScheduledTask> ScheduledTasks
        {
            get
            {
                if (_scheduledTasks == null)
                    Loadtasks();
                return _scheduledTasks;
            }
            set
            {
                _scheduledTasks = value;
            }
        }
        private static TaskScheduler scheduler;

        public static TaskScheduler getInstance()
        {
            if (scheduler == null)
                scheduler = new TaskScheduler();
            return scheduler;

        }
        private TaskScheduler()
        {

        }
        public void Start()
        {
            CreateTimer();
        }

        private void Loadtasks()
        {
            Assembly itaskass = Assembly.GetAssembly(typeof(ITask));
            _scheduledTasks = new List<ScheduledTask>();
            foreach (var type in itaskass.GetTypes().Where(t => t != typeof(ITask) && typeof(ITask).IsAssignableFrom(t)))
            {
                ScheduledTask t = new ScheduledTask();
                t.interval = ScheduledTask.Interval.Minute;
                t.increment = 1;
                t.lastRunat = DateTime.MinValue;
                t.assemblyfilepath = itaskass.Location;
                t.typeName = type.FullName;
                t.LoadTask();
                ScheduledTasks.Add(t);
            }



        }

        private void CreateTimer()
        {
            myTimer = new Timer();
            myTimer.Interval = 1 * 10 * 1000; //Check every 1 minutes
            myTimer.Enabled = true;
            myTimer.Elapsed += new ElapsedEventHandler(this.myTimer_Elapsed);
            myTimer.Start();
        }


        private static DateTime _lastRunTime = DateTime.MinValue;
        public void myTimer_Elapsed(System.Object sender, ElapsedEventArgs e)
        {
            _lastRunTime = DateTime.Now;
            foreach (ScheduledTask task in ScheduledTasks.Where(x => x.nextRunAt < _lastRunTime))
            {
                //Use EventManager here
                //ThreadPoolManager.getInstance().AddNewTask(task.task);
                task.lastRunat = _lastRunTime;

                switch (task.interval)
                {
                    case ScheduledTask.Interval.OnceOnly:
                        task.nextRunAt = DateTime.MaxValue;
                        break;
                    case ScheduledTask.Interval.Day:
                        task.nextRunAt = _lastRunTime.AddDays(task.increment);
                        break;
                    case ScheduledTask.Interval.Hour:
                        task.nextRunAt = _lastRunTime.AddDays(task.increment);
                        break;
                    case ScheduledTask.Interval.Minute:
                    default:
                        task.nextRunAt = _lastRunTime.AddDays(task.increment);
                        break;
                }
                //save ScheduledTask
            }
        }
    }

    public class ScheduledTask
    {
        public enum Interval
        {
            OnceOnly, Minute, Hour,
            Day
        }
        public bool SkipWeekend { get; set; }

        public Interval interval { get; set; }
        public int increment { get; set; }
        public DateTime startAt { get; set; }
        public DateTime lastRunat { get; set; }
        public DateTime nextRunAt { get; set; }
        public int attemptTimeIfFailed { get; set; }

        public ITask task { get; set; }

        public string assemblyfilepath;
        public string typeName;
        public void LoadTask(string path, string name)
        {
            assemblyfilepath = path;
            typeName = name;
            LoadTask();

        }
        public void LoadTask()
        {
            Assembly assembly = Assembly.LoadFile(assemblyfilepath);
            //task = (ITask)assembly.CreateInstance(typeName);
        }
    }
}
