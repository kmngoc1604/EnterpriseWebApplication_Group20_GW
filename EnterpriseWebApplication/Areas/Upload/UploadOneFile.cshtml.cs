using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace EnterpriseWebApplication.Areas.Upload
{
    public class UploadOneFileModel : PageModel
    {

        private IWebHostEnvironment _environment;
        public UploadOneFileModel(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [Required(ErrorMessage = "Choose a file")]
        [DataType(DataType.Upload)]
        [FileExtensions(Extensions = "rar")]
        [Display(Name = "Choose file to upload")]
        [BindProperty]
        public IFormFile FileUpload { get; set; }
        public async Task OnPostAsync()
        {
            if (FileUpload != null)
            {
                var file = Path.Combine(_environment.ContentRootPath, "uploads", FileUpload.FileName);
                using (var fileStream = new FileStream(file, FileMode.Create))
                {
                    await FileUpload.CopyToAsync(fileStream);
                }
            }
        }
    }
}
