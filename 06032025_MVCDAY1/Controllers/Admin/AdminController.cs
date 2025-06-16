using _06032025_MVCDAY1.Models;
using _06032025_MVCDAY1.Repository;
using ClosedXML.Excel;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Microsoft.AspNetCore.Mvc;
using Rotativa;
using Rotativa.AspNetCore;
using System.Reflection;

namespace _06032025_MVCDAY1.Controllers.Admin
{
    public class AdminController : Controller
    {
        private readonly IAdminRepository _Repo;
        public AdminController(IAdminRepository categoryRepo)
        {
            _Repo = categoryRepo;
        }

        
        public IActionResult Index()
        {
            var data = _Repo.GetDashboardData();
            return View(data);
        }
        public IActionResult Orders()
        {
            return View();
            
        }
        [HttpGet]
        public IActionResult filterorder(string status)
        {
            var orders = _Repo.GetAllOrders(status);
            return Json(orders);
        }

        public IActionResult Products()
        {
            var products = _Repo.GetAllProducts();
            return View(products);
        }

        //category show
        public IActionResult GetAllcategory()
        {
            return View();
        }
        [HttpGet]
        
        public IActionResult GetAllcategory1()
        {
            var cate = _Repo.GetAll();
            return Json(cate);
        }
        [HttpGet]
        public IActionResult GetCategoryById(int id)
        {
            var category = _Repo.GetById(id);
            return Json(category);
        }

        [HttpPost]
        public IActionResult SaveCategory(Category model)
        {
            if (model.category_id == 0)
                _Repo.Add(model);
            else
                _Repo.Update(model);

            return Json(new { success = true });
        }

