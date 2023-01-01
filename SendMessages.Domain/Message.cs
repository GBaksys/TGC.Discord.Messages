namespace SendMessages.Domain
{
    public class Message
    {
        public DateTime TimeStamp { get; set; }
        public string UserId { get; set; }
        public string Text { get; set; }
        public string Channel { get; set; }
    }
}