using Mapsui.Tiling;
using MauiApp1.Services;
using Mapsui.Projections;
using Mapsui.UI.Maui;
using Mapsui;
using Mapsui.Extensions;
using Map = Mapsui.Map;

namespace MauiApp1
{
    public partial class MapPage : ContentPage
    {
        private readonly ILocationService locationService;

        public MapPage(ILocationService locationService)
        {
            this.locationService = locationService;

            InitializeComponent();
        }

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

        private void UpdateLocationOnMap(Position position)
        {
            var pin = new Pin(mapView)
            {
                Label = "My pin",
                Position = position,
                Type = PinType.Pin,
                Color = new Color(128, 128, 128),
                Scale = 1,
                RotateWithMap = true
            };
            mapView.Pins.Clear();
            mapView.Pins.Add(pin);

            var point = new MPoint(position.Longitude, position.Latitude);
            // OSM uses spherical mercator coordinates. So transform the lon lat coordinates to spherical mercator
            var sphericalMercatorCoordinate = SphericalMercator.FromLonLat(point.X, point.Y).ToMPoint();
            mapView.Navigator.NavigateTo(sphericalMercatorCoordinate, mapView.Map.Resolutions[15]);
            var layer = mapView.Map.Layers.FindLayer("OpenStreetMap").First();
            layer.DataHasChanged();
        }

        protected override void OnAppearing()
        {
            MessagingCenter.Subscribe<string>(this, "OnLocationChanged", (location) => {
                var coordinates = location.Split(';');
                double.TryParse(coordinates[0], out double latitude);
                double.TryParse(coordinates[1], out double longitude);
                UpdateLocationOnMap(new Position(latitude, longitude));
            });
 
            var pin = new Pin(mapView)
            {
                Label = "My pin",
                Position = new Position(38.048517, 23.7989813),
                Type = PinType.Pin,
                Color = new Color(128, 128, 128),
                Scale = 1,
                RotateWithMap = true
            };
            var point = new MPoint(23.7989813, 38.048517);
            // OSM uses spherical mercator coordinates. So transform the lon lat coordinates to spherical mercator
            var sphericalMercatorCoordinate = SphericalMercator.FromLonLat(point.X, point.Y).ToMPoint();
            mapView.Map.Layers.Add(OpenStreetMap.CreateTileLayer());
            mapView.Pins.Add(pin);
            // Set the center of the viewport to the coordinate. The UI will refresh automatically
            // Additionally you might want to set the resolution, this could depend on your specific purpose
            mapView.Map.Home = n => n.NavigateTo(sphericalMercatorCoordinate, mapView.Map.Resolutions[15]);
        }

        protected override void OnDisappearing()
        {
            mapView.Map.Layers.Clear();
            mapView.Pins.Clear();
        }
    }
}