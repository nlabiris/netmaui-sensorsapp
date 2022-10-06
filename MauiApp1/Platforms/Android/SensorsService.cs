using Android.App;
using Android.Content;
using Android.Hardware;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Util;
using AndroidX.Core.App;
using MauiApp1.DataAccess;
using Renci.SshNet;

namespace MauiApp1.Services
{
    [Service]
    public class SensorsService : Service, ISensorsService, ISensorEventListener
    {
        public SensorsService()
        {

        }

        private static bool isServiceRunning;
        private static Database database;
        #region Android-specific sensors implementation
        private SensorManager sensorManager;
        #endregion
        private CancellationTokenSource cancelTokenSource;
        private bool isCheckingLocation;

        public static Database Database
        {
            get
            {
                if (database == null)
                {
                    database = new Database(Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "sensors.db3"));
                }
                return database;
            }
        }

        #region Generic methods

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            System.Diagnostics.Debug.WriteLine($"{DateTime.Now}: Service is starting...");

            #region Nothing here...

            //Task.Run(() =>
            //{
            //    while (isServiceRunning)
            //    {
            //        System.Diagnostics.Debug.WriteLine($"{DateTime.Now}: Foreground Service is Running");
            //        Thread.Sleep(2000);
            //    }
            //});

            #endregion

            #region Setup notification

