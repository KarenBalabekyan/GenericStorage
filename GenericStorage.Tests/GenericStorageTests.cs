using Dynamix.Net;
using Dynamix.Net.Helpers;
using FluentAssertions;
using GenericStorageTests.Stubs;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Xunit;

namespace GenericStorageTests
{
    public class GenericStorageTests
    {
        [Fact(DisplayName = "GenericStorage should be initializable")]
        public void GenericStorage_Should_Be_Initializable()
        {
            var target = new GenericStorage();
            target.Should().NotBeNull();
        }

        [Fact(DisplayName = "GenericStorage should implement IDisposable")]
        public void GenericStorage_Should_Implement_IDisposable()
        {
            using (var target = new GenericStorage())
            {
                target.Should().NotBeNull();
            }
        }

        [Fact(DisplayName = "GenericStorage.Store() should persist simple string")]
        public void GenericStorage_Store_Should_Persist_Simple_String()
        {
            // arrange
            var key = Guid.NewGuid().ToString();
            var expectedValue = "I-AM-GROOT";
            var storage = new GenericStorage();

            // act
            storage.Store(key, expectedValue);
            storage.Persist();

            // assert
            var target = storage.Get(key);
            target.Should().BeOfType<string>();
            target.Should().Be(expectedValue);
        }

        [Fact(DisplayName = "GenericStorage.Store() should persist simple DateTime as struct")]
        public void GenericStorage_Store_Should_Persist_Simple_DateTime_As_Struct()
        {
            // arrange
            var key = Guid.NewGuid().ToString();
            var expectedValue = DateTime.Now;
            var storage = new GenericStorage();

            // act
            storage.Store(key, expectedValue);
            storage.Persist();

            // assert
            var target = storage.Get<DateTime>(key);
            target.Should().Be(expectedValue);
        }

        [Fact(DisplayName = "GenericStorage.Store() should persist and retrieve correct type")]
        public void GenericStorage_Store_Should_Persist_And_Retrieve_Correct_Type()
        {
            // arrange
            var key = Guid.NewGuid().ToString();
            var value = (double) 42.4m;
            var storage = new GenericStorage();

            // act
            storage.Store(key, value);
            storage.Persist();

            // assert
            var target = storage.Get<double>(key);
            target.Should().Be(value);
        }

        [Fact(DisplayName = "GenericStorage.Store() should persist multiple values")]
        public void GenericStorage_Store_Should_Persist_Multiple_Values()
        {
            // arrange - create multiple values, of different types
            var key1 = Guid.NewGuid().ToString();
            var key2 = Guid.NewGuid().ToString();
            var key3 = Guid.NewGuid().ToString();
            var value1 = "It was the best of times, it was the worst of times.";
            var value2 = DateTime.Now;
            var value3 = Int32.MaxValue;
            var storage = new GenericStorage();

            // act
            storage.Store(key1, value1);
            storage.Store(key2, value2);
            storage.Store(key3, value3);
            storage.Persist();

            // assert
            var target1 = storage.Get<string>(key1);
            var target2 = storage.Get<DateTime>(key2);
            var target3 = storage.Get<int>(key3);

            target1.Should().Be(value1);
            target2.Should().Be(value2);
            target3.Should().Be(value3);
        }

        [Fact(DisplayName = "GenericStorage.Store() should overwrite existing key")]
        public void GenericStorage_Store_Should_Overwrite_Existing_Key()
        {
            // arrange
            const string key = "I-Will-Be-Used-Twice";
            var storage = new GenericStorage();
            var originalValue = new Joke {Id = 1, Text = "Yo mammo is so fat..."};
            storage.Store(key, originalValue);
            storage.Persist();
            var expectedValue = new Joke
            {
                Id = 2,
                Text = "... she left the house in high heels and when she came back she had on flip flops"
            };

            // act - overwrite the existing value
            storage.Store(key, expectedValue);
            storage.Persist();
            var target = storage.Get<Joke>(key);

            // assert - last stored value should be the truth
            target.Should().NotBeNull();
            target.Should().BeEquivalentTo(expectedValue);
        }

        [Fact(DisplayName = "GenericStorage.Clear() should clear all in-memory content")]
        public void GenericStorage_Clear_Should_Clear_All_Content()
        {
            // arrange - make sure something is stored in the GenericStorage
            var storage = new GenericStorage();
            var key = Guid.NewGuid().ToString();
            var value = Guid.NewGuid();
            storage.Store(key, value);
            storage.Persist();

            // act - clear the store
            storage.Clear();

            // assert - open the file here and make sure the contents are empty
            storage.Count.Should().Be(0);
        }

