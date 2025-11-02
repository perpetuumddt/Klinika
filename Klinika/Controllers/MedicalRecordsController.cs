using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Klinika.Data;
using Klinika.Models;

namespace Klinika.Controllers
{
    public class MedicalRecordsController : Controller
    {
        private readonly KlinikaDbContext _context;

        public MedicalRecordsController(KlinikaDbContext context)
        {
            _context = context;
        }

        private void PopulateDropdowns(int? selectedPatientId = null, int? selectedDoctorId = null)
        {
            ViewData["PatientId"] = new SelectList(_context.Patients, "Id", "FullName", selectedPatientId);
            ViewData["DoctorId"] = new SelectList(_context.Doctors.Where(d => d.IsActive), "Id", "FullNameWithTitle", selectedDoctorId);
        }

        // GET: MedicalRecords
        public async Task<IActionResult> Index(int? patientId)
        {
            var query = _context.MedicalRecords
                .Include(mr => mr.Patient)
                .Include(mr => mr.Doctor)
                .AsQueryable();

            if (patientId.HasValue)
            {
                query = query.Where(mr => mr.PatientId == patientId.Value);
                ViewBag.PatientId = patientId.Value;
                ViewBag.PatientName = await _context.Patients
                    .Where(p => p.Id == patientId.Value)
                    .Select(p => p.FullName)
                    .FirstOrDefaultAsync();
            }

            var medicalRecords = await query.ToListAsync();
            return View(medicalRecords);
        }

        // GET: MedicalRecords/Details/
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicalRecord = await _context.MedicalRecords
                .Include(mr => mr.Patient)
                .Include(mr => mr.Doctor)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (medicalRecord == null)
            {
                return NotFound();
            }

            return View(medicalRecord);
        }

        // GET: MedicalRecords/Create
        public IActionResult Create(int? patientId, int? doctorId)
        {
            var patientsQuery = _context.Patients.AsQueryable();
            if (patientId.HasValue)
            {
                patientsQuery = patientsQuery.Where(p => p.Id == patientId.Value);
            }

            var model = new MedicalRecord
            {
                PatientId = patientId ?? 0,
                DoctorId = doctorId ?? 0,
                RecordDate = DateTime.Now
            };
            PopulateDropdowns(model.PatientId == 0 ? null : model.PatientId, model.DoctorId == 0 ? null : model.DoctorId);
            return View(model);
        }

        // POST: MedicalRecords/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PatientId,DoctorId,RecordDate,Diagnosis,Symptoms,Treatment,Prescriptions,Recommendations,Notes,NextVisitDate")] MedicalRecord medicalRecord)
        {
            if (ModelState.IsValid)
            {
                var patientExists = await _context.Patients.AnyAsync(p => p.Id == medicalRecord.PatientId);
                var doctorExists = await _context.Doctors.AnyAsync(d => d.Id == medicalRecord.DoctorId && d.IsActive);

                if (!patientExists)
                    ModelState.AddModelError("PatientId", "Пацієнт не знайдений");
                if (!doctorExists)
                    ModelState.AddModelError("DoctorId", "Лікар не знайдений або не активний");

                if (ModelState.IsValid)
                {
                    _context.Add(medicalRecord);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            PopulateDropdowns(medicalRecord.PatientId, medicalRecord.DoctorId);
            return View(medicalRecord);
        }


        // GET: MedicalRecords/Edit/
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicalRecord = await _context.MedicalRecords.FindAsync(id);
            if (medicalRecord == null)
            {
                return NotFound();
            }
            ViewData["PatientId"] = new SelectList(_context.Patients, "Id", "FullName", medicalRecord.PatientId);
            ViewData["DoctorId"] = new SelectList(_context.Doctors.Where(d => d.IsActive), "Id", "FullNameWithTitle", medicalRecord.DoctorId);
            return View(medicalRecord);
        }

        // POST: MedicalRecords/Edit/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PatientId,DoctorId,RecordDate,Diagnosis,Symptoms,Treatment,Prescriptions,Recommendations,Temperature,BloodPressure,Pulse,Weight,Height,Notes,NextVisitDate")] MedicalRecord medicalRecord)
        {
            if (id != medicalRecord.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(medicalRecord);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MedicalRecordExists(medicalRecord.Id))
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
            ViewData["PatientId"] = new SelectList(_context.Patients, "Id", "FullName", medicalRecord.PatientId);
            ViewData["DoctorId"] = new SelectList(_context.Doctors.Where(d => d.IsActive), "Id", "FullNameWithTitle", medicalRecord.DoctorId);
            return View(medicalRecord);
        }

        // GET: MedicalRecords/Delete/
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicalRecord = await _context.MedicalRecords
                .Include(mr => mr.Patient)
                .Include(mr => mr.Doctor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (medicalRecord == null)
            {
                return NotFound();
            }

            return View(medicalRecord);
        }

        // POST: MedicalRecords/Delete/
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var medicalRecord = await _context.MedicalRecords.FindAsync(id);
            if (medicalRecord != null)
            {
                _context.MedicalRecords.Remove(medicalRecord);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MedicalRecordExists(int id)
        {
            return _context.MedicalRecords.Any(e => e.Id == id);
        }

        public async Task<ViewResult> Index()
        {
            throw new NotImplementedException();
        }
    }
}

