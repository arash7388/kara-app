using Kara.Assets;
using Kara.CustomRenderer;
using Plugin.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Kara
{
    public class InsertedInformationsOrdersCustomCell : ViewCell
    {
        public static readonly BindableProperty OrderIdProperty =
            BindableProperty.Create("OrderId", typeof(Guid), typeof(InsertedInformationsOrdersCustomCell), Guid.Empty);
        public Guid OrderId
        {
            get { return (Guid)GetValue(OrderIdProperty); }
            set { SetValue(OrderIdProperty, value); }
        }

        public InsertedInformationsOrdersCustomCell()
        {
            this.SetBinding(InsertedInformationsOrdersCustomCell.OrderIdProperty, "Id");

            Grid GridWrapper = new Grid()
            {
                Padding = new Thickness(5, 0),
                RowSpacing = 1,
                ColumnSpacing = 1,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
            GridWrapper.SetBinding(Grid.BackgroundColorProperty, "RowColor");

            GridWrapper.RowDefinitions = new RowDefinitionCollection()
            {
                new RowDefinition() { Height = 35 },
                new RowDefinition() { Height = 25 }
            };

            GridWrapper.ColumnDefinitions = new ColumnDefinitionCollection()
            {
                new ColumnDefinition() { Width = new GridLength(5, GridUnitType.Star) },
                new ColumnDefinition() { Width = new GridLength(4, GridUnitType.Star) },
                new ColumnDefinition() { Width = new GridLength(4, GridUnitType.Star) },
                new ColumnDefinition() { Width = new GridLength(8, GridUnitType.Star) },
                new ColumnDefinition() { Width = new GridLength(4, GridUnitType.Star) }
            };
            var CheckboxColumnDefinition = new ColumnDefinition() { };
            GridWrapper.ColumnDefinitions.Add(CheckboxColumnDefinition);

            Label PreCodeLabel = null, PartnerNameLabel = null, DateLabel = null, TimeLabel = null, PriceLabel = null, DescriptionLabel = null;
            MyCheckBox CheckBox = null;

            PriceLabel = new Label() { LineBreakMode = LineBreakMode.TailTruncation, HorizontalOptions = LayoutOptions.Start, HorizontalTextAlignment = TextAlignment.Start, VerticalOptions = LayoutOptions.Center, FontSize = 16, TextColor = Color.FromHex("1845A5") };
            DateLabel = new Label() { LineBreakMode = LineBreakMode.TailTruncation, HorizontalOptions = LayoutOptions.Start, HorizontalTextAlignment = TextAlignment.Start, VerticalOptions = LayoutOptions.Center, FontSize = 16, TextColor = Color.FromHex("222222") };
            TimeLabel = new Label() { LineBreakMode = LineBreakMode.TailTruncation, HorizontalOptions = LayoutOptions.Start, HorizontalTextAlignment = TextAlignment.Start, VerticalOptions = LayoutOptions.Center, FontSize = 16, TextColor = Color.FromHex("222222") };
            PartnerNameLabel = new Label() { LineBreakMode = LineBreakMode.TailTruncation, HorizontalOptions = LayoutOptions.End, HorizontalTextAlignment = TextAlignment.End, VerticalOptions = LayoutOptions.Center, FontSize = 16, TextColor = Color.FromHex("222222") };
            PreCodeLabel = new Label() { LineBreakMode = LineBreakMode.HeadTruncation, HorizontalOptions = LayoutOptions.End, HorizontalTextAlignment = TextAlignment.End, VerticalOptions = LayoutOptions.Center, FontSize = 16, TextColor = Color.FromHex("222222") };
            DescriptionLabel = new Label() { LineBreakMode = LineBreakMode.TailTruncation, HorizontalOptions = LayoutOptions.End, HorizontalTextAlignment = TextAlignment.End, VerticalOptions = LayoutOptions.Center, FontSize = 13 };
            CheckBox = new MyCheckBox() { HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };

            GridWrapper.Children.Add(PriceLabel, 0, 0);
            GridWrapper.Children.Add(DateLabel, 1, 0);
            GridWrapper.Children.Add(TimeLabel, 2, 0);
            GridWrapper.Children.Add(PartnerNameLabel, 3, 0);
            GridWrapper.Children.Add(PreCodeLabel, 4, 0);
            GridWrapper.Children.Add(DescriptionLabel, 0, 1);
            Grid.SetColumnSpan(DescriptionLabel, 5);
            GridWrapper.Children.Add(CheckBox, 5, 0);
            Grid.SetRowSpan(CheckBox, 2);

            PriceLabel.SetBinding(Label.TextProperty, "Price");
            DateLabel.SetBinding(Label.TextProperty, "Date");
            TimeLabel.SetBinding(Label.TextProperty, "Time");
            PartnerNameLabel.SetBinding(Label.TextProperty, "PartnerName");
            PreCodeLabel.SetBinding(Label.TextProperty, "PreCode");
            DescriptionLabel.SetBinding(Label.TextProperty, "Description");
            DescriptionLabel.SetBinding(Label.TextColorProperty, "DescriptionColor");
            CheckboxColumnDefinition.SetBinding(ColumnDefinition.WidthProperty, "CheckBoxColumnWidth");
            CheckBox.SetBinding(MyCheckBox.CheckedProperty, "Selected", BindingMode.TwoWay);
            CheckBox.SetBinding(MyCheckBox.IsVisibleProperty, "CanBeSelectedInMultiselection");

            View = GridWrapper;
        }
    }

    public partial class InsertedInformations_Orders : GradientContentPage
    {
        ObservableCollection<DBRepository.OrderListModel> OrdersList;
        private bool justToday = false;
        private bool justLocal = false;
        InsertedInformations InsertedInformations;
        private bool BackButton_Visible, ToolbarItem_SearchBar_Visible, ToolbarItem_Delete_Visible, ToolbarItem_SendToServer_Visible, ToolbarItem_Edit_Visible, ToolbarItem_Show_Visible, ToolbarItem_SelectAll_Visible;
        
        public InsertedInformations_Orders(InsertedInformations InsertedInformations)
        {
            InitializeComponent();
            Title = "سفارشات";
            this.InsertedInformations = InsertedInformations;

            BusyIndicatorContainder.BackgroundColor = Color.FromRgba(255, 255, 255, 70);
            BusyIndicator.Color = Color.FromRgba(80, 100, 150, 255);
            
            OrderItems.HasUnevenRows = true;
            OrderItems.SeparatorColor = Color.FromHex("A5ABB7");
            OrderItems.ItemTemplate = new DataTemplate(typeof(InsertedInformationsOrdersCustomCell));
            OrderItems.ItemSelected += (sender, e) => {
                ((ListView)sender).SelectedItem = null;
            };
            OrderItems.ItemTapped += (sender, e) => {
                var TappedItem = (DBRepository.OrderListModel)e.Item;
                OrderItems_ItemTapped(TappedItem);
            };
            OrderItems.OnLongClick += OrderItems_OnLongClick;

            VisitsSearchBar.TextChanged += async (sender, args) => {
                await FillOrders(args.NewTextValue);
            };
            VisitsSearchBar.SearchButtonPressed += (sender, args) => {
                VisitsSearchBar.IsVisible = false;
                FiltersSection.IsVisible = true;
                RefreshToolbarItems();
            };

            justToday = new SettingField<bool>("OrdersList_JustToday", false).Value;
            JustTodaySwitch.IsToggled = justToday;
            justLocal = new SettingField<bool>("OrdersList_JustLocal", false).Value;
            JustLocalSwitch.IsToggled = justLocal;
            
            JustTodaySwitch.Toggled += FilterChanged;
            JustLocalSwitch.Toggled += FilterChanged;

            FillOrders("");
        }
        
        public void SetCurrentPage()
        {
            InsertedInformations.ToolbarItem_SearchBar.Activated += ToolbarItem_SearchBar_Activated;
            InsertedInformations.ToolbarItem_SendToServer.Activated += ToolbarItem_SendToServer_Activated;
            InsertedInformations.ToolbarItem_Delete.Activated += ToolbarItem_Delete_Activated;
            InsertedInformations.ToolbarItem_Edit.Activated += ToolbarItem_Edit_Activated;
            InsertedInformations.ToolbarItem_Show.Activated += ToolbarItem_Show_Activated;
            InsertedInformations.ToolbarItem_SelectAll.Activated += ToolbarItem_SelectAll_Activated;
            RefreshToolbarItems();
        }
        
        public void UnsetCurrentPage()
        {
            InsertedInformations.ToolbarItem_SearchBar.Activated -= ToolbarItem_SearchBar_Activated;
            InsertedInformations.ToolbarItem_SendToServer.Activated -= ToolbarItem_SendToServer_Activated;
            InsertedInformations.ToolbarItem_Delete.Activated -= ToolbarItem_Delete_Activated;
            InsertedInformations.ToolbarItem_Edit.Activated -= ToolbarItem_Edit_Activated;
            InsertedInformations.ToolbarItem_Show.Activated -= ToolbarItem_Show_Activated;
            InsertedInformations.ToolbarItem_SelectAll.Activated -= ToolbarItem_SelectAll_Activated;
        }

        bool MultiSelectionMode = false;
        private async void ExitMultiSelectionMode()
        {
            foreach (var item in OrdersList.Where(a => a.Selected))
                item.Selected = false;
            MultiSelectionMode = false;
            DBRepository.OrderListModel.Multiselection = false;
            OrderItems.ItemsSource = null;
            OrderItems.ItemsSource = OrdersList;
            RefreshToolbarItems();
        }
        private async void OrderItems_OnLongClick(object sender, EventArgs e)
        {
            var Position = ((ListViewLongClickEventArgs)e).Position;
            var Order = OrdersList[Position - 1];
            if (!Order.Sent && !MultiSelectionMode)
            {
                MultiSelectionMode = true;
                DBRepository.OrderListModel.Multiselection = true;
                if (LastSelectedOrder != null)
                {
                    LastSelectedOrder.Selected = false;
                    LastSelectedOrder = null;
                }
                OrderItems.ItemsSource = null;
                OrderItems.ItemsSource = OrdersList;
                await Task.Delay(100);
                OrderItems_ItemTapped(Order);
            }
        }

        protected override bool OnBackButtonPressed()
        {
            if(MultiSelectionMode)
            {
                ExitMultiSelectionMode();
                return true;
            }
            return base.OnBackButtonPressed();
        }

        DBRepository.OrderListModel LastSelectedOrder = null;
        bool? TappedItemSent;
        private void OrderItems_ItemTapped(DBRepository.OrderListModel TappedItem)
        {
            if (MultiSelectionMode)
            {
                LastSelectedOrder = null;
                if (!TappedItem.Sent)
                    TappedItem.Selected = !TappedItem.Selected;
            }
            else
            {
                if (TappedItem == LastSelectedOrder)
                {
                    LastSelectedOrder = null;
                    TappedItem.Selected = false;
                    TappedItemSent = null;
                }
                else
                {
                    if (LastSelectedOrder != null)
                        LastSelectedOrder.Selected = false;
                    LastSelectedOrder = TappedItem;
                    LastSelectedOrder.Selected = true;

                    TappedItemSent = TappedItem.Sent;
                }
            }

            RefreshToolbarItems();
        }

        private void ToolbarItem_SelectAll_Activated(object sender, EventArgs e)
        {
            var AllUnSentOrdersSelected = OrdersList.Where(a => !a._PreCode.HasValue).All(a => a.Selected);
            if (AllUnSentOrdersSelected)
            {
                foreach (var item in OrdersList.Where(a => !a._PreCode.HasValue && a.Selected))
                    item.Selected = false;
                InsertedInformations.ToolbarItem_SelectAll.Icon = "SelectAll_Empty.png";
            }
            else
            {
                foreach (var item in OrdersList.Where(a => !a._PreCode.HasValue && !a.Selected))
                    item.Selected = true;
                InsertedInformations.ToolbarItem_SelectAll.Icon = "SelectAll_Full.png";
            }
        }

        private void ToolbarItem_Show_Activated(object sender, EventArgs e)
        {
            LastSelectedOrder.SaleOrderData.CashDiscounts = App.DB.conn.Table<Models.CashDiscount>().ToArray().Where(a => a.OrderId == LastSelectedOrder.SaleOrderData.Id).ToArray();
            var OrderPreviewForm = new OrderPreviewForm(LastSelectedOrder.SaleOrderData, this, null, null, null, false)
            {
                StartColor = Color.FromHex("ffffff"),
                EndColor = Color.FromHex("ffffff")
            };
            this.Navigation.PushAsync(OrderPreviewForm);
        }

        private void ToolbarItem_Edit_Activated(object sender, EventArgs e)
        {
            var OrderInsertForm = new OrderInsertForm(null, LastSelectedOrder.SaleOrderData, this, null, LastSelectedOrder.SaleOrderData.WarehouseId)
            {
                StartColor = Color.FromHex("E6EBEF"),
                EndColor = Color.FromHex("A6CFED")
            };
            this.Navigation.PushAsync(OrderInsertForm);
        }

        private async void ToolbarItem_Delete_Activated(object sender, EventArgs e)
        {
            var SelectedOrders = MultiSelectionMode ? OrdersList.Where(a => a.Selected).ToArray() : new DBRepository.OrderListModel[] { LastSelectedOrder };
            if (SelectedOrders.Length == 0)
            {
                App.ShowError("خطا", "هیچ سفارشی انتخاب نشده است.", "خوب");
                return;
            }

            var answer = await DisplayAlert("حذف " + SelectedOrders.Length + " سفارش", "آیا مطمئنید؟", "بله", "خیر");
            if (answer)
            {
                WaitToggle(false);
                await Task.Delay(500);
                var result = await App.DB.DeleteSaleOrdersAsync(SelectedOrders.Select(a => a.Id).ToArray());
                if (!result.Success)
                {
                    WaitToggle(false);
                    App.ShowError("خطا", result.Message, "خوب");
                }
                else
                {
                    ExitMultiSelectionMode();
                    WaitToggle(true);
                    FillOrders(VisitsSearchBar.Text);
                }
            }
        }

        private async void ToolbarItem_SendToServer_Activated(object sender, EventArgs e)
        {
            var SelectedOrders = MultiSelectionMode ? OrdersList.Where(a => a.Selected).ToArray() : new DBRepository.OrderListModel[] { LastSelectedOrder };
            if (SelectedOrders.Length == 0)
            {
                App.ShowError("خطا", "هیچ سفارشی انتخاب نشده است.", "خوب");
                return;
            }
            
            WaitToggle(false);
            var submitResult = await Connectivity.SubmitSaleOrdersAsync(SelectedOrders.Select(a => a.SaleOrderData).ToArray());
            if (!submitResult.Success)
            {
                WaitToggle(false);
                App.ShowError("خطا", submitResult.Message, "خوب");
                if(submitResult.Data != 0)
                {
                    ExitMultiSelectionMode();
                    FillOrders(VisitsSearchBar.Text);
                    InsertedInformations.Partners.FillPartners();
                }
            }
            else
            {
                ExitMultiSelectionMode();
                WaitToggle(true);
                FillOrders(VisitsSearchBar.Text);
                InsertedInformations.Partners.FillPartners();
            }
        }

        private void ToolbarItem_SearchBar_Activated(object sender, EventArgs e)
        {
            VisitsSearchBar.IsVisible = true;
            FiltersSection.IsVisible = false;
            RefreshToolbarItems();
        }

        public void FilterChanged(object sender, EventArgs e)
        {
            justToday = JustTodaySwitch.IsToggled;
            new SettingField<bool>("OrdersList_JustToday", justToday).Value = justToday;
            justLocal = JustLocalSwitch.IsToggled;
            new SettingField<bool>("OrdersList_JustLocal", justLocal).Value = justLocal;
            FillOrders(VisitsSearchBar.Text);
        }

        public void WaitToggle(bool WithSuccess)
        {
            if (!BusyIndicatorContainder.IsVisible)
            {
                BusyIndicatorContainder.IsVisible = true;
            }
            else
            {
                BusyIndicatorContainder.IsVisible = false;
                if (!WithSuccess)
                {
                }
            }
            RefreshToolbarItems();
        }

        private async Task FillOrders(string Filter)
        {
            OrderItems.IsRefreshing = true;
            await Task.Delay(100);

            var OrdersResult = await App.DB.GetOrdersListAsync(justToday, justLocal, Filter.ReplacePersianDigits());
            if (!OrdersResult.Success)
            {
                App.ShowError("خطا", "در نمایش لیست سفارشات خطایی رخ داد.\n" + OrdersResult.Message, "خوب");
                OrderItems.IsRefreshing = false;
                return;
            }
            OrdersList = new ObservableCollection<DBRepository.OrderListModel>(OrdersResult.Data);
            OrderItems.ItemsSource = null;
            OrderItems.ItemsSource = OrdersList;

            RefreshToolbarItems();
            OrderItems.IsRefreshing = false;
        }

        public void RefreshToolbarItems()
        {
            ToolbarItem_SendToServer_Visible = false;
            ToolbarItem_Delete_Visible = false;
            ToolbarItem_Edit_Visible = false;
            ToolbarItem_Show_Visible = false;
            ToolbarItem_SearchBar_Visible = false;
            ToolbarItem_SelectAll_Visible = false;
            BackButton_Visible = false;

            if (MultiSelectionMode)
            {
                ToolbarItem_SelectAll_Visible = true;
                var AllUnSentOrdersSelected = OrdersList.Where(a => !a._PreCode.HasValue).All(a => a.Selected);
                InsertedInformations.ToolbarItem_SelectAll.Icon = AllUnSentOrdersSelected ? "SelectAll_Full.png" : "SelectAll_Empty.png";
            }
            else
                BackButton_Visible = true;

            var SelectedCount = OrdersList != null ? OrdersList.Count(a => a.Selected) : 0;
            if (SelectedCount == 0)
            {
                if(!MultiSelectionMode)
                    ToolbarItem_SearchBar_Visible = true;
            }
            else if (MultiSelectionMode)
            {
                ToolbarItem_SendToServer_Visible = true;
                ToolbarItem_Delete_Visible = true;
            }
            else
            {
                if (!OrdersList.Single(a => a.Selected).Sent)
                {
                    ToolbarItem_SendToServer_Visible = true;
                    ToolbarItem_Delete_Visible = true;
                    ToolbarItem_Edit_Visible = true;
                }
                ToolbarItem_Show_Visible = true;
            }

            InsertedInformations.RefreshToolbarItems(BackButton_Visible, ToolbarItem_SearchBar_Visible, ToolbarItem_Delete_Visible, ToolbarItem_SendToServer_Visible, ToolbarItem_Edit_Visible, ToolbarItem_Show_Visible, ToolbarItem_SelectAll_Visible);
        }
    }
}
