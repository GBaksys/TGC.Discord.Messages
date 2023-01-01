using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SendMessages.Abstracts;

namespace SendMessages.Domain
{
    public class MessageService : IMessageService<Message>
    {
        private readonly IChatsRepository<Message> _chatsRepository;

        public MessageService(IChatsRepository<Message> chatsRepository) 
        { 
            _chatsRepository = chatsRepository 
                ?? throw new ArgumentNullException(nameof(chatsRepository));
        }

        public async Task SaveChatAsync(Message message)
        {
           await _chatsRepository.SaveChatAsync(message);
        }
    }
}
