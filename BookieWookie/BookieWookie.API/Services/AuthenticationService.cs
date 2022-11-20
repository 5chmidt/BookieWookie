

namespace BookieWookie.API.Services
{
    using System.Text;
    using Konscious.Security.Cryptography;
    using BookieWookie.API.Helpers;
    using System.Security.Authentication;

    /// <summary>
    /// Interface should implent salting, hashing and verification.
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// Generate a cryptographically random number using initialized parameters.
        /// </summary>
        /// <returns></returns>
        byte[] CreateSalt();

        /// <summary>
        /// Generate a secure password has using the salt and initialized parameters.
        /// </summary>
        /// <param name="password"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        byte[] HashPassword(string password, byte[] salt);

        /// <summary>
        /// Verify newly hashed password against stored hash.
        /// </summary>
        /// <param name="password">Unmodified string password.</param>
        /// <param name="salt">Randomly generated number stored with hash.</param>
        /// <param name="hash">Stored password hash.</param>
        /// <returns>True if password matches stored hash.</returns>
        bool VerifyHash(string password, byte[] salt, byte[] hash);
    }

    /// <summary>
    /// Uses Argon2 implimentation from:
    /// https://github.com/kmaragon/Konscious.Security.Cryptography
    /// </summary>
    public class AuthenticationService : IAuthenticationService
    {
        /// <summary>
        /// Default value set targeted to process in 1 second.
        /// </summary>
        public AuthenticationService()
        {
        }

        /// <summary>
        /// Intialize different parameter values for secure password hashing.
        /// !!IMPORTANT!! Changing these values will create a different hash for the same password.
        /// </summary>
        /// <param name="byteSize">Number of bytes to be used in salt.</param>
        /// <param name="memorgySize">Amount of memeory to dedicate to hashing.</param>
        /// <param name="iterations">Number of times to hash password + salt.</param>
        /// <param name="degreeOfParallelism">Number of cores used in hashing.</param>
        public AuthenticationService(
            int byteSize,
            int memorgySize,
            int iterations,
            int degreeOfParallelism)
        {
            ByteSize = byteSize;
            MemorgySize = memorgySize;
            Iterations = iterations;
            DegreeOfParallelism = degreeOfParallelism;
        }

        /// <summary>
        /// Number of kb to be used in salt.
        /// </summary>
        public int ByteSize { get; init; } = 128;

        /// <summary>
        /// Amount of memeory (bytes) to dedicate to hashing.
        /// </summary>
        public int MemorgySize { get; init; } = 1024 * 100; // 100 MB

        /// <summary>
        /// Number of times to hash password + salt.
        /// </summary>
        public int Iterations { get; init; } = 40;

        /// <summary>
        /// Number of cores used in hashing.
        /// </summary>
        public int DegreeOfParallelism { get; init; } = 8; // four cores
    
        /// <inheritdoc/>
        public byte[] CreateSalt()
        {
            return System.Security.Cryptography.RandomNumberGenerator.GetBytes(this.ByteSize);
        }
        
        /// <inheritdoc/>
        public byte[] HashPassword(string password, byte[] salt)
        {
            byte[] data = Encoding.UTF8.GetBytes(password);
            var argon2 = new Argon2i(data);

            argon2.Salt = salt;
            argon2.DegreeOfParallelism = this.DegreeOfParallelism;
            argon2.Iterations = this.Iterations;
            argon2.MemorySize = this.MemorgySize;

            return argon2.GetBytes(this.ByteSize);
        }
        
        /// <inheritdoc/>
        public bool VerifyHash(string password, byte[] salt, byte[] hash)
        {
            var newHash = HashPassword(password, salt);
            return hash.SequenceEqual(newHash);
        }

        /// <summary>
        /// Check user password meets requirements.
        /// </summary>
        /// <param name="password">Password user is attmepting to set.</param>
        /// <param name="throwException">If true an exception will be thrown otherwise return false.</param>
        /// <returns>True if successful, else throws an exception.</returns>
        /// <exception cref="AuthenticationException"></exception>
        public static bool CheckPasswordRequirements(string? password, bool throwException = true)
        {
            //TODO: move to app.settings //
            int minLength = 8;

            // set the most basic password of requirements //
            if (password == null || password.Length < minLength)
            {
                if (throwException)
                {
                    throw new AuthenticationException($"Password must be at least 8 charectors long.");
                }
                else
                {
                    return false;
                }
            }

            return true;
        }
    }
}
