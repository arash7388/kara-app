using System;
using Kara.CustomRenderer;
using Xamarin.Forms;
using Kara.Assets;
using System.Collections.Generic;
using System.Linq;
using Kara.Helpers;

namespace Kara
{
    public partial class SettingsForm_PrintersAddEdit : GradientContentPage
    {
        SettingsForm_Printers SettingsForm_Printers;
        ToolbarItem ToolbarItem_OK;
        PrinterSettingModel EditingPrinter;
        string MacID;
        List<BluetoothDeviceModel> DevicesList;
        MyEntry TitleLabel;
        Picker BluetoothPrinterPicker;
        RightRoundedLabel BluetoothPrinterLabel;
        LeftEntryCompanionLabel BluetoothPrinterChangeButton;
        MyEntry WidthLabel;
        MyEntry DensityLabel;
        MyEntry FontSizeLabel;

        public SettingsForm_PrintersAddEdit(PrinterSettingModel Printer, SettingsForm_Printers SettingsForm_Printers)
        {
            InitializeComponent();

            TitleLabel = new MyEntry()
            {
                LeftRounded = true,
                RightRounded = true,
                Placeholder = "عنوان چاپگر",
                Padding = new Thickness(40, 10),
                FontSize = 18,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HorizontalTextAlignment = TextAlignment.End
            };
            GridLayout.Children.Add(TitleLabel, 0, 0);
            Grid.SetColumnSpan(TitleLabel, 2);

            BluetoothPrinterPicker = new Picker()
            {
                VerticalOptions = LayoutOptions.Start,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
            GridLayout.Children.Add(BluetoothPrinterPicker, 0, 1);

            BluetoothPrinterLabel = new RightRoundedLabel()
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HorizontalTextAlignment = TextAlignment.End,
                Text = "چاپگر بلوتوث",
                Padding = new Thickness(40, 10),
                FontSize = 18
            };
            GridLayout.Children.Add(BluetoothPrinterLabel, 1, 1);

            BluetoothPrinterChangeButton = new LeftEntryCompanionLabel()
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HorizontalTextAlignment = TextAlignment.Center,
                FontSize = 18,
                Text = "..."
            };
            GridLayout.Children.Add(BluetoothPrinterChangeButton, 0, 1);

            WidthLabel = new MyEntry()
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HorizontalTextAlignment = TextAlignment.End,
                LeftRounded = true,
                RightRounded = true,
                Placeholder = "پهنای قابل چاپ(mm)",
                Padding = new Thickness(40, 10),
                FontSize = 18
            };
            GridLayout.Children.Add(WidthLabel, 0, 2);
            Grid.SetColumnSpan(WidthLabel, 2);

