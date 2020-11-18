using Ionic.Zip;
using OnlinePapermarking.Models;
using OnlinePapermarking.Models.HotSpot;
using OnlinePapermarking.Models.TutorDashboard;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlinePapermarking.Controllers
{
    public class TutorDashboardController : Controller
    {
        OnlinePaperMarkingEntities db = new OnlinePaperMarkingEntities();
        // GET: TutorDashboard
        public ActionResult Dashboard()
        {
            return View();
        }

        public JsonResult GetTutorDashboardBoxDetails(int LoginId)
        {
            var result = false;
            try
            {
                db.Configuration.ProxyCreationEnabled = false;

                var pendingExamsCount = db.Exams.Where(x => x.TutorStatus == "Pending" && x.Tutor1LoginId == LoginId).ToList().Count();
                var completedExamsCount = db.Exams.Where(x => x.TutorStatus == "Completed" && x.Tutor1LoginId == LoginId).ToList().Count();
                decimal moneyToBeRecieve = 0;
                decimal moneyRecieved = 0;
                if (db.Exams.Where(x => x.TutorStatus == "Completed" && x.IsPaidForTutor == false && x.Tutor1LoginId == LoginId).Count() > 0)
                {
                    moneyToBeRecieve = db.Exams.Where(x => x.TutorStatus == "Completed" && x.IsPaidForTutor == false && x.Tutor1LoginId == LoginId).Sum(x => x.PriceForTutor);
                }

                if (db.Exams.Where(x => x.TutorStatus == "Completed" && x.IsPaidForTutor == true && x.Tutor1LoginId == LoginId).Count() > 0)
                {
                    moneyRecieved = db.Exams.Where(x => x.TutorStatus == "Completed" && x.IsPaidForTutor == true && x.Tutor1LoginId == LoginId).Sum(x => x.PriceForTutor);
                }

                int percentage = db.OnlineUsers.Where(x => x.LoginID == LoginId).Select(x => x.ProfileCompletionPercentage).FirstOrDefault();

                result = true;

                return Json(new { success = result, pendingExamsCount, completedExamsCount, moneyToBeRecieve, moneyRecieved, percentage }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        //----------------------------------------------Shared Codes------------------------------------------------------------------------------------------
        public JsonResult SelectNotification(long AssignId)
        {
            var result = false;
            try
            {
                var notiList = db.NotificationAssigns.Where(x => x.AssignID == AssignId).FirstOrDefault();
                notiList.IsExpired = true;
                db.SaveChanges();

                result = true;

                return Json(new { success = result, controller = notiList.ControllerName, method = notiList.MethodName }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        //----------------------------------------------Tutor Profile-----------------------------------------------------------------------------------------
        public ActionResult TutorProfile()
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

        public List<ExamType> GetExamTypesForCombo()
        {
            var typList = db.ExamTypes.ToList();
            return typList;
        }

        public List<Bank> GetBanksForCombo()
        {
            var bankList = db.Banks.ToList();
            return bankList;
        }

        public JsonResult LoadBankBranchesCombo(int BankId)
        {
            var result = false;
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var branchList = db.BankBranches.Where(x => x.BankId == BankId).ToList().Select(x => new
                {
                    x.BranchID,
                    x.BranchName
                });

                result = true;

                return Json(new { success = result, dataList = branchList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetTutorNotifications(int LoginId)
        {
            var result = false;
            try
            {
                var notiList = db.NotificationAssigns.Where(x => x.LoginId == LoginId && x.IsExpired == false).Join(
                        db.Notifications,
                        NoA => NoA.NotificationId,
                        No => No.NotificationID,
                        (NoA, No) => new { NotificationAssign = NoA, Notification = No }).ToList().Select(x => new
                        {
                            x.Notification.NotificationHeader,
                            x.Notification.NotificationDetail,
                            x.NotificationAssign.ControllerName,
                            x.NotificationAssign.MethodName,
                            x.NotificationAssign.AssignID
                        });

                var count = notiList.Count();

                result = true;

                return Json(new { success = result, dataList = notiList, dataCount = count }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetTutorProfileDetailsForSideBar(int LoginId)
        {
            var result = false;
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var stdDetails = db.tblTutors.Where(x => x.LoginID == LoginId).ToList().Select(x => new {
                    x.TutorFirstName,
                    x.TutorLastName,
                    x.School_University,
                    x.Occupation,
                    x.District
                });

                int percentage = db.OnlineUsers.Where(x => x.LoginID == LoginId).Select(x => x.ProfileCompletionPercentage).FirstOrDefault();

                result = true;

                return Json(new { success = result, dataList = stdDetails, percentage }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetTutorProfileDetails(int LoginId)
        {
            var result = false;
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var stdDetails = db.tblTutors.Where(x => x.LoginID == LoginId).ToList().Select(x => new {
                    x.TutorFirstName,
                    x.TutorLastName,
                    DateOfBirth = x.DateOfBirth?.ToString("MM/dd/yyyy"),
                    x.AddressLine1,
                    x.ContactNo1,
                    x.TutorEmail,
                    x.School_University,
                   x.TutorNIC,
                   x.Occupation,
                   x.DistrictId,
                   x.BankId,
                   x.BranchId,
                   x.AccountNo
                });

                var subjectDet = db.PreferedTutorSubjects.Where(x => x.LoginId == LoginId).ToList().Select(x => new
                {
                    x.SubjectId
                }).ToList();

                var mediumDet = db.PreferedTutorMediums.Where(x => x.LoginId == LoginId).ToList().Select(x => new
                {
                    x.MediumId
                }).ToList();

                var exmTypDet = db.PreferedTutorExamTypes.Where(x => x.LoginId == LoginId).ToList().Select(x => new
                {
                    x.ExamTypeId
                }).ToList();

                var penExam = db.Exams.Where(x => x.Tutor1LoginId == LoginId && x.TutorStatus == "Pending").ToList().Count();

                var comExam = db.Exams.Where(x => x.Tutor1LoginId == LoginId && x.TutorStatus == "Completed").ToList().Count();

                result = true;

                return Json(new { success = result, dataList = stdDetails, subjectDet = subjectDet, mediumDet = mediumDet, exmTypDet = exmTypDet, pendingExam = penExam, completedExam = comExam }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SaveTutorPersonalInfo(tblTutor model)
        {
            var result = false;
            try
            {
                var tutor = db.tblTutors.Where(x => x.LoginID == model.LoginID).FirstOrDefault();
                tutor.TutorFirstName = model.TutorFirstName;
                tutor.TutorLastName = model.TutorLastName;
                tutor.AddressLine1 = model.AddressLine1;
                tutor.ContactNo1 = model.ContactNo1;
                tutor.TutorNIC = model.TutorNIC;
                tutor.ModifiedBy = model.LoginID.ToString();
                tutor.ModifiedDate = DateTime.Now;
                db.SaveChanges();

                CalculateProfilePercentage((int)model.LoginID);

                result = true;

                return Json(new { success = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public JsonResult SaveTutorInfo(List<Int32> Subjects, List<Int32> Mediums, List<Int32> ExamTypes, string Occupation, string University, int DistrictId, int LoginId)
        {
            var result = false;
            try
            {
                var tutor = db.tblTutors.Where(x => x.LoginID == LoginId).FirstOrDefault();
                tutor.DistrictId = DistrictId;
                tutor.Occupation = Occupation;
                tutor.School_University = University;
                tutor.ModifiedBy = LoginId.ToString();
                tutor.ModifiedDate = DateTime.Now;

                db.PreferedTutorExamTypes.RemoveRange(db.PreferedTutorExamTypes.Where(x => x.LoginId == LoginId));
                db.PreferedTutorMediums.RemoveRange(db.PreferedTutorMediums.Where(x => x.LoginId == LoginId));
                db.PreferedTutorSubjects.RemoveRange(db.PreferedTutorSubjects.Where(x => x.LoginId == LoginId));

                if(ExamTypes != null)
                {
                    foreach (var examType in ExamTypes)
                    {
                        PreferedTutorExamType et = new PreferedTutorExamType();
                        et.LoginId = LoginId;
                        et.ExamTypeId = examType;
                        et.CreatedUser = LoginId.ToString();
                        et.CreatedDate = DateTime.Now;
                        db.PreferedTutorExamTypes.Add(et);
                    }
                }
                
                if(Mediums != null)
                {
                    foreach (var medium in Mediums)
                    {
                        PreferedTutorMedium me = new PreferedTutorMedium();
                        me.LoginId = LoginId;
                        me.MediumId = medium;
                        me.CreatedUser = LoginId.ToString();
                        //me.CreatedDate = DateTime.Now;
                        db.PreferedTutorMediums.Add(me);
                    }
                }
                
                if(Subjects != null)
                {
                    foreach (var subject in Subjects)
                    {
                        PreferedTutorSubject su = new PreferedTutorSubject();
                        su.LoginId = LoginId;
                        su.SubjectId = subject;
                        su.CreatedUser = LoginId.ToString();
                        su.CreatedDate = DateTime.Now;
                        db.PreferedTutorSubjects.Add(su);
                    }
                }
                
                db.SaveChanges();

                int percentage = CalculateProfilePercentage(LoginId);

                result = true;

                return Json(new { success = result, percentage }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public JsonResult SaveTutorBankInfo(tblTutor model)
        {
            var result = false;
            try
            {
                var tutor = db.tblTutors.Where(x => x.LoginID == model.LoginID).FirstOrDefault();
                tutor.BankId = model.BankId;
                tutor.BranchId = model.BranchId;
                tutor.AccountNo = model.AccountNo;
                tutor.ModifiedBy = model.LoginID.ToString();
                tutor.ModifiedDate = DateTime.Now;
                db.SaveChanges();

                int percentage = CalculateProfilePercentage((int)model.LoginID);

                result = true;

                return Json(new { success = result, percentage }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public JsonResult AddTutorQualifications()
        {
            var result = false;
            try
            {
                string tuterLoginId = Request.Form["TutorLoginId"];
                string certificateType = Request.Form["CertificateType"];
                
                string mainFolderName = tuterLoginId;
                string webConfigPath = ConfigurationManager.AppSettings["TutorQualificationPath"];
                string mainPath = @"" + webConfigPath + "" + mainFolderName;

                if (!(Directory.Exists(mainPath)))
                {
                    Directory.CreateDirectory(mainPath);
                }

                for (int i = 0; i < Request.Files.Count; i++)
                {
                    var file = Request.Files[i];
                    var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                    string yymmssfff = DateTime.Now.ToString("yymmssfff");
                    string newFileName = fileName + yymmssfff;
                    fileName = fileName + yymmssfff + Path.GetExtension(file.FileName);
                    string newPath = mainPath + "\\";
                    newPath = newPath + fileName;
                    file.SaveAs(newPath);

                    TutorQualification qua = new TutorQualification();
                    qua.LoginId = Convert.ToInt64(tuterLoginId);
                    qua.ImagePath = newPath;
                    qua.CertificateType = certificateType;
                    qua.CreatedDate = DateTime.Now;
                    db.TutorQualifications.Add(qua);
                    db.SaveChanges();

                    result = true;
                }

                int percentage = CalculateProfilePercentage(Convert.ToInt32(tuterLoginId));

                return Json(new { success = result, percentage }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult GetQualificationDetails(int LoginId)
        {
            var result = false;
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var quaDetails = db.TutorQualifications.Where(x => x.LoginId == LoginId).Join(
                        db.tblTutors,
                        qua => qua.LoginId,
                        tu => (long)tu.LoginID,
                        (qua, tu) => new { TutorQualification = qua, tblTutor = tu }).Select(x => new
                        {
                            x.TutorQualification.QualificationID,
                            x.TutorQualification.CertificateType,
                            x.tblTutor.AdminApproval
                        }).ToList();

                result = true;

                return Json(new { success = result, dataList = quaDetails }, JsonRequestBehavior.AllowGet);
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

        [HttpPost]
        public JsonResult DeleteQualification(long QualificationId)
        {
            var result = false;
            try
            {
                var qua = db.TutorQualifications.Where(x => x.QualificationID == QualificationId).FirstOrDefault();
                if(qua != null)
                {
                    db.TutorQualifications.Remove(qua);
                    db.SaveChanges();
                }

                int percentage = CalculateProfilePercentage(Convert.ToInt32(qua.LoginId));

                result = true;

                return Json(new { success = result, percentage }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        public int CalculateProfilePercentage(int LoginId)
        {
            int amount = 0;
            var tutor = db.tblTutors.Where(x => x.LoginID == LoginId).FirstOrDefault();
            if(tutor != null)
            {
                //Check personal details
                if(tutor.TutorFirstName != null && tutor.TutorFirstName != "")
                {
                    amount = amount + 6;
                }
                if (tutor.TutorLastName != null && tutor.TutorLastName != "")
                {
                    amount = amount + 6;
                }
                if (tutor.ContactNo1 != null && tutor.ContactNo1 != "")
                {
                    amount = amount + 6;
                }
                if (tutor.TutorNIC != null && tutor.TutorNIC != "")
                {
                    amount = amount + 6;
                }
                if (tutor.AddressLine1 != null && tutor.AddressLine1 != "")
                {
                    amount = amount + 6;
                }

                //Check bank details
                if (tutor.BankId != null)
                {
                    amount = amount + 7;
                }
                if (tutor.BranchId != null)
                {
                    amount = amount + 7;
                }
                if (tutor.AccountNo != null && tutor.AccountNo != "")
                {
                    amount = amount + 7;
                }

                //Check tutor details
                if (tutor.DistrictId != null)
                {
                    amount = amount + 7;
                }
                if (tutor.School_University != null && tutor.School_University != "")
                {
                    amount = amount + 7;
                }
                if (tutor.Occupation != null && tutor.Occupation != "")
                {
                    amount = amount + 7;
                }

                var prefMedium = db.PreferedTutorMediums.Where(x => x.LoginId == LoginId).ToList().Count();
                if(prefMedium > 0)
                {
                    amount = amount + 7;
                }

                var prefExamType = db.PreferedTutorExamTypes.Where(x => x.LoginId == LoginId).ToList().Count();
                if (prefExamType > 0)
                {
                    amount = amount + 7;
                }

                var prefSubjects = db.PreferedTutorSubjects.Where(x => x.LoginId == LoginId).ToList().Count();
                if (prefSubjects > 0)
                {
                    amount = amount + 7;
                }

                //Check qualifications
                var qualification = db.TutorQualifications.Where(x => x.LoginId == LoginId).ToList().Count();
                if (qualification > 0)
                {
                    amount = amount + 7;
                }

                //Update amount
                var user = db.OnlineUsers.Where(x => x.LoginID == LoginId).FirstOrDefault();
                if (user != null)
                {
                    user.ProfileCompletionPercentage = amount;
                    user.ModifiedDate = DateTime.Now;
                    db.SaveChanges();
                }

            }

            return amount;
        }

        //-------------------------------------------Exam Approvals---------------------------------------------------------------------------------------
        public ActionResult ExamApprovals()
        {
            return View();
        }

        public JsonResult GetNotAssignedExamsForTutor(int LoginId)
        {
            var result = false;
            try
            {
                db.Configuration.ProxyCreationEnabled = false;

                var penExamsCount = db.Exams.Where(x => x.Tutor1LoginId == LoginId && x.TutorStatus == "Pending").Count();

                var adminStatus = db.tblTutors.Where(x => x.LoginID == LoginId).Select(x => x.AdminApproval).FirstOrDefault();

                var penExams = db.Exams.Where(x => x.AssignToTutor == "Pending" && penExamsCount < 5 && adminStatus == "Approved").Join(
                        db.PreferedTutorExamTypes.Where(x => x.LoginId == LoginId),
                        ex => ex.ExamTypeId,
                        pet => pet.ExamTypeId,
                        (ex, pet) => new { Exam = ex, PreferedTutorExamType = pet }).Join(
                        db.PreferedTutorMediums.Where(x => x.LoginId == LoginId),
                        ex => ex.Exam.MediumId,
                        pm => pm.MediumId,
                        (ex, pm) => new { ex.Exam, ex.PreferedTutorExamType, PreferedTutorMedium = pm }).Join(
                        db.PreferedTutorSubjects.Where(x => x.LoginId == LoginId),
                        ex => ex.Exam.SubjectId,
                        ps => ps.SubjectId,
                        (ex, ps) => new { ex.Exam, ex.PreferedTutorExamType, ex.PreferedTutorMedium, PreferedTutorSubject = ps }).Join(
                        db.ExamTypes,
                        ex => ex.Exam.ExamTypeId,
                        ext => ext.ExamTypeID,
                        (ex, ext) => new { ex.Exam, ex.PreferedTutorExamType, ex.PreferedTutorMedium, ex.PreferedTutorSubject, ExamType = ext }).Join(
                        db.Mediums,
                        ex => ex.Exam.MediumId,
                        me => me.MediumID,
                        (ex, me) => new { ex.Exam, ex.PreferedTutorExamType, ex.PreferedTutorMedium, ex.PreferedTutorSubject, ex.ExamType, Medium = me }).Join(
                        db.Subjects,
                        ex => ex.Exam.SubjectId,
                        su => su.SubjectID,
                        (ex, su) => new { ex.Exam, ex.PreferedTutorExamType, ex.PreferedTutorMedium, ex.PreferedTutorSubject, ex.ExamType, ex.Medium, Subject = su }).Join(
                        db.tblOnlineStudentMasters,
                        ex => ex.Exam.StudentLoginId,
                        stu => stu.LoginID,
                        (ex, stu) => new { ex.Exam, ex.PreferedTutorExamType, ex.PreferedTutorMedium, ex.PreferedTutorSubject, ex.ExamType, ex.Medium, ex.Subject, tblOnlineStudentMaster = stu }).Join(
                        db.PastPapers,
                        ex => ex.Exam.PastPaperId,
                        pp => pp.PastPaerID,
                        (ex, pp) => new { ex.Exam, ex.PreferedTutorExamType, ex.PreferedTutorMedium, ex.PreferedTutorSubject, ex.ExamType, ex.Medium, ex.Subject, ex.tblOnlineStudentMaster, PastPaper = pp }).
                        ToList().Select(x => new
                        {
                            x.Exam.ExamID,
                            x.Exam.ExamNo,
                            StudentName = x.tblOnlineStudentMaster.StudentFirstName + " " + x.tblOnlineStudentMaster.StudentLastName,
                            ExamType = x.ExamType.ExamTypeCode,
                            Medium = x.Medium.Description,
                            Subject = x.Subject.Description,
                            x.PastPaper.Year
                        });

                result = true;

                return Json(new { success = result, data = penExams}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetAssignedExamsForTutor(int TutorLoginId)
        {
            var result = false;
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var penExams = db.Exams.Where(x => x.Tutor1LoginId == TutorLoginId && x.TutorStatus == "Pending").Join(
                        db.ExamTypes,
                        ex => ex.ExamTypeId,
                        ext => ext.ExamTypeID,
                        (ex, ext) => new { Exam = ex , ExamType = ext }).Join(
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
                            x.Exam.ExamNo,
                            x.Exam.ExamID,
                            StudentName = x.tblOnlineStudentMaster.StudentFirstName + " " + x.tblOnlineStudentMaster.StudentLastName,
                            ExamType = x.ExamType.ExamTypeCode,
                            Medium = x.Medium.Description,
                            Subject = x.Subject.Description,
                            x.PastPaper.Year,
                            ApprovedDate = x.Exam.TutorApprovedDate?.ToString("MM/dd/yyyy"),
                            Deadline = x.Exam.TutorDeadlineDate?.ToString("MM/dd/yyyy"),
                            Paper1ExamId = (x.PastPaper.HasMcq == true ? "" :
                           x.PastPaper.HasMcq == false ? x.Exam.ExamID.ToString() : ""),
                            Paper2ExamId = (x.PastPaper.HasMap == false ? "" :
                           x.PastPaper.HasMap == true ? x.Exam.ExamID.ToString() : ""),
                        });

                result = true;

                return Json(new { success = result, data = penExams }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetCompletedExamsForTutor(int TutorLoginId)
        {
            var result = false;
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var penExams = db.Exams.Where(x => x.Tutor1LoginId == TutorLoginId && x.TutorStatus == "Completed").Join(
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
                            x.Exam.ExamNo,
                            x.Exam.ExamID,
                            StudentName = x.tblOnlineStudentMaster.StudentFirstName + " " + x.tblOnlineStudentMaster.StudentLastName,
                            ExamType = x.ExamType.ExamTypeCode,
                            Medium = x.Medium.Description,
                            Subject = x.Subject.Description,
                            x.PastPaper.Year,
                            ApprovedDate = x.Exam.TutorApprovedDate?.ToString("MM/dd/yyyy"),
                            DeadlineDate = x.Exam.TutorDeadlineDate?.ToString("MM/dd/yyyy"),
                            CompletedDate = x.Exam.TutorCompletedDate?.ToString("MM/dd/yyyy"),
                            Paper1ExamId = (x.PastPaper.HasMcq == true ? "" :
                           x.PastPaper.HasMcq == false ? x.Exam.ExamID.ToString() : ""),
                        });

                result = true;

                return Json(new { success = result, data = penExams }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult ApproveExam(int ExamId, int TutorLoginId)
        {
            var result = false;
            try
            {
                var tutor = db.Exams.Where(x => x.ExamID == ExamId).FirstOrDefault();
                tutor.Tutor1LoginId = TutorLoginId;
                tutor.TutorStatus = "Pending";
                tutor.AssignToTutor = "Completed";
                tutor.TutorApprovedDate = DateTime.Now;
                tutor.TutorDeadlineDate = DateTime.Now.AddDays(3);
                db.SaveChanges();

                var tutorMaster = db.tblTutors.Where(x => x.LoginID == TutorLoginId).FirstOrDefault();
                tutorMaster.PendingExamsCount = tutorMaster.PendingExamsCount + 1;
                db.SaveChanges();
                result = true;

                return Json(new { success = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public JsonResult CheckPaperHasMcq(int ExamId)
        {
            var result = false;
            try
            {
                var hasMcq = db.Exams.Where(x => x.ExamID == ExamId).Join(
                        db.PastPapers,
                        ex => ex.PastPaperId,
                        pp => pp.PastPaerID,
                        (ex, pp) => new { Exam = ex, PastPaper = pp }).Select(x => x.PastPaper.HasMcq).FirstOrDefault();

                result = true;

                return Json(new { success = result , hasMcq = hasMcq }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public FileResult DownloadFirstPaperMarkedSheets(int ExamId)
        {
            //Define file Type
            string fileType = "application/octet-stream";

            //Define Output Memory Stream
            var outputStream = new MemoryStream();

            //Create object of ZipFile library
            using (ZipFile zipFile = new ZipFile())
            {
                //Add Root Directory Name "Files" or Any string name
                zipFile.AddDirectoryByName("Files");

                var tutor = db.Exams.Where(x => x.ExamID == ExamId).FirstOrDefault();
                string folderPath = tutor.FirstPaperMarkedSheetsPath;

                //Get all filepath from folder
                String[] files = Directory.GetFiles(folderPath);
                foreach (string file in files)
                {
                    string filePath = file;

                    //Adding files from filepath into Zip
                    zipFile.AddFile(filePath, "Files");
                }

                Response.ClearContent();
                Response.ClearHeaders();

                //Set zip file name
                Response.AppendHeader("content-disposition", "attachment; filename=PuneetGoelDotNetZip.zip");

                //Save the zip content in output stream
                zipFile.Save(outputStream);
            }

            //Set the cursor to start position
            outputStream.Position = 0;

            //Dispance the stream
            return new FileStreamResult(outputStream, fileType);
        }

        [HttpPost]
        public FileResult DownloadSecondPaperMarkedSheets(int ExamId)
        {
            //Define file Type
            string fileType = "application/octet-stream";

            //Define Output Memory Stream
            var outputStream = new MemoryStream();

            //Create object of ZipFile library
            using (ZipFile zipFile = new ZipFile())
            {
                //Add Root Directory Name "Files" or Any string name
                zipFile.AddDirectoryByName("Files");

                var tutor = db.Exams.Where(x => x.ExamID == ExamId).FirstOrDefault();
                string folderPath = tutor.SecondPaperMarkedSheetsPath;

                //Get all filepath from folder
                String[] files = Directory.GetFiles(folderPath);
                foreach (string file in files)
                {
                    string filePath = file;

                    //Adding files from filepath into Zip
                    zipFile.AddFile(filePath, "Files");
                }

                Response.ClearContent();
                Response.ClearHeaders();

                //Set zip file name
                Response.AppendHeader("content-disposition", "attachment; filename=PuneetGoelDotNetZip.zip");

                //Save the zip content in output stream
                zipFile.Save(outputStream);
            }

            //Set the cursor to start position
            outputStream.Position = 0;

            //Dispance the stream
            return new FileStreamResult(outputStream, fileType);
        }

        [HttpPost]
        public FileResult DownloadFirstPaperAnswers(int ExamId)
        {
            //Define file Type
            string fileType = "application/octet-stream";

            //Define Output Memory Stream
            var outputStream = new MemoryStream();

            //Create object of ZipFile library
            using (ZipFile zipFile = new ZipFile())
            {
                //Add Root Directory Name "Files" or Any string name
                zipFile.AddDirectoryByName("Files");

                var tutor = db.Exams.Where(x => x.ExamID == ExamId).FirstOrDefault();
                string folderPath = tutor.FirstPaperAnswerSheetsPath;

                //Get all filepath from folder
                String[] files = Directory.GetFiles(folderPath);
                foreach (string file in files)
                {
                    string filePath = file;

                    //Adding files from filepath into Zip
                    zipFile.AddFile(filePath, "Files");
                }

                Response.ClearContent();
                Response.ClearHeaders();

                //Set zip file name
                Response.AppendHeader("content-disposition", "attachment; filename=PuneetGoelDotNetZip.zip");

                //Save the zip content in output stream
                zipFile.Save(outputStream);
            }

            //Set the cursor to start position
            outputStream.Position = 0;

            //Dispance the stream
            return new FileStreamResult(outputStream, fileType);
        }

        [HttpPost]
        public FileResult DownloadSecondPaperAnswers(int ExamId)
        {
            //Define file Type
            string fileType = "application/octet-stream";

            //Define Output Memory Stream
            var outputStream = new MemoryStream();

            //Create object of ZipFile library
            using (ZipFile zipFile = new ZipFile())
            {
                //Add Root Directory Name "Files" or Any string name
                zipFile.AddDirectoryByName("Files");

                var tutor = db.Exams.Where(x => x.ExamID == ExamId).FirstOrDefault();
                string folderPath = tutor.SecondPaperAnswerSheetsPath;

                //Get all filepath from folder
                String[] files = Directory.GetFiles(folderPath);
                foreach (string file in files)
                {
                    string filePath = file;

                    //Adding files from filepath into Zip
                    zipFile.AddFile(filePath, "Files");
                }

                Response.ClearContent();
                Response.ClearHeaders();

                //Set zip file name
                Response.AppendHeader("content-disposition", "attachment; filename=PuneetGoelDotNetZip.zip");

                //Save the zip content in output stream
                zipFile.Save(outputStream);
            }

            //Set the cursor to start position
            outputStream.Position = 0;

            //Dispance the stream
            return new FileStreamResult(outputStream, fileType);
        }

        [HttpPost]
        public JsonResult UploadMarkedSheet()
        {
            var result = false;
            try
            {
                string tuterLoginId = Request.Form["TutorLoginId"];
                string examId = Request.Form["ExamId"];
                string radioBtnValue = Request.Form["RadioBtnValue"];
                string marks = Request.Form["Marks"];
                string comment = Request.Form["Comment"];
                int intExamId = Convert.ToInt32(examId);

                var exam = db.Exams.Where(x => x.ExamID == intExamId).FirstOrDefault();
                
                string mainFolderName = examId;
                string subFolderName = "";
                string webConfigPath = ConfigurationManager.AppSettings["TutorUploadPath"];
                string mainPath = @""+ webConfigPath + ""+ mainFolderName;
                

                if (radioBtnValue == "1")
                {
                    subFolderName = examId + "-1";
                }else if(radioBtnValue == "2")
                {
                    subFolderName = examId + "-2";
                }

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

                    if (radioBtnValue == "1")
                    {
                        exam.FirstPaperMarkedSheetsPath = subPath;
                        exam.FirstPaperMarks = Convert.ToDecimal(marks);
                        exam.TutorCommentsForFirstPaper = comment;
                    }
                    else if (radioBtnValue == "2")
                    {
                        exam.SecondPaperMarkedSheetsPath = subPath;
                        exam.SecondPaperMarks = Convert.ToDecimal(marks);
                        exam.TutorCommentsForSecondPaper = comment;
                    }
                    
                    exam.UpdateDate = DateTime.Now;
                    exam.UpdateUser = tuterLoginId;
                    db.SaveChanges();

                    result = true;
                }
                else
                {
                    for (int i = 0; i < Request.Files.Count; i++)
                    {
                        var file = Request.Files[i];
                        var fileName = file.FileName.ToString();
                        fileName = fileName + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(file.FileName);
                        string newPath = subPath + "\\";
                        newPath = newPath + fileName;
                        file.SaveAs(newPath);
                    }

                    if (radioBtnValue == "1")
                    {
                        exam.FirstPaperMarkedSheetsPath = subPath;
                        exam.FirstPaperMarks = Convert.ToDecimal(marks);
                        exam.TutorCommentsForFirstPaper = comment;
                    }
                    else if (radioBtnValue == "2")
                    {
                        exam.SecondPaperMarkedSheetsPath = subPath;
                        exam.SecondPaperMarks = Convert.ToDecimal(marks);
                        exam.TutorCommentsForSecondPaper = comment;
                    }

                    exam.UpdateDate = DateTime.Now;
                    exam.UpdateUser = tuterLoginId;
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

        [HttpPost]
        public JsonResult CompleteExam(int ExamId , int TutorLoginId)
        {
            var result = false;
            try
            {
                var exam = db.Exams.Where(x => x.ExamID == ExamId).FirstOrDefault();
                if(exam != null){
                    var tutorPrice = db.Subjects.Where(x => x.SubjectID == exam.SubjectId).Select(x => x.PriceForTutor).FirstOrDefault();
                    exam.TutorStatus = "Completed";
                    exam.TutorCompletedDate = DateTime.Now;
                    exam.PriceForTutor = tutorPrice;
                    exam.UpdateDate = DateTime.Now;
                    exam.UpdateUser = TutorLoginId.ToString();
                    db.SaveChanges();

                    var tutorMaster = db.tblTutors.Where(x => x.LoginID == TutorLoginId).FirstOrDefault();
                    tutorMaster.PendingExamsCount = tutorMaster.PendingExamsCount - 1;
                    tutorMaster.CompletedExamsCount = tutorMaster.CompletedExamsCount + 1;
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

        public FileResult DispalyMarkingScheme(int ExamId)
        {

            try
            {
                string fileLocation = db.Exams.Where(x => x.ExamID == ExamId).Join(
                        db.PastPapers,
                        ex => ex.PastPaperId,
                        pp => pp.PastPaerID,
                        (ex,pp) => new { Exam = ex , PastPaper = pp}).Select(x => x.PastPaper.MarkingSchemePath).SingleOrDefault().ToString();
                //string filePath = Server.MapPath(fileLocation);
                byte[] fileBytes = System.IO.File.ReadAllBytes(fileLocation);
                return File(fileBytes, "application/pdf");

            }
            catch (Exception ex)
            {
                return File("", "application/pdf");
            }

        }

        //-----------------------------------------------------------------------Tutor Wallet History--------------------------------------------------------------------------------------------------------------------------------
        public ActionResult WalletHistory()
        {
            return View();
        }

        public JsonResult GetTutorWalletHistory(int LoginId , bool IsPaid)
        {
            var result = false;
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var walletHistory = db.Exams.Where(x => x.Tutor1LoginId == LoginId && x.TutorStatus == "Completed" && x.IsPaidForTutor == IsPaid).Join(
                        db.PastPapers,
                        ex => ex.PastPaperId,
                        pp => pp.PastPaerID,
                        (ex, pp) => new { Exam = ex, PastPaper = pp }).Join(
                        db.Subjects,
                        pp => pp.PastPaper.SubjectID,
                        su => su.SubjectID,
                        (pp, su) => new { pp.PastPaper, pp.Exam, Subject = su }).ToList().Select(x => new
                        {
                            x.Exam.ExamNo,
                            PaperName = (x.PastPaper.ExamTypeID == 1 ? "O/L-" + x.Subject.Description + "-" + x.PastPaper.Year :
                             x.PastPaper.ExamTypeID == 2 ? "A/L" + x.Subject.Description + "-" + x.PastPaper.Year : ""),
                            TutorCompletedDate = x.Exam.TutorCompletedDate?.ToString("MM/dd/yyyy"),
                            PriceForTutor = "Rs. " + x.Exam.PriceForTutor,
                            PaidDate = x.Exam.PaidDateForTutor?.ToString("MM/dd/yyyy"),
                            CalPrice = x.Subject.PriceForTutor
                        });

                result = true;

                return Json(new { success = result, data = walletHistory }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        //-----------------------------------------------------------------------------Map Page For Tutor--------------------------------------------------------------------------------------------------------------------------------------------
        public ActionResult MapPageForTutor()
        {
            return View();
        }

        public ActionResult GetMap()
        {
            string path = @"D:\Disapamock\MapImages\sl-map.jpg";
            if (Session["SecondPaperExamIdForMap"] != null)
            {
                int examId = Convert.ToInt32(Session["SecondPaperExamIdForMap"]);
                var paper = db.Exams.Where(x => x.ExamID == examId).Join(
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
            }
            return File(path, "image/jpg");
        }

        public JsonResult SetSessionForMapImage(int ExamId)
        {
            var result = false;
            try
            {
                var exams = db.Exams.Where(x => x.ExamID == ExamId).Join(
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
                           PaperName = (x.PastPaper.ExamTypeID == 1 ? "O/L-" + x.Subject.Description + "-" + x.PastPaper.Year :
                            x.PastPaper.ExamTypeID == 2 ? "A/L" + x.Subject.Description + "-" + x.PastPaper.Year : "")
                       }).FirstOrDefault();

                Session["SecondPaperNameForMap"] = exams.PaperName;
                Session["SecondPaperExamIdForMap"] = ExamId;
                result = true;

                return Json(new { success = result}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult SubmitHotspotDetails(int ExamId, int LoginId, List<HotSpotDetails> HpDetails)
        {
            var result = false;
            
            try
            {
                var exam = db.Exams.Where(x => x.ExamID == ExamId).FirstOrDefault();
                if (exam != null)
                {
                    foreach (var hp in HpDetails)
                    {
                        var checkHp = db.ExamMapDetails.Where(x => x.X == hp.x && x.Y == hp.y && x.ExamId == ExamId).FirstOrDefault();
                        if (checkHp == null)
                        {
                            ExamMapDetail map = new ExamMapDetail();
                            map.ExamId = ExamId;
                            map.X = hp.x;
                            map.Y = hp.y;
                            map.Title = hp.Title;
                            map.Message = hp.Message;
                            map.CreatedType = "Tutor";
                            db.ExamMapDetails.Add(map);
                            db.SaveChanges();

                            result = true;
                        }
                    }

                }

                return Json(new { success = result }, JsonRequestBehavior.AllowGet);
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
                    db.ExamMapDetails.RemoveRange(db.ExamMapDetails.Where(x => x.ExamId == ExamId && x.CreatedType == "Tutor"));
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

        //----------------------------------------------------------------------------Tutor Corrections------------------------------------------------------------------------------------------------------------------------------------------------------
        public ActionResult Corrections()
        {
            return View();
        }

        public ActionResult FirstPaperCorrections()
        {
            return View();
        }

        public JsonResult SetSessionForFirstPaperCorrections(int ExamId)
        {
            var result = false;
            try
            {
                var exams = db.Exams.Where(x => x.ExamID == ExamId).Join(
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
                           PaperName = (x.PastPaper.ExamTypeID == 1 ? x.Exam.ExamNo + "-O/L-" + x.Subject.Description + "-" + x.PastPaper.Year + " (First Paper)" :
                            x.PastPaper.ExamTypeID == 2 ? x.Exam.ExamNo + "-A/L" + x.Subject.Description + "-" + x.PastPaper.Year + " (First Paper)" : ""),
                       }).FirstOrDefault();

                Session["PaperNameForFirstPaperCorrections"] = exams.PaperName;
                Session["PaperExamIdForFirstPaperCorrections"] = ExamId;
                result = true;

                return Json(new { success = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        //----------------------------------------------------------------------------Second Paper Corrections---------------------------------------------------------------------------------------------------------------------------------------------------------------
        public ActionResult SecondPaperCorrections()
        {
            return View();
        }

        public JsonResult SetSessionForSecondPaperCorrections(int ExamId)
        {
            var result = false;
            try
            {
                var exams = db.Exams.Where(x => x.ExamID == ExamId).Join(
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
                       }).FirstOrDefault();

                Session["PaperNameForSecondPaperCorrections"] = exams.PaperName;
                Session["PaperExamIdForSecondPaperCorrections"] = ExamId;
                result = true;

                return Json(new { success = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetImagesForFirstPaperCorrections(int ExamId)
        {
            var result = false;
            try
            {
                List<SecondPaperImages> dataList = new List<SecondPaperImages>();
                var exam = db.Exams.Where(x => x.ExamID == ExamId).FirstOrDefault();
                if (exam != null)
                {
                    string folderPath = exam.FirstPaperAnswerSheetsPath;

                    //Get all filepath from folder
                    String[] files = Directory.GetFiles(folderPath);
                    foreach (string file in files)
                    {
                        string filePath = file;
                        string fileName = Path.GetFileNameWithoutExtension(filePath);
                        string src = "/Tutordashboard/GetImageForSecondPaperCorrections?FileName=" + fileName + "&ExamId=" + ExamId;
                        dataList.Add(new SecondPaperImages { FileName = fileName, Src = src });
                    }

                    result = true;
                }

                return Json(new { success = result, dataList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetImagesForSecondPaperCorrections(int ExamId)
        {
            var result = false;
            try
            {
                List<SecondPaperImages> dataList = new List<SecondPaperImages>();
                var exam = db.Exams.Where(x => x.ExamID == ExamId).FirstOrDefault();
                if(exam != null)
                {
                    string folderPath = exam.SecondPaperAnswerSheetsPath;

                    //Get all filepath from folder
                    String[] files = Directory.GetFiles(folderPath);
                    foreach (string file in files)
                    {
                        string filePath = file;
                        string fileName = Path.GetFileNameWithoutExtension(filePath);
                        string src = "/Tutordashboard/GetImageForSecondPaperCorrections?FileName="+ fileName + "&ExamId=" + ExamId;
                        dataList.Add(new SecondPaperImages { FileName = fileName, Src = src });
                    }

                    result = true;
                }

                return Json(new { success = result, dataList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetImageForSecondPaperCorrections(string FileName , int ExamId)
        {
            var path = db.UploadImages.Where(x => x.ImageId == FileName && x.ExamId == ExamId).Select(x => x.ImagePath).FirstOrDefault();
            
            return File(path, "image/*");
        }

        public JsonResult SubmitHotspotDetailsForSecondPaper(int ExamId, string ImageId, HotSpotDetails HpDetails)
        {
            var result = false;

            try
            {
                var exam = db.Exams.Where(x => x.ExamID == ExamId).FirstOrDefault();
                if (exam != null)
                {
                    //foreach (var hp in HpDetails)
                    //{
                        var checkHp = db.TutorHotSpots.Where(x => x.X == HpDetails.x && x.Y == HpDetails.y && x.ExamId == ExamId && x.ImageId == ImageId).FirstOrDefault();
                        if (checkHp == null)
                        {
                            TutorHotSpot map = new TutorHotSpot();
                            map.ExamId = ExamId;
                            map.X = HpDetails.x;
                            map.Y = HpDetails.y;
                            map.ImageId = ImageId;
                            map.Title = HpDetails.Title;
                            map.Message = HpDetails.Message;
                            map.CreatedDate = DateTime.Now;
                            db.TutorHotSpots.Add(map);
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

        public JsonResult GetHotspotDetailsForSecondPaper(int ExamId, string ImageId)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var result = false;

            try
            {
                var exam = db.Exams.Where(x => x.ExamID == ExamId).FirstOrDefault();
                if (exam != null)
                {
                    var hotSpotDetails = db.TutorHotSpots.Where(x => x.ExamId == ExamId && x.ImageId == ImageId).Select(y => new HotSpotDetails
                    {
                        x = y.X,
                        y = y.Y,
                        Title = y.Title,
                        Message = y.Message
                    }).ToList();

                    result = true;

                    return Json(new { success = result, dataList = hotSpotDetails }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        //----------------------------------------------------------------------------Edit First Paper Hotspots--------------------------------------------------------------------------------------------------------------------------------------
        public ActionResult EditFirstPaperHotspots(int ExamId, string ImageId)
        {
            var exams = db.Exams.Where(x => x.ExamID == ExamId).Join(
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
                           PaperName = (x.PastPaper.ExamTypeID == 1 ? x.Exam.ExamNo + "-O/L-" + x.Subject.Description + "-" + x.PastPaper.Year + " (First Paper)" :
                            x.PastPaper.ExamTypeID == 2 ? x.Exam.ExamNo + "-A/L" + x.Subject.Description + "-" + x.PastPaper.Year + " (First Paper)" : ""),
                       }).FirstOrDefault();

            Session["ExamIdForEditFirstPaperHotspots"] = ExamId;
            Session["ImageIdForEditFirstPaperHotspots"] = ImageId;
            Session["PaperNameForEditFirstPaperHotspots"] = exams.PaperName;
            return View();
        }

        //----------------------------------------------------------------------------Edit Second Paper Hotspots-------------------------------------------------------------------------------------------------------------------------------------
        public ActionResult EditSecondPaperHotspots(int ExamId , string ImageId)
        {
            var exams = db.Exams.Where(x => x.ExamID == ExamId).Join(
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
                       }).FirstOrDefault();
            Session["ExamIdForEditSecondPaperHotspots"] = ExamId;
            Session["ImageIdForEditSecondPaperHotspots"] = ImageId;
            Session["PaperNameForEditSecondPaperHotspots"] = exams.PaperName;
            return View();
        }

        public JsonResult DeleteHotspotDetailsForSecondPaper(int ExamId, string ImageId)
        {
            var result = false;

            try
            {
                var exam = db.Exams.Where(x => x.ExamID == ExamId).FirstOrDefault();
                if (exam != null)
                {
                    db.TutorHotSpots.RemoveRange(db.TutorHotSpots.Where(x => x.ExamId == ExamId && x.ImageId == ImageId));
                    db.SaveChanges();

                    result = true;

                    return Json(new { success = result}, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult DeleteOneHotspotDetailsForPapers(int ExamId, string ImageId, string X, string Y)
        {
            var result = false;

            try
            {
                var exam = db.Exams.Where(x => x.ExamID == ExamId).FirstOrDefault();
                if (exam != null)
                {
                    db.TutorHotSpots.RemoveRange(db.TutorHotSpots.Where(x => x.ExamId == ExamId && x.ImageId == ImageId && x.X == X && x.Y == Y));
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

        //-----------------------------------------------------------------Preview Previous Tutor Uploads--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //public ActionResult PreviewPreviousTutorUploads(int ExamId , )
        //{
        //    Session["ExamIdForEditSecondPaperHotspots"] = ExamId;
        //    Session["ImageIdForEditSecondPaperHotspots"] = ImageId;
        //    return View();
        //}
    }
}