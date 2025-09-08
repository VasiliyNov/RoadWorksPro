using System.ComponentModel.DataAnnotations;

namespace RoadWorksPro.Models.ViewModels
{
    public class ServiceRequestViewModel
    {
        [Required(ErrorMessage = "Вкажіть ваше ім'я")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Вкажіть номер телефону")]
        [Phone(ErrorMessage = "Невірний формат телефону")]
        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Service { get; set; }

        [MaxLength(1000)]
        public string? Message { get; set; }
    }
}