using Kara.Assets;
using Kara.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Kara.CustomRenderer;
using static Kara.Assets.Connectivity;

namespace Kara
{
    public partial class OrderBeforePreviewForm : GradientContentPage
    {
        private ObservableCollection<DBRepository.StuffListModel> _stuffsList = null;
        private Partner _selectedPartner;
        public Partner SelectedPartner
        {
            get => _selectedPartner;
            set
            {
                _selectedPartner = value;
                PartnerSelected();
            }
        }

        private readonly PartnerListForm _partnerListForm;
        private readonly InsertedInformations_Orders _ordersForm;
        public Guid? EditingSaleOrderId;
        private readonly SaleOrder _editingSaleOrder;
        private SaleOrder _saleOrder;
        private readonly OrderInsertForm _orderInsertForm;
        private Guid? _settlementTypeId;
        private readonly MyKeyboard<QuantityEditingStuffModel, decimal> _quantityKeyboard;
        private readonly Guid? _warehouseId;
        private DtoReversionReason[] _reversionReasons = null;

        public OrderBeforePreviewForm
        (
            List<DBRepository.StuffListModel> AllStuffsData,
            Partner Partner,
            SaleOrder SaleOrder,
            PartnerListForm PartnerListForm,
            InsertedInformations_Orders OrdersForm,
            Guid? settlementTypeId,
            string Description,
            OrderInsertForm OrderInsertForm,
            bool CanChangePartner,
            Guid? WarehouseId,
            bool fromTour=false
        )
        {
            InitializeComponent();

            _saleOrder = SaleOrder;
            _warehouseId = WarehouseId;
            _orderInsertForm = OrderInsertForm;

            EditingSaleOrderId = SaleOrder != null ? SaleOrder.Id : new Nullable<Guid>();
            _editingSaleOrder = SaleOrder;
            this.AllStuffsData = AllStuffsData;

            _partnerListForm = PartnerListForm;
            _ordersForm = OrdersForm;

            CustomStuffListCell.UnitNameTapEventHandler = (s, e) => {
                var ids = ((string)((TappedEventArgs)e).Parameter).Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                var stuffId = new Guid(ids[0]);
                var batchNumberId = ids.Length == 2 ? new Guid(ids[1]) : new Nullable<Guid>();
                UnitNameClicked(stuffId);
            };
            CustomStuffListCell.QuantityTextBoxTapEventHandler = (s, e) => {
                var ids = ((string)((TappedEventArgs)e).Parameter).Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                var stuffId = new Guid(ids[0]);
                var batchNumberId = ids.Length == 2 ? new Guid(ids[1]) : new Nullable<Guid>();
                FocusedQuantityTextBoxId = batchNumberId.HasValue ? new Guid[] { stuffId, batchNumberId.Value } : new Guid[] { stuffId };
            };
            CustomStuffListCell.QuantityPlusTapEventHandler = (s, e) => {
                var ids = ((string)((TappedEventArgs)e).Parameter).Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                var stuffId = new Guid(ids[0]);
                var batchNumberId = ids.Length == 2 ? new Guid(ids[1]) : new Nullable<Guid>();
                QuantityPlusClicked(stuffId, batchNumberId);
            };
            CustomStuffListCell.QuantityMinusTapEventHandler = (s, e) => {
                var ids = ((string)((TappedEventArgs)e).Parameter).Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                var stuffId = new Guid(ids[0]);
                var batchNumberId = ids.Length == 2 ? new Guid(ids[1]) : new Nullable<Guid>();
                QuantityMinusClicked(stuffId, batchNumberId);
            };
            StuffItems.ItemTemplate = new DataTemplate(typeof(CustomStuffListCell));

            SelectedPartner = Partner != null ? Partner : SaleOrder != null ? SaleOrder.Partner : null;
            if (CanChangePartner)
                PartnerChangeButton.IsEnabled = true;
            else
                PartnerChangeButton.IsEnabled = false;
            PartnerLabel.FontSize *= 1.5;
            
            var partnerChangeButtonTapGestureRecognizer = new TapGestureRecognizer();
            partnerChangeButtonTapGestureRecognizer.Tapped += (sender, e) => {
                if (PartnerChangeButton.IsEnabled)
                {
                    FocusedQuantityTextBoxId = null;
                    var partnerListForm1 = new PartnerListForm();
                    partnerListForm1.OrderBeforePreviewForm = this;
                    Navigation.PushAsync(partnerListForm1);
                }
            };
            partnerChangeButtonTapGestureRecognizer.SetBinding(TapGestureRecognizer.CommandParameterProperty, "Id");
            PartnerChangeButton.GestureRecognizers.Add(partnerChangeButtonTapGestureRecognizer);
            PartnerChangeButton.WidthRequest = 150;

            StuffItems.HasUnevenRows = true;
            StuffItems.SeparatorVisibility = SeparatorVisibility.None;
            StuffItems.ItemSelected += StuffItems_ItemSelected;

            var toolbarItemOrderPreviewForm = new ToolbarItem();
            toolbarItemOrderPreviewForm.Text = "پیش نمایش سفارش";
            toolbarItemOrderPreviewForm.Icon = "ShowInvoice.png";
            toolbarItemOrderPreviewForm.Activated += ToolbarItem_OrderPreviewForm_Activated;
            toolbarItemOrderPreviewForm.Order = ToolbarItemOrder.Primary;
            toolbarItemOrderPreviewForm.Priority = 2;
            if (!ToolbarItems.Contains(toolbarItemOrderPreviewForm))
                ToolbarItems.Add(toolbarItemOrderPreviewForm);

            if (fromTour)
            {
                var toolbarItemSaveOrder = new ToolbarItem();
                toolbarItemSaveOrder.Text = "ذخیره";
                toolbarItemSaveOrder.Icon = "Upload.png";
                toolbarItemSaveOrder.Activated += ToolbarItem_SaveOrder_Activated;
                toolbarItemSaveOrder.Order = ToolbarItemOrder.Primary;
                toolbarItemSaveOrder.Priority = 3;

                if (!ToolbarItems.Contains(toolbarItemSaveOrder))
                    ToolbarItems.Add(toolbarItemSaveOrder);
            }
            

            var settlementTypes = App.SettlementTypes.Where(a => a.Enabled).ToArray();
            foreach (var settlementType in settlementTypes)
                SettlementTypePicker.Items.Add(settlementType.Name);

            if(SaleOrder != null)
                settlementTypeId = SaleOrder.SettlementTypeId;

            if (settlementTypeId.HasValue && settlementTypeId.Value!=Guid.Parse("00000000-0000-0000-0000-000000000000"))
            {
                SettlementTypePicker.SelectedIndex = settlementTypes.Select((a, index) => new { a, index }).Single(a => a.a.Id == settlementTypeId).index;
                SettlementTypeLabel.Text = settlementTypes[SettlementTypePicker.SelectedIndex].Name;
            }

            _settlementTypeId = settlementTypeId;

            SettlementTypePicker.SelectedIndexChanged += (sender, e) => {
                FocusedQuantityTextBoxId = null;

                SettlementTypeLabel.Text = settlementTypes[SettlementTypePicker.SelectedIndex].Name;

                var settlementType = SettlementTypePicker.SelectedIndex == -1 ? null : settlementTypes[SettlementTypePicker.SelectedIndex];
                _settlementTypeId = settlementType == null ? new Nullable<Guid>() : settlementType.Id;
                OrderInsertForm.SettlementTypeId = _settlementTypeId;
            };

            var description = SaleOrder != null ? SaleOrder.Description : Description;
            DescriptionEditor.Text = !string.IsNullOrEmpty(description) ? description : "";
            
            _quantityKeyboard = new MyKeyboard<QuantityEditingStuffModel, decimal>
            (
                QuantityKeyboardHolder,
                new Command((parameter) => {        //OnOK
                    FocusedQuantityTextBoxId = null;
                    CheckToBackToOrderInsertFormIfStuffsEmpty();
                }),
                new Command((parameter) => {        //OnChange
                    CheckToBackToOrderInsertFormIfStuffsEmpty();
                })
            );

            if(fromTour)
            {
                gridReasons.IsVisible = true;
                FillReversionReasons();
            }

            
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            //if (App.AllowOptionalDiscountRules.Value)
            //{
            //    var discountRules = await App.DB.GetDiscountRulesAsync();

            //    if (discountRules == null || !discountRules.Any())
            //        return;

            //    int? settlementDay = _saleOrder?.SettlementType?.Day;

            //    OrderModel saleOrderModel = new OrderModel()
            //    {
            //        SettlementTypeId = Guid.Empty,
            //        SettlementDay = settlementDay.GetValueOrDefault(0),
            //        VisitorId = App.UserPersonnelId.Value,
            //        OrderInsertDate = DateTime.Now,
            //        //Partner = _saleOrder.Partner,
            //        Articles = _saleOrder?.SaleOrderStuffs.Select(a => new ArticleModel()
            //        {
            //            Id = a.Id,
            //            Stuff = a.Package.Stuff,
            //            Package = a.Package,
            //            BatchNumber = a.BatchNumber,
            //            Quantity = a.Quantity,
            //            UnitPrice = a.SalePrice / a.Package.Coefficient
            //        }).ToArray()
            //    };

            //    var allSystemStuffs = (await App.DB.GetStuffsAsync()).Data.ToDictionary(a => a.Id);

            //    var dc = new DiscountCalculator(App.SystemName.Value,
            //        App.AllowOptionalDiscountRules_MultiSelection.Value, allSystemStuffs,
            //        saleOrderModel, discountRules);

            //    dc.ClaculateOrderDiscounts();
            //}
        }

