using Dynamix.Net;
using Dynamix.Net.Helpers;
using FluentAssertions;
using System;
using System.IO;
using Xunit;

namespace GenericStorageTests
{
    public class GenericStorageConfigurationTests
    {
        [Fact(DisplayName = "GenericStorage should not be initializable with null for configuration")]
        public void GenericStorage_Should_Not_Be_Initializable_With_Argument_Null()
        {
            Assert.Throws<ArgumentNullException>(() => { new GenericStorage(null); });
        }

        [Fact(DisplayName = "GenericStorageConfiguration should respect custom filename")]
        public void GenericStorageConfiguration_Should_Respect_Custom_Filename()
        {
            // arrange - configure GenericStorage to use a custom filename
            var randomFilename = Guid.NewGuid().ToString("N");
            var config = new GenericStorageConfiguration
            {
                FileName = randomFilename
            };

            // act - store the container
            var storage = new GenericStorage(config);
            storage.Persist();
            var target = FileHelpers.GetGenericStoreFilePath(randomFilename);

            // assert
            File.Exists(target).Should().BeTrue();

            // cleanup
            storage.Destroy();
        }

        [Fact(DisplayName = "GenericStorageConfiguration.AutoLoad should load persisted state when enabled",
            Skip = "TODO")]
        public void GenericStorageConfiguration_AutoLoad_Should_Load_Previous_State_OnLoad()
        {
            throw new NotImplementedException();
        }

        [Fact(DisplayName = "GenericStorageConfiguration.AutoLoad should skip loading persisted state when disabled",
            Skip = "TODO")]
        public void GenericStorageConfiguration_AutoLoad_Should_Skip_Loading_Previous_State_OnLoad()
        {
            throw new NotImplementedException();
        }

        [Fact(DisplayName = "GenericStorageConfiguration.AutoSave should save changes to disk when enabled",
            Skip = "TODO")]
        public void GenericStorageConfiguration_AutoSave_Should_Persist_When_Enabled()
        {
            throw new NotImplementedException();
        }

        [Fact(DisplayName = "GenericStorageConfiguration.AutoSave should not save changes to disk when disabled",
            Skip = "TODO")]
        public void GenericStorageConfiguration_AutoSave_Should_Not_Persist_When_Disabled()
        {
            throw new NotImplementedException();
        }
    }
}