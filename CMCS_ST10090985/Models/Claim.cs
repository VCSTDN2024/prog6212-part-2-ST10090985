using System.ComponentModel.DataAnnotations;

namespace CMCS_ST10090985.Models
{
    public class Claim
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Hours Worked is required.")]
        public decimal WorkedHours { get; set; }

        [Required(ErrorMessage = "Hourly Rate is required.")]
        public decimal HourlyRate { get; set; }

        public string AdditionalNotes { get; set; }
        public string Status { get; set; }

        public string UploadedFileName { get; set; }  // Name of the uploaded file
    }
}
