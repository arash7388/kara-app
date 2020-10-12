using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Kara.Assets;
using Kara.Models;
using System.ComponentModel;
using Plugin.Settings;
using Kara.CustomRenderer;
using Xamarin.Forms.Maps;

namespace Kara
{
    public partial class PartnerListForm_MapView : GradientContentPage
    {
        PartnerListForm PartnerListForm;
        public StackLayout PartnersMapContainer { get { return _PartnersMapContainer; } }
        public SearchBar PartnersSearchBar { get { return _PartnersSearchBar; } }
        public StackLayout FiltersSection { get { return _FiltersSection; } }
        public Switch IncludeVisitedsSwitch { get { return _IncludeVisitedsSwitch; } }
        public StackLayout NearbyCustomersSection { get { return _NearbyCustomersSection; } }
        public Slider NearbyCustomers_DistanceSlider { get { return _NearbyCustomers_DistanceSlider; } }
        public Label NearbyCustomers_DistanceDisplay { get { return _NearbyCustomers_DistanceDisplay; } }
        public CustomMap Map;

        private bool _ToolbarItem_OrderInsert_Visible, _ToolbarItem_FailedOrderInsert_Visible, _ToolbarItem_NextDay_Visible, _ToolbarItem_PreDay_Visible, _ToolbarItem_PartnerEdit_Visible, _ToolbarItem_PartnerAdd_Visible, _ToolbarItem_PartnerReport_Visible;
        public bool ToolbarItem_OrderInsert_Visible { get { return _ToolbarItem_OrderInsert_Visible; } set { _ToolbarItem_OrderInsert_Visible = value; RefreshToolbarItems(); } }
        public bool ToolbarItem_FailedOrderInsert_Visible { get { return _ToolbarItem_FailedOrderInsert_Visible; } set { _ToolbarItem_FailedOrderInsert_Visible = value; RefreshToolbarItems(); } }
        public bool ToolbarItem_NextDay_Visible { get { return _ToolbarItem_NextDay_Visible; } set { _ToolbarItem_NextDay_Visible = value; RefreshToolbarItems(); } }
        public bool ToolbarItem_PreDay_Visible { get { return _ToolbarItem_PreDay_Visible; } set { _ToolbarItem_PreDay_Visible = value; RefreshToolbarItems(); } }
        public bool ToolbarItem_PartnerEdit_Visible { get { return _ToolbarItem_PartnerEdit_Visible; } set { _ToolbarItem_PartnerEdit_Visible = value; RefreshToolbarItems(); } }
        public bool ToolbarItem_PartnerAdd_Visible { get { return _ToolbarItem_PartnerAdd_Visible; } set { _ToolbarItem_PartnerAdd_Visible = value; RefreshToolbarItems(); } }
        public bool ToolbarItem_PartnerReport_Visible { get { return _ToolbarItem_PartnerReport_Visible; } set { _ToolbarItem_PartnerReport_Visible = value; RefreshToolbarItems(); } }
        
        public PartnerListForm_MapView(PartnerListForm PartnerListForm)
        {
            InitializeComponent();

            Title = "نقشه";

            _NearbyCustomers_DistanceSlider.Minimum = 0;
            _NearbyCustomers_DistanceSlider.Maximum = 100;
            _NearbyCustomers_DistanceSlider.Value = 0.000324684131;
            _NearbyCustomers_DistanceSlider.ValueChanged += (sender, e) => {
                if(Math.Abs(PartnerListForm.NearbyCustomers_DistanceSlider - _NearbyCustomers_DistanceSlider.Value) >= 0.5)
                    PartnerListForm.NearbyCustomers_DistanceSlider = _NearbyCustomers_DistanceSlider.Value;
            };
            
            this.PartnerListForm = PartnerListForm;

            Map = new CustomMap(MapSpan.FromCenterAndRadius(new Position(32.7295492, 53.9990942), Distance.FromMiles(650)))
            {
                IsShowingUser = true
            };
            Map.CustomPins = new List<CustomPin>();
            PartnersMapContainer.Children.Add(Map);
            Map.ShowingPinChanged += Map_OnShowingPinChanged;
            
            PartnersSearchBar.TextChanged += async (sender, args) => {
                if (CurrentPageSet)
                {
                    await PartnerListForm.FillPartners(args.NewTextValue);
                    if (PartnerListForm.ListView.PartnersSearchBar.Text != args.NewTextValue)
                        PartnerListForm.ListView.PartnersSearchBar.Text = args.NewTextValue;
                }
            };
            PartnersSearchBar.SearchButtonPressed += async (sender, args) => {
                await PartnersSearchBar.FadeTo(0);
                PartnersSearchBar.IsVisible = false;
                PartnerListForm.ListView.PartnersSearchBar.IsVisible = false;
                if (App.UseVisitProgram.Value)
                {
                    FiltersSection.IsVisible = true;
                    await FiltersSection.FadeTo(1);
                    PartnerListForm.ListView.FiltersSection.IsVisible = true;
                    await PartnerListForm.ListView.FiltersSection.FadeTo(1);
                }
            };

            IncludeVisitedsSwitch.Toggled += PartnerListForm.FilterChanged;
            if (!App.UseVisitProgram.Value)
                FiltersSection.IsVisible = false;
        }

