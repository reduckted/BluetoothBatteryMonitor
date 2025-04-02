// Ignore Spelling: DEVICEINTERFACE HCMNOTIFICATION

#pragma warning disable IDE1006 // Naming Styles

using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;

namespace BluetoothBatteryMonitor;

internal static partial class NativeMethods {

    public const int CR_SUCCESS = 0x00000000;
    public const int CM_NOTIFY_FILTER_FLAG_ALL_INTERFACE_CLASSES = 1;
    public const int CM_NOTIFY_FILTER_TYPE_DEVICEINTERFACE = 0;


    public delegate int CM_NOTIFY_CALLBACK(
        IntPtr hNotify,
        IntPtr Context,
        int Action,
        IntPtr EventData,
        int EventDataSize
    );


    [LibraryImport("cfgmgr32.dll")]
    public static partial int CM_Register_Notification(
        in CM_NOTIFY_FILTER pFilter,
        IntPtr pContext,
        CM_NOTIFY_CALLBACK pCallback,
        out HCMNOTIFICATION pNotifyContext
    );


    [LibraryImport("cfgmgr32.dll")]
    private static partial int CM_Unregister_Notification(IntPtr NotifyContext);


    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct CM_NOTIFY_FILTER {

        private const int MAX_DEVICE_ID_LEN = 200;

        [FieldOffset(0)]
        public int cbSize;


        [FieldOffset(4)]
        public int Flags;


        [FieldOffset(8)]
        public int FilterType;


        [FieldOffset(12)]
        public uint Reserved;


        [FieldOffset(16)]
        public fixed char InstanceId[MAX_DEVICE_ID_LEN];


        [FieldOffset(16)]
        public IntPtr hTarget;


        [FieldOffset(16)]
        public Guid ClassGuid;

    }


    public class HCMNOTIFICATION : SafeHandleZeroOrMinusOneIsInvalid {

        public HCMNOTIFICATION() : base(true) { }


        protected override bool ReleaseHandle() {
            return CM_Unregister_Notification(handle) == CR_SUCCESS;
        }

    }

}
