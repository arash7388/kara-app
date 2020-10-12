using Kara.Assets;
using Kara.CustomRenderer;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
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
    public partial class ReportTabbedForm_BarChart : ContentPage
    {
        public ReportTabbedForm_BarChart(string ChartTitle, Connectivity.ReportGeneralModel[] Data)
        {
            BarChartViewModel.ChartTitle = ChartTitle;
            BarChartViewModel.Data = Data;

            InitializeComponent();
            Icon = "ReportIcon_Bar.png";
            
            //BusyIndicatorContainder.BackgroundColor = Color.FromRgba(255, 255, 255, 70);
            //BusyIndicator.Color = Color.FromRgba(80, 100, 150, 255);
        }

        public void CreateChart()
        {
            //var model = new PlotModel
            //{
            //    Title = ChartTitle,
            //    LegendPlacement = LegendPlacement.Outside,
            //    LegendPosition = LegendPosition.BottomCenter,
            //    LegendOrientation = LegendOrientation.Horizontal,
            //    LegendBorderThickness = 0
            //};

            //var s1 = new BarSeries
            //{
            //    Title = "فروش ریالی",
            //    IsStacked = false,
            //    StrokeColor = OxyColors.Black,
            //    StrokeThickness = 1
            //};
            //foreach (var item in Data)
            //    s1.Items.Add(new BarItem { Value = Convert.ToDouble(item._Column5) });

            ////var s2 = new BarSeries
            ////{
            ////    Title = "Series 2",
            ////    IsStacked = false,
            ////    StrokeColor = OxyColors.Black,
            ////    StrokeThickness = 1
            ////};
            ////s2.Items.Add(new BarItem { Value = 12 });
            ////s2.Items.Add(new BarItem { Value = 14 });
            ////s2.Items.Add(new BarItem { Value = 120 });
            ////s2.Items.Add(new BarItem { Value = 26 });

            //var categoryAxis = new CategoryAxis { Position = AxisPosition.Bottom };
            //foreach (var item in Data)
            //    categoryAxis.Labels.Add(item._Column3 != null ? item.Column3 : item._Column2 != null ? item.Column2 : item.Column1);

            //var valueAxis = new LinearAxis
            //{
            //    Position = AxisPosition.Left,
            //    MinimumPadding = 0,
            //    MaximumPadding = 0.06,
            //    AbsoluteMinimum = 0
            //};
            //model.Series.Add(s1);
            ////model.Series.Add(s2);
            //model.Axes.Add(categoryAxis);
            //model.Axes.Add(valueAxis);

            //BarChart.Model = model;
        }
    }

    public class BarChartViewModel
    {
        public PlotModel Model { get; set; }

        public static string ChartTitle;
        public static Connectivity.ReportGeneralModel[] Data;

        public BarChartViewModel()
        {
            Model = new PlotModel();
            Model.Title = ChartTitle;

            if (Data != null)
            {
                CategoryAxis xaxis = new CategoryAxis();
                xaxis.Position = AxisPosition.Bottom;
                xaxis.MajorGridlineStyle = LineStyle.Solid;
                xaxis.MinorGridlineStyle = LineStyle.Dot;
                foreach (var item in Data)
                    xaxis.Labels.Add(item._Column3 != null ? item.Column3 : item._Column2 != null ? item.Column2 : item.Column1);

                LinearAxis yaxis = new LinearAxis();
                yaxis.Position = AxisPosition.Left;
                yaxis.MajorGridlineStyle = LineStyle.Dot;
                yaxis.Unit = "1,000,000 ریال".ReplaceLatinDigits();
                xaxis.MinorGridlineStyle = LineStyle.Dot;

                yaxis.IsZoomEnabled = false;
                yaxis.IsPanEnabled = false;
                xaxis.IsZoomEnabled = false;
                xaxis.IsPanEnabled = false;
                
                ColumnSeries s1 = new ColumnSeries();
                s1.IsStacked = true;
                foreach (var item in Data)
                    s1.Items.Add(new ColumnItem(Convert.ToDouble(item._Column5) / 1000000));
                
                Model.Axes.Add(xaxis);
                Model.Axes.Add(yaxis);
                Model.Series.Add(s1);
            }
        }
    }
}
