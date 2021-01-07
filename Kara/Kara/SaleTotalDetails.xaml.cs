using Kara.Assets;
using Kara.CustomRenderer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XLabs;
using static Kara.Assets.Connectivity;
using Connectivity = Kara.Assets.Connectivity;

namespace Kara
{
    public class SaleTotalDetailsCustomCell : ViewCell
    {
        public static readonly BindableProperty TotalIdProperty = BindableProperty.Create("TotalId", typeof(Guid), typeof(SaleTotalCustomCell), Guid.Empty);
        public Guid TotalId
        {
            get { return (Guid)GetValue(TotalIdProperty); }
            set { SetValue(TotalIdProperty, value); }
        }

        public SaleTotalDetailsCustomCell()
        {
            this.SetBinding(TotalIdProperty, "Id");
        }
    }

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SaleTotalDetails : GradientContentPage
    {
        public ToolbarItem ToolbarItem_Confirm, ToolbarItem_SelectAll;
        private bool ToolbarItem_Confirm_Visible, ToolbarItem_SelectAll_Visible, BackButton_Visible;
        private bool MultiSelectionMode = false;

        Guid SelectedTotalId;
                       
        public ObservableCollection<TotalDetailModel> TotalDetailsObservableCollection { get; private set; }

        bool TapEventHandlingInProgress = false;

        public SaleTotalDetails(Guid totalId)
        {
            InitializeComponent();

            SelectedTotalId = totalId;

            MultiSelectionMode = false;
            TotalDetailModel.Multiselection = false;

            TotalDetailsView.ItemTapped += TotalDetailsView_ItemTapped;
            TotalDetailsView.OnLongClick += TotalDetailsView_OnLongClick;

            ToolbarItem_SelectAll = new ToolbarItem();
            ToolbarItem_SelectAll.Text = "انتخاب همه";
            ToolbarItem_SelectAll.Icon = "SelectAll_Empty.png";
            ToolbarItem_SelectAll.Order = ToolbarItemOrder.Primary;
            ToolbarItem_SelectAll.Priority = 9;
            ToolbarItem_SelectAll.Activated += ToolbarItem_SelectAll_Activated;

            ToolbarItem_Confirm = new ToolbarItem();
            ToolbarItem_Confirm.Text = "تایید پ ف";
            ToolbarItem_Confirm.Icon = "Confirm.png";
            ToolbarItem_Confirm.Order = ToolbarItemOrder.Primary;
            ToolbarItem_Confirm.Priority = 1;
            ToolbarItem_Confirm.Activated += ToolbarItem_Confirm_Activated;

            BusyIndicatorContainder.BackgroundColor = Color.FromRgba(255, 255, 255, 70);
            BusyIndicator.Color = Color.FromRgba(80, 100, 150, 255);

            //Navigate();
        }

        private void ToolbarItem_SelectAll_Activated(object sender, EventArgs e)
        {
            var AllSelected = TotalDetailsObservableCollection.All(a => a.Selected);
            if (AllSelected)
            {
                foreach (var item in TotalDetailsObservableCollection)
                    item.Selected = false;
                ToolbarItem_SelectAll.Icon = "SelectAll_Empty.png";
            }
            else
            {
                foreach (var item in TotalDetailsObservableCollection)
                    item.Selected = true;

                ToolbarItem_SelectAll.Icon = "SelectAll_Full.png";
            }
        }

        private void ToolbarItem_Confirm_Activated(object sender, EventArgs e)
        {
            var selectedOrderIds = TotalDetailsObservableCollection.Where(a => a.Selected).Select(a => a.OrderId).ToList();

            SignPad signPad = new SignPad(selectedOrderIds);

            Navigation.PushModalAsync(signPad, true);
        }

        protected override void OnAppearing()
        {
            var source = CalcOrderedDetails(SelectedTotalId);

            TotalDetailsView.IsVisible = true;
            TotalDetailsObservableCollection = new ObservableCollection<TotalDetailModel>(source.GetAwaiter().GetResult());
            TotalDetailsView.ItemsSource = TotalDetailsObservableCollection;
            base.OnAppearing();
        }

        public async Task Navigate(Location location)
        {           
            //var options = new MapLaunchOptions { NavigationMode = NavigationMode.Driving };
            //Launcher.OpenAsync(location);
            await Map.OpenAsync(location);
        }

        private async void ToolbarItem_Bank_Activated(object sender, EventArgs e)
        {
            //ReceiptBank receiptBank = new ReceiptBank(sumSelected, SelectedPartner.Id)
            //{
            //    StartColor = Color.FromHex("E6EBEF"),
            //    EndColor = Color.FromHex("A6CFED")
            //};

            //try { await Navigation.PushAsync(receiptBank); } catch (Exception) { }
        }
               
        private async void TotalDetailsView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var TappedItem = (TotalDetailModel)e.Item;
            await Navigate(new Location((double)TappedItem.GeoLocation_Lat, (double)TappedItem.GeoLocation_Long));
        }

