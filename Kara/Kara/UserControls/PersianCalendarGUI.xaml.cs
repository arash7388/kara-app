using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Kara.Assets;

namespace Kara.UserControls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PersianCalendarGUI : ContentView
    {
        private int _Year, _Month;
        private DateTime SelectedDate;
        private Label SelectedDateLabel;
        private Label PersianMonthNameLabel, JulianMonthNameLabel;
        private Grid DaysGrid;
        private Label[][] DayLabels, DayJulianLabel;
        private KeyValuePair<DateTime, string>[][] DaysArray;
        private Button OK, Cancel;
        private PopUpDialogView Popup;
        private DateTime GetSelectedDate()
        {
            return SelectedDate;
        }

        public PersianCalendarGUI(DateTime SelectedDate, PopUpDialogView Popup)
        {
            InitializeComponent();

            this.SelectedDate = SelectedDate;
            this.Popup = Popup;
        }

        public async Task Initialize()
        {
            var SelectedDateString = App.PersianDateConverter.PersianDate(SelectedDate);
            _Year = Convert.ToInt32(SelectedDateString.Substring(0, 4));
            _Month = Convert.ToInt32(SelectedDateString.Substring(5, 2));

            var MonthSectionGrid = new Grid()
            {
                RowDefinitions = new RowDefinitionCollection()
                {
                    new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) },
                },
                ColumnDefinitions = new ColumnDefinitionCollection()
                {
                    new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition() { Width = new GridLength(5, GridUnitType.Star) },
                    new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) }
                },
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.FromHex("2196F3")
            };

            PersianMonthNameLabel = new Label() { HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, FontSize = 14 * 1.2, TextColor = Color.White, FontAttributes = FontAttributes.Bold };
            MonthSectionGrid.Children.Add(PersianMonthNameLabel, 1, 0);

            JulianMonthNameLabel = new Label() { HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, FontSize = 14 * 0.9, TextColor = Color.White };
            MonthSectionGrid.Children.Add(JulianMonthNameLabel, 1, 1);

            var NextButton = new Image() { HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand, Source = "Next.png" };
            MonthSectionGrid.Children.Add(NextButton, 2, 0);
            Grid.SetRowSpan(NextButton, 2);
            var NextButtonTapGestureRecognizer = new TapGestureRecognizer();
            NextButtonTapGestureRecognizer.Tapped += (sender, e) =>
            {
                PreviouseMonth();
            };
            NextButton.GestureRecognizers.Add(NextButtonTapGestureRecognizer);

            var PreviouseButton = new Image() { HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand, Source = "Previouse.png" };
            MonthSectionGrid.Children.Add(PreviouseButton, 0, 0);
            Grid.SetRowSpan(PreviouseButton, 2);
            var PreviouseButtonTapGestureRecognizer = new TapGestureRecognizer();
            PreviouseButtonTapGestureRecognizer.Tapped += (sender, e) =>
            {
                NextMonth();
            };
            PreviouseButton.GestureRecognizers.Add(PreviouseButtonTapGestureRecognizer);

            LayoutGrid.Children.Add(MonthSectionGrid, 0, 0);

            DaysGrid = new Grid()
            {
                RowDefinitions = new RowDefinitionCollection()
                {
                    new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) },
                    new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) }
                },
                ColumnDefinitions = new ColumnDefinitionCollection()
                {
                    new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) }
                },
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.FromHex("ccc"),
                RowSpacing = 1,
                ColumnSpacing = 1
            };

            DaysGrid.Children.Add(new Label() { HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, LineBreakMode = LineBreakMode.TailTruncation, Text = "شنبه", FontSize = 14 * 0.9, BackgroundColor = Color.White }, 6, 0);
            DaysGrid.Children.Add(new Label() { HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, LineBreakMode = LineBreakMode.TailTruncation, Text = "یکشنبه", FontSize = 14 * 0.9, BackgroundColor = Color.White }, 5, 0);
            DaysGrid.Children.Add(new Label() { HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, LineBreakMode = LineBreakMode.TailTruncation, Text = "دوشنبه", FontSize = 14 * 0.9, BackgroundColor = Color.White }, 4, 0);
            DaysGrid.Children.Add(new Label() { HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, LineBreakMode = LineBreakMode.TailTruncation, Text = "سه شنبه", FontSize = 14 * 0.9, BackgroundColor = Color.White }, 3, 0);
            DaysGrid.Children.Add(new Label() { HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, LineBreakMode = LineBreakMode.TailTruncation, Text = "چهارشنبه", FontSize = 14 * 0.9, BackgroundColor = Color.White }, 2, 0);
            DaysGrid.Children.Add(new Label() { HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, LineBreakMode = LineBreakMode.TailTruncation, Text = "پنجشنبه", FontSize = 14 * 0.9, BackgroundColor = Color.White }, 1, 0);
            DaysGrid.Children.Add(new Label() { HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, LineBreakMode = LineBreakMode.TailTruncation, Text = "جمعه", FontSize = 14 * 0.9, BackgroundColor = Color.White }, 0, 0);

            DayLabels = new Label[5][];
            DayJulianLabel = new Label[5][];
            for (int i = 0; i < 5; i++)
            {
                DayLabels[i] = new Label[7];
                DayJulianLabel[i] = new Label[7];
                for (int j = 0; j < 7; j++)
                {
                    DayLabels[i][j] = new Label() { VerticalOptions = LayoutOptions.FillAndExpand, HorizontalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, BackgroundColor = Color.FromHex("fff"), FontSize = 14, FontAttributes = FontAttributes.Bold };
                    DayJulianLabel[i][j] = new Label() { VerticalOptions = LayoutOptions.EndAndExpand, HorizontalOptions = LayoutOptions.StartAndExpand, HorizontalTextAlignment = TextAlignment.Start, VerticalTextAlignment = TextAlignment.End, FontSize = 14 * 0.7, FontAttributes = FontAttributes.Bold };
                    DaysGrid.Children.Add(DayLabels[i][j], 6 - j, i + 1);
                    DaysGrid.Children.Add(DayJulianLabel[i][j], 6 - j, i + 1);

                    var DayLabelTapGestureRecognizer = new TapGestureRecognizer();
                    var arg = i.ToString() + "|" + j.ToString();
                    DayLabelTapGestureRecognizer.Tapped += (sender, e) =>
                    {
                        ChangeSelectedDate(arg);
                    };
                    DayLabels[i][j].GestureRecognizers.Add(DayLabelTapGestureRecognizer);
                }
            }

            LayoutGrid.Children.Add(DaysGrid, 0, 1);

            OK = new Button() { Text = "تأیید", TextColor = Color.White, BorderColor = Color.FromHex("#1E87D8"), BorderWidth = 1, BorderRadius = 10, BackgroundColor = Color.FromHex("#2196F3"), HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand };
            Cancel = new Button() { Text = "انصراف", TextColor = Color.White, BorderColor = Color.FromHex("#1E87D8"), BorderWidth = 1, BorderRadius = 10, BackgroundColor = Color.FromHex("#2196F3"), HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand };

            OK.Clicked += (sender, e) => {
                Popup.HideDialog(GetSelectedDate());
            };
            Cancel.Clicked += (sender, e) => {
                Popup.HideDialog(null);
            };

            var ButtonsGrid = new Grid()
            {
                RowDefinitions = new RowDefinitionCollection()
                {
                    new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) }
                },
                ColumnDefinitions = new ColumnDefinitionCollection()
                {
                    new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) },
                    new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) }
                },
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.FromHex("fff"),
                ColumnSpacing = 10
            };

            ButtonsGrid.Children.Add(OK, 1, 0);
            ButtonsGrid.Children.Add(Cancel, 0, 0);

            LayoutGrid.Children.Add(ButtonsGrid, 0, 2);

            BindMonthIntoGrid();
        }

        public void ChangeSelectedDate(string arg)
        {
            int i = Convert.ToInt32(arg.Split('|')[0]);
            int j = Convert.ToInt32(arg.Split('|')[1]);

            var TappedCellIsValid = !DaysArray[i][j].Equals(default(KeyValuePair<DateTime, string>));
            if(TappedCellIsValid)
            {
                if (SelectedDateLabel != null && SelectedDateLabel.Text != "")
                    SelectedDateLabel.BackgroundColor = Color.FromHex("fff");

                SelectedDate = DaysArray[i][j].Key;
                SelectedDateLabel = DayLabels[i][j];
                SelectedDateLabel.BackgroundColor = DaysArray[i][j].Key == SelectedDate ? Color.SkyBlue : Color.FromHex("fff");
            }
        }

        public void NextMonth()
        {
            _Month++;
            if(_Month == 13)
            {
                _Month = 1;
                _Year++;
            }
            BindMonthIntoGrid();
        }

        public void PreviouseMonth()
        {
            _Month--;
            if (_Month == 0)
            {
                _Month = 12;
                _Year--;
            }
            BindMonthIntoGrid();
        }

        private string[] PersianMonthNames = new string[] { "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور", "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند" };
        private string[] JulianMonthNames = new string[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
        private Dictionary<DayOfWeek, int> PersianDayOfWeeks = new List<KeyValuePair<DayOfWeek, int>>()
        {
            new KeyValuePair<DayOfWeek, int>(DayOfWeek.Saturday, 1),
            new KeyValuePair<DayOfWeek, int>(DayOfWeek.Sunday, 2),
            new KeyValuePair<DayOfWeek, int>(DayOfWeek.Monday, 3),
            new KeyValuePair<DayOfWeek, int>(DayOfWeek.Tuesday, 4),
            new KeyValuePair<DayOfWeek, int>(DayOfWeek.Wednesday, 5),
            new KeyValuePair<DayOfWeek, int>(DayOfWeek.Thursday, 6),
            new KeyValuePair<DayOfWeek, int>(DayOfWeek.Friday, 7)
        }.ToDictionary(a => a.Key, a => a.Value);

        private void BindMonthIntoGrid()
        {
            var PersianMonthName = PersianMonthNames[_Month - 1] + " " + _Year;
            DateTime BDate = (_Year.ToString() + "/" + _Month.ToString().PadLeft(2, '0') + "/01").PersianDateStringToDate();
            DateTime EDate = (_Year.ToString() + "/" + _Month.ToString().PadLeft(2, '0') + "/29").PersianDateStringToDate();
            try
            {
                EDate = (_Year.ToString() + "/" + _Month.ToString().PadLeft(2, '0') + "/31").PersianDateStringToDate();
            }
            catch (Exception)
            {
                try
                {
                    EDate = (_Year.ToString() + "/" + _Month.ToString().PadLeft(2, '0') + "/30").PersianDateStringToDate();
                }
                catch (Exception)
                {
                }
            }
            var JulianMonthName = JulianMonthNames[BDate.Month - 1] + (BDate.Year != EDate.Year ? (BDate.Year.ToString() + " ") : "")
                + " - " + JulianMonthNames[EDate.Month - 1] + " " + EDate.Year.ToString();

            PersianMonthNameLabel.Text = PersianMonthName.ReplaceLatinDigits();
            JulianMonthNameLabel.Text = JulianMonthName;

            var MonthDays = new int[31].Select((a, index) => BDate.AddDays(index)).Select(a => new
            {
                JulianDate = a,
                PersianDate = App.PersianDateConverter.PersianDate(a),
                PersianDayOfWeek = PersianDayOfWeeks[a.DayOfWeek]
            }).Where(a => a.PersianDate.StartsWith(_Year.ToString() + "/" + _Month.ToString().PadLeft(2, '0')))
            .OrderBy(a => a.JulianDate).ToArray();

            DaysArray = new KeyValuePair<DateTime, string>[5][];
            var WeekIndex = MonthDays.First().PersianDayOfWeek == 1 ? -1 : 0;
            foreach (var MonthDay in MonthDays)
            {
                if (MonthDay.PersianDayOfWeek == 1)
                {
                    WeekIndex++;
                    if (WeekIndex == 5)
                        WeekIndex = 0;
                }

                if (DaysArray[WeekIndex] == null)
                    DaysArray[WeekIndex] = new KeyValuePair<DateTime, string>[7];

                DaysArray[WeekIndex][MonthDay.PersianDayOfWeek - 1] = new KeyValuePair<DateTime, string>(MonthDay.JulianDate, MonthDay.PersianDate);
            }
            
            for (int i = 0; i < DaysArray.Length; i++)
            {
                for (int j = 0; j < DaysArray[i].Length; j++)
                {
                    if (!DaysArray[i][j].Equals(default(KeyValuePair<DateTime, string>)))
                    {
                        DayLabels[i][j].Text = Convert.ToInt32(DaysArray[i][j].Value.Substring(8)).ToString().ReplaceLatinDigits();
                        DayLabels[i][j].BackgroundColor = DaysArray[i][j].Key == SelectedDate ? Color.SkyBlue : Color.FromHex("fff");
                        if(DaysArray[i][j].Key == SelectedDate)
                            SelectedDateLabel = DayLabels[i][j];
                        DayJulianLabel[i][j].Text = DaysArray[i][j].Key.Day.ToString();
                    }
                    else
                    {
                        DayLabels[i][j].Text = "";
                        DayLabels[i][j].BackgroundColor = Color.FromHex("ddd");
                        DayJulianLabel[i][j].Text = "";
                    }
                }
            }
        }
    }
}