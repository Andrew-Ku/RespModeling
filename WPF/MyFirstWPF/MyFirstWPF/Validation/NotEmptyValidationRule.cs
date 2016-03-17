using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MyFirstWPF.Validation
{
    public class NotEmptyValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            return (string)value == string.Empty ? new ValidationResult(false, "Значение не должно быть пусто") : new ValidationResult(true, null);
        }
    }
}
