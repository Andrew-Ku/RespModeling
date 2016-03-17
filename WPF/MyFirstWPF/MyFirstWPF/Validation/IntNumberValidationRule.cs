using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MyFirstWPF.Validation
{
    public class IntNumberValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
               int.Parse((string) value);
            }
            catch
            {
                return new ValidationResult(false, "Недопустимое значение");
            }

            return new ValidationResult(true, null);
        }
    }
}
