using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eStore.BusinessModules.Task
{
    public class DeferAgent : TaskBase
    {
        public TaskBase DeferredTask { get; set; }

        public DeferAgent(TaskBase eventPublisher, int deferDuration = 60000 /*default 60 seconds*/)
        {
            DeferredTask = eventPublisher;
            DeferDuration = deferDuration;
        }

        #region Task_Execution method
        /// <summary>
        /// This method will be executed to publish event
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override object execute(object obj)
        {
            System.Threading.Thread.Sleep(DeferDuration);   //sleep
            OnCompleted();
            return null;
        }
        #endregion //Task_Execution method
    }
}
