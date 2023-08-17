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


namespace default_audio_devicer
{
    public partial class DefaultAudioDevicer : ServiceBase, IObserver<DeviceChangedArgs>
    {
        protected readonly EventLog eventLog;

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
        }

        protected override void OnStart(string[] args)
        {
            eventLog.WriteEntry("Start");
            controller = new CoreAudioController();
            controller.AudioDeviceChanged.Subscribe(this);
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
            defaultOut = controller.DefaultPlaybackDevice;
            defaultComs = controller.DefaultPlaybackCommunicationsDevice;
        }

        protected void SetComsToOut()
        {
            FetchDefaultDevices();
            if (defaultComs != defaultOut)
            {
                eventLog.WriteEntry(
                    "default audio device change detected\n" + 
                    "old device: " + defaultComs.FullName + "\n" +
                    "new device: " + defaultOut.FullName);
                defaultOut.SetAsDefaultCommunications();
                return;
            }
        }

        public void OnNext(DeviceChangedArgs value)
        {
            SetComsToOut();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }
    }
}
