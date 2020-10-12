using System;
using Android.Content;
using Android.Widget;
using Kara.Helpers;
using System.Net;
using System.IO;
using System.Threading.Tasks;
using System.Reflection;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Renderscripts;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Kara.Assets;
using System.Globalization;
using Android.Bluetooth;
using Java.Util;
using System.Collections.Generic;
using System.Linq;
using Java.IO;
using Java.Nio;
using System.Net.Sockets;
using Android.App;
using Android.App.Admin;
using Android.Locations;
using Kara.Models;
using Android.Util;
using Java.Net;
using ProtoBuf;
using static Kara.Assets.Connectivity;
using Newtonsoft.Json;
using System.Net.NetworkInformation;
using ZpCPCLSDK.ZpCPCLSDK;

namespace Kara.Droid.Helpers
{
    public class ToastMessageHandler : IToastMessage
    {
        public Context Context;
        public ToastMessageHandler(Context Context)
        {
            this.Context = Context;
        }
        public void ShowMessage(string ShowingMessage, ToastMessageDuration Duration)
        {
            try
            {
                Toast.MakeText(Context, ShowingMessage.Replace("<br />", "\n").Replace("<br/>", "\n"), Duration == ToastMessageDuration.Short ? ToastLength.Short : ToastLength.Long).Show();
            }
            catch (System.Exception err)
            {
                var x = 0;
            }
        }
    }
    
    public class PersianDateConverter : IPersianDateConverter
    {
        public DateTime JoulianDate(string date)
        {
            var pc = new PersianCalendar();
            var Year = Convert.ToInt32(date.Substring(0, 4));
            var Month = Convert.ToInt32(date.Substring(5, 2));
            var Day = Convert.ToInt32(date.Substring(8, 2));
            return pc.ToDateTime(Year, Month, Day, 0, 0, 0, 0);
        }

        public string PersianDate(DateTime dateTime)
        {
            var pc = new PersianCalendar();
            return pc.GetYear(dateTime).ToString() + "/" + pc.GetMonth(dateTime).ToString().PadLeft(2, '0') + "/" + pc.GetDayOfMonth(dateTime).ToString().PadLeft(2, '0');
        }
    }

    public class Downloader : IDownloader
    {
        DownloadFileCompleted DownloadFileCompleted_EventHandler;
        DownloadProgressChanged DownloadProgressChanged_EventHandler;
        
        private void Client_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (DownloadFileCompleted_EventHandler != null)
                DownloadFileCompleted_EventHandler();
        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (DownloadProgressChanged_EventHandler != null)
                DownloadProgressChanged_EventHandler(e.ProgressPercentage);
        }

        public async Task<ResultSuccess> DownloadFile(string ServerPath, string FileName, string SavePath, string SaveAsFileName, DownloadFileCompleted OnDownloadFileCompleted, DownloadProgressChanged OnDownloadProgressChanged)
        {
            var webClient = new WebClient();

            webClient.DownloadFileCompleted += Client_DownloadFileCompleted;
            webClient.DownloadProgressChanged += Client_DownloadProgressChanged;

            try
            {
                DownloadFileCompleted_EventHandler = OnDownloadFileCompleted;
                DownloadProgressChanged_EventHandler = OnDownloadProgressChanged;
                
                var url = new Uri(ServerPath + FileName);
                var localDirectory = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), SavePath);
                var localSavePath = System.IO.Path.Combine(localDirectory, !string.IsNullOrEmpty(SaveAsFileName) ? SaveAsFileName : FileName);
                
                if (!System.IO.Directory.Exists(localDirectory))
                    System.IO.Directory.CreateDirectory(localDirectory);
                
                var task = webClient.DownloadFileTaskAsync(url, localSavePath);
                await task;
                if (task.Exception != null)
                    throw new Exception(task.Exception.Message);

