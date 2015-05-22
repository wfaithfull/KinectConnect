using Microsoft.Kinect.Toolkit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using KinectConnect.Util;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using KinectConnect.SDK1x.Extraction;
using System.Windows.Media.Imaging;
using Microsoft.Kinect;
using Microsoft.Samples.Kinect.WpfViewers;
using KinectConnect.SDK1x.Models;

namespace KinectConnect.SDK1x.ViewModels
{
    public class KinectViewModel : INotifyPropertyChanged
    {
        private string loadingMessage;
        private BitmapSource colorStream;

        private FaceDataExtractor faceExtractor;

        public SensorModel Model { get; set; }
        public KinectSensorChooser Chooser { get; set; }
        public KinectSensorManager Manager { get; set; }
        public string LoadingMessage
        {
            get { return loadingMessage; }
            set { loadingMessage = value; FirePropertyChanged(); }
        }

        public BitmapSource ColorStream
        {
            get { return colorStream; }
            set { colorStream = value; FirePropertyChanged(); }
        }

        private ICommand initAsyncCommand;

        public ICommand InitAsyncCommand
        {
            get 
            {
                return initAsyncCommand ?? (initAsyncCommand = new KinectConnect.Util.AsyncDelegateCommand(() =>
                {
                    LoadingMessage = "Searching for kinect sensors...";
                    return Task.Run(() => {


                        Chooser.Start();
                    });
                }));
            }
        }

        public KinectViewModel()
        {
            Chooser = new KinectSensorChooser();
            Manager = new KinectSensorManager();
            Chooser.KinectChanged += KinectChangedHandler;
        }

        

        private async void KinectChangedHandler(object sender, KinectChangedEventArgs args)
        {
            KinectSensor newSensor = args.NewSensor;
            KinectSensor oldSensor = args.OldSensor;

            LoadingMessage = string.Format("Kinect found: [{0}] [{1}]", newSensor.UniqueKinectId, newSensor.Status.ToString());

            Manager.KinectSensor = newSensor;

            await Task.Factory.StartNew(() => { faceExtractor = new FaceDataExtractor(newSensor); });
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void FirePropertyChanged([CallerMemberName] string name = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        
    }
}
