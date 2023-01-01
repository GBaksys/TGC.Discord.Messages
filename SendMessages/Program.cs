using System.Configuration;
using System.Globalization;
using System.Reflection;
using log4net;
using log4net.Config;
using SendMessages.Persistence;
using SendMessages.Domain;

namespace SendMessages
{
    internal class Program
    {
        private const string DateTimeFormat = "yyyy-MM-dd\\TH:mm:ss.fff";

        private static readonly ILog _log = LogManager.GetLogger(typeof(Program));

        private static readonly string[] _guildChannelNumbers = (ConfigurationManager.AppSettings["guildchatnumbers"] ?? "")
            .Split(',', StringSplitOptions.TrimEntries & StringSplitOptions.RemoveEmptyEntries);

        private static readonly string[] _officerChannelNumbers = (ConfigurationManager.AppSettings["officerchatnumbers"] ?? "")
            .Split(',', StringSplitOptions.TrimEntries & StringSplitOptions.RemoveEmptyEntries);

        private static readonly MessageService _messageService = new MessageService(new ChatRepository());

        private  static DiscordClient _discordClient = new DiscordClient(ConfigurationManager.AppSettings["guildchatterurl"],
                    ConfigurationManager.AppSettings["officerchatterUrl"]);

        static void Main(string[] args)
        {
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4netconfig.config"));

            try
            {
                var tempFile = ConfigurationManager.AppSettings["tempfile"];

                var logFile = ConfigurationManager.AppSettings["esochatlog"];

                if (string.IsNullOrWhiteSpace(logFile))
                    throw new ArgumentException(nameof(logFile));

                _log.Info($"{DateTime.Now} Copying {logFile} and creating {tempFile}.");

                // we have to copy the file because ESO has 
                // a handle on the ChatLog.log file it creates
                // and we can't even read the contents without getting 
                // file access exception
                File.Copy(logFile, tempFile, true);

                _log.Info($"{DateTime.Now} Processing {tempFile}");

                ProcessTempFile(tempFile);

                _log.Info($"{DateTime.Now} App Finished.");
            }
            catch (Exception ex) 
            { 
                _log.Error(ex.ToString());
            }
        }

        static private void ProcessTempFile(string filePath)
        {
            ILog log = LogManager.GetLogger(typeof(Program));

            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException(nameof(filePath));

            // since we reading in the entire chat log every time this is run
            // it will contain messages we have already processed, so
            // we capture the timestamp of the last message read on the 
            // prior run on this app.   This allows us to only process
            // those messages with a new time stamp.
            var lastReadTimeStamp = ConfigurationManager.AppSettings["messagelastreaddatetime"] ?? "";

            var lastReadTime = string.IsNullOrWhiteSpace(lastReadTimeStamp)
                ? null
                : ConvertTimeStampToDateTime(lastReadTimeStamp);

            if (File.Exists(filePath)) 
            {
                var sentCounter = 0;
                var readCounter = 0;
                var totalLinesCounter = 0;
                var messageTimeStamp = "";
                var lines = System.IO.File.ReadLines(filePath);

                foreach (string line in lines)
                {
                    // if it's not long enough to have a time stamp,
                    // we can skip the chat message
                    if (line.Length < 24) 
                        continue;

                    totalLinesCounter++;

                    var messageTime = ConvertTimeStampToDateTime(line.Substring(0, 23));

                    // some of the messages in the chat log don't begin 
                    // with a timestamp, luckily they are not guild chat messages,
                    // so we can skip them.
                    if (messageTime == null)
                        continue;
                    else
                        messageTimeStamp = line.Substring(0, 23);
                    
                    readCounter++;

                    // if this app is running for the first time and we don't have a last read time
                    // or the message timestamp is newer than the last read time 
                    // process it 
                    if (lastReadTime == null || messageTime > lastReadTime)
                    {
                        var chatNumber = line.Substring(30, 2);

                        // post guild chats to Discord
                        if (_guildChannelNumbers.Contains(chatNumber))
                        {
                            _discordClient
                                .PostGuildChat(new DiscordMessage($"{messageTime} ESO G{int.Parse(chatNumber)-11} - {line.Substring(33)}"))
                                .Wait();
                            _messageService
                                .SaveChatAsync(ConvertToMessage(line))
                                .Wait();
                            sentCounter++;
                        }

                        // post officer chats to Discord
                        if (_officerChannelNumbers.Contains(chatNumber))
                        {
                            _discordClient
                                .PostOfficerChat(new DiscordMessage($"{messageTime} ESO O{int.Parse(chatNumber)-16} - {line.Substring(33)}"))
                                .Wait();
                            _messageService
                                .SaveChatAsync(ConvertToMessage(line))
                                .Wait();
                            sentCounter++;
                        }
                    }
                }

                // save the message time stamp of the last message read in from 
                // the log.
                if (!string.IsNullOrWhiteSpace(messageTimeStamp))
                {
                    var config = ConfigurationManager
                        .OpenExeConfiguration(System.Reflection.Assembly.GetExecutingAssembly().Location);
                    config.AppSettings.Settings["MessageLastReadDateTime"].Value = messageTimeStamp;
                    config.Save();
                }

                log.Info($"{sentCounter} of {readCounter} Guild chats posted to TGC #in-game-chatter.");
            }
        }

        // returns a datetime or null if timeStamp
        // can't be converted
        static private DateTime? ConvertTimeStampToDateTime(string timeStamp)
        {
            if (DateTime.TryParseExact(timeStamp,
                DateTimeFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AssumeLocal,
                out var dateTime))
                return dateTime;
            else 
                return null;
        }

        static private Message ConvertToMessage(string line)
        {
            return new Message()
            {
                TimeStamp = ConvertTimeStampToDateTime(line.Substring(0, 23)).Value,
                UserId = line.Substring(33, line.IndexOf(",",34) - 33),
                Text = line.Substring(line.IndexOf(",",34) + 1),
                Channel = line.Substring(30, 2)
            };
        }
    }
}