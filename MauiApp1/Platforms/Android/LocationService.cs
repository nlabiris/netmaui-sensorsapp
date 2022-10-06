using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using AndroidX.Core.App;
using Mapsui.UI.Maui;
using MauiApp1.DataAccess;

namespace MauiApp1.Services
{
    [Service]
    public class LocationService : Service, ILocationService, ILocationListener
    {
        public LocationService()
        {

        }

        //private static readonly Random rng = new Random(0);
        private static bool isServiceRunning;
        private LocationManager locationManager;
        private static Database database;
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

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            System.Diagnostics.Debug.WriteLine($"{DateTime.Now}: Service is starting...");

            #region Setup notification

            string channelID = "LocationServiceChannel";
            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var notfificationChannel = new NotificationChannel(channelID, channelID, NotificationImportance.Low);
                notificationManager.CreateNotificationChannel(notfificationChannel);
            }

            var notificationBuilder = new NotificationCompat.Builder(this, channelID)
                                         .SetContentTitle("LocationServiceStarted")
                                         .SetSmallIcon(Resource.Mipmap.appicon)
                                         .SetContentText("Location service running in foreground")
                                         .SetPriority(1)
                                         .SetOngoing(true)
                                         .SetChannelId(channelID)
                                         .SetAutoCancel(true);


            StartForeground(1002, notificationBuilder.Build());

            #endregion

            System.Diagnostics.Debug.WriteLine($"{DateTime.Now}: Notification setup done...");
            System.Diagnostics.Debug.WriteLine($"{DateTime.Now}: Network provider enabled: {locationManager.IsProviderEnabled(LocationManager.NetworkProvider)}");
            System.Diagnostics.Debug.WriteLine($"{DateTime.Now}: GPS provider enabled: {locationManager.IsProviderEnabled(LocationManager.GpsProvider)}");

            //locationManager.RequestSingleUpdate(LocationManager.NetworkProvider, this, null);
            locationManager.RequestLocationUpdates(LocationManager.NetworkProvider, 10000, 1, this);

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
            #region Android-specific location implementation
            locationManager = (LocationManager)GetSystemService(LocationService);
            #endregion
        }

        public override void OnDestroy()
        {
            System.Diagnostics.Debug.WriteLine($"{DateTime.Now}: Entered OnDestroy");
            isServiceRunning = false;
            #region Android-specific location implementation
            locationManager.RemoveUpdates(this);
            #endregion
            base.OnDestroy();
        }

        public void StartService()
        {
            var intent = new Intent(Android.App.Application.Context, typeof(LocationService));
            Android.App.Application.Context.StartForegroundService(intent);
        }

        public void StopService()
        {
            var intent = new Intent(Android.App.Application.Context, typeof(LocationService));
            Android.App.Application.Context.StopService(intent);
        }

        public bool IsServiceRunning()
        {
            return isServiceRunning;
        }

        #region Android-specific location implementation

        public async void OnLocationChanged(Android.Locations.Location location)
        {
            //var coordinates = new Dictionary<int, string>
            //{
            //    { 0, "38.0518394;23.7958184" },
            //    { 1, "38.0505495;23.8050379" },
            //    { 2, "38.0603702;23.808501" },
            //    { 3, "38.0537261;23.8003922" },
            //    { 4, "38.0537261;23.8003922" },
            //    { 5, "38.0555079;23.7959239" }
            //};
            //var i = rng.Next(0, 6);
            //MessagingCenter.Send(coordinates[i], nameof(OnLocationChanged));
            //MessagingCenter.Send($"{location.Latitude};{location.Longitude}", nameof(OnLocationChanged));
            await Database.SaveLocationAsync(new Models.Location
            {
                Latitude = location.Latitude,
                Longitude = location.Longitude,
                Accuracy = location.Accuracy,
                Altitude = location.Altitude,
                Speed = location.Speed,
                VerticalAccuracy = location.VerticalAccuracyMeters,
                Created = DateTime.UtcNow
            });
            System.Diagnostics.Debug.WriteLine($"{DateTime.Now}: Latitude: {location.Latitude}, Longitude: {location.Longitude}, Provider: {location.Provider}");
        }

        public void OnProviderDisabled(string provider)
        {
            System.Diagnostics.Debug.WriteLine($"{DateTime.Now}: The provider {provider} is disabled");
        }

        public void OnProviderEnabled(string provider)
        {
            System.Diagnostics.Debug.WriteLine($"{DateTime.Now}: The provider {provider} is enabled");
        }

        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {
            System.Diagnostics.Debug.WriteLine($"{DateTime.Now}: The provider {provider} has changed status to {status}");
        }

        #endregion
    }
}
