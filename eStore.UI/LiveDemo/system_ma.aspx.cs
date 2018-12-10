using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.LiveDemo
{
    public partial class system_ma : Presentation.eStoreBaseControls.eStoreBasePage 
    {
        public string liveDemoStoreId = ""; // 前台并未调用过
        public bool showMapLinks = true;
        protected void Page_Load(object sender, EventArgs e)
        {
            string storeID = "AUS"; // Get store ID
            switch (storeID)
            {
                case "AJP":
                    liveDemoStoreId = "/AJP";
                    break;
                case "AAC":
                    liveDemoStoreId = "";
                    break;
                case "ATW":
                    liveDemoStoreId = "/ATW";
                    break;
                case "ADL":
                    liveDemoStoreId = "";
                    break;
                case "AIT":
                    liveDemoStoreId = "";
                    break;
                case "AKR":
                    liveDemoStoreId = "/AKR";
                    showMapLinks = false;
                    break;
                case "ACN":
                    liveDemoStoreId = "/ACN";
                    break;
                case "AAU":
                    liveDemoStoreId = "";
                    break;
                case "ABR":
                    liveDemoStoreId = "";
                    break;
                case "ELMARK":
                    liveDemoStoreId = "";
                    break;
            }
        }
    }
}