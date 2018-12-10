using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace eStore.UI.Modules
{
    public partial class ServerTime : eStore.Presentation.eStoreBaseControls.eStoreBaseUserControl
    {
        private DateTime? _startDate = null;
        private string _timeId = "tm_";
        public string TimeId
        {
            get { return _timeId; }
            set { _timeId = value; }
        }

        public string StartDate
        {
            get 
            {
                if (_startDate == null)
                    initIn();
                return _startDate.Value.ToString("HH:mm"); 
            }
        }

        private DateTime? _endDate = null;

        public string EndDate
        {
            get 
            {
                if (_endDate == null)
                    initIn();
                return _endDate.Value.ToString("HH:mm"); 
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {

        }

        private void initIn()
        {
            string startDate = Request.Form[TimeId+"ContactHourStart"];
            string endDate = Request.Form[TimeId + "ContactHourEnd"];
            DateTime d1, d2;
            DateTime.TryParse(startDate, out d1);
            DateTime.TryParse(endDate, out d2);
            if (d1 < d2)
            {
                _startDate = d1;
                _endDate = d2;
            }
            else
            {
                _startDate = d2;
                _endDate = d1;
            }
        }
    }
}