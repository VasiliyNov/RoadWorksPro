using System.ComponentModel.DataAnnotations;

namespace RoadWorksPro.Models.ViewModels
{
    public class ProductCreateViewModel
    {
        [Required(ErrorMessage = "Вкажіть назву товару")]
        [MaxLength(200)]
        [Display(Name = "Назва")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        [Display(Name = "Опис")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Вкажіть ціну")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Ціна повинна бути більше 0")]
        [Display(Name = "Ціна")]
        public decimal Price { get; set; }

        [MaxLength(100)]
        [Display(Name = "Розміри")]
        public string? Dimensions { get; set; }

        [Required(ErrorMessage = "Оберіть категорію")]
        [Display(Name = "Категорія")]
        public string Category { get; set; } = "other";

        [MaxLength(100)]
        [Display(Name = "Матеріал")]
        public string? Material { get; set; }

        [MaxLength(100)]
        [Display(Name = "Стандарт")]
        public string? Standard { get; set; }

        [Display(Name = "Активний")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Зображення")]
        public IFormFile? ImageFile { get; set; }
    }

    public class ProductEditViewModel : ProductCreateViewModel
    {
        public int Id { get; set; }
        public string? CurrentImagePath { get; set; }
    }
}