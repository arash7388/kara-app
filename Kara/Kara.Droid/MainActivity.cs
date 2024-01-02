using Android.App;
using Android.App.Admin;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Java.Lang;
using Java.Lang.Reflect;
using Kara.Assets;
using Kara.CustomRenderer;
using Kara.Droid.Helpers;
using Kara.Helpers;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Exception = System.Exception;
using Xamarin.Essentials;
using System.Collections.Generic;
using Kara.Services;
using Android.Support.V4.App;
using Android;

namespace Kara.Droid
{
    [Activity(Label = "@string/ApplicationName", Icon = "@drawable/icon", Theme = "@style/MainTheme", NoHistory = false, MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public static string DeviceInformation;
        public DevicePolicyManager devicePolicyManager;
        public ComponentName myDeviceAdmin;
        public static Typeface IranSansFont;
        public static MainActivity MainActivityInstance;

        protected override void OnCreate(Bundle bundle)
        {
            try
            {

            
            AppCenter.Start("da02ffa0-e4a9-4292-8423-aee119dd51b4",typeof(Analytics), typeof(Crashes));

            DeviceInformation = $"Model :{DeviceInfo.Model}, VersionString:{DeviceInfo.VersionString},Platform:{DeviceInfo.Platform},Version:{DeviceInfo.Version} , Idiom:{DeviceInfo.Idiom}";
             
            MainActivityInstance = this;

            devicePolicyManager = (DevicePolicyManager)GetSystemService(Context.DevicePolicyService);
            myDeviceAdmin = new ComponentName(this, Java.Lang.Class.FromType(typeof(DeviceAdmin)));

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            //////////////////////////////////////////

            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledExceptionHandler;
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskExceptionHandler;

            //Forms.Init(this, bundle);
            //DisplayCrashReport();

            //////////////////////////////////////////

            IranSansFont = Typeface.CreateFromAsset(Assets, "IRANSansMobile.ttf");
            foreach (var font in new string[] { "DEFAULT", "MONOSPACE", "SERIF", "SANS_SERIF" })
                FontsOverride.SetDefaultFont(font, IranSansFont);

            Xamarin.FormsMaps.Init(this, bundle);

            Forms.Init(this, bundle);

            OxyPlot.Xamarin.Forms.Platform.Android.PlotViewRenderer.Init();

            App.imagesDirectory = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "StuffsGallery");

            InitializeSharedResources(this, ContentResolver);

            App.Downloader = new Downloader();
            App.QRScanner = new QRScan(this);
            App.Uploader = new Uploader();
            App.BluetoothPrinter = new BluetoothPrinter(this);
            PersianDatePicker.FragmentManager = FragmentManager;
            App.PersianDatePicker = new PersianDatePicker();

            App.DeviceSizeDensity = Resources.DisplayMetrics.Density;

            try
            {
                CreateMapIcons();
            }
            catch (Exception err)
            {
                Crashes.TrackError(err, GetDeviceInfoAsDic());

            }

            MessagingCenter.Subscribe<MainMenu>(this, "MainMenuOpened", StartService);

            LoadApplication(new App());

            Xamarin.Essentials.Platform.Init(this, bundle); // add this line to your code, it may also be called: bundle
            Analytics.TrackEvent("OnCreate", GetDeviceInfoAsDic());

            //if (!CheckPermissionGranted("READ_PHONE_STATE"))
            if (!CheckPermissionGranted(Manifest.Permission.ReadPhoneState))
            {
                RequestPhonePermission();
            }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void RequestPhonePermission()
        {
            //if (ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.ReadPhoneState))
            //{
            //    // Provide an additional rationale to the user if the permission was not granted
            //    // and the user would benefit from additional context for the use of the permission.
            //    // For example if the user has previously denied the permission.
                ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.ReadPhoneState }, 0);

            //}

        }

        public bool CheckPermissionGranted(string Permissions)
        {
            // Check if the permission is already available.
            if (ActivityCompat.CheckSelfPermission(this, Permissions) != Permission.Granted)
            {
                return false;
            }
            else
            {
                return true;
            }


        }
        private void StartService(MainMenu page)
        {
            KaraNewServiceLauncher.StartAndScheduleAlarmManagerForkaraNewService(this);
      }

        private static void TaskSchedulerOnUnobservedTaskExceptionHandler(object sender, UnobservedTaskExceptionEventArgs unobservedTaskExceptionEventArgs)
        {
            var newExc = new Exception("TaskSchedulerOnUnobservedTaskException", unobservedTaskExceptionEventArgs.Exception);
            LogUnhandledException(newExc);
        }

        private static void CurrentDomainOnUnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            try
            {
                var newExc = new Exception("CurrentDomainOnUnhandledException", unhandledExceptionEventArgs.ExceptionObject as Exception);
                LogUnhandledException(newExc);
            }
            catch(Exception e)
            {
                var data = new Dictionary<string, string>();
                data.Add("methodname", "exception occured in CurrentDomainOnUnhandledExceptionHandler");
                Crashes.TrackError(e, data);
            }
            
        }

