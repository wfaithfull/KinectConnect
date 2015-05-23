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
using System.Windows.Data;
using System.Windows;

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

        private object padlock = new object();

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
                        if (Chooser.Kinect == null)
                        {
                            MessageBox.Show("Unable to detect an available Kinect Sensor");
                            Application.Current.Shutdown();
                        }
                        Manager.Dispatcher.Invoke(() => {
                            var kinectSensorBinding = new Binding("Kinect") { Source = Chooser };
                            BindingOperations.SetBinding(this.Manager, KinectSensorManager.KinectSensorProperty, kinectSensorBinding);
                        });
                    });
                    //return App.Current.Dispatcher.InvokeAsync(new Action(Chooser.Start)).Task;
                }));
            }
        }

        public KinectViewModel()
        {
            Manager = new KinectSensorManager();
            Manager.KinectSensorChanged += KinectChangedHandler;
            //Manager = new KinectSensorManager();
            //Chooser = new KinectSensorChooser(); 
            //Chooser.KinectChanged += KinectChangedHandler;
        }

        

        private async void KinectChangedHandler(object sender, KinectSensorManagerEventArgs<KinectSensor> args)
        {
            KinectSensor newSensor = args.NewValue;
            KinectSensor oldSensor = args.OldValue;

            if (oldSensor != null)
            {
                oldSensor.ColorStream.Disable();
                oldSensor.DepthStream.Disable();
                oldSensor.DepthStream.Range = DepthRange.Default;
                oldSensor.SkeletonStream.Disable();
                oldSensor.SkeletonStream.EnableTrackingInNearRange = false;
                oldSensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Default;
            }
            
            LoadingMessage = string.Format("Kinect found: [{0}] [{1}]", newSensor.UniqueKinectId, newSensor.Status.ToString());

            // Bind the KinectSensor from the sensorChooser to the KinectSensor on the KinectSensorManager
            
            //Debug.WriteLine("changing elevation angle");
            //newSensor.ElevationAngle += 5;
            Debug.WriteLine("Preparing sensor");
            Manager.ColorStreamEnabled = true;
            Manager.DepthStreamEnabled = true;
            Manager.SkeletonEnableTrackingInNearMode = true;
            Manager.SkeletonTrackingMode = SkeletonTrackingMode.Seated;
            Manager.SkeletonStreamEnabled = true;
            Manager.KinectSensorEnabled = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void FirePropertyChanged([CallerMemberName] string name = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        
    }
}
