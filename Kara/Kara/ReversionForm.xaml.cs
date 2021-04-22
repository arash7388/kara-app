using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kara.Models;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using Kara.CustomRenderer;
using Kara.Helpers;
using CarouselView.FormsPlugin.Abstractions;
using FFImageLoading.Forms;
using XLabs.Forms.Controls;
using static Kara.Assets.Connectivity;
using Kara.Assets;

namespace Kara
{
    //public class ReversionCustomStuffListCell : CustomStuffListCell
    //{
    //    //public ReversionCustomStuffListCell()
    //    //{
    //    //    this.SetBinding(CustomStuffListCell.IdProperty, "Id");

    //    //    //var ShowConsumerPrice = App.ShowConsumerPrice.Value;
    //    //    //var ShowConsumerPrice = false;
    //    //    //Grid GridWrapper = new Grid()
    //    //    //{
    //    //    //    Padding = new Thickness(5, 0),
    //    //    //    RowSpacing = 1,
    //    //    //    ColumnSpacing = 1,
    //    //    //    HorizontalOptions = LayoutOptions.FillAndExpand
    //    //    //};
    //    //    //GridWrapper.SetBinding(Grid.BackgroundColorProperty, "RowColor");
    //    //    //var GroupRow1 = new RowDefinition() { };
    //    //    //GroupRow1.SetBinding(RowDefinition.HeightProperty, "ListGroupRow1Height");
    //    //    //var GroupRow2 = new RowDefinition() { };
    //    //    //GroupRow2.SetBinding(RowDefinition.HeightProperty, "ListGroupRow2Height");
    //    //    //var StuffRow1 = new RowDefinition() { };
    //    //    //StuffRow1.SetBinding(RowDefinition.HeightProperty, "ListStuffRowHeight");
    //    //    //var StuffRow2 = new RowDefinition() { };
    //    //    //StuffRow2.SetBinding(RowDefinition.HeightProperty, "ListStuffRowHeight");
    //    //    //var BatchNumberRow = new RowDefinition() { };
    //    //    //BatchNumberRow.SetBinding(RowDefinition.HeightProperty, "BatchNumberRowHeight");
    //    //    //GridWrapper.RowDefinitions = new RowDefinitionCollection() { GroupRow1, GroupRow2, StuffRow1, StuffRow2, BatchNumberRow };

    //    //    //GridWrapper.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(3, GridUnitType.Star) });
    //    //    //GridWrapper.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });
    //    //    //GridWrapper.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            
    //    //    //if (ShowConsumerPrice)
    //    //    //    GridWrapper.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(3, GridUnitType.Star) });

    //    //    //GridWrapper.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
    //    //    //GridWrapper.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });
    //    //    //GridWrapper.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
    //    //    //GridWrapper.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });

    //    //    //Label GroupLabel = null, GroupPriceSumLabel = null, CodeLabel = null, NameLabel = null, UnitNameLabel = null, StockLabel = null, ConsumerFeeLabel = null, PriceLabel = null, QuantityPlusLabel = null, QuantityMinusLabel = null, QuantityEntry = null;
    //    //    //Entry FeeText;
    //    //    //Label FeeLabel;
    //    //    //Label BatchNumberLabel = null, ExpirationDateLabel = null, BatchNumberStockLabel = null, BatchNumberQuantityPlusLabel = null, BatchNumberQuantityMinusLabel = null, BatchNumberQuantityEntry = null;
    //    //    //Image GroupButtonImage = null;

    //    //    //GroupLabel = new Label() { LineBreakMode = LineBreakMode.NoWrap, TextColor = Color.FromHex("222"), HorizontalOptions = LayoutOptions.End, HorizontalTextAlignment = TextAlignment.End, VerticalOptions = LayoutOptions.Center, FontSize = 16 + 2 ,BackgroundColor=Color.Red};
    //    //    //GroupButtonImage = new Image() { WidthRequest = 15, HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Center ,BackgroundColor=Color.Yellow};
    //    //    //GroupPriceSumLabel = new Label() { LineBreakMode = LineBreakMode.TailTruncation, TextColor = Color.FromHex("1845A5"), HorizontalOptions = LayoutOptions.End, HorizontalTextAlignment = TextAlignment.End, VerticalOptions = LayoutOptions.Center, FontSize = 13,BackgroundColor=Color.Blue };
    //    //    //CodeLabel = new Label() { LineBreakMode = LineBreakMode.HeadTruncation, TextColor = Color.FromHex("222"), HorizontalOptions = LayoutOptions.End, HorizontalTextAlignment = TextAlignment.End, VerticalOptions = LayoutOptions.Center, FontSize = 16,BackgroundColor=Color.Cyan };
    //    //    //NameLabel = new Label() { LineBreakMode = LineBreakMode.TailTruncation, TextColor = Color.FromHex("222"), HorizontalOptions = LayoutOptions.End, HorizontalTextAlignment = TextAlignment.End, VerticalOptions = LayoutOptions.Center, FontSize = 16 , BackgroundColor = Color.Purple };
    //    //    //UnitNameLabel = new FullRoundedLabel() { LineBreakMode = LineBreakMode.TailTruncation, TextColor = Color.FromHex("222"), HorizontalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.Center, VerticalOptions = LayoutOptions.Center, VerticalTextAlignment = TextAlignment.Center, FontSize = 16, Margin = 0,BackgroundColor = Color.Gray };
    //    //    //StockLabel = new Label() { LineBreakMode = LineBreakMode.TailTruncation, TextColor = Color.FromHex("222"), HorizontalOptions = LayoutOptions.End, HorizontalTextAlignment = TextAlignment.End, VerticalOptions = LayoutOptions.Center, FontSize = 16 };
            
    //    //    //FeeLabel = new Label() {Text="فی:", LineBreakMode = LineBreakMode.TailTruncation, TextColor = Color.FromHex("222"), HorizontalOptions = LayoutOptions.Start, HorizontalTextAlignment = TextAlignment.Start, VerticalOptions = LayoutOptions.Center, FontSize = 16 , BackgroundColor = Color.Cornsilk };
    //    //    //FeeText = new Entry() { TextColor = Color.FromHex("222"), HorizontalOptions = LayoutOptions.Start, HorizontalTextAlignment = TextAlignment.End, VerticalOptions = LayoutOptions.Center, FontSize = 16 , BackgroundColor = Color.Aqua };
    //    //    //ConsumerFeeLabel = new Label() { LineBreakMode = LineBreakMode.TailTruncation, TextColor = Color.FromHex("1845A5"), HorizontalOptions = LayoutOptions.StartAndExpand, HorizontalTextAlignment = TextAlignment.Start, VerticalOptions = LayoutOptions.Center, FontSize = 16 };
    //    //    //PriceLabel = new Label() { LineBreakMode = LineBreakMode.TailTruncation, TextColor = Color.FromHex("1845A5"), HorizontalOptions = LayoutOptions.Start, HorizontalTextAlignment = TextAlignment.Start, VerticalOptions = LayoutOptions.Center, FontSize = 16, BackgroundColor = Color.DeepPink };
    //    //    //QuantityPlusLabel = new RightEntryCompanionLabel() { LineBreakMode = LineBreakMode.TailTruncation, Text = "+", TextColor = Color.FromHex("222"), HorizontalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.Center, VerticalOptions = LayoutOptions.Center, VerticalTextAlignment = TextAlignment.Center, FontSize = 16, Margin = 1 };
    //    //    //QuantityMinusLabel = new LeftEntryCompanionLabel() { LineBreakMode = LineBreakMode.TailTruncation, Text = "-", TextColor = Color.FromHex("222"), HorizontalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.Center, VerticalOptions = LayoutOptions.Center, VerticalTextAlignment = TextAlignment.Center, FontSize = 16, Margin = 1 };
    //    //    //QuantityEntry = new MyLabel() { LineBreakMode = LineBreakMode.TailTruncation, TextColor = Color.FromHex("222"), HorizontalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.Center, VerticalOptions = LayoutOptions.Center, VerticalTextAlignment = TextAlignment.Center, FontSize = 16, Margin = 0, BackgroundColor = Color.FromHex("fff"), Padding = new Thickness(10) };

    //    //    //BatchNumberLabel = new Label() { LineBreakMode = LineBreakMode.NoWrap, TextColor = Color.FromHex("222"), HorizontalOptions = LayoutOptions.End, HorizontalTextAlignment = TextAlignment.End, VerticalOptions = LayoutOptions.Center, FontSize = 14 };
    //    //    //ExpirationDateLabel = new Label() { LineBreakMode = LineBreakMode.NoWrap, TextColor = Color.FromHex("222"), HorizontalOptions = LayoutOptions.End, HorizontalTextAlignment = TextAlignment.End, VerticalOptions = LayoutOptions.Center, FontSize = 14 };
    //    //    //BatchNumberStockLabel = new Label() { LineBreakMode = LineBreakMode.TailTruncation, TextColor = Color.FromHex("222"), HorizontalOptions = LayoutOptions.End, HorizontalTextAlignment = TextAlignment.End, VerticalOptions = LayoutOptions.Center, FontSize = 14 };
    //    //    //BatchNumberQuantityPlusLabel = new RightEntryCompanionLabel() { LineBreakMode = LineBreakMode.TailTruncation, Text = "+", TextColor = Color.FromHex("222"), HorizontalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.Center, VerticalOptions = LayoutOptions.Center, VerticalTextAlignment = TextAlignment.Center, FontSize = 14, Margin = 1 };
    //    //    //BatchNumberQuantityMinusLabel = new LeftEntryCompanionLabel() { LineBreakMode = LineBreakMode.TailTruncation, Text = "-", TextColor = Color.FromHex("222"), HorizontalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.Center, VerticalOptions = LayoutOptions.Center, VerticalTextAlignment = TextAlignment.Center, FontSize = 14, Margin = 1 };
    //    //    //BatchNumberQuantityEntry = new MyLabel() { LineBreakMode = LineBreakMode.TailTruncation, TextColor = Color.FromHex("222"), HorizontalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.Center, VerticalOptions = LayoutOptions.Center, VerticalTextAlignment = TextAlignment.Center, FontSize = 14, Margin = 0, BackgroundColor = Color.FromHex("fff"), Padding = new Thickness(10) };

    //    //    //var ShowConsumerPriceAddedSpace = ShowConsumerPrice ? 1 : 0;

    //    //    //GridWrapper.Children.Add(GroupLabel, 1, 0);
    //    //    //Grid.SetColumnSpan(GroupLabel, 5 + ShowConsumerPriceAddedSpace);
    //    //    //GridWrapper.Children.Add(GroupButtonImage, 0, 0);
    //    //    //Grid.SetRowSpan(GroupButtonImage, 2);
    //    //    //GridWrapper.Children.Add(GroupPriceSumLabel, 1, 1);
    //    //    //Grid.SetColumnSpan(GroupPriceSumLabel, 5 + ShowConsumerPriceAddedSpace);

    //    //    //if (App.ShowStuffCodeInOrderInsertForm.Value)
    //    //    //{
    //    //    //    GridWrapper.Children.Add(CodeLabel, 5 + ShowConsumerPriceAddedSpace, 2);
    //    //    //    GridWrapper.Children.Add(NameLabel, 1, 2);
    //    //    //    Grid.SetColumnSpan(NameLabel, 4 + ShowConsumerPriceAddedSpace);
    //    //    //}
    //    //    //else
    //    //    //{
    //    //    //    GridWrapper.Children.Add(NameLabel, 1, 2);
    //    //    //    Grid.SetColumnSpan(NameLabel, 5 + ShowConsumerPriceAddedSpace);
    //    //    //}
    //    //    //GridWrapper.Children.Add(UnitNameLabel, 0, 2);

    //    //    //GridWrapper.Children.Add(PriceLabel, 0, 3);
    //    //    //GridWrapper.Children.Add(FeeText, 1, 3);
    //    //    //GridWrapper.Children.Add(FeeLabel, 1, 3);
            
    //    //    //if (ShowConsumerPrice)
    //    //    //    GridWrapper.Children.Add(ConsumerFeeLabel, 2, 3);

    //    //    //GridWrapper.Children.Add(QuantityMinusLabel, 2 + ShowConsumerPriceAddedSpace, 3);
    //    //    //GridWrapper.Children.Add(QuantityEntry, 3 + ShowConsumerPriceAddedSpace, 3);
    //    //    //GridWrapper.Children.Add(QuantityPlusLabel, 4 + ShowConsumerPriceAddedSpace, 3);
    //    //    //GridWrapper.Children.Add(StockLabel, 5 + ShowConsumerPriceAddedSpace, 3);

    //    //    //GridWrapper.Children.Add(ExpirationDateLabel, 0, 4);
    //    //    //GridWrapper.Children.Add(BatchNumberLabel, 1, 4);
    //    //    //GridWrapper.Children.Add(BatchNumberQuantityMinusLabel, 2 + ShowConsumerPriceAddedSpace, 4);
    //    //    //GridWrapper.Children.Add(BatchNumberQuantityEntry, 3 + ShowConsumerPriceAddedSpace, 4);
    //    //    //GridWrapper.Children.Add(BatchNumberQuantityPlusLabel, 4 + ShowConsumerPriceAddedSpace, 4);
    //    //    //GridWrapper.Children.Add(BatchNumberStockLabel, 5 + ShowConsumerPriceAddedSpace, 4);

