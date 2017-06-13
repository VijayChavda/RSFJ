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
        public string Account { get; set; }

        /// <summary>
        /// Stock item's name.
        /// </summary>
        public string StockItem { get; set; }

        /// <summary>
        /// Gets or sets a value that determines if uplak is clear.
        /// </summary>
        public bool UplakClear { get; set; }

        /// <summary>
        /// Parameter 1 of rojmel entry.
        /// </summary>
        public double Param1 { get; set; }

        /// <summary>
        /// Parameter 2 of rojmel entry.
        /// </summary>
        public double Param2 { get; set; }

        /// <summary>
        /// Result of rojmel entry.
        /// </summary>
        public double Result { get; set; }

        /// <summary>
        /// Direction of rojmel entry. Represents credit/debit.
        /// </summary>
        public bool IsLeftSide { get; set; }

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
