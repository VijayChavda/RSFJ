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

            if (entry.Account == null)
            {
                return new ValidationResult(false, "Account is required");
            }
            if (entry.StockItem == null)
            {
                return new ValidationResult(false, "Stock item is required");
            }
            if (entry.Param1 == null)
            {
                return new ValidationResult(false, "Param 1 is required");
            }
            if (entry.StockItem != DataContextService.Instance.DataContext.Cash && entry.Param2 == null)
            {
                return new ValidationResult(false, "Param 2 is required");
            }

            entry.CommitEntry();
            
            return ValidationResult.ValidResult;
        }
    }
}