    //    //    //GroupLabel.SetBinding(Label.TextProperty, "DisplayGroupName");
    //    //    //GroupButtonImage.SetBinding(Image.SourceProperty, "GroupButtonIcon");
    //    //    //GroupPriceSumLabel.SetBinding(Label.TextProperty, "GroupSummary");

    //    //    //CodeLabel.SetBinding(Label.TextProperty, "Code");
    //    //    //NameLabel.SetBinding(Label.TextProperty, "Name");
    //    //    //UnitNameLabel.SetBinding(Label.TextProperty, "UnitName");
    //    //    //StockLabel.SetBinding(Label.TextProperty, "Stock");
    //    //    //FeeText.SetBinding(Entry.TextProperty, "Fee");
    //    //    //ConsumerFeeLabel.SetBinding(Label.TextProperty, "ConsumerFee");
    //    //    //PriceLabel.SetBinding(Label.TextProperty, "Price");
    //    //    //QuantityEntry.SetBinding(Label.TextProperty, "QuantityLabel");

    //    //    //QuantityPlusLabel.SetBinding(RightEntryCompanionLabel.DisabledProperty, "HasBatchNumbers");
    //    //    //QuantityMinusLabel.SetBinding(LeftEntryCompanionLabel.DisabledProperty, "HasBatchNumbers");

    //    //    //ExpirationDateLabel.SetBinding(Label.TextProperty, "ExpirationDate");
    //    //    //BatchNumberLabel.SetBinding(Label.TextProperty, "BatchNumber");
    //    //    //BatchNumberQuantityEntry.SetBinding(Label.TextProperty, "QuantityLabel");
    //    //    //BatchNumberStockLabel.SetBinding(Label.TextProperty, "Stock");

    //    //    //var UnitNameTapGestureRecognizer = new TapGestureRecognizer();
    //    //    //UnitNameTapGestureRecognizer.Tapped += UnitNameTapEventHandler;
    //    //    //UnitNameTapGestureRecognizer.SetBinding(TapGestureRecognizer.CommandParameterProperty, "Id");
    //    //    //UnitNameLabel.GestureRecognizers.Add(UnitNameTapGestureRecognizer);

    //    //    //var QuantityTextBoxTapGestureRecognizer = new TapGestureRecognizer();
    //    //    //QuantityTextBoxTapGestureRecognizer.Tapped += QuantityTextBoxTapEventHandler;
    //    //    //QuantityTextBoxTapGestureRecognizer.SetBinding(TapGestureRecognizer.CommandParameterProperty, "Id");
    //    //    //QuantityEntry.GestureRecognizers.Add(QuantityTextBoxTapGestureRecognizer);
    //    //    //BatchNumberQuantityEntry.GestureRecognizers.Add(QuantityTextBoxTapGestureRecognizer);

    //    //    //var QuantityPlusTapGestureRecognizer = new TapGestureRecognizer();
    //    //    //QuantityPlusTapGestureRecognizer.Tapped += QuantityPlusTapEventHandler;
    //    //    //QuantityPlusTapGestureRecognizer.SetBinding(TapGestureRecognizer.CommandParameterProperty, "Id");
    //    //    //QuantityPlusLabel.GestureRecognizers.Add(QuantityPlusTapGestureRecognizer);
    //    //    //BatchNumberQuantityPlusLabel.GestureRecognizers.Add(QuantityPlusTapGestureRecognizer);

    //    //    //var QuantityMinusTapGestureRecognizer = new TapGestureRecognizer();
    //    //    //QuantityMinusTapGestureRecognizer.Tapped += QuantityMinusTapEventHandler;
    //    //    //QuantityMinusTapGestureRecognizer.SetBinding(TapGestureRecognizer.CommandParameterProperty, "Id");
    //    //    //QuantityMinusLabel.GestureRecognizers.Add(QuantityMinusTapGestureRecognizer);
    //    //    //BatchNumberQuantityMinusLabel.GestureRecognizers.Add(QuantityMinusTapGestureRecognizer);

    //    //    //var GroupTapGestureRecognizer = new TapGestureRecognizer();
    //    //    //GroupTapGestureRecognizer.Tapped += GroupTapEventHandler;
    //    //    //GroupTapGestureRecognizer.SetBinding(TapGestureRecognizer.CommandParameterProperty, "Id");
    //    //    //GridWrapper.GestureRecognizers.Add(GroupTapGestureRecognizer);

    //    //    //View = GridWrapper;
    //    //}
    //}

    //public class StuffGallaryView : ContentView
    //{
    //    public static EventHandler UnitNameTapEventHandler, QuantityPlusTapEventHandler, QuantityMinusTapEventHandler, QuantityTextBoxTapEventHandler;

    //    Label TopContentBG = null, BottomContentBG = null, CodeLabel = null, NameLabel = null, UnitNameLabel = null, StockLabel = null, FeeLabel = null, ConsumerFeeLabel = null, PriceLabel = null, QuantityPlusLabel = null, QuantityMinusLabel = null, QuantityEntry = null;
    //    public CachedImage NewStuffImage;
    //    Grid GridWrapper;

    //    public static readonly BindableProperty IdProperty =
    //        BindableProperty.Create("Id", typeof(string), typeof(CustomStuffListCell), string.Empty);
    //    public string Id
    //    {
    //        get { return (string)GetValue(IdProperty); }
    //        set { SetValue(IdProperty, value); }
    //    }

    //    public StuffGallaryView()
    //    {
    //        this.SetBinding(StuffGallaryView.IdProperty, "Id");

    //        GridWrapper = new Grid()
    //        {
    //            Padding = 3,
    //            RowSpacing = 0,
    //            ColumnSpacing = 0,
    //            Margin = 0,
    //            HorizontalOptions = LayoutOptions.FillAndExpand,
    //            VerticalOptions = LayoutOptions.FillAndExpand,
    //        };

    //        GridWrapper.RowDefinitions = new RowDefinitionCollection();
    //        GridWrapper.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
    //        GridWrapper.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
    //        GridWrapper.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });

    //        GridWrapper.ColumnDefinitions = new ColumnDefinitionCollection();
    //        GridWrapper.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(3, GridUnitType.Star) });
    //        GridWrapper.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(3, GridUnitType.Star) });
    //        if (App.ShowConsumerPrice.Value)
    //            GridWrapper.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(3, GridUnitType.Star) });
    //        GridWrapper.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
    //        GridWrapper.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });
    //        GridWrapper.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
    //        GridWrapper.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });

    //        TopContentBG = new MyLabel() { IsGallaryTopBG = true, HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand };
    //        BottomContentBG = new MyLabel() { IsGallaryBottomBG = true, HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand };
    //        NewStuffImage = new CachedImage() { HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand, DownsampleToViewSize = true };
    //        CodeLabel = new MyLabel() { LineBreakMode = LineBreakMode.HeadTruncation, TextColor = Color.FromHex("fff"), HorizontalOptions = LayoutOptions.End, HorizontalTextAlignment = TextAlignment.End, VerticalOptions = LayoutOptions.Center, FontSize = 16, Padding = new Thickness(8) };
    //        NameLabel = new MyLabel() { LineBreakMode = LineBreakMode.TailTruncation, TextColor = Color.FromHex("fff"), HorizontalOptions = LayoutOptions.End, HorizontalTextAlignment = TextAlignment.End, VerticalOptions = LayoutOptions.Center, FontSize = 16, Padding = new Thickness(8) };
    //        UnitNameLabel = new FullRoundedLabel() { LineBreakMode = LineBreakMode.TailTruncation, TextColor = Color.FromHex("222"), HorizontalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.Center, VerticalOptions = LayoutOptions.Center, VerticalTextAlignment = TextAlignment.Center, FontSize = 16, Margin = 3 };
    //        StockLabel = new MyLabel() { LineBreakMode = LineBreakMode.TailTruncation, TextColor = Color.FromHex("fff"), HorizontalOptions = LayoutOptions.End, HorizontalTextAlignment = TextAlignment.End, VerticalOptions = LayoutOptions.Center, FontSize = 16, Padding = new Thickness(8) };
    //        FeeLabel = new MyLabel() { LineBreakMode = LineBreakMode.TailTruncation, TextColor = Color.FromHex("fff"), HorizontalOptions = LayoutOptions.Start, HorizontalTextAlignment = TextAlignment.Start, VerticalOptions = LayoutOptions.Center, FontSize = 16, Padding = new Thickness(8) };
    //        ConsumerFeeLabel = new MyLabel() { LineBreakMode = LineBreakMode.TailTruncation, TextColor = Color.FromHex("fff"), HorizontalOptions = LayoutOptions.Start, HorizontalTextAlignment = TextAlignment.Start, VerticalOptions = LayoutOptions.Center, FontSize = 16, Padding = new Thickness(8) };
    //        PriceLabel = new MyLabel() { LineBreakMode = LineBreakMode.TailTruncation, TextColor = Color.FromHex("fff"), HorizontalOptions = LayoutOptions.Start, HorizontalTextAlignment = TextAlignment.Start, VerticalOptions = LayoutOptions.Center, FontSize = 16, Padding = new Thickness(8) };
    //        QuantityPlusLabel = new RightEntryCompanionLabel() { LineBreakMode = LineBreakMode.TailTruncation, Text = "+", TextColor = Color.FromHex("222"), HorizontalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.Center, VerticalOptions = LayoutOptions.Center, VerticalTextAlignment = TextAlignment.Center, FontSize = 16, Margin = 1 };
    //        QuantityMinusLabel = new LeftEntryCompanionLabel() { LineBreakMode = LineBreakMode.TailTruncation, Text = "-", TextColor = Color.FromHex("222"), HorizontalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.Center, VerticalOptions = LayoutOptions.Center, VerticalTextAlignment = TextAlignment.Center, FontSize = 16, Margin = 1 };
    //        QuantityEntry = new MyLabel() { LineBreakMode = LineBreakMode.TailTruncation, TextColor = Color.FromHex("222"), HorizontalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.Center, VerticalOptions = LayoutOptions.Center, VerticalTextAlignment = TextAlignment.Center, FontSize = 16, Margin = 0, BackgroundColor = Color.FromHex("fff"), Padding = new Thickness(10) };

    //        var ShowConsumerPriceAddedSpace = App.ShowConsumerPrice.Value ? 1 : 0;

    //        var BackgroundLabel = new MyLabel() { HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand, IsGallaryBG = true };
    //        GridWrapper.Children.Add(BackgroundLabel, 0, 0);
    //        Grid.SetRowSpan(BackgroundLabel, 3);
    //        Grid.SetColumnSpan(BackgroundLabel, 6 + ShowConsumerPriceAddedSpace);
    //        GridWrapper.Children.Add(NewStuffImage, 0, 0);
    //        Grid.SetRowSpan(NewStuffImage, 3);
    //        Grid.SetColumnSpan(NewStuffImage, 6 + ShowConsumerPriceAddedSpace);
    //        GridWrapper.Children.Add(TopContentBG, 0, 0);
    //        Grid.SetColumnSpan(TopContentBG, 6 + ShowConsumerPriceAddedSpace);
    //        GridWrapper.Children.Add(BottomContentBG, 0, 2);
    //        Grid.SetColumnSpan(BottomContentBG, 6 + ShowConsumerPriceAddedSpace);

    //        if (App.ShowStuffCodeInOrderInsertForm.Value)
    //        {
    //            GridWrapper.Children.Add(CodeLabel, 5 + ShowConsumerPriceAddedSpace, 0);
    //            GridWrapper.Children.Add(NameLabel, 1, 0);
    //            Grid.SetColumnSpan(NameLabel, 4 + ShowConsumerPriceAddedSpace);
    //        }
    //        else
    //        {
    //            GridWrapper.Children.Add(NameLabel, 1, 0);
    //            Grid.SetColumnSpan(NameLabel, 5 + ShowConsumerPriceAddedSpace);
    //        }
    //        GridWrapper.Children.Add(UnitNameLabel, 0, 0);

    //        GridWrapper.Children.Add(PriceLabel, 0, 2);
    //        GridWrapper.Children.Add(FeeLabel, 1, 2);
    //        if (App.ShowConsumerPrice.Value)
    //            GridWrapper.Children.Add(ConsumerFeeLabel, 2, 2);
    //        GridWrapper.Children.Add(QuantityMinusLabel, 2 + ShowConsumerPriceAddedSpace, 2);
    //        GridWrapper.Children.Add(QuantityEntry, 3 + ShowConsumerPriceAddedSpace, 2);
    //        GridWrapper.Children.Add(QuantityPlusLabel, 4 + ShowConsumerPriceAddedSpace, 2);
    //        GridWrapper.Children.Add(StockLabel, 5 + ShowConsumerPriceAddedSpace, 2);

    //        CodeLabel.SetBinding(Label.TextProperty, "Code");
    //        NameLabel.SetBinding(Label.TextProperty, "Name");
    //        UnitNameLabel.SetBinding(Label.TextProperty, "UnitName");
    //        StockLabel.SetBinding(Label.TextProperty, "Stock");
    //        FeeLabel.SetBinding(Label.TextProperty, "Fee");
    //        ConsumerFeeLabel.SetBinding(Label.TextProperty, "ConsumerFee");
    //        PriceLabel.SetBinding(Label.TextProperty, "Price");
    //        QuantityEntry.SetBinding(Label.TextProperty, "QuantityLabel");
    //        NewStuffImage.SetBinding(CachedImage.SourceProperty, "ImageSource");

