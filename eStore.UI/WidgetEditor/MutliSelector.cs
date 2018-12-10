using AjaxControlToolkit;
using System.ComponentModel;
using System.Web.UI;
using AjaxControlToolkit.HTMLEditor.ToolbarButton;

namespace eStore.UI.WidgetEditor.MutliSelector
{

       [RequiredScript(typeof(CommonToolkitScripts))]
    public class UserControlSelector : DesignModeSelectButton
    {

        public override string ScriptPath
        {
            get { return "~/Scripts/InsertIcon.js"; }
        }
    }
}