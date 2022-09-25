namespace MauiApp1.Services
{
    public interface ILocationService
    {
        void StartService();
        void StopService();
        bool IsServiceRunning();
    }
}
