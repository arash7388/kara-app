using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kara.CustomRenderer;
using Xamarin.Forms;
using Kara.Models;
using Kara.Assets;

namespace Kara
{
    public partial class SettingsForm : GradientContentPage
    {
        KeyValuePair<PriceListVersion, string>[] PriceListVersions;
        KeyValuePair<Warehouse, string>[] Warehouses;
        string[] StuffListGroupings = new string[] { "عدم تفکیک", "به تفکیک گروه های کالایی", "به تفکیک سبدهای کالایی" };
        public Label ServerAddressLabel2 { get { return _ServerAddressLabel2; } }
        public Label GallaryStuffCountLabel2 { get { return _GallaryStuffCountLabel2; } }
        public Label PrintersLabel2 { get { return _PrintersLabel2; } }



        Label ServerAddressLabel1,
            _ServerAddressLabel2,
            PartnerShowAccountingCycleLabel,
            PartnerShowAccountingCycleLabel_Remainder,
            PartnerShowAccountingCycleLabel_UncashedCheques,
            PartnerShowAccountingCycleLabel_ReturnedCheques,
            GallaryStuffCountLabel1,
            _GallaryStuffCountLabel2,
            DefaultPriceListLabel1,
            DefaultPriceListLabel2,
            DefaultWarehouseLabel1,
            DefaultWarehouseLabel2,
            StuffListGroupingLabel1,
            StuffListGroupingLabel2,
            PrintersLabel1,
            _PrintersLabel2;
        Switch ShowNotAvailableStuffsOnOrderInsertionSwitch,
            ShowConsumerPriceSwitch,
            PartnerShowAccountingCycleSwitch_Remainder,
            PartnerShowAccountingCycleSwitch_UncashedCheques,
            PartnerShowAccountingCycleSwitch_ReturnedCheques,
            ShowPartnerLegalNameInList,
            ShowPartnerGroupInList,
            ShowStuffCodeInOrderInsertForm;
        Picker DefaultPriceListPicker,
            DefaultWarehousePicker,
            StuffListGroupingPicker;

