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
    public partial class SettingsForm_ServerAddressAddEdit : GradientContentPage
    {
        SettingsForm_ServerAddress SettingsForm_ServerAddress;
        Entry ServerAddress = new MyEntry() { HorizontalTextAlignment = TextAlignment.End, Placeholder = "آدرس سرور، مثلا: 192.168.1.2".ToPersianDigits(), LeftRounded = true };
        ToolbarItem ToolbarItem_OK;
        string EditingAddress;

        public SettingsForm_ServerAddressAddEdit(string Address, bool Editing, SettingsForm_ServerAddress SettingsForm_ServerAddress)
        {
            InitializeComponent();

            this.SettingsForm_ServerAddress = SettingsForm_ServerAddress;

            ToolbarItem_OK = new ToolbarItem();
            ToolbarItem_OK.Text = "تایید";
            ToolbarItem_OK.Icon = "Save.png";
            ToolbarItem_OK.Order = ToolbarItemOrder.Primary;
            ToolbarItem_OK.Priority = 1;
            ToolbarItem_OK.Activated += ToolbarItem_OK_Activated;
            ToolbarItems.Add(ToolbarItem_OK);

            EditingAddress = Editing ? Address : "";
            ServerAddress.Text = Address;
            LayoutStack.Children.Add(ServerAddress);
        }

        private void ToolbarItem_OK_Activated(object sender, EventArgs e)
        {
            var AllAddresses = App.AllServerAddresses.Value.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries)
                .Select(a => new
                {
                    Address = a.Split('_')[0],
                    Selected = Convert.ToBoolean(a.Split('_')[1])
                }).ToArray();

            if (ServerAddress.Text == "")
            {
                App.ShowError("خطا", "آدرس وارد شده معتبر نیست.", "خوب");
                return;
            }

            if (EditingAddress == "")//Insert
            {
                if(AllAddresses.Any(a => a.Address == ServerAddress.Text.ToLatinDigits()))
                {
                    App.ShowError("خطا", "آدرس وارد شده به ثبت رسیده است.", "خوب");
                    return;
                }

                AllAddresses = AllAddresses.Select(a => new
                {
                    Address = a.Address,
                    Selected = false
                }).Union(new[] { new
                {
                    Address = ServerAddress.Text.ToLatinDigits(),
                    Selected = true
                } }).ToArray();
            }
            else//Update
            {
                AllAddresses = AllAddresses.Select(a => new
                {
                    Address = a.Address == EditingAddress.ToLatinDigits() ? ServerAddress.Text.ToLatinDigits() : a.Address,
                    Selected = a.Selected
                }).ToArray();
            }

            App.AllServerAddresses.Value = AllAddresses.Any() ? AllAddresses.Select(a => a.Address + "_" + a.Selected.ToString()).Aggregate((sum, x) => sum + "|" + x) : "";
            App.ServerAddress = AllAddresses.First(a => a.Selected).Address;

            SettingsForm_ServerAddress.RefreshAddressList();
            try { Navigation.PopAsync(); } catch (Exception) { }
        }
    }
}
