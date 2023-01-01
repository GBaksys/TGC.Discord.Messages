using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMessages.Abstracts
{
    public interface IChatsRepository<TMessage> 
    {
        public Task SaveChatAsync(TMessage message);
    }
}
