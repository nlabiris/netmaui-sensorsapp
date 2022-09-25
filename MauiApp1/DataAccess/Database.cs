using MauiApp1.Models;
using SQLite;
using Accelerometer = MauiApp1.Models.Accelerometer;
using Compass = MauiApp1.Models.Compass;
using Gyroscope = MauiApp1.Models.Gyroscope;
using Location = MauiApp1.Models.Location;
using Magnetometer = MauiApp1.Models.Magnetometer;
using Orientation = MauiApp1.Models.Orientation;

namespace MauiApp1.DataAccess
{
    public class Database
    {
        readonly SQLiteAsyncConnection database;

        public Database(string dbPath)
        {
            database = new SQLiteAsyncConnection(dbPath, false);
            database.CreateTableAsync<Worker>().Wait();
            database.CreateTableAsync<Accelerometer>().Wait();
            database.CreateTableAsync<Compass>().Wait();
            database.CreateTableAsync<Gyroscope>().Wait();
            database.CreateTableAsync<Location>().Wait();
            database.CreateTableAsync<Magnetometer>().Wait();
            database.CreateTableAsync<Orientation>().Wait();
        }

        public Task<Worker> GetWorkerByAliasAsync(string alias)
        {
            return database.Table<Worker>().FirstOrDefaultAsync(x => x.Alias == alias);
        }

        public Task<int> InsertWorkerAsync(Worker worker)
        {
            return database.InsertAsync(worker);
        }

        public Task<int> SaveAccelerometerAsync(Accelerometer accelerometer)
        {
            return database.InsertAsync(accelerometer);
        }

        public Task<int> SaveCompassAsync(Compass compass)
        {
            return database.InsertAsync(compass);
        }

        public Task<int> SaveGyroscopeAsync(Gyroscope gyroscope)
        {
            return database.InsertAsync(gyroscope);
        }

        public Task<int> SaveLocationAsync(Location location)
        {
            return database.InsertAsync(location);
        }

        public Task<int> SaveMagnetometerAsync(Magnetometer magnetometer)
        {
            return database.InsertAsync(magnetometer);
        }

        public Task<int> SaveOrientationAsync(Orientation orientation)
        {
            return database.InsertAsync(orientation);
        }
    }
}