        private async Task FillReversionReasons()
        {
            try
            {
                var result = await GetReversionReasons();

                if (result.Success && result.Data != null)
                {
                    _reversionReasons = result.Data.ToArray();

                    if (_reversionReasons != null)
                        foreach (var r in _reversionReasons)
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

        private async void ToolbarItem_SaveOrder_Activated(object sender, EventArgs e)
        {
            //WaitToggle(false);
            try
            {


                await Task.Delay(100);

                if (ReversionReasonPicker.SelectedIndex == -1)
                {
                    App.ShowError("خطا", "دلیل انتخاب نشده است.", "خوب");
                    return;
                }

                var data = new DtoEditSaleOrderMobile()
                {
                    DistributionReversionReasonId = _reversionReasons[ReversionReasonPicker.SelectedIndex].Id,
                    OrderId = (Guid)EditingSaleOrderId,
                    UserId = App.UserId.Value,
                    Description = _editingSaleOrder?.Description.ToSafeString()=="" ? "***" : DescriptionEditor.Text,
                    Stuffs = _stuffsList.Where(a => a.Quantity > 0).Select(a => new DtoEditStuffMobile
                    {
                        ArticleId = _editingSaleOrder?.SaleOrderStuffs?.SingleOrDefault(x=>x.Package.StuffId==a.StuffId)?.ArticleId,
                        StuffId = a.StuffId.ToSafeGuid(),
                        PackageId = a.SelectedPackage.Id,
                        Quantity = a.Quantity,
                        SalePrice = a.Price.ToLatinDigits().ToSafeDecimal(),
                        StuffQuantity = a.SelectedPackage.Coefficient * a.Quantity,
                        BatchNumberId = a.BatchNumberId
                    }).ToList()
                };

                var submitResult = await EditSaleOrder(data);

                if (!submitResult.Success)
                {
                    App.ShowError("خطا", "خطا در ارسال اطلاعات" + "\n" + submitResult.Message, "خوب");
                }
                else
                {
                    Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count - 1]);

                    App.ToastMessageHandler.ShowMessage("فاکتور با موفقیت به روزرسانی شد", Helpers.ToastMessageDuration.Long);
                    try { await Navigation.PopAsync(); } catch (Exception) { }
                }
            }
            catch (Exception ex)
            {
                App.ShowError("خطا", ex.Message, "باشه");
            }
        }

        private void DescriptionEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            _orderInsertForm.Description = ((PlaceholderEditor)sender).Text;
        }
        
