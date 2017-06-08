namespace RSFJ.Model
{
    /// <summary>
    /// Represents a contact person's account.
    /// </summary>
    public class Account
    {
        /// <summary>
        /// Name of the contact.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Phone number.
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Something to note about the contact.
        /// </summary>
        public string Note { get; set; }

        /// <summary>
        /// Fine due.
        /// </summary>
        public double FineDue { get; set; }

        /// <summary>
        /// Uplak due.
        /// </summary>
        public double UplakDue { get; set; }
    }
}
