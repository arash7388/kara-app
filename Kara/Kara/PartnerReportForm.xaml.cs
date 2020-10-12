using Kara.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Kara
{
    public partial class PartnerReportForm : TabbedPage
    {
        public PartnerReportForm_CycleDataForm CycleDataForm;
        public PartnerReportForm_UncashedChequesForm UncashedChequesForm;
        public PartnerReportForm_ReturnedChequesForm ReturnedChequesForm;
        private Guid? _SelectedPartnerId;
        bool JustCreated = true;

        public Guid? SelectedPartnerId
        {
            get { return _SelectedPartnerId; }
            set
            {
                if (_SelectedPartnerId != value)
                {
                    _SelectedPartnerId = value;
                    CycleDataForm = new PartnerReportForm_CycleDataForm(this, SelectedPartnerId.Value) { StartColor = Color.FromHex("E6EBEF"), EndColor = Color.FromHex("A6CFED") };
                    UncashedChequesForm = new PartnerReportForm_UncashedChequesForm(this, SelectedPartnerId.Value) { StartColor = Color.FromHex("E6EBEF"), EndColor = Color.FromHex("A6CFED") };
                    ReturnedChequesForm = new PartnerReportForm_ReturnedChequesForm(this, SelectedPartnerId.Value) { StartColor = Color.FromHex("E6EBEF"), EndColor = Color.FromHex("A6CFED") };

                    var _Children = new List<Page>();
                    foreach (var item in Children)
                        _Children.Add(item);
                    foreach (var item in _Children)
                        Children.Remove(item);

                    Children.Add(ReturnedChequesForm);
                    Children.Add(UncashedChequesForm);
                    Children.Add(CycleDataForm);

                    CurrentPage = CycleDataForm;
                }
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if(!JustCreated && !SelectedPartnerId.HasValue)
                try { Navigation.PopAsync(); } catch (Exception) { }
        }

        public PartnerReportForm(Guid? PartnerId)
        {
            InitializeComponent();

            this.SelectedPartnerId = PartnerId;

            Task.Factory.StartNew(async () =>
            {
                await Task.Delay(300);
                JustCreated = false;
            });
        }
    }
}
