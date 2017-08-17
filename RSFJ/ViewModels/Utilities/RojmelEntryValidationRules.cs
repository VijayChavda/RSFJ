using RSFJ.Model;
using RSFJ.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace RSFJ.ViewModels.Utilities
{
    public class RojmelEntryValidationRules : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var entry = (value as BindingGroup).Items[0] as RojmelEntryViewModel;

            string temp = entry.Validate();
            if (temp != null)
            {
                return new ValidationResult(false, temp);
            }

            entry.CommitEntry();

            return ValidationResult.ValidResult;
        }
    }
}
