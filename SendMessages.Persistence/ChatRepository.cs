using SendMessages.Domain;
using SendMessages.Abstracts;
using System.Configuration;
using System.Threading.Channels;
using System.Diagnostics;

namespace SendMessages.Persistence
{
    public class ChatRepository : IChatsRepository<Message>
    {
        private readonly string _connectionString;
        public ChatRepository() 
        {
            var connectionString = ConfigurationManager.ConnectionStrings["chats"].ConnectionString;
            
            if (string.IsNullOrWhiteSpace(connectionString))                
                throw new ArgumentException(nameof(connectionString));

            _connectionString = connectionString;
        }

        public async Task SaveChatAsync(Message message)
        {
            using (var chatsContext = new ChatsContext(_connectionString))
            {
                var esoChat = new EsoChat()
                {
                    TimeStamp = message.TimeStamp,
                    EsoUserId = message.UserId,
                    Text = message.Text,
                    Channel = message.Channel
                };

                chatsContext.EsoChats.Add(esoChat);
                try
                {
                    await chatsContext.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }
    }
}
