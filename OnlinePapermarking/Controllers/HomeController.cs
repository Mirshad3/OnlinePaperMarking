using Ionic.Zip;
using OnlinePapermarking.Models;
using OnlinePapermarking.Models.Home;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace OnlinePapermarking.Controllers
{
    
    public class HomeController : Controller
    {
        OnlinePaperMarkingEntities db = new OnlinePaperMarkingEntities();

        public ActionResult Index()
        {
            //var examData = db.Exams.Where(x =>  x.TutorStatus == "Completed" && x.StudentStatus == "Completed").Join(
            //            db.PastPapers,
            //            ex => ex.PastPaperId,
            //            pp => pp.PastPaerID,
            //            (ex, pp) => new { Exam = ex, PastPaper = pp }).GroupBy(x => x.Exam.PastPaperId).ToList();
            //dynamic newModal = new ExpandoObject();
            //newModal.PastPapers = GetPastPapers();
            

            return View();
        }

        public ActionResult Register()
        {
            dynamic myModal = new ExpandoObject();
            myModal.Districts = GetDistrictForCombo();
            return View(myModal);
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult PastPaperCatelog()
        {
            return View();
        }

        public List<PastPaper> GetPastPapers()
        {
            var list = db.PastPapers.ToList();
            return list;
        }

        public ActionResult UpdatePaperPartialView()
        {
            var list = db.PastPapers.ToList();

            return PartialView("_LoadPastPapers", list);
        }

        public List<District> GetDistrictForCombo()
        {
            var disList = db.Districts.ToList();
            return disList;
        }

        public ActionResult LoadMainSubjects(int MediumId, int ExamTypeId)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                List<MainSubjects> subjectList = new List<MainSubjects>();
                var paperList = db.PastPapers.Where(x => x.MediumID == MediumId && x.ExamTypeID == ExamTypeId).
                    GroupBy(x => x.SubjectID).ToList();

                foreach(var papers in paperList)
                {
                    int subjectId = papers.Key;
                    var imageId = db.SubjectImages.Where(x => x.SubjectId == subjectId && x.MediumId == MediumId).Select(x => x.ImageID).FirstOrDefault();
                    subjectList.Add(new MainSubjects { SubjectId = subjectId, ImageId = imageId });
                }
                    
                return Json(new { dataList = subjectList }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            
        }

        public ActionResult LoadSubjectImage(int ImageId)
        {
            var path = db.SubjectImages.Where(x => x.ImageID == ImageId).Select(x => x.ImagePath).FirstOrDefault();
            
            return File(path, "image/*");
        }

        public ActionResult LoadPaperPreviewImage(int PaperId)
        {
            var path = db.PastPapers.Where(x => x.PastPaerID == PaperId).Select(x => x.PaperPreviewPath).FirstOrDefault();

            return File(path, "image/*");
        }

        public ActionResult LoadPastPapersToPopup(int SubjectId , int ExamTypeId , int MediumId)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var list = db.PastPapers.Where(x => x.SubjectID == SubjectId && x.MediumID == MediumId && x.ExamTypeID == ExamTypeId).ToList();

                return Json(new { dataList = list }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult LoadPastPapers(int MediumId, int ExamTypeId, int SubjectId)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var list = db.PastPapers.Where(x => x.MediumID == MediumId && x.ExamTypeID == ExamTypeId && x.SubjectID == SubjectId).Join(
                        db.Subjects,
                        pp => pp.SubjectID,
                        su => su.SubjectID,
                        (pp, su) => new { PastPaper = pp, Subject = su }).ToList().Select(x => new
                        {
                            PaperName = (x.PastPaper.ExamTypeID == 1 ? "General Certificate of Education (Ordinary Level) Examination - " + x.PastPaper.Year + " December " + x.Subject.Description :
                                         x.PastPaper.ExamTypeID == 2 ? "General Certificate of Education (Advanced Level) Examination - " + x.PastPaper.Year + " August " + x.Subject.Description : ""),
                            x.PastPaper.PastPaerID
                        }).ToList();
                    

                return Json(new { dataList = list }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public ActionResult CheckLogin(Login logModel)
        {
            var result = false;
            var controllerName = "";
            var actionName = "";
            try
            {
                var olduser = db.OldUsers.Where(x => x.MobileNo == logModel.Email).FirstOrDefault();
                if (olduser != null)
                {
                    Session["IsOldUser"] = "True";
                    return Json(new { success = result, isOldUser = true }, JsonRequestBehavior.AllowGet);
                }
                var userDetails = db.OnlineUsers.Where(x => x.UserName == logModel.Email && x.Password == logModel.Password).SingleOrDefault();
                if(userDetails!=null)
                {
                    if(userDetails.IsVerified == false)
                    {
                        return Json(new { success = result, isVerified = false, email = userDetails.UserName }, JsonRequestBehavior.AllowGet);
                    }
                    else if (userDetails.IsBlocked == true)
                    {
                        return Json(new { success = result, isVerified = true, email = userDetails.UserName, message = "You are blocked by admin. Please contact adminstration team." }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        Session["LoginId"] = userDetails.LoginID;
                        HttpCookie userNameCookie = new HttpCookie("UserName", logModel.Email);
                        HttpCookie passwordCookie = new HttpCookie("Password", logModel.Password);

                        userNameCookie.Expires.AddDays(10);
                        passwordCookie.Expires.AddDays(10);

                        HttpContext.Response.SetCookie(userNameCookie);
                        HttpContext.Response.SetCookie(passwordCookie);

                        var controllerNameAction = Request.Cookies["controllerName"] == null ? "" : Request.Cookies["controllerName"].Value;
                        var actionNameAction = Request.Cookies["actionName"] == null ? "" : Request.Cookies["actionName"].Value;
                        
                        if (userDetails.UserRoleId == 2)
                        {
                            if (controllerNameAction.Length > 0)
                            {
                                controllerName = controllerNameAction;
                                actionName = actionNameAction;
                            }
                            else
                            {
                                controllerName = "Dashboard";
                                actionName = "Dashboard";
                            }

                            var student = db.tblOnlineStudentMasters.Where(x => x.LoginID == userDetails.LoginID).FirstOrDefault();
                            Session["FirstName"] = student.StudentFirstName;
                            Session["LastName"] = student.StudentLastName;
                            Session["UserRoleId"] = "2";
                            Session["Role"] = "Student";

                        }
                        else if (userDetails.UserRoleId == 3)
                        {
                            controllerName = "TutorDashboard";
                            actionName = "Dashboard";

                            var tutors = db.tblTutors.Where(x => x.LoginID == userDetails.LoginID).FirstOrDefault();
                            Session["FirstName"] = tutors.TutorFirstName;
                            Session["LastName"] = tutors.TutorLastName;
                            Session["UserRoleId"] = "3";
                            Session["Role"] = "Lecturer";
                        }
                        else if (userDetails.UserRoleId == 1)
                        {
                            controllerName = "AdminPanel";
                            actionName = "AdminDashboard";
                        }

                        CreateLoggedDetails((long)userDetails.LoginID);

                        result = true;
                    }
                    
                }
                return Json(new { success = result, controllerName = controllerName, actionName = actionName }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { success = result , message =  ex.Message}, JsonRequestBehavior.AllowGet);
            }
        }

        
        public void CheckCookies()
        {
            try
            {
                if (Request.Cookies["UserName"] != null && Request.Cookies["Password"] != null)
                {
                    var userName = Request.Cookies["UserName"].Value;
                    var password = Request.Cookies["Password"].Value;

                    var userDetails = db.OnlineUsers.Where(x => x.UserName == userName && x.Password == password && x.IsVerified == true && x.IsBlocked == false).SingleOrDefault();
                    if (userDetails != null)
                    {
                        Session["Success"] = "true";
                        Session["LoginId"] = userDetails.LoginID;

                        if (userDetails.UserRoleId == 2)
                        {
                            var student = db.tblOnlineStudentMasters.Where(x => x.LoginID == userDetails.LoginID).FirstOrDefault();
                            Session["FirstName"] = student.StudentFirstName;
                            Session["LastName"] = student.StudentLastName;
                            Session["UserRoleId"] = "2";
                            Session["Role"] = "Student";

                        }
                        else if (userDetails.UserRoleId == 3)
                        {
                            var tutors = db.tblTutors.Where(x => x.LoginID == userDetails.LoginID).FirstOrDefault();
                            Session["FirstName"] = tutors.TutorFirstName;
                            Session["LastName"] = tutors.TutorLastName;
                            Session["UserRoleId"] = "3";
                            Session["Role"] = "Lecturer";
                        }
                        else if (userDetails.UserRoleId == 1)
                        {
                            Session["FirstName"] = userDetails.UserName;
                            Session["UserRoleId"] = "1";
                        }

                        CreateLoggedDetails((long)userDetails.LoginID);
                    }
                    else
                    {
                        Session["Success"] = "false";
                        HttpContext.Response.Redirect("/");
                        //return new EmptyResult(); 
                    }
                }
                else
                {
                    Session["Success"] = "false";
                    HttpContext.Response.Redirect("/Home/Login");
                    // HttpContext.Response.Redirect("/");
                    //return new EmptyResult();
                }

                //return new EmptyResult();
            }
            catch
            {
                HttpContext.Response.Redirect("/");
            }
            
        }

        private void CreateLoggedDetails(long LoginId)
        {
            LoggedDetail loggedDetail = new LoggedDetail();
            loggedDetail.LoginId = LoginId;
            loggedDetail.LoggedDate = DateTime.Now;
            loggedDetail.LoggedDateTime = DateTime.Now;
            db.LoggedDetails.Add(loggedDetail);
            db.SaveChanges();
        }

        public void ReturnToHome()
        {
            HttpContext.Response.Redirect("/");
        }

        public ActionResult CheckCookiesNew()
        {
            var result = false;
            try
            {
                if (Request.Cookies["UserName"] != null && Request.Cookies["UserName"] != null)
                {
                    var userName = Request.Cookies["UserName"].Value;
                    var password = Request.Cookies["Password"].Value;

                    var userDetails = db.OnlineUsers.Where(x => x.UserName == userName && x.Password == password).SingleOrDefault();
                    if (userDetails != null)
                    {
                        result = true;
                    }
                }
                
                return Json(new { success = result }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { success = result }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult CheckSessions()
        {
            var result = false;
            try
            {
                //var context = filterContext.RequestContext.HttpContext;
                string log = Session["LoginId"].ToString();
                if (Session["LoginId"] != null)
                {
                    result = true;
                }

                return Json(new { success = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result }, JsonRequestBehavior.AllowGet);
            }

        }

        public void LogOut()
        {
            var result = false;
            try
            {
                string[] myCookies = Request.Cookies.AllKeys;
                foreach (string cookie in myCookies)
                {
                    Response.Cookies[cookie].Expires = DateTime.Now.AddDays(-1);
                }

                Session.RemoveAll();
                Session.Clear();
                Session.Abandon();
                FormsAuthentication.SignOut();
                Response.Buffer = true;
                Response.ExpiresAbsolute = DateTime.Now.AddDays(-1d);
                Response.Expires = -1000;
                Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetNoStore();
                //Response.ClearHeaders();
                Response.AddHeader("Cache-Control", "no-store , no-store , max-age=0 , must-revalidate");
                Response.AddHeader("Pragma", "no-cache");
                Response.AppendHeader("Cache-Control", "no-store");
                HttpContext.Response.Redirect("/");

                result = true;

                
            }
            catch (Exception ex)
            {
                
            }

        }

        public ActionResult LogOut1()
        {
            var result = false;
            try
            {
                Session.Abandon();

                result = true;

                return Json(new { success = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public JsonResult StudentRegister(StudentRegister stdModel)
        {
            var result = false;
            var isEmailExists = false;
            try
            {
                var containsEmail = db.OnlineUsers.Where(x => x.UserName == stdModel.Email).FirstOrDefault();
                if (containsEmail != null)
                {
                    isEmailExists = true;
                    return Json(new { success = result, emailExists = isEmailExists }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    int? maxLoginId = db.OnlineUsers.Max(x => x.LoginID);
                    int userRollId = db.UserRoles.Where(x => x.Code == "Student").Select(x => x.ID).SingleOrDefault();
                    if (maxLoginId == null)
                        maxLoginId = 0;

                    tblOnlineStudentMaster std = new tblOnlineStudentMaster();
                    std.StudentFirstName = stdModel.FirstName;
                    std.StudentLastName = stdModel.LastName;
                    std.AddressLine1 = stdModel.Address;
                    std.ContactNo1 = stdModel.MobileNo;
                    std.Email = stdModel.Email;
                    //std.LoginID = maxLoginId + 1;
                    std.District = stdModel.District;
                    std.PurchasedPaperCount = 0;
                    db.tblOnlineStudentMasters.Add(std);

                    OnlineUser user = new OnlineUser();
                    //user.LoginID = maxLoginId + 1;
                    user.UserName = stdModel.Email;
                    user.Password = stdModel.Password;
                    user.IsVerified = false;
                    user.VerificationCode = GetVerificationCode();
                    user.UserRoleId = userRollId;
                    user.ProfileCompletionPercentage = 50;
                    user.CreatedDate = DateTime.Now;
                    db.OnlineUsers.Add(user);
                    db.SaveChanges();

                    std.LoginID = (int)user.ID;
                    user.LoginID = (int)user.ID;
                    db.SaveChanges();

                    SendVerificationCode(stdModel.Email , user.VerificationCode);

                    result = true;


                    //var checkEmail = db.OnlineUsers.Where(x => x.UserName == stdModel.Email).FirstOrDefault();
                    //if (checkEmail == null)
                    //{
                    //    return Json(new { success = result, message = "Email not exists" }, JsonRequestBehavior.AllowGet);
                    //}
                    //else
                    //{
                    //    string emailPort = ConfigurationManager.AppSettings["EmailPort"]; ;
                    //    string verifyCode = user.VerificationCode;
                    //    var body = "<p>Email From: Disapamock</p><p>Message: Your verification code is : " + verifyCode + "</p>";
                    //    var message = new MailMessage();
                    //    message.To.Add(new MailAddress(stdModel.Email));  // replace with valid value 
                    //    message.From = new MailAddress("disapamockinfo@gmail.com");  // replace with valid value
                    //    message.Subject = "Verify Account";
                    //    message.Body = string.Format(body, "Disapamock", stdModel.Email, "Your code is : 000111");
                    //    message.IsBodyHtml = true;

                    //    using (var smtp = new SmtpClient())
                    //    {
                    //        var credential = new NetworkCredential
                    //        {
                    //            UserName = "disapamockinfo@gmail.com",  // replace with valid value
                    //            Password = "Disapamock@123"  // replace with valid value
                    //        };
                    //        smtp.Credentials = credential;
                    //        smtp.Host = "smtp.gmail.com";
                    //        smtp.Port = Convert.ToInt32(emailPort);
                    //        smtp.EnableSsl = true;
                    //        smtp.Send(message);
                    //        result = true;
                    //        //return RedirectToAction("Sent");
                    //    }


                    //    return Json(new { success = result }, JsonRequestBehavior.AllowGet);
                    //}

                    return Json(new { success = result , emailExists = isEmailExists, loginId = user.LoginID, email = stdModel.Email }, JsonRequestBehavior.AllowGet);

                }
            }
            catch(Exception ex)
            {
                return Json(new { success = result, message = ex.Message}, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult TutorRegister(TutorRegistration stdModel)
        {
            var result = false;
            var isEmailExists = false;
            try
            {
                var containsEmail = db.OnlineUsers.Where(x => x.UserName == stdModel.Email).FirstOrDefault();
                if (containsEmail != null)
                {
                    isEmailExists = true;
                    return Json(new { success = result, emailExists = isEmailExists }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    int? maxLoginId = db.OnlineUsers.Max(x => x.LoginID);
                    int userRollId = db.UserRoles.Where(x => x.Code == "Tutor").Select(x => x.ID).SingleOrDefault();
                    if (maxLoginId == null)
                        maxLoginId = 0;

                    tblTutor std = new tblTutor();
                    std.TutorFirstName = stdModel.FirstName;
                    std.TutorLastName = stdModel.LastName;
                    std.AddressLine1 = stdModel.Address;
                    std.ContactNo1 = stdModel.MobileNo;
                    std.TutorEmail = stdModel.Email;
                    //std.LoginID = maxLoginId + 1;
                    std.DistrictId = Convert.ToInt32(stdModel.District);
                    std.School_University = stdModel.University;
                    std.Occupation = stdModel.Occupation;
                    std.TutorNIC = stdModel.NICNo;
                    std.AdminApproval = "Pending";
                    db.tblTutors.Add(std);
                    db.SaveChanges();

                    OnlineUser user = new OnlineUser();
                    //user.LoginID = maxLoginId + 1;
                    user.UserName = stdModel.Email;
                    user.Password = stdModel.Password;
                    user.IsVerified = false;
                    user.VerificationCode = GetVerificationCode();
                    user.UserRoleId = userRollId;
                    user.ProfileCompletionPercentage = 50;
                    user.CreatedDate = DateTime.Now;
                    db.OnlineUsers.Add(user);
                    db.SaveChanges();

                    std.LoginID = (int)user.ID;
                    user.LoginID = (int)user.ID;
                    db.SaveChanges();

                    SendVerificationCode(stdModel.Email, user.VerificationCode);

                    result = true;

                    return Json(new { success = result, emailExists = isEmailExists, loginId = user.LoginID, email = user.UserName }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message , emailExists = isEmailExists }, JsonRequestBehavior.AllowGet);
            }
        }

        public string GetVerificationCode()
        {
            Random r = new Random();
            var x = r.Next(0, 1000000);
            string s = x.ToString("000000");
            return s;
        }

        public JsonResult CheckEmail(string StudentEmail)
        {
            var result = true;
            try
            {
                var containsEmail = db.OnlineUsers.Where(x => x.UserName == StudentEmail).FirstOrDefault();
                if (containsEmail!=null)
                {
                    result = false;
                }

                return Json(new { success = result},JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { success = result, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult CheckVerificationCode(int LoginId, string VerificationCode)
        {
            var result = false;
            var errorMsg = "";
            try
            {
                var user = db.OnlineUsers.Where(x => x.LoginID == LoginId && x.VerificationCode== VerificationCode).FirstOrDefault();
                if (user == null)
                {
                    errorMsg = "Verification Faild. Please Retry.";
                }
                else
                {
                    user.IsVerified = true;
                    db.SaveChanges();
                    result = true;
                }
                return Json(new { success = result, message = errorMsg, Email = user.UserName, Password = user.Password }, JsonRequestBehavior.AllowGet);
                //return Json(new { success = result, message = errorMsg }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult SubmitVerification(string Email, string VerificationCode)
        {
            var result = false;
            var errorMsg = "";
            var controllerName = "";
            var actionName = "";
            try
            {
                var user = db.OnlineUsers.Where(x => x.UserName == Email && x.VerificationCode == VerificationCode).FirstOrDefault();
                if (user == null)
                {
                    errorMsg = "Verification Faild. Please Retry.";
                }
                else
                {
                    if (user.UserRoleId == 2)
                    {
                        controllerName = "Dashboard";
                        actionName = "Dashboard";
                    }
                    else if (user.UserRoleId == 3)
                    {
                        controllerName = "TutorDashboard";
                        actionName = "Dashboard";
                    }
                    else if (user.UserRoleId == 1)
                    {
                        controllerName = "AdminPanel";
                        actionName = "AdminDashboard";
                    }
                    user.IsVerified = true;
                    db.SaveChanges();
                    result = true;
                }

                return Json(new { success = result, message = errorMsg, controllerName , actionName }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult ChangePassword(string Email, string ConfirmPassword)
        {
            var result = false;
            var errorMsg = "";
            try
            {
                var user = db.OnlineUsers.Where(x => x.UserName == Email).FirstOrDefault();
                if (user == null)
                {
                    errorMsg = "Email not exists.";
                }
                else
                {
                    user.Password = ConfirmPassword;
                    user.IsVerified = true;
                    db.SaveChanges();
                    result = true;
                }

                return Json(new { success = result, message = errorMsg }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message });
            }
        }

        public JsonResult SendEmail(string Email)
        {
            var result = false;
            try
            {
                var checkEmail = db.OnlineUsers.Where(x => x.UserName == Email).FirstOrDefault();
                if (checkEmail == null)
                {
                    return Json(new { success = result, message = "Email not exists" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string verifyCode = GetVerificationCode();
                    var body = "<p>Email From: Disapamock</p><p>Message: Your code is : "+ verifyCode + "</p>";
                    var message = new MailMessage();
                    message.To.Add(new MailAddress(Email));  // replace with valid value 
                    message.From = new MailAddress("disapamockinfo@gmail.com");  // replace with valid value
                    message.Subject = "Reset Password Confirmation";
                    message.Body = string.Format(body, "Disapamock", Email, "Your code is : 000111");
                    message.IsBodyHtml = true;

                    using (var smtp = new SmtpClient())
                    {
                        var credential = new NetworkCredential
                        {
                            UserName = "disapamockinfo@gmail.com",  // replace with valid value
                            Password = "Disapamock@123"  // replace with valid value
                        };
                        smtp.Credentials = credential;
                        smtp.Host = "smtp.gmail.com";
                        smtp.Port = 587;
                        smtp.EnableSsl = true;
                        smtp.Send(message);
                        result = true;
                        //return RedirectToAction("Sent");
                    }

                    checkEmail.PasswordResetCode = verifyCode;
                    db.SaveChanges();

                    return Json(new { success = result }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult SendEmailToVerify(string Email)
        {
            var result = false;
            try
            {
                var checkEmail = db.OnlineUsers.Where(x => x.UserName == Email).FirstOrDefault();
                if (checkEmail == null)
                {
                    return Json(new { success = result, message = "Email not exists" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string verifyCode = GetVerificationCode();
                    var body = "<p>Email From: Disapamock</p><p>Message: Your code is : " + verifyCode + "</p>";
                    var message = new MailMessage();
                    message.To.Add(new MailAddress(Email));  // replace with valid value 
                    message.From = new MailAddress("disapamockinfo@gmail.com");  // replace with valid value
                    message.Subject = "Reset Password Confirmation";
                    message.Body = string.Format(body, "Disapamock", Email, "Your code is : 000111");
                    message.IsBodyHtml = true;

                    using (var smtp = new SmtpClient())
                    {
                        var credential = new NetworkCredential
                        {
                            UserName = "disapamockinfo@gmail.com",  // replace with valid value
                            Password = "Disapamock@123"  // replace with valid value
                        };
                        smtp.Credentials = credential;
                        smtp.Host = "smtp.gmail.com";
                        smtp.Port = 587;
                        smtp.EnableSsl = true;
                        smtp.Send(message);
                        result = true;
                        //return RedirectToAction("Sent");
                    }

                    checkEmail.VerificationCode = verifyCode;
                    db.SaveChanges();

                    return Json(new { success = result }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult CheckPasswordResetCode(string Email, string VerificationCode)
        {
            var result = false;
            var errorMsg = "";
            try
            {
                var user = db.OnlineUsers.Where(x => x.UserName == Email && x.PasswordResetCode == VerificationCode).FirstOrDefault();
                if (user == null)
                {
                    errorMsg = "Verification Faild. Please Retry.";
                }
                else
                {
                    result = true;
                }

                return Json(new { success = result, message = errorMsg }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message });
            }
        }

        public JsonResult SendVerificationCode(string email , string VerifyCode)
        {
            var result = false;
            try
            {
                var checkEmail = db.OnlineUsers.Where(x => x.UserName == email).FirstOrDefault();
                if (checkEmail == null)
                {
                    return Json(new { success = result, message = "Email not exists" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string verifyCode = VerifyCode;
                    var body = "<p>Email From: Disapamock</p><p>Message: Your verification code is : " + verifyCode + "</p>";
                    var message = new MailMessage();
                    message.To.Add(new MailAddress(email));  // replace with valid value 
                    message.From = new MailAddress("disapamockinfo@gmail.com");  // replace with valid value
                    message.Subject = "Verify Account";
                    message.Body = string.Format(body, "Disapamock", email, "Your code is : 000111");
                    message.IsBodyHtml = true;

                    using (var smtp = new SmtpClient())
                    {
                        var credential = new NetworkCredential
                        {
                            UserName = "disapamockinfo@gmail.com",  // replace with valid value
                            Password = "Disapamock@123"  // replace with valid value
                        };
                        smtp.Credentials = credential;
                        smtp.Host = "smtp.gmail.com";
                        smtp.Port = 587;
                        smtp.EnableSsl = true;
                        smtp.Send(message);
                        result = true;
                        //return RedirectToAction("Sent");
                    }

                    
                    return Json(new { success = result }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        //-------------------------------------------------Past Paper Catelog-----------------------------------------------------------------------------------
        [HttpPost]
        public FileResult DownloadPapers(int PaperId, int LoginId)
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

                var tutor = db.PastPapers.Where(x => x.PastPaerID == PaperId).FirstOrDefault();
                string folderPath = tutor.PaperDownloadPath;

                if (tutor != null)
                {
                    PaperDownloadDetail pd = new PaperDownloadDetail();
                    pd.LoginId = LoginId;
                    pd.PaperId = tutor.PastPaerID;
                    pd.CreatedDate = DateTime.Now;
                    db.PaperDownloadDetails.Add(pd);
                    db.SaveChanges();

                    var st = db.tblOnlineStudentMasters.Where(x => x.LoginID == LoginId).FirstOrDefault();
                    var tu = db.tblTutors.Where(x => x.LoginID == LoginId).FirstOrDefault();
                    if(st != null)
                    {
                        st.DownloadedPaperCount = st.DownloadedPaperCount + 1;
                        db.SaveChanges();
                    }else if (tu != null)
                    {
                        tu.DownloadedPaperCount = tu.DownloadedPaperCount + 1;
                        db.SaveChanges();
                    }
                }

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

        public JsonResult SendEmailToContacUs()
        {
            var result = false;
            try
            {
                string senderName = Request.Form["SenderName"];
                string senderEmail = Request.Form["SenderEmail"];
                string subject = Request.Form["Subject"];
                string fullMessage = Request.Form["Message"];
                string email = "ishara@infoxglobal.com";
                var body = "<p>Sender Name: "+ senderName + "</p><p>Sender Email: " + senderEmail + "</p><p>Message: " + fullMessage + "</p>";
                var message = new MailMessage();
                message.To.Add(new MailAddress(email));  // replace with valid value 
                message.From = new MailAddress("disapamockinfo@gmail.com");  // replace with valid value
                message.Subject = subject;
                message.Body = string.Format(body, "Disapamock", email, "Your code is : 000111");
                message.IsBodyHtml = true;

                using (var smtp = new SmtpClient())
                {
                    var credential = new NetworkCredential
                    {
                        UserName = "disapamockinfo@gmail.com",  // replace with valid value
                        Password = "Disapamock@123"  // replace with valid value
                    };
                    smtp.Credentials = credential;
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    smtp.Send(message);
                    result = true;
                    //return RedirectToAction("Sent");
                }


                return Json(new { success = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        //-----------------------------------------------------------------------View All---------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public ActionResult ViewAll(int ExamTypeId , int MediumId)
        {
            Session["ExamTypeId"] = ExamTypeId;
            Session["MediumId"] = MediumId;
            return View();
        }

    }
}