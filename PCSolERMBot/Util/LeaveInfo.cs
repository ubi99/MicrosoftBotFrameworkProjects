using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DemoHelpDeskBot.Util
{
    [Serializable]
    public class LeaveInfo
    {
        public string LeaveType;
        public string OpeningBalance;
        public string LeaveCredited;
        public string LeaveAvailed;
        public string BalanceAvailable;
    }
}