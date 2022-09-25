using SQLite;

namespace MauiApp1.Models
{
    public class Compass
    {
        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }
        public double HeadingMagneticNorthDegrees { get; set; }
        public DateTime Created { get; set; }
    }
}