        [Fact(DisplayName = "GenericStorage.Persist() should leave previous entries intact")]
        public void GenericStorage_Persist_Should_Leave_Previous_Entries_Intact()
        {
            // arrange - add an arbitrary item and persist
            var storage = new GenericStorage();
            var key1 = Guid.NewGuid().ToString();
            var value1 = "Some kind of monster";
            storage.Store(key1, value1);
            storage.Persist();

            // act - add a second item
            var key2 = Guid.NewGuid().ToString();
            var value2 = "Some kind of monster";
            storage.Store(key2, value2);
            storage.Persist();

            // assert - prove that both items remain intact
            var target1 = storage.Get<string>(key1);
            var target2 = storage.Get<string>(key2);
            target1.Should().Be(value1);
            target2.Should().Be(value2);
        }

        [Fact(DisplayName = "GenericStorage should remain intact between multiple instances")]
        public void GenericStorage_Should_Remain_Intact_Between_Multiple_Instances()
        {
            // arrange - add an arbitrary item and persist
            var storage1 = new GenericStorage();
            var key1 = Guid.NewGuid().ToString();
            var value1 = "Megan Ryan";
            storage1.Store(key1, value1);
            storage1.Persist();

            // act - create a second instance of the GenericStorage,
            // and persist some more stuff
            var storage2 = new GenericStorage();
            var key2 = Guid.NewGuid().ToString();
            var value2 = "George Morgan";
            storage2.Store(key2, value2);
            storage2.Persist();

            // assert - prove that entries from both instances still exist
            var storage3 = new GenericStorage();
            var target1 = storage3.Get<string>(key1);
            var target2 = storage3.Get<string>(key2);
            target1.Should().Be(value1);
            target2.Should().Be(value2);
        }

        [Fact(DisplayName = "GenericStorage should support unicode")]
        public void GenericStorage_Store_Should_Support_Unicode()
        {
            // arrange
            var key = Guid.NewGuid().ToString();
            const string expectedValue = "Dynamix's Special Characters: ~!@#$%^&*()œōøęs";
            var storage = new GenericStorage();

            // act
            storage.Store(key, expectedValue);
            storage.Persist();

            // assert
            var target = storage.Get(key);
            target.Should().BeOfType<string>();
            target.Should().Be(expectedValue);
        }

        [Fact(DisplayName = "GenericStorage should perform decently with large collections")]
        public void GenericStorage_Should_Perform_Decently_With_Large_Collections()
        {
            // arrange - create a larger collection (100K records)
            var stopwatch = Stopwatch.StartNew();
            var storage = new GenericStorage();
            for (var i = 0; i < 100000; i++)
                storage.Store(Guid.NewGuid().ToString(), i);

            storage.Persist();

            // act - create new instance (e.g. load the larger collection from disk)
            var target = new GenericStorage();
            target.Clear();
            stopwatch.Stop();

            // cleanup - delete the (large!) persisted file
            target.Destroy();

            // assert - make sure the entire operation is done in < 1sec. (psychological boundry, if you will)
            stopwatch.ElapsedMilliseconds.Should().BeLessOrEqualTo(1000);
        }

        [Fact(DisplayName = "GenericStorage.Query() should cast to a collection")]
        public void GenericStorage_Query_Should_Cast_Response_To_Collection()
        {
            // arrange - persist a collection to storage
            var collection = CarFactory.Create();
            var storage = new GenericStorage();
            var key = Guid.NewGuid().ToString();
            var enumerable = collection as Car[] ?? collection.ToArray();
            var expectedAmount = enumerable.Length;
            storage.Store(key, enumerable);

            // act - fetch directly as a collection, passing along a where-clause
            var target = storage.Query<Car>(key);

            // assert
            var cars = target as Car[] ?? target.ToArray();
            cars.Should().NotBeNull();
            cars.Length.Should().Be(expectedAmount);
        }

        [Fact(DisplayName = "GenericStorage.Query() should respect a provided predicate")]
        public void GenericStorage_Query_Should_Respect_Provided_Predicate()
        {
            // arrange - persist a collection to storage
            var collection = CarFactory.Create();
            var storage = new GenericStorage();
            var key = Guid.NewGuid().ToString();
            var expected_brand = "Audi";
            var enumerable = collection as Car[] ?? collection.ToArray();
            var expectedAmount = enumerable.Count(c => c.Brand == expected_brand);
            storage.Store(key, enumerable);

            // act - fetch directly as a collection, passing along a where-clause
            var target = storage.Query<Car>(key, c => c.Brand == expected_brand);

            // assert
            var cars = target as Car[] ?? target.ToArray();
            cars.Should().NotBeNull();
            cars.Length.Should().Be(expectedAmount);
        }

        [Fact(DisplayName = "GenericStorage.Destroy() should delete file on disk")]
        public void GenericStorage_Destroy_Should_Delete_File_On_Disk()
        {
            // arrange
            var randomFilename = Guid.NewGuid().ToString("N");
            var filepath = FileHelpers.GetGenericStoreFilePath(randomFilename);
            var config = new GenericStorageConfiguration()
            {
                FileName = randomFilename
            };

            var storage = new GenericStorage(config);
            storage.Persist();

            // act 
            storage.Destroy();

            // assert
            File.Exists(filepath).Should().BeFalse();
        }
    }
}