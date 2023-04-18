using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.PasswordHasher
{
    internal class Program
    {
        // Password => GUIDSalt, RNGSalt, PasswordHash
        static void Main(string[] args)
        {
            Console.Write("아이디 : ");
            string userId = Console.ReadLine();

            Console.Write("비밀번호: ");
            string password = Console.ReadLine();

            string guidSalt = Guid.NewGuid().ToString();

            string rngSalt = GetRNGSalt();

            string passwordHashed = GetPasswordHash(userId, password, rngSalt, guidSalt);

            bool check = CheckThePasswordInfo(userId, password, rngSalt, guidSalt, passwordHashed); // 데이터베이스의 비밀번호 정보와 현재 입력한 비밀번호 정보를 비교해서 같은 해시값일 경우 check를 true로 변경


            Console.WriteLine($"userId :{userId}");
            Console.WriteLine($"password :{password}");
            Console.WriteLine($"GuidSalt: {guidSalt}");
            Console.WriteLine($"RNGSalt: {rngSalt}");
            Console.WriteLine($"passwordHashed: {passwordHashed}");
            Console.WriteLine($"CheckThePasswordInfo: {(check ? "비밀번호가 일치합니다.":"불일치")}");

            Console.ReadLine();
        }

        private static string GetRNGSalt()
        {
            // generate a 128-bit salt using a secure PRNG
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            return Convert.ToBase64String(salt);
        }

        private static string GetPasswordHash(string userId, string password, string rngSalt,string guidSalt)
        {
            // derive a 256-bit subkey (use HMACSHA1 with 10,000 iterations)
            // Pbkdf2
            // Password Based key derivation function 2
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: userId + password + guidSalt,
                salt: Encoding.UTF8.GetBytes(rngSalt),
                prf: KeyDerivationPrf.HMACSHA512,
                iterationCount: 45000, // 10000, 25000, 45000
                numBytesRequested: 256 / 8));
        }

        private static bool CheckThePasswordInfo(string userId, string password, string rngSalt, string guidSalt, string passwordHashed)
        {
            return GetPasswordHash(userId, password, rngSalt, guidSalt).Equals(passwordHashed);
        }
    }
}
