using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace SendMessages
{
    internal class DiscordClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _inGameChatterUrl;
        private readonly string _inOfficerChatterUrl;

        public DiscordClient(string guildChatterUrl, string officerChatUrl)
        {
            if (string.IsNullOrWhiteSpace(guildChatterUrl))
                throw new ArgumentException(nameof(guildChatterUrl));

            if (string.IsNullOrWhiteSpace(officerChatUrl))
                throw new ArgumentException(nameof(officerChatUrl));

            _inGameChatterUrl = guildChatterUrl;

            _inOfficerChatterUrl = officerChatUrl;

            _httpClient = new HttpClient();
        }

        public async Task PostGuildChat(DiscordMessage message)
        {
            if (string.IsNullOrWhiteSpace(message?.Content))
                throw new ArgumentException(nameof(message));

            await _httpClient.PostAsJsonAsync<DiscordMessage>(_inGameChatterUrl, message);
        }

        public async Task PostOfficerChat(DiscordMessage message)
        {
            if (string.IsNullOrWhiteSpace(message?.Content))
                throw new ArgumentException(nameof(message));

            await _httpClient.PostAsJsonAsync<DiscordMessage>(_inOfficerChatterUrl, message);
        }
    }
}
