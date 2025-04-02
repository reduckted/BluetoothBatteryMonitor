using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Globalization;

namespace BluetoothBatteryMonitor;

internal sealed class StatusIcon : IDisposable {

    private const int UnknownLevel = -1;


    private enum IconKind {
        Disconnected,
        Connected,
        Warning
    }


    private static readonly Dictionary<(IconKind Kind, int Level), Icon> Icons = CreateIcons();


    private readonly Settings _settings;
    private readonly DeviceMonitor _monitor;
    private readonly NotifyIcon _icon;
    private readonly SynchronizationContext _synchronizationContext;
    private readonly EventHandler _onSettingsSaved;
    private readonly EventHandler _onMonitorChanged;
    private bool _hasWarned;


    public StatusIcon(Settings settings, DeviceMonitor monitor) {
        _settings = settings;
        _monitor = monitor;

        _icon = new NotifyIcon();

        _icon.MouseDoubleClick += OnMouseDoubleClick;

        _icon.ContextMenuStrip = new ContextMenuStrip();
        _icon.ContextMenuStrip.Items.Add(new ToolStripMenuItem("Settings", null, OnSettingsClick));
        _icon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
        _icon.ContextMenuStrip.Items.Add(new ToolStripMenuItem("Exit", null, OnExitClick));

        _synchronizationContext = SynchronizationContext.Current!;

        _onSettingsSaved = (_, _) => _synchronizationContext.Post((_) => Update(), null);
        _settings.Saved += _onSettingsSaved;

        _onMonitorChanged = (_, _) => _synchronizationContext.Post((_) => Update(), null);
        _monitor.Changed += _onMonitorChanged;

        Update();
        _icon.Visible = true;
    }


    private void Update() {
        DeviceStatus? status;
        int level;
        IconKind kind;
        string levelText;


        status = _monitor.GetCurrentStatus();

        if (status is not null) {
            level = status.BatteryLevel;

            if (status.Connected) {
                if (status.BatteryLevel <= _settings.WarningLevel) {
                    kind = IconKind.Warning;
                } else {
                    kind = IconKind.Connected;
                }

            } else {
                kind = IconKind.Disconnected;
            }

        } else {
            level = UnknownLevel;
            kind = IconKind.Disconnected;
        }

        if (level == UnknownLevel) {
            levelText = "Unknown Battery Level";
        } else {
            levelText = (level / 100.0).ToString("P0", CultureInfo.CurrentUICulture);
        }

        _icon.Text = _settings.DeviceName + Environment.NewLine + levelText;

        _icon.Icon =
            Icons.GetValueOrDefault((kind, level)) ??
            Icons.GetValueOrDefault((kind, UnknownLevel));

        if (kind == IconKind.Warning) {
            if (!_hasWarned) {
                _icon.ShowBalloonTip(0, "Low Batter Level", $"Battery level is {levelText}", ToolTipIcon.Warning);
                _hasWarned = true;
            }

        } else {
            _hasWarned = false;
        }
    }


    private void OnExitClick(object? sender, EventArgs e) {
        Exit?.Invoke(this, EventArgs.Empty);
    }


    private void OnSettingsClick(object? sender, EventArgs e) {
        ShowSettings();
    }


    private void OnMouseDoubleClick(object? sender, MouseEventArgs e) {
        ShowSettings();
    }


    private void ShowSettings() {
        using (SettingsForm form = new(_settings)) {
            form.ShowDialog();
        }
    }


    public event EventHandler? Exit;


    public void Dispose() {
        _settings.Saved -= _onSettingsSaved;
        _monitor.Changed -= _onMonitorChanged;

        _icon.Visible = false;
        _icon.Dispose();
    }


    private static Dictionary<(IconKind Kind, int Level), Icon> CreateIcons() {
        const string FontName = "Segoe UI";
        Dictionary<(IconKind Kind, int Level), Icon> icons;
        Rectangle dimensions;
        Size cornerRadius;


        dimensions = new(0, 0, 16, 16);
        cornerRadius = new Size(4, 4);

        icons = [];

        using (Font largeFont = new(FontName, 8, FontStyle.Bold)) {
            using (Font smallFont = new(FontName, 7, FontStyle.Bold)) {
                foreach (IconKind kind in Enum.GetValues<IconKind>()) {
                    Color backgroundColor;


                    backgroundColor = (kind) switch {
                        IconKind.Connected => Color.FromArgb(0, 130, 255),
                        IconKind.Warning => Color.FromArgb(240, 75, 90),
                        _ => Color.Gray
                    };

                    using (SolidBrush backgroundBrush = new(backgroundColor)) {
                        foreach (int level in Enumerable.Range(0, 101).Append(UnknownLevel)) {
                            using (Bitmap bitmap = new(dimensions.Width, dimensions.Height)) {
                                using (Graphics graphics = Graphics.FromImage(bitmap)) {
                                    string text;
                                    Font font;
                                    SizeF textSize;


                                    graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                                    if (level == UnknownLevel) {
                                        text = "?";
                                    } else {
                                        text = level.ToString(CultureInfo.InvariantCulture);
                                    }

                                    font = level == 100 ? smallFont : largeFont;
                                    textSize = graphics.MeasureString(text, font);

                                    // Turn off anti-aliasing while we draw the background.
                                    graphics.SmoothingMode = SmoothingMode.None;

                                    graphics.FillRoundedRectangle(backgroundBrush, dimensions, cornerRadius);

                                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                                    graphics.DrawString(
                                        text,
                                        font,
                                        Brushes.White,
                                        ((dimensions.Width - textSize.Width) / 2.0f) + 1,
                                        ((dimensions.Width - textSize.Height) / 2.0f) + 1
                                    );
                                }

                                icons[(kind, level)] = Icon.FromHandle(bitmap.GetHicon());
                            }
                        }
                    }
                }
            }
        }

        return icons;
    }

}
