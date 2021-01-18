using Kara.Assets;
using Kara.CustomRenderer;
using Plugin.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Kara
{
    public class ReturnedChequeCustomCell : ViewCell
    {
        public ReturnedChequeCustomCell()
        {
            View = GetView(true);
        }

        public View GetView(bool WithBinding)
        {
            Grid GridWrapper = new Grid()
            {
                Padding = new Thickness(5, 0),
                RowSpacing = 1,
                ColumnSpacing = 1,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.FromHex(WithBinding ? "#DCE6FA" : "#0062C4")
            };
            
            GridWrapper.RowDefinitions = new RowDefinitionCollection()
            {
                new RowDefinition() { Height = GridLength.Auto }
            };

            GridWrapper.ColumnDefinitions = new ColumnDefinitionCollection()
            {
                new ColumnDefinition() { Width = new GridLength(3, GridUnitType.Star) },
                new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) },
                new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) },
                new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) },
                new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) }
            };

            Label DescriptionLabel = null, PriceLabel = null, MaturityDateLabel = null, SerialLabel = null, BackNumberLabel = null;

            DescriptionLabel = new Label() { LineBreakMode = LineBreakMode.WordWrap, HorizontalOptions = LayoutOptions.End, HorizontalTextAlignment = TextAlignment.End, VerticalOptions = LayoutOptions.Center, FontSize = 16, TextColor = Color.FromHex(WithBinding ? "222" : "fff") };
            PriceLabel = new Label() { LineBreakMode = LineBreakMode.TailTruncation, HorizontalOptions = LayoutOptions.End, HorizontalTextAlignment = TextAlignment.End, VerticalOptions = LayoutOptions.Center, FontSize = 16, TextColor = Color.FromHex(WithBinding ? "222" : "fff") };
            MaturityDateLabel = new Label() { LineBreakMode = LineBreakMode.TailTruncation, HorizontalOptions = LayoutOptions.End, HorizontalTextAlignment = TextAlignment.End, VerticalOptions = LayoutOptions.Center, FontSize = 16, TextColor = Color.FromHex(WithBinding ? "222" : "fff") };
            SerialLabel = new Label() { LineBreakMode = LineBreakMode.TailTruncation, HorizontalOptions = LayoutOptions.End, HorizontalTextAlignment = TextAlignment.End, VerticalOptions = LayoutOptions.Center, FontSize = 16, TextColor = Color.FromHex(WithBinding ? "222" : "fff") };
            BackNumberLabel = new Label() { LineBreakMode = LineBreakMode.TailTruncation, HorizontalOptions = LayoutOptions.End, HorizontalTextAlignment = TextAlignment.End, VerticalOptions = LayoutOptions.Center, FontSize = 16, TextColor = Color.FromHex(WithBinding ? "222" : "fff") };

            GridWrapper.Children.Add(DescriptionLabel, 0, 0);
            GridWrapper.Children.Add(PriceLabel, 2, 0);
            GridWrapper.Children.Add(MaturityDateLabel, 3, 0);
            GridWrapper.Children.Add(SerialLabel, 4, 0);
            GridWrapper.Children.Add(BackNumberLabel, 5, 0);
            
            if (WithBinding)
            {
                DescriptionLabel.SetBinding(Label.TextProperty, "Description");
                PriceLabel.SetBinding(Label.TextProperty, "Price");
                MaturityDateLabel.SetBinding(Label.TextProperty, "MaturityDate");
                SerialLabel.SetBinding(Label.TextProperty, "Serial");
                BackNumberLabel.SetBinding(Label.TextProperty, "BackNumber");
            }
            else
            {
                DescriptionLabel.Text = "شرح";
                PriceLabel.Text = "مبلغ";
                MaturityDateLabel.Text = "سررسید";
                SerialLabel.Text = "سریال";
                BackNumberLabel.Text = "پشت نمره";
            }

            return GridWrapper;
        }
    }

    public partial class PartnerReportForm_ReturnedChequesForm : GradientContentPage
    {
        ObservableCollection<Connectivity.ReturnedChequeListModel> ReturnedChequesList;
        PartnerReportForm PartnerReportForm;
        Guid PartnerId;

        public PartnerReportForm_ReturnedChequesForm(PartnerReportForm PartnerReportForm, Guid PartnerId)
        {
            InitializeComponent();
            Title = "چک های برگشتی";
            this.PartnerReportForm = PartnerReportForm;

            this.PartnerId = PartnerId;

            BusyIndicatorContainder.BackgroundColor = Color.FromRgba(255, 255, 255, 70);
            BusyIndicator.Color = Color.FromRgba(80, 100, 150, 255);

            ReturnedChequesItems.HasUnevenRows = true;
            ReturnedChequesItems.SeparatorColor = Color.FromHex("A5ABB7");
            ReturnedChequesItems.ItemTemplate = new DataTemplate(typeof(ReturnedChequeCustomCell));
            ReturnedChequesItems.ItemSelected += (sender, e) => {
                ((ListView)sender).SelectedItem = null;
            };
            ReturnedChequesItemsHeader.Children.Add(new ReturnedChequeCustomCell().GetView(false));

            FillReturnedCheques(false);
        }
        
        private async Task FillReturnedCheques(bool Refresh)
        {
            ReturnedChequesItems.IsRefreshing = true;
            await Task.Delay(100);

            var ReturnedChequesResult = await Connectivity.GetReturnedChequesListAsync(PartnerId, Refresh);
            if (!ReturnedChequesResult.Success)
            {
                App.ShowError("خطا", "در نمایش لیست چک های برگشتی خطایی رخ داد.\n" + ReturnedChequesResult.Message, "خوب");
                ReturnedChequesItems.IsRefreshing = false;
                return;
            }
            ReturnedChequesList = new ObservableCollection<Connectivity.ReturnedChequeListModel>(ReturnedChequesResult.Data);
            ReturnedChequesItems.ItemsSource = null;
            ReturnedChequesItems.ItemsSource = ReturnedChequesList;

            Title = ("چک های برگشتی (" + ReturnedChequesList.Count + " فقره)").ToPersianDigits();
            
            ReturnedChequesItems.IsRefreshing = false;
        }
    }
}
