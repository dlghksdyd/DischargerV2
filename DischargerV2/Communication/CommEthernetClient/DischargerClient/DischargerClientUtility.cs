using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace Ethernet.Client.Discharger
{
    public static class DischargerClientUtility
    {
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
    }
}
