using Kara.Assets;
using Kara.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Kara.CustomRenderer;
using Kara.Helpers;
using System.IO;
using Android.Views;
using TextAlignment = Xamarin.Forms.TextAlignment;
using View = Xamarin.Forms.View;

namespace Kara
{
    public partial class OrderPreviewForm : GradientContentPage
    {
        private ToolbarItem ToolbarItem_LocalSave, ToolbarItem_SendToServer, ToolbarItem_AddDiscounts, ToolbarItem_AddFreeProducts, ToolbarItem_Print, ToolbarItem_ZoomIn, ToolbarItem_ZoomOut;

        private SaleOrder SaleOrder;
        private InsertedInformations_Orders OrdersForm;
        private PartnerListForm PartnerListForm;
        private OrderInsertForm OrderInsertForm;
        private OrderBeforePreviewForm OrderBeforePreviewForm;
        private bool SaveOption;
        private List<KeyValuePair<Guid, RuleModel>> _userSelectedOptionalRules = new List<KeyValuePair<Guid, RuleModel>>();

        public OrderPreviewForm(SaleOrder SaleOrder, InsertedInformations_Orders OrdersForm, PartnerListForm PartnerListForm, OrderInsertForm OrderInsertForm, OrderBeforePreviewForm OrderBeforePreviewForm, bool SaveOption)
        {
            InitializeComponent();

            BusyIndicatorContainder.BackgroundColor = Color.FromRgba(130, 140, 150, 255);
            BusyIndicator.Color = Color.White;

            ToolbarItem_LocalSave = new ToolbarItem();
            ToolbarItem_LocalSave.Text = "ذخیره محلی";
            ToolbarItem_LocalSave.Icon = "Save.png";
            ToolbarItem_LocalSave.Activated += SubmitSaleOrderToStorage;
            ToolbarItem_LocalSave.Order = ToolbarItemOrder.Primary;
            ToolbarItem_LocalSave.Priority = 0;

            ToolbarItem_SendToServer = new ToolbarItem();
            ToolbarItem_SendToServer.Text = "ارسال به سرور";
            ToolbarItem_SendToServer.Icon = "Upload.png";
            ToolbarItem_SendToServer.Activated += SubmitSaleOrderToServer;
            ToolbarItem_SendToServer.Order = ToolbarItemOrder.Primary;
            ToolbarItem_SendToServer.Priority = 0;

            ToolbarItem_AddDiscounts = new ToolbarItem();
            ToolbarItem_AddDiscounts.Text = "اضافه کردن تخفیفات";
            ToolbarItem_AddDiscounts.Icon = "AddDiscounts.png";
            ToolbarItem_AddDiscounts.Activated += ToolbarItem_AddDiscounts_Activated;
            ToolbarItem_AddDiscounts.Order = ToolbarItemOrder.Primary;
            ToolbarItem_AddDiscounts.Priority = 0;

            ToolbarItem_AddFreeProducts = new ToolbarItem();
            ToolbarItem_AddFreeProducts.Text = "اضافه کردن اشانتیون ها";
            ToolbarItem_AddFreeProducts.Icon = "AddFreeProducts.png";
            ToolbarItem_AddFreeProducts.Activated += ToolbarItem_AddFreeProducts_Activated;
            ToolbarItem_AddFreeProducts.Order = ToolbarItemOrder.Primary;
            ToolbarItem_AddFreeProducts.Priority = 0;

            ToolbarItem_Print = new ToolbarItem();
            ToolbarItem_Print.Text = "چاپ";
            ToolbarItem_Print.Icon = "InvoicePreview.png";
            ToolbarItem_Print.Activated += ToolbarItem_Print_Activated;
            ToolbarItem_Print.Order = ToolbarItemOrder.Primary;
            ToolbarItem_Print.Priority = 0;
            this.ToolbarItems.Add(ToolbarItem_Print);

            //ToolbarItem_ZoomIn = new ToolbarItem();
            //ToolbarItem_ZoomIn.Text = "بزرگ نمایی";
            //ToolbarItem_ZoomIn.Icon = "ZoomIn.png";
            //ToolbarItem_ZoomIn.Activated += ToolbarItem_ZoomIn_Activated;
            //ToolbarItem_ZoomIn.Order = ToolbarItemOrder.Primary;
            //ToolbarItem_ZoomIn.Priority = 0;
            //this.ToolbarItems.Add(ToolbarItem_ZoomIn);
            //
            //ToolbarItem_ZoomOut = new ToolbarItem();
            //ToolbarItem_ZoomOut.Text = "کوچک نمایی";
            //ToolbarItem_ZoomOut.Icon = "ZoomOut.png";
            //ToolbarItem_ZoomOut.Activated += ToolbarItem_ZoomOut_Activated;
            //ToolbarItem_ZoomOut.Order = ToolbarItemOrder.Primary;
            //ToolbarItem_ZoomOut.Priority = 0;
            //this.ToolbarItems.Add(ToolbarItem_ZoomOut);

            this.SaveOption = SaveOption;
            if (SaveOption)
            {
                if (App.Accesses.AddDiscounts)
                    this.ToolbarItems.Add(ToolbarItem_AddDiscounts);
                if (App.Accesses.AddFreeProducts)
                    this.ToolbarItems.Add(ToolbarItem_AddFreeProducts);
                this.ToolbarItems.Add(ToolbarItem_LocalSave);
                this.ToolbarItems.Add(ToolbarItem_SendToServer);
            }

            this.SaleOrder = SaleOrder;
            this.OrdersForm = OrdersForm;
            this.PartnerListForm = PartnerListForm;
            this.OrderInsertForm = OrderInsertForm;
            this.OrderBeforePreviewForm = OrderBeforePreviewForm;
        }

        private void ToolbarItem_ZoomOut_Activated(object sender, EventArgs e)
        {
            //Device.GetNamedSize(NamedSize.Small, typeof(Label))--;
            //ShowOrder(false);
        }

        private void ToolbarItem_ZoomIn_Activated(object sender, EventArgs e)
        {
            //Device.GetNamedSize(NamedSize.Small, typeof(Label))++;
            //ShowOrder(false);
        }

        private async void ToolbarItem_Print_Activated(object sender, EventArgs e)
        {
            if (App.SelectedPrinter == null)
                App.ShowError("خطا", "ابتدا از قسمت تنظیمات یک چاپگر تعریف نمایید.", "خوب");
            else
            {
                var printPreviewPage = new PrintPreviewPage();
                await this.Navigation.PushAsync(printPreviewPage);
                printPreviewPage.SetContent(PrintPreview.Content, App.SelectedPrinter.PrintWidthPixel);
                await LayoutPrint(true);

                await Task.Delay(100);

                var result = await printPreviewPage.ScanPreview();
                if (!result.Success)
                    App.ShowError("خطا", result.Message, "خوب");
            }
        }

        bool PreviewCreated = false;
        public static bool ShowPreviewOnAppearing = true;
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            //if(ShowPreviewOnAppearing)
            //{
            //    await Task.Delay(10);

            //    if (!PreviewCreated)
            //    {
            //        await ShowOrder(!SaveOption);
            //        PreviewCreated = true;
            //    }

            //    await LayoutPrint(false);
            //    PrintPreview.WidthRequest = Math.Max(this.Width, 100);

            //    var temp = PrintPreview.Content;
            //    PrintPreview.Content = null;
            //    PrintPreview.Content = temp;
            //}

