using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kara.Models;
using Xamarin.Forms;
using System.ComponentModel;
using Kara.Assets;
using Kara.CustomRenderer;

namespace Kara
{   
    public partial class FailedOrderInsertForm : GradientContentPage
    {
        private ToolbarItem ToolbarItem_LocalSave, ToolbarItem_SendToServer;
        
        NotOrderReason[] NotOrderReasons;
        Guid? ShowingId;
        InsertedInformations_FailedVisits FailedVisitsForm;
        PartnerListForm PartnerListForm;

        private Partner _SelectedPartner;
        public Partner SelectedPartner
        {
            get { return _SelectedPartner; }
            set
            {
                _SelectedPartner = value;
                PartnerSelected();
            }
        }

        public FailedOrderInsertForm(Partner Partner, Guid? ShowingId, InsertedInformations_FailedVisits FailedVisitsForm, PartnerListForm PartnerListForm)
        {
            InitializeComponent();
            
            Initialize(Partner, ShowingId, FailedVisitsForm, PartnerListForm);
        }

        private async Task Initialize(Partner Partner, Guid? ShowingId, InsertedInformations_FailedVisits FailedVisitsForm, PartnerListForm PartnerListForm)
        {
            BusyIndicatorContainder.BackgroundColor = Color.FromRgba(255, 255, 255, 70);
            BusyIndicator.Color = Color.FromRgba(80, 100, 150, 255);

            ToolbarItem_LocalSave = new ToolbarItem();
            ToolbarItem_LocalSave.Text = "ذخیره محلی";
            ToolbarItem_LocalSave.Icon = "Save.png";
            ToolbarItem_LocalSave.Activated += SubmitFailedVisitToStorage;
            ToolbarItem_LocalSave.Order = ToolbarItemOrder.Primary;
            ToolbarItem_LocalSave.Priority = 0;
            this.ToolbarItems.Add(ToolbarItem_LocalSave);

            ToolbarItem_SendToServer = new ToolbarItem();
            ToolbarItem_SendToServer.Text = "ذخیره محلی";
            ToolbarItem_SendToServer.Icon = "Upload.png";
            ToolbarItem_SendToServer.Activated += SubmitFailedVisitToServer;
            ToolbarItem_SendToServer.Order = ToolbarItemOrder.Primary;
            ToolbarItem_SendToServer.Priority = 0;
            this.ToolbarItems.Add(ToolbarItem_SendToServer);

            this.ShowingId = ShowingId;
            var ShowingFailedVisit = ShowingId.HasValue ? (await App.DB.GetFailedVisitAsync(ShowingId.Value)) : null;
            this.FailedVisitsForm = FailedVisitsForm;
            this.PartnerListForm = PartnerListForm;

            SelectedPartner = Partner != null ? Partner : ShowingFailedVisit != null ? ShowingFailedVisit.Data.Partner : null;
            PartnerSelected();
            if (SelectedPartner == null)
                PartnerChangeButton.IsEnabled = true;
            else
                PartnerChangeButton.IsEnabled = false;
            PartnerLabel.FontSize *= 1.5;

            var PartnerChangeButtonTapGestureRecognizer = new TapGestureRecognizer();
            PartnerChangeButtonTapGestureRecognizer.Tapped += (sender, e) =>
            {
                if (PartnerChangeButton.IsEnabled)
                {
                    var PartnerListForm1 = new PartnerListForm();
                    PartnerListForm1.FailedOrderInsertForm = this;
                    Navigation.PushAsync(PartnerListForm1);
                }
            };
            PartnerChangeButtonTapGestureRecognizer.SetBinding(TapGestureRecognizer.CommandParameterProperty, "Id");
            PartnerChangeButton.GestureRecognizers.Add(PartnerChangeButtonTapGestureRecognizer);
            PartnerChangeButton.WidthRequest = 150;

            var NotOrderReasonsResult = await App.DB.GetNotOrderReasonsAsync();
            if (!NotOrderReasonsResult.Success)
            {
                App.ShowError("خطا", "NotOrderReasonsResult.Message", "خوب");
                try { Navigation.PopAsync(); } catch (Exception) { }
            }
            NotOrderReasons = NotOrderReasonsResult.Data.ToArray();
            foreach (var NotOrderReason in NotOrderReasons)
                FailedOrderReason.Items.Add(NotOrderReason.Name);

            FailedOrderReason.SelectedIndexChanged += (sender, e) =>
            {
                FailedOrderReasonLabel.Text = NotOrderReasons[FailedOrderReason.SelectedIndex].Name;
            };

            if (ShowingFailedVisit != null)
            {
                for (int i = 0; i < NotOrderReasons.Length; i++)
                {
                    if (NotOrderReasons[i].Id == ShowingFailedVisit.Data.NotOrderReason.Id)
                    {
                        FailedOrderReason.SelectedIndex = i;
                        FailedOrderReasonLabel.Text = NotOrderReasons[i].Name;
                    }
                }
                Description.Text = ShowingFailedVisit.Data.FailedVisit.Description;
                if (FailedVisitsForm == null)
                {
                    Description.IsEnabled = false;
                    FailedOrderReason.IsEnabled = false;
                    FailedOrderReasonChangeButton.IsEnabled = false;
                    this.ToolbarItems.Remove(ToolbarItem_LocalSave);
                    this.ToolbarItems.Remove(ToolbarItem_SendToServer);
                }
            }
        }
        
