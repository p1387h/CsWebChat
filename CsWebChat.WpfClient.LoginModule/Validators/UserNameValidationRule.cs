using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CsWebChat.WpfClient.LoginModule.Validators
{
    class UserNameValidationRule : ValidationRule
    {
        private static readonly int MIN_LENGTH = 4;
        private static readonly int MAX_LENGTH = 20;
        private static readonly Regex CHARACTER_REGEX = new Regex(@"^[a-zA-Z0-9]*$");

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            ValidationResult result;
            var name = value as string;

            if(String.IsNullOrEmpty(name))
            {
                if(name == null)
                {
                    result = new ValidationResult(false, "Name must be set.");
                }
                else
                {
                    result = new ValidationResult(false, "Name must not be empty.");
                }
            }
            else
            {
                var lengthOk = name.Length >= MIN_LENGTH && name.Length <= MAX_LENGTH;
                var charactersOk = CHARACTER_REGEX.IsMatch(name);

                if(!lengthOk)
                {
                    var errorMessage = String.Format("Name must contain {0} to {1} characters.", MIN_LENGTH, MAX_LENGTH);
                    result = result = new ValidationResult(false, errorMessage);
                }
                else if(!charactersOk)
                {
                    result = result = new ValidationResult(false, "Name must only contain characters: a-z, A-Z, 0-9");
                }
                else
                {
                    result = ValidationResult.ValidResult;
                }
            }

            return result;
        }
    }
}
