using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Objects;
using System.Data.Common;

namespace eStore.POCOS.DAL
{
    public class Helper
    {
        protected eStore3Entities6 context;

        //default constructor
        public Helper()
        {
            context = new eStore3Entities6();
        }

        //default destructor
        /*
        ~Helper()
        {
            if (context != null)
                context.Dispose();
        }
         * */

        public eStore3Entities6 getContext()
        {
            //Make sure ObjectContext is not disposed and connection is still good to be used.
            try
            {
                if (context.Connection != null)
                    return context;
                else
                    return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool setContext(eStore3Entities6 newContext)
        {
            bool bSuccess = false;
            try
            {
                if (newContext.Connection != null)
                {
                    context = newContext;
                    bSuccess = true;
                }
            }
            catch (Exception)
            {
            }

            return bSuccess;
        }

        /// <summary>
        /// //print out context
        /// </summary>
        /// <param name="job"></param>
        /// <param name="context"></param>
        public static void printObjectState(string job,eStore3Entities6 context)
        {
            var objectEntries = context.ObjectStateManager.GetObjectStateEntries(System.Data.EntityState.Modified);

            Console.WriteLine( "************ " + job + "  ************");
            foreach (ObjectStateEntry a in objectEntries)
            {
               if (a.Entity !=null) 
               {
                   Console.Write(a.EntitySet.Name + ":"  );
               }
            }
            var objectEntries2 = context.ObjectStateManager.GetObjectStateEntries(System.Data.EntityState.Unchanged);

            Console.WriteLine("************ " + job + "  ************");
            foreach (ObjectStateEntry a in objectEntries2)
            {
                if (a.Entity != null)
                {
                    Console.Write(a.EntitySet.Name + ":");
                }
            }
      
        }

    }


   


}
