using EviCRM.Backend4;
using EviCRM.Backend4.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace EviCRM.Alexandra.Core
{
    public class AlexandraFileService
    {
        public const string ALEXANDRA_CONTROL_NAME = "Alexandra File Storage Service";
        public bool isEviCRM_PathValid()
        {
            if (Program.EviCRM_WebRootEnvironment == null)
            {
                return false;
            }
            else
            {
                try
                {
                    if (System.IO.Directory.Exists(Program.EviCRM_WebRootEnvironment))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch
                {
                    return false;
                }

                return false;
            }

        }

        string getUnicalFileName(string filename_we, string extension)
        {
            string guid_str = Guid.NewGuid().ToString();
            return (filename_we + "_" + Path.GetRandomFileName() + extension);
        }

        string filepath_fix(string path)
        {
            string new_path = path;
            if (new_path.Contains(' ')) new_path = new_path.Replace(" ", "_");
            return new_path;
        }

        public async Task FileUpload(ITelegramBotClient botClient, Message message)
        {

            if (message.Document != null)
            {
                string voice_file_id = message.Document.FileId;
                string url_to_file = GetLinkToDownloadDocument(voice_file_id);
                var file = await botClient.GetFileAsync(voice_file_id);

                if (!Directory.Exists(Path.Combine(Program.EviCRM_WebRootEnvironment, "alexendra_store")))
                {
                    Directory.CreateDirectory(Path.Combine(System.IO.Directory.GetCurrentDirectory(), "alexandra_store"));
                    Program.alexandra_log.Log(ALEXANDRA_CONTROL_NAME, "Alexandra Storing files directory has been created!", AlexandraLog.LogStatus.Warning);
                }

                int? arr_num = Program.logic_drive.getArrNumByChatID(message.Chat.Id.ToString());

                if (arr_num == null)
                {
                    await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                  text: "К сожалению, я не могу загрузить файл, так как я не знаю тебя 😔 \nПопробуй зарегистрироваться заново");
                    return;
                }

                if (!Directory.Exists(Path.Combine(Program.EviCRM_WebRootEnvironment, "alexendra_store",Program.telegram_login_lst[(int)arr_num])))
                {
                    Directory.CreateDirectory(Path.Combine(Program.EviCRM_WebRootEnvironment, "alexendra_store", Program.telegram_login_lst[(int)arr_num]));
                    Program.alexandra_log.Log(ALEXANDRA_CONTROL_NAME, "Alexandra user's storing files directory has been created!", AlexandraLog.LogStatus.Warning);
                }

                string file_name = (Path.Combine(Program.EviCRM_WebRootEnvironment, "alexendra_store", Program.telegram_login_lst[(int)arr_num]));

                if (System.IO.File.Exists(Path.Combine(file_name,filepath_fix(message.Document.FileName))))
                {
                    string filename_without_extension = Path.GetFileNameWithoutExtension(message.Document.FileName);
                    string filename_extension = Path.GetExtension(message.Document.FileName);

                    string new_filename = getUnicalFileName(filename_without_extension, filename_extension);
                    file_name = Path.Combine(file_name, new_filename);

                    //Файл уже существует
                    await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                  text: "Загружаю файл💾\nНо ты уже загружал файл с таким названием\nПоэтому, новое имя файла: " + new_filename);

                }
                else
                {
                    await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                  text: $"Загружаю файл {message.Document.FileName} 💾");
                    file_name = Path.Combine(file_name, filepath_fix(message.Document.FileName));

                }

                FileStream fs = new FileStream(file_name, FileMode.Create);
                await botClient.DownloadFileAsync(file.FilePath, fs);
                fs.Close();
                fs.Dispose();

                await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                 text: $"Файл {filepath_fix(message.Document.FileName)} загружен успешно ✅");
            }
            else
            {
                await botClient.SendTextMessageAsync(chatId: message.Chat.Id,
                                                   text: "К сожалению, ты отправил пустой файл или он побился пока отправлялся 😔\nПопробуй отправить его мне ещё раз");
            }


        }

        string GetLinkToDownloadDocument(string file_id)
        {
            return "https://api.telegram.org/bot" + Backend4.Telegram.Configuration.BotToken + "/getFile?file_id=" + file_id;
        }


    }
}
