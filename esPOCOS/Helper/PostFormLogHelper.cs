// -----------------------------------------------------------------------
// <copyright file="PostFormLogHelper.cs" company="Adv">
// add PostFormLogHelper by andy
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.Utilities;

namespace eStore.POCOS.DAL
{
    /// <summary>
    /// PostFormLogHelper
    /// </summary>
    public class PostFormLogHelper : Helper
    {

        internal int save(PostFormLog postFormLog)
        {
            try
            {
                context.PostFormLogs.AddObject(postFormLog);
                context.SaveChanges();
                return 0;
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                return -5000;
            }
        }
    }
}