        protected override bool OnBackButtonPressed()
        {
            if (MultiSelectionMode)
            {
                ExitMultiSelectionMode();
                return true;
            }
            return base.OnBackButtonPressed();
        }

        private async void ExitMultiSelectionMode()
        {
            foreach (var item in TotalDetailsObservableCollection.Where(a => a.Selected))
                item.Selected = false;

            MultiSelectionMode = false;
            TotalDetailModel.Multiselection = false;
            TotalDetailsView.ItemsSource = null;
            TotalDetailsView.ItemsSource = TotalDetailsObservableCollection;
            RefreshToolbarItems();
        }


        public void RefreshToolbarItems()
        {
            ToolbarItem_Confirm_Visible = false;


            if (MultiSelectionMode)
            {
                ToolbarItem_SelectAll_Visible = true;
                var AllOrdersSelected = TotalDetailsObservableCollection.All(a => a.Selected);
                ToolbarItem_SelectAll.Icon = AllOrdersSelected ? "SelectAll_Full.png" : "SelectAll_Empty.png";
            }
            else
            {
                BackButton_Visible = true;
            }

            var SelectedCount = TotalDetailsObservableCollection != null ? TotalDetailsObservableCollection.Count(a => a.Selected) : 0;
            if (SelectedCount == 0)
            {
                //if (!MultiSelectionMode)
                //    ToolbarItem_SearchBar_Visible = true;
                ToolbarItem_Confirm_Visible = false;
            }
            else if (MultiSelectionMode)
            {
                ToolbarItem_Confirm_Visible = true;
            }
            else
            {
                MultiSelectionMode = false;
                TotalDetailModel.Multiselection = false;
                ToolbarItem_Confirm_Visible = false;
                ToolbarItem_SelectAll_Visible = false;
                TotalDetailsObservableCollection.ForEach(item => item.Selected = false);
            }

            RefreshToolbarItems(BackButton_Visible,ToolbarItem_Confirm_Visible, ToolbarItem_SelectAll_Visible);
        }

        public void RefreshToolbarItems
        (
            bool BackButton_Visible,
            bool ToolbarItem_Confirm_Visible,
            bool ToolbarItem_SelectAll_Visible
        )
        {
            if (BackButton_Visible)
                NavigationPage.SetHasBackButton(this, true);
            else
                NavigationPage.SetHasBackButton(this, false);

            if (ToolbarItem_Confirm_Visible && !this.ToolbarItems.Contains(ToolbarItem_Confirm))
                this.ToolbarItems.Add(ToolbarItem_Confirm);
            if (!ToolbarItem_Confirm_Visible && this.ToolbarItems.Contains(ToolbarItem_Confirm))
                this.ToolbarItems.Remove(ToolbarItem_Confirm);

            if (ToolbarItem_SelectAll_Visible && !this.ToolbarItems.Contains(ToolbarItem_SelectAll))
                this.ToolbarItems.Add(ToolbarItem_SelectAll);
            if (!ToolbarItem_SelectAll_Visible && this.ToolbarItems.Contains(ToolbarItem_SelectAll))
                this.ToolbarItems.Remove(ToolbarItem_SelectAll);
        }


        private async Task<List<TotalDetailModel>> CalcOrderedDetails(Guid totalId)
        {
            var orderedResult = new List<TotalDetailModel>();

            try
            {
                BusyIndicatorContainder.IsVisible = true;

                //temp
                //var details = await Connectivity.GetTotalDetails(App.Username.Value, App.Password.Value, App.CurrentVersionNumber, totalId);
                
                var details = new ResultSuccess<List<TotalDetailModel>>() { Success = true };
                
                details.Data = new List<TotalDetailModel>
                {
                                                                                                                    //35.77819690121176, 51.33577083417429 sare ashrafi 3
                                                                                                                    //35.77058910303877, 51.33699392144137 taghato ashrafi niyayesh 2
                                                                                                                    //35.77047516927987, 51.31835622724807 taghato  satari niyayesh 1

                    new TotalDetailModel{EntityCode="1",EntityName="ی لیبل",Address="آدرس", GeoLocation_Lat=(decimal?)35.77047516927987,GeoLocation_Long=(decimal?)51.31835622724807},
                    new TotalDetailModel{EntityCode="2",EntityName="شسی",Address="آدرس", GeoLocation_Lat=(decimal?)35.77058910303877,GeoLocation_Long=(decimal?)51.33699392144137},
                    new TotalDetailModel{EntityCode="3",EntityName="سیبس",Address="آدرس", GeoLocation_Lat=(decimal?)35.77819690121176,GeoLocation_Long=(decimal?)51.33577083417429},

                    
                }; 
                
                var currentLocation = await Geolocation.GetLastKnownLocationAsync();
                details.Data.Insert(0, new TotalDetailModel() { GeoLocation_Lat = (decimal?)currentLocation.Latitude, GeoLocation_Long = (decimal?)currentLocation.Longitude });

                var orderedLocations = new List<Location>();
                var orderedIndexes = new int[details.Data.Count];
                orderedLocations.Add(new Location(currentLocation.Latitude, currentLocation.Longitude));

                var locationCount = details.Data.Count;

                var distMatrix = new double[locationCount, locationCount];

                for (int i = 0; i < locationCount; i++)
                {
                    for (int j = 0; j < locationCount; j++)
                    {
                        if (i == j)
                            distMatrix[i, j] = 0;
                        else if (distMatrix[j, i] != 0)
                            distMatrix[i, j] = distMatrix[j, i];
                        else
                            distMatrix[i, j] = Location.CalculateDistance(new Location((double)details.Data[i].GeoLocation_Lat, (double)details.Data[i].GeoLocation_Long), new Location((double)details.Data[j].GeoLocation_Lat, (double)details.Data[j].GeoLocation_Long), DistanceUnits.Kilometers);
                    }
                }

                double d = double.MaxValue;

                var totalCount = 0;

                while (totalCount != locationCount-1)
                {
                    int k = orderedIndexes[totalCount];
                    
                    for (int m = 0; m < locationCount; m++)
                    {
                        if (k != m && distMatrix[k, m] < d && !orderedIndexes.Contians(m))
                        {
                            orderedIndexes[totalCount+1] = m;
                            d = distMatrix[k, m];
                        }
                    }

                    d = double.MaxValue;
                    
                    totalCount++;

                }
                               
                orderedResult = new List<TotalDetailModel>();

                for (int x = 0; x < orderedIndexes.Length; x++)
                {
                    orderedResult.Add(details.Data[orderedIndexes[x]]);
                }

            }
            catch (Exception ex)
            {

            }
            finally
            {
                BusyIndicatorContainder.IsVisible = false;
            }

            return orderedResult;
        }

