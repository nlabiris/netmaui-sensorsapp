namespace MauiApp1.Services
{
    public interface ISensorsService
    {
        void StartService();
        void StopService();
        void UploadDb();
        bool IsServiceRunning();
    }
}
