using System.ComponentModel.DataAnnotations;

namespace RoadWorksPro.Models.ViewModels
{
    public class ServiceCreateViewModel
    {
        [Required(ErrorMessage = "Вкажіть назву послуги")]
        [MaxLength(200)]
        [Display(Name = "Назва")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(2000)]
        [Display(Name = "Опис")]
        public string? Description { get; set; }

        [MaxLength(100)]
        [Display(Name = "Інформація про ціну")]
        public string? PriceInfo { get; set; }

        [Display(Name = "Активна")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Зображення")]
        public IFormFile? ImageFile { get; set; }
    }

    public class ServiceEditViewModel : ServiceCreateViewModel
    {
        public int Id { get; set; }
        public string? CurrentImagePath { get; set; }
    }
}