namespace IpcUno
{
    public static class AppBuilderExtensions
    {
        public static MauiAppBuilder UseMauiControls(this MauiAppBuilder builder) =>
            builder
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("IpcUno/Assets/Fonts/OpenSansRegular.ttf", "OpenSansRegular");
                    fonts.AddFont("IpcUno/Assets/Fonts/OpenSansSemibold.ttf", "OpenSansSemibold");
                });
    }
}