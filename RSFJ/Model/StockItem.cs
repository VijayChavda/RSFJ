using RSFJ.Services;
using RSFJ.ViewModels;
using System.Collections.Generic;
using System.ComponentModel;

namespace RSFJ.Model
{
    /// <summary>
    /// Represents a stock item.
    /// </summary>
    public class StockItem : ViewModelBase
    {
        /// <summary>
        /// Name of the stock item.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// How much amount of this stock is in possession.
        /// </summary>
        public double InStock { get => _InStock; set => SetProperty(ref _InStock, value); }
        private double _InStock;

        /// <summary>
        /// The rate/purity of this item.
        /// </summary>
        public double Rate_Purity { get => _Rate_Purity; set => SetProperty(ref _Rate_Purity, value); }
        public double _Rate_Purity;

        /// <summary>
        /// Normalised value of this stock item as gold.
        /// </summary>
        public double EquivalentGold { get => _EquivalentGold; set => SetProperty(ref _EquivalentGold, value); }
        public double _EquivalentGold;

        protected override void APropertyChanged<T>(string PropertyName, T OldValue, T NewValue)
        {
            if (PropertyName == nameof(InStock) || PropertyName == nameof(Rate_Purity))
            {
                if (DataContextService.Instance.DataContext == null)
                {
                    EquivalentGold = 0;
                    return;
                }

                if (this == DataContextService.Instance.DataContext.Cash)
                {
                    EquivalentGold = (double)(InStock / Rate_Purity);
                }
                else
                {
                    EquivalentGold = (double)(InStock * Rate_Purity / 100);
                }
            }
        }

        public override string ToString()
        {
            return Name;
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return (obj as StockItem).Name == this.Name;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public static bool operator ==(StockItem a, StockItem b)
        {
            // If both are null, or both are same instance, return true.
            if (object.ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            return a.Equals(b);
        }

        public static bool operator !=(StockItem a, StockItem b)
        {
            return !(a == b);
        }
    }
}
