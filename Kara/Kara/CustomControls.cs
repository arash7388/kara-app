using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System;
using Kara.Assets;
using System.Linq.Expressions;
using System.Windows.Input;
using System.Reflection;
using Xamarin.Forms.Maps;
using System.IO;

namespace Kara.CustomRenderer
{
    public class GradientContentPage : ContentPage
    {
        public static BindableProperty StartColorProperty = BindableProperty.Create<GradientContentPage, Color>(p => p.StartColor, Color.White);
        public static BindableProperty EndColorProperty = BindableProperty.Create<GradientContentPage, Color>(p => p.EndColor, Color.Gray);

        public Color StartColor
        {
            get { return (Color)GetValue(StartColorProperty); }
            set { SetValue(StartColorProperty, value); }
        }

        public Color EndColor
        {
            get { return (Color)GetValue(EndColorProperty); }
            set { SetValue(EndColorProperty, value); }
        }
    }

    public class PrintPreviewPage : ContentPage
    {
        private ScrollView scrollView;
        public int PrintPreviewWidth;
        public int PrintPreviewHeight;
        public object ScanBitmap;
        public Exception ScanException;
        public List<KeyValuePair<int, object>> MergingBitmaps;
        public object MergedBitmap;
        public Exception MergeException;

        ToolbarItem ToolbarItem_Print;
        public PrintPreviewPage()
        {
            ToolbarItem_Print = new ToolbarItem();
            ToolbarItem_Print.Text = "چاپ";
            ToolbarItem_Print.Icon = "PrintBT" + (App.HasPrintingOption.Value ? "" : "Disabled") + ".png";
            ToolbarItem_Print.Activated += ToolbarItem_Print_Activated;
            ToolbarItem_Print.Order = ToolbarItemOrder.Primary;
            ToolbarItem_Print.Priority = 0;
        }

        private async void ToolbarItem_Print_Activated(object sender, EventArgs e)
        {
            if (App.HasPrintingOption.Value)
            {
                var result = await App.BluetoothPrinter.PrintBitmap(MergedBitmap, App.SelectedPrinter.MACID);
                if (!result.Success)
                    await DisplayAlert("خطا", result.Message, "خوب");
                else
                    App.ToastMessageHandler.ShowMessage("ارسال موفق به چاپگر", Helpers.ToastMessageDuration.Short);

                try { await Navigation.PopAsync(); } catch (Exception) { }
            }
            else
                await DisplayAlert("خطا", "امکان چاپ در نسخه شما فراهم نیست.", "خوب");
        }

        public void SetContent(View PrintPreviewContent, int width)
        {
            PrintPreviewWidth = width;
            var Grid = new Grid()
            {
                RowSpacing = 0,
                ColumnSpacing = 0,
                Padding = 0,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                RowDefinitions = new RowDefinitionCollection() { new RowDefinition() { Height = GridLength.Auto } },
                ColumnDefinitions = new ColumnDefinitionCollection()
                {
                    new ColumnDefinition() { Width = GridLength.Star },
                    new ColumnDefinition() { Width = 2 },
                    new ColumnDefinition() { Width = new GridLength((PrintPreviewWidth - 4) / App.DeviceSizeDensity, GridUnitType.Absolute) },
                    new ColumnDefinition() { Width = 2 },
                    new ColumnDefinition() { Width = GridLength.Star }
                }
            };
            Grid.Children.Add(PrintPreviewContent, 2, 0);

            Grid.Children.Add(new Label() { HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand, BackgroundColor = Color.FromRgb(0.4, 0.45, 0.45) }, 0, 0);
            Grid.Children.Add(new Label() { HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand, BackgroundColor = Color.White }, 1, 0);
            Grid.Children.Add(new Label() { HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand, BackgroundColor = Color.White }, 3, 0);
            Grid.Children.Add(new Label() { HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand, BackgroundColor = Color.FromRgb(0.4, 0.45, 0.45) }, 4, 0);
            
            scrollView = new ScrollView()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                Content = Grid
            };
            this.Content = scrollView;
        }
        
