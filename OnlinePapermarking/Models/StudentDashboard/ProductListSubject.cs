using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlinePapermarking.Models.StudentDashboard
{
    public class ProductListSubject
    {
        public int ExamTypeId { get; set; }
        public int SubjectId { get; set; }
        public int MediumId { get; set; }
        public string PaperName { get; set; }
    }
}