        private void ToolbarItem_OrderPreviewForm_Activated(object sender, EventArgs e)
        {
            FocusedQuantityTextBoxId = null;
            if (SelectedPartner == null && !_settlementTypeId.HasValue)
            {
                App.ShowError("خطا", "مشتری و نحوه تسویه را مشخص کنید.", "خوب");
                return;
            }
            else if(SelectedPartner == null)
            {
                App.ShowError("خطا", "مشتری را مشخص کنید.", "خوب");
                return;
            }
            else if (!_settlementTypeId.HasValue)
            {
                App.ShowError("خطا", "نحوه تسویه را مشخص کنید.", "خوب");
                return;
            }

            var withoutPriceStuffs = AllStuffsData.Where(a => a.Quantity != 0 && !a._UnitPrice.HasValue).ToList();
            if (withoutPriceStuffs.Any())
            {
                var message = "برخی اقلام در لیست قیمت این مشتری ثبت نشده اند:\n" + withoutPriceStuffs.Select(a => a.StuffData.Name).Aggregate((sum, x) => sum + "\n" + x);
                App.ShowError("خطا", message, "خوب");
                return;
            }

            SaleOrder saleOrder;
            try
            {
                saleOrder = MakeOrder(SelectedPartner.Id, _settlementTypeId.Value, DescriptionEditor.Text);
            }
            catch (Exception err)
            {
                App.ShowError("خطا", err.ProperMessage(), "خوب");
                return;
            }
            
            var orderPreviewForm = new OrderPreviewForm(saleOrder, _ordersForm, _partnerListForm, _orderInsertForm, this, true)
            {
                StartColor = Color.FromHex("ffffff"),
                EndColor = Color.FromHex("ffffff")
            };
            Navigation.PushAsync(orderPreviewForm);
        }

