using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Media;
using System.Drawing;
using System.Windows.Media.Imaging;

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

        public static char[] FromStringToCharArray(this string stringData)
        {
            char[] chars = new char[stringData.Length];
            Array.Copy(stringData.ToCharArray(), chars, stringData.Length);
            return chars;
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

        public static int GetDataLength(this object instance)
        {
            int totalSize = 0;

            Type type = instance.GetType();

            FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var field in fields)
            {
                object fieldValue = field.GetValue(instance);

                Type fieldType = field.FieldType;

                if (typeof(Array).IsAssignableFrom(fieldType)) // 배열인 경우 처리
                {
                    Array array = fieldValue as Array;

                    // 배열의 요소 타입
                    Type elementType = fieldType.GetElementType();

                    if (elementType.IsPrimitive || elementType.IsEnum)
                    {
                        // 기본형 및 enum은 요소 하나당 Marshal.SizeOf(elementType)
                        int elementSize = Marshal.SizeOf(elementType);
                        totalSize += array.Length * elementSize;
                    }
                    else
                    {
                        // 복합 타입일 경우, 각 요소에 대해 재귀적으로 계산
                        foreach (var elem in array)
                        {
                            totalSize += GetDataLength(elem);
                        }
                    }
                }
                else if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    // List<T> 타입인 경우, fieldValue를 IList로 캐스팅
                    IList list = fieldValue as IList;
                    if (list == null) continue;

                    // List의 요소 타입
                    Type elementType = fieldType.GetGenericArguments()[0];

                    if (elementType.IsPrimitive || elementType.IsEnum)
                    {
                        // 기본형 및 enum은 요소 하나당 Marshal.SizeOf(elementType)
                        int elementSize = Marshal.SizeOf(elementType);
                        totalSize += list.Count * elementSize;
                    }
                    else
                    {
                        // 복합 타입일 경우 각 요소에 대해 재귀적으로 계산
                        foreach (object elem in list)
                        {
                            if (elem != null)
                            {
                                totalSize += GetDataLength(elem);
                            }
                        }
                    }
                }
                else if (fieldType == typeof(string))
                {
                    string str = fieldValue as string;
                    totalSize += (str.Length * 1); // 문자열의 경우 길이 × 1 (UTF-8)
                }
                else if (fieldType.IsPrimitive) // 기본형
                {
                    totalSize += Marshal.SizeOf(fieldType);
                }
                else if (fieldType.IsEnum) // enum
                {
                    Type underlyingType = Enum.GetUnderlyingType(fieldType);

                    totalSize += Marshal.SizeOf(underlyingType);
                }
                else if (fieldType.IsValueType) // 값 타입
                {
                    totalSize += Marshal.SizeOf(fieldValue);
                }
                else // 참조형
                {
                    totalSize += GetDataLength(fieldValue); // 재귀
                }
            }

            return totalSize;
        }

        /// <summary>
        /// 객체의 모든 필드 값을 직렬화하여 byte[]로 반환합니다.
        /// </summary>
        public static byte[] SerializeObjectToByteArray(this object obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(ms))
                {
                    WriteValue(obj, writer);
                }
                return ms.ToArray();
            }
        }

        /// <summary>
        /// 재귀적으로 객체 또는 값을 BinaryWriter에 씁니다.
        /// </summary>
        private static void WriteValue(object value, BinaryWriter writer)
        {
            Type type = value.GetType();

            // 기본형(primitive) 또는 enum 타입이면
            if (type.IsPrimitive || type.IsEnum || type == typeof(decimal))
            {
                WritePrimitive(value, writer);
            }
            // 문자열인 경우
            else if (type == typeof(string))
            {
                string s = (string)value;
                byte[] stringBytes = Encoding.UTF8.GetBytes(s);
                writer.Write(stringBytes.Length);  // 문자열 길이 기록
                writer.Write(stringBytes);
            }
            // IEnumerable (가변 길이 배열, List 등) – 문자열은 이미 위에서 처리됨
            else if (typeof(IEnumerable).IsAssignableFrom(type))
            {
                IEnumerable enumerable = value as IEnumerable;
                foreach (var item in enumerable)
                {
                    WriteValue(item, writer);
                }
            }
            // 그 외 복합 객체(클래스, 구조체 등)인 경우
            else
            {
                // 해당 객체의 필드들을 Reflection을 통해 순회하여 재귀적으로 직렬화합니다.
                FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                // 각 필드를 순차적으로 작성(필드 구분자나 개수를 기록할 수도 있으나 여기서는 단순히 연속 기록)
                foreach (var field in fields)
                {
                    object fieldValue = field.GetValue(value);
                    WriteValue(fieldValue, writer);
                }
            }
        }

        /// <summary>
        /// 기본형 또는 enum 값을 BinaryWriter로 기록합니다.
        /// </summary>
        private static void WritePrimitive(object value, BinaryWriter writer)
        {
            // enum은 기본적으로 underlying type으로 변환하여 처리
            if (value.GetType().IsEnum)
            {
                object underlyingValue = Convert.ChangeType(value, Enum.GetUnderlyingType(value.GetType()));
                WritePrimitive(underlyingValue, writer);
                return;
            }

            switch (Type.GetTypeCode(value.GetType()))
            {
                case TypeCode.Boolean:
                    writer.Write((bool)value);
                    break;
                case TypeCode.Char:
                    writer.Write((char)value);
                    break;
                case TypeCode.Byte:
                    writer.Write((byte)value);
                    break;
                case TypeCode.SByte:
                    writer.Write((sbyte)value);
                    break;
                case TypeCode.Int16:
                    writer.Write((short)value);
                    break;
                case TypeCode.UInt16:
                    writer.Write((ushort)value);
                    break;
                case TypeCode.Int32:
                    writer.Write((int)value);
                    break;
                case TypeCode.UInt32:
                    writer.Write((uint)value);
                    break;
                case TypeCode.Int64:
                    writer.Write((long)value);
                    break;
                case TypeCode.UInt64:
                    writer.Write((ulong)value);
                    break;
                case TypeCode.Single:
                    writer.Write((float)value);
                    break;
                case TypeCode.Double:
                    writer.Write((double)value);
                    break;
                case TypeCode.Decimal:
                    writer.Write((decimal)value);
                    break;
                default:
                    throw new ArgumentException("지원하지 않는 기본형 타입: " + value.GetType().FullName);
            }
        }


        /// <summary>
        /// 바이트 배열로부터 T 형식의 객체를 역직렬화합니다.
        /// </summary>
        public static T DeserializeObjectFromByteArray<T>(this byte[] data) where T : new()
        {
            using (MemoryStream ms = new MemoryStream(data))
            using (BinaryReader reader = new BinaryReader(ms, Encoding.UTF8))
            {
                T obj = new T();
                PopulateObject(obj, reader);
                return obj;
            }
        }

        /// <summary>
        /// 객체의 모든 인스턴스 필드를 읽어 할당합니다.
        /// 만약 필드가 배열이나 List<T>라면, 클래스에 이미 생성되어 있는 인스턴스의 길이를 사용합니다.
        /// </summary>
        private static void PopulateObject(object obj, BinaryReader reader)
        {
            Type type = obj.GetType();
            FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var field in fields)
            {
                int? fixedLength = null;
                // 배열(T[])인 경우
                if (field.FieldType.IsArray)
                {
                    var existingArray = field.GetValue(obj) as Array;
                    if (existingArray != null)
                    {
                        fixedLength = existingArray.Length;
                    }
                }
                // List<T>와 같이 가변 길이 컬렉션인 경우
                else if (field.FieldType.IsGenericType &&
                         field.FieldType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    var existingList = field.GetValue(obj) as IList;
                    if (existingList != null)
                    {
                        fixedLength = existingList.Count;
                    }
                }

                object fieldValue = ReadValue(field.FieldType, reader, fixedLength);
                field.SetValue(obj, fieldValue);
            }
        }

        /// <summary>
        /// BinaryReader로부터 지정한 타입의 값을 읽어 반환합니다.
        /// fixedLength가 제공되면, 배열이나 List<T>는 그 길이만큼 요소를 읽습니다.
        /// </summary>
        private static object ReadValue(Type type, BinaryReader reader, int? fixedLength)
        {
            // 기본형 처리
            if (type.IsPrimitive)
            {
                return ReadPrimitive(type, reader);
            }
            // enum 처리 (underlying 타입으로 읽은 후 변환)
            else if (type.IsEnum)
            {
                Type underlying = Enum.GetUnderlyingType(type);
                object underlyingValue = ReadPrimitive(underlying, reader);
                return Enum.ToObject(type, underlyingValue);
            }
            // 배열 처리: 고정 길이(fixedLength)가 제공되어야 함
            else if (type.IsArray)
            {
                if (!fixedLength.HasValue)
                    throw new Exception("배열의 고정 길이 정보가 제공되지 않았습니다: " + type.FullName);
                int count = fixedLength.Value;
                Type elementType = type.GetElementType();
                Array arr = Array.CreateInstance(elementType, count);
                for (int i = 0; i < count; i++)
                {
                    // 배열의 각 요소는 재귀적으로 읽습니다.
                    arr.SetValue(ReadValue(elementType, reader, null), i);
                }
                return arr;
            }
            // 가변 길이 배열 처리: List<T>인 경우, fixedLength를 사용하여 요소를 읽습니다.
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                if (!fixedLength.HasValue)
                    throw new Exception("리스트의 고정 길이 정보가 제공되지 않았습니다: " + type.FullName);
                int count = fixedLength.Value;
                Type elementType = type.GetGenericArguments()[0];
                IList list = (IList)Activator.CreateInstance(type);
                for (int i = 0; i < count; i++)
                {
                    list.Add(ReadValue(elementType, reader, null));
                }
                return list;
            }
            // 복합 객체(클래스, 구조체)
            else
            {
                object instance = Activator.CreateInstance(type);
                PopulateObject(instance, reader);
                return instance;
            }
        }

        /// <summary>
        /// 기본형 값을 BinaryReader에서 읽어 반환합니다.
        /// </summary>
        private static object ReadPrimitive(Type type, BinaryReader reader)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean: return reader.ReadBoolean();
                case TypeCode.Char: return reader.ReadChar();
                case TypeCode.Byte: return reader.ReadByte();
                case TypeCode.SByte: return reader.ReadSByte();
                case TypeCode.Int16: return reader.ReadInt16();
                case TypeCode.UInt16: return reader.ReadUInt16();
                case TypeCode.Int32: return reader.ReadInt32();
                case TypeCode.UInt32: return reader.ReadUInt32();
                case TypeCode.Int64: return reader.ReadInt64();
                case TypeCode.UInt64: return reader.ReadUInt64();
                case TypeCode.Single: return reader.ReadSingle();
                case TypeCode.Double: return reader.ReadDouble();
                case TypeCode.Decimal: return reader.ReadDecimal();
                default:
                    throw new NotSupportedException("지원하지 않는 기본형 타입: " + type.FullName);
            }
        }

        public static string ToDescription(this Enum source)
        {
            FieldInfo fi = source.GetType().GetField(source.ToString());
            var att = (DescriptionAttribute)fi.GetCustomAttribute(typeof(DescriptionAttribute));
            if (att != null)
            {
                return att.Description;
            }
            else
            {
                return source.ToString();
            }
        }
    }
}
