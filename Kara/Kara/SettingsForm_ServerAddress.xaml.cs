using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kara.CustomRenderer;
using Xamarin.Forms;
using Kara.Models;
using Kara.Assets;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Kara
{
    public class ServerAddressListCell : ViewCell
    {
        public ServerAddressListCell()
        {
            Grid GridWrapper = new Grid()
            {
                Padding = new Thickness(5, 0),
                RowSpacing = 1,
                ColumnSpacing = 1,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
            GridWrapper.SetBinding(Grid.BackgroundColorProperty, "RowColor");
            GridWrapper.RowDefinitions.Add(new RowDefinition() { Height = 50 });

            GridWrapper.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
            GridWrapper.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            
            Image SelectedImage = null;
            Label AddressLabel = null;
            SelectedImage = new Image() { Opacity = 0.7, WidthRequest = 30 };
            AddressLabel = new Label() { LineBreakMode = LineBreakMode.NoWrap, TextColor = Color.FromHex("222"), HorizontalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.End, VerticalOptions = LayoutOptions.Center, VerticalTextAlignment = TextAlignment.Center, FontSize = 18 };
            GridWrapper.Children.Add(SelectedImage, 0, 0);
            GridWrapper.Children.Add(AddressLabel, 1, 0);

            AddressLabel.SetBinding(Label.TextProperty, "Address");
            SelectedImage.SetBinding(Image.SourceProperty, "SelectedImageSource");

            View = GridWrapper;
        }
    }

    public class ServerAddressModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private string __Address;
        public string _Address { get { return __Address; } set { __Address = value; OnPropertyChanged("Address"); } }
        public string Address { get { return _Address.ToPersianDigits(); } }
        private bool _Selected;
        public bool Selected { get { return _Selected; } set { _Selected = value; OnPropertyChanged("RowColor"); } }
        public string RowColor { get { return _Selected ? "#A4DEF5" : "#DCE6FA"; } }
        private bool _Activated;
        public bool Activated { get { return _Activated; } set { _Activated = value; OnPropertyChanged("SelectedImageSource"); } }
        public string SelectedImageSource { get { return _Activated ? "OK.png" : null; } }
    }

    public partial class SettingsForm_ServerAddress : GradientContentPage
    {
        SettingsForm SettingsForm;
        ObservableCollection<ServerAddressModel> Addresses;
        ServerAddressModel LastSelectedAddress;
        ToolbarItem ToolbarItem_Add, ToolbarItem_Remove, ToolbarItem_Edit, ToolbarItem_Activate;

        public SettingsForm_ServerAddress(SettingsForm SettingsForm, List<ServerAddressModel> Addresses)
        {
            InitializeComponent();

            this.SettingsForm = SettingsForm;

            ToolbarItem_Add = new ToolbarItem();
            ToolbarItem_Add.Text = "آدرس جدید";
            ToolbarItem_Add.Icon = "Add.png";
            ToolbarItem_Add.Order = ToolbarItemOrder.Primary;
            ToolbarItem_Add.Priority = 4;
            ToolbarItem_Add.Activated += ToolbarItem_Add_Activated;

            ToolbarItem_Edit = new ToolbarItem();
            ToolbarItem_Edit.Text = "ویرایش";
            ToolbarItem_Edit.Icon = "Edit.png";
            ToolbarItem_Edit.Order = ToolbarItemOrder.Primary;
            ToolbarItem_Edit.Priority = 3;
            ToolbarItem_Edit.Activated += ToolbarItem_Edit_Activated;

            ToolbarItem_Remove = new ToolbarItem();
            ToolbarItem_Remove.Text = "حذف";
            ToolbarItem_Remove.Icon = "Delete.png";
            ToolbarItem_Remove.Order = ToolbarItemOrder.Primary;
            ToolbarItem_Remove.Priority = 2;
            ToolbarItem_Remove.Activated += ToolbarItem_Remove_Activated;

            ToolbarItem_Activate = new ToolbarItem();
            ToolbarItem_Activate.Text = "فعال";
            ToolbarItem_Activate.Icon = "OK_W.png";
            ToolbarItem_Activate.Order = ToolbarItemOrder.Primary;
            ToolbarItem_Activate.Priority = 1;
            ToolbarItem_Activate.Activated += ToolbarItem_Activate_Activated;

            ServerAddressList.ItemTemplate = new DataTemplate(typeof(ServerAddressListCell));
            ServerAddressList.ItemSelected += (sender, e) => {
                ((ListView)sender).SelectedItem = null;
            };
            ServerAddressList.ItemTapped += (sender, e) => {
                var TappedItem = (ServerAddressModel)e.Item;
                if (TappedItem == LastSelectedAddress)
                {
                    LastSelectedAddress = null;
                    TappedItem.Selected = false;

                    if (!ToolbarItems.Contains(ToolbarItem_Add))
                        ToolbarItems.Add(ToolbarItem_Add);

                    if (ToolbarItems.Contains(ToolbarItem_Edit))
                        ToolbarItems.Remove(ToolbarItem_Edit);
                    if (ToolbarItems.Contains(ToolbarItem_Remove))
                        ToolbarItems.Remove(ToolbarItem_Remove);
                    if (ToolbarItems.Contains(ToolbarItem_Activate))
                        ToolbarItems.Remove(ToolbarItem_Activate);
                }
                else
                {
                    if (LastSelectedAddress != null)
                        LastSelectedAddress.Selected = false;
                    LastSelectedAddress = TappedItem;
                    LastSelectedAddress.Selected = true;
                    
                    if (ToolbarItems.Contains(ToolbarItem_Add))
                        ToolbarItems.Remove(ToolbarItem_Add);

                    if (!ToolbarItems.Contains(ToolbarItem_Edit))
                        ToolbarItems.Add(ToolbarItem_Edit);
                    if (!ToolbarItems.Contains(ToolbarItem_Remove))
                        ToolbarItems.Add(ToolbarItem_Remove);
                    if (!ToolbarItems.Contains(ToolbarItem_Activate))
                        ToolbarItems.Add(ToolbarItem_Activate);
                }
            };
            ServerAddressList.SeparatorColor = Color.FromHex("A5ABB7");
            ServerAddressList.HasUnevenRows = true;

            RefreshAddressList();
        }

        private void ToolbarItem_Activate_Activated(object sender, EventArgs e)
        {
            if (LastSelectedAddress != null)
            {
                var ActivatingAddress = (ServerAddressModel)LastSelectedAddress;
                var AllAddresses = App.AllServerAddresses.Value.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(a => new
                    {
                        Address = a.Split('_')[0],
                        Selected = a.Split('_')[0] == ActivatingAddress._Address
                    }).ToArray();
                App.AllServerAddresses.Value = AllAddresses.Any() ? AllAddresses.Select(a => a.Address + "_" + a.Selected.ToString()).Aggregate((sum, x) => sum + "|" + x) : "";
                App.ServerAddress = AllAddresses.First(a => a.Selected).Address;

                RefreshAddressList();
            }
        }

        private async void ToolbarItem_Remove_Activated(object sender, EventArgs e)
        {
            if (LastSelectedAddress != null)
            {
                var answer = await DisplayAlert("حذف آدرس", "آیا مطمئنید؟", "بله", "خیر");
                if (answer)
                {
                    var DeletingAddress = (ServerAddressModel)LastSelectedAddress;
                    var AllAddresses = App.AllServerAddresses.Value.Split(new string[] { "|"}, StringSplitOptions.RemoveEmptyEntries)
                        .Select(a => new
                        {
                            Address = a.Split('_')[0],
                            Selected = Convert.ToBoolean(a.Split('_')[1])
                        }).ToArray();
                    if (AllAddresses.Count() <= 1)
                        App.ShowError("خطا", "حداقل باید یک آدرس در برنامه تعریف شده باشد.", "خوب");
                    else
                    {
                        AllAddresses = AllAddresses.Where(a => a.Address != DeletingAddress._Address).ToArray();
                        App.AllServerAddresses.Value = AllAddresses.Any() ? AllAddresses.Select(a => a.Address + "_" + a.Selected.ToString()).Aggregate((sum, x) => sum + "|" + x) : "";
                        if (!AllAddresses.Any(a => a.Selected))
                            App.ServerAddress = AllAddresses[0].Address;
                        else
                            App.ServerAddress = AllAddresses.First(a => a.Selected).Address;

                        RefreshAddressList();
                    }
                }
            }
        }

        private async void ToolbarItem_Edit_Activated(object sender, EventArgs e)
        {
            if (LastSelectedAddress != null)
            {
                var SettingsForm_ServerAddressAddEdit = new SettingsForm_ServerAddressAddEdit(((ServerAddressModel)LastSelectedAddress).Address, true, this)
                {
                    StartColor = Color.FromHex("E6EBEF"),
                    EndColor = Color.FromHex("A6CFED")
                };
                await this.Navigation.PushAsync(SettingsForm_ServerAddressAddEdit);
            }
        }

        private async void ToolbarItem_Add_Activated(object sender, EventArgs e)
        {
            var SettingsForm_ServerAddressAddEdit = new SettingsForm_ServerAddressAddEdit("", false, this)
            {
                StartColor = Color.FromHex("E6EBEF"),
                EndColor = Color.FromHex("A6CFED")
            };
            await this.Navigation.PushAsync(SettingsForm_ServerAddressAddEdit);
        }

        public async void RefreshAddressList()
        {
            ServerAddressList.IsRefreshing = true;
            await Task.Delay(100);

            var AddressesList = App.AllServerAddresses.Value.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(a => new ServerAddressModel()
                {
                    _Address = a.Split('_')[0],
                    Activated = Convert.ToBoolean(a.Split('_')[1]),
                    Selected = false
                }).ToList();

            Addresses = new ObservableCollection<ServerAddressModel>(AddressesList);
            ServerAddressList.ItemsSource = null;
            ServerAddressList.ItemsSource = Addresses;

            if (!ToolbarItems.Contains(ToolbarItem_Add))
                ToolbarItems.Add(ToolbarItem_Add);

            if (ToolbarItems.Contains(ToolbarItem_Edit))
                ToolbarItems.Remove(ToolbarItem_Edit);
            if (ToolbarItems.Contains(ToolbarItem_Remove))
                ToolbarItems.Remove(ToolbarItem_Remove);
            if (ToolbarItems.Contains(ToolbarItem_Activate))
                ToolbarItems.Remove(ToolbarItem_Activate);

            ServerAddressList.IsRefreshing = false;
            await Task.Delay(100);
            ServerAddressList.IsRefreshing = false;

            SettingsForm.ServerAddressLabel2.Text = App.ServerAddress.ToPersianDigits();
        }
    }
}
