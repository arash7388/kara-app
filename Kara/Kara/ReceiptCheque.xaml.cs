using Kara.Assets;
using Kara.CustomRenderer;
using Kara.Models;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Kara.Models.ModelsInterface;

namespace Kara
{
    
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReceiptCheque : GradientContentPage
    {
        Bank[] Banks;
        Cash[] Cashes;

        FinancialTransactionDocument EditingFDT;
        public Guid PartnerId { get; set; }
        private ToolbarItem ToolbarItem_SendToServer, ToolbarItem_LocalSave;
        bool JustShow = false;


        public decimal Price { get; set; }

        public ReceiptCheque(decimal price,Guid partnerId)
        {
            InitializeComponent();
            
            Price = price;
            PartnerId = partnerId;

            txtPrice.Text = Price.ToString().ReplaceLatinDigits();
            txtDate.Text = DateTime.Now.ToShortStringForDate().ReplaceLatinDigits();
            
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

            FillBanks();
            FillCashes();
        }

        private async void FillBanks()
        {
            var result = await App.DB.GetBanksAsync();

            if(result.Success && result.Data!=null)
            {
                Banks = result.Data.ToArray();

                if (Banks != null)
                    foreach (var b in Banks)
                        BankPicker.Items.Add(b.Name);
            }
            else
            {
                App.ShowError("خطا", "هیچ بانکی وجود ندارد لطفا اطلاعات را به روزرسانی کنید", "خوب");
            }
            
        }

        private async void FillCashes()
        {
            var result = await App.DB.GetCashesAsync();

            if (result.Success && result.Data != null)
            {
                Cashes = result.Data.ToArray();

                if (Cashes != null)
                    foreach (var c in Cashes)
                        CashPicker.Items.Add(c.EntityName);
            }
            else
            {
                App.ShowError("خطا", "هیچ صندوقی وجود ندارد لطفا اطلاعات را به روزرسانی کنید", "خوب");
            }

        }

        private async void SubmitToServer(object sender, System.EventArgs e)
        {
            WaitToggle(false);
            await Task.Delay(100);
            var SaveResult = await SaveReceiptCheque();

            if (!SaveResult.Success)
            {
                WaitToggle(false);
                App.ShowError("خطا", SaveResult.Message, "خوب");
            }
            else
            {
                var submitResult = await Connectivity.SubmitReceiptChequeAsync(new FinancialTransactionDocument[] { SaveResult.Data });

                if (!submitResult.Success)
                {
                    WaitToggle(false);
                    App.ShowError("خطا", "اطلاعات به صورت محلی ثبت شد اما در ارسال اطلاعات به سرور خطایی رخ داده است: " + submitResult.Message, "خوب");
                }
                else
                {
                    WaitToggle(true);

                    Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count - 1]);
                    
                    App.ToastMessageHandler.ShowMessage("اطلاعات با موفقیت به سرور ارسال شد.", Helpers.ToastMessageDuration.Long);
                    try { await Navigation.PopAsync(); } catch (Exception) { }
                }
            }
        }
        
        private async void SubmitToStorage(object sender, System.EventArgs e)
        {
            WaitToggle(false);
            await Task.Delay(100);
            var SaveResult = await SaveReceiptCheque();

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

        private async Task<ResultSuccess<FinancialTransactionDocument>> SaveReceiptCheque()
        {
            try
            {
                if (BankPicker.SelectedIndex == -1)
                    return new ResultSuccess<FinancialTransactionDocument>(false, "بانک انتخاب نشده است.");

                if (CashPicker.SelectedIndex == -1)
                    return new ResultSuccess<FinancialTransactionDocument>(false, "صندوق انتخاب نشده است.");

                if(string.IsNullOrWhiteSpace(txtBranchName.Text))
                    return new ResultSuccess<FinancialTransactionDocument>(false, "نام شعبه را وارد نمایید.");

                if (string.IsNullOrWhiteSpace(txtChequeCode.Text))
                    return new ResultSuccess<FinancialTransactionDocument>(false, "شماره چک را وارد نمایید.");

                if (string.IsNullOrWhiteSpace(txtAccountNo.Text))
                    return new ResultSuccess<FinancialTransactionDocument>(false, "شماره حساب را وارد نمایید.");

                if (string.IsNullOrWhiteSpace(txtDueDate.Text))
                    return new ResultSuccess<FinancialTransactionDocument>(false, "تاریخ سررسید را وارد نمایید.");

                //if (EditingFDT == null)
                //{

                EditingFDT = new FinancialTransactionDocument()
                    {
                        DocumentId = Guid.NewGuid(),
                        YearId = Guid.Parse("00000000-0000-0000-0000-000000000000"),
                        TransactionType = (int)TransactionType.ChequeDocumentIn,
                        PartnerId = PartnerId,
                        InputPrice = decimal.Parse(txtPrice.Text.Replace(",", "").ReplacePersianDigits()),
                        OutputPrice = 0,
                        DocumentState = 0,
                        PersianDocumentDate = txtDate.Text.ReplacePersianDigits(),
                        DocumentDate = txtDate.Text.PersianDateStringToDate(),
                        DocumentUserId = App.UserId.Value,
                        DocumentDescription = txtDesc.Text,
                        //BankTransferCode = txtHavalehNo.Text.ReplacePersianDigits(),
                        ChequeCode = txtChequeCode.Text.ReplacePersianDigits(),
                        BranchName = txtBranchName.Text,
                        BranchCode = txtBranchCode.Text,
                        Delivery = "-",
                        Issuance = txtCity.Text,
                        PersianMaturityDate = txtDueDate.Text.ReplacePersianDigits(),
                        CollectorId = App.UserEntityId.Value,
                        ChequeBankId = Banks[BankPicker.SelectedIndex].Id,
                        CashAccountId = Cashes[CashPicker.SelectedIndex].EntityId,
                        BankAccountNumber = txtAccountNo.Text.ReplacePersianDigits()
                };

                    var result = await App.DB.InsertOrUpdateRecordAsync<FinancialTransactionDocument>(EditingFDT);
                    if (!result.Success)
                        return new ResultSuccess<FinancialTransactionDocument>(false, result.Message);
                   
                //}
                //else
                //{
                //    EditingFDT.InputPrice = decimal.Parse(txtPrice.Text.Replace(",", ""));
                //    EditingFDT.DocumentDate = txtDate.Text.PersianDateStringToDate();
                //    EditingFDT.PersianDocumentDate = txtDate.Text;
                //    EditingFDT.DocumentDescription = txtDesc.Text;

                    
                //    var result = await App.DB.InsertOrUpdateRecordAsync<FinancialTransactionDocument>(EditingFDT);
                //    if (!result.Success)
                //        return new ResultSuccess<FinancialTransactionDocument>(false, result.Message);
                //}

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

