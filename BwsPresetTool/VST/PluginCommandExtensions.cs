using Jacobi.Vst.Interop.Host;

namespace BwsPresetTool.VST
{
    static class PluginCommandExtensions
    {
        public static FxPreset ToPreset(this VstPluginContext plugin, bool asPreset = true)
        {            
            return new FxPreset(
                plugin.PluginCommandStub.GetProgramName() ?? "",
                plugin.PluginCommandStub.GetChunk(asPreset),
                1,
                (uint)plugin.PluginInfo.PluginID,
                (uint)plugin.PluginInfo.PluginVersion,
                (uint)(asPreset ? plugin.PluginInfo.ParameterCount : plugin.PluginInfo.ProgramCount)
                );
        }

        public static FxPreset ToBank(this VstPluginContext plugin, byte[] chunk)
        {
            return new FxPreset(
                plugin.PluginCommandStub.GetProgramName() ?? "",
                chunk,
                1,
                (uint)plugin.PluginInfo.PluginID,
                (uint)plugin.PluginInfo.PluginVersion,
                (uint)plugin.PluginInfo.ParameterCount
                );
        }

    }
}