    //        QuantityPlusLabel.SetBinding(RightEntryCompanionLabel.DisabledProperty, "HasBatchNumbers");
    //        QuantityMinusLabel.SetBinding(LeftEntryCompanionLabel.DisabledProperty, "HasBatchNumbers");

    //        CodeLabel.SetBinding(Label.IsVisibleProperty, "SelectedInGallaryMode");
    //        NameLabel.SetBinding(Label.IsVisibleProperty, "SelectedInGallaryMode");
    //        UnitNameLabel.SetBinding(Label.IsVisibleProperty, "SelectedInGallaryMode");
    //        StockLabel.SetBinding(Label.IsVisibleProperty, "SelectedInGallaryMode");
    //        FeeLabel.SetBinding(Label.IsVisibleProperty, "SelectedInGallaryMode");
    //        ConsumerFeeLabel.SetBinding(Label.IsVisibleProperty, "SelectedInGallaryMode");
    //        PriceLabel.SetBinding(Label.IsVisibleProperty, "SelectedInGallaryMode");
    //        QuantityEntry.SetBinding(Label.IsVisibleProperty, "SelectedInGallaryMode");
    //        TopContentBG.SetBinding(Label.IsVisibleProperty, "SelectedInGallaryMode");
    //        BottomContentBG.SetBinding(Label.IsVisibleProperty, "SelectedInGallaryMode");
    //        QuantityPlusLabel.SetBinding(Label.IsVisibleProperty, "SelectedInGallaryMode");
    //        QuantityMinusLabel.SetBinding(Label.IsVisibleProperty, "SelectedInGallaryMode");

    //        var UnitNameTapGestureRecognizer = new TapGestureRecognizer();
    //        UnitNameTapGestureRecognizer.Tapped += UnitNameTapEventHandler;
    //        UnitNameTapGestureRecognizer.SetBinding(TapGestureRecognizer.CommandParameterProperty, "Id");
    //        UnitNameLabel.GestureRecognizers.Add(UnitNameTapGestureRecognizer);

    //        var QuantityTextBoxTapGestureRecognizer = new TapGestureRecognizer();
    //        QuantityTextBoxTapGestureRecognizer.Tapped += QuantityTextBoxTapEventHandler;
    //        QuantityTextBoxTapGestureRecognizer.SetBinding(TapGestureRecognizer.CommandParameterProperty, "Id");
    //        QuantityEntry.GestureRecognizers.Add(QuantityTextBoxTapGestureRecognizer);

    //        var StuffImageTapGestureRecognizer = new TapGestureRecognizer();
    //        StuffImageTapGestureRecognizer.Tapped += QuantityPlusTapEventHandler;
    //        StuffImageTapGestureRecognizer.SetBinding(TapGestureRecognizer.CommandParameterProperty, "Id");
    //        NewStuffImage.GestureRecognizers.Add(StuffImageTapGestureRecognizer);

    //        var QuantityPlusTapGestureRecognizer = new TapGestureRecognizer();
    //        QuantityPlusTapGestureRecognizer.Tapped += QuantityPlusTapEventHandler;
    //        QuantityPlusTapGestureRecognizer.SetBinding(TapGestureRecognizer.CommandParameterProperty, "Id");
    //        QuantityPlusLabel.GestureRecognizers.Add(QuantityPlusTapGestureRecognizer);

    //        var QuantityMinusTapGestureRecognizer = new TapGestureRecognizer();
    //        QuantityMinusTapGestureRecognizer.Tapped += QuantityMinusTapEventHandler;
    //        QuantityMinusTapGestureRecognizer.SetBinding(TapGestureRecognizer.CommandParameterProperty, "Id");
    //        QuantityMinusLabel.GestureRecognizers.Add(QuantityMinusTapGestureRecognizer);

    //        Content = new StackLayout()
    //        {
    //            Orientation = StackOrientation.Horizontal,
    //            HorizontalOptions = LayoutOptions.FillAndExpand,
    //            VerticalOptions = LayoutOptions.FillAndExpand,
    //            Children = { GridWrapper }
    //        };
    //    }
    //}

    //public class StuffGallaryViewGrid : ContentView
    //{
    //    readonly Grid _grid;
    //    public static int RowCount = 1, ColumnCount = 1;

    //    public static readonly BindableProperty StuffsArrayProperty =
    //        BindableProperty.Create("StuffsArray", typeof(DBRepository.StuffListModel[]), typeof(StuffGallaryViewGrid), null, propertyChanged: OnStuffsArrayChanged);
    //    public static Dictionary<Guid, StuffGallaryView> StuffViewHistory;
    //    public static Dictionary<int, Guid[]> PageStuffIds;

    //    static Guid[] LastStuffIds = null;
    //    static void OnStuffsArrayChanged(BindableObject bindable, object oldValue, object newValue)
    //    {
    //        var OldStuffsArray = LastStuffIds != null ? LastStuffIds.ToArray() : null;
    //        var NewStuffsArray = ((DBRepository.StuffListModel[])newValue).Where(a => !a.BatchNumberId.HasValue).Select(a => a.StuffId);
    //        LastStuffIds = NewStuffsArray.ToArray();

    //        var OldPageIndex = OldStuffsArray == null ? -1 : PageStuffIds.Single(a => OldStuffsArray.All(b => a.Value.Contains(b))).Key;
    //        var NewPageIndex = PageStuffIds.Single(a => NewStuffsArray.All(b => a.Value.Contains(b))).Key;

    //        var ShouldBeRemovedPageIndex = NewPageIndex + (OldPageIndex > NewPageIndex ? 3 : -3);
    //        if(PageStuffIds.Any(a => a.Key == ShouldBeRemovedPageIndex))
    //        {
    //            var ShouldBeRemovedStuffIds = PageStuffIds.Single(a => a.Key == ShouldBeRemovedPageIndex).Value;

    //            foreach (var item in ShouldBeRemovedStuffIds)
    //            {
    //                if (StuffViewHistory.ContainsKey(item))
    //                {
    //                    var OldView = StuffViewHistory[item];
    //                    OldView.NewStuffImage.Source = null;
    //                    StuffViewHistory.Remove(item);
    //                }
    //            }
    //        }

    //        //foreach (var item in NewStuffsArray)
    //        //{
    //        //    if(StuffViewHistory.ContainsKey(item.Id))
    //        //    {
    //        //        var OldView = StuffViewHistory[item.Id];
    //        //        OldView.NewStuffImage.Source = null;
    //        //        StuffViewHistory.Remove(item.Id);
    //        //    }
    //        //}

    //        ((StuffGallaryViewGrid)bindable).CreatePageContents();
    //    }

    //    public static void SetPageStuffIds(Dictionary<int, Guid[]> _PageStuffIds)
    //    {
    //        if (StuffViewHistory != null)
    //        {
    //            var _StuffViewHistory = StuffViewHistory.ToArray();
    //            foreach (var item in _StuffViewHistory)
    //            {
    //                var OldView = StuffViewHistory[item.Key];
    //                OldView.NewStuffImage.Source = null;
    //                StuffViewHistory.Remove(item.Key);
    //            }
    //        }
    //        LastStuffIds = null;
    //        StuffViewHistory = new Dictionary<Guid, StuffGallaryView>();
    //        PageStuffIds = _PageStuffIds;
    //    }

    //    public DBRepository.StuffListModel[] StuffsArray
    //    {
    //        get { return (DBRepository.StuffListModel[])GetValue(StuffsArrayProperty); }
    //        set { SetValue(StuffsArrayProperty, value); }
    //    }

    //    public StuffGallaryViewGrid()
    //    {
    //        this.SetBinding(StuffGallaryViewGrid.StuffsArrayProperty, "Stuffs");

    //        _grid = new Grid()
    //        {
    //            RowSpacing = 0,
    //            ColumnSpacing = 0,
    //            Padding = 0,
    //            Margin = 0,
    //            HorizontalOptions = LayoutOptions.FillAndExpand,
    //            VerticalOptions = LayoutOptions.FillAndExpand,
    //            RowDefinitions = new RowDefinitionCollection() { },
    //            ColumnDefinitions = new ColumnDefinitionCollection() { }
    //        };

    //        Content = _grid;
    //    }

    //    public IList<View> Children { get { return _grid.Children; } }

    //    private Guid LastLayingoutRequestId;
    //    private double ThisWidth, ThisHeight;
    //    protected override async void LayoutChildren(double x, double y, double width, double height)
    //    {
    //        base.LayoutChildren(x, y, width, height);
    //        var ThisLayingoutRequestId = Guid.NewGuid();
    //        LastLayingoutRequestId = ThisLayingoutRequestId;
    //        await Task.Delay(100);
    //        if (LastLayingoutRequestId != ThisLayingoutRequestId)
    //            return;

    //        var OldIsHorizontal = ThisWidth > ThisHeight;
    //        var NewIsHorizontal = width > height;

    //        ThisWidth = width;
    //        ThisHeight = height;
    //        foreach (var child in Children)
    //        {
    //            child.WidthRequest = ThisWidth;
    //            child.HeightRequest = ThisHeight;
    //        }

    //        if (OldIsHorizontal != NewIsHorizontal)
    //        {
    //            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
    //            {
    //                var Children = this.Children.ToList();
    //                foreach (var child in Children)
    //                    _grid.Children.Remove(child);

    //                _grid.RowDefinitions.Clear();
    //                _grid.ColumnDefinitions.Clear();
    //                for (int i = 0; i < (NewIsHorizontal ? RowCount : ColumnCount); i++)
    //                    _grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Star });
    //                for (int j = 0; j < (NewIsHorizontal ? ColumnCount : RowCount); j++)
    //                    _grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Star });

    //                for (int i = 0; i < (NewIsHorizontal ? RowCount : ColumnCount); i++)
    //                {
    //                    for (int j = 0; j < (NewIsHorizontal ? ColumnCount : RowCount); j++)
    //                    {
    //                        if (i * (NewIsHorizontal ? ColumnCount : RowCount) + j < Children.Count)
    //                        {
    //                            var Child = Children[i * (NewIsHorizontal ? ColumnCount : RowCount) + j];
    //                            _grid.Children.Add(Child, j, i);
    //                        }
    //                    }
    //                }
    //            });
    //        }
    //    }

    //    static DataTemplate ItemTemplate = new DataTemplate(typeof(StuffGallaryView));
    //    public View MakeNewItemInPage(object dataSource)
    //    {
    //        var view = (View)ItemTemplate.CreateContent();
    //        var bindableObject = view as BindableObject;
    //        if (bindableObject != null)
    //            bindableObject.BindingContext = dataSource;
    //        return view;
    //    }

    //    static bool Initialized = false;
    //    public void CreatePageContents()
    //    {
    //        if (!Initialized)
    //        {
    //            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
    //            {
    //                var IsHorizontal = ThisWidth > ThisHeight;
    //                for (int i = 0; i < (IsHorizontal ? RowCount : ColumnCount); i++)
    //                {
    //                    for (int j = 0; j < (IsHorizontal ? ColumnCount : RowCount); j++)
    //                    {
    //                        if (i * (IsHorizontal ? ColumnCount : RowCount) + j < StuffsArray.Length)
    //                        {
    //                            var dataSource = StuffsArray[i * (IsHorizontal ? ColumnCount : RowCount) + j];
    //                            var NewItemInPage = MakeNewItemInPage(dataSource);
    //                            _grid.Children.Add(NewItemInPage, j, i);
    //                            if(!StuffViewHistory.ContainsKey(dataSource.StuffId))
    //                                StuffViewHistory.Add(dataSource.StuffId, (StuffGallaryView)NewItemInPage);
    //                        }
    //                    }
    //                }
    //            });
    //        }
    //    }
    //}

    public partial class ReversionForm : GradientContentPage
    {
        //public static EventHandler UnitNameTapEventHandler, QuantityPlusTapEventHandler, QuantityMinusTapEventHandler, QuantityTextBoxTapEventHandler, GroupTapEventHandler;
        public static SettingField<bool> InsertOrderForm_ShowGallaryMode = new SettingField<bool>("InsertOrderForm_ShowGallaryMode", false);
        ObservableCollection<DBRepository.StuffListModel> _StuffsList = null;
        private ToolbarItem ToolbarItem_SendToServer, ToolbarItem_LocalSave;

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
        private ToolbarItem ToolbarItem_SearchBar, ToolbarItem_QRSearchBar, ToolbarItem_GallaryMode, ToolbarItem_StuffListMode, ToolbarItem_OrderBasket;
        private InsertedInformations_Orders OrdersForm;
        private PartnerListForm PartnerListForm;
        public SaleOrder EditingSaleOrder;
        public Guid? SettlementTypeId;
        public string Description;
        MyKeyboard<QuantityEditingStuffModel, decimal> QuantityKeyboard;
        LeftEntryCompanionLabel PartnerChangeButton;
        RightRoundedLabel PartnerLabel;
        LeftEntryCompanionLabel PartnerRemainderDetailButton;
        RightRoundedLabel PartnerRemainderLabel;
        LeftEntryCompanionLabel GallaryStuffGroupChangeButton;
        RightRoundedLabel GallaryStuffGroupLabel;
        string GallaryStuffGroupLabelPlaceholder;
        Picker GallaryStuffGroupPicker;
        Guid? WarehouseId;
        bool TapEventHandlingInProgress = false;

        CarouselViewControl gallaryCarousel;

        Guid LastSearchWhenTypingId = Guid.NewGuid();
        DtoReversionReason[] ReversionReasons = null;
        DtoPersonnelForSaleOrder[] Visitors = null;

