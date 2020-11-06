using Kara.Assets;
using Kara.CustomRenderer;
using Kara.Models;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kara
{
    
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReceiptPecuniary : GradientContentPage
    {
        FinancialTransactionDocument EditingFDT;
        public Guid PartnerId { get; set; }
        private ToolbarItem ToolbarItem_SendToServer, ToolbarItem_LocalSave;
        bool JustShow = false;

        public decimal Price { get; set; }

        public ReceiptPecuniary()
        {
            InitializeComponent();

            txtDate.Text = DateTime.Now.ToShortStringForDate();

            ToolbarItem_LocalSave = new ToolbarItem();
            ToolbarItem_LocalSave.Text = "ذخیره محلی";
            ToolbarItem_LocalSave.Icon = "Save.png";
            ToolbarItem_LocalSave.Clicked += SubmitToStorage;
            ToolbarItem_LocalSave.Order = ToolbarItemOrder.Primary;
            ToolbarItem_LocalSave.Priority = 0;
            if (!JustShow)
                this.ToolbarItems.Add(ToolbarItem_LocalSave);

            ToolbarItem_SendToServer = new ToolbarItem();
            ToolbarItem_SendToServer.Text = "ارسال به سرور";
            ToolbarItem_SendToServer.Icon = "Upload.png";
            ToolbarItem_SendToServer.Activated += SubmitToServer;
            ToolbarItem_SendToServer.Order = ToolbarItemOrder.Primary;
            ToolbarItem_SendToServer.Priority = 0;
            if (!JustShow)
                this.ToolbarItems.Add(ToolbarItem_SendToServer);
        }

        private async void SubmitToServer(object sender, System.EventArgs e)
        {
            WaitToggle(false);
            await Task.Delay(100);
            var SaveResult = await SaveReceiptPecuniary();

            if (!SaveResult.Success)
            {
                WaitToggle(false);
                App.ShowError("خطا", SaveResult.Message, "خوب");
            }
            else
            {
                var submitResult = await Connectivity.SubmitReceiptPecuniaryAsync(new FinancialTransactionDocument[] { SaveResult.Data });

                if (!submitResult.Success)
                {
                    WaitToggle(false);
                    App.ShowError("خطا", "اطلاعات به صورت محلی ثبت شد اما در ارسال اطلاعات به سرور خطایی رخ داده است: " + submitResult.Message, "خوب");
                }
                else
                {
                    WaitToggle(true);
                    App.ToastMessageHandler.ShowMessage("اطلاعات با موفقیت به سرور ارسال شد.", Helpers.ToastMessageDuration.Long);
                    try { await Navigation.PopAsync(); } catch (Exception) { }
                }
            }
        }
        
        private async void SubmitToStorage(object sender, System.EventArgs e)
        {
            WaitToggle(false);
            await Task.Delay(100);
            var SaveResult = await SaveReceiptPecuniary();

            if (!SaveResult.Success)
            {
                WaitToggle(false);
                App.ShowError("خطا", SaveResult.Message, "خوب");
            }
            else
            {
                WaitToggle(true);
                App.ToastMessageHandler.ShowMessage("اطلاعات با موفقیت به صورت محلی ثبت شد.", Helpers.ToastMessageDuration.Long);
                try { await Navigation.PopAsync(); } catch (Exception) { }
            }
        }

        private async Task<ResultSuccess<FinancialTransactionDocument>> SaveReceiptPecuniary()
        {
            try
            {
                //if (CityPicker.SelectedIndex == -1)
                //    return new ResultSuccess<Partner>(false, "شهر انتخاب نشده است.");

                //var ZoneId = Zones.Where(a => a.ParentId == CityId).ToArray()[ZonePicker.SelectedIndex].Id;
                               

                if (EditingFDT == null)
                {
                    EditingFDT = new FinancialTransactionDocument()
                    {
                        DocumentId = Guid.NewGuid(),
                        YearId = Guid.Parse("00000000-0000-0000-0000-000000000000"),
                        TransactionType = 1,
                        PartnerId = PartnerId,
                        CashAccountId = Guid.Parse("8c2876e4-03b7-462b-9a8f-621bb2a647d5"), //read from config
                        InputPrice = decimal.Parse(txtPrice.Text.Replace(",", "")),
                        OutputPrice = 0,
                        DocumentCode = "",
                        DocumentState = 0,
                        PersianDocumentDate = txtDate.Text,
                        DocumentDate = txtDate.Text.PersianDateStringToDate(),
                        DocumentUserId = App.UserId.Value,
                        DocumentDescription = txtDesc.Text,
                        ChequeCode = "",
                        BranchName = "",
                        BranchCode = "",
                        Delivery = "",
                        Issuance = "",
                        BankTransferCode = "",
                        CollectorId = App.UserId.Value
                    };

                    var result = await App.DB.InsertOrUpdateRecordAsync<FinancialTransactionDocument>(EditingFDT);
                    if (!result.Success)
                        return new ResultSuccess<FinancialTransactionDocument>(false, result.Message);
                   
                }
                else
                {
                    EditingFDT.InputPrice = decimal.Parse(txtPrice.Text.Replace(",", ""));
                    EditingFDT.DocumentDate = txtDate.Text.PersianDateStringToDate();
                    EditingFDT.DocumentDescription = txtDesc.Text;

                    
                    var result = await App.DB.InsertOrUpdateRecordAsync<FinancialTransactionDocument>(EditingFDT);
                    if (!result.Success)
                        return new ResultSuccess<FinancialTransactionDocument>(false, result.Message);
                }

                //if (PartnerListForm != null)
                //    await PartnerListForm.FillPartners();

                return new ResultSuccess<FinancialTransactionDocument>(true, "", EditingFDT);
            }
            catch (Exception err)
            {
                return new ResultSuccess<FinancialTransactionDocument>(false, err.ProperMessage());
            }
        }

        public void WaitToggle(bool FormWorkFinished)
        {
            if (!BusyIndicatorContainder.IsVisible)
            {
                BusyIndicatorContainder.IsVisible = true;
                this.ToolbarItems.Remove(ToolbarItem_LocalSave);
                this.ToolbarItems.Remove(ToolbarItem_SendToServer);
            }
            else
            {
                BusyIndicatorContainder.IsVisible = false;
                if (!FormWorkFinished && !JustShow)
                {
                    this.ToolbarItems.Add(ToolbarItem_LocalSave);
                    this.ToolbarItems.Add(ToolbarItem_SendToServer);
                }
            }
        }
    }
}