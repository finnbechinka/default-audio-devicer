using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;
using System.Timers;


namespace default_audio_devicer
{
    public partial class DefaultAudioDevicer : ServiceBase
    {
        EventLog eventLog;

        Timer timer;

        protected CoreAudioController controller;
        protected CoreAudioDevice defaultOut;
        protected CoreAudioDevice defaultComs;
        public DefaultAudioDevicer()
        {
            InitializeComponent();
            this.AutoLog = false;
            eventLog = new System.Diagnostics.EventLog();
            if (!System.Diagnostics.EventLog.SourceExists("DefaultAudioDevicer"))
            {
                System.Diagnostics.EventLog.CreateEventSource(
                    "DefaultAudioDevicer", "DefaultAudioDevicerLog");
            }
            eventLog.Source = "DefaultAudioDevicer";
            eventLog.Log = "DefaultAudioDevicerLog";

            timer = new Timer();
        }

        protected override void OnStart(string[] args)
        {
            eventLog.WriteEntry("Start");
            controller = new CoreAudioController();
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = 5000;
            timer.Enabled = true;
        }

        private void OnElapsedTime(object sender, ElapsedEventArgs e)
        {
            SetComsToOut();
        }

        protected override void OnPause()
        {
            eventLog.WriteEntry("Pause");
        }

        protected override void OnContinue()
        {
            eventLog.WriteEntry("Continue");
        }

        protected override void OnStop()
        {
            eventLog.WriteEntry("Stop");
        }

        protected void FetchDefaultDevices()
        {
            // eventLog.WriteEntry("fetch default devices");
            defaultOut = controller.DefaultPlaybackDevice;
            defaultComs = controller.DefaultPlaybackCommunicationsDevice;
            // eventLog.WriteEntry($"default out: {defaultOut.FullName}, default coms: {defaultComs.FullName}");
        }

        protected void SetComsToOut()
        {
            // eventLog.WriteEntry("set coms to out");
            FetchDefaultDevices();
            if (defaultComs != defaultOut)
            {
                // eventLog.WriteEntry("set coms and out different");
                defaultOut.SetAsDefaultCommunications();
                // eventLog.WriteEntry("coms set to out");
                return;
            }
            // eventLog.WriteEntry("coms is out");
        }
    }
}
