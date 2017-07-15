using RSFJ.ViewModels;

namespace RSFJ.Model
{
    /// <summary>
    /// Represents an account.
    /// </summary>
    public class Account : ViewModelBase
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
        public double FineInGold { get => _FineInGold; set => SetProperty(ref _FineInGold, value); }
        private double _FineInGold;

        /// <summary>
        /// Fine due in money cash.
        /// </summary>
        public double FineInMoney { get => _FineInMoney; set => SetProperty(ref _FineInMoney, value); }
        private double _FineInMoney;

        /// <summary>
        /// A group to categorize and organize accounts.
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// Something to note about this contact.
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Type of this account.
        /// </summary>
        public AccountType Type { get; set; }

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
