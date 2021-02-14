using Kara.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace Kara
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReceiptTypeSelect : ContentPage
    {
        private List<KeyValuePair<Guid, decimal>> SelectedOrderIdsPrices;
        private SaleTotalDetails SaleTotalDetailsForm;
        private Guid SelectedPartnerGuid;

        public ReceiptTypeSelect(List<KeyValuePair<Guid,decimal>> selectedOrderIdsPrices,Guid selectedPartnerGuid, SaleTotalDetails saleTotalDetailsForm)
        {
            InitializeComponent();
            SelectedOrderIdsPrices = selectedOrderIdsPrices;
            SaleTotalDetailsForm = saleTotalDetailsForm;
            SelectedPartnerGuid = selectedPartnerGuid;
        }

        private async void btnNaghd_Clicked(object sender, EventArgs e)
        {
            try
            {
                var sum = SelectedOrderIdsPrices.Select(a => a.Value).Sum().ToSafeDecimal();

                ReceiptPecuniary receiptPecuniary = new ReceiptPecuniary(sum, SelectedPartnerGuid)
                {
                    StartColor = Color.FromHex("E6EBEF"),
                    EndColor = Color.FromHex("A6CFED")
                };

                try { await Navigation.PushAsync(receiptPecuniary); } catch (Exception) { }
            }
            catch (Exception ex)
            {
                App.ShowError("خطا",  ex.Message, "باشه");
            }

        }

        private async void btnHavaleh_Clicked(object sender, EventArgs e)
        {
            var sum = SelectedOrderIdsPrices.Select(a => a.Value).Sum().ToSafeDecimal();

            ReceiptBank receiptBank = new ReceiptBank(sum, SelectedPartnerGuid)
            {
                StartColor = Color.FromHex("E6EBEF"),
                EndColor = Color.FromHex("A6CFED")
            };

            //receiptPecuniary.Price = sumSelected;
            //receiptPecuniary.PartnerId = SelectedPartner.Id;
            try { await Navigation.PushAsync(receiptBank); } catch (Exception) { }
        }
        
        private async void btnCheque_Clicked(object sender, EventArgs e)
        {
            var sum = SelectedOrderIdsPrices.Select(a => a.Value).Sum().ToSafeDecimal();

            ReceiptCheque receiptCheque = new ReceiptCheque(sum, SelectedPartnerGuid)
            {
                StartColor = Color.FromHex("E6EBEF"),
                EndColor = Color.FromHex("A6CFED")
            };

            try { await Navigation.PushAsync(receiptCheque); } catch (Exception) { }
        }

        
    }
}