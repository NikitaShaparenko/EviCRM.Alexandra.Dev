using EviCRM.Alexandra.Core;
using EviCRM.Backend4.Core;
using EviCRM.Backend4.Models;
using EviCRM.Backend4.Telegram;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types.Enums;

namespace EviCRM.Backend4
{
    public class Program
    {
        const bool IS_DEBUG = false;
        const string HTTPS_URL = "localhost";
        const int HTTPS_PORT = 9254;

        const string ALEXA_VERSION = "1.01";
        const string ALEXA_BUILD_DATE = "05.08.2022";

        public const string ALEXA_AUTH_KEY_ID = "#$AUTH_ID";
        public const string ALEXA_AUTH_KEY_ID_SECRET = "#$AUTH_KEY";

        public static string? EviCRM_WebRootEnvironment { get; set; }

        public static MySQL mysql_C = new MySQL();

        public static List<string> telegram_chatID_lst = new List<string>();
        public static List<string> telegram_login_lst = new List<string>();
        public static List<AlexandraCore.AlexandraLoginStatus> telegram_login_status = new List<AlexandraCore.AlexandraLoginStatus>();
        public static List<LogicDrive.AlexandraPages> telegram_page = new List<LogicDrive.AlexandraPages>();

        public static List<string> telegram_queue_chatID_lst = new List<string>();
        public static List<string> telegram_queue_message_lst = new List<string>();

        public static AlexandraCore alexandra_core = new AlexandraCore();
        public static AlexandraCronList alexandra_cron_list = new AlexandraCronList();

        public static httpServer alexandra_httpServer = new httpServer();
        public static httpClient alexandra_httpClient = new httpClient();

        public static TelegramSenderHandler alexandra_tsh = new TelegramSenderHandler();

        public static AlexandraLog alexandra_log = new AlexandraLog(IS_DEBUG);

        public static LogicDrive logic_drive = new LogicDrive();

        public static AlexandraYandexAPI alexandra_yandex_api = new AlexandraYandexAPI();

        public static AlexandraFileService alexandra_file_service = new AlexandraFileService();

        public static List<CronTask> cron_list = new List<CronTask>();

        static async Task Main(string[] args)
        {
            alexandra_log.Log("=====================================================");
            alexandra_log.Log("EviCRM Backend (Alexandra Core v" + ALEXA_VERSION + " , build: " + ALEXA_BUILD_DATE);
            alexandra_log.Log("Hosted on: " + HTTPS_URL + ":" + HTTPS_PORT.ToString());
            alexandra_log.Log("=====================================================");

            #region HTTP Web Server Module
            Task httpWebServer = new Task(async() => await alexandra_httpServer.http_handler(HTTPS_URL, HTTPS_PORT.ToString()));
            httpWebServer.Start();
            alexandra_log.Log("Alexandra HTTP Web Server", "Started!", Core.AlexandraLog.LogStatus.Debug);
            #endregion

            #region HTTP Web Client Module

            HttpClient client = new HttpClient();
            Program.alexandra_httpClient.Init_HttpClient(client);
            alexandra_log.Log("Alexandra HTTP Web Client", "Initialized!", Core.AlexandraLog.LogStatus.Debug);
            #endregion

            #region Telegram Bot Server Module
            var bot = new TelegramBotClient(EviCRM.Backend4.Telegram.Configuration.BotToken);
            alexandra_cron_list.Init_CronBotClientSet(bot);

            Task telegramQueueHandler = new Task(async() => await alexandra_tsh.QueueHandler(bot));
            telegramQueueHandler.Start();
            alexandra_log.Log("Alexandra Telegram Queue Handler", "Started!", Core.AlexandraLog.LogStatus.Debug);

            var me = await bot.GetMeAsync();
            Console.Title = "Alexandra Core v" + ALEXA_VERSION + " , build: " + ALEXA_BUILD_DATE + " (EviCRM Backend)";
            alexandra_log.Log("Alexandra Core", "Telegram Bot " + me.Username + " initialized!", Core.AlexandraLog.LogStatus.Debug);

            #endregion

            await Program.alexandra_core.LoadUserRecords(); //Загрузка данных о пользователях


            using var cts = new CancellationTokenSource();

            // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
            bot.StartReceiving(updateHandler: EviCRM.Backend4.Telegram.Handlers.HandleUpdateAsync,
                               errorHandler: EviCRM.Backend4.Telegram.Handlers.HandleErrorAsync,
                               receiverOptions: new ReceiverOptions()
                               {
                                   AllowedUpdates = Array.Empty<UpdateType>()
                               },
                               cancellationToken: cts.Token);

            alexandra_log.Log("Alexandra Core", "Telegram Bot started completly for " + me.Username + "!", Core.AlexandraLog.LogStatus.Info);

            //Yandex Auth
            AuthModel yandex_api_auth = await Program.alexandra_yandex_api.YandexAuth_IAM();

            alexandra_log.Log("Yandex API", "Authorization complete: [Expire At: " + yandex_api_auth.ExpireAt + " ; IAM Key: " + yandex_api_auth.IAM + " ]",AlexandraLog.LogStatus.Info);

            //alexandra_log.Log("Yandex API STT", "STT Text Response: " + await alexandra_yandex_api.Send_STT_API_KEY(@"C:\Users\Public\ogg.ogg", ALEXA_AUTH_KEY_ID_SECRET), AlexandraLog.LogStatus.Info);

            //

            Console.ReadLine();

            // Send cancellation request to stop bot
            cts.Cancel();
        }
    }
}

