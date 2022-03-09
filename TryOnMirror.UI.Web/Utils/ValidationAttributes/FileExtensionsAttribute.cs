using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SymaCord.TryOnMirror.UI.Web.Utils.ValidationAttributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class FileExtensionsAttribute : ValidationAttribute
    {
        private List<string> ValidExtensions { get; set; }

        public FileExtensionsAttribute(string fileExtensions)
        {
            ValidExtensions = fileExtensions.ToLower().Split('|').ToList();
        }

        public override bool IsValid(object value)
        {
            var file = value as HttpPostedFileBase;
            if (file != null)
            {
                var fileName = file.FileName.ToLower();
                var isValidExtension = ValidExtensions.Any(y => fileName.EndsWith(y));
                return isValidExtension;
            }
            return true;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientFileExtensionValidationRule(ErrorMessage, ValidExtensions);
            yield return rule;
        }

    }

    public class ModelClientFileExtensionValidationRule : ModelClientValidationRule
    {
        public ModelClientFileExtensionValidationRule(string errorMessage, List<string> fileExtensions)
        {
            ErrorMessage = errorMessage;
            ValidationType = "fileextensions";
            ValidationParameters.Add("fileextensions", string.Join(",", fileExtensions));
        }
    }

}