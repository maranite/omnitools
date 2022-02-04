using System;

namespace BwsPresetTool.VST
{
    [Serializable]
    public class PresetException : Exception
    {
        public PresetException() { }
        public PresetException(string message) : base(message) { }
        public PresetException(string message, Exception inner) : base(message, inner) { }
        protected PresetException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
