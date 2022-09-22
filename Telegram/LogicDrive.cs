using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace EviCRM.Backend4.Telegram
{

    public class LogicDrive
    {

        


      public enum AlexandraPages
        {
            Main,
            Login,
        }

        public void LogicAnalyzer(string command)
        {

        }

        public int? getArrNumByChatID(string chatID)
        {
            if (Program.telegram_chatID_lst!= null)
            {
                for (int i = 0; i<Program.telegram_chatID_lst.Count; i++)
                {
                    if (Program.telegram_chatID_lst[i]==chatID)
                    {
                        return i;
                    }
                }
            }
            return null;
        }


        public async Task OnReceivePollHandler(ITelegramBotClient botClient, Message message)
        {
            if (message.Poll == null)
            {
                await botClient.SendTextMessageAsync(
                   chatId: message.Chat.Id,
                   text: $"Ты отправил мне пустой опросник.\nЭто вообще как получилось?");
                return;
            }

            Poll poll = message.Poll;
            await Program.mysql_C.MySql_ContextAsync(Program.mysql_C.createPoll(poll));

            await botClient.SendTextMessageAsync(
                 chatId: message.Chat.Id,
                 text: $"Отправила опросник в EviCRM ✅");
            return;
        }

        public async Task OnReceiveContactHandler(ITelegramBotClient botClient, Message message)
        {
            if (message.Contact == null)
            {
                await botClient.SendTextMessageAsync(
                   chatId: message.Chat.Id,
                   text: $"Ты отправил мне пустой контакт.\nЭто вообще как получилось?");
                return;
            }

            Contact contact = message.Contact;

            int? p = getArrNumByChatID(message.Chat.Id.ToString());

            if (p.HasValue)
            {
                await Program.mysql_C.MySql_ContextAsync(Program.mysql_C.createContact(contact,Program.telegram_login_lst[(int)p]));

                await botClient.SendTextMessageAsync(
                     chatId: message.Chat.Id,
                     text: $"Отправила контакт в твою запись в EviCRM ✅");
                return;
            }
            else
            {
                await botClient.SendTextMessageAsync(
                   chatId: message.Chat.Id,
                   text: $"Не могу найти тебя среди пользователей\nМожет быть не зарегистрирован?😔");
                return;
            }

            return;
        }

        public async Task OnReceiveMapPointHandler(ITelegramBotClient botClient, Message message)
        {
            if (message.Location == null)
            {
                await botClient.SendTextMessageAsync(
                   chatId: message.Chat.Id,
                   text: $"Ты отправил мне пустую карту.\nЭто вообще как получилось?");
                return;
            }

            Location location = message.Location;

            int? p = getArrNumByChatID(message.Chat.Id.ToString());

            if (p.HasValue)
            {
                await Program.mysql_C.MySql_ContextAsync(Program.mysql_C.createMapPoint(location, "Точка на карте от Александры", Program.telegram_login_lst[(int)p]));

                await botClient.SendTextMessageAsync(
                  chatId: message.Chat.Id,
                  text: $"Отправила точку на карте в EviCRM ✅");
                return;
            }
            else
            {
                await botClient.SendTextMessageAsync(
                   chatId: message.Chat.Id,
                   text: $"Не могу найти тебя среди пользователей\nМожет быть не зарегистрирован?😔");
                return;
            }
        }

        public async Task QueryAnalyzerAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            string query_data = "Ошибка";

            if (callbackQuery.Data != null)
            {
                query_data = callbackQuery.Data;
            }

            int? arr_num = getArrNumByChatID(callbackQuery.Message.Chat.Id.ToString());
            
            if (arr_num == null)
            {
                //Пользователя среди знакомых нет.

                await botClient.SendTextMessageAsync(
                    chatId: callbackQuery.Message.Chat.Id,
                    text: $"Я тебя не знаю, но ты пытаешься мне что-то сообщить через всплывающую клавиатуру.\n Это вообще как получилось?");
                return;
            }
           
            switch (query_data)
            {
                case "Домой":
                    Program.telegram_page[(int)arr_num] = AlexandraPages.Main;
                    break;
            }

            //await botClient.AnswerCallbackQueryAsync(
            //    callbackQueryId: callbackQuery.Id,
            //    text: $"Received {callbackQuery.Data}");

        }

        public InlineKeyboardMarkup km_auth_register()
        {
            InlineKeyboardMarkup inlineKeyboard = new(
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Давай","Зарегистрироваться"),
                InlineKeyboardButton.WithCallbackData("Не хочу", "Домой"),
            });
            return inlineKeyboard;
        }

        public InlineKeyboardMarkup km_auth_register2()
        {
            InlineKeyboardMarkup inlineKeyboard = new(
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Давай","Зарегистрироваться"),
                InlineKeyboardButton.WithCallbackData("Не хочу", "Домой"),
            });
            return inlineKeyboard;
        }


        static async Task<Message> Se(ITelegramBotClient botClient, Message message)
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

        public InlineKeyboardMarkup getKeyboard(string request)
        {
            
            return null;
        }
    }
}
