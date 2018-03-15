using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DemoHelpDeskBot.Util
{
    [Serializable]
    public class AttendanceInfo
    {
        public string Date;
        public string AttendanceType;
        public string Status;
    }
}