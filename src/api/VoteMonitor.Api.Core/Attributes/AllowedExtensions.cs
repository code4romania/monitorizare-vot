using System.ComponentModel.DataAnnotations;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace VoteMonitor.Api.Core.Attributes
{
    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] _extensions;
        private readonly string _validationMessage;

        public AllowedExtensionsAttribute(string[] extensions) : this(extensions, "This extension is not allowed")
        {
        }

        public AllowedExtensionsAttribute(string[] extensions, string validationMessage)
        {
            _extensions = extensions;
            _validationMessage = validationMessage;
        }

        protected override ValidationResult IsValid(
            object value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                var extension = Path.GetExtension(file.FileName);

                if (!_extensions.Contains(extension.ToLower()))
                {
                    return new ValidationResult(_validationMessage);
                }

            }

            return ValidationResult.Success;
        }

    }
}