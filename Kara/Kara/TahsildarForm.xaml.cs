using Kara.Assets;
using Kara.CustomRenderer;
using Kara.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Kara.Assets.Connectivity;

namespace Kara
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TahsildarForm : GradientContentPage
    {
        private Partner _BeforeSelectedPartner;
        private Partner _SelectedPartner;
        public Partner SelectedPartner
        {
            get { return _SelectedPartner; }
            set
            {
                _BeforeSelectedPartner = _SelectedPartner;
                _SelectedPartner = value;
                PartnerSelected();
            }
        }
        public ObservableCollection<UnSettledOrderModel> FactorsObservableCollection { get; private set; }

        LeftEntryCompanionLabel PartnerChangeButton;
        RightRoundedLabel PartnerLabel;
        bool TapEventHandlingInProgress = false;

        public TahsildarForm()
        {
            InitializeComponent();

            PartnerChangeButton = new LeftEntryCompanionLabel() { VerticalOptions = LayoutOptions.FillAndExpand, VerticalTextAlignment = TextAlignment.Center, HorizontalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.Center, FontSize = 18, Text = "🔍" };
            //App.UniversalLineInApp = 875234009;
            PartnerLabel = new RightRoundedLabel() { VerticalOptions = LayoutOptions.FillAndExpand, VerticalTextAlignment = TextAlignment.Center, HorizontalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.End, Text = "مشتری", Padding = new Thickness(0, 0, 50, 0) };
            //App.UniversalLineInApp = 875234010;
            PartnerLabel.FontSize *= 1.5;
            //App.UniversalLineInApp = 875234011;

            if (SelectedPartner == null)
            {
                PartnerChangeButton.IsEnabled = true;
                App.UniversalLineInApp = 875234038;
                //App.UniversalLineInApp = 875234039;
            }
            else
            {
                PartnerChangeButton.IsEnabled = false;
                App.UniversalLineInApp = 875234040;
                //App.UniversalLineInApp = 875234041;
            }

            var PartnerChangeButtonTapGestureRecognizer = new TapGestureRecognizer();
            //App.UniversalLineInApp = 875234042;
            PartnerChangeButtonTapGestureRecognizer.Tapped += (sender, e) => {
                if (!TapEventHandlingInProgress)
                {
                    TapEventHandlingInProgress = true;

                    try
                    {
                        //App.UniversalLineInApp = 875234043;
                        if (PartnerChangeButton.IsEnabled)
                        {
                            App.UniversalLineInApp = 875234044;
                            //FocusedQuantityTextBoxId = null;
                            //App.UniversalLineInApp = 875234045;
                            var PartnerListForm1 = new PartnerListForm();
                            //App.UniversalLineInApp = 875234046;
                            PartnerListForm1.TahsildarForm = this;  
                            //App.UniversalLineInApp = 875234047;
                            Navigation.PushAsync(PartnerListForm1);
                            //App.UniversalLineInApp = 875234048;
                        }
                        //App.UniversalLineInApp = 875234049;
                    }
                    catch (Exception)
                    { }

                    TapEventHandlingInProgress = false;
                }
            };

            App.UniversalLineInApp = 875234050;
            PartnerChangeButtonTapGestureRecognizer.SetBinding(TapGestureRecognizer.CommandParameterProperty, "Id");
            //App.UniversalLineInApp = 875234051;
            PartnerChangeButton.GestureRecognizers.Add(PartnerChangeButtonTapGestureRecognizer);
            //App.UniversalLineInApp = 875234052;
            PartnerChangeButton.WidthRequest = 150;
            //App.UniversalLineInApp = 875234053;
        }

        Guid LastSizeAllocationId;
        protected override async void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            Guid ThisSizeAllocationId = Guid.NewGuid();
            LastSizeAllocationId = ThisSizeAllocationId;
            await Task.Delay(100);
            if (LastSizeAllocationId == ThisSizeAllocationId)
                sizeChanged(width, height);
        }


        double LastWidth, LastHeight;
        public void sizeChanged(double width, double height)
        {
            if (LastWidth != width || LastHeight != height)
            {
                LastWidth = width;
                LastHeight = height;

                RefreshPartnerSection(LastWidth > LastHeight);
            }
        }


        private void RefreshPartnerSection(bool Horizental)
        {
            PartnerSection.RowDefinitions = new RowDefinitionCollection();
            PartnerSection.ColumnDefinitions = new ColumnDefinitionCollection();
            PartnerSection.Children.Clear();

            var ShowAccountingCycleOfPartner =
                App.Accesses.AccessToViewPartnerRemainder &&
                (
                    App.ShowAccountingCycleOfPartner_Remainder.Value ||
                    App.ShowAccountingCycleOfPartner_UncashedCheques.Value ||
                    App.ShowAccountingCycleOfPartner_ReturnedCheques.Value
                );

            if (Horizental)
            {
                PartnerSection.RowDefinitions.Add(new RowDefinition() { Height = 50 });
                //if (ShowAccountingCycleOfPartner)
                //    PartnerSection.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

                PartnerSection.ColumnDefinitions.Add(new ColumnDefinition() { Width = 50 });
                PartnerSection.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                var GallaryStuffGroupOffset = 0;
                
                //if (App.StuffListGroupingMethod.Value != 0 && InsertOrderForm_ShowGallaryMode.Value)
                //{
                //    PartnerSection.ColumnDefinitions.Add(new ColumnDefinition() { Width = 5 });
                //    PartnerSection.ColumnDefinitions.Add(new ColumnDefinition() { Width = 50 });
                //    PartnerSection.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                //    GallaryStuffGroupOffset = 3;
                //}

                PartnerSection.Children.Add(PartnerChangeButton, 0 + GallaryStuffGroupOffset, 0);
                PartnerSection.Children.Add(PartnerLabel, 1 + GallaryStuffGroupOffset, 0);

                //if (ShowAccountingCycleOfPartner)
                //{
                //    PartnerSection.Children.Add(PartnerRemainderDetailButton, 0 + GallaryStuffGroupOffset, 1);
                //    PartnerSection.Children.Add(PartnerRemainderLabel, 1 + GallaryStuffGroupOffset, 1);
                //}

                //if (App.StuffListGroupingMethod.Value != 0 && InsertOrderForm_ShowGallaryMode.Value)
                //{
                //    PartnerSection.Children.Add(GallaryStuffGroupPicker, 0, 0);
                //    PartnerSection.Children.Add(GallaryStuffGroupChangeButton, 0, 0);
                //    PartnerSection.Children.Add(GallaryStuffGroupLabel, 1, 0);
                //}
            }
            else
            {
                PartnerSection.RowDefinitions.Add(new RowDefinition() { Height = 50 });
                
                PartnerSection.ColumnDefinitions.Add(new ColumnDefinition() { Width = 50 });
                PartnerSection.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

                PartnerSection.Children.Add(PartnerChangeButton, 0, 0);
                PartnerSection.Children.Add(PartnerLabel, 1, 0);
            }
        }

        private void PartnerSelected()
        {
            PartnerLabel.Text = SelectedPartner == null ? "مشتری" :
                !string.IsNullOrEmpty(SelectedPartner.LegalName) ? (SelectedPartner.LegalName + (!string.IsNullOrEmpty(SelectedPartner.Name) ? (" (" + SelectedPartner.Name + ")") : "")) : (SelectedPartner.Name);

            var BeforeSelectedPartnerId = _BeforeSelectedPartner == null ? Guid.NewGuid() : _BeforeSelectedPartner.Id;
            var NewSelectedPartnerId = SelectedPartner == null ? Guid.NewGuid() : SelectedPartner.Id;
            if (SelectedPartner != null && BeforeSelectedPartnerId != NewSelectedPartnerId)
            {
                FillFactors(SelectedPartner.Code);
                //FetchPartnerCycleInformationFromServer();
            }
        }

        private async void FillFactors(string partnerCode)
        {
            partnerCode = "21200017"; //tempppp

            var unsetteledFactors = await Connectivity.GetUnSettledOrdersFromServerAsync(App.Username.Value, App.Password.Value, App.CurrentVersionNumber, partnerCode, "", "");

            FactorsObservableCollection = new ObservableCollection<UnSettledOrderModel>(unsetteledFactors.Data);

            FactorsView.ItemsSource = FactorsObservableCollection;
        }
    }
}