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

namespace Kara
{
    public class CustomPartnerListCell : ViewCell
    {
        public static bool HasGroupColumn = true;
        public CustomPartnerListCell()
        {
            Grid GridWrapper = new Grid()
            {
                Padding = new Thickness(5, 0),
                RowSpacing = 1,
                ColumnSpacing = 1,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
            GridWrapper.SetBinding(Grid.BackgroundColorProperty, "RowColor");
            GridWrapper.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
            GridWrapper.RowDefinitions.Add(new RowDefinition() { Height = 25 });

            GridWrapper.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(12, GridUnitType.Star) });
            if (HasGroupColumn)
                GridWrapper.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(10, GridUnitType.Star) });
            GridWrapper.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength((App.ShowPartnerLegalNameInList.Value ? 15 : 0) + 20, GridUnitType.Star) });
            GridWrapper.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(8, GridUnitType.Star) });

            Label PhoneLabel = null, GroupLabel = null, NameLabel = null, CodeLabel = null, AddressLabel = null;
            PhoneLabel = new Label() { LineBreakMode = LineBreakMode.WordWrap, TextColor = Color.FromHex("1845A5"), HorizontalOptions = LayoutOptions.Start, HorizontalTextAlignment = TextAlignment.Start, VerticalOptions = LayoutOptions.Center, FontSize = 16 };
            if (HasGroupColumn)
                GroupLabel = new Label() { LineBreakMode = LineBreakMode.TailTruncation, TextColor = Color.FromHex("222"), HorizontalOptions = LayoutOptions.End, HorizontalTextAlignment = TextAlignment.End, VerticalOptions = LayoutOptions.Center, FontSize = 16 };
            NameLabel = new Label() { LineBreakMode = LineBreakMode.TailTruncation, TextColor = Color.FromHex("222"), HorizontalOptions = LayoutOptions.End, HorizontalTextAlignment = TextAlignment.End, VerticalOptions = LayoutOptions.Center, FontSize = 16 };
            CodeLabel = new Label() { LineBreakMode = LineBreakMode.TailTruncation, TextColor = Color.FromHex("222"), HorizontalOptions = LayoutOptions.End, HorizontalTextAlignment = TextAlignment.End, VerticalOptions = LayoutOptions.Center, FontSize = 16 };
            AddressLabel = new Label() { LineBreakMode = LineBreakMode.TailTruncation, TextColor = Color.FromHex("777"), HorizontalOptions = LayoutOptions.End, HorizontalTextAlignment = TextAlignment.End, VerticalOptions = LayoutOptions.Center, FontSize = 13 };
            GridWrapper.Children.Add(PhoneLabel, 0, 0);
            if (HasGroupColumn)
                GridWrapper.Children.Add(GroupLabel, 1, 0);
            GridWrapper.Children.Add(NameLabel, HasGroupColumn ? 2 : 1, 0);
            GridWrapper.Children.Add(CodeLabel, HasGroupColumn ? 3 : 2, 0);
            GridWrapper.Children.Add(AddressLabel, 0, 1);
            Grid.SetColumnSpan(AddressLabel, HasGroupColumn ? 4 : 3);

            PhoneLabel.SetBinding(Label.TextProperty, "Phone");
            if (HasGroupColumn)
                GroupLabel.SetBinding(Label.TextProperty, "Group");
            NameLabel.SetBinding(Label.TextProperty, "Name");
            CodeLabel.SetBinding(Label.TextProperty, "Code");
            AddressLabel.SetBinding(Label.TextProperty, "Address");
            
            View = GridWrapper;
        }
    }

    public partial class PartnerListForm_ListView : GradientContentPage
    {
        PartnerListForm PartnerListForm;
        public ListView PartnerItems { get { return _PartnerItems; } }
        public SearchBar PartnersSearchBar { get { return _PartnersSearchBar; } }
        public StackLayout FiltersSection { get { return _FiltersSection; } }
        public Switch IncludeVisitedsSwitch { get { return _IncludeVisitedsSwitch; } }
        public StackLayout NearbyCustomersSection { get { return _NearbyCustomersSection; } }
        public Slider NearbyCustomers_DistanceSlider { get { return _NearbyCustomers_DistanceSlider; } }
        public Label NearbyCustomers_DistanceDisplay { get { return _NearbyCustomers_DistanceDisplay; } }

        private bool _ToolbarItem_OrderInsert_Visible, _ToolbarItem_FailedOrderInsert_Visible, _ToolbarItem_NextDay_Visible, _ToolbarItem_PreDay_Visible, _ToolbarItem_PartnerEdit_Visible, _ToolbarItem_PartnerAdd_Visible, _ToolbarItem_PartnerReport_Visible;
        public bool ToolbarItem_OrderInsert_Visible { get { return _ToolbarItem_OrderInsert_Visible; } set { _ToolbarItem_OrderInsert_Visible = value; RefreshToolbarItems(); } }
        public bool ToolbarItem_FailedOrderInsert_Visible { get { return _ToolbarItem_FailedOrderInsert_Visible; } set { _ToolbarItem_FailedOrderInsert_Visible = value; RefreshToolbarItems(); } }
        public bool ToolbarItem_NextDay_Visible { get { return _ToolbarItem_NextDay_Visible; } set { _ToolbarItem_NextDay_Visible = value; RefreshToolbarItems(); } }
        public bool ToolbarItem_PreDay_Visible { get { return _ToolbarItem_PreDay_Visible; } set { _ToolbarItem_PreDay_Visible = value; RefreshToolbarItems(); } }
        public bool ToolbarItem_PartnerEdit_Visible { get { return _ToolbarItem_PartnerEdit_Visible; } set { _ToolbarItem_PartnerEdit_Visible = value; RefreshToolbarItems(); } }
        public bool ToolbarItem_PartnerAdd_Visible { get { return _ToolbarItem_PartnerAdd_Visible; } set { _ToolbarItem_PartnerAdd_Visible = value; RefreshToolbarItems(); } }
        public bool ToolbarItem_PartnerReport_Visible { get { return _ToolbarItem_PartnerReport_Visible; } set { _ToolbarItem_PartnerReport_Visible = value; RefreshToolbarItems(); } }

        public PartnerListForm_ListView(PartnerListForm PartnerListForm)
        {
            InitializeComponent();

            Title = "لیست";

            _NearbyCustomers_DistanceSlider.Minimum = 0;
            _NearbyCustomers_DistanceSlider.Maximum = 100;
            _NearbyCustomers_DistanceSlider.Value = 0.000324684131;
            _NearbyCustomers_DistanceSlider.ValueChanged += (sender, e) => {
                if(Math.Abs(PartnerListForm.NearbyCustomers_DistanceSlider - _NearbyCustomers_DistanceSlider.Value) >= 0.5)
                    PartnerListForm.NearbyCustomers_DistanceSlider = _NearbyCustomers_DistanceSlider.Value;
            };
            
            this.PartnerListForm = PartnerListForm;
            
            CustomPartnerListCell.HasGroupColumn = App.ShowPartnerGroupInList.Value;
            PartnerItems.ItemTemplate = new DataTemplate(typeof(CustomPartnerListCell));

            PartnerItems.ItemSelected += PartnerItems_ItemSelected;
            PartnerItems.ItemTapped += PartnerItems_ItemTapped;
            PartnerItems.SeparatorColor = Color.FromHex("A5ABB7");
            PartnerItems.HasUnevenRows = true;
            
            PartnersSearchBar.TextChanged += async (sender, args) => {
                if (CurrentPageSet)
                {
                    await PartnerListForm.FillPartners(args.NewTextValue);
                    if (PartnerListForm.MapView.PartnersSearchBar.Text != args.NewTextValue)
                        PartnerListForm.MapView.PartnersSearchBar.Text = args.NewTextValue;
                }
            };
            PartnersSearchBar.SearchButtonPressed += async (sender, args) => {
                await PartnersSearchBar.FadeTo(0);
                PartnersSearchBar.IsVisible = false;
                PartnerListForm.MapView.PartnersSearchBar.IsVisible = false;
                if (App.UseVisitProgram.Value)
                {
                    FiltersSection.IsVisible = true;
                    await FiltersSection.FadeTo(1);
                    PartnerListForm.MapView.FiltersSection.IsVisible = true;
                    await PartnerListForm.MapView.FiltersSection.FadeTo(1);
                }
            };
            
            IncludeVisitedsSwitch.Toggled += PartnerListForm.FilterChanged;
            if (!App.UseVisitProgram.Value)
                FiltersSection.IsVisible = false;
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
        
        private void PartnerItems_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var TappedItem = (DBRepository.PartnerListModel)e.Item;
            var ShowingPin = PartnerListForm.MapView.Map.ShowingPin;
            if (TappedItem == PartnerListForm.LastSelectedPartner)
            {
                PartnerListForm.LastSelectedPartner = null;
                TappedItem.Selected = false;

                PartnerListForm.MapView.Map.NotifyChangeShowingPin();

                ToolbarItem_FailedOrderInsert_Visible = false;
                ToolbarItem_OrderInsert_Visible = false;
                ToolbarItem_PartnerEdit_Visible = false;
                ToolbarItem_PartnerReport_Visible = false;

                PartnerListForm.MapView.ToolbarItem_FailedOrderInsert_Visible = false;
                PartnerListForm.MapView.ToolbarItem_OrderInsert_Visible = false;
                PartnerListForm.MapView.ToolbarItem_PartnerEdit_Visible = false;
                PartnerListForm.MapView.ToolbarItem_PartnerReport_Visible = false;

                ToolbarItem_PartnerAdd_Visible = true;

                PartnerListForm.MapView.ToolbarItem_PartnerAdd_Visible = true;
            }
            else
            {
                if (PartnerListForm.LastSelectedPartner != null)
                    PartnerListForm.LastSelectedPartner.Selected = false;
                PartnerListForm.LastSelectedPartner = TappedItem;
                PartnerListForm.LastSelectedPartner.Selected = true;

                PartnerListForm.MapView.Map.ShowingPin = PartnerListForm.MapView.Map.CustomPins.SingleOrDefault(a => a.Id == TappedItem.Id.ToString());
                PartnerListForm.MapView.Map.NotifyChangeShowingPin();
                
                if (PartnerListForm.OrderInsertForm != null)
                {
                    PartnerListForm.OrderInsertForm.SelectedPartner = TappedItem.PartnerData;
                    try { Navigation.PopAsync(); } catch (Exception) { }
                }
                else if (PartnerListForm.FailedOrderInsertForm != null)
                {
                    PartnerListForm.FailedOrderInsertForm.SelectedPartner = TappedItem.PartnerData;
                    try { Navigation.PopAsync(); } catch (Exception) { }
                }
                else if (PartnerListForm.OrderBeforePreviewForm != null)
                {
                    PartnerListForm.OrderBeforePreviewForm.SelectedPartner = TappedItem.PartnerData;
                    try { Navigation.PopAsync(); } catch (Exception) { }
                }
                else if (PartnerListForm.PartnerReportForm != null)
                {
                    PartnerListForm.PartnerReportForm.SelectedPartnerId = TappedItem.PartnerData.Id;
                    try { Navigation.PopAsync(); } catch (Exception) { }
                }
                else
                {
                    ToolbarItem_FailedOrderInsert_Visible = true;
                    ToolbarItem_OrderInsert_Visible = true;
                    ToolbarItem_PartnerEdit_Visible = true;
                    ToolbarItem_PartnerReport_Visible = true;

                    PartnerListForm.MapView.ToolbarItem_FailedOrderInsert_Visible = true;
                    PartnerListForm.MapView.ToolbarItem_OrderInsert_Visible = true;
                    PartnerListForm.MapView.ToolbarItem_PartnerEdit_Visible = true;
                    PartnerListForm.MapView.ToolbarItem_PartnerReport_Visible = true;
                }
                ToolbarItem_PartnerAdd_Visible = false;

                PartnerListForm.MapView.ToolbarItem_PartnerAdd_Visible = false;
            }
        }

        private void PartnerItems_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ((ListView)sender).SelectedItem = null;
        }
    }
}
