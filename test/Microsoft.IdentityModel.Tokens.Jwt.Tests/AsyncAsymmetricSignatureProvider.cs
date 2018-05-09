using Microsoft.IdentityModel.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.IdentityModel.Tokens.Jwt.Tests
{
    public class AsyncAsymmetricSignatureProvider: AsymmetricSignatureProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncAsymmetricSignatureProvider"/> class used to create and verify signatures.
        /// </summary>
        /// <param name="key">The <see cref="SecurityKey"/> that will be used for signature operations.<see cref="SecurityKey"/></param>
        /// <param name="algorithm">The signature algorithm to apply.</param>
        public AsyncAsymmetricSignatureProvider(SecurityKey key, string algorithm, bool willCreateSignatures)
            : base(key, algorithm, willCreateSignatures)
        {
        }

        public override async Task<byte[]> SignAsync(byte[] input)
        {
            await Task.Delay(2000);
            return base.Sign(input);
        }

        public override async Task<bool> VerifyAsync(byte[] input, byte[] signature)
        {
            await Task.Delay(2000);
            return base.Verify(input, signature);
        }
    }
}
