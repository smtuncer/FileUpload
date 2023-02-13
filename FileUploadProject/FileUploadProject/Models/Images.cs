using System.ComponentModel.DataAnnotations;

namespace FileUploadProject.Models
{
    public class Images
    {
        [Key]
        public int Id { get; set; }
        public string ImagePath { get; set; }
    }
}
