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
    public class CycleDataCustomCell : ViewCell
    {
        public CycleDataCustomCell()
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
                new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) },
                new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) },
                new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) },
                new ColumnDefinition() { Width = new GridLength(3, GridUnitType.Star) },
                new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) }
            };

            Label RemainderLabel = null, CreditorLabel = null, DebtorLabel = null, DescriptionLabel = null, DateLabel = null;

            RemainderLabel = new Label() { LineBreakMode = LineBreakMode.TailTruncation, HorizontalOptions = LayoutOptions.Start, HorizontalTextAlignment = TextAlignment.Start, VerticalOptions = LayoutOptions.Center, FontSize = 16, TextColor = Color.FromHex(WithBinding ? "222" : "fff") };
            CreditorLabel = new Label() { LineBreakMode = LineBreakMode.TailTruncation, HorizontalOptions = LayoutOptions.End, HorizontalTextAlignment = TextAlignment.End, VerticalOptions = LayoutOptions.Center, FontSize = 16, TextColor = Color.FromHex(WithBinding ? "222" : "fff") };
            DebtorLabel = new Label() { LineBreakMode = LineBreakMode.TailTruncation, HorizontalOptions = LayoutOptions.End, HorizontalTextAlignment = TextAlignment.End, VerticalOptions = LayoutOptions.Center, FontSize = 16, TextColor = Color.FromHex(WithBinding ? "222" : "fff") };
            DescriptionLabel = new Label() { LineBreakMode = LineBreakMode.WordWrap, HorizontalOptions = LayoutOptions.End, HorizontalTextAlignment = TextAlignment.End, VerticalOptions = LayoutOptions.Center, FontSize = 16, TextColor = Color.FromHex(WithBinding ? "222" : "fff") };
            DateLabel = new Label() { LineBreakMode = LineBreakMode.TailTruncation, HorizontalOptions = LayoutOptions.End, HorizontalTextAlignment = TextAlignment.End, VerticalOptions = LayoutOptions.Center, FontSize = 16, TextColor = Color.FromHex(WithBinding ? "222" : "fff") };

            GridWrapper.Children.Add(RemainderLabel, 0, 0);
            GridWrapper.Children.Add(CreditorLabel, 1, 0);
            GridWrapper.Children.Add(DebtorLabel, 2, 0);
            GridWrapper.Children.Add(DescriptionLabel, 3, 0);
            GridWrapper.Children.Add(DateLabel, 4, 0);

            if (WithBinding)
            {
                RemainderLabel.SetBinding(Label.TextProperty, "Remainder");
                CreditorLabel.SetBinding(Label.TextProperty, "Creditor");
                DebtorLabel.SetBinding(Label.TextProperty, "Debtor");
                DescriptionLabel.SetBinding(Label.TextProperty, "Description");
                DateLabel.SetBinding(Label.TextProperty, "Date");
            }
            else
            {
                RemainderLabel.Text = "مانده";
                CreditorLabel.Text = "بستانکار";
                DebtorLabel.Text = "بدهکار";
                DescriptionLabel.Text = "شرح";
                DateLabel.Text = "تاریخ";
            }

            return GridWrapper;
        }
    }

    public partial class PartnerReportForm_CycleDataForm : GradientContentPage
    {
        ObservableCollection<Connectivity.CycleDataListModel> CycleDatasList;
        PartnerReportForm PartnerReportForm;
        Guid PartnerId;

        public PartnerReportForm_CycleDataForm(PartnerReportForm PartnerReportForm, Guid PartnerId)
        {
            InitializeComponent();
            Title = "گردش حساب";
            this.PartnerReportForm = PartnerReportForm;

            this.PartnerId = PartnerId;

            BusyIndicatorContainder.BackgroundColor = Color.FromRgba(255, 255, 255, 70);
            BusyIndicator.Color = Color.FromRgba(80, 100, 150, 255);

            CycleDatasItems.HasUnevenRows = true;
            CycleDatasItems.SeparatorColor = Color.FromHex("A5ABB7");
            CycleDatasItems.ItemTemplate = new DataTemplate(typeof(CycleDataCustomCell));
            CycleDatasItems.ItemSelected += (sender, e) => {
                ((ListView)sender).SelectedItem = null;
            };
            CycleDatasItemsHeader.Children.Add(new CycleDataCustomCell().GetView(false));
            
            FillCycleDatas(false);
        }
        
        private async Task FillCycleDatas(bool Refresh)
        {
            CycleDatasItems.IsRefreshing = true;
            await Task.Delay(100);

            var CycleDatasResult = await Connectivity.GetCycleDatasListAsync(PartnerId, Refresh);
            if (!CycleDatasResult.Success)
            {
                App.ShowError("خطا", "در نمایش گردش حساب خطایی رخ داد.\n" + CycleDatasResult.Message, "خوب");
                CycleDatasItems.IsRefreshing = false;
                return;
            }
            CycleDatasList = new ObservableCollection<Connectivity.CycleDataListModel>(CycleDatasResult.Data);
            CycleDatasItems.ItemsSource = null;
            CycleDatasItems.ItemsSource = CycleDatasList;

            var Remainder = CycleDatasResult.Data.Any() ? CycleDatasResult.Data.Last()._Remainder : 0;
            Title = ("گردش حساب (مانده:" + (Math.Abs(Remainder).ToString("###,###,###,###,###,###,##0.") + (Remainder > 0 ? " بدهکار" : Remainder < 0 ? " بستانکار" : "")) + ")").ToPersianDigits();
            
            CycleDatasItems.IsRefreshing = false;
        }
    }
}
