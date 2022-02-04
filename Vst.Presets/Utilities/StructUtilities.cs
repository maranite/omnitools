using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vst.Presets.Utilities
{
    public static class StructUtilities
    {
        public static bool IsIn<T>(this T value, params T[] args)            where T : IEquatable<T>
        {
            for (int i = 0; i < args.Length; i++)
                if (value.Equals(args[i]))
                    return true;
            return false;
        }
    }
}