        public ReversionForm(Partner Partner, SaleOrder SaleOrder, InsertedInformations_Orders OrdersForm, PartnerListForm PartnerListForm, Guid? __WarehouseId,bool fromTour=false)
        {
            App.UniversalLineInApp = 875234001;

            InitializeComponent();
            App.UniversalLineInApp = 875234002;

            WarehouseId = __WarehouseId;

            if (!WarehouseId.HasValue && !string.IsNullOrEmpty(App.DefaultWarehouseId.Value))
                WarehouseId = new Guid(App.DefaultWarehouseId.Value);

            if (!WarehouseId.HasValue && App.DefineWarehouseForSaleAndBuy.Value)
                PrepareWarehousePicker();

            var CloseButtonTapGestureRecognizer = new TapGestureRecognizer();
            CloseButtonTapGestureRecognizer.Tapped += CloseButtonTapGestureRecognizer_Tapped;
            CloseButton.GestureRecognizers.Add(CloseButtonTapGestureRecognizer);
            App.UniversalLineInApp = 875234008;

            PartnerChangeButton = new LeftEntryCompanionLabel() { VerticalOptions = LayoutOptions.FillAndExpand, VerticalTextAlignment = TextAlignment.Center, HorizontalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.Center, FontSize = 18, Text = "🔍" };
            PartnerLabel = new RightRoundedLabel() { VerticalOptions = LayoutOptions.FillAndExpand, VerticalTextAlignment = TextAlignment.Center, HorizontalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.End, Text = "مشتری", Padding = new Thickness(0, 0, 50, 0) };
            PartnerLabel.FontSize *= 1.5;

            PartnerRemainderLabel = new RightRoundedLabel() { VerticalOptions = LayoutOptions.FillAndExpand, VerticalTextAlignment = TextAlignment.Center, HorizontalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.End, Text = PartnerAccountingData(), Padding = new Thickness(0, 0, 50, 0) };
            PartnerRemainderDetailButton = new LeftEntryCompanionLabel() { VerticalOptions = LayoutOptions.FillAndExpand, VerticalTextAlignment = TextAlignment.Center, HorizontalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.Center, FontSize = 18, Text = "🔍" };

            GallaryStuffGroupChangeButton = new LeftEntryCompanionLabel() { VerticalOptions = LayoutOptions.FillAndExpand, HorizontalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.Center, FontSize = 18, Text = "🔍" };
            App.UniversalLineInApp = 875234014;
            GallaryStuffGroupLabelPlaceholder = App.StuffListGroupingMethod.Value == 0 ? "" : App.StuffListGroupingMethod.Value == 1 ? "گروه کالا" : "سبد کالا";
            GallaryStuffGroupLabel = new RightRoundedLabel() { VerticalOptions = LayoutOptions.FillAndExpand, HorizontalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.End, Text = GallaryStuffGroupLabelPlaceholder };
            GallaryStuffGroupLabel.FontSize *= 1.5;
            GallaryStuffGroupPicker = new Picker();
            GallaryStuffGroupPicker.SelectedIndexChanged += (sender, e) =>
            {
                if (GallaryStuffGroupPicker.SelectedIndex != -1)
                    OpenGroup(LastStuffsGroups[GallaryStuffGroupPicker.SelectedIndex].StuffId);
            };
            App.UniversalLineInApp = 875234021;

            //991128
            //StuffItems.IsVisible = !InsertOrderForm_ShowGallaryMode.Value;
            //GallaryContainer.IsVisible = InsertOrderForm_ShowGallaryMode.Value;

            this.OrdersForm = OrdersForm;
            this.PartnerListForm = PartnerListForm;

            EditingSaleOrder = SaleOrder != null ? SaleOrder : null;
            App.UniversalLineInApp = 875234026;
            if (EditingSaleOrder != null)
            {
                App.UniversalLineInApp = 875234027;
                this.SettlementTypeId = EditingSaleOrder.SettlementTypeId;
                this.Description = EditingSaleOrder.Description;
            }
            App.UniversalLineInApp = 875234030;

            var _Partner = Partner != null ? Partner : SaleOrder != null ? SaleOrder.Partner : null;
            if (_Partner != null)
            {
                App.UniversalLineInApp = 875234032;
                _BeforeSelectedPartner = _Partner;
                _SelectedPartner = _Partner;
                PartnerLabel.Text = !string.IsNullOrEmpty(SelectedPartner.LegalName) ? (SelectedPartner.LegalName + (!string.IsNullOrEmpty(SelectedPartner.Name) ? (" (" + SelectedPartner.Name + ")") : "")) : (SelectedPartner.Name);
            }
            App.UniversalLineInApp = 875234037;
            if (SelectedPartner == null)
            {
                PartnerChangeButton.IsEnabled = true;
                App.UniversalLineInApp = 875234038;
                PartnerRemainderDetailButton.IsEnabled = true;
            }
            else
            {
                PartnerChangeButton.IsEnabled = false;
                App.UniversalLineInApp = 875234040;
                PartnerRemainderDetailButton.IsEnabled = false;
            }

            var PartnerChangeButtonTapGestureRecognizer = new TapGestureRecognizer();
            PartnerChangeButtonTapGestureRecognizer.Tapped += (sender, e) => {
                if (!TapEventHandlingInProgress)
                {
                    TapEventHandlingInProgress = true;

                    try
                    {
                        if (PartnerChangeButton.IsEnabled)
                        {
                            App.UniversalLineInApp = 875234044;
                            FocusedQuantityTextBoxId = null;
                            var PartnerListForm1 = new PartnerListForm();
                            PartnerListForm1.ReversionForm = this;
                            Navigation.PushAsync(PartnerListForm1);
                        }
                    }
                    catch (Exception)
                    { }
                    
                    TapEventHandlingInProgress = false;
                }
            };
            App.UniversalLineInApp = 875234050;
            PartnerChangeButtonTapGestureRecognizer.SetBinding(TapGestureRecognizer.CommandParameterProperty, "Id");
            PartnerChangeButton.GestureRecognizers.Add(PartnerChangeButtonTapGestureRecognizer);
            PartnerChangeButton.WidthRequest = 150;

            var PartnerRemainderDetailButtonTapGestureRecognizer = new TapGestureRecognizer();
            PartnerRemainderDetailButtonTapGestureRecognizer.Tapped += (sender, e) => {
                if (!TapEventHandlingInProgress)
                {
                    TapEventHandlingInProgress = true;

                    try
                    {
                        if (PartnerRemainderDetailButton.IsEnabled)
                        {
                            App.UniversalLineInApp = 875234056;
                            FocusedQuantityTextBoxId = null;
                            var PartnerReportForm = new PartnerReportForm(SelectedPartner.Id);
                            Navigation.PushAsync(PartnerReportForm);
                        }
                    }
                    catch (Exception)
                    { }
                    
                    TapEventHandlingInProgress = false;
                }
            };
            
            PartnerRemainderDetailButton.GestureRecognizers.Add(PartnerRemainderDetailButtonTapGestureRecognizer);
            PartnerRemainderDetailButton.WidthRequest = 150;
            
            StuffItems.HasUnevenRows = true;
            App.UniversalLineInApp = 875234064;
            StuffItems.SeparatorVisibility = SeparatorVisibility.None;
            StuffItems.ItemSelected += VisitItems_ItemSelected;

            GallaryStuffBatchNumbersList.HasUnevenRows = true;
            GallaryStuffBatchNumbersList.SeparatorVisibility = SeparatorVisibility.None;
            GallaryStuffBatchNumbersList.ItemSelected += VisitItems_ItemSelected;

            ToolbarItem_SearchBar = new ToolbarItem();
            ToolbarItem_SearchBar.Text = "جستجو";
            ToolbarItem_SearchBar.Icon = "Search.png";
            ToolbarItem_SearchBar.Activated += ToolbarItem_SearchBar_Activated;
            ToolbarItem_SearchBar.Order = ToolbarItemOrder.Primary;
            ToolbarItem_SearchBar.Priority = 10;
            if (!this.ToolbarItems.Contains(ToolbarItem_SearchBar) && !StuffsSearchBar.IsVisible)
                this.ToolbarItems.Add(ToolbarItem_SearchBar);

            ToolbarItem_QRSearchBar = new ToolbarItem();
            ToolbarItem_QRSearchBar.Text = "اسکنر بارکد";
            ToolbarItem_QRSearchBar.Icon = "SearchQR.png";
            ToolbarItem_QRSearchBar.Activated += ToolbarItem_QRSearchBar_Activated;
            ToolbarItem_QRSearchBar.Order = ToolbarItemOrder.Primary;
            ToolbarItem_QRSearchBar.Priority = 10;
            if (!this.ToolbarItems.Contains(ToolbarItem_QRSearchBar) && (App.UseBarcodeScannerInVisitorAppToSelectStuff.Value || App.UseQRScannerInVisitorAppToSelectStuff.Value))
                this.ToolbarItems.Add(ToolbarItem_QRSearchBar);

            ToolbarItem_GallaryMode = new ToolbarItem();
            ToolbarItem_GallaryMode.Text = "گالری تصاویر";
            ToolbarItem_GallaryMode.Icon = "ImageGallary.png";
            ToolbarItem_GallaryMode.Activated += ToolbarItem_GallaryMode_Activated;
            ToolbarItem_GallaryMode.Order = ToolbarItemOrder.Primary;

            ToolbarItem_LocalSave = new ToolbarItem();
            ToolbarItem_LocalSave.Text = "ذخیره محلی";
            ToolbarItem_LocalSave.Icon = "Save.png";
            ToolbarItem_LocalSave.Clicked += SubmitToStorage;
            ToolbarItem_LocalSave.Order = ToolbarItemOrder.Primary;
            ToolbarItem_LocalSave.Priority = 0;
           
            //if (!JustShow)
            //    this.ToolbarItems.Add(ToolbarItem_LocalSave);

            if (!this.ToolbarItems.Contains(ToolbarItem_LocalSave)) 
                this.ToolbarItems.Add(ToolbarItem_LocalSave);

            ToolbarItem_SendToServer = new ToolbarItem();
            ToolbarItem_SendToServer.Text = "ارسال به سرور";
            ToolbarItem_SendToServer.Icon = "Upload.png";
            ToolbarItem_SendToServer.Activated += SubmitToServer;
            ToolbarItem_SendToServer.Order = ToolbarItemOrder.Primary;
            ToolbarItem_SendToServer.Priority = 0;
            //if (!JustShow)
            //    this.ToolbarItems.Add(ToolbarItem_SendToServer);
            if (!this.ToolbarItems.Contains(ToolbarItem_SendToServer))
                this.ToolbarItems.Add(ToolbarItem_SendToServer);

            ToolbarItem_GallaryMode.Priority = 11;
            //if (!InsertOrderForm_ShowGallaryMode.Value && !this.ToolbarItems.Contains(ToolbarItem_GallaryMode))
            //    this.ToolbarItems.Add(ToolbarItem_GallaryMode);

            ToolbarItem_StuffListMode = new ToolbarItem();
            ToolbarItem_StuffListMode.Text = "لیست کالاها";
            ToolbarItem_StuffListMode.Icon = "StuffList.png";
            ToolbarItem_StuffListMode.Activated += ToolbarItem_StuffListMode_Activated;
            ToolbarItem_StuffListMode.Order = ToolbarItemOrder.Primary;
            ToolbarItem_StuffListMode.Priority = 11;
            if (InsertOrderForm_ShowGallaryMode.Value && !this.ToolbarItems.Contains(ToolbarItem_StuffListMode))
                this.ToolbarItems.Add(ToolbarItem_StuffListMode);

            ToolbarItem_OrderBasket = new ToolbarItem();
            ToolbarItem_OrderBasket.Text = "سبد سفارش";
            ToolbarItem_OrderBasket.Activated += ToolbarItem_OrderBasket_Activated;
            ToolbarItem_OrderBasket.Order = ToolbarItemOrder.Primary;
            ToolbarItem_OrderBasket.Priority = 1;

            StuffsSearchBar.TextChanged += async (sender, args) => {
                App.UniversalLineInApp = 875234103;
                var thisTextSearchId = Guid.NewGuid();
                LastSearchWhenTypingId = thisTextSearchId;
                await Task.Delay(1000);
                if (LastSearchWhenTypingId == thisTextSearchId)
                    await FillStuffs(args.NewTextValue, null, false, false);
                App.UniversalLineInApp = 875234107;
            };
            StuffsSearchBar.SearchButtonPressed += async (sender, args) =>
            {
                App.UniversalLineInApp = 875234108;
                await StuffsSearchBar.FadeTo(0, 350);
                StuffsSearchBar.IsVisible = false;
                ToolbarItem_SearchBar.Icon = "Search.png";
                App.UniversalLineInApp = 875234111;
            };

            StuffGallaryViewGrid.RowCount = App.GallaryStuffCount[0];
            StuffGallaryViewGrid.ColumnCount = App.GallaryStuffCount[1];
            gallaryCarousel = new CarouselViewControl()
            {
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
                ItemTemplate = new DataTemplate(typeof(StuffGallaryViewGrid))
            };

            GallaryContainer.Children.Add(gallaryCarousel);

            FillStuffs(StuffsSearchBar.Text, SaleOrder, true, false);
            App.UniversalLineInApp = 875234117;

            QuantityKeyboard = new MyKeyboard<QuantityEditingStuffModel, decimal>
            (
                QuantityKeyboardHolder,
                new Command((parameter) => {        //OnOK
                    FocusedQuantityTextBoxId = null;
                    CheckToShowOrderBasketTollbar();
                }),
                new Command((parameter) => {        //OnChange
                    CheckToShowOrderBasketTollbar();
                })
            );
            App.UniversalLineInApp = 875234118;

            FillReversionReasons();
            Task.Delay(100);
        }
        

