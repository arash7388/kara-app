using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kara.Assets;
using Xamarin.Forms;
using static Kara.Assets.Connectivity;

namespace Kara.Helpers
{
    public enum ToastMessageDuration
    {
        Short,
        Long
    }
    public interface IToastMessage
    {
        void ShowMessage(string Message, ToastMessageDuration Duration);
    }
    
    public interface IPersianDateConverter
    {
        string PersianDate(DateTime date);
        DateTime JoulianDate(string date);
    }

    public delegate void DownloadFileCompleted();
    public delegate void DownloadProgressChanged(int ProgressPercentage);
    public interface IDownloader
    {
        Task<ResultSuccess> DownloadFile(string ServerPath, string FileName, string SavePath, string SaveAsFileName, DownloadFileCompleted OnDownloadFileCompleted, DownloadProgressChanged OnDownloadProgressChanged);
    }

    public delegate void UploadFileCompleted();
    public delegate void UploadProgressChanged(int ProgressPercentage);
    public interface IUploader
    {
        Task<ResultSuccess> UploadFile(string UploadURL, string File, string FileType, UploadFileCompleted OnUploadFileCompleted, UploadProgressChanged OnUploadProgressChanged);
    }

    public interface IFile
    {
        ResultSuccess Copy(string sourceFileName, string destFileName);
        ResultSuccess Delete(string fileName);
    }

    public enum EncodingType
    {
        ASCII = 1,
        BigEndianUnicode = 2,
        Unicode = 3,
        UTF32 = 4,
        UTF7 = 5,
        UTF8 = 6
    }
    public class BluetoothDeviceModel
    {
        public string MACID { get; set; }
        public string Name { get; set; }
        public string Class { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }
    public interface IBluetoothPrinter
    {
        Task<ResultSuccess> PrintBitmap(object Bitmap, string PrinterMACID);

        Task<ResultSuccess<List<BluetoothDeviceModel>>> GetDevicesNameAndMACID();
    }

    public interface IInternetDate
    {
        DateTime? GetTime();
    }

    public interface IKaraTimeProvider
    {
        DateTime? GetTime();
    }

    public class QRScanResult
    {
        public bool success { get; set; }
        public string contents { get; set; }
        public string format { get; set; }
    }
    public class IQRScan
    {
        public bool HasScannerApplication;
        public virtual void StartScan() { }
        public Action<QRScanResult> OnScanResult { get; set; }
    }

    public interface IPersianDatePicker
    {
        void ShowDatePicker();
    }

    public enum SettingDialougeLauncherRequestCode
    {
        DateTime = 1864,
        GPSSetting = 4653,
        DeviceAdminSetting = 9831,
        GPSPermission = 6322,
        InternetConnection = 9652
    }
    public enum ChangedMajorDeviceSetting
    {
        InitialStartup = 1,

        DeviceAdminEnabled = 11,
        DeviceAdminDisabled = 12,

        AutomaticTimeEnabled = 21,
        AutomaticTimeDisabled = 22,

        GPSEnabled = 31,
        GPSDisabled = 32,
        GPSPermissionGranted = 33,
        GPSPermissionDenied = 34,

        InternetConnected = 41,
        InternetDisconnected = 42
    }
    //public interface IMajorDeviceSetting
    //{
    //    bool CheckDeviceAdminSetting();
    //    bool CheckDateTimeSetting();
    //    bool CheckGPSSetting();
    //    bool CheckGPSPermission();
    //    Task<bool> CheckInternetConnection(bool WithoutTryingToConnect);

    //    void OpenDeviceAdminSetting(string Explanation);
    //    void OpenDateTimeSetting(string Explanation);
    //    void OpenGPSSetting(string Explanation);
    //    void OpenGPSPermissionRequest(string Explanation);
    //    void OpenInternetSetting(string Explanation);

    //    void MajorDeviceSettingsChanged(ChangedMajorDeviceSetting setting);
    //}


    public class ISubmittingLocationModel
    {
        public int Index { get; set; }
        public long TimeStamp { get; set; }
        public int State { get; set; }
        public double? Lat { get; set; }
        public double? Long { get; set; }
        public double? Accuracy { get; set; }
    }
    public class ILocationSubmitModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string CurrentVersionNumber { get; set; }
        public ISubmittingLocationModel[] Points { get; set; }
    }
    public interface ITCPClient
    {
        Task<ResultSuccess> SubmitLocations(string IP, int Port, ILocationSubmitModel submitData);
    }

    public interface IKaraVersion
    {
        string GetVersion();
    }
}