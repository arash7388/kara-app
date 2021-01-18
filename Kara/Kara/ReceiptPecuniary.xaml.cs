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
        Cash[] Cashes;

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

        public ReceiptPecuniary(decimal price,Guid partnerId)
        {
            InitializeComponent();
            
            //txtPrice.SetBinding(Entry.TextProperty, "Price");
            Price = price;
            PartnerId = partnerId;

            txtPrice.Text = Price.ToString().ToPersianDigits();
            txtDate.Text = DateTime.Now.ToShortStringForDate().ToPersianDigits();
            //txtDate.TextChanged += TxtDate_TextChanged;

            //txtDate.Focused += TxtDate_Focused;
            //txtDate.Unfocused += TxtDate_Unfocused;
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

            FillCashes();
        }

        //private void TxtDate_Unfocused(object sender, FocusEventArgs e)
        //{
        //    ((Entry)sender).Text = ((Entry)sender).Text.ReplaceLatinDigits();
        //}

        //private void TxtDate_Focused(object sender, FocusEventArgs e)
        //{
        //    ((Entry)sender).Text = DateTime.Now.ToShortStringForDate().ReplaceLatinDigits().Substring(0, 4) + "/"; 
        //}

        //private void TxtDate_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    txtDate.TextChanged -= TxtDate_TextChanged;
        //    ((Entry)sender).Text = ((Entry)sender).Text.ReplaceLatinDigits();
        //    txtDate.TextChanged += TxtDate_TextChanged;
        //}

        private async void FillCashes()
        {
            Cashes = (await App.DB.GetCashesAsync()).Data.ToArray();

            foreach (var c in Cashes)
                CashPicker.Items.Add(c.EntityName);
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
                if (CashPicker.SelectedIndex == -1)
                    return new ResultSuccess<FinancialTransactionDocument>(false, "صندوق انتخاب نشده است.");

                //var ZoneId = Zones.Where(a => a.ParentId == CityId).ToArray()[ZonePicker.SelectedIndex].Id;


                //if (EditingFDT == null)
                //{
                EditingFDT = new FinancialTransactionDocument()
                    {
                        DocumentId = Guid.NewGuid(),
                        YearId = Guid.Parse("00000000-0000-0000-0000-000000000000"),
                        TransactionType = 1,
                        PartnerId = PartnerId,
                        CashAccountId = Cashes[CashPicker.SelectedIndex].EntityId, 
                        InputPrice = decimal.Parse(txtPrice.Text.Replace(",", "").ToLatinDigits()),
                        OutputPrice = 0,
                        DocumentCode = "",
                        DocumentState = 0,
                        PersianDocumentDate = txtDate.Text.ToLatinDigits(),
                        DocumentDate = txtDate.Text.PersianDateStringToDate(),
                        DocumentUserId = App.UserId.Value,
                        DocumentDescription = txtDesc.Text,
                        ChequeCode = "",
                        BranchName = "",
                        BranchCode = "",
                        Delivery = "",
                        Issuance = "",
                        BankTransferCode = "",
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