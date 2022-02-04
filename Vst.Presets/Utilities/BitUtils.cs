using System;
using System.Runtime.InteropServices;

namespace Vst.Presets.Utilities
{
    static class BitUtils
    {
        public static void ReadStruct<T>(byte[] bytes, out T result)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));

            if (bytes.Length != Marshal.SizeOf(typeof(T)))
                throw new ArgumentException($"length of bytes[] argument is not equal to size of {typeof(T).Name}", nameof(bytes));

            var handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            result = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();
        }

        public static byte[] StructureToBytes<T>(this T value) where T : struct
        {
            int size = Marshal.SizeOf(value);
            var result = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(value, ptr, true);
            Marshal.Copy(ptr, result, 0, size);
            Marshal.FreeHGlobal(ptr);
            return result;
        }

        public static byte[] Reverse(this byte[] b)
        {
            var result = new byte[b.Length];
            Array.Copy(b, result, b.Length);
            Array.Reverse(result);
            return result;
        }

        public static ushort Reverse(this ushort value)
        {
            return (ushort)((value >> 08) | (value << 08));
        }

        public static uint Reverse(this uint x)
        {
            x = (x >> 16) | (x << 16);
            return ((x & 0xFF00FF00) >> 8) | ((x & 0x00FF00FF) << 8);
        }

        public static ulong Reverse(this ulong x)
        {
            x = (x >> 32) | (x << 32);
            x = ((x & 0xFFFF0000FFFF0000) >> 16) | ((x & 0x0000FFFF0000FFFF) << 16);
            return ((x & 0xFF00FF00FF00FF00) >> 8) | ((x & 0x00FF00FF00FF00FF) << 8);
        }

    }
}