            DensityLabel = new MyEntry()
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HorizontalTextAlignment = TextAlignment.End,
                LeftRounded = true,
                RightRounded = true,
                Placeholder = "تراکم نقاط(dpi)",
                Padding = new Thickness(40, 10),
                FontSize = 18
            };
            GridLayout.Children.Add(DensityLabel, 0, 3);
            Grid.SetColumnSpan(DensityLabel, 2);

            FontSizeLabel = new MyEntry()
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HorizontalTextAlignment = TextAlignment.End,
                LeftRounded = true,
                RightRounded = true,
                Placeholder = "سایز فونت",
                Padding = new Thickness(40, 10),
                FontSize = 18
            };
            GridLayout.Children.Add(FontSizeLabel, 0, 4);
            Grid.SetColumnSpan(FontSizeLabel, 2);
            
            this.SettingsForm_Printers = SettingsForm_Printers;

            ToolbarItem_OK = new ToolbarItem();
            ToolbarItem_OK.Text = "تایید";
            ToolbarItem_OK.Icon = "Save.png";
            ToolbarItem_OK.Order = ToolbarItemOrder.Primary;
            ToolbarItem_OK.Priority = 1;
            ToolbarItem_OK.Activated += ToolbarItem_OK_Activated;
            ToolbarItems.Add(ToolbarItem_OK);
            
            EditingPrinter = Printer;

            BluetoothPrinterPicker.SelectedIndexChanged += (sender, e) =>
            {
                var SelectedDevice = (BluetoothDeviceModel)BluetoothPrinterPicker.SelectedItem;
                MacID = SelectedDevice.MACID;
                BluetoothPrinterLabel.Text = SelectedDevice.Name;
            };

            SetEditingPrinter();
        }
        
        private async void SetEditingPrinter()
        {
            var DevicesResult = await App.BluetoothPrinter.GetDevicesNameAndMACID();
            if (!DevicesResult.Success)
            {
                App.ShowError("خطا", DevicesResult.Message, "خوب");
                try { await Navigation.PopAsync(); } catch (Exception) { }
            }
            else
            {
                DevicesList = DevicesResult.Data;
                BluetoothPrinterPicker.Items.Clear();
                BluetoothPrinterPicker.ItemsSource = DevicesList;

                if (EditingPrinter != null)
                {
                    TitleLabel.Text = EditingPrinter.Title;
                    MacID = EditingPrinter.MACID;
                    WidthLabel.Text = EditingPrinter.Width.ToString().ToPersianDigits();
                    DensityLabel.Text = EditingPrinter.Density.ToString().ToPersianDigits();
                    FontSizeLabel.Text = EditingPrinter.FontSize.ToString().ToPersianDigits();
                    var Device = DevicesList.SingleOrDefault(a => a.MACID == MacID);
                    BluetoothPrinterLabel.Text = Device == null ? "دستگاه شناسایی نشد!" : Device.Name;
                    if (Device.Name != null)
                        BluetoothPrinterPicker.SelectedItem = Device;
                }
                else
                {
                    TitleLabel.Text = "";
                    MacID = "";
                    WidthLabel.Text = "";
                    DensityLabel.Text = "";
                    FontSizeLabel.Text = "";
                }

                if (!DevicesList.Any())
                    App.ShowError("خطا", "هیچ چاپگر بلوتوث جفت شده ای یافت نشد. لطفا ابتدا به چاپگر بلوتوث متصل شوید.", "خوب");
            }
        }


        private void ToolbarItem_OK_Activated(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(MacID))
            {
                App.ShowError("خطا", "چاپگر بلوتوث بدرستی انتخاب نشده است.", "خوب");
                return;
            }
            if (TitleLabel.Text == "")
            {
                App.ShowError("خطا", "عنوان وارد شده معتبر نیست.", "خوب");
                return;
            }
            try
            {
                Convert.ToInt32(WidthLabel.Text.ToLatinDigits());
            }
            catch (Exception)
            {
                App.ShowError("خطا", "پهنای قابل چاپ وارد شده معتبر نیست.", "خوب");
                return;
            }
            try
            {
                Convert.ToInt32(DensityLabel.Text.ToLatinDigits());
            }
            catch (Exception)
            {
                App.ShowError("خطا", "تراکم نقاط وارد شده معتبر نیست.", "خوب");
                return;
            }
            try
            {
                Convert.ToInt32(FontSizeLabel.Text.ToLatinDigits());
            }
            catch (Exception)
            {
                App.ShowError("خطا", "سایز فونت وارد شده معتبر نیست.", "خوب");
                return;
            }
            
            if (EditingPrinter == null)//Insert
            {
                var newPrinter = new PrinterSettingModel(TitleLabel.Text.ToLatinDigits() + "|" + MacID.ToLatinDigits() + "|" + WidthLabel.Text.ToLatinDigits() + "|" + DensityLabel.Text.ToLatinDigits() + "|" + FontSizeLabel.Text.ToLatinDigits() + "|" + !App.AllPrinters.Any());
                newPrinter.Add();
            }
            else//Update
            {
                EditingPrinter.Title = TitleLabel.Text.ToLatinDigits();
                EditingPrinter.MACID = MacID.ToLatinDigits();
                EditingPrinter.Width = Convert.ToInt32(WidthLabel.Text.ToLatinDigits());
                EditingPrinter.Density = Convert.ToInt32(DensityLabel.Text.ToLatinDigits());
                EditingPrinter.FontSize = Convert.ToInt32(FontSizeLabel.Text.ToLatinDigits());
            }
            
            SettingsForm_Printers.RefreshPrintersList();
            try { Navigation.PopAsync(); } catch (Exception) { }
        }
    }
}
