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
    }
}