        private SaleOrder MakeOrder(Guid PartnerId, Guid SettlementTypeId, string Description)
        {
            var saleOrder = new SaleOrder()
            {
                Id = EditingSaleOrderId.HasValue ? EditingSaleOrderId.Value : Guid.NewGuid(),
                PreCode = null,
                InsertDateTime = DateTime.Now,
                PartnerId = PartnerId,
                SettlementTypeId = SettlementTypeId,
                Description = Description,
                CashPrise = 0,
                DiscountPercent = 0,
                GeoLocation_Latitude = null,//TODO
                GeoLocation_Longitude = null,//TODO
                GeoLocation_Accuracy = null,//TODO
                WarehouseId = _warehouseId
            };

            var _saleOrderStuffs = AllStuffsData.Where(a => !a.HasBatchNumbers).SelectMany(a => a.PackagesData.Where(b => b.Quantity != 0).Select(b => new { Stuff = a, BatchNumber = (DBRepository.StuffListModel)null, Package = b }))
                .Union(AllStuffsData.Where(a => a.HasBatchNumbers).SelectMany(a => a.StuffRow_BatchNumberRows.SelectMany(b => b.PackagesData.Where(c => c.Quantity != 0).Select(c => new { Stuff = a, BatchNumber = b, Package = c }))))
                .Select((a, index) => new
                {
                    Id = a.Stuff.Id,
                    ArticleIndex = index + 1,
                    StuffData = a.Stuff.StuffData,
                    PackageData = a.Package.Package,
                    BatchNumberData = a.BatchNumber != null ? a.BatchNumber.BatchNumberData : null,
                    Quantity = a.Package.Quantity,
                    PackagePrice = a.Stuff._UnitPrice.Value * a.Package.Package.Coefficient
                }).Where(a => a.Quantity != 0).ToList();

            var minSaleConflicts = _saleOrderStuffs.GroupBy(a => a.StuffData).Where(a => a.Sum(b => b.Quantity * b.PackageData.Coefficient) < a.Key.MinForSale).ToArray();
            if (minSaleConflicts.Any())
                throw new Exception("کالا" + (minSaleConflicts.Count() > 1 ? "ها" : "") + "ی زیر کمتر از حداقل تعیین شده در سیستم سفارش داده شده اند:\n" +
                    minSaleConflicts.Select(a => a.Key.Name + " (حداقل سفارش: " + a.Key.MinForSale + " " + a.Key.Packages.Single(b => b.Coefficient == 1).Name + ")").Aggregate((sum, x) => sum + "\n" + x));

            var saleCoefficientConflicts = _saleOrderStuffs.Where(a => a.StuffData.SaleCoefficient != 0 && a.StuffData.SaleCoefficient != 1).GroupBy(a => a.StuffData).Where(a => a.Sum(b => b.Quantity * b.PackageData.Coefficient) % a.Key.SaleCoefficient != 0).ToArray();
            if (saleCoefficientConflicts.Any())
                throw new Exception("تعداد سفارش کالا" + (saleCoefficientConflicts.Count() > 1 ? "ها" : "") + "ی زیر با ضریب فروش تعیین شده در سیستم مغایرت دارد:\n" +
                    saleCoefficientConflicts.Select(a => a.Key.Name + " (ضریب فروش: " + a.Key.SaleCoefficient + " " + a.Key.Packages.Single(b => b.Coefficient == 1).Name + ")").Aggregate((sum, x) => sum + "\n" + x));

            var saleOrderStuffs = _saleOrderStuffs.Select(a => new SaleOrderStuff()
            {
                //1400/06/19 replaced to solve duplicate article ids in saving ...

                Id = Guid.NewGuid(),  //a.Id ==string.Empty || a.Id==null ? Guid.NewGuid() : a.Id.Replace("|","").ToSafeGuid(),
                OrderId = saleOrder.Id,
                SaleOrder = saleOrder,
                ArticleIndex = a.ArticleIndex,
                PackageId = a.PackageData.Id,
                BatchNumberId = a.BatchNumberData != null ? a.BatchNumberData.BatchNumberId : new Nullable<Guid>(),
                Quantity = a.Quantity,
                SalePrice = a.PackagePrice,
                DiscountPercent = 0,
                VATPercent = saleOrder.Partner.CalculateVATForThisPerson && a.PackageData.Stuff.HasVAT ? App.VATPercent.Value : 0,
                FreeProduct = false,
                FreeProduct_UnitPrice = null
            }).ToArray();

            var saleOrderCashDiscounts = new CashDiscount[] { };

            saleOrder.SaleOrderStuffs = saleOrderStuffs;
            saleOrder.CashDiscounts = saleOrderCashDiscounts;
            
            saleOrder = App.DB.CalculateProporatedDiscount(saleOrder);

            return saleOrder;
        }

