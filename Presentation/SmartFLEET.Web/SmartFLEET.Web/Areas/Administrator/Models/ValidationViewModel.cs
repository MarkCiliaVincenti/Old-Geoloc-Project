using System.Collections.Generic;
using System.Linq;

namespace SmartFLEET.Web.Areas.Administrator.Models
{
    public class ValidationViewModel
    {
       
        public ValidationViewModel(List<string> errors , string message)
        {
            Errors = errors;
            Message = message;
            ValidationStatus = Errors.Any() ? ValidationStatus.ValidationError : ValidationStatus.Ok;
        }
        public List<string> Errors { get; set; }
        public string Message { get; set; }
        public ValidationStatus ValidationStatus { get; set; }
    }

    public enum ValidationStatus
    {
        Ok,
        UnknownError,
        ValidationError
    }
}