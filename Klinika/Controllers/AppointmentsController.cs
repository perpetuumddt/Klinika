using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Klinika.Data;
using Klinika.Models;

namespace Klinika.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly KlinikaDbContext _context;

        public AppointmentsController(KlinikaDbContext context)
        {
            _context = context;
        }

        private void PopulateDropdowns(int? selectedPatientId = null, int? selectedDoctorId = null)
        {
            ViewData["PatientId"] = new SelectList(_context.Patients, "Id", "FullName", selectedPatientId);
            ViewData["DoctorId"] = new SelectList(_context.Doctors.Where(d => d.IsActive), "Id", "FullNameWithTitle", selectedDoctorId);
        }

        // GET: Appointments
        public async Task<IActionResult> Index()
        {
            var appointments = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .ToListAsync();
            return View(appointments);
        }

        // GET: Appointments/Details/
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // GET: Appointments/Create
        public IActionResult Create(int? patientId, int? doctorId)
        {
            var model = new Appointment
            {
                PatientId = patientId ?? 0,
                DoctorId = doctorId ?? 0,
                AppointmentDateTime = DateTime.Now.AddHours(1),
                DurationMinutes = 30,
                Status = AppointmentStatus.Scheduled
            };
            PopulateDropdowns(model.PatientId == 0 ? null : model.PatientId, model.DoctorId == 0 ? null : model.DoctorId);
            return View(model);
        }

        // POST: Appointments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PatientId,DoctorId,AppointmentDateTime,DurationMinutes,AppointmentType,Complaints,Status,Notes")] Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                var patientExists = await _context.Patients.AnyAsync(p => p.Id == appointment.PatientId);
                var doctorExists = await _context.Doctors.AnyAsync(d => d.Id == appointment.DoctorId && d.IsActive);
                if (!patientExists)
                {
                    ModelState.AddModelError("PatientId", "Пацієнт не знайдений");
                }
                if (!doctorExists)
                {
                    ModelState.AddModelError("DoctorId", "Лікар не знайдений або не активний");
                }
                if (ModelState.IsValid)
                {
                    try
                    {
                        appointment.CreatedDate = DateTime.Now;
                        _context.Add(appointment);
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    catch (DbUpdateException ex)
                    {
                        if (ex.InnerException != null && ex.InnerException.Message.Contains("IX_Appointments_DoctorId_AppointmentDateTime", StringComparison.OrdinalIgnoreCase))
                        {
                            ModelState.AddModelError("AppointmentDateTime", "На обраний час у лікаря вже є запис. Оберіть інший час.");
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "Не вдалося зберегти запис. Спробуйте ще раз.");
                        }
                    }
                }
            }
            PopulateDropdowns(appointment.PatientId, appointment.DoctorId);
            return View(appointment);
        }

        // GET: Appointments/Edit/
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }
            ViewData["PatientId"] = new SelectList(_context.Patients, "Id", "FullName", appointment.PatientId);
            ViewData["DoctorId"] = new SelectList(_context.Doctors.Where(d => d.IsActive), "Id", "FullNameWithTitle", appointment.DoctorId);
            return View(appointment);
        }

        // POST: Appointments/Edit/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PatientId,DoctorId,AppointmentDateTime,DurationMinutes,AppointmentType,Complaints,Status,Notes,CreatedDate")] Appointment appointment)
        {
            if (id != appointment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    appointment.UpdatedDate = DateTime.Now;
                    _context.Update(appointment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentExists(appointment.Id))
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
            ViewData["PatientId"] = new SelectList(_context.Patients, "Id", "FullName", appointment.PatientId);
            ViewData["DoctorId"] = new SelectList(_context.Doctors.Where(d => d.IsActive), "Id", "FullNameWithTitle", appointment.DoctorId);
            return View(appointment);
        }

        // GET: Appointments/Delete/
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // POST: Appointments/Delete/
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.Id == id);
        }
    }
}
