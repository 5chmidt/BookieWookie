namespace BookieWookie.Test
{
    using NUnit.Framework;
    using NUnit.Framework.Internal;
    using static API.Services.AuthenticationService;

    public class AuthenticationTests
    {
        private API.Services.IAuthenticationService AuthenticationService;

        [SetUp]
        public void Setup() 
        {
            this.AuthenticationService = new API.Services.AuthenticationService();
        }

        /// <summary>
        /// Ensure it will not produce the same hash for two differnt users with the same password.
        /// </summary>
        [Test]
        public void SaltTest()
        {
            string password = Guid.NewGuid().ToString();

            byte[] salt1 = this.AuthenticationService.CreateSalt();
            byte[] salt2 = this.AuthenticationService.CreateSalt();
            // make sure salt is different //
            Assert.That(salt1, Is.Not.EqualTo(salt2));

            var hash1 = this.AuthenticationService.HashPassword(password, salt1);
            var hash2 = this.AuthenticationService.HashPassword(password, salt1);
            var hash3 = this.AuthenticationService.HashPassword(password, salt2);

            // same password same salt, same hash //
            Assert.That(hash1, Is.EqualTo(hash2));

            // same password, different salt, different hash //
            Assert.That(hash1, Is.Not.EqualTo(hash3));
        }

        /// <summary>
        /// Check hashing to make sure password can be verified.
        /// </summary>
        [Test]
        public void HashingTest()
        {
            byte[] salt = this.AuthenticationService.CreateSalt();
            string password = Guid.NewGuid().ToString();
            byte[] hash = this.AuthenticationService.HashPassword(password, salt);
            bool result = this.AuthenticationService.VerifyHash(password, salt, hash);
            Assert.That(result, Is.True);
        }

        /// <summary>
        /// Test if password is too short to meet requirements.
        /// </summary>
        [Test]
        public void ShortPasswordTest()
        {
            string password = "test123";
            bool result = CheckPasswordRequirements(password, false);
            Assert.That(result, Is.False);
        }

        /// <summary>
        /// Test if password is null.
        /// </summary>
        [Test]
        public void NullPasswordTest()
        {
            string password = null;
            bool result = CheckPasswordRequirements(password, false);
            Assert.That(result, Is.False);
        }

        /// <summary>
        /// Test strong password.
        /// </summary>
        [Test]
        public void StrongPasswordTest() 
        { 
            string password = Guid.NewGuid().ToString();
            bool result = CheckPasswordRequirements(password, false);
            Assert.That(result, Is.True);
        }
    }
}