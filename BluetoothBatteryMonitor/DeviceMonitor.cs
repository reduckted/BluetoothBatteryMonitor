using System.Runtime.InteropServices;

namespace BluetoothBatteryMonitor;

internal class DeviceMonitor : IDisposable {

    private static readonly TimeSpan PollInterval = TimeSpan.FromMinutes(1);


    private readonly Settings _settings;
    private readonly CancellationTokenSource _cancellation;
    private readonly ManualResetEventSlim _gate;
    private readonly Lock _lock;
    private readonly Task _task;
    private DeviceStatus? _status;
    private NativeMethods.HCMNOTIFICATION? _notificationContext;


    public DeviceMonitor(Settings settings) {
        _settings = settings;

        _cancellation = new CancellationTokenSource();
        _gate = new ManualResetEventSlim();
        _lock = new Lock();

        _settings.Saved += OnSettingsSaved;

        _task = Task.Factory.StartNew(Monitor, TaskCreationOptions.LongRunning).Unwrap();

        NativeMethods.CM_Register_Notification(
            new NativeMethods.CM_NOTIFY_FILTER() {
                Flags = NativeMethods.CM_NOTIFY_FILTER_FLAG_ALL_INTERFACE_CLASSES,
                FilterType = NativeMethods.CM_NOTIFY_FILTER_TYPE_DEVICEINTERFACE,
                cbSize = Marshal.SizeOf<NativeMethods.CM_NOTIFY_FILTER>(),
            },
            IntPtr.Zero,
            OnDeviceChange,
            out _notificationContext
        );
    }


    private int OnDeviceChange(IntPtr hNotify, IntPtr Context, int Action, IntPtr EventDataPtr, int EventDataSize) {
        _gate.Set();
        return 0;
    }


    private async Task Monitor() {
        while (!_cancellation.IsCancellationRequested) {
            DeviceStatus? newStatus;
            bool changed;


            _gate.Reset();

            try {
                newStatus = WmiProvider.GetDeviceStatus(_settings.DeviceName);

            } catch (Exception) {
                newStatus = null;
            }

            lock (_lock) {
                if (_status is null) {
                    changed = newStatus is not null;
                } else {
                    changed = !_status.Equals(newStatus);
                }

                if (changed) {
                    _status = newStatus;
                }
            }

            if (changed) {
                Changed?.Invoke(this, EventArgs.Empty);
            }

            try {
                if (_gate.Wait(PollInterval, _cancellation.Token)) {
                    // The gate is set when a device changes, and when the settings are saved.
                    // Settings will be saved infrequently, so we won't worry about that situation.
                    // Devices might change often, and when we receive one notification, we will
                    // often receive a couple more at the same time. Rather than immediately
                    // updating the status when a device changes, we'll pause for a second so that
                    // we can avoid repeatedly updating the status in a very short period of time.
                    await Task.Delay(1000, _cancellation.Token);
                }

            } catch (OperationCanceledException) { }
        }
    }


    public DeviceStatus? GetCurrentStatus() {
        lock (_lock) {
            return _status;
        }
    }


    private void OnSettingsSaved(object? sender, EventArgs e) {
        _gate.Set();
    }


    public event EventHandler? Changed;


    public void Dispose() {
        _notificationContext?.Close();
        _notificationContext = null;

        _cancellation.Cancel();
        _task.Wait();

        _gate.Dispose();
        _cancellation.Dispose();
    }

}
