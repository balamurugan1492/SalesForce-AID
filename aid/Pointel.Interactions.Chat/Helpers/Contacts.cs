
namespace Pointel.Interactions.Chat.Helpers
{
    public class Contacts : IContacts 
    {
        public Contacts(string name, string number, string type)
        {
            Name = name;
            Number = number;
            Type = type;
        }
        public string Name { get; set; }
        public string Number { get; set; }
        public string Type { get; set; }  
    }
}
