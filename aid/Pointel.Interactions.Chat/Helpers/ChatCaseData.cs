
namespace Pointel.Interactions.Chat.Helpers
{
    public class ChatCaseData : IChatCaseData 
    {
        public ChatCaseData(string key, string value)
        {
            Key = key;
            Value = value;           
        }

        public string Key { get; set; }

        public string Value { get; set; }
    }
}
