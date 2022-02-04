using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using Automation = System.Management.Automation;
using System.IO;

namespace VstTools
{
    /// <summary>
    /// Base class for Cmdlets
    /// </summary>
    public abstract class PSCmdlet : System.Management.Automation.PSCmdlet
    {
        /// <summary>constructor</summary>
        protected PSCmdlet() : base()
        {
        }

        /// <summary>PRepares the cmdlet to run</summary>
        protected override void BeginProcessing()
        {
            base.BeginProcessing();

            Directory.SetCurrentDirectory(SessionState.Path.CurrentFileSystemLocation.Path);

            // resolve any FileInfo properties to contain absolute paths.
            foreach (var property in 
                        from property in GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        where property.CustomAttributes.OfType<Automation.ParameterAttribute>().Any()
                        where property.PropertyType == typeof(FileInfo)
                        select property)
            {
                if (property.GetValue(this) is FileInfo info && !Path.IsPathRooted(info.FullName))
                {
                    var path = GetUnresolvedProviderPathFromPSPath(info.FullName);
                    property.SetValue(this, new FileInfo(path));
                }
            }

            // resolve any directoryInfo properties to contain absolute paths.
            foreach (var property in
                        from property in GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        where property.CustomAttributes.OfType<Automation.ParameterAttribute>().Any()
                        where property.PropertyType == typeof(DirectoryInfo)
                        select property)
            {
                if (property.GetValue(this) is DirectoryInfo info && !Path.IsPathRooted(info.FullName))
                {
                    var path = GetUnresolvedProviderPathFromPSPath(info.FullName);
                    property.SetValue(this, new DirectoryInfo(path));
                }
            }
        }
    }
}
