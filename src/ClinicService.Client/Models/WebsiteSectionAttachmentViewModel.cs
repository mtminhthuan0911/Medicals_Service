using System;
namespace ClinicService.Client.Models
{
    public class WebsiteSectionAttachmentViewModel
    {
        public int Id { get; set; }

        public string FileName { get; set; }

        public string FileType { get; set; }

        public long? FileSize { get; set; }

        public string FilePath { get; set; }

        public string WebsiteSectionId { get; set; }
    }
}
