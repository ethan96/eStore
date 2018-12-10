using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;
using eStore.Utilities;



namespace eStore.POCOS.DAL
{
    public partial class WidgetHelper : Helper
    {
        public int save(Widget widget)
        {
            return -5000;
        }

        public int delete(Widget widget)
        {
            try
            {
                if (widget == null || widget.validate() == false)
                {
                    widget.error_message.Add(new PocoX.ErrorMessage("Error", "Widget is null."));
                    return 1; 
                }
                else
                {
                    Widget _widget = context.Widgets.FirstOrDefault(c => c.WidgetID == widget.WidgetID);
                    if (_widget != null)
                    {
                        context.Widgets.DeleteObject(_widget);
                        context.SaveChanges();
                        return 0;
                    }
                    else
                    {
                        widget.error_message.Add(new PocoX.ErrorMessage("Error", "Widget is null."));
                        return 1;
                    }
                    //context = widget.WidgetPage.helper.context;
                    //context.DeleteObject(widget);
                    //context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                eStoreLoger.Fatal(ex.Message, "", "", "", ex);
                widget.error_message.Add(new PocoX.ErrorMessage("Error",ex.Message));
                return -5000;
            }
        }


    }
}
