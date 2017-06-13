namespace RSFJ.Model
{
    /// <summary>
    /// Represents an account.
    /// </summary>
    public class Account
    {
        /// <summary>
        /// Name of the account head.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Contact phone number of the account.
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Fine due in gold weight.
        /// </summary>
        public double FineInGold { get; set; }

        /// <summary>
        /// Fine due in money cash.
        /// </summary>
        public double FineInMoney { get; set; }

        /// <summary>
        /// A group to categorize and organize accounts.
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// Something to note about this contact.
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// They usual type of transaction done with this account.
        /// </summary>
        public string PreferredTransactionType { get; set; }

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return (obj as Account).Name == this.Name;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public static bool operator ==(Account a, Account b)
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

        public static bool operator !=(Account a, Account b)
        {
            return !(a == b);
        }
    }
}