        public async Task<ResultSuccess> ScanPreview()
        {
            await Task.Delay(2000);
            
            var result = await TotalScan();
            if (!result.Success)
                return new ResultSuccess(false, "در پیش نمایش چاپ خطایی رخ داده است. لطفا مجددا تلاش کنید.");
            
            MergingBitmaps = result.Data;
            var mergeResult = await Merge();
            if (!mergeResult.Success)
                return new ResultSuccess(false, "در پیش نمایش چاپ خطایی رخ داده است. لطفا مجددا تلاش کنید.");

            this.ToolbarItems.Add(ToolbarItem_Print);

            return new ResultSuccess();
        }
        
        public async Task<ResultSuccess<List<KeyValuePair<int, object>>>> TotalScan()
        {
            var PageHeight = this.Height;
            PrintPreviewHeight = (int)(PageHeight * App.DeviceSizeDensity);
            var TotalHeight = ((Grid)scrollView.Content).Children[0].Height;
            var ScrollPosition = -PageHeight;
            List<KeyValuePair<int, object>> ScanBitmaps = new List<KeyValuePair<int, object>>();
            while(true)
            {
                ScrollPosition += PageHeight;
                await scrollView.ScrollToAsync(0, ScrollPosition, false);
                var result = await Scan();
                if (!result.Success)
                    return new ResultSuccess<List<KeyValuePair<int, object>>> (false, result.Message);
                if (!ScanBitmaps.Any() || ScanBitmaps.Last().Key < Math.Round(scrollView.ScrollY * App.DeviceSizeDensity))
                    ScanBitmaps.Add(new KeyValuePair<int, object>((int)Math.Round(scrollView.ScrollY * App.DeviceSizeDensity), ScanBitmap));
                if (scrollView.ScrollY < ScrollPosition)
                {
                    await scrollView.ScrollToAsync(0, 0, false);
                    break;
                }
            }

            return new ResultSuccess<List<KeyValuePair<int, object>>>(true, "", ScanBitmaps);
        }
        
        public async Task<ResultSuccess> Scan()
        {
            ScanBitmap = null;
            ScanException = null;
            ScanCommand = new Random().Next(1, 1000000000);
            while (ScanBitmap == null && ScanException == null)
                await Task.Delay(100);

            return new ResultSuccess(ScanException == null, ScanException == null ? "" : ScanException.ProperMessage());
        }

        public static BindableProperty ScanCommandProperty = BindableProperty.Create<PrintPreviewPage, int>(p => p.ScanCommand, 0);
        public int ScanCommand
        {
            get { return (int)GetValue(ScanCommandProperty); }
            set { SetValue(ScanCommandProperty, value); }
        }
        
        public async Task<ResultSuccess> Merge()
        {
            MergedBitmap = null;
            MergeException = null;
            MergeCommand = new Random().Next(1, 1000000000);
            while (MergedBitmap == null && MergeException == null)
                await Task.Delay(100);

            return new ResultSuccess(MergeException == null, MergeException == null ? "" : MergeException.ProperMessage());
        }

