

namespace BookieWookie.API.Services
{
    using System.Text;
    using Konscious.Security.Cryptography;
    using BookieWookie.API.Helpers;

    public interface IAuthenticationService
    {
        byte[] CreateSalt();
        byte[] HashPassword(string password, byte[] salt);
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

        public int ByteSize { get; init; } = 128;

        public int MemorgySize { get; init; } = 1024 * 100; // 100 MB

        public int Iterations { get; init; } = 40;

        public int DegreeOfParallelism { get; set; } = 8; // four cores

        public byte[] CreateSalt()
        {
            return System.Security.Cryptography.RandomNumberGenerator.GetBytes(this.ByteSize);
        }

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

        public bool VerifyHash(string password, byte[] salt, byte[] hash)
        {
            var newHash = HashPassword(password, salt);
            return hash.SequenceEqual(newHash);
        }
    }
}
