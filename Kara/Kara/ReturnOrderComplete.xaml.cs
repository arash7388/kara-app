using Kara.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Kara.Assets.Connectivity;

namespace Kara
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReturnOrderComplete : ContentPage
    {
        private Guid SelectedOrderId;
        DtoReversionReason[] ReversionReasons = null;
        SaleTotalDetails SaleTotalDetailsForm;
        public ReturnOrderComplete(Guid selectedOrderId,SaleTotalDetails saleTotalDetailsForm)
        {
            InitializeComponent();
            SelectedOrderId = selectedOrderId;
            SaleTotalDetailsForm = saleTotalDetailsForm;
            FillReversionReasons();
        }

        private async Task FillReversionReasons()
        {
            try
            {
                var result = await Connectivity.GetReversionReasons();

                if (result.Success && result.Data != null)
                {
                    ReversionReasons = result.Data.ToArray();

                    if (ReversionReasons != null)
                        foreach (var r in ReversionReasons)
                            ReversionReasonPicker.Items.Add(r.Name);
                }
                else
                {
                    App.ShowError("خطا", "هیچ علتی در سیستم تعریف نشده است ", "خوب");
                }
            }
            catch (Exception ex)
            {
                App.ShowError("خطا", ex.Message, "خوب");
            }
        }


        private async void btnReturn_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (ReversionReasonPicker.SelectedIndex == -1)
                {
                    App.ShowError("خطا", "دلیل انتخاب نشده است.", "خوب");
                    return;
                }

                var result = await Connectivity.ReturnOrderCompletely(SelectedOrderId, ReversionReasons[ReversionReasonPicker.SelectedIndex].Id, txtDesc.Text);

                if (!result.Success)
                {
                    App.ShowError("خطا در بازگشت کامل", result.Message, "خوب");
                    return;
                }

                if (!result.Data.IsSuccess)
                {
                    App.ShowError("خطا در بازگشت کامل", result.Message, "خوب");
                    return;
                }

                if (result.Data.IsSuccess)
                {
                    App.ToastMessageHandler.ShowMessage("فاکتور به طور کامل بازگشت داده شد", Helpers.ToastMessageDuration.Short);
                    await Navigation.PopAsync();
                    SaleTotalDetailsForm.RefreshDetails();
                }

            }
            catch (Exception ex)
            {
                App.ShowError("خطا", ex.Message, "خوب");
            }
        }
    }
}