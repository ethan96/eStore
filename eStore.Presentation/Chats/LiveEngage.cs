using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eStore.Presentation.Chats
{
    public class LiveEngage : IChat
    {
        public string GetStoreChats(POCOS.Store store)
        {
            return "<div id=\"LP_DIV_eStore_Top\"></div>";
        }
    }
}
