using System;
using System.IO;
using Microsoft.SPOT;

namespace SpaDuino
{
    public class DataLogger
    {
        public DataLogger(String filename)
        {
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
                    using (StreamWriter sOut = new StreamWriter(_logFilename))
                    {
                        sOut.WriteLine(line);
                    }
                }
                catch (IOException ex)
                {
                    Debug.Print("WriteLine failed. " + ex.Message);
                }
            }
        }

        private String _logFilename = null;
    }
}