        private async Task FillReversionReasons()
        {
            try
            {
                var result = await Connectivity.GetReversionReasons();

                if (result.Success && result.Data != null)
                {
                    ReversionReasons = result.Data.ToArray();

                    if (ReversionReasons != null)
                        foreach (var r in ReversionReasons)
                            ReversionReasonPicker.Items.Add(r.Name);
                }
                else
                {
                    App.ShowError("خطا", "هیچ علتی در سیستم تعریف نشده است ", "خوب");
                }
            }
            catch (Exception ex)
            {
                App.ShowError("خطا", ex.Message, "خوب");
            }
        }

        private async Task FillVisitors()
        {
            try
            {
                var result = await Connectivity.GetReversionVisitors();

                if (result.Success && result.Data != null)
                {
                    Visitors = result.Data.ToArray();

                    if (Visitors != null)
                        foreach (var r in Visitors)
                            VisitorPicker.Items.Add(r.Name);
                }
                else
                {
                    App.ShowError("خطا", "هیچ ویزیتوری یافت نشد ", "خوب");
                }
            }
            catch (Exception ex)
            {
                App.ShowError("خطا", ex.Message, "خوب");
            }
        }

        public async void PrepareWarehousePicker()
        {
            var _Warehouses = (await App.DB.GetWarehousesAsync()).Data.ToArray()
                .Select(a => new KeyValuePair<Warehouse, string>(a, a.WarehouseName.ToPersianDigits())).ToArray();
            if (_Warehouses.Length == 1)
            {
                WarehouseId = _Warehouses.Single().Key.WarehouseId;
                return;
            }
            
            WarehousePicker.Items.Clear();
            foreach (var Warehouse in _Warehouses)
                WarehousePicker.Items.Add(Warehouse.Value);

            WarehousePicker.SelectedIndex = -1;
            
            WarehousePicker.SelectedIndexChanged += (sender, e) =>
            {
                WarehouseId = _Warehouses[WarehousePicker.SelectedIndex].Key.WarehouseId;
                WarehousePicker.IsEnabled = false;
                FillStuffs();
            };

            WarehousePicker.Unfocused += (s, e) =>
            {
                if(!WarehouseId.HasValue)
                    try { Navigation.PopAsync(); } catch (Exception) { }
            };

            await Task.Delay(100);
            Device.BeginInvokeOnMainThread(() =>
            {
                if (WarehousePicker.IsFocused)
                    WarehousePicker.Unfocus();

                WarehousePicker.Focus();
            });
        }
        

        async Task FetchPartnerCycleInformationFromServer()
        {
            if (App.ShowAccountingCycleOfPartner_Remainder.Value || App.ShowAccountingCycleOfPartner_UncashedCheques.Value || App.ShowAccountingCycleOfPartner_ReturnedCheques.Value)
            {
                GettingPartnerCycleInformations = 1;
                PartnerRemainder = null;
                PartnerUncashedChequesCount = null;
                PartnerUncashedChequesPrice = null;
                PartnerReturnedChequesCount = null;
                PartnerReturnedChequesPrice = null;
                await RefreshPartnerCycleInformation();

                var result = await Connectivity.GetPartnerCycleInformationFromServerAsync(SelectedPartner.Id, false);
                GettingPartnerCycleInformations = 0;
                if (result.Data != null)
                {
                    PartnerRemainder = result.Data.Remainder;
                    PartnerUncashedChequesCount = result.Data.UncashedChequesCount;
                    PartnerUncashedChequesPrice = result.Data.UncashedChequesPrice;
                    PartnerReturnedChequesCount = result.Data.ReturnedChequesCount;
                    PartnerReturnedChequesPrice = result.Data.ReturnedChequesPrice;
                }
                await RefreshPartnerCycleInformation();
                if (!result.Success)
                    App.ShowError("خطا", result.Message, "خوب");
            }
        }

        int GettingPartnerCycleInformations = 0;
        decimal? PartnerRemainder, PartnerUncashedChequesCount, PartnerUncashedChequesPrice, PartnerReturnedChequesCount, PartnerReturnedChequesPrice;
        string PartnerAccountingData()
        {
            var Strs = new string[] {
                App.ShowAccountingCycleOfPartner_Remainder.Value ? ("مانده حساب: " + (PartnerRemainder.HasValue ? (Math.Abs(PartnerRemainder.Value).ToString("###,###,###,###,###,###,##0.") + (PartnerRemainder.Value > 0 ? " بدهکار" : PartnerRemainder.Value < 0 ? " بستانکار" : "")) : (GettingPartnerCycleInformations == 0 ? "---" : "".PadLeft(GettingPartnerCycleInformations, '.')))) : "",
                App.ShowAccountingCycleOfPartner_UncashedCheques.Value ? ("چک های وصول نشده: " + (PartnerUncashedChequesCount.HasValue ? (PartnerUncashedChequesCount.Value == 0 ? "0" : PartnerUncashedChequesCount.Value + " فقره چک به مجموع مبلغ " + PartnerUncashedChequesPrice.Value.ToString("###,###,###,###,###,###,##0.") + " ریال") : (GettingPartnerCycleInformations == 0 ? "---" : "".PadLeft(GettingPartnerCycleInformations, '.')))) : "",
                App.ShowAccountingCycleOfPartner_ReturnedCheques.Value ? ("چک های برگشتی: " + (PartnerReturnedChequesCount.HasValue ? (PartnerReturnedChequesCount.Value == 0 ? "0" : PartnerReturnedChequesCount.Value + " فقره چک به مجموع مبلغ " + PartnerReturnedChequesPrice.Value.ToString("###,###,###,###,###,###,##0.") + " ریال") : (GettingPartnerCycleInformations == 0 ? "---" : "".PadLeft(GettingPartnerCycleInformations, '.')))) : ""
            };
            return Strs.Any(a => !string.IsNullOrWhiteSpace(a)) ? Strs.Where(a => !string.IsNullOrWhiteSpace(a)).Aggregate((sum, x) => sum + "\n" + x).ToPersianDigits() : "";
        }

        async Task RefreshPartnerCycleInformation()
        {
            if (App.Accesses.AccessToViewPartnerRemainder)
            {
                PartnerRemainderLabel.Text = PartnerAccountingData();
                if (GettingPartnerCycleInformations != 0)
                {
                    GettingPartnerCycleInformations = new int[] { -1, 2, 3, 1 }[GettingPartnerCycleInformations];
                    await Task.Delay(500);
                    await RefreshPartnerCycleInformation();
                }
            }
        }

        private void UnitNameTapEventHandler(object s, EventArgs e)
        {
            if (!TapEventHandlingInProgress)
            {
                TapEventHandlingInProgress = true;

                try
                {
                    App.UniversalLineInApp = 875234121;
                    var Ids = ((string)((TappedEventArgs)e).Parameter).Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                    var StuffId = new Guid(Ids[0]);
                    var BatchNumberId = Ids.Length == 2 ? new Guid(Ids[1]) : new Nullable<Guid>();
                    UnitNameClicked(StuffId);
                    App.UniversalLineInApp = 875234125;
                }
                catch (Exception)
                { }

                TapEventHandlingInProgress = false;
            }
        }

        private void QuantityTextBoxTapEventHandler(object s, EventArgs e)
        {
            if (!TapEventHandlingInProgress)
            {
                TapEventHandlingInProgress = true;

                try
                {
                    App.UniversalLineInApp = 875234127;
                    var Ids = ((string)((TappedEventArgs)e).Parameter).Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                    var StuffId = new Guid(Ids[0]);
                    var BatchNumberId = Ids.Length == 2 ? new Guid(Ids[1]) : new Nullable<Guid>();
                    FocusedQuantityTextBoxId = BatchNumberId.HasValue ? new Guid[] { StuffId, BatchNumberId.Value } : new Guid[] { StuffId };
                    App.UniversalLineInApp = 875234131;
                }
                catch (Exception)
                { }

                TapEventHandlingInProgress = false;
            }
        }

        private void QuantityPlusTapEventHandler(object s, EventArgs e)
        {
            if (!TapEventHandlingInProgress)
            {
                TapEventHandlingInProgress = true;

                try
                {
                    App.UniversalLineInApp = 875234133;
                    var Ids = ((string)((TappedEventArgs)e).Parameter).Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                    var StuffId = new Guid(Ids[0]);
                    var BatchNumberId = Ids.Length == 2 ? new Guid(Ids[1]) : new Nullable<Guid>();
                    QuantityPlusClicked(StuffId, BatchNumberId);
                }
                catch (Exception)
                { }

                TapEventHandlingInProgress = false;
            }
        }

        private void UnitNameTapEventHandle(object s, EventArgs e)
        {
            if (!TapEventHandlingInProgress)
            {
                TapEventHandlingInProgress = true;

                try
                {
                    App.UniversalLineInApp = 875234121;
                    var Ids = ((string)((TappedEventArgs)e).Parameter).Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                    var StuffId = new Guid(Ids[0]);
                    var BatchNumberId = Ids.Length == 2 ? new Guid(Ids[1]) : new Nullable<Guid>();
                    UnitNameClicked(StuffId);
                    App.UniversalLineInApp = 875234125;
                }
                catch (Exception)
                { }

                TapEventHandlingInProgress = false;
            }
        }

        private void QuantityMinusTapEventHandler(object s, EventArgs e)
        {
            if (!TapEventHandlingInProgress)
            {
                TapEventHandlingInProgress = true;

                try
                {
                    App.UniversalLineInApp = 875234138;
                    var Ids = ((string)((TappedEventArgs)e).Parameter).Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                    var StuffId = new Guid(Ids[0]);
                    var BatchNumberId = Ids.Length == 2 ? new Guid(Ids[1]) : new Nullable<Guid>();
                    QuantityMinusClicked(StuffId, BatchNumberId);
                    App.UniversalLineInApp = 875234142;
                }
                catch (Exception)
                { }

                TapEventHandlingInProgress = false;
            }
        }
        protected override async void OnAppearing()
        {
            try
            {
                App.UniversalLineInApp = 875234119;
                base.OnAppearing();
                App.UniversalLineInApp = 875234120;

                                
                App.UniversalLineInApp = 875234132;

                

               

                //ReversionCustomStuffListCell.GroupTapEventHandler = (s, e) => {
                //    if (!TapEventHandlingInProgress)
                //    {
                //        TapEventHandlingInProgress = true;

                //        try
                //        {
                //            App.UniversalLineInApp = 875234143;//THISLINE
                //            var Ids = ((string)((TappedEventArgs)e).Parameter).Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                //            App.UniversalLineInApp = 875234144;
                //            var GroupId = new Guid(Ids[0]);
                //            App.UniversalLineInApp = 875234145;
                //            GroupClicked(GroupId);
                //            App.UniversalLineInApp = 875234146;
                //        }
                //        catch (Exception)
                //        { }

                //        TapEventHandlingInProgress = false;
                //    }
                //};

                //StuffItems.ItemTemplate = new DataTemplate(typeof(ReversionCustomStuffListCell));
                //GallaryStuffBatchNumbersList.ItemTemplate = new DataTemplate(typeof(ReversionCustomStuffListCell));


                StuffGallaryView.UnitNameTapEventHandler = (s, e) => {
                    if (!TapEventHandlingInProgress)
                    {
                        TapEventHandlingInProgress = true;

                        try
                        {
                            App.UniversalLineInApp = 875234150;
                            var Ids = ((string)((TappedEventArgs)e).Parameter).Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                            var StuffId = new Guid(Ids[0]);
                            var BatchNumberId = Ids.Length == 2 ? new Guid(Ids[1]) : new Nullable<Guid>();
                            UnitNameClicked(StuffId);
                            App.UniversalLineInApp = 875234154;
                        }
                        catch (Exception)
                        { }

                        TapEventHandlingInProgress = false;
                    }
                };
                App.UniversalLineInApp = 875234155;

                StuffGallaryView.QuantityTextBoxTapEventHandler = (s, e) => {
                    if (!TapEventHandlingInProgress)
                    {
                        TapEventHandlingInProgress = true;

                        try
                        {
                            App.UniversalLineInApp = 875234156;
                            var Ids = ((string)((TappedEventArgs)e).Parameter).Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                            var StuffId = new Guid(Ids[0]);
                            var BatchNumberId = Ids.Length == 2 ? new Guid(Ids[1]) : new Nullable<Guid>();
                            FocusedQuantityTextBoxId = BatchNumberId.HasValue ? new Guid[] { StuffId, BatchNumberId.Value } : new Guid[] { StuffId };
                            App.UniversalLineInApp = 875234160;
                        }
                        catch (Exception)
                        { }

                        TapEventHandlingInProgress = false;
                    }
                };
                App.UniversalLineInApp = 875234161;//THISLINE

                StuffGallaryView.QuantityPlusTapEventHandler = (s, e) => {
                    if (!TapEventHandlingInProgress)
                    {
                        TapEventHandlingInProgress = true;

                        try
                        {
                            App.UniversalLineInApp = 875234162;
                            var Ids = ((string)((TappedEventArgs)e).Parameter).Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                            //App.UniversalLineInApp = 875234163;
                            var StuffId = new Guid(Ids[0]);
                            //App.UniversalLineInApp = 875234164;
                            var BatchNumberId = Ids.Length == 2 ? new Guid(Ids[1]) : new Nullable<Guid>();
                            //App.UniversalLineInApp = 875234165;

                            var StuffModel = AllStuffsData.SingleOrDefault(a => a.StuffId == StuffId);
                            //App.UniversalLineInApp = 875234166;
                            if (!StuffModel.SelectedInGallaryMode)
                                StuffModel.SelectedInGallaryMode = true;
                            else
                                QuantityPlusClicked(StuffId, BatchNumberId);
                            //App.UniversalLineInApp = 875234167;

                            var ShouldBeUnfocused = AllStuffsData.Where(a => a.SelectedInGallaryMode).ToArray().Where(a => a.StuffId != StuffId && a.TotalStuffQuantity == 0).ToArray();
                            //App.UniversalLineInApp = 875234168;
                            foreach (var item in ShouldBeUnfocused)
                                item.SelectedInGallaryMode = false;
                            App.UniversalLineInApp = 875234169;
                        }
                        catch (Exception)
                        { }

                        TapEventHandlingInProgress = false;
                    }
                };
                App.UniversalLineInApp = 875234170;//THISLINE

                StuffGallaryView.QuantityMinusTapEventHandler = (s, e) => {
                    if (!TapEventHandlingInProgress)
                    {
                        TapEventHandlingInProgress = true;

                        try
                        {
                            App.UniversalLineInApp = 875234171;
                            var Ids = ((string)((TappedEventArgs)e).Parameter).Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                            //App.UniversalLineInApp = 875234172;
                            var StuffId = new Guid(Ids[0]);
                            //App.UniversalLineInApp = 875234173;
                            var BatchNumberId = Ids.Length == 2 ? new Guid(Ids[1]) : new Nullable<Guid>();
                            //App.UniversalLineInApp = 875234174;

                            QuantityMinusClicked(StuffId, BatchNumberId);
                            App.UniversalLineInApp = 875234175;
                        }
                        catch (Exception)
                        { }

                        TapEventHandlingInProgress = false;
                    }
                };

                if(Visitors==null)
                   await FillVisitors();

            }
            catch (Exception err)
            {
                App.ShowError("خطا", err.Message, "خوب");
            }
        }

