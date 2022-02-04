using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vst.Presets.Utilities
{
    public class KeyValueStringAdapter
    {
        public KeyValueStringAdapter(Func<string> getValue, Action<string> setValue)
        {
            this.Getter = getValue;
            this.Setter = setValue;
        }

        private readonly Func<string> Getter;
        private readonly Action<string> Setter;

        public IEnumerable<string> this[string key]
        {
            get
            {
                return from part in Getter().Split(';')
                       let x = part.Split('=')
                       where x.Length > 0 && x[0] == key
                       select x.Length > 1 ? x[1] : "";                    
            }
            set
            {
                Setter(string.Join(";",
                            (value ?? Enumerable.Empty<string>())
                            .Where(_ => _ != null) 
                            .Select(_ => string.Format("{0}={1}", key, _))
                             .Concat(from part in Getter().Split(';')
                                     let x = part.Split('=')
                                     where x.Length == 2 && x[0] != key
                                     select part)
                        ));
            }
        }
    }
}
