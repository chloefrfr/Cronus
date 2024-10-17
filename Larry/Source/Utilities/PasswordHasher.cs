using System.Security.Cryptography;

namespace Larry.Source.Utilities
{
    public class PasswordHasher
    {
        /// <summary>
        /// Hashes a password with a randomly generated salt.
        /// </summary>
        /// <param name="password">The password to hash.</param>
        /// <returns>
        /// A tuple containing the hashed password and the salt, both as Base64 strings.
        /// </returns>
        public static (string hashedPassword, string salt) HashPassword(string password)
        {
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000))
            {
                byte[] hash = pbkdf2.GetBytes(20);
                byte[] hashBytes = new byte[36];
                Array.Copy(salt, 0, hashBytes, 0, 16);
                Array.Copy(hash, 0, hashBytes, 16, 20);
                string hashedPassword = Convert.ToBase64String(hashBytes);
                return (hashedPassword, Convert.ToBase64String(salt));
            }
        }

        /// <summary>
        /// Verifies a password against a hashed password.
        /// </summary>
        /// <param name="password">The password to verify.</param>
        /// <param name="hashedPassword">The hashed password to compare against.</param>
        /// <returns>
        /// True if the password matches the hashed password; otherwise, false.
        /// </returns>
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            byte[] hashBytes = Convert.FromBase64String(hashedPassword);
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000))
            {
                byte[] hash = pbkdf2.GetBytes(20);

                for (int i = 0; i < 20; i++)
                {
                    if (hashBytes[i + 16] != hash[i])
                    {
                        return false; 
                    }
                }
                return true; 
            }
        }
    }
}
