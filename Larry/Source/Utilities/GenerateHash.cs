using System.Security.Cryptography;
using System.Text;

namespace Larry.Source.Utilities
{
    public class GenerateHash
    {
        /// <summary>
        /// Generates a hash from the provided string using the specified hash algorithm.
        /// </summary>
        /// <param name="content">The string to be hashed.</param>
        /// <param name="algorithm">The <see cref="HashAlgorithmName"/> to use for hashing (e.g., SHA256, MD5).</param>
        /// <returns>A hexadecimal string representation of the computed hash.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the specified hash algorithm cannot be created.</exception>
        public static string Generate(string content, HashAlgorithmName algorithm)
        {
            using (var hashAlgorithm = HashAlgorithm.Create(algorithm.Name))
            {
                if (hashAlgorithm == null) throw new InvalidOperationException("Unable to create hash algorithm.");

                var hash = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(content));
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            }
        }

        /// <summary>
        /// Generates a hash from the provided byte array using the specified hash algorithm.
        /// </summary>
        /// <param name="data">The byte array to be hashed.</param>
        /// <param name="algorithm">The <see cref="HashAlgorithm"/> instance to use for hashing.</param>
        /// <returns>A hexadecimal string representation of the computed hash.</returns>
        public static string GenerateHashByByte(byte[] data, HashAlgorithm algorithm)
        {
            using (algorithm)
            {
                byte[] hash = algorithm.ComputeHash(data);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}
