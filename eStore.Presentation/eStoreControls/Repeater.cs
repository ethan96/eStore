using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace eStore.Presentation.eStoreControls
{
    public class Repeater :System.Web.UI.WebControls.Repeater 
    {
        private ITemplate emptyDataTemplate;

        [PersistenceMode(PersistenceMode.InnerProperty), TemplateContainer(typeof(TemplateControl))]
        public ITemplate EmptyDataTemplate
        {
            get { return emptyDataTemplate; }
            set { emptyDataTemplate = value; }
        }
        protected override void OnDataBinding(EventArgs e)
        {

            base.OnDataBinding(e);
            if (emptyDataTemplate != null)
            {
                if (this.Items.Count == 0)
                {
                    EmptyDataTemplate.InstantiateIn(this);
               
                }
            }
        }
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (this.Items.Count == 0)
                this.Visible = false;
            else
                this.Visible = true;
        }

        public string DataID
        {
            get;
            set;
        }
    }
}
