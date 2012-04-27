using System;
using Microsoft.SPOT;
using System.IO;

namespace SpaDuino
{
    public static class Utility
    {
        public static bool CheckForDir(String path, bool createIfNotFound = false)
        {
            bool result = false; // Assume it exists

            String dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
            {
                if (createIfNotFound)
                {
                    try
                    {
                        Directory.CreateDirectory(dir);
                        result = true;
                    }
                    catch (IOException ex)
                    {
                        Debug.Print(ex.Message);
                    }
                }
                else
                    result = true;
            }

            return result;
        }
    }
}
