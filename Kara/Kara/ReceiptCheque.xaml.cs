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
        BankAccount[] BankAccounts;

        FinancialTransactionDocument EditingFDT;
        public Guid PartnerId { get; set; }
        private ToolbarItem ToolbarItem_SendToServer, ToolbarItem_LocalSave;
        bool JustShow = false;


        //public static readonly BindableProperty PriceProperty = 
        //    BindableProperty.Create("Price", typeof(decimal), typeof(ReceiptPecuniary), (decimal)0);

        //public decimal Price
        //{
        //    get { return (decimal)GetValue(PriceProperty); }
        //    set { SetValue(PriceProperty, value); }
        //}

        public decimal Price { get; set; }

        public ReceiptCheque(decimal price,Guid partnerId)
        {
            InitializeComponent();
            
            //txtPrice.SetBinding(Entry.TextProperty, "Price");
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

            FillBankAccounts();
        }

        private async void FillBankAccounts()
        {
            var result = await App.DB.GetBankAccountsAsync();

            if(result.Success && result.Data!=null)
            {
                BankAccounts = result.Data.ToArray();

                if (BankAccounts != null)
                    foreach (var b in BankAccounts)
                        BankAccountPicker.Items.Add(b.Name);
            }
            else
            {
                App.ShowError("خطا", "هیچ حساب بانکی وجود ندارد لطفا اطلاعات را به روزرسانی کنید", "خوب");
            }
            
        }

        private async void SubmitToServer(object sender, System.EventArgs e)
        {
            WaitToggle(false);
            await Task.Delay(100);
            var SaveResult = await SaveReceiptBank();

            if (!SaveResult.Success)
            {
                WaitToggle(false);
                App.ShowError("خطا", SaveResult.Message, "خوب");
            }
            else
            {
                var submitResult = await Connectivity.SubmitReceiptBankAsync(new FinancialTransactionDocument[] { SaveResult.Data });

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
            var SaveResult = await SaveReceiptBank();

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

        private async Task<ResultSuccess<FinancialTransactionDocument>> SaveReceiptBank()
        {
            try
            {
                if (BankAccountPicker.SelectedIndex == -1)
                    return new ResultSuccess<FinancialTransactionDocument>(false, "بانک انتخاب نشده است.");

                //if (EditingFDT == null)
                //{

                EditingFDT = new FinancialTransactionDocument()
                    {
                        DocumentId = Guid.NewGuid(),
                        YearId = Guid.Parse("00000000-0000-0000-0000-000000000000"),
                        TransactionType = (int)TransactionType.BankDocumentIn,
                        PartnerId = PartnerId,
                        InputPrice = decimal.Parse(txtPrice.Text.Replace(",", "").ReplacePersianDigits()),
                        OutputPrice = 0,
                        DocumentState = 0,
                        PersianDocumentDate = txtDate.Text.ReplacePersianDigits(),
                        DocumentDate = txtDate.Text.PersianDateStringToDate(),
                        DocumentUserId = App.UserId.Value,
                        DocumentDescription = txtDesc.Text,
                        BankAccountId = BankAccounts[BankAccountPicker.SelectedIndex].Id,
                        BankTransferCode = txtHavalehNo.Text.ReplacePersianDigits(),
                        ChequeCode = "",
                        BranchName = "",
                        BranchCode = "",
                        Delivery = "",
                        Issuance = "",
                        CollectorId = App.UserEntityId.Value
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