                webClient.DownloadFileCompleted += Client_DownloadFileCompleted;
                webClient.DownloadProgressChanged += Client_DownloadProgressChanged;
                return new ResultSuccess(true, "");
            }
            catch (System.Exception err)
            {
                webClient.DownloadFileCompleted += Client_DownloadFileCompleted;
                webClient.DownloadProgressChanged += Client_DownloadProgressChanged;
                return new ResultSuccess(false, err.Message);
            }
        }
    }

    public class Uploader : IUploader
    {
        UploadFileCompleted UploadFileCompleted_EventHandler;
        UploadProgressChanged UploadProgressChanged_EventHandler;

        private void Client_UploadFileCompleted(object sender, UploadFileCompletedEventArgs e)
        {
            if (UploadFileCompleted_EventHandler != null)
                UploadFileCompleted_EventHandler();
        }

        private void Client_UploadProgressChanged(object sender, UploadProgressChangedEventArgs e)
        {
            if (UploadProgressChanged_EventHandler != null)
                UploadProgressChanged_EventHandler(e.ProgressPercentage);
        }

        public async Task<ResultSuccess> UploadFile(string UploadURL, string File, string FileType, UploadFileCompleted OnUploadFileCompleted, UploadProgressChanged OnUploadProgressChanged)
        {
            try
            {
                UploadFileCompleted_EventHandler = OnUploadFileCompleted;
                UploadProgressChanged_EventHandler = OnUploadProgressChanged;

                System.Net.WebClient client = new WebClient();

                client.UploadProgressChanged += Client_UploadProgressChanged;
                client.UploadFileCompleted += Client_UploadFileCompleted;

                client.Headers["Authorization"] = "Basic ";
                //client.Headers.Add("Content-Type", FileType);
                await client.UploadFileTaskAsync(UploadURL, "POST", File);

                client.UploadProgressChanged -= Client_UploadProgressChanged;
                client.UploadFileCompleted -= Client_UploadFileCompleted;

                return new ResultSuccess();
            }
            catch (Exception err)
            {
                return new ResultSuccess(false, err.ProperMessage());
            }
        }
    }

    public class File : IFile
    {
        public ResultSuccess Copy(string sourceFileName, string destFileName)
        {
            try
            {
                System.IO.File.Copy(sourceFileName, destFileName, true);
                return new ResultSuccess();
            }
            catch (Exception err)
            {
                return new ResultSuccess(false, err.ProperMessage());
            }
        }

        public ResultSuccess Delete(string fileName)
        {
            try
            {
                System.IO.File.Delete(fileName);
                return new ResultSuccess();
            }
            catch (Exception err)
            {
                return new ResultSuccess(false, err.ProperMessage());
            }
        }
    }

    public class BluetoothPrinter : IBluetoothPrinter
    {
        BluetoothAdapter mBluetoothAdapter = null;
        MainActivity mainActivity;
        BluetoothDevice printerDevice;
        BluetoothSocket printerSocket;
        Stream printerOutputStream, printerInputStream;
        System.Text.Encoding Encoding;
        bool Printing = false;

        public BluetoothPrinter(MainActivity mainActivity)
        {
            this.mainActivity = mainActivity;
        }
        
        public async Task<ResultSuccess> PrintBitmap(object Bitmap, string PrinterMACID)
        {
            try
            {
                while (Printing)
                    await Task.Delay(100);

                Printing = true;

                var findResult = await findBT(PrinterMACID);
                if (!findResult.Success)
                    throw new Exception(findResult.Message);
                
                var openResult = openBT();
                if (!openResult.Success)
                    throw new Exception(openResult.Message);

                //DrawBitmap((Bitmap)Bitmap, 0, 0, PrinterMACID);

                var result = await sendDataBitmap((Bitmap)Bitmap);

                Printing = false;
                if (!result.Success)
                {
                    if(result.Message.ToLower().Contains("Broken Pipe".ToLower()))
                    {
                        printerDevice = null;
                        printerSocket = null;
                        return await PrintBitmap(Bitmap, PrinterMACID);
                    }
                    throw new Exception(result.Message);
                }
                else
                    return new ResultSuccess(true, "");
            }
            catch (Exception err)
            {
                Printing = false;
                return new ResultSuccess(false, err.ProperMessage());
            }
        }

        public void DrawBitmap(Bitmap bp, int x, int y,string PrinterMACID)
        {
            int[] PixelData = new int[bp.Width * bp.Height];

            int i = 0;
            int j = 0;
            int ndata = 0; //line data count
            int len = (bp.Width + 7) / 8;  //one line data length
            byte[] data = new byte[len * bp.Height];
            //-----Clear Data--------
            for (i = 0; i < data.Length; i++)
            {
                data[i] = 0;
            }

            //-----Get bitmap Pixel data-----------
            i = 0;
            for (int row = 0; row < bp.Height; row++)
            {
                for (int col = 0; col < bp.Width; col++)
                {
                    Android.Graphics.Color c = new Android.Graphics.Color(bp.GetPixel(col, row));
                    int color = 0;
                    color += c.R << 16;
                    color += c.G << 8;
                    color += c.B;
                    PixelData[i++] = color;
                }
            }

            //----Get Cmd Data---------------
            for (i = 0; i < bp.Height; i++)
            {
                for (j = 0; j < len; j++)
                {
                    data[ndata + j] = 0;
                }
                for (j = 0; j < bp.Width; j++)
                {
                    int color = PixelData[i * bp.Width + j];
                    int b = (color >> 0) & 0xff;
                    int g = (color >> 8) & 0xff;
                    int r = (color >> 16) & 0xff;
                    int grey = (r + g + b) / 3;
                    //if( grey <12 )
                    if (grey < 153) //153
                        data[ndata + j / 8] |= (byte)(0x80 >> (j % 8));
                }
                ndata += len;
            }

            //-----Set Cmd-----------
            string cmd = "\nCG " + len.ToString() + " " + bp.Height.ToString() + " " + x.ToString() + " " + y.ToString() + " ";
                        
            Zp_cpcl_BluetoothPrinter zpSDK = new ZpCPCLSDK.ZpCPCLSDK.Zp_cpcl_BluetoothPrinter(Forms.Context);

            zpSDK.Connect(PrinterMACID);
            zpSDK.PageSetup(570, 2 * 30 + 200);
            zpSDK.Write(System.Text.Encoding.Default.GetBytes(cmd));
            zpSDK.Write(data);
            zpSDK.Write(System.Text.Encoding.Default.GetBytes("\n"));
        }

        async Task<ResultSuccess> sendDataBitmap(Bitmap Data)
        {
            try
            {
                var width = Data.Width;
                var height = Data.Height;

                Bitmap bmpMonochrome = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888);
                Canvas canvas = new Canvas(bmpMonochrome);
                canvas.DrawBitmap(Data, 0, 0, null);

                int[] pixels = new int[width * height];
                bmpMonochrome.GetPixels(pixels, 0, width, 0, 0, width, height);

                //var grayBytes = pixels.Select(a => (byte)(a & 0xff)).ToArray();
                //
                //var ditheredBytes = floydSteinberg(grayBytes, width, height);
                //
                //var dots = new bool[height][];
                //for (int i = 0; i < height; i++)
                //{
                //    dots[i] = new bool[width];
                //    for (int j = 0; j < width; j++)
                //    {
                //        int pixel = ditheredBytes[i * width + j];
                //        dots[i][j] = pixel == 0;
                //    }
                //}

                var dots = new bool[height][];
                for (int i = 0; i < height/2; i++) //temp
                {
                    dots[i] = new bool[width];
                    for (int j = 0; j < width; j++)
                    {
                        int pixel = bmpMonochrome.GetPixel(j, i);
                        int lowestBit = pixel & 0xff;
                        dots[i][j] = lowestBit < 128 ||
                            (lowestBit == 170 && (i % 2 == 1 && j % 2 == 1)) ||
                            (lowestBit == 204 && (i % 2 == 1 && j % 2 == 1 && (i + j) % 4 == 2));
                    }
                }

                var bytes = GetBytesForPrinter(dots, width, height/2); //temp
                
                await printerOutputStream.WriteAsync(bytes, 0, bytes.Length);

                return new ResultSuccess();
            }
            catch (Exception e)
            {
                return new ResultSuccess(false, e.ProperMessage());
            }
        }

        public byte[] floydSteinberg(byte[] sb, int w, int h)
        {
            for (var i = 0; i < h; i++)
                for (var j = 0; j < w; j++)
                {
                    var ci = i * w + j;               // current buffer index
                    var cc = sb[ci];              // current color
                    var rc = (byte)(cc < 128 ? 0 : 255);      // real (rounded) color
                    var err = (byte)(cc - rc);              // error amount
                    sb[ci] = rc;                  // saving real color
                    if (j + 1 < w) sb[ci + 1] += (byte)((err * 7) / 16);  // if right neighbour exists
                    if (i + 1 == h) continue;   // if we are in the last line
                    if (j > 0) sb[ci + w - 1] += (byte)((err * 3) / 16);  // bottom left neighbour
                    sb[ci + w] += (byte)((err * 5) / 16);  // bottom neighbour
                    if (j + 1 < w) sb[ci + w + 1] += (byte)((err * 1) / 16);  // bottom right neighbour
                }
            
            return sb;
        }

        public async Task<ResultSuccess<List<BluetoothDeviceModel>>> GetDevicesNameAndMACID()
        {
            mBluetoothAdapter = BluetoothAdapter.DefaultAdapter;

            if (mBluetoothAdapter == null)
                return new ResultSuccess<List<BluetoothDeviceModel>>(false, "آداپتر بلوتوث یافت نشد.");

            if (!mBluetoothAdapter.IsEnabled)
            {
                mBluetoothAdapter.Enable();
                await Task.Delay(3000);
            }
            
            var pairedDevices = mBluetoothAdapter.BondedDevices;

            var result = new List<BluetoothDeviceModel>();
            foreach (var device in pairedDevices)
            {
                var Class = device.BluetoothClass;
                var Name = device.Name;
                var MACID = device.Address;

                //if (device.BluetoothClass.MajorDeviceClass == MajorDeviceClass.Imaging)
                result.Add(new BluetoothDeviceModel()
                {
                    MACID = MACID,
                    Name = Name,
                    Class = Class.ToString()
                });
            }
            
            return new ResultSuccess<List<BluetoothDeviceModel>>(true, "", result);
        }

        async Task<ResultSuccess> findBT(string PrinterMACID)
        {
            try
            {
                mBluetoothAdapter = BluetoothAdapter.DefaultAdapter;

                if (mBluetoothAdapter == null)
                    return new ResultSuccess(false, "آداپتر بلوتوث یافت نشد.");

                if (!mBluetoothAdapter.IsEnabled)
                {
                    mBluetoothAdapter.Enable();
                    await Task.Delay(3000);
                }

                foreach (var item in mBluetoothAdapter.BondedDevices)
                {
                    if(item.Address == PrinterMACID)
                    {
                        printerDevice = item;
                        break;
                    }
                }
                
                if(printerDevice == null)
                    throw new Exception("چاپگر مورد نظر در لیست دستگاه های جفت شده یافت نشد.");
                
                return new ResultSuccess();
            }
            catch (Exception err)
            {
                return new ResultSuccess(false, "در پیدا کردن پرینتر بلوتوث خطایی رخ داد.\n" + err.ProperMessage());
            }
        }

        ResultSuccess openBT()
        {
            try
            {
                // Standard SerialPortService ID
                var uuid = UUID.FromString("00001101-0000-1000-8000-00805f9b34fb");
                if (printerSocket != null)
                {
                    printerSocket.Close();
                    printerSocket.Dispose();
                }
                printerSocket = printerDevice.CreateRfcommSocketToServiceRecord(uuid);
                printerSocket.Connect();
                printerOutputStream = printerSocket.OutputStream;
                printerInputStream = printerSocket.InputStream;
                
                beginListenForData();
                
                return new ResultSuccess();
            }
            catch (Exception err)
            {
                return new ResultSuccess(false, "در برقراری اتصال با پرینتر بلوتوث خطایی رخ داد. مطمئن شوید که دستگاه پرینتر روشن است. \n" + err.ProperMessage());
            }
        }

        async void beginListenForData()
        {
            try
            {
                // this is the ASCII code for a newline character
                byte delimiter = 10;

                var stopWorker = false;
                var readBufferPosition = 0;
                var readBuffer = new byte[1024];

                while (!stopWorker)
                {
                    try
                    {
                        await Task.Delay(100);
                        if (printerInputStream.IsDataAvailable())
                        {
                            var bytesAvailable = (int)printerInputStream.Length;
                            byte[] packetBytes = new byte[bytesAvailable];
                            printerInputStream.Read(packetBytes, 0, bytesAvailable);
                            for (int i = 0; i < bytesAvailable; i++)
                            {
                                byte b = packetBytes[i];
                                if (b == delimiter)
                                {
                                    var encodedBytes = readBuffer.ToArray();
                                    string data = System.Text.Encoding.ASCII.GetString(encodedBytes);
                                    readBufferPosition = 0;

                                    // tell the user data were sent to bluetooth printer device
                                }
                                else
                                {
                                    readBuffer[readBufferPosition++] = b;
                                }
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        stopWorker = true;
                    }
                }
            }
            catch (Exception err)
            {
            }
        }
        
        byte[] GetBytesForPrinter(bool[][] dots, int Width, int Height)
        {
            byte[] widthBytes = BitConverter.GetBytes(Width);
            var nl = widthBytes[0];
            var nh = widthBytes[1];

            var ret = new List<byte>();
            ret.AddRange(PrinterCommands.INIT);

            ret.AddRange(PrinterCommands.SET_LINE_SPACING_24);
            
            int offset = 0;
            while (offset < Height)
            {
                ret.AddRange(PrinterCommands.SELECT_BIT_IMAGE_MODE);
                ret.Add(nl);
                ret.Add(nh);

                for (int x = 0; x < Width; ++x)
                {
                    for (int k = 0; k < 3; ++k)
                    {
                        byte slice = 0;
                        for (int b = 0; b < 8; ++b)
                        {
                            int y = offset + k * 8 + b;
                            
                            bool v = y < Height ? dots[y][x] : false;
                                
                            slice |= (byte)((v ? 1 : 0) << (7 - b));
                        }
                        ret.Add(slice);
                    }
                }
                offset += 24;
                ret.AddRange(PrinterCommands.FEED_LINE);
            }
            
            ret.AddRange(PrinterCommands.SET_LINE_SPACING_30);

            return ret.ToArray();
        }
        class PrinterCommands
        {
            public static byte[] INIT = { 27, 64 };
            public static byte[] FEED_LINE = { 10 };

            public static byte[] SELECT_FONT_A = { 27, 33, 0 };

            public static byte[] SET_BAR_CODE_HEIGHT = { 29, 104, 100 };
            public static byte[] PRINT_BAR_CODE_1 = { 29, 107, 2 };
            public static byte[] SEND_NULL_BYTE = { 0x00 };

            public static byte[] SELECT_PRINT_SHEET = { 0x1B, 0x63, 0x30, 0x02 };
            public static byte[] FEED_PAPER_AND_CUT = { 0x1D, 0x56, 66, 0x00 };

            public static byte[] SELECT_CYRILLIC_CHARACTER_CODE_TABLE = { 0x1B, 0x74, 0x11 };
            
            public static byte[] SELECT_BIT_IMAGE_MODE = { 0x1B, 0x2A, 33 };
            public static byte[] SET_LINE_SPACING_24 = { 0x1B, 0x33, 24 };
            public static byte[] SET_LINE_SPACING_30 = { 0x1B, 0x33, 30 };

            public static byte[] TRANSMIT_DLE_PRINTER_STATUS = { 0x10, 0x04, 0x01 };
            public static byte[] TRANSMIT_DLE_OFFLINE_PRINTER_STATUS = { 0x10, 0x04, 0x02 };
            public static byte[] TRANSMIT_DLE_ERROR_STATUS = { 0x10, 0x04, 0x03 };
            public static byte[] TRANSMIT_DLE_ROLL_PAPER_SENSOR_STATUS = { 0x10, 0x04, 0x04 };
        }
    }
    
    public class QRScan : IQRScan
    {
        public MainActivity mainActivity;
        public QRScan(MainActivity mainActivity)
        {
            this.mainActivity = mainActivity;
        }
        public override void StartScan()
        {
            Intent intent = new Intent("com.google.zxing.client.android.SCAN");
            HasScannerApplication = intent.ResolveActivity(mainActivity.PackageManager) != null;
            if (HasScannerApplication)
                mainActivity.StartActivityForResult(intent, MainActivity.QRScanRequestCode);
        }

        public void OnActivityResult(Result resultCode, Intent data)
        {
            if (resultCode == Result.Ok)
            {
                var contents = data.GetStringExtra("SCAN_RESULT");
                var format = data.GetStringExtra("SCAN_RESULT_FORMAT");
                OnScanResult(new QRScanResult()
                {
                    success = true,
                    contents = contents,
                    format = format
                });
            }
            else if (resultCode == Result.Canceled)
            {
                OnScanResult(new QRScanResult()
                {
                    success = false
                });
            }
        }
    }

    public class PersianDatePicker : IPersianDatePicker
    {
        public static FragmentManager FragmentManager;
        public void ShowDatePicker()
        {
            //var d = new Com.Mohamadamin.Persianmaterialdatetimepicker.Date.DatePickerDialog();
            //d.Show(FragmentManager, "test");
        }
    }

    //public class MajorDeviceSetting : IMajorDeviceSetting
    //{
    //    public ContentResolver ContentResolver { get; set; }
    //    public Context Context { get; set; }
    //    public bool CheckDeviceAdminSetting()
    //    {
    //        if (MainActivity.MainActivityInstance.devicePolicyManager == null || !MainActivity.MainActivityInstance.devicePolicyManager.IsAdminActive(MainActivity.MainActivityInstance.myDeviceAdmin))
    //        {
    //            OpenDeviceAdminSetting("برای استفاده از اپلیکیشن کارا باید تنظیمات Device Administrator را برای آن فعال کنید.");
    //            return true;
    //        }
    //        return false;
    //    }
    //    public bool CheckDateTimeSetting()
    //    {
    //        if (Android.Provider.Settings.Global.GetInt(ContentResolver, Android.Provider.Settings.Global.AutoTime, 0) != 1)
    //        {
    //            OpenDateTimeSetting("تنظیمات تاریخ و ساعت خودکار را فعال نمایید.");
    //            return true;
    //        }
    //        return false;
    //    }
    //    public bool CheckGPSSetting()
    //    {
    //        LocationManager lm = (LocationManager)Context.GetSystemService(Context.LocationService);
    //        var gps_enabled = false;
    //        try
    //        {
    //            gps_enabled = lm.IsProviderEnabled(LocationManager.GpsProvider);
    //        }
    //        catch (Exception ex) { }
    //        if (!gps_enabled)
    //        {
    //            OpenGPSSetting("GPS را فعال نمایید.");
    //            return true;
    //        }
    //        return false;
    //    }
    //    public bool CheckGPSPermission()
    //    {
    //        var HasPermission = false;
    //        if (MainActivity.MainActivityInstance != null)
    //            HasPermission = Android.Support.V4.Content.ContextCompat.CheckSelfPermission(MainActivity.MainActivityInstance, Android.Manifest.Permission.AccessFineLocation) == Android.Content.PM.Permission.Granted;
    //        else if(KaraNewService.KaraNewServiceInstance != null)
    //            HasPermission = KaraNewService.KaraNewServiceInstance.PackageManager.CheckPermission(Android.Manifest.Permission.AccessFineLocation, KaraNewService.KaraNewServiceInstance.PackageName) != Android.Content.PM.Permission.Granted;
            
    //        if (!HasPermission)
    //        {
    //            OpenGPSPermissionRequest("دسترسی GPS را برای اپلیکیشن کارا فعال نمایید.");
    //            return true;
    //        }
    //        return false;
    //    }
    //    public async Task<bool> CheckInternetConnection(bool WithoutTryingToConnect)
    //    {
    //        var IsConnected = await IsOnline();
    //        if (!IsConnected)
    //        {
    //            var cm = (Android.Net.ConnectivityManager)Context.GetSystemService(Context.ConnectivityService);
    //            try
    //            {
    //                var cmClass = Java.Lang.Class.ForName(cm.Class.Name);
    //                var method = cmClass.GetDeclaredMethod("getMobileDataEnabled");
    //                method.Accessible = true;
    //                IsConnected = (bool)method.Invoke(cm);
    //            }
    //            catch (Exception e)
    //            {
    //            }
    //        }

    //        if (!IsConnected && !WithoutTryingToConnect)
    //        {
    //            OpenInternetSetting("لطفا اتصال اینترنت خود را چک نمایید.");
    //            return true;
    //        }
    //        return false;
    //    }

    //    public void OpenDeviceAdminSetting(string Explanation)
    //    {
    //        var intent = new Intent(DevicePolicyManager.ActionAddDeviceAdmin);
    //        intent.PutExtra(DevicePolicyManager.ExtraDeviceAdmin, MainActivity.MainActivityInstance.myDeviceAdmin);
    //        intent.PutExtra(DevicePolicyManager.ExtraAddExplanation, Explanation);
    //        MainActivity.MainActivityInstance.StartActivityForResult(intent, (int)SettingDialougeLauncherRequestCode.DeviceAdminSetting);
    //    }
    //    public void OpenDateTimeSetting(string Explanation)
    //    {
    //        Intent __intent = new Intent(Android.Provider.Settings.ActionDateSettings);
    //        App.ToastMessageHandler.ShowMessage(Explanation, ToastMessageDuration.Long);
    //        if(MainActivity.MainActivityInstance != null)
    //            MainActivity.MainActivityInstance.StartActivityForResult(__intent, (int)SettingDialougeLauncherRequestCode.DateTime);
    //        else
    //            KaraNewService.KaraNewServiceInstance.StartActivity(__intent);
    //    }
    //    public void OpenGPSSetting(string Explanation)
    //    {
    //        Intent __intent = new Intent(Android.Provider.Settings.ActionLocationSourceSettings);
    //        App.ToastMessageHandler.ShowMessage(Explanation, ToastMessageDuration.Long);
    //        if(MainActivity.MainActivityInstance != null)
    //            MainActivity.MainActivityInstance.StartActivityForResult(__intent, (int)SettingDialougeLauncherRequestCode.GPSSetting);
    //        else
    //            KaraNewService.KaraNewServiceInstance.StartActivity(__intent);
    //    }
    //    public void OpenGPSPermissionRequest(string Explanation)
    //    {
    //        App.ToastMessageHandler.ShowMessage(Explanation, ToastMessageDuration.Long);
    //        if (MainActivity.MainActivityInstance != null)
    //            Android.Support.V4.App.ActivityCompat.RequestPermissions(MainActivity.MainActivityInstance, new string[] { Android.Manifest.Permission.AccessFineLocation }, (int)SettingDialougeLauncherRequestCode.GPSPermission);
    //    }
        
    //    public void OpenInternetSetting(string Explanation)
    //    {
    //        Intent __intent = new Intent(Android.Provider.Settings.ActionDateSettings);
    //        App.ToastMessageHandler.ShowMessage(Explanation, ToastMessageDuration.Long);
    //        if (MainActivity.MainActivityInstance != null)
    //            MainActivity.MainActivityInstance.StartActivityForResult(__intent, (int)SettingDialougeLauncherRequestCode.InternetConnection);
    //        else
    //            KaraNewService.KaraNewServiceInstance.StartActivity(__intent);
    //    }

    //    public async void MajorDeviceSettingsChanged(ChangedMajorDeviceSetting setting)
    //    {
    //        try
    //        {
    //            var DeviceSettingChange = new DeviceSettingChange()
    //            {
    //                DateTime = DateTime.Now,
    //                Type = (int)setting,
    //                Sent = false
    //            };
    //            await App.DB.InsertRecordAsync(DeviceSettingChange);
    //            Connectivity.SubmitDeviceSettingChange();
    //        }
    //        catch (Exception err)
    //        {
    //            Log.Error("MajorDeviceSettingsChanged", err.ProperMessage());
    //        }
    //    }

        

    //    private async Task<bool> IsOnline()
    //    {
    //        return await new TaskFactory<bool>().StartNew(() =>
    //        {
    //            try
    //            {
    //                int timeoutMs = 1500;
    //                var sockaddr = new InetSocketAddress("8.8.8.8", 53);
    //                var sock = new Java.Net.Socket();
    //                sock.Connect(sockaddr, timeoutMs);
    //                sock.Close();
    //                return true;
    //            }
    //            catch (Exception e) { return false; }
    //        });
    //    }
    //}

    public class TCPClient : ITCPClient
    {
        public async Task<ResultSuccess> SubmitLocations(string IP, int Port, ILocationSubmitModel _submitData)
        {
            return await Task.Factory.StartNew(() => {
                try
                {
                    var submitData = new LocationSubmitModel()
                    {
                        UserName = _submitData.UserName,
                        Password = _submitData.Password,
                        CurrentVersionNumber = _submitData.CurrentVersionNumber,
                        Points = _submitData.Points.Select(a => new SubmittingLocationModel()
                        {
                            Index = a.Index,
                            TimeStamp = a.TimeStamp,
                            State = a.State,
                            Lat = a.Lat,
                            Long = a.Long,
                            Accuracy = a.Accuracy
                        }).ToArray()
                    };


                    var data = ConvertLocationSubmitDataToByteArray(submitData);
                    var LengthBytes = BitConverter.GetBytes(data.Length);
                    if (BitConverter.IsLittleEndian)
                        Array.Reverse(LengthBytes);
                    data = LengthBytes.Concat(data).ToArray();

                    //IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
                    //TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();

                    //foreach (TcpConnectionInformation tcpi in tcpConnInfoArray)
                    //{
                    //    if (tcpi.LocalEndPoint.Port == Port)
                    //    {
                    //        //isAvailable = false;
                    //        break;
                    //    }
                    //}

                    //string hostname = "89.165.114.174";
                    //int portno = Port;
                    //IPAddress ipa = (IPAddress)Dns.GetHostAddresses(hostname)[0];


                    //try
                    //{
                    //    System.Net.Sockets.Socket sock = new System.Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);
                    //    sock.Connect(ipa, portno);
                    //    if (sock.Connected == true)  // Port is in use and connection is successful
                    //    {
                    //    }
                    //    //MessageBox.Show("Port is Closed");
                    //    sock.Close();

                    //}
                    //catch (System.Net.Sockets.SocketException ex)
                    //{

                    //}

                    TcpClient client = new TcpClient(IP, Port);
                    NetworkStream stream = client.GetStream();
                    stream.Write(data, 0, data.Length);

                    var response = new byte[1024];
                    var bytesLength = stream.Read(response, 0, response.Length);
                    var responseData = System.Text.Encoding.ASCII.GetString(response, 0, bytesLength);

                    var resultDeserialized = JsonConvert.DeserializeObject<ResultSuccess>(responseData);

                    client.Close();

                    return resultDeserialized;
                }
                catch (Exception err)
                {
                    Log.Error("Submit Locations", err.Message + (err.StackTrace != null ? (", StackTrace:" + err.StackTrace) : ""));
                    return new ResultSuccess(false, err.ProperMessage());
                }
            });
        }

        private static byte[] ConvertLocationSubmitDataToByteArray(LocationSubmitModel obj)
        {
            byte[] data;
            using (var ms = new MemoryStream())
            {
                Serializer.Serialize(ms, obj);
                data = ms.ToArray();
            }

            return data;
        }
    }

    [ProtoContract]
    public class SubmittingLocationModel : ISubmittingLocationModel
    {
        [ProtoMember(1)]
        public new int Index { get; set; }
        [ProtoMember(2)]
        public new long TimeStamp { get; set; }
        [ProtoMember(3)]
        public new int State { get; set; }
        [ProtoMember(4)]
        public new double? Lat { get; set; }
        [ProtoMember(5)]
        public new double? Long { get; set; }
        [ProtoMember(6)]
        public new double? Accuracy { get; set; }
    }
    [ProtoContract]
    public class LocationSubmitModel : ILocationSubmitModel
    {
        [ProtoMember(1)]
        public new string UserName { get; set; }
        [ProtoMember(2)]
        public new string Password { get; set; }
        [ProtoMember(3)]
        public new string CurrentVersionNumber { get; set; }
        [ProtoMember(4)]
        public new SubmittingLocationModel[] Points { get; set; }
    }

    public class KaraVersion : IKaraVersion
    {
        public string GetVersion()
        {
            var Context = MainActivity.MainActivityInstance != null ? (Context)MainActivity.MainActivityInstance : (Context)KaraNewService.KaraNewServiceInstance;
            return Context.PackageManager.GetPackageInfo(Context.PackageName, 0).VersionName;
        }
    }
}