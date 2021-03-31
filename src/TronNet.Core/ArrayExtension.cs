using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TronNet
{
    public static class ArrayExtension
    {
        public static T[] SubArray<T>(this T[] array, int offset, int length)
        {
            return array.Skip(offset)
                        .Take(length)
                        .ToArray();
        }

        public static byte[] Merge(params byte[][] arrays)
        {
            return MergeToEnum(arrays).ToArray();
        }
        private static IEnumerable<byte> MergeToEnum(params byte[][] arrays)
        {
            foreach (var a in arrays)
                foreach (var b in a)
                    yield return b;
        }
    }
}