        public static BindableProperty MergeCommandProperty = BindableProperty.Create<PrintPreviewPage, int>(p => p.MergeCommand, 0);
        public int MergeCommand
        {
            get { return (int)GetValue(MergeCommandProperty); }
            set { SetValue(MergeCommandProperty, value); }
        }
    }

    public class RoundButton : Button
    {
        public RoundButton()
        {
        }
    }

    public class EntryCompanionIcon : Image
    {
        public EntryCompanionIcon()
        {
        }
    }

    public class MyEntry : Entry
    {
        public Thickness? Padding = null;
        public bool RightRounded, LeftRounded;
        public MyEntry()
        {
        }
    }

    public class MyLabel : Label
    {
        public Color MyBackgroundColor = Color.Transparent;
        public Thickness Padding = new Thickness(0);
        public int[] BorderRadius = new int[] { 0, 0 };
        public bool IsGallaryBG;
        public bool IsGallaryTopBG;
        public bool IsGallaryBottomBG;
        public bool IsPrintGrayBackground;
        
        public MyLabel()
        {
        }
    }
    
    public class LeftEntryCompanionLabel : Label
    {
        public static readonly BindableProperty DisabledProperty =
            BindableProperty.Create("Disabled", typeof(bool), typeof(LeftEntryCompanionLabel), false);
        public bool Disabled
        {
            get { return (bool)GetValue(DisabledProperty); }
            set { SetValue(DisabledProperty, value); }
        }
    }
    public class RightEntryCompanionLabel : Label
    {
        public static readonly BindableProperty DisabledProperty =
            BindableProperty.Create("Disabled", typeof(bool), typeof(RightEntryCompanionLabel), false);
        public bool Disabled
        {
            get { return (bool)GetValue(DisabledProperty); }
            set { SetValue(DisabledProperty, value); }
        }
    }
    public class FullRoundedLabel : Label
    {
    }
    public class RightRoundedLabel : Label
    {
        public Thickness? Padding;
    }

    public class MyPicker : Picker
    {
        public MyPicker()
        {
        }
    }

    public class PlaceholderEditor : Editor
    {
        public Thickness? Padding;

        public static readonly BindableProperty PlaceholderProperty =
            BindableProperty.Create("Placeholder", typeof(string), typeof(string), "");

        public PlaceholderEditor()
        {
        }

        public string Placeholder
        {
            get
            {
                return (string)GetValue(PlaceholderProperty);
            }

            set
            {
                SetValue(PlaceholderProperty, value);
            }
        }
    }

    //public class NewCarouselLayout : ScrollView
    //{
    //    readonly Grid _grid;
    //    Dictionary<int, View> Pages;
    //    public static int RowCountInEachPage = 1, ColumnCountInEachPage = 1;

    //    public NewCarouselLayout()
    //    {
    //        Orientation = ScrollOrientation.Horizontal;
    //        _grid = new Grid()
    //        {
    //            RowSpacing = 0,
    //            ColumnSpacing = 0,
    //            Padding = 0,
    //            Margin = 0,
    //            HorizontalOptions = LayoutOptions.FillAndExpand,
    //            VerticalOptions = LayoutOptions.FillAndExpand,
    //            RowDefinitions = new RowDefinitionCollection() { new RowDefinition() { Height = GridLength.Star } },
    //            ColumnDefinitions = new ColumnDefinitionCollection() { }
    //        };

    //        Content = _grid;
    //    }

    //    public IList<View> Children { get { return _grid.Children; } }

    //    private Guid LastLayingoutRequestId;
    //    private double eachPageWidth, eachPageHeight;
    //    protected override async void LayoutChildren(double x, double y, double width, double height)
    //    {
    //        base.LayoutChildren(x, y, width, height);
    //        var ThisLayingoutRequestId = Guid.NewGuid();
    //        LastLayingoutRequestId = ThisLayingoutRequestId;
    //        await Task.Delay(100);
    //        if (LastLayingoutRequestId != ThisLayingoutRequestId)
    //            return;

    //        var OldIsHorizontal = eachPageWidth > eachPageHeight;
    //        var NewIsHorizontal = width > height;

    //        eachPageWidth = width;
    //        eachPageHeight = height;
    //        foreach (var child in Children) child.WidthRequest = eachPageWidth;

    //        if (OldIsHorizontal != NewIsHorizontal)
    //        {
    //            if (Pages != null)
    //            {
    //                foreach (var page in Pages)
    //                {
    //                    Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
    //                    {
    //                        var ThisPageGrid = (Grid)page.Value;
    //                        var Children = ThisPageGrid.Children.ToList();
    //                        foreach (var child in Children)
    //                            ThisPageGrid.Children.Remove(child);

    //                        ThisPageGrid.RowDefinitions.Clear();
    //                        ThisPageGrid.ColumnDefinitions.Clear();
    //                        for (int i = 0; i < (NewIsHorizontal ? RowCountInEachPage : ColumnCountInEachPage); i++)
    //                            ThisPageGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Star });
    //                        for (int j = 0; j < (NewIsHorizontal ? ColumnCountInEachPage : RowCountInEachPage); j++)
    //                            ThisPageGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Star });

    //                        for (int i = 0; i < (NewIsHorizontal ? RowCountInEachPage : ColumnCountInEachPage); i++)
    //                        {
    //                            for (int j = 0; j < (NewIsHorizontal ? ColumnCountInEachPage : RowCountInEachPage); j++)
    //                            {
    //                                if (i * (NewIsHorizontal ? ColumnCountInEachPage : RowCountInEachPage) + j < Children.Count)
    //                                {
    //                                    var Child = Children[i * (NewIsHorizontal ? ColumnCountInEachPage : RowCountInEachPage) + j];
    //                                    ThisPageGrid.Children.Add(Child, j, i);
    //                                }
    //                            }
    //                        }
    //                    });
    //                }
    //            }
    //        }
    //    }

    //    private IList _ItemsSource;
    //    private int _ItemsSourceCount;
    //    public IList ItemsSource { get { return _ItemsSource; } set { _ItemsSource = value; _ItemsSourceCount = _ItemsSource.Count; ItemsSourceChanged(); } }

    //    public void ItemsSourceChanged()
    //    {
    //        Pages = new Dictionary<int, View>();
    //        _grid.Children.Clear();
    //        _grid.ColumnDefinitions.Clear();
    //        //for (int i = 0; i < ItemsSource.Count; i++)
    //        //    _grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = eachPageWidth });
    //        Initialized = false;
    //        SelectedIndex = 0;
    //    }
        
    //    public DataTemplate ItemTemplate { get; set; }
    //    public View MakeNewItemInPage(object dataSource)
    //    {
    //        var view = (View)ItemTemplate.CreateContent();
    //        var bindableObject = view as BindableObject;
    //        if (bindableObject != null)
    //            bindableObject.BindingContext = dataSource;
    //        return view;
    //    }

    //    int _selectedIndex;

    //    public static readonly BindableProperty SelectedIndexProperty =
    //        BindableProperty.Create(
    //            nameof(SelectedIndex),
    //            typeof(int),
    //            typeof(NewCarouselLayout),
    //            0,
    //            BindingMode.TwoWay
    //        );

    //    static bool Initialized = false;
    //    public int SelectedIndex
    //    {
    //        get
    //        {
    //            return (int)GetValue(SelectedIndexProperty);
    //        }
    //        set
    //        {
    //            var CurrentSelectedIndex = (int)GetValue(SelectedIndexProperty);
    //            if (CurrentSelectedIndex != value || !Initialized)
    //            {
    //                var NewSelectedIndex = value;
    //                SetValue(SelectedIndexProperty, value);

    //                if (CurrentSelectedIndex != NewSelectedIndex || !Initialized)
    //                {
    //                    var Appearings = new List<int>();
    //                    var Disappearings = new List<int>();
    //                    if (!Initialized)
    //                    {
    //                        Initialized = true;
    //                        Appearings.Add(0);
    //                        Appearings.Add(1);
    //                    }
    //                    else
    //                    {
    //                        Appearings.Add(CurrentSelectedIndex);
    //                        Appearings.Add(NewSelectedIndex);
    //                        Appearings.Add(NewSelectedIndex + NewSelectedIndex - CurrentSelectedIndex);
    //                        Disappearings.Add(CurrentSelectedIndex + CurrentSelectedIndex - NewSelectedIndex);
    //                    }

    //                    foreach (var indx in Appearings)
    //                    {
    //                        Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
    //                        {
    //                            if (indx >= 0 && indx < _ItemsSource.Count)
    //                            {
    //                                if (!Pages.ContainsKey(indx))
    //                                {
    //                                    var _ThisPageItemsSource = (IList)(_ItemsSource[indx]);
    //                                    var NewPage = new Grid()
    //                                    {
    //                                        RowSpacing = 2,
    //                                        ColumnSpacing = 2,
    //                                        WidthRequest = eachPageWidth,
    //                                        RowDefinitions = new RowDefinitionCollection() { },
    //                                        ColumnDefinitions = new ColumnDefinitionCollection() { },
    //                                        HorizontalOptions = LayoutOptions.FillAndExpand,
    //                                        VerticalOptions = LayoutOptions.FillAndExpand
    //                                    };
    //                                    var IsHorizontal = eachPageWidth > eachPageHeight;
    //                                    for (int i = 0; i < (IsHorizontal ? RowCountInEachPage : ColumnCountInEachPage); i++)
    //                                        NewPage.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Star });
    //                                    for (int j = 0; j < (IsHorizontal ? ColumnCountInEachPage : RowCountInEachPage); j++)
    //                                        NewPage.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Star });
    //                                    _grid.Children.Add(NewPage, indx, 0);
    //                                    Pages.Add(indx, NewPage);
    //                                    for (int i = 0; i < (IsHorizontal ? RowCountInEachPage : ColumnCountInEachPage); i++)
    //                                    {
    //                                        for (int j = 0; j < (IsHorizontal ? ColumnCountInEachPage : RowCountInEachPage); j++)
    //                                        {
    //                                            if (i * (IsHorizontal ? ColumnCountInEachPage : RowCountInEachPage) + j < _ThisPageItemsSource.Count)
    //                                            {
    //                                                var dataSource = _ThisPageItemsSource[i * (IsHorizontal ? ColumnCountInEachPage : RowCountInEachPage) + j];
    //                                                var NewItemInPage = MakeNewItemInPage(dataSource);
    //                                                NewPage.Children.Add(NewItemInPage, j, i);
    //                                            }
    //                                        }
    //                                    }
    //                                }
    //                            }
    //                        });
    //                    }

    //                    foreach (var indx in Disappearings)
    //                    {
    //                        Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
    //                        {
    //                            if (indx >= 0 && indx < _ItemsSource.Count)
    //                            {
    //                                if (Pages.ContainsKey(indx))
    //                                {
    //                                    var CurrentPage = Pages[indx];
    //                                    _grid.Children.Remove(CurrentPage);
    //                                    Pages.Remove(indx);
    //                                }
    //                            }
    //                        });
    //                    }
    //                }
    //            }
    //        }
    //    }
    //}

    public class MyProgressBar : ProgressBar
    {
        public Color Color = Color.FromHex("32A527");
        public MyProgressBar()
        {
        }
    }

    public enum KeyboardType
    {
        Int = 1,
        Decimal = 2,
        Default = 0
    }
    class KeyboardButton
    {
        public Label Label { get; set; }
        public Image Image { get; set; }
        public string Value { get; set; }
    }
    public class MyKeyboard<ObjectType, PropertyType>
    {
        Expression<Func<ObjectType, PropertyType>> PropertySelector;
        ObjectType Object;
        ICommand OnOK, OnChange;
        KeyboardType KeyboardType;
        Grid KeyboardHolderGrid;
        KeyboardButton Button_1, Button_2, Button_3, Button_4, Button_5, Button_6, Button_7, Button_8, Button_9, Button_0,
            Button_Dot, Button_Clear, Button_OK,
            Button_Temp1;
        KeyboardButton[][] Labels_Portrait, Labels_Landscape;
        string str;
        
        public MyKeyboard(Grid KeyboardHolderGrid, ICommand OnOK, ICommand OnChange)
        {
            this.KeyboardType = typeof(PropertyType).Equals(typeof(int)) ? KeyboardType.Int : typeof(PropertyType).Equals(typeof(decimal)) ? KeyboardType.Decimal : KeyboardType.Default;
            this.KeyboardHolderGrid = KeyboardHolderGrid;
            this.OnOK = OnOK;
            this.OnChange = OnChange;
            
            Button_1 = new KeyboardButton() { Value = "1", Label = new Label() { Text = "۱", VerticalOptions = LayoutOptions.FillAndExpand, HorizontalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, FontSize = 20, FontAttributes = FontAttributes.Bold } };
            Button_2 = new KeyboardButton() { Value = "2", Label = new Label() { Text = "۲", VerticalOptions = LayoutOptions.FillAndExpand, HorizontalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, FontSize = 20, FontAttributes = FontAttributes.Bold } };
            Button_3 = new KeyboardButton() { Value = "3", Label = new Label() { Text = "۳", VerticalOptions = LayoutOptions.FillAndExpand, HorizontalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, FontSize = 20, FontAttributes = FontAttributes.Bold } };
            Button_4 = new KeyboardButton() { Value = "4", Label = new Label() { Text = "۴", VerticalOptions = LayoutOptions.FillAndExpand, HorizontalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, FontSize = 20, FontAttributes = FontAttributes.Bold } };
            Button_5 = new KeyboardButton() { Value = "5", Label = new Label() { Text = "۵", VerticalOptions = LayoutOptions.FillAndExpand, HorizontalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, FontSize = 20, FontAttributes = FontAttributes.Bold } };
            Button_6 = new KeyboardButton() { Value = "6", Label = new Label() { Text = "۶", VerticalOptions = LayoutOptions.FillAndExpand, HorizontalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, FontSize = 20, FontAttributes = FontAttributes.Bold } };
            Button_7 = new KeyboardButton() { Value = "7", Label = new Label() { Text = "۷", VerticalOptions = LayoutOptions.FillAndExpand, HorizontalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, FontSize = 20, FontAttributes = FontAttributes.Bold } };
            Button_8 = new KeyboardButton() { Value = "8", Label = new Label() { Text = "۸", VerticalOptions = LayoutOptions.FillAndExpand, HorizontalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, FontSize = 20, FontAttributes = FontAttributes.Bold } };
            Button_9 = new KeyboardButton() { Value = "9", Label = new Label() { Text = "۹", VerticalOptions = LayoutOptions.FillAndExpand, HorizontalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, FontSize = 20, FontAttributes = FontAttributes.Bold } };
            Button_0 = new KeyboardButton() { Value = "0", Label = new Label() { Text = "۰", VerticalOptions = LayoutOptions.FillAndExpand, HorizontalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, FontSize = 20, FontAttributes = FontAttributes.Bold } };
            Button_Dot = new KeyboardButton() { Value = ".", Label = new Label() { Text = "/", VerticalOptions = LayoutOptions.FillAndExpand, HorizontalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, FontSize = 20, FontAttributes = FontAttributes.Bold } };
            Button_Clear = new KeyboardButton() { Value = "Clear", Image = new Image() { VerticalOptions = LayoutOptions.FillAndExpand, HorizontalOptions = LayoutOptions.FillAndExpand, Source = "BackSpace.png" } };
            Button_OK = new KeyboardButton() { Value = "OK", Image = new Image() { VerticalOptions = LayoutOptions.FillAndExpand, HorizontalOptions = LayoutOptions.FillAndExpand, Source = "OK_B.png" } };
            Button_Temp1 = new KeyboardButton() { Value = "", Label = new Label() { Text = "", VerticalOptions = LayoutOptions.FillAndExpand, HorizontalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.Center, VerticalTextAlignment = TextAlignment.Center, FontSize = 20, FontAttributes = FontAttributes.Bold } };

            if (KeyboardType == KeyboardType.Int)
            {
                Labels_Landscape = new KeyboardButton[1][];
                Labels_Landscape[0] = new KeyboardButton[12] { Button_1, Button_2, Button_3, Button_4, Button_5, Button_6, Button_7, Button_8, Button_9, Button_0, Button_Clear, Button_OK };
                Labels_Portrait = new KeyboardButton[2][];
                Labels_Portrait[0] = new KeyboardButton[6] { Button_1, Button_2, Button_3, Button_4, Button_5, Button_Clear };
                Labels_Portrait[1] = new KeyboardButton[6] { Button_6, Button_7, Button_8, Button_9, Button_0, Button_OK };
            }
            else if (KeyboardType == KeyboardType.Decimal)
            {
                Labels_Landscape = new KeyboardButton[1][];
                Labels_Landscape[0] = new KeyboardButton[13] { Button_1, Button_2, Button_3, Button_4, Button_5, Button_6, Button_7, Button_8, Button_9, Button_0, Button_Dot, Button_Clear, Button_OK };
                Labels_Portrait = new KeyboardButton[2][];
                Labels_Portrait[0] = new KeyboardButton[7] { Button_1, Button_2, Button_3, Button_4, Button_5, Button_Temp1, Button_Clear };
                Labels_Portrait[1] = new KeyboardButton[7] { Button_6, Button_7, Button_8, Button_9, Button_0, Button_Dot, Button_OK };
            }

            var AllButtons = new List<KeyboardButton>() { Button_1, Button_2, Button_3, Button_4, Button_5, Button_6, Button_7, Button_8, Button_9, Button_0,
                Button_Dot, Button_Clear, Button_OK,
                Button_Temp1
            };
            foreach (var button in AllButtons)
            {
                var TapGestureRecognizer = new TapGestureRecognizer();
                TapGestureRecognizer.Tapped += (s, e) =>
                {
                    var Value = button.Value;
                    KeyboardClicked(Value);
                };
                if(button.Label != null)
                    button.Label.GestureRecognizers.Add(TapGestureRecognizer);
                else
                    button.Image.GestureRecognizers.Add(TapGestureRecognizer);
            }
        }

        public void SetObject(ObjectType Object, Expression<Func<ObjectType, PropertyType>> PropertySelector)
        {
            this.Object = Object;
            this.PropertySelector = PropertySelector;

            var CurrentValueFunc = PropertySelector.Compile();
            var CurrentValue = CurrentValueFunc(Object);
            str = typeof(PropertyType).Equals(typeof(int)) ? CurrentValue.ToString() : typeof(PropertyType).Equals(typeof(decimal)) ? Convert.ToDecimal(CurrentValue.ToString()).ToString("##############0.###############") : "";
        }

        void KeyboardClicked(string Value)
        {
            var oldStr = str + "";
            try
            {
                App.UniversalLineInApp = 4567456001;
                if (Object != null && Value != "")
                {
                    App.UniversalLineInApp = 4567456002;
                    var func = PropertySelector.Compile();
                    //App.UniversalLineInApp = 4567456003;
                    var CurrentValue = func(Object);
                    //App.UniversalLineInApp = 4567456004;

                    switch (Value)
                    {
                        case "OK":
                            //App.UniversalLineInApp = 4567456005;
                            Hide();
                            //App.UniversalLineInApp = 4567456006;
                            OnOK.Execute(CurrentValue);
                            //App.UniversalLineInApp = 4567456007;
                            return;
                        case "Clear":
                            //App.UniversalLineInApp = 4567456008;
                            str = str.Length == 0 ? "" : str.Substring(0, str.Length - 1);
                            //App.UniversalLineInApp = 4567456009;
                            if (str == "" && (KeyboardType == KeyboardType.Int || KeyboardType == KeyboardType.Decimal))
                                str = "0";
                            //App.UniversalLineInApp = 4567456010;
                            break;
                        default:
                            //App.UniversalLineInApp = 4567456011;
                            str = str + Value;
                            //App.UniversalLineInApp = 4567456012;
                            break;
                    }
                    App.UniversalLineInApp = 4567456013;

                    var StrLastCharIsDot = str.EndsWith(".");
                    //App.UniversalLineInApp = 4567456014;
                    if (StrLastCharIsDot)
                        str += "354168413153848456";
                    //App.UniversalLineInApp = 4567456015;
                    object NewValue = str;
                    //App.UniversalLineInApp = 4567456016;
                    App.SpecialLog = "KeyboardType: " + (KeyboardType == KeyboardType.Int ? "KeyboardType.Int" : KeyboardType == KeyboardType.Decimal ? "KeyboardType.Decimal" : "---") +
                        "str: " + str;
                    if (KeyboardType == KeyboardType.Int)
                        NewValue = Convert.ToInt32(str);
                    App.UniversalLineInApp = 4567456017;
                    if (KeyboardType == KeyboardType.Decimal)
                        NewValue = Convert.ToDecimal(str);
                    //App.UniversalLineInApp = 4567456018;

                    var prop = (PropertyInfo)((MemberExpression)PropertySelector.Body).Member;
                    //App.UniversalLineInApp = 4567456019;
                    prop.SetValue(Object, NewValue, null);
                    //App.UniversalLineInApp = 4567456020;

                    NewValue = func(Object);
                    App.UniversalLineInApp = 4567456021;
                    if (KeyboardType == KeyboardType.Int || KeyboardType == KeyboardType.Decimal)
                        str = NewValue.ToString();
                    //App.UniversalLineInApp = 4567456022;
                    if (StrLastCharIsDot)
                        str = str + ".";
                    //App.UniversalLineInApp = 4567456023;

                    OnChange.Execute(NewValue);
                    App.UniversalLineInApp = 4567456024;
                }
            }
            catch (Exception err)
            {
                str = oldStr;
            }
        }

        public void OrientationChanged(bool IsLandscape)
        {
            var ButtonsArray = IsLandscape ? Labels_Landscape : Labels_Portrait;
            KeyboardHolderGrid.RowDefinitions = new RowDefinitionCollection() { };
            for (int i = 0; i < ButtonsArray.Length; i++)
                KeyboardHolderGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(42) });
            KeyboardHolderGrid.ColumnDefinitions = new ColumnDefinitionCollection() { };
            for (int i = 0; i < ButtonsArray[0].Length; i++)
                KeyboardHolderGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

            KeyboardHolderGrid.Children.Clear();
            for (int i = 0; i < ButtonsArray.Length; i++)
                for (int j = 0; j < ButtonsArray[i].Length; j++)
                    if(ButtonsArray[i][j].Label != null)
                        KeyboardHolderGrid.Children.Add(ButtonsArray[i][j].Label, j, i);
                    else
                        KeyboardHolderGrid.Children.Add(ButtonsArray[i][j].Image, j, i);
        }

        public void Hide()
        {
            KeyboardHolderGrid.IsVisible = false;
        }

        public void Show()
        {
            KeyboardHolderGrid.IsVisible = true;
        }
    }

    public enum MapIcon
    {
        BlueBallon = 1,
        RedBallon = 2,
        GreenBallon = 3
    }
    public class CustomPin
    {
        public Pin Pin { get; set; }
        public string Id { get; set; }
        public object Data { get; set; }
        public MapIcon Icon { get; set; }
        public object Marker { get; set; }
    }
    public class CustomMap : Map
    {
        public CustomMap() : base()
        {
        }

        public CustomMap(MapSpan MapSpan) : base(MapSpan)
        {
        }
        
        private List<CustomPin> _CustomPins;
        public List<CustomPin> CustomPins
        {
            get { return _CustomPins; }
            set {
                _CustomPins = value;
                OnPropertyChanged("CustomPins");
            }
        }
        public event EventHandler ShowingPinChanged;

        public virtual void OnShowingPinChanged(EventArgs e)
        {
            EventHandler handler = ShowingPinChanged;
            if (handler != null)
                handler(this, e);
        }

        public void NotifyChangeShowingPin()
        {
            OnPropertyChanged("ShowingPin");
            if(ShowingPin != null && ((DBRepository.PartnerListModel)ShowingPin.Data).Selected && this.VisibleRegion != null)
                MoveToRegion(MapSpan.FromCenterAndRadius(ShowingPin.Pin.Position, this.VisibleRegion.Radius));
        }
        
        public CustomPin ShowingPin;
    }

    public class ListViewLongClickEventArgs : EventArgs
    {
        public int Position { get; set; }
    }
    public class MyListView : ListView
    {
        public event EventHandler OnLongClick;
        public virtual void LongClick(int Position)
        {
            EventHandler handler = OnLongClick;
            if (handler != null)
                handler(this, new ListViewLongClickEventArgs() { Position = Position });
        }
    }

    public class MyCheckBox : Button
    {
        public Thickness? Padding { get; set; }
        public static readonly BindableProperty CheckedProperty =
            BindableProperty.Create("Checked", typeof(bool), typeof(MyCheckBox), false);
        public bool Checked
        {
            get { return (bool)GetValue(CheckedProperty); }
            set { SetValue(CheckedProperty, value); }
        }
    }
}