            if (App.AllowOptionalDiscountRules.Value)
            {
                var AllSystemStuffs = (await App.DB.GetStuffsAsync()).Data.ToDictionary(a => a.Id);

                var DiscountRules = await App.DB.GetDiscountRulesAsync();
                int SettlementDay = SaleOrder.SettlementType.Day;

                OrderModel SaleOrderModel = new OrderModel()
                {
                    SettlementTypeId = SaleOrder.SettlementTypeId,
                    SettlementDay = SettlementDay,
                    VisitorId = App.UserPersonnelId.Value,
                    OrderInsertDate = SaleOrder.InsertDateTime,
                    Partner = SaleOrder.Partner,
                    Articles = SaleOrder.SaleOrderStuffs.Select(a => new ArticleModel()
                    {
                        Id = a.Id,
                        Stuff = a.Package.Stuff,
                        Package = a.Package,
                        BatchNumber = a.BatchNumber,
                        Quantity = a.Quantity,
                        UnitPrice = a.SalePrice / a.Package.Coefficient
                    }).ToArray()
                };

                var DiscountCalculator = new DiscountCalculator(App.SystemName.Value, App.AllowOptionalDiscountRules_MultiSelection.Value, AllSystemStuffs, SaleOrderModel, DiscountRules,null);

                var equalPriorities = new List<Tuple<int,double, Guid, string>>();

                foreach (KeyValuePair<int, Dictionary<string, List<RuleModel>>> discountRule in DiscountRules)
                {
                    foreach (var rule in discountRule.Value)
                    {
                        for (var k = 0; k < rule.Value.Count; k++)
                        {
                            for (int x = 0; x < SaleOrderModel.Articles.Length; x++)
                            {
                                if (DiscountCalculator.CheckRuleConditions(rule.Value[k], SaleOrderModel, x))
                                {
                                    Dictionary<string, List<RuleModel>> discountRuleValue = discountRule.Value;

                                    var rules = discountRuleValue.Values.ToList();
                                    var r = rules[0];

                                    if (rules.Count(e => e[0].Priority == rule.Value[k].Priority) > 1)
                                    {
                                        var result = $"Row:{x + 1},Pr:{rule.Value[k].Priority},id:{rule.Value[k].RuleId},desc:{rule.Value[k].RuleDescription}" + Environment.NewLine;
                                        equalPriorities.Add(new Tuple<int, double, Guid, string>(x, rule.Value[k].Priority,rule.Value[k].RuleId, rule.Value[k].RuleDescription));
                                    }
                                }

                            }
                        }
                    }
                }

                List<int> distinctRows = new List<int>();


                foreach (Tuple<int,double, Guid, string> tuple in equalPriorities)
                {
                    if(!distinctRows.Any(a=>a==tuple.Item1))
                       distinctRows.Add(tuple.Item1);
                }

                List<KeyValuePair<int, List<RuleModel>>> eachRowRules = new List<KeyValuePair<int, List<RuleModel>>>(); 

                foreach (int row in distinctRows)
                {
                    eachRowRules.Add( new KeyValuePair<int, List<RuleModel>>
                        (row, equalPriorities.Where(a=>a.Item1==row).Select(x=>new RuleModel()
                        {
                            Priority = x.Item2,RuleId = x.Item3,RuleDescription = x.Item4
                        }).ToList()));
                }

                foreach (KeyValuePair<int, List<RuleModel>> keyValuePair in eachRowRules)
                {
                    var options = keyValuePair.Value.Select(x => x.RuleDescription).ToList();

                    SaleOrderStuff stuff = this.SaleOrder.SaleOrderStuffs[keyValuePair.Key];

                    var rowStuffName = (
                        stuff.Package.Stuff.Name 
                        //+ (stuff.BatchNumberId.HasValue
                        //    ? ("\nسری ساخت: " + stuff.BatchNumber.BatchNumber + "، تاریخ انقضاء: " +
                        //       (stuff.BatchNumber.ExpirationDatePresentation == (int) DatePresentation.Shamsi
                        //           ? stuff.BatchNumber.ExpirationDate.ToShortStringForDate()
                        //           : stuff.BatchNumber.ExpirationDate.ToString("yyyy/MM/dd")))
                        //    : "") +
                        //(stuff.StuffSettlementDay.HasValue
                        //    ? (" (" + stuff.StuffSettlementDay.Value.ToString() + " روزه)")
                        //    : "")
                    ).ToPersianDigits();

                    var answer = await DisplayActionSheet($"تعیین تخفیف {rowStuffName}", "بستن", "", options.ToArray());
                    
                    var selectedRule = keyValuePair.Value.FirstOrDefault(r => r.RuleDescription == answer);

                    _userSelectedOptionalRules.Add(new KeyValuePair<Guid, RuleModel>(stuff.Id, selectedRule));
                }

                //await CaclculateDiscounts(_userSelectedOptionalRules);
            }

            if (ShowPreviewOnAppearing)
            {
                await Task.Delay(10);

                if (!PreviewCreated)
                {
                    await ShowOrder(!SaveOption);
                    PreviewCreated = true;
                }

                await LayoutPrint(false);
                PrintPreview.WidthRequest = Math.Max(this.Width, 100);

                var temp = PrintPreview.Content;
                PrintPreview.Content = null;
                PrintPreview.Content = temp;
            }
        }

        public async Task LayoutPrint(bool ForPrintPreview)
        {
            try
            {
                App.UniversalLineInApp = 65481001;
                double PageWitdth = ForPrintPreview ? (App.SelectedPrinter == null ? 0 : App.SelectedPrinter.PrintWidthPixel) : this.Width * App.DeviceSizeDensity;
                //App.UniversalLineInApp = 65481002;
                double FontSize = ForPrintPreview ? (App.SelectedPrinter == null ? 10 : App.SelectedPrinter.FontSize) : App.OrderPreviewFontSize.Value;
                //App.UniversalLineInApp = 65481003;

                foreach (var item in LabelFontSizes)
                    item.Key.FontSize = item.Value * FontSize;
                //App.UniversalLineInApp = 65481004;

                PrintPreview.Content.WidthRequest = (PageWitdth - 4) / App.DeviceSizeDensity;
                App.UniversalLineInApp = 65481005;

                ArticlesGrid.RowSpacing = 2.0 / (ForPrintPreview ? App.DeviceSizeDensity : 1);
                //App.UniversalLineInApp = 65481006;
                ArticlesGrid.ColumnSpacing = 2.0 / (ForPrintPreview ? App.DeviceSizeDensity : 1);
                //App.UniversalLineInApp = 65481007;

                if (!ForPrintPreview)
                {
                    App.UniversalLineInApp = 65481008;
                    for (int i = 0; i < BodyFirstRowLabels.Count; i++)
                        ((Grid)(((StackLayout)PrintPreview.Content).Children[1])).ColumnDefinitions[i].Width = GridLength.Auto;
                    //App.UniversalLineInApp = 65481009;

                    await Task.Delay(100);
                    //App.UniversalLineInApp = 65481010;

                    var ColumnsWidth = BodyFirstRowLabels.Select((a, index) => new
                    {
                        index,
                        Width = a.Width
                    }).ToArray();
                    App.UniversalLineInApp = 65481011;

                    if (ColumnsWidth.Any(a => a.Width > 0))
                    {
                        App.UniversalLineInApp = 65481012;
                        ColumnsWidth = ColumnsWidth.Select(a => new
                        {
                            index = a.index,
                            Width = a.Width > 0 ? a.Width : 1
                        }).ToArray();
                        //App.UniversalLineInApp = 65481013;

                        for (int i = 0; i < ColumnsWidth.Length; i++)
                            ((Grid)(((StackLayout)PrintPreview.Content).Children[1])).ColumnDefinitions[i].Width = new GridLength(ColumnsWidth[ColumnsWidth.Length - 1 - i].Width, GridUnitType.Star);
                        App.UniversalLineInApp = 65481014;
                    }
                    App.UniversalLineInApp = 65481015;
                }
            }
            catch (Exception)
            {
            }
        }

        private void ToolbarItem_AddDiscounts_Activated(object sender, EventArgs e)
        {
            var AddDiscountsForm = new AddDiscounts(SaleOrder, this)
            {
                StartColor = Color.FromHex("E6EBEF"),
                EndColor = Color.FromHex("A6CFED")
            };
            Navigation.PushAsync(AddDiscountsForm);
        }

        private void ToolbarItem_AddFreeProducts_Activated(object sender, EventArgs e)
        {
            var AddDiscountsForm = new AddFreeProducts(SaleOrder, this)
            {
                StartColor = Color.FromHex("E6EBEF"),
                EndColor = Color.FromHex("A6CFED")
            };
            Navigation.PushAsync(AddDiscountsForm);
        }

        View ResultViewOfOrder;
        List<KeyValuePair<Label, double>> LabelFontSizes;
        List<Label> BodyFirstRowLabels;
        Grid ArticlesGrid;


        public async Task ShowOrder(bool WithoutCalculation)
        {
            WaitToggle(false);

            try
            {
                if (!WithoutCalculation)
                {
                    var result1 = await CheckPossibleQuantityCoefficients();
                    if (!result1.Success)
                    {
                        App.ShowError("خطا", result1.Message, "خوب");
                        WaitToggle(false);
                        try { await Navigation.PopAsync(); } catch (Exception) { }
                        return;
                    }

                    var result = await CaclculateDiscounts(_userSelectedOptionalRules);
                    if (!result.Success)
                    {
                        App.ShowError("خطا", result.Message, "خوب");
                        WaitToggle(false);
                        try { await Navigation.PopAsync(); } catch (Exception) { }
                        return;
                    }

                    result = await CaclculateVATs();
                    if (!result.Success)
                    {
                        App.ShowError("خطا", result.Message, "خوب");
                        WaitToggle(false);
                        try { await Navigation.PopAsync(); } catch (Exception) { }
                        return;
                    }
                }
                
                var _FontSizeForView = App.OrderPreviewFontSize.Value;
                var _FontSizeForPrint = App.SelectedPrinter == null ? 10 : App.SelectedPrinter.FontSize;

                Color WHITE = Color.White, BLACK = Color.Black, LIGHTGRAY = Color.FromHex("ccc"), DARKGRAY = Color.FromHex("ccc");

                LabelFontSizes = new List<KeyValuePair<Label, double>>();

                BodyFirstRowLabels = new List<Label>();
                var Content = await Task.Factory.StartNew(() =>
                {
                    var HeaderGrid = new Grid()
                    {
                        Padding = 0,
                        RowSpacing = 1,
                        ColumnSpacing = 1,
                        BackgroundColor = WHITE,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        VerticalOptions = LayoutOptions.StartAndExpand,
                        RowDefinitions = new RowDefinitionCollection() { },
                        ColumnDefinitions = new ColumnDefinitionCollection()
                        {
                            new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) },
                            new ColumnDefinition() { Width = new GridLength(1.5, GridUnitType.Star) },
                            new ColumnDefinition() { Width = new GridLength(1.5, GridUnitType.Star) },
                            new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) }
                        }
                    };

