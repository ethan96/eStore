using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.POCOS;
using System.Data;
using eStore.Utilities;

namespace eStore.POCOS.DAL
{
    public partial class EquotationHelper
    {
        public Equotation.Quote GetQuotationMasterByQuoteID(string quoteID)
        {
            Equotation.ForeStore ws = new Equotation.ForeStore();
            ws.Timeout = 3000;
            Equotation.Quote quote = ws.GetQuotationMasterByQuoteID(quoteID.Trim());
            ws.Dispose();
            return quote;
        }

        public Equotation.Quote GetQuotationAllByQuoteID(string quoteID)
        {
            Equotation.ForeStore ws = new Equotation.ForeStore();
            ws.Timeout = 3000;
            Equotation.Quote quote = ws.GetQuotationTotalByQuoteID(quoteID.Trim());
            ws.Dispose();
            return quote;
        }

    }

}
