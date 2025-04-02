using System.Text.Json;

namespace BluetoothBatteryMonitor;

internal class Settings {

    public string DeviceName { get; set; } = "";


    public int WarningLevel { get; set; } = 25;


    public void Load() {
        try {
            string fileName;
            SerializableSettings? settings;


            fileName = GetFileName();

            settings = JsonSerializer.Deserialize<SerializableSettings>(
                File.ReadAllText(fileName)
            );

            if (settings is not null) {
                DeviceName = settings.DeviceName ?? "";
                WarningLevel = Math.Max(Math.Min(100, settings.WarningLevel), 0);
            }

        } catch (JsonException) {
        } catch (IOException) { }
    }


    public void Save() {
        string fileName;


        fileName = GetFileName();
        Directory.CreateDirectory(Path.GetDirectoryName(fileName)!);

        File.WriteAllText(
            fileName,
            JsonSerializer.Serialize(
                new SerializableSettings {
                    DeviceName = DeviceName,
                    WarningLevel = WarningLevel
                }
            )
        );

        Saved?.Invoke(this, EventArgs.Empty);
    }


    private static string GetFileName() {
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "BluetoothBatteryMonitor",
            "settings.json"
        );
    }


    public event EventHandler? Saved;


    private class SerializableSettings {

        public string? DeviceName { get; set; }


        public int WarningLevel { get; set; }

    }

}
