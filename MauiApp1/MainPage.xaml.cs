using MauiApp1.Services;

namespace MauiApp1;

public partial class MainPage : ContentPage
{
    private readonly ISensorsService sensorsService;
    private readonly ILocationService locationService;

    public MainPage(ISensorsService sensorsService, ILocationService locationService)
	{
        this.sensorsService = sensorsService;
        this.locationService = locationService;
        InitializeComponent();
    }

    #region Android-specific sensors service

    private void btnStartSensorsService_Clicked(object sender, EventArgs e)
    {
        if (sensorsService.IsServiceRunning())
        {
            Application.Current.MainPage.DisplayAlert("Error", "Sensors service is already running", "OK");
        }
        else
        {
            sensorsService.StartService();
        }
    }

    private void btnStopSensorsService_Clicked(object sender, EventArgs e)
    {
        sensorsService.StopService();
    }

    private void btnUploadDb_Clicked(object sender, EventArgs e)
    {
        sensorsService.UploadDb();
    }

    #endregion

    #region Android-specific location service

    private void btnStartLocationService_Clicked(object sender, EventArgs e)
    {
        if (locationService.IsServiceRunning())
        {
            Application.Current.MainPage.DisplayAlert("Error", "Location service is already running", "OK");
        }
        else
        {
            locationService.StartService();
        }
    }

    private void btnStopLocationService_Clicked(object sender, EventArgs e)
    {
        locationService.StopService();
    }

    #endregion

    #region MAUI/Xamarin sensor related methods

    #region Accelerometer

    private void btnToggleAccelerometer_Clicked(object sender, EventArgs e)
    {
        ToggleAccelerometer();
    }

    public void ToggleAccelerometer()
    {
        if (Accelerometer.Default.IsSupported)
        {
            if (!Accelerometer.Default.IsMonitoring)
            {
                Accelerometer.Default.ReadingChanged += Accelerometer_ReadingChanged;
                Accelerometer.Default.Start(SensorSpeed.UI);
            }
            else
            {
                Accelerometer.Default.Stop();
                Accelerometer.Default.ReadingChanged -= Accelerometer_ReadingChanged;
            }
        }
    }

    private void Accelerometer_ReadingChanged(object sender, AccelerometerChangedEventArgs e)
    {
        accelX.Text = e.Reading.Acceleration.X.ToString("N2");
        accelY.Text = e.Reading.Acceleration.Y.ToString("N2");
        accelZ.Text = e.Reading.Acceleration.Z.ToString("N2");
    }

    #endregion

    #region Compass

    private void btnToggleCompass_Clicked(object sender, EventArgs e)
    {
        ToggleCompass();
    }

    private void ToggleCompass()
    {
        if (Compass.Default.IsSupported)
        {
            if (!Compass.Default.IsMonitoring)
            {
                Compass.Default.ReadingChanged += Compass_ReadingChanged;
                Compass.Default.Start(SensorSpeed.UI);
            }
            else
            {
                Compass.Default.Stop();
                Compass.Default.ReadingChanged -= Compass_ReadingChanged;
            }
        }
    }

    private void Compass_ReadingChanged(object sender, CompassChangedEventArgs e)
    {
        compassDegrees.Text = e.Reading.HeadingMagneticNorth.ToString("N2");
    }

    #endregion

    #region Gyroscope

    private void btnToggleGyroscope_Clicked(object sender, EventArgs e)
    {
        ToggleGyroscope();
    }

    private void ToggleGyroscope()
    {
        if (Gyroscope.Default.IsSupported)
        {
            if (!Gyroscope.Default.IsMonitoring)
            {
                Gyroscope.Default.ReadingChanged += Gyroscope_ReadingChanged;
                Gyroscope.Default.Start(SensorSpeed.UI);
            }
            else
            {
                Gyroscope.Default.Stop();
                Gyroscope.Default.ReadingChanged -= Gyroscope_ReadingChanged;
            }
        }
    }

    private void Gyroscope_ReadingChanged(object sender, GyroscopeChangedEventArgs e)
    {
        gyroX.Text = e.Reading.AngularVelocity.X.ToString("N2");
        gyroY.Text = e.Reading.AngularVelocity.Y.ToString("N2");
        gyroZ.Text = e.Reading.AngularVelocity.Z.ToString("N2");
    }

    #endregion

    #region Magnetometer

    private void btnToggleMagnetometer_Clicked(object sender, EventArgs e)
    {
        ToggleMagnetometer();
    }

    private void ToggleMagnetometer()
    {
        if (Magnetometer.Default.IsSupported)
        {
            if (!Magnetometer.Default.IsMonitoring)
            {
                Magnetometer.Default.ReadingChanged += Magnetometer_ReadingChanged;
                Magnetometer.Default.Start(SensorSpeed.UI);
            }
            else
            {
                Magnetometer.Default.Stop();
                Magnetometer.Default.ReadingChanged -= Magnetometer_ReadingChanged;
            }
        }
    }

    private void Magnetometer_ReadingChanged(object sender, MagnetometerChangedEventArgs e)
    {
        magnetX.Text = e.Reading.MagneticField.X.ToString("N2");
        magnetY.Text = e.Reading.MagneticField.Y.ToString("N2");
        magnetZ.Text = e.Reading.MagneticField.Z.ToString("N2");
    }

    #endregion

    #region Orientation

    private void btnToggleOrientation_Clicked(object sender, EventArgs e)
    {
        ToggleOrientation();
    }

    private void ToggleOrientation()
    {
        if (OrientationSensor.Default.IsSupported)
        {
            if (!OrientationSensor.Default.IsMonitoring)
            {
                // Turn on compass
                OrientationSensor.Default.ReadingChanged += Orientation_ReadingChanged;
                OrientationSensor.Default.Start(SensorSpeed.UI);
            }
            else
            {
                // Turn off compass
                OrientationSensor.Default.Stop();
                OrientationSensor.Default.ReadingChanged -= Orientation_ReadingChanged;
            }
        }
    }

    private void Orientation_ReadingChanged(object sender, OrientationSensorChangedEventArgs e)
    {
        orientationZ.Text = e.Reading.Orientation.Z.ToString("N2");
        orientationX.Text = e.Reading.Orientation.X.ToString("N2");
        orientationY.Text = e.Reading.Orientation.Y.ToString("N2");
        orientationW.Text = e.Reading.Orientation.W.ToString("N2");
    }

    #endregion

    #region Location

    private async void btnRequestLocation_Clicked(object sender, EventArgs e)
    {
        await GetCurrentLocation();
    }

    private CancellationTokenSource cancelTokenSource;
    private bool isCheckingLocation;

    public async Task GetCurrentLocation()
    {
        try
        {
            isCheckingLocation = true;
            GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
            cancelTokenSource = new CancellationTokenSource();
            Location location = await Geolocation.Default.GetLocationAsync(request, cancelTokenSource.Token);

            if (location != null)
            {
                System.Diagnostics.Debug.WriteLine($"{DateTime.Now}: Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
                locationLat.Text = location.Latitude.ToString();
                locationLng.Text = location.Longitude.ToString();
            }
        }
        // Catch one of the following exceptions:
        //   FeatureNotSupportedException
        //   FeatureNotEnabledException
        //   PermissionException
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Unable to get location", "OK");
        }
        finally
        {
            isCheckingLocation = false;
        }
    }

    public void CancelRequest()
    {
        if (isCheckingLocation && cancelTokenSource != null && cancelTokenSource.IsCancellationRequested == false)
        {
            cancelTokenSource.Cancel();
        }
    }

    #endregion

    #endregion
}
