using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Vst.Presets.NKS
{

    [Serializable]
    public class NksfException : Exception
    {
        public NksfException() { }
        public NksfException(string message) : base(message) { }
        public NksfException(string message, Exception inner) : base(message, inner) { }
        protected NksfException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

  
}
