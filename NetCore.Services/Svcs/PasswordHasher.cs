using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using NetCore.Services.Bridges;
using NetCore.Services.Data;
using NetCore.Services.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.Services.Svcs
{
    public class PasswordHasher : IPasswordHasher
    {
        private DBFirstDbContext _context;

        public PasswordHasher(DBFirstDbContext context)
        {
            _context = context;
        }



        #region private methods
        private string GetGUIDSalt()
        {
            return Guid.NewGuid().ToString();
        }
        private string GetRNGSalt()
        {
            // generate a 128-bit salt using a secure PRNG
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            return Convert.ToBase64String(salt);
        }

        // 아이디 소문자 처리 .ToLower()
        private string GetPasswordHash(string userId, string password, string rngSalt, string guidSalt)
        {
            // derive a 256-bit subkey (use HMACSHA1 with 10,000 iterations)
            // Pbkdf2
            // Password Based key derivation function 2
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: userId.ToLower() + password + guidSalt,
                salt: Encoding.UTF8.GetBytes(rngSalt),
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: 45000, // 10000, 25000, 45000
                numBytesRequested: 256 / 8));
        }

        private bool CheckThePasswordInfo(string userId, string password, string rngSalt, string guidSalt, string passwordHashed)
        {
            return GetPasswordHash(userId, password, rngSalt, guidSalt).Equals(passwordHashed);
        }

        private PasswordHashInfo PasswordInfo(string userId, string password)
        {
            string guidSalt = GetGUIDSalt();
            string rngSalt = GetRNGSalt();
            var passwordInfo = new PasswordHashInfo()
            {
                GUIDSalt = guidSalt,
                RNGSalt = rngSalt,
                PasswordHash = GetPasswordHash(userId, password, rngSalt, guidSalt)
            };

            return passwordInfo;
        }
        #endregion
        string IPasswordHasher.GetGUIDSalt()
        {
            return GetGUIDSalt();
        }

        string IPasswordHasher.GetRNGSalt()
        {
            return GetRNGSalt();
        }

        string IPasswordHasher.GetPasswordHash(string userId, string password, string rngSalt, string guidSalt)
        {
            return GetPasswordHash(userId, password, rngSalt, guidSalt);
        }

        //bool IPasswordHasher.MatchTheUserInfo(string userId, string password)
        //{
        //    var user = _context.Users.Where(u => u.UserId.Equals(userId)).FirstOrDefault();
        //    string rngSalt = user.RNGSalt;
        //    string guidSalt = user.GUIDSalt;
        //    string passwordHashed = user.PasswordHash;
        //    return CheckThePasswordInfo(userId, password, rngSalt, guidSalt, passwordHashed);
        //}

        bool IPasswordHasher.CheckThePasswordInfo(string userId, string password, string rngSalt, string guidSalt, string passwordHashed)
        {
            return CheckThePasswordInfo(userId, password, rngSalt, guidSalt, passwordHashed);
        }

        PasswordHashInfo IPasswordHasher.SetPasswordInfo(string userId, string password)
        {
            return PasswordInfo(userId, password);
        }
    }
}
