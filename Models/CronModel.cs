using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EviCRM.Backend4.Models
{
    internal class CronModel
    {
        
    }

    public class CronTask
        {

        public string Cron_BodyMessage { get; set; }

        public DateTime Cron_RemindWhen { get; set; }

        public string Cron_chatID { get; set; }

        }
}
