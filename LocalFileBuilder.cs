using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;


namespace AudioStreamer_TestClient
{
    internal class LocalFileBuilder
    {

        public static void Run(string path)
        {
            List<string> songDirectory = Directory.GetFiles(path).ToList();
            List<byte> outputBuffer = new();
            FileStream f = File.Open("built_file.wav", FileMode.OpenOrCreate);
            foreach (string pathToFile in songDirectory)
            {
                int packetID = songDirectory.IndexOf(pathToFile);

                using (FileStream fsSource = new FileStream(pathToFile, FileMode.Open, FileAccess.Read))
                {
                    int numBytesToRead = (int)fsSource.Length;
                    int numBytesRead = 0;
                    byte[] binContent = new byte[fsSource.Length];


                    while (numBytesToRead > 0)
                    {
                        int n = fsSource.Read(binContent, numBytesRead, numBytesToRead);
                        numBytesRead += n;
                        numBytesToRead -= n;
                    }
                    numBytesToRead = binContent.Length;
                    foreach (byte bufferElement in binContent)
                    {
                        f.WriteByte(bufferElement);
                    }
                }
            }           
            
            f.Close();
            GC.Collect();
        }
    }
}
