using _06032025_MVCDAY1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Razorpay.Api;
using System.Transactions;


namespace _06032025_MVCDAY1.Controllers
{
    public class PaymentController : Controller
    {
        private readonly RazorPayKeys _razorPayKeys;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(ILogger<PaymentController> logger,RazorPayKeys razorPayKeys)
        {
            _logger = logger;
            _razorPayKeys = razorPayKeys;
        }

        //  public PaymentInitiateModel paydetails { get; set; }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult PaymentProduct()
        {
            return View();
        }
        public IActionResult CheckOutSection()
        {
            return View();
        }
        [HttpPost]
        public IActionResult InitiateOrder([FromBody] PaymentInitiateModel amount)
        {
            //
            try
            {
                int finalamount = amount.amount * 100;
                RazorpayClient client = new RazorpayClient(_razorPayKeys.KeyID, _razorPayKeys.KeySecret);
                Dictionary<string, object> options = new Dictionary<string, object>
                {
                    {"amount", finalamount},  // Amount will in paise
                   
                    { "currency", "INR"},
                    { "receipt", "order_rcptid_14"},
                    { "payment_capture", 1}

                };
                Order order = client.Order.Create(options);
                return Json(new { orderId = order["id"].ToString() });

            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        public IActionResult Success(string paymentId, string razorpay_order_id, string razorpay_signature, int productId)
        {
            ViewBag.PaymentId = paymentId;
            return View();
        }
        
    }
}
