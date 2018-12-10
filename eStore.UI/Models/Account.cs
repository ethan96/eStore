using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eStore.UI.Models
{
    public class Account
    {
        public Account()
        {

        }

        public Account(List<POCOS.Order> orders)
        {
            this.Orders = (from order in orders select new Models.Order(order)).ToList();
        }

        public Account(List<POCOS.Quotation> quotes, List<POCOS.Quotation> tQuotes)
        {
            this.Quotes = quotes != null ? (from quote in quotes select new Models.Quote(quote)).ToList() : null;
            this.tQuotes = tQuotes != null ? (from quote in tQuotes select new Models.Quote(quote)).ToList() : null;
        }

        public List<Models.Order> Orders { get; set; }
        public List<Models.Quote> Quotes { get; set; }
        public List<Models.Quote> tQuotes { get; set; }

        private string welcomename = string.Empty;
        public string WelcomeName 
        { 
            get
            {
                if (string.IsNullOrEmpty(this.welcomename))
                {
                    if (eStore.Presentation.eStoreContext.Current.User != null)
                        this.welcomename = string.Format("Hi! {0}", eStore.Presentation.eStoreContext.Current.User.FirstName);
                    else
                        this.welcomename = "Hi!";
                }
                return this.welcomename;
            }
            set
            {
                this.welcomename = value;
            }
        }

    }
}