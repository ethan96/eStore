using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace eStore.BusinessModules.Task
{
    public enum TaskStatus
    {
        Init,
        Ready,
        Processing,
        Completed,
        Failed
    }

    [Serializable]
    public   class TaskBase : ITask, IDisposable
    {
        private bool _isDisposed;
        public bool _isSaved { get; set; }

        public string ID { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public int AttemptTimes { get; set; }
        public TaskStatus Status { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// Once this property is set to true, this task will be re-run once again. After arranging the task re-run, task manager will
        /// reset this flag to false to prevent surprisingly continuous re-run.  If task need to be re-run, task should turn on the re-run flag
        /// after the task is executed.
        /// </summary>
        public bool NeedRerun { get; set; }
        /// <summary>
        /// If the property has value and the re-run is turned on, task manager will sustain the re-run for specified duration before it re-execute
        /// this task.
        /// </summary>
        public int DeferDuration { get; set; }

        public TaskBase()
        {
            ID = Guid.NewGuid().ToString();
            Name = this.GetType().FullName;
            StartAt = DateTime.Now;
            EndAt = DateTime.Now;
            Status = TaskStatus.Init;
            _isDisposed = false;
            _isSaved = false;
        }

        public override string ToString()
        {
            return string.Format("ID={0}&Name={1}&StartAt={2}&EndAt={3}&AttemptTimes={4}&Status={5}", ID, Name, StartAt, EndAt, AttemptTimes, Status);
        }

        public virtual bool PreProcess()
        {
            this.Status = TaskStatus.Ready;
            return true;
        }

        public virtual void OnCompleted()
        {
            this.Status = TaskStatus.Completed;
            this.EndAt = DateTime.Now;
        }

        public virtual void OnFailed()
        {
            this.Status = TaskStatus.Failed;
            this.EndAt = DateTime.Now;
            this.AttemptTimes++;
        }

        /// <summary>
        /// This property will be used by EventManager to carry event instance for reference
        /// </summary>
        public Object eventInfo { get; set; }

        public void Dispose()
        {
            if (!_isDisposed)
                _isDisposed = true;
        }

        public virtual object execute(object obj)
        {
            return null;
        }

        private string testing = System.Configuration.ConfigurationManager.AppSettings.Get("TestingMode");
        protected bool testMode()
        {
            if (String.IsNullOrEmpty(testing) || testing.ToLower() == "true")
                return true;
            else
                return false;
        }
    }
}
