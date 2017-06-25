using System.Collections.Generic;

namespace RSFJ.Model
{
    /// <summary>
    /// Represents a stock item.
    /// </summary>
    public class StockItem
    {
        /// <summary>
        /// Name of the stock item.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// How much amount of this stock is in possession.
        /// </summary>
        public double InStock { get; set; }

        /// <summary>
        /// The rate/purity of this item.
        /// </summary>
        public double Rate_Purity { get; set; }

        /// <summary>
        /// Normalised value of this stock item as gold.
        /// </summary>
        public double EquivalentGold { get; set; }

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