        internal static void LogUnhandledException(Exception exception)
        {
            try
            {
                Crashes.TrackError(exception, GetDeviceInfoAsDic());
                const string errorFileName = "Fatal.log";
                var libraryPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal); // iOS: Environment.SpecialFolder.Resources
                var errorFilePath = System.IO.Path.Combine(libraryPath, errorFileName);
                var errorMessage = System.String.Format("Time: {0}\r\nUniversalLineInApp: {1}\r\nSpecialLog: {2}\r\nError: Unhandled Exception\r\n{3}",
                DateTime.Now, App.Last5UniversalLineInApp, App.SpecialLog, exception.ToString());
                if (!string.IsNullOrEmpty(Kara.OrderInsertForm.MultipleRecordsInAllStuffsData_Log))
                    errorMessage = "MultipleRecordsInAllStuffsData_Log: " + Kara.OrderInsertForm.MultipleRecordsInAllStuffsData_Log + ", errorMessage: " + errorMessage;
                System.IO.File.WriteAllText(errorFilePath, errorMessage);
            }
            catch
            {
                // just suppress any error logging exceptions
            }
        }

        private static Dictionary<string, string> GetDeviceInfoAsDic()
        {
            var deviceData = new Dictionary<string, string>();
            deviceData.Add("deviceInfo", DeviceInformation);
            return deviceData;
        }

        /// <summary>
        // If there is an unhandled exception, the exception information is diplayed
        // on screen the next time the app is started (only in debug configuration)
        /// </summary>
        //[Conditional("DEBUG")]
        //private void DisplayCrashReport()
        //{
        //    const string errorFilename = "Fatal.log";
        //    var libraryPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
        //    var errorFilePath = System.IO.Path.Combine(libraryPath, errorFilename);

        //    if (!System.IO.File.Exists(errorFilePath))
        //    {
        //        return;
        //    }

        //    var errorText = System.IO.File.ReadAllText(errorFilePath);
        //    new AlertDialog.Builder(this)
        //        .SetPositiveButton("ارسال به سرور", async (sender, args) =>
        //        {
        //            var SendResult = await Connectivity.SubmitExceptionsLog(errorText);
        //            if (SendResult.Success)
        //                System.IO.File.Delete(errorFilePath);
        //        })
        //        .SetNegativeButton("انصراف و حذف", (sender, args) =>
        //        {
        //            System.IO.File.Delete(errorFilePath);
        //        })
        //        .SetMessage("در اجرای قبلی نرم افزار خطایی رخ داده است. لطفا برای کمک به ما در بهبود کیفیت برنامه این موارد را برایمان ارسال کنید.")
        //        .SetTitle("گزارش خطا")
        //        .Show();
        //}


        public static void InitializeSharedResources(Context Context, ContentResolver ContentResolver)
        {
            App.KaraVersion = new KaraVersion();
            App.TCPClient = new TCPClient();
            App.DBFileName = GetLocalFilePath("karadb.db3");
            App.DB = new DBRepository(App.DBFileName);
            App.ToastMessageHandler = new ToastMessageHandler(Context);
            App.File = new Helpers.File();
            App.PersianDateConverter = new PersianDateConverter();
            //App.MajorDeviceSetting = new MajorDeviceSetting() { ContentResolver = ContentResolver, Context = Context };
        }

        private void CreateMapIcons()
        {
            CustomMapRenderer.MapIcons = new System.Collections.Generic.Dictionary<MapIcon, BitmapDescriptor>();

            var BlueBitmap = BitmapFactory.DecodeResource(Resources, Resource.Drawable.PinBlueMarker);
            var RedBitmap = BitmapFactory.DecodeResource(Resources, Resource.Drawable.PinRedMarker);
            var GreenBitmap = BitmapFactory.DecodeResource(Resources, Resource.Drawable.PinGreenMarker);

            var BlueBallon = BitmapDescriptorFactory.FromBitmap(BlueBitmap);
            var RedBallon = BitmapDescriptorFactory.FromBitmap(RedBitmap);
            var GreenBallon = BitmapDescriptorFactory.FromBitmap(GreenBitmap);

            CustomMapRenderer.MapIcons.Add(CustomRenderer.MapIcon.BlueBallon, BlueBallon);
            CustomMapRenderer.MapIcons.Add(CustomRenderer.MapIcon.RedBallon, RedBallon);
            CustomMapRenderer.MapIcons.Add(CustomRenderer.MapIcon.GreenBallon, GreenBallon);
        }

        private static string GetLocalFilePath(string filename)
        {
            string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            return System.IO.Path.Combine(path, filename);
        }

        protected override void OnStart()
        {
            base.OnStart();
        }

        protected override void OnPause()
        {
            base.OnPause();
        }

        protected override void OnResume()
        {
            base.OnResume();
        }

        protected override void OnStop()
        {
            base.OnStop();
        }

        public const int QRScanRequestCode = 0;//Max:65536

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (requestCode == QRScanRequestCode)
            {
                ((QRScan)App.QRScanner).OnActivityResult(resultCode, data);
            }
            else if (requestCode == (int)SettingDialougeLauncherRequestCode.DateTime ||
                requestCode == (int)SettingDialougeLauncherRequestCode.DeviceAdminSetting ||
                requestCode == (int)SettingDialougeLauncherRequestCode.GPSSetting ||
                requestCode == (int)SettingDialougeLauncherRequestCode.InternetConnection)
            {
                //CheckMajorSystemSettingsToBeTruelySet(requestCode);
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            //if (requestCode == (int)SettingDialougeLauncherRequestCode.GPSPermission)
            //{
            //    //CheckMajorSystemSettingsToBeTruelySet(requestCode);
            //}

            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void Terminate()
        {
            this.FinishAffinity();
            JavaSystem.Exit(0);
        }
       
    }


    public class FontsOverride
    {
        public static void SetDefaultFont(string staticTypefaceFieldName, Typeface InsteadFont)
        {
            ReplaceFont(staticTypefaceFieldName, InsteadFont);
        }
        protected static void ReplaceFont(string staticTypefaceFieldName, Typeface newTypeface)
        {
            try
            {
                Field staticField = ((Java.Lang.Object)(newTypeface)).Class.GetDeclaredField(staticTypefaceFieldName);
                staticField.Accessible = true;
                staticField.Set(null, newTypeface);
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }

    
}