using Kara.Assets;
using Kara.CustomRenderer;
using Kara.Models;
using Plugin.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace Kara
{
    public partial class PartnerListForm : TabbedPage
    {
        public int dayOffset = 0;
        public bool includeVisiteds = false;
        public bool nearbyCustomers = false;
        public bool searchCustomers = false;
        public ObservableCollection<DBRepository.PartnerListModel> Partners;
        public OrderInsertForm OrderInsertForm;
        public OrderBeforePreviewForm OrderBeforePreviewForm;
        public FailedOrderInsertForm FailedOrderInsertForm;
        public PartnerReportForm PartnerReportForm;

        public PartnerListForm_ListView ListView;
        public PartnerListForm_MapView MapView;
        public ToolbarItem ToolbarItem_NearbyCustomers, ToolbarItem_SearchBar, ToolbarItem_OrderInsert, ToolbarItem_FailedOrderInsert, ToolbarItem_NextDay, ToolbarItem_PreDay, ToolbarItem_PartnerEdit, ToolbarItem_PartnerAdd, ToolbarItem_PartnerReport;

        private double _NearbyCustomers_DistanceSlider;
        private int? _NearbyCustomers_Distance;
        public double NearbyCustomers_DistanceSlider
        {
            get { return _NearbyCustomers_DistanceSlider; }
            set
            {
                _NearbyCustomers_DistanceSlider = value;
                ListView.NearbyCustomers_DistanceSlider.Value = value;
                MapView.NearbyCustomers_DistanceSlider.Value = value;
                new SettingField<double>("NearbyCustomersDistance", _NearbyCustomers_DistanceSlider).Value = _NearbyCustomers_DistanceSlider;
                
                _NearbyCustomers_Distance = nearbyCustomers ? ((int)(20 * Math.Pow(NearbyCustomers_Base, _NearbyCustomers_DistanceSlider / NearbyCustomers_PowerDivider))) : new Nullable<int>();
                if (_NearbyCustomers_Distance.HasValue)
                {
                    _NearbyCustomers_Distance = _NearbyCustomers_Distance < 1000 ? _NearbyCustomers_Distance : (int)(Math.Round((double)_NearbyCustomers_Distance / 100) * 100);
                    ListView.NearbyCustomers_DistanceDisplay.Text = (_NearbyCustomers_Distance < 1000 ? (_NearbyCustomers_Distance + " متر") : ((_NearbyCustomers_Distance / 1000.0).Value.ToString("##0.#") + " کیلومتر")).ReplaceLatinDigits();
                    MapView.NearbyCustomers_DistanceDisplay.Text = (_NearbyCustomers_Distance < 1000 ? (_NearbyCustomers_Distance + " متر") : ((_NearbyCustomers_Distance / 1000.0).Value.ToString("##0.#") + " کیلومتر")).ReplaceLatinDigits();
                }

                FillPartners();
            }
        }
        public double NearbyCustomers_Base = Math.Pow(1000, 0.15);
        public double NearbyCustomers_PowerDivider = 15;
        LocationModel LastLocation = null;

        public PartnerListForm()
        {
            InitializeComponent();

            ListView = new PartnerListForm_ListView(this) { StartColor = Color.FromHex("E6EBEF"), EndColor = Color.FromHex("A6CFED") };
            MapView = new PartnerListForm_MapView(this) { StartColor = Color.FromHex("E6EBEF"), EndColor = Color.FromHex("A6CFED") };

            App.LocationChanged += (sender, e) => {
                var NewLocation = ((App.LocationChangedEventArgs)e).NewLocation;
                var ChangeLocation = false;
                if (LastLocation == null)
                    ChangeLocation = true;
                else if (new Position(LastLocation.Latitude.Value, LastLocation.Longitude.Value).meterDistanceBetweenPoints(new Position(NewLocation.Latitude.Value, NewLocation.Longitude.Value)) > 50)
                    ChangeLocation = true;
                if(ChangeLocation)
                {
                    if (nearbyCustomers && (LastSelectedPartner == null || !LastSelectedPartner.Selected))
                    {
                        LastLocation = NewLocation;
                        FillPartners();
                    }
                }
            };

            Children.Add(MapView);
            Children.Add(ListView);

            ToolbarItem_SearchBar = new ToolbarItem();
            ToolbarItem_SearchBar.Text = "جستجو";
            ToolbarItem_SearchBar.Icon = "Search.png";
            ToolbarItem_SearchBar.Order = ToolbarItemOrder.Primary;
            ToolbarItem_SearchBar.Priority = 10;
            this.ToolbarItems.Add(ToolbarItem_SearchBar);
            ToolbarItem_SearchBar.Activated += ToolbarItem_SearchBar_Activated;
            searchCustomers = new SettingField<bool>("SearchCustomers", false).Value;
            if (searchCustomers)
            {
                ToolbarItem_SearchBar.Icon = "Search.png";
                ListView.PartnersSearchBar.IsVisible = true;
                ListView.PartnersSearchBar.FadeTo(1);
                MapView.PartnersSearchBar.IsVisible = true;
                MapView.PartnersSearchBar.FadeTo(1);
            }
            else
            {
                ToolbarItem_SearchBar.Icon = "SearchDeactive.png";
                ListView.PartnersSearchBar.IsVisible = false;
                ListView.PartnersSearchBar.FadeTo(0);
                MapView.PartnersSearchBar.IsVisible = false;
                MapView.PartnersSearchBar.FadeTo(0);
            }

            ToolbarItem_NearbyCustomers = new ToolbarItem();
            ToolbarItem_NearbyCustomers.Text = "مشتریان اطراف من";
            ToolbarItem_NearbyCustomers.Icon = "NearbyPerson.png";
            ToolbarItem_NearbyCustomers.Order = ToolbarItemOrder.Primary;
            ToolbarItem_NearbyCustomers.Priority = 10;
            ToolbarItem_NearbyCustomers.Activated += ToolbarItem_NearbyCustomers_Activated;
            this.ToolbarItems.Add(ToolbarItem_NearbyCustomers);
            
            nearbyCustomers = new SettingField<bool>("NearbyCustomers", false).Value;
            NearbyCustomers_DistanceSlider = new SettingField<double>("NearbyCustomersDistance", 0).Value;
            if (nearbyCustomers)
            {
                ToolbarItem_NearbyCustomers.Icon = "NearbyPerson.png";
                ListView.NearbyCustomersSection.IsVisible = true;
                MapView.NearbyCustomersSection.IsVisible = true;
            }
            else
            {
                ToolbarItem_NearbyCustomers.Icon = "NearbyPersonDeactive.png";
                ListView.NearbyCustomersSection.IsVisible = false;
                MapView.NearbyCustomersSection.IsVisible = false;
            }
            
            ToolbarItem_FailedOrderInsert = new ToolbarItem();
            ToolbarItem_FailedOrderInsert.Text = "عدم سفارش";
            ToolbarItem_FailedOrderInsert.Icon = "AddFailedInvoice.png";
            ToolbarItem_FailedOrderInsert.Order = ToolbarItemOrder.Primary;
            ToolbarItem_FailedOrderInsert.Priority = 1;
            ToolbarItem_FailedOrderInsert.Activated += GoToFailedOrderInsertForm;

            ToolbarItem_OrderInsert = new ToolbarItem();
            ToolbarItem_OrderInsert.Text = "ثبت سفارش";
            ToolbarItem_OrderInsert.Icon = "AddInvoice.png";
            ToolbarItem_OrderInsert.Order = ToolbarItemOrder.Primary;
            ToolbarItem_OrderInsert.Priority = 2;
            ToolbarItem_OrderInsert.Activated += GoToOrderInsertForm;

            ToolbarItem_PartnerReport = new ToolbarItem();
            ToolbarItem_PartnerReport.Text = "گردش حساب";
            ToolbarItem_PartnerReport.Icon = "PartnerTurnover.png";
            ToolbarItem_PartnerReport.Order = ToolbarItemOrder.Primary;
            ToolbarItem_PartnerReport.Priority = 3;
            ToolbarItem_PartnerReport.Activated += GoToPartnerReportForm;

            if (App.Accesses.PartnerInsert)
            {
                ToolbarItem_PartnerAdd = new ToolbarItem();
                ToolbarItem_PartnerAdd.Text = "مشتری جدید";
                ToolbarItem_PartnerAdd.Icon = "AddCustomer.png";
                ToolbarItem_PartnerAdd.Order = ToolbarItemOrder.Primary;
                ToolbarItem_PartnerAdd.Priority = 0;
                //ListView.ToolbarItem_PartnerAdd_Visible = true;
                //MapView.ToolbarItem_PartnerAdd_Visible = true;
                ToolbarItem_PartnerAdd.Activated += ToolbarItem_PartnerAdd_Activated;
            }

            if (App.Accesses.PartnerUpdate)
            {
                ToolbarItem_PartnerEdit = new ToolbarItem();
                ToolbarItem_PartnerEdit.Text = "ویرایش";
                ToolbarItem_PartnerEdit.Icon = "EditCustomer.png";
                ToolbarItem_PartnerEdit.Order = ToolbarItemOrder.Primary;
                ToolbarItem_PartnerEdit.Priority = 0;
                ToolbarItem_PartnerEdit.Activated += ToolbarItem_PartnerEdit_Activated;
            }

            ToolbarItem_PreDay = new ToolbarItem();
            ToolbarItem_PreDay.Text = "روز قبل";
            ToolbarItem_PreDay.Icon = "PreDay.png";
            ToolbarItem_PreDay.Order = ToolbarItemOrder.Primary;
            ToolbarItem_PreDay.Priority = 3;
            ToolbarItem_PreDay.Activated += GoToPreDay;

            ToolbarItem_NextDay = new ToolbarItem();
            ToolbarItem_NextDay.Text = "روز بعد";
            ToolbarItem_NextDay.Icon = "NextDay.png";
            ToolbarItem_NextDay.Order = ToolbarItemOrder.Primary;
            ToolbarItem_NextDay.Priority = 4;
            ToolbarItem_NextDay.Activated += GoToNextDay;


            this.CurrentPageChanged += (sender, e) => {
                if (this.CurrentPage == ListView)
                {
                    ListView.SetCurrentPage();
                    MapView.UnsetCurrentPage();
                    new SettingField<bool>("PartnerList_ListView", true).Value = true;
                }
                if (this.CurrentPage == MapView)
                {
                    ListView.UnsetCurrentPage();
                    MapView.SetCurrentPage();
                    new SettingField<bool>("PartnerList_ListView", false).Value = false;
                }
            };

            var DefaultViewIsListView = new SettingField<bool>("PartnerList_ListView", true).Value;
            CurrentPage = DefaultViewIsListView ? (Page)ListView : (Page)MapView;
            
            includeVisiteds = new SettingField<bool>("PartnerList_IncludeVisiteds", false).Value;
            ListView.IncludeVisitedsSwitch.IsToggled = includeVisiteds;
        }

        private void ToolbarItem_SearchBar_Activated(object sender, EventArgs e)
        {
            searchCustomers = !searchCustomers;
            new SettingField<bool>("SearchCustomers", searchCustomers).Value = searchCustomers;
            if (searchCustomers)
            {
                ToolbarItem_SearchBar.Icon = "Search.png";

                ListView.PartnersSearchBar.IsVisible = true;
                ListView.PartnersSearchBar.FadeTo(1);
                MapView.PartnersSearchBar.IsVisible = true;
                MapView.PartnersSearchBar.FadeTo(1);
            }
            else
            {
                ToolbarItem_SearchBar.Icon = "SearchDeactive.png";

                ListView.PartnersSearchBar.IsVisible = false;
                ListView.PartnersSearchBar.FadeTo(0);
                MapView.PartnersSearchBar.IsVisible = false;
                MapView.PartnersSearchBar.FadeTo(0);
            }

            FillPartners();
        }

        private void ToolbarItem_NearbyCustomers_Activated(object sender, EventArgs e)
        {
            nearbyCustomers = !nearbyCustomers;
            new SettingField<bool>("NearbyCustomers", nearbyCustomers).Value = nearbyCustomers;
            if (nearbyCustomers)
            {
                ToolbarItem_NearbyCustomers.Icon = "NearbyPerson.png";

                ListView.NearbyCustomersSection.IsVisible = true;
                ListView.NearbyCustomersSection.FadeTo(1);
                MapView.NearbyCustomersSection.IsVisible = true;
                MapView.NearbyCustomersSection.FadeTo(1);
            }
            else
            {
                ToolbarItem_NearbyCustomers.Icon = "NearbyPersonDeactive.png";

                ListView.NearbyCustomersSection.IsVisible = false;
                ListView.NearbyCustomersSection.FadeTo(0);
                MapView.NearbyCustomersSection.IsVisible = false;
                MapView.NearbyCustomersSection.FadeTo(0);
            }
            NearbyCustomers_DistanceSlider = new SettingField<double>("NearbyCustomersDistance", 0).Value;

            FillPartners();
        }

        private void ToolbarItem_PartnerAdd_Activated(object sender, EventArgs e)
        {
            Navigation.PushAsync(new PartnerChange(null, this, null, false)
            {
                StartColor = Color.FromHex("E6EBEF"),
                EndColor = Color.FromHex("A6CFED")
            });
        }

        public DBRepository.PartnerListModel LastSelectedPartner = null;
        private void ToolbarItem_PartnerEdit_Activated(object sender, EventArgs e)
        {
            Navigation.PushAsync(new PartnerChange(LastSelectedPartner.PartnerData, this, null, false)
            {
                StartColor = Color.FromHex("E6EBEF"),
                EndColor = Color.FromHex("A6CFED")
            });
        }

        private void GoToOrderInsertForm(object sender, EventArgs e)
        {
            var OrderInsertForm = new OrderInsertForm(LastSelectedPartner.PartnerData, null, null, this, null)
            {
                StartColor = Color.FromHex("E6EBEF"),
                EndColor = Color.FromHex("A6CFED")
            };
            Navigation.PushAsync(OrderInsertForm);
        }

        private void GoToPartnerReportForm(object sender, EventArgs e)
        {
            var PartnerReportForm = new PartnerReportForm(LastSelectedPartner.PartnerData.Id);
            Navigation.PushAsync(PartnerReportForm);
        }

        private void GoToFailedOrderInsertForm(object sender, EventArgs e)
        {
            Navigation.PushAsync(new FailedOrderInsertForm(LastSelectedPartner.PartnerData, null, null, this)
            {
                StartColor = Color.FromHex("E6EBEF"),
                EndColor = Color.FromHex("A6CFED")
            });
        }

        public void FilterChanged(object sender, EventArgs e)
        {
            includeVisiteds = CurrentPage == ListView ? ListView.IncludeVisitedsSwitch.IsToggled : MapView.IncludeVisitedsSwitch.IsToggled;
            ListView.IncludeVisitedsSwitch.IsToggled = includeVisiteds;
            MapView.IncludeVisitedsSwitch.IsToggled = includeVisiteds;
            new SettingField<bool>("PartnerList_IncludeVisiteds", includeVisiteds).Value = includeVisiteds;
            FillPartners(CurrentPage == ListView ? ListView.PartnersSearchBar.Text : MapView.PartnersSearchBar.Text);
        }

        public void RefreshToolbarItems
        (
            bool ToolbarItem_OrderInsert_Visible,
            bool ToolbarItem_FailedOrderInsert_Visible,
            bool ToolbarItem_NextDay_Visible,
            bool ToolbarItem_PreDay_Visible,
            bool ToolbarItem_PartnerEdit_Visible,
            bool ToolbarItem_PartnerAdd_Visible,
            bool ToolbarItem_PartnerReport_Visible
        )
        {
            if (ToolbarItem_OrderInsert_Visible && !this.ToolbarItems.Contains(ToolbarItem_OrderInsert))
                this.ToolbarItems.Add(ToolbarItem_OrderInsert);
            if (!ToolbarItem_OrderInsert_Visible && this.ToolbarItems.Contains(ToolbarItem_OrderInsert))
                this.ToolbarItems.Remove(ToolbarItem_OrderInsert);

            if (App.Accesses.AccessToViewPartnerRemainder)
            {
                if (ToolbarItem_PartnerReport_Visible && !this.ToolbarItems.Contains(ToolbarItem_PartnerReport))
                    this.ToolbarItems.Add(ToolbarItem_PartnerReport);
                if (!ToolbarItem_PartnerReport_Visible && this.ToolbarItems.Contains(ToolbarItem_PartnerReport))
                    this.ToolbarItems.Remove(ToolbarItem_PartnerReport);
            }

            if (ToolbarItem_FailedOrderInsert_Visible && !this.ToolbarItems.Contains(ToolbarItem_FailedOrderInsert))
                this.ToolbarItems.Add(ToolbarItem_FailedOrderInsert);
            if (!ToolbarItem_FailedOrderInsert_Visible && this.ToolbarItems.Contains(ToolbarItem_FailedOrderInsert))
                this.ToolbarItems.Remove(ToolbarItem_FailedOrderInsert);

            if (ToolbarItem_NextDay_Visible && !this.ToolbarItems.Contains(ToolbarItem_NextDay))
                this.ToolbarItems.Add(ToolbarItem_NextDay);
            if (!ToolbarItem_NextDay_Visible && this.ToolbarItems.Contains(ToolbarItem_NextDay))
                this.ToolbarItems.Remove(ToolbarItem_NextDay);

            if (ToolbarItem_PreDay_Visible && !this.ToolbarItems.Contains(ToolbarItem_PreDay))
                this.ToolbarItems.Add(ToolbarItem_PreDay);
            if (!ToolbarItem_PreDay_Visible && this.ToolbarItems.Contains(ToolbarItem_PreDay))
                this.ToolbarItems.Remove(ToolbarItem_PreDay);

            if (App.Accesses.PartnerUpdate)
            {
                if (ToolbarItem_PartnerEdit_Visible && !this.ToolbarItems.Contains(ToolbarItem_PartnerEdit))
                    this.ToolbarItems.Add(ToolbarItem_PartnerEdit);
                if (!ToolbarItem_PartnerEdit_Visible && this.ToolbarItems.Contains(ToolbarItem_PartnerEdit))
                    this.ToolbarItems.Remove(ToolbarItem_PartnerEdit);
            }

            if (App.Accesses.PartnerInsert)
            {
                if (ToolbarItem_PartnerAdd_Visible && !this.ToolbarItems.Contains(ToolbarItem_PartnerAdd))
                    this.ToolbarItems.Add(ToolbarItem_PartnerAdd);
                if (!ToolbarItem_PartnerAdd_Visible && this.ToolbarItems.Contains(ToolbarItem_PartnerAdd))
                    this.ToolbarItems.Remove(ToolbarItem_PartnerAdd);
            }
        }

        Guid LastFillPartnersId = Guid.NewGuid();
        public async Task FillPartners(string Filter)
        {
            var ThisFillPartnersId = Guid.NewGuid();
            LastFillPartnersId = ThisFillPartnersId;
            await Task.Delay(600);
            if (LastFillPartnersId != ThisFillPartnersId) return;

            ListView.PartnerItems.IsRefreshing = true;
            await Task.Delay(100);

            this.Title = !App.UseVisitProgram.Value ? "" : ("لیست " + (
                dayOffset == 0 ? "امروز" :
                dayOffset == 1 ? "فردا" :
                dayOffset == -1 ? "دیروز" :
                dayOffset > 1 ? dayOffset + " روز بعد" : -dayOffset + " روز قبل").ReplaceLatinDigits());
            
            var PartnersResult = App.UseVisitProgram.Value ?
                await App.DB.GetDayPartnersListAsync(dayOffset, includeVisiteds, _NearbyCustomers_Distance, searchCustomers ? Filter.ReplacePersianDigits() : "") :
                await App.DB.GetAllPartnersListAsync(false, null, null, _NearbyCustomers_Distance, searchCustomers ? Filter.ReplacePersianDigits() : "");
            if (!PartnersResult.Success)
            {
                App.ShowError("خطا", "در نمایش لیست مشتریان خطایی رخ داد.\n" + PartnersResult.Message, "خوب");
                ListView.PartnerItems.IsRefreshing = false;
                return;
            }
            Partners = new ObservableCollection<DBRepository.PartnerListModel>(PartnersResult.Data.Select(a => new DBRepository.PartnerListModel()
            {
                Id = a.Id,
                Code = a.Code.StartsWith("-") ? "---" : a.Code.ReplaceLatinDigits(),
                Name = (a.Name + (App.ShowPartnerLegalNameInList.Value ? (" (" + a.LegalName + ")") : "")).ReplaceLatinDigits(),
                Group = !string.IsNullOrWhiteSpace(a.Group) ? a.Group.ReplaceLatinDigits() : "---",
                Zone = a.Zone.ReplaceLatinDigits(),
                Address = (a.Zone + " - " + a.Address).ReplaceLatinDigits(),
                Phone = a.Phone.ReplaceLatinDigits(),
                HasOrder = a.HasOrder,
                HasFailedVisit = a.HasFailedVisit,
                Sent = a.Sent,
                ForChangedPartnersList = a.ForChangedPartnersList,
                PartnerData = a.PartnerData,
                DistanceFromMe = a.DistanceFromMe
            }));
            ListView.PartnerItems.ItemsSource = null;
            ListView.PartnerItems.ItemsSource = Partners;

            ListView.ToolbarItem_FailedOrderInsert_Visible = false;
            ListView.ToolbarItem_OrderInsert_Visible = false;
            ListView.ToolbarItem_PartnerEdit_Visible = false;
            ListView.ToolbarItem_PartnerReport_Visible = false;
            ListView.ToolbarItem_PartnerAdd_Visible = true;

            MapView.ToolbarItem_FailedOrderInsert_Visible = false;
            MapView.ToolbarItem_OrderInsert_Visible = false;
            MapView.ToolbarItem_PartnerEdit_Visible = false;
            MapView.ToolbarItem_PartnerReport_Visible = false;
            MapView.ToolbarItem_PartnerAdd_Visible = true;

            if (App.UseVisitProgram.Value)
            {
                ListView.ToolbarItem_NextDay_Visible = dayOffset != App.Accesses.AccessToNextDay;
                MapView.ToolbarItem_NextDay_Visible = dayOffset != App.Accesses.AccessToNextDay;

                ListView.ToolbarItem_PreDay_Visible = dayOffset != -App.Accesses.AccessToPreDay;
                MapView.ToolbarItem_PreDay_Visible = dayOffset != -App.Accesses.AccessToPreDay;
            }

            ListView.PartnerItems.IsRefreshing = false;
            await Task.Delay(100);
            ListView.PartnerItems.IsRefreshing = false;

            var PartnersWithLocation = Partners.Where(a => a.PartnerData.Latitude.HasValue).Select(a => new {
                Location = new Position(a.PartnerData.Latitude.Value, a.PartnerData.Longitude.Value),
                Data = a
            }).ToArray();
            var CustomPins = new List<CustomPin>();
            foreach (var item in PartnersWithLocation)
            {
                var customerPin = new CustomPin
                {
                    Pin = new Pin
                    {
                        Type = PinType.Place,
                        Position = item.Location,
                        Label = "",
                        Address = ""
                    },
                    Id = item.Data.PartnerData.Id.ToString(),
                    Data = item.Data,
                    Icon = item.Data.HasOrder ? MapIcon.GreenBallon : item.Data.HasFailedVisit ? MapIcon.RedBallon : MapIcon.BlueBallon
                };
                
                CustomPins.Add(customerPin);
                MapView.Map.Pins.Add(customerPin.Pin);

                customerPin.Pin.Clicked += (object sender, EventArgs e) =>
                {
                    var p = sender as Pin;
                };
            }
            try
            {
                MapView.Map.CustomPins = CustomPins;
            }
            catch (Exception)
            {
            }
            
            if (PartnersWithLocation.Any())
            {
                MapSpan MapSpan = null;
                if (_NearbyCustomers_Distance.HasValue)
                {
                    var CurrentPosition = new Position(App.LastLocation.Latitude.Value, App.LastLocation.Longitude.Value);
                    MapSpan = MapSpan.FromCenterAndRadius(CurrentPosition, Distance.FromMeters(_NearbyCustomers_Distance.Value));
                }
                else
                {
                    var MinLat = PartnersWithLocation.Min(a => a.Location.Latitude);
                    var MaxLat = PartnersWithLocation.Max(a => a.Location.Latitude);
                    var MinLong = PartnersWithLocation.Min(a => a.Location.Longitude);
                    var MaxLong = PartnersWithLocation.Max(a => a.Location.Longitude);
                    var CenterOfPins = new Position((MinLat + MaxLat) / 2, (MinLong + MaxLong) / 2);
                    var latlongdegrees = Math.Max(MaxLat - MinLat, MaxLong - MinLong) * 2;
                    if (latlongdegrees == 0)
                        MapSpan = MapSpan.FromCenterAndRadius(CenterOfPins, Distance.FromMiles(0.5));
                    else
                        MapSpan = new MapSpan(CenterOfPins, latlongdegrees, latlongdegrees);
                }

                MapView.Map.MoveToRegion(MapSpan);
            }
            else
            {
                if (App.LastLocation != null && App.LastLocation.Latitude.HasValue)
                    MapView.Map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(App.LastLocation.Latitude.Value, App.LastLocation.Longitude.Value), Distance.FromMiles(0.5)));
                else
                    MapView.Map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(32.7295492, 53.9990942), Distance.FromMiles(650 * 360 / Math.Min(MapView.Map.Width, MapView.Map.Height))));
            }
        }

        //protected override void OnAppearing()
        //{
        //    base.OnAppearing();

        //    MapView.Map.MoveToMapRegion(Center, true); // all right!
        //}

        public async Task FillPartners()
        {
            FillPartners(CurrentPage == ListView ? ListView.PartnersSearchBar.Text : MapView.PartnersSearchBar.Text);
        }

        private void GoToNextDay(object sender, EventArgs e)
        {
            if (dayOffset < App.Accesses.AccessToNextDay)
                dayOffset++;

            FillPartners(CurrentPage == ListView ? ListView.PartnersSearchBar.Text : MapView.PartnersSearchBar.Text);
        }
        private void GoToPreDay(object sender, EventArgs e)
        {
            if (dayOffset > -App.Accesses.AccessToPreDay)
                dayOffset--;

            FillPartners(CurrentPage == ListView ? ListView.PartnersSearchBar.Text : MapView.PartnersSearchBar.Text);
        }
    }
}
