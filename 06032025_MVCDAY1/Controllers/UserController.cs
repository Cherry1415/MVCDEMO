using _06032025_MVCDAY1.Models;
using _06032025_MVCDAY1.Repository;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using System.Net.Mail;
using System.Net;
using System.Reflection;
using System.Text.Json;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using iTextSharp.text.pdf;
using iTextSharp.text;

namespace _06032025_MVCDAY1.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserRepository _repo;
        private readonly IOrdersRepository _orderrepo;

        public UserController(IUserRepository repository, IOrdersRepository orderrepo)
        {
            _repo = repository;
            _orderrepo = orderrepo;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult UserHomePage()
        {
            ViewData["Layout"] = "~/Views/Shared/_HomeLayout.cshtml";
            return View();
        }

        public IActionResult EmailVerify()
        {
            return View();
        }

        [HttpPost]
        public IActionResult EmailVerify(OTPModel model)
        {
            if (string.IsNullOrEmpty(model.Email))
                return Json(new { success = false, message = "Email is required." });

            string otp = GenerateOTP();
            HttpContext.Session.SetString("UserEmail", model.Email);
            _repo.SaveOTP(model.Email, otp);
            SendEmail(model.Email, otp);

            return Json(new { success = true, message = "OTP sent to your email.",email=model.Email});
        }
        private string GenerateOTP()
        {
            return new Random().Next(100000, 999999).ToString();
        }

        private void SendEmail(string toEmail, string otp)
        {
            var fromEmail = "chigipatel8887@gmail.com";
            var fromPassword = "cxyrdbgmsbedgjlo";

            var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = "Your OTP Code",
                Body = $"<h3>Your OTP is: {otp}</h3><p>Valid for 1 minute only.</p>",
                IsBodyHtml = true
            };

            var smtp = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(fromEmail, fromPassword),
                EnableSsl = true
            };

            smtp.Send(message);
        }
        public IActionResult VerifyOTP()
        {
            return View();
        }

        [HttpPost]
        public IActionResult VerifyOTP(string otp, string email)
        {
            if (string.IsNullOrEmpty(email))
                email = HttpContext.Session.GetString("UserEmail");

            if (_repo.ValidateOTP(email, otp))
            {
                return Json(new { success = true, message = "OTP verified successfully.", redirectUrl = Url.Action("Register", "User") });
            }
            else
            {
                return Json(new { success = false, message = "Invalid or expired OTP." });
            }
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForgotPassword(ForgotPasswordModel model)
        {
            if (model.Password != model.ConfirmPassword)
            {
                TempData["Error"] = "Passwords do not match.";
                return View(model);
            }

            string useremail = HttpContext.Session.GetString("User_Email");
            if (string.IsNullOrEmpty(useremail))
            {
                TempData["Error"] = "Email session expired. Please verify again.";
                return RedirectToAction("EmailVerifyForPass");
            }
            _repo.updatepassword(model.ConfirmPassword,useremail);
            return RedirectToAction("SignIn");
        }
        public IActionResult EmailVerifyForPass()
        {
            return View();
        }

        [HttpPost]
        public IActionResult EmailVerifyForPass(OTPModel model)
        {
            
            if (string.IsNullOrEmpty(model.Email))
                return Json(new { success = false, message = "Email is required." });

            string otp = GenerateOTP();
            HttpContext.Session.SetString("User_Email", model.Email);
            _repo.SaveOTP(model.Email, otp);
            SendEmail(model.Email, otp);

            return Json(new { success = true, message = "OTP sent to your email.", email = model.Email });
        }
        public IActionResult VerifyOTPForPass()
        {
            return View();
        }

        [HttpPost]
        public IActionResult VerifyOTPForPass(string otp, string email)
        {
            if (string.IsNullOrEmpty(email))
                email = HttpContext.Session.GetString("UserEmail");

            if (_repo.ValidateOTP(email, otp))
            {
                return Json(new { success = true, message = "OTP verified successfully.", redirectUrl = Url.Action("ForgotPassword", "User") });
            }
            else
            {
                return Json(new { success = false, message = "Invalid or expired OTP." });
            }
        }
        public IActionResult Register()
        {

            ViewBag.roles = _repo.GetRoles();
            return View();
        }
        [HttpPost]
        public IActionResult Register(User user)
        {
            var email= HttpContext.Session.GetString("UserEmail");
            user.email = email;
            bool res = _repo.Register(user);
            if (res)
            {
                TempData["Success"] = "Registration successful!";
                return RedirectToAction("SignIn");
            }
            else
            {
                TempData["Error"] = "Registration failed!";
            }
            ViewBag.roles = _repo.GetRoles();
            return View(user);
        }
        [HttpGet]
        public IActionResult SignIn(int? role)
        {
            if (role != null)
            {
                HttpContext.Session.SetInt32("Temp_Role", role ?? 2);
            }
            return View();
        }

        [HttpPost]
        public IActionResult SignIn(string email, string password)
        {
            if (_repo.Login(email, password))
            {
                var user = _repo.getSessionData(email);
                int roleFromDB = user.Role_ID;

                // Optional: Validate role from session (if someone used vendor login link)
                int tempRole = HttpContext.Session.GetInt32("Temp_Role") ?? 2;
                if (tempRole != roleFromDB)
                {
                    TempData["Error"] = "You are not allowed to login as this user type.";
                    return View();
                }
                
                var cookieData = new UserCookieModel
                {
                    user_id = user.user_id,
                    name = user.first_name,
                    role = user.Role_ID
                };
                string jsonData = JsonSerializer.Serialize(cookieData);

                CookieOptions options = new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(30), // Keep cookie for 30 days
                    HttpOnly = false,  // So JS can access it
                    Secure = false,    // Set true if using https
                    SameSite = SameSiteMode.Lax // This prevents CSRF attacks
                };

                Response.Cookies.Append("UserData", jsonData, options);
                string fname = user.first_name;
                int uid = user.user_id;
                var wishitem = _repo.GetUserWishlist(uid);
                HttpContext.Session.SetString("email", email);
                HttpContext.Session.SetString("first_name", fname);
                HttpContext.Session.SetString("Role_ID", roleFromDB.ToString());
                HttpContext.Session.SetString("user_id", uid.ToString());
                HttpContext.Session.SetString("wishlist", string.Join(",", wishitem));

                if (roleFromDB == 3)
                    return RedirectToAction("SellerHome", "Seller");
                else if (roleFromDB == 4)
                    return RedirectToAction("Index", "Supplier");

                return RedirectToAction("HomeDashBoard", "DashBoard");
            }
            else
            {
                TempData["Error"] = "Invalid Credentials!!";
                return View();
            }

        }


        public ActionResult ProductWishList()
        {
            int uid = Convert.ToInt32(HttpContext.Session.GetString("user_id"));
            List<Product> wishlist = _repo.GetUserWishlist(uid);

            foreach (var wish in wishlist)
            {
                wish.ProductImages = _repo.GetImagesByProductId(wish.product_id);
            }
            return View(wishlist);

        }

        //Customer WishList
        public ActionResult AddToWishlist()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddToWishlist(int userid, int productId)

        {
            //int uid = Convert.ToInt32(HttpContext.Session.GetString("user_id"));
            //  int uid = Convert.ToInt32(HttpContext.Session.GetString("user_id"));
            if (userid == 0)
            {
                TempData["Error"] = "Please log in First to add Wishlist!!!!";
                return RedirectToAction("SignIn", "User");
            }
            bool isInwishlist = _repo.IsInWishlist(productId, userid);

            if (isInwishlist)
            {
                _repo.RemoveFromWishlist(userid, productId);
                return Json(new { success = true, action = "removed" });
            }
            else
            {
                _repo.AddToWishlist(userid, productId);
                TempData["Message"] = "Item added to wishlist.";
            }
            var wishlist = _repo.GetUserWishlist(userid);
            //HttpContext.Session.SetString("Wishlist", string.Join(",", wishlist));
            return Content(""); // Redirect back to product listing
        }



        [HttpPost]
        public ActionResult togglewishlist(int prodid)
        {
            int uid = Convert.ToInt32(HttpContext.Session.GetString("user_id"));
            if (uid == 0)
            {
                TempData["Error"] = "Please log in First to add Wishlist!!";
            }

            bool isInwishlist = _repo.IsInWishlist(prodid, uid);

            if (isInwishlist)
            {

            }
            else
            {
                _repo.AddToWishlist(uid, prodid);
            }
            return Content("");
        }

        //customer addresses

        public IActionResult SelectAddress()
        {

            int userId = Convert.ToInt32(HttpContext.Session.GetString("user_id"));
            var addresses = _repo.GetAddressesByUserId(userId);
            return View(addresses);
        }

        [HttpPost]
        public IActionResult AddAddress(AddressViewModel model)
        {
            int userId = Convert.ToInt32(HttpContext.Session.GetString("user_id"));
            _repo.AddAddress(userId, model);
            return RedirectToAction("SelectAddress");
        }

        [HttpPost]
        public IActionResult SelectAddress(int AddressId)
        {
            HttpContext.Session.SetInt32("selected_address", AddressId);
            return View(); // or wherever next
        }

        public IActionResult MyOrders()
        {
            int userId = Convert.ToInt32(HttpContext.Session.GetString("user_id"));
            var orders = _orderrepo.GetUserOrdersWithItemsAndImages(userId);
            return View(orders);
        }

        //product reviews
        [HttpPost]
        public IActionResult SubmitReview(int productId, int rating, string review)
        {
            int userId = Convert.ToInt32(HttpContext.Session.GetString("user_id"));
            var newReview = new ProductReview
            {
                ProductId = productId,
                UserId = userId,
                Rating = rating,
                Review = review,
                CreatedDate = DateTime.Now
            };

            _repo.SubmitReview(newReview);

            return Json(new { success = true, message = "Review submitted successfully!" });
        }
        public IActionResult GenerateBill(int orderId)
        {
            var order = _repo.GetOrderById(orderId);
            var items = _repo.GetOrderItemsByOrderId(orderId);
            var user = _repo.GetUserById(order.UserId);
            var address = _repo.GetAddressById(order.ordered_addressid);

            using (MemoryStream stream = new MemoryStream())
            {
                Document doc = new Document(PageSize.A4, 40, 40, 60, 50);
                PdfWriter writer = PdfWriter.GetInstance(doc, stream);
                writer.CloseStream = true; // Let it close the stream
                doc.Open();

                // --- Header ---
                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 20, BaseColor.BLACK);
                var subFont = FontFactory.GetFont(FontFactory.HELVETICA, 12, BaseColor.DARK_GRAY);

                Paragraph header = new Paragraph("🧾 DealBazar Billing Invoice", titleFont);
                header.Alignment = Element.ALIGN_CENTER;
                doc.Add(header);

                Paragraph date = new Paragraph("Date: " + order.CreatedDate.ToString("dd MMM yyyy"), subFont);
                date.Alignment = Element.ALIGN_CENTER;
                doc.Add(date);
                doc.Add(new Paragraph("\n"));

                // --- Customer Info ---
                var infoFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);
                var boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);

                PdfPTable userTable = new PdfPTable(2);
                userTable.WidthPercentage = 100;
                userTable.SetWidths(new float[] { 30f, 70f });

                userTable.AddCell(new PdfPCell(new Phrase("Customer Name:", boldFont)) { Border = 0 });
                userTable.AddCell(new PdfPCell(new Phrase($"{user.User_name}", infoFont)) { Border = 0 });

                userTable.AddCell(new PdfPCell(new Phrase("Email:", boldFont)) { Border = 0 });
                userTable.AddCell(new PdfPCell(new Phrase(user.email, infoFont)) { Border = 0 });

                userTable.AddCell(new PdfPCell(new Phrase("Phone:", boldFont)) { Border = 0 });
                userTable.AddCell(new PdfPCell(new Phrase(user.phone, infoFont)) { Border = 0 });

                userTable.AddCell(new PdfPCell(new Phrase("Delivery Address:", boldFont)) { Border = 0 });
                userTable.AddCell(new PdfPCell(new Phrase($"{address.Name}, {address.Street}, {address.City}, {address.ZipCode}", infoFont)) { Border = 0 });

                userTable.SpacingAfter = 20;
                doc.Add(userTable);

                // --- Product Table ---
                PdfPTable table = new PdfPTable(4);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 50f, 15f, 15f, 20f });

                var tableHeader = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.WHITE);
                BaseColor headerBg = new BaseColor(44, 62, 80);

                void AddHeaderCell(string text) =>
                    table.AddCell(new PdfPCell(new Phrase(text, tableHeader)) { BackgroundColor = headerBg, HorizontalAlignment = Element.ALIGN_CENTER });

                AddHeaderCell("Product");
                AddHeaderCell("Qty");
                AddHeaderCell("Price");
                AddHeaderCell("Total");

                decimal grandTotal = 0;
                var rowFont = FontFactory.GetFont(FontFactory.HELVETICA, 11);

                foreach (var item in items)
                {
                    decimal total = item.Price * item.Quantity;
                    grandTotal += total;

                    table.AddCell(new PdfPCell(new Phrase(item.product_name, rowFont)));
                    table.AddCell(new PdfPCell(new Phrase(item.Quantity.ToString(), rowFont)) { HorizontalAlignment = Element.ALIGN_CENTER });
                    table.AddCell(new PdfPCell(new Phrase("₹" + item.Price.ToString("0.00"), rowFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                    table.AddCell(new PdfPCell(new Phrase("₹" + total.ToString("0.00"), rowFont)) { HorizontalAlignment = Element.ALIGN_RIGHT });
                }

                doc.Add(table);

                doc.Add(new Paragraph("\n"));

                // --- Total Amount ---
                var totalFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14);
                Paragraph totalAmount = new Paragraph($"Total Amount: ₹{grandTotal:0.00}", totalFont)
                {
                    Alignment = Element.ALIGN_RIGHT
                };
                doc.Add(totalAmount);

                doc.Add(new Paragraph("\n"));

                // --- Footer ---
                var footerFont = FontFactory.GetFont(FontFactory.HELVETICA_OBLIQUE, 10, BaseColor.GRAY);
                Paragraph thanks = new Paragraph("Thank you for shopping with DealBazar!", footerFont);
                thanks.Alignment = Element.ALIGN_CENTER;
                doc.Add(thanks);

                doc.Close();

                // 📌 FIX: Copy to new stream and return (prevent ObjectDisposedException)
                byte[] pdfBytes = stream.ToArray();
                return File(new MemoryStream(pdfBytes), "application/pdf", $"Bill_Order_{orderId}.pdf");
            }
        }




    }
}