        private void CheckToShowOrderBasketTollbar()
        {
            var OrderStuffsCount = AllStuffsData.Count(a => a.TotalStuffQuantity > 0);
            if (OrderStuffsCount != 0)
            {
                //if (!this.ToolbarItems.Contains(ToolbarItem_OrderBasket))
                //    this.ToolbarItems.Add(ToolbarItem_OrderBasket);

                ToolbarItem_OrderBasket.Icon = "OrderListOK.png";
            }
            else
            {
                if (this.ToolbarItems.Contains(ToolbarItem_OrderBasket))
                    this.ToolbarItems.Remove(ToolbarItem_OrderBasket);
            }

            if (AllStuffGroupsData != null)
            {
                var Groups = AllStuffGroupsData.Select(a => new { Group = a, Stuffs = AllStuffsData.Where(b => !b.IsGroup && b.GroupCode == a.GroupCode).ToArray() }).ToArray();
                foreach (var Group in Groups)
                {
                    var OrderedStuffs = Group.Stuffs.Where(a => a.TotalStuffQuantity > 0);
                    Group.Group.GroupSummary = !OrderedStuffs.Any() ? "" : (
                        OrderedStuffs.Count() + " قلم، " +
                        OrderedStuffs.SelectMany(a => a.HasBatchNumbers ? a.StuffRow_BatchNumberRows.SelectMany(b => b.PackagesData.Where(c => c.Quantity != 0)) : a.PackagesData.Where(b => b.Quantity != 0)).GroupBy(a => a.Package.Name).Select(a => a.Sum(b => b.Quantity) + " " + a.Key).Aggregate((sum, x) => sum + " + " + x) + "، " +
                        (OrderedStuffs.Sum(a => a._Price).GetValueOrDefault(0) == 0 ? "---" : OrderedStuffs.Sum(a => a._Price).GetValueOrDefault(0).ToString("###,###,###,###,###,###,##0.")) + " ریال"
                        ).ToPersianDigits();
                    var GroupInStuffsList = AllStuffsData.SingleOrDefault(a => a.Id == Group.Group.Id);
                    if (GroupInStuffsList != null)
                        GroupInStuffsList.GroupSummary = Group.Group.GroupSummary;
                }
            }
        }

        private void ToolbarItem_GallaryMode_Activated(object sender, EventArgs e)
        {
            FocusedQuantityTextBoxId = null;

            if (this.ToolbarItems.Contains(ToolbarItem_GallaryMode))
                this.ToolbarItems.Remove(ToolbarItem_GallaryMode);
            if (!this.ToolbarItems.Contains(ToolbarItem_StuffListMode))
                this.ToolbarItems.Add(ToolbarItem_StuffListMode);
            InsertOrderForm_ShowGallaryMode.Value = true;
            StuffItems.IsVisible = false;
            GallaryContainer.IsVisible = true;

            RefreshPartnerSection(LastWidth > LastHeight);
        }

        private void ToolbarItem_StuffListMode_Activated(object sender, EventArgs e)
        {
            FocusedQuantityTextBoxId = null;

            if (this.ToolbarItems.Contains(ToolbarItem_StuffListMode))
                this.ToolbarItems.Remove(ToolbarItem_StuffListMode);
            if (!this.ToolbarItems.Contains(ToolbarItem_GallaryMode))
                this.ToolbarItems.Add(ToolbarItem_GallaryMode);
            InsertOrderForm_ShowGallaryMode.Value = false;
            GallaryContainer.IsVisible = false;
            
            //991128
            //StuffItems.IsVisible = true;

            RefreshPartnerSection(LastWidth > LastHeight);
        }

        private void ToolbarItem_OrderBasket_Activated(object sender, EventArgs e)
        {
            //FocusedQuantityTextBoxId = null;

            //var BewforePreviewForm = new OrderBeforePreviewForm(AllStuffsData, SelectedPartner, EditingSaleOrder, PartnerListForm, OrdersForm, SettlementTypeId, Description, this, PartnerChangeButton.IsEnabled, WarehouseId,FromTour)
            //{
            //    StartColor = Color.FromHex("E6EBEF"),
            //    EndColor = Color.FromHex("A6CFED")
            //};
            //Navigation.PushAsync(BewforePreviewForm);
        }
        
        private async void PartnerSelected()
        {
            PartnerLabel.Text = SelectedPartner == null ? "مشتری" :
                !string.IsNullOrEmpty(SelectedPartner.LegalName) ? (SelectedPartner.LegalName + (!string.IsNullOrEmpty(SelectedPartner.Name) ? (" (" + SelectedPartner.Name + ")") : "")) : (SelectedPartner.Name);

            var BeforeSelectedPartnerId = _BeforeSelectedPartner == null ? Guid.NewGuid() : _BeforeSelectedPartner.Id;
            var NewSelectedPartnerId = SelectedPartner == null ? Guid.NewGuid() : SelectedPartner.Id;
            if (SelectedPartner != null && BeforeSelectedPartnerId != NewSelectedPartnerId)
            {
                await FillStuffs(StuffsSearchBar.Text, null, true, false);
                //await FetchPartnerCycleInformationFromServer();
                StuffItems.IsVisible = true;
            }
        }

        private class QuantityEditingStuffModel
        {
            DBRepository.StuffListModel StuffModel;
            
            private decimal _Quantity;
            public decimal Quantity
            {
                get { return _Quantity; }
                set
                {
                    if (_Quantity != value)
                    {
                        if (StuffModel != null)
                        {
                            StuffModel.Quantity = value;
                            _Quantity = StuffModel.Quantity;
                        }
                    }
                }
            }

            public QuantityEditingStuffModel(DBRepository.StuffListModel StuffModel, decimal _Quantity)
            {
                this.StuffModel = StuffModel;
                this._Quantity = _Quantity;
            }
        }
        QuantityEditingStuffModel KeyboardObject;
        Guid[] _FocusedQuantityTextBoxId = null;
        Guid[] FocusedQuantityTextBoxId
        {
            get { return _FocusedQuantityTextBoxId; }
            set
            {
                if (_FocusedQuantityTextBoxId != value)
                {
                    if (_FocusedQuantityTextBoxId != null)
                    {
                        var StuffId = _FocusedQuantityTextBoxId[0];
                        var BatchNumberId = _FocusedQuantityTextBoxId.Length == 2 ? _FocusedQuantityTextBoxId[1] : new Nullable<Guid>();
                        if (AllStuffsData != null)
                        {
                            var StuffModel = AllStuffsData.SingleOrDefault(a => a.StuffId == StuffId);
                            if (StuffModel != null)
                            {
                                if (BatchNumberId.HasValue)
                                    StuffModel = StuffModel.StuffRow_BatchNumberRows.SingleOrDefault(a => a.BatchNumberId == BatchNumberId);
                                if (BatchNumberId.HasValue || !StuffModel.HasBatchNumbers)
                                    StuffModel.QuantityFocused = false;
                            }
                        }
                    }
                    _FocusedQuantityTextBoxId = value;
                    if (_FocusedQuantityTextBoxId != null)
                    {
                        var StuffId = _FocusedQuantityTextBoxId[0];
                        var BatchNumberId = _FocusedQuantityTextBoxId.Length == 2 ? _FocusedQuantityTextBoxId[1] : new Nullable<Guid>();
                        if (AllStuffsData != null)
                        {
                            var StuffModel = AllStuffsData.SingleOrDefault(a => a.StuffId == StuffId);
                            if (StuffModel != null)
                            {
                                if (BatchNumberId.HasValue)
                                    StuffModel = StuffModel.StuffRow_BatchNumberRows.SingleOrDefault(a => a.BatchNumberId == BatchNumberId);
                                if (BatchNumberId.HasValue || !StuffModel.HasBatchNumbers)
                                {
                                    StuffModel.QuantityFocused = true;
                                    KeyboardObject = new QuantityEditingStuffModel(StuffModel, StuffModel.Quantity);
                                    QuantityKeyboard.SetObject(KeyboardObject, a => a.Quantity);
                                    QuantityKeyboard.Show();
                                }
                                else
                                {
                                    QuantityKeyboard.Hide();
                                    if (InsertOrderForm_ShowGallaryMode.Value && StuffModel.HasBatchNumbers)
                                    {
                                        GallaryStuffBatchNumbersListContainer.IsVisible = true;
                                        FillBatchNumbers(StuffId);
                                    }
                                }
                            }
                            else
                                QuantityKeyboard.Hide();
                        }
                        else
                            QuantityKeyboard.Hide();
                    }
                    else
                        QuantityKeyboard.Hide();
                }
            }
        }

        public void UnitNameClicked(Guid StuffId)
        {
            FocusedQuantityTextBoxId = null;
            if (AllStuffsData != null)
            {
                var StuffModel = AllStuffsData.SingleOrDefault(a => a.StuffId == StuffId && !a.BatchNumberId.HasValue);
                if (StuffModel != null)
                {
                    if (StuffModel.PackagesData.Count() > 1)
                    {
                        var CurrentPackageIndex = StuffModel.PackagesData.Select((a, index) => new { a, index }).Single(a => StuffModel.SelectedPackage.Id == a.a.Package.Id).index;
                        var NewPackageIndex = CurrentPackageIndex == StuffModel.PackagesData.Length - 1 ? 0 : CurrentPackageIndex + 1;
                        StuffModel.SelectedPackage = StuffModel.PackagesData[NewPackageIndex].Package;
                    }
                }
            }
        }

        private Guid GetSelectedPackageId(DBRepository.StuffListModel item)
        {
            var CurrentPackageIndex = item.PackagesData.Select((a, index) => new { a, index }).Single(a => item.SelectedPackage.Id == a.a.Package.Id).index;
            //var NewPackageIndex = CurrentPackageIndex == item.PackagesData.Length - 1 ? 0 : CurrentPackageIndex + 1;
            return item.PackagesData[CurrentPackageIndex].Package.Id;
        }

