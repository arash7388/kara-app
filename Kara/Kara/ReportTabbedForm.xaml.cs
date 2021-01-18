using Kara.Assets;
using Kara.CustomRenderer;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Kara
{
    public partial class ReportTabbedForm : TabbedPage
    {
        string ReportType;
        DateTime BDate, EDate;
        public ReportTabbedForm_Report Report;
        public ReportTabbedForm_PieChart PieChart;
        public ReportTabbedForm_BarChart BarChart;

        public ReportTabbedForm(string ReportType, DateTime BDate, DateTime EDate)
        {
            InitializeComponent();

            this.ReportType = ReportType;
            this.BDate = BDate;
            this.EDate = EDate;

            CustomReportColumnModel[] Columns;
            switch (ReportType)
            {
                case "Daily":
                    Columns = new CustomReportColumnModel[] {
                        new CustomReportColumnModel() { Name = "Column1", Title = "روز", WidthStar = 1 },
                        new CustomReportColumnModel() { Name = "Column4", Title = "فروش تعدادی", WidthStar = 1 },
                        new CustomReportColumnModel() { Name = "Column5", Title = "فروش ریالی", WidthStar = 1 }
                    };
                    break;
                case "Weekly":
                    Columns = new CustomReportColumnModel[] {
                        new CustomReportColumnModel() { Name = "Column1", Title = "هفته", WidthStar = 1 },
                        new CustomReportColumnModel() { Name = "Column4", Title = "فروش تعدادی", WidthStar = 1 },
                        new CustomReportColumnModel() { Name = "Column5", Title = "فروش ریالی", WidthStar = 1 }
                    };
                    break;
                case "Monthly":
                    Columns = new CustomReportColumnModel[] {
                        new CustomReportColumnModel() { Name = "Column1", Title = "ماه", WidthStar = 1 },
                        new CustomReportColumnModel() { Name = "Column4", Title = "فروش تعدادی", WidthStar = 1 },
                        new CustomReportColumnModel() { Name = "Column5", Title = "فروش ریالی", WidthStar = 1 }
                    };
                    break;
                case "StuffGroups":
                    Columns = new CustomReportColumnModel[] {
                        new CustomReportColumnModel() { Name = "Column1", Title = "گروه کالا", WidthStar = 1 },
                        new CustomReportColumnModel() { Name = "Column4", Title = "فروش تعدادی", WidthStar = 1 },
                        new CustomReportColumnModel() { Name = "Column5", Title = "فروش ریالی", WidthStar = 1 }
                    };
                    break;
                case "StuffSubGroups":
                    Columns = new CustomReportColumnModel[] {
                        new CustomReportColumnModel() { Name = "Column1", Title = "گروه کالا", WidthStar = 1 },
                        new CustomReportColumnModel() { Name = "Column2", Title = "زیرگروه کالا", WidthStar = 1 },
                        new CustomReportColumnModel() { Name = "Column4", Title = "فروش تعدادی", WidthStar = 1 },
                        new CustomReportColumnModel() { Name = "Column5", Title = "فروش ریالی", WidthStar = 1 }
                    };
                    break;
                case "Stuffs":
                    Columns = new CustomReportColumnModel[] {
                        new CustomReportColumnModel() { Name = "Column1", Title = "گروه کالا", WidthStar = 1 },
                        new CustomReportColumnModel() { Name = "Column2", Title = "زیرگروه کالا", WidthStar = 1 },
                        new CustomReportColumnModel() { Name = "Column3", Title = "کالا", WidthStar = 1 },
                        new CustomReportColumnModel() { Name = "Column4", Title = "فروش تعدادی", WidthStar = 1 },
                        new CustomReportColumnModel() { Name = "Column5", Title = "فروش ریالی", WidthStar = 1 }
                    };
                    break;
                case "PartnerGroups":
                    Columns = new CustomReportColumnModel[] {
                        new CustomReportColumnModel() { Name = "Column1", Title = "گروه مشتری", WidthStar = 1 },
                        new CustomReportColumnModel() { Name = "Column4", Title = "فروش تعدادی", WidthStar = 1 },
                        new CustomReportColumnModel() { Name = "Column5", Title = "فروش ریالی", WidthStar = 1 }
                    };
                    break;
                default:
                    Columns = null;
                    break;
            }

            CustomReportCustomCell.Columns = Columns;

            Report = new ReportTabbedForm_Report() { StartColor = Color.FromHex("E6EBEF"), EndColor = Color.FromHex("A6CFED") };
            PieChart = new ReportTabbedForm_PieChart(); // { StartColor = Color.FromHex("E6EBEF"), EndColor = Color.FromHex("A6CFED") };
            BarChart = new ReportTabbedForm_BarChart(null, null); // { StartColor = Color.FromHex("E6EBEF"), EndColor = Color.FromHex("A6CFED") };
            
            Children.Add(PieChart);
            Children.Add(BarChart);
            Children.Add(Report);

            CurrentPage = Report;

            FetchReportData();
        }

        async Task FetchReportData()
        {
            App.UniversalLineInApp = 16431001;
            var result = await Connectivity.GetReportDataAsync(ReportType, BDate, EDate);
            App.UniversalLineInApp = 16431002;
            if (!result.Success)
            {
                App.UniversalLineInApp = 16431003;
                App.ShowError("خطا", "خطایی در بارگذاری اطلاعات گزارش از سرور رخ داده است. لطفا مجددا تلاش کنید.\n" + result.Message, "خوب");
                App.UniversalLineInApp = 16431004;
                return;
            }
            App.UniversalLineInApp = 16431005;

            App.SpecialLog = !result.Data.Any() ? "result.Data is empty!" : result.Data.Select(a => "_Column1: " + a._Column1 + ", _Column2: " + a._Column2 + ", _Column3: " + a._Column3 + ", _Column4: " + a._Column4 + ", _Column5: " + a._Column5).Aggregate((sum, x) => sum + "|" + x);
            var SumRow = new Connectivity.ReportGeneralModel()
            {
                _Column1 = "جمع:",
                _Column2 = "",
                _Column3 = "",
                _Column4 = result.Data.Sum(a => Convert.ToDecimal(a._Column4.Replace(",", ""))).ToString("###,###,###,###,###,###,##0.###").ToPersianDigits(),
                _Column5 = result.Data.Sum(a => Convert.ToDecimal(a._Column5.Replace(",", ""))).ToString("###,###,###,###,###,###,##0.###").ToPersianDigits()
            };
            App.UniversalLineInApp = 16431006;

            Children.Remove(PieChart);
            App.UniversalLineInApp = 16431007;
            Children.Remove(BarChart);
            App.UniversalLineInApp = 16431008;
            Children.Remove(Report);
            App.UniversalLineInApp = 16431009;

            Report.FillReportData(result.Data, SumRow);
            App.UniversalLineInApp = 16431010;

            var ChartTitle = "نمودار فروش " + (
                ReportType == "Daily" ? "روزانه" :
                ReportType == "Weekly" ? "هفتگی" :
                ReportType == "Monthly" ? "ماهانه" :
                ReportType == "StuffGroups" ? "گروه کالاها" :
                ReportType == "StuffSubGroups" ? "زیرگروه کالاها" :
                ReportType == "Stuffs" ? "کالاها" :
                ReportType == "PartnerGroups" ? "گروه مشتریان" : ""
            );
            App.UniversalLineInApp = 16431011;

            BarChart = new ReportTabbedForm_BarChart(ChartTitle, result.Data);
            App.UniversalLineInApp = 16431012;

            PieChart.CreateChart(ChartTitle, result.Data);
            App.UniversalLineInApp = 16431013;

            Children.Add(PieChart);
            App.UniversalLineInApp = 16431014;
            Children.Add(BarChart);
            App.UniversalLineInApp = 16431015;
            Children.Add(Report);
            App.UniversalLineInApp = 16431016;

            CurrentPage = Report;
            App.UniversalLineInApp = 16431017;
        }
    }
}
