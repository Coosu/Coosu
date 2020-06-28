using System;
using System.IO;

namespace OSharp.Api.V1.Internal
{
    internal static class OsrBinaryUtility
    {
        /// <summary>
        /// Write binary String.
        /// </summary>
        /// <param name="binWriter">The main BinaryWriter.</param>
        /// <param name="content">A string.</param>
        public static void WriteString(this BinaryWriter binWriter, string content)
        {
            binWriter.Write((byte)0x0B);
            binWriter.Write(content);
        }

        /// <summary>
        /// Write binary Byte.
        /// </summary>
        /// <param name="binWriter">The main BinaryWriter.</param>
        /// <param name="content">A string.</param>
        public static void WriteByte(this BinaryWriter binWriter, string content)
        {
            binWriter.Write(byte.Parse(content));
        }

        /// <summary>
        /// Write binary Int16 (short).
        /// </summary>
        /// <param name="binWriter">The main BinaryWriter.</param>
        /// <param name="content">A string.</param>
        public static void WriteShort(this BinaryWriter binWriter, string content)
        {
            binWriter.Write(Convert.ToUInt16(content));
        }

        /// <summary>
        /// Write binary Int32 (int).
        /// </summary>
        /// <param name="binWriter">The main BinaryWriter.</param>
        /// <param name="content">A string.</param>
        public static void WriteInteger(this BinaryWriter binWriter, string content)
        {
            binWriter.Write(Convert.ToUInt32(content));
        }

        /// <summary>
        /// Write binary database timestamp.
        /// </summary>
        /// <param name="binWriter">The main BinaryWriter.</param>
        /// <param name="dateString">A date parsed as a string.</param>
        public static void WriteDate(this BinaryWriter binWriter, string dateString)
        {
            // Weird string and timestamp manipulation. I have no clue how it works.
            // The credits for this one goes to omkelderman(https://github.com/omkelderman/osu-replay-downloader).

            var constant = 429.4967296;
            DateTime date = DateTime.Parse(dateString.Replace(' ', 'T') + "+08:00");

            var temp1 = (GetUnixTimestamp(date) / 1000) + 62135596800;
            var temp2 = temp1 / constant;
            var high = Math.Round(temp2);
            var low = (temp2 - high) * constant * 10000000;
            byte[] toBytes2 = BitConverter.GetBytes(Convert.ToInt32(high));
            byte[] toBytes1 = BitConverter.GetBytes(Convert.ToInt32(low));


            binWriter.Write(toBytes1, 0, 4);
            binWriter.Write(toBytes2, 0, 4);
        }

        /// <summary>
        /// Calculate MD5Hash
        /// </summary>
        /// <param name="input">Decoded MD5 string.</param>
        /// <returns></returns>
        public static string ComputeMd5Hash(string input)
        {
            var md5 = System.Security.Cryptography.MD5.Create();
            var sb = new System.Text.StringBuilder();

            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }

            return sb.ToString().ToLower();
        }

        /// <summary>
        /// Get unix timestamp.
        /// </summary>
        /// <param name="date">A date from DateTime.</param>
        /// <returns></returns>
        private static long GetUnixTimestamp(DateTime date)
        {
            var st = new DateTime(1970, 1, 1);
            TimeSpan t = (date.ToUniversalTime() - st);
            long retVal = (long)(t.TotalMilliseconds);
            return retVal;
        }
    }
}
