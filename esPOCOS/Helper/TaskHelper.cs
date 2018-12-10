using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;
namespace eStore.POCOS.DAL
{

    public partial class TaskHelper : Helper
    {

        #region Business Read

        ~TaskHelper()
        {
            if (context != null)
                context.Dispose();
        }

        public Task getTask(string id)
        {
            return context.Tasks.FirstOrDefault(x => x.ID == id);
        }

        /// <summary>
        /// This method is to retrieve not-finished task due to system shutdown and unexpected issue.
        /// Usually this method will only be invoked at the very beginning when eStore application 
        /// resume from previous interruption.
        /// </summary>
        /// <returns></returns>
        public List<Task> getPendingTasks(String storeID)
        {
            var results = (from task in context.Tasks
                         where task.StoreID == storeID && task.AttemptTimes < 5
                         select task);

            List<Task> tasks = (results == null) ? new List<Task>() : results.ToList<Task>();
            foreach (Task task in tasks)
                task.helper = this;

            return tasks;
        }

        #endregion

        #region Creat Update Delete
        public int save(Task _task)
        {

            //if parameter is null or validation is false, then return  -1 
            if (_task == null || _task.validate() == false) return 1;
            //Try to retrieve object from DB
            Task _exist_task = getTask(_task.ID);
            try
            {
                if (_exist_task == null)  //object not exist 
                {
                    //Insert
                    context.Tasks.AddObject(_task);
                    context.SaveChanges();
                    return 0;
                }
                else
                {
                    //Update
                    context.Tasks.ApplyCurrentValues(_task);
                    context.SaveChanges();
                    return 0;
                }
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
        }

        public int delete(Task _task)
        {


            if (_task == null || _task.validate() == false) return 1;
            try
            {
                context.DeleteObject(_task);
                context.SaveChanges();
                return 0;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
        }
        #endregion

        #region Others

        private static string myclassname()
        {
            return typeof(TaskHelper).ToString();
        }
        #endregion
    }
}