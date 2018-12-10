using System;
using System.Collections.Generic;

namespace eStore.BusinessModules.Task
{
    public interface ITask
    {
        bool PreProcess();
        object execute(object obj);
        void OnCompleted();
        void OnFailed();
    }



}
