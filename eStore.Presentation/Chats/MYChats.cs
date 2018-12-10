namespace eStore.Presentation.Chats
{
    public class MYChats : IChat
    {
        public string GetStoreChats(POCOS.Store store)
        {
            return "<img src='/images/SAP/livechat/mychats.png' />" +
                "<div>" +
                "<div class='mobile_pop' id='a_mobile_pop'>" +
                "<img src='/images/SAP/livechat/malaysia.jpg' />" +
                "<a target='_blank' href='https://web.whatsapp.com/'>(Click to scan QR Code)</a>" +
                "</div>" +
                "<div class='clearfix'></div>" +
                "</div>";
        }
    }
}
