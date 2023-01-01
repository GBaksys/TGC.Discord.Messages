using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendMessages
{
    public class DiscordMessage
    {
        public DiscordMessage(string content) 
        {
            Content = content;
        }
        public string Content { get; set; }
    }
}
