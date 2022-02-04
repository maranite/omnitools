using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Vst.Presets.NKS
{

    public struct PLID
    {
        public int chunkVersion;
        public int vstMagic;
        public PLID(int vstMagic, int chunkVersion = 1)
        {
            this.vstMagic = vstMagic;
            this.chunkVersion = chunkVersion;
        }
    }

  
}
