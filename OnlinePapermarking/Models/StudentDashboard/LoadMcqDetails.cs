using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlinePapermarking.Models.StudentDashboard
{
    public class LoadMcqDetails
    {
        public string CorrectQuestionNumber {get;set;}
        public string CorrectAnswer { get; set; }
        public string StudentQuestionNumber { get; set; }
        public string StudentAnswer { get; set; }
    }
}