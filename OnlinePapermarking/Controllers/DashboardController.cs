using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OnlinePapermarking.Models;
using OnlinePapermarking.Models.StudentDashboard;
using OnlinePapermarking.Models.TutorDashboard;
using OnlinePapermarking.PaymentGateway;
//using OnlinePapermarking.PaymentGateway;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace OnlinePapermarking.Controllers
{
    //[MyAuthorizeAttribute]
    public class DashboardController : Controller
    {
        OnlinePaperMarkingEntities db = new OnlinePaperMarkingEntities();

        // GET: Dashboard
        public ActionResult Dashboard()
        {
            return View();
        }

        public JsonResult GetStudentDashboardBoxDetails(int LoginId)
        {
            var result = false;
            try
            {
                db.Configuration.ProxyCreationEnabled = false;

                var pendingExamsCount = db.Exams.Where(x => x.StudentStatus == "Pending" && x.StudentLoginId == LoginId).ToList().Count();
                var completedExamsCount = db.Exams.Where(x => x.StudentStatus == "Completed" && x.StudentLoginId == LoginId).ToList().Count();
                var over70ExamsCount = db.Exams.Where(x => (x.FirstPaperMarks + x.SecondPaperMarks)>70 && x.StudentLoginId == LoginId).ToList().Count();
                var less40ExamsCount = db.Exams.Where(x => (x.FirstPaperMarks + x.SecondPaperMarks) < 40 && x.StudentStatus == "Completed" && x.StudentLoginId == LoginId).ToList().Count();
                int percentage = db.OnlineUsers.Where(x => x.LoginID == LoginId).Select(x => x.ProfileCompletionPercentage).FirstOrDefault();

                result = true;

                return Json(new { success = result, pendingExamsCount, completedExamsCount, over70ExamsCount, less40ExamsCount, percentage }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        //------------------------------------------------Shared------------------------------------------------------------------------------------------------------------------------------------------------
        public JsonResult GetStudentProfileDetailsForSideBar(int LoginId)
        {
            var result = false;
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var stdDetails = db.tblOnlineStudentMasters.Where(x => x.LoginID == LoginId).ToList().Select(x => new {
                    x.StudentFirstName,
                    x.StudentLastName,
                    x.School,
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

        public void RedirectToExamPage()
        {
            HttpContext.Response.Redirect("/Dashboard/Exams");
        }

        //-------------------------------------Student Profile-------------------------------------------------------------------------------------------------------------
        public ActionResult StudentProfile()
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

        public JsonResult GetStudentProfileDetails(int LoginId)
        {
            var result = false;
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var stdDetails = db.tblOnlineStudentMasters.Where(x => x.LoginID == LoginId).ToList().Select(x=> new {
                    x.StudentFirstName,
                    x.StudentLastName,
                    DateOfBirth=x.DateOfBirth?.ToString("MM/dd/yyyy"),
                    x.AddressLine1,
                    x.ContactNo1,
                    x.Email,
                    x.School,
                    x.DistrictId,
                    x.ProvinceId,
                    x.MediumId,
                    x.ExamTypeId,
                    x.AcademicYear
                });

                var penExam = db.Exams.Where(x => x.StudentLoginId == LoginId && x.StudentStatus == "Pending").ToList().Count();

                var comExam = db.Exams.Where(x => x.StudentLoginId == LoginId && x.StudentStatus == "Completed").ToList().Count();

                int percentage = db.OnlineUsers.Where(x => x.LoginID == LoginId).Select(x => x.ProfileCompletionPercentage).FirstOrDefault();

                result = true;

                return Json(new { success = result, dataList = stdDetails, pendingExams = penExam, completedExam = comExam, percentage }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message}, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult SaveStudentPersonalInfo(tblOnlineStudentMaster model)
        {
            var result = false;
            try
            {
                var student = db.tblOnlineStudentMasters.Where(x => x.LoginID == model.LoginID).FirstOrDefault();
                student.StudentFirstName = model.StudentFirstName;
                student.StudentLastName = model.StudentLastName;
                student.AddressLine1 = model.AddressLine1;
                student.ContactNo1 = model.ContactNo1;
                student.DateOfBirth = model.DateOfBirth;
                student.ModifiedBy = model.LoginID.ToString();
                student.ModifiedDate = DateTime.Now;
                db.SaveChanges();

                int percentage = CalculateStudentPercentage((int)model.LoginID);

                result = true;

                return Json(new { success = result, percentage }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            
        }

        [HttpPost]
        public JsonResult SaveStudentSchoolInfo(tblOnlineStudentMaster model)
        {
            var result = false;
            try
            {
                var student = db.tblOnlineStudentMasters.Where(x => x.LoginID == model.LoginID).FirstOrDefault();
                student.School = model.School;
                student.ProvinceId = model.ProvinceId;
                student.DistrictId = model.DistrictId;
                student.MediumId = model.MediumId;
                student.ModifiedBy = model.LoginID.ToString();
                student.ModifiedDate = DateTime.Now;
                db.SaveChanges();

                int percentage = CalculateStudentPercentage((int)model.LoginID);

                result = true;

                return Json(new { success = result, percentage }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public JsonResult SaveStudentExamType(tblOnlineStudentMaster model)
        {
            var result = false;
            try
            {
                var student = db.tblOnlineStudentMasters.Where(x => x.LoginID == model.LoginID).FirstOrDefault();
                student.ExamTypeId = model.ExamTypeId;
                student.AcademicYear = model.AcademicYear;
                student.ModifiedBy = model.LoginID.ToString();
                student.ModifiedDate = DateTime.Now;
                db.SaveChanges();

                int percentage = CalculateStudentPercentage((int)model.LoginID);

                result = true;

                return Json(new { success = result, percentage }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public JsonResult SaveUserProfileImage()
        {
            var result = false;
            try
            {
                string loginId = Request.Form["LoginId"];
                int loginIdInt = Convert.ToInt32(loginId);
                
                string mainFolderName = loginId;
                string webConfigPath = ConfigurationManager.AppSettings["ProfileImagePath"];
                string mainPath = @"" + webConfigPath + "" + mainFolderName;

                if (!(Directory.Exists(mainPath)))
                {
                    Directory.CreateDirectory(mainPath);
                }

                var user = db.OnlineUsers.Where(x => x.LoginID == loginIdInt).FirstOrDefault();
                if(user != null)
                {
                    string[] files = Directory.GetFiles(mainPath);
                    foreach (var fle in files)
                    {
                        System.IO.File.Delete(fle);
                    }
                    
                    var file = Request.Files[0];
                    var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                    string yymmssfff = DateTime.Now.ToString("yymmssfff");
                    string newFileName = fileName + yymmssfff;
                    fileName = fileName + yymmssfff + Path.GetExtension(file.FileName);
                    string newPath = mainPath + "\\";
                    newPath = newPath + fileName;
                    file.SaveAs(newPath);

                    user.ProfileImagePath = newPath;
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

        public ActionResult LoadProfileImage(int LoginId)
        {
            var correctPath = "";
            var path = db.OnlineUsers.Where(x => x.LoginID == LoginId).Select(x => x.ProfileImagePath).FirstOrDefault();
            if(path == "" || path == null)
            {
                correctPath = ConfigurationManager.AppSettings["DefaultProfileImagePath"];
            }
            else
            {
                correctPath = path;
            }

            return File(correctPath, "image/*");
        }

        public int CalculateStudentPercentage(int LoginId)
        {
            int amount = 0;
            var student = db.tblOnlineStudentMasters.Where(x => x.LoginID == LoginId).FirstOrDefault();
            if(student != null)
            {
                //Check personal details
                if(student.StudentFirstName != null && student.StudentFirstName != "")
                {
                    amount = amount + 10;
                }
                if (student.StudentLastName != null && student.StudentLastName != "")
                {
                    amount = amount + 10;
                }
                if (student.DateOfBirth != null)
                {
                    amount = amount + 10;
                }
                if (student.AddressLine1 != null && student.AddressLine1 != "")
                {
                    amount = amount + 10;
                }
                if (student.ContactNo1 != null && student.ContactNo1 != "")
                {
                    amount = amount + 10;
                }

                //Check exam details
                if (student.ExamTypeId != null)
                {
                    amount = amount + 10;
                }
                if (student.AcademicYear != null && student.AcademicYear != "")
                {
                    amount = amount + 10;
                }

                //Check school details
                if (student.School != null && student.School != "")
                {
                    amount = amount + 10;
                }
                if (student.MediumId != null)
                {
                    amount = amount + 10;
                }
                if (student.DistrictId != null)
                {
                    amount = amount + 5;
                }
                if (student.ProvinceId != null)
                {
                    amount = amount + 5;
                }

                var user = db.OnlineUsers.Where(x => x.LoginID == LoginId).FirstOrDefault();
                if(user != null)
                {
                    user.ProfileCompletionPercentage = amount;
                    user.ModifiedDate = DateTime.Now;
                    db.SaveChanges();
                }
            }

            return amount;
        }

        //-------------------------------------------------------------------Product List-------------------------------------------------------------------------------------------------------------------------------------------------
        public ActionResult ProductList()
        {
            return View();
        }

        public ActionResult LoadMainSubjects(int MediumId, int ExamTypeId)
        {
            try
            {
                List<ProductListSubject> culist = new List<ProductListSubject>();
                db.Configuration.ProxyCreationEnabled = false;
                var list = db.PastPapers.Where(x => x.MediumID == MediumId && x.ExamTypeID == ExamTypeId && x.IsOnlineExam == true).
                    GroupBy(x => x.SubjectID).ToList();

                foreach(var paper in list)
                {
                    var newPap = db.PastPapers.Where(x => x.ExamTypeID == ExamTypeId && x.MediumID == MediumId && x.SubjectID == paper.Key).FirstOrDefault();
                    if (newPap != null)
                    {
                        culist.Add(new ProductListSubject { ExamTypeId = newPap.ExamTypeID, MediumId = newPap.MediumID, SubjectId = newPap.SubjectID, PaperName = newPap.PaperName });
                    }
                }

                //culist.Add(new string[] { "fdsfds", });

                return Json(new { dataList = culist }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult LoadPastPapers(int MediumId, int ExamTypeId, int SubjectId, int Year)
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var list = db.PastPapers.Where(x => x.MediumID == MediumId && x.ExamTypeID == ExamTypeId && 
                            (x.SubjectID == SubjectId || SubjectId == 0) && (x.Year == Year || Year == 0) && x.IsOnlineExam == true).Join(
                        db.Subjects,
                        pp => pp.SubjectID,
                        su => su.SubjectID,
                        (pp,su) => new { PastPaper = pp , Subject = su}).
                            ToList().Select(x => new {
                                ExamType = (x.PastPaper.MediumID == 1 && x.PastPaper.ExamTypeID == 1 ? "අ.පො.ස. සාමාන්‍ය පෙළ" :
                                            x.PastPaper.MediumID == 1 && x.PastPaper.ExamTypeID == 2 ? "අ.පො.ස. උසස් පෙළ" :
                                            x.PastPaper.MediumID == 2 && x.PastPaper.ExamTypeID == 1 ? "Ordinary Level" :
                                            x.PastPaper.MediumID == 2 && x.PastPaper.ExamTypeID == 2 ? "Advanced Level" :
                                            x.PastPaper.MediumID == 3 && x.PastPaper.ExamTypeID == 1 ? "சாதாரண நிலை" :
                                            x.PastPaper.MediumID == 3 && x.PastPaper.ExamTypeID == 2 ? "மேம்பட்ட நிலை" : ""),
                               x.PastPaper.PastPaerID,
                               x.PastPaper.PaperName,
                               x.PastPaper.Year,
                               x.Subject.Price
                            });

                return Json(new { dataList = list }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult GetCartDetailsToNotificationList(int LoginId)
        {
            var result = false;
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var cartList = db.Carts.Where(x => x.LoginId == LoginId && x.IsCheckOut == false).Join(
                        db.CartItems,
                        ca => ca.CartId,
                        ci => ci.CartId,
                        (ca, ci) => new { Cart = ca, CartItem = ci }).Select(x => new
                        {
                            x.Cart.CartId,
                            x.CartItem.ItemID,
                            x.CartItem.UnitPrice,
                            x.CartItem.Qty,
                            x.Cart.Total,
                            x.Cart.SubTotal,
                            x.CartItem.ItemName

                        }).ToList();

                result = true;

                return Json(new { success = result, dataList = cartList, count = cartList.Count }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        //--------------------------------------------------------------------------Cart----------------------------------------------------------------------------------------------------------------------------------------------------------------
        public ActionResult Cart()
        {
            return View();
        }

        public JsonResult AddToCart(int LoginId, int PastPaperId)
        {
            var result = false;
            try
            {
                var paper = db.PastPapers.Where(x => x.PastPaerID == PastPaperId).Join(
                        db.ExamTypes,
                        pp => pp.ExamTypeID,
                        et => et.ExamTypeID,
                        (pp, et) => new { PastPaper = pp, ExamType = et }).Join(
                        db.Subjects,
                        pp => pp.PastPaper.SubjectID,
                        su => su.SubjectID,
                        (pp, su) => new { pp.PastPaper, pp.ExamType, Subject = su }).Select(x => new
                        {
                            x.PastPaper.PastPaerID,
                            x.Subject.Price,
                            PaperName = (x.PastPaper.ExamTypeID == 1 ? "O/L-" + x.Subject.Description + "-" + x.PastPaper.Year :
                             x.PastPaper.ExamTypeID == 2 ? "A/L" + x.Subject.Description + "-" + x.PastPaper.Year : "")
                        }).FirstOrDefault();

                if(paper != null)
                {
                    var cartItem = new CartItem();
                    var cartDet = db.Carts.Where(x => x.LoginId == LoginId && x.IsCheckOut == false).FirstOrDefault();
                    if (cartDet != null)
                    {
                        cartDet.Total = cartDet.Total + (decimal)paper.Price;
                        cartDet.SubTotal = cartDet.SubTotal + (decimal)paper.Price;

                        cartItem.CartId = cartDet.CartId;
                        cartItem.PastPaperId = paper.PastPaerID;
                        cartItem.UnitPrice = (decimal)paper.Price;
                        cartItem.Qty = 1;
                        cartItem.ItemName = paper.PaperName;
                        db.CartItems.Add(cartItem);
                        db.SaveChanges();

                        result = true;
                    }
                    else
                    {
                        var cart = new Cart();
                        cart.LoginId = LoginId;
                        cart.IsCheckOut = false;
                        cart.Total = 0;
                        cart.SubTotal = 0;
                        cart.IsPromoApplied = false;
                        db.Carts.Add(cart);
                        db.SaveChanges();

                        cartItem.CartId = cart.CartId;
                        cartItem.PastPaperId = paper.PastPaerID;
                        cartItem.UnitPrice = (decimal)paper.Price;
                        cartItem.Qty = 1;
                        cartItem.ItemName = paper.PaperName;
                        db.CartItems.Add(cartItem);

                        cart.Total = cart.Total + (decimal)paper.Price;
                        cart.SubTotal = cart.SubTotal + (decimal)paper.Price;

                        db.SaveChanges();

                        result = true;
                    }
                }
                
                return Json(new { success = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result , message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetCartDetails(int LoginId)
        {
            var result = false;
            try
            {
                var cartList = db.Carts.Where(x => x.LoginId == LoginId && x.IsCheckOut == false).Join(
                        db.CartItems,
                        ca => ca.CartId,
                        ci => ci.CartId,
                        (ca,ci) => new { Cart = ca , CartItem = ci}).Select(x => new
                        {
                            x.Cart.CartId,
                            x.CartItem.ItemID,
                            x.CartItem.UnitPrice,
                            x.CartItem.Qty,
                            x.Cart.Total,
                            x.Cart.SubTotal,
                            x.CartItem.ItemName

                        }).ToList();

                return Json(new { success = result , dataList = cartList , itemCount = cartList.Count }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult DeleteFromCart(long ItemId)
        {
            var result = false;
            try
            {
                CartItem cartItem = db.CartItems.Where(x => x.ItemID == ItemId).FirstOrDefault();
                if (cartItem != null)
                {
                    long cartId = cartItem.CartId;
                    decimal price = cartItem.UnitPrice;
                    db.CartItems.Remove(cartItem);
                    db.SaveChanges();

                    Cart cart = db.Carts.Where(x => x.CartId == cartId && x.IsCheckOut == false).FirstOrDefault();
                    var std = db.tblOnlineStudentMasters.Where(x => x.LoginID == cart.LoginId).FirstOrDefault();
                    if (db.CartItems.Where(x => x.CartId == cartId).Count() <= 0)
                    {
                        //std.IsPromoCodeUsed = false;
                        db.Carts.Remove(cart);
                        db.SaveChanges();
                    }
                    else
                    {
                        decimal total = (decimal)cart.Total - price;
                        if (total < 0)
                        {
                            cart.Total = 0;
                        }
                        else
                        {
                            cart.Total = total;
                        }

                        decimal subTotal = (decimal)cart.SubTotal - price;
                        if (subTotal < 0)
                        {
                            cart.SubTotal = 0;
                        }
                        else
                        {
                            cart.SubTotal = subTotal;
                        }

                        //std.IsPromoCodeUsed = false;
                        db.SaveChanges();
                    }
                    
                    result = true;
                }

                return Json(new { success = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ApplyPromoCode(long LoginId , string PromoCode)
        {
            var result = false;
            string message = "";
            try
            {
                var cart = db.Carts.Where(x => x.LoginId == LoginId && x.IsCheckOut == false).FirstOrDefault();
                if (cart != null)
                {
                    var promo = db.PromoCodes.Where(x => x.PromoCode1 == PromoCode).FirstOrDefault();
                    if (promo != null)
                    {
                        var student = db.tblOnlineStudentMasters.Where(x => x.LoginID == LoginId && x.IsPromoCodeUsed == false).FirstOrDefault();
                        if (student != null)
                        {
                            var discount = ConfigurationManager.AppSettings["Discount"];
                            //cart.SubTotal = cart.SubTotal - Convert.ToDecimal(discount);
                            //student.IsPromoCodeUsed = true;
                            cart.IsPromoApplied = true;
                            db.SaveChanges();
                            message = "Promo code applied successfully. You will get a discount Rs " + discount + " from total price.";
                            result = true;

                            return Json(new { success = result, cart.SubTotal, message }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            message = "Promo Code Already Used.";
                        }

                    }
                    else
                    {
                        message = "Invalid Promo Code.";
                    }
                }

                return Json(new { success = result, message }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult CheckOut(long LoginId)
        {
            var result = false;
            bool moveToGateway = false;
            try
            {
                var cart = db.Carts.Where(x => x.LoginId == LoginId && x.IsCheckOut == false).FirstOrDefault();
                if (cart != null)
                {
                    var discount = ConfigurationManager.AppSettings["Discount"];
                    var subTotal = cart.SubTotal;

                    if (cart.IsPromoApplied)
                    {
                        subTotal = subTotal - Convert.ToDecimal(discount);
                    }

                    if (subTotal > 0)
                    {
                        GatewayApiRequest gatewayApiRequest = new GatewayApiRequest();
                        gatewayApiRequest.ApiOperation = "CREATE_CHECKOUT_SESSION";
                        gatewayApiRequest.Password = "f646a0741e4c372eb075ddc530f2ba44";

                        string loginIdstr = encrypt(LoginId.ToString());
                        string orderIdstr = encrypt(cart.CartId.ToString());

                        gatewayApiRequest.ReturnUrl = "http://disapamock.com/Dashboard/PaymentSummery?LoginId=" + loginIdstr + "&OrderId=" + orderIdstr;

                        //gatewayApiRequest.ReturnUrl = "http://localhost:60826/Dashboard/PaymentSummery?LoginId=" + loginIdstr + "&OrderId=" + orderIdstr;

                        gatewayApiRequest.ApiUsername = "merchant.DISAPAMOCLKR";
                        gatewayApiRequest.Merchant = "DISAPAMOCLKR";

                        gatewayApiRequest.OrderId = cart.CartId.ToString();//IdUtils.generateSampleId();
                        gatewayApiRequest.OrderCurrency = "LKR";//GatewayApiConfig.Currency;
                        gatewayApiRequest.OrderAmount = subTotal.ToString();
                        gatewayApiRequest.InteractionOperation = "PURCHASE";
                        gatewayApiRequest.InteractionMerchantName = "Disapamock";

                        gatewayApiRequest.OrderDescription = "ORD" + cart.CartId.ToString();


                        gatewayApiRequest.ApiMethod = "POST";
                        gatewayApiRequest.RequestUrl = "https://cbcmpgs.gateway.mastercard.com/api/nvp/version/52";

                        //
                        var nvc = new Dictionary<string, string>();
                        nvc.Add("apiOperation", gatewayApiRequest.ApiOperation);
                        nvc.Add("apiPassword", gatewayApiRequest.Password);
                        nvc.Add("interaction.returnUrl", gatewayApiRequest.ReturnUrl);
                        nvc.Add("apiUsername", gatewayApiRequest.ApiUsername);
                        nvc.Add("merchant", gatewayApiRequest.Merchant);
                        nvc.Add("order.id", gatewayApiRequest.OrderId);
                        nvc.Add("order.amount", gatewayApiRequest.OrderAmount);
                        nvc.Add("order.currency", gatewayApiRequest.OrderCurrency);
                        nvc.Add("interaction.operation", gatewayApiRequest.InteractionOperation);
                        nvc.Add("interaction.merchant.name", gatewayApiRequest.InteractionMerchantName);
                        nvc.Add("order.description", gatewayApiRequest.OrderDescription);

                        Session["PaymentOrderId"] = gatewayApiRequest.OrderId;
                        Session["OrderAmount"] = subTotal.ToString();

                        using (HttpClient client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                            HttpResponseMessage response = client.PostAsync(gatewayApiRequest.RequestUrl, new FormUrlEncodedContent(nvc)).Result;
                            var tokne = response.Content.ReadAsStringAsync().Result;
                            String[] arr1 = tokne.Split('&');
                            String[] arr2 = arr1[2].Split('=');
                            Session["PaymentSession"] = arr2[1];
                            result = true;
                            moveToGateway = true;

                            return Json(new { success = result, moveToGateway }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        var cartitems = db.Carts.Where(x => x.CartId == cart.CartId).Join(
                        db.CartItems,
                        ca => ca.CartId,
                        ci => ci.CartId,
                        (ca, ci) => new { Cart = ca, CartItem = ci }).Select(x => new {
                            x.Cart.CartId,
                            x.CartItem.PastPaperId
                        }).ToList();

                        if (cart != null)
                        {
                            foreach (var cartDetail in cartitems)
                            {
                                var pastPaper = db.PastPapers.Where(x => x.PastPaerID == cartDetail.PastPaperId).FirstOrDefault();
                                if (pastPaper != null)
                                {
                                    if (cartDetail.PastPaperId != null)
                                    {
                                        var exam = new Exam();
                                        exam.StudentLoginId = (int)LoginId;
                                        exam.ExamTypeId = pastPaper.ExamTypeID;
                                        exam.MediumId = pastPaper.MediumID;
                                        exam.SubjectId = pastPaper.SubjectID;
                                        exam.PastPaperId = cartDetail.PastPaperId;
                                        exam.StudentStatus = "Pending";
                                        exam.IsFirstPaperCompleted = false;
                                        exam.IsSecondPaperCompleted = false;
                                        exam.IsThirdPaperCompleted = false;
                                        exam.FirstPaperRemainingTime = pastPaper.FirstPaperTime != null ? (int)pastPaper.FirstPaperTime : 0;
                                        exam.SecondPaperRemainingTime = pastPaper.SecondPaperTime != null ? (int)pastPaper.SecondPaperTime : 0;
                                        exam.ThirdPaperRemainingTime = pastPaper.ThirdPaperTime != null ? (int)pastPaper.ThirdPaperTime : 0;
                                        exam.CreatedUser = LoginId.ToString();
                                        exam.CreatedDate = DateTime.Now;
                                        db.Exams.Add(exam);
                                        db.SaveChanges();

                                        var upExam = db.Exams.Where(x => x.ExamID == exam.ExamID).FirstOrDefault();
                                        if (upExam != null)
                                        {
                                            upExam.ExamNo = "EXM" + exam.ExamID;
                                            db.SaveChanges();
                                        }

                                        int stuLoginId = (int)LoginId;
                                        var stu = db.tblOnlineStudentMasters.Where(x => x.LoginID == stuLoginId).FirstOrDefault();
                                        if (stu != null)
                                        {
                                            stu.PurchasedPaperCount = stu.PurchasedPaperCount + 1;
                                            stu.IsPromoCodeUsed = true;
                                            db.SaveChanges();
                                        }
                                    }

                                }
                            }

                            var cartDel = db.Carts.Where(x => x.CartId == cart.CartId).FirstOrDefault();
                            if (cartDel != null)
                            {
                                cartDel.IsCheckOut = true;
                                cartDel.PaymentDate = DateTime.Now;

                                if (cartDel.IsPromoApplied)
                                {
                                    cartDel.SubTotal = cartDel.SubTotal - Convert.ToDecimal(discount);
                                }

                                db.SaveChanges();

                                result = true;
                            }

                        }
                    }
                    
                }


                return Json(new { success = result, moveToGateway }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public string encrypt(string encryptString)
        {
            string EncryptionKey = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            byte[] clearBytes = Encoding.Unicode.GetBytes(encryptString);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
            0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
        });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    encryptString = Convert.ToBase64String(ms.ToArray());
                }
            }
            return encryptString;
        }

        public string Decrypt(string cipherText)
        {
            string EncryptionKey = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            cipherText = cipherText.Replace(" ", "+");
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {
            0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76
        });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }

        //-------------------------------------------------------------------Payment Confirmation---------------------------------------------------------------------------------------------------------------------------------------------------------------
        public ActionResult PaymentConfirmation()
        {
            return View();
        }

        //-------------------------------------------------------------------Payment Summery---------------------------------------------------------------------------------------------------------------------------------------------------------------
        public ActionResult PaymentSummery(string LoginId , string OrderId)
        {
            string orderIdStr = Decrypt(OrderId);
            string loginIdStr = Decrypt(LoginId);

            Session["PaymentOrderId"] = Decrypt(OrderId);
            Session["LoginId"] = Decrypt(LoginId);

            return View();
        }

        public JsonResult GetPaymentSummery(long LoginId , string PaymentOrderId)
        {
            var result = false;
            try
            {
                GatewayApiRespons gatewayApiRespon = new GatewayApiRespons();
                gatewayApiRespon.ApiOperation = "RETRIEVE_ORDER";
                gatewayApiRespon.OrderId = PaymentOrderId;//IdUtils.generateSampleId();
                gatewayApiRespon.ApiMethod = "POST";
                gatewayApiRespon.Password = "f646a0741e4c372eb075ddc530f2ba44";
                gatewayApiRespon.ApiUsername = "merchant.DISAPAMOCLKR";
                gatewayApiRespon.Merchant = "DISAPAMOCLKR";
                gatewayApiRespon.RequestUrl = "https://cbcmpgs.gateway.mastercard.com/api/nvp/version/52";

                var nvcR = new Dictionary<string, string>();
                nvcR.Add("apiOperation", gatewayApiRespon.ApiOperation);
                nvcR.Add("apiPassword", gatewayApiRespon.Password);
                nvcR.Add("apiUsername", gatewayApiRespon.ApiUsername);
                nvcR.Add("merchant", gatewayApiRespon.Merchant);
                nvcR.Add("order.id", gatewayApiRespon.OrderId);

                using (HttpClient client1 = new HttpClient())
                {
                    client1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                    HttpResponseMessage response = client1.PostAsync(gatewayApiRespon.RequestUrl, new FormUrlEncodedContent(nvcR)).Result;
                    var res = response.Content.ReadAsStringAsync().Result;
                    string[] arr = res.Split('&');
                    Dictionary<string, string> responseList = new Dictionary<string, string>();

                    foreach (var item in arr)
                    {
                        string[] arr2 = item.ToString().Split('=');
                        responseList.Add(arr2[0], arr2[1]);
                    }

                    string[] statusArr = arr[12].Split('=');
                    string status = responseList["result"];

                    string[] amountArr = arr[0].Split('=');
                    string amount = responseList["amount"];

                    string paymentDate = DateTime.Now.ToString("dddd, dd MMMM yyyy");

                    long cartId = Convert.ToInt64(PaymentOrderId);

                    if(status == "SUCCESS")
                    {
                        var cart = db.Carts.Where(x => x.CartId == cartId).Join(
                        db.CartItems,
                        ca => ca.CartId,
                        ci => ci.CartId,
                        (ca, ci) => new { Cart = ca, CartItem = ci }).Select(x => new {
                            x.Cart.CartId,
                            x.CartItem.PastPaperId
                        }).ToList();

                        if (cart != null)
                        {
                            foreach (var cartDetail in cart)
                            {
                                var pastPaper = db.PastPapers.Where(x => x.PastPaerID == cartDetail.PastPaperId).FirstOrDefault();
                                if (pastPaper != null)
                                {
                                    if (cartDetail.PastPaperId != null)
                                    {
                                        var exam = new Exam();
                                        exam.StudentLoginId = (int)LoginId;
                                        exam.ExamTypeId = pastPaper.ExamTypeID;
                                        exam.MediumId = pastPaper.MediumID;
                                        exam.SubjectId = pastPaper.SubjectID;
                                        exam.PastPaperId = cartDetail.PastPaperId;
                                        exam.StudentStatus = "Pending";
                                        exam.IsFirstPaperCompleted = false;
                                        exam.IsSecondPaperCompleted = false;
                                        exam.IsThirdPaperCompleted = false;
                                        exam.FirstPaperRemainingTime = pastPaper.FirstPaperTime != null ? (int)pastPaper.FirstPaperTime : 0;
                                        exam.SecondPaperRemainingTime = pastPaper.SecondPaperTime != null ? (int)pastPaper.SecondPaperTime : 0;
                                        exam.ThirdPaperRemainingTime = pastPaper.ThirdPaperTime != null ? (int)pastPaper.ThirdPaperTime : 0;
                                        exam.CreatedUser = LoginId.ToString();
                                        exam.CreatedDate = DateTime.Now;
                                        db.Exams.Add(exam);
                                        db.SaveChanges();

                                        var upExam = db.Exams.Where(x => x.ExamID == exam.ExamID).FirstOrDefault();
                                        if (upExam != null)
                                        {
                                            upExam.ExamNo = "EXM" + exam.ExamID;
                                            db.SaveChanges();
                                        }

                                        int stuLoginId = (int)LoginId;
                                        var stu = db.tblOnlineStudentMasters.Where(x => x.LoginID == stuLoginId).FirstOrDefault();
                                        if(stu != null)
                                        {
                                            stu.PurchasedPaperCount = stu.PurchasedPaperCount + 1;
                                            stu.IsPromoCodeUsed = true;
                                            db.SaveChanges();
                                        }
                                    }

                                }
                            }

                            var cartDel = db.Carts.Where(x => x.CartId == cartId).FirstOrDefault();
                            if (cartDel != null)
                            {
                                cartDel.IsCheckOut = true;
                                cartDel.PaymentDate = DateTime.Now;

                                if (cartDel.IsPromoApplied)
                                {
                                    var discount = ConfigurationManager.AppSettings["Discount"];
                                    cartDel.SubTotal = cartDel.SubTotal - Convert.ToDecimal(discount);
                                }
                                db.SaveChanges();

                                result = true;
                            }
                            
                        }
                    }

                    result = true;

                    return Json(new { success = result, status, amount, paymentDate, paymentOrderId = PaymentOrderId }, JsonRequestBehavior.AllowGet);
                }

                //return Json(new { success = result,status  }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        //-------------------------------------------------------------------Student pending exams------------------------------------------------------------------------------------------------------------------------------------------------
        public ActionResult Exams()
        {
            return View();
        }

        public JsonResult GetPendingExams(int LoginId)
        {
            var result = false;
            try
            {
                var pendingExams = db.Exams.Where(x => x.StudentLoginId == LoginId && x.StudentStatus == "Pending").Join(
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
                             x.PastPaper.ExamTypeID == 2 ? "A/L" + x.Subject.Description + "-" + x.PastPaper.Year : ""),
                            x.Exam.ExamID,
                            FirstPaperId = (x.Exam.IsFirstPaperCompleted == false ? x.Exam.ExamID.ToString() : ""),
                            x.Exam.PastPaperId,
                            SecondPaperId = (x.Exam.IsSecondPaperCompleted == false ? x.Exam.ExamID.ToString() : 
                            (x.PastPaper.HasThirdPaper == true && x.Exam.IsThirdPaperCompleted == false) ? x.Exam.ExamID.ToString() :""),
                            RegisterdDate = x.Exam.CreatedDate?.ToString("MM/dd/yyyy")
                        }).OrderBy(x => x.ExamID);

                return Json(new { success = result, data = pendingExams}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetCompletedExams(int LoginId)
        {
            var result = false;
            try
            {
                var pendingExams = db.Exams.Where(x => x.StudentLoginId == LoginId && x.StudentStatus == "Completed").Join(
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
                             x.PastPaper.ExamTypeID == 2 ? "A/L" + x.Subject.Description + "-" + x.PastPaper.Year : ""),
                            x.Exam.ExamID,
                            TutorStatus = (x.Exam.TutorStatus == null || x.Exam.TutorStatus == "" ? "Pending" : x.Exam.TutorStatus),
                            FirstPaperMarks = (x.Exam.FirstPaperMarks > 0 ? (Math.Round(x.Exam.FirstPaperMarks, 1)).ToString() : "0.00"),
                            FirstPaperPercentMarks = (x.Exam.FirstPaperMarks > 0 ? ((int)((x.Exam.FirstPaperMarks / 40) * 100)).ToString() : "00"),
                            SecondPaperMarks = (x.Exam.SecondPaperMarks > 0 ? (Math.Round(x.Exam.SecondPaperMarks, 1)).ToString() : "0.00"),
                            SecondPaperPercentMarks = (x.Exam.SecondPaperMarks > 0 ? ((int)((x.Exam.SecondPaperMarks / 60) * 100)).ToString()  : "00"),
                            TotalMarks = x.Exam.FirstPaperMarks + x.Exam.SecondPaperMarks,
                            RegisterdDate = x.Exam.CreatedDate?.ToString("MM/dd/yyyy")
                        }).OrderBy(x=>x.ExamID);

                return Json(new { success = result, data = pendingExams }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult MoveToMcqExam(int ExamId)
        {
            var result = false;
            try
            {
                Session["FirstPaperExamId"] = ExamId;

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
                            PaperName = (x.PastPaper.ExamTypeID == 1 ? x.Exam.ExamNo + "-O/L-" + x.Subject.Description + "-" + x.PastPaper.Year + " (First Paper)" :
                            x.PastPaper.ExamTypeID == 2 ? x.Exam.ExamNo + "-A/L" + x.Subject.Description + "-" + x.PastPaper.Year + " (First Paper)" : ""),
                             x.PastPaper.HasMcq
                        }).FirstOrDefault();

                Session["FirstPaperName"] = pendingExams.PaperName;

                result = true;

                return Json(new { success = result , hasMcq = pendingExams.HasMcq }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult MoveToSecondExam(int ExamId)
        {
            var result = false;
            try
            {
                Session["SecondPaperExamId"] = ExamId;

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

                            PaperName = ((x.PastPaper.ExamTypeID == 1 && x.PastPaper.HasThirdPaper == false) ? x.Exam.ExamNo + "-O/L-" + x.Subject.Description + "-" + x.PastPaper.Year + " (Second Paper)" :
                            (x.PastPaper.ExamTypeID == 2 && x.PastPaper.HasThirdPaper == false) ? x.Exam.ExamNo + "-A/L" + x.Subject.Description + "-" + x.PastPaper.Year + " (Second Paper)" :
                            (x.PastPaper.ExamTypeID == 1 && x.PastPaper.HasThirdPaper == true && x.Exam.IsSecondPaperCompleted == false) ? x.Exam.ExamNo + "-O/L" + x.Subject.Description + "-" + x.PastPaper.Year + " (Second Paper)" :
                            (x.PastPaper.ExamTypeID == 2 && x.PastPaper.HasThirdPaper == true && x.Exam.IsSecondPaperCompleted == false) ? x.Exam.ExamNo + "-A/L" + x.Subject.Description + "-" + x.PastPaper.Year + " (Second Paper)" :
                            (x.PastPaper.ExamTypeID == 1 && x.PastPaper.HasThirdPaper == true && x.Exam.IsSecondPaperCompleted == true) ? x.Exam.ExamNo + "-O/L" + x.Subject.Description + "-" + x.PastPaper.Year + " (Third Paper)" :
                            (x.PastPaper.ExamTypeID == 2 && x.PastPaper.HasThirdPaper == true && x.Exam.IsSecondPaperCompleted == true) ? x.Exam.ExamNo + "-A/L" + x.Subject.Description + "-" + x.PastPaper.Year + " (Third Paper)" : ""),
                             x.PastPaper.HasMap
                        }).FirstOrDefault();

                Session["SecondPaperName"] = pendingExams.PaperName;
                Session["HasMap"] = pendingExams.HasMap;

                result = true;

                return Json(new { success = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetSecondPaperTutorComment(int ExamId)
        {
            var result = false;
            try
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
                            x.Exam.TutorCommentsForSecondPaper,
                            PaperName = (x.PastPaper.ExamTypeID == 1 ? "O/L-" + x.Subject.Description + "-" + x.PastPaper.Year :
                             x.PastPaper.ExamTypeID == 2 ? "A/L" + x.Subject.Description + "-" + x.PastPaper.Year : "")
                        }).FirstOrDefault();

                result = true;

                return Json(new { success = result , comment = pendingExams.TutorCommentsForSecondPaper , paperName = pendingExams.PaperName }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetFirstPaperTutorComment(int ExamId)
        {
            var result = false;
            try
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
                            x.Exam.TutorCommentsForFirstPaper,
                            PaperName = (x.PastPaper.ExamTypeID == 1 ? "O/L-" + x.Subject.Description + "-" + x.PastPaper.Year :
                             x.PastPaper.ExamTypeID == 2 ? "A/L" + x.Subject.Description + "-" + x.PastPaper.Year : "")
                        }).FirstOrDefault();

                result = true;

                return Json(new { success = result, comment = pendingExams.TutorCommentsForFirstPaper, paperName = pendingExams.PaperName }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetSecondPaperMarks(int ExamId)
        {
            var result = false;
            try
            {
                var exam = db.Exams.Where(x => x.ExamID == ExamId).Select(x=> new {
                    x.SecondPaperMarks
                }).FirstOrDefault();

                int percentMark = (int)(exam.SecondPaperMarks / 60) * 100;

                result = true;

                return Json(new { success = result, studentMarks = exam.SecondPaperMarks, studentpercentMark = percentMark }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetMcqPaperAnswers(int ExamId)
        {
            var result = false;
            List<LoadMcqDetails> outputList = new List<LoadMcqDetails>();
            try
            {
                var exam = db.Exams.Where(x => x.ExamID == ExamId).Join(
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
                        (pp, su) => new { pp.Exam, pp.PastPaper, pp.Medium, Subject = su }).ToList().Select(x => new {
                            x.Exam.StudentLoginId,
                            x.PastPaper.ExamTypeID,
                            x.PastPaper.SubjectID,
                            x.PastPaper.Year,
                            PaperName = (x.PastPaper.ExamTypeID == 1 ? "O/L-" + x.Subject.Description + "-" + x.PastPaper.Year :
                             x.PastPaper.ExamTypeID == 2 ? "A/L" + x.Subject.Description + "-" + x.PastPaper.Year : "")
                        }).FirstOrDefault();

                var correctAnswers = db.McqDetails.Where(x => x.ExamTypeID == exam.ExamTypeID && x.SubjectID == exam.SubjectID && x.Year == exam.Year).
                    OrderBy(x => x.QuestionNo).ToList();

                foreach(var answer in correctAnswers)
                {
                    var studentAnswer = db.StudentsMcqAnswers.Where(x => x.StudentLoginId == exam.StudentLoginId && x.ExamId == ExamId && x.QuestionNo == answer.QuestionNo).FirstOrDefault();
                    if(studentAnswer != null)
                    {
                        outputList.Add(new LoadMcqDetails
                        {
                            CorrectQuestionNumber = answer.QuestionNo.ToString(),
                            CorrectAnswer = answer.Answer,
                            StudentQuestionNumber = studentAnswer.QuestionNo.ToString(),
                            StudentAnswer = studentAnswer.Answer.ToString()
                        });
                    }
                    else
                    {
                        outputList.Add(new LoadMcqDetails
                        {
                            CorrectQuestionNumber = answer.QuestionNo.ToString(),
                            CorrectAnswer = answer.Answer,
                            StudentQuestionNumber = "",
                            StudentAnswer = ""
                        });
                    }
                }

                result = true;

                return Json(new { success = result, dataList = outputList, paperName = exam.PaperName }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult CheckHasMcq(int ExamId)
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

                return Json(new { success = result, hasMcq = hasMcq }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }


        //----------------------------------------------------------------Student Payment History----------------------------------------------------------------------------------------------------------------------------------------
        public ActionResult PaymentHistory()
        {
            return View();
        }

        public JsonResult GetStudentPayments(int LoginId)
        {
            var result = false;
            try
            {
                var payments = db.Carts.Where(x => x.LoginId == LoginId && x.IsCheckOut == true).Join(
                        db.CartItems,
                        ca => ca.CartId,
                        ci => ci.CartId,
                        (ca, ci) => new { Cart = ca, CartItem = ci }).ToList().Select(x => new
                        {
                            OrderNo = "ORD" + x.Cart.CartId,
                            PaymentDate = x.Cart.PaymentDate?.ToString("MM/dd/yyyy"),
                            x.CartItem.ItemName,
                            UnitPrice = "Rs. " + x.CartItem.UnitPrice
                        });

                return Json(new { success = result, data = payments }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        //--------------------------------------------------------------------------Online Classes----------------------------------------------------------------------------------------------------------------------------------
        public ActionResult OnlineClasses()
        {
            HttpCookie controllerNameCookie = new HttpCookie("controllerName", "Dashboard");
            HttpCookie actionNameCookie = new HttpCookie("actionName", "OnlineClasses");
            controllerNameCookie.Expires.AddDays(10);
            actionNameCookie.Expires.AddDays(10);
            HttpContext.Response.SetCookie(controllerNameCookie);
            HttpContext.Response.SetCookie(actionNameCookie);
            return View();
        }

        public ActionResult LoadClasses()
        {
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                var list = db.OnlineClasses.ToList().Select(x => new
                {
                    x.ClassID,
                    x.ClassName,
                    x.Price,
                    x.Date,
                    x.Master,
                    x.Time,
                    x.Image
                });

                return Json(new { dataList = list }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult LoadOnlineClasssImage(int ClassID)
        {
            var imagefile = db.OnlineClasses.Where(x => x.ClassID == ClassID).Select(x => x.Image).FirstOrDefault();
            var path = "~/Content/img/OnlineClasses/" + imagefile;
            return File(path, "image/*");
        }

        public JsonResult AddToCartClasses(int LoginId, long ClassId)
        {
            var result = false;
            try
            {
                var cls = db.OnlineClasses.Where(x => x.ClassID == ClassId).FirstOrDefault();

                if (cls != null)
                {
                    var cartItem = new CartItem();
                    var cartDet = db.Carts.Where(x => x.LoginId == LoginId && x.IsCheckOut == false).FirstOrDefault();
                    if (cartDet != null)
                    {
                        cartDet.Total = cartDet.Total + (decimal)cls.Price;
                        cartDet.SubTotal = cartDet.SubTotal + (decimal)cls.Price;

                        cartItem.CartId = cartDet.CartId;
                        cartItem.PastPaperId = null;
                        cartItem.UnitPrice = (decimal)cls.Price;
                        cartItem.Qty = 1;
                        cartItem.ItemName = cls.ClassName;
                        db.CartItems.Add(cartItem);
                        db.SaveChanges();

                        result = true;
                    }
                    else
                    {
                        var cart = new Cart();
                        cart.LoginId = LoginId;
                        cart.IsCheckOut = false;
                        cart.Total = 0;
                        cart.SubTotal = 0;
                        db.Carts.Add(cart);
                        db.SaveChanges();

                        cartItem.CartId = cart.CartId;
                        cartItem.PastPaperId = null;
                        cartItem.UnitPrice = (decimal)cls.Price;
                        cartItem.Qty = 1;
                        cartItem.ItemName = cls.ClassName;
                        db.CartItems.Add(cartItem);

                        cart.Total = cart.Total + (decimal)cls.Price;
                        cart.SubTotal = cart.SubTotal + (decimal)cls.Price;

                        db.SaveChanges();

                        result = true;
                    }
                }

                return Json(new { success = result }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        //-------------------------------------------------------------------------Student Performance---------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public ActionResult Performance()
        {
            dynamic myModal = new ExpandoObject();
            myModal.Subjects = GetSubjectsForCombo();

            return View(myModal);
        }

        public List<Subject> GetSubjectsForCombo()
        {
            var subList = db.Subjects.ToList();
            return subList;
        }

        public JsonResult GetMainChartDetails(int StudentLoginId)
        {
            var result = false;
            try
            {
                List<ChartXY> fpMarks = new List<ChartXY>();
                List<ChartXY> spMarks = new List<ChartXY>();

                var examData = db.Exams.Where(x=>x.StudentLoginId == StudentLoginId && x.TutorStatus == "Completed" && x.StudentStatus == "Completed").GroupBy(x => x.PastPaperId).Select(x => new
                {
                    PastPaperId = x.Key,
                    Date = x.Max(y => y.AssignToTutorReadyDate)
                }).ToList();

                foreach(var data in examData)
                {
                    var exam = db.Exams.Where(x => x.PastPaperId == data.PastPaperId && x.AssignToTutorReadyDate == data.Date).Join(
                            db.Subjects,
                            ex => ex.SubjectId,
                            su => su.SubjectID,
                            (ex,su) => new { Exam = ex , Subject = su}).FirstOrDefault();
                    if(exam != null)
                    {
                        fpMarks.Add(new ChartXY { x = exam.Subject.Description.ToString(), y = exam.Exam.FirstPaperMarks.ToString() });
                        spMarks.Add(new ChartXY { x = exam.Subject.Description.ToString(), y = exam.Exam.SecondPaperMarks.ToString() });
                    }
                }

                result = true;

                return Json(new { success = result, fpMarks, spMarks }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetSubChartDetails(int StudentLoginId , int SubjectId)
        {
            var result = false;
            try
            {
                db.Configuration.ProxyCreationEnabled = false;
                List<ChartXY> fpMarks = new List<ChartXY>();
                List<ChartXY> spMarks = new List<ChartXY>();

                var examData = db.Exams.Where(x => x.StudentLoginId == StudentLoginId && x.TutorStatus == "Completed" && x.StudentStatus == "Completed" && x.SubjectId == SubjectId).Join(
                        db.PastPapers,
                        ex => ex.PastPaperId,
                        pp => pp.PastPaerID,
                        (ex, pp) => new { Exam = ex, PastPaper = pp }).Select(x => new ChartXY
                        {
                            x = x.PastPaper.Year.ToString(),
                            y = (x.Exam.FirstPaperMarks + x.Exam.SecondPaperMarks).ToString()
                        }).ToList();

                

                
 

                return Json(new { success = result, dataList = examData }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = result, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        //---------------------------------------------------------------------View My Answers-------------------------------------------------------------------------------------------------------------------------------------------------------
        public ActionResult ViewMyAnswers(int ExamId)
        {
            var exam = db.Exams.Where(x => x.ExamID == ExamId).Join(
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
                            PaperName = (x.PastPaper.ExamTypeID == 1 ? x.Exam.ExamNo + "-O/L-" + x.Subject.Description + "-" + x.PastPaper.Year + " (Second Paper)" :
                             x.PastPaper.ExamTypeID == 2 ? x.Exam.ExamNo + "-A/L" + x.Subject.Description + "-" + x.PastPaper.Year + " (Second Paper)" : "")
                        }).FirstOrDefault();

            Session["PaperExamIdForSecondPaperAnswerView"] = ExamId;
            Session["PaperNameForSecondPaperAnswerView"] = exam.PaperName;

            return View();
        }

        public ActionResult ViewAnswerImagesForFirstPaper(int ExamId)
        {
            var exam = db.Exams.Where(x => x.ExamID == ExamId).Join(
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
                            PaperName = (x.PastPaper.ExamTypeID == 1 ? x.Exam.ExamNo + "-O/L-" + x.Subject.Description + "-" + x.PastPaper.Year + " (First Paper)" :
                             x.PastPaper.ExamTypeID == 2 ? x.Exam.ExamNo + "-A/L" + x.Subject.Description + "-" + x.PastPaper.Year + " (First Paper)" : "")
                        }).FirstOrDefault();

            Session["PaperExamIdForFirstPaperAnswerView"] = ExamId;
            Session["PaperNameForFirstPaperAnswerView"] = exam.PaperName;

            return View();
        }

        //---------------------------------------------------------------------View Tutor Uploads------------------------------------------------------------------------------------------------------------------------------------------------------
        public ActionResult ViewTutorUploads(int ExamId)
        {
            var exam = db.Exams.Where(x => x.ExamID == ExamId).Join(
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
                            PaperName = (x.PastPaper.ExamTypeID == 1 ? x.Exam.ExamNo + "-O/L-" + x.Subject.Description + "-" + x.PastPaper.Year + " (Second Paper)" :
                             x.PastPaper.ExamTypeID == 2 ? x.Exam.ExamNo + "-A/L" + x.Subject.Description + "-" + x.PastPaper.Year + " (Second Paper)" : "")
                        }).FirstOrDefault();

            Session["PaperExamIdForSecondPaperTutorUploads"] = ExamId;
            Session["PaperNameForSecondPaperTutorUploads"] = exam.PaperName; ;

            return View();
        }

        public ActionResult ViewTutorUploadsForFirstPaper(int ExamId)
        {
            var exam = db.Exams.Where(x => x.ExamID == ExamId).Join(
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
                           PaperName = (x.PastPaper.ExamTypeID == 1 ? x.Exam.ExamNo + "-O/L-" + x.Subject.Description + "-" + x.PastPaper.Year + " (First Paper)" :
                            x.PastPaper.ExamTypeID == 2 ? x.Exam.ExamNo + "-A/L" + x.Subject.Description + "-" + x.PastPaper.Year + " (First Paper)" : "")
                       }).FirstOrDefault();

            Session["PaperExamIdForFirstPaperTutorUploads"] = ExamId;
            Session["PaperNameForFirstPaperTutorUploads"] = exam.PaperName; ;

            return View();
        }

        public JsonResult GetImagesForFirstPaperTutorUploads(int ExamId)
        {
            var result = false;
            try
            {
                List<SecondPaperImages> dataList = new List<SecondPaperImages>();
                var exam = db.Exams.Where(x => x.ExamID == ExamId).FirstOrDefault();
                if (exam != null)
                {
                    string folderPath = exam.FirstPaperMarkedSheetsPath;

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

        public JsonResult GetImagesForSecondPaperTutorUploads(int ExamId)
        {
            var result = false;
            try
            {
                List<SecondPaperImages> dataList = new List<SecondPaperImages>();
                var exam = db.Exams.Where(x => x.ExamID == ExamId).FirstOrDefault();
                if (exam != null)
                {
                    string folderPath = exam.SecondPaperMarkedSheetsPath;

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
    }
}