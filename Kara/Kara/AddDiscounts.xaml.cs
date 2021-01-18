using Kara.Assets;
using Kara.CustomRenderer;
using Kara.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Kara
{
    public class ArticleListModel : INotifyPropertyChanged
    {
        public SaleOrderStuff ArtcileData { get; set; }
        public Guid Id { get { return ArtcileData.Id; } }
        public string StuffCode { get { return ArtcileData.Package.Stuff.Code.ToPersianDigits(); } }
        public string StuffName { get { return ArtcileData.Package.Stuff.Name.ToPersianDigits(); } }
        public string Quantity { get { return (ArtcileData.Quantity.ToString() + " " + ArtcileData.Package.Name).ToPersianDigits(); } }
        public string UnitPrice { get { return ArtcileData.SalePrice.ToString("###,###,###,###,###,###,##0.").ToPersianDigits(); } }
        public string SalePrice { get { return ArtcileData.SalePriceQuantity.ToString("###,###,###,###,###,###,##0.").ToPersianDigits(); } }
        public string CurrentDiscountPercent { get { return " + " + (ArtcileData.DiscountPercent - ArtcileData.AddedDiscountPercent).ToString("##0.##").ToPersianDigits(); } }
        bool PointJustEntered;
        public decimal AddedDiscountPercent
        {
            get { return ArtcileData.AddedDiscountPercent; }
            set
            {
                var NewValue = value;
                PointJustEntered = (double)(value % 1) == (double)0.354168413153848456;
                if (PointJustEntered)
                    NewValue = Math.Floor(NewValue);
                var MinValue = 0;
                var NewDiscountPercent = Convert.ToDecimal((ArtcileData.DiscountPercent - ArtcileData.AddedDiscountPercent + NewValue).ToString("##0.#####################"));
                var NewAddedDiscountPercent = NewValue;

                var Correction = NewDiscountPercent > 100 ? 100 - NewDiscountPercent : NewAddedDiscountPercent < 0 ? -NewAddedDiscountPercent : 0;
                NewDiscountPercent += Correction;
                NewAddedDiscountPercent += Correction;

                ArtcileData.DiscountPercent = NewDiscountPercent;
                ArtcileData.AddedDiscountPercent = NewAddedDiscountPercent;

                OnPropertyChanged("AddedDiscountPercent");
                OnPropertyChanged("AddedDiscountPercentLabel");
                OnPropertyChanged("DiscountAmount");
                OnPropertyChanged("AfterDiscountReduction");
            }
        }
        private bool _AddedDiscountPercentFocused;
        public bool AddedDiscountPercentFocused
        {
            get
            {
                return _AddedDiscountPercentFocused;
            }
            set
            {
                _AddedDiscountPercentFocused = value;
                OnPropertyChanged("AddedDiscountPercentFocused");
                OnPropertyChanged("AddedDiscountPercentLabel");
            }
        }
        public string AddedDiscountPercentLabel { get { return AddedDiscountPercent.ToString().ToPersianDigits().Replace(".", "/") + (PointJustEntered ? "/" : "") + (_AddedDiscountPercentFocused ? "|" : ""); } }
        public string DiscountAmount { get { return ArtcileData.DiscountAmount.ToString("###,###,###,###,###,###,##0.").ToPersianDigits(); } }
        public string AfterDiscountReduction { get { return (ArtcileData.SalePriceQuantity - ArtcileData.DiscountAmount).ToString("###,###,###,###,###,###,##0.").ToPersianDigits(); } }
        
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class CustomArticleListCell : ViewCell
    {
        public static EventHandler AddedDiscountPercentPlusTapEventHandler, AddedDiscountPercentMinusTapEventHandler, AddedDiscountPercentTextBoxTapEventHandler;
        
        public static readonly BindableProperty ArticleIdProperty =
            BindableProperty.Create("ArticleId", typeof(Guid), typeof(CustomArticleListCell), Guid.Empty);
        public Guid ArticleId
        {
            get { return (Guid)GetValue(ArticleIdProperty); }
            set { SetValue(ArticleIdProperty, value); }
        }

        public CustomArticleListCell()
        {
            View = GetView(true);
        }

        public View GetView(bool WithBinding)
        {
            if (WithBinding)
                this.SetBinding(CustomArticleListCell.ArticleIdProperty, "Id");

            Grid GridWrapper = new Grid()
            {
                Padding = new Thickness(5, 0),
                RowSpacing = 1,
                ColumnSpacing = 1,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.FromHex(WithBinding ? "#DCE6FA" : "#0062C4")
            };
            GridWrapper.RowDefinitions.Add(new RowDefinition() { Height = 25 });
            GridWrapper.RowDefinitions.Add(new RowDefinition() { Height = WithBinding ? 45 : 25 });

            GridWrapper.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(4, GridUnitType.Star) });
            GridWrapper.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(4, GridUnitType.Star) });
            GridWrapper.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0.5, GridUnitType.Star) });
            GridWrapper.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            GridWrapper.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });
            GridWrapper.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            GridWrapper.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });
            GridWrapper.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(4, GridUnitType.Star) });

            Label CodeLabel = null,
                NameLabel = null,
                QuantityLabel = null,
                PriceLabel = null,
                CurrentDiscountPercentLabel = null,
                DiscountAmountLabel = null,
                AfterDiscountReductionLabel = null,
                AddedDiscountPercentPlusLabel = null,
                AddedDiscountPercentMinusLabel = null;
            Label AddedDiscountPercentEntry = null;


            CodeLabel = new Label() { LineBreakMode = LineBreakMode.HeadTruncation, TextColor = Color.FromHex(WithBinding ? "222" : "fff"), HorizontalOptions = LayoutOptions.End, HorizontalTextAlignment = TextAlignment.End, VerticalOptions = LayoutOptions.Center, FontSize = 16 };
            NameLabel = new Label() { LineBreakMode = LineBreakMode.TailTruncation, TextColor = Color.FromHex(WithBinding ? "222" : "fff"), HorizontalOptions = LayoutOptions.End, HorizontalTextAlignment = TextAlignment.End, VerticalOptions = LayoutOptions.Center, FontSize = 16 };
            QuantityLabel = new Label() { LineBreakMode = LineBreakMode.TailTruncation, TextColor = Color.FromHex(WithBinding ? "222" : "fff"), HorizontalOptions = LayoutOptions.End, HorizontalTextAlignment = TextAlignment.End, VerticalOptions = LayoutOptions.Center, FontSize = 16 };
            PriceLabel = new Label() { LineBreakMode = LineBreakMode.TailTruncation, TextColor = Color.FromHex(WithBinding ? "222" : "fff"), HorizontalOptions = LayoutOptions.End, HorizontalTextAlignment = TextAlignment.Start, VerticalOptions = LayoutOptions.Center, FontSize = 16 };
            CurrentDiscountPercentLabel = new Label() { LineBreakMode = LineBreakMode.TailTruncation, TextColor = Color.FromHex(WithBinding ? "222" : "fff"), HorizontalOptions = LayoutOptions.End, HorizontalTextAlignment = TextAlignment.Start, VerticalOptions = LayoutOptions.Center, FontSize = 16 };
            AddedDiscountPercentEntry = new MyLabel() { LineBreakMode = LineBreakMode.TailTruncation, TextColor = Color.FromHex("222"), HorizontalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.Center, VerticalOptions = LayoutOptions.Center, VerticalTextAlignment = TextAlignment.Center, FontSize = 16, Margin = 0, BackgroundColor = Color.FromHex("fff"), Padding = new Thickness(10) };
            AddedDiscountPercentPlusLabel = new RightEntryCompanionLabel() { Text = "+", TextColor = Color.FromHex("222"), HorizontalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.Center, VerticalOptions = LayoutOptions.Center, FontSize = 16 };
            AddedDiscountPercentMinusLabel = new LeftEntryCompanionLabel() { Text = "-", TextColor = Color.FromHex("222"), HorizontalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.Center, VerticalOptions = LayoutOptions.Center, FontSize = 16 };
            DiscountAmountLabel = new Label() { LineBreakMode = LineBreakMode.TailTruncation, TextColor = Color.FromHex(WithBinding ? "222" : "fff"), HorizontalOptions = LayoutOptions.End, HorizontalTextAlignment = TextAlignment.Start, VerticalOptions = LayoutOptions.Center, FontSize = 16 };
            AfterDiscountReductionLabel = new Label() { LineBreakMode = LineBreakMode.TailTruncation, TextColor = Color.FromHex(WithBinding ? "222" : "fff"), HorizontalOptions = LayoutOptions.End, HorizontalTextAlignment = TextAlignment.Start, VerticalOptions = LayoutOptions.Center, FontSize = 16 };
            
            GridWrapper.Children.Add(CodeLabel, 7, 0);
            GridWrapper.Children.Add(NameLabel, 1, 0);
            Grid.SetColumnSpan(NameLabel, 6);
            GridWrapper.Children.Add(QuantityLabel, 0, 0);

            GridWrapper.Children.Add(AfterDiscountReductionLabel, 0, 1);
            if (WithBinding)
            {
                GridWrapper.Children.Add(DiscountAmountLabel, 1, 1);
                GridWrapper.Children.Add(AddedDiscountPercentMinusLabel, 3, 1);
                GridWrapper.Children.Add(AddedDiscountPercentEntry, 4, 1);
                GridWrapper.Children.Add(AddedDiscountPercentPlusLabel, 5, 1);
                GridWrapper.Children.Add(CurrentDiscountPercentLabel, 6, 1);
            }
            else
            {
                GridWrapper.Children.Add(DiscountAmountLabel, 1, 1);
                GridWrapper.Children.Add(CurrentDiscountPercentLabel, 3, 1);
                Grid.SetColumnSpan(CurrentDiscountPercentLabel, 4);
            }
            GridWrapper.Children.Add(PriceLabel, 7, 1);

            if (WithBinding)
            {
                CodeLabel.SetBinding(Label.TextProperty, "StuffCode");
                NameLabel.SetBinding(Label.TextProperty, "StuffName");
                QuantityLabel.SetBinding(Label.TextProperty, "Quantity");
                PriceLabel.SetBinding(Label.TextProperty, "SalePrice");
                CurrentDiscountPercentLabel.SetBinding(Label.TextProperty, "CurrentDiscountPercent");
                AddedDiscountPercentEntry.SetBinding(Label.TextProperty, "AddedDiscountPercentLabel");
                DiscountAmountLabel.SetBinding(Label.TextProperty, "DiscountAmount");
                AfterDiscountReductionLabel.SetBinding(Label.TextProperty, "AfterDiscountReduction");
            }
            else
            {
                CodeLabel.Text = "کد کالا";
                NameLabel.Text = "نام کالا";
                QuantityLabel.Text = "تعداد";
                PriceLabel.Text = "مبلغ ناخالص";
                CurrentDiscountPercentLabel.Text = "تخفیف(%)";
                DiscountAmountLabel.Text = "تخفیف($)";
                AfterDiscountReductionLabel.Text = "با کسر تخفیف";
            }

            var AddedDiscountPercentPlusTapGestureRecognizer = new TapGestureRecognizer();
            AddedDiscountPercentPlusTapGestureRecognizer.Tapped += AddedDiscountPercentPlusTapEventHandler;
            AddedDiscountPercentPlusTapGestureRecognizer.SetBinding(TapGestureRecognizer.CommandParameterProperty, "Id");
            AddedDiscountPercentPlusLabel.GestureRecognizers.Add(AddedDiscountPercentPlusTapGestureRecognizer);

            var AddedDiscountPercentMinusTapGestureRecognizer = new TapGestureRecognizer();
            AddedDiscountPercentMinusTapGestureRecognizer.Tapped += AddedDiscountPercentMinusTapEventHandler;
            AddedDiscountPercentMinusTapGestureRecognizer.SetBinding(TapGestureRecognizer.CommandParameterProperty, "Id");
            AddedDiscountPercentMinusLabel.GestureRecognizers.Add(AddedDiscountPercentMinusTapGestureRecognizer);

            var AddedDiscountPercentTextBoxTapGestureRecognizer = new TapGestureRecognizer();
            AddedDiscountPercentTextBoxTapGestureRecognizer.Tapped += AddedDiscountPercentTextBoxTapEventHandler;
            AddedDiscountPercentTextBoxTapGestureRecognizer.SetBinding(TapGestureRecognizer.CommandParameterProperty, "Id");
            AddedDiscountPercentEntry.GestureRecognizers.Add(AddedDiscountPercentTextBoxTapGestureRecognizer);

            return GridWrapper;
        }
    }

    public partial class AddDiscounts : GradientContentPage
    {
        SaleOrderAddedDiscountModel SaleOrder;
        ObservableCollection<ArticleListModel> ArticlesData;
        private OrderPreviewForm OrderPreviewForm;
        MyKeyboard<AddedDiscountPercentEditingArticleModel, decimal> RowAddedDiscountPercentKeyboard;
        MyKeyboard<AddedDiscountPercentEditingOrderModel, decimal> OrderAddedDiscountPercentKeyboard;
        
        public class SaleOrderAddedDiscountModel : INotifyPropertyChanged
        {
            public SaleOrder SaleOrder { get; set; }

            bool PointJustEntered;
            public decimal AddedDiscountPercent
            {
                get { return SaleOrder.AddedDiscountPercent; }
                set
                {
                    var NewValue = value;
                    PointJustEntered = (double)(value % 1) == (double)0.354168413153848456;
                    if (PointJustEntered)
                        NewValue = Math.Floor(NewValue);
                    var MinValue = 0;
                    var NewDiscountPercent = Convert.ToDecimal((SaleOrder.DiscountPercent - SaleOrder.AddedDiscountPercent + NewValue).ToString("##0.#####################"));
                    var NewAddedDiscountPercent = NewValue;

                    var Correction = NewDiscountPercent > 100 ? 100 - NewDiscountPercent : NewAddedDiscountPercent < 0 ? -NewAddedDiscountPercent : 0;
                    NewDiscountPercent += Correction;
                    NewAddedDiscountPercent += Correction;

                    SaleOrder.DiscountPercent = NewDiscountPercent;
                    SaleOrder.AddedDiscountPercent = NewAddedDiscountPercent;

                    OnPropertyChanged("AddedDiscountPercent");
                    OnPropertyChanged("AddedDiscountPercentLabel");
                    OnPropertyChanged("DiscountAmount");
                }
            }

            private bool _AddedDiscountPercentFocused;
            public bool AddedDiscountPercentFocused
            {
                get
                {
                    return _AddedDiscountPercentFocused;
                }
                set
                {
                    _AddedDiscountPercentFocused = value;
                    OnPropertyChanged("AddedDiscountPercentFocused");
                    OnPropertyChanged("AddedDiscountPercentLabel");
                }
            }
            public string AddedDiscountPercentLabel { get { return SaleOrder.AddedDiscountPercent.ToString().ToPersianDigits().Replace(".", "/") + (PointJustEntered ? "/" : "") + (_AddedDiscountPercentFocused ? "|" : ""); } }

            public event PropertyChangedEventHandler PropertyChanged;
            public void OnPropertyChanged(string propertyName)
            {
                var handler = PropertyChanged;
                if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public AddDiscounts(SaleOrder saleOrder, OrderPreviewForm OrderPreviewForm)
        {
            InitializeComponent();

            SaleOrder = new SaleOrderAddedDiscountModel() { SaleOrder = saleOrder };
            OrderAddedDiscountPercent.BindingContext = SaleOrder;
            OrderAddedDiscountPercent.SetBinding(Label.TextProperty, new Binding() { Mode = BindingMode.OneWay, Path = "AddedDiscountPercentLabel" });

            this.OrderPreviewForm = OrderPreviewForm;
            
            ArticlesList.HasUnevenRows = true;
            ArticlesList.SeparatorColor = Color.FromHex("A5ABB7");
            ArticlesList.ItemSelected += ArticlesList_ItemSelected;
            CustomArticleListCell.AddedDiscountPercentPlusTapEventHandler = (s, e) => {
                var ArticleId = (Guid)((TappedEventArgs)e).Parameter;
                AddedDiscountPercentPlusClicked(ArticleId);
            };
            CustomArticleListCell.AddedDiscountPercentMinusTapEventHandler = (s, e) => {
                var ArticleId = (Guid)((TappedEventArgs)e).Parameter;
                AddedDiscountPercentMinusClicked(ArticleId);
            };
            CustomArticleListCell.AddedDiscountPercentTextBoxTapEventHandler = async (s, e) =>
            {
                var ArticleId = (Guid)((TappedEventArgs)e).Parameter;
                FocusedAddedDiscountPercentTextBoxArticleId = ArticleId;
                var ArticleRow = ArticlesData.SingleOrDefault(a => a.ArtcileData.Id == ArticleId);
                if (ArticleRow != null)
                {
                    await Task.Delay(200);
                    ArticlesList.ScrollTo(ArticleRow, ScrollToPosition.Start, true);
                }
            };
            ArticlesList.ItemTemplate = new DataTemplate(typeof(CustomArticleListCell));
            ArticlesListHeader.Children.Add(new CustomArticleListCell().GetView(false));

            ShowingOrder = false;

            FillArticlesData();

            CalculateTotalAddedDiscount();

            var OrderAddedDiscountPercentPlusTapGestureRecognizer = new TapGestureRecognizer();
            OrderAddedDiscountPercentPlusTapGestureRecognizer.Tapped += (s, e) => {
                var NewDiscountPercent = SaleOrder.SaleOrder.DiscountPercent + 1;
                var NewAddedDiscountPercent = SaleOrder.AddedDiscountPercent + 1;
                var Correction = NewDiscountPercent > 100 ? 100 - NewDiscountPercent : NewAddedDiscountPercent < 0 ? -NewAddedDiscountPercent : 0;
                NewDiscountPercent += Correction;
                NewAddedDiscountPercent += Correction;

                //SaleOrder.SaleOrder.DiscountPercent = NewDiscountPercent;
                SaleOrder.AddedDiscountPercent = NewAddedDiscountPercent;

                CalculateTotalAddedDiscount();
            };
            OrderAddedDiscountPercentPlusLabel.GestureRecognizers.Add(OrderAddedDiscountPercentPlusTapGestureRecognizer);

            var OrderAddedDiscountPercentMinusTapGestureRecognizer = new TapGestureRecognizer();
            OrderAddedDiscountPercentMinusTapGestureRecognizer.Tapped += (s, e) => {
                var NewDiscountPercent = SaleOrder.SaleOrder.DiscountPercent - 1;
                var NewAddedDiscountPercent = SaleOrder.AddedDiscountPercent - 1;
                var Correction = NewDiscountPercent > 100 ? 100 - NewDiscountPercent : NewAddedDiscountPercent < 0 ? -NewAddedDiscountPercent : 0;
                NewDiscountPercent += Correction;
                NewAddedDiscountPercent += Correction;

                //SaleOrder.SaleOrder.DiscountPercent = NewDiscountPercent;
                SaleOrder.AddedDiscountPercent = NewAddedDiscountPercent;

                CalculateTotalAddedDiscount();
            };
            OrderAddedDiscountPercentMinusLabel.GestureRecognizers.Add(OrderAddedDiscountPercentMinusTapGestureRecognizer);

            var OrderAddedDiscountPercentTapGestureRecognizer = new TapGestureRecognizer();
            OrderAddedDiscountPercentTapGestureRecognizer.Tapped += (s, e) => {
                SaleOrder.AddedDiscountPercentFocused = true;
                
                OrderKeyboardObject = new AddedDiscountPercentEditingOrderModel(SaleOrder, SaleOrder != null ? SaleOrder.AddedDiscountPercent : 0);
                OrderAddedDiscountPercentKeyboard.SetObject(OrderKeyboardObject, a => a.AddedDiscountPercent);
                OrderAddedDiscountPercentKeyboard.Show();

                FocusedAddedDiscountPercentTextBoxArticleId = null;
            };
            OrderAddedDiscountPercent.GestureRecognizers.Add(OrderAddedDiscountPercentTapGestureRecognizer);
            
            RowAddedDiscountPercentKeyboard = new MyKeyboard<AddedDiscountPercentEditingArticleModel, decimal>
            (
                RowAddedDiscountPercentKeyboardHolder,
                new Command((parameter) => {        //OnOK
                    FocusedAddedDiscountPercentTextBoxArticleId = null;
                    CalculateTotalAddedDiscount();
                }),
                new Command((parameter) => {        //OnChange
                    CalculateTotalAddedDiscount();
                })
            );

            OrderAddedDiscountPercentKeyboard = new MyKeyboard<AddedDiscountPercentEditingOrderModel, decimal>
            (
                OrderAddedDiscountPercentKeyboardHolder,
                new Command((parameter) => {        //OnOK
                    SaleOrder.AddedDiscountPercentFocused = false;
                    OrderAddedDiscountPercentKeyboard.Hide();
                    CalculateTotalAddedDiscount();
                }),
                new Command((parameter) => {        //OnChange
                    CalculateTotalAddedDiscount();
                })
            );
        }

        class AddedDiscountPercentEditingOrderModel
        {
            SaleOrderAddedDiscountModel SaleOrderModel;

            private decimal _AddedDiscountPercent;
            public decimal AddedDiscountPercent
            {
                get { return _AddedDiscountPercent; }
                set
                {
                    _AddedDiscountPercent = value;
                    if (SaleOrderModel != null)
                    {
                        SaleOrderModel.AddedDiscountPercent = _AddedDiscountPercent;
                        _AddedDiscountPercent = SaleOrderModel.AddedDiscountPercent;
                    }
                }
            }

            public AddedDiscountPercentEditingOrderModel(SaleOrderAddedDiscountModel SaleOrderModel, decimal _AddedDiscountPercent)
            {
                this.SaleOrderModel = SaleOrderModel;
                this._AddedDiscountPercent = _AddedDiscountPercent;
            }
        }
        AddedDiscountPercentEditingOrderModel OrderKeyboardObject;

        class AddedDiscountPercentEditingArticleModel
        {
            ArticleListModel ArticleModel;

            private decimal _AddedDiscountPercent;
            public decimal AddedDiscountPercent
            {
                get { return _AddedDiscountPercent; }
                set
                {
                    //if (_AddedDiscountPercent.ToString() != value.ToString())
                    //{
                        _AddedDiscountPercent = value;
                        if (ArticleModel != null)
                        {
                            ArticleModel.AddedDiscountPercent = _AddedDiscountPercent;
                            _AddedDiscountPercent = ArticleModel.AddedDiscountPercent;
                        }
                    //}
                }
            }

            public AddedDiscountPercentEditingArticleModel(ArticleListModel ArticleModel, decimal _AddedDiscountPercent)
            {
                this.ArticleModel = ArticleModel;
                this._AddedDiscountPercent = _AddedDiscountPercent;
            }
        }
        AddedDiscountPercentEditingArticleModel KeyboardObject;
        Guid? _FocusedAddedDiscountPercentTextBoxArticleId = null;
        Guid? FocusedAddedDiscountPercentTextBoxArticleId
        {
            get { return _FocusedAddedDiscountPercentTextBoxArticleId; }
            set
            {
                if (_FocusedAddedDiscountPercentTextBoxArticleId != value)
                {
                    if (_FocusedAddedDiscountPercentTextBoxArticleId.HasValue)
                    {
                        if (ArticlesData != null)
                        {
                            var ArticleModel = ArticlesData.SingleOrDefault(a => a.Id == _FocusedAddedDiscountPercentTextBoxArticleId);
                            if (ArticleModel != null)
                                ArticleModel.AddedDiscountPercentFocused = false;
                        }
                    }
                    _FocusedAddedDiscountPercentTextBoxArticleId = value;
                    if (_FocusedAddedDiscountPercentTextBoxArticleId.HasValue)
                    {
                        if (ArticlesData != null)
                        {
                            var ArticleModel = ArticlesData.SingleOrDefault(a => a.Id == _FocusedAddedDiscountPercentTextBoxArticleId);
                            if (ArticleModel != null)
                                ArticleModel.AddedDiscountPercentFocused = true;
                            KeyboardObject = new AddedDiscountPercentEditingArticleModel(ArticleModel, ArticleModel != null ? ArticleModel.AddedDiscountPercent : 0);
                            RowAddedDiscountPercentKeyboard.SetObject(KeyboardObject, a => a.AddedDiscountPercent);
                            RowAddedDiscountPercentKeyboard.Show();

                            SaleOrder.AddedDiscountPercentFocused = false;
                            OrderAddedDiscountPercentKeyboard.Hide();
                        }
                        else
                            RowAddedDiscountPercentKeyboard.Hide();
                    }
                    else
                        RowAddedDiscountPercentKeyboard.Hide();
                }
            }
        }
        public void AddedDiscountPercentPlusClicked(Guid ArticleId)
        {
            FocusedAddedDiscountPercentTextBoxArticleId = null;
            if (ArticlesData != null)
            {
                var ArticleModel = ArticlesData.SingleOrDefault(a => a.Id == ArticleId);
                if (ArticleModel != null)
                    ArticleModel.AddedDiscountPercent++;

                CalculateTotalAddedDiscount();
            }
        }
        public void AddedDiscountPercentMinusClicked(Guid ArticleId)
        {
            FocusedAddedDiscountPercentTextBoxArticleId = null;
            if (ArticlesData != null)
            {
                var ArticleModel = ArticlesData.SingleOrDefault(a => a.Id == ArticleId);
                if (ArticleModel != null)
                    ArticleModel.AddedDiscountPercent--;

                CalculateTotalAddedDiscount();
            }
        }
        
        bool ShowingOrder = false;
        Guid LastCalculateTotalAddedDiscountId = Guid.Empty;
        async void CalculateTotalAddedDiscount()
        {
            App.DB.CalculateProporatedDiscount(SaleOrder.SaleOrder);

            RowsDiscountAmountSum_Current.Text = (SaleOrder.SaleOrder.SaleOrderStuffs.Any() ? SaleOrder.SaleOrder.SaleOrderStuffs.Sum(a => a.DiscountAmount - a.AddedDiscountAmount) : 0).ToString("###,###,###,###,###,###,##0.").ToPersianDigits();
            RowsDiscountAmountSum_Added.Text = (SaleOrder.SaleOrder.SaleOrderStuffs.Any() ? SaleOrder.SaleOrder.SaleOrderStuffs.Sum(a => a.AddedDiscountAmount) : 0).ToString("###,###,###,###,###,###,##0.").ToPersianDigits();
            RowsDiscountAmountSum_Total.Text = (SaleOrder.SaleOrder.SaleOrderStuffs.Any() ? SaleOrder.SaleOrder.SaleOrderStuffs.Sum(a => a.DiscountAmount) : 0).ToString("###,###,###,###,###,###,##0.").ToPersianDigits();

            CurrentOrderDiscountPercent.Text = (SaleOrder.SaleOrder.DiscountPercent - SaleOrder.AddedDiscountPercent).ToString("##0.##").ToPersianDigits() + " %";
            //OrderAddedDiscountPercent.Text = SaleOrder.SaleOrder.AddedDiscountPercent.ToString("##0.##").ReplaceLatinDigits();

            OrderDiscountAmountSum_Current.Text = (SaleOrder.SaleOrder.DiscountAmount - SaleOrder.SaleOrder.AddedDiscountAmount).ToString("###,###,###,###,###,###,##0.").ToPersianDigits();
            OrderDiscountAmountSum_Added.Text = SaleOrder.SaleOrder.AddedDiscountAmount.ToString("###,###,###,###,###,###,##0.").ToPersianDigits();
            OrderDiscountAmountSum_Total.Text = SaleOrder.SaleOrder.DiscountAmount.ToString("###,###,###,###,###,###,##0.").ToPersianDigits();

            TotalDiscountAmountSum_Current.Text = (SaleOrder.SaleOrder.DiscountAmount - SaleOrder.SaleOrder.AddedDiscountAmount + (SaleOrder.SaleOrder.SaleOrderStuffs.Any() ? SaleOrder.SaleOrder.SaleOrderStuffs.Sum(a => a.DiscountAmount - a.AddedDiscountAmount) : 0)).ToString("###,###,###,###,###,###,##0.").ToPersianDigits();
            TotalDiscountAmountSum_Added.Text = (SaleOrder.SaleOrder.AddedDiscountAmount + (SaleOrder.SaleOrder.SaleOrderStuffs.Any() ? SaleOrder.SaleOrder.SaleOrderStuffs.Sum(a => a.AddedDiscountAmount) : 0)).ToString("###,###,###,###,###,###,##0.").ToPersianDigits();
            TotalDiscountAmountSum_Total.Text = (SaleOrder.SaleOrder.DiscountAmount + (SaleOrder.SaleOrder.SaleOrderStuffs.Any() ? SaleOrder.SaleOrder.SaleOrderStuffs.Sum(a => a.DiscountAmount) : 0)).ToString("###,###,###,###,###,###,##0.").ToPersianDigits();
            
            if (ShowingOrder)
            {
                var ThisCalculateTotalAddedDiscountId = Guid.NewGuid();
                LastCalculateTotalAddedDiscountId = ThisCalculateTotalAddedDiscountId;
                await Task.Delay(1000);
                if (LastCalculateTotalAddedDiscountId == ThisCalculateTotalAddedDiscountId)
                {
                    OrderPreviewForm.ShowPreviewOnAppearing = false;
                    OrderPreviewForm.ShowOrder(true);
                    await Task.Delay(1000);
                    OrderPreviewForm.ShowPreviewOnAppearing = true;
                }
            }
        }

        private void ArticlesList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            FocusedAddedDiscountPercentTextBoxArticleId = null;
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

                RowAddedDiscountPercentKeyboard.OrientationChanged(width > height);
                OrderAddedDiscountPercentKeyboard.OrientationChanged(width > height);
            }
        }

        async void FillArticlesData()
        {
            ArticlesList.IsRefreshing = true;
            await Task.Delay(100);
            ArticlesData = new ObservableCollection<ArticleListModel>(SaleOrder.SaleOrder.SaleOrderStuffs.Where(a => !a.FreeProduct).Select(a => new ArticleListModel()
            {
                ArtcileData = a
            }));
            ArticlesList.ItemsSource = null;
            ArticlesList.ItemsSource = ArticlesData;

            ArticlesList.IsRefreshing = false;

            ShowingOrder = true;
            FocusedAddedDiscountPercentTextBoxArticleId = null;
        }
    }
}
