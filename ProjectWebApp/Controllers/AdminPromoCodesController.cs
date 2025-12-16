using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectWebApp.Model;
using ProjectWebApp.Model.ViewModels;
using System.Security.Claims;

namespace ProjectWebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminPromoCodesController : Controller
    {
        private readonly FlightlyDBContext _context;

        public AdminPromoCodesController(FlightlyDBContext context)
        {
            _context = context;
        }

        // -----------------------------
        // LIST PROMO CODES
        // -----------------------------
        public IActionResult Index()
        {

            AutoActivatePromos();   // Upcoming → Active
            AutoExpirePromos();     // Active → Expired
            AutoOverLimitPromos();  // Active → OverLimit

            var promos = _context.PromoCodes
      .Include(p => p.Status)
      .Where(p => p.StatusId != 5) // Hide deleted
      .OrderByDescending(p => p.CreatedAt)
      .ToList();

            return View(promos);
        }

        // -----------------------------
        // CREATE PAGE (GET)
        // -----------------------------
        public IActionResult Create()
        {
            var allowedStatuses = _context.PromoStatuses
    .Where(s => s.StatusId != 2) // Exclude Expired
    .ToList();

            ViewBag.StatusList = new SelectList(allowedStatuses, "StatusId", "StatusName");
            return View();
        }

        // -----------------------------
        // CREATE PROMO (POST)
        // -----------------------------
        // CREATE PROMO (POST)
        [HttpPost]
        public IActionResult Create(PromoCodeCreateViewModel model)
        {
            // 🔵 Normalize promo code
            model.Code = model.Code?.Trim().ToUpper();

            if (!ModelState.IsValid)
            {
                ReloadStatusList();
                return View(model);
            }

            // 🔴 CHECK: Promo code already exists
            bool codeExists = _context.PromoCodes
                .Any(p => p.Code == model.Code);

            if (codeExists)
            {
                ModelState.AddModelError("Code", "This promo code already exists.");
                ReloadStatusList();
                return View(model);
            }

            // ✅ AUTO-CALCULATE STATUS (🔥 THE FIX 🔥)
            var today = DateTime.Today;
            int statusId;

            if (model.EndDate < today)
            {
                statusId = 2; // Expired
            }
            else if (model.StartDate > today)
            {
                statusId = 3; // Upcoming
            }
            else
            {
                statusId = 1; // Active
            }

            // ✅ Get logged-in admin
            var email = User.FindFirstValue(ClaimTypes.Email);
            int? userId = null;

            if (!string.IsNullOrEmpty(email))
            {
                var user = _context.UserProfiles.FirstOrDefault(u => u.Email == email);
                if (user != null)
                    userId = user.UserId;
            }

            var promo = new PromoCode
            {
                Code = model.Code,
                Value = model.Value,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                UsageLimit = model.UsageLimit,
                Notes = model.Notes,
                StatusId = statusId, // ✅ USE CALCULATED VALUE
                CreatedAt = DateTime.Now,
                UserId = userId
            };

            _context.PromoCodes.Add(promo);
            _context.SaveChanges();

            TempData["Success"] = "Promo code created successfully.";
            return RedirectToAction("Index");
        }

        private void ReloadStatusList()
        {
            var allowedStatuses = _context.PromoStatuses
                .Where(s => s.StatusId != 2) // Exclude Expired
                .ToList();

            ViewBag.StatusList = new SelectList(
                allowedStatuses,
                "StatusId",
                "StatusName"
            );
        }



        // -----------------------------
        // EDIT PAGE (GET)
        // -----------------------------
        public IActionResult Edit(int id)
        {
            var promo = _context.PromoCodes
                .Include(p => p.Status)
                .FirstOrDefault(p => p.PromoId == id);

            if (promo == null)
                return NotFound();

            if (promo.StatusId == 2 || promo.StatusId == 6) // Expired OR OverLimit
            {
                TempData["Error"] = "Expired or OverLimit promo codes cannot be edited.";
                return RedirectToAction("Index");
            }

            var vm = new PromoCodeEditViewModel
            {
                PromoId = promo.PromoId,
                Code = promo.Code,
                StatusId = promo.StatusId,
                Value = promo.Value,
                StartDate = promo.StartDate,
                EndDate = promo.EndDate,
                UsageLimit = promo.UsageLimit,
                Notes = promo.Notes
            };

            var allowedStatuses = _context.PromoStatuses
                .Where(s => s.StatusId != 2) // Exclude Expired (date-controlled)
                .ToList();

            ViewBag.StatusList = new SelectList(allowedStatuses, "StatusId", "StatusName", promo.StatusId);

            return View(vm);
        }


        // -----------------------------
        // EDIT PROMO (POST)
        // -----------------------------
        // -----------------------------
        // EDIT PROMO (POST)
        // -----------------------------
        [HttpPost]
        public IActionResult Edit(PromoCodeEditViewModel model)
        {
            // 🔵 Normalize promo code
            model.Code = model.Code?.Trim().ToUpper();

            if (!ModelState.IsValid)
            {
                ReloadStatusList(model.StatusId);
                return View(model);
            }

            var promo = _context.PromoCodes.Find(model.PromoId);
            if (promo == null)
                return NotFound();

            // 🔴 CHECK: Promo code already exists (exclude current promo)
            bool codeExists = _context.PromoCodes
                .Any(p => p.Code == model.Code && p.PromoId != model.PromoId);

            if (codeExists)
            {
                ModelState.AddModelError("Code", "This promo code already exists.");
                ReloadStatusList(model.StatusId);
                return View(model);
            }

            // ✅ AUTO-CALCULATE STATUS (🔥 FIX 🔥)
            var today = DateTime.Today;
            int statusId;

            if (model.EndDate < today)
            {
                statusId = 2; // Expired
            }
            else if (model.StartDate > today)
            {
                statusId = 3; // Upcoming
            }
            else
            {
                statusId = 1; // Active
            }

            promo.Code = model.Code;
            promo.Value = model.Value;
            promo.StartDate = model.StartDate;
            promo.EndDate = model.EndDate;
            promo.UsageLimit = model.UsageLimit;
            promo.Notes = model.Notes;
            promo.StatusId = statusId; // ✅ USE CALCULATED STATUS

            _context.SaveChanges();

            TempData["Success"] = "Promo code updated successfully.";
            return RedirectToAction("Index");
        }

        private void ReloadStatusList(int selectedStatusId)
        {
            var allowedStatuses = _context.PromoStatuses
                .Where(s => s.StatusId != 2) // Exclude Expired
                .ToList();

            ViewBag.StatusList = new SelectList(
                allowedStatuses,
                "StatusId",
                "StatusName",
                selectedStatusId
            );
        }



        // -----------------------------
        // ENABLE / DISABLE PROMO
        // -----------------------------
        public IActionResult Enable(int id)
        {
            var promo = _context.PromoCodes.Find(id);
            if (promo == null)
                return NotFound();

            var today = DateTime.Today;

            if (promo.EndDate < today)
            {
                TempData["Error"] = "Cannot enable an expired promo code.";
                return RedirectToAction("Index");
            }

            if (promo.StartDate > today)
            {
                promo.StatusId = 3; // Upcoming
            }
            else
            {
                promo.StatusId = 1; // Active
            }

            _context.SaveChanges();
            return RedirectToAction("Index");
        }


        public IActionResult Disable(int id)
        {
            var promo = _context.PromoCodes.Find(id);

            if (promo == null)
                return NotFound();

            promo.StatusId = 4; // Disabled
            _context.SaveChanges();

            return RedirectToAction("Index");
        }


        private void AutoActivatePromos()
        {
            var today = DateTime.Today;

            var upcoming = _context.PromoCodes
                .Where(p => p.StatusId == 3 && p.StartDate <= today && p.EndDate >= today)
                .ToList();

            foreach (var promo in upcoming)
                promo.StatusId = 1; // Active

            if (upcoming.Any())
                _context.SaveChanges();
        }

        private void AutoExpirePromos()
        {
            var today = DateTime.Today;

            var active = _context.PromoCodes
                .Where(p => p.StatusId == 1 && p.EndDate < today)
                .ToList();

            foreach (var promo in active)
                promo.StatusId = 2; // Expired

            if (active.Any())
                _context.SaveChanges();
        }

        public IActionResult Details(int id)
        {
            var promo = _context.PromoCodes
                .Include(p => p.Status)
                .FirstOrDefault(p => p.PromoId == id);

            if (promo == null)
                return NotFound();

            return View("PromocodeDetails", promo);
        }


        public IActionResult Delete(int id)
        {
            var promo = _context.PromoCodes.Find(id);
            if (promo == null)
                return NotFound();

            if (promo.StatusId != 2 && promo.StatusId != 6)
            {
                TempData["Error"] = "Only expired or over-limit promo codes can be deleted.";
                return RedirectToAction("Index");
            }

            promo.StatusId = 5; // Deleted
            _context.SaveChanges();

            TempData["Success"] = "Promo code deleted successfully.";
            return RedirectToAction("Index");
        }


        private void AutoOverLimitPromos()
        {
            var overLimitPromos = _context.PromoCodes
                .Where(p => p.StatusId == 1 && p.UsageLimit != null && p.UsageCount >= p.UsageLimit)
                .ToList();

            foreach (var promo in overLimitPromos)
            {
                promo.StatusId = 6; // OverLimit status
            }

            if (overLimitPromos.Any())
                _context.SaveChanges();
        }


    }


}
