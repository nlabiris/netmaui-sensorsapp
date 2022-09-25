using SQLite;

namespace MauiApp1.Models
{
    public class Gyroscope
    {
        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public DateTime Created { get; set; }
    }
}
