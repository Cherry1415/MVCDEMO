using _06032025_MVCDAY1.Models;
using _06032025_MVCDAY1.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Razorpay.Api;
using System.Transactions;


namespace _06032025_MVCDAY1.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IOrdersRepository _orderRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly RazorPayKeys _razorPayKeys;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(ILogger<PaymentController> logger,RazorPayKeys razorPayKeys,IOrdersRepository repository,IPaymentRepository paymentRepository)
        {
            _logger = logger;
            _razorPayKeys = razorPayKeys;
            _orderRepository = repository;
            _paymentRepository = paymentRepository;
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
        public IActionResult InitiateOrder([FromBody] PaymentInitiateModel m)
        {
            int uid = Convert.ToInt32(HttpContext.Session.GetString("user_id"));
            try
            {
                int finalAmount = m.Amount * 100;

                RazorpayClient client = new RazorpayClient(_razorPayKeys.KeyID, _razorPayKeys.KeySecret);
                Dictionary<string,object> options = new Dictionary<string, object>
        {
            { "amount", finalAmount },
            { "currency", "INR" },
            { "receipt", "order_rcptid_14" },
            { "payment_capture", 1 }
        };

                Order order = client.Order.Create(options);
                string razorpayOrderId = order["id"].ToString();
                Console.WriteLine($"Order has {m.orderItems?.Count ?? 0} items.");
                // Make sure you validate and create order
                var createdOrder = _orderRepository.CreateOrder(uid, m.Amount, razorpayOrderId,m.orderItems);

                return Json(new {  orderId = razorpayOrderId });
            }
            catch (Exception ex)
            {
                // Log the exception if needed (e.g., to file or database)
                return Json(new { error= ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Success([FromBody] PaymentSuccessModel paymentResponse)
        {
            int uid = Convert.ToInt32(HttpContext.Session.GetString("user_id"));
            try
            {
                Console.WriteLine("Payment Response received:");
                Console.WriteLine("PaymentId: " + paymentResponse.razorpay_payment_id);
                Console.WriteLine("OrderId: " + paymentResponse.razorpay_order_id);
                Console.WriteLine("Signature: " + paymentResponse.razorpay_signature);
                Console.WriteLine("Amount: " + paymentResponse.amount);
                var signature = GetSHA256Signature(paymentResponse.razorpay_order_id + "|" + paymentResponse.razorpay_payment_id, _razorPayKeys.KeySecret);

                if (signature == paymentResponse.razorpay_signature)
                {
                    // Save payment in DB
                    _paymentRepository.SavePayment(paymentResponse.razorpay_order_id, paymentResponse.razorpay_payment_id, paymentResponse.amount, "Paid");

                    // Update Order Status
                    _orderRepository.UpdateOrderStatus(paymentResponse.razorpay_order_id, "Paid");
                    _orderRepository.ClearCart(uid);

                    return Json(new { success = true });       
                }

                return Json(new { success = false, message = "Payment verification failed" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
            
        }
        private string GetSHA256Signature(string text, string key)
        {
            var encoding = new System.Text.UTF8Encoding();
            byte[] keyBytes = encoding.GetBytes(key);
            byte[] textBytes = encoding.GetBytes(text);

            using (var hmac = new System.Security.Cryptography.HMACSHA256(keyBytes))
            {
                byte[] hashBytes = hmac.ComputeHash(textBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        public IActionResult ThankYou()
        {
            return View();
        }

    }
}
