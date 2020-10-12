using Kara.Assets;
using Kara.CustomRenderer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Kara
{
    public class PrinterListCell : ViewCell
    {
        public PrinterListCell()
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
            Label PrinterLabel = null;
            SelectedImage = new Image() { Opacity = 0.7, WidthRequest = 30 };
            PrinterLabel = new Label() { LineBreakMode = LineBreakMode.NoWrap, TextColor = Color.FromHex("222"), HorizontalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.End, VerticalOptions = LayoutOptions.Center, VerticalTextAlignment = TextAlignment.Center, FontSize = 18 };
            GridWrapper.Children.Add(SelectedImage, 0, 0);
            GridWrapper.Children.Add(PrinterLabel, 1, 0);

            PrinterLabel.SetBinding(Label.TextProperty, "Title");
            SelectedImage.SetBinding(Image.SourceProperty, "SelectedImageSource");

            View = GridWrapper;
        }
    }

    public class PrinterModel : INotifyPropertyChanged
    {
        public PrinterSettingModel PrinterSetting { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public string Title { get { return PrinterSetting.Title.ReplaceLatinDigits(); } }
        private bool _Selected;
        public bool Selected { get { return _Selected; } set { _Selected = value; OnPropertyChanged("RowColor"); } }
        public string RowColor { get { return _Selected ? "#A4DEF5" : "#DCE6FA"; } }
        public bool Activated { get { return PrinterSetting.Selected; } set { PrinterSetting.Selected = value; OnPropertyChanged("SelectedImageSource"); } }
        public string SelectedImageSource { get { return PrinterSetting.Selected ? "OK.png" : null; } }
    }

    public partial class SettingsForm_Printers : GradientContentPage
    {
        SettingsForm SettingsForm;
        ObservableCollection<PrinterModel> Printers;
        PrinterModel LastSelectedPrinter;
        ToolbarItem ToolbarItem_Add, ToolbarItem_Remove, ToolbarItem_Edit, ToolbarItem_Activate;

        public SettingsForm_Printers(SettingsForm SettingsForm, List<PrinterModel> Printers)
        {
            InitializeComponent();

            this.SettingsForm = SettingsForm;

            ToolbarItem_Add = new ToolbarItem();
            ToolbarItem_Add.Text = "چاپگر جدید";
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

            PrintersList.ItemTemplate = new DataTemplate(typeof(PrinterListCell));
            PrintersList.ItemSelected += (sender, e) => {
                ((ListView)sender).SelectedItem = null;
            };
            PrintersList.ItemTapped += (sender, e) => {
                var TappedItem = (PrinterModel)e.Item;
                if (TappedItem == LastSelectedPrinter)
                {
                    LastSelectedPrinter = null;
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
                    if (LastSelectedPrinter != null)
                        LastSelectedPrinter.Selected = false;
                    LastSelectedPrinter = TappedItem;
                    LastSelectedPrinter.Selected = true;

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
            PrintersList.SeparatorColor = Color.FromHex("A5ABB7");
            PrintersList.HasUnevenRows = true;

            RefreshPrintersList();
        }

        private void ToolbarItem_Activate_Activated(object sender, EventArgs e)
        {
            if (LastSelectedPrinter != null)
            {
                foreach (var item in App.AllPrinters)
                    item.Selected = false;
                var ActivatingPrinter = LastSelectedPrinter;
                ActivatingPrinter.Activated = true;
                RefreshPrintersList();
            }
        }

        private async void ToolbarItem_Remove_Activated(object sender, EventArgs e)
        {
            if (LastSelectedPrinter != null)
            {
                var answer = await DisplayAlert("حذف چاپگر", "آیا مطمئنید؟", "بله", "خیر");
                if (answer)
                {
                    var DeletingPrinter = LastSelectedPrinter;
                    DeletingPrinter.PrinterSetting.Remove();
                    if (App.AllPrinters.Any() && !App.AllPrinters.Any(a => a.Selected))
                        App.AllPrinters.First().Selected = true;
                    RefreshPrintersList();
                }
            }
        }

        private async void ToolbarItem_Edit_Activated(object sender, EventArgs e)
        {
            if (LastSelectedPrinter != null)
            {
                var SettingsForm_PrintersAddEdit = new SettingsForm_PrintersAddEdit(LastSelectedPrinter.PrinterSetting, this)
                {
                    StartColor = Color.FromHex("E6EBEF"),
                    EndColor = Color.FromHex("A6CFED")
                };
                await this.Navigation.PushAsync(SettingsForm_PrintersAddEdit);
            }
        }

        private async void ToolbarItem_Add_Activated(object sender, EventArgs e)
        {
            var SettingsForm_PrintersAddEdit = new SettingsForm_PrintersAddEdit(null, this)
            {
                StartColor = Color.FromHex("E6EBEF"),
                EndColor = Color.FromHex("A6CFED")
            };
            await this.Navigation.PushAsync(SettingsForm_PrintersAddEdit);
        }

        public async void RefreshPrintersList()
        {
            PrintersList.IsRefreshing = true;
            await Task.Delay(100);

            var PrintersListItems = App.AllPrinters
                .Select(a => new PrinterModel()
                {
                    PrinterSetting = a,
                    Selected = false
                }).ToList();

            Printers = new ObservableCollection<PrinterModel>(PrintersListItems);
            PrintersList.ItemsSource = null;
            PrintersList.ItemsSource = Printers;

            if (!ToolbarItems.Contains(ToolbarItem_Add))
                ToolbarItems.Add(ToolbarItem_Add);

            if (ToolbarItems.Contains(ToolbarItem_Edit))
                ToolbarItems.Remove(ToolbarItem_Edit);
            if (ToolbarItems.Contains(ToolbarItem_Remove))
                ToolbarItems.Remove(ToolbarItem_Remove);
            if (ToolbarItems.Contains(ToolbarItem_Activate))
                ToolbarItems.Remove(ToolbarItem_Activate);

            PrintersList.IsRefreshing = false;
            await Task.Delay(100);
            PrintersList.IsRefreshing = false;

            SettingsForm.PrintersLabel2.Text = App.SelectedPrinter != null ? App.SelectedPrinter.Title.ReplaceLatinDigits() : "تعریف نشده";
        }
    }
}
