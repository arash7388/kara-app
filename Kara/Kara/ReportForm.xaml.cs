using Kara.Assets;
using Kara.CustomRenderer;
using Kara.UserControls;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Kara
{
    public partial class ReportForm : GradientContentPage
    {
        class MenuModel
        {
            public string Tag { get; set; }
            public Button Button { get; set; }
            public Grid FilterItemGrid { get; set; }
            public static ReportForm ReportForm;
            public void Button_Clicked(object sender, EventArgs e)
            {
                var ReportTabbedForm = new ReportTabbedForm(Tag, ReportForm.BDatePicker.Value, ReportForm.EDatePicker.Value);
                ReportForm.Navigation.PushAsync(ReportTabbedForm, false);
            }
        }
        MenuModel[][] VerticalAlignmentOfMenus, HorizontalAlignmentOfMenus;
        PersianDatePicker BDatePicker, EDatePicker;

        public ReportForm()
        {
            InitializeComponent();

            MenuModel.ReportForm = this;

            var ThisMonthDateString = DateTime.Today.ToShortStringForDate().Substring(0, 7) + "/";
            DateTime BDate = (ThisMonthDateString + "01").PersianDateStringToDate();
            DateTime EDate = DateTime.Today;
            
            var BDateLabel = new Label() { Text = "از تاریخ: ", HorizontalOptions = LayoutOptions.EndAndExpand };
            BDatePicker = new PersianDatePicker(PopUpDialog) { Value = BDate, HorizontalOptions = LayoutOptions.EndAndExpand };
            var BDateGrid = new MenuModel()
            {
                FilterItemGrid = new Grid()
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    RowDefinitions = new RowDefinitionCollection() {
                        new RowDefinition() { Height = 40 }
                    },
                    ColumnDefinitions = new ColumnDefinitionCollection() {
                        new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) },
                        new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) }
                    }
                }
            };
            BDateGrid.FilterItemGrid.Children.Add(BDatePicker, 0, 0);
            BDateGrid.FilterItemGrid.Children.Add(BDateLabel, 1, 0);

            var EDateLabel = new Label() { Text = "تا تاریخ: ", HorizontalOptions = LayoutOptions.EndAndExpand };
            EDatePicker = new PersianDatePicker(PopUpDialog) { Value = EDate, HorizontalOptions = LayoutOptions.EndAndExpand };
            var EDateGrid = new MenuModel()
            {
                FilterItemGrid = new Grid()
                {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    RowDefinitions = new RowDefinitionCollection() {
                        new RowDefinition() { Height = 40 }
                    },
                    ColumnDefinitions = new ColumnDefinitionCollection() {
                        new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) },
                        new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) }
                    }
                }
            };
            EDateGrid.FilterItemGrid.Children.Add(EDatePicker, 0, 0);
            EDateGrid.FilterItemGrid.Children.Add(EDateLabel, 1, 0);

            var DailyReport = new MenuModel() { Button = new Button() { Text = "روزانه" }, Tag = "Daily" };
            var WeeklyReport = new MenuModel() { Button = new Button() { Text = "هفتگی" }, Tag = "Weekly" };
            var MonthlyReport = new MenuModel() { Button = new Button() { Text = "ماهانه" }, Tag = "Monthly" };
            var StuffGroupsReport = new MenuModel() { Button = new Button() { Text = "گروه کالاها(سطح 1)" }, Tag = "StuffGroups" };
            var StuffSubGroupsReport = new MenuModel() { Button = new Button() { Text = "گروه کالاها(سطح 2)" }, Tag = "StuffSubGroups" };
            var StuffsReport = new MenuModel() { Button = new Button() { Text = "کالاها" }, Tag = "Stuffs" };
            var PartnerGroupsReport = new MenuModel() { Button = new Button() { Text = "گروه مشتریان" }, Tag = "PartnerGroups" };

            VerticalAlignmentOfMenus = new MenuModel[][] {
                new MenuModel[] {
                    BDateGrid,
                    EDateGrid,
                    DailyReport,
                    WeeklyReport,
                    MonthlyReport,
                    StuffGroupsReport,
                    StuffSubGroupsReport,
                    StuffsReport,
                    PartnerGroupsReport
                }
            };
            HorizontalAlignmentOfMenus = new MenuModel[][] {
                new MenuModel[] {
                    BDateGrid,
                    DailyReport,
                    WeeklyReport,
                    MonthlyReport,
                    PartnerGroupsReport
                },
                new MenuModel[] {
                    EDateGrid,
                    StuffGroupsReport,
                    StuffSubGroupsReport,
                    StuffsReport,
                    null
                }
            };

            for (int i = 0; i < VerticalAlignmentOfMenus.Length; i++)
                for (int j = 0; j < VerticalAlignmentOfMenus[i].Length; j++)
                {
                    if (VerticalAlignmentOfMenus[i][j].Button != null)
                    {
                        VerticalAlignmentOfMenus[i][j].Button.BorderColor = Color.FromHex("#1E87D8");
                        VerticalAlignmentOfMenus[i][j].Button.TextColor = Color.White;
                        VerticalAlignmentOfMenus[i][j].Button.BorderWidth = 1;
                        VerticalAlignmentOfMenus[i][j].Button.BorderRadius = 10;
                        VerticalAlignmentOfMenus[i][j].Button.BackgroundColor = Color.FromHex("#2196F3");
                        VerticalAlignmentOfMenus[i][j].Button.HorizontalOptions = LayoutOptions.FillAndExpand;
                        VerticalAlignmentOfMenus[i][j].Button.VerticalOptions = LayoutOptions.FillAndExpand;
                        VerticalAlignmentOfMenus[i][j].Button.Clicked += VerticalAlignmentOfMenus[i][j].Button_Clicked;
                    }
                }
        }

        Guid LastSizeAllocationId = Guid.NewGuid();
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

                int RowCount, ColCount;
                MenuModel[][] MenuArray;
                if (height > width)
                    MenuArray = VerticalAlignmentOfMenus;
                else
                    MenuArray = HorizontalAlignmentOfMenus;

                RowCount = MenuArray[0].Length;
                ColCount = MenuArray.Length;
                
                ReportMenuGrid.RowDefinitions = new RowDefinitionCollection();
                ReportMenuGrid.ColumnDefinitions = new ColumnDefinitionCollection();
                ReportMenuGrid.RowDefinitions.Add(new RowDefinition() { Height = 10 });
                for (int i = 0; i < RowCount; i++)
                    ReportMenuGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                ReportMenuGrid.RowDefinitions.Add(new RowDefinition() { Height = 10 });

                for (int i = 0; i < ColCount; i++)
                {
                    ReportMenuGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = 10 });
                    ReportMenuGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                    ReportMenuGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = 10 });
                }
                
                ReportMenuGrid.Children.Clear();
                for (int i = 0; i < RowCount; i++)
                {
                    for (int j = 0; j < ColCount; j++)
                    {
                        var Menu = MenuArray[j][i];
                        if(Menu != null)
                            ReportMenuGrid.Children.Add(Menu.Button != null ? (View)Menu.Button : (View)Menu.FilterItemGrid, (ColCount - 1 - j) * 3 + 1, i + 1);
                    }
                }
            }
        }
    }
}