        private async void TotalDetailsView_OnLongClick(object sender, EventArgs e)
        {
            var Position = ((ListViewLongClickEventArgs)e).Position;
            var OrderedItem = TotalDetailsObservableCollection[Position - 1];

            if (!MultiSelectionMode)
            {
                MultiSelectionMode = true;
                TotalDetailModel.Multiselection = true;

                TotalDetailsView.ItemsSource = null;
                TotalDetailsView.ItemsSource = TotalDetailsObservableCollection;

                await Task.Delay(100);
                FactorsView_ItemTapped(OrderedItem);
            }
        }

        private void FactorsView_ItemTapped(TotalDetailModel TappedItem)
        {
            if (MultiSelectionMode)
            {
                TappedItem.Selected = !TappedItem.Selected;
            }
            else
            {
                //if (TappedItem == LastSelectedOrder)
                //{
                //    LastSelectedOrder = null;
                //    TappedItem.Selected = false;
                //    TappedItemSent = null;
                //}
                //else
                //{
                //    if (LastSelectedOrder != null)
                //        LastSelectedOrder.Selected = false;
                //    LastSelectedOrder = TappedItem;
                //    LastSelectedOrder.Selected = true;

                //    TappedItemSent = TappedItem.Sent;
                //}
            }

            RefreshToolbarItems();
        }

        Guid LastSizeAllocationId;
        protected override async void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            Guid ThisSizeAllocationId = Guid.NewGuid();
            LastSizeAllocationId = ThisSizeAllocationId;
            await Task.Delay(100);
            if (LastSizeAllocationId == ThisSizeAllocationId)
                sizeChanged(width, height);
        }


        double LastWidth, LastHeight;

        public void sizeChanged(double width, double height)
        {
            if (LastWidth != width || LastHeight != height)
            {
                LastWidth = width;
                LastHeight = height;
            }
        }

                   
            //var testResult = new List<UnSettledOrderModel>
            //            {
            //                new UnSettledOrderModel
            //                {
            //                    DriverName = "رضا محمودی",
            //                    OrderCode = "1001",
            //                    OrderDate = DateTime.Now.ToShortStringForDate().ReplaceLatinDigits(),
            //                    OrderId = new Guid(),
            //                    Remainder = 10000.ToString(),
            //                    Price = 14000.ToString()
            //                },
            //                 new UnSettledOrderModel
            //                {
            //                    DriverName = "علی محمودی",
            //                    OrderCode = "1002",
            //                    OrderDate = DateTime.Now.AddDays(4). ToShortStringForDate().ReplaceLatinDigits(),
            //                    OrderId = new Guid(),
            //                    Remainder = 20000.ToString(),
            //                    Price = 24000.ToString()
            //                },
            //                  new UnSettledOrderModel
            //                {
            //                    DriverName = "آرش سیبسی",
            //                    OrderCode = "1003",
            //                    OrderDate = DateTime.Now.AddDays(8).ToShortStringForDate().ReplaceLatinDigits(),
            //                    OrderId = new Guid(),
            //                    Remainder = 30000.ToString(),
            //                    Price = 34000.ToString()
            //                }
            //            };

            //    TotalsView.IsVisible = true;
            //    FactorsObservableCollection = new ObservableCollection<UnSettledOrderModel>(testResult);
            //    TotalsView.ItemsSource = FactorsObservableCollection;
        

       
    }
}