using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BwsPresetTool.VST
{
    public interface IMetaData
    {
        string PresetName { get; set; }
        string Plugin { get; set; }
        string Author{ get; set; }
        string Category { get; set; }
        IEnumerable<string> Tags { get; set; }
        string Comments { get; set; }
    }
}
