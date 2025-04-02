namespace BluetoothBatteryMonitor;

internal static class Program {

    [STAThread]
    static void Main() {
        Settings settings;


        ApplicationConfiguration.Initialize();

        settings = new Settings();
        settings.Load();

        using (DeviceMonitor monitor = new(settings)) {
            using (StatusIcon icon = new(settings, monitor)) {
                icon.Exit += (_, _) => Application.Exit();

                Application.Run();
            }
        }
    }

}
