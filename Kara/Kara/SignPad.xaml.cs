using Kara.Assets;
using SignaturePad.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kara
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignPad : ContentPage
    {
        private List<Guid> SelectedOrderIds;

        public SignPad(List<Guid> selectedOrderIds)
        {
            InitializeComponent();
            SelectedOrderIds = selectedOrderIds;
        }

        private async void btnOk_Clicked(object sender, EventArgs e)
        {
            Stream stream = await signatureView.GetImageStreamAsync(SignatureImageFormat.Png);
            try
            {
                foreach (var orderId in SelectedOrderIds)
                {
                    await Connectivity.AddSignature(orderId, stream.ConvertToBase64());
                }
                
            }
            catch (Exception ex)
            {
                App.ShowError("خطا", "خطا در تایید فاکتورها"+"\n" + ex.Message, "باشه");
            }
            
        }

        private void btnCancel_Clicked(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
}