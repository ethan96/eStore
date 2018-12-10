using eStore.BusinessModules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules.KitTheme
{
    public partial class List : BaseTheme
    {
        private bool _IsFelter;

        public bool IsFelter
        {
            get { return _IsFelter; }
            set { _IsFelter = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (CmsModels != null)
            {   
                cmslist.DataSource = CmsModels;
                cmslist.DataBind();
            }
        }

        public string ShowImage(object obj)
        {
            if (obj != null && !string.IsNullOrEmpty(obj.ToString()))
                return string.Format("<div class=\"leftImg\"><img src=\"{0}\" class=\"img fullimg\" /></div>", obj.ToString());
            return "";
        }
    }
}