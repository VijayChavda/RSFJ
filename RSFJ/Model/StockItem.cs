namespace RSFJ.Model
{
    /// <summary>
    /// Represents a stock item.
    /// </summary>
    public class StockItem
    {
        /// <summary>
        /// Name of the item.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// How much (weight) of this item is currently in stock.
        /// </summary>
        public double InStock { get; set; }

        /// <summary>
        /// The current rate of this stock item.
        /// </summary>
        public double Rate { get; set; }

        #region Equals, ==, != overrides
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return Name == (obj as StockItem).Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public static bool operator ==(StockItem a, StockItem b)
        {
            if (object.ReferenceEquals(a, b))
            {
                return true;
            }

            return a.Equals(b);
        }

        public static bool operator !=(StockItem a, StockItem b)
        {
            return !(a == b);
        }
        #endregion
    }
}
