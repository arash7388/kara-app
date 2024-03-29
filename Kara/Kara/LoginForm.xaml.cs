﻿using Kara.Assets;
using Kara.CustomRenderer;
using Kara.Services;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;
using Java.Security;
using Permissions = Xamarin.Essentials.Permissions;

namespace Kara
{
    public partial class LoginForm : GradientContentPage
    {
        private Entry ServerAddress = new MyEntry() { HorizontalTextAlignment = TextAlignment.End, Placeholder = "آدرس سرور، مثلا: 192.168.1.2".ToPersianDigits(), LeftRounded = true };
        private Image ServerAddressIcon = new EntryCompanionIcon() { Source = "url.png" };
        private Entry Username = new MyEntry() { HorizontalTextAlignment = TextAlignment.End, Placeholder = "نام کاربری", LeftRounded = true };
        private Image UsernameIcon = new EntryCompanionIcon() { Source = "username.png" };
        private Entry Password = new MyEntry() { HorizontalTextAlignment = TextAlignment.End, Placeholder = "کلمه عبور", IsPassword = true, LeftRounded = true };
        private Image PasswordIcon = new EntryCompanionIcon() { Source = "password.png" };
        private Button LoginButton = new RoundButton() { Text = "ورود", FontAttributes = FontAttributes.Bold, BackgroundColor = Color.Black };
        private Button UniqueIdButton = new RoundButton() { Text = "شناسه سیستمی", FontAttributes = FontAttributes.Bold };
        private ActivityIndicator BusyIndicator = new ActivityIndicator() { VerticalOptions = LayoutOptions.Center, HorizontalOptions = LayoutOptions.Center, HeightRequest = 30, Color = Color.FromHex("E6EBEF"), IsRunning = false };
        private Label LoginErrorText = new Label() { TextColor = Color.FromHex("f33"), HorizontalTextAlignment = TextAlignment.Center };

        public LoginForm()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);

            LoginButton.Clicked += Login;
            UniqueIdButton.Clicked += UniqueIdButton_Clicked;

            ServerAddress.Text = App.ServerAddress;
            if (App.Username.Value != "")
                Username.Text = App.Username.Value;

