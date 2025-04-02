namespace BluetoothBatteryMonitor;

internal partial class SettingsForm : Form {

    private readonly Settings _settings;


    public SettingsForm(Settings settings) {
        InitializeComponent();

        _settings = settings;

        DeviceNameComboBox.Text = _settings.DeviceName;
        WarningLevelUpDown.Value = _settings.WarningLevel;

        Task.Run(() => {
            IReadOnlyCollection<string> devices;


            devices = WmiProvider.GetBluetoothDevicesWithBatteryLevel();

            if (!IsDisposed) {
                BeginInvoke(() => {
                    if (!IsDisposed) {
                        foreach (string device in devices.Order()) {
                            DeviceNameComboBox.Items.Add(device);
                        }
                    }
                });
            }
        });
    }


    private void SaveButton_Click(object sender, EventArgs e) {
        _settings.DeviceName = DeviceNameComboBox.Text;
        _settings.WarningLevel = Convert.ToInt32(WarningLevelUpDown.Value);
        _settings.Save();

        DialogResult = DialogResult.OK;
        Close();
    }


    private void DismissButton_Click(object sender, EventArgs e) {
        DialogResult = DialogResult.Cancel;
        Close();
    }

}
