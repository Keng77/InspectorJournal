using InspectorJournal.DataLayer.Data;
using InspectorJournal.DataLayer.Models;
using InspectorJournal.Infrastructure;
using InspectorJournal.Infrastructure.Filters;
using InspectorJournal.ViewModels;
using InspectorJournal.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InspectorJournal.Controllers
{
    public class InspectionsController : Controller
    {
        private readonly InspectionsDbContext _context;
        private readonly int pageSize = 10;   // количество элементов на странице

        public InspectionsController(InspectionsDbContext context, IConfiguration appConfig = null)
        {
            _context = context;
            if (appConfig != null)
            {
                pageSize = int.Parse(appConfig["Parameters:PageSize"]);
            }
        }

        // GET: Operations
        [SetToSession("Inspection")] //Фильтр действий для сохранение в сессию параметров отбора
        public IActionResult Index(FilterInspectionViewModel inspection, SortState sortOrder = SortState.No, int page = 1)
        {
            if (inspection.Enterprise == null & inspection.ViolationType == null & inspection.PenaltyAmount == null)
            {
                // Считывание данных из сессии
                if (HttpContext != null)
                {
                    var sessionInspection = Infrastructure.SessionExtensions.Get(HttpContext.Session, "Inspection");
                    if (sessionInspection != null)
                        inspection = Transformations.DictionaryToObject<FilterInspectionViewModel>(sessionInspection);

                }
            }

            // Сортировка и фильтрация данных
            IQueryable<Inspection> inspectionContext = _context.Inspections;
            inspectionContext = Sort_Search(inspectionContext, sortOrder, inspection.Enterprise ?? "", inspection.ViolationType ?? "", inspection.PenaltyAmount > 0 ? inspection.PenaltyAmount : 0);

            // Разбиение на страницы
            var count = inspectionContext.Count();
            inspectionContext = inspectionContext.Skip((page - 1) * pageSize).Take(pageSize);

            // Формирование модели для передачи представлению
            InspectionsViewModel inspections = new()
            {
                Inspections = inspectionContext,
                PageViewModel = new PageViewModel(count, page, pageSize),
                SortViewModel = new SortViewModel(sortOrder),
                FilterInspectionViewModel = inspection
            };
            return View(inspections);
        }


        // GET: Inspections/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inspection = await _context.Inspections
                .Include(o => o.Enterprise)
                .Include(o => o.ViolationType)
                .Include(o => o.Inspector)
                .SingleOrDefaultAsync(m => m.InspectionId == id);
            if (inspection == null)
            {
                return NotFound();
            }

            return View(inspection);
        }

        // GET: Inspections/Create
        public IActionResult Create()
        {
            var enterprises = _context.Enterprises;
            if (enterprises != null) ViewData["EnterpriseId"] = new SelectList(enterprises, "EnterpriseId", "Enterprise");
            var violationTypes = _context.ViolationTypes;
            if (violationTypes != null) ViewData["ViolationTypeId"] = new SelectList(violationTypes, "ViolationTypeId", "ViolationType");
            var inspectors = _context.Inspectors;
            if (inspectors != null) ViewData["InspectorId"] = new SelectList(inspectors, "InspectorId", "ResponsiblePerson");

            return View();
        }

        // POST: Inspections/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("InspectionID,EnterpriseId,ViolationTypeId,InspectorId,ProtocolNumber,InspectionDate,PenaltyAmount,PaymentStatus,CorrectionStatus,CorrectionDeadline")] Inspection inspection)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                _context.Add(inspection);
                await _context.SaveChangesAsync();

            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Inspections/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inspection = await _context.Inspections.SingleOrDefaultAsync(m => m.InspectionId == id);
            if (inspection == null)
            {
                return NotFound();
            }

            var enterprises = _context.Enterprises;
            if (enterprises != null) ViewData["EnterpriseId"] = new SelectList(enterprises, "EnterpriseId", "Enterprise");
            var violationTypes = _context.ViolationTypes;
            if (violationTypes != null) ViewData["ViolationTypeId"] = new SelectList(violationTypes, "ViolationTypeId", "ViolationType");
            var inspectors = _context.Inspectors;
            if (inspectors != null) ViewData["InspectorId"] = new SelectList(inspectors, "InspectorId", "ResponsiblePerson");

            return View(inspection);
        }

        // POST: Inspections/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,[Bind("InspectionID,EnterpriseId,ViolationTypeId,InspectorId,ProtocolNumber,InspectionDate,PenaltyAmount,PaymentStatus,CorrectionStatus,CorrectionDeadline")] Inspection inspection)
        {
            if (id != inspection.InspectionId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(inspection);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OperationExists(inspection.InspectionId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            var enterprises = _context.Enterprises;
            if (enterprises != null) ViewData["EnterpriseId"] = new SelectList(enterprises, "EnterpriseId", "Enterprise");
            var violationTypes = _context.ViolationTypes;
            if (violationTypes != null) ViewData["ViolationTypeId"] = new SelectList(violationTypes, "ViolationTypeId", "ViolationType");
            var inspectors = _context.Inspectors;
            if (inspectors != null) ViewData["InspectorId"] = new SelectList(inspectors, "InspectorId", "ResponsiblePerson");

            return View(inspection);
        }

        // GET: Inspections/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inspection = await _context.Inspections
                .Include(o => o.Enterprise)
                .Include(o => o.ViolationType)
                .Include(o => o.Inspector)
                .SingleOrDefaultAsync(m => m.InspectionId == id);
            if (inspection == null)
            {
                return NotFound();
            }

            return View(inspection);
        }

        // POST: Inspections/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var inspection = await _context.Inspections.SingleOrDefaultAsync(m => m.InspectionId == id);
            _context.Inspections.Remove(inspection);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OperationExists(int id)
        {
            return _context.Inspections.Any(e => e.InspectionId == id);
        }
        private static IQueryable<Inspection> Sort_Search(IQueryable<Inspection> inspections, SortState sortOrder, string searchEnterpriseName, string searchViolationType, decimal searchPenaltyAmount)
        {
            // Применяем сортировку
            switch (sortOrder)
            {
                case SortState.EnterpriseNameAsc:
                    inspections = inspections.OrderBy(s => s.Enterprise.Name);
                    break;
                case SortState.EnterpriseNameDesc:
                    inspections = inspections.OrderByDescending(s => s.Enterprise.Name);
                    break;
                case SortState.ViolationTypeAsc:
                    inspections = inspections.OrderBy(s => s.ViolationType.Name);
                    break;
                case SortState.ViolationTypeDesc:
                    inspections = inspections.OrderByDescending(s => s.ViolationType.Name);
                    break;
                case SortState.PenaltyAmountAsc:
                    inspections = inspections.OrderBy(s => s.PenaltyAmount);
                    break;
                case SortState.PenaltyAmountDesc:
                    inspections = inspections.OrderByDescending(s => s.PenaltyAmount);
                    break;
            }

            // Применяем фильтры
            inspections = inspections.Include(o => o.Enterprise)
                                     .Include(o => o.ViolationType)
                                     .Where(o =>
                                         (string.IsNullOrEmpty(searchEnterpriseName) || o.Enterprise.Name.Contains(searchEnterpriseName)) &&
                                         (string.IsNullOrEmpty(searchViolationType) || o.ViolationType.Name.Contains(searchViolationType)) &&
                                         (searchPenaltyAmount <= 0 || o.PenaltyAmount >= searchPenaltyAmount));

            return inspections;
        }

    }
}
