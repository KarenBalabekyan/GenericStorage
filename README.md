# GenericStorage for .NET
Generic storage for .net/core applications

## What is GenericStorage?

GenericStorage is a simple storage that solves a common problem pragmatically - storing and accessing objects quickly in a .NET app. 
It is designed with a specific vision in mind: provide a **simple solution for persisting objects**, even between sessions, 
that is unobtrusive and requires zero configuration.

## Getting Started

First, you might want to dive into the [examples in this document](#examples). 
Once you're game, simply add it to your project [through NuGet](https://www.nuget.org/packages/GenericStorage).

NuGet Package Manager: 

    $ Install-Package GenericStorage

NuGet CLI:

    $ nuget install GenericStorage

## If you've got an issue...

... [grab a tissue](https://www.youtube.com/watch?v=UmnN3eVMWgA). No, seriously; don't hesitate to [post an issue](https://github.com/hanssens/localstorage-for-dotnet/issues), or send me a message at [@jhanssens](https://twitter.com/jhanssens).

GenericStorage is Copyright &copy; 2019 [Dynamix](https://dynamix.am) under the [MIT license](LICENSE.txt).

## Documentation

### Examples

The tests are some pretty good examples to get familiar with the lib. But here is the short summary on the GenericStorage API.

#### Basic CRUD operations

##### Initialize

	// initialize, with default settings
	var storage = new GenericStorage();

	// ... or initialize with a custom configuration 
	var config = new GenericStorageConfiguration() { 
		// see the section "Configuration" further on
	};
    
	var storage = new GenericStorage(config);

##### Store()

	// store any object, or collection providing only a 'key'
	var key = "whatever";
	var value = "...";

	storage.Store(key, value);

##### Get()

	// fetch any object - as object
	storage.Get(key);

	// if you know the type, simply provide it as a generic parameter
	storage.Get<Car>(key);

##### Query()

	// fetch a strong-typed collection
	storage.Query<Animal>(key);

	// you can also provide a strong-typed where-clause in one go
	storage.Query<Car>(key, x => x.Name == "Cow");

#### Other operations

##### Count
Returns the amount of items currently in the GenericStorage container.

##### Clear()
Clears the in-memory contents of the GenericStorage, but leaves any persisted state on disk intact.

##### Destroy()
Deletes the persisted file on disk, if it exists, but keeps the in-memory data intact.

##### Load()
Loads the persisted state from disk into memory, overriding the current memory instance. If the file does not exist, it simply does nothing.  
By default, this is done automatically at initialization and can be overriden by disabling `AutoLoad` in the configuration.

##### Persist()
Persists the in-memory store to disk.  
Note that by default, this is also done automatically when the GenericStorage disposes properly. 
This can be changed by disabling `AutoSave` in the configuration.

### Configuration

Here is a sample configuration with all configurable members (and their default values assigned):

* **AutoLoad** (bool)  
  Indicates if GenericStorage should automatically load previously persisted state from disk, when it is initialized (defaults to true).
  
* **AutoSave** (bool)  
  Indicates if GenericStorage should automatically persist the latest state to disk, on dispose (defaults to true).
  
* **Filename** (string)  
  Filename for the persisted state on disk (defaults to ".genericstorage").

* **EnableEncryption**  

Security is an important feature. GenericStorage has support for encrypting the data, both in-memory as well as persisted on disk. 

You only need to define a custom configuration indication that encryption should be enabled:

	// setup a configuration with encryption enabled (defaults to 'false')
	// note that adding EncryptionSalt is optional, but recommended
	var config = new GenericStorageConfiguration() {
		EnableEncryption = true,
		EncryptionSalt = "(optional) add your own random salt string"
	};

	// initialize GenericStorage with a password of your choice
	var encryptedStorage = new GenericStorage(config, "password");

All write operations are first encrypted with AES, before they are persisted in-memory. 
In case of disk persistance, the encrypted value is respected. 
Although enabling encryption increases security, it does add a slight overhead to the Get/Store operations, in terms of performance.

By design, only the values are encrypted and not the keys. 
