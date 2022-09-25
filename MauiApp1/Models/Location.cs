using SQLite;

namespace MauiApp1.Models
{
    public class Location
    {
        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double? Altitude { get; set; }
        public double? Accuracy { get; set; }
        public double? Course { get; set; }
        public double? Speed { get; set; }
        public double? VerticalAccuracy { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public DateTime Created { get; set; }
    }
}
