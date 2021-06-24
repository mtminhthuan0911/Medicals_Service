using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicService.Client.Models
{
    public class MedicalExaminationAttachmentViewModel
    {
        public int Id { get; set; }

        public string FileName { get; set; }

        public string FileType { get; set; }

        public long? FileSize { get; set; }

        public string FilePath { get; set; }

        public int MedicalExaminationId { get; set; }
    }
}
