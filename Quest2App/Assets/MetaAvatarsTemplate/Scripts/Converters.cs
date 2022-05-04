using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Chiligames.MetaAvatars
{
    public class Converters
    {
        // Description:
        // MemoryCopy the contents of a NativeArray to a ByteArray and back.
        //
        public static unsafe void MoveToByteArray<T>(ref NativeArray<T> src, ref byte[] dst) where T : struct
        {

    #if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckReadAndThrow(NativeArrayUnsafeUtility.GetAtomicSafetyHandle(src));
            if (dst == null)
                throw new ArgumentNullException(nameof(dst));
    #endif

            var size = UnsafeUtility.SizeOf<T>() * src.Length;
            var srcAddr = (byte*)src.GetUnsafeReadOnlyPtr();
            if (dst.Length != size)
                dst = new byte[size];

            fixed (byte* dstAddr = dst)
            {
                UnsafeUtility.MemCpy(&dstAddr[0], &srcAddr[0], size);
            }
        }

        public static unsafe NativeArray<T> MoveFromByteArray<T>(ref byte[] src) where T : struct
        {
            /*
    #if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckReadAndThrow(NativeArrayUnsafeUtility.GetAtomicSafetyHandle(dst));
            if (src == null)
                throw new ArgumentNullException(nameof(src));
    #endif*/
            var size = UnsafeUtility.SizeOf<T>();

            var dst = new NativeArray<T>(src.Length / size, Allocator.Temp);
    #if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckReadAndThrow(NativeArrayUnsafeUtility.GetAtomicSafetyHandle(dst));
    #endif

            var dstAddr = (byte*)dst.GetUnsafeReadOnlyPtr();
            fixed (byte* srcAddr = src)
            {
                UnsafeUtility.MemCpy(&dstAddr[0], &srcAddr[0], src.Length);
            }

            return dst;
        }
    }
}
