namespace eStore.Presentation.Chats
{
    public class SAPChats: IChat
    {
        public string GetStoreChats(POCOS.Store store)
        {
            return "<img src='/images/SAP/livechat/chats.png' />" +
                "<div>" +
                "<div class='mobile_pop' id='a_mobile_pop'>" +
                "<img src='/images/SAP/livechat/singapore.jpg' />" +
                "</div>" +
                "<div class='clearfix'></div>" +
                "</div>";
        }
    }
}