                    var row = 0;
                    var TitleRow = (!string.IsNullOrEmpty(App.PrintTitle.Value) ? 1 : 0) +
                        (!string.IsNullOrEmpty(App.CompanyNameForPrint.Value) ? 1 : 0);
                    if (TitleRow == 0 && !string.IsNullOrEmpty(App.CompanyLogoForPrint.Value))
                        TitleRow = 1;
                    for (int i = 0; i < TitleRow; i++)
                        HeaderGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1.4, GridUnitType.Star) });

                    HeaderGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                    HeaderGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0.5, GridUnitType.Star) });
                    HeaderGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                    HeaderGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                    HeaderGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1.5, GridUnitType.Star) });
                    HeaderGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                    
                    var TempLabel1 = new MyLabel() { Padding = new Thickness(10, 0), TextColor = BLACK, Text = "" };
                    HeaderGrid.Children.Add(TempLabel1, 3, row);
                    if (TitleRow == 2)
                        Grid.SetRowSpan(TempLabel1, 2);

                    if (!string.IsNullOrEmpty(App.PrintTitle.Value))
                    {
                        var TitleLabel = new MyLabel()
                        {
                            Padding = new Thickness(10, 0),
                            TextColor = BLACK,
                            Text = App.PrintTitle.Value,
                            HorizontalTextAlignment = TextAlignment.Center,
                            VerticalTextAlignment = TextAlignment.Center,
                            VerticalOptions = LayoutOptions.FillAndExpand,
                            FontAttributes = FontAttributes.Bold,
                            LineBreakMode = LineBreakMode.NoWrap
                        };
                        LabelFontSizes.Add(new KeyValuePair<Label, double>(TitleLabel, 1.4));
                        HeaderGrid.Children.Add(TitleLabel, 1, row);
                        Grid.SetColumnSpan(TitleLabel, 2);
                        row++;
                    }

                    if (!string.IsNullOrEmpty(App.CompanyNameForPrint.Value))
                    {
                        var TitleLabel2 = new MyLabel()
                        {
                            Padding = new Thickness(10, 0),
                            TextColor = BLACK,
                            Text = App.CompanyNameForPrint.Value,
                            HorizontalTextAlignment = TextAlignment.Center,
                            VerticalTextAlignment = TextAlignment.Center,
                            VerticalOptions = LayoutOptions.FillAndExpand,
                            FontAttributes = FontAttributes.Bold,
                            LineBreakMode = LineBreakMode.NoWrap
                        };
                        LabelFontSizes.Add(new KeyValuePair<Label, double>(TitleLabel2, 1.4));
                        HeaderGrid.Children.Add(TitleLabel2, 1, row);
                        Grid.SetColumnSpan(TitleLabel2, 2);
                        row++;
                    }

                    if (!string.IsNullOrEmpty(App.CompanyLogoForPrint.Value))
                    {
                        var CompanyLogo = new Image()
                        {
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            VerticalOptions = LayoutOptions.FillAndExpand,
                            Source = ImageSource.FromFile(Path.Combine(App.imagesDirectory, App.CompanyLogoForPrint.Value))
                        };
                        HeaderGrid.Children.Add(CompanyLogo, 0, 0);
                        if (TitleRow == 2)
                            Grid.SetRowSpan(CompanyLogo, 2);
                    }

                    row = TitleRow;
                    var PreCodeLabel = new MyLabel()
                    {
                        Padding = new Thickness(10, 0),
                        TextColor = BLACK,
                        Text = "شماره:" + (SaleOrder.PreCode.HasValue ? SaleOrder.PreCode.Value.ToString().ToPersianDigits() : "---"),
                        HorizontalTextAlignment = TextAlignment.End,
                        VerticalTextAlignment = TextAlignment.Center,
                        VerticalOptions = LayoutOptions.FillAndExpand,
                        LineBreakMode = LineBreakMode.NoWrap
                    };
                    LabelFontSizes.Add(new KeyValuePair<Label, double>(PreCodeLabel, 1));
                    HeaderGrid.Children.Add(PreCodeLabel, 2, row);
                    Grid.SetColumnSpan(PreCodeLabel, 2);
                    
                    var DateLabel = new MyLabel()
                    {
                        Padding = new Thickness(10, 0),
                        TextColor = BLACK,
                        Text = "تاریخ:" + SaleOrder.InsertDateTime.ToShortStringForDate().ToPersianDigits(),
                        HorizontalTextAlignment = TextAlignment.Start,
                        VerticalTextAlignment = TextAlignment.Center,
                        VerticalOptions = LayoutOptions.FillAndExpand,
                        LineBreakMode = LineBreakMode.NoWrap
                    };
                    LabelFontSizes.Add(new KeyValuePair<Label, double>(DateLabel, 1));
                    HeaderGrid.Children.Add(DateLabel, 0, row);
                    Grid.SetColumnSpan(DateLabel, 2);

                    row++;
                    var TempLabel = new MyLabel()
                    {
                        Padding = new Thickness(10, 0),
                        Text = "",
                        LineBreakMode = LineBreakMode.NoWrap
                    };
                    LabelFontSizes.Add(new KeyValuePair<Label, double>(TempLabel, 0.5));
                    HeaderGrid.Children.Add(TempLabel, 0, row);

                    row++;
                    var SettlementTypeLabel = new MyLabel()
                    {
                        Padding = new Thickness(10, 0),
                        TextColor = BLACK,
                        Text = "تسویه:",
                        HorizontalTextAlignment = TextAlignment.End,
                        VerticalTextAlignment = TextAlignment.Center,
                        LineBreakMode = LineBreakMode.NoWrap
                    };
                    LabelFontSizes.Add(new KeyValuePair<Label, double>(SettlementTypeLabel, 1));
                    HeaderGrid.Children.Add(SettlementTypeLabel, 3, row);
                    var SettlementTypeLabel2 = new MyLabel()
                    {
                        Padding = new Thickness(10, 0),
                        TextColor = BLACK,
                        Text = (SaleOrder.SettlementType.Name + " " + SaleOrder.SettlementDay.ToString() + " روزه").ToPersianDigits(),
                        HorizontalTextAlignment = TextAlignment.End,
                        VerticalTextAlignment = TextAlignment.Center,
                        LineBreakMode = LineBreakMode.NoWrap
                    };
                    LabelFontSizes.Add(new KeyValuePair<Label, double>(SettlementTypeLabel2, 1));
                    HeaderGrid.Children.Add(SettlementTypeLabel2, 0, row);
                    Grid.SetColumnSpan(SettlementTypeLabel2, 3);

                    row++;
                    var PartnerNameLabel = new MyLabel()
                    {
                        Padding = new Thickness(10, 0),
                        TextColor = BLACK,
                        Text = "خریدار:",
                        HorizontalTextAlignment = TextAlignment.End,
                        VerticalTextAlignment = TextAlignment.Center,
                        LineBreakMode = LineBreakMode.NoWrap
                    };
                    LabelFontSizes.Add(new KeyValuePair<Label, double>(PartnerNameLabel, 1));
                    HeaderGrid.Children.Add(PartnerNameLabel, 3, row);
                    var PartnerNameLabel2 = new MyLabel()
                    {
                        Padding = new Thickness(10, 0),
                        TextColor = BLACK,
                        Text = (!string.IsNullOrWhiteSpace(SaleOrder.Partner.LegalName) ? (SaleOrder.Partner.LegalName + (!string.IsNullOrWhiteSpace(SaleOrder.Partner.Name) ? ("(" + SaleOrder.Partner.Name + ")") : "")) : SaleOrder.Partner.Name).ToPersianDigits(),
                        HorizontalTextAlignment = TextAlignment.End,
                        VerticalTextAlignment = TextAlignment.Center,
                        LineBreakMode = LineBreakMode.NoWrap
                    };
                    LabelFontSizes.Add(new KeyValuePair<Label, double>(PartnerNameLabel2, 1));
                    HeaderGrid.Children.Add(PartnerNameLabel2, 0, row);
                    Grid.SetColumnSpan(PartnerNameLabel2, 3);

                    row++;
                    var PartnerAddressLabel = new MyLabel()
                    {
                        Padding = new Thickness(10, 0),
                        TextColor = BLACK,
                        Text = "آدرس:",
                        HorizontalTextAlignment = TextAlignment.End,
                        VerticalTextAlignment = TextAlignment.Center,
                        LineBreakMode = LineBreakMode.NoWrap
                    };
                    LabelFontSizes.Add(new KeyValuePair<Label, double>(PartnerAddressLabel, 1));
                    HeaderGrid.Children.Add(PartnerAddressLabel, 3, row);
                    var PartnerAddressLabel2 = new MyLabel()
                    {
                        Padding = new Thickness(10, 0),
                        TextColor = BLACK,
                        Text = SaleOrder.Partner.Address.ToPersianDigits(),
                        HorizontalTextAlignment = TextAlignment.End,
                        VerticalTextAlignment = TextAlignment.Center,
                        LineBreakMode = LineBreakMode.WordWrap
                    };
                    LabelFontSizes.Add(new KeyValuePair<Label, double>(PartnerAddressLabel2, 1));
                    HeaderGrid.Children.Add(PartnerAddressLabel2, 0, row);
                    Grid.SetColumnSpan(PartnerAddressLabel2, 3);

                    row++;
                    var PartnerPhones = !string.IsNullOrWhiteSpace(SaleOrder.Partner.Phone1) || !string.IsNullOrWhiteSpace(SaleOrder.Partner.Phone2) || !string.IsNullOrWhiteSpace(SaleOrder.Partner.Mobile) ? new string[] { SaleOrder.Partner.Phone1, SaleOrder.Partner.Phone2, SaleOrder.Partner.Mobile }.Where(a => !string.IsNullOrWhiteSpace(a)).Aggregate((sum, x) => sum + "، " + x) : "---";
                    var PartnerPhoneLabel = new MyLabel()
                    {
                        Padding = new Thickness(10, 0),
                        TextColor = BLACK,
                        Text = "تلفن:",
                        HorizontalTextAlignment = TextAlignment.End,
                        VerticalTextAlignment = TextAlignment.Center,
                        LineBreakMode = LineBreakMode.NoWrap
                    };
                    LabelFontSizes.Add(new KeyValuePair<Label, double>(PartnerPhoneLabel, 1));
                    HeaderGrid.Children.Add(PartnerPhoneLabel, 3, row);
                    var PartnerPhoneLabel2 = new MyLabel()
                    {
                        Padding = new Thickness(10, 0),
                        TextColor = BLACK,
                        Text = PartnerPhones.ToPersianDigits(),
                        HorizontalTextAlignment = TextAlignment.End,
                        VerticalTextAlignment = TextAlignment.Center,
                        LineBreakMode = LineBreakMode.NoWrap
                    };
                    LabelFontSizes.Add(new KeyValuePair<Label, double>(PartnerPhoneLabel2, 1));
                    HeaderGrid.Children.Add(PartnerPhoneLabel2, 0, row);
                    Grid.SetColumnSpan(PartnerPhoneLabel2, 3);
                    
                    
                    ArticlesGrid = new Grid()
                    {
                        Padding = 1,
                        RowSpacing = 2.0,
                        ColumnSpacing = 2.0,
                        BackgroundColor = BLACK,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        VerticalOptions = LayoutOptions.StartAndExpand
                    };

                    var _SaleOrderStuffs = SaleOrder.SaleOrderStuffs.Where(a => a.Quantity != 0).ToArray();

                    var RowCount = _SaleOrderStuffs.Length * 2 + SaleOrder.CashDiscounts.Length + (SaleOrder.DiscountAmount != 0 ? 2 : 0) + (SaleOrder.StuffsVATSum != 0 ? 2 : 0) + 5;
                    for (int i = 0; i < RowCount; i++)
                        ArticlesGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

                    var AnyRowDiscount = _SaleOrderStuffs.Any(a => a.DiscountPercent != 0 || a.CashDiscountPercent != 0);
                    var AllRowsAreUnitPackage = _SaleOrderStuffs.All(a => a.Package.Coefficient == 1 && !a.Package.IsTpUnit.GetValueOrDefault(false) && a.Package.Enabled);
                    var AllPackageNames = _SaleOrderStuffs.Select(a => a.Package.Name).Distinct();
                    var pps = _SaleOrderStuffs.Where(a => a.Package.Coefficient != 1 && !a.Package.IsTpUnit.GetValueOrDefault(false));

                    var AllEquivalnetPackageNames = pps.Select(a => a.Package.Stuff.Packages.FirstOrDefault(b => b.Coefficient == 1 && !b.IsTpUnit.GetValueOrDefault(false) && a.Package.Enabled).Name).Distinct();

                    var ColumnCount = 4 +
                         (SaleOrder.StuffsVATSum != 0 ? 1 : 0)  + //for VAT 1401/02/30
                        (AnyRowDiscount ? 2 : 0) +
                        (!AllRowsAreUnitPackage ? 1 : 0);

                    for (int i = 0; i < ColumnCount; i++)
                        ArticlesGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });

                    row = 0;
                    var col = ColumnCount;
                    var ArticlesDescription = new MyLabel()
                    {
                        Padding = new Thickness(10, 0),
                        BackgroundColor = DARKGRAY,
                        TextColor = BLACK,
                        LineBreakMode = LineBreakMode.NoWrap,
                        Text = "مشخصات اقلام سفارش (کلیه مبالغ به ریال می باشد.)",
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        HorizontalTextAlignment = TextAlignment.End
                    };
                    LabelFontSizes.Add(new KeyValuePair<Label, double>(ArticlesDescription, 0.9));
                    ArticlesGrid.Children.Add(ArticlesDescription, 0, row);
                    Grid.SetColumnSpan(ArticlesDescription, col);
                    col--;

                    row++;
                    var ArticlesHeader_RowNumber = new MyLabel()
                    {
                        Padding = new Thickness(10, 0),
                        BackgroundColor = DARKGRAY,
                        TextColor = BLACK,
                        LineBreakMode = LineBreakMode.NoWrap,
                        Text = "#"
                    };
                    ArticlesGrid.Children.Add(ArticlesHeader_RowNumber, col, row);
                    col--;

                    var ArticlesHeader_Quantity = new MyLabel()
                    {
                        Padding = new Thickness(10, 0),
                        BackgroundColor = DARKGRAY,
                        TextColor = BLACK,
                        LineBreakMode = AllPackageNames.Count() == 1 ? LineBreakMode.WordWrap : LineBreakMode.NoWrap,
                        Text = "تعداد" + (AllPackageNames.Count() == 1 ? ("\n(" + AllPackageNames.First() + ")") : ""),
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        HorizontalTextAlignment = TextAlignment.Center,
                        VerticalTextAlignment = TextAlignment.Center,
                        FontAttributes = FontAttributes.Bold
                    };
                    LabelFontSizes.Add(new KeyValuePair<Label, double>(ArticlesHeader_Quantity, 1));
                    ArticlesGrid.Children.Add(ArticlesHeader_Quantity, col, row);
                    col--;

                    if (!AllRowsAreUnitPackage)
                    {
                        var ArticlesHeader_Equivalent = new MyLabel()
                        {
                            Padding = new Thickness(10, 0),
                            BackgroundColor = DARKGRAY,
                            TextColor = BLACK,
                            LineBreakMode = AllEquivalnetPackageNames.Count() == 1 ? LineBreakMode.WordWrap : LineBreakMode.NoWrap,
                            Text = "معادل" + (AllEquivalnetPackageNames.Count() == 1 ? ("\n(" + AllEquivalnetPackageNames.First() + ")") : ""),
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            HorizontalTextAlignment = TextAlignment.Center,
                            VerticalTextAlignment = TextAlignment.Center,
                            FontAttributes = FontAttributes.Bold
                        };
                        LabelFontSizes.Add(new KeyValuePair<Label, double>(ArticlesHeader_Equivalent, 1));
                        ArticlesGrid.Children.Add(ArticlesHeader_Equivalent, col, row);
                        col--;
                    }

                    var ArticlesHeader_Fee = new MyLabel()
                    {
                        Padding = new Thickness(10, 0),
                        BackgroundColor = DARKGRAY,
                        TextColor = BLACK,
                        LineBreakMode = LineBreakMode.NoWrap,
                        Text = "فی",
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        HorizontalTextAlignment = TextAlignment.Center,
                        VerticalTextAlignment = TextAlignment.Center,
                        FontAttributes = FontAttributes.Bold
                    };
                    LabelFontSizes.Add(new KeyValuePair<Label, double>(ArticlesHeader_Fee, 1));
                    ArticlesGrid.Children.Add(ArticlesHeader_Fee, col, row);
                    col--;

                    if (AnyRowDiscount)
                    {
                        var ArticlesHeader_Discount = new MyLabel()
                        {
                            Padding = new Thickness(10, 0),
                            BackgroundColor = DARKGRAY,
                            TextColor = BLACK,
                            LineBreakMode = LineBreakMode.NoWrap,
                            Text = "تخفیف",
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            HorizontalTextAlignment = TextAlignment.Center,
                            VerticalTextAlignment = TextAlignment.Center,
                            FontAttributes = FontAttributes.Bold
                        };
                        LabelFontSizes.Add(new KeyValuePair<Label, double>(ArticlesHeader_Discount, 1));
                        ArticlesGrid.Children.Add(ArticlesHeader_Discount, col, row);
                        col--;
                        var ArticlesHeader_FinalPrice = new MyLabel()
                        {
                            Padding = new Thickness(10, 0),
                            BackgroundColor = DARKGRAY,
                            TextColor = BLACK,
                            LineBreakMode = LineBreakMode.NoWrap,
                            Text = "مبلغ نهایی",
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            HorizontalTextAlignment = TextAlignment.Center,
                            VerticalTextAlignment = TextAlignment.Center,
                            FontAttributes = FontAttributes.Bold
                        };
                        LabelFontSizes.Add(new KeyValuePair<Label, double>(ArticlesHeader_FinalPrice, 1));
                        ArticlesGrid.Children.Add(ArticlesHeader_FinalPrice, col, row);
                    }

                    if (SaleOrder.StuffsVATSum != 0)
                    {
                        var RowVATAmount = new MyLabel()
                        {
                            Padding = new Thickness(10, 0),
                            BackgroundColor = DARKGRAY,
                            TextColor = BLACK,
                            LineBreakMode = LineBreakMode.NoWrap,
                            Text = "م.ا.ا",
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            HorizontalTextAlignment = TextAlignment.Center,
                            VerticalTextAlignment = TextAlignment.Center,
                            FontAttributes = FontAttributes.Bold
                        };
                        LabelFontSizes.Add(new KeyValuePair<Label, double>(RowVATAmount, 1));
                        ArticlesGrid.Children.Add(RowVATAmount, col, row);
                        col--;
                    }

                    var ArticlesHeader_Price = new MyLabel()
                    {
                        Padding = new Thickness(10, 0),
                        BackgroundColor = DARKGRAY,
                        TextColor = BLACK,
                        LineBreakMode = LineBreakMode.NoWrap,
                        Text = "مبلغ",
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        HorizontalTextAlignment = TextAlignment.Center,
                        VerticalTextAlignment = TextAlignment.Center,
                        FontAttributes = FontAttributes.Bold
                    };
                    LabelFontSizes.Add(new KeyValuePair<Label, double>(ArticlesHeader_Price, 1));
                    ArticlesGrid.Children.Add(ArticlesHeader_Price, col, row);
                    col--;

                    

                    row--;
                    var VATExceptions = new List<int>();
                    
                    //_SaleOrderStuffs[0].DiscountPercent = decimal.Parse("1.1234");

                    for (int i = 0; i < _SaleOrderStuffs.Length; i++)
                    {
                        var BG = i % 2 == 1 ? LIGHTGRAY : WHITE;
                        row += 2;
                        col = ColumnCount - 1;
                        var ArticlesBody_RowNumber = new MyLabel()
                        {
                            Padding = new Thickness(10, 0),
                            BackgroundColor = BG,
                            TextColor = BLACK,
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            VerticalOptions = LayoutOptions.FillAndExpand,
                            HorizontalTextAlignment = TextAlignment.Center,
                            VerticalTextAlignment = TextAlignment.Center,
                            LineBreakMode = LineBreakMode.NoWrap,
                            Text = (i + 1).ToString().ToPersianDigits()
                        };
                        LabelFontSizes.Add(new KeyValuePair<Label, double>(ArticlesBody_RowNumber, 1));
                        ArticlesGrid.Children.Add(ArticlesBody_RowNumber, col, row);
                        col--;
                        Grid.SetRowSpan(ArticlesBody_RowNumber, 2);
                        if (i == 0) BodyFirstRowLabels.Add(ArticlesBody_RowNumber);

                        var ArticlesBody_Stuff = new MyLabel()
                        {
                            Padding = new Thickness(10, 0),
                            BackgroundColor = BG,
                            TextColor = BLACK,
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            VerticalOptions = LayoutOptions.FillAndExpand,
                            HorizontalTextAlignment = TextAlignment.End,
                            VerticalTextAlignment = TextAlignment.Center,
                            LineBreakMode = LineBreakMode.WordWrap,
                            Text = (
                                _SaleOrderStuffs[i].Package.Stuff.Name +
                                (_SaleOrderStuffs[i].BatchNumberId.HasValue ? ("\nسری ساخت: " + _SaleOrderStuffs[i].BatchNumber.BatchNumber + "، تاریخ انقضاء: " + (_SaleOrderStuffs[i].BatchNumber.ExpirationDatePresentation == (int)DatePresentation.Shamsi ? _SaleOrderStuffs[i].BatchNumber.ExpirationDate.ToShortStringForDate() : _SaleOrderStuffs[i].BatchNumber.ExpirationDate.ToString("yyyy/MM/dd"))) : "") +
                                (_SaleOrderStuffs[i].StuffSettlementDay.HasValue ? (" (" + _SaleOrderStuffs[i].StuffSettlementDay.Value.ToString() + " روزه)") : "")
                            ).ToPersianDigits()
                        };
                        LabelFontSizes.Add(new KeyValuePair<Label, double>(ArticlesBody_Stuff, 1));
                        ArticlesGrid.Children.Add(ArticlesBody_Stuff, 0, row);
                        Grid.SetColumnSpan(ArticlesBody_Stuff, ColumnCount - 1);

                        var ArticlesBody_Quantity = new MyLabel()
                        {
                            Padding = new Thickness(10, 0),
                            BackgroundColor = BG,
                            TextColor = BLACK,
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            VerticalOptions = LayoutOptions.FillAndExpand,
                            HorizontalTextAlignment = TextAlignment.End,
                            VerticalTextAlignment = TextAlignment.Center,
                            LineBreakMode = LineBreakMode.NoWrap,
                            Text = (_SaleOrderStuffs[i].Quantity.ToString("###,###,###,##0.###").Replace(".", "/") + (AllPackageNames.Count() == 1 ? "" : (" " + _SaleOrderStuffs[i].Package.Name))).ToPersianDigits()
                        };
                        LabelFontSizes.Add(new KeyValuePair<Label, double>(ArticlesBody_Quantity, 1));
                        ArticlesGrid.Children.Add(ArticlesBody_Quantity, col, row + 1);
                        col--;
                        if (i == 0) BodyFirstRowLabels.Add(ArticlesBody_Quantity);

                        if (!AllRowsAreUnitPackage)
                        {
                            var coeff1s = _SaleOrderStuffs[i].Package.Stuff.Packages.Where(a => a.Coefficient == 1).ToList();
                            var us = coeff1s.Select(k => new { Id = k.Id.ToSafeString(), k.Name,k.IsTpUnit }).ToList();

                            var ArticlesBody_Equivalent = new MyLabel()
                            {
                                Padding = new Thickness(10, 0),
                                BackgroundColor = BG,
                                TextColor = BLACK,
                                HorizontalOptions = LayoutOptions.FillAndExpand,
                                VerticalOptions = LayoutOptions.FillAndExpand,
                                HorizontalTextAlignment = TextAlignment.End,
                                VerticalTextAlignment = TextAlignment.Center,
                                LineBreakMode = LineBreakMode.NoWrap,
                                Text = (_SaleOrderStuffs[i].StuffQuantity.ToString("###,###,###,##0.###").Replace(".", "/") + (AllEquivalnetPackageNames.Count() == 1 ? "" : (" " + _SaleOrderStuffs[i].Package.Stuff.Packages.Single(a => a.Coefficient == 1 && !a.IsTpUnit.GetValueOrDefault(false) && a.Enabled).Name))).ToPersianDigits()
                            };
                            LabelFontSizes.Add(new KeyValuePair<Label, double>(ArticlesBody_Equivalent, 1));
                            ArticlesGrid.Children.Add(ArticlesBody_Equivalent, col, row + 1);
                            col--;
                            if (i == 0) BodyFirstRowLabels.Add(ArticlesBody_Equivalent);
                        }

                        var ArticlesBody_Fee = new MyLabel()
                        {
                            Padding = new Thickness(10, 0),
                            BackgroundColor = BG,
                            TextColor = BLACK,
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            VerticalOptions = LayoutOptions.FillAndExpand,
                            HorizontalTextAlignment = TextAlignment.End,
                            VerticalTextAlignment = TextAlignment.Center,
                            LineBreakMode = LineBreakMode.NoWrap,
                            Text = _SaleOrderStuffs[i].FreeProduct ? "اشانتیون" : _SaleOrderStuffs[i].SalePrice.ToString("###,###,###,###,###,##0.").ToPersianDigits()
                        };
                        LabelFontSizes.Add(new KeyValuePair<Label, double>(ArticlesBody_Fee, 1));
                        ArticlesGrid.Children.Add(ArticlesBody_Fee, col, row + 1);
                        col--;
                        if (i == 0) BodyFirstRowLabels.Add(ArticlesBody_Fee);


                        //var VatPercent = new MyLabel()
                        //{
                        //    Padding = new Thickness(10, 0),
                        //    BackgroundColor = BG,
                        //    TextColor = BLACK,
                        //    HorizontalOptions = LayoutOptions.FillAndExpand,
                        //    VerticalOptions = LayoutOptions.FillAndExpand,
                        //    HorizontalTextAlignment = TextAlignment.End,
                        //    VerticalTextAlignment = TextAlignment.Center,
                        //    LineBreakMode = LineBreakMode.NoWrap,
                        //    Text =_SaleOrderStuffs[i].VATPercent.ToString().ToPersianDigits()
                        //};
                        //LabelFontSizes.Add(new KeyValuePair<Label, double>(VatPercent, 1));
                        //ArticlesGrid.Children.Add(VatPercent, col, row + 1);
                        //col--;

                        if(SaleOrder.StuffsVATSum != 0)
                        {
                            var VatAmount = new MyLabel()
                            {
                                Padding = new Thickness(10, 0),
                                BackgroundColor = BG,
                                TextColor = BLACK,
                                HorizontalOptions = LayoutOptions.FillAndExpand,
                                VerticalOptions = LayoutOptions.FillAndExpand,
                                HorizontalTextAlignment = TextAlignment.End,
                                VerticalTextAlignment = TextAlignment.Center,
                                LineBreakMode = LineBreakMode.NoWrap,
                                Text = _SaleOrderStuffs[i].VATAmount.ToString("###,###,###,###,###,###,##0.").ToPersianDigits()
                            };
                            LabelFontSizes.Add(new KeyValuePair<Label, double>(VatAmount, 1));
                            ArticlesGrid.Children.Add(VatAmount, col, row + 1);
                            col--;
                        }

                        
                        var ArticlesBody_Price = new MyLabel()
                        {
                            Padding = new Thickness(10, 0),
                            BackgroundColor = BG,
                            TextColor = BLACK,
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            VerticalOptions = LayoutOptions.FillAndExpand,
                            HorizontalTextAlignment = TextAlignment.End,
                            VerticalTextAlignment = TextAlignment.Center,
                            LineBreakMode = LineBreakMode.NoWrap,
                            Text = _SaleOrderStuffs[i].FreeProduct ? "---" : _SaleOrderStuffs[i].SalePriceQuantity.ToString("###,###,###,###,###,###,##0.").ToPersianDigits()
                        };
                        LabelFontSizes.Add(new KeyValuePair<Label, double>(ArticlesBody_Price, 1));
                        ArticlesGrid.Children.Add(ArticlesBody_Price, col, row + 1);
                        col--;
                        if (i == 0) BodyFirstRowLabels.Add(ArticlesBody_Price);

                        if (AnyRowDiscount)
                        {
                            var ArticlesBody_Discount = new MyLabel()
                            {
                                Padding = new Thickness(10, 0),
                                BackgroundColor = BG,
                                TextColor = BLACK,
                                HorizontalOptions = LayoutOptions.FillAndExpand,
                                VerticalOptions = LayoutOptions.FillAndExpand,
                                HorizontalTextAlignment = TextAlignment.End,
                                VerticalTextAlignment = TextAlignment.Center,
                                LineBreakMode = LineBreakMode.WordWrap,
                                Text = _SaleOrderStuffs[i].FreeProduct ? "---" : (_SaleOrderStuffs[i].DiscountAmount + _SaleOrderStuffs[i].CashDiscountAmount != 0 ? (_SaleOrderStuffs[i].DiscountAmount + _SaleOrderStuffs[i].CashDiscountAmount).ToString("###,###,###,###,###,###,##0.") + "\n" + (_SaleOrderStuffs[i].DiscountPercent != 0 && _SaleOrderStuffs[i].CashDiscountPercent != 0 ? "(" : "") + (new decimal[] { _SaleOrderStuffs[i].DiscountPercent , _SaleOrderStuffs[i].CashDiscountPercent }.Where(a => a != 0).Select(a => a.ToString("##0.####").Replace(".", "/")).Aggregate((sum, x) => sum + " + " + x)) + (_SaleOrderStuffs[i].DiscountPercent != 0 && _SaleOrderStuffs[i].CashDiscountPercent != 0 ? ")" : "") + " %" : "0").ToPersianDigits()
                            };
                            LabelFontSizes.Add(new KeyValuePair<Label, double>(ArticlesBody_Discount, 1));
                            ArticlesGrid.Children.Add(ArticlesBody_Discount, col, row + 1);
                            col--;
                            if (i == 0) BodyFirstRowLabels.Add(ArticlesBody_Discount);

                            var ArticlesBody_FinalPrice = new MyLabel()
                            {
                                Padding = new Thickness(10, 0),
                                BackgroundColor = BG,
                                TextColor = BLACK,
                                HorizontalOptions = LayoutOptions.FillAndExpand,
                                VerticalOptions = LayoutOptions.FillAndExpand,
                                HorizontalTextAlignment = TextAlignment.End,
                                VerticalTextAlignment = TextAlignment.Center,
                                LineBreakMode = LineBreakMode.NoWrap,
                                Text = _SaleOrderStuffs[i].FreeProduct ? "---" : _SaleOrderStuffs[i].FinalPrice.ToString("###,###,###,###,###,###,##0.").ToPersianDigits()
                            };
                            LabelFontSizes.Add(new KeyValuePair<Label, double>(ArticlesBody_FinalPrice, 1));
                            ArticlesGrid.Children.Add(ArticlesBody_FinalPrice, col, row + 1);
                            col--;
                            if (i == 0) BodyFirstRowLabels.Add(ArticlesBody_FinalPrice);
                        }

                        if (SaleOrder.StuffsVATSum != 0 && !_SaleOrderStuffs[i].FreeProduct && _SaleOrderStuffs[i].VATPercent == 0)
                            VATExceptions.Add(i + 1);
                    }

                    row += 2;
                    col = ColumnCount - 3 - (AllRowsAreUnitPackage ? 0 : 1) - (SaleOrder.StuffsVATSum == 0 ? 0 : 1);
                    var ArticlesFooter_Sum = new MyLabel()
                    {
                        Padding = new Thickness(10, 0),
                        BackgroundColor = DARKGRAY,
                        TextColor = BLACK,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        LineBreakMode = LineBreakMode.TailTruncation,
                        Text = "جمع:",
                        HorizontalTextAlignment = TextAlignment.End,
                        VerticalTextAlignment = TextAlignment.Center,
                        FontAttributes = FontAttributes.Bold
                    };
                    LabelFontSizes.Add(new KeyValuePair<Label, double>(ArticlesFooter_Sum, 1));
                    ArticlesGrid.Children.Add(ArticlesFooter_Sum, col, row);
                    col--;
                    Grid.SetColumnSpan(ArticlesFooter_Sum, 3 + (AllRowsAreUnitPackage ? 0 : 1));

                    var ThisIsFinalPrice = !AnyRowDiscount && SaleOrder.DiscountAmount == 0 && SaleOrder.StuffsVATSum == 0;
                    var ArticlesFooter_Price = new MyLabel()
                    {
                        Padding = new Thickness(10, 0),
                        BackgroundColor = ThisIsFinalPrice ? BLACK : DARKGRAY,
                        TextColor = ThisIsFinalPrice ? WHITE : BLACK,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        LineBreakMode = LineBreakMode.TailTruncation,
                        Text = SaleOrder.StuffsPriceSum.ToString("###,###,###,###,###,###,##0.").ToPersianDigits(),
                        HorizontalTextAlignment = TextAlignment.End,
                        VerticalTextAlignment = TextAlignment.Center,
                        FontAttributes = FontAttributes.Bold
                    };
                    LabelFontSizes.Add(new KeyValuePair<Label, double>(ArticlesFooter_Price, 1));
                    ArticlesGrid.Children.Add(ArticlesFooter_Price, col, row);
                    col--;

                    if (AnyRowDiscount)
                    {
                        var ArticlesFooter_Discount = new MyLabel()
                        {
                            Padding = new Thickness(10, 0),
                            BackgroundColor = DARKGRAY,
                            TextColor = BLACK,
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            LineBreakMode = LineBreakMode.TailTruncation,
                            Text = SaleOrder.StuffsRowDiscountSum.ToString("###,###,###,###,###,###,##0.").ToPersianDigits(),
                            HorizontalTextAlignment = TextAlignment.End,
                            VerticalTextAlignment = TextAlignment.Center,
                            FontAttributes = FontAttributes.Bold
                        };
                        LabelFontSizes.Add(new KeyValuePair<Label, double>(ArticlesFooter_Discount, 1));
                        ArticlesGrid.Children.Add(ArticlesFooter_Discount, col, row);
                        col--;

                        ThisIsFinalPrice = SaleOrder.DiscountAmount == 0 && SaleOrder.StuffsVATSum == 0;
                        var ArticlesFooter_FinalPrice = new MyLabel()
                        {
                            Padding = new Thickness(10, 0),
                            BackgroundColor = ThisIsFinalPrice ? BLACK : DARKGRAY,
                            TextColor = ThisIsFinalPrice ? WHITE : BLACK,
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            LineBreakMode = LineBreakMode.TailTruncation,
                            Text = SaleOrder.OrderFinalPrice.ToString("###,###,###,###,###,###,##0.").ToPersianDigits(),
                            HorizontalTextAlignment = TextAlignment.End,
                            VerticalTextAlignment = TextAlignment.Center,
                            FontAttributes = FontAttributes.Bold
                        };
                        LabelFontSizes.Add(new KeyValuePair<Label, double>(ArticlesFooter_FinalPrice, 1));
                        ArticlesGrid.Children.Add(ArticlesFooter_FinalPrice, col, row);
                        col--;
                    }

                    if (SaleOrder.DiscountAmount != 0)
                    {
                        row++;
                        var SaleOrder_DiscountAmount = new MyLabel()
                        {
                            Padding = new Thickness(10, 0),
                            BackgroundColor = DARKGRAY,
                            TextColor = BLACK,
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            LineBreakMode = LineBreakMode.WordWrap,
                            Text = SaleOrder.DiscountAmount.ToString("###,###,###,###,###,###,###,##0.").ToPersianDigits(),
                            HorizontalTextAlignment = TextAlignment.End,
                            VerticalTextAlignment = TextAlignment.Center,
                            FontAttributes = FontAttributes.Bold
                        };
                        LabelFontSizes.Add(new KeyValuePair<Label, double>(SaleOrder_DiscountAmount, 1));
                        ArticlesGrid.Children.Add(SaleOrder_DiscountAmount, 0, row);

                        var SaleOrder_DiscountAmount_Label = new MyLabel()
                        {
                            Padding = new Thickness(10, 0),
                            BackgroundColor = DARKGRAY,
                            TextColor = BLACK,
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            LineBreakMode = LineBreakMode.WordWrap,
                            Text = "تخفیف حجمی(" + SaleOrder.DiscountPercent.ToString("##0.##").Replace(".", "/").ToPersianDigits() + "%):",
                            HorizontalTextAlignment = TextAlignment.Start,
                            VerticalTextAlignment = TextAlignment.Center,
                            FontAttributes = FontAttributes.Bold
                        };
                        LabelFontSizes.Add(new KeyValuePair<Label, double>(SaleOrder_DiscountAmount_Label, 1));
                        ArticlesGrid.Children.Add(SaleOrder_DiscountAmount_Label, 1, row);
                        Grid.SetColumnSpan(SaleOrder_DiscountAmount_Label, ColumnCount - 1);

                        ThisIsFinalPrice = SaleOrder.StuffsVATSum == 0;
                        row++;
                        var SaleOrder_AfterDiscountAmount = new MyLabel()
                        {
                            Padding = new Thickness(10, 0),
                            BackgroundColor = ThisIsFinalPrice ? BLACK : DARKGRAY,
                            TextColor = ThisIsFinalPrice ? WHITE : BLACK,
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            LineBreakMode = LineBreakMode.WordWrap,
                            Text = (SaleOrder.StuffsPriceSum - SaleOrder.StuffsRowDiscountSum - SaleOrder.DiscountAmount).ToString("###,###,###,###,###,###,###,##0.").ToPersianDigits(),
                            HorizontalTextAlignment = TextAlignment.End,
                            VerticalTextAlignment = TextAlignment.Center,
                            FontAttributes = FontAttributes.Bold
                        };
                        LabelFontSizes.Add(new KeyValuePair<Label, double>(SaleOrder_AfterDiscountAmount, 1));
                        ArticlesGrid.Children.Add(SaleOrder_AfterDiscountAmount, 0, row);

                        var SaleOrder_AfterDiscountAmount_Label = new MyLabel()
                        {
                            Padding = new Thickness(10, 0),
                            BackgroundColor = DARKGRAY,
                            TextColor = BLACK,
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            LineBreakMode = LineBreakMode.WordWrap,
                            Text = SaleOrder.StuffsVATSum != 0 ? "پس از کسر تخفیف حجمی:" : "مبلغ نهایی سفارش:",
                            HorizontalTextAlignment = TextAlignment.Start,
                            VerticalTextAlignment = TextAlignment.Center,
                            FontAttributes = FontAttributes.Bold
                        };
                        LabelFontSizes.Add(new KeyValuePair<Label, double>(SaleOrder_AfterDiscountAmount_Label, 1));
                        ArticlesGrid.Children.Add(SaleOrder_AfterDiscountAmount_Label, 1, row);
                        Grid.SetColumnSpan(SaleOrder_AfterDiscountAmount_Label, ColumnCount - 1);
                    }

                    if (SaleOrder.StuffsVATSum != 0)
                    {
                        row++;
                        var SaleOrder_VATAmount = new MyLabel()
                        {
                            Padding = new Thickness(10, 0),
                            BackgroundColor = DARKGRAY,
                            TextColor = BLACK,
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            LineBreakMode = LineBreakMode.WordWrap,
                            Text = SaleOrder.StuffsVATSum.ToString("###,###,###,###,###,###,###,##0.").ToPersianDigits(),
                            HorizontalTextAlignment = TextAlignment.End,
                            VerticalTextAlignment = TextAlignment.Center,
                            FontAttributes = FontAttributes.Bold
                        };
                        LabelFontSizes.Add(new KeyValuePair<Label, double>(SaleOrder_VATAmount, 1));
                        ArticlesGrid.Children.Add(SaleOrder_VATAmount, 0, row);

                        var SaleOrder_VATAmount_Label = new MyLabel()
                        {
                            Padding = new Thickness(10, 0),
                            BackgroundColor = DARKGRAY,
                            TextColor = BLACK,
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            LineBreakMode = LineBreakMode.WordWrap,

                            //1401/02/30 now it has different amount of vats
                            //Text = "مالیات ا.ا.(" + App.VATPercent.Value.ToString("##0.##").Replace(".", "/").ToPersianDigits() + "%)" + (VATExceptions.Any() ? "*" : "") + ":",
                            
                            Text = "مالیات ا.ا." + (VATExceptions.Any() ? "*" : "") + ":",
                            HorizontalTextAlignment = TextAlignment.Start,
                            VerticalTextAlignment = TextAlignment.Center,
                            FontAttributes = FontAttributes.Bold
                        };

                        LabelFontSizes.Add(new KeyValuePair<Label, double>(SaleOrder_VATAmount_Label, 1));
                        ArticlesGrid.Children.Add(SaleOrder_VATAmount_Label, 1, row);

                        Grid.SetColumnSpan(SaleOrder_VATAmount_Label, ColumnCount - 1);

                        row++;
                        var SaleOrder_AfterVATAmount = new MyLabel()
                        {
                            Padding = new Thickness(10, 0),
                            BackgroundColor = BLACK,
                            TextColor = WHITE,
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            LineBreakMode = LineBreakMode.WordWrap,
                            Text = SaleOrder.OrderFinalPrice.ToString("###,###,###,###,###,###,###,##0.").ToPersianDigits(),
                            HorizontalTextAlignment = TextAlignment.End,
                            VerticalTextAlignment = TextAlignment.Center,
                            FontAttributes = FontAttributes.Bold
                        };
                        LabelFontSizes.Add(new KeyValuePair<Label, double>(SaleOrder_AfterVATAmount, 1));
                        ArticlesGrid.Children.Add(SaleOrder_AfterVATAmount, 0, row);

                        var SaleOrder_AfterVATAmount_Label = new MyLabel()
                        {
                            Padding = new Thickness(10, 0),
                            BackgroundColor = DARKGRAY,
                            TextColor = BLACK,
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            LineBreakMode = LineBreakMode.WordWrap,
                            Text = "مبلغ نهایی سفارش:",
                            HorizontalTextAlignment = TextAlignment.Start,
                            VerticalTextAlignment = TextAlignment.Center,
                            FontAttributes = FontAttributes.Bold
                        };
                        LabelFontSizes.Add(new KeyValuePair<Label, double>(SaleOrder_AfterVATAmount_Label, 1));
                        ArticlesGrid.Children.Add(SaleOrder_AfterVATAmount_Label, 1, row);
                        Grid.SetColumnSpan(SaleOrder_AfterVATAmount_Label, ColumnCount - 1);
                    }

                    row++;
                    var FooterPriceInLetters = new MyLabel()
                    {
                        Padding = new Thickness(10, 0),
                        BackgroundColor = DARKGRAY,
                        TextColor = BLACK,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        LineBreakMode = LineBreakMode.WordWrap,
                        Text = SaleOrder.OrderFinalPrice.ToLitteralText().ToPersianDigits(),
                        HorizontalTextAlignment = TextAlignment.End,
                        VerticalTextAlignment = TextAlignment.Center,
                        FontAttributes = FontAttributes.Bold
                    };
                    LabelFontSizes.Add(new KeyValuePair<Label, double>(FooterPriceInLetters, 1));
                    ArticlesGrid.Children.Add(FooterPriceInLetters, 0, row);
                    Grid.SetColumnSpan(FooterPriceInLetters, ColumnCount);

                    for (int i = 0; i < SaleOrder.CashDiscounts.Length; i++)
                    {
                        row++;
                        var Step = SaleOrder.CashDiscounts[i];
                        var StepStr = ("در صورت تسویه " + Step.Day + " روزه، این سفارش مشمول تخفیف نقدی " + Step.Percent.ToString("##0.##").Replace(".", "/") + "% معادل " + Step.DiscountAmount.ToString("###,###,###,###,###,###,###,##0.") + " ریال شده و مبلغ قابل پرداخت " + Step.OrderFinalPrice.ToString("###,###,###,###,###,###,###,##0.") + " ریال خواهد بود.").ToPersianDigits();
                        var FooterCashDiscountStep = new MyLabel()
                        {
                            Padding = new Thickness(10, 0),
                            BackgroundColor = DARKGRAY,
                            TextColor = BLACK,
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            LineBreakMode = LineBreakMode.WordWrap,
                            Text = StepStr,
                            HorizontalTextAlignment = TextAlignment.End,
                            VerticalTextAlignment = TextAlignment.Center,
                            FontAttributes = FontAttributes.Bold
                        };
                        LabelFontSizes.Add(new KeyValuePair<Label, double>(FooterCashDiscountStep, 1));
                        ArticlesGrid.Children.Add(FooterCashDiscountStep, 0, row);
                        Grid.SetColumnSpan(FooterCashDiscountStep, ColumnCount);
                    }

                    var result = new StackLayout()
                    {
                        Orientation = StackOrientation.Vertical,
                        Spacing = 0,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        BackgroundColor = Color.FromHex("111"),
                        Children =
                        {
                        HeaderGrid,
                        ArticlesGrid
                        }
                    };

                    var Descriptions = new StackLayout()
                    {
                        Spacing = 3,
                        Padding = 0,
                        BackgroundColor = Color.White,
                        Orientation = StackOrientation.Vertical
                    };
                    var FooterDescription = new MyLabel()
                    {
                        Padding = new Thickness(10, 0),
                        BackgroundColor = WHITE,
                        TextColor = BLACK,
                        Text = (VATExceptions.Any() ? ("*سطر" + (VATExceptions.Count == 1 ? "" : "های") + " " + VATExceptions.Select((a, index) => (index == VATExceptions.Count - 1 ? "|||" : "") + a.ToString()).Aggregate((sum, x) => sum + "، " + x).Replace("، |||", " و ").Replace("|||", "").ToPersianDigits() + " مشمول مالیات نمی شود.") : "") +
                            "\nتوضیحات: " + (!string.IsNullOrWhiteSpace(SaleOrder.Description) ? SaleOrder.Description.ToPersianDigits() : "---"),
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        HorizontalTextAlignment = TextAlignment.End,
                        VerticalTextAlignment = TextAlignment.Start
                    };
                    LabelFontSizes.Add(new KeyValuePair<Label, double>(FooterDescription, 1));
                    Descriptions.Children.Add(FooterDescription);

                    if (!string.IsNullOrEmpty(App.EndOfPrintDescription.Value))
                    {
                        var EndOfPrintDescriptionLabel = new MyLabel()
                        {
                            Padding = new Thickness(10, 0),
                            BackgroundColor = WHITE,
                            TextColor = BLACK,
                            Text = App.EndOfPrintDescription.Value,
                            HorizontalOptions = LayoutOptions.FillAndExpand,
                            HorizontalTextAlignment = TextAlignment.End,
                            VerticalTextAlignment = TextAlignment.Start
                        };
                        LabelFontSizes.Add(new KeyValuePair<Label, double>(EndOfPrintDescriptionLabel, 1));
                        Descriptions.Children.Add(EndOfPrintDescriptionLabel);
                    }

                    result.Children.Add(Descriptions);

                    return result;
                });

                foreach (var item in LabelFontSizes)
                    item.Key.FontSize = item.Value * _FontSizeForView;

                PrintPreview.Content = Content;

                await Task.Delay(1000);

                var ColumnsWidth = BodyFirstRowLabels.Select((a, index) => new
                {
                    index,
                    Width = a.Width
                }).ToArray();

                for (int i = 0; i < ColumnsWidth.Length; i++)
                {
                    var Width = ColumnsWidth[ColumnsWidth.Length - 1 - i].Width;
                    Width = Width >= 1 ? Width : 1;
                    ((Grid)(((StackLayout)PrintPreview.Content).Children[1])).ColumnDefinitions[i].Width = new GridLength(Width, GridUnitType.Star);
                }
            }
            catch (Exception err)
            {
                App.ShowError("خطا", err.ProperMessage(), "خوب");
            }

            WaitToggle(false);
        }

        public async void SubmitSaleOrderToServer(object sender, EventArgs e)
        {
            WaitToggle(false);
            var SaveResult = await SaveSaleOrder();
            if (!SaveResult.Success)
            {
                WaitToggle(false);
                App.ShowError("خطا", SaveResult.Message, "خوب");
            }
            else
            {
                var submitResult = await Connectivity.SubmitSaleOrdersAsync(new SaleOrder[] { SaveResult.Data });
                if (!submitResult.Success)
                {
                    WaitToggle(false);
                    App.ShowError("خطا", "سفارش به صورت محلی ثبت شد اما در ارسال اطلاعات به سرور خطایی رخ داده است: " + submitResult.Message, "خوب");
                }
                else
                {
                    WaitToggle(true);
                    App.ToastMessageHandler.ShowMessage("سفارش با موفقیت به سرور ارسال شد.", Helpers.ToastMessageDuration.Long);
                    var NavigationStackCount = Navigation.NavigationStack.Count() - (PartnerListForm != null || OrdersForm != null ? 1 : 0) - 1;
                    for (int i = 0; i < NavigationStackCount; i++)
                        try { Navigation.PopAsync(); } catch (Exception) { }
                }
                if (OrdersForm != null)
                    OrdersForm.FilterChanged(null, null);
                if (PartnerListForm != null)
                    PartnerListForm.FilterChanged(null, null);
            }
        }

        public async void SubmitSaleOrderToStorage(object sender, EventArgs e)
        {
            WaitToggle(false);
            var SaveResult = await SaveSaleOrder();
            if (!SaveResult.Success)
            {
                WaitToggle(false);
                App.ShowError("خطا", SaveResult.Message, "خوب");
            }
            else
            {
                WaitToggle(true);
                App.ToastMessageHandler.ShowMessage("سفارش با موفقیت به صورت محلی ثبت شد.", Helpers.ToastMessageDuration.Long);
                var NavigationStackCount = Navigation.NavigationStack.Count() - (PartnerListForm != null || OrdersForm != null ? 1 : 0) - 1;
                for (int i = 0; i < NavigationStackCount; i++)
                    try { await Navigation.PopAsync(); } catch (Exception) { }
                if (OrdersForm != null)
                    OrdersForm.FilterChanged(null, null);
                if (PartnerListForm != null)
                    PartnerListForm.FilterChanged(null, null);
            }
        }

        public void WaitToggle(bool FormWorkFinished)
        {
            if (!BusyIndicatorContainder.IsVisible)
            {
                BusyIndicatorContainder.IsVisible = true;
                this.ToolbarItems.Remove(ToolbarItem_LocalSave);
                this.ToolbarItems.Remove(ToolbarItem_SendToServer);
                this.ToolbarItems.Remove(ToolbarItem_AddDiscounts);
                this.ToolbarItems.Remove(ToolbarItem_AddFreeProducts);
            }
            else
            {
                BusyIndicatorContainder.IsVisible = false;
                if (!FormWorkFinished)
                {
                    if (SaveOption)
                    {
                        if (App.Accesses.AddDiscounts)
                            this.ToolbarItems.Add(ToolbarItem_AddDiscounts);
                        if (App.Accesses.AddFreeProducts)
                            this.ToolbarItems.Add(ToolbarItem_AddFreeProducts);
                        this.ToolbarItems.Add(ToolbarItem_LocalSave);
                        this.ToolbarItems.Add(ToolbarItem_SendToServer);
                    }
                }
            }
        }

        private async Task<ResultSuccess<SaleOrder>> SaveSaleOrder()
        {
            if (!App.GpsEnabled)
                return new ResultSuccess<SaleOrder>(false, "لطفا مکان یاب را فعال نمایید");


            //جلوگیری از ثبت=2 هشدار=1 هیچکدام=0
            if (App.WarnIfSalePriceIsLessThanTheLastBuyPrice.Value != 0)
            {
                var lastBuyPrices = await Kara.Assets.Connectivity.GetLastBuyPrice(SaleOrder.SaleOrderStuffs.Select(a => a.Id).ToList());

                string resultMessage = "";
                int rowNumber = 1;

                foreach (SaleOrderStuff item in SaleOrder.SaleOrderStuffs)
                {
                    if (item.SalePrice < lastBuyPrices.FirstOrDefault(a => a.Key == item.Id).Value)
                    {
                        resultMessage += "فی فروش کمتر از آخرین فی خرید، سطر " + rowNumber + Environment.NewLine;
                    }

                    rowNumber++;
                }

                if (resultMessage!="" && App.WarnIfSalePriceIsLessThanTheLastBuyPrice.Value == 2)
                    return new ResultSuccess<SaleOrder>(false, resultMessage);

                if (resultMessage != "" &&  App.WarnIfSalePriceIsLessThanTheLastBuyPrice.Value == 1)
                {
                    var answer = await DisplayActionSheet(resultMessage, "بستن","", "ادامه", "انصراف");
                    if(answer=="انصراف")
                        return new ResultSuccess<SaleOrder>(false, "ثبت فاکتور کنسل شد");
                }
            }

            var result = await App.SaveSaleOrder(SaleOrder);

            if(result.Success)
            {
                if (OrderInsertForm != null)
                    OrderInsertForm.EditingSaleOrder = SaleOrder;
                if (OrderBeforePreviewForm != null)
                    OrderBeforePreviewForm.EditingSaleOrderId = SaleOrder.Id;
            }
            return result;
        }

        public async Task<ResultSuccess> CaclculateDiscounts(List<KeyValuePair<Guid, RuleModel>> userSelectedOptionalDiscounts)
        {
            return await App.CalculateDiscounts(SaleOrder, OrderBeforePreviewForm.AllStuffsData.ToDictionary(a => a.StuffId, a => a._UnitPrice.GetValueOrDefault(0)), userSelectedOptionalDiscounts);
        }

        public async Task<ResultSuccess> CaclculateVATs()
        {
            return await Task.Factory.StartNew(() =>
            {
                try
                {
                    var VATPercent = App.VATPercent.Value;
                    var CalculateVATForThisPerson = SaleOrder.Partner.CalculateVATForThisPerson;
                    foreach (var item in SaleOrder.SaleOrderStuffs)
                        //Arash 1401/02/30
                        //item.VATPercent = CalculateVATForThisPerson && item.Package.Stuff.HasVAT ? VATPercent : 0;
                        item.VATPercent = CalculateVATForThisPerson && item.Package.Stuff.EnableStuffTaxValue ? item.Package.Stuff.StuffTaxValue :  item.Package.Stuff.HasVAT ? VATPercent : 0;

                    return new ResultSuccess();
                }
                catch (Exception err)
                {
                    return new ResultSuccess(false, "خطایی در محاسبه مالیات رخ داده است. لطفا با پشتیبانی نرم افزار تماس بگیرید.\n" + err.ProperMessage());
                }

            });

        }

        public async Task<ResultSuccess> CheckPossibleQuantityCoefficients()
        {
            return await Task.Factory.StartNew(() =>
            {
                try
                {
                    foreach (var item in SaleOrder.SaleOrderStuffs)
                        if (item.Quantity % item.Package.PossibleQuantityCoefficient != 0)
                            item.Quantity = item.Package.PossibleQuantityCoefficient * Math.Round(item.Quantity / item.Package.PossibleQuantityCoefficient, MidpointRounding.AwayFromZero);

                    return new ResultSuccess();
                }
                catch (Exception err)
                {
                    return new ResultSuccess(false, "خطایی در محاسبه مالیات رخ داده است. لطفا با پشتیبانی نرم افزار تماس بگیرید.\n" + err.ProperMessage());
                }

            });

        }
    }
}
