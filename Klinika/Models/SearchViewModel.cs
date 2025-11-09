using Klinika.Models;

namespace Klinika.Models
{
    public class SearchViewModel
    {
        // Date/Time filters
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }

        // List filters
        public List<int>? SelectedPatientIds { get; set; }
        public List<int>? SelectedDoctorIds { get; set; }
        public List<AppointmentStatus>? SelectedStatuses { get; set; }

        // String filters (StartsWith/EndsWith)
        public string? PatientNameStartsWith { get; set; }
        public string? Diagnosis { get; set; }
        public string? DoctorName { get; set; }
        public string? DoctorSpecializationStartsWith { get; set; }
        
        // JOIN filter (from dependent table)
        public string? DiagnosisContains { get; set; }
    }

    public class SearchResultsViewModel
    {
        public SearchViewModel SearchCriteria { get; set; } = new();
        public List<Appointment> Results { get; set; } = new();
        public int TotalResults { get; set; }
    }
}