using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MyFirstWPF.Validation
{
    internal class WeightValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value != null)
            {
              //  var strValue = (value as string).Replace(",", ".");
                double proposedValue;

                if (!double.TryParse(value.ToString(), NumberStyles.Number, CultureInfo.InvariantCulture, out proposedValue))
                {
                    return new ValidationResult(false, "'" + value + "' Значение веса должно быть числом");
                }

                if (proposedValue < 0.0)
                {
                    // Something was wrong.
                    return new ValidationResult(false, "Вес должен быть положительным");
                }
            }

            // Everything OK.
            return new ValidationResult(true, null);
        }
    }
}
