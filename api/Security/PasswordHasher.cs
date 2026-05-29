using System.Security.Cryptography;

namespace ESCOLA_API.Security
{
    public static class PasswordHasher
    {
        private const string Algorithm = "PBKDF2-SHA256";
        private const int Iterations = 100000;
        private const int SaltSize = 16;
        private const int KeySize = 32;

        public static string HashPassword(string password, string? salt = null)
        {
            var saltBytes = salt == null
                ? RandomNumberGenerator.GetBytes(SaltSize)
                : Convert.FromBase64String(salt);

            var hashBytes = Rfc2898DeriveBytes.Pbkdf2(
                password,
                saltBytes,
                Iterations,
                HashAlgorithmName.SHA256,
                KeySize);

            var hash = Convert.ToBase64String(hashBytes);
            return $"{Algorithm}${Iterations}${Convert.ToBase64String(saltBytes)}${hash}";
        }

        public static bool VerifyPassword(string password, string storedHash)
        {
            var parts = storedHash.Split('$');
            if (parts.Length != 4 || parts[0] != Algorithm)
            {
                return false;
            }

            if (!int.TryParse(parts[1], out var iterations))
            {
                return false;
            }

            var saltBytes = Convert.FromBase64String(parts[2]);
            var expectedHashBytes = Convert.FromBase64String(parts[3]);

            var actualHashBytes = Rfc2898DeriveBytes.Pbkdf2(
                password,
                saltBytes,
                iterations,
                HashAlgorithmName.SHA256,
                expectedHashBytes.Length);

            return CryptographicOperations.FixedTimeEquals(actualHashBytes, expectedHashBytes);
        }
    }
}
