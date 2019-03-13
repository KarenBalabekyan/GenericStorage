using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Dynamix.Net.Extensions
{
    public static class ByteArrayExtension
    {
        /// <summary>
        /// Convert object to byte array
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] ToByteArray(this object obj)
        {
            if (obj == null)
            {
                return null;
            }

            var binaryFormatter = new BinaryFormatter();

            using (var memoryStream = new MemoryStream())
            {
                binaryFormatter.Serialize(memoryStream, obj);
                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// Convert byte array to object 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="byteArray"></param>
        /// <returns></returns>
        public static T FromByteArray<T>(this byte[] byteArray)
        {
            if (byteArray == null)
            {
                return default(T);
            }

            var binaryFormatter = new BinaryFormatter();

            using (var memoryStream = new MemoryStream(byteArray))
            {
                return (T) binaryFormatter.Deserialize(memoryStream);
            }
        }
    }
}
