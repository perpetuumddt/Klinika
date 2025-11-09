using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Klinika.Data;
using Klinika.Models;

namespace Klinika.Controllers
{
    public class SearchController : Controller
    {
        private readonly KlinikaDbContext _context;

        public SearchController(KlinikaDbContext context)
        {
            _context = context;
        }

        // GET: Search
        public IActionResult Index()
        {
            // Populate dropdowns for search filters
            ViewData["Patients"] = new SelectList(_context.Patients.OrderBy(p => p.LastName), "Id", "FullName");
            ViewData["Doctors"] = new SelectList(_context.Doctors.Where(d => d.IsActive).OrderBy(d => d.LastName), "Id", "FullName");
            ViewData["AppointmentStatuses"] = new SelectList(Enum.GetValues(typeof(AppointmentStatus)));
            
            // Create empty model
            var model = new SearchViewModel();
            
            return View(model);
        }

        // POST: Search/Results
        [HttpPost]
        public async Task<IActionResult> Results(SearchViewModel model)
        {
            // Start with base query that includes necessary joins
            var query = _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .Include(a => a.Patient.MedicalRecords)
                .AsQueryable();

            // ============================================================
            // FILTER 1: Search by Date Range
            // ============================================================
            if (model.DateFrom.HasValue)
            {
                query = query.Where(a => a.AppointmentDateTime >= model.DateFrom.Value);
            }
            
            if (model.DateTo.HasValue)
            {
                var dateTo = model.DateTo.Value.AddDays(1).AddSeconds(-1); // Include full day
                query = query.Where(a => a.AppointmentDateTime <= dateTo);
            }

            // ============================================================
            // FILTER 2: Search by List of Elements (Patients & Doctors)
            // ============================================================
            if (model.SelectedPatientIds != null && model.SelectedPatientIds.Any())
            {
                query = query.Where(a => model.SelectedPatientIds.Contains(a.PatientId));
            }
            
            if (model.SelectedDoctorIds != null && model.SelectedDoctorIds.Any())
            {
                query = query.Where(a => model.SelectedDoctorIds.Contains(a.DoctorId));
            }
            
            if (model.SelectedStatuses != null && model.SelectedStatuses.Any())
            {
                query = query.Where(a => model.SelectedStatuses.Contains(a.Status));
            }

            // ============================================================
            // FILTER 3: Search by Beginning/End of Value
            // ============================================================
            if (!string.IsNullOrWhiteSpace(model.PatientNameStartsWith))
            {
                var searchTerm = model.PatientNameStartsWith.Trim().ToLower();
                query = query.Where(a => 
                    a.Patient.FirstName.ToLower().StartsWith(searchTerm) ||
                    a.Patient.LastName.ToLower().StartsWith(searchTerm));
            }
            
            if (!string.IsNullOrWhiteSpace(model.Diagnosis))
            {
                var searchTerm = model.Diagnosis.Trim().ToLower();
                query = query.Where(a => 
                    a.Patient.FirstName.ToLower().EndsWith(searchTerm) ||
                    a.Patient.LastName.ToLower().EndsWith(searchTerm));
            }
            
            if (!string.IsNullOrWhiteSpace(model.DoctorSpecializationStartsWith))
            {
                var searchTerm = model.DoctorSpecializationStartsWith.Trim().ToLower();
                query = query.Where(a => a.Doctor.Specialization.ToLower().StartsWith(searchTerm));
            }
            
            if (!string.IsNullOrWhiteSpace(model.DoctorName))
            {
                var searchTerm = model.DoctorName.Trim().ToLower();
                query = query.Where(a => a.Complaints != null && a.Complaints.ToLower().Contains(searchTerm));
            }

            // ============================================================
            // ADDITIONAL FILTER: Diagnosis from MedicalRecords (JOIN)
            // ============================================================
            if (!string.IsNullOrWhiteSpace(model.DiagnosisContains))
            {
                var searchTerm = model.DiagnosisContains.Trim().ToLower();
                
                // Get patient IDs with matching diagnoses
                var patientIdsWithDiagnosis = _context.MedicalRecords
                    .Where(mr => mr.Diagnosis != null && mr.Diagnosis.ToLower().Contains(searchTerm))
                    .Select(mr => mr.PatientId)
                    .Distinct();
                
                query = query.Where(a => patientIdsWithDiagnosis.Contains(a.PatientId));
            }

            // Execute query and order results
            var results = await query
                .OrderByDescending(a => a.AppointmentDateTime)
                .Take(100) // Limit results
                .ToListAsync();

            // Populate dropdowns again for the view
            ViewData["Patients"] = new SelectList(_context.Patients.OrderBy(p => p.LastName), "Id", "FullName");
            ViewData["Doctors"] = new SelectList(_context.Doctors.Where(d => d.IsActive).OrderBy(d => d.LastName), "Id", "FullName");
            ViewData["AppointmentStatuses"] = new SelectList(Enum.GetValues(typeof(AppointmentStatus)));

            var viewModel = new SearchResultsViewModel
            {
                SearchCriteria = model,
                Results = results,
                TotalResults = results.Count
            };

            return View(viewModel);
        }
    }
}