        private void PartnerSelected()
        {
            PartnerLabel.Text = SelectedPartner == null ? "مشتری" :
                !string.IsNullOrEmpty(SelectedPartner.LegalName) ? (SelectedPartner.LegalName + (!string.IsNullOrEmpty(SelectedPartner.Name) ? (" (" + SelectedPartner.Name + ")") : "")) : (SelectedPartner.Name);
        }
        
        public async void SubmitFailedVisitToServer(object sender, EventArgs e)
        {
            WaitToggle(false);
            await Task.Delay(100);
            var SaveResult = await SaveFailedVisit();
            
            if (!SaveResult.Success)
            {
                WaitToggle(false);
                App.ShowError("خطا", SaveResult.Message, "خوب");
            }
            else
            {
                var submitResult = await Connectivity.SubmitFailedVisitsAsync(new FailedVisit[] { SaveResult.Data });
                
                if (!submitResult.Success)
                {
                    WaitToggle(false);
                    App.ShowError("خطا", "عدم سفارش به صورت محلی ثبت شد اما در ارسال اطلاعات به سرور خطایی رخ داده است: " + submitResult.Message, "خوب");
                }
                else
                {
                    WaitToggle(true);
                    App.ToastMessageHandler.ShowMessage("عدم سفارش با موفقیت به سرور ارسال شد.", Helpers.ToastMessageDuration.Long);
                    if (FailedVisitsForm == null && PartnerListForm == null)
                        await Task.Delay(1000);
                    if (Navigation.NavigationStack.Count() > 1)
                        try { Navigation.PopAsync(); } catch (Exception) { }
                }
                if (FailedVisitsForm != null)
                    FailedVisitsForm.FilterChanged(null, null);
                if (PartnerListForm != null)
                    PartnerListForm.FilterChanged(null, null);
            }
        }

        public async void SubmitFailedVisitToStorage(object sender, EventArgs e)
        {
            WaitToggle(false);
            await Task.Delay(100);
            var SaveResult = await SaveFailedVisit();
            
            if (!SaveResult.Success)
            {
                WaitToggle(false);
                    App.ShowError("خطا", SaveResult.Message, "خوب");
            }   
            else
            {
                WaitToggle(true);
                App.ToastMessageHandler.ShowMessage("عدم سفارش با موفقیت به صورت محلی ثبت شد.", Helpers.ToastMessageDuration.Long);
                if (FailedVisitsForm == null && PartnerListForm == null)
                    await Task.Delay(1000);
                if (Navigation.NavigationStack.Count() > 1)
                    try { Navigation.PopAsync(); } catch (Exception) { }
                if (FailedVisitsForm != null)
                    FailedVisitsForm.FilterChanged(null, null);
                if (PartnerListForm != null)
                    PartnerListForm.FilterChanged(null, null);
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
                if(!FormWorkFinished)
                {
                    this.ToolbarItems.Add(ToolbarItem_LocalSave);
                    this.ToolbarItems.Add(ToolbarItem_SendToServer);
                }
            }
        }

        private async Task<ResultSuccess<FailedVisit>> SaveFailedVisit()
        {
            try
            {
                FailedVisit FailedVisit = ShowingId.HasValue ? (await App.DB.GetFailedVisitAsync(ShowingId.Value)).Data.FailedVisit : null;

                if (SelectedPartner == null)
                    return new ResultSuccess<FailedVisit>(false, "مشتری مشخص نشده است.");
                
                if (FailedOrderReason.SelectedIndex == -1)
                    return new ResultSuccess<FailedVisit>(false, "دلیل عدم سفارش مشخص نشده است.");

                var FailedOrderReasonId = NotOrderReasons[FailedOrderReason.SelectedIndex].Id;

                var _Description = Description.Text != null ? Description.Text : "";

                if (FailedVisit == null)
                {
                    var LastValidLocation = App.LastLocation != null ? App.LastLocation.DateTime >= DateTime.Now.AddMinutes(-10) ? App.LastLocation : null : null;
                    FailedVisit = new FailedVisit()
                    {
                        Id = Guid.NewGuid(),
                        PartnerId = SelectedPartner.Id,
                        VisitTime = DateTime.Now,
                        ReasonId = FailedOrderReasonId,
                        Description = _Description,
                        GeoLocationLat = LastValidLocation != null ? (decimal)LastValidLocation.Latitude : new Nullable<decimal>(),
                        GeoLocationLong = LastValidLocation != null ? (decimal)LastValidLocation.Longitude : new Nullable<decimal>(),
                        GeoLocationAccuracy = LastValidLocation != null ? (decimal)LastValidLocation.Accuracy : new Nullable<decimal>(),
                        Sent = false
                    };
                }
                else
                {
                    FailedVisit.ReasonId = FailedOrderReasonId;
                    FailedVisit.Description = _Description;
                }

                var result = await App.DB.InsertOrUpdateRecordAsync<FailedVisit>(FailedVisit);
                if (!result.Success)
                    return new ResultSuccess<FailedVisit>(false, result.Message);

                ShowingId = FailedVisit.Id;
                return new ResultSuccess<FailedVisit>(true, "", FailedVisit);
            }
            catch (Exception err)
            {
                return new ResultSuccess<FailedVisit>(false, err.ProperMessage());
            }
        }
    }
}
