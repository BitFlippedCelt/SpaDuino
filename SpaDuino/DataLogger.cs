using System;
using System.IO;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

namespace SpaDuino
{
    public class DataLogger
    {
        public DataLogger(String filename)
        {
            String dirPath = Path.GetDirectoryName(filename);
            if (Utility.CheckForDir(Path.GetDirectoryName(filename), true))
            {
                _logFilename = filename;
            }
            else
            {
                Debug.Print("Data logging directory not found!");
            }
        }

        public void WriteLine(String line)
        {
            if (_logFilename != null)
            {
                try
                {
                    using (var fs = new FileStream(_logFilename, FileMode.OpenOrCreate | FileMode.Append, FileAccess.Write, FileShare.Read, 512))
                    using (StreamWriter sOut = new StreamWriter(fs))
                    {
                        sOut.WriteLine(DateTime.Now + " - " + line);
                        Debug.Print(DateTime.Now + " - " + line);
                    }
                }
                catch (IOException ex)
                {
                    Debug.Print("WriteLine failed. " + ex.Message);
                }
            }
        }

        public void WriteBreak()
        {
            WriteLine("***************************************************************");
        }

        private String _logFilename = null;
    }
}
