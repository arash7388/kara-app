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
    public class CustomReportColumnModel
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public double WidthStar { get; set; }
    }

    public class CustomReportCustomCell : ViewCell
    {
        public bool IsFooter { get; set; }
        public static CustomReportColumnModel[] Columns;
        public CustomReportCustomCell()
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

            GridWrapper.ColumnDefinitions = new ColumnDefinitionCollection();

            Label[] Labels = new Label[Columns.Count()];

            var i = 0;
            foreach (var Column in Columns)
            {
                GridWrapper.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(Column.WidthStar, GridUnitType.Star) });
                Labels[i] = new Label() { LineBreakMode = LineBreakMode.TailTruncation, HorizontalOptions = LayoutOptions.End, HorizontalTextAlignment = TextAlignment.End, VerticalOptions = LayoutOptions.Center, FontSize = 16, TextColor = Color.FromHex(WithBinding ? "222" : "fff") };
                GridWrapper.Children.Add(Labels[i], Columns.Length - 1 - i, 0);
                if (WithBinding)
                    Labels[i].SetBinding(Label.TextProperty, Column.Name);
                else if (!IsFooter)
                    Labels[i].Text = Column.Title;
                i++;
            }

            return GridWrapper;
        }
    }

    public partial class ReportTabbedForm_Report : GradientContentPage
    {
        public ReportTabbedForm_Report()
        {
            InitializeComponent();
            Icon = "ReportIcon_Table.png";

            BusyIndicatorContainder.BackgroundColor = Color.FromRgba(255, 255, 255, 70);
            BusyIndicator.Color = Color.FromRgba(80, 100, 150, 255);
            
            ReportList.HasUnevenRows = true;
            ReportList.SeparatorColor = Color.FromHex("A5ABB7");
            ReportList.ItemTemplate = new DataTemplate(typeof(CustomReportCustomCell));
            ReportList.ItemSelected += (sender, e) => {
                ((ListView)sender).SelectedItem = null;
            };
            ReportListHeader.Children.Add(new CustomReportCustomCell().GetView(false));
            ReportListFooter.Children.Add(new CustomReportCustomCell() { IsFooter = true }.GetView(false));
        }
        
        public void FillReportData(Connectivity.ReportGeneralModel[] _ReportData, Connectivity.ReportGeneralModel SumRow)
        {
            var ReportData = new ObservableCollection<Connectivity.ReportGeneralModel>(_ReportData);
            ReportList.ItemsSource = null;
            ReportList.ItemsSource = ReportData;

            var ColumnCount = ((Grid)ReportListFooter.Children.First()).Children.Count;
            ((Label)((Grid)ReportListFooter.Children.First()).Children[0]).Text = "جمع:";
            ((Label)((Grid)ReportListFooter.Children.First()).Children[ColumnCount - 2]).Text = SumRow.Column4;
            ((Label)((Grid)ReportListFooter.Children.First()).Children[ColumnCount - 1]).Text = SumRow.Column5;
        }
    }
}
