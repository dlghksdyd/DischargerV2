using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace Utility.Common
{
    public static class CommonUtility
    {
        public static BitmapSource GetBitmapSource(this Bitmap bitmap)
        {
            if (bitmap == null)
                throw new ArgumentNullException("bitmap");

            var rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            var bitmapData = bitmap.LockBits(
                rect,
                System.Drawing.Imaging.ImageLockMode.ReadWrite,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            try
            {
                var size = (rect.Width * rect.Height) * 4;

                return BitmapSource.Create(
                    bitmap.Width,
                    bitmap.Height,
                    bitmap.HorizontalResolution,
                    bitmap.VerticalResolution,
                    PixelFormats.Bgra32,
                    null,
                    bitmapData.Scan0,
                    size,
                    bitmapData.Stride);
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }
        }


        public static string FromCharArrayToString(this char[] charArray)
        {
            return new string(charArray).Replace("\0", String.Empty);
        }

        public static T[] ExtractSubArray<T>(this T[] data, int startIdx, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, startIdx, result, 0, length);
            return result;
        }

        public static byte[] AppendByteArray(this byte[] originByte, byte[] appendByte)
        {
            byte[] bytes = new byte[originByte.Length + appendByte.Length];
            Buffer.BlockCopy(originByte, 0, bytes, 0, originByte.Length);
            Buffer.BlockCopy(appendByte, 0, bytes, originByte.Length, appendByte.Length);
            return bytes;
        }

        public static T[] ResizeArray<T>(this T[] oldArray, int length)
        {
            T[] newArray = new T[oldArray.Length];

            Array.Copy(oldArray, newArray, oldArray.Length);

            Array.Resize(ref newArray, length);

            return newArray;
        }

        public static byte[] FromPacketToByteArray<T>(this T packet)
        {
            int size = Marshal.SizeOf(typeof(T));
            byte[] buffer = new byte[size];
            IntPtr pBuffer = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(packet, pBuffer, false);
            Marshal.Copy(pBuffer, buffer, 0, size);
            Marshal.FreeHGlobal(pBuffer);
            return buffer;
        }

        public static T FromByteArrayToPacket<T>(this byte[] data)
        {
            T structure;
            int size = Marshal.SizeOf(typeof(T));
            IntPtr pBuffer = Marshal.AllocHGlobal(size);
            Marshal.Copy(data, 0, pBuffer, size);
            structure = (T)Marshal.PtrToStructure(pBuffer, typeof(T));
            Marshal.FreeHGlobal(pBuffer);
            return structure;
        }

        public static string FromByteArrayToString(this byte[] byteArray, Encoding encoding)
        {
            return encoding.GetString(byteArray).Replace("\0", String.Empty);
        }

        public static byte[] FromStringToByteArray(this string stringData, Encoding encoding)
        {
            return encoding.GetBytes(stringData);
        }

        public static byte[] FromStringToByteArray(this string stringData, Encoding encoding, int length)
        {
            byte[] bytes = new byte[length];
            Array.Copy(stringData.FromStringToByteArray(encoding), bytes, stringData.Length);

            return bytes;
        }

        public static string GetRawDataString(this byte[] _buffer)
        {
            string rawData = "";
            for (int i = 0; i < _buffer.Length; i++)
            {
                if (((i % 16) == 0) && (i != 0))
                {
                    rawData += "\n";
                }
                rawData += string.Format("{0:X2}", _buffer[i]) + " ";
            }

            return rawData;
        }
    }
}
