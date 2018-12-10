using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eStore.POCOS;

namespace eStore.UI.Modules.Category
{
    public partial class CategoryRepeater : Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        private List<POCOS.ProductCategory> _dataSource;
        public List<POCOS.ProductCategory> DataSource 
        {
            get 
            { 
                return _dataSource ?? new List<POCOS.ProductCategory>(); 
            }
            set { _dataSource = value; }
        }
        
        
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public override void  DataBind()
        {
            foreach (var c in DataSource)
            {
                VCategoryBase cb = new VCategoryBase() { productCategory = c };
                this.Controls.Add(cb.LoadCategory());
            }
        }


    }
}