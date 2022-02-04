using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vst.Presets.Utilities
{
    public static class ILookupExtensions
    {
        public static IEnumerable<U> AnyOf<T,U>(this ILookup<T,U> lookup, params T[] keys)
        {
            return keys.SelectMany(_ => lookup[_]);
        }
    }
}
