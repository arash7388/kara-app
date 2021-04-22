using Android;
using Android.Content;
using Android.Content.PM;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Kara.Droid;
using Kara.Services;
using System;
using Xamarin.Forms;
using Android.Provider;

[assembly: Xamarin.Forms.Dependency(typeof(UniqueIdAndroid))]
namespace Kara.Droid
{
    public class UniqueIdAndroid : IDevice
    {
        Android.Telephony.TelephonyManager mTelephonyMgr;

        public string GetIdentifier()
        {
            //mTelephonyMgr = (Android.Telephony.TelephonyManager)Forms.Context.GetSystemService(Context.TelephonyService);
            //return mTelephonyMgr.DeviceId;
            return Settings.Secure.GetString(Forms.Context.ContentResolver, Settings.Secure.AndroidId);
        }

        public bool PhonePermissionGranted()
        {
            return MainActivity.MainActivityInstance.CheckPermissionGranted(Manifest.Permission.ReadPhoneState);
        }
    }
}