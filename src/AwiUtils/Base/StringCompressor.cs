using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace AwiUtils
{
    public static class StringCompressor
    {
        /// <summary> Compresses the string to a byte[]. </summary>
        public static byte[] CompressToBytes(string text)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            var memoryStream = new MemoryStream();
            using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
            {
                gZipStream.Write(buffer, 0, buffer.Length);
            }

            memoryStream.Position = 0;

            var compressedData = new byte[memoryStream.Length];
            memoryStream.Read(compressedData, 0, compressedData.Length);

            var gZipBuffer = new byte[compressedData.Length + 4];
            Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);
            return gZipBuffer;
        }

        public static string Compress(string text)
        {
            var gZipBuffer = CompressToBytes(text);
            return Convert.ToBase64String(gZipBuffer);
        }

        public static void CompressFile(string fileName, bool shallRemoveOriginal)
        {
            using (FileStream originalFileStream = File.Open(fileName, FileMode.Open))
            {
                using FileStream compressedFileStream = File.Create(fileName + ".gz");
                using var compressor = new GZipStream(compressedFileStream, CompressionMode.Compress);
                originalFileStream.CopyTo(compressor);
            }
            if (shallRemoveOriginal)
                File.Delete(fileName);
        }

        public static void DecompressFile(string fileName, bool shallRemoveGz)
        {
            string ofilename = fileName.Substring(0, fileName.Length - 3); // Remove .gz at end of name.
            using (FileStream compressedFileStream = File.Open(fileName, FileMode.Open))
            {
                using FileStream outputFileStream = File.Create(ofilename);
                using var decompressor = new GZipStream(compressedFileStream, CompressionMode.Decompress);
                decompressor.CopyTo(outputFileStream);
            }
            if (shallRemoveGz)
                File.Delete(fileName);
        }

        /// <summary> Decompresses the string. </summary>
        public static string Decompress(string compressedText)
        {
            byte[] gZipBuffer = Convert.FromBase64String(compressedText);
            return DecompressFromBytes(gZipBuffer);
        }

        public static string DecompressFromBytes(byte[] gZipBuffer)
        {
            using (var memoryStream = new MemoryStream())
            {
                int dataLength = BitConverter.ToInt32(gZipBuffer, 0);
                memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);

                var buffer = new byte[dataLength];

                memoryStream.Position = 0;
                using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    gZipStream.Read(buffer, 0, buffer.Length);
                }

                return Encoding.UTF8.GetString(buffer);
            }
        }
    }
}
