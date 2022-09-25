using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using AndroidX.Core.App;
using MauiApp1.DataAccess;

namespace MauiApp1.Services
{
    [Service]
    public class LocationService : Service, ILocationService, ILocationListener
    {
        public LocationService()
        {

        }

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

            locationManager.RequestSingleUpdate(LocationManager.NetworkProvider, this, null);

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
            #region Android-specific sensors implementation
            locationManager = (LocationManager)GetSystemService(LocationService);
            #endregion
        }

        public override void OnDestroy()
        {
            System.Diagnostics.Debug.WriteLine($"{DateTime.Now}: Entered OnDestroy");
            isServiceRunning = false;
            #region Android-specific sensors implementation
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

        #region Android-specific sensors implementation

        #region Location

        public void StartRequestingLocationUpdates()
        {
            locationManager.RequestLocationUpdates(LocationManager.GpsProvider, 5000, 1, this);
        }

        public void StopRequestingLocationUpdates()
        {
            locationManager.RemoveUpdates(this);
        }

        public void OnLocationChanged(Android.Locations.Location location)
        {
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

        #endregion
    }
}
