using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Piruzhaolu.ThreejsEditor
{
    public static class BytesUtility
    {

        public static byte[] ToBytes<T>(T[] datas) where T:struct
        {
            var nativeArray = new NativeArray<T>(datas, Allocator.Temp);
            var bytes = nativeArray.Reinterpret<byte>(UnsafeUtility.SizeOf<T>());
            return bytes.ToArray();
        }
    }
}