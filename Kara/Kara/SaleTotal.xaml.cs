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
using static Kara.Assets.Connectivity;
using Connectivity = Kara.Assets.Connectivity;

namespace Kara
{
    public class SaleTotalCustomCell : ViewCell
    {
        public static readonly BindableProperty TotalIdProperty = BindableProperty.Create("TotalId", typeof(Guid), typeof(SaleTotalCustomCell), Guid.Empty);
        public Guid TotalId
        {
            get { return (Guid)GetValue(TotalIdProperty); }
            set { SetValue(TotalIdProperty, value); }
        }

        public SaleTotalCustomCell()
        {
            this.SetBinding(TotalIdProperty, "Id");
        }
    }

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SaleTotal : GradientContentPage
    {
                       
        public ObservableCollection<SaleTotalsViewModel> TotalsObservableCollection { get; private set; }

        bool TapEventHandlingInProgress = false;

        public SaleTotal()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {

            }
            

            TotalsView.ItemTapped += TotalsView_ItemTapped;
            TotalsView.OnLongClick += TotalsView_OnLongClick;

            BusyIndicatorContainder.BackgroundColor = Color.FromRgba(255, 255, 255, 70);
            BusyIndicator.Color = Color.FromRgba(80, 100, 150, 255);

            //Navigate();
            FillList();
        }

        public async Task Navigate()
        {
           
            var location = new Location(35.77051610021248, 51.32016909923495);
            var options = new MapLaunchOptions { NavigationMode = NavigationMode.Driving };
            //Launcher.OpenAsync(location);
            await Map.OpenAsync(location);
        }

                       
        private void TotalsView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var TappedItem = (SaleTotalsViewModel)e.Item;
            TotalsView_ItemTappedAsync(TappedItem);
        }

        protected override bool OnBackButtonPressed()
        {
            return base.OnBackButtonPressed();
        }
              

        private async Task TotalsView_ItemTappedAsync(SaleTotalsViewModel TappedItem)
        {
            SaleTotalDetails saleTotalDetails = new SaleTotalDetails(TappedItem.TotalId)
            {
                StartColor = Color.FromHex("E6EBEF"),
                EndColor = Color.FromHex("A6CFED")
            };

            try 
            { 
                await Navigation.PushAsync(saleTotalDetails); 
            } catch (Exception) 
            { 
            }
        }

        private async void TotalsView_OnLongClick(object sender, EventArgs e)
        {
            var Position = ((ListViewLongClickEventArgs)e).Position;
            var OrderedItem = TotalsObservableCollection[Position - 1];

            //TotalsView.ItemsSource = null;
            //TotalsView.ItemsSource = TotalsObservableCollection;

            //await Task.Delay(100);
            //TotalsView_ItemTapped(OrderedItem);
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

        private async Task FillList()
        {
            try
            {
                BusyIndicatorContainder.IsVisible = true;
                //temp

                //var totals = await Connectivity.GetPayeeSaleTotals(App.Username.Value, App.Password.Value, App.CurrentVersionNumber, App.UserId.Value, App.EntityCode.Value);

                var totals = new ResultSuccess<List<SaleTotalsModel>>() { Success = true };
                totals.Data = new List<SaleTotalsModel>()
                {
                    new SaleTotalsModel(){DriverCode="123",DriverName="d1 name",TotalCode="123",OrdersCount=3,PayeeCode="10213",PayeeName="asdsadasd",TotalId=new Guid(),TotalPrice=200000,TotalDate= DateTime.Now},
                    new SaleTotalsModel(){DriverCode="456",DriverName="d2 name",TotalCode="456",OrdersCount=15,PayeeCode="3465213",PayeeName="relktj dfg",TotalId=new Guid(),TotalPrice=14500000,TotalDate=DateTime.Now.AddDays(5)},
                };

                if (!totals.Success)
                    App.ShowError("خطا", totals.Message, "انصراف");
                else if (totals.Success)
                {
                    var totalVMs = totals.Data.Select(t => new SaleTotalsViewModel
                    {
                        DriverCode= t.DriverCode,
                        DriverName=t.DriverName,
                        Machine=t.Machine,
                        OrdersCount=t.OrdersCount.ToString(),
                        PayeeCode=t.PayeeCode,
                        TotalDate=t.TotalDate,
                        TotalCode=t.TotalCode,
                        TotalId=t.TotalId,
                        TotalPrice=t.TotalPrice.ToString(),
                    }
                    );
                    TotalsView.IsVisible = true;
                    TotalsObservableCollection = new ObservableCollection<SaleTotalsViewModel>(totalVMs);
                    TotalsView.ItemsSource = TotalsObservableCollection;
                }
            }
            finally
            {
                BusyIndicatorContainder.IsVisible = false;
                TotalsView.IsVisible = true;
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
}