using System;
using eStore.Utilities;


namespace eStore.POCOS.DAL
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class WidgetRequestHelper : Helper
    {

        internal int save(WidgetRequest widgetRequest)
        {
            //if parameter is null or validation is false, then return  -1 
            if (widgetRequest == null || widgetRequest.validate() == false) return 1;
            try
            {
                context.WidgetRequests.AddObject(widgetRequest);
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
