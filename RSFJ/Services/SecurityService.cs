using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RSFJ.Services
{
    /// <summary>
    /// Service that provides various security features.
    /// </summary>
    public class SecurityService
    {
        /// <summary>
        /// The only instance of SecurityService.
        /// </summary>
        public static SecurityService Instance => _Instance ?? (_Instance = new SecurityService());
        private static SecurityService _Instance;

        /// <summary>
        /// Computes a hash for the given string.
        /// </summary>
        /// <param name="secret">A string whose hash is to be computed.</param>
        /// <returns></returns>
        public string Hash(string secret)
        {
            if (String.IsNullOrEmpty(secret))
                return String.Empty;

            using (var sha = new SHA256Managed())
            {
                byte[] textData = Encoding.UTF8.GetBytes(secret);
                byte[] hash = sha.ComputeHash(textData);
                return BitConverter.ToString(hash).Replace("-", String.Empty);
            }
        }
    }
}
