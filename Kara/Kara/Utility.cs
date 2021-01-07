using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kara
{
    public static class Utility
    {
        public static bool Contians(this int[] arr, int input)
        {
            for (int i = 0; i < arr.Length - 1; i++)
            {
                if (arr[i] == input)
                    return true;
            }

            return false;
        }

        public static string ConvertToBase64(this Stream stream)
        {
            byte[] bytes;
            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                bytes = memoryStream.ToArray();
            }

            string base64 = Convert.ToBase64String(bytes);
            return base64;
        }

    }
}
