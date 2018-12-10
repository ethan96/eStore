using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS.DAL;

 namespace eStore.POCOS{

    public partial class CampaignMailLog
    {
        private bool isApplied = false;
        public bool IsApplied
        {
            get { return isApplied; }
            set { isApplied = value; }
        }
        
	} 
 }