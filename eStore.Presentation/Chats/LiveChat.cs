using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eStore.POCOS;

namespace eStore.Presentation.Chats
{
    public class LiveChat: IChat
    {
        public string GetStoreChats(Store store)
        {
            return string.Format("<a id=\"tophl_LiveChat\" href=\"{0}ContactUs.aspx\">&nbsp; </a>", store.StoreURL);
        }
    }
}
