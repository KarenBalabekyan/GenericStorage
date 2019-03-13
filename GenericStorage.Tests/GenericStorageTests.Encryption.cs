using Dynamix.Net;
using Dynamix.Net.Helpers;
using FluentAssertions;
using System;
using System.Text;
using Xunit;

namespace GenericStorageTests
{
    public class EncryptionTests
    {
        [Fact(DisplayName = "Helpers.Decrypt() should decode an encrypted string")]
        public void Decrypt_Should_Decode_An_Encrypted_String()
        {
            // arrange
            var key = Guid.NewGuid().ToString();
            var salt = Guid.NewGuid().ToString();
            var originalValue = Encoding.UTF8.GetBytes("lorem ipsum dom dolor sit amet");
            var encryptedValue = CryptographyHelpers.Encrypt(key, salt, originalValue);

            // act
            var target = CryptographyHelpers.Decrypt(key, salt, encryptedValue);

            // assert
            target.Should().NotBeNullOrEmpty();
        }

        [Fact(DisplayName = "Helpers.Decrypt() should decode an encrypted string with special characters")]
        public void Decrypt_Should_Decode_An_Encrypted_String_With_Special_Characters()
        {
            // arrange
            var key = Guid.NewGuid().ToString("N");
            var salt = Guid.NewGuid().ToString("N");
            var originalValue = Encoding.UTF8.GetBytes("Søm€ unicode s-tring+");
            var encryptedValue = CryptographyHelpers.Encrypt(key, salt, originalValue);

            // act
            var target = CryptographyHelpers.Decrypt(key, salt, encryptedValue);

            // assert
            target.Should().NotBeNullOrEmpty();
            target.Should().BeEquivalentTo(originalValue);
        }

        [Fact(DisplayName = "Helpers.Encrypt() should encrypt a string")]
        public void Encryption_Should_Encrypt_String()
        {
            // arrange
            var key = Guid.NewGuid().ToString();
            var salt = Guid.NewGuid().ToString();
            var bytes = Encoding.UTF8.GetBytes("lorem ipsum dom dolor sit amet");

            // act
            var target = CryptographyHelpers.Encrypt(key, salt, bytes);

            // assert
            target.Should().NotBeNullOrEmpty();
            target.Should().NotBeEquivalentTo(bytes);
        }

        [Fact(DisplayName = "GenericStorage.Store() [Encrypted] should persist and retrieve correct type")]
        public void GenericStorage_Store_Encrypted_Should_Persist_And_Retrieve_Correct_Type()
        {
            // arrange
            var key = Guid.NewGuid().ToString();
            var value = (double) 42.4m;
            var password = Guid.NewGuid().ToString();
            var storage = new GenericStorage(EncryptedConfiguration(), password);

            // act
            storage.Store(key, value);
            storage.Persist();

            // assert
            var target = storage.Get<double>(key);
            target.Should().Be(value);
        }

        private GenericStorageConfiguration EncryptedConfiguration()
        {
            return new GenericStorageConfiguration()
            {
                EnableEncryption = true,
                EncryptionSalt = "SALT-N-PEPPA"
            };
        }
    }
}