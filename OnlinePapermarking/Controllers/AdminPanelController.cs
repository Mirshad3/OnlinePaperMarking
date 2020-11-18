using OnlinePapermarking.Models;
using OnlinePapermarking.Models.AdminPanel;
using OnlinePapermarking.Models.TutorDashboard;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace OnlinePapermarking.Controllers
{
    public class AdminPanelController : Controller
    {
        OnlinePaperMarkingEntities db = new OnlinePaperMarkingEntities();

        // GET: AdminPanel
        public ActionResult AdminDashboard()
        {
            dynamic myModal = new ExpandoObject();
            myModal.Subjects = GetSubjectsForCombo();
            myModal.Districts = GetDistrictForCombo();
            myModal.Mediums = GetMediumsForCombo();
            myModal.ExamTypes = GetExamTypesForCombo();
            myModal.Banks = GetBanksForCombo();
            return View(myModal);
        }

        public JsonResult GetDashboardBoxDetails()
        {
            var result = false;
            try
            {
                var oldStudentList = new HashSet<string>(db.OldUsers.ToList().Select(x => x.MobileNo));
                var totalStudentList = db.tblOnlineStudentMasters.Join(
                        db.OnlineUsers.Where(x => x.IsBlocked == false),
                        st => st.LoginID,
                        ou => ou.LoginID,
                        (st, ou) => new { tblOnlineStudentMaster = st, OnlineUser = ou }).ToList();

                int activeStudentCount = db.tblOnlineStudentMasters.Join(
                        db.OnlineUsers.Where(x => x.IsBlocked == false),
                        st => st.LoginID,
                        ou => ou.LoginID,
                        (st, ou) => new { tblOnlineStudentMaster = st, OnlineUser = ou }).Join(
                        db.OldUsers,
                        st => st.tblOnlineStudentMaster.ContactNo1,
                        ou => ou.MobileNo,
                        (st, ou) => new { st.tblOnlineStudentMaster, st.OnlineUser, OldUser = ou }).Count();

                int newOlStudentCount = totalStudentList.Where(x => (!oldStudentList.Contains(x.tblOnlineStudentMaster.ContactNo1)) && (x.tblOnlineStudentMaster.ExamTypeId == 1)).Count();

                int newAlStudentCount = totalStudentList.Where(x => (!oldStudentList.Contains(x.tblOnlineStudentMaster.ContactNo1)) && (x.tblOnlineStudentMaster.ExamTypeId == 2)).Count();

                int newStudentCount = totalStudentList.Where(x => !oldStudentList.Contains(x.tblOnlineStudentMaster.ContactNo1)).Count();

                int purchasedExamsCount = db.Exams.Count();

                int downloadedPaperCount = db.PaperDownloadDetails.Count() + 25497;

                result = true;

                return Json(new { success = result, activeStudentCount, newOlStudentCount, newAlStudentCount, newStudentCount, purchasedExamsCount, downloadedPaperCount }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetDashboardChartDetails()
        {
            var result = false;
            try
            {
                List<string> monthList = new List<string>();
                List<string> valuesList = new List<string>();
                for(int i = 1; i <= 12; i++)
                {
                    var user = db.OnlineUsers.Where(x => x.CreatedDate.Value.Year == DateTime.Now.Year && x.CreatedDate.Value.Month == i && x.UserRoleId != 1).Join(
                            db.tblOnlineStudentMasters,
                            ou => ou.LoginID,
                            stu => stu.LoginID,
                            (ou,stu) => new { OnlineUser = ou , tblOnlineStudentMaster = stu}).ToList();
                    string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i);
                    int count = user.Count;
                    monthList.Add(monthName);
                    valuesList.Add(count.ToString());
                }

                result = true;

                return Json(new { success = result, month = monthList.ToArray(), values = valuesList.ToArray() }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        //-----------------------------------------------------------Students------------------------------------------------------------------------------------------------------------------------------------------------
        public ActionResult Students()
        {
            dynamic myModal = new ExpandoObject();
            myModal.Provinces = GetProvicesForCombo();
            myModal.Districts = GetDistrictForCombo();
            myModal.Mediums = GetMediumsForCombo();
            return View(myModal);
        }

        public List<Province> GetProvicesForCombo()
        {
            var proList = db.Provinces.ToList();
            return proList;
        }

        public JsonResult GetStudents()
        {
            var result = false;
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var students = db.tblOnlineStudentMasters.Join(
                        db.OnlineUsers,
                        tu => tu.LoginID,
                        ou => ou.LoginID,
                        (tu, ou) => new { tblOnlineStudentMaster = tu, OnlineUser = ou }).ToList().Select(x => new {
                            Name = x.tblOnlineStudentMaster.StudentFirstName + " " + x.tblOnlineStudentMaster.StudentLastName,
                            x.tblOnlineStudentMaster.Email,
                            x.tblOnlineStudentMaster.ContactNo1,
                            x.OnlineUser.IsBlocked,
                            x.OnlineUser.LoginID,
                            x.OnlineUser.CreatedDate,
                            JoinDate = x.OnlineUser.CreatedDate?.ToString("MM/dd/yyyy",CultureInfo.InvariantCulture),
                            x.tblOnlineStudentMaster.PurchasedPaperCount,
                            x.tblOnlineStudentMaster.DownloadedPaperCount
                        }).ToList().OrderBy(x=>x.CreatedDate);

                result = true;

                return Json(new { success = result, data = students }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult BlockUnblockUsers(long LoginId)
        {
            var result = false;
            try
            {
                var user = db.OnlineUsers.Where(x => x.LoginID == LoginId).FirstOrDefault();
                if(user != null)
                {
                    //WebRequest request = HttpWebRequest.Create("https://richcommunication.dialog.lk/api/sms/inline/send?q=8fb8d95fe0b4dec&destination=94719775173&message=Test&from=DISAPAMOCK");
                    //WebResponse response = request.GetResponse();
                    //StreamReader reader = new StreamReader(response.GetResponseStream());
                    //string urlText = reader.ReadToEnd();

                    if (user.IsBlocked)
                    {
                        user.IsBlocked = false;
                    }else if(user.IsBlocked == false)
                    {
                        user.IsBlocked = true;
                    }

                    db.SaveChanges();
                    result = true;
                }

                return Json(new { success = result}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        //------------------------------------------------------------Lectures----------------------------------------------------------------------------------------------------------------------------------------------
        public ActionResult Lectures()
        {
            dynamic myModal = new ExpandoObject();
            myModal.Subjects = GetSubjectsForCombo();
            myModal.Districts = GetDistrictForCombo();
            myModal.Mediums = GetMediumsForCombo();
            myModal.ExamTypes = GetExamTypesForCombo();
            myModal.Banks = GetBanksForCombo();
            return View(myModal);
        }

        public List<Subject> GetSubjectsForCombo()
        {
            var subList = db.Subjects.ToList();
            return subList;
        }

        public List<District> GetDistrictForCombo()
        {
            var disList = db.Districts.ToList();
            return disList;
        }

        public List<Medium> GetMediumsForCombo()
        {
            var medList = db.Mediums.ToList();
            return medList;
        }

        
        public List<Bank> GetBanksForCombo()
        {
            var bankList = db.Banks.ToList();
            return bankList;
        }

        public JsonResult GetPendingLectures(string AdminApproval)
        {
            var result = false;
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var tutors = db.tblTutors.Where(x => x.AdminApproval == AdminApproval).Join(
                        db.OnlineUsers,
                        tu => tu.LoginID,
                        ou => ou.LoginID,
                        (tu,ou) => new { tblTutor = tu , OnlineUser = ou}).ToList().Select(x=> new {
                        x.tblTutor.TutorID,
                        Name = x.tblTutor.TutorFirstName + " " + x.tblTutor.TutorLastName,
                        x.tblTutor.TutorEmail,
                        x.tblTutor.ContactNo1,
                        x.OnlineUser.IsBlocked,
                        x.OnlineUser.LoginID,
                        x.tblTutor.DownloadedPaperCount,
                        x.tblTutor.CompletedExamsCount,
                        JoinDate = x.OnlineUser.CreatedDate?.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture),
                        }).ToList();

                result = true;

                return Json(new { success = result, data = tutors }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult ApproveTutor(int TutorId)
        {
            var result = false;
            try
            {
                var tutuors = db.tblTutors.Where(x => x.TutorID == TutorId).FirstOrDefault();
                if (tutuors != null)
                {
                    tutuors.AdminApproval = "Approved";
                    db.SaveChanges();
                    result = true;
                }

                return Json(new { success = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult RejectTutor(int TutorId)
        {
            var result = false;
            try
            {
                var tutuors = db.tblTutors.Where(x => x.TutorID == TutorId).FirstOrDefault();
                if (tutuors != null)
                {
                    tutuors.AdminApproval = "Rejected";
                    db.SaveChanges();
                    result = true;
                }

                return Json(new { success = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetQualificationImages(int LoginId)
        {
            var result = false;
            try
            {
                List<SecondPaperImages> dataList = new List<SecondPaperImages>();
                var qualifi = db.TutorQualifications.Where(x => x.LoginId == LoginId).ToList(); ;
                foreach(var qua in qualifi)
                {
                    string fileName = qua.QualificationID.ToString();
                    string src = "/Tutordashboard/LoadQualificationImage?QualificationId=" + fileName ;
                    dataList.Add(new SecondPaperImages { FileName = fileName, Src = src });
                }

                result = true;

                return Json(new { success = result, dataList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult LoadQualificationImage(long QualificationId)
        {
            var path = db.TutorQualifications.Where(x => x.QualificationID == QualificationId).Select(x => x.ImagePath).FirstOrDefault();

            return File(path, "image/*");
        }

        public ActionResult Users()
        {
            return View();
        }

        //-------------------------------------------------------------Logged Details---------------------------------------------------------------------------------------------------------------------------------------------------
        public ActionResult LoggedDetails()
        {
            return View();
        }

        public JsonResult GetLoggedDetails(string FromDate , string ToDate)
        {
            var result = false;
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                DateTime fromDate = Convert.ToDateTime(FromDate);
                DateTime toDate = Convert.ToDateTime(ToDate);
                List<LoggedDetailModel> dataList = new List<LoggedDetailModel>();
                var loggedDetails = db.LoggedDetails.Where(x => x.LoginId != 0 && x.LoggedDate >= fromDate && x.LoggedDate <= toDate).GroupBy(x => new { x.LoginId});

                foreach(var detail in loggedDetails)
                {
                    long loginId = detail.Key.LoginId;
                    DateTime lastLoggedDate = db.LoggedDetails.Where(x => x.LoginId == loginId && x.LoggedDate >= fromDate && x.LoggedDate <= toDate).Max(x => x.LoggedDateTime);

                    var user = db.OnlineUsers.Where(x => x.LoginID == loginId).FirstOrDefault();
                    if(user != null)
                    {
                        if(user.UserRoleId == 2)//student
                        {
                            var student = db.tblOnlineStudentMasters.Where(x => x.LoginID == loginId).FirstOrDefault();
                            if(student != null)
                            {
                                dataList.Add(new LoggedDetailModel { Name = student.StudentFirstName + " " + student.StudentLastName, Email = user.UserName, LoggedDate = lastLoggedDate.ToString("MM/dd/yyyy h:mm tt", CultureInfo.InvariantCulture), UserType = user.UserRoleId.ToString() });
                            }
                        }else if(user.UserRoleId == 3)//tutor
                        {
                            var tutor = db.tblTutors.Where(x => x.LoginID == loginId).FirstOrDefault();
                            if(tutor != null)
                            {
                                dataList.Add(new LoggedDetailModel { Name = tutor.TutorFirstName + " " + tutor.TutorLastName, Email = user.UserName, LoggedDate = lastLoggedDate.ToString("MM/dd/yyyy h:mm tt", CultureInfo.InvariantCulture), UserType = user.UserRoleId.ToString()});
                            }
                        }
                    }
                }

                result = true;

                return Json(new { success = result, data = dataList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        //--------------------------------------------------------------Subject Master-----------------------------------------------------------------------------------------------------------------------------------
        public ActionResult Subjects()
        {
            dynamic myModal = new ExpandoObject();
            myModal.ExamTypes = GetExamTypesForCombo();
            return View(myModal);
        }

        public List<ExamType> GetExamTypesForCombo()
        {
            var typList = db.ExamTypes.ToList();
            return typList;
        }

        [HttpPost]
        public JsonResult SaveSubjects()
        {
            var result = false;
            string msg = "";
            try
            {
                int examTypeId = Convert.ToInt32(Request.Form["ExamTypeId"]);
                string subjectCode = Request.Form["SubjectCode"];
                string subjectName = Request.Form["SubjectName"];
                string loginId = Request.Form["LoginId"];
                bool isActive = Convert.ToBoolean(Request.Form["LoginId"]);
                string priceForPaper = Request.Form["PriceForPaper"];
                string priceForTutor = Request.Form["PriceForTutor"];
                //int intExamId = Convert.ToInt32(examId);

                var existsSubjects = db.Subjects.Where(x => x.SubjectName == subjectCode && x.ExamTypeId == examTypeId).FirstOrDefault();
                if (existsSubjects != null)
                {
                    msg = "Subject is already exists.";
                }
                else
                {
                    //string webConfigPath = ConfigurationManager.AppSettings["SubjectImagePath"];
                    //string path = @"" + webConfigPath;

                    //for (int i = 0; i < Request.Files.Count; i++)
                    //{
                    //    var file = Request.Files[i];
                    //    var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                    //    fileName = fileName + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(file.FileName);
                    //    string newPath = "";
                    //    newPath = path + fileName;
                    //    file.SaveAs(newPath);

                    //    var subject = new Subject();
                    //    subject.ExamTypeId = examTypeId;
                    //    subject.SubjectName = subjectCode;
                    //    subject.Description = subjectName;
                    //    subject.ImagePath = newPath;
                    //    subject.IsActive = isActive;
                    //    subject.CreatedDate = DateTime.Now;
                    //    subject.CreatedUser = loginId;
                    //    db.Subjects.Add(subject);
                    //    db.SaveChanges();
                    //}

                    var subject = new Subject();
                    subject.ExamTypeId = examTypeId;
                    subject.SubjectName = subjectCode;
                    subject.Description = subjectName;
                    subject.IsActive = isActive;
                    subject.Price = Convert.ToDecimal(priceForPaper);
                    subject.PriceForTutor = Convert.ToDecimal(priceForTutor);
                    subject.CreatedDate = DateTime.Now;
                    subject.CreatedUser = loginId;
                    db.Subjects.Add(subject);
                    db.SaveChanges();

                    result = true;
                }

                return Json(new { success = result, message = msg }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult GetSubjects()
        {
            var result = false;
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var subjects = db.Subjects.Join(
                        db.ExamTypes,
                        su => su.ExamTypeId,
                        et => et.ExamTypeID,
                        (su,et) => new { Subject = su , ExamType = et}).ToList().Select(x => new
                        {
                            x.Subject.SubjectID,
                            SubjectCode = x.Subject.SubjectName,
                            SubjectName = x.Subject.Description,
                            ExamType = x.ExamType.Description,
                            x.Subject.IsActive,
                            x.Subject.Price,
                            x.Subject.PriceForTutor
                        }).ToList();

                result = true;

                return Json(new { success = result, data = subjects }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetSubjectsForEdit(int SubjectId)
        {
            var result = false;
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var subjects = db.Subjects.Where(x=>x.SubjectID == SubjectId).ToList().Select(x => new
                        {
                            SubjectCode = x.SubjectName,
                            SubjectName = x.Description,
                            x.ExamTypeId,
                            x.IsActive,
                            x.SubjectID,
                            x.Price,
                            x.PriceForTutor
                        }).ToList();

                result = true;

                return Json(new { success = result, dataList = subjects }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult UpdateSubjects()
        {
            var result = false;
            string msg = "";
            try
            {
                int examTypeId = Convert.ToInt32(Request.Form["ExamTypeId"]);
                string subjectCode = Request.Form["SubjectCode"];
                string subjectName = Request.Form["SubjectName"];
                string loginId = Request.Form["LoginId"];
                bool isActive = Convert.ToBoolean(Request.Form["IsActive"]);
                int subjectId = Convert.ToInt32(Request.Form["SubjectId"]);
                string priceForPaper = Request.Form["PriceForPaper"];
                string priceForTutor = Request.Form["PriceForTutor"];
                //int intExamId = Convert.ToInt32(examId);

                var existsSubjects = db.Subjects.Where(x => x.SubjectName == subjectCode && x.ExamTypeId == examTypeId && x.SubjectID != subjectId).FirstOrDefault();
                if (existsSubjects != null)
                {
                    msg = "Subject is already exists.";
                }
                else
                {
                    var subject = db.Subjects.Where(x => x.SubjectID == subjectId).FirstOrDefault();
                    if (subject == null)
                    {
                        msg = "Can't modify. Record is already deleted";
                    }
                    else
                    {

                        //string webConfigPath = ConfigurationManager.AppSettings["SubjectImagePath"];
                        //string path = @"" + webConfigPath;

                        //for (int i = 0; i < Request.Files.Count; i++)
                        //{
                        //    if (System.IO.File.Exists(subject.ImagePath))
                        //    {
                        //        System.IO.File.Delete(subject.ImagePath);
                        //    }

                        //    var file = Request.Files[i];
                        //    var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                        //    fileName = fileName + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(file.FileName);
                        //    string newPath = "";
                        //    newPath = path + fileName;
                        //    file.SaveAs(newPath);

                        //    subject.ExamTypeId = examTypeId;
                        //    subject.SubjectName = subjectCode;
                        //    subject.Description = subjectName;
                        //    subject.ImagePath = newPath;
                        //    subject.IsActive = isActive;
                        //    subject.UpdatedDate = DateTime.Now;
                        //    subject.UpdatedUser = loginId;
                        //    db.SaveChanges();
                        //}

                        subject.ExamTypeId = examTypeId;
                        subject.SubjectName = subjectCode;
                        subject.Description = subjectName;
                        subject.IsActive = isActive;
                        subject.Price = Convert.ToDecimal(priceForPaper);
                        subject.PriceForTutor = Convert.ToDecimal(priceForTutor);
                        subject.UpdatedDate = DateTime.Now;
                        subject.UpdatedUser = loginId;
                        db.SaveChanges();

                        result = true;
                    }
                }

                return Json(new { success = result, message = msg }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        //------------------------------------------------------------------Subject Images--------------------------------------------------------------------------------------------------------------------------
        public ActionResult SubjectImages()
        {
            dynamic myModal = new ExpandoObject();
            myModal.ExamTypes = GetExamTypesForCombo();
            myModal.Mediums = GetMediumsForCombo();
            return View(myModal);
        }

        //-------------------------------------------------------------------Paper Master------------------------------------------------------------------------------------------------------------
        public ActionResult Papers()
        {
            dynamic myModal = new ExpandoObject();
            myModal.ExamTypes = GetExamTypesForCombo();
            myModal.Mediums = GetMediumsForCombo();
            myModal.Subjects = GetSubjectsForCombo();
            return View(myModal);
        }

        [HttpPost]
        public JsonResult SavePastPaper()
        {
            var result = false;
            try
            {
                if (Request.Files.Count > 0)
                {
                    string examTypeId = Request.Form["ExamTypeId"];
                    string mediumId = Request.Form["MediumId"];
                    string subjectId = Request.Form["SubjectId"];
                    string paperName = Request.Form["PaperName"];
                    string year = Request.Form["Year"];
                    int firstPaperTime = Convert.ToInt32((Request.Form["FirstPaperTime"] != "" && Request.Form["FirstPaperTime"] != null) ? Request.Form["FirstPaperTime"] : "0");
                    int secondPaperTime = Convert.ToInt32((Request.Form["SecondPaperTime"] != "" && Request.Form["SecondPaperTime"] != null) ? Request.Form["SecondPaperTime"] : "0");
                    int thirdPaperTime = Convert.ToInt32((Request.Form["ThirdPaperTime"] != "" && Request.Form["ThirdPaperTime"] != null) ? Request.Form["ThirdPaperTime"] : "0");
                    bool haveMcq = Convert.ToBoolean(Request.Form["HaveMcq"]);
                    bool hasMap = Convert.ToBoolean(Request.Form["HasMap"]);
                    bool isOnlineExam = Convert.ToBoolean(Request.Form["IsOnlineExam"]);
                    bool hasThirdPaper = Convert.ToBoolean(Request.Form["HasThirdPaper"]);
                    string medium = "";
                    int examTypeIdInt = Convert.ToInt32(examTypeId);
                    int subjectIdInt = Convert.ToInt32(subjectId);
                    int mediumIdInt = Convert.ToInt32(mediumId);
                    int yearInt = Convert.ToInt32(year);

                    var checkPaper = db.PastPapers.Where(x => x.ExamTypeID == examTypeIdInt && x.MediumID == mediumIdInt && x.SubjectID == subjectIdInt && x.Year == yearInt).FirstOrDefault();
                    if(checkPaper == null)
                    {
                        if (mediumId == "1")
                        {
                            medium = "Sinhala";
                        }
                        else if (mediumId == "2")
                        {
                            medium = "English";
                        }
                        else if (mediumId == "3")
                        {
                            medium = "Tamil";
                        }

                        var paper = new PastPaper();
                        paper.ExamTypeID = examTypeIdInt;
                        paper.MediumID = mediumIdInt;
                        paper.SubjectID = subjectIdInt;
                        paper.PaperName = paperName;
                        paper.Year = yearInt;
                        paper.FirstPaperTime = firstPaperTime;
                        paper.SecondPaperTime = secondPaperTime;
                        paper.ThirdPaperTime = thirdPaperTime;
                        paper.HasThirdPaper = hasThirdPaper;
                        paper.HasMcq = haveMcq;
                        paper.HasMap = hasMap;
                        paper.IsOnlineExam = isOnlineExam;
                        paper.IsActive = true;

                        //Save paper preview image
                        var previewImagePath = ConfigurationManager.AppSettings["PastPaperPreviewImagePath"];
                        var previewImageFile = Request.Files["PreviewImage"];
                        if(previewImageFile != null)
                        {
                            previewImagePath = previewImagePath + medium + "\\" + subjectId;
                            if (!(Directory.Exists(previewImagePath)))
                            {
                                Directory.CreateDirectory(previewImagePath);
                            }
                            previewImagePath = previewImagePath + "\\" + year + Path.GetExtension(previewImageFile.FileName);
                            previewImageFile.SaveAs(previewImagePath);
                            paper.PaperPreviewPath = previewImagePath;
                        }
                        
                        //Save paper pdf
                        var pastPaperPdfPath = ConfigurationManager.AppSettings["PastPaperPdfPath"];
                        var paperPdfFile = Request.Files["PaperPdf"];
                        if(paperPdfFile != null)
                        {
                            pastPaperPdfPath = pastPaperPdfPath + medium + "\\" + subjectId;
                            if (!(Directory.Exists(pastPaperPdfPath)))
                            {
                                Directory.CreateDirectory(pastPaperPdfPath);
                            }
                            if (!(Directory.Exists(pastPaperPdfPath + "\\" + year)))
                            {
                                Directory.CreateDirectory(pastPaperPdfPath + "\\" + year);
                            }
                            string pastPaperPdfPathToDb = pastPaperPdfPath + "\\" + year;
                            pastPaperPdfPath = pastPaperPdfPath + "\\" + year + "\\" + year + Path.GetExtension(paperPdfFile.FileName);
                            paperPdfFile.SaveAs(pastPaperPdfPath);
                            paper.PaperDownloadPath = pastPaperPdfPathToDb;
                        }
                        
                        //Save marking scheme pdf
                        var markingSchemePath = ConfigurationManager.AppSettings["MarkingSchemePath"];
                        var markingSchemePdfFile = Request.Files["MarkingScheme"];
                        if(markingSchemePdfFile != null)
                        {
                            markingSchemePath = markingSchemePath + medium + "\\" + subjectId;
                            if (!(Directory.Exists(markingSchemePath)))
                            {
                                Directory.CreateDirectory(markingSchemePath);
                            }
                            if (!(Directory.Exists(markingSchemePath + "\\" + year)))
                            {
                                Directory.CreateDirectory(markingSchemePath + "\\" + year);
                            }
                            markingSchemePath = markingSchemePath + "\\" + year + "\\" + year + Path.GetExtension(markingSchemePdfFile.FileName);
                            markingSchemePdfFile.SaveAs(markingSchemePath);
                            paper.MarkingSchemePath = markingSchemePath;
                        }
                        
                        //Save map image
                        var mapImagePath = ConfigurationManager.AppSettings["MapImagePath"];
                        var mapImageFile = Request.Files["MapImage"];
                        if(mapImageFile != null)
                        {
                            mapImagePath = mapImagePath + medium;
                            if (!(Directory.Exists(mapImagePath)))
                            {
                                Directory.CreateDirectory(mapImagePath);
                            }
                            mapImagePath = mapImagePath + "\\" + year + Path.GetExtension(mapImageFile.FileName);
                            mapImageFile.SaveAs(mapImagePath);
                            paper.MapImagePath = mapImagePath;
                        }

                        db.PastPapers.Add(paper);
                        db.SaveChanges();
                        result = true;
                        return Json(new { success = result }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = result, message = "Already has a past paper to selected ExamTypeId,MediumId & SubjectId." }, JsonRequestBehavior.AllowGet);
                    }
                }

                return Json(new { success = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult GetPastPapers()
        {
            var result = false;
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var paperList = db.PastPapers.Join(
                        db.ExamTypes,
                        pp => pp.ExamTypeID,
                        et => et.ExamTypeID,
                        (pp, et) => new { PastPaper = pp, ExamType = et }).Join(
                        db.Mediums,
                        pp => pp.PastPaper.MediumID,
                        me => me.MediumID,
                        (pp, me) => new { pp.ExamType, pp.PastPaper, Medium = me }).Join(
                        db.Subjects,
                        pp => pp.PastPaper.SubjectID,
                        su => su.SubjectID,
                        (pp, su) => new { pp.ExamType, pp.PastPaper, pp.Medium, Subject = su }).OrderBy(x=> x.PastPaper.SubjectID).ToList().Select(x => new
                        {
                            x.PastPaper.PaperName,
                            ExamType = x.ExamType.Description,
                            Medium = x.Medium.Description,
                            Subject = x.Subject.Description,
                            x.PastPaper.Year,
                            x.PastPaper.FirstPaperTime,
                            x.PastPaper.SecondPaperTime,
                            x.PastPaper.HasMcq,
                            x.PastPaper.HasMap,
                            x.PastPaper.IsOnlineExam,
                            x.PastPaper.IsActive,
                            x.PastPaper.PastPaerID,
                            x.PastPaper.HasThirdPaper,
                            x.PastPaper.ThirdPaperTime
                        }).ToList();

                result = true;

                return Json(new { success = result, data = paperList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetPastPapersToEdit(int PaperId)
        {
            var result = false;
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var paperList = db.PastPapers.Where(x=>x.PastPaerID == PaperId).Select(x => new
                {
                    x.PaperName,
                    x.ExamTypeID,
                    x.MediumID,
                    x.SubjectID,
                    x.Year,
                    x.FirstPaperTime,
                    x.SecondPaperTime,
                    x.ThirdPaperTime,
                    x.HasMcq,
                    x.HasMap,
                    x.IsOnlineExam,
                    x.IsActive,
                    x.HasThirdPaper
                }).ToList();

                result = true;

                return Json(new { success = result, dataList = paperList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult UpdatePastPaper()
        {
            var result = false;
            try
            {
                string examTypeId = Request.Form["ExamTypeId"];
                string mediumId = Request.Form["MediumId"];
                string subjectId = Request.Form["SubjectId"];
                string paperName = Request.Form["PaperName"];
                string year = Request.Form["Year"];
                int firstPaperTime = Convert.ToInt32((Request.Form["FirstPaperTime"] != "" && Request.Form["FirstPaperTime"] != null) ? Request.Form["FirstPaperTime"] : "0");
                int secondPaperTime = Convert.ToInt32((Request.Form["SecondPaperTime"] != "" && Request.Form["SecondPaperTime"] != null) ? Request.Form["SecondPaperTime"] : "0");
                int thirdPaperTime = Convert.ToInt32((Request.Form["ThirdPaperTime"] != "" && Request.Form["ThirdPaperTime"] != null) ? Request.Form["ThirdPaperTime"] : "0");
                bool haveMcq = Convert.ToBoolean(Request.Form["HaveMcq"]);
                bool hasMap = Convert.ToBoolean(Request.Form["HasMap"]);
                bool isOnlineExam = Convert.ToBoolean(Request.Form["IsOnlineExam"]);
                bool hasThirdPaper = Convert.ToBoolean(Request.Form["HasThirdPaper"]);
                bool isActive = Convert.ToBoolean(Request.Form["IsActive"]);
                string medium = "";
                int examTypeIdInt = Convert.ToInt32(examTypeId);
                int subjectIdInt = Convert.ToInt32(subjectId);
                int mediumIdInt = Convert.ToInt32(mediumId);
                int yearInt = Convert.ToInt32(year);
                int paperId = Convert.ToInt32(Request.Form["PaperId"]);

                var checkPaper = db.PastPapers.Where(x => x.ExamTypeID == examTypeIdInt && x.MediumID == mediumIdInt && x.SubjectID == subjectIdInt && x.Year == yearInt && x.PastPaerID != paperId).FirstOrDefault();
                if (checkPaper == null)
                {
                    if (mediumId == "1")
                    {
                        medium = "Sinhala";
                    }
                    else if (mediumId == "2")
                    {
                        medium = "English";
                    }
                    else if (mediumId == "3")
                    {
                        medium = "Tamil";
                    }

                    var paper = db.PastPapers.Where(x=>x.PastPaerID == paperId).FirstOrDefault();
                    paper.ExamTypeID = examTypeIdInt;
                    paper.MediumID = mediumIdInt;
                    paper.SubjectID = subjectIdInt;
                    paper.PaperName = paperName;
                    paper.Year = yearInt;
                    paper.FirstPaperTime = firstPaperTime;
                    paper.SecondPaperTime = secondPaperTime;
                    paper.ThirdPaperTime = thirdPaperTime;
                    paper.HasThirdPaper = hasThirdPaper;
                    paper.HasMcq = haveMcq;
                    paper.HasMap = hasMap;
                    paper.IsOnlineExam = isOnlineExam;
                    paper.IsActive = isActive;

                    if (Request.Files.Count > 0)
                    {
                        //Save paper preview image
                        var previewImagePath = ConfigurationManager.AppSettings["PastPaperPreviewImagePath"];
                        var previewImageFile = Request.Files["PreviewImage"];
                        if (previewImageFile != null)
                        {
                            previewImagePath = previewImagePath + medium + "\\" + subjectId;
                            if (!(Directory.Exists(previewImagePath)))
                            {
                                Directory.CreateDirectory(previewImagePath);
                            }
                            previewImagePath = previewImagePath + "\\" + year + Path.GetExtension(previewImageFile.FileName);
                            if (System.IO.File.Exists(previewImagePath))
                            {
                                System.IO.File.Delete(previewImagePath);
                            }
                            previewImageFile.SaveAs(previewImagePath);
                            paper.PaperPreviewPath = previewImagePath;
                        }

                        //Save paper pdf
                        var pastPaperPdfPath = ConfigurationManager.AppSettings["PastPaperPdfPath"];
                        var paperPdfFile = Request.Files["PaperPdf"];
                        if (paperPdfFile != null)
                        {
                            pastPaperPdfPath = pastPaperPdfPath + medium + "\\" + subjectId;
                            if (!(Directory.Exists(pastPaperPdfPath)))
                            {
                                Directory.CreateDirectory(pastPaperPdfPath);
                            }
                            if (!(Directory.Exists(pastPaperPdfPath + "\\" + year)))
                            {
                                Directory.CreateDirectory(pastPaperPdfPath + "\\" + year);
                            }
                            string pastPaperPdfPathToDb = pastPaperPdfPath + "\\" + year;
                            pastPaperPdfPath = pastPaperPdfPath + "\\" + year + "\\" + year + Path.GetExtension(paperPdfFile.FileName);
                            if (System.IO.File.Exists(pastPaperPdfPath))
                            {
                                System.IO.File.Delete(pastPaperPdfPath);
                            }
                            paperPdfFile.SaveAs(pastPaperPdfPath);
                            paper.PaperDownloadPath = pastPaperPdfPathToDb;
                        }

                        //Save marking scheme pdf
                        var markingSchemePath = ConfigurationManager.AppSettings["MarkingSchemePath"];
                        var markingSchemePdfFile = Request.Files["MarkingScheme"];
                        if (markingSchemePdfFile != null)
                        {
                            markingSchemePath = markingSchemePath + medium + "\\" + subjectId;
                            if (!(Directory.Exists(markingSchemePath)))
                            {
                                Directory.CreateDirectory(markingSchemePath);
                            }
                            if (!(Directory.Exists(markingSchemePath + "\\" + year)))
                            {
                                Directory.CreateDirectory(markingSchemePath + "\\" + year);
                            }
                            markingSchemePath = markingSchemePath + "\\" + year + "\\" + year + Path.GetExtension(markingSchemePdfFile.FileName);
                            if (System.IO.File.Exists(markingSchemePath))
                            {
                                System.IO.File.Delete(markingSchemePath);
                            }
                            markingSchemePdfFile.SaveAs(markingSchemePath);
                            paper.MarkingSchemePath = markingSchemePath;
                        }

                        //Save map image
                        var mapImagePath = ConfigurationManager.AppSettings["MapImagePath"];
                        var mapImageFile = Request.Files["MapImage"];
                        if (mapImageFile != null)
                        {
                            mapImagePath = mapImagePath + medium;
                            if (!(Directory.Exists(mapImagePath)))
                            {
                                Directory.CreateDirectory(mapImagePath);
                            }
                            mapImagePath = mapImagePath + "\\" + year + Path.GetExtension(mapImageFile.FileName);
                            if (System.IO.File.Exists(mapImagePath))
                            {
                                System.IO.File.Delete(mapImagePath);
                            }
                            mapImageFile.SaveAs(mapImagePath);
                            paper.MapImagePath = mapImagePath;
                        }
                    }
                    
                    db.SaveChanges();
                    result = true;
                    return Json(new { success = result }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = result, message = "Already has a past paper to selected ExamTypeId,MediumId & SubjectId." }, JsonRequestBehavior.AllowGet);
                }

                
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public FileResult DispalyPaperPdf(int PaperId)
        {

            try
            {
                var paper = db.PastPapers.Where(x => x.PastPaerID == PaperId).Select(x => new { x.PaperDownloadPath, x.Year }).SingleOrDefault();
                string fileLocation = paper.PaperDownloadPath + "\\" + paper.Year + ".pdf";
                byte[] fileBytes = System.IO.File.ReadAllBytes(fileLocation);
                return File(fileBytes, "application/pdf");

            }
            catch (Exception ex)
            {
                return File("", "image/*");
            }

        }

        public FileResult DispalyMarkingScheme(int PaperId)
        {

            try
            {
                string fileLocation = db.PastPapers.Where(x => x.PastPaerID == PaperId).Select(x => x.MarkingSchemePath).SingleOrDefault();
                byte[] fileBytes = System.IO.File.ReadAllBytes(fileLocation);
                return File(fileBytes, "application/pdf");

            }
            catch (Exception ex)
            {
                return File("", "application/pdf");
            }

        }

        public FileResult DispalyPreviewImage(int PaperId)
        {

            try
            {
                string fileLocation = db.PastPapers.Where(x => x.PastPaerID == PaperId).Select(x => x.PaperPreviewPath).SingleOrDefault();
                byte[] fileBytes = System.IO.File.ReadAllBytes(fileLocation);
                return File(fileBytes, "image/*");
            }
            catch (Exception ex)
            {
                return File("", "image/*");
            }

        }

        public FileResult DispalyMapImage(int PaperId)
        {

            try
            {
                string fileLocation = db.PastPapers.Where(x => x.PastPaerID == PaperId).Select(x => x.MapImagePath).SingleOrDefault();
                byte[] fileBytes = System.IO.File.ReadAllBytes(fileLocation);
                return File(fileBytes, "image/*");
            }
            catch (Exception ex)
            {
                return File("", "image/*");
            }

        }

        //-------------------------------------------------------------------Exam Flow--------------------------------------------------------------------------------------------------------------
        public ActionResult ExamFlow()
        {
            return View();
        }

        public JsonResult GetAllNotAssignedExamsForTutor()
        {
            var result = false;
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var penExams = db.Exams.Where(x => x.AssignToTutor == "Pending").Join(
                        db.ExamTypes,
                        ex => ex.ExamTypeId,
                        ext => ext.ExamTypeID,
                        (ex, ext) => new { Exam = ex, ExamType = ext }).Join(
                        db.Mediums,
                        ex => ex.Exam.MediumId,
                        me => me.MediumID,
                        (ex, me) => new { ex.Exam, ex.ExamType, Medium = me }).Join(
                        db.Subjects,
                        ex => ex.Exam.SubjectId,
                        su => su.SubjectID,
                        (ex, su) => new { ex.Exam, ex.ExamType, ex.Medium, Subject = su }).Join(
                        db.tblOnlineStudentMasters,
                        ex => ex.Exam.StudentLoginId,
                        stu => stu.LoginID,
                        (ex, stu) => new { ex.Exam, ex.ExamType, ex.Medium, ex.Subject, tblOnlineStudentMaster = stu }).Join(
                        db.PastPapers,
                        ex => ex.Exam.PastPaperId,
                        pp => pp.PastPaerID,
                        (ex, pp) => new { ex.Exam, ex.ExamType, ex.Medium, ex.Subject, ex.tblOnlineStudentMaster, PastPaper = pp }).ToList().Select(x => new
                        {
                            x.Exam.ExamID,
                            x.Exam.ExamNo,
                            StudentName = x.tblOnlineStudentMaster.StudentFirstName + " " + x.tblOnlineStudentMaster.StudentLastName,
                            ExamType = x.ExamType.ExamTypeCode,
                            Medium = x.Medium.Description,
                            Subject = x.Subject.Description,
                            x.PastPaper.Year,
                            CompleteDate = x.Exam.AssignToTutorReadyDate?.ToString("MM/dd/yyyy"),
                            x.Exam.AssignToTutorReadyDate
                        });

                result = true;

                return Json(new { success = result, data = penExams }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetAllApprovedExamsByTutors()
        {
            var result = false;
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var penExams = db.Exams.Where(x => x.AssignToTutor == "Completed" && x.TutorStatus == "Pending").Join(
                        db.ExamTypes,
                        ex => ex.ExamTypeId,
                        ext => ext.ExamTypeID,
                        (ex, ext) => new { Exam = ex, ExamType = ext }).Join(
                        db.Mediums,
                        ex => ex.Exam.MediumId,
                        me => me.MediumID,
                        (ex, me) => new { ex.Exam, ex.ExamType, Medium = me }).Join(
                        db.Subjects,
                        ex => ex.Exam.SubjectId,
                        su => su.SubjectID,
                        (ex, su) => new { ex.Exam, ex.ExamType, ex.Medium, Subject = su }).Join(
                        db.tblOnlineStudentMasters,
                        ex => ex.Exam.StudentLoginId,
                        stu => stu.LoginID,
                        (ex, stu) => new { ex.Exam, ex.ExamType, ex.Medium, ex.Subject, tblOnlineStudentMaster = stu }).Join(
                        db.PastPapers,
                        ex => ex.Exam.PastPaperId,
                        pp => pp.PastPaerID,
                        (ex, pp) => new { ex.Exam, ex.ExamType, ex.Medium, ex.Subject, ex.tblOnlineStudentMaster, PastPaper = pp }).Join(
                        db.tblTutors,
                        ex => ex.Exam.Tutor1LoginId,
                        tu => tu.LoginID,
                        (ex,tu) => new { ex.Exam, ex.ExamType, ex.Medium, ex.Subject, ex.tblOnlineStudentMaster, ex.PastPaper, tblTutor = tu }).ToList().Select(x => new
                        {
                            x.Exam.ExamNo,
                            x.Exam.ExamID,
                            StudentName = x.tblOnlineStudentMaster.StudentFirstName + " " + x.tblOnlineStudentMaster.StudentLastName,
                            TutorName = x.tblTutor.TutorFirstName + " " + x.tblTutor.TutorLastName,
                            ExamType = x.ExamType.ExamTypeCode,
                            Medium = x.Medium.Description,
                            Subject = x.Subject.Description,
                            x.PastPaper.Year,
                            ApprovedDate = x.Exam.TutorApprovedDate?.ToString("MM/dd/yyyy"),
                            Deadline = x.Exam.TutorDeadlineDate?.ToString("MM/dd/yyyy"),
                            Paper1ExamId = (x.PastPaper.HasMcq == true ? "" :
                           x.PastPaper.HasMcq == false ? x.Exam.ExamID.ToString() : ""),
                            CompleteDate = x.Exam.AssignToTutorReadyDate?.ToString("MM/dd/yyyy")
                        });

                result = true;

                return Json(new { success = result, data = penExams }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetAllCompletedExamsByTutors()
        {
            var result = false;
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var penExams = db.Exams.Where(x => x.AssignToTutor == "Completed" && x.TutorStatus == "Completed").Join(
                        db.ExamTypes,
                        ex => ex.ExamTypeId,
                        ext => ext.ExamTypeID,
                        (ex, ext) => new { Exam = ex, ExamType = ext }).Join(
                        db.Mediums,
                        ex => ex.Exam.MediumId,
                        me => me.MediumID,
                        (ex, me) => new { ex.Exam, ex.ExamType, Medium = me }).Join(
                        db.Subjects,
                        ex => ex.Exam.SubjectId,
                        su => su.SubjectID,
                        (ex, su) => new { ex.Exam, ex.ExamType, ex.Medium, Subject = su }).Join(
                        db.tblOnlineStudentMasters,
                        ex => ex.Exam.StudentLoginId,
                        stu => stu.LoginID,
                        (ex, stu) => new { ex.Exam, ex.ExamType, ex.Medium, ex.Subject, tblOnlineStudentMaster = stu }).Join(
                        db.PastPapers,
                        ex => ex.Exam.PastPaperId,
                        pp => pp.PastPaerID,
                        (ex, pp) => new { ex.Exam, ex.ExamType, ex.Medium, ex.Subject, ex.tblOnlineStudentMaster, PastPaper = pp }).Join(
                        db.tblTutors,
                        ex => ex.Exam.Tutor1LoginId,
                        tu => tu.LoginID,
                        (ex, tu) => new { ex.Exam, ex.ExamType, ex.Medium, ex.Subject, ex.tblOnlineStudentMaster, ex.PastPaper, tblTutor = tu }).ToList().Select(x => new
                        {
                            x.Exam.ExamNo,
                            x.Exam.ExamID,
                            StudentName = x.tblOnlineStudentMaster.StudentFirstName + " " + x.tblOnlineStudentMaster.StudentLastName,
                            TutorName = x.tblTutor.TutorFirstName + " " + x.tblTutor.TutorLastName,
                            ExamType = x.ExamType.ExamTypeCode,
                            Medium = x.Medium.Description,
                            Subject = x.Subject.Description,
                            x.PastPaper.Year,
                            ApprovedDate = x.Exam.TutorApprovedDate?.ToString("MM/dd/yyyy"),
                            Deadline = x.Exam.TutorDeadlineDate?.ToString("MM/dd/yyyy"),
                            Paper1ExamId = (x.PastPaper.HasMcq == true ? "" :
                            x.PastPaper.HasMcq == false ? x.Exam.ExamID.ToString() : ""),
                            StudentCompleteDate = x.Exam.AssignToTutorReadyDate?.ToString("MM/dd/yyyy"),
                            TutorCompleteDate = x.Exam.TutorCompletedDate?.ToString("MM/dd/yyyy")
                        });

                result = true;

                return Json(new { success = result, data = penExams }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetTutorsForPopup(int ExamId)
        {
            var result = false;
            
            try
            {
                db.Configuration.ProxyCreationEnabled = false;

                var exam = db.Exams.Where(x => x.ExamID == ExamId).FirstOrDefault();
                if (exam != null)
                {
                    var tutors = db.tblTutors.Join(
                            db.PreferedTutorExamTypes.Where(x => x.ExamTypeId == exam.ExamTypeId),
                            tu => tu.LoginID,
                            pet => pet.LoginId,
                            (tu, pet) => new { tblTutor = tu, PreferedTutorExamType = pet }).Join(
                            db.PreferedTutorMediums.Where(x => x.MediumId == exam.MediumId),
                            tu => tu.tblTutor.LoginID,
                            pme => pme.LoginId,
                            (tu, pme) => new { tu.tblTutor, tu.PreferedTutorExamType, PreferedTutorMedium = pme }).Join(
                            db.PreferedTutorSubjects.Where(x => x.SubjectId == exam.SubjectId),
                            tu => tu.tblTutor.LoginID,
                            psu => psu.LoginId,
                            (tu, psu) => new { tu.tblTutor, tu.PreferedTutorExamType, tu.PreferedTutorMedium, PreferedTutorSubject = psu }).ToList().Select(x => new
                            {
                                TuterName = x.tblTutor.TutorEmail + " - " + x.tblTutor.TutorFirstName + " " + x.tblTutor.TutorLastName,
                                x.tblTutor.LoginID
                            }).ToList();

                    result = true;

                    return Json(new { success = result, dataList = tutors }, JsonRequestBehavior.AllowGet);
                }


                return Json(new { success = result, message = "" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult AssignTutor(int ExamId , int TutorLoginId , int LoginId , string Reason)
        {
            var result = false;

            try
            {
                var exam = db.Exams.Where(x => x.ExamID == ExamId).FirstOrDefault();
                if (exam != null)
                {
                    exam.Tutor1LoginId = TutorLoginId;
                    exam.AssignToTutor = "Completed";
                    exam.TutorApprovedDate = DateTime.Now;
                    exam.TutorDeadlineDate = exam.AssignToTutorReadyDate?.AddDays(3);
                    exam.TutorStatus = "Pending";
                    exam.IsAdminAssignToTutor = true;
                    exam.AdminAssignToTutorReason = Reason;
                    exam.UpdateDate = DateTime.Now;
                    exam.UpdateUser = LoginId.ToString();
                    db.SaveChanges();

                    //Create notification
                    var subject = db.Exams.Where(x => x.ExamID == ExamId).Join(
                            db.Subjects,
                            ex => ex.SubjectId,
                            su => su.SubjectID,
                            (ex, su) => new { Exam = ex, Subject = su }).Select(x => x.Subject.Description).FirstOrDefault();

                    var notification = new Notification();
                    notification.NotificationHeader = subject + " Paper Assigned By Admin.";
                    notification.NotificationDetail = "You can view the paper by clicking this notification.";
                    notification.CreatedUser = "System";
                    notification.CreatedDate = DateTime.Now;
                    db.Notifications.Add(notification);
                    db.SaveChanges();

                    //Assign notification
                    var notiAssign = new NotificationAssign();
                    notiAssign.NotificationId = notification.NotificationID;
                    notiAssign.LoginId = TutorLoginId;
                    notiAssign.ControllerName = "TutorDashboard";
                    notiAssign.MethodName = "ExamApprovals";
                    notiAssign.IsExpired = false;
                    notiAssign.CreatedUser = "System";
                    notiAssign.CreatedDate = DateTime.Now;
                    db.NotificationAssigns.Add(notiAssign);
                    exam.NotificationId = notification.NotificationID;
                    db.SaveChanges();

                    var tutor = db.tblTutors.Where(x => x.LoginID == TutorLoginId).FirstOrDefault();
                    if(tutor != null)
                    {
                        tutor.PendingExamsCount = tutor.PendingExamsCount + 1;
                        db.SaveChanges();
                    }

                    result = true;
                }


                return Json(new { success = result, message = "" }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        //-------------------------------------------------------------Students Payments----------------------------------------------------------------------------------------------------
        public ActionResult Payments()
        {
            return View();
        }

        public JsonResult GetStudentPayments(string FromDate , string ToDate)
        {
            var result = false;
            try
            {
                DateTime fromDate = Convert.ToDateTime(FromDate);
                DateTime toDate = DateTime.Parse(Convert.ToDateTime(ToDate).ToShortDateString() + " 11:59 PM");

                var payments = db.Carts.Where(x => x.IsCheckOut == true && x.PaymentDate >= fromDate && x.PaymentDate <= toDate).Join(
                        db.CartItems,
                        ca => ca.CartId,
                        ci => ci.CartId,
                        (ca, ci) => new { Cart = ca, CartItem = ci }).Join(
                        db.tblOnlineStudentMasters,
                        ca => ca.Cart.LoginId,
                        stu => (long)stu.LoginID,
                        (ca,stu) => new { ca.Cart , ca.CartItem , tblOnlineStudentMaster = stu}).ToList().Select(x => new
                        {
                            OrderNo = "ORD" + x.Cart.CartId,
                            PaymentDate = x.Cart.PaymentDate?.ToString("MM/dd/yyyy"),
                            x.CartItem.ItemName,
                            UnitPrice = "Rs. " + x.CartItem.UnitPrice,
                            StudentName = x.tblOnlineStudentMaster.StudentFirstName+" "+x.tblOnlineStudentMaster.StudentLastName,
                            Price = x.CartItem.UnitPrice,
                            x.tblOnlineStudentMaster.Email
                        });

                return Json(new { success = result, data = payments }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        //-------------------------------------------------------------------------Lecture Payments--------------------------------------------------------------------------------------------------------------------
        public ActionResult LecturePayments()
        {
            return View();
        }

        public JsonResult GetPendingLecturePayments(string FromDate, string ToDate)
        {
            var result = false;
            try
            {
                DateTime fromDate = Convert.ToDateTime(FromDate);
                DateTime toDate = DateTime.Parse(Convert.ToDateTime(FromDate).ToShortDateString() + "11:59 PM");

                db.Configuration.ProxyCreationEnabled = false;
                var walletHistory = db.Exams.Where(x => x.IsPaidForTutor == false && x.TutorStatus == "Completed" && x.TutorCompletedDate >= fromDate  && x.TutorCompletedDate <= toDate).Join(
                        db.PastPapers,
                        ex => ex.PastPaperId,
                        pp => pp.PastPaerID,
                        (ex, pp) => new { Exam = ex, PastPaper = pp }).Join(
                        db.Subjects,
                        pp => pp.PastPaper.SubjectID,
                        su => su.SubjectID,
                        (pp, su) => new { pp.PastPaper, pp.Exam, Subject = su }).Join(
                        db.tblTutors,
                        ex => ex.Exam.Tutor1LoginId,
                        tu => tu.LoginID,
                        (ex,tu) => new { ex.PastPaper, ex.Exam, ex.Subject, tblTutor = tu }).ToList().Select(x => new
                        {
                            x.Exam.ExamNo,
                            x.Exam.ExamID,
                            PaperName = (x.PastPaper.ExamTypeID == 1 ? "O/L-" + x.Subject.Description + "-" + x.PastPaper.Year :
                             x.PastPaper.ExamTypeID == 2 ? "A/L" + x.Subject.Description + "-" + x.PastPaper.Year : ""),
                            TutorCompletedDate = x.Exam.TutorCompletedDate?.ToString("MM/dd/yyyy"),
                            PriceForTutor = "Rs. " + x.Exam.PriceForTutor,
                            Price = x.Exam.PriceForTutor,
                            TutorName = x.tblTutor.TutorFirstName + " " + x.tblTutor.TutorLastName,
                            x.tblTutor.TutorEmail
                        });

                result = true;

                return Json(new { success = result, data = walletHistory }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetPaidLecturePayments(string FromDate, string ToDate)
        {
            var result = false;
            try
            {
                DateTime fromDate = Convert.ToDateTime(FromDate);
                DateTime toDate = DateTime.Parse(Convert.ToDateTime(FromDate).ToShortDateString() + "11:59 PM");

                db.Configuration.ProxyCreationEnabled = false;
                var walletHistory = db.Exams.Where(x => x.IsPaidForTutor == true && x.TutorStatus == "Completed" && x.TutorCompletedDate >= fromDate && x.TutorCompletedDate <= toDate).Join(
                        db.PastPapers,
                        ex => ex.PastPaperId,
                        pp => pp.PastPaerID,
                        (ex, pp) => new { Exam = ex, PastPaper = pp }).Join(
                        db.Subjects,
                        pp => pp.PastPaper.SubjectID,
                        su => su.SubjectID,
                        (pp, su) => new { pp.PastPaper, pp.Exam, Subject = su }).Join(
                        db.tblTutors,
                        ex => ex.Exam.Tutor1LoginId,
                        tu => tu.LoginID,
                        (ex, tu) => new { ex.PastPaper, ex.Exam, ex.Subject, tblTutor = tu }).ToList().Select(x => new
                        {
                            x.Exam.ExamNo,
                           PaperName = (x.PastPaper.ExamTypeID == 1 ? "O/L-" + x.Subject.Description + "-" + x.PastPaper.Year :
                              x.PastPaper.ExamTypeID == 2 ? "A/L" + x.Subject.Description + "-" + x.PastPaper.Year : ""),
                            TutorCompletedDate = x.Exam.TutorCompletedDate?.ToString("MM/dd/yyyy"),
                            PriceForTutor = "Rs. " + x.Exam.PriceForTutor,
                            Price = x.Exam.PriceForTutor,
                            TutorName = x.tblTutor.TutorFirstName + " " + x.tblTutor.TutorLastName,
                            x.tblTutor.TutorEmail,
                            PaidDateForTutor = x.Exam.PaidDateForTutor?.ToString("MM/dd/yyyy")
                        });

                result = true;

                return Json(new { success = result, data = walletHistory }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult SubmitPaid(int ExamId, long LoginId)
        {
            var result = false;
            try
            {
                var exam = db.Exams.Where(x => x.ExamID == ExamId).FirstOrDefault();
                if (exam != null)
                {
                    exam.IsPaidForTutor = true;
                    exam.PaidDateForTutor = DateTime.Now;
                    exam.UpdateDate = DateTime.Now;
                    exam.UpdateUser = LoginId.ToString();
                    db.SaveChanges();
                }

                result = true;

                return Json(new { success = result}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        //-----------------------------------------------------------------------Change Admin Password---------------------------------------------------------------------------------------------------------------------------------------------------
        public ActionResult ChangePassword()
        {
            return View();
        }

        public JsonResult GetAdminUserName()
        {
            var result = false;
            try
            {
                var userName = db.OnlineUsers.Where(x => x.UserRoleId == 1).Select(x=>x.UserName).FirstOrDefault();
                result = true;

                return Json(new { success = result, UserName = userName }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult UpdatePassword()
        {
            var result = false;
            string alert = "";
            try
            {
                string userName = Request.Form["UserName"];
                string oldPassword = Request.Form["OldPassword"];
                string newPassword = Request.Form["ConfirmPassword"];
                
                var user = db.OnlineUsers.Where(x => x.UserName == userName).FirstOrDefault();
                if (user != null)
                {
                    if (user.Password.Equals(oldPassword))
                    {
                        user.Password = newPassword;
                        db.SaveChanges();
                        result = true;
                    }
                    else
                    {
                        alert = "Old password not matched.";
                    }
                    
                }

                return Json(new { success = result, message = alert }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}