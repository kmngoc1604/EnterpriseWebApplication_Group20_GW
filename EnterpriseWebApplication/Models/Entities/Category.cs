using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EnterpriseWebApplication.Models.Entities
{
    public class Category : BaseEntity
    {
        [Required(ErrorMessage = "Category must have a name")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "{0} must be from {1} to {2} characters.")]
        [Display(Name = "Category")]
        public string Name { get; set; }

        [Display(Name = "Parents category")]
        public int? ParentCategoryId { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Content")]
        public string Content { set; get; }

        [Required(ErrorMessage = "Must have an url")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "{0} must be from {1} to {2} characters.")]
        [RegularExpression(@"^[a-z0-9-]*$", ErrorMessage = "Only use characters from [a-z0-9-]")]
        [Display(Name = "Url")]
        public string Slug { set; get; }

        [ForeignKey("ParentCategoryId")]
        [Display(Name = "Parents category")]
        public Category ParentCategory { set; get; }


        public ICollection<Category> CategoryChildren { get; set; }
    
        public virtual ICollection<NIdea> Ideas { get; set; }
    }
}
