using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kara.Assets;
using Xamarin.Forms;
using Kara.CustomRenderer;

namespace Kara
{
    public partial class UpdateDBForm : GradientContentPage
    {

        private ToolbarItem UpdateDBToolbarButton;
        
        ObservableCollection<UpdateItemModel> UpdateItems;

        public UpdateDBForm()
        {
            InitializeComponent();

            UpdateDBToolbarButton = new ToolbarItem();
            UpdateDBToolbarButton.Text = "بروزرسانی";
            UpdateDBToolbarButton.Icon = "Refresh.png";
            UpdateDBToolbarButton.Activated += UpdateDB;
            UpdateDBToolbarButton.Order = ToolbarItemOrder.Primary;
            UpdateDBToolbarButton.Priority = 0;
            this.ToolbarItems.Add(UpdateDBToolbarButton);

            UpdateItems = new ObservableCollection<UpdateItemModel>();
            UpdateItems.Add(new UpdateItemModel("Stuffs", "کالاها", new PartialUpdateDB_Stuffs(), 100));
            UpdateItems.Add(new UpdateItemModel("StuffGallary", "گالری کالا", new PartialUpdateDB_StuffImages(), 1));
            UpdateItems.Add(new UpdateItemModel("Stocks", "موجودی", new PartialUpdateDB_Stocks(), 100));
            UpdateItems.Add(new UpdateItemModel("Partners", "مشتریان", new PartialUpdateDB_Partners(), 100));
            UpdateItems.Add(new UpdateItemModel("PriceLists", "لیست قیمت", new PartialUpdateDB_PriceLists(), 100));
            UpdateItems.Add(new UpdateItemModel("DiscountRules", "فرمول تخفیفات", new PartialUpdateDB_DiscountRules(), 100));
            UpdateItems.Add(new UpdateItemModel("OtherInformations", "سایر اطلاعات", new PartialUpdateDB_OtherInformations(), 100));

            UpdatableItems.ItemsSource = UpdateItems;
        }

        async void UpdateDB(object sender, EventArgs e)
        {
            try
            {
                this.ToolbarItems.Remove(UpdateDBToolbarButton);
                
                foreach (var item in UpdateItems)
                    item.UpdateOperationNotStarted = false;
                
                var UpdatingItems = UpdateItems.Where(a => a.Selected).ToArray();
                foreach (var UpdatingItem in UpdatingItems)
                {
                    UpdatingItem.IsUpdating = true;
                    var resultTask = Connectivity.UpdateDB(UpdatingItem);
                    var result = await resultTask;
                    UpdatingItem.IsUpdating = false;

                    if (resultTask.Exception != null)
                    {
                        this.ToolbarItems.Add(UpdateDBToolbarButton);
                        foreach (var item in UpdateItems)
                            item.UpdateOperationNotStarted = true;
                        ShowError(resultTask.Exception.ProperMessage());
                        return;
                    }
                    if (!result.Success)
                    {
                        this.ToolbarItems.Add(UpdateDBToolbarButton);
                        foreach (var item in UpdateItems)
                            item.UpdateOperationNotStarted = true;
                        ShowError(result.Message);
                        return;
                    }

                    if (UpdatingItem.PartialDBUpdater.GetType().Equals(typeof(PartialUpdateDB_Stuffs)))
                    {
                        await App.DB.FetchStuffsListAsync();
                        DBRepository._StuffsSettlementDays = null;
                    }
                }

                DBRepository._AllStuffsDataInitialized = false;
                App.DB.ClearFetchedDiscountRules();

                if(App.LastPriceListOrDiscountRuleVersionChanged)
                {
                    await App.RecalculateUnsentOrdersPriceAndDiscount();
                    App.LastPriceListOrDiscountRuleVersionChanged = false;
                }

                ShowSuccess("داده های مورد نظر با موفقیت به روز شد.");
            }
            catch (Exception err)
            {
                ShowError(err.ProperMessage());
                this.ToolbarItems.Remove(UpdateDBToolbarButton);
                foreach (var item in UpdateItems)
                    item.UpdateOperationNotStarted = true;
            }
        }

        private void ShowError(string Message)
        {
            foreach (var UpdateItem in UpdateItems)
                UpdateItem.Progress = 0;
            App.ShowError("خطا", "در بروز رسانی داده های مورد نظر خطایی رخ داده است. لطفا مجددا تلاش کرده و در صورت مشاهده مجدد این خطا با پشتیبانی نرم افزار تماس بگیرید.\n" + Message, "خوب");
        }

        private async void ShowSuccess(string Message)
        {
            App.ToastMessageHandler.ShowMessage(Message, Helpers.ToastMessageDuration.Long);
            await Task.Delay(3000);

            if (Navigation.NavigationStack.Count() > 1)
                try { Navigation.PopAsync(); } catch (Exception) { }
        }
    }
}
