using System.ComponentModel.DataAnnotations;

namespace RoadWorksPro.Models.ViewModels
{
    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "Вкажіть ваше ім'я")]
        [Display(Name = "Ім'я")]
        [MaxLength(100)]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Вкажіть номер телефону")]
        [Display(Name = "Телефон")]
        [Phone(ErrorMessage = "Невірний формат телефону")]
        [MaxLength(20)]
        public string CustomerPhone { get; set; } = string.Empty;

        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Невірний формат email")]
        [MaxLength(100)]
        public string? CustomerEmail { get; set; }

        [Display(Name = "Коментар до замовлення")]
        [MaxLength(1000)]
        public string? Comment { get; set; }

        public CartViewModel Cart { get; set; } = new();
    }
}