            string channelID = "SensorsServiceChannel";
            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var notfificationChannel = new NotificationChannel(channelID, channelID, NotificationImportance.Low);
                notificationManager.CreateNotificationChannel(notfificationChannel);
            }

            var notificationBuilder = new NotificationCompat.Builder(this, channelID)
                                         .SetContentTitle("SensorsServiceStarted")
                                         .SetSmallIcon(Resource.Mipmap.appicon)
                                         .SetContentText("Sensors service running in foreground")
                                         .SetPriority(1)
                                         .SetOngoing(true)
                                         .SetChannelId(channelID)
                                         .SetAutoCancel(true);


            StartForeground(1001, notificationBuilder.Build());

            #endregion

            System.Diagnostics.Debug.WriteLine($"{DateTime.Now}: Notification setup done...");

            #region Android-specific sensors implementation

            sensorManager.RegisterListener(this, sensorManager.GetDefaultSensor(SensorType.Accelerometer), SensorDelay.Normal);
            //sensorManager.RegisterListener(this, sensorManager.GetDefaultSensor(SensorType.Gyroscope), SensorDelay.Normal);

            #endregion

            #region MAUI/Xamarin-specific sensors implementation

            //Accelerometer.Default.ReadingChanged += Accelerometer_ReadingChanged;
            //Accelerometer.Default.Start(SensorSpeed.UI);
            //Compass.Default.ReadingChanged += Compass_ReadingChanged;
            //Compass.Default.Start(SensorSpeed.UI);
            //Gyroscope.Default.ReadingChanged += Gyroscope_ReadingChanged;
            //Gyroscope.Default.Start(SensorSpeed.UI);
            //Magnetometer.Default.ReadingChanged += Magnetometer_ReadingChanged;
            //Magnetometer.Default.Start(SensorSpeed.UI);
            //OrientationSensor.Default.ReadingChanged += Orientation_ReadingChanged;
            //OrientationSensor.Default.Start(SensorSpeed.UI);

            #endregion

            System.Diagnostics.Debug.WriteLine($"{DateTime.Now}: Sensor event listeners registered...");

            return base.OnStartCommand(intent, flags, startId);
        }

        public override IBinder OnBind(Intent intent)
        {
            throw new NotImplementedException();
        }

        public override void OnCreate()
        {
            isServiceRunning = true;
            System.Diagnostics.Debug.WriteLine($"{DateTime.Now}: Entered OnCreate");
            base.OnCreate();
            SetWorker();
            #region Android-specific sensors implementation
            sensorManager = (SensorManager)GetSystemService(SensorService);
            #endregion
        }

        public override void OnDestroy()
        {
            System.Diagnostics.Debug.WriteLine($"{DateTime.Now}: Entered OnDestroy");
            isServiceRunning = false;
            #region Android-specific sensors implementation
            sensorManager.UnregisterListener(this);
            #endregion
            #region MAUI/Xamarin-specific sensors implementation
            //Accelerometer.Default.Stop();
            //Accelerometer.Default.ReadingChanged -= Accelerometer_ReadingChanged;
            //Compass.Default.Stop();
            //Compass.Default.ReadingChanged -= Compass_ReadingChanged;
            //Gyroscope.Default.Stop();
            //Gyroscope.Default.ReadingChanged -= Gyroscope_ReadingChanged;
            //Magnetometer.Default.Stop();
            //Magnetometer.Default.ReadingChanged -= Magnetometer_ReadingChanged;
            //OrientationSensor.Default.Stop();
            //OrientationSensor.Default.ReadingChanged -= Orientation_ReadingChanged;
            #endregion
            base.OnDestroy();
        }

        public void StartService()
        {
            var intent = new Intent(Android.App.Application.Context, typeof(SensorsService));
            Android.App.Application.Context.StartForegroundService(intent);
        }

        public void StopService()
        {
            var intent = new Intent(Android.App.Application.Context, typeof(SensorsService));
            Android.App.Application.Context.StopService(intent);
        }

        public bool IsServiceRunning()
        {
            return isServiceRunning;
        }

        #endregion

        #region Android-specific sensors implementation

        public async void OnSensorChanged(SensorEvent e)
        {
            if (e.Sensor.Type == SensorType.Accelerometer)
            {
                await Database.SaveAccelerometerAsync(new Models.Accelerometer
                {
                    X = e.Values[0],
                    Y = e.Values[1],
                    Z = e.Values[2],
                    Created = DateTime.UtcNow
                });
            }
            else if (e.Sensor.Type == SensorType.Gyroscope)
            {
                await Database.SaveGyroscopeAsync(new Models.Gyroscope
                {
                    X = e.Values[0],
                    Y = e.Values[1],
                    Z = e.Values[2],
                    Created = DateTime.UtcNow
                });
            }
            else if (e.Sensor.Type == SensorType.MagneticField)
            {
                await Database.SaveMagnetometerAsync(new Models.Magnetometer
                {
                    X = e.Values[0],
                    Y = e.Values[1],
                    Z = e.Values[2],
                    Created = DateTime.UtcNow
                });
            }
            else if (e.Sensor.Type == SensorType.Orientation)
            {
                await Database.SaveOrientationAsync(new Models.Orientation
                {
                    Z = e.Values[0],
                    X = e.Values[1],
                    Y = e.Values[2],
                    Created = DateTime.UtcNow
                });
            }
        }

        public void OnAccuracyChanged(Sensor sensor, [GeneratedEnum] SensorStatus accuracy)
        {

        }

        #endregion

        #region MAUI/Xamarin-specific sensors implementation

        private async void Accelerometer_ReadingChanged(object sender, AccelerometerChangedEventArgs e)
        {
            await Database.SaveAccelerometerAsync(new Models.Accelerometer
            {
                X = e.Reading.Acceleration.X,
                Y = e.Reading.Acceleration.Y,
                Z = e.Reading.Acceleration.Z,
                Created = DateTime.UtcNow
            });
        }

        private async void Compass_ReadingChanged(object sender, CompassChangedEventArgs e)
        {
            await Database.SaveCompassAsync(new Models.Compass
            {
                HeadingMagneticNorthDegrees = e.Reading.HeadingMagneticNorth,
                Created = DateTime.UtcNow
            });
        }

        private async void Gyroscope_ReadingChanged(object sender, GyroscopeChangedEventArgs e)
        {
            await Database.SaveGyroscopeAsync(new Models.Gyroscope
            {
                X = e.Reading.AngularVelocity.X,
                Y = e.Reading.AngularVelocity.Y,
                Z = e.Reading.AngularVelocity.Z,
                Created = DateTime.UtcNow
            });
        }

        private async void Magnetometer_ReadingChanged(object sender, MagnetometerChangedEventArgs e)
        {
            await Database.SaveMagnetometerAsync(new Models.Magnetometer
            {
                X = e.Reading.MagneticField.X,
                Y = e.Reading.MagneticField.Y,
                Z = e.Reading.MagneticField.Z,
                Created = DateTime.UtcNow
            });
        }

        private async void Orientation_ReadingChanged(object sender, OrientationSensorChangedEventArgs e)
        {
            await Database.SaveOrientationAsync(new Models.Orientation
            {
                Z = e.Reading.Orientation.Z,
                X = e.Reading.Orientation.X,
                Y = e.Reading.Orientation.Y,
                W = e.Reading.Orientation.W,
                Created = DateTime.UtcNow
            });
        }

        #endregion

        #region Upload sensors DB

        public async void UploadDb()
        {
            // adb -d shell "run-as com.companyname.mauiapp1 rm /data/user/0/com.companyname.mauiapp1/files/sensors.db3"
            var path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "sensors.db3");
            if (!File.Exists(path)) return;
            var task = Database.GetWorkerByAliasAsync("upload_db");
            task.Wait();
            await Task.Factory.StartNew(() => FileUploadSFTP(path, task.Result));
        }

        private void FileUploadSFTP(string path, Models.Worker worker)
        {
            var keyFilePath = Path.Combine(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDocuments).AbsolutePath, "id_rsa");
            if (!File.Exists(keyFilePath)) return;
            var keyFile = new PrivateKeyFile(keyFilePath, worker.Password);
            using (var authenticationMethodRsa = new PrivateKeyAuthenticationMethod(worker.Username, keyFile))
            {
                var conn = new ConnectionInfo(worker.Hostname, worker.Port, worker.Username, authenticationMethodRsa);
                using (SftpClient client = new SftpClient(conn))
                {
                    client.Connect();
                    if (client.IsConnected)
                    {
                        using (var fileStream = new FileStream(path, FileMode.Open))
                        {
                            client.BufferSize = 4 * 1024; // bypass Payload error large files
                            client.UploadFile(fileStream, Path.GetFileName(path));
                        }
                    }
                    else
                    {
                        // Not connected
                    }
                }
            }
        }

        private async void SetWorker()
        {
            var task = Database.GetWorkerByAliasAsync("upload_db");
            task.Wait();
            if (task.Result == null)
            {
                await Database.InsertWorkerAsync(new Models.Worker
                {
                    Alias = "upload_db",
                    Username = "",
                    Password = "",
                    Port = 22,
                    Scheme = "sftp",
                    Hostname = "192.168.1.250",
                    Created = DateTime.UtcNow
                });
            }
        }

        #endregion
    }
}
