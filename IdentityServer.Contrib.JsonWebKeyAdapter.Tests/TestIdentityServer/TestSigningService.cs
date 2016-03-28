using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Jose;
using Microsoft.IdentityModel.Protocols;

namespace IdentityServer.Contrib.JsonWebKeyAdapter.Tests.TestIdentityServer
{
    public class TestSigningService : ISigningService
    {
        private readonly RSACryptoServiceProvider mService;

        public TestSigningService()
        {
            mService = new RSACryptoServiceProvider(2048);
        }

        public Task<IEnumerable<JsonWebKey>> GetPublicKeysAsync()
        {
            var exportParameters = mService.ExportParameters(false);
            var jwk = new JsonWebKey
            {
                N = Convert.ToBase64String(exportParameters.Modulus),
                E = Convert.ToBase64String(exportParameters.Exponent),
                Kty = "RSA",
                Kid = "1"
            };

            return Task.FromResult(Enumerable.Repeat(jwk, 1));
        }

        public Task<string> EncodeToSignedJWTAsync(JwtPayload base64EncodedPayload)
        {
            var headers = new Dictionary<string, object> {{"kid", "1"}};

            return Task.FromResult(JWT.Encode(base64EncodedPayload.SerializeToJson(), mService, JwsAlgorithm.RS256, headers));
        }
    }
}