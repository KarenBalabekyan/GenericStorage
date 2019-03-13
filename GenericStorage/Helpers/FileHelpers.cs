using System;
using System.IO;

namespace Dynamix.Net.Helpers
{
    internal static class FileHelpers
    {
        internal static string GetGenericStoreFilePath(string filename)
        {
            return Path.Combine(AppContext.BaseDirectory, filename);
        }
    }
}