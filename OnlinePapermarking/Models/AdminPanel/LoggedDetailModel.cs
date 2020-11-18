using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlinePapermarking.Models.AdminPanel
{
    public class LoggedDetailModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string UserType { get; set; }
        public string LoggedDate { get; set; }
    }
}