        private void PartnerSelected()
        {
            PartnerLabel.Text = SelectedPartner == null ? "مشتری" :
                !string.IsNullOrEmpty(SelectedPartner.LegalName) ? (SelectedPartner.LegalName + (!string.IsNullOrEmpty(SelectedPartner.Name) ? (" (" + SelectedPartner.Name + ")") : "")) : (SelectedPartner.Name);
            FillStuffs("", EditingSaleOrderId, true);
            _orderInsertForm.SelectedPartner = SelectedPartner;
        }

        private class QuantityEditingStuffModel
        {
            private readonly DBRepository.StuffListModel _stuffModel;

            private decimal _quantity;
            public decimal Quantity
            {
                get { return _quantity; }
                set
                {
                    if (_quantity != value)
                    {
                        if (_stuffModel != null)
                        {
                            _stuffModel.Quantity = value;
                            _quantity = _stuffModel.Quantity;
                        }
                    }
                }
            }

            public QuantityEditingStuffModel(DBRepository.StuffListModel StuffModel, decimal quantity)
            {
                _stuffModel = StuffModel;
                _quantity = quantity;
            }
        }

        private QuantityEditingStuffModel _keyboardObject;
        private Guid[] _focusedQuantityTextBoxId = null;

        private Guid[] FocusedQuantityTextBoxId
        {
            get { return _focusedQuantityTextBoxId; }
            set
            {
                if (_focusedQuantityTextBoxId != value)
                {
                    if (_focusedQuantityTextBoxId != null)
                    {
                        var stuffId = _focusedQuantityTextBoxId[0];
                        var batchNumberId = _focusedQuantityTextBoxId.Length == 2 ? _focusedQuantityTextBoxId[1] : new Nullable<Guid>();
                        if (AllStuffsData != null)
                        {
                            var stuffModel = AllStuffsData.SingleOrDefault(a => a.StuffId == stuffId);
                            if (stuffModel != null)
                            {
                                if (batchNumberId.HasValue)
                                    stuffModel = stuffModel.StuffRow_BatchNumberRows.SingleOrDefault(a => a.BatchNumberId == batchNumberId);
                                if (batchNumberId.HasValue || !stuffModel.HasBatchNumbers)
                                    stuffModel.QuantityFocused = false;
                            }
                        }
                    }
                    _focusedQuantityTextBoxId = value;
                    if (_focusedQuantityTextBoxId != null)
                    {
                        var stuffId = _focusedQuantityTextBoxId[0];
                        var batchNumberId = _focusedQuantityTextBoxId.Length == 2 ? _focusedQuantityTextBoxId[1] : new Nullable<Guid>();
                        if (AllStuffsData != null)
                        {
                            var stuffModel = AllStuffsData.SingleOrDefault(a => a.StuffId == stuffId);
                            if (stuffModel != null)
                            {
                                if (batchNumberId.HasValue)
                                    stuffModel = stuffModel.StuffRow_BatchNumberRows.SingleOrDefault(a => a.BatchNumberId == batchNumberId);
                                if (batchNumberId.HasValue || !stuffModel.HasBatchNumbers)
                                    stuffModel.QuantityFocused = true;
                            }
                            if (stuffModel != null)
                            {
                                _keyboardObject = new QuantityEditingStuffModel(stuffModel, stuffModel.Quantity);
                                _quantityKeyboard.SetObject(_keyboardObject, a => a.Quantity);
                                _quantityKeyboard.Show();
                            }
                            else
                                _quantityKeyboard.Hide();
                        }
                        else
                            _quantityKeyboard.Hide();
                    }
                    else
                        _quantityKeyboard.Hide();
                }
            }
        }

