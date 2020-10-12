using Android.App;
using Android.App.Admin;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using Kara.Helpers;
using System.Threading.Tasks;

namespace Kara.Droid
{
    [BroadcastReceiver(Permission = "android.permission.BIND_DEVICE_ADMIN")]
    [MetaData("android.app.device_admin", Resource = "@xml/device_admin")]
    [IntentFilter(new[] { "android.app.action.DEVICE_ADMIN_ENABLED", Intent.ActionMain })]
    public class DeviceAdmin : DeviceAdminReceiver, IJavaObject
    {
        public override void OnEnabled(Context context, Intent intent)
        {
            base.OnEnabled(context, intent);
            MainActivity.InitializeSharedResources(context, context.ContentResolver);
            //App.MajorDeviceSetting.MajorDeviceSettingsChanged(ChangedMajorDeviceSetting.DeviceAdminEnabled);
        }

        public override void OnDisabled(Context context, Intent intent)
        {
            base.OnDisabled(context, intent);
            MainActivity.InitializeSharedResources(context, context.ContentResolver);
            //App.MajorDeviceSetting.MajorDeviceSettingsChanged(ChangedMajorDeviceSetting.DeviceAdminDisabled);
        }
    }
}