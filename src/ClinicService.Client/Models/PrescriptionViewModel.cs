using System;

namespace ClinicService.Client.Models
{
    public class PrescriptionViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Subname { get; set; }

        public string Quantity { get; set; }

        public string AvailableQuantity { get; set; }

        public int Total { get; set; }

        public int Take { get; set; }

        public bool? IsMorning { get; set; }

        public bool? IsAfternoon { get; set; }

        public bool? IsEvening { get; set; }

        public string Note { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string DoctorId { get; set; }

        public int MedicalExaminationId { get; set; }
    }
}