        public void UnitNameClicked(Guid StuffId)
        {
            FocusedQuantityTextBoxId = null;
            if (AllStuffsData != null)
            {
                var stuffModel = AllStuffsData.SingleOrDefault(a => a.StuffId == StuffId && !a.BatchNumberId.HasValue);
                if (stuffModel != null)
                {
                    if (stuffModel.PackagesData.Count() > 1)
                    {
                        var currentPackageIndex = stuffModel.PackagesData.Select((a, index) => new { a, index }).Single(a => stuffModel.SelectedPackage.Id == a.a.Package.Id).index;
                        var newPackageIndex = currentPackageIndex == stuffModel.PackagesData.Length - 1 ? 0 : currentPackageIndex + 1;
                        stuffModel.SelectedPackage = stuffModel.PackagesData[newPackageIndex].Package;
                    }
                }
            }
        }

        public void QuantityPlusClicked(Guid StuffId, Guid? BatchNumberId)
        {
            FocusedQuantityTextBoxId = null;
            if (AllStuffsData != null)
            {
                var stuffModel = AllStuffsData.SingleOrDefault(a => a.StuffId == StuffId);
                if (stuffModel != null)
                {
                    if (BatchNumberId.HasValue)
                        stuffModel = stuffModel.StuffRow_BatchNumberRows.SingleOrDefault(a => a.BatchNumberId == BatchNumberId);
                    if (BatchNumberId.HasValue || !stuffModel.HasBatchNumbers)
                        stuffModel.Quantity++;
                }
            }
        }
        public void QuantityMinusClicked(Guid StuffId, Guid? BatchNumberId)
        {
            FocusedQuantityTextBoxId = null;
            if (AllStuffsData != null)
            {
                var stuffModel = AllStuffsData.SingleOrDefault(a => a.StuffId == StuffId);
                if (stuffModel != null)
                {
                    if (BatchNumberId.HasValue)
                        stuffModel = stuffModel.StuffRow_BatchNumberRows.SingleOrDefault(a => a.BatchNumberId == BatchNumberId);
                    if (BatchNumberId.HasValue || !stuffModel.HasBatchNumbers)
                        stuffModel.Quantity = stuffModel.Quantity > 0 ? stuffModel.Quantity - 1 : 0;
                }
                CheckToBackToOrderInsertFormIfStuffsEmpty();
            }
        }
        