            BusyIndicator.IsRunning = false;
        }

        private async void UniqueIdButton_Clicked(object sender, EventArgs e)
        {
            string deviceIdentifier = DependencyService.Get<IDevice>().GetIdentifier();
            var s = await DisplayActionSheet("", "انصراف", deviceIdentifier + " : شناسه سیستمی", "کپی");

            if (s == "کپی")
            {
                await Clipboard.SetTextAsync(deviceIdentifier);
            }
        }

        private Guid LastSizeAllocationId = Guid.NewGuid();

        protected override async void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            Guid ThisSizeAllocationId = Guid.NewGuid();
            LastSizeAllocationId = ThisSizeAllocationId;
            await Task.Delay(100);
            if (LastSizeAllocationId == ThisSizeAllocationId)
                sizeChanged(width, height);
        }

        private double LastWidth, LastHeight;

        public void sizeChanged(double width, double height)
        {
            try
            {
                if (LastWidth != width || LastHeight != height)
                {
                    LastWidth = width;
                    LastHeight = height;

                    LoginLayoutGrid.RowDefinitions = new RowDefinitionCollection() {
                    new RowDefinition() { Height = new GridLength(10, GridUnitType.Star) },
                    new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) },
                    new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) },
                    new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) },
                    new RowDefinition() { Height = new GridLength(0.5, GridUnitType.Auto) },
                    new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) },
                    new RowDefinition() { Height = new GridLength(10, GridUnitType.Star) },
                    new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) },
                };

                    var y = 0.8;
                    LoginLayoutGrid.ColumnDefinitions = new ColumnDefinitionCollection() {
                    new ColumnDefinition() { Width = new GridLength((1 - y) / 2, GridUnitType.Star) },
                    new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) },
                    new ColumnDefinition() { Width = new GridLength(y, GridUnitType.Star) },
                    new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) },
                    new ColumnDefinition() { Width = new GridLength((1 - y) / 2, GridUnitType.Star) }
                };

                    LoginLayoutGrid.Children.Clear();

                    LoginLayoutGrid.Children.Add(ServerAddress, 1, 1);
                    Grid.SetColumnSpan(ServerAddress, 2);

                    LoginLayoutGrid.Children.Add(ServerAddressIcon, 3, 1);

                    LoginLayoutGrid.Children.Add(Username, 1, 2);
                    Grid.SetColumnSpan(Username, 2);

                    LoginLayoutGrid.Children.Add(UsernameIcon, 3, 2);

                    LoginLayoutGrid.Children.Add(Password, 1, 3);
                    Grid.SetColumnSpan(Password, 2);

                    LoginLayoutGrid.Children.Add(PasswordIcon, 3, 3);

                    LoginLayoutGrid.Children.Add(LoginButton, 1, 5);
                    Grid.SetColumnSpan(LoginButton, 3);

                    LoginLayoutGrid.Children.Add(UniqueIdButton, 1, 7);
                    Grid.SetColumnSpan(UniqueIdButton, 3);

                    LoginLayoutGrid.Children.Add(BusyIndicator, 1, 5);

                    LoginLayoutGrid.Children.Add(LoginErrorText, 1, 6);
                    Grid.SetColumnSpan(LoginErrorText, 3);
                }
            }
            catch (Exception)
            {
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            _ = await App.CheckGps();
        }

        public async void Login(object sender, EventArgs args)
        {
            try
            {
                BusyIndicator.IsRunning = true;

                LoginErrorText.IsVisible = false;
                var _ServerAddress = ServerAddress != null ? ServerAddress.Text != null ? ServerAddress.Text.ToLatinDigits() : "" : "";
                App.ServerAddress = _ServerAddress;

                if (!DependencyService.Get<IDevice>().PhonePermissionGranted())
                {
                    // Notify user permission was denied
                    App.ShowError("خطا", "لطفا از قسمت تنظیمات اجازه دسترسی به اطلاعات گوشی را صادر نمایید ", "باشه");
                    return;
                }

                var loginTask = Kara.Assets.Connectivity.Login(Username.Text, Password.Text);
                var loginResult = await loginTask;
                if (!loginResult.Success)
                {
                    LoginErrorText.Text = loginResult.Message;
                    LoginErrorText.IsVisible = true;
                    return;
                }

                var getSettingTask = Kara.Assets.Connectivity.GetAndroidAppSettings(Username.Text, Password.Text);
                await getSettingTask;

                //App.UseVisitorsNadroidApplication.Value = getSettingTask.Result.Data.UseVisitorsNadroidApplication;
                //App.UseCollectorAndroidApplication.Value = getSettingTask.Result.Data.UseCollectorAndroidApplication;
                //App.UseDistributerAndroidApplication.Value = getSettingTask.Result.Data.UseDistributerAndroidApplication;

                //check max concurrent number of users considering their access to V,T,D
                //if number does not exceed, devId should be added to LoggedInAndroidUsers
                var result = await Kara.Assets.Connectivity.CheckLicense(DependencyService.Get<IDevice>().GetIdentifier(),Username.Text,Password.Text);

                if(!string.IsNullOrWhiteSpace(result.ErrorMessage))
                {
                    App.ShowError("خطا", result.ErrorMessage, "بستن");
                    return;
                }

                result.HasVisitorLic = true;

                if (!result.HasVisitorLic && !result.HasTahsildarLic && !result.HasDistributerLic)
                {
                    App.ShowError("خطا", "شما هیچ یک از مجوزهای استفاده از برنامه اندروید را ندارید", "بستن");
                    return;
                }

                App.UseVisitorsNadroidApplication.Value = result.HasVisitorLic;
                App.UseCollectorAndroidApplication.Value = result.HasTahsildarLic;
                App.UseDistributerAndroidApplication.Value = result.HasDistributerLic;


                //if (checkLic)
                //{
                //    if (!App.UseVisitorsNadroidApplication.Value)
                //    {
                //        App.ShowError("خطا", "شما لایسنس استفاده از نسخه ویزیتور را ندارید. لطفا با پشتیبانی تماس بگیرید", "بستن");
                //        return;
                //    }

                //    if (!App.UseCollectorAndroidApplication.Value)
                //    {
                //        App.ShowError("خطا", "شما لایسنس استفاده از نسخه تحصیلدار را ندارید. لطفا با پشتیبانی تماس بگیرید", "بستن");
                //        return;
                //    }

                //    if (!App.UseDistributerAndroidApplication.Value)
                //    {
                //        App.ShowError("خطا", "شما لایسنس استفاده از نسخه موزع را ندارید. لطفا با پشتیبانی تماس بگیرید", "بستن");
                //        return;
                //    }
                //}

                //commented on 1401/2/1 (old approach)
                //if (checkLic)
                //{
                //    if (App.UseVisitorsNadroidApplication.Value)
                //        if (!lic.HasVisitorLic)
                //        {
                //            App.ShowError("خطا", "شما لایسنس استفاده از نسخه ویزیتور را ندارید. لطفا با پشتیبانی تماس بگیرید", "بستن");
                //            return;
                //        }

                //    if (App.UseCollectorAndroidApplication.Value)
                //        if (!lic.HasTahsildarLic)
                //        {
                //            App.ShowError("خطا", "شما لایسنس استفاده از نسخه تحصیلدار را ندارید. لطفا با پشتیبانی تماس بگیرید", "بستن");
                //            return;
                //        }

                //    if (App.UseDistributerAndroidApplication.Value)
                //        if (!lic.HasDistributerLic)
                //        {
                //            App.ShowError("خطا", "شما لایسنس استفاده از نسخه موزع را ندارید. لطفا با پشتیبانی تماس بگیرید", "بستن");
                //            return;
                //        }
                //}

                var locationTask = App.CheckGps();

                var tasks = new Task[] { locationTask, loginTask, getSettingTask };
                await locationTask;
                Task.WaitAll(tasks);

                if (locationTask.Result == null)
                {
                    return;
                }

                App.UserId.Value = loginResult.Data.UserId;
                App.Username.Value = Username.Text;
                App.Password.Value = Password.Text;
                App.UserPersonnelId.Value = loginResult.Data.PersonnelId;
                App.UserEntityId.Value = loginResult.Data.EntityId;
                App.UserRealName.Value = loginResult.Data.RealName;
                App.EntityCode.Value = loginResult.Data.EntityCode;
                App.WarnIfSalePriceIsLessThanTheLastBuyPrice.Value = getSettingTask.Result.Data.WarnIfSalePriceIsLessThanTheLastBuyPrice;

                await Navigation.PushAsync(new MainMenu()
                {
                    StartColor = Color.FromHex("E6EBEF"),
                    EndColor = Color.FromHex("A6CFED")
                });

                if (App.UserId.Value != App.LastLoginUserId.Value)
                {
                    await App.DB.CleanDataBaseAsync();
                    await Navigation.PushAsync(new UpdateDBForm()
                    {
                        StartColor = Color.FromHex("E6EBEF"),
                        EndColor = Color.FromHex("A6CFED")
                    });
                    //App.MajorDeviceSetting.MajorDeviceSettingsChanged(ChangedMajorDeviceSetting.InitialStartup);
                }
                Navigation.RemovePage(this);
            }
            catch (Exception exc)
            {
                throw;
            }
            finally
            {
                BusyIndicator.IsRunning = false;
            }
        }
    }
}