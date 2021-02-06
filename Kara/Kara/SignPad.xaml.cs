using Kara.Assets;
using Kara.CustomRenderer;
using SignaturePad.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XLabs;
using static Kara.Assets.Connectivity;
using Connectivity = Kara.Assets.Connectivity;


namespace Kara
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignPad : ContentPage
    {
        private List<Guid> SelectedOrderIds;
        private SaleTotalDetails SaleTotalDetailsForm;

        public SignPad(List<Guid> selectedOrderIds, SaleTotalDetails saleTotalDetailsForm)
        {
            InitializeComponent();
            SelectedOrderIds = selectedOrderIds;
            SaleTotalDetailsForm = saleTotalDetailsForm;
        }

        private async void btnOk_Clicked(object sender, EventArgs e)
        {
            Stream stream = await signatureView.GetImageStreamAsync(SignatureImageFormat.Png);
            try
            {
                foreach (var orderId in SelectedOrderIds)
                {
                    var result = await Connectivity.AddSigniture(orderId, stream.ConvertToBase64());

                    if (!result.Success)
                    {
                        App.ShowError("خطا در تایید فاکتورها", "" + "\n" + result.Message, "خوب");
                        await Navigation.PopModalAsync();
                        return;
                    }

                    if (result!=null && result.Data!=null)
                    {
                        var relatedOrder = SaleTotalDetailsForm.TotalDetailsObservableCollection.FirstOrDefault(a => a.OrderId == result.Data.OrderId);

                        if(relatedOrder!=null)
                        {
                            relatedOrder.Confirmed = true;
                            relatedOrder.OrderCode = result.Data.OrderCode.ToSafeString().ToPersianDigits();
                        }
                    }
                }

                App.ToastMessageHandler.ShowMessage("فاکتورها تایید شدند", Helpers.ToastMessageDuration.Short);
                await Navigation.PopModalAsync();
            }
            catch (Exception ex)
            {
                App.ShowError("خطا", "خطا در تایید فاکتورها" + "\n" + ex.Message, "باشه");
            }

        }

        private void btnCancel_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
}