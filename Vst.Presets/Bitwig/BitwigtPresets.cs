using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vst.Presets.Utilities;

namespace Vst.Presets.Bitwig
{
    public static class BitwigPresets
    {
        public static BitwigPreset Synthmaster =>
            BitwigPreset.Load.From(Properties.Resources.Synthmaster_Template);

        public static BitwigPreset OmnisphereMulti => 
            BitwigPreset.Load.From(Properties.Resources.OmnisphereTemplatebwpreset);
    }
}
