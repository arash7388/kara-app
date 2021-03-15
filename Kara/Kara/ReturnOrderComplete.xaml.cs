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

                var orderData = await Connectivity.GetEditData(SelectedOrderId, true, true, false, false);

                if (!orderData.Success)
                {
                    App.ShowError("خطا در دریافت اطلاعات فاکتور", orderData.Message, "خوب");
                    return;
                }

                if (orderData.Data.SaleOrder == null)
                {
                    App.ShowError("خطا در دریافت اطلاعات فاکتور", "orderData.Data.SaleOrder is null", "خوب");
                    return;
                }

                var data = new DtoEditSaleOrderMobile()
                {
                    DistributionReversionReasonId = ReversionReasons[ReversionReasonPicker.SelectedIndex].Id,
                    OrderId = SelectedOrderId,
                    UserId = App.UserId.Value,
                    Description = "***",
                    Stuffs = orderData.Data.SaleOrderStuffs.Where(a => a.Quantity > 0).Select(a => new DtoEditStuffMobile
                    {
                        ArticleId = a.Id,
                        StuffId = a.StuffId.ToSafeGuid(),
                        PackageId = a.PackageId,
                        Quantity = 0,
                        SalePrice = a.SalePrice,
                        StuffQuantity = 0,
                        BatchNumberId = a.BatchNumberId.ToSafeString() != "" ? Guid.Parse(a.BatchNumberId) : (Guid?)null,
                    }).ToList()
                };

                var result = await Connectivity.EditSaleOrder(data);

                if (!result.Success)
                {
                    App.ShowError("خطا در بازگشت کامل", result.Message, "خوب");
                    return;
                }

                //if (!result.Data.IsSuccess)
                //{
                //    App.ShowError("خطا در بازگشت کامل", result.Message, "خوب");
                //    return;
                //}

                if (result.Success)
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