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
    public class FreeProductArticleListModel : INotifyPropertyChanged
    {
        public Guid? ArticleId { get; set; }
        public Stuff StuffData { get; set; }
        public Guid StuffId { get { return StuffData.Id; } }
        public Package[] PackagesData { get; set; }
        private Package _SelectedPackage { get; set; }
        public Package SelectedPackage {
            get { return _SelectedPackage; }
            set {
                _SelectedPackage = value;
                OnPropertyChanged("UnitName");
                OnPropertyChanged("Stock");
                OnPropertyChanged("Fee");
                OnPropertyChanged("Price");
            }
        }
        public string UnitName { get { return _SelectedPackage.Name.ReplaceLatinDigits(); } }
        public string StuffCode { get { return StuffData.Code.ReplaceLatinDigits(); } }
        public string StuffName { get { return StuffData.Name.ReplaceLatinDigits(); } }
        public decimal _UnitStock { get; set; }
        public decimal _CurrentQuantity { get; set; }
        public string CurrentQuantity { get { return " + " + _CurrentQuantity.ToString().ReplaceLatinDigits(); } }
        bool PointJustEntered;
        private decimal _AddedQuantity;
        public decimal AddedQuantity
        {
            get { return _AddedQuantity; }
            set
            {
                var NewValue = value;
                PointJustEntered = (double)(value % 1) == (double)0.354168413153848456;
                if (PointJustEntered)
                    NewValue = Math.Floor(NewValue);
                _AddedQuantity = Convert.ToDecimal(NewValue.ToString("##0.#####################"));
                
                OnPropertyChanged("AddedQuantity");
                OnPropertyChanged("AddedQuantityLabel");
            }
        }
        private bool _AddedQuantityFocused;
        public bool AddedQuantityFocused
        {
            get
            {
                return _AddedQuantityFocused;
            }
            set
            {
                _AddedQuantityFocused = value;
                OnPropertyChanged("AddedQuantityFocused");
                OnPropertyChanged("AddedQuantityLabel");
            }
        }
        public string AddedQuantityLabel { get { return AddedQuantity.ToString().ReplaceLatinDigits().Replace(".", "/") + (PointJustEntered ? "/" : "") + (_AddedQuantityFocused ? "|" : ""); } }
        public decimal Quantity { get { return _CurrentQuantity + _AddedQuantity; } }
        public decimal? _UnitPrice { get; set; }
        public decimal? _ConsumerUnitPrice { get; set; }
        public string Price { get { return _UnitPrice.HasValue ? ((_CurrentQuantity + _AddedQuantity) * _UnitPrice.GetValueOrDefault(0) * _SelectedPackage.Coefficient).ToString("###,###,###,###,###,###,##0.").ReplaceLatinDigits() : "---"; } }
        public string Stock { get { return (_UnitStock < 0 ? "0" : Math.Round(_UnitStock / _SelectedPackage.Coefficient).ToString("###,###,###,##0.")).ReplaceLatinDigits(); } }
        public string Fee { get { return !_UnitPrice.HasValue ? "---" : (_UnitPrice.Value * _SelectedPackage.Coefficient).ToString("###,###,###,###,###,###,##0.").ReplaceLatinDigits(); } }
        public string ConsumerFee { get { return !_ConsumerUnitPrice.HasValue ? "---" : (_ConsumerUnitPrice.Value * _SelectedPackage.Coefficient).ToString("###,###,###,###,###,###,##0.").ReplaceLatinDigits(); } }
        
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class CustomFreeProductStuffListCell : ViewCell
    {
        public static EventHandler UnitNameTapEventHandler, QuantityPlusTapEventHandler, QuantityMinusTapEventHandler, AddedQuantityTextBoxTapEventHandler;
        //public static ICommand OnQuantityChanged;
        public static readonly BindableProperty StuffIdProperty =
            BindableProperty.Create("StuffId", typeof(Guid), typeof(CustomFreeProductStuffListCell), Guid.Empty);
        public Guid StuffId
        {
            get { return (Guid)GetValue(StuffIdProperty); }
            set { SetValue(StuffIdProperty, value); }
        }

        public CustomFreeProductStuffListCell()
        {
            View = GetView(true);
        }

        public View GetView(bool WithBinding)
        {
            if (WithBinding)
                this.SetBinding(CustomFreeProductStuffListCell.StuffIdProperty, "StuffId");

            Grid GridWrapper = new Grid()
            {
                Padding = new Thickness(5, 0),
                RowSpacing = 1,
                ColumnSpacing = 1,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.FromHex(WithBinding ? "#DCE6FA" : "#0062C4")
            };
            GridWrapper.RowDefinitions.Add(new RowDefinition() { Height = WithBinding ? 45 : 25 });
            GridWrapper.RowDefinitions.Add(new RowDefinition() { Height = WithBinding ? 45 : 25 });

            GridWrapper.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(3, GridUnitType.Star) });
            GridWrapper.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(3, GridUnitType.Star) });
            GridWrapper.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0.5, GridUnitType.Star) });
            GridWrapper.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            GridWrapper.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });
            GridWrapper.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            GridWrapper.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            GridWrapper.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Star) });

            Label CodeLabel = null, NameLabel = null, UnitNameLabel = null, StockLabel = null, FeeLabel = null, PriceLabel = null, QuantityLabel = null, AddedQuantityPlusLabel = null, AddedQuantityMinusLabel = null;
            Label AddedQuantityEntry = null;

            CodeLabel = new Label() { LineBreakMode = LineBreakMode.HeadTruncation, TextColor = Color.FromHex(WithBinding ? "222" : "fff"), HorizontalOptions = LayoutOptions.End, HorizontalTextAlignment = TextAlignment.End, VerticalOptions = LayoutOptions.Center, FontSize = 16 };
            NameLabel = new Label() { LineBreakMode = LineBreakMode.TailTruncation, TextColor = Color.FromHex(WithBinding ? "222" : "fff"), HorizontalOptions = LayoutOptions.End, HorizontalTextAlignment = TextAlignment.End, VerticalOptions = LayoutOptions.Center, FontSize = 16 };
            UnitNameLabel = WithBinding ? new FullRoundedLabel() { LineBreakMode = LineBreakMode.TailTruncation, TextColor = Color.FromHex(WithBinding ? "222" : "fff"), HorizontalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.End, VerticalOptions = LayoutOptions.Center, VerticalTextAlignment = TextAlignment.Center, FontSize = 16, Margin = 0 }
                : new Label() { LineBreakMode = LineBreakMode.TailTruncation, TextColor = Color.FromHex(WithBinding ? "222" : "fff"), HorizontalOptions = LayoutOptions.End, HorizontalTextAlignment = TextAlignment.End, VerticalOptions = LayoutOptions.Center, FontSize = 16 };
            StockLabel = new Label() { LineBreakMode = LineBreakMode.TailTruncation, TextColor = Color.FromHex(WithBinding ? "222" : "fff"), HorizontalOptions = LayoutOptions.End, HorizontalTextAlignment = TextAlignment.End, VerticalOptions = LayoutOptions.Center, FontSize = 16 };
            FeeLabel = new Label() { LineBreakMode = LineBreakMode.TailTruncation, TextColor = Color.FromHex(WithBinding ? "1845A5" : "fff"), HorizontalOptions = LayoutOptions.End, HorizontalTextAlignment = TextAlignment.End, VerticalOptions = LayoutOptions.Center, FontSize = 16 };
            PriceLabel = new Label() { LineBreakMode = LineBreakMode.TailTruncation, TextColor = Color.FromHex(WithBinding ? "1845A5" : "fff"), HorizontalOptions = LayoutOptions.End, HorizontalTextAlignment = TextAlignment.End, VerticalOptions = LayoutOptions.Center, FontSize = 16 };
            QuantityLabel = new Label() { LineBreakMode = LineBreakMode.TailTruncation, TextColor = Color.FromHex(WithBinding ? "222" : "fff"), HorizontalOptions = LayoutOptions.End, HorizontalTextAlignment = TextAlignment.End, VerticalOptions = LayoutOptions.Center, FontSize = 16 };
            AddedQuantityPlusLabel = new RightEntryCompanionLabel() { LineBreakMode = LineBreakMode.TailTruncation, Text = "+", TextColor = Color.FromHex("222"), HorizontalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.Center, VerticalOptions = LayoutOptions.Center, VerticalTextAlignment = TextAlignment.Center, FontSize = 16, Margin = 1 };
            AddedQuantityMinusLabel = new LeftEntryCompanionLabel() { LineBreakMode = LineBreakMode.TailTruncation, Text = "-", TextColor = Color.FromHex("222"), HorizontalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.Center, VerticalOptions = LayoutOptions.Center, VerticalTextAlignment = TextAlignment.Center, FontSize = 16, Margin = 1 };
            AddedQuantityEntry = new MyLabel() { LineBreakMode = LineBreakMode.TailTruncation, TextColor = Color.FromHex("222"), HorizontalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.Center, VerticalOptions = LayoutOptions.Center, VerticalTextAlignment = TextAlignment.Center, FontSize = 16, Margin = 0, BackgroundColor = Color.FromHex("fff"), Padding = new Thickness(10) };

            GridWrapper.Children.Add(CodeLabel, 7, 0);
            GridWrapper.Children.Add(NameLabel, 1, 0);
            Grid.SetColumnSpan(NameLabel, 6);
            GridWrapper.Children.Add(UnitNameLabel, 0, 0);

            if (WithBinding)
            {
                GridWrapper.Children.Add(PriceLabel, 0, 1);
                GridWrapper.Children.Add(FeeLabel, 1, 1);
                GridWrapper.Children.Add(AddedQuantityMinusLabel, 3, 1);
                GridWrapper.Children.Add(AddedQuantityEntry, 4, 1);
                GridWrapper.Children.Add(AddedQuantityPlusLabel, 5, 1);
                GridWrapper.Children.Add(QuantityLabel, 6, 1);
                GridWrapper.Children.Add(StockLabel, 7, 1);
            }
            else
            {
                GridWrapper.Children.Add(PriceLabel, 0, 1);
                GridWrapper.Children.Add(FeeLabel, 1, 1);
                GridWrapper.Children.Add(QuantityLabel, 3, 1);
                Grid.SetColumnSpan(QuantityLabel, 4);
                GridWrapper.Children.Add(StockLabel, 7, 1);
            }

            if (WithBinding)
            {
                CodeLabel.SetBinding(Label.TextProperty, "StuffCode");
                NameLabel.SetBinding(Label.TextProperty, "StuffName");
                UnitNameLabel.SetBinding(Label.TextProperty, "UnitName");
                StockLabel.SetBinding(Label.TextProperty, "Stock");
                FeeLabel.SetBinding(Label.TextProperty, "Fee");
                PriceLabel.SetBinding(Label.TextProperty, "Price");
                QuantityLabel.SetBinding(Label.TextProperty, "CurrentQuantity");
                AddedQuantityEntry.SetBinding(Label.TextProperty, "AddedQuantityLabel");
            }
            else
            {
                CodeLabel.Text = "کد کالا";
                NameLabel.Text = "نام کالا";
                UnitNameLabel.Text = "واحد";
                StockLabel.Text = "موجودی";
                FeeLabel.Text = "فی";
                PriceLabel.Text = "ارزش اشانتیون";
                QuantityLabel.Text = "تعداد";
            }

            var UnitNameTapGestureRecognizer = new TapGestureRecognizer();
            UnitNameTapGestureRecognizer.Tapped += UnitNameTapEventHandler;
            UnitNameTapGestureRecognizer.SetBinding(TapGestureRecognizer.CommandParameterProperty, "StuffId");
            UnitNameLabel.GestureRecognizers.Add(UnitNameTapGestureRecognizer);

            var QuantityPlusTapGestureRecognizer = new TapGestureRecognizer();
            QuantityPlusTapGestureRecognizer.Tapped += QuantityPlusTapEventHandler;
            QuantityPlusTapGestureRecognizer.SetBinding(TapGestureRecognizer.CommandParameterProperty, "StuffId");
            AddedQuantityPlusLabel.GestureRecognizers.Add(QuantityPlusTapGestureRecognizer);

            var QuantityMinusTapGestureRecognizer = new TapGestureRecognizer();
            QuantityMinusTapGestureRecognizer.Tapped += QuantityMinusTapEventHandler;
            QuantityMinusTapGestureRecognizer.SetBinding(TapGestureRecognizer.CommandParameterProperty, "StuffId");
            AddedQuantityMinusLabel.GestureRecognizers.Add(QuantityMinusTapGestureRecognizer);

            var AddedQuantityTextBoxTapGestureRecognizer = new TapGestureRecognizer();
            AddedQuantityTextBoxTapGestureRecognizer.Tapped += AddedQuantityTextBoxTapEventHandler;
            AddedQuantityTextBoxTapGestureRecognizer.SetBinding(TapGestureRecognizer.CommandParameterProperty, "StuffId");
            AddedQuantityEntry.GestureRecognizers.Add(AddedQuantityTextBoxTapGestureRecognizer);

            return GridWrapper;
        }
    }

    public partial class AddFreeProducts : GradientContentPage
    {
        SaleOrder SaleOrder;
        ObservableCollection<FreeProductArticleListModel> ArticlesData;
        List<FreeProductArticleListModel> AllArticlesDataWithoutFilter;
        private OrderPreviewForm OrderPreviewForm;
        private ToolbarItem ToolbarItem_SearchBar;
        Guid LastSearchWhenTypingId = Guid.NewGuid();
        MyKeyboard<AddedQuantityEditingArticleModel, decimal> RowAddedQuantityKeyboard;

        public AddFreeProducts(SaleOrder SaleOrder, OrderPreviewForm OrderPreviewForm)
        {
            InitializeComponent();

            this.SaleOrder = SaleOrder;
            this.OrderPreviewForm = OrderPreviewForm;

            ArticlesList.HasUnevenRows = true;
            ArticlesList.SeparatorColor = Color.FromHex("A5ABB7");
            ArticlesList.ItemSelected += ArticlesList_ItemSelected;
            CustomFreeProductStuffListCell.UnitNameTapEventHandler = (s, e) => {
                var StuffId = (Guid)((TappedEventArgs)e).Parameter;
                UnitNameClicked(StuffId);
            };
            CustomFreeProductStuffListCell.QuantityPlusTapEventHandler = (s, e) => {
                var StuffId = (Guid)((TappedEventArgs)e).Parameter;
                QuantityPlusClicked(StuffId);
            };
            CustomFreeProductStuffListCell.QuantityMinusTapEventHandler = (s, e) => {
                var StuffId = (Guid)((TappedEventArgs)e).Parameter;
                QuantityMinusClicked(StuffId);
            };
            CustomFreeProductStuffListCell.AddedQuantityTextBoxTapEventHandler = async (s, e) =>
            {
                var StuffId = (Guid)((TappedEventArgs)e).Parameter;
                FocusedAddedQuantityTextBoxStuffId = StuffId;
                var ArticleRow = ArticlesData.SingleOrDefault(a => a.StuffId == StuffId);
                if (ArticleRow != null)
                {
                    await Task.Delay(200);
                    ArticlesList.ScrollTo(ArticleRow, ScrollToPosition.Start, true);
                }
            };
            ArticlesList.ItemTemplate = new DataTemplate(typeof(CustomFreeProductStuffListCell));
            ArticlesListHeader.Children.Add(new CustomFreeProductStuffListCell().GetView(false));

            ToolbarItem_SearchBar = new ToolbarItem();
            ToolbarItem_SearchBar.Text = "جستجو";
            ToolbarItem_SearchBar.Icon = "Search.png";
            ToolbarItem_SearchBar.Activated += ToolbarItem_SearchBar_Activated;
            ToolbarItem_SearchBar.Order = ToolbarItemOrder.Primary;
            ToolbarItem_SearchBar.Priority = 10;
            if (!this.ToolbarItems.Contains(ToolbarItem_SearchBar) && !StuffsSearchBar.IsVisible)
                this.ToolbarItems.Add(ToolbarItem_SearchBar);

            StuffsSearchBar.TextChanged += async (sender, args) => {
                var thisTextSearchId = Guid.NewGuid();
                LastSearchWhenTypingId = thisTextSearchId;
                await Task.Delay(1000);
                if (LastSearchWhenTypingId == thisTextSearchId)
                    await FillArticlesData(args.NewTextValue);
            };
            StuffsSearchBar.SearchButtonPressed += async (sender, args) =>
            {
                await StuffsSearchBar.FadeTo(0, 350);
                StuffsSearchBar.IsVisible = false;
                ToolbarItem_SearchBar.Icon = "Search.png";
            };

            ShowingOrder = false;

            FillArticlesData("");

            RowAddedQuantityKeyboard = new MyKeyboard<AddedQuantityEditingArticleModel, decimal>
            (
                RowAddedQuantityKeyboardHolder,
                new Command((parameter) => {        //OnOK
                    FocusedAddedQuantityTextBoxStuffId = null;
                    CalculateTotalAddedDiscount();
                }),
                new Command((parameter) => {        //OnChange
                    CalculateTotalAddedDiscount();
                })
            );

            CalculateTotalAddedDiscount();
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

        public void UnitNameClicked(Guid StuffId)
        {
            if (ArticlesData != null)
            {
                var StuffModel = ArticlesData.SingleOrDefault(a => a.StuffData.Id == StuffId);
                if (StuffModel != null)
                {
                    if (StuffModel.PackagesData.Count() > 1)
                    {
                        var CurrentPackageIndex = -1;
                        for (int i = 0; i < StuffModel.PackagesData.Length; i++)
                        {
                            if (StuffModel.PackagesData[i].Id == StuffModel.SelectedPackage.Id)
                                CurrentPackageIndex = i;
                        }
                        var NewPackageIndex = CurrentPackageIndex == StuffModel.PackagesData.Length - 1 ? 0 : CurrentPackageIndex + 1;
                        StuffModel.SelectedPackage = StuffModel.PackagesData[NewPackageIndex];
                    }

                    var StuffModel2 = AllArticlesDataWithoutFilter.SingleOrDefault(a => a.StuffData.Id == StuffId);
                    if (StuffModel2 != null)
                        StuffModel2.SelectedPackage = StuffModel.SelectedPackage;

                    CalculateTotalAddedDiscount();
                }
            }
        }
        class AddedQuantityEditingArticleModel
        {
            FreeProductArticleListModel ArticleModel1, ArticleModel2;

            private decimal _AddedQuantity;
            public decimal AddedQuantity
            {
                get { return _AddedQuantity; }
                set
                {
                    _AddedQuantity = value;
                    if (ArticleModel1 != null)
                    {
                        ArticleModel1.AddedQuantity = _AddedQuantity;
                        _AddedQuantity = ArticleModel1.AddedQuantity;
                    }
                    if (ArticleModel2 != null)
                    {
                        ArticleModel2.AddedQuantity = _AddedQuantity;
                        _AddedQuantity = ArticleModel2.AddedQuantity;
                    }
                }
            }

            public AddedQuantityEditingArticleModel(FreeProductArticleListModel ArticleModel1, FreeProductArticleListModel ArticleModel2, decimal _AddedQuantity)
            {
                this.ArticleModel1 = ArticleModel1;
                this.ArticleModel2 = ArticleModel2;
                this._AddedQuantity = _AddedQuantity;
            }
        }
        AddedQuantityEditingArticleModel KeyboardObject;
        Guid? _FocusedAddedQuantityTextBoxStuffId = null;
        Guid? FocusedAddedQuantityTextBoxStuffId
        {
            get { return _FocusedAddedQuantityTextBoxStuffId; }
            set
            {
                if (_FocusedAddedQuantityTextBoxStuffId != value)
                {
                    if (_FocusedAddedQuantityTextBoxStuffId.HasValue)
                    {
                        if (ArticlesData != null)
                        {
                            var ArticleModel = ArticlesData.SingleOrDefault(a => a.StuffId == _FocusedAddedQuantityTextBoxStuffId);
                            if (ArticleModel != null)
                                ArticleModel.AddedQuantityFocused = false;
                        }
                        if (AllArticlesDataWithoutFilter != null)
                        {
                            var ArticleModel = AllArticlesDataWithoutFilter.SingleOrDefault(a => a.StuffId == _FocusedAddedQuantityTextBoxStuffId);
                            if (ArticleModel != null)
                                ArticleModel.AddedQuantityFocused = false;
                        }
                    }
                    _FocusedAddedQuantityTextBoxStuffId = value;
                    if (_FocusedAddedQuantityTextBoxStuffId.HasValue)
                    {
                        if (ArticlesData != null || AllArticlesDataWithoutFilter != null)
                        {
                            FreeProductArticleListModel ArticleModel1 = null, ArticleModel2 = null;
                            if (AllArticlesDataWithoutFilter != null)
                            {
                                ArticleModel1 = AllArticlesDataWithoutFilter.SingleOrDefault(a => a.StuffId == _FocusedAddedQuantityTextBoxStuffId);
                                if (ArticleModel1 != null)
                                    ArticleModel1.AddedQuantityFocused = true;
                            }
                            if (ArticlesData != null)
                            {
                                ArticleModel2 = ArticlesData.Single(a => a.StuffId == _FocusedAddedQuantityTextBoxStuffId);
                                if (ArticleModel2 != null)
                                    ArticleModel2.AddedQuantityFocused = true;
                            }

                            KeyboardObject = new AddedQuantityEditingArticleModel(ArticleModel1, ArticleModel2, ArticleModel1 != null ? ArticleModel1.AddedQuantity : ArticleModel2 != null ? ArticleModel2.AddedQuantity : 0);
                            RowAddedQuantityKeyboard.SetObject(KeyboardObject, a => a.AddedQuantity);
                            RowAddedQuantityKeyboard.Show();
                        }
                        else
                            RowAddedQuantityKeyboard.Hide();
                    }
                    else
                        RowAddedQuantityKeyboard.Hide();
                }
            }
        }
        public void QuantityPlusClicked(Guid StuffId)
        {
            FocusedAddedQuantityTextBoxStuffId = null;
            if (ArticlesData != null)
            {
                var StuffModel = ArticlesData.SingleOrDefault(a => a.StuffData.Id == StuffId);
                if (StuffModel != null)
                    StuffModel.AddedQuantity++;
                var StuffModel2 = AllArticlesDataWithoutFilter.SingleOrDefault(a => a.StuffData.Id == StuffId);
                if (StuffModel2 != null)
                    StuffModel2.AddedQuantity++;

                CalculateTotalAddedDiscount();
            }
        }
        public void QuantityMinusClicked(Guid StuffId)
        {
            FocusedAddedQuantityTextBoxStuffId = null;
            if (ArticlesData != null)
            {
                var StuffModel = ArticlesData.SingleOrDefault(a => a.StuffData.Id == StuffId);
                if (StuffModel != null)
                    StuffModel.AddedQuantity = StuffModel.AddedQuantity > 0 ? StuffModel.AddedQuantity - 1 : 0;
                var StuffModel2 = AllArticlesDataWithoutFilter.SingleOrDefault(a => a.StuffData.Id == StuffId);
                if (StuffModel2 != null)
                    StuffModel2.AddedQuantity = StuffModel2.AddedQuantity > 0 ? StuffModel2.AddedQuantity - 1 : 0;

                CalculateTotalAddedDiscount();
            }
        }
        

        bool ShowingOrder = false;
        Guid LastCalculateTotalAddedDiscountId = Guid.Empty;
        async void CalculateTotalAddedDiscount()
        {
            if (ArticlesData != null)
            {
                var NewData = AllArticlesDataWithoutFilter.Where(a => a.Quantity != 0).ToArray();
                var OldData = SaleOrder.SaleOrderStuffs.Where(a => a.FreeProduct).ToArray();

                var MustBeInserted = NewData.Where(a => !a.ArticleId.HasValue).ToArray();
                var MustBeUpdated = OldData.Select(a => new
                {
                    oldData = a,
                    newData = NewData.SingleOrDefault(b => b.ArticleId == a.Id)
                }).Where(a => a.newData != null).ToArray();
                var MustBeDeleted = OldData.Where(a => !NewData.Any(b => b.ArticleId == a.Id)).ToArray();
                
                foreach (var item in MustBeInserted)
                {
                    var NewArticle = new SaleOrderStuff()
                    {
                        Id = Guid.NewGuid(),
                        ArticleIndex = SaleOrder.SaleOrderStuffs.Any(a => a.FreeProduct) ? (SaleOrder.SaleOrderStuffs.Where(a => a.FreeProduct).Max(a => a.ArticleIndex) + 1) : 1,
                        OrderId = SaleOrder.Id,
                        PackageId = item.SelectedPackage.Id,
                        BatchNumberId = null,
                        Quantity = item._CurrentQuantity + item.AddedQuantity,
                        FreeProductAddedQuantity = item.AddedQuantity,
                        FreeProduct = true,
                        FreeProduct_UnitPrice = item._UnitPrice,
                        SalePrice = 0,
                        DiscountPercent = 0,
                        ProporatedDiscount = 0,
                        VATPercent = 0
                    };
                    var list = SaleOrder.SaleOrderStuffs.ToList();
                    list.Add(NewArticle);
                    SaleOrder.SaleOrderStuffs = list.ToArray();
                    item.ArticleId = NewArticle.Id;
                }
                foreach (var item in MustBeUpdated)
                {
                    item.oldData._Package = null;
                    item.oldData.PackageId = item.newData.SelectedPackage.Id;
                    item.oldData.Quantity = item.oldData.Quantity - item.oldData.FreeProductAddedQuantity + item.newData.AddedQuantity;
                    item.oldData.FreeProductAddedQuantity = item.newData.AddedQuantity;
                }

                foreach (var item in MustBeDeleted)
                {
                    var oldData = AllArticlesDataWithoutFilter.Single(a => a.ArticleId == item.Id);
                    SaleOrder.SaleOrderStuffs = SaleOrder.SaleOrderStuffs.Where(a => a.Id != item.Id).ToArray();
                    oldData.ArticleId = null;
                }
            }

            TotalAddedFreeProductsPrice.Text = "جمع مبلغ اشانتیون های اضافه شده: " + (SaleOrder.SaleOrderStuffs.Any(a => a.FreeProductAddedQuantity != 0) ? SaleOrder.SaleOrderStuffs.Sum(a => a.FreeProductAddedQuantity * a.FreeProduct_UnitPrice.GetValueOrDefault(0) * a.Package.Coefficient) : 0).ToString("###,###,###,###,###,###,##0.").ReplaceLatinDigits() + " ریال";
            
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
            ((ListView)sender).SelectedItem = null;
        }
        
        async Task FillArticlesData(string Filter)
        {
            ArticlesList.IsRefreshing = true;
            await Task.Delay(100);

            var StuffsResult = await App.DB.GetAllStuffsListAsync(SaleOrder.PartnerId, null, true, SaleOrder.WarehouseId);
            if (!StuffsResult.Success)
            {
                App.ShowError("خطا", "در نمایش لیست اشانتیون ها خطایی رخ داد.\n" + StuffsResult.Message, "خوب");
                ArticlesList.IsRefreshing = false;
                return;
            }
            var AllStuffsData = StuffsResult.Data[0];
            var FreeProducts = SaleOrder.SaleOrderStuffs.Where(a => a.FreeProduct).ToArray();

            AllArticlesDataWithoutFilter = AllStuffsData.Select(a => new FreeProductArticleListModel()
            {
                ArticleId = FreeProducts.Any(b => b.Package.StuffId == a.StuffId) ? FreeProducts.Single(b => b.Package.StuffId == a.StuffId).Id : new Nullable<Guid>(),
                StuffData = a.StuffData,
                PackagesData = a.PackagesData.Select(b => b.Package).ToArray(),
                _UnitPrice = a._UnitPrice,
                _ConsumerUnitPrice = a._ConsumerUnitPrice,
                _UnitStock = a._UnitStock,
                SelectedPackage = FreeProducts.Any(b => b.Package.StuffId == a.StuffId) ? FreeProducts.Single(b => b.Package.StuffId == a.StuffId).Package : a.SelectedPackage,
                _CurrentQuantity = FreeProducts.Any(b => b.Package.StuffId == a.StuffId) ? FreeProducts.Single(b => b.Package.StuffId == a.StuffId).Quantity : 0,
                AddedQuantity = FreeProducts.Any(b => b.Package.StuffId == a.StuffId) ? FreeProducts.Single(b => b.Package.StuffId == a.StuffId).FreeProductAddedQuantity : 0
            }).OrderBy(a => a._CurrentQuantity == 0).ThenBy(a => a.AddedQuantity != 0).ToList();

            var FilteredStuffs = await App.DB.FilterStuffsAsync(AllStuffsData, Filter);

            var Data = FilteredStuffs.Select(a => new FreeProductArticleListModel()
            {
                ArticleId = FreeProducts.Any(b => b.Package.StuffId == a.StuffId) ? FreeProducts.Single(b => b.Package.StuffId == a.StuffId).Id : new Nullable<Guid>(),
                StuffData = a.StuffData,
                PackagesData = a.PackagesData.Select(b => b.Package).ToArray(),
                _UnitPrice = a._UnitPrice,
                _ConsumerUnitPrice = a._ConsumerUnitPrice,
                _UnitStock = a._UnitStock,
                SelectedPackage = FreeProducts.Any(b => b.Package.StuffId == a.StuffId) ? FreeProducts.Single(b => b.Package.StuffId == a.StuffId).Package : a.SelectedPackage,
                _CurrentQuantity = FreeProducts.Any(b => b.Package.StuffId == a.StuffId) ? FreeProducts.Single(b => b.Package.StuffId == a.StuffId).Quantity : 0,
                AddedQuantity = FreeProducts.Any(b => b.Package.StuffId == a.StuffId) ? FreeProducts.Single(b => b.Package.StuffId == a.StuffId).FreeProductAddedQuantity : 0
            }).OrderBy(a => a._CurrentQuantity == 0).ThenBy(a => a.AddedQuantity != 0).ToList();


            ArticlesData = new ObservableCollection<FreeProductArticleListModel>(Data);
            ArticlesList.ItemsSource = null;
            ArticlesList.ItemsSource = ArticlesData;

            ArticlesList.IsRefreshing = false;
            
            ShowingOrder = true;
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

                RowAddedQuantityKeyboard.OrientationChanged(width > height);
            }
        }
    }
}