        [HttpPost]
        public JsonResult DeleteCategory(int id)
        {
            try
            {
                _Repo.Delete(id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        //Brands show

        public IActionResult GetAllBrands()
        {
            return View();
        }
        [HttpGet]

        public IActionResult GetAllBrands1()
        {
            var brand = _Repo.GetAllBrands();
            return Json(brand);
        }
        [HttpGet]
        public IActionResult GetBrandById(int id)
        {
            var brand = _Repo.BrandGetById(id);
            return Json(brand);
        }

        [HttpPost]
        public IActionResult SaveBrand(Brands model)
        {
            if (model.brand_id == 0)
                _Repo.AddBrand(model);
            else
                _Repo.UpdateBrand(model);

            return Json(new { success = true });
        }

        [HttpPost]
        public JsonResult DeleteBrand(int id)
        {
            try
            {
                _Repo.DeleteBrand(id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        //SubCategory show
        public IActionResult GetAllsubcategory()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetAllSubCategory1()
        {
            var data = _Repo.GetAllSubCategory();
            return Json(data);
        }
        [HttpGet]
        public JsonResult GetSubCategoryById(int id)
        {
            var data = _Repo.subcategoryGetById(id);
            return Json(data);
        }
        [HttpPost]
        public JsonResult SaveSubCategory(Subcategory subcategory)
        {
            _Repo.Savesubcategory(subcategory);
            return Json(new { success = true });
        }

        [HttpPost]
        public JsonResult DeleteSubCategory(int id)
        {
            _Repo.Deletesubcategory(id);
            return Json(new { success = true });
        }



        public IActionResult ProductApproval()
        {
            var products = _Repo.GetVendorProductApproval();
            return View(products);
        }

        public ActionResult ApproveProduct(int id)
        {
            _Repo.ApproveProduct(id);
            return RedirectToAction("ProductApproval");
        }

        public IActionResult RejectProduct(int id)
        {
            _Repo.RejectProduct(id);
            return RedirectToAction("ProductApproval");
        }

        public ActionResult PaymentHistory()
        {
            var payments = _Repo.GetAllPaymentsWithOrders();
            return View(payments);
        }

        public ActionResult GetOrderItems(int orderId)
        {
            var items = _Repo.GetOrderItemsByOrderId(orderId); // implement this in repo
            return PartialView("_OrderItemsPartial", items);
        }

        public IActionResult ExportPaymentsToExcel()
        {
            var payments = _Repo.GetAllPaymentsWithOrders(); // You already have this method or create one

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Payments");
                var row = 1;

                // Header
                worksheet.Cell(row, 1).Value = "Payment ID";
                worksheet.Cell(row, 2).Value = "Order ID";
                worksheet.Cell(row, 3).Value = "Amount";
                worksheet.Cell(row, 4).Value = "UserID";
                worksheet.Cell(row, 5).Value = "Paid On";
                worksheet.Cell(row, 6).Value = "Razorpay Order ID";

                // Data
                foreach (var p in payments)
                {
                    row++;
                    worksheet.Cell(row, 1).Value = p.PaymentId;
                    worksheet.Cell(row, 2).Value = p.OrderId;
                    worksheet.Cell(row, 3).Value = p.Amount;
                    worksheet.Cell(row, 4).Value = p.UserId;
                    worksheet.Cell(row, 5).Value = p.PaidOn.ToString("yyyy-MM-dd");
                    worksheet.Cell(row, 6).Value = p.RazorpayOrderId;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Payments.xlsx");
                }
            }
        }

        public IActionResult ExportPaymentsToPdf()
        {
            var payments = _Repo.GetAllPaymentsWithOrders(); // Your list of payments
            MemoryStream ms = new MemoryStream();

            Document document = new Document(PageSize.A4, 10f, 10f, 10f, 10f);
            PdfWriter.GetInstance(document, ms);
            document.Open();

            // Title
            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
            Paragraph title = new Paragraph("Payment History Report", titleFont)
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingAfter = 20f
            };
            document.Add(title);

            // Table with 5 columns
            PdfPTable table = new PdfPTable(5)
            {
                WidthPercentage = 100
            };
            table.SetWidths(new float[] { 10, 15, 15, 20, 25 });

            // Header row
            string[] headers = { "Payment ID", "Order ID", "Amount", "Paid On", "Razorpay Order ID" };
            foreach (var header in headers)
            {
                PdfPCell cell = new PdfPCell(new Phrase(header, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12)))
                {
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    BackgroundColor = BaseColor.LIGHT_GRAY
                };
                table.AddCell(cell);
            }

            // Add data rows
            foreach (var p in payments)
            {
                table.AddCell(p.PaymentId.ToString());
                table.AddCell(p.OrderId.ToString());
                table.AddCell("₹" + p.Amount.ToString("F2"));
                table.AddCell(p.PaidOn.ToString("yyyy-MM-dd"));
                table.AddCell(p.RazorpayOrderId);
            }

            document.Add(table);
            document.Close();

            byte[] bytes = ms.ToArray();
            ms.Close();

            return File(bytes, "application/pdf", "Payment_History.pdf");
        }

        public IActionResult ExportPdfView()
        {
            return View();
        }

        public IActionResult OrderReport(DateTime? fromDate, DateTime? toDate, string orderStatus)
        {
            var orders = _Repo.GetFilteredOrders(fromDate, toDate, orderStatus);

            var model = new OrderReportViewModel
            {
                FromDate = fromDate,
                ToDate = toDate,
                OrderStatus = orderStatus,
                Orders = orders
            };

            return View(model);
        }

        public ActionResult ExportToPDF(DateTime? fromDate, DateTime? toDate, string orderStatus)
        {
            var orders = _Repo.GetFilteredOrders(fromDate, toDate, orderStatus);

            using (MemoryStream ms = new MemoryStream())
            {
                Document doc = new Document(PageSize.A4.Rotate(), 25, 25, 30, 30);
                PdfWriter.GetInstance(doc, ms);
                doc.Open();

                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
                var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.WHITE);
                var cellFont = FontFactory.GetFont(FontFactory.HELVETICA, 11, BaseColor.BLACK);
                var grandTotalFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.BLACK);

                Paragraph title = new Paragraph("Order Report", titleFont)
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 20f
                };
                doc.Add(title);

                PdfPTable table = new PdfPTable(5)
                {
                    WidthPercentage = 100,
                    SpacingBefore = 10f,
                    SpacingAfter = 10f
                };
                table.SetWidths(new float[] { 1.2f, 1.8f, 1.8f, 1.5f, 1.8f});

                BaseColor headerBgColor = new BaseColor(52, 73, 94);
                string[] headers = {
            "Order ID", "User ID", "Date", "Amount", "Status"
        };

                foreach (var header in headers)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(header, headerFont))
                    {
                        BackgroundColor = headerBgColor,
                        HorizontalAlignment = Element.ALIGN_CENTER,
                        Padding = 6
                    };
                    table.AddCell(cell);
                }

                decimal grandTotal = 0;

                foreach (var order in orders)
                {
                    table.AddCell(new PdfPCell(new Phrase(order.Id.ToString(), cellFont)) { Padding = 5 });
                    table.AddCell(new PdfPCell(new Phrase(order.UserId.ToString(), cellFont)) { Padding = 5 });
                    table.AddCell(new PdfPCell(new Phrase(order.CreatedDate.ToString("dd-MM-yyyy"), cellFont)) { Padding = 5 });
                    table.AddCell(new PdfPCell(new Phrase("₹" + order.TotalAmount.ToString("F2"), cellFont)) { Padding = 5 });
                    table.AddCell(new PdfPCell(new Phrase(order.Status, cellFont)) { Padding = 5 });
                    

                    grandTotal += order.TotalAmount;
                }

