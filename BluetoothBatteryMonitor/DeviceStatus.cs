namespace BluetoothBatteryMonitor;

internal class DeviceStatus : IEquatable<DeviceStatus> {

    public DeviceStatus(int batteryLevel, bool connected) {
        BatteryLevel = batteryLevel;
        Connected = connected;
    }


    public int BatteryLevel { get; }


    public bool Connected { get; }


    public override int GetHashCode() {
        return HashCode.Combine(BatteryLevel, Connected);
    }


    public override bool Equals(object? obj) {
        return base.Equals(obj as DeviceStatus);
    }


    public bool Equals(DeviceStatus? other) {
        return (other is not null) &&
            BatteryLevel == other.BatteryLevel &&
            Connected == other.Connected;
    }

}