        private void CheckToBackToOrderInsertFormIfStuffsEmpty()
        {
            if (AllStuffsData.All(a => a.TotalStuffQuantity <= 0))
                try { Navigation.PopAsync(); } catch (Exception) { }
        }

        private void StuffItems_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            _focusedQuantityTextBoxId = null;
            ((ListView)sender).SelectedItem = null;
        }

        private Guid _lastSizeAllocationId;
        protected override async void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            Guid thisSizeAllocationId = Guid.NewGuid();
            _lastSizeAllocationId = thisSizeAllocationId;
            await Task.Delay(100);
            if (_lastSizeAllocationId == thisSizeAllocationId)
                sizeChanged(width, height);
        }

        private double _lastWidth, _lastHeight;
        public void sizeChanged(double width, double height)
        {
            if (_lastWidth != width || _lastHeight != height)
            {
                _lastWidth = width;
                _lastHeight = height;

                _quantityKeyboard.OrientationChanged(width > height);
            }
        }

        public List<DBRepository.StuffListModel> AllStuffsData;
        private async Task FillStuffs(string Filter, Guid? EditingOrderId, bool RefreshStuffsData)
        {
            StuffItems.IsRefreshing = true;
            await Task.Delay(100);
            if (AllStuffsData == null || RefreshStuffsData)
            {
                var stuffsResult = await App.DB.GetAllStuffsListAsync(SelectedPartner != null ? SelectedPartner.Id : new Guid?(), EditingOrderId, true, _warehouseId);
                if (!stuffsResult.Success)
                {
                    App.ShowError("خطا", "در نمایش لیست کالاها خطایی رخ داد.\n" + stuffsResult.Message, "خوب");
                    StuffItems.IsRefreshing = false;
                    return;
                }
                var newStuffsData = stuffsResult.Data[0];
                
                if (AllStuffsData != null)
                {
                    try
                    {
                        foreach (var stuffInList in newStuffsData)
                        {
                            var currentStuffInList = AllStuffsData.SingleOrDefault(a => a.StuffData.Id == stuffInList.StuffId);
                            if (currentStuffInList != null)
                            {
                                foreach (var p in currentStuffInList.PackagesData)
                                {
                                    stuffInList.SelectedPackage = p.Package;
                                    stuffInList.Quantity = p.Quantity;
                                    foreach (var batchNumberInList in stuffInList.StuffRow_BatchNumberRows)
                                    {
                                        var currentBatchNumberInList = currentStuffInList.StuffRow_BatchNumberRows.SingleOrDefault(a => a.BatchNumberId == batchNumberInList.BatchNumberId);
                                        if (currentBatchNumberInList != null)
                                            batchNumberInList.Quantity = currentBatchNumberInList.PackagesData.Single(a => a.Package.Id == p.Package.Id).Quantity;
                                    }
                                }
                                stuffInList.SelectedPackage = currentStuffInList.SelectedPackage;
                                stuffInList.Selected = currentStuffInList.Selected;
                            }
                        }
                    }
                    catch (Exception err)
                    {
                    }
                }
                
                AllStuffsData = newStuffsData;
                _orderInsertForm.AllStuffsData = newStuffsData;
                _orderInsertForm.FillStuffs();
            }

            //if (EditingOrder != null && !EditingOrderStuffsInitialized)
            //{
            //    var EditingSaleOrderStuffs = EditingOrder.SaleOrderStuffs.Where(a => !a.FreeProduct);
            //    foreach (var saleOrderStuff in EditingSaleOrderStuffs)
            //    {
            //        var StuffInList = AllStuffsData.SingleOrDefault(a => a.StuffId == saleOrderStuff.Package.StuffId);
            //        if (StuffInList != null)
            //        {
            //            if (StuffInList.HasBatchNumbers)
            //                StuffInList = StuffInList.StuffRow_BatchNumberRows.SingleOrDefault(a => a.BatchNumberId == saleOrderStuff.BatchNumberId.GetValueOrDefault(Guid.Empty));
            //            if (StuffInList != null)
            //            {
            //                if (StuffInList.BatchNumberId.HasValue)
            //                    StuffInList.BatchNumberRow_StuffParentRow.SelectedPackage = saleOrderStuff.Package;
            //                else
            //                    StuffInList.SelectedPackage = saleOrderStuff.Package;
            //                StuffInList.Quantity = saleOrderStuff.Quantity;
            //            }
            //        }
            //    }

            //    EditingOrderStuffsInitialized = true;
            //}

            var filteredStuffs = await App.DB.FilterStuffsAsync(AllStuffsData, Filter);
            
            filteredStuffs = filteredStuffs.Where(a => a.TotalStuffQuantity > 0).ToList();

            
            try
            {
                var stuffsListTemp = filteredStuffs.ToList();

                foreach (var item in stuffsListTemp)
                    item.SelectedPackage = item.PackagesData.OrderByDescending(a => item.HasBatchNumbers ? item.StuffRow_BatchNumberRows.Sum(b => b.PackagesData.Single(c => c.Package.Id == a.Package.Id).Quantity * b.PackagesData.Single(c => c.Package.Id == a.Package.Id).Package.Coefficient) : (a.Quantity * a.Package.Coefficient)).First().Package;

                var batchNumbers = stuffsListTemp.Where(a => !a.IsGroup).SelectMany(a => a.StuffRow_BatchNumberRows).ToList();

                stuffsListTemp.AddRange(batchNumbers);

                var stuffsListOrderDic = stuffsListTemp.Where(a => !a.BatchNumberId.HasValue).Select((a, index) => new { a, index }).ToDictionary(a => a.a.StuffId, a => a.index);
                foreach (var item in stuffsListTemp)
                    item.OddRow = stuffsListOrderDic[item.StuffId] % 2 == 1;

                stuffsListTemp = stuffsListTemp.OrderBy(a => stuffsListOrderDic[a.StuffId]).ThenBy(a => a.BatchNumberId.HasValue).ToList();

                _stuffsList = new ObservableCollection<DBRepository.StuffListModel>(stuffsListTemp);
            }
            catch (Exception err)
            {
                var wefwef = err;
            }
            
            StuffItems.ItemsSource = null;
            StuffItems.ItemsSource = _stuffsList;

            StuffItems.IsRefreshing = false;
        }
    }
}
