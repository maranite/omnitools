using Jacobi.Vst.Core;
using Jacobi.Vst.Core.Host;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace BwsPresetTool.VST
{
    public class HostCommandStub : IVstHostCommandStub, IVstHostCommands20, IVstHostCommands10
    {
        static bool GetAssemblyAttribute<TAttribute>(out TAttribute attribute) where TAttribute : Attribute
        {
            var att = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(TAttribute), false);
            if (att != null && att.Length > 0)
            {
                attribute = att[0] as TAttribute;
                return true;
            }
            attribute = null;
            return false;
        }

        public HostCommandStub()
        {
            ProductString = GetAssemblyAttribute(out AssemblyProductAttribute product) ? product.Product : "VSTHost";
            VendorString = GetAssemblyAttribute(out AssemblyCompanyAttribute company) ? company.Company : "Unknown";
            VendorVersion = 1000;

            var version = Assembly.GetExecutingAssembly().GetName().Version;
            VendorVersion = version.Major * 1000 +
                            version.Minor * 100 +
                            version.Build * 10 +
                            version.Revision;

            Directory = Assembly.GetExecutingAssembly().Location;
        }

        public IVstPluginContext PluginContext { get; set; }

        public readonly SortedList<string, bool> Capabilities = new SortedList<string, bool>();
        public VstAutomationStates AutomationState { get; set; } = VstAutomationStates.Off;
        public string ProductString { get; set; }
        public string VendorString { get; set; }
        public int VendorVersion { get; set; }
        public string Directory { get; set; }
        public int BlockSize { get; set; } = 1024;
        public int InputLatency { get; set; } = 0;
        public int OutputLatency { get; set; } = 0;
        public VstHostLanguage GetLanguage() { return VstHostLanguage.NotSupported; }
        public VstProcessLevels ProcessLevel { get; set; } = VstProcessLevels.Realtime;
        
        public float SampleRate { get; set; } = 44.8f;

        public virtual VstTimeInfo GetTimeInfo(VstTimeInfoFlags filterFlags)
        {
            return new VstTimeInfo
            {
                Flags = filterFlags,
                Tempo = 128,
                CycleEndPosition = 1,
                TimeSignatureNumerator = 4,
                TimeSignatureDenominator = 4,
                SmpteFrameRate = VstSmpteFrameRate.Smpte25fps,
                SamplesToNearestClock = 24
            };
        }

        public virtual bool OpenFileSelector(VstFileSelect fileSelect) { return false; }
        public virtual bool CloseFileSelector(VstFileSelect fileSelect) { return false; }

        public virtual bool BeginEdit(int index) { return false; }
        public virtual bool EndEdit(int index) { return false; }
        public virtual void SetParameterAutomated(int index, float value) { }

        public virtual bool ProcessEvents(VstEvent[] events) { return false; }
        public virtual bool SizeWindow(int width, int height) { return false; }
        public virtual bool UpdateDisplay() { return false; }
        public virtual void ProcessIdle() { }
        public virtual bool IoChanged() { return false; }

        #region Property-Backed Host Commands

        VstCanDoResult IVstHostCommands20.CanDo(string cando)
        {
            if (Capabilities.TryGetValue(cando, out bool cap))
                return cap ? VstCanDoResult.Yes : VstCanDoResult.No;
            return VstCanDoResult.Unknown;
        }
        VstAutomationStates IVstHostCommands20.GetAutomationState() { return AutomationState; }
        int IVstHostCommands20.GetBlockSize() { return BlockSize; }
        string IVstHostCommands20.GetDirectory() { return Directory; }
        int IVstHostCommands20.GetInputLatency() { return InputLatency; }
        int IVstHostCommands20.GetOutputLatency() { return OutputLatency; }
        VstProcessLevels IVstHostCommands20.GetProcessLevel() { return ProcessLevel; }
        string IVstHostCommands20.GetProductString() { return ProductString; }
        float IVstHostCommands20.GetSampleRate() { return SampleRate; }
        string IVstHostCommands20.GetVendorString() { return VendorString; }
        int IVstHostCommands20.GetVendorVersion() { return VendorVersion; }
        int IVstHostCommands10.GetVersion() { return VendorVersion; }
        int IVstHostCommands10.GetCurrentPluginID() { return PluginContext.PluginInfo.PluginID; }

        #endregion
    }
}
