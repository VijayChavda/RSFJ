using System;

namespace RSFJ.Model
{
    /// <summary>
    /// Represents a Rojmel entry.
    /// </summary>
    public class RojmelEntry
    {
        /// <summary>
        /// Identifier of current entry.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Date of entry.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Account holder's name.
        /// </summary>
        public Account Account { get; set; }

        /// <summary>
        /// The type of this Rojmel entry.
        /// </summary>
        public RojmelEntryType Type { get; set; }

        /// <summary>
        /// Stock item's name.
        /// </summary>
        public StockItem StockItem { get; set; }

        /// <summary>
        /// Parameter 1 of rojmel entry.
        /// </summary>
        public double Param1 { get; set; }

        /// <summary>
        /// Parameter 2 of rojmel entry.
        /// </summary>
        public double? Param2 { get; set; }

        /// <summary>
        /// Result of rojmel entry.
        /// </summary>
        public double Result { get; set; }

        /// <summary>
        /// Special parameter when Account type is Customer.
        /// </summary>
        public double? Labour { get; set; }

        /// <summary>
        /// Special parameter when Account type is Customer.
        /// </summary>
        public double? Waste { get; set; }

        /// <summary>
        /// Interval in days for partial payment of this entry.
        /// </summary>
        public int PartialPaymentInterval { get; set; }

        /// <summary>
        /// Days until full payment of this entry.
        /// </summary>
        public int FullPaymentDueDays { get; set; }

        /// <summary>
        /// Determines if Labour is interpreted as an Amount or otherwise as Rate.
        /// </summary>
        public bool IsLabourAsAmount { get; set; }

        /// <summary>
        /// Direction of rojmel entry. Represents credit/debit.
        /// </summary>
        public bool IsLeftSide { get; set; }

        public override string ToString()
        {
            return string.Concat("T-", Id);
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return (obj as RojmelEntry).Id == this.Id;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return Id;
        }

        public static bool operator ==(RojmelEntry a, RojmelEntry b)
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

        public static bool operator !=(RojmelEntry a, RojmelEntry b)
        {
            return !(a == b);
        }
    }
}
