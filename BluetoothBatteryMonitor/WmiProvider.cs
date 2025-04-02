
using System.Collections;
using System.Management;

namespace BluetoothBatteryMonitor;

internal static class WmiProvider {

    private const string BluetoothClassName = "{e0cbf06c-cd8b-4647-bb8a-263b43f0f974}";
    private const string BatteryLevelKeyName = "{104EA319-6EE2-4701-BD47-8DDBF425BBE5} 2";
    private const string IsConnectedKeyName = "{83DA6326-97A6-4088-9453-A1923F573B29} 15";



    public static IReadOnlyCollection<string> GetBluetoothDevicesWithBatteryLevel() {
        List<string> devices;
        string query;


        devices = [];

        query =
            $"""
            SELECT Caption, DeviceID
            FROM Win32_PnPEntity
            WHERE ClassGuid = '{BluetoothClassName}'
            """;

        foreach (ManagementObject entity in ExecuteQuery(query)) {
            PropertyData? captionProperty;


            captionProperty = GetPropertyDataByName(entity.Properties, "Caption");

            if (captionProperty?.Value is string caption) {
                PropertyData? deviceProperties;


                deviceProperties = GetDeviceProperties(entity);

                if (deviceProperties is not null) {
                    if (GetBatteryLevel(deviceProperties).HasValue) {
                        devices.Add(caption);
                    }
                }
            }
        }

        return devices;
    }


    public static DeviceStatus? GetDeviceStatus(string deviceName) {
        string query;


        query = $"""
            SELECT Caption, DeviceID
            FROM Win32_PnPEntity
            WHERE Caption = '{Escape(deviceName)}'
            """;

        foreach (ManagementObject entity in ExecuteQuery(query)) {
            PropertyData? deviceProperties;


            deviceProperties = GetDeviceProperties(entity);

            if (deviceProperties is not null) {
                PropertyData? isConnectedProperty;


                isConnectedProperty = GetPropertyDataByKeyName(deviceProperties, IsConnectedKeyName);

                return new DeviceStatus(
                    GetBatteryLevel(deviceProperties) ?? 0,
                    (isConnectedProperty is not null) && Convert.ToBoolean(isConnectedProperty.Value)
                );
            }
        }

        return null;
    }


    private static PropertyData? GetDeviceProperties(ManagementObject entity) {
        ManagementBaseObject parameters;
        ManagementBaseObject result;


        parameters = entity.GetMethodParameters("GetDeviceProperties");
        result = entity.InvokeMethod("GetDeviceProperties", parameters, new InvokeMethodOptions());

        return GetPropertyDataByName(result.Properties, "deviceProperties");
    }


    private static int? GetBatteryLevel(PropertyData bluetoothDeviceProperties) {
        int? level;
        PropertyData? siblingsProperty;


        // Try getting the battery level from
        // the Bluetooth device's properties.
        level = GetBatteryLevelFromDeviceProperties(bluetoothDeviceProperties);

        if (level is not null) {
            return level;
        }

        // The Bluetooth device doesn't have a property for
        // the battery level, so look through its siblings.
        siblingsProperty = GetPropertyDataByKeyName(bluetoothDeviceProperties, "DEVPKEY_Device_Siblings");

        if (siblingsProperty?.Value is IEnumerable<string> siblings) {
            foreach (string sibling in siblings) {
                string query;


                query =
                    $"""
                    SELECT DeviceID
                    FROM Win32_PnPEntity
                    WHERE DeviceID = '{Escape(sibling)}'
                    """;

                foreach (ManagementObject entity in ExecuteQuery(query)) {
                    PropertyData? siblingDeviceProperties;


                    siblingDeviceProperties = GetDeviceProperties(entity);

                    if (siblingDeviceProperties is not null) {
                        level = GetBatteryLevelFromDeviceProperties(siblingDeviceProperties);

                        if (level is not null) {
                            return level;
                        }
                    }
                }
            }
        }

        return null;
    }


    private static int? GetBatteryLevelFromDeviceProperties(PropertyData deviceProperties) {
        PropertyData? batteryLevelProperty;


        batteryLevelProperty = GetPropertyDataByKeyName(deviceProperties, BatteryLevelKeyName);

        if (batteryLevelProperty is not null) {
            return Convert.ToInt32(batteryLevelProperty.Value);
        } else {
            return null;
        }
    }


    private static PropertyData? GetPropertyDataByKeyName(PropertyData deviceProperties, string propertyKeyName) {
        if (deviceProperties.Value is IEnumerable value) {
            foreach (var obj in value.OfType<ManagementBaseObject>()) {
                PropertyData? keyName;


                keyName = GetPropertyDataByName(obj.Properties, "KeyName");

                if (keyName?.Value is string keyNameValue && keyNameValue == propertyKeyName) {
                    return GetPropertyDataByName(obj.Properties, "Data");
                }
            }
        }

        return null;
    }


    private static PropertyData? GetPropertyDataByName(PropertyDataCollection properties, string name) {
        return properties.OfType<PropertyData>().FirstOrDefault((x) => x.Name == name);
    }


    private static IEnumerable<ManagementObject> ExecuteQuery(string query) {
        using (ManagementObjectSearcher searcher = new(query)) {
            using (ManagementObjectCollection results = searcher.Get()) {
                foreach (ManagementObject obj in results.OfType<ManagementObject>()) {
                    yield return obj;
                }
            }
        }
    }


    private static string Escape(string value) {
        return value.Replace("\\", "\\\\").Replace("'", "\\'");
    }

}
