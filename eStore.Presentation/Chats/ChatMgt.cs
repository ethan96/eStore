namespace eStore.Presentation.Chats
{
    public class ChatMgt
    {
        public static IChat GetChat(string assem)
        {
            return ((Chats.IChat)System.Reflection.Assembly.GetExecutingAssembly()
                        .CreateInstance("eStore.Presentation.Chats." + assem));
        }
    }
}