        private void PrepareFormElements()
        {
            ContentGrid.RowDefinitions = new RowDefinitionCollection();

            var i = 0;
            ContentGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30, GridUnitType.Absolute) });
            ContentGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(25, GridUnitType.Absolute) });
            ContentGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(10, GridUnitType.Absolute) });
            
            ServerAddressLabel1 = new Label() { Text = "آدرس سرور", FontSize = 18, FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.End, VerticalOptions = LayoutOptions.Center };
            ContentGrid.Children.Add(ServerAddressLabel1, 1, i);

            _ServerAddressLabel2 = new Label() { TextColor = Color.Gray, Text = "", FontSize = 15, FontAttributes = FontAttributes.None, HorizontalOptions = LayoutOptions.End, VerticalOptions = LayoutOptions.Center };
            ContentGrid.Children.Add(_ServerAddressLabel2, 1, i + 1);



            i += 3;
            ContentGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30, GridUnitType.Absolute) });
            ContentGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(25, GridUnitType.Absolute) });
            ContentGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(10, GridUnitType.Absolute) });

            ShowNotAvailableStuffsOnOrderInsertionSwitch = new Switch() { HorizontalOptions = LayoutOptions.EndAndExpand, VerticalOptions = LayoutOptions.Center };
            ContentGrid.Children.Add(ShowNotAvailableStuffsOnOrderInsertionSwitch, 0, i);
            Grid.SetRowSpan(ShowNotAvailableStuffsOnOrderInsertionSwitch, 2);
            
            Label label1 = new Label() { Text = "مشاهده کالاهای بدون موجودی", FontSize = 18, FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.End, VerticalOptions = LayoutOptions.Center };
            ContentGrid.Children.Add(label1, 1, i);
            Grid.SetRowSpan(label1, 2);



            i += 3;
            ContentGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30, GridUnitType.Absolute) });
            ContentGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(25, GridUnitType.Absolute) });
            ContentGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(10, GridUnitType.Absolute) });

            ShowConsumerPriceSwitch = new Switch() { HorizontalOptions = LayoutOptions.EndAndExpand, VerticalOptions = LayoutOptions.Center };
            ContentGrid.Children.Add(ShowConsumerPriceSwitch, 0, i);
            Grid.SetRowSpan(ShowConsumerPriceSwitch, 2);

            Label label2 = new Label() { Text = "نمایش قیمت مصرف کننده کالاها", FontSize = 18, FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.End, VerticalOptions = LayoutOptions.Center };
            ContentGrid.Children.Add(label2, 1, i);
            Grid.SetRowSpan(label2, 2);


            i += 3;
            if (App.Accesses.AccessToViewPartnerRemainder)
            {
                ContentGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30, GridUnitType.Absolute) });
                ContentGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(25, GridUnitType.Absolute) });
                ContentGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(25, GridUnitType.Absolute) });
                ContentGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(25, GridUnitType.Absolute) });
                ContentGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(25, GridUnitType.Absolute) });

                PartnerShowAccountingCycleLabel = new Label() { Text = "نمایش خلاصه وضعیت حساب مشتری:", FontSize = 18, FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.End, VerticalOptions = LayoutOptions.Center };
                ContentGrid.Children.Add(PartnerShowAccountingCycleLabel, 1, i);

                PartnerShowAccountingCycleSwitch_Remainder = new Switch() { HorizontalOptions = LayoutOptions.EndAndExpand, VerticalOptions = LayoutOptions.Center };
                ContentGrid.Children.Add(PartnerShowAccountingCycleSwitch_Remainder, 0, i + 1);

                PartnerShowAccountingCycleLabel_Remainder = new Label() { Text = "        مانده حساب", FontSize = 18, FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.End, VerticalOptions = LayoutOptions.Center };
                ContentGrid.Children.Add(PartnerShowAccountingCycleLabel_Remainder, 1, i + 1);

                PartnerShowAccountingCycleSwitch_UncashedCheques = new Switch() { HorizontalOptions = LayoutOptions.EndAndExpand, VerticalOptions = LayoutOptions.Center };
                ContentGrid.Children.Add(PartnerShowAccountingCycleSwitch_UncashedCheques, 0, i + 2);

                PartnerShowAccountingCycleLabel_UncashedCheques = new Label() { Text = "        چک های وصول نشده", FontSize = 18, FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.End, VerticalOptions = LayoutOptions.Center };
                ContentGrid.Children.Add(PartnerShowAccountingCycleLabel_UncashedCheques, 1, i + 2);

                PartnerShowAccountingCycleSwitch_ReturnedCheques = new Switch() { HorizontalOptions = LayoutOptions.EndAndExpand, VerticalOptions = LayoutOptions.Center };
                ContentGrid.Children.Add(PartnerShowAccountingCycleSwitch_ReturnedCheques, 0, i + 3);

                PartnerShowAccountingCycleLabel_ReturnedCheques = new Label() { Text = "        چک های برگشتی", FontSize = 18, FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.End, VerticalOptions = LayoutOptions.Center };
                ContentGrid.Children.Add(PartnerShowAccountingCycleLabel_ReturnedCheques, 1, i + 3);

                i += 5;
            }
            
            

            ContentGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30, GridUnitType.Absolute) });
            ContentGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(25, GridUnitType.Absolute) });
            ContentGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(10, GridUnitType.Absolute) });

            GallaryStuffCountLabel1 = new Label() { Text = "تعداد کالا در هر صفحه گالری کالا", FontSize = 18, FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.End, VerticalOptions = LayoutOptions.Center };
            ContentGrid.Children.Add(GallaryStuffCountLabel1, 1, i);

            _GallaryStuffCountLabel2 = new Label() { TextColor = Color.Gray, Text = "", FontSize = 15, FontAttributes = FontAttributes.None, HorizontalOptions = LayoutOptions.End, VerticalOptions = LayoutOptions.Center };
            ContentGrid.Children.Add(_GallaryStuffCountLabel2, 1, i + 1);



            i += 3;
            ContentGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30, GridUnitType.Absolute) });
            ContentGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(25, GridUnitType.Absolute) });
            ContentGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(10, GridUnitType.Absolute) });

            DefaultPriceListLabel1 = new Label() { Text = "لیست قیمت پیشفرض", FontSize = 18, FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.End, VerticalOptions = LayoutOptions.Center };
            ContentGrid.Children.Add(DefaultPriceListLabel1, 1, i);

            DefaultPriceListLabel2 = new Label() { TextColor = Color.Gray, Text = "", FontSize = 15, FontAttributes = FontAttributes.None, HorizontalOptions = LayoutOptions.End, VerticalOptions = LayoutOptions.Center };
            ContentGrid.Children.Add(DefaultPriceListLabel2, 1, i + 1);

            DefaultPriceListPicker = new Picker() { VerticalOptions = LayoutOptions.Start, HorizontalOptions = LayoutOptions.FillAndExpand };
            ContentGrid.Children.Add(DefaultPriceListPicker, 1, i + 2);



            if (App.DefineWarehouseForSaleAndBuy.Value)
            {
                i += 3;
                ContentGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30, GridUnitType.Absolute) });
                ContentGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(25, GridUnitType.Absolute) });
                ContentGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(10, GridUnitType.Absolute) });

                DefaultWarehouseLabel1 = new Label() { Text = "انبار پیشفرض", FontSize = 18, FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.End, VerticalOptions = LayoutOptions.Center };
                ContentGrid.Children.Add(DefaultWarehouseLabel1, 1, i);

                DefaultWarehouseLabel2 = new Label() { TextColor = Color.Gray, Text = "انتخاب در زمان سفارش", FontSize = 15, FontAttributes = FontAttributes.None, HorizontalOptions = LayoutOptions.End, VerticalOptions = LayoutOptions.Center };
                ContentGrid.Children.Add(DefaultWarehouseLabel2, 1, i + 1);

                DefaultWarehousePicker = new Picker() { VerticalOptions = LayoutOptions.Start, HorizontalOptions = LayoutOptions.FillAndExpand };
                ContentGrid.Children.Add(DefaultWarehousePicker, 1, i + 1);
            }



            i += 3;
            ContentGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30, GridUnitType.Absolute) });
            ContentGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(25, GridUnitType.Absolute) });
            ContentGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(10, GridUnitType.Absolute) });

            StuffListGroupingLabel1 = new Label() { Text = "تفکیک کالاها", FontSize = 18, FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.End, VerticalOptions = LayoutOptions.Center };
            ContentGrid.Children.Add(StuffListGroupingLabel1, 1, i);

            StuffListGroupingLabel2 = new Label() { TextColor = Color.Gray, Text = "", FontSize = 15, FontAttributes = FontAttributes.None, HorizontalOptions = LayoutOptions.End, VerticalOptions = LayoutOptions.Center };
            ContentGrid.Children.Add(StuffListGroupingLabel2, 1, i + 1);
            
            StuffListGroupingPicker = new Picker() { VerticalOptions = LayoutOptions.Start, HorizontalOptions = LayoutOptions.FillAndExpand };
            ContentGrid.Children.Add(StuffListGroupingPicker, 1, i + 1);



            i += 3;
            ContentGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30, GridUnitType.Absolute) });
            ContentGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(25, GridUnitType.Absolute) });
            ContentGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(10, GridUnitType.Absolute) });

            ShowPartnerLegalNameInList = new Switch() { HorizontalOptions = LayoutOptions.EndAndExpand, VerticalOptions = LayoutOptions.Center };
            ContentGrid.Children.Add(ShowPartnerLegalNameInList, 0, i);
            Grid.SetRowSpan(ShowPartnerLegalNameInList, 2);
            
            Label label3 = new Label() { Text = "نمایش نام حقوقی مشتری در لیست", FontSize = 18, FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.End, VerticalOptions = LayoutOptions.Center };
            ContentGrid.Children.Add(label3, 1, i);
            Grid.SetRowSpan(label3, 2);
            


            i += 3;
            ContentGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30, GridUnitType.Absolute) });
            ContentGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(25, GridUnitType.Absolute) });
            ContentGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(10, GridUnitType.Absolute) });

            ShowPartnerGroupInList = new Switch() { HorizontalOptions = LayoutOptions.EndAndExpand, VerticalOptions = LayoutOptions.Center };
            ContentGrid.Children.Add(ShowPartnerGroupInList, 0, i);
            Grid.SetRowSpan(ShowPartnerGroupInList, 2);
            
            Label label4 = new Label() { Text = "نمایش گروه مشتری در لیست", FontSize = 18, FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.End, VerticalOptions = LayoutOptions.Center };
            ContentGrid.Children.Add(label4, 1, i);
            Grid.SetRowSpan(label4, 2);



            i += 3;
            ContentGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30, GridUnitType.Absolute) });
            ContentGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(25, GridUnitType.Absolute) });
            ContentGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(10, GridUnitType.Absolute) });
            
            ShowStuffCodeInOrderInsertForm = new Switch() { HorizontalOptions = LayoutOptions.EndAndExpand, VerticalOptions = LayoutOptions.Center };
            ContentGrid.Children.Add(ShowStuffCodeInOrderInsertForm, 0, i);
            Grid.SetRowSpan(ShowStuffCodeInOrderInsertForm, 2);
            
            Label label5 = new Label() { Text = "نمایش کد کالا در فرم ثبت سفارش", FontSize = 18, FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.End, VerticalOptions = LayoutOptions.Center };
            ContentGrid.Children.Add(label5, 1, i);
            Grid.SetRowSpan(label5, 2);



            i += 3;
            ContentGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30, GridUnitType.Absolute) });
            ContentGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(25, GridUnitType.Absolute) });
            ContentGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(10, GridUnitType.Absolute) });

            PrintersLabel1 = new Label() { Text = "تنظیمات چاپگر", FontSize = 18, FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.End, VerticalOptions = LayoutOptions.Center };
            ContentGrid.Children.Add(PrintersLabel1, 1, i);

            _PrintersLabel2 = new Label() { TextColor = Color.Gray, Text = "", FontSize = 15, FontAttributes = FontAttributes.None, HorizontalOptions = LayoutOptions.End, VerticalOptions = LayoutOptions.Center };
            ContentGrid.Children.Add(_PrintersLabel2, 1, i + 1);
        }

        public SettingsForm()
        {
            InitializeComponent();

            PrepareFormElements();

            ShowNotAvailableStuffsOnOrderInsertionSwitch.IsToggled = App.ShowNotAvailableStuffsOnOrderInsertion.Value;
            ShowNotAvailableStuffsOnOrderInsertionSwitch.Toggled += (sender, e) =>
            {
                App.ShowNotAvailableStuffsOnOrderInsertion.Value = ShowNotAvailableStuffsOnOrderInsertionSwitch.IsToggled;
            };

            ShowConsumerPriceSwitch.IsToggled = App.ShowConsumerPrice.Value;
            ShowConsumerPriceSwitch.Toggled += (sender, e) =>
            {
                App.ShowConsumerPrice.Value = ShowConsumerPriceSwitch.IsToggled;
            };

            if (App.Accesses.AccessToViewPartnerRemainder)
            {
                PartnerShowAccountingCycleSwitch_Remainder.IsToggled = App.ShowAccountingCycleOfPartner_Remainder.Value;
                PartnerShowAccountingCycleSwitch_Remainder.Toggled += (sender, e) =>
                {
                    App.ShowAccountingCycleOfPartner_Remainder.Value = PartnerShowAccountingCycleSwitch_Remainder.IsToggled;
                };

                PartnerShowAccountingCycleSwitch_UncashedCheques.IsToggled = App.ShowAccountingCycleOfPartner_UncashedCheques.Value;
                PartnerShowAccountingCycleSwitch_UncashedCheques.Toggled += (sender, e) =>
                {
                    App.ShowAccountingCycleOfPartner_UncashedCheques.Value = PartnerShowAccountingCycleSwitch_UncashedCheques.IsToggled;
                };

                PartnerShowAccountingCycleSwitch_ReturnedCheques.IsToggled = App.ShowAccountingCycleOfPartner_ReturnedCheques.Value;
                PartnerShowAccountingCycleSwitch_ReturnedCheques.Toggled += (sender, e) =>
                {
                    App.ShowAccountingCycleOfPartner_ReturnedCheques.Value = PartnerShowAccountingCycleSwitch_ReturnedCheques.IsToggled;
                };
            }

            ShowPartnerLegalNameInList.IsToggled = App.ShowPartnerLegalNameInList.Value;
            ShowPartnerLegalNameInList.Toggled += (sender, e) =>
            {
                App.ShowPartnerLegalNameInList.Value = ShowPartnerLegalNameInList.IsToggled;
            };

            ShowPartnerGroupInList.IsToggled = App.ShowPartnerGroupInList.Value;
            ShowPartnerGroupInList.Toggled += (sender, e) =>
            {
                App.ShowPartnerGroupInList.Value = ShowPartnerGroupInList.IsToggled;
            };

            ShowStuffCodeInOrderInsertForm.IsToggled = App.ShowStuffCodeInOrderInsertForm.Value;
            ShowStuffCodeInOrderInsertForm.Toggled += (sender, e) =>
            {
                App.ShowStuffCodeInOrderInsertForm.Value = ShowStuffCodeInOrderInsertForm.IsToggled;
            };

            DefaultPriceListPicker.IsVisible = false;
            var DefaultPriceListTapGestureRecognizer = new TapGestureRecognizer();
            DefaultPriceListTapGestureRecognizer.Tapped += (sender, e) => {
                DefaultPriceListPicker.Focus();
            };
            DefaultPriceListLabel1.GestureRecognizers.Add(DefaultPriceListTapGestureRecognizer);
            DefaultPriceListLabel2.GestureRecognizers.Add(DefaultPriceListTapGestureRecognizer);

            FillPriceListVersionsAsync(!string.IsNullOrEmpty(App.DefaultPriceListVersionId.Value) ? new Guid(App.DefaultPriceListVersionId.Value) : new Nullable<Guid>());
            
            if(App.DefineWarehouseForSaleAndBuy.Value)
            {
                DefaultWarehousePicker.IsVisible = false;
                var DefaultWarehouseTapGestureRecognizer = new TapGestureRecognizer();
                DefaultWarehouseTapGestureRecognizer.Tapped += (sender, e) => {
                    DefaultWarehousePicker.Focus();
                };
                DefaultWarehouseLabel1.GestureRecognizers.Add(DefaultWarehouseTapGestureRecognizer);
                DefaultWarehouseLabel2.GestureRecognizers.Add(DefaultWarehouseTapGestureRecognizer);

                FillWarehousesAsync(!string.IsNullOrEmpty(App.DefaultWarehouseId.Value) ? new Guid(App.DefaultWarehouseId.Value) : new Guid?());
            }
            
            StuffListGroupingPicker.IsVisible = false;
            var StuffListGroupingTapGestureRecognizer = new TapGestureRecognizer();
            StuffListGroupingTapGestureRecognizer.Tapped += (sender, e) => {
                StuffListGroupingPicker.Focus();
            };
            StuffListGroupingLabel1.GestureRecognizers.Add(StuffListGroupingTapGestureRecognizer);
            StuffListGroupingLabel2.GestureRecognizers.Add(StuffListGroupingTapGestureRecognizer);
            
            StuffListGroupingPicker.Items.Clear();
            StuffListGroupingPicker.Items.Add(StuffListGroupings[0]);
            StuffListGroupingPicker.Items.Add(StuffListGroupings[1]);
            StuffListGroupingPicker.Items.Add(StuffListGroupings[2]);

            FillStuffListGroupingsAsync(App.StuffListGroupingMethod.Value);

            var ServerAddressTapGestureRecognizer = new TapGestureRecognizer();
            ServerAddressTapGestureRecognizer.Tapped += async (sender, e) =>
            {
                var SettingsForm_ServerAddress = new SettingsForm_ServerAddress(this, null)
                {
                    StartColor = Color.FromHex("E6EBEF"),
                    EndColor = Color.FromHex("A6CFED")
                };
                await this.Navigation.PushAsync(SettingsForm_ServerAddress);
            };
            ServerAddressLabel1.GestureRecognizers.Add(ServerAddressTapGestureRecognizer);
            ServerAddressLabel2.GestureRecognizers.Add(ServerAddressTapGestureRecognizer);
            ServerAddressLabel2.Text = App.ServerAddress.ReplaceLatinDigits();

            var GallaryStuffCountTapGestureRecognizer = new TapGestureRecognizer();
            GallaryStuffCountTapGestureRecognizer.Tapped += async (sender, e) =>
            {
                var SettingsForm_GallaryStuffCount = new SettingsForm_GallaryStuffCount(this)
                {
                    StartColor = Color.FromHex("E6EBEF"),
                    EndColor = Color.FromHex("A6CFED")
                };
                await this.Navigation.PushAsync(SettingsForm_GallaryStuffCount);
            };
            GallaryStuffCountLabel1.GestureRecognizers.Add(GallaryStuffCountTapGestureRecognizer);
            GallaryStuffCountLabel2.GestureRecognizers.Add(GallaryStuffCountTapGestureRecognizer);
            RefreshGallryStuffCount();

            var PrinterTapGestureRecognizer = new TapGestureRecognizer();
            PrinterTapGestureRecognizer.Tapped += async (sender, e) =>
            {
                var SettingsForm_Printers = new SettingsForm_Printers(this, null)
                {
                    StartColor = Color.FromHex("E6EBEF"),
                    EndColor = Color.FromHex("A6CFED")
                };
                await this.Navigation.PushAsync(SettingsForm_Printers);
            };
            PrintersLabel1.GestureRecognizers.Add(PrinterTapGestureRecognizer);
            PrintersLabel2.GestureRecognizers.Add(PrinterTapGestureRecognizer);
            PrintersLabel2.Text = App.SelectedPrinter != null ? App.SelectedPrinter.Title.ReplaceLatinDigits() : "تعریف نشده";
        }
        
        public void RefreshGallryStuffCount()
        {
            GallaryStuffCountLabel2.Text = App._GallaryStuffCount.Value.ReplaceLatinDigits();
        }

        public async void FillPriceListVersionsAsync(Guid? VersionId)
        {
            PriceListVersions = new PriceListVersion[] { null }.Union((await App.DB.GetPriceListVersionsAsync()).Data.ToArray()).ToArray()
                .Select(a => new KeyValuePair<PriceListVersion, string>(a, (a == null ? "ـ ـ ـ" : (a.PriceListName + " (از " + a.BeginDate.ToShortStringForDate() + " تا " + a.EndDate.ToShortStringForDate() + ")")).ReplaceLatinDigits())).ToArray();
            
            DefaultPriceListPicker.Items.Clear();
            foreach (var PriceListVersion in PriceListVersions)
                DefaultPriceListPicker.Items.Add(PriceListVersion.Value);

            for (var i = 0; i < PriceListVersions.Length; i++)
                if (VersionId == (PriceListVersions[i].Key == null ? new Guid?() : PriceListVersions[i].Key.Id))
                {
                    DefaultPriceListPicker.SelectedIndex = i;
                    DefaultPriceListLabel2.Text = PriceListVersions[i].Value;
                }

            DefaultPriceListPicker.SelectedIndexChanged += (sender, e) =>
            {
                App.DefaultPriceListVersionId.Value = PriceListVersions[DefaultPriceListPicker.SelectedIndex].Key == null ? null : PriceListVersions[DefaultPriceListPicker.SelectedIndex].Key.Id.ToString();
                DefaultPriceListLabel2.Text = PriceListVersions[DefaultPriceListPicker.SelectedIndex].Value;

                DBRepository._AllStuffsDataInitialized = false;
            };
        }
        
        public async void FillWarehousesAsync(Guid? WarehouseId)
        {
            var _Warehouses = (await App.DB.GetWarehousesAsync()).Data.ToArray();
            if (_Warehouses.Length > 1)
                _Warehouses = new Warehouse[] { null }.Union(_Warehouses).ToArray();
            
            Warehouses = _Warehouses.Select(a => new KeyValuePair<Warehouse, string>(a, a == null ? "انتخاب در زمان سفارش" : a.WarehouseName.ReplaceLatinDigits())).ToArray();

            DefaultWarehousePicker.Items.Clear();
            foreach (var Warehouse in Warehouses)
                DefaultWarehousePicker.Items.Add(Warehouse.Value);

            for (var i = 0; i < Warehouses.Length; i++)
                if (WarehouseId == (Warehouses[i].Key == null ? new Guid?() : Warehouses[i].Key.WarehouseId))
                {
                    DefaultWarehousePicker.SelectedIndex = i;
                    DefaultWarehouseLabel2.Text = Warehouses[i].Value;
                }

            DefaultWarehousePicker.SelectedIndexChanged += (sender, e) =>
            {
                App.DefaultWarehouseId.Value = Warehouses[DefaultWarehousePicker.SelectedIndex].Key == null ? null : Warehouses[DefaultWarehousePicker.SelectedIndex].Key.WarehouseId.ToString();
                DefaultWarehouseLabel2.Text = Warehouses[DefaultWarehousePicker.SelectedIndex].Value;

                DBRepository._AllStuffsDataInitialized = false;
            };
        }
        
        public async void FillStuffListGroupingsAsync(int StuffListGroupingMethod)
        {
            StuffListGroupingPicker.SelectedIndexChanged += (sender, e) =>
            {
                App.StuffListGroupingMethod.Value = StuffListGroupingPicker.SelectedIndex;
                StuffListGroupingLabel2.Text = StuffListGroupings[StuffListGroupingPicker.SelectedIndex];

                DBRepository._AllStuffsDataInitialized = false;
            };

            StuffListGroupingPicker.SelectedIndex = StuffListGroupingMethod;
        }
    }
}
