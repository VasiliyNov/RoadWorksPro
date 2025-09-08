using System.ComponentModel.DataAnnotations;
using RoadWorksPro.Models.Entities;

namespace RoadWorksPro.Models.ViewModels
{
    public class PortfolioCreateViewModel
    {
        [Required(ErrorMessage = "Вкажіть назву проекту")]
        [MaxLength(200)]
        [Display(Name = "Назва")]
        public string Title { get; set; } = string.Empty;

        [MaxLength(200)]
        [Display(Name = "Локація")]
        public string? Location { get; set; }

        [MaxLength(2000)]
        [Display(Name = "Опис")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Оберіть категорію")]
        [Display(Name = "Категорія")]
        public string Category { get; set; } = "other";

        [MaxLength(100)]
        [Display(Name = "Обсяг робіт")]
        public string? WorkVolume { get; set; }

        [MaxLength(200)]
        [Display(Name = "Матеріали")]
        public string? Materials { get; set; }

        [Display(Name = "Дата виконання")]
        public DateTime CompletedDate { get; set; } = DateTime.Now;

        [Display(Name = "Розмір картки")]
        public CardSize DisplaySize { get; set; } = CardSize.Normal;

        [Display(Name = "Порядок відображення")]
        public int DisplayOrder { get; set; }

        [Display(Name = "Рекомендована робота")]
        public bool IsFeatured { get; set; }

        [Display(Name = "Активна")]
        public bool IsActive { get; set; } = true;

        [Required(ErrorMessage = "Додайте головне зображення")]
        [Display(Name = "Головне зображення")]
        public IFormFile MainImageFile { get; set; } = null!;

        [Display(Name = "Додаткові зображення")]
        public List<IFormFile>? AdditionalImageFiles { get; set; }
    }

    public class PortfolioEditViewModel : PortfolioCreateViewModel
    {
        public int Id { get; set; }
        public string? CurrentMainImagePath { get; set; }
        public List<PortfolioImage> CurrentAdditionalImages { get; set; } = new();

        public new IFormFile? MainImageFile { get; set; }
    }
}