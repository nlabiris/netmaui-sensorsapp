using MauiApp1.Services;

namespace MauiApp1;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

        builder.Services.AddSingleton<ISensorsService, SensorsService>();
        builder.Services.AddSingleton<ILocationService, LocationService>();
        builder.Services.AddTransient<MainPage>();
        return builder.Build();
	}
}
