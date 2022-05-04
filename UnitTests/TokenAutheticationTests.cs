using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using WowInventoryStats.Authentication;
using WowInventoryStats.Configuration;

namespace WowInventoryStats
{
    [TestClass]
    public class TestTokenAuthenticator
    {
        static AppConfiguration config = new();

        [TestMethod]
        public void DefaultTokenAuthenticatorValidTest()
        {
            TokenAuthenticator wowAuth = new();
            Assert.AreEqual(wowAuth.Token, new OAuthToken());
            Assert.AreEqual(wowAuth.IsAuthenticated, false);
        }

        [TestMethod]
        public async Task NullArugmentAuthenticationTest()
        {
            TokenAuthenticator wowAuth = new();

            string? client_id = null;
            string? client_secret = "12345";
            TokenCredentials credentials = new TokenCredentials(client_id, client_secret);
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => wowAuth.Authenticate(credentials));
            Assert.AreEqual(wowAuth.IsAuthenticated, false);

            client_id = "34343";
            client_secret = null;
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => wowAuth.Authenticate(credentials));
            Assert.AreEqual(wowAuth.IsAuthenticated, false);
        }

        [TestMethod]
        public async Task WrongAuthenticationTest()
        {
            TokenAuthenticator wowAuth = new();
            var client_id = "1234545sdfsf6";
            var client_secret = "1234545sdfsf6";
            TokenCredentials credentials = new TokenCredentials(client_id, client_secret);
            await Assert.ThrowsExceptionAsync<AuthenticationException>(() => wowAuth.Authenticate(credentials));
            Assert.AreEqual(wowAuth.IsAuthenticated, false);
        }

        [TestMethod]
        public async Task CorrectAuthenticationTest()
        {
            TokenAuthenticator wowAuth = new();
            Assert.AreEqual(wowAuth.IsAuthenticated, false);
            await wowAuth.Authenticate(config.Parameters.Credentials);
            Assert.AreEqual(wowAuth.IsAuthenticated, true);
        }
    }
}