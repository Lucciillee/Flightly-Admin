using System.Security.Claims;
using System.Text;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectWebApp.Model;
using ProjectWebApp.ViewModels;

namespace ProjectWebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminReportsController : Controller
    {
        private readonly FlightlyDBContext _context;

        // Report type codes from dbo.ReportTypes
        private const string RT_TIME_RANGE = "TIME_RANGE";
        private const string RT_AIRLINE = "AIRLINE";
        private const string RT_ORIGIN = "ORIGIN";
        private const string RT_DESTINATION = "DESTINATION";
        private const string RT_CUSTOMER = "CUSTOMER";

        public AdminReportsController(FlightlyDBContext context)
        {
            _context = context;
        }

        // GET: Reports page
        public async Task<IActionResult> Index()
        {
            var vm = new ReportVM
            {
                ReportTypes = await _context.ReportTypes
                    .Select(r => new ReportTypeItem
                    {
                        ReportTypeId = r.ReportTypeId,
                        TypeName = r.TypeName,
                        Code = r.Code
                    })
                    .ToListAsync()
            };

            return View(vm);
        }

        // POST: Generate / Preview / Export
        [HttpPost]
        public async Task<IActionResult> Generate(ReportVM model, string action)
        {
            // ---- VALIDATION ----
            if (model.ReportTypeId == null)
            {
                TempData["Error"] = "Please select a report type.";
                return RedirectToAction("Index");
            }

            if (model.DateFrom == null || model.DateTo == null)
            {
                TempData["Error"] = "Please select both dates.";
                return RedirectToAction("Index");
            }

            if (model.DateFrom > model.DateTo)
            {
                TempData["Error"] = "Date From cannot be later than Date To.";
                return RedirectToAction("Index");
            }

            var reportType = await _context.ReportTypes
                .FirstAsync(r => r.ReportTypeId == model.ReportTypeId.Value);

            var code = reportType.Code; // TIME_RANGE / AIRLINE / ORIGIN / DESTINATION / CUSTOMER

            // ---- LOAD BASE BOOKINGS ----
            var from = model.DateFrom.Value.Date;
            var toInclusive = model.DateTo.Value.Date.AddDays(1).AddTicks(-1);

            var baseQuery = _context.Bookings
                .Include(b => b.User)
                .Include(b => b.Guest)
                .Include(b => b.Flight).ThenInclude(f => f.Airline)
                .Include(b => b.Flight).ThenInclude(f => f.OriginAirport)
                .Include(b => b.Flight).ThenInclude(f => f.DestinationAirport)
                .Include(b => b.BookingStatus)
                .Where(b => b.BookingDate >= from && b.BookingDate <= toInclusive);

            // ---- EXECUTE QUERY BASED ON REPORT TYPE ----
            List<BookingReportRow> bookingRows = null;
            List<AirlineReportRow> airlineRows = null;
            List<AirportReportRow> originRows = null;
            List<AirportReportRow> destinationRows = null;
            List<CustomerReportRow> customerRows = null;

            switch (code)
            {
                case RT_TIME_RANGE:
                    bookingRows = await baseQuery
                        .OrderBy(b => b.BookingDate)
                        .Select(b => new BookingReportRow
                        {
                            BookingId = b.BookingId,
                            BookingDate = b.BookingDate,
                            PassengerName = b.GuestId != null
                                ? (b.Guest.FirstName + " " + b.Guest.LastName)
                                : (b.User.FirstName + " " + b.User.LastName),
                            FlightNumber = b.Flight.FlightNumber,
                            Route = b.Flight.OriginAirport.Code + " → " + b.Flight.DestinationAirport.Code,
                            Status = b.BookingStatus.StatusName,
                            TotalAmount = b.TotalAmount
                        })
                        .ToListAsync();
                    break;

                case RT_AIRLINE:
                    airlineRows = await baseQuery
                        .GroupBy(b => new { b.Flight.Airline.AirlineName, b.Flight.Airline.Code })
                        .Select(g => new AirlineReportRow
                        {
                            AirlineName = g.Key.AirlineName,
                            AirlineCode = g.Key.Code,
                            FlightCount = g.Select(x => x.FlightId).Distinct().Count(),
                            TotalBookings = g.Count(),
                            TotalRevenue = g.Sum(x => x.TotalAmount)
                        })
                        .OrderByDescending(r => r.TotalRevenue)
                        .ToListAsync();
                    break;

                case RT_ORIGIN:
                    originRows = await baseQuery
                        .GroupBy(b => new { b.Flight.OriginAirport.AirportName, b.Flight.OriginAirport.Code })
                        .Select(g => new AirportReportRow
                        {
                            AirportName = g.Key.AirportName,
                            AirportCode = g.Key.Code,
                            TotalBookings = g.Count(),
                            TotalRevenue = g.Sum(x => x.TotalAmount)
                        })
                        .OrderByDescending(r => r.TotalRevenue)
                        .ToListAsync();
                    break;

                case RT_DESTINATION:
                    destinationRows = await baseQuery
                        .GroupBy(b => new { b.Flight.DestinationAirport.AirportName, b.Flight.DestinationAirport.Code })
                        .Select(g => new AirportReportRow
                        {
                            AirportName = g.Key.AirportName,
                            AirportCode = g.Key.Code,
                            TotalBookings = g.Count(),
                            TotalRevenue = g.Sum(x => x.TotalAmount)
                        })
                        .OrderByDescending(r => r.TotalRevenue)
                        .ToListAsync();
                    break;

                case RT_CUSTOMER:
                    customerRows = await baseQuery
                        .GroupBy(b => new
                        {
                            CustomerName = b.UserId != null
                                ? (b.User.FirstName + " " + b.User.LastName)
                                : (b.Guest.FirstName + " " + b.Guest.LastName),

                            Email = b.UserId != null
                                ? b.User.Email
                                : b.Guest.Email  // <-- FIXED HERE
                        })
                        .Select(g => new CustomerReportRow
                        {
                            CustomerName = g.Key.CustomerName,
                            Email = g.Key.Email,
                            TotalBookings = g.Count(),
                            TotalRevenue = g.Sum(x => x.TotalAmount)
                        })
                        .OrderByDescending(r => r.TotalRevenue)
                        .ToListAsync();
                    break;
            }

            // ---- TRACK REPORT GENERATION ----
            var adminEmail = User.FindFirstValue(ClaimTypes.Email);
            var admin = await _context.UserProfiles.FirstOrDefaultAsync(u => u.Email == adminEmail);
            int? adminId = admin?.UserId;

            var report = new Report
            {
                ReportTypeId = model.ReportTypeId.Value,
                DateFrom = from,
                DateTo = model.DateTo.Value.Date,
                GeneratedOn = DateTime.Now,
                UserId = adminId,
                FilePath = null,
                Notes = null
            };

            _context.Reports.Add(report);
            await _context.SaveChangesAsync();

            // ---- ADD LOG ENTRY ----
            // ---- ADD LOG ENTRY ----
            _context.Logs.Add(new Log
            {
                UserId = adminId,
                ActionType = "Report Generated",
                Description = $"Generated Report (Type={report.ReportTypeId}) " +
                              $"Range: {report.DateFrom:yyyy-MM-dd} ➜ {report.DateTo:yyyy-MM-dd}",
                Timestamp = DateTime.Now
            });
            await _context.SaveChangesAsync();


            // ---- HANDLE ACTIONS ----
            switch (action)
            {
                // PREVIEW: return Index with filled VM
                case "preview":
                    {
                        var vm = new ReportVM
                        {
                            ReportTypeId = model.ReportTypeId,
                            DateFrom = model.DateFrom,
                            DateTo = model.DateTo,
                            ReportTypes = await _context.ReportTypes
                                .Select(r => new ReportTypeItem
                                {
                                    ReportTypeId = r.ReportTypeId,
                                    TypeName = r.TypeName,
                                    Code = r.Code
                                })
                                .ToListAsync(),
                            SelectedReportCode = code,
                            BookingRows = bookingRows,
                            AirlineRows = airlineRows,
                            OriginRows = originRows,
                            DestinationRows = destinationRows,
                            CustomerRows = customerRows
                        };

                        TempData["Success"] = "Report preview generated.";
                        return View("Index", vm);
                    }

                // CSV / EXCEL EXPORTS
                case "csv":
                    {
                        byte[] bytes = code switch
                        {
                            RT_TIME_RANGE => GenerateBookingsCsv(bookingRows),
                            RT_AIRLINE => GenerateAirlineCsv(airlineRows),
                            RT_ORIGIN => GenerateAirportCsv(originRows, "Origin"),
                            RT_DESTINATION => GenerateAirportCsv(destinationRows, "Destination"),
                            RT_CUSTOMER => GenerateCustomerCsv(customerRows),
                            _ => Array.Empty<byte>()
                        };

                        var fileName = $"{code}_{from:yyyyMMdd}_{model.DateTo:yyyyMMdd}.csv";
                        return File(bytes, "text/csv", fileName);
                    }

                case "excel":
                    {
                        byte[] bytes = code switch
                        {
                            RT_TIME_RANGE => GenerateBookingsExcel(bookingRows),
                            RT_AIRLINE => GenerateAirlineExcel(airlineRows),
                            RT_ORIGIN => GenerateAirportExcel(originRows, "Origin"),
                            RT_DESTINATION => GenerateAirportExcel(destinationRows, "Destination"),
                            RT_CUSTOMER => GenerateCustomerExcel(customerRows),
                            _ => Array.Empty<byte>()
                        };

                        var fileName = $"{code}_{from:yyyyMMdd}_{model.DateTo:yyyyMMdd}.xlsx";
                        return File(bytes,
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            fileName);
                    }

                default:
                    return RedirectToAction("Index");
            }
        }

        // ---------- CSV HELPERS ----------

        private byte[] GenerateBookingsCsv(List<BookingReportRow> rows)
        {
            var sb = new StringBuilder();
            sb.AppendLine("BookingId,BookingDate,Passenger,Flight,Route,Status,TotalAmount");

            foreach (var r in rows)
            {
                sb.AppendLine(
                    $"{r.BookingId}," +
                    $"{r.BookingDate:yyyy-MM-dd}," +
                    $"\"{r.PassengerName}\"," +
                    $"{r.FlightNumber}," +
                    $"\"{r.Route}\"," +
                    $"{r.Status}," +
                    $"{r.TotalAmount}");
            }
            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        private byte[] GenerateAirlineCsv(List<AirlineReportRow> rows)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Airline,Code,FlightCount,TotalBookings,TotalRevenue");

            foreach (var r in rows)
            {
                sb.AppendLine(
                    $"\"{r.AirlineName}\"," +
                    $"{r.AirlineCode}," +
                    $"{r.FlightCount}," +
                    $"{r.TotalBookings}," +
                    $"{r.TotalRevenue}");
            }
            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        private byte[] GenerateAirportCsv(List<AirportReportRow> rows, string label)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{label}Airport,Code,TotalBookings,TotalRevenue");

            foreach (var r in rows)
            {
                sb.AppendLine(
                    $"\"{r.AirportName}\"," +
                    $"{r.AirportCode}," +
                    $"{r.TotalBookings}," +
                    $"{r.TotalRevenue}");
            }
            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        private byte[] GenerateCustomerCsv(List<CustomerReportRow> rows)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Customer,Email,TotalBookings,TotalRevenue");

            foreach (var r in rows)
            {
                sb.AppendLine(
                    $"\"{r.CustomerName}\"," +
                    $"{r.Email}," +
                    $"{r.TotalBookings}," +
                    $"{r.TotalRevenue}");
            }
            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        // ---------- EXCEL HELPERS (ClosedXML) ----------

        private byte[] GenerateBookingsExcel(List<BookingReportRow> rows)
        {
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Bookings");

            ws.Cell(1, 1).Value = "BookingId";
            ws.Cell(1, 2).Value = "BookingDate";
            ws.Cell(1, 3).Value = "Passenger";
            ws.Cell(1, 4).Value = "Flight";
            ws.Cell(1, 5).Value = "Route";
            ws.Cell(1, 6).Value = "Status";
            ws.Cell(1, 7).Value = "TotalAmount";

            var row = 2;
            foreach (var r in rows)
            {
                ws.Cell(row, 1).Value = r.BookingId;
                ws.Cell(row, 2).Value = r.BookingDate;
                ws.Cell(row, 3).Value = r.PassengerName;
                ws.Cell(row, 4).Value = r.FlightNumber;
                ws.Cell(row, 5).Value = r.Route;
                ws.Cell(row, 6).Value = r.Status;
                ws.Cell(row, 7).Value = r.TotalAmount;
                row++;
            }
            ws.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            wb.SaveAs(stream);
            return stream.ToArray();
        }

        private byte[] GenerateAirlineExcel(List<AirlineReportRow> rows)
        {
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("By Airline");

            ws.Cell(1, 1).Value = "Airline";
            ws.Cell(1, 2).Value = "Code";
            ws.Cell(1, 3).Value = "FlightCount";
            ws.Cell(1, 4).Value = "TotalBookings";
            ws.Cell(1, 5).Value = "TotalRevenue";

            var row = 2;
            foreach (var r in rows)
            {
                ws.Cell(row, 1).Value = r.AirlineName;
                ws.Cell(row, 2).Value = r.AirlineCode;
                ws.Cell(row, 3).Value = r.FlightCount;
                ws.Cell(row, 4).Value = r.TotalBookings;
                ws.Cell(row, 5).Value = r.TotalRevenue;
                row++;
            }
            ws.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            wb.SaveAs(stream);
            return stream.ToArray();
        }

        private byte[] GenerateAirportExcel(List<AirportReportRow> rows, string label)
        {
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add($"By {label}");

            ws.Cell(1, 1).Value = $"{label} Airport";
            ws.Cell(1, 2).Value = "Code";
            ws.Cell(1, 3).Value = "TotalBookings";
            ws.Cell(1, 4).Value = "TotalRevenue";

            var row = 2;
            foreach (var r in rows)
            {
                ws.Cell(row, 1).Value = r.AirportName;
                ws.Cell(row, 2).Value = r.AirportCode;
                ws.Cell(row, 3).Value = r.TotalBookings;
                ws.Cell(row, 4).Value = r.TotalRevenue;
                row++;
            }
            ws.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            wb.SaveAs(stream);
            return stream.ToArray();
        }

        private byte[] GenerateCustomerExcel(List<CustomerReportRow> rows)
        {
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("By Customer");

            ws.Cell(1, 1).Value = "Customer";
            ws.Cell(1, 2).Value = "Email";
            ws.Cell(1, 3).Value = "TotalBookings";
            ws.Cell(1, 4).Value = "TotalRevenue";

            var row = 2;
            foreach (var r in rows)
            {
                ws.Cell(row, 1).Value = r.CustomerName;
                ws.Cell(row, 2).Value = r.Email;
                ws.Cell(row, 3).Value = r.TotalBookings;
                ws.Cell(row, 4).Value = r.TotalRevenue;
                row++;
            }
            ws.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            wb.SaveAs(stream);
            return stream.ToArray();
        }
    }
}
