using System;
using System.Security.Cryptography;
using System.Text;

namespace HousingRegisterApi.V1.Infrastructure
{
    public class SHA256Helper : ISHA256Helper
    {
        public string Generate(string input)
        {
            StringBuilder Sb = new StringBuilder();

            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                Byte[] result = hash.ComputeHash(enc.GetBytes(input));

                foreach (Byte b in result)
                    Sb.Append(b.ToString("x2"));
            }

            return Sb.ToString();
        }
    }
}
