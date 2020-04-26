using Microsoft.AspNetCore.DataProtection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _24hplusdotnetcore.Services
{
    public class CipherServices
    {
        private readonly IDataProtectionProvider _dataProtectionProvider;
        private const string Key = "P9NIloZwV0lYGANEEr9xIZq5CXiKjBcG;";
        public CipherServices(IDataProtectionProvider dataProtectionProvider)
        {
            _dataProtectionProvider = dataProtectionProvider;
        }
        public string Encrypt(string input)
        {
            var protector = _dataProtectionProvider.CreateProtector(Key);
            return protector.Protect(input);
        }

        public string Decrypt(string cipherText)
        {
            var protector = _dataProtectionProvider.CreateProtector(Key);
            return protector.Unprotect(cipherText);
        }
    }
}
