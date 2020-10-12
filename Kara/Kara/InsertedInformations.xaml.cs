using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Kara
{
    public partial class InsertedInformations : TabbedPage
    {
        public InsertedInformations_Orders Orders;
        public InsertedInformations_FailedVisits FailedVisits;
        public InsertedInformations_Partners Partners;
        public ToolbarItem ToolbarItem_SearchBar, ToolbarItem_Delete, ToolbarItem_SendToServer, ToolbarItem_Edit, ToolbarItem_Show, ToolbarItem_SelectAll;

        public InsertedInformations()
        {
            InitializeComponent();

            Partners = new InsertedInformations_Partners(this) { StartColor = Color.FromHex("E6EBEF"), EndColor = Color.FromHex("A6CFED") };
            FailedVisits = new InsertedInformations_FailedVisits(this) { StartColor = Color.FromHex("E6EBEF"), EndColor = Color.FromHex("A6CFED") };
            Orders = new InsertedInformations_Orders(this) { StartColor = Color.FromHex("E6EBEF"), EndColor = Color.FromHex("A6CFED") };
            App.InsertedInformations_Partners = Partners;
            App.InsertedInformations_FailedVisits = FailedVisits;
            App.InsertedInformations_Orders = Orders;
            Children.Add(Partners);
            Children.Add(FailedVisits);
            Children.Add(Orders);

            ToolbarItem_SearchBar = new ToolbarItem();
            ToolbarItem_SearchBar.Text = "جستجو";
            ToolbarItem_SearchBar.Icon = "Search.png";
            ToolbarItem_SearchBar.Order = ToolbarItemOrder.Primary;
            ToolbarItem_SearchBar.Priority = 10;

            ToolbarItem_SelectAll = new ToolbarItem();
            ToolbarItem_SelectAll.Text = "انتخاب همه";
            ToolbarItem_SelectAll.Icon = "SelectAll_Empty.png";
            ToolbarItem_SelectAll.Order = ToolbarItemOrder.Primary;
            ToolbarItem_SelectAll.Priority = 9;

            ToolbarItem_SendToServer = new ToolbarItem();
            ToolbarItem_SendToServer.Text = "ارسال";
            ToolbarItem_SendToServer.Icon = "Upload.png";
            ToolbarItem_SendToServer.Order = ToolbarItemOrder.Primary;
            ToolbarItem_SendToServer.Priority = 1;

            ToolbarItem_Delete = new ToolbarItem();
            ToolbarItem_Delete.Text = "حذف";
            ToolbarItem_Delete.Icon = "Delete.png";
            ToolbarItem_Delete.Order = ToolbarItemOrder.Primary;
            ToolbarItem_Delete.Priority = 2;

            ToolbarItem_Edit = new ToolbarItem();
            ToolbarItem_Edit.Text = "ویرایش";
            ToolbarItem_Edit.Icon = "Edit.png";
            ToolbarItem_Edit.Order = ToolbarItemOrder.Primary;
            ToolbarItem_Edit.Priority = 3;

            ToolbarItem_Show = new ToolbarItem();
            ToolbarItem_Show.Text = "نمایش";
            ToolbarItem_Show.Icon = "ShowInvoice.png";
            ToolbarItem_Show.Order = ToolbarItemOrder.Primary;
            ToolbarItem_Show.Priority = 4;

            this.CurrentPageChanged += (sender, e) => {
                if (this.CurrentPage == Orders)
                {
                    Orders.SetCurrentPage();
                    FailedVisits.UnsetCurrentPage();
                    Partners.UnsetCurrentPage();
                }
                if (this.CurrentPage == FailedVisits)
                {
                    Orders.UnsetCurrentPage();
                    FailedVisits.SetCurrentPage();
                    Partners.UnsetCurrentPage();
                }
                if (this.CurrentPage == Partners)
                {
                    Orders.UnsetCurrentPage();
                    FailedVisits.UnsetCurrentPage();
                    Partners.SetCurrentPage();
                }
            };
            CurrentPage = Orders;
        }

        public void RefreshToolbarItems
        (
            bool BackButton_Visible,
            bool ToolbarItem_SearchBar_Visible,
            bool ToolbarItem_Delete_Visible,
            bool ToolbarItem_SendToServer_Visible,
            bool ToolbarItem_Edit_Visible,
            bool ToolbarItem_Show_Visible,
            bool ToolbarItem_SelectAll_Visible
        )
        {
            if(BackButton_Visible)
                NavigationPage.SetHasBackButton(this, true);
            else
                NavigationPage.SetHasBackButton(this, false);

            if (ToolbarItem_SearchBar_Visible && !this.ToolbarItems.Contains(ToolbarItem_SearchBar))
                this.ToolbarItems.Add(ToolbarItem_SearchBar);
            if (!ToolbarItem_SearchBar_Visible && this.ToolbarItems.Contains(ToolbarItem_SearchBar))
                this.ToolbarItems.Remove(ToolbarItem_SearchBar);

            if (ToolbarItem_Delete_Visible && !this.ToolbarItems.Contains(ToolbarItem_Delete))
                this.ToolbarItems.Add(ToolbarItem_Delete);
            if (!ToolbarItem_Delete_Visible && this.ToolbarItems.Contains(ToolbarItem_Delete))
                this.ToolbarItems.Remove(ToolbarItem_Delete);

            if (ToolbarItem_SendToServer_Visible && !this.ToolbarItems.Contains(ToolbarItem_SendToServer))
                this.ToolbarItems.Add(ToolbarItem_SendToServer);
            if (!ToolbarItem_SendToServer_Visible && this.ToolbarItems.Contains(ToolbarItem_SendToServer))
                this.ToolbarItems.Remove(ToolbarItem_SendToServer);

            if (ToolbarItem_Edit_Visible && !this.ToolbarItems.Contains(ToolbarItem_Edit))
                this.ToolbarItems.Add(ToolbarItem_Edit);
            if (!ToolbarItem_Edit_Visible && this.ToolbarItems.Contains(ToolbarItem_Edit))
                this.ToolbarItems.Remove(ToolbarItem_Edit);

            if (ToolbarItem_Show_Visible && !this.ToolbarItems.Contains(ToolbarItem_Show))
                this.ToolbarItems.Add(ToolbarItem_Show);
            if (!ToolbarItem_Show_Visible && this.ToolbarItems.Contains(ToolbarItem_Show))
                this.ToolbarItems.Remove(ToolbarItem_Show);

            if (ToolbarItem_SelectAll_Visible && !this.ToolbarItems.Contains(ToolbarItem_SelectAll))
                this.ToolbarItems.Add(ToolbarItem_SelectAll);
            if (!ToolbarItem_SelectAll_Visible && this.ToolbarItems.Contains(ToolbarItem_SelectAll))
                this.ToolbarItems.Remove(ToolbarItem_SelectAll);
        }
    }
}
