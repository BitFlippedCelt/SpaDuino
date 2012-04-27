using System;
using System.IO;
using Microsoft.SPOT;

namespace SpaDuino
{
    public class SpaConfig
    {
        public static void WriteFile(String filename)
        {
            if (!Directory.Exists(Path.GetDirectoryName(filename)))
                try 
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(filename));
                }
                catch (IOException ex) 
                {
                    Debug.Print(ex.Message);
                }
        }
    }
}
