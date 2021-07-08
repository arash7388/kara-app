using Kara.Assets;
using Kara.CustomRenderer;
using Kara.Helpers;
using Kara.Models;
using Plugin.Connectivity;
using Plugin.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Kara
{
    //test
    public class PrinterSettingModel
    {
        private string _Title;
        private string _MACID;
        private int _Width;
        private int _Density;
        private int _FontSize;
        private bool _Selected;

        public string MACID
        {
            get { return _MACID; }
            set
            {
                _MACID = value;
                SavePrinters();
            }
        }

        public string Title
        {
            get { return _Title; }
            set
            {
                _Title = value;
                SavePrinters();
            }
        }

        public int Width
        {
            get { return _Width; }
            set
            {
                _Width = value;
                SavePrinters();
            }
        }

        public int Density
        {
            get { return _Density; }
            set
            {
                _Density = value;
                SavePrinters();
            }
        }

        public int FontSize
        {
            get { return _FontSize; }
            set
            {
                _FontSize = value;
                SavePrinters();
            }
        }

        public bool Selected
        {
            get { return _Selected; }
            set
            {
                _Selected = value;
                SavePrinters();
            }
        }

        public int PrintWidthPixel { get { return (int)Math.Round(Width / 25.4 * Density); } }

        public PrinterSettingModel(string str)
        {
            var strArray = str.Split('|');
            _Title = strArray.Length > 0 ? strArray[0] : "";
            _MACID = strArray.Length > 1 ? strArray[1] : "";
            _Width = strArray.Length > 2 ? Convert.ToInt32(strArray[2]) : 0;
            _Density = strArray.Length > 3 ? Convert.ToInt32(strArray[3]) : 0;
            _FontSize = strArray.Length > 4 ? Convert.ToInt32(strArray[4]) : 0;
            _Selected = strArray.Length > 5 ? Convert.ToBoolean(strArray[5]) : false;
        }

        public override string ToString()
        {
            return Title + "|" +
                MACID + "|" +
                Width.ToString() + "|" +
                Density.ToString() + "|" +
                FontSize.ToString() + "|" +
                Selected.ToString();
        }

        public static void SavePrinters()
        {
            App.AllPrintersStr.Value = App.AllPrinters.Any() ? App.AllPrinters.Select(a => a.ToString()).Aggregate((sum, x) => sum + "|||" + x) : "";
        }

        public void Remove()
        {
            App._AllPrinters.Remove(this);
            SavePrinters();
        }

        public void Add()
        {
            App._AllPrinters.Add(this);
            SavePrinters();
        }
    }

    public class SettingField<T>
    {
        private string Name;
        private T DefaultValue;
        private T _Field;

        public SettingField(string name, T defaultValue)
        {
            this.Name = name;
            this.DefaultValue = defaultValue;

            if (!CrossSettings.Current.Contains(Name))
                this.Value = DefaultValue;

            if (typeof(T).Equals(typeof(bool)))
                _Field = (T)Convert.ChangeType(CrossSettings.Current.GetValueOrDefault(Name, (bool)(object)DefaultValue), typeof(T));
            else if (typeof(T).Equals(typeof(double)))
                _Field = (T)Convert.ChangeType(CrossSettings.Current.GetValueOrDefault(Name, (double)(object)DefaultValue), typeof(T));
            else if (typeof(T).Equals(typeof(Guid)))
                _Field = (T)Convert.ChangeType(CrossSettings.Current.GetValueOrDefault(Name, (Guid)(object)DefaultValue), typeof(T));
            else if (typeof(T).Equals(typeof(DateTime)))
                _Field = (T)Convert.ChangeType(CrossSettings.Current.GetValueOrDefault(Name, (DateTime)(object)DefaultValue), typeof(T));
            else if (typeof(T).Equals(typeof(float)))
                _Field = (T)Convert.ChangeType(CrossSettings.Current.GetValueOrDefault(Name, (float)(object)DefaultValue), typeof(T));
            else if (typeof(T).Equals(typeof(int)))
                _Field = (T)Convert.ChangeType(CrossSettings.Current.GetValueOrDefault(Name, (int)(object)DefaultValue), typeof(T));
            else if (typeof(T).Equals(typeof(string)))
                _Field = (T)Convert.ChangeType(CrossSettings.Current.GetValueOrDefault(Name, (string)(object)DefaultValue), typeof(T));
            else if (typeof(T).Equals(typeof(long)))
                _Field = (T)Convert.ChangeType(CrossSettings.Current.GetValueOrDefault(Name, (long)(object)DefaultValue), typeof(T));
            else if (typeof(T).Equals(typeof(decimal)))
                _Field = (T)Convert.ChangeType(CrossSettings.Current.GetValueOrDefault(Name, (decimal)(object)DefaultValue), typeof(T));
        }

        public T Value
        {
            get
            {
                return _Field;
            }
            set
            {
                _Field = value;
                if (typeof(T).Equals(typeof(bool)))
                    CrossSettings.Current.AddOrUpdateValue(Name, (bool)(object)value);
                else if (typeof(T).Equals(typeof(double)))
                    CrossSettings.Current.AddOrUpdateValue(Name, (double)(object)value);
                else if (typeof(T).Equals(typeof(Guid)))
                    CrossSettings.Current.AddOrUpdateValue(Name, (Guid)(object)value);
                else if (typeof(T).Equals(typeof(DateTime)))
                    CrossSettings.Current.AddOrUpdateValue(Name, (DateTime)(object)value);
                else if (typeof(T).Equals(typeof(float)))
                    CrossSettings.Current.AddOrUpdateValue(Name, (float)(object)value);
                else if (typeof(T).Equals(typeof(int)))
                    CrossSettings.Current.AddOrUpdateValue(Name, (int)(object)value);
                else if (typeof(T).Equals(typeof(string)))
                    CrossSettings.Current.AddOrUpdateValue(Name, (string)(object)value);
                else if (typeof(T).Equals(typeof(long)))
                    CrossSettings.Current.AddOrUpdateValue(Name, (long)(object)value);
                else if (typeof(T).Equals(typeof(decimal)))
                    CrossSettings.Current.AddOrUpdateValue(Name, (decimal)(object)value);
            }
        }
    }

    public class App : Application
    {
        #region debugFlags

        public static int SendLocationsToServerPeriod = 5;

        #endregion debugFlags

        public static string DeviceInfo;
        //Settings
        public static SettingField<decimal> VATPercent = new SettingField<decimal>("VATPercent", 0);

        public static SettingField<bool> CheckForNegativeStocksOnOrderInsertion = new SettingField<bool>("CheckForNegativeStocksOnOrderInsertion", false);
        public static SettingField<bool> OrderPrintShowMainUnitFee = new SettingField<bool>("OrderPrintShowMainUnitFee", true);
        public static SettingField<bool> OrderPrintShowSmallUnitFee = new SettingField<bool>("OrderPrintShowSmallUnitFee", true);
        public static SettingField<bool> UseBatchNumberAndExpirationDate = new SettingField<bool>("UseBatchNumberAndExpirationDate", false);
        public static SettingField<bool> UseVisitProgram = new SettingField<bool>("UseVisitProgram", false);
        public static SettingField<bool> AllowOptionalDiscountRules_MultiSelection = new SettingField<bool>("AllowOptionalDiscountRules_MultiSelection", false);
        public static SettingField<string> SystemName = new SettingField<string>("SystemName", "");
        public static SettingField<TimeSpan?> VisitorBeginWorkTime = new SettingField<TimeSpan?>("VisitorBeginWorkTime", null);
        public static SettingField<TimeSpan?> VisitorEndWorkTime = new SettingField<TimeSpan?>("VisitorEndWorkTime", null);
        public static SettingField<bool> GPSShouldBeTurnedOnDuringWorkTime = new SettingField<bool>("GPSShouldBeTurnedOnDuringWorkTime", false);
        public static SettingField<bool> InternetShouldBeConnectedDuringWorkTime = new SettingField<bool>("InternetShouldBeConnectedDuringWorkTime", false);

        //public static SettingField<bool> GPSTracking_GPSShouldBeTurnedOnToWorkWithApp = new SettingField<bool>("GPSTracking_GPSShouldBeTurnedOnToWorkWithApp", false);
        //public static SettingField<bool> GPSTracking_NetworkShouldBeTurnedOnToWorkWithApp = new SettingField<bool>("GPSTracking_NetworkShouldBeTurnedOnToWorkWithApp", false);
        public static SettingField<decimal> ShowSaleVisitProgramPartnersToVisitorHourShift = new SettingField<decimal>("ShowSaleVisitProgramPartnersToVisitorHourShift", 0);

        public static SettingField<decimal> DayStartTime = new SettingField<decimal>("DayStartTime", 0);
        public static SettingField<decimal> DayEndTime = new SettingField<decimal>("DayEndTime", 24);
        public static SettingField<string> CompanyNameForPrint = new SettingField<string>("CompanyNameForPrint", "");
        public static SettingField<string> CompanyLogoForPrint = new SettingField<string>("CompanyLogoForPrint", "");
        public static SettingField<string> PrintTitle = new SettingField<string>("PrintTitle", "");
        public static SettingField<string> EndOfPrintDescription = new SettingField<string>("EndOfPrintDescription", "");
        public static SettingField<bool> HasPrintingOption = new SettingField<bool>("HasPrintingOption", false);
        public static SettingField<bool> DefineWarehouseForSaleAndBuy = new SettingField<bool>("DefineWarehouseForSaleAndBuy", false);
        public static SettingField<int> CalculateStuffsSettlementDaysBasedOn = new SettingField<int>("CalculateStuffsSettlementDaysBasedOn", 0);
        public static SettingField<bool> UseCollectorAndroidApplication = new SettingField<bool>("UseCollectorAndroidApplication", false);
        public static SettingField<bool> UseDistributerAndroidApplication = new SettingField<bool>("UseDistributerAndroidApplication", false);
        public static SettingField<bool> UseVisitorsNadroidApplication = new SettingField<bool>("UseVisitorsNadroidApplication", false);
        public static SettingField<bool> UseBarcodeScannerInVisitorAppToSelectStuff = new SettingField<bool>("UseBarcodeScannerInVisitorAppToSelectStuff", false);
        public static SettingField<bool> UseQRScannerInVisitorAppToSelectStuff = new SettingField<bool>("UseQRScannerInVisitorAppToSelectStuff", false);
        private static SettingField<string> _QRScannerInVisitorAppForSelectingStuffTemplates = new SettingField<string>("QRScannerInVisitorAppForSelectingStuffTemplates", "");
        private static string[] __QRScannerInVisitorAppForSelectingStuffTemplates;

        public static string[] QRScannerInVisitorAppForSelectingStuffTemplates
        {
            get
            {
                if (__QRScannerInVisitorAppForSelectingStuffTemplates == null)
                    __QRScannerInVisitorAppForSelectingStuffTemplates = _QRScannerInVisitorAppForSelectingStuffTemplates.Value.Split(new string[] { "|||" }, StringSplitOptions.RemoveEmptyEntries);
                return __QRScannerInVisitorAppForSelectingStuffTemplates;
            }
            set
            {
                _QRScannerInVisitorAppForSelectingStuffTemplates.Value = value.Any() ? value.Aggregate((sum, x) => sum + "|||" + x) : "";
                __QRScannerInVisitorAppForSelectingStuffTemplates = _QRScannerInVisitorAppForSelectingStuffTemplates.Value.Split(new string[] { "|||" }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        public static SettingField<int> LastPriceListVersion = new SettingField<int>("LastPriceListVersion", 0);
        public static SettingField<int> LastDiscountRuleVersion = new SettingField<int>("LastDiscountRuleVersion", 0);
        public static bool LastPriceListOrDiscountRuleVersionChanged = false;

        public static SettingField<bool> ShowNotAvailableStuffsOnOrderInsertion = new SettingField<bool>("ShowNotAvailableStuffsOnOrderInsertion", false);
        public static SettingField<bool> ShowConsumerPrice = new SettingField<bool>("ShowConsumerPrice", false);
        public static SettingField<bool> ShowAccountingCycleOfPartner_Remainder = new SettingField<bool>("ShowAccountingCycleOfPartner_Remainder", true);
        public static SettingField<bool> ShowAccountingCycleOfPartner_UncashedCheques = new SettingField<bool>("ShowAccountingCycleOfPartner_UncashedCheques", true);
        public static SettingField<bool> ShowAccountingCycleOfPartner_ReturnedCheques = new SettingField<bool>("ShowAccountingCycleOfPartner_ReturnedCheques", true);
        public static SettingField<bool> ShowPartnerGroupInList = new SettingField<bool>("ShowPartnerGroupInList", true);
        public static SettingField<bool> ShowPartnerLegalNameInList = new SettingField<bool>("ShowPartnerLegalNameInList", false);
        public static SettingField<bool> ShowStuffCodeInOrderInsertForm = new SettingField<bool>("ShowStuffCodeInOrderInsertForm", true);
        public static SettingField<int> StuffListGroupingMethod = new SettingField<int>("StuffListGroupingMethod", 0);
        public static SettingField<bool> UseDefaultPriceList = new SettingField<bool>("UseDefaultPriceList", false);
        public static SettingField<string> DefaultPriceListVersionId = new SettingField<string>("DefaultPriceListVersionId", null);
        public static SettingField<string> DefaultWarehouseId = new SettingField<string>("DefaultWarehouseId", null);
        public static SettingField<string> _GallaryStuffCount = new SettingField<string>("GallaryStuffCount", "1x1");

        private static int[] _GallaryStuffCountIntArray;

        public static MainMenu MainMenu;

        public static int[] GallaryStuffCount
        {
            get
            {
                if (_GallaryStuffCountIntArray == null)
                    _GallaryStuffCountIntArray = _GallaryStuffCount.Value.Split('x').Select(a => Convert.ToInt32(a)).ToArray();
                return _GallaryStuffCountIntArray;
            }
            set
            {
                if (_GallaryStuffCountIntArray != value)
                {
                    _GallaryStuffCountIntArray = value;
                    _GallaryStuffCount.Value = _GallaryStuffCountIntArray.Select(a => a.ToString()).Aggregate((sum, x) => sum + "x" + x);
                }
            }
        }

        public static SettingField<int> DailyTrackingBeginTime_Seconds = new SettingField<int>("DailyTrackingBeginTime_Seconds1", (int)new TimeSpan(8, 0, 0).TotalSeconds);
        public static SettingField<int> DailyTrackingEndTime_Seconds = new SettingField<int>("DailyTrackingEndTime_Seconds1", (int)new TimeSpan(20, 0, 0).TotalSeconds);
        public static SettingField<int> MaxAcceptableAccuracy = new SettingField<int>("MaxAcceptableAccuracy1", 20);
        public static SettingField<int> GetLocationsPerid = new SettingField<int>("GetLocationsPerid1", 10);
        public static SettingField<bool> ShouldTurnGPSAndNetworkAutomatically = new SettingField<bool>("ShouldTurnGPSAndNetworkAutomatically1", false);

        public static float DeviceSizeDensity;

        public static long[] _Last5UniversalLineInApp = new long[3] { 0, 0, 0 };
        public static string Last5UniversalLineInApp { get { return "[" + _Last5UniversalLineInApp[0] + ", " + _Last5UniversalLineInApp[1] + ", " + _Last5UniversalLineInApp[2] + "]"; } }
        public static string SpecialLog = "";

        public static long UniversalLineInApp
        {
            set
            {
                _Last5UniversalLineInApp[0] = _Last5UniversalLineInApp[1];
                _Last5UniversalLineInApp[1] = _Last5UniversalLineInApp[2];
                _Last5UniversalLineInApp[2] = value;
            }
        }

        //public static IMajorDeviceSetting MajorDeviceSetting;
        public static IKaraVersion KaraVersion;

        public static ITCPClient TCPClient;
        public static IDownloader Downloader;
        public static IQRScan QRScanner;
        public static IUploader Uploader;
        public static IFile File;
        public static IBluetoothPrinter BluetoothPrinter;
        public static IPersianDatePicker PersianDatePicker;
        public static IToastMessage ToastMessageHandler;
        public static IPersianDateConverter PersianDateConverter;

        public static string CurrentVersionNumber
        {
            get
            {
                return KaraVersion != null ? KaraVersion.GetVersion() : "2.0.0";
            }
        }

        public static string DBFileName;
        public static string imagesDirectory;
        private static string _ServerAddress;

        public static string ServerAddress
        {
            get
            {
                if (_ServerAddress == null)
                {
                    var AllAddresses = AllServerAddresses.Value.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(a => new
                        {
                            Address = a.Split('_')[0],
                            Selected = Convert.ToBoolean(a.Split('_')[1])
                        });
                    _ServerAddress = AllAddresses.Any() ? AllAddresses.Single(a => a.Selected).Address : null;
                }
                return _ServerAddress;
            }
            set
            {
                var AllAddresses = AllServerAddresses.Value.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(a => new
                        {
                            Address = a.Split('_')[0],
                            Selected = Convert.ToBoolean(a.Split('_')[1])
                        });
                AllAddresses = AllAddresses.Select(a => new
                {
                    Address = a.Address,
                    Selected = a.Address == value
                })
                    .Union((AllAddresses.Any(a => a.Address == value) ? new string[] { } : new string[] { value }).Select(a => new
                    {
                        Address = a,
                        Selected = true
                    })).ToArray();
                AllAddresses = AllAddresses.Where(a => !string.IsNullOrWhiteSpace(a.Address)).ToArray();
                if (!AllAddresses.Any(a => a.Selected))
                    AllAddresses = AllAddresses.Select((a, index) => new
                    {
                        Address = a.Address,
                        Selected = index == 0
                    }).ToArray();
                AllServerAddresses.Value = AllAddresses.Any() ? AllAddresses.Select(a => a.Address + "_" + a.Selected.ToString()).Aggregate((sum, x) => sum + "|" + x) : "";
                _ServerAddress = value;
            }
        }

        public static SettingField<string> AllServerAddresses = new SettingField<string>("AllServerAddresses", "");

        public static SettingField<string> AllPrintersStr = new SettingField<string>("AllPrintersStr", "");
        public static List<PrinterSettingModel> _AllPrinters;

        public static List<PrinterSettingModel> AllPrinters
        {
            get
            {
                if (_AllPrinters == null)
                    _AllPrinters = AllPrintersStr.Value.Split(new string[] { "|||" }, StringSplitOptions.RemoveEmptyEntries).Select(a => new PrinterSettingModel(a)).ToList();
                return _AllPrinters;
            }
        }

        public static PrinterSettingModel SelectedPrinter
        {
            get { return AllPrinters.Any(a => a.Selected) ? AllPrinters.Single(a => a.Selected) : null; }
        }

        public static SettingField<int> OrderPreviewFontSize;

        public static SettingField<Guid> UserId = new SettingField<Guid>("UserId", Guid.Empty);
        public static SettingField<string> Username = new SettingField<string>("Username", "");
        public static SettingField<string> Password = new SettingField<string>("Password", "");
        public static SettingField<Guid> UserPersonnelId = new SettingField<Guid>("UserPersonnelId", Guid.Empty);
        public static SettingField<Guid> UserEntityId = new SettingField<Guid>("UserEntityId", Guid.Empty);
        public static SettingField<string> UserRealName = new SettingField<string>("UserRealName", "");
        public static SettingField<string> EntityCode = new SettingField<string>("EntityCode", "");
        public static SettingField<int> WarnIfSalePriceIsLessThanTheLastBuyPrice = new SettingField<int>("WarnIfSalePriceIsLessThanTheLastBuyPrice", 2);
        public static SettingField<Guid> LastLoginUserId = new SettingField<Guid>("LastLoginUserId", Guid.Empty);
        public static DBRepository DB;
        public static DBRepository.AccessModel Accesses;
        private static SettlementType[] _SettlementTypes;
        public static SettlementType[] SettlementTypes { get { if (_SettlementTypes == null) _SettlementTypes = DB.GetSettlementTypes(); return _SettlementTypes; } }

        public static event EventHandler LocationChanged;

        public class LocationChangedEventArgs : EventArgs
        {
            public LocationModel OldLocation { get; set; }
            public LocationModel NewLocation { get; set; }
        }

        private static LocationModel _LastLocation;

        public static LocationModel LastLocation
        {
            get
            {
                return _LastLocation;
            }
            set
            {
                if (_LastLocation == null || _LastLocation.Latitude != value.Latitude || _LastLocation.Longitude != value.Longitude || _LastLocation.Accuracy != value.Accuracy)
                {
                    var OldLocation = _LastLocation;
                    _LastLocation = value;
                    EventHandler handler = LocationChanged;
                    if (handler != null)
                        handler(null, new LocationChangedEventArgs() { OldLocation = OldLocation, NewLocation = _LastLocation });
                }
            }
        }

        public static InsertedInformations_Orders InsertedInformations_Orders;
        public static InsertedInformations_FailedVisits InsertedInformations_FailedVisits;
        public static InsertedInformations_Partners InsertedInformations_Partners;

        public App()
        {
            InitializeApp();
        }

        public static bool GpsEnabled { get; set; }
        public static bool FirstGpsDetecting { get; set; } = true;

        public static void ShowError(string title, string message, string cancel)
        {
            StaticMainPage.DisplayAlert(title.ToPersianDigits(), message.ToPersianDigits(), cancel.ToPersianDigits());
        }

        private static Page StaticMainPage;

        private async Task InitializeApp()
        {
            if (NavigationStackState == null)
            {
                var SplashScreen = new GradientContentPage()
                {
                    StartColor = Color.FromHex("E6EBEF"),
                    EndColor = Color.FromHex("A6CFED")
                };
                var SplashImage = new Image() { VerticalOptions = LayoutOptions.FillAndExpand, HorizontalOptions = LayoutOptions.FillAndExpand, WidthRequest = 400, HeightRequest = 400 };
                var LoadingIndicator = new ActivityIndicator() { IsRunning = true, Color = Color.FromHex("8CB9DB"), VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center };
                var LoadingError = new Label() { TextColor = Color.FromHex("ff3333"), VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center };
                var SplashGrid = new Grid()
                {
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    ColumnDefinitions = new ColumnDefinitionCollection() {
                    new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition() { Width = new GridLength(7, GridUnitType.Star) },
                    new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) }
                },
                    RowDefinitions = new RowDefinitionCollection() {
                    new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition() { Height = new GridLength(7, GridUnitType.Star) },
                    new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) }
                },
                };
                SplashGrid.Children.Add(SplashImage, 1, 1);
                SplashGrid.Children.Add(LoadingIndicator, 1, 2);
                SplashGrid.Children.Add(LoadingError, 1, 3);

                SplashScreen.Content = SplashGrid;

                SplashImage.Source = ImageSource.FromFile("splash_logo.png");
                SplashImage.Aspect = Aspect.AspectFit;

                MainPage = new NavigationPage(SplashScreen);
                NavigationPage.SetHasNavigationBar(SplashScreen, false);

                var CreateTablesTask = DB.CreateTablesAsync();
                var CreateTablesResult = await CreateTablesTask;
                if (!CreateTablesResult.Success)
                {
                    LoadingIndicator.IsVisible = false;
                    LoadingError.IsVisible = true;
                    LoadingError.Text = CreateTablesResult.Message;
                    return;
                }

                ContentPage NextForm = UserId.Value != Guid.Empty ?
                (ContentPage)new MainMenu()
                {
                    StartColor = Color.FromHex("E6EBEF"),
                    EndColor = Color.FromHex("A6CFED")
                } :
                (ContentPage)new LoginForm()
                {
                    StartColor = Color.FromHex("E6EBEF"),
                    EndColor = Color.FromHex("A6CFED")
                };

                await Task.Delay(2000);

                await MainPage.Navigation.PushAsync(NextForm);
                MainPage.Navigation.RemovePage(SplashScreen);
            }
            else
            {
                MainPage = new NavigationPage();
                for (int i = 0; i < NavigationStackState.Length; i++)
                {
                    if (NavigationStackState[i] != null)
                    {
                        var page = NavigationStackState[i];
                        try
                        {
                            page.Parent = null;
                            await MainPage.Navigation.PushAsync(page, false);
                        }
                        catch (Exception err)
                        {
                        }
                    }
                }
            }

            var AccessesResult = await App.DB.FetchUserAccessesAsync();

            CheckForPersonnelWorkingTime();

            StaticMainPage = MainPage;

            await Task.Delay(500);
            OrderPreviewFontSize = new SettingField<int>("OrderPreviewFontSize", (int)Math.Round(Math.Min(StaticMainPage.Width, StaticMainPage.Height) * App.DeviceSizeDensity / 70));

            if (QRScanner != null)
                QRScanner.OnScanResult = new Action<QRScanResult>((result) =>
                {
                    //TODO
                });
        }

        private async void CheckForPersonnelWorkingTime()
        {
            var Now = (decimal)DateTime.Now.TimeOfDay.TotalHours;
            if (App.DayStartTime.Value <= Now && Now <= App.DayEndTime.Value)
            {
                await Task.Delay(60);
                GoToOutOfBoundTimeForm();
                CheckForPersonnelWorkingTime();
            }
            else
            {
                BackFromOutOfBoundTimeForm();
            }
        }

        private void BackFromOutOfBoundTimeForm()
        {
        }

        private void GoToOutOfBoundTimeForm()
        {
        }

        private static Page[] NavigationStackState;

        protected override void OnStart()
        {
            CrossConnectivity.Current.ConnectivityChanged += ConnectivityStateChanged;
        }

        protected override void OnSleep()
        {
            NavigationStackState = MainPage.Navigation.NavigationStack.ToArray();
        }

        private void ConnectivityStateChanged(object sender, Plugin.Connectivity.Abstractions.ConnectivityChangedEventArgs e)
        {
            //TODO

            //ConnectionType:
            //CrossConnectivity.Current.ConnectionTypes.First().ToString()
        }

        public static async Task<ResultSuccess> RecalculateUnsentOrdersPriceAndDiscount()
        {
            var UnsentOrders = await DB.GetOrdersListAsync(false, true, "");
            foreach (var SaleOrder in UnsentOrders.Data)
            {
                var Today = DateTime.Now.Date;
                List<PriceListStuff> PriceListStuffs = new List<PriceListStuff>();
                var PriceListStuffResult = await DB.GetPartnerPriceListStuffsAsync(SaleOrder.SaleOrderData.PartnerId, Today);
                if (PriceListStuffResult.Success)
                    PriceListStuffs = PriceListStuffResult.Data;
                var AllStuffsData = DB.conn.Table<Stuff>().ToList();
                var PriceData = from StuffData in AllStuffsData
                                from PriceListStuff in PriceListStuffs.Where(a => a.StuffId == StuffData.Id).DefaultIfEmpty()
                                select new { StuffData, PriceListStuff };
                Dictionary<Guid, decimal> UnitPrices = PriceData.ToDictionary(a => a.StuffData.Id, a => a.PriceListStuff != null ? a.PriceListStuff.SalePrice : 0);

                var SaleOrderData = SaleOrder.SaleOrderData;

                var result = await CalculatePrices(SaleOrderData, UnitPrices);
                if (!result.Success)
                    return result;
                result = await CalculateDiscounts(SaleOrderData, UnitPrices);
                if (!result.Success)
                    return result;
                var result2 = await SaveSaleOrder(SaleOrderData);
                if (!result2.Success)
                    return new ResultSuccess(false, result2.Message);
            }

            return new ResultSuccess();
        }

        public static async Task<ResultSuccess> CalculatePrices(SaleOrder SaleOrder, Dictionary<Guid, decimal> UnitPrices)
        {
            return await await Task.Factory.StartNew(async () =>
            {
                try
                {
                    await Task.Delay(200);
                    foreach (var Article in SaleOrder.SaleOrderStuffs)
                        Article.SalePrice = UnitPrices[Article.Package.StuffId] * Article.Package.Coefficient;

                    return new ResultSuccess();
                }
                catch (Exception err)
                {
                    return new ResultSuccess(false, "خطایی در محاسبه قیمت ها رخ داده است. لطفا با پشتیبانی نرم افزار تماس بگیرید.\n" + err.ProperMessage());
                }
            });
        }

        public static async Task<ResultSuccess> CalculateDiscounts(SaleOrder SaleOrder, Dictionary<Guid, decimal> UnitPrices)
        {
            return await await Task.Factory.StartNew(async () =>
            {
                try
                {
                    await Task.Delay(200);

                    var AllSystemStuffs = (await App.DB.GetStuffsAsync()).Data.ToDictionary(a => a.Id);
                    var StuffsSettlementDays = (await App.DB.GetStuffsSettlementDaysAsync()).Data;
                    int SettlementDay = SaleOrder.SettlementType.Day;
                    if (App.CalculateStuffsSettlementDaysBasedOn.Value != 0)
                    {
                        decimal SettlementDaysSum = 0;
                        decimal CoefficientsSum = 0;
                        for (var i = 0; i < SaleOrder.SaleOrderStuffs.Length; i++)
                        {
                            var Coefficient = App.CalculateStuffsSettlementDaysBasedOn.Value == 1 ? SaleOrder.SaleOrderStuffs[i].StuffQuantity : SaleOrder.SaleOrderStuffs[i].SalePriceQuantity;
                            var StuffId = SaleOrder.SaleOrderStuffs[i].Package.StuffId.ToString().ToLower();
                            var StuffGroupCode = SaleOrder.SaleOrderStuffs[i].Package.Stuff.GroupCode;
                            var ThisRowSettlementDay = StuffsSettlementDays.ContainsKey(StuffId) ? StuffsSettlementDays[StuffId] : new int?();
                            while (!ThisRowSettlementDay.HasValue && StuffGroupCode.Length > 0)
                            {
                                ThisRowSettlementDay = StuffsSettlementDays.ContainsKey(StuffGroupCode) ? StuffsSettlementDays[StuffGroupCode] : new int?();
                                StuffGroupCode = StuffGroupCode.Substring(0, StuffGroupCode.Length - 2);
                            }
                            SaleOrder.SaleOrderStuffs[i].StuffSettlementDay = ThisRowSettlementDay;
                            if (Coefficient * ThisRowSettlementDay.GetValueOrDefault(0) != 0)
                            {
                                SettlementDaysSum += Coefficient * ThisRowSettlementDay.GetValueOrDefault(0);
                                CoefficientsSum += Coefficient;
                            }
                        }
                        if (SettlementDaysSum != 0 && CoefficientsSum != 0)
                            SettlementDay = (int)Math.Round(SettlementDaysSum / CoefficientsSum);
                    }

                    OrderModel SaleOrderModel = new OrderModel()
                    {
                        SettlementTypeId = SaleOrder.SettlementTypeId,
                        SettlementDay = SettlementDay,
                        VisitorId = App.UserPersonnelId.Value,
                        OrderInsertDate = SaleOrder.InsertDateTime,
                        Partner = SaleOrder.Partner,
                        Articles = SaleOrder.SaleOrderStuffs.Select(a => new ArticleModel()
                        {
                            Id = a.Id,
                            Stuff = a.Package.Stuff,
                            Package = a.Package,
                            BatchNumber = a.BatchNumber,
                            Quantity = a.Quantity,
                            UnitPrice = a.SalePrice / a.Package.Coefficient
                        }).ToArray()
                    };

                    var DiscountRules = await App.DB.GetDiscountRulesAsync();
                    if (DiscountRules == null)
                        return new ResultSuccess(false, "خطایی در محاسبه تخفیفات رخ داده است. لطفا با پشتیبانی نرم افزار تماس بگیرید.");

                    var DiscountCalculator = new DiscountCalculator(App.SystemName.Value, App.AllowOptionalDiscountRules_MultiSelection.Value, AllSystemStuffs, SaleOrderModel, DiscountRules);
                    SaleOrderModel = DiscountCalculator.ClaculateOrderDiscounts();

                    SaleOrder.SettlementDay = SaleOrderModel.SettlementDay;
                    SaleOrder.DiscountPercent = SaleOrderModel.DiscountPercent;
                    SaleOrder.CashPrise = SaleOrderModel.CashPrise;
                    foreach (var Article in SaleOrderModel.Articles)
                    {
                        var SaleOrderStuff = SaleOrder.SaleOrderStuffs.Single(a => a.Id == Article.Id);
                        SaleOrderStuff.DiscountPercent = Article.DiscountPercent;
                        SaleOrderStuff.CashDiscountPercent = Article.CashDiscountPercent;
                    }
                    SaleOrder.SaleOrderStuffs = SaleOrder.SaleOrderStuffs.Where(a => !a.FreeProduct).ToArray();
                    var FreeProductSaleOrderStuffs = SaleOrderModel.FreeProducts.Select((a, indx) => new SaleOrderStuff()
                    {
                        Id = Guid.NewGuid(),
                        ArticleIndex = indx + 1,
                        OrderId = SaleOrder.Id,
                        PackageId = a.Package.Id,
                        BatchNumberId = a.BatchNumber != null ? a.BatchNumber.BatchNumberId : new Nullable<Guid>(),
                        Quantity = a.Quantity,
                        FreeProduct = true,
                        FreeProduct_UnitPrice = UnitPrices.ContainsKey(a.Stuff.Id) ? UnitPrices[a.Stuff.Id] : 0,
                        SalePrice = 0,
                        DiscountPercent = 0,
                        ProporatedDiscount = 0,
                        VATPercent = 0
                    }).ToList();
                    var SaleOrderStuffs = SaleOrder.SaleOrderStuffs.ToList();
                    SaleOrderStuffs.AddRange(FreeProductSaleOrderStuffs);
                    SaleOrder.SaleOrderStuffs = SaleOrderStuffs.ToArray();

                    SaleOrder.CashDiscounts = SaleOrderModel.CashSettlementDiscounts.Select(a => new CashDiscount()
                    {
                        Id = Guid.NewGuid(),
                        OrderId = SaleOrder.Id,
                        SaleOrder = SaleOrder,
                        Day = a.Day,
                        Percent = a.Percent
                    }).ToArray();

                    SaleOrder = App.DB.CalculateProporatedDiscount(SaleOrder);

                    return new ResultSuccess();
                }
                catch (Exception err)
                {
                    return new ResultSuccess(false, "خطایی در محاسبه تخفیفات رخ داده است. لطفا با پشتیبانی نرم افزار تماس بگیرید.\n" + err.ProperMessage());
                }
            });
        }

        public static async Task<ResultSuccess<SaleOrder>> SaveSaleOrder(SaleOrder SaleOrder)
        {
            App.DB.InitTransaction();
            try
            {
                var CurrentSaleOrder = App.DB.GetSaleOrder(SaleOrder.Id);
                if (CurrentSaleOrder != null)
                    await App.DB.DeleteSaleOrdersAsync(new Guid[] { SaleOrder.Id });
                else
                {
                    var lastKnownLocation = await Geolocation.GetLastKnownLocationAsync();
                    //var LocationState = App.LastLocation != null ?
                    //App.LastLocation.DateTime >= DateTime.Now.AddMinutes(-10) ? "OK" : ("App.LastLocation.DateTime = " + App.LastLocation.DateTime.ToString() + " and DateTime.Now = " + DateTime.Now.ToString()) : "App.LastLocation is null";
                    //var LastValidLocation = App.LastLocation != null ? App.LastLocation.DateTime >= DateTime.Now.AddMinutes(-10) ? App.LastLocation : null : null;
                    //App.ToastMessageHandler.ShowMessage(LocationState, ToastMessageDuration.Long);
                    //SaleOrder.GeoLocation_Latitude = LastValidLocation != null ? (decimal)LastValidLocation.Latitude : new Nullable<decimal>();
                    //SaleOrder.GeoLocation_Longitude = LastValidLocation != null ? (decimal)LastValidLocation.Longitude : new Nullable<decimal>();
                    //SaleOrder.GeoLocation_Accuracy = LastValidLocation != null ? (decimal)LastValidLocation.Accuracy : new Nullable<decimal>();
                    decimal.TryParse(lastKnownLocation.Latitude.ToString(), out decimal lat);
                    SaleOrder.GeoLocation_Latitude = lat;

                    decimal.TryParse(lastKnownLocation.Longitude.ToString(), out decimal lng);
                    SaleOrder.GeoLocation_Longitude = lng;

                    decimal.TryParse(lastKnownLocation.Accuracy.ToString(), out decimal accuracy);
                    SaleOrder.GeoLocation_Accuracy = accuracy;

                    SaleOrder.InsertDateTime = DateTime.Now;
                }

                //if (App.WarnIfSalePriceIsLessThanTheLastBuyPrice.Value != 2)
                //{
                //    var lastBuyPrices = await Kara.Assets.Connectivity.GetLastBuyPrice(SaleOrder.SaleOrderStuffs.Select(a => a.Id).ToList());

                //    string resultMessage = "";
                //    int rowNumber = 1;
                        
                //    foreach (SaleOrderStuff item in SaleOrder.SaleOrderStuffs)
                //    {
                //        if (item.SalePrice < lastBuyPrices.FirstOrDefault(a => a.Key == item.Id).Value)
                //        {
                //            resultMessage+="فی فروش کمتر از آخرین فی خرید، سطر " + rowNumber;
                //        }

                //        rowNumber++;
                //    }
                //}

                var result = await App.DB.InsertOrUpdateRecordAsync<SaleOrder>(SaleOrder);
                if (!result.Success)
                {
                    App.DB.RollbackTransaction();
                    return new ResultSuccess<SaleOrder>(false, result.Message);
                }

                result = await App.DB.InsertOrUpdateAllRecordsAsync<SaleOrderStuff>(SaleOrder.SaleOrderStuffs);
                if (!result.Success)
                {
                    App.DB.RollbackTransaction();
                    return new ResultSuccess<SaleOrder>(false, result.Message);
                }

                result = await App.DB.InsertOrUpdateAllRecordsAsync<CashDiscount>(SaleOrder.CashDiscounts);
                if (!result.Success)
                {
                    App.DB.RollbackTransaction();
                    return new ResultSuccess<SaleOrder>(false, result.Message);
                }

                App.DB.CommitTransaction();
                return new ResultSuccess<SaleOrder>(true, "", SaleOrder);
            }
            catch (Exception err)
            {
                App.DB.RollbackTransaction();
                return new ResultSuccess<SaleOrder>(false, err.ProperMessage());
            }
        }

        public static async Task<Location> CheckGps()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                var location = await Geolocation.GetLocationAsync(request);

                if (location != null)
                {
                    return location;
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception
                App.ToastMessageHandler.ShowMessage("جهت استفاده از برنامه باید مکان یاب فعال باشد", ToastMessageDuration.Long);
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
                App.ToastMessageHandler.ShowMessage("خطا در دسترسی به مکان یاب", ToastMessageDuration.Long);
            }
            catch (Exception ex)
            {
                // Unable to get location
                App.ToastMessageHandler.ShowMessage("جهت استفاده از برنامه باید مکان یاب فعال باشد", ToastMessageDuration.Long);
            }

            return null;
        }
    }
}