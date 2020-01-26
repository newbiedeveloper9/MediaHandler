using fbchat_sharp.API;

namespace MediaHandler.PubSub
{
    public struct NewMessageStruct
    {
        public string ThreadId { get; set; }
        public FB_Message Message { get; set; }

        public NewMessageStruct(string threadId, FB_Message message)
        {
            ThreadId = threadId;
            Message = message;
        }
    }
}