        public static string MultipleRecordsInAllStuffsData_Log;
        public void QuantityPlusClicked(Guid StuffId, Guid? BatchNumberId)
        {
            FocusedQuantityTextBoxId = null;
            if (AllStuffsData != null)
            {
                if(AllStuffsData.Count(a => a.StuffId == StuffId) > 1)
                {
                    MultipleRecordsInAllStuffsData_Log = AllStuffsData.Where(a => a.StuffId == StuffId).Select(a =>
                        "BarCode: " + a.BarCode +
                        ", BatchNumber: " + a.BatchNumber +
                        ", BatchNumberId: " + (a.BatchNumberId.HasValue ? a.BatchNumberId.Value.ToString() : "null") +
                        ", BatchNumberRowHeight: " + a.BatchNumberRowHeight +
                        ", Code: " + a.Code +
                        ", ConsumerFee: " + a.ConsumerFee +
                        ", Description: " + a.Description +
                        ", DisplayGroupName: " + a.DisplayGroupName +
                        ", ExpirationDate: " + a.ExpirationDate +
                        ", Fee: " + a.Fee +
                        ", GallaryTitle: " + a.GallaryTitle +
                        ", GroupCode: " + a.GroupCode +
                        ", GroupName: " + a.GroupName +
                        ", GroupNumber: " + (a.GroupNumber.HasValue ? a.GroupNumber.Value.ToString() : "null") +
                        ", GroupStuffsCount: " + a.GroupStuffsCount +
                        ", GroupSummary: " + a.GroupSummary +
                        ", HasBatchNumbers: " + a.HasBatchNumbers +
                        ", Id: " + a.Id +
                        ", IsGroup: " + a.IsGroup +
                        ", IsGroupOpen: " + a.IsGroupOpen +
                        ", ListGroupRow1Height: " + a.ListGroupRow1Height +
                        ", ListGroupRow2Height: " + a.ListGroupRow2Height +
                        ", ListStuffRowHeight: " + a.ListStuffRowHeight +
                        ", Name: " + a.Name +
                        ", OddRow: " + a.OddRow +
                        ", PackagesData: " + (a.PackagesData.Any() ? "[" + a.PackagesData.Select(b => "PackageId: " + b.Package.Id.ToString() + "_Quantity: " + b.Quantity.ToString()).Aggregate((sum2, x2) => sum2 + ", " + x2) + "]" : "[]") +
                        ", Price: " + a.Price +
                        ", QRScannedThisBatchNumber: " + a.QRScannedThisBatchNumber +
                        ", Quantity: " + a.Quantity +
                        ", QuantityFocused: " + a.QuantityFocused +
                        ", QuantityLabel: " + a.QuantityLabel +
                        ", ReportName: " + a.ReportName +
                        ", RowColor: " + a.RowColor +
                        ", Selected: " + a.Selected +
                        ", SelectedInGallaryMode: " + a.SelectedInGallaryMode +
                        ", SelectedPackageId: " + a.SelectedPackage.Id.ToString() +
                        ", Stock: " + a.Stock +
                        ", StuffDataId: " + a.StuffData.Id.ToString() +
                        ", StuffId: " + a.StuffId.ToString() +
                        ", TotalStuffQuantity: " + a.TotalStuffQuantity +
                        ", UnitName: " + a.UnitName +
                        ", _ConsumerUnitPrice: " + (a._ConsumerUnitPrice.HasValue ? a._ConsumerUnitPrice.Value.ToString() : "null") +
                        ", _Price: " + (a._Price.HasValue ? a._Price.Value.ToString() : "null") +
                        ", _UnitPrice: " + (a._UnitPrice.HasValue ? a._UnitPrice.Value.ToString() : "null") +
                        ", _UnitStock: " + a._UnitStock
                        ).Aggregate((sum, x) => sum + "|" + x);
                }

                var StuffModel = AllStuffsData.SingleOrDefault(a => a.StuffId == StuffId);
                if (StuffModel != null)
                {
                    if (BatchNumberId.HasValue)
                        StuffModel = StuffModel.StuffRow_BatchNumberRows.SingleOrDefault(a => a.BatchNumberId == BatchNumberId);
                    if (BatchNumberId.HasValue || !StuffModel.HasBatchNumbers)
                        StuffModel.Quantity++;
                    if (InsertOrderForm_ShowGallaryMode.Value && StuffModel.HasBatchNumbers)
                    {
                        GallaryStuffBatchNumbersListContainer.IsVisible = true;
                        FillBatchNumbers(StuffId);
                    }
                }
                CheckToShowOrderBasketTollbar();
            }
        }
        public void QuantityMinusClicked(Guid StuffId, Guid? BatchNumberId)
        {
            FocusedQuantityTextBoxId = null;
            if (AllStuffsData != null)
            {
                var StuffModel = AllStuffsData.SingleOrDefault(a => a.StuffId == StuffId);
                if (StuffModel != null)
                {
                    if (BatchNumberId.HasValue)
                        StuffModel = StuffModel.StuffRow_BatchNumberRows.SingleOrDefault(a => a.BatchNumberId == BatchNumberId);
                    if (BatchNumberId.HasValue || !StuffModel.HasBatchNumbers)
                        StuffModel.Quantity = StuffModel.Quantity > 0 ? StuffModel.Quantity - 1 : 0;
                    if (InsertOrderForm_ShowGallaryMode.Value && StuffModel.HasBatchNumbers)
                    {
                        GallaryStuffBatchNumbersListContainer.IsVisible = true;
                        FillBatchNumbers(StuffId);
                    }
                }
                CheckToShowOrderBasketTollbar();
            }
        }

        protected override bool OnBackButtonPressed()
        {
            if (GallaryStuffBatchNumbersListContainer.IsVisible)
                GallaryStuffBatchNumbersListContainer.IsVisible = false;
            else
            {
                Task<bool> action = DisplayAlert("انصراف از ثبت ", "با خروج از این قسمت اطلاعات وارد شده از دست خواهد رفت. آیا مطمئنید؟", "بله", "خیر");
                action.ContinueWith(task =>
                {
                    if (task.Result)
                        Xamarin.Forms.Device.BeginInvokeOnMainThread(() => {
                            try { Navigation.PopAsync(); } catch (Exception) { }
                        });
                });
            }

            return true;
        }

        private void CloseButtonTapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            if (!TapEventHandlingInProgress)
            {
                TapEventHandlingInProgress = true;

                try
                {
                    GallaryStuffBatchNumbersListContainer.IsVisible = false;
                }
                catch (Exception)
                { }

                TapEventHandlingInProgress = false;
            }
        }
        public async void GroupClicked(Guid GroupId)
        {
            FocusedQuantityTextBoxId = null;
            if (AllStuffGroupsData != null)
            {
                var StuffModel = AllStuffGroupsData.SingleOrDefault(a => a.StuffId == GroupId && !a.BatchNumberId.HasValue);
                if (StuffModel != null && StuffModel.IsGroup)
                {
                    if (StuffModel.IsGroupOpen)
                        CloseGroup(StuffModel.StuffId);
                    else
                        OpenGroup(StuffModel.StuffId);
                }
            }
        }

        async void CloseGroup(Guid GroupId)
        {
            var StuffModel = AllStuffGroupsData.SingleOrDefault(a => a.StuffId == GroupId);
            if (StuffModel != null)
                StuffModel.IsGroupOpen = false;
            
            GallaryStuffGroupLabel.Text = GallaryStuffGroupLabelPlaceholder;

            await FillStuffs(StuffsSearchBar.Text, null, false, false);
        }

        async void OpenGroup(Guid GroupId)
        {
            var StuffModel = AllStuffGroupsData.SingleOrDefault(a => a.StuffId == GroupId);
            if (StuffModel != null)
            {
                StuffModel.IsGroupOpen = true;
                GallaryStuffGroupLabel.Text = StuffModel.DisplayGroupName;

                var OtherGroups = AllStuffGroupsData.Where(a => a.StuffId != GroupId && a.IsGroup).ToArray();
                foreach (var item in OtherGroups)
                    item.IsGroupOpen = false;
            }

            await FillStuffs(StuffsSearchBar.Text, null, false, false);

            StuffItems.ScrollTo(AllStuffGroupsData.Single(a => a.StuffId == GroupId), ScrollToPosition.Start, true);
        }

        private async void ToolbarItem_SearchBar_Activated(object sender, EventArgs e)
        {
            if (StuffsSearchBar.IsVisible)
            {
                await StuffsSearchBar.FadeTo(0, 350);
                StuffsSearchBar.IsVisible = false;
                ToolbarItem_SearchBar.Icon = "Search.png";
            }
            else
            {
                StuffsSearchBar.IsVisible = true;
                await StuffsSearchBar.FadeTo(1, 350);
                ToolbarItem_SearchBar.Icon = "ClearSearch.png";
            }
        }

        private async void ToolbarItem_QRSearchBar_Activated(object sender, EventArgs e)
        {
            App.QRScanner.OnScanResult = new Action<QRScanResult>((result) =>
            {
                if(!result.success)
                    App.ToastMessageHandler.ShowMessage("در خواندن بارکد/QR Code خطایی رخ داده است. لطفا مجددا تلاش کنید.", ToastMessageDuration.Long);
                else
                    FillStuffs("ScannedBarcode:" + result.contents, null, false, false);
            });
            App.QRScanner.StartScan();
            if (!App.QRScanner.HasScannerApplication)
                App.ToastMessageHandler.ShowMessage("هیچ اپلیکیشن اسکنری یافت نشد.", ToastMessageDuration.Long);
        }

        private void VisitItems_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            FocusedQuantityTextBoxId = null;
            ((ListView)sender).SelectedItem = null;
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

                QuantityKeyboard.OrientationChanged(width > height);

