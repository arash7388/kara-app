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
        ObservableCollection<DBRepository.StuffListModel> _StuffsList = null;
        private Partner _SelectedPartner;
        public Partner SelectedPartner
        {
            get { return _SelectedPartner; }
            set
            {
                _SelectedPartner = value;
                PartnerSelected();
            }
        }
        private ToolbarItem ToolbarItem_OrderPreviewForm, ToolbarItem_SaveOrder;
        private SettlementType[] SettlementTypes;
        private PartnerListForm PartnerListForm;
        private InsertedInformations_Orders OrdersForm;
        public Guid? EditingSaleOrderId;
        private SaleOrder EditingSaleOrder;
        private OrderInsertForm OrderInsertForm;
        private Guid? SettlementTypeId;
        MyKeyboard<QuantityEditingStuffModel, decimal> QuantityKeyboard;
        Guid? WarehouseId;
        bool FromTour;
        DtoReversionReason[] ReversionReasons = null;
        //Dictionary<Guid, decimal> PackagesQuantity;

        public OrderBeforePreviewForm
        (
            List<DBRepository.StuffListModel> AllStuffsData,
            Partner Partner,
            SaleOrder SaleOrder,
            PartnerListForm PartnerListForm,
            InsertedInformations_Orders OrdersForm,
            Guid? _SettlementTypeId,
            string Description,
            OrderInsertForm OrderInsertForm,
            bool CanChangePartner,
            Guid? WarehouseId,
            bool fromTour=false
        )
        {
            InitializeComponent();

            FromTour = fromTour;

            this.WarehouseId = WarehouseId;

            this.OrderInsertForm = OrderInsertForm;

            EditingSaleOrderId = SaleOrder != null ? SaleOrder.Id : new Nullable<Guid>();
            EditingSaleOrder = SaleOrder;
            this.AllStuffsData = AllStuffsData;

            this.PartnerListForm = PartnerListForm;
            this.OrdersForm = OrdersForm;

            CustomStuffListCell.UnitNameTapEventHandler = (s, e) => {
                var Ids = ((string)((TappedEventArgs)e).Parameter).Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                var StuffId = new Guid(Ids[0]);
                var BatchNumberId = Ids.Length == 2 ? new Guid(Ids[1]) : new Nullable<Guid>();
                UnitNameClicked(StuffId);
            };
            CustomStuffListCell.QuantityTextBoxTapEventHandler = (s, e) => {
                var Ids = ((string)((TappedEventArgs)e).Parameter).Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                var StuffId = new Guid(Ids[0]);
                var BatchNumberId = Ids.Length == 2 ? new Guid(Ids[1]) : new Nullable<Guid>();
                FocusedQuantityTextBoxId = BatchNumberId.HasValue ? new Guid[] { StuffId, BatchNumberId.Value } : new Guid[] { StuffId };
            };
            CustomStuffListCell.QuantityPlusTapEventHandler = (s, e) => {
                var Ids = ((string)((TappedEventArgs)e).Parameter).Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                var StuffId = new Guid(Ids[0]);
                var BatchNumberId = Ids.Length == 2 ? new Guid(Ids[1]) : new Nullable<Guid>();
                QuantityPlusClicked(StuffId, BatchNumberId);
            };
            CustomStuffListCell.QuantityMinusTapEventHandler = (s, e) => {
                var Ids = ((string)((TappedEventArgs)e).Parameter).Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                var StuffId = new Guid(Ids[0]);
                var BatchNumberId = Ids.Length == 2 ? new Guid(Ids[1]) : new Nullable<Guid>();
                QuantityMinusClicked(StuffId, BatchNumberId);
            };
            StuffItems.ItemTemplate = new DataTemplate(typeof(CustomStuffListCell));

            SelectedPartner = Partner != null ? Partner : SaleOrder != null ? SaleOrder.Partner : null;
            if (CanChangePartner)
                PartnerChangeButton.IsEnabled = true;
            else
                PartnerChangeButton.IsEnabled = false;
            PartnerLabel.FontSize *= 1.5;
            
            var PartnerChangeButtonTapGestureRecognizer = new TapGestureRecognizer();
            PartnerChangeButtonTapGestureRecognizer.Tapped += (sender, e) => {
                if (PartnerChangeButton.IsEnabled)
                {
                    FocusedQuantityTextBoxId = null;
                    var PartnerListForm1 = new PartnerListForm();
                    PartnerListForm1.OrderBeforePreviewForm = this;
                    Navigation.PushAsync(PartnerListForm1);
                }
            };
            PartnerChangeButtonTapGestureRecognizer.SetBinding(TapGestureRecognizer.CommandParameterProperty, "Id");
            PartnerChangeButton.GestureRecognizers.Add(PartnerChangeButtonTapGestureRecognizer);
            PartnerChangeButton.WidthRequest = 150;

            StuffItems.HasUnevenRows = true;
            StuffItems.SeparatorVisibility = SeparatorVisibility.None;
            StuffItems.ItemSelected += StuffItems_ItemSelected;

            ToolbarItem_OrderPreviewForm = new ToolbarItem();
            ToolbarItem_OrderPreviewForm.Text = "پیش نمایش سفارش";
            ToolbarItem_OrderPreviewForm.Icon = "ShowInvoice.png";
            ToolbarItem_OrderPreviewForm.Activated += ToolbarItem_OrderPreviewForm_Activated;
            ToolbarItem_OrderPreviewForm.Order = ToolbarItemOrder.Primary;
            ToolbarItem_OrderPreviewForm.Priority = 2;
            if (!this.ToolbarItems.Contains(ToolbarItem_OrderPreviewForm))
                this.ToolbarItems.Add(ToolbarItem_OrderPreviewForm);

            if (FromTour)
            {
                ToolbarItem_SaveOrder = new ToolbarItem();
                ToolbarItem_SaveOrder.Text = "ذخیره";
                ToolbarItem_SaveOrder.Icon = "Upload.png";
                ToolbarItem_SaveOrder.Activated += ToolbarItem_SaveOrder_Activated;
                ToolbarItem_SaveOrder.Order = ToolbarItemOrder.Primary;
                ToolbarItem_SaveOrder.Priority = 3;

                if (!this.ToolbarItems.Contains(ToolbarItem_SaveOrder))
                    this.ToolbarItems.Add(ToolbarItem_SaveOrder);
            }
            

            SettlementTypes = App.SettlementTypes.Where(a => a.Enabled).ToArray();
            foreach (var SettlementType in SettlementTypes)
                SettlementTypePicker.Items.Add(SettlementType.Name);

            if(SaleOrder != null)
                _SettlementTypeId = SaleOrder.SettlementTypeId;

            if (_SettlementTypeId.HasValue && _SettlementTypeId.Value!=Guid.Parse("00000000-0000-0000-0000-000000000000"))
            {
                SettlementTypePicker.SelectedIndex = SettlementTypes.Select((a, index) => new { a, index }).Single(a => a.a.Id == _SettlementTypeId).index;
                SettlementTypeLabel.Text = SettlementTypes[SettlementTypePicker.SelectedIndex].Name;
            }

            SettlementTypeId = _SettlementTypeId;

            SettlementTypePicker.SelectedIndexChanged += (sender, e) => {
                FocusedQuantityTextBoxId = null;

                SettlementTypeLabel.Text = SettlementTypes[SettlementTypePicker.SelectedIndex].Name;

                var SettlementType = SettlementTypePicker.SelectedIndex == -1 ? null : SettlementTypes[SettlementTypePicker.SelectedIndex];
                this.SettlementTypeId = SettlementType == null ? new Nullable<Guid>() : SettlementType.Id;
                OrderInsertForm.SettlementTypeId = SettlementTypeId;
            };

            var _Description = SaleOrder != null ? SaleOrder.Description : Description;
            DescriptionEditor.Text = !string.IsNullOrEmpty(_Description) ? _Description : "";
            
            QuantityKeyboard = new MyKeyboard<QuantityEditingStuffModel, decimal>
            (
                QuantityKeyboardHolder,
                new Command((parameter) => {        //OnOK
                    FocusedQuantityTextBoxId = null;
                    CheckToBackToOrderInsertFormIfStuffsEmpty();
                }),
                new Command((parameter) => {        //OnChange
                    CheckToBackToOrderInsertFormIfStuffsEmpty();
                })
                //new Command((parameter) => {        //OnOK
                //    var Value = (decimal)parameter;
                //    //var StuffModel3 = OrderInsertForm.StuffsList.SingleOrDefault(a => a.StuffId == FocusedQuantityTextBoxId);
                //    var StuffModel4 = OrderInsertForm.AllStuffsData.Where(a => a.StuffId == FocusedQuantityTextBoxId).ToArray();
                //    //if (StuffModel3 != null)
                //    //    StuffModel3.Quantity = Value;
                //    foreach (var item in StuffModel4)
                //        item.Quantity = Value;
                //    FocusedQuantityTextBoxId = null;
                //    CheckToBackToOrderInsertFormIfStuffsEmpty();
                //}),
                //new Command((parameter) => {        //OnChange
                //    var Value = (decimal)parameter;
                //    //var StuffModel3 = OrderInsertForm.StuffsList.SingleOrDefault(a => a.StuffId == FocusedQuantityTextBoxId);
                //    var StuffModel4 = OrderInsertForm.AllStuffsData.Where(a => a.StuffId == FocusedQuantityTextBoxId).ToArray();
                //    //if (StuffModel3 != null)
                //    //    StuffModel3.Quantity = Value;
                //    foreach (var item in StuffModel4)
                //        item.Quantity = Value;
                //    CheckToBackToOrderInsertFormIfStuffsEmpty();
                //})
            );

            if(FromTour)
            {
                gridReasons.IsVisible = true;
                FillReversionReasons();
            }
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
                    DistributionReversionReasonId = ReversionReasons[ReversionReasonPicker.SelectedIndex].Id,
                    OrderId = (Guid)EditingSaleOrderId,
                    UserId = App.UserId.Value,
                    Description = EditingSaleOrder?.Description.ToSafeString()=="" ? "***" : DescriptionEditor.Text,
                    Stuffs = _StuffsList.Where(a => a.Quantity > 0).Select(a => new DtoEditStuffMobile
                    {
                        ArticleId = EditingSaleOrder?.SaleOrderStuffs?.SingleOrDefault(x=>x.Package.StuffId==a.StuffId)?.ArticleId,
                        StuffId = a.StuffId.ToSafeGuid(),
                        PackageId = a.SelectedPackage.Id,
                        Quantity = a.Quantity,
                        SalePrice = a.Price.ToLatinDigits().ToSafeDecimal(),
                        StuffQuantity = a.SelectedPackage.Coefficient * a.Quantity,
                        BatchNumberId = a.BatchNumberId
                    }).ToList()
                };

                var submitResult = await Connectivity.EditSaleOrder(data);

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
            OrderInsertForm.Description = ((CustomRenderer.PlaceholderEditor)sender).Text;
        }
        
        private void ToolbarItem_OrderPreviewForm_Activated(object sender, EventArgs e)
        {
            FocusedQuantityTextBoxId = null;
            if (SelectedPartner == null && !SettlementTypeId.HasValue)
            {
                App.ShowError("خطا", "مشتری و نحوه تسویه را مشخص کنید.", "خوب");
                return;
            }
            else if(SelectedPartner == null)
            {
                App.ShowError("خطا", "مشتری را مشخص کنید.", "خوب");
                return;
            }
            else if (!SettlementTypeId.HasValue)
            {
                App.ShowError("خطا", "نحوه تسویه را مشخص کنید.", "خوب");
                return;
            }

            var WithoutPriceStuffs = AllStuffsData.Where(a => a.Quantity != 0 && !a._UnitPrice.HasValue).ToList();
            if (WithoutPriceStuffs.Any())
            {
                var Message = "برخی اقلام در لیست قیمت این مشتری ثبت نشده اند:\n" + WithoutPriceStuffs.Select(a => a.StuffData.Name).Aggregate((sum, x) => sum + "\n" + x);
                App.ShowError("خطا", Message, "خوب");
                return;
            }

            SaleOrder SaleOrder;
            try
            {
                SaleOrder = MakeOrder(SelectedPartner.Id, SettlementTypeId.Value, DescriptionEditor.Text);
            }
            catch (Exception err)
            {
                App.ShowError("خطا", err.ProperMessage(), "خوب");
                return;
            }
            
            var OrderPreviewForm = new OrderPreviewForm(SaleOrder, OrdersForm, PartnerListForm, OrderInsertForm, this, true)
            {
                StartColor = Color.FromHex("ffffff"),
                EndColor = Color.FromHex("ffffff")
            };
            this.Navigation.PushAsync(OrderPreviewForm);
        }

        private SaleOrder MakeOrder(Guid PartnerId, Guid SettlementTypeId, string Description)
        {
            var SaleOrder = new SaleOrder()
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
                WarehouseId = WarehouseId
            };

            var _SaleOrderStuffs = AllStuffsData.Where(a => !a.HasBatchNumbers).SelectMany(a => a.PackagesData.Where(b => b.Quantity != 0).Select(b => new { Stuff = a, BatchNumber = (DBRepository.StuffListModel)null, Package = b }))
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

            var MinSaleConflicts = _SaleOrderStuffs.GroupBy(a => a.StuffData).Where(a => a.Sum(b => b.Quantity * b.PackageData.Coefficient) < a.Key.MinForSale).ToArray();
            if (MinSaleConflicts.Any())
                throw new Exception("کالا" + (MinSaleConflicts.Count() > 1 ? "ها" : "") + "ی زیر کمتر از حداقل تعیین شده در سیستم سفارش داده شده اند:\n" +
                    MinSaleConflicts.Select(a => a.Key.Name + " (حداقل سفارش: " + a.Key.MinForSale + " " + a.Key.Packages.Single(b => b.Coefficient == 1).Name + ")").Aggregate((sum, x) => sum + "\n" + x));

            var SaleCoefficientConflicts = _SaleOrderStuffs.Where(a => a.StuffData.SaleCoefficient != 0 && a.StuffData.SaleCoefficient != 1).GroupBy(a => a.StuffData).Where(a => a.Sum(b => b.Quantity * b.PackageData.Coefficient) % a.Key.SaleCoefficient != 0).ToArray();
            if (SaleCoefficientConflicts.Any())
                throw new Exception("تعداد سفارش کالا" + (SaleCoefficientConflicts.Count() > 1 ? "ها" : "") + "ی زیر با ضریب فروش تعیین شده در سیستم مغایرت دارد:\n" +
                    SaleCoefficientConflicts.Select(a => a.Key.Name + " (ضریب فروش: " + a.Key.SaleCoefficient + " " + a.Key.Packages.Single(b => b.Coefficient == 1).Name + ")").Aggregate((sum, x) => sum + "\n" + x));

            var SaleOrderStuffs = _SaleOrderStuffs.Select(a => new SaleOrderStuff()
            {
                Id = a.Id ==string.Empty || a.Id==null ? Guid.NewGuid() : a.Id.Replace("|","").ToSafeGuid(),
                OrderId = SaleOrder.Id,
                SaleOrder = SaleOrder,
                ArticleIndex = a.ArticleIndex,
                PackageId = a.PackageData.Id,
                BatchNumberId = a.BatchNumberData != null ? a.BatchNumberData.BatchNumberId : new Nullable<Guid>(),
                Quantity = a.Quantity,
                SalePrice = a.PackagePrice,
                DiscountPercent = 0,
                VATPercent = SaleOrder.Partner.CalculateVATForThisPerson && a.PackageData.Stuff.HasVAT ? App.VATPercent.Value : 0,
                FreeProduct = false,
                FreeProduct_UnitPrice = null
            }).ToArray();

            var SaleOrderCashDiscounts = new CashDiscount[] { };

            SaleOrder.SaleOrderStuffs = SaleOrderStuffs;
            SaleOrder.CashDiscounts = SaleOrderCashDiscounts;
            
            SaleOrder = App.DB.CalculateProporatedDiscount(SaleOrder);

            return SaleOrder;
        }

        private void PartnerSelected()
        {
            PartnerLabel.Text = SelectedPartner == null ? "مشتری" :
                !string.IsNullOrEmpty(SelectedPartner.LegalName) ? (SelectedPartner.LegalName + (!string.IsNullOrEmpty(SelectedPartner.Name) ? (" (" + SelectedPartner.Name + ")") : "")) : (SelectedPartner.Name);
            FillStuffs("", EditingSaleOrderId, true);
            OrderInsertForm.SelectedPartner = SelectedPartner;
        }
        
        class QuantityEditingStuffModel
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
                                    StuffModel.QuantityFocused = true;
                            }
                            if (StuffModel != null)
                            {
                                KeyboardObject = new QuantityEditingStuffModel(StuffModel, StuffModel.Quantity);
                                QuantityKeyboard.SetObject(KeyboardObject, a => a.Quantity);
                                QuantityKeyboard.Show();
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

        public void QuantityPlusClicked(Guid StuffId, Guid? BatchNumberId)
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
                        StuffModel.Quantity++;
                }
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
            _FocusedQuantityTextBoxId = null;
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
            }
        }

        public List<DBRepository.StuffListModel> AllStuffsData;
        private async Task FillStuffs(string Filter, Guid? EditingOrderId, bool RefreshStuffsData)
        {
            StuffItems.IsRefreshing = true;
            await Task.Delay(100);
            if (AllStuffsData == null || RefreshStuffsData)
            {
                var StuffsResult = await App.DB.GetAllStuffsListAsync(SelectedPartner != null ? SelectedPartner.Id : new Guid?(), EditingOrderId, true, WarehouseId);
                if (!StuffsResult.Success)
                {
                    App.ShowError("خطا", "در نمایش لیست کالاها خطایی رخ داد.\n" + StuffsResult.Message, "خوب");
                    StuffItems.IsRefreshing = false;
                    return;
                }
                var NewStuffsData = StuffsResult.Data[0];
                
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
                OrderInsertForm.AllStuffsData = NewStuffsData;
                OrderInsertForm.FillStuffs();
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

            var FilteredStuffs = await App.DB.FilterStuffsAsync(AllStuffsData, Filter);
            
            FilteredStuffs = FilteredStuffs.Where(a => a.TotalStuffQuantity > 0).ToList();

            
            try
            {
                var StuffsListTemp = FilteredStuffs.ToList();

                foreach (var item in StuffsListTemp)
                    item.SelectedPackage = item.PackagesData.OrderByDescending(a => item.HasBatchNumbers ? item.StuffRow_BatchNumberRows.Sum(b => b.PackagesData.Single(c => c.Package.Id == a.Package.Id).Quantity * b.PackagesData.Single(c => c.Package.Id == a.Package.Id).Package.Coefficient) : (a.Quantity * a.Package.Coefficient)).First().Package;

                var BatchNumbers = StuffsListTemp.Where(a => !a.IsGroup).SelectMany(a => a.StuffRow_BatchNumberRows).ToList();

                StuffsListTemp.AddRange(BatchNumbers);

                var StuffsListOrderDic = StuffsListTemp.Where(a => !a.BatchNumberId.HasValue).Select((a, index) => new { a, index }).ToDictionary(a => a.a.StuffId, a => a.index);
                foreach (var item in StuffsListTemp)
                    item.OddRow = StuffsListOrderDic[item.StuffId] % 2 == 1;

                StuffsListTemp = StuffsListTemp.OrderBy(a => StuffsListOrderDic[a.StuffId]).ThenBy(a => a.BatchNumberId.HasValue).ToList();

                _StuffsList = new ObservableCollection<DBRepository.StuffListModel>(StuffsListTemp);
            }
            catch (Exception err)
            {
                var wefwef = err;
            }
            
            StuffItems.ItemsSource = null;
            StuffItems.ItemsSource = _StuffsList;

            StuffItems.IsRefreshing = false;
        }
    }
}
