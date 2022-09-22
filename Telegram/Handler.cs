using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace EviCRM.Backend4.Telegram
{
    public class Handlers
    {
     

        //static List<string> telegram_chatID_lst = new List<string>();
        //static List<string> telegram_login_lst = new List<string>();
        public const string ALEXANDRA_CONTROL_NAME = "Alexandra Telegram Handler";


        public static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Program.alexandra_log.Log(ALEXANDRA_CONTROL_NAME + "Error has occured in HandleErrorAsync - Telegram Bot - " + ErrorMessage, Core.AlexandraLog.LogStatus.Error);
            return Task.CompletedTask;
        }

        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {

            var handler = update.Type switch
            {
                // UpdateType.Unknown:
                // UpdateType.ChannelPost:
                // UpdateType.EditedChannelPost:
                // UpdateType.ShippingQuery:
                // UpdateType.PreCheckoutQuery:
                UpdateType.Poll => BotOnPollMessageReceived(botClient, update.Poll!),
                UpdateType.Message => BotOnMessageReceived(botClient, update.Message!),
                UpdateType.EditedMessage => BotOnMessageReceived(botClient, update.EditedMessage!),
                UpdateType.CallbackQuery => BotOnCallbackQueryReceived(botClient, update.CallbackQuery!),
                UpdateType.InlineQuery => BotOnInlineQueryReceived(botClient, update.InlineQuery!),
                UpdateType.ChosenInlineResult => BotOnChosenInlineResultReceived(botClient, update.ChosenInlineResult!),
                _ => UnknownUpdateHandlerAsync(botClient, update)
            };

            try
            {
                await handler;

            }
            catch (Exception exception)
            {
                await HandleErrorAsync(botClient, exception, cancellationToken);
            }
        }

        private static async Task BotOnPollMessageReceived(ITelegramBotClient botClient, Poll poll)
        {

        }

        private static async Task BotOnMessageReceived(ITelegramBotClient botClient, Message message)
        {
            Program.alexandra_log.Log(ALEXANDRA_CONTROL_NAME, "Incoming message: Type = " + message.Type + " ; ChatID = " + message.Chat.Id + " ; Text = " + message.Text + ")", Core.AlexandraLog.LogStatus.Info);

            //Login Middleware Check
            Core.AlexandraCore.AlexandraLoginStatus login_status = Core.AlexandraCore.AlexandraLoginStatus.Unauthorized;
            switch (Program.alexandra_core.LoginMiddleware(message.Chat.Id.ToString()))
            {
                case Core.AlexandraCore.AlexandraLoginStatus.Banned:
                    await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                            text: "К сожалению, этот профиль забанен в EviCRM и я не могу с тобой общаться 😔 \nОбратись, пожалуйста, к @nikita4shap за восстановлением доступа");
                    return;
                    break;
                case Core.AlexandraCore.AlexandraLoginStatus.Unauthorized:
                    await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                            text: "К сожалению, я тебя не знаю. \nДавай познакомимся? 😌", replyMarkup: Program.logic_drive.km_auth_register());
                    return;
                    break;
                default:
                    break;

            }

            switch(message.Type)
            {
                case MessageType.Text:
                    var action = message.Text!.Split(' ')[0] switch
                    {
                        //"/inline" => SendInlineKeyboard(botClient, message),
                        //"/keyboard" => SendReplyKeyboard(botClient, message),
                        //"/remove" => RemoveKeyboard(botClient, message),
                        //"/photo" => SendFile(botClient, message),
                        "/request" => RequestContactAndLocation(botClient, message),
                        "/debug_chatID" => TelegramBot_Debug_ChatID(botClient, message),
                        "/debug_doyouknowme" => TelegramBot_Debug_DoYouKnowMe(botClient, message),
                        // "/debug_queue" => TelegramBot_Debug_Queue(botClient, message),
                        "/debug_mysql" => TelegramBot_Debug_MySQL_Load(botClient, message),
                        //Кнопки бота
                        //"Задачи" => Telegram

                        _ => Usage(botClient, message)
                    };
                    Message sentMessage = await action;
                    Console.WriteLine($"The message was sent with id: {sentMessage.MessageId}");
                    break;

                case MessageType.Voice:
                    string voice_file_id = message.Voice.FileId;
                    string url_to_file = GetLinkToDownloadOggAPI(voice_file_id);
                    var file = await botClient.GetFileAsync(voice_file_id);

                    if (!Directory.Exists(Path.Combine(System.IO.Directory.GetCurrentDirectory(), "voice_msgs")))
                    {
                        Directory.CreateDirectory(Path.Combine(System.IO.Directory.GetCurrentDirectory(), "voice_msgs"));
                    }

                    string file_name = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "voice_msgs", "voice_msg_" + message.Chat.Id.ToString() + Guid.NewGuid() + ".oga");

                    FileStream fs = new FileStream(file_name, FileMode.Create);
                    await botClient.DownloadFileAsync(file.FilePath, fs);
                    fs.Close();
                    fs.Dispose();

                    string stt_result = await Program.alexandra_yandex_api.Send_STT_API_KEY(file_name, Program.ALEXA_AUTH_KEY_ID_SECRET);
                    Program.alexandra_log.Log("Yandex API STT", "STT Text Response: " + stt_result, Core.AlexandraLog.LogStatus.Info);

                    //await Usage(botClient, message);

                    await TelegramBot_VoiceRecognition_SendBack(botClient, message, stt_result);
                    break;

                case MessageType.Audio:

                    break;

                case MessageType.Location:
                    await Program.logic_drive.OnReceiveMapPointHandler(botClient, message);
                    break;

                case MessageType.Document:
                    if (Program.alexandra_file_service.isEviCRM_PathValid())
                    {
                        await Program.alexandra_file_service.FileUpload(botClient, message);
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                              text: "К сожалению, хранилище EviCRM не готово принять файл 😔 \nПопробуй немного позднее");
                    }
                    break;

                case MessageType.Contact:
                    await Program.logic_drive.OnReceiveContactHandler(botClient, message);
                    break;

                case MessageType.Poll:
                    await Program.logic_drive.OnReceivePollHandler(botClient, message);
                    break;

                case MessageType.MessagePinned:
                    await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                          text: "К сожалению, хранилище EviCRM не готово принять файл 😔 \nПопробуй немного позднее");
                    break;

                case MessageType.Photo:

                    break;
            }
           
        }

        // Send inline keyboard
        // You can process responses in BotOnCallbackQueryReceived handler
        static async Task<Message> SendInlineKeyboard(ITelegramBotClient botClient, Message message)
        {
            await botClient.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

            // Simulate longer running task
            await Task.Delay(500);

            InlineKeyboardMarkup inlineKeyboard = new(
                new[]
                {
                    // first row
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("1.1", "11"),
                        InlineKeyboardButton.WithCallbackData("1.2", "12"),
                    },
                    // second row
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("2.1", "21"),
                        InlineKeyboardButton.WithCallbackData("2.2", "22"),
                    },
                });

            return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                        text: "Choose",
                                                        replyMarkup: inlineKeyboard);
        }


        static async Task<bool> TelegramBot_VoiceRecognition_SendBack(ITelegramBotClient botClient, Message chatMessage, string STT_result)
        {
            await botClient.SendChatActionAsync(chatMessage.Chat.Id, ChatAction.Typing);
            await botClient.SendTextMessageAsync(chatId: chatMessage.Chat.Id,
                                                            text: "📝 Я услышала: " + STT_result);
            return true;
        }

        static async Task<Message> TelegramBot_Debug_ChatID(ITelegramBotClient botClient, Message message)
        {
            await botClient.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

            return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                        text: "[Разработчик] : Для этого диалога Chat ID = " + message.Chat.Id.ToString());
        }

        static async Task<Message> TelegramBot_Debug_MySQL_Load(ITelegramBotClient botClient, Message message)
        {
            await botClient.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

            Program.telegram_chatID_lst.Clear();
            Program.telegram_login_lst.Clear();
            Program.telegram_login_status.Clear();

            Program.telegram_chatID_lst = await Program.mysql_C.getListChatIDAsync();
            Program.telegram_login_lst = await Program.mysql_C.getListLoginAsync();

            List<string> status = await Program.mysql_C.getListStatusAsync();

            if (status != null)
            {
                foreach (var elem in status)
                {
                    switch (elem)
                    {
                        case "ban":
                            Program.telegram_login_status.Add(Core.AlexandraCore.AlexandraLoginStatus.Banned);
                            break;

                        case "admin":
                            Program.telegram_login_status.Add(Core.AlexandraCore.AlexandraLoginStatus.AuthorizedAdmin);
                            break;

                        case "secretary":
                            Program.telegram_login_status.Add(Core.AlexandraCore.AlexandraLoginStatus.AuthorizedAdmin);
                            break;

                        case "owner":
                            Program.telegram_login_status.Add(Core.AlexandraCore.AlexandraLoginStatus.AuthorizedAdmin);
                            break;

                        case "user":
                            Program.telegram_login_status.Add(Core.AlexandraCore.AlexandraLoginStatus.AuthorizedUser);
                            break;

                        default:
                            Program.telegram_login_status.Add(Core.AlexandraCore.AlexandraLoginStatus.Unauthorized);
                            break;
                    }

                    break;
                }
            }

            return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                        text: "[Разработчик] : Загружено " + Program.telegram_chatID_lst.Count + " записей");
        }

        static async Task<Message> TelegramBot_Debug_DoYouKnowMe(ITelegramBotClient botClient, Message message)
        {
            await botClient.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

            for (int i = 0; i < Program.telegram_chatID_lst.Count; i++)
            {
                if (Program.telegram_chatID_lst[i] == message.Chat.Id.ToString())
                {
                    return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                            text: "[Разработчик] : Знаю тебя, запись c логином = " + Program.telegram_login_lst[i]);
                }


            }
            return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                            text: "[Разработчик] : Не знаю тебя( ");
        }


        static async Task<Message> SendReplyKeyboard(ITelegramBotClient botClient, Message message)
        {
            ReplyKeyboardMarkup replyKeyboardMarkup = new(
                new[]
                {
                        new KeyboardButton[] { "1.1", "1.2" },
                        new KeyboardButton[] { "2.1", "2.2" },
                })
            {
                ResizeKeyboard = true
            };

            return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                        text: "Choose",
                                                        replyMarkup: replyKeyboardMarkup);
        }

        static async Task<Message> RemoveKeyboard(ITelegramBotClient botClient, Message message)
        {
            return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                        text: "Removing keyboard",
                                                        replyMarkup: new ReplyKeyboardRemove());
        }

        static string GetLinkToDownloadOggAPI(string file_id)
        {
            return "https://api.telegram.org/bot" + EviCRM.Backend4.Telegram.Configuration.BotToken + "/getFile?file_id=" + file_id;
        }

        static async Task<Message> SendFile(ITelegramBotClient botClient, Message message)
        {
            await botClient.SendChatActionAsync(message.Chat.Id, ChatAction.UploadPhoto);

            const string filePath = @"Files/tux.png";
            using FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            var fileName = filePath.Split(Path.DirectorySeparatorChar).Last();

            return await botClient.SendPhotoAsync(chatId: message.Chat.Id,
                                                  photo: new InputOnlineFile(fileStream, fileName),
                                                  caption: "Nice Picture");
        }

        static async Task<Message> RequestContactAndLocation(ITelegramBotClient botClient, Message message)
        {
            ReplyKeyboardMarkup RequestReplyKeyboard = new(
                new[]
                {
                    KeyboardButton.WithRequestLocation("Location"),
                    KeyboardButton.WithRequestContact("Contact"),
                });

            return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                        text: "Who or Where are you?",
                                                        replyMarkup: RequestReplyKeyboard);
        }

        static async Task<Message> Usage(ITelegramBotClient botClient, Message message)
        {
            return await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                        text: message.Text,
                                                        replyMarkup: new ReplyKeyboardRemove());
        }
        // Process Inline Keyboard callback data
        private static async Task BotOnCallbackQueryReceived(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            await Program.logic_drive.QueryAnalyzerAsync(botClient, callbackQuery);
            //await botClient.AnswerCallbackQueryAsync(
            //    callbackQueryId: callbackQuery.Id,
            //    text: $"Received {callbackQuery.Data}");

            //await botClient.SendTextMessageAsync(
            //    chatId: callbackQuery.Message.Chat.Id,
            //    text: $"Received {callbackQuery.Data}");
        }

        private static async Task BotOnInlineQueryReceived(ITelegramBotClient botClient, InlineQuery inlineQuery)
        {
            Console.WriteLine($"Received inline query from: {inlineQuery.From.Id}");

            InlineQueryResult[] results = {
            // displayed result
            new InlineQueryResultArticle(
                id: "3",
                title: "TgBots",
                inputMessageContent: new InputTextMessageContent(
                    "hello"
                )
            )
        };

            await botClient.AnswerInlineQueryAsync(inlineQueryId: inlineQuery.Id,
                                                   results: results,
                                                   isPersonal: true,
                                                   cacheTime: 0);
        }

        private static Task BotOnChosenInlineResultReceived(ITelegramBotClient botClient, ChosenInlineResult chosenInlineResult)
        {
            Console.WriteLine($"Received inline result: {chosenInlineResult.ResultId}");
            return Task.CompletedTask;
        }

        private static Task UnknownUpdateHandlerAsync(ITelegramBotClient botClient, Update update)
        {
            Console.WriteLine($"Unknown update type: {update.Type}");
            return Task.CompletedTask;
        }
    }
}