                // Add Grand Total row inside the table
                PdfPCell totalLabelCell = new PdfPCell(new Phrase("Grand Total", grandTotalFont))
                {
                    Colspan = 3,
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    Padding = 8,
                    BackgroundColor = new BaseColor(230, 230, 230)
                };
                table.AddCell(totalLabelCell);

                PdfPCell totalValueCell = new PdfPCell(new Phrase("₹" + grandTotal.ToString("F2"), grandTotalFont))
                {
                    Colspan = 5,
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    Padding = 8,
                    BackgroundColor = new BaseColor(230, 230, 230)
                };
                table.AddCell(totalValueCell);

                doc.Add(table);
                doc.Close();

                return File(ms.ToArray(), "application/pdf", "OrderReport.pdf");
            }
        }

        public ActionResult ExportToExcel(DateTime? fromDate, DateTime? toDate, string orderStatus)
        {
            var orders = _Repo.GetFilteredOrders(fromDate, toDate, orderStatus);

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Order Report");

                // Header row
                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "Order ID";
                worksheet.Cell(currentRow, 2).Value = "User ID";
                worksheet.Cell(currentRow, 3).Value = "Date";
                worksheet.Cell(currentRow, 4).Value = "Amount";
                worksheet.Cell(currentRow, 5).Value = "Status";
               

                // Style for header
                for (int i = 1; i <= 5; i++)
                {
                    worksheet.Cell(1, i).Style.Font.Bold = true;
                    worksheet.Cell(1, i).Style.Fill.BackgroundColor = XLColor.DarkBlue;
                    worksheet.Cell(1, i).Style.Font.FontColor = XLColor.White;
                    worksheet.Cell(1, i).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                }

                // Data rows
                decimal grandTotal = 0;
                foreach (var order in orders)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = order.Id;
                    worksheet.Cell(currentRow, 2).Value = order.UserId;
                    worksheet.Cell(currentRow, 3).Value = order.CreatedDate.ToString("dd-MM-yyyy");
                    worksheet.Cell(currentRow, 4).Value = order.TotalAmount;
                    worksheet.Cell(currentRow, 5).Value = order.Status;
                   

                    grandTotal += order.TotalAmount;
                }

                // Grand Total Row
                currentRow++;
                worksheet.Cell(currentRow, 1).Value = "Grand Total";
                worksheet.Range(currentRow, 1, currentRow, 3).Merge();
                worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
                worksheet.Cell(currentRow, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                worksheet.Cell(currentRow, 4).Value = grandTotal;
                worksheet.Cell(currentRow, 4).Style.Font.Bold = true;
                worksheet.Cell(currentRow, 4).Style.NumberFormat.Format = "₹#,##0.00";
                worksheet.Column(4).AdjustToContents();
                // Auto-fit all columns
                worksheet.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0;
                    return File(stream.ToArray(),
                                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                "OrderReport.xlsx");
                }
            }
        }

        public IActionResult PayOut()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetWeeklySales(int month,int year)
        {
            var weeklyData = _Repo.GetWeeklySales(month, year);

            decimal payout = weeklyData.Sum(w => w.Payout);
            decimal pending = weeklyData.Sum(w => w.Pending);
            decimal totalSales = payout + pending;

            return Json(new
            {
                Weekly = weeklyData,
                TotalSales = totalSales,
                Payout = payout,
                Pending = pending
            });
        }
        public ActionResult DownloadPayoutPdf(int month, int year)
        {
            var summary = _Repo.GetPayoutSummary(month, year);

            using (MemoryStream ms = new MemoryStream())
            {
                Document doc = new Document(PageSize.A4, 40, 40, 50, 50);
                PdfWriter writer = PdfWriter.GetInstance(doc, ms);

                // Footer with page number
                writer.PageEvent = new PdfPageEvents();

                doc.Open();

                // Title
                Font titleFont = FontFactory.GetFont("Arial", 22, Font.BOLD, new BaseColor(0, 70, 127));
                Paragraph title = new Paragraph($"Payout Summary - {summary.MonthName} {year}", titleFont)
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 20f
                };
                doc.Add(title);

                // Summary Cards Table
                PdfPTable summaryTable = new PdfPTable(3)
                {
                    WidthPercentage = 100,
                    SpacingAfter = 25f
                };
                summaryTable.SetWidths(new float[] { 1f, 1f, 1f });

                Font cardTitleFont = FontFactory.GetFont("Arial", 14, Font.BOLD, BaseColor.WHITE);
                Font cardValueFont = FontFactory.GetFont("Arial", 16, Font.BOLD, BaseColor.WHITE);

                BaseColor blue = new BaseColor(10, 99, 177);
                BaseColor green = new BaseColor(39, 174, 96);
                BaseColor orange = new BaseColor(243, 156, 18);

                // Total Sales Card
                PdfPCell totalSalesCell = new PdfPCell();
                totalSalesCell.BackgroundColor = blue;
                totalSalesCell.Padding = 15;
                totalSalesCell.AddElement(new Paragraph("Total Sales", cardTitleFont));
                totalSalesCell.AddElement(new Paragraph($"₹{summary.TotalSales:N2}", cardValueFont));
                totalSalesCell.HorizontalAlignment = Element.ALIGN_CENTER;
                totalSalesCell.Border = Rectangle.NO_BORDER;
                summaryTable.AddCell(totalSalesCell);

                // Overall Payout Card
                PdfPCell payoutCell = new PdfPCell();
                payoutCell.BackgroundColor = green;
                payoutCell.Padding = 15;
                payoutCell.AddElement(new Paragraph("Overall Payout", cardTitleFont));
                payoutCell.AddElement(new Paragraph($"₹{summary.Payout:N2}", cardValueFont));
                payoutCell.HorizontalAlignment = Element.ALIGN_CENTER;
                payoutCell.Border = Rectangle.NO_BORDER;
                summaryTable.AddCell(payoutCell);

                // Pending Payout Card
                PdfPCell pendingCell = new PdfPCell();
                pendingCell.BackgroundColor = orange;
                pendingCell.Padding = 15;
                pendingCell.AddElement(new Paragraph("Pending Payout", cardTitleFont));
                pendingCell.AddElement(new Paragraph($"₹{summary.Pending:N2}", cardValueFont));
                pendingCell.HorizontalAlignment = Element.ALIGN_CENTER;
                pendingCell.Border = Rectangle.NO_BORDER;
                summaryTable.AddCell(pendingCell);

                doc.Add(summaryTable);

                // Weekly Sales Table
                PdfPTable table = new PdfPTable(2)
                {
                    WidthPercentage = 100,
                    SpacingBefore = 10f,
                    SpacingAfter = 20f
                };
                table.SetWidths(new float[] { 1f, 2f });

                Font headerFont = FontFactory.GetFont("Arial", 13, Font.BOLD, BaseColor.WHITE);
                BaseColor headerBgColor = new BaseColor(10, 99, 177);

                // Header
                PdfPCell weekHeader = new PdfPCell(new Phrase("Week", headerFont))
                {
                    BackgroundColor = headerBgColor,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Padding = 8
                };
                PdfPCell salesHeader = new PdfPCell(new Phrase("Sales (₹)", headerFont))
                {
                    BackgroundColor = headerBgColor,
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Padding = 8
                };
                table.AddCell(weekHeader);
                table.AddCell(salesHeader);

                // Data Rows
                Font rowFont = FontFactory.GetFont("Arial", 12, Font.NORMAL, BaseColor.BLACK);

                foreach (var week in summary.Weekly)
                {
                    PdfPCell weekCell = new PdfPCell(new Phrase($"Week {week.WeekNumber}", rowFont))
                    {
                        Padding = 6,
                        HorizontalAlignment = Element.ALIGN_CENTER
                    };
                    PdfPCell salesCell = new PdfPCell(new Phrase($"₹{week.TotalSales:N2}", rowFont))
                    {
                        Padding = 6,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    };

                    // Add border bottom
                    weekCell.BorderWidthBottom = 0.5f;
                    salesCell.BorderWidthBottom = 0.5f;

                    table.AddCell(weekCell);
                    table.AddCell(salesCell);
                }

                doc.Add(table);

                doc.Close();

                byte[] byteInfo = ms.ToArray();
                return File(byteInfo, "application/pdf", $"Payout_Summary_{summary.MonthName}_{year}.pdf");
            }
        }

        // Custom page event handler for footer
        public class PdfPageEvents : PdfPageEventHelper
        {
            public override void OnEndPage(PdfWriter writer, Document document)
            {
                base.OnEndPage(writer, document);
                var cb = writer.DirectContent;
                cb.BeginText();
                var font = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                cb.SetFontAndSize(font, 10);
                cb.SetTextMatrix(document.PageSize.Width - 100, 30);
                cb.ShowText($"Page {writer.PageNumber}");
                cb.EndText();
            }
        }


    }
}
