using SQLite;

namespace MauiApp1.Models
{
    public class Worker
    {
        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }
        public string Alias { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public ushort Port { get; set; }
        public string Scheme { get; set; }
        public string Hostname { get; set; }
        public DateTime Created { get; set; }
    }
}