        private void Map_OnShowingPinChanged(object sender, EventArgs e)
        {
            var ShowingPin = Map.ShowingPin;
            if (ShowingPin != null && ((DBRepository.PartnerListModel)ShowingPin.Data).Selected)
            {
                PartnerListForm.LastSelectedPartner = (DBRepository.PartnerListModel)Map.ShowingPin.Data;
                PartnerListForm.ListView.PartnerItems.ScrollTo(PartnerListForm.LastSelectedPartner, ScrollToPosition.Center, false);
                
                if (PartnerListForm.OrderInsertForm != null)
                {
                    PartnerListForm.OrderInsertForm.SelectedPartner = PartnerListForm.LastSelectedPartner.PartnerData;
                    try { Navigation.PopAsync(); } catch (Exception) { }
                }
                else if (PartnerListForm.FailedOrderInsertForm != null)
                {
                    PartnerListForm.FailedOrderInsertForm.SelectedPartner = PartnerListForm.LastSelectedPartner.PartnerData;
                    try { Navigation.PopAsync(); } catch (Exception) { }
                }
                else if (PartnerListForm.OrderBeforePreviewForm != null)
                {
                    PartnerListForm.OrderBeforePreviewForm.SelectedPartner = PartnerListForm.LastSelectedPartner.PartnerData;
                    try { Navigation.PopAsync(); } catch (Exception) { }
                }
                else if (PartnerListForm.PartnerReportForm != null)
                {
                    PartnerListForm.PartnerReportForm.SelectedPartnerId = PartnerListForm.LastSelectedPartner.PartnerData.Id;
                    try { Navigation.PopAsync(); } catch (Exception) { }
                }
                else
                {
                    ToolbarItem_FailedOrderInsert_Visible = true;
                    ToolbarItem_OrderInsert_Visible = true;
                    ToolbarItem_PartnerEdit_Visible = true;
                    ToolbarItem_PartnerReport_Visible = true;

                    PartnerListForm.ListView.ToolbarItem_FailedOrderInsert_Visible = true;
                    PartnerListForm.ListView.ToolbarItem_OrderInsert_Visible = true;
                    PartnerListForm.ListView.ToolbarItem_PartnerEdit_Visible = true;
                    PartnerListForm.ListView.ToolbarItem_PartnerReport_Visible = true;
                }
                ToolbarItem_PartnerAdd_Visible = false;

                PartnerListForm.ListView.ToolbarItem_PartnerAdd_Visible = false;
            }
            else
            {
                PartnerListForm.LastSelectedPartner = null;

                ToolbarItem_FailedOrderInsert_Visible = false;
                ToolbarItem_OrderInsert_Visible = false;
                ToolbarItem_PartnerEdit_Visible = false;
                ToolbarItem_PartnerReport_Visible = false;

                PartnerListForm.ListView.ToolbarItem_FailedOrderInsert_Visible = false;
                PartnerListForm.ListView.ToolbarItem_OrderInsert_Visible = false;
                PartnerListForm.ListView.ToolbarItem_PartnerEdit_Visible = false;
                PartnerListForm.ListView.ToolbarItem_PartnerReport_Visible = false;

                ToolbarItem_PartnerAdd_Visible = true;

                PartnerListForm.ListView.ToolbarItem_PartnerAdd_Visible = true;
            }
        }

        bool CurrentPageSet = false;
        public void SetCurrentPage()
        {
            CurrentPageSet = true;
            RefreshToolbarItems();
        }
        public void UnsetCurrentPage()
        {
            CurrentPageSet = false;
        }

        private void RefreshToolbarItems()
        {
            if(CurrentPageSet)
                PartnerListForm.RefreshToolbarItems(ToolbarItem_OrderInsert_Visible, ToolbarItem_FailedOrderInsert_Visible, ToolbarItem_NextDay_Visible, ToolbarItem_PreDay_Visible, ToolbarItem_PartnerEdit_Visible, ToolbarItem_PartnerAdd_Visible, ToolbarItem_PartnerReport_Visible);
        }
    }
}
