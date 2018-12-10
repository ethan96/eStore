using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace eStore.Presentation.eStoreControls
{
    public class TextBox :System.Web.UI.WebControls.TextBox
    {
        public override string Text
        {
            get
            {
                return base.Text.Trim();
            }
            set
            {
                base.Text = value;
            }
        }
    }
}
