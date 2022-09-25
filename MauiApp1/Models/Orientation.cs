using SQLite;

namespace MauiApp1.Models
{
    public class Orientation
    {
        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float W { get; set; }
        public DateTime Created { get; set; }
    }
}
