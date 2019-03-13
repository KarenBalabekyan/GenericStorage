using System;
using System.IO;
using System.Security.Cryptography;

namespace Dynamix.Net.Helpers
{
    /// <summary>
    /// Helpers for encryption and decryption.
    /// </summary>
    internal class CryptographyHelpers
    {
        internal static byte[] Decrypt(string password, string salt, byte[] encryptedValue)
        {
            using (var aes = Aes.Create())
            {
                if (aes == null)
                {
                    throw new InvalidOperationException(nameof(aes));
                }

                var keys = GetAesKeyAndIv(password, salt, aes);
                aes.Key = keys.Item1;
                aes.IV = keys.Item2;

                // Creating a decrytor to perform the stream transform.
                var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                // Creating the streams used for encryption.
                using (var memoryStream = new MemoryStream(encryptedValue))
                {
                    using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        cryptoStream.CopyTo(memoryStream);
                        {
                            return memoryStream.ToArray();
                        }
                    }
                }
            }
        }

        internal static byte[] Encrypt(string password, string salt, byte[] plainText)
        {
            using (var aes = Aes.Create())
            {
                if (aes == null)
                {
                    throw new InvalidOperationException(nameof(aes));
                }

                var keys = GetAesKeyAndIv(password, salt, aes);
                aes.Key = keys.Item1;
                aes.IV = keys.Item2;

                // Creating a decrytor to perform the stream transform.
                var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                // Creating the streams used for encryption.
                using (var memoryStream = new MemoryStream())
                {
                    using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (var writer = new StreamWriter(cryptoStream))
                        {
                            //write all data to the stream.
                            writer.Write(plainText);
                        }

                        return memoryStream.ToArray();
                    }
                }
            }
        }

        #region private methods

        private static byte[] ToByteArray(string input)
        {
            var validBase64 = input.Replace('-', '+');
            return Convert.FromBase64String(validBase64);
        }

        private static string ToString(byte[] input)
        {
            return Convert.ToBase64String(input);
        }

        private static Tuple<byte[], byte[]> GetAesKeyAndIv(string password, string salt,
            SymmetricAlgorithm symmetricAlgorithm)
        {
            const int bits = 8;
            var deriveBytes = new Rfc2898DeriveBytes(password, ToByteArray(salt));
            var key = deriveBytes.GetBytes(symmetricAlgorithm.KeySize / bits);
            var iv = deriveBytes.GetBytes(symmetricAlgorithm.BlockSize / bits);

            return new Tuple<byte[], byte[]>(key, iv);
        }

        #endregion
    }
}