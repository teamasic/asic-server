using AsicServer.Infrastructure;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace AsicServer.Core.Utils
{
    public class PasswordManipulation
    {
        public static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var encrypt = new HMACSHA256())
            {
                passwordSalt = encrypt.Key;
                passwordHash = encrypt.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        public static bool VerifyPasswordHash(string password, byte[] storedPasswordHash, byte[] storedPasswordSalt)
        {
            if (storedPasswordHash == null || storedPasswordSalt == null)
            {
                return false;
            }
            using (var encrypt = new HMACSHA256(storedPasswordSalt))
            {
                var passwordHash = encrypt.ComputeHash(Encoding.UTF8.GetBytes(password));

                for (int i = 0; i < storedPasswordHash.Length; i++)
                {
                    if (storedPasswordHash[i] != passwordHash[i])
                        return false;
                }
            }
            return true;
        }


    }
}
