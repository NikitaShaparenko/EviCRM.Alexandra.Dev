using EviCRM.Backend4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;

namespace EviCRM.Backend4.Core
{
    public class AlexandraCronList
    {

        TelegramBotClient bot;

        public void CronListHandler()
        {
            DateTime dt_now = DateTime.Now;

            if (Program.cron_list != null)
            {
                if (Program.cron_list.Count > 0)
                {
                    //Есть отложенные задачи
                    for (int i = 0; i< Program.cron_list.Count;i++)
                    {
                        if (Program.cron_list[i].Cron_RemindWhen > dt_now)
                        {
                            CronExecuter(i);
                        }
                    }
                }
            }
        }

        void CronDelete(int index)
        {
            Program.cron_list.RemoveAt(index);
        }

        async Task CronExecuter(int index)
        {
            CronTask task = Program.cron_list[index];

            if (task != null)
            {
                await Program.alexandra_tsh.SendCustomMessage(bot, task.Cron_chatID, task.Cron_BodyMessage);
            }

            CronDelete(index);
        }

        void CronAdd(CronTask task)
        {
            Program.cron_list.Add(task);
        }

       public void Init_CronBotClientSet(TelegramBotClient _bot)
        {
            if (_bot != null)
            {
                bot = _bot;
            }
        }
    }
}
