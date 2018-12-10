using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules.SolutionStoreTemplate
{
    public partial class ProductAutomation_Content_AUS : eStore.Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Presentation.eStoreContext.Current.keywords.Add("Keywords", "Production Automation");
            base.setPageMeta("Production Automation", "Advantech offers complete embedded solutions for Production Automation applications, from software to PCI cards, ISA cards, PC/104 modules, PCI-104 modules and USB data acquisition products. ActiveDAQ Pro software combines OCX components with a friendly graphic user interface (GUI) and allows users to create unique operation interfaces in a short amount of time. The new high performance USB daq modules and PCI-104 modules help users build flexible and cost-effective test and measurement fixtures for various manufacturing industries, such as cell phones, LEDs, ICs, touchscreens, and stress testing applications for consumer products.", "Production Automation");

        }
    }
}