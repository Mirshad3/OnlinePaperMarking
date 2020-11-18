using OnlinePapermarking.Models;
using OnlinePapermarking.Models.HotSpot;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace OnlinePapermarking.Controllers
{
    public class ExamController : Controller
    {
        OnlinePaperMarkingEntities db = new OnlinePaperMarkingEntities();
        // GET: Exam
        public ActionResult MCQExam()
        {
            string id = Request.QueryString["__id__"];
            ViewBag.MyQSVal = Request.QueryString["id"];
            return View();
        }

        public ActionResult FirstExam()
        {
            return View();
        }

        public ActionResult GetFirstPaper()
        {
            string path = "";
            if (Session["FirstPaperExamId"] != null)
            {
                int examId = Convert.ToInt32(Session["FirstPaperExamId"]);
                var paper = db.Exams.Where(x => x.ExamID == examId).Join(
                        db.PastPapers,
                        ex => ex.PastPaperId,
                        pp => pp.PastPaerID,
                        (ex, pp) => new { Exam = ex, PastPaper = pp }).ToList().Select(x => new
                        {
                            x.PastPaper.PaperDownloadPath,
                            x.PastPaper.Year
                        }).FirstOrDefault();

                if (paper != null)
                {
                    path = paper.PaperDownloadPath + "/" + paper.Year + ".pdf";
                }
            }
            return File(path, "application/pdf");
        }

        public JsonResult LoadMcqAnswersList(int PastPaperId)
        {
            var result = false;
            try
            {
                int questionCount = 0;
                var paper = db.PastPapers.Where(x => x.PastPaerID == PastPaperId).FirstOrDefault();
                if(paper != null)
                {
                    questionCount = (int)paper.McqQuestionCount;
                }

                result = true;

                return Json(new { success = result, questionCount = questionCount }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetFirstPaperTime(int ExamId)
        {
            var result = false;
            int time = 0;
            try
            {
                var exam = db.Exams.Where(x => x.ExamID == ExamId).FirstOrDefault();
                if(exam != null)
                {
                    time = (int)db.PastPapers.Where(x => x.PastPaerID == exam.PastPaperId).Select(x => x.FirstPaperTime != null ? x.FirstPaperTime : 0).FirstOrDefault();
                    time = time - (time - exam.FirstPaperRemainingTime);
                }

                result = true;

                return Json(new { success = result, time }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult UpdateFirstPaperTime(int ExamId, int Hours, int Minuites)
        {
            var result = false;
            
            try
            {
                var exam = db.Exams.Where(x => x.ExamID == ExamId).FirstOrDefault();
                if (exam != null)
                {
                    int firstPaperTime = (int)db.PastPapers.Where(x => x.PastPaerID == exam.PastPaperId).Select(x => x.FirstPaperTime != null ? x.FirstPaperTime : 0).FirstOrDefault();
                    exam.FirstPaperRemainingTime = (Hours * 60 + Minuites) - 1 >= 0 ? (Hours * 60 + Minuites) - 1 : firstPaperTime;
                    db.SaveChanges();
                }

                result = true;

                return Json(new { success = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult UpdateMcqAnswer(int ExamId , int LoginId , string Answer)
        {
            var result = false;
            try
            {
                if(Answer != "")
                {
                    string[] answerArr = Answer.Split('.');
                    int questionNo = Convert.ToInt32(answerArr[0]);
                    int answer = Convert.ToInt32(answerArr[1]);

                    var answ = db.StudentsMcqAnswers.Where(x => x.StudentLoginId == LoginId && x.ExamId == ExamId && x.QuestionNo == questionNo).FirstOrDefault();
                    if(answ != null)
                    {
                        answ.Answer = answer;
                        answ.UpdatedUser = LoginId.ToString();
                        answ.UpdatedDate = DateTime.Now;
                        db.SaveChanges();
                    }
                    else
                    {
                        var newAnswer = new StudentsMcqAnswer();
                        newAnswer.ExamId = ExamId;
                        newAnswer.StudentLoginId = LoginId;
                        newAnswer.QuestionNo = questionNo;
                        newAnswer.Answer = answer;
                        newAnswer.CreatedUser = LoginId.ToString();
                        newAnswer.CreatedDate = DateTime.Now;
                        db.StudentsMcqAnswers.Add(newAnswer);
                        db.SaveChanges();
                    }
                }

                result = true;

                return Json(new { success = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult SubmitMcqAnswerSheet(int ExamId, int LoginId)
        {
            var result = false;
            int marks = 0;
            string notification = "";
            try
            {
                var exam = db.Exams.Where(x => x.ExamID == ExamId).FirstOrDefault();
                if(exam != null)
                {
                    var paper = db.PastPapers.Where(x => x.PastPaerID == exam.PastPaperId).FirstOrDefault();
                    if (paper != null)
                    {
                        var correctAnswersList = db.McqDetails.Where(x => x.ExamTypeID == paper.ExamTypeID && x.SubjectID == paper.SubjectID && x.Year == paper.Year).Select(x => new
                        {
                            x.Answer,
                            x.QuestionNo
                        }).OrderBy(x => x.QuestionNo).ToList();

                        foreach (var corAnswr in correctAnswersList)
                        {
                            int answer = Convert.ToInt32(corAnswr.Answer);
                            var studentAnswer = db.StudentsMcqAnswers.Where(x => x.StudentLoginId == LoginId && x.ExamId == ExamId && x.QuestionNo == corAnswr.QuestionNo && x.Answer == answer).FirstOrDefault();
                            if(studentAnswer != null)
                            {
                                marks = marks + 1;
                            }
                        }
                    }

                    exam.IsFirstPaperCompleted = true;
                    if(exam.IsSecondPaperCompleted == true)
                    {
                        exam.StudentStatus = "Completed";
                        notification = "Your submission is success and you can view your marks from your completed exams list.";
                    }
                    else
                    {
                        notification = "Your submission is success. Please complete your second paper for this exam to view your marks.";
                    }
                    exam.FirstPaperMarks = marks;
                    db.SaveChanges();
                }

                result = true;

                return Json(new { success = result , message = notification }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult UploadFirstPaperAnswers()
        {
            var result = false;
            try
            {
                string studentLoginId = Request.Form["LoginId"];
                string examId = Request.Form["ExamId"];
                int intExamId = Convert.ToInt32(examId);

                var exam = db.Exams.Where(x => x.ExamID == intExamId).FirstOrDefault();

                var paper = db.PastPapers.Where(x => x.PastPaerID == exam.PastPaperId).FirstOrDefault();

                string mainFolderName = examId;
                string subFolderName = examId + "-1";
                string webConfigPath = ConfigurationManager.AppSettings["StudentUploadPath"];
                string mainPath = @"" + webConfigPath + "" + mainFolderName;

                string subPath = mainPath + "\\" + subFolderName;

                if (!(Directory.Exists(mainPath)))
                {
                    Directory.CreateDirectory(mainPath);
                }

                if (!(Directory.Exists(subPath)))
                {
                    Directory.CreateDirectory(subPath);

                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        var file = Request.Files[i];
                        var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                        string yymmssfff = DateTime.Now.ToString("yymmssfff");
                        string newFileName = fileName + yymmssfff;
                        fileName = fileName + yymmssfff + Path.GetExtension(file.FileName);
                        string newPath = subPath + "\\";
                        newPath = newPath + fileName;
                        file.SaveAs(newPath);

                        UploadImage upImg = new UploadImage();
                        upImg.ImageId = newFileName;
                        upImg.ImagePath = newPath;
                        upImg.ExamId = intExamId;
                        db.UploadImages.Add(upImg);
                        db.SaveChanges();
                    }

                    
                    if (exam.IsSecondPaperCompleted == true)
                    {
                        exam.StudentStatus = "Completed";
                        exam.AssignToTutor = "Pending";
                        exam.AssignToTutorReadyDate = DateTime.Now;
                    }
                    exam.IsFirstPaperCompleted = true;
                    exam.FirstPaperAnswerSheetsPath = subPath;
                    exam.UpdateDate = DateTime.Now;
                    exam.UpdateUser = studentLoginId;
                    db.SaveChanges();

                    if (exam.IsSecondPaperCompleted == true)
                    {
                        //Create notification
                        var subject = db.Exams.Where(x => x.ExamID == intExamId).Join(
                                db.Subjects,
                                ex => ex.SubjectId,
                                su => su.SubjectID,
                                (ex, su) => new { Exam = ex, Subject = su }).Select(x => x.Subject.Description).FirstOrDefault();

                        var notification = new Notification();
                        notification.NotificationHeader = subject + " Paper Recieved.";
                        notification.NotificationDetail = "You can get the paper by clicking this notification.";
                        notification.CreatedUser = "System";
                        notification.CreatedDate = DateTime.Now;
                        db.Notifications.Add(notification);
                        db.SaveChanges();

                        //Assign notification
                        var tutors = db.tblTutors.Where(x => x.PendingExamsCount < 5 && x.AdminApproval == "Approved").Join(
                                db.PreferedTutorExamTypes.Where(x => x.ExamTypeId == exam.ExamTypeId),
                                tu => tu.LoginID,
                                pet => pet.LoginId,
                                (tu, pet) => new { tblTutor = tu, PreferedTutorExamType = pet }).Join(
                                db.PreferedTutorMediums.Where(x => x.MediumId == exam.MediumId),
                                tu => tu.tblTutor.LoginID,
                                pme => pme.LoginId,
                                (tu, pet) => new { tu.tblTutor, tu.PreferedTutorExamType, PreferedTutorMediums = pet }).Join(
                                db.PreferedTutorSubjects.Where(x => x.SubjectId == exam.SubjectId),
                                tu => tu.tblTutor.LoginID,
                                psu => psu.LoginId,
                                (tu, psu) => new { tu.tblTutor, tu.PreferedTutorExamType, tu.PreferedTutorMediums, PreferedTutorSubjects = psu }).ToList().Select(x => new
                                {
                                    x.tblTutor.LoginID
                                }).ToList();

                        foreach (var tutor in tutors)
                        {
                            var notiAssign = new NotificationAssign();
                            notiAssign.NotificationId = notification.NotificationID;
                            notiAssign.LoginId = (long)tutor.LoginID;
                            notiAssign.ControllerName = "TutorDashboard";
                            notiAssign.MethodName = "ExamApprovals";
                            notiAssign.IsExpired = false;
                            notiAssign.CreatedUser = "System";
                            notiAssign.CreatedDate = DateTime.Now;
                            db.NotificationAssigns.Add(notiAssign);
                            exam.NotificationId = notification.NotificationID;
                            db.SaveChanges();
                        }
                    }
                    
                    result = true;
                }
                else
                {
                    //for (int i = 0; i < Request.Files.Count; i++)
                    //{
                    //    var file = Request.Files[i];
                    //    var fileName = file.FileName.ToString();
                    //    fileName = fileName + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(file.FileName);
                    //    string newPath = subPath + "\\";
                    //    newPath = newPath + fileName;
                    //    file.SaveAs(newPath);
                    //}

                    //exam.AssignToTutor = "Pending";
                    //exam.StudentStatus = "Completed";
                    //exam.SecondPaperAnswerSheetsPath = subPath;
                    //exam.UpdateDate = DateTime.Now;
                    //exam.UpdateUser = studentLoginId;
                    //db.SaveChanges();

                    //result = true;
                }

                //result = true;

                return Json(new { success = result , hasMcq = paper.HasMcq , isSecondPaperCompleted = exam.IsSecondPaperCompleted }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        //--------------------------------------------------------------Second Paper-----------------------------------------------------------------------------------------------------------------------------
        public ActionResult SecondPaper()
        {
            return View();
        }

        public ActionResult GetSecondPaper()
        {
            string path = "";
            if (Session["SecondPaperExamId"] != null)
            {
                int examId = Convert.ToInt32(Session["SecondPaperExamId"]);
                var paper = db.Exams.Where(x => x.ExamID == examId).Join(
                        db.PastPapers,
                        ex => ex.PastPaperId,
                        pp => pp.PastPaerID,
                        (ex, pp) => new { Exam = ex, PastPaper = pp }).ToList().Select(x => new
                        {
                            x.PastPaper.PaperDownloadPath,
                            x.PastPaper.Year
                        }).FirstOrDefault();

                if (paper != null)
                {
                    path = paper.PaperDownloadPath + "/" + paper.Year + ".pdf";
                }
            }
            return File(path, "application/pdf");
        }

        public JsonResult GetSecondPaperTime(int ExamId)
        {
            var result = false;
            int time = 0;
            try
            {
                var exam = db.Exams.Where(x => x.ExamID == ExamId).FirstOrDefault();
                if (exam != null)
                {
                    var paper = db.PastPapers.Where(x => x.PastPaerID == exam.PastPaperId).FirstOrDefault();
                    if (paper.HasThirdPaper)
                    {
                        if (exam.IsSecondPaperCompleted)
                        {
                            time = paper.ThirdPaperTime != null ? (int)paper.ThirdPaperTime - ((int)paper.ThirdPaperTime - exam.ThirdPaperRemainingTime) : 0;
                        }
                        else
                        {
                            time = paper.SecondPaperTime != null ? (int)paper.SecondPaperTime - ((int)paper.SecondPaperTime - exam.SecondPaperRemainingTime) : 0;
                        }
                    }
                    else
                    {
                        time = paper.SecondPaperTime != null ? (int)paper.SecondPaperTime - ((int)paper.SecondPaperTime - exam.SecondPaperRemainingTime) : 0;
                    }
                    
                }

                result = true;

                return Json(new { success = result, time }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult UploadSecondPaperAnswers()
        {
            var result = false;
            try
            {
                string studentLoginId = Request.Form["LoginId"];
                string examId = Request.Form["ExamId"];
                int intExamId = Convert.ToInt32(examId);

                var exam = db.Exams.Where(x => x.ExamID == intExamId).FirstOrDefault();

                var paper = db.PastPapers.Where(x => x.PastPaerID == exam.PastPaperId).FirstOrDefault();

                string mainFolderName = examId;
                string subFolderName = examId + "-2";
                string webConfigPath = ConfigurationManager.AppSettings["StudentUploadPath"];
                string mainPath = @"" + webConfigPath + "" + mainFolderName;

                string subPath = mainPath + "\\" + subFolderName;

                if (!(Directory.Exists(mainPath)))
                {
                    Directory.CreateDirectory(mainPath);
                }

                if (!(Directory.Exists(subPath)))
                {
                    Directory.CreateDirectory(subPath);
                }

                for (int i = 0; i < Request.Files.Count; i++)
                {
                    var file = Request.Files[i];
                    var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                    string yymmssfff = DateTime.Now.ToString("yymmssfff");
                    string newFileName = fileName + yymmssfff;
                    fileName = fileName + yymmssfff + Path.GetExtension(file.FileName);
                    string newPath = subPath + "\\";
                    newPath = newPath + fileName;
                    file.SaveAs(newPath);

                    UploadImage upImg = new UploadImage();
                    upImg.ImageId = newFileName;
                    upImg.ImagePath = newPath;
                    upImg.ExamId = intExamId;
                    db.UploadImages.Add(upImg);
                    db.SaveChanges();
                }

                if (paper.HasThirdPaper)
                {
                    if (paper.HasMcq)
                    {
                        if (exam.IsSecondPaperCompleted)
                        {
                            exam.IsThirdPaperCompleted = true;
                            exam.AssignToTutor = "Pending";
                            exam.AssignToTutorReadyDate = DateTime.Now;
                            if (exam.IsFirstPaperCompleted == true)
                            {
                                exam.StudentStatus = "Completed";
                            }
                        }
                    }
                    else
                    {
                        if (exam.IsFirstPaperCompleted == true && exam.IsSecondPaperCompleted)
                        {
                            exam.IsThirdPaperCompleted = true;
                            exam.AssignToTutor = "Pending";
                            exam.StudentStatus = "Completed";
                            exam.AssignToTutorReadyDate = DateTime.Now;
                        }
                        else if (exam.IsSecondPaperCompleted)
                        {
                            exam.IsThirdPaperCompleted = true;
                        }
                    }
                }
                else
                {
                    if (paper.HasMcq)
                    {
                        exam.AssignToTutor = "Pending";
                        exam.AssignToTutorReadyDate = DateTime.Now;
                        if (exam.IsFirstPaperCompleted == true)
                        {
                            exam.StudentStatus = "Completed";
                        }
                    }
                    else
                    {
                        if (exam.IsFirstPaperCompleted == true)
                        {
                            exam.AssignToTutor = "Pending";
                            exam.StudentStatus = "Completed";
                            exam.AssignToTutorReadyDate = DateTime.Now;
                        }
                    }
                }

                exam.IsSecondPaperCompleted = true;
                exam.SecondPaperAnswerSheetsPath = subPath;
                exam.UpdateDate = DateTime.Now;
                exam.UpdateUser = studentLoginId;
                db.SaveChanges();

                var paperName = db.Exams.Where(x => x.ExamID == intExamId).Join(
                        db.PastPapers,
                        ex => ex.PastPaperId,
                        pp => pp.PastPaerID,
                        (ex, pp) => new { Exam = ex, PastPaper = pp }).Join(
                        db.Mediums,
                        pp => pp.PastPaper.MediumID,
                        me => me.MediumID,
                        (pp, me) => new { pp.Exam, pp.PastPaper, Medium = me }).Join(
                        db.Subjects,
                        pp => pp.PastPaper.SubjectID,
                        su => su.SubjectID,
                        (pp, su) => new { pp.Exam, pp.PastPaper, pp.Medium, Subject = su }).ToList().Select(x =>
                        
                            (x.PastPaper.ExamTypeID == 1 && x.PastPaper.HasThirdPaper == false) ? x.Exam.ExamNo + "-O/L-" + x.Subject.Description + "-" + x.PastPaper.Year + " (Second Paper)" :
                            (x.PastPaper.ExamTypeID == 2 && x.PastPaper.HasThirdPaper == false) ? x.Exam.ExamNo + "-A/L" + x.Subject.Description + "-" + x.PastPaper.Year + " (Second Paper)" :
                            (x.PastPaper.ExamTypeID == 1 && x.PastPaper.HasThirdPaper == true && x.Exam.IsSecondPaperCompleted == false) ? x.Exam.ExamNo + "-O/L" + x.Subject.Description + "-" + x.PastPaper.Year + " (Second Paper)" :
                            (x.PastPaper.ExamTypeID == 2 && x.PastPaper.HasThirdPaper == true && x.Exam.IsSecondPaperCompleted == false) ? x.Exam.ExamNo + "-A/L" + x.Subject.Description + "-" + x.PastPaper.Year + " (Second Paper)" :
                            (x.PastPaper.ExamTypeID == 1 && x.PastPaper.HasThirdPaper == true && x.Exam.IsSecondPaperCompleted == true) ? x.Exam.ExamNo + "-O/L" + x.Subject.Description + "-" + x.PastPaper.Year + " (Third Paper)" :
                            (x.PastPaper.ExamTypeID == 2 && x.PastPaper.HasThirdPaper == true && x.Exam.IsSecondPaperCompleted == true) ? x.Exam.ExamNo + "-A/L" + x.Subject.Description + "-" + x.PastPaper.Year + " (Third Paper)" : ""
                            
                        ).FirstOrDefault();

                if ((paper.HasThirdPaper && paper.HasMcq && exam.IsThirdPaperCompleted) ||
                    (paper.HasThirdPaper && paper.HasMcq == false && exam.IsThirdPaperCompleted && exam.IsFirstPaperCompleted == true) ||
                    (paper.HasThirdPaper == false && paper.HasMcq) ||
                    (paper.HasThirdPaper == false && paper.HasMcq == false && exam.IsFirstPaperCompleted == true))
                {
                    //Create notification
                    var subject = db.Exams.Where(x => x.ExamID == intExamId).Join(
                            db.Subjects,
                            ex => ex.SubjectId,
                            su => su.SubjectID,
                            (ex, su) => new { Exam = ex, Subject = su }).Select(x => x.Subject.Description).FirstOrDefault();

                    var notification = new Notification();
                    notification.NotificationHeader = subject + " Paper Recieved.";
                    notification.NotificationDetail = "You can get the paper by clicking this notification.";
                    notification.CreatedUser = "System";
                    notification.CreatedDate = DateTime.Now;
                    db.Notifications.Add(notification);
                    db.SaveChanges();

                    //Assign notification
                    var tutors = db.tblTutors.Where(x => x.PendingExamsCount < 5 && x.AdminApproval == "Approved").Join(
                            db.PreferedTutorExamTypes.Where(x => x.ExamTypeId == exam.ExamTypeId),
                            tu => tu.LoginID,
                            pet => pet.LoginId,
                            (tu, pet) => new { tblTutor = tu, PreferedTutorExamType = pet }).Join(
                            db.PreferedTutorMediums.Where(x => x.MediumId == exam.MediumId),
                            tu => tu.tblTutor.LoginID,
                            pme => pme.LoginId,
                            (tu, pet) => new { tu.tblTutor, tu.PreferedTutorExamType, PreferedTutorMediums = pet }).Join(
                            db.PreferedTutorSubjects.Where(x => x.SubjectId == exam.SubjectId),
                            tu => tu.tblTutor.LoginID,
                            psu => psu.LoginId,
                            (tu, psu) => new { tu.tblTutor, tu.PreferedTutorExamType, tu.PreferedTutorMediums, PreferedTutorSubjects = psu }).ToList().Select(x => new
                            {
                                x.tblTutor.LoginID
                            }).ToList();

                    foreach (var tutor in tutors)
                    {
                        var notiAssign = new NotificationAssign();
                        notiAssign.NotificationId = notification.NotificationID;
                        notiAssign.LoginId = (long)tutor.LoginID;
                        notiAssign.ControllerName = "TutorDashboard";
                        notiAssign.MethodName = "ExamApprovals";
                        notiAssign.IsExpired = false;
                        notiAssign.CreatedUser = "System";
                        notiAssign.CreatedDate = DateTime.Now;
                        db.NotificationAssigns.Add(notiAssign);
                        exam.NotificationId = notification.NotificationID;
                        db.SaveChanges();
                    }
                }

                result = true;

                //result = true;

                return Json(new { success = result , hasMcq = paper.HasMcq, hasThirdPaper = paper.HasThirdPaper, isFirstPaperCompleted = exam.IsFirstPaperCompleted, isSecondPaperCompleted = exam.IsSecondPaperCompleted, isThirdPaperCompleted = exam.IsThirdPaperCompleted, thirdPaperTime = paper.ThirdPaperTime, paperName }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult UpdateSecondPaperUsedTime(int ExamId, int Hours, int Minuites)
        {
            var result = false;
            try
            {
                var exam = db.Exams.Where(x => x.ExamID == ExamId).FirstOrDefault();
                if (exam != null)
                {
                    var paper = db.PastPapers.Where(x => x.PastPaerID == exam.PastPaperId).FirstOrDefault();
                    if (paper.HasThirdPaper)
                    {
                        if (exam.IsSecondPaperCompleted)
                        {
                            int thirdPaperTime = (int)(paper.ThirdPaperTime == null ? 0 : paper.ThirdPaperTime);
                            exam.ThirdPaperRemainingTime = (Hours * 60 + Minuites)-1 >= 0 ? (Hours * 60 + Minuites) - 1 : thirdPaperTime;
                        }
                        else
                        {
                            int secondPaperTime = (int)(paper.SecondPaperTime == null ? 0 : paper.SecondPaperTime);
                            exam.SecondPaperRemainingTime = (Hours * 60 + Minuites) - 1 >= 0 ? (Hours * 60 + Minuites) - 1 : secondPaperTime;
                        }
                    }
                    else
                    {
                        int secondPaperTime = (int)(paper.SecondPaperTime == null ? 0 : paper.SecondPaperTime);
                        exam.SecondPaperRemainingTime = (Hours * 60 + Minuites) - 1 >= 0 ? (Hours * 60 + Minuites) - 1 : secondPaperTime;
                    }

                    db.SaveChanges();
                }

                result = true;

                return Json(new { success = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Test()
        {
            return View();
        }

        public ActionResult MapPage(int ExamId)
        {
            var pendingExams = db.Exams.Where(x => x.ExamID == ExamId).Join(
                        db.PastPapers,
                        ex => ex.PastPaperId,
                        pp => pp.PastPaerID,
                        (ex, pp) => new { Exam = ex, PastPaper = pp }).Join(
                        db.Mediums,
                        pp => pp.PastPaper.MediumID,
                        me => me.MediumID,
                        (pp, me) => new { pp.Exam, pp.PastPaper, Medium = me }).Join(
                        db.Subjects,
                        pp => pp.PastPaper.SubjectID,
                        su => su.SubjectID,
                        (pp, su) => new { pp.Exam, pp.PastPaper, pp.Medium, Subject = su }).ToList().Select(x => new
                        {
                            Medium = x.Medium.Description,
                            x.Exam.ExamNo,
                            PaperName = (x.PastPaper.ExamTypeID == 1 ? x.Exam.ExamNo + "-O/L-" + x.Subject.Description + "-" + x.PastPaper.Year + " (Second Paper)" :
                            x.PastPaper.ExamTypeID == 2 ? x.Exam.ExamNo + "-A/L" + x.Subject.Description + "-" + x.PastPaper.Year + " (Second Paper)" : ""),
                            x.PastPaper.HasMap
                        }).FirstOrDefault();

            Session["SecondPaperExamIdForMap"] = ExamId;
            Session["SecondPaperNameForMap"] = pendingExams.PaperName;

            return View();
        }

        public ActionResult GetMap(int ExamId)
        {
            string path = @"";
            //if (Session["SecondPaperExamId"] != null)
            //{
            //    int examId = Convert.ToInt32(Session["SecondPaperExamId"]);
            var paper = db.Exams.Where(x => x.ExamID == ExamId).Join(
                    db.PastPapers,
                    ex => ex.PastPaperId,
                    pp => pp.PastPaerID,
                    (ex, pp) => new { Exam = ex, PastPaper = pp }).ToList().Select(x => new
                    {
                        x.PastPaper.MapImagePath,
                        x.PastPaper.Year
                    }).FirstOrDefault();

            if (paper != null)
            {
                path = paper.MapImagePath;
            }
            //}
            return File(path, "image/jpg");
        }

        public JsonResult SubmitHotspotDetails(int ExamId, int LoginId, string Type, HotSpotDetails HpDetails)
        {
            var result = false;
            int marks = 0;
            try
            {
                var exam = db.Exams.Where(x => x.ExamID == ExamId).FirstOrDefault();
                if (exam != null)
                {
                   //foreach(var hp in HpDetails)
                    //{
                        var checkHp = db.ExamMapDetails.Where(x => x.X == HpDetails.x && x.Y == HpDetails.y && x.ExamId == ExamId).FirstOrDefault();
                        if (checkHp == null)
                        {
                            ExamMapDetail map = new ExamMapDetail();
                            map.ExamId = ExamId;
                            map.X = HpDetails.x;
                            map.Y = HpDetails.y;
                            map.Title = HpDetails.Title;
                            map.Message = HpDetails.Message;
                            map.CreatedType = Type;
                            db.ExamMapDetails.Add(map);
                            db.SaveChanges();

                            result = true;
                        }
                    //}
                    
                }

                return Json(new { success = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetHotSpotData(int ExamId)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var result = false;
           
            try
            {
                var exam = db.Exams.Where(x => x.ExamID == ExamId).FirstOrDefault();
                if (exam != null)
                {
                    var hotSpotDetails = db.ExamMapDetails.Where(x => x.ExamId == ExamId).Select(y => new HotSpotDetails
                    {
                        x = y.X,
                        y = y.Y,
                        Title = y.Title,
                        Message = y.Message
                    }).ToList();

                    result = true;

                    return Json(new { success = result, dataList = hotSpotDetails }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = result}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult DeleteHotSpotData(int ExamId)
        {
            var result = false;
            try
            {
                var exam = db.Exams.Where(x => x.ExamID == ExamId).FirstOrDefault();
                if (exam != null)
                {
                    db.ExamMapDetails.RemoveRange(db.ExamMapDetails.Where(x => x.ExamId == ExamId && x.CreatedType == "Student"));
                    db.SaveChanges();

                    result = true;

                    return Json(new { success = result }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult DeleteOneSpotData(int ExamId, string Type, string X, string Y)
        {
            var result = false;
            try
            {
                var exam = db.Exams.Where(x => x.ExamID == ExamId).FirstOrDefault();
                if (exam != null)
                {
                    var hotspot = db.ExamMapDetails.Where(x => x.ExamId == ExamId && x.X == X && x.Y == Y).FirstOrDefault();
                    if (hotspot != null)
                    {
                        if(hotspot.CreatedType == "Student" && Type == "Tutor")
                        {
                            return Json(new { success = result , message = "Can't remove student's comments." }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            result = true;
                            db.ExamMapDetails.Remove(hotspot);
                            db.SaveChanges();
                        }
                    }
                    
                    return Json(new { success = result , message = "Some error occured." }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}