                RefreshPartnerSection(LastWidth > LastHeight);
            }
        }
        
        void RefreshPartnerSection(bool Horizental)
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

            ShowAccountingCycleOfPartner = false;

            if (Horizental)
            {
                PartnerSection.RowDefinitions.Add(new RowDefinition() { Height = 50 });
                if (ShowAccountingCycleOfPartner)
                    PartnerSection.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

                PartnerSection.ColumnDefinitions.Add(new ColumnDefinition() { Width = 50 });
                PartnerSection.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                var GallaryStuffGroupOffset = 0;
                if (App.StuffListGroupingMethod.Value != 0 && InsertOrderForm_ShowGallaryMode.Value)
                {
                    PartnerSection.ColumnDefinitions.Add(new ColumnDefinition() { Width = 5 });
                    PartnerSection.ColumnDefinitions.Add(new ColumnDefinition() { Width = 50 });
                    PartnerSection.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                    GallaryStuffGroupOffset = 3;
                }
                
                PartnerSection.Children.Add(PartnerChangeButton, 0 + GallaryStuffGroupOffset, 0);
                PartnerSection.Children.Add(PartnerLabel, 1 + GallaryStuffGroupOffset, 0);

                if (ShowAccountingCycleOfPartner)
                {
                    PartnerSection.Children.Add(PartnerRemainderDetailButton, 0 + GallaryStuffGroupOffset, 1);
                    PartnerSection.Children.Add(PartnerRemainderLabel, 1 + GallaryStuffGroupOffset, 1);
                }

                if (App.StuffListGroupingMethod.Value != 0 && InsertOrderForm_ShowGallaryMode.Value)
                {
                    PartnerSection.Children.Add(GallaryStuffGroupPicker, 0, 0);
                    PartnerSection.Children.Add(GallaryStuffGroupChangeButton, 0, 0);
                    PartnerSection.Children.Add(GallaryStuffGroupLabel, 1, 0);
                }
            }
            else
            {
                PartnerSection.RowDefinitions.Add(new RowDefinition() { Height = 50 });
                if (ShowAccountingCycleOfPartner)
                    PartnerSection.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                if (App.StuffListGroupingMethod.Value != 0 && InsertOrderForm_ShowGallaryMode.Value)
                    PartnerSection.RowDefinitions.Add(new RowDefinition() { Height = 50 });

                PartnerSection.ColumnDefinitions.Add(new ColumnDefinition() { Width = 50 });
                PartnerSection.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

                PartnerSection.Children.Add(PartnerChangeButton, 0, 0);
                PartnerSection.Children.Add(PartnerLabel, 1, 0);

                if (ShowAccountingCycleOfPartner)
                {
                    PartnerSection.Children.Add(PartnerRemainderDetailButton, 0, 1);
                    PartnerSection.Children.Add(PartnerRemainderLabel, 1, 1);
                }

                if (App.StuffListGroupingMethod.Value != 0 && InsertOrderForm_ShowGallaryMode.Value)
                {
                    PartnerSection.Children.Add(GallaryStuffGroupPicker, 0, 1 + (ShowAccountingCycleOfPartner ? 1 : 0));
                    PartnerSection.Children.Add(GallaryStuffGroupChangeButton, 0, 1 + (ShowAccountingCycleOfPartner ? 1 : 0));
                    PartnerSection.Children.Add(GallaryStuffGroupLabel, 1, 1 + (ShowAccountingCycleOfPartner ? 1 : 0));
                }
            }
        }

        public List<DBRepository.StuffListModel> AllStuffsData;

        //private void chkIsFreeProduct_Clicked(object sender, EventArgs e)
        //{
        //    var Ids = ((string)((TappedEventArgs)e).Parameter).Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
        //    var StuffId = new Guid(Ids[0]);
        //    var StuffModel = AllStuffsData.SingleOrDefault(a => a.StuffId == StuffId);

        //    if (StuffModel != null)
        //    {
        //        StuffModel.ReversionVATPercent = "";
        //        StuffModel.ReversionDiscountPercent = "";
        //    }
        //}

        public List<DBRepository.StuffListModel> AllStuffGroupsData;
        List<DBRepository.StuffListModel> LastStuffsGroups;

        public async Task FillStuffs()
        {
            await FillStuffs(StuffsSearchBar.Text, null, false, false);
        }
        bool EditingOrderStuffsInitialized = false;
        private async Task FillStuffs(string Filter, SaleOrder EditingOrder, bool RefreshStuffsData, bool WithRefreshingAnimation)
        {
            if (!WarehouseId.HasValue && App.DefineWarehouseForSaleAndBuy.Value)
                return;

            if (WithRefreshingAnimation) StuffItems.IsRefreshing = true;
            await Task.Delay(100);
            if (AllStuffsData == null || RefreshStuffsData)
            {
                //var StuffsResult = await App.DB.GetAllStuffsListAsync(SelectedPartner != null ? SelectedPartner.Id : new Nullable<Guid>(), EditingOrder != null ? EditingOrder.Id : new Nullable<Guid>(), false, WarehouseId);
                var StuffsResult = await App.DB.GetAllStuffsListForReversionAsync();
                if (!StuffsResult.Success)
                {
                    App.ShowError("خطا", "در نمایش لیست کالاها خطایی رخ داد.\n" + StuffsResult.Message, "خوب");
                    if (WithRefreshingAnimation) StuffItems.IsRefreshing = false;
                    return;
                }
                var NewStuffsData = StuffsResult.Data[0];
                var NewStuffGroupsData = StuffsResult.Data[1];
                
                if (AllStuffsData != null)
                {
                    try
                    {
                        foreach (var stuffInList in NewStuffsData)
                        {
                            var CurrentStuffInList = AllStuffsData.SingleOrDefault(a => a.StuffData.Id == stuffInList.StuffId);
                            if (CurrentStuffInList != null)
                            {
                                foreach (var p in CurrentStuffInList.PackagesData)
                                {
                                    stuffInList.SelectedPackage = p.Package;
                                    stuffInList.Quantity = p.Quantity;
                                    foreach (var batchNumberInList in stuffInList.StuffRow_BatchNumberRows)
                                    {
                                        var CurrentBatchNumberInList = CurrentStuffInList.StuffRow_BatchNumberRows.SingleOrDefault(a => a.BatchNumberId == batchNumberInList.BatchNumberId);
                                        if (CurrentBatchNumberInList != null)
                                            batchNumberInList.Quantity = CurrentBatchNumberInList.PackagesData.Single(a => a.Package.Id == p.Package.Id).Quantity;
                                    }
                                }
                                stuffInList.SelectedPackage = CurrentStuffInList.SelectedPackage;
                                stuffInList.Selected = CurrentStuffInList.Selected;
                            }
                        }
                    }
                    catch (Exception err)
                    {
                    }
                }
                
                AllStuffsData = NewStuffsData;
                AllStuffGroupsData = NewStuffGroupsData;
            }
            
            if(EditingOrder != null && !EditingOrderStuffsInitialized)
            {
                var EditingSaleOrderStuffs = EditingOrder.SaleOrderStuffs.Where(a => !a.FreeProduct);
                foreach (var saleOrderStuff in EditingSaleOrderStuffs)
                {
                    var StuffInList = AllStuffsData.SingleOrDefault(a => a.StuffId == saleOrderStuff.Package.StuffId);
                    if(StuffInList != null)
                    {
                        if (StuffInList.HasBatchNumbers)
                            StuffInList = StuffInList.StuffRow_BatchNumberRows.SingleOrDefault(a => a.BatchNumberId == saleOrderStuff.BatchNumberId.GetValueOrDefault(Guid.Empty));
                        if(StuffInList != null)
                        {
                            if (StuffInList.BatchNumberId.HasValue)
                                StuffInList.BatchNumberRow_StuffParentRow.SelectedPackage = saleOrderStuff.Package;
                            else
                                StuffInList.SelectedPackage = saleOrderStuff.Package;
                            StuffInList.Quantity = saleOrderStuff.Quantity;
                        }
                    }
                }
                EditingOrderStuffsInitialized = true;
            }

            foreach (var item in AllStuffsData)
                if (item.TotalStuffQuantity > 0)
                    item.SelectedInGallaryMode = true;

            var FilteredStuffs = await App.DB.FilterStuffsAsync(AllStuffsData, Filter);
            
            if (EditingOrder != null)
                FilteredStuffs = FilteredStuffs.OrderBy(a => a.Quantity == 0).ToList();
            
            var StuffsWithGroupsData = FilteredStuffs.ToList();
            if(AllStuffGroupsData != null)
            {
                var StuffCounts = from g in AllStuffGroupsData
                                  from c in FilteredStuffs.GroupBy(a => a.GroupCode).Select(a => new { GroupCode = a.Key, Count = a.Count() }).ToList().Where(a => a.GroupCode == g.GroupCode)
                                  select new { g, c };
                foreach (var item in StuffCounts)
                    item.g.GroupStuffsCount = item.c.Count;
                    
                LastStuffsGroups = StuffCounts.Select(a => a.g).ToList();

                StuffsWithGroupsData.AddRange(LastStuffsGroups);
                StuffsWithGroupsData = StuffsWithGroupsData.Select((a, index) => new { a, index }).OrderBy(a => a.a.GroupCode).ThenBy(a => !a.a.IsGroup).ThenBy(a => a.index).Select(a => a.a).ToList();

                StuffsWithGroupsData = StuffsWithGroupsData.Where(a => a.IsGroup || LastStuffsGroups.Any(b => b.GroupCode == a.GroupCode && b.IsGroup && b.IsGroupOpen)).ToList();

                GallaryStuffGroupPicker.Items.Clear();
                foreach (var item in LastStuffsGroups)
                    GallaryStuffGroupPicker.Items.Add(item.DisplayGroupName);
            }

            
            try
            {
                var StuffsListTemp = StuffsWithGroupsData.ToList();
                
                var BatchNumbers = StuffsListTemp.Where(a => !a.IsGroup).SelectMany(a => a.StuffRow_BatchNumberRows).ToList();
                
                StuffsListTemp.AddRange(BatchNumbers);
                
                var StuffsListOrderDic = StuffsListTemp.Where(a => !a.BatchNumberId.HasValue).Select((a, index) => new { a, index }).ToDictionary(a => a.a.StuffId, a => a.index);

                foreach (var item in StuffsListTemp)
                {
                    item.OddRow = StuffsListOrderDic[item.StuffId] % 2 == 1;
                }

                StuffsListTemp = StuffsListTemp.OrderBy(a => StuffsListOrderDic[a.StuffId]).ThenBy(a => a.BatchNumberId.HasValue).ToList();

                _StuffsList = new ObservableCollection<DBRepository.StuffListModel>(StuffsListTemp);
               
                //var xxx = _StuffsList.Select(a => a.Code).ToList();
            }
            catch (Exception err)
            {
                var wefwef = err;
            }
            
            StuffItems.ItemsSource = null;
            StuffItems.ItemsSource = _StuffsList;

            if (WithRefreshingAnimation) 
                StuffItems.IsRefreshing = false;

            await Task.Delay(100);

            //var StuffsListForGallaryCarousel = _StuffsList.Where(a => !a.IsGroup && !a.BatchNumberId.HasValue).Select((a, index) => new
            //{
            //    a,
            //    PageIndex = Math.Floor((double)(index / (App.GallaryStuffCount[0] * App.GallaryStuffCount[1])))
            //}).GroupBy(a => a.PageIndex).Select(a => new
            //{
            //    a.Key,
            //    Stuffs = a.Select(b => b.a).ToArray()
            //}).ToArray();
            //StuffGallaryViewGrid.SetPageStuffIds(StuffsListForGallaryCarousel.ToDictionary(a => (int)a.Key, a => a.Stuffs.Select(b => b.StuffId).ToArray()));
            //gallaryCarousel.ItemsSource = new ObservableCollection<DBRepository.StuffListModelArray>(StuffsListForGallaryCarousel.Select(a => new DBRepository.StuffListModelArray() { Stuffs = a.Stuffs }));
            
            if (EditingOrder != null)
                CheckToShowOrderBasketTollbar();

            CheckToShowOrderBasketTollbar();

            FocusedQuantityTextBoxId = null;
        }
        
        private async Task FillBatchNumbers(Guid StuffId)
        {
            await Task.Delay(100);
            
            var _BatchNumbersList = new ObservableCollection<DBRepository.StuffListModel>(AllStuffsData.Single(a => a.StuffId == StuffId).StuffRow_BatchNumberRows.ToList());

            GallaryStuffBatchNumbersList.ItemsSource = null;
            GallaryStuffBatchNumbersList.ItemsSource = _BatchNumbersList;
            
            FocusedQuantityTextBoxId = null;
        }

        private async void SubmitToServer(object sender, System.EventArgs e)
        {
            WaitToggle(false);
            await Task.Delay(100);
            var SaveResult = await SaveReversion();

            if (!SaveResult.Success)
            {
                WaitToggle(false);
                App.ShowError("خطا", SaveResult.Message, "خوب");
            }
            else
            {
                var submitResult = await Connectivity.SubmitReversion( SaveResult.Data);

                if (!submitResult.Success)
                {
                    WaitToggle(false);
                    App.ShowError("خطا", "اطلاعات به صورت محلی ثبت شد اما در ارسال اطلاعات به سرور خطایی رخ داده است: " + submitResult.Message, "خوب");
                }
                else
                {
                    WaitToggle(true);

                    Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count - 1]);

                    App.ToastMessageHandler.ShowMessage("اطلاعات با موفقیت به سرور ارسال شد.", Helpers.ToastMessageDuration.Long);
                    try { await Navigation.PopAsync(); } catch (Exception) { }
                }
            }
        }

        private async void SubmitToStorage(object sender, System.EventArgs e)
        {
            WaitToggle(false);
            await Task.Delay(100);
            var SaveResult = await SaveReversion();

            if (!SaveResult.Success)
            {
                WaitToggle(false);
                App.ShowError("خطا", SaveResult.Message, "خوب");
            }
            else
            {
                WaitToggle(true);
                App.ToastMessageHandler.ShowMessage("اطلاعات با موفقیت به صورت محلی ثبت شد.", Helpers.ToastMessageDuration.Long);
                try { await Navigation.PopAsync(); } catch (Exception) { }
            }
        }

        private async Task<ResultSuccess<Reversion>> SaveReversion()
        {
            try
            {
                if (ReversionReasonPicker.SelectedIndex == -1)
                    return new ResultSuccess<Reversion>(false, "دلیل انتخاب نشده است.");

                if (SelectedPartner==null || SelectedPartner.Id == null)
                    return new ResultSuccess<Reversion>(false, "مشتری انتخاب نشده است.");

                if (VisitorPicker.SelectedIndex == -1)
                    return new ResultSuccess<Reversion>(false, "بازاریاب انتخاب نشده است.");

                if(txtDate.Text?.Length!=10)
                    return new ResultSuccess<Reversion>(false, "تاریخ وارد شده صحیح نیست.");

                if(txtReversionDiscountAmount.Text.ToSafeString()!="" && txtReversionDiscountPercent.Text.ToSafeString()!="")
                        return new ResultSuccess<Reversion>(false, "یکی از مقادیر درصد یا مبلغ تخفیف باید پر شود");

                Guid newId = Guid.NewGuid();

                var newReversion = new Reversion()
                {
                    Id = newId,
                    ReversionDiscountAmount = txtReversionDiscountAmount.Text.ToLatinDigits().ToSafeDecimal(),
                    ReversionDiscountPercent = txtReversionDiscountPercent.Text.ToLatinDigits().ToSafeDecimal(),
                    PartnerId = SelectedPartner.Id,
                    Description=txtDesc.Text.ToSafeString()=="" ? "***" : txtDesc.Text,
                    ReversionDate = txtDate.Text.PersianDateStringToDate(),
                    PersonelId = Visitors[VisitorPicker.SelectedIndex].Id,
                    ReasonId = ReversionReasons[ReversionReasonPicker.SelectedIndex].Id,
                    Stuffs = _StuffsList.Where(a=>a.Quantity>0).Select(a => new ReversionStuff
                    {
                        //BatchNumber = a.BatchNumber,
                        ReversionId = newId,
                        Id = Guid.NewGuid(),
                        DiscountPercent = !a.ReversionIsFreeProduct ? a.ReversionDiscountPercent.ToSafeString().ToLatinDigits().ToSafeDecimal() : 0,
                        IsFreeProduct = a.ReversionIsFreeProduct,
                        Quantity = a.Quantity,
                        VATAmount = a.ReversionVATAmount.ToLatinDigits().ToSafeDecimal(),
                        VATPercent = !a.ReversionIsFreeProduct ?  a.ReversionVATPercent.ToLatinDigits().ToSafeDecimal() : 0,
                        StuffId = a.StuffId,
                        MinorPackagePrice= a.ReversionFee.ToLatinDigits().ToSafeDecimal(), //???
                        ReversionFee = a.ReversionFee.ToLatinDigits().ToSafeDecimal(),
                        PackageId = GetSelectedPackageId(a)
                    }).ToArray()
                };

                var result = await App.DB.InsertOrUpdateRecordAsync<Reversion>(newReversion);
                result = await App.DB.InsertOrUpdateAllRecordsAsync<ReversionStuff>(newReversion.Stuffs);
                
                if (!result.Success)
                    return new ResultSuccess<Reversion>(false, result.Message);
                
                return new ResultSuccess<Reversion>(true, "", newReversion);
            }
            catch (Exception err)
            {
                return new ResultSuccess<Reversion>(false, err.ProperMessage());
            }
        }

        public void WaitToggle(bool FormWorkFinished)
        {
            //if (!BusyIndicatorContainder.IsVisible)
            //{
            //    BusyIndicatorContainder.IsVisible = true;
            //    this.ToolbarItems.Remove(ToolbarItem_LocalSave);
            //    this.ToolbarItems.Remove(ToolbarItem_SendToServer);
            //}
            //else
            //{
            //    BusyIndicatorContainder.IsVisible = false;
            //    if (!FormWorkFinished && !JustShow)
            //    {
            //        this.ToolbarItems.Add(ToolbarItem_LocalSave);
            //        this.ToolbarItems.Add(ToolbarItem_SendToServer);
            //    }
            //}
        }
    }
}
