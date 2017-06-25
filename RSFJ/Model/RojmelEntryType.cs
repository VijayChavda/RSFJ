namespace RSFJ.Model
{
    /// <summary>
    /// Represents various types of Rojmel entry types.
    /// </summary>
    public sealed class RojmelEntryType
    {
        /// <summary>
        /// Newly added stock item.
        /// </summary>
        public const string Initital = "Initial";

        /// <summary>
        /// Exchange of a stock item.
        /// </summary>
        public const string Exchange = "Exchange";

        /// <summary>
        /// Money given/taken for an stock item previously taken/given.
        /// </summary>
        public const string Uplak = "Uplak";

        /// <summary>
        /// Due money calculated and decided for an stock item previously taken/given.
        /// </summary>
        public const string UplakClear = "Uplak Clear";

        /// <summary>
        /// Exchanging and instantly paying money.
        /// </summary>
        public const string InstantCash = "InstantCash";

        /// <summary>
        /// Exchange of fine gold with a bullion.
        /// </summary>
        public const string Bullion = "Bullion";

        /// <summary>
        /// Exchange of a stock item with a customer.
        /// </summary>
        public const string Customer = "Customer";
    }
}