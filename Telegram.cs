using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;

namespace EviCRM.Backend4
{
   public class TelegramSenderHandler
    {
        public async Task QueueHandler(ITelegramBotClient botClient)
        {
            while(true)
            { 
            if (Program.telegram_queue_chatID_lst.Count > 0)
            {
                for (int z = 0; z < Program.telegram_queue_chatID_lst.Count; z+=2)
                {
                    if (Program.telegram_queue_chatID_lst[z] != "")
                            if (long.TryParse(Program.telegram_queue_chatID_lst[z],out long a))
                        { 
                        await botClient.SendTextMessageAsync(chatId: long.Parse(Program.telegram_queue_chatID_lst[z]),
                                                           text: Program.telegram_queue_message_lst[z]);
                    
                      
                        Program.telegram_queue_message_lst.RemoveAt(z);
                        Program.telegram_queue_chatID_lst.RemoveAt(z);
                        }
                    }
                
            }
            }
        }

        public async Task SendCustomMessage(ITelegramBotClient botClient, string chatID, string body_text)
        {
            await botClient.SendTextMessageAsync(chatId: chatID, text: body_text);
        }

    }
}
