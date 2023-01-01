namespace SendMessages.Abstracts
{
    public interface IMessageService<TMessage>
    {
        public Task SaveChatAsync(TMessage message);
    }
}