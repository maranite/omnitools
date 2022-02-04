using System;
using System.Xml;
using System.Reflection;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Jacobi.Vst.Core;
using Jacobi.Vst.Core.Host;
using Jacobi.Vst.Interop.Host;
using System.Runtime.InteropServices;
using System.Threading;
using System.Collections.Concurrent;
using static System.Environment;
using System.Diagnostics;
using BwsPresetTool.Bitwig;
using BwsPresetTool.VST;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace BwsPresetTool
{
    // TODO:    Add binary scraper functionality
    // TODO:    Add midi-pulser menu
    // TODO:    Add generic save all presets using midi pulsing menu
    public partial class VstPluginHost : Form, IVstHostCommandStub
    {
        #region Initialization

        SynchronizationContext context = SynchronizationContext.Current;

        void OnMainThread(Action toDo)
        {
            if (context != null)
                context.Post(cv => { toDo(); }, null);
            else
                toDo();
        }

        public VstPluginHost(string pluginPath)
        {
            InitializeComponent();

            var assembly = Assembly.GetEntryAssembly();
            var att = assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
            if (att != null && att.Length > 0)
                ProductString = ((AssemblyProductAttribute)att[0]).Product;

            att = assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
            if (att != null && att.Length > 0)
                VendorString = ((AssemblyCompanyAttribute)att[0]).Company;

            var version = assembly.GetName().Version;
            VendorVersion = version.Major * 1000 +
                            version.Minor * 100 +
                            version.Build * 10 +
                            version.Revision;

            Directory = Assembly.GetExecutingAssembly().Location;


            var plugin = VstPluginContext.Create(pluginPath, this);
            if (plugin == null)
                throw new ArgumentException();

            if ((plugin.PluginInfo.Flags & VstPluginFlags.CanReplacing) == 0)
                throw new Exception();

            Application.Idle += Application_Idle;

            plugin.Set("PluginPath", pluginPath);
            plugin.Set("HostCmdStub", this);
            plugin.AcceptPluginInfoData(false);
            Plugin = plugin;
            Directory = pluginPath;
        }

        private void Application_Idle(object sender, EventArgs e)
        {
            if (PluginCommandStub != null)
                PluginCommandStub.EditorIdle();
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);
            EngineRunning = false;
            if (Plugin != null)
            {
                Application.Idle -= Application_Idle;
                PluginCommandStub.MainsChanged(false);
                Plugin.Dispose();
                Plugin = null;
                PluginCommandStub = null;
            }
        }

        protected override void OnShown(EventArgs e)
        {
            context = SynchronizationContext.Current;
            base.OnShown(e);
            PluginCommandStub.EditorOpen(pluginPanel.Handle);
            EngineRunning = true;
            synthMasterToolStripMenuItem.Visible = Plugin.PluginInfo.PluginID == 0x536d3269;
            z3taToolStripMenuItem.Visible = Plugin.PluginInfo.PluginID == 0x7a337432;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            EngineRunning = false;
            PluginCommandStub.EditorClose();
        }

        public IVstPluginCommandStub PluginCommandStub { get; private set; }

        private VstPluginContext _plugin;

        public VstPluginContext Plugin
        {
            get { return _plugin; }
            private set
            {
                _plugin = value;
                if (value == null)
                    return;

                value.Set("HostCmdStub", this);
                // actually open the plugin itself
                PluginCommandStub = value.PluginCommandStub;
                PluginCommandStub.Open();
                PluginCommandStub.MainsChanged(true);
                PluginCommandStub.SetBlockSize(BlockSize);
                PluginCommandStub.SetSampleRate(44100f);

                Rectangle wndRect = new Rectangle();
                if (PluginCommandStub.EditorGetRect(out wndRect))
                    Size = SizeFromClientSize(new Size(wndRect.Width, wndRect.Height + menuStrip1.Size.Height));

                var name = PluginCommandStub.GetEffectName();
                Text = name;

                synthMasterToolStripMenuItem.Enabled = name.ToLowerInvariant().Contains("synthmaster");

                //saveAllPatchesToPCHKToolStripMenuItem.Enabled = name.ToLowerInvariant().Contains("omnisphere");
            }
        }

        #endregion

        #region Engine & Preset Enumerator

        ConcurrentQueue<VstEvent[]> queuedEvents = new ConcurrentQueue<VstEvent[]>();
        CancellationTokenSource engineCancellation;
        Task engine;

        public bool EngineRunning
        {
            get
            {
                return engine != null;
            }
            set
            {
                if (value == (engine != null))
                    return;

                if (value)
                {
                    timer1.Enabled = true;
                    engineCancellation = new CancellationTokenSource();
                    engine = Task.Factory.StartNew(RunEngine);
                }
                else
                {
                    timer1.Enabled = false;
                    engineCancellation.Cancel();
                    engine.Wait();
                    engine.Dispose();
                    engine = null;
                    engineCancellation.Dispose();
                    engineCancellation = null;
                }
            }
        }

        private void RunEngine()
        {
            PluginCommandStub.MainsChanged(true);
            PluginCommandStub.StartProcess();
            PluginCommandStub.SetProcessPrecision(VstProcessPrecision.Process32);
            PluginCommandStub.SetBlockSize(BlockSize);
            PluginCommandStub.SetSampleRate(44100f);

            var cancel = engineCancellation.Token;
            using (var inputMgr = new VstAudioBufferManager(Plugin.PluginInfo.AudioInputCount, BlockSize))
            using (var outputMgr = new VstAudioBufferManager(Plugin.PluginInfo.AudioOutputCount, BlockSize))
            {
                var inputBuffers = inputMgr.ToArray();
                var outputBuffers = outputMgr.ToArray();
                while (!cancel.IsCancellationRequested)
                {
                    lock (this)
                    {
                        PluginCommandStub.SetSampleRate(44100f);
                        PluginCommandStub.ProcessReplacing(inputBuffers, outputBuffers);

                        if (queuedEvents.TryDequeue(out VstEvent[] events))
                            PluginCommandStub.ProcessEvents(events);
                    }
                    Thread.Sleep(50);
                }
            }

            Plugin.PluginCommandStub.StopProcess();
        }

        private void SendEventsToPlugin(params VstEvent[] events)
        {
            if (events != null)
                queuedEvents.Enqueue(events);
        }

        private void SendMidi(params byte[] data)
        {
            SendEventsToPlugin(new VstMidiEvent(0, 0, 0, data, 0, 0));
        }

        private void PulseMidiCC(byte cc = 55)
        {
            SendMidi(0xB0, cc, 127);
            //SendMidi(0xB0, cc, 0);
        }

        private byte[] DoUntilNewChunk(Action action, bool preset = false, int sleepTime = 1500, int maxAttempts = 5)
        {
            if (!EngineRunning) return null;
            var oldChunk = PluginCommandStub.GetChunk(preset);
            for (int attempt = 1; (attempt < maxAttempts && maxAttempts > 0); attempt++)
            {
                if (!Visible && !IsHandleCreated)
                    break;

                action();
                Thread.Sleep(sleepTime);

                var newChunk = PluginCommandStub.GetChunk(preset);
                if (!newChunk.SequenceEqual(oldChunk))
                    return newChunk;
            }
            return null;
        }


        private void SendMidiNow(params byte[] data)
        {
            PluginCommandStub.ProcessEvents(
                new VstEvent[] { new VstMidiEvent(0, 0, 0, data, 0, 0) });
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lock (this)
                PluginCommandStub.EditorIdle();
        }

        private void SetStatus(string message, bool isBusy = true)
        {
            context.Post(cv =>
            {
                Text = message;
                UseWaitCursor = isBusy;
            }, null);
        }

        private Task CreateMidiCCEnumerator(Action<byte[]> OnNextPreset)
        {
            EngineRunning = true;
            var token = new CancellationTokenSource();
            FormClosingEventHandler handler = (o, e) => { token.Cancel(); Thread.Sleep(1000); };
            FormClosing += handler;

            SetStatus("Tell Plugin to Midi Learn 'Next Preset'");

            return Task.Run(() =>
            {
                Action NextPreset = () => PulseMidiCC();

                //if (Plugin.PluginInfo.PluginID == 0x536d3269)        // synthmaster?
                //    NextPreset = () => OnMainThread(() => LeftMouseClick(pluginPanel.PointToScreen(SynthMaster2NextPreset)));

                try
                {
                    var done = new List<byte[]>();
                    var chunk = PluginCommandStub.GetChunk(false);
                    done.Add(chunk);
                    OnNextPreset(chunk);

                    while (!token.IsCancellationRequested && PluginCommandStub.GetChunk(false).SequenceEqual(chunk))
                    {
                        Thread.Sleep(500);
                        NextPreset();
                    }

                    if (token.IsCancellationRequested)
                    {
                        SetStatus("Aborted");
                        return;
                    }

                    SetStatus("Running");

                    chunk = PluginCommandStub.GetChunk(false);
                    done.Add(chunk);
                    OnNextPreset(chunk);

                    var sleepTime = 100;
                    for (int presetCount = 2; presetCount < 500 && !token.IsCancellationRequested; presetCount++)
                    {
                        var oldChunk = chunk;
                        do
                        {
                            NextPreset();
                            Thread.Sleep(sleepTime);
                            for (int i = 0; !token.IsCancellationRequested && i < 4; i++)
                            {
                                chunk = PluginCommandStub.GetChunk(false);
                                if (!oldChunk.SequenceEqual(chunk))
                                    break;

                                sleepTime += 100;
                                Thread.Sleep(sleepTime);
                            }
                        } while (!token.IsCancellationRequested && oldChunk.SequenceEqual(chunk));

                        if (done.Any(d => d.SequenceEqual(chunk)))
                            break;

                        OnNextPreset(chunk);

                        done.Add(chunk);

                        SetStatus($"Running: Processed {presetCount} presets");
                    }
                    SetStatus("Done", false);
                }
                catch (Exception ex)
                {
                    SetStatus(ex.Message, false);
                }
                finally
                {
                    FormClosing -= handler;
                }
            });
        }

        #endregion

        #region VstHostStub support properties

        IVstPluginContext IVstHostCommandStub.PluginContext { get; set; }
        public VstAutomationStates AutomationState { get; set; } = VstAutomationStates.Off;
        public string ProductString { get; set; } = "VSTHost";
        public string VendorString { get; set; } = "VSTHost";
        public int VendorVersion { get; set; } = 1000;
        public string Directory { get; set; }
        public int BlockSize { get; set; } = 64;
        public int InputLatency { get; set; } = 0;
        public int OutputLatency { get; set; } = 0;
        public float SampleRate { get; set; } = 44.8f;
        public VstHostLanguage GetLanguage() { return VstHostLanguage.NotSupported; }
        public VstProcessLevels ProcessLevel { get; set; } = VstProcessLevels.Realtime;

        #region IVstHostCommandStub methods to support properties

        VstCanDoResult IVstHostCommands20.CanDo(string cando)
        {
            switch (VstCanDoHelper.ParseHostCanDo(cando))
            {
                case VstHostCanDo.SizeWindow: return VstCanDoResult.Yes;
                case VstHostCanDo.SendVstMidiEvent: return VstCanDoResult.Yes;
                case VstHostCanDo.SendVstEvents: return VstCanDoResult.Yes;
                case VstHostCanDo.OpenFileSelector: return VstCanDoResult.Yes;
            }
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
        int IVstHostCommands10.GetCurrentPluginID() { return Plugin.PluginInfo.PluginID; }

        #endregion

        #endregion

        #region IVstHostCommandStub methods

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

        public virtual void ProcessIdle() { }
        public virtual bool IoChanged() { return false; }

        public virtual bool SizeWindow(int width, int height)
        {
            Size = SizeFromClientSize(new Size(width, height + menuStrip1.Size.Height));
            return true;
        }

        public virtual bool UpdateDisplay()
        {
            Refresh();
            return true;
        }

        #endregion

        #region Helpers

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        public void LeftMouseClick(Point point)
        {
            //BringToFront();
            const int MOUSEEVENTF_LEFTDOWN = 0x02;
            const int MOUSEEVENTF_LEFTUP = 0x04;
            Cursor.Position = point;
            Application.DoEvents();
            mouse_event(MOUSEEVENTF_LEFTDOWN, point.X, point.Y, 0, 0);
            Application.DoEvents();
            mouse_event(MOUSEEVENTF_LEFTUP, point.X, point.Y, 0, 0);
            Application.DoEvents();
        }

        public void LeftMouseClickOnThread(Point point)
        {
            const int MOUSEEVENTF_LEFTDOWN = 0x02;
            const int MOUSEEVENTF_LEFTUP = 0x04;
            SetCursorPos(point.X, point.Y);
            mouse_event(MOUSEEVENTF_LEFTDOWN, point.X, point.Y, 0, 0);
            Thread.Sleep(30);
            mouse_event(MOUSEEVENTF_LEFTUP, point.X, point.Y, 0, 0);
            Thread.Sleep(30);
        }

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        private string GetActiveWindowTitle()
        {
            const int nChars = 256;
            StringBuilder Buff = new StringBuilder(nChars);
            IntPtr handle = GetForegroundWindow();

            if (GetWindowText(handle, Buff, nChars) > 0)
                return Buff.ToString();

            return string.Empty;
        }

        #endregion

        #region SynthMaster 2

        private async void saveBanksAssmprToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Left = Screen.PrimaryScreen.Bounds.Width - Width;
            TopMost = true;
            Text = "Saving Presets...";
            var SynthMaster2Browse = pluginPanel.PointToScreen(new Point(120, 30));
            var SynthMaster2Presets = pluginPanel.PointToScreen(new Point(120, 45));
            var SynthMaster2BankName = pluginPanel.PointToScreen(new Point(120, 195));
            //var SynthMaster2NextPreset = pluginPanel.PointToScreen(new Point(590, 12));
            var SynthMaster2NextPreset = pluginPanel.PointToScreen(new Point(500, 12));

            var fallBackBankName = toolStripTextBoxBankName.Text.Trim();

            await Task.Run(() =>
            {
                var noChanges = 0;
                byte[] chunk = PluginCommandStub.GetChunk(false);
                while (++noChanges < 3)
                {
                    LeftMouseClickOnThread(SynthMaster2Presets);

                    if (!string.IsNullOrEmpty(fallBackBankName))
                    {
                        LeftMouseClickOnThread(SynthMaster2BankName);
                        SendKeys.SendWait(fallBackBankName);
                        SendKeys.SendWait("{ENTER}");
                        chunk = PluginCommandStub.GetChunk(false);
                    }

                    var sm = new SynthMasterPreset(chunk, "In Memory");
                    //var category = PluginCommandStub.GetCategory();
                    var progname = sm.PresetName ?? PluginCommandStub.GetProgramName().Trim();
                    var bankName = string.IsNullOrEmpty(fallBackBankName) ?
                        sm.BankName : fallBackBankName;

                    if (string.IsNullOrEmpty(bankName))
                        bankName = (sm.Author ?? "");

                    foreach (var c in @"()[]{}<>!?*\/+")
                    {
                        progname = progname.Replace(c, ' ').Trim();
                        bankName = bankName.Replace(c, ' ').Trim();
                    }

                    //   var preset = Plugin.ToPreset(false);
                    var filePath = $"{GetFolderPath(SpecialFolder.MyDocuments)}\\SynthMaster\\Xresets\\{bankName}\\{progname}.smpr";
                    var info = new FileInfo(filePath);
                    if (!info.Directory.Exists)
                        info.Directory.Create();

                    if (!File.Exists(filePath))
                    {
                        noChanges = 0;
                        File.WriteAllBytes(filePath, chunk);
                    }

                    byte[] prevChunk = chunk;
                    for (var evr = 0; evr < 100; evr++)
                    {
                        LeftMouseClickOnThread(SynthMaster2NextPreset);
                        Thread.Sleep(800);
                        chunk = PluginCommandStub.GetChunk(false);
                        if (!chunk.SequenceEqual(prevChunk))
                            break;
                        if (evr == 50)
                            SendKeys.Send("{ESC}");
                    }

                    if (chunk.SequenceEqual(prevChunk))
                    {
                        MessageBox.Show("Something is wrong - cannot change to a new preset!");
                        return;
                    }
                }
                LeftMouseClickOnThread(SynthMaster2Browse);
            });

            TopMost = false;
            //MessageBox.Show("All SynthMaster 2 Presets saved to SMPR format");
            Text = "Ready";
        }

        private async void resaveAllPresetsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Top = Screen.PrimaryScreen.Bounds.Height - Height + 100;
            Left = Screen.PrimaryScreen.Bounds.Width - Width;
            TopMost = true;
            Text = "Resaving Presets...";
            var SynthMaster29NextPreset = pluginPanel.PointToScreen(new Point(590, 12));
            var SynthMaster29SaveButton = pluginPanel.PointToScreen(new Point(630, 12));
            var SynthMaster29SavePreset = pluginPanel.PointToScreen(new Point(630, 30));
            var scr = Screen.PrimaryScreen.Bounds;
            var ClickOnYes = new Point(scr.Width / 2 + 45, scr.Height / 2 + 60);

            await Task.Run(() =>
            {
                var seen = new List<string>();
                while (true)
                {
                    LeftMouseClickOnThread(SynthMaster29SaveButton);
                    Thread.Sleep(100);
                    LeftMouseClickOnThread(SynthMaster29SavePreset);
                    Thread.Sleep(100);
                    LeftMouseClickOnThread(ClickOnYes);
                    Thread.Sleep(100);

                    var name = PluginCommandStub.GetProgramName();
                    for (int i = 1; i < 5 && name == PluginCommandStub.GetProgramName(); i++)
                    {
                        LeftMouseClickOnThread(SynthMaster29NextPreset);
                        Thread.Sleep(500);
                    }

                    name = PluginCommandStub.GetProgramName();
                    if (seen.Contains(name))
                        break;
                    seen.Add(name);

                    break;
                }
            });

            TopMost = false;
            Text = "Ready";
        }

        #endregion

        #region Utility Methods

        static string SearchChunkForFxpName(byte[] chunk)
        {
            var end = chunk.IndexOf(0x2E, 0x66, 0x78, 0x70, 0);      // find the phrase ".fxp\0"
            if (end > 0)
            {
                int start = end;
                end += 3;
                while (start > 1 && chunk[start + 1] != ':' && chunk[start - 1] >= 0x20)
                    start--;

                unsafe
                {
                    fixed (byte* pAscii = chunk)
                    {
                        return new String((sbyte*)pAscii, start, 1 + end - start);
                    }
                }
            }
            return null;
        }

        private void FileWriteAllBytes(string path, byte[] data)
        {
            var fi = new FileInfo(path);
            if (!fi.Directory.Exists)
                fi.Directory.Create();
            File.WriteAllBytes(path, data);
        }

        private bool PromptForFolder(string description, ref string selectedPath)
        {
            var sourceDialog = new FolderBrowserDialog()
            {
                RootFolder = SpecialFolder.MyComputer,
                Description = description,
                ShowNewFolderButton = true,
                SelectedPath = selectedPath
            };

            if (DialogResult.OK != sourceDialog.ShowDialog())
                return false;

            selectedPath = sourceDialog.SelectedPath;
            return true;
        }

        private bool PromptForBitwigTemplate(out BitwigPreset preset)
        {
            preset = null;
            var nae = Path.Combine(Environment.GetFolderPath(SpecialFolder.MyDocuments) + @"\Bitwig Studio\Library\Presets");
            var d = new OpenFileDialog()
            {
                Title = $"Select a .bwPreset file to use as a template",
                Filter = $"Bitwig Preset|*.bwpreset|All Files|*.*",
                //FileName = Path.Combine(Environment.GetFolderPath(SpecialFolder.MyDocuments) + @"\Bitwig Studio\Library\Presets"),
                InitialDirectory = Path.Combine(Environment.GetFolderPath(SpecialFolder.MyDocuments) + @"\Bitwig Studio\Library\Presets"),
                DefaultExt = "bwpreset",
                AddExtension = true,
                CheckPathExists = true,
                RestoreDirectory = false,
                Multiselect = false
            };

            // d.CustomPlaces.Add(Path.Combine(Environment.GetFolderPath(SpecialFolder.MyDocuments), @"\Bitwig Studio\Library\Presets\"));
            // d.CustomPlaces.Add(GetFolderPath(SpecialFolder.MyDocuments));

            if (DialogResult.OK != d.ShowDialog())
                return false;

            preset = BitwigPreset.From(d.FileName);
            return true;
        }


        #endregion

        #region Z3ta+

        private void WriteZ3taAsFXB(byte[] chunk, string targetpath)
        {
            var destPath = Path.Combine(targetpath, string.Format("{0:D}.fxb", Guid.NewGuid()));
            var presetPath = SearchChunkForFxpName(PluginCommandStub.GetChunk(true));
            if (!string.IsNullOrEmpty(presetPath))
            {
                var pinfo = new FileInfo(presetPath);
                var fileName = new FileInfo(presetPath);
                var name = new string(fileName.Name.SkipWhile(c => !char.IsLetter(c)).ToArray());
                name = name.Replace("_", " ").Replace("    ", " ").Replace("   ", " ").Replace("  ", " ").Trim();

                var folder = new string(fileName.Directory.Name.SkipWhile(c => !char.IsLetter(c)).ToArray());
                folder = folder.Replace("_", " ").Replace("    ", " ").Replace("   ", " ").Replace("  ", " ").Trim();
                if (folder.Length == 0)
                    folder = fileName.Directory.Name;

                destPath = Path.Combine(targetpath, fileName.Directory.Parent.Name, folder, name);
                destPath = Path.ChangeExtension(destPath, ".fxb");
            }

            var preset = new FxPreset(
                            Plugin.PluginCommandStub.GetProgramName() ?? "",
                            chunk,
                            1,
                            (uint)Plugin.PluginInfo.PluginID,
                            (uint)Plugin.PluginInfo.PluginVersion,
                            (uint)Plugin.PluginInfo.ParameterCount
                            );

            var destInfo = new FileInfo(destPath);

            if (!destInfo.Directory.Exists)
                destInfo.Directory.Create();

            FileWriteAllBytes(destPath, preset.ToFXB());
        }

        private void saveAllPresetsToFXBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var targetPath = Environment.GetFolderPath(SpecialFolder.MyDocuments);
            if (!PromptForFolder("Select where to save FXB files", ref targetPath))
                return;

            CreateMidiCCEnumerator(chunk => WriteZ3taAsFXB(chunk, targetPath));
        }

        #endregion

        #region Load and Save

        private void loadRawChunkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openRawChunkDialog.ShowDialog() == DialogResult.OK)
            {
                PluginCommandStub.SetChunk(
                    File.ReadAllBytes(openRawChunkDialog.FileName),
                    true
                    );
            }
        }

        private void saveRawChunbkMenuItem_Click(object sender, EventArgs e)
        {
            if (saveRawCunkDialog.ShowDialog() == DialogResult.OK)
            {
                FileWriteAllBytes(
                    saveRawCunkDialog.FileName,
                    PluginCommandStub.GetChunk(false)
                    );
            }
        }

        private void saveFXPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFX(false);
        }

        private void saveFXBBankToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFX(true);
        }

        private void SaveFX(bool asFXB = false)
        {
            var preset = Plugin.ToPreset(!asFXB);

            if (preset.PluginCode == "z3t2")
            {
                preset.PluginCode = "z3ta";
                preset.PluginVersion = 2;
                preset.FormatVersion = 2;
            }

            var ext = asFXB ? ".fxb" : ".fxp";
            var name = asFXB ? "Bank" : "Preset";

            var d = new SaveFileDialog()
            {
                Title = $"Save {name} As",
                Filter = $"{name}|{ext}|All Files|*.*",
                FileName = string.Format("{0}{1}", string.IsNullOrWhiteSpace(preset.Name) ? "Unknown" : preset.Name, ext),
                DefaultExt = ext,
                AddExtension = true,
                CheckPathExists = true,
                OverwritePrompt = false,
                RestoreDirectory = true
            };

            d.CustomPlaces.Add(GetFolderPath(SpecialFolder.MyDocuments));
            d.CustomPlaces.Add(GetFolderPath(SpecialFolder.ProgramFiles) + @"\Steinberg\VstPlugins");
            d.CustomPlaces.Add(GetFolderPath(SpecialFolder.ProgramFiles) + @"\Steinberg\VstPlugins\Z3TA+ 2\Programs\");
            d.CustomPlaces.Add(GetFolderPath(SpecialFolder.ProgramFiles));
            d.CustomPlaces.Add(GetFolderPath(SpecialFolder.ProgramFilesX86));

            if (DialogResult.OK == d.ShowDialog())
            {
                using (var file = File.Create(d.FileName))
                using (var writer = new BinaryWriter(file))
                {
                    if (asFXB)
                        preset.ToFXB(writer);
                    else
                        preset.ToFXP(writer);
                    file.Flush();
                }
            }
        }

        private void LoadFX(bool asFXB = false)
        {
            var ext = asFXB ? ".fxb" : ".fxp";
            var name = asFXB ? "Bank" : "Preset";

            var d = new OpenFileDialog()
            {
                Title = $"Load {name}",
                Filter = $"{name}|{ext}|All Files|*.*",
                FileName = $"*{ext}",
                DefaultExt = ext,
                AddExtension = true,
                CheckPathExists = true,
                RestoreDirectory = true
            };

            d.CustomPlaces.Add(GetFolderPath(SpecialFolder.MyDocuments));
            d.CustomPlaces.Add(GetFolderPath(SpecialFolder.ProgramFiles) + @"\Steinberg\VstPlugins\");
            d.CustomPlaces.Add(GetFolderPath(SpecialFolder.ProgramFiles) + @"\Steinberg\VstPlugins\Z3TA+ 2\Programs\");
            d.CustomPlaces.Add(GetFolderPath(SpecialFolder.ProgramFiles));
            d.CustomPlaces.Add(GetFolderPath(SpecialFolder.ProgramFilesX86));

            if (DialogResult.OK == d.ShowDialog())
            {
                var preset = FxPreset.From(d.FileName);
                PluginCommandStub.SetChunk(preset.Chunk, !asFXB);
            }
        }

        private void loadFXPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadFX(false);
        }

        private void loadFXBBankToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadFX(true);
        }

        #endregion


        private void savePresetsUsingMidiCCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var targetPath = Path.Combine(Environment.GetFolderPath(SpecialFolder.MyDocuments), @"Bitwig Studio\Library\Presets\");
            if (!PromptForFolder("Select where to save Bitwig presets", ref targetPath))
                return;

            if (!PromptForBitwigTemplate(out var template))
                return;

            CreateMidiCCEnumerator(chunk =>
               {
                   var fxb = Plugin.ToPreset(false);
                   template.PresetName = fxb.Name;
                   template.VstPreset = fxb;
                   //InferMetaData(template, null, out var bankName);
                   //var bwsPath = $"{targetPath}\\{bankName ?? ""}\\{template.PresetName}.bwPreset";
                   //FileWriteAllBytes(bwsPath, template.ToArray());
               });
        }

        private void testFXPVsFXBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var bank = PluginCommandStub.GetChunk(false);
            var pres = PluginCommandStub.GetChunk(true);

            var bs = BitConverter.ToString(bank);
            var ps = BitConverter.ToString(pres);

            MessageBox.Show(
                bank.SequenceEqual(pres) ? "Bank and Preset are identical" : "Banks are different to presets");
        }


    }
}
