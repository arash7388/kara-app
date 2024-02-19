using Kara.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Kara.Assets
{
    public enum ConditionType
    {
        Partner = 1,
        PartnerGroup = 2,
        Zone = 3,
        Visitor = 4,
        Stuff = 5,
        StuffGroup = 6,
        StuffBasket = 7,
        SettlementType = 8,
        SettlementDay = 9
    }

    public enum DiscountBasedOn
    {
        TotalQuantitySum = 1,
        TotalPriceSum = 2,
        QuantityOfAnArticle = 3,
        PriceOfAnArticle = 4,
        CountOfArticles = 5,
        QuantityOfSomeArticles = 6,
        CombinedStuffBasket = 7
    }

    public enum DiscountWay
    {
        Percent = 1,
        SpecificMoney = 2,
        FreeProduct = 3,
        CashSettlementPercent = 4
    }

    public enum RatioCalculationMethod
    {
        Round = 1,
        RoundToBottom = 2
    }
    
    public class OrderModel
    {
        public Guid SettlementTypeId { get; set; }
        public int SettlementDay { get; set; }
        public Guid VisitorId { get; set; }
        public DateTime OrderInsertDate { get; set; }
        public Partner Partner { get; set; }
        public ArticleModel[] Articles { get; set; }
        public decimal ArticlesPriceSum { get { return Articles.Any() ? Articles.Sum(a => a.Price) : 0; } }
        public decimal ArticlesFinalPriceSum { get { return Articles.Any() ? Articles.Sum(a => a.FinalPrice) : 0; } }
        public decimal DiscountPercent { get; set; }
        public decimal DiscountAmount { get { return Math.Round(DiscountPercent * ArticlesFinalPriceSum / 100); } }
        public decimal CashPrise { get; set; }
        public decimal FinalPrice { get { return ArticlesFinalPriceSum - DiscountAmount - CashPrise; } }


        public List<FreeProductModel> FreeProducts { get; set; }
        public List<CashSettlementDiscountModel> CashSettlementDiscounts { get; set; }
        public bool AggregateOtherDiscounts_Percent { get; set; }
        public bool AggregateOtherDiscounts_SpecificMoney { get; set; }
        public bool AggregateOtherDiscounts_FreeProduct { get; set; }
        public bool AggregateOtherDiscounts_CashSettlement { get; set; }

        public OrderModel Clone()
        {
            return new OrderModel()
            {
                SettlementTypeId = this.SettlementTypeId,
                SettlementDay = this.SettlementDay,
                VisitorId = this.VisitorId,
                OrderInsertDate = this.OrderInsertDate,
                Partner = this.Partner,
                Articles = this.Articles.Select(a => a.Clone()).ToArray(),
                DiscountPercent = this.DiscountPercent,
                CashPrise = this.CashPrise,
                FreeProducts = this.FreeProducts.Select(a => a.Clone()).ToList(),
                CashSettlementDiscounts = this.CashSettlementDiscounts.Select(a => a.Clone()).ToList(),
                AggregateOtherDiscounts_Percent = this.AggregateOtherDiscounts_Percent,
                AggregateOtherDiscounts_SpecificMoney = this.AggregateOtherDiscounts_SpecificMoney,
                AggregateOtherDiscounts_FreeProduct = this.AggregateOtherDiscounts_FreeProduct,
                AggregateOtherDiscounts_CashSettlement = this.AggregateOtherDiscounts_CashSettlement
            };
        }
    }

    public class ArticleModel
    {
        public Guid Id { get; set; }
        public Stuff Stuff { get; set; }
        public Package Package { get; set; }
        public StuffBatchNumber BatchNumber { get; set; }
        public decimal Quantity { get; set; }
        public decimal StuffQuantity { get { return Quantity * Package.Coefficient; } }
        public decimal UnitPrice { get; set; }
        public decimal FeeChange { get; set; }
        public decimal Price { get { return Math.Round((UnitPrice + FeeChange) * StuffQuantity); } }
        public decimal DiscountPercent { get; set; }
        public decimal DiscountAmount { get { return Math.Round(DiscountPercent * Price / 100); } }
        public decimal CashDiscountPercent { get; set; }
        public decimal CashDiscountAmount { get { return Math.Round(CashDiscountPercent * (Price - DiscountAmount) / 100); } }
        public decimal FinalPrice { get { return Price - DiscountAmount - CashDiscountAmount; } }

        public bool ChangePercentToFeeChange { get; set; }
        public bool AggregateOtherDiscounts_Percent { get; set; }
        public bool AggregateOtherDiscounts_SpecificMoney { get; set; }
        public bool AggregateOtherDiscounts_FreeProduct { get; set; }
        public bool AggregateOtherDiscounts_CashSettlement { get; set; }
        public decimal StuffCalculatedCoefficient { get; set; }
        public decimal SpecialStuffQuantity { get; set; }

        public ArticleModel Clone()
        {
            return new ArticleModel()
            {
                Id = this.Id,
                Stuff = this.Stuff,
                Package = this.Package,
                BatchNumber = this.BatchNumber,
                Quantity = this.Quantity,
                UnitPrice = this.UnitPrice,
                FeeChange = this.FeeChange,
                DiscountPercent = this.DiscountPercent,
                CashDiscountPercent = this.CashDiscountPercent,
                ChangePercentToFeeChange = this.ChangePercentToFeeChange,
                AggregateOtherDiscounts_Percent = this.AggregateOtherDiscounts_Percent,
                AggregateOtherDiscounts_SpecificMoney = this.AggregateOtherDiscounts_SpecificMoney,
                AggregateOtherDiscounts_FreeProduct = this.AggregateOtherDiscounts_FreeProduct,
                AggregateOtherDiscounts_CashSettlement = this.AggregateOtherDiscounts_CashSettlement,
                StuffCalculatedCoefficient = this.StuffCalculatedCoefficient,
                SpecialStuffQuantity = this.SpecialStuffQuantity
            };
        }
    }

    public class FreeProductModel
    {
        public Stuff Stuff { get; set;}
        public Package Package { get; set;}
        public StuffBatchNumber BatchNumber { get; set;}
        public decimal Quantity { get; set; }
        public decimal StuffQuantity { get { return Quantity * Package.Coefficient; } }
        public FreeProductModel Clone()
        {
            return new FreeProductModel()
            {
                Stuff = this.Stuff,
                Package = this.Package,
                BatchNumber = this.BatchNumber,
                Quantity = this.Quantity
            };
        }
    }

    public class CashSettlementDiscountModel
    {
        public int Day {get;set;}
        public decimal Percent {get;set;}
        public CashSettlementDiscountModel Clone()
        {
            return new CashSettlementDiscountModel()
            {
                Day = this.Day,
                Percent = this.Percent
            };
        }
    }

    public class RuleModel
    {
        public Guid RuleId { get; set; }
        public string RuleDescription { get; set; }
        public double Priority { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public DiscountBasedOn BasedOn { get; set; }
        public DiscountWay Way { get; set; }
        public bool ConditionOnPartner { get; set; }
        public Guid[] PartnerIds { get; set; }
        public bool ConditionOnPartnerGroup { get; set; }
        public Guid[] PartnerGroupIds { get; set; }
        public bool ConditionOnZone { get; set; }
        public string[] ZoneGeneralCodes { get; set; }
        public bool ConditionOnVisitor { get; set; }
        public Guid[] VisitorIds { get; set; }
        public bool ConditionOnStuff { get; set; }
        public Guid[] StuffIds { get; set; }
        public Guid?[] StuffConditionExtraParameters_UnitId { get; set; }
        public decimal?[] StuffConditionExtraParameters_Quantity { get; set; }
        public bool ConditionOnStuffGroup { get; set; }
        public string[] StuffGroupCodes { get; set; }
        public bool ConditionOnStuffBasket { get; set; }
        public Guid[] StuffBasketIds { get; set; }
        public bool ConditionOnSettlementType { get; set; }
        public Guid[] SettlementTypeIds { get; set; }
        public bool ConditionOnSettlementDay { get; set; }
        public int? SettlementDay { get; set; }
        public bool AggregateOtherDiscounts_Percent { get; set; }
        public bool AggregateOtherDiscounts_SpecificMoney { get; set; }
        public bool AggregateOtherDiscounts_FreeProduct { get; set; }
        public bool AggregateOtherDiscounts_CashSettlement { get; set; }
        public bool CombineStuffBaskets_SpecifyEachSoledBasket { get; set; }
        public bool CombineStuffBaskets_SpecifyEachBasketWhichGetDiscount { get; set; }
        public DiscountRuleCombinedStuffBasket[] CombinedStuffBaskets_Bought { get; set; }
        public DiscountRuleCombinedStuffBasket[] CombinedStuffBaskets_Discount { get; set; }
        public RuleStepModel[] Steps { get; set; }
        public bool ShowDiscountInUnitFee { get; set; }
    }
    public class DiscountRuleCombinedStuffBasket
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string[] StuffGroupCodes { get; set; }
        public Guid[] StuffIds { get; set; }
    }
    public class RuleStepModel
    {
        public int StepIndex { get; set; }
        public Guid?[] SpecificUnitIds { get; set; }
        public Guid? SpecificStuffId { get; set; }
        public Guid? UnitId { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public int CashSettlementDay { get; set; }
        public int RowCount { get; set; }
        public decimal Percent { get; set; }
        public decimal SpecificMoney { get; set; }
        public decimal FreeProductQuantity { get; set; }
        public bool AscendingCalculation { get; set; }
        public bool ContinueToCalculateOnThisRatio { get; set; }
        public RatioCalculationMethod RatioCalculationMethod { get; set; }
        public Guid[] BoughtBasketIds { get; set; }
        public Guid[] DiscountBasketIds { get; set; }
    }

    public class DiscountCalculator
    {
        Dictionary<Guid, Stuff> AllSystemStuffs;
        string SystemName;
        bool AllowOptionalDiscountRules_MultiSelection;
        OrderModel Order;
        Dictionary<int, Dictionary<string, List<RuleModel>>> DiscountRules;
        private readonly List<KeyValuePair<Guid, RuleModel>> _userSelectedOptionalDiscounts;

        public DiscountCalculator(string SystemName, bool AllowOptionalDiscountRules_MultiSelection,
            Dictionary<Guid, Stuff> AllSystemStuffs, OrderModel Order,
            Dictionary<int, Dictionary<string, List<RuleModel>>> DiscountRules,
            List<KeyValuePair<Guid, RuleModel>> userSelectedOptionalDiscounts)
        {
            this.SystemName = SystemName;
            this.AllSystemStuffs = AllSystemStuffs;
            this.AllowOptionalDiscountRules_MultiSelection = AllowOptionalDiscountRules_MultiSelection;
            this.Order = Order;
            this.DiscountRules = DiscountRules;
            _userSelectedOptionalDiscounts = userSelectedOptionalDiscounts;
        }
        
        //TODO
        public void ShowDiscountSelctions()
        {
            #region TODO_Body
            /*
            var ShowingPriorities = [];
            for (var i = 0; i < ChoosedOptions.Length; i++) {
                if (ChoosedOptions[i].Rules.Length > 1)
                    ShowingPriorities[ShowingPriorities.Length] = ChoosedOptions[i];
            }
            if (ShowingPriorities.Length != 0) {
                try {
                    $('#OtionalDiscountsSelctionTR').show();
                } catch (e) {
                }
                var html = '<table style="width:100%;">';
                for (var i = 0; i < ShowingPriorities.Length; i++) {
                    html += '<tr><td style="font-size:120%;" rowspan="' + ShowingPriorities[i].Rules.Length + '">' + ShowingPriorities[i].Rules[0].Rules[0].Priority + '</td><td rowspan="' + ShowingPriorities[i].Rules.Length + '" style="height:' + (17 * ShowingPriorities[i].Rules.Length) + 'px;"><div class="CurelyBracket Right" style="display:inline-block; height:' + (17 * ShowingPriorities[i].Rules.Length) + 'px;"></div></td>';
                    for (var j = 0; j < ShowingPriorities[i].Rules.Length; j++) {
                        if (j != 0)
                            html += '<tr>';
                        if (AllowOptionalDiscountRules_MultiSelection)
                            html += '<td style="height:17px;"><input type="checkbox" name="' + ShowingPriorities[i].Rules[j].Rules[0].Priority + '_SelectDiscountRule" ' + (ShowingPriorities[i].Rules[j].Choosed ? 'checked="checked"' : '') + ' id="' + ShowingPriorities[i].Rules[j].RuleId + '_SelectDiscountRule" /></td>';
                        else
                            html += '<td style="height:17px;"><input type="radio" name="' + ShowingPriorities[i].Rules[j].Rules[0].Priority + '_SelectDiscountRule" ' + (ShowingPriorities[i].Rules[j].Choosed ? 'checked="checked"' : '') + ' id="' + ShowingPriorities[i].Rules[j].RuleId + '_SelectDiscountRule" /></td>';
                        html += '<td style="width:99%;">' + ShowingPriorities[i].Rules[j].Rules[0].RuleDescription + '</td>';
                        html += '</tr>';
                    }
                }
                html += '</table>';
                try {
                    $('#OtionalDiscountsSelctionForm').html(html);
                    $('[id*=_SelectDiscountRule]').each(function () {
                        $(this).change(function () {
                            if ($(this).attr('checked') || AllowOptionalDiscountRules_MultiSelection) {
                                var ThisPriority = parseInt($(this).attr('name').replace('_SelectDiscountRule', ''));
                                var ThisRuleId = $(this).attr('id').replace('_SelectDiscountRule', '');

                                RuleSelectionChanged(ThisPriority, ThisRuleId, null);
                            }
                        });
                    });
                } catch (e) {
                }
            }
            else {
                try {
                    $('#OtionalDiscountsSelctionTR').hide();
                } catch (e) {
                }
            }
            */
            #endregion
        }

        //TODO
        public void RuleSelectionChanged(object ThisPriority, object ThisRuleId, object Callback)
        {
            #region TODO_Body
            /*
            for (var j = 0; j < ChoosedOptions.Length; j++) {
                if (ChoosedOptions[j].Priority == ThisPriority) {
                    for (var k = 0; k < ChoosedOptions[j].Rules.Length; k++) {
                        if (AllowOptionalDiscountRules_MultiSelection) {
                            try {
                                ChoosedOptions[j].Rules[k].Choosed = $('#' + ChoosedOptions[j].Rules[k].RuleId + '_SelectDiscountRule').attr('checked');
                            } catch (e) {
                            }
                        }
                        else {
                            if (ChoosedOptions[j].Rules[k].RuleId == ThisRuleId)
                                ChoosedOptions[j].Rules[k].Choosed = true;
                            else
                                ChoosedOptions[j].Rules[k].Choosed = false;
                        }
                    }
                }
            }

            try {
                //on Browser
                CalculateTheDiscounts(DataGrid_Data['SaleOrderDataGrid'], 0, DataGrid_DataSum['SaleOrderDataGrid'], DataGrid_Data['SaleOrder_FreeProductDataGrid']);
            } catch (e) {
                //on Android
                GoForPreOrderPreview();
            }
    

            if (Callback)
                Callback();
            */
            #endregion
        }

        Dictionary<int, List<KeyValuePair<KeyValuePair<string, List<RuleModel>>, bool>>> ChoosedOptions = new Dictionary<int, List<KeyValuePair<KeyValuePair<string, List<RuleModel>>, bool>>>();
        public OrderModel ClaculateOrderDiscounts()
        {
            for (var i = 0; i < Order.Articles.Length; i++)
            {
                Order.Articles[i].FeeChange = 0;
                Order.Articles[i].DiscountPercent = 0;
                Order.Articles[i].AggregateOtherDiscounts_Percent = true;
                Order.Articles[i].AggregateOtherDiscounts_SpecificMoney = true;
                Order.Articles[i].AggregateOtherDiscounts_FreeProduct = true;
                Order.Articles[i].AggregateOtherDiscounts_CashSettlement = true;
            }
            Order.DiscountPercent = 0;
            Order.CashPrise = 0;
            Order.FreeProducts = new List<FreeProductModel>();
            Order.CashSettlementDiscounts = new List<CashSettlementDiscountModel>();
            Order.AggregateOtherDiscounts_Percent = true;
            Order.AggregateOtherDiscounts_SpecificMoney = true;
            Order.AggregateOtherDiscounts_FreeProduct = true;
            Order.AggregateOtherDiscounts_CashSettlement = true;

            foreach (var BatchRules in DiscountRules)
            {
                try
                {
                    var Priority = BatchRules.Key;
                    var AffectiveRules = new List<KeyValuePair<KeyValuePair<string, List<RuleModel>>, string>>();
                    foreach (var Rule in BatchRules.Value)
                    {
                        var OriginalOrder = Order.Clone();
                        var EffectOnRows = "";
                        var ThisIsEffective = false;
                        for (var k = 0; k < Rule.Value.Count; k++)
                        {
                            var ThisEffectOnRows = "";
                            CalculateRuleOnOrder(Order, Rule.Value[k], out ThisEffectOnRows);
                            if ((ThisEffectOnRows != "" && ThisEffectOnRows != "سطرهای ") || Rule.Value[k].Way == DiscountWay.CashSettlementPercent || Rule.Value[k].BasedOn == DiscountBasedOn.CombinedStuffBasket)
                                ThisIsEffective = true;
                            EffectOnRows = EffectOnRows + "، " + ThisEffectOnRows;
                        }
                        if (ThisIsEffective)
                            AffectiveRules.Add(new KeyValuePair<KeyValuePair<string, List<RuleModel>>, string>(Rule, EffectOnRows));
                        
                        Order = OriginalOrder.Clone();
                    }
                    
                    if (AffectiveRules.Any())
                    {
                        var ThisPriorityChoosedOptions = ChoosedOptions.ContainsKey(Priority) ? ChoosedOptions[Priority] : null;
                        if (ThisPriorityChoosedOptions == null)
                        {
                            ThisPriorityChoosedOptions = AffectiveRules.Select((a, index) => new KeyValuePair<KeyValuePair<string, List<RuleModel>>, bool>(a.Key, index == 0)).ToList();
                            ChoosedOptions.Add(Priority, ThisPriorityChoosedOptions);
                        }
                        else
                        {
                            var ChoosedRuleIds = new List<string>();
                            var ChoosedRules = ThisPriorityChoosedOptions.Single(a => a.Value);
                            for (var k = 0; k < AffectiveRules.Count; k++)
                                if (AffectiveRules[k].Value == ChoosedRules.Key.Key)
                                    if (AllowOptionalDiscountRules_MultiSelection)
                                        ChoosedRuleIds.Add(AffectiveRules[k].Key.Key);
                                    else
                                    {
                                        ChoosedRuleIds = new List<string>() { AffectiveRules[k].Key.Key };
                                    }
                            
                            ThisPriorityChoosedOptions = new List<KeyValuePair<KeyValuePair<string, List<RuleModel>>, bool>>();
                            for (var k = 0; k < AffectiveRules.Count; k++)
                            {
                                var ThisOption = AffectiveRules[k].Key;
                                var Choosed = !ChoosedRuleIds.Any() && k == 0 && !AllowOptionalDiscountRules_MultiSelection;
                                for (var l = 0; l < ChoosedRuleIds.Count; l++)
                                    Choosed = Choosed || ThisOption.Key == ChoosedRuleIds[l];
                                ThisPriorityChoosedOptions.Add(new KeyValuePair<KeyValuePair<string, List<RuleModel>>, bool>(ThisOption, Choosed));
                            }
                            ChoosedOptions.Add(Priority, ThisPriorityChoosedOptions);
                        }

                        if (App.AllowOptionalDiscountRules.Value && _userSelectedOptionalDiscounts!=null)
                        {
                            var userChosenValues = new List<KeyValuePair<string, List<RuleModel>>>();
                            
                            if(ThisPriorityChoosedOptions.Count==1)
                                userChosenValues.Add(ThisPriorityChoosedOptions[0].Key);
                            else
                            for (var j = 0; j < ThisPriorityChoosedOptions.Count; j++)
                            {
                                foreach (KeyValuePair<Guid, RuleModel> userSelectedOptionalDiscount in _userSelectedOptionalDiscounts)
                                {
                                    if (ThisPriorityChoosedOptions[j].Key.Value.Select(v=>v.RuleId).Contains(userSelectedOptionalDiscount.Value.RuleId))
                                        userChosenValues.Add(ThisPriorityChoosedOptions[j].Key);
                                }
                            }

                            var effect = "";
                            for (var l = 0; l < userChosenValues.Count; l++)
                            for (var j = 0; j < userChosenValues[l].Value.Count; j++)
                                CalculateRuleOnOrder(Order, userChosenValues[l].Value[j], out effect);
                        }
                        else
                        {
                            var ChoosedRules2 = new List<KeyValuePair<string, List<RuleModel>>>();
                            for (var j = 0; j < ThisPriorityChoosedOptions.Count; j++)
                                if (ThisPriorityChoosedOptions[j].Value)
                                    ChoosedRules2.Add(ThisPriorityChoosedOptions[j].Key);

                            var effect = "";
                            for (var l = 0; l < ChoosedRules2.Count; l++)
                            for (var j = 0; j < ChoosedRules2[l].Value.Count; j++)
                                CalculateRuleOnOrder(Order, ChoosedRules2[l].Value[j], out effect);
                        }
                        
                    }
                }
                catch (Exception err)
                {
                    throw err;
                }
            }

            ShowDiscountSelctions();

            return Order;
        }

        public OrderModel CalculateRuleOnOrder(OrderModel Order, RuleModel DiscountRule, out string effectOnRows)
        {
            if(DiscountRule.Priority == 20)
            {
                var i = 0;
            }

            var EffectOnRows = "";

            decimal CalculatedCoefficient = 0;
            decimal CalculatedDiscountPercent = 0;
            decimal CalculatedCashPrise = 0;
            List<FreeProductModel> CalculatedFreeProducts = new List<FreeProductModel>();
            List<CashSettlementDiscountModel> CalculatedCashSettlementDiscounts = new List<CashSettlementDiscountModel>();
            decimal PriceSum = 0;
            int RowCount = 0;

            var RowCashDiscountIsSeperate =
            (
                DiscountRule.BasedOn == DiscountBasedOn.QuantityOfAnArticle ||
                DiscountRule.BasedOn == DiscountBasedOn.PriceOfAnArticle ||
                DiscountRule.BasedOn == DiscountBasedOn.QuantityOfSomeArticles ||
                DiscountRule.BasedOn == DiscountBasedOn.CombinedStuffBasket
            ) && DiscountRule.Way == DiscountWay.Percent && DiscountRule.ConditionOnSettlementType && SystemName == "MehradMehr";

            switch (DiscountRule.BasedOn)
            {
                case DiscountBasedOn.TotalQuantitySum:
                    #region
                    CalculatedDiscountPercent = 0;
                    CalculatedCashPrise = 0;
                    CalculatedFreeProducts = new List<FreeProductModel>();

                    if (CheckRuleConditions(DiscountRule, Order, null))
                    {
                        switch (DiscountRule.Way)
                        {
                            case DiscountWay.Percent:
                                #region
                                if (Order.AggregateOtherDiscounts_Percent)
                                {
                                    for (var i = 0; i < DiscountRule.Steps.Length; i++)
                                    {
                                        decimal QuantitySum = 0;
                                        PriceSum = 0;
                                        decimal UnusedPriceSum = 0;
                                        for (var j = 0; j < Order.Articles.Length; j++)
                                        {
                                            if (Order.Articles[j].AggregateOtherDiscounts_Percent)
                                            {
                                                for (var sui = 0; sui < DiscountRule.Steps[i].SpecificUnitIds.Length; sui++)
                                                    QuantitySum += (!DiscountRule.Steps[i].SpecificUnitIds[sui].HasValue ? Order.Articles[j].StuffQuantity : ArticleQuantityInAUnit(Order.Articles[j], DiscountRule.Steps[i].SpecificUnitIds[sui].Value));
                                                
                                                PriceSum += Order.Articles[j].FinalPrice;
                                            }
                                            else
                                            {
                                                UnusedPriceSum += Order.Articles[j].FinalPrice;
                                            }
                                        }
                                        if (QuantitySum >= DiscountRule.Steps[i].Quantity)
                                        {
                                            CalculatedDiscountPercent = (PriceSum + UnusedPriceSum) != 0 ? DiscountRule.Steps[i].Percent * PriceSum / (PriceSum + UnusedPriceSum) : 0;
                                            if (CalculatedDiscountPercent > 0)
                                                EffectOnRows = "کل فاکتور";
                                        }
                                    }
                                }
                                #endregion
                                break;
                            case DiscountWay.SpecificMoney:
                                #region
                                CalculatedCoefficient = 0;
                                while (CalculatedCoefficient < 1)
                                {
                                    decimal ThisEnumerationCalculatedCashPrise = 0;
                                    decimal ThisEnumerationCalculatedCoefficient = 0;
                                    for (var i = 0; i < DiscountRule.Steps.Length; i++)
                                    {
                                        decimal QuantitySum = 0;
                                        for (var j = 0; j < Order.Articles.Length; j++)
                                        {
                                            if (Order.Articles[j].AggregateOtherDiscounts_SpecificMoney)
                                                for (var sui = 0; sui < DiscountRule.Steps[i].SpecificUnitIds.Length; sui++)
                                                    QuantitySum += (!DiscountRule.Steps[i].SpecificUnitIds[sui].HasValue ? Order.Articles[j].StuffQuantity : ArticleQuantityInAUnit(Order.Articles[j], DiscountRule.Steps[i].SpecificUnitIds[sui].Value));
                                        }
                                        QuantitySum = Math.Round(QuantitySum * (1 - CalculatedCoefficient) * 100000) / 100000;
                                        if (QuantitySum >= DiscountRule.Steps[i].Quantity)
                                        {
                                            if (DiscountRule.Steps[i].AscendingCalculation)
                                            {
                                                var RealValue = QuantitySum / DiscountRule.Steps[i].Quantity;
                                                var IntValue = Math.Floor(RealValue);
                                                ThisEnumerationCalculatedCashPrise = DiscountRule.Steps[i].SpecificMoney * IntValue;
                                                ThisEnumerationCalculatedCoefficient = IntValue / RealValue;
                                            }
                                            else
                                            {
                                                ThisEnumerationCalculatedCashPrise = DiscountRule.Steps[i].SpecificMoney;
                                                ThisEnumerationCalculatedCoefficient = 1;
                                            }
                                        }
                                    }
                                    if (ThisEnumerationCalculatedCashPrise != 0)
                                    {
                                        CalculatedCashPrise += ThisEnumerationCalculatedCashPrise;
                                        if (CalculatedCashPrise > 0)
                                            EffectOnRows = "کل فاکتور";
                                    }
                                    else
                                        break;
                                    CalculatedCoefficient = CalculatedCoefficient + ThisEnumerationCalculatedCoefficient * (1 - CalculatedCoefficient);
                                }
                                #endregion
                                break;
                            case DiscountWay.FreeProduct:
                                #region
                                CalculatedCoefficient = 0;
                                while (CalculatedCoefficient < 1)
                                {
                                    FreeProductModel ThisEnumerationCalculatedFreeProduct = null;
                                    decimal ThisEnumerationCalculatedCoefficient = 0;
                                    for (var i = 0; i < DiscountRule.Steps.Length; i++)
                                    {
                                        decimal QuantitySum = 0;
                                        for (var j = 0; j < Order.Articles.Length; j++)
                                            if (Order.Articles[j].AggregateOtherDiscounts_FreeProduct)
                                                for (var sui = 0; sui < DiscountRule.Steps[i].SpecificUnitIds.Length; sui++)
                                                    QuantitySum += (!DiscountRule.Steps[i].SpecificUnitIds[sui].HasValue ? Order.Articles[j].StuffQuantity : ArticleQuantityInAUnit(Order.Articles[j], DiscountRule.Steps[i].SpecificUnitIds[sui].Value));

                                        QuantitySum = Math.Round(QuantitySum * (1 - CalculatedCoefficient) * 100000) / 100000;
                                        if (QuantitySum >= DiscountRule.Steps[i].Quantity)
                                        {
                                            var Package = GetStuffPackage(DiscountRule.Steps[i].SpecificStuffId.Value, DiscountRule.Steps[i].UnitId);
                                            if (Package != null)
                                            {
                                                ThisEnumerationCalculatedFreeProduct = new FreeProductModel()
                                                {
                                                    Stuff = AllSystemStuffs[DiscountRule.Steps[i].SpecificStuffId.Value],
                                                    BatchNumber = null,
                                                    Package = Package,
                                                    Quantity = DiscountRule.Steps[i].FreeProductQuantity
                                                };
                                                
                                                if (DiscountRule.Steps[i].ContinueToCalculateOnThisRatio)
                                                {
                                                    var RealValue = QuantitySum / DiscountRule.Steps[i].Quantity;
                                                    var ShouldBeQuantityRounded = ThisEnumerationCalculatedFreeProduct.Quantity * RealValue;
                                                    if (DiscountRule.Steps[i].RatioCalculationMethod == RatioCalculationMethod.Round)
                                                        ShouldBeQuantityRounded = Math.Floor(ShouldBeQuantityRounded);
                                                    else
                                                        ShouldBeQuantityRounded = Math.Round(ShouldBeQuantityRounded);
                                                    
                                                    ThisEnumerationCalculatedFreeProduct.Quantity = ShouldBeQuantityRounded;
                                                    ThisEnumerationCalculatedCoefficient = 1;
                                                }
                                                else
                                                {
                                                    if (DiscountRule.Steps[i].AscendingCalculation)
                                                    {
                                                        var RealValue = QuantitySum / DiscountRule.Steps[i].Quantity;
                                                        var IntValue = Math.Floor(QuantitySum / DiscountRule.Steps[i].Quantity);
                                                        ThisEnumerationCalculatedFreeProduct.Quantity *= IntValue;
                                                        ThisEnumerationCalculatedCoefficient = IntValue / RealValue;
                                                    }
                                                    else
                                                        ThisEnumerationCalculatedCoefficient = 1;
                                                }
                                            }
                                        }
                                    }
                                    if (ThisEnumerationCalculatedFreeProduct != null)
                                    {
                                        CalculatedFreeProducts = AddToFreeProducts(CalculatedFreeProducts, ThisEnumerationCalculatedFreeProduct);
                                        if (CalculatedFreeProducts.Any(a => a.Quantity > 0)) {
                                            EffectOnRows = "کل فاکتور";
                                        }
                                    }
                                    else
                                        break;
                                    CalculatedCoefficient = CalculatedCoefficient + ThisEnumerationCalculatedCoefficient * (1 - CalculatedCoefficient);
                                }
                                #endregion
                                break;
                            case DiscountWay.CashSettlementPercent:
                                break;
                            default:
                                break;
                        }
                    }

                    Order.DiscountPercent += CalculatedDiscountPercent;
                    Order.CashPrise += CalculatedCashPrise;
                    Order.FreeProducts = AddToFreeProducts(Order.FreeProducts, CalculatedFreeProducts);
                    if (CalculatedDiscountPercent != 0 || CalculatedCashPrise != 0 || CalculatedFreeProducts.Any(a => a.Quantity != 0))
                    {
                        Order.AggregateOtherDiscounts_Percent = DiscountRule.AggregateOtherDiscounts_Percent;
                        Order.AggregateOtherDiscounts_SpecificMoney = DiscountRule.AggregateOtherDiscounts_SpecificMoney;
                        Order.AggregateOtherDiscounts_FreeProduct = DiscountRule.AggregateOtherDiscounts_FreeProduct;
                        Order.AggregateOtherDiscounts_CashSettlement = DiscountRule.AggregateOtherDiscounts_CashSettlement;
                    }
                    #endregion
                    break;
                case DiscountBasedOn.TotalPriceSum:
                    #region
                    if (CheckRuleConditions(DiscountRule, Order, null))
                    {
                        CalculatedDiscountPercent = 0;
                        CalculatedCashPrise = 0;
                        CalculatedFreeProducts = new List<FreeProductModel>();
                        CalculatedCashSettlementDiscounts = new List<CashSettlementDiscountModel>();

                        PriceSum = 0;
                        decimal UnusedPriceSum = 0;
                        for (var j = 0; j < Order.Articles.Length; j++)
                        {
                            if ((DiscountRule.Way == DiscountWay.Percent && Order.Articles[j].AggregateOtherDiscounts_Percent) ||
                                    (DiscountRule.Way == DiscountWay.SpecificMoney && Order.Articles[j].AggregateOtherDiscounts_SpecificMoney) ||
                                    (DiscountRule.Way == DiscountWay.FreeProduct && Order.Articles[j].AggregateOtherDiscounts_FreeProduct) ||
                                    (DiscountRule.Way == DiscountWay.CashSettlementPercent && Order.Articles[j].AggregateOtherDiscounts_CashSettlement))
                                PriceSum += Order.Articles[j].FinalPrice;
                            else
                                UnusedPriceSum += Order.Articles[j].FinalPrice;
                        }

                        switch (DiscountRule.Way)
                        {
                            case DiscountWay.Percent:
                                #region
                                if (Order.AggregateOtherDiscounts_Percent)
                                {
                                    for (var i = 0; i < DiscountRule.Steps.Length; i++)
                                    {
                                        if (PriceSum >= DiscountRule.Steps[i].Price)
                                        {
                                            CalculatedDiscountPercent = (PriceSum + UnusedPriceSum) != 0 ? DiscountRule.Steps[i].Percent * PriceSum / (PriceSum + UnusedPriceSum) : 0;
                                            if (CalculatedDiscountPercent > 0)
                                                EffectOnRows = "کل فاکتور";
                                        }
                                    }
                                }
                                #endregion
                                break;
                            case DiscountWay.SpecificMoney:
                                #region
                                if (Order.AggregateOtherDiscounts_SpecificMoney)
                                {
                                    CalculatedCoefficient = 0;
                                    while (CalculatedCoefficient < 1)
                                    {
                                        decimal ThisPriceSum = PriceSum;
                                        decimal ThisEnumerationCalculatedCashPrise = 0;
                                        decimal ThisEnumerationCalculatedCoefficient = 0;
                                        for (var i = 0; i < DiscountRule.Steps.Length; i++)
                                        {
                                            ThisPriceSum = Math.Round(ThisPriceSum * (1 - CalculatedCoefficient) * 100000) / 100000;
                                            if (ThisPriceSum >= DiscountRule.Steps[i].Price)
                                            {
                                                if (DiscountRule.Steps[i].AscendingCalculation)
                                                {
                                                    var RealValue = ThisPriceSum / DiscountRule.Steps[i].Price;
                                                    var IntValue = Math.Floor(ThisPriceSum / DiscountRule.Steps[i].Price);
                                                    ThisEnumerationCalculatedCashPrise = DiscountRule.Steps[i].SpecificMoney * IntValue;
                                                    ThisEnumerationCalculatedCoefficient = IntValue / RealValue;
                                                }
                                                else
                                                {
                                                    ThisEnumerationCalculatedCashPrise = DiscountRule.Steps[i].SpecificMoney;
                                                    ThisEnumerationCalculatedCoefficient = 1;
                                                }
                                            }
                                        }
                                        if (ThisEnumerationCalculatedCashPrise != 0)
                                        {
                                            CalculatedCashPrise += ThisEnumerationCalculatedCashPrise;
                                            if (CalculatedCashPrise > 0)
                                                EffectOnRows = "کل فاکتور";
                                        }
                                        else
                                            break;
                                        CalculatedCoefficient = CalculatedCoefficient + ThisEnumerationCalculatedCoefficient * (1 - CalculatedCoefficient);
                                    }
                                }
                                #endregion
                                break;
                            case DiscountWay.FreeProduct:
                                #region
                                if (Order.AggregateOtherDiscounts_FreeProduct)
                                {
                                    CalculatedCoefficient = 0;
                                    while (CalculatedCoefficient < 1)
                                    {
                                        decimal ThisPriceSum = PriceSum;
                                        FreeProductModel ThisEnumerationCalculatedFreeProduct = null;
                                        decimal ThisEnumerationCalculatedCoefficient = 0;
                                        for (var i = 0; i < DiscountRule.Steps.Length; i++)
                                        {
                                            ThisPriceSum = Math.Round(ThisPriceSum * (1 - CalculatedCoefficient) * 100000) / 100000;
                                            if (ThisPriceSum >= DiscountRule.Steps[i].Price)
                                            {
                                                var Package = GetStuffPackage(DiscountRule.Steps[i].SpecificStuffId.Value, DiscountRule.Steps[i].UnitId);
                                                if (Package != null)
                                                {
                                                    ThisEnumerationCalculatedFreeProduct = new FreeProductModel()
                                                    {
                                                        Stuff = AllSystemStuffs[DiscountRule.Steps[i].SpecificStuffId.Value],
                                                        BatchNumber = null,
                                                        Package = Package,
                                                        Quantity = DiscountRule.Steps[i].FreeProductQuantity
                                                    };
                                                
                                                    if (DiscountRule.Steps[i].ContinueToCalculateOnThisRatio)
                                                    {
                                                        var RealValue = ThisPriceSum / DiscountRule.Steps[i].Price;
                                                        var ShouldBeQuantityRounded = ThisEnumerationCalculatedFreeProduct.Quantity * RealValue;
                                                        if (DiscountRule.Steps[i].RatioCalculationMethod == RatioCalculationMethod.Round)
                                                            ShouldBeQuantityRounded = Math.Floor(ShouldBeQuantityRounded);
                                                        else
                                                            ShouldBeQuantityRounded = Math.Round(ShouldBeQuantityRounded);
                                                
                                                        ThisEnumerationCalculatedFreeProduct.Quantity = ShouldBeQuantityRounded;
                                                        ThisEnumerationCalculatedCoefficient = 1;
                                                    }
                                                    else {
                                                        if (DiscountRule.Steps[i].AscendingCalculation)
                                                        {
                                                            var RealValue = ThisPriceSum / DiscountRule.Steps[i].Price;
                                                            var IntValue = Math.Floor(ThisPriceSum / DiscountRule.Steps[i].Price);
                                                            ThisEnumerationCalculatedFreeProduct.Quantity *= IntValue;
                                                            ThisEnumerationCalculatedCoefficient = IntValue / RealValue;
                                                        }
                                                        else {
                                                            ThisEnumerationCalculatedCoefficient = 1;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        if (ThisEnumerationCalculatedFreeProduct != null)
                                        {
                                            CalculatedFreeProducts = AddToFreeProducts(CalculatedFreeProducts, ThisEnumerationCalculatedFreeProduct);
                                            if(CalculatedFreeProducts.Any(a => a.Quantity != 0))
                                                EffectOnRows = "کل فاکتور";
                                        }
                                        else
                                            break;
                                        CalculatedCoefficient = CalculatedCoefficient + ThisEnumerationCalculatedCoefficient * (1 - CalculatedCoefficient);
                                    }
                                }
                                #endregion
                                break;
                            case DiscountWay.CashSettlementPercent:
                                #region
                                if (Order.AggregateOtherDiscounts_CashSettlement)
                                {
                                    for (var i = 0; i < DiscountRule.Steps.Length; i++)
                                    {
                                        if (PriceSum >= DiscountRule.Steps[i].Price)
                                        {
                                            var ThisDayCurrentPercent = CalculatedCashSettlementDiscounts.SingleOrDefault(a => a.Day == DiscountRule.Steps[i].CashSettlementDay);
                                            if (ThisDayCurrentPercent == null)
                                                CalculatedCashSettlementDiscounts.Add(new CashSettlementDiscountModel()
                                                {
                                                    Day = DiscountRule.Steps[i].CashSettlementDay,
                                                    Percent = (PriceSum + UnusedPriceSum) != 0 ? DiscountRule.Steps[i].Percent * PriceSum / (PriceSum + UnusedPriceSum) : 0
                                                });
                                            else if (ThisDayCurrentPercent.Percent < DiscountRule.Steps[i].Percent)
                                                ThisDayCurrentPercent.Percent = (PriceSum + UnusedPriceSum) != 0 ? DiscountRule.Steps[i].Percent * PriceSum / (PriceSum + UnusedPriceSum) : 0;
                                        }
                                    }
                                }
                                #endregion
                                break;
                            default:
                                break;
                        }
                        
                        Order.DiscountPercent += CalculatedDiscountPercent;
                        Order.CashPrise += CalculatedCashPrise;
                        Order.FreeProducts = AddToFreeProducts(Order.FreeProducts, CalculatedFreeProducts);
                        
                        if (CalculatedCashSettlementDiscounts != null)
                            Order.CashSettlementDiscounts = CalculatedCashSettlementDiscounts;
                        if (CalculatedDiscountPercent != 0 || CalculatedCashPrise != 0 || CalculatedFreeProducts.Any(a => a.Quantity != 0) || CalculatedCashSettlementDiscounts.Any(a => a.Percent != 0))
                        {
                            Order.AggregateOtherDiscounts_Percent = DiscountRule.AggregateOtherDiscounts_Percent;
                            Order.AggregateOtherDiscounts_SpecificMoney = DiscountRule.AggregateOtherDiscounts_SpecificMoney;
                            Order.AggregateOtherDiscounts_FreeProduct = DiscountRule.AggregateOtherDiscounts_FreeProduct;
                            Order.AggregateOtherDiscounts_CashSettlement = DiscountRule.AggregateOtherDiscounts_CashSettlement;
                        }
                    }
                    #endregion
                    break;
                case DiscountBasedOn.QuantityOfAnArticle:
                    #region
                    var RowNumbers = "";
                    var ThisRuleArticles = new List<ArticleModel>();
                    for (var i = 0; i < Order.Articles.Length; i++)
                    {
                        if ((DiscountRule.Way == DiscountWay.Percent && Order.Articles[i].AggregateOtherDiscounts_Percent) ||
                                (DiscountRule.Way == DiscountWay.SpecificMoney && Order.Articles[i].AggregateOtherDiscounts_SpecificMoney) ||
                                (DiscountRule.Way == DiscountWay.FreeProduct && Order.Articles[i].AggregateOtherDiscounts_FreeProduct) ||
                                (DiscountRule.Way == DiscountWay.CashSettlementPercent && Order.Articles[i].AggregateOtherDiscounts_CashSettlement))
                        {
                            if (CheckRuleConditions(DiscountRule, Order, i))
                            {
                                ThisRuleArticles.Add(Order.Articles[i]);
                                RowNumbers = (RowNumbers == "" ? "" : "، ") + i;
                            }
                        }
                    }
                    var TriedToGetDiscount = false;
                    CalculatedDiscountPercent = 0;
                    CalculatedCashPrise = 0;
                    CalculatedFreeProducts = new List<FreeProductModel>();

                    switch (DiscountRule.Way)
                    {
                        case DiscountWay.Percent:
                            #region
                            for (var i = 0; i < DiscountRule.Steps.Length; i++) {
                                decimal QuantitySum = 0;
                                foreach (var ThisRuleArticle in ThisRuleArticles)
                                    for (var sui = 0; sui < DiscountRule.Steps[i].SpecificUnitIds.Length; sui++)
                                        QuantitySum += (!DiscountRule.Steps[i].SpecificUnitIds[sui].HasValue ? ThisRuleArticle.StuffQuantity : ArticleQuantityInAUnit(ThisRuleArticle, DiscountRule.Steps[i].SpecificUnitIds[sui].Value));

                                if (QuantitySum >= DiscountRule.Steps[i].Quantity)
                                {
                                    CalculatedDiscountPercent = DiscountRule.Steps[i].Percent;
                                    if (DiscountRule.ShowDiscountInUnitFee)
                                        foreach (var ThisRuleArticle in ThisRuleArticles)
                                            ThisRuleArticle.ChangePercentToFeeChange = true;

                                    if (CalculatedDiscountPercent != 0 || SystemName == "Saman") {
                                        EffectOnRows = "سطرهای " + RowNumbers;
                                        TriedToGetDiscount = true;
                                    }
                                }
                            }
                            #endregion
                            break;
                        case DiscountWay.SpecificMoney:
                            #region
                            CalculatedCoefficient = 0;
                            while (CalculatedCoefficient < 1) {
                                decimal ThisEnumerationCalculatedCashPrise = 0;
                                decimal ThisEnumerationCalculatedCoefficient = 0;
                                for (var i = 0; i < DiscountRule.Steps.Length; i++) {
                                    decimal QuantitySum = 0;
                                    foreach (var ThisRuleArticle in ThisRuleArticles)
                                        for (var sui = 0; sui < DiscountRule.Steps[i].SpecificUnitIds.Length; sui++)
                                            QuantitySum += (!DiscountRule.Steps[i].SpecificUnitIds[sui].HasValue ? ThisRuleArticle.StuffQuantity : ArticleQuantityInAUnit(ThisRuleArticle, DiscountRule.Steps[i].SpecificUnitIds[sui].Value));

                                    QuantitySum = Math.Round(QuantitySum * (1 - CalculatedCoefficient) * 100000) / 100000;
                                    if (QuantitySum >= DiscountRule.Steps[i].Quantity) {
                                        if (DiscountRule.Steps[i].AscendingCalculation) {
                                            var RealValue = QuantitySum / DiscountRule.Steps[i].Quantity;
                                            var IntValue = Math.Floor(QuantitySum / DiscountRule.Steps[i].Quantity);
                                            ThisEnumerationCalculatedCashPrise = DiscountRule.Steps[i].SpecificMoney;
                                            ThisEnumerationCalculatedCoefficient = IntValue / RealValue;
                                        }
                                        else {
                                            ThisEnumerationCalculatedCashPrise = DiscountRule.Steps[i].SpecificMoney;
                                            ThisEnumerationCalculatedCoefficient = 1;
                                        }
                                    }
                                }
                                if (ThisEnumerationCalculatedCashPrise != 0 || SystemName == "Saman") {
                                    CalculatedCashPrise += ThisEnumerationCalculatedCashPrise;
                                    if (CalculatedCashPrise != 0 || SystemName == "Saman") {
                                        EffectOnRows = "سطرهای " + RowNumbers;
                                        TriedToGetDiscount = true;
                                    }
                                    if (SystemName == "Saman")
                                        CalculatedCoefficient = 1;
                                }
                                else
                                    break;
                                CalculatedCoefficient = CalculatedCoefficient + ThisEnumerationCalculatedCoefficient * (1 - CalculatedCoefficient);
                            }
                            #endregion
                            break;
                        case DiscountWay.FreeProduct:
                            #region
                            foreach (var ThisRuleArticle in ThisRuleArticles)
                                ThisRuleArticle.StuffCalculatedCoefficient = 0;
                            CalculatedCoefficient = 0;
                            var TotalFreeProducts = new List<FreeProductModel>();

                            while (CalculatedCoefficient < 1)
                            {
                                FreeProductModel ThisEnumerationCalculatedFreeProduct = null;
                                decimal ThisEnumerationCalculatedCoefficient = 0;
                                for (var i = 0; i < DiscountRule.Steps.Length; i++)
                                {
                                    decimal QuantitySum = 0;
                                    foreach (var ThisRuleArticle in ThisRuleArticles)
                                    {
                                        decimal ThisStuffQuantity = 0;
                                        for (var sui = 0; sui < DiscountRule.Steps[i].SpecificUnitIds.Length; sui++)
                                            ThisStuffQuantity += (!DiscountRule.Steps[i].SpecificUnitIds[sui].HasValue ? ThisRuleArticle.StuffQuantity : ArticleQuantityInAUnit(ThisRuleArticle, DiscountRule.Steps[i].SpecificUnitIds[sui].Value)) * (1 - ThisRuleArticle.StuffCalculatedCoefficient);
                                        QuantitySum += ThisStuffQuantity;
                                        ThisRuleArticle.SpecialStuffQuantity = ThisStuffQuantity;
                                    }
                                    QuantitySum = Math.Round(QuantitySum * (1 - CalculatedCoefficient) * 100000) / 100000;
                                    if (QuantitySum >= DiscountRule.Steps[i].Quantity)
                                    {
                                        FreeProductModel Stuff = null;
                                        if (DiscountRule.Steps[i].SpecificStuffId.HasValue)
                                            Stuff = new FreeProductModel()
                                            {
                                                Stuff = AllSystemStuffs[DiscountRule.Steps[i].SpecificStuffId.Value],
                                                BatchNumber = null
                                            };
                                        else
                                        {
                                            ArticleModel TheMostCompetentArticle = null;
                                            decimal TheMostRemainingStuffQuantity = 0;
                                            foreach (var ThisRuleArticle in ThisRuleArticles)
                                                if ((1 - ThisRuleArticle.StuffCalculatedCoefficient) * ThisRuleArticle.SpecialStuffQuantity > TheMostRemainingStuffQuantity)
                                                {
                                                    TheMostCompetentArticle = ThisRuleArticle;
                                                    TheMostRemainingStuffQuantity = (1 - ThisRuleArticle.StuffCalculatedCoefficient) * ThisRuleArticle.SpecialStuffQuantity;
                                                }

                                            Stuff = new FreeProductModel()
                                            {
                                                Stuff = TheMostCompetentArticle.Stuff,
                                                BatchNumber = TheMostCompetentArticle.BatchNumber
                                            };
                                        }

                                        var Package = GetStuffPackage(Stuff.Stuff.Id, DiscountRule.Steps[i].UnitId);
                                        if (Package != null)
                                        {
                                            ThisEnumerationCalculatedFreeProduct = Stuff;
                                            ThisEnumerationCalculatedFreeProduct.Package = Package;
                                            ThisEnumerationCalculatedFreeProduct.Quantity = DiscountRule.Steps[i].FreeProductQuantity;

                                            if (DiscountRule.Steps[i].ContinueToCalculateOnThisRatio)
                                            {
                                                var RealValue = QuantitySum / DiscountRule.Steps[i].Quantity;
                                                var ShouldBeQuantityRounded = ThisEnumerationCalculatedFreeProduct.Quantity * RealValue;
                                                if (DiscountRule.Steps[i].RatioCalculationMethod == RatioCalculationMethod.Round)
                                                    ShouldBeQuantityRounded = Math.Floor(ShouldBeQuantityRounded);
                                                else
                                                    ShouldBeQuantityRounded = Math.Round(ShouldBeQuantityRounded);

                                                ThisEnumerationCalculatedFreeProduct.Quantity = ShouldBeQuantityRounded;
                                                ThisEnumerationCalculatedCoefficient = 1;
                                            }
                                            else {
                                                if (DiscountRule.Steps[i].AscendingCalculation)
                                                {
                                                    var RealValue = QuantitySum / DiscountRule.Steps[i].Quantity;
                                                    var IntValue = (int)Math.Floor(QuantitySum / DiscountRule.Steps[i].Quantity);
                                                    ThisEnumerationCalculatedFreeProduct.Quantity *= IntValue;
                                                    ThisEnumerationCalculatedCoefficient = IntValue / RealValue;
                                                }
                                                else
                                                    ThisEnumerationCalculatedCoefficient = 1;
                                            }
                                        }
                                    }
                                }
                                if (ThisEnumerationCalculatedFreeProduct != null)
                                    TotalFreeProducts = AddToFreeProducts(TotalFreeProducts, ThisEnumerationCalculatedFreeProduct);
                                else
                                    break;
                                CalculatedCoefficient = CalculatedCoefficient + ThisEnumerationCalculatedCoefficient * (1 - CalculatedCoefficient);
                            }
                            var TotalCalculatedCoefficient = CalculatedCoefficient;

                            var EachStuffsFreeProducts = new List<FreeProductModel>();
                            foreach (var ThisRuleArticle in ThisRuleArticles)
                            {
                                CalculatedCoefficient = 0;
                                while (CalculatedCoefficient < 1)
                                {
                                    FreeProductModel ThisEnumerationCalculatedFreeProduct = null;
                                    decimal ThisEnumerationCalculatedCoefficient = 0;
                                    for (var i = 0; i < DiscountRule.Steps.Length; i++)
                                    {
                                        decimal QuantitySum = 0;
                                        for (var sui = 0; sui < DiscountRule.Steps[i].SpecificUnitIds.Length; sui++)
                                            QuantitySum += (!DiscountRule.Steps[i].SpecificUnitIds[sui].HasValue ? ThisRuleArticle.StuffQuantity : ArticleQuantityInAUnit(ThisRuleArticle, DiscountRule.Steps[i].SpecificUnitIds[sui].Value));
                                        
                                        QuantitySum = Math.Round(QuantitySum * (1 - CalculatedCoefficient) * (1 - TotalCalculatedCoefficient) * 100000) / 100000;
                                        if (QuantitySum >= DiscountRule.Steps[i].Quantity)
                                        {
                                            FreeProductModel Stuff = DiscountRule.Steps[i].SpecificStuffId.HasValue ? new FreeProductModel()
                                            {
                                                Stuff = AllSystemStuffs[DiscountRule.Steps[i].SpecificStuffId.Value],
                                                BatchNumber = null
                                            } : new FreeProductModel()
                                            {
                                                Stuff = ThisRuleArticle.Stuff,
                                                BatchNumber = ThisRuleArticle.BatchNumber
                                            };
                                            var Package = GetStuffPackage(Stuff.Stuff.Id, DiscountRule.Steps[i].UnitId);
                                            if (Package != null)
                                            {
                                                ThisEnumerationCalculatedFreeProduct = Stuff;
                                                ThisEnumerationCalculatedFreeProduct.Package = Package;
                                                ThisEnumerationCalculatedFreeProduct.Quantity = DiscountRule.Steps[i].FreeProductQuantity;
                                                
                                                if (DiscountRule.Steps[i].ContinueToCalculateOnThisRatio)
                                                {
                                                    var RealValue = QuantitySum / DiscountRule.Steps[i].Quantity;
                                                    var ShouldBeQuantityRounded = ThisEnumerationCalculatedFreeProduct.Quantity * RealValue;
                                                    if (DiscountRule.Steps[i].RatioCalculationMethod == RatioCalculationMethod.Round)
                                                        ShouldBeQuantityRounded = Math.Floor(ShouldBeQuantityRounded);
                                                    else
                                                        ShouldBeQuantityRounded = Math.Round(ShouldBeQuantityRounded);
                                                
                                                    ThisEnumerationCalculatedFreeProduct.Quantity = ShouldBeQuantityRounded;
                                                    ThisEnumerationCalculatedCoefficient = 1;
                                                }
                                                else
                                                {
                                                    if (DiscountRule.Steps[i].AscendingCalculation)
                                                    {
                                                        var RealValue = QuantitySum / DiscountRule.Steps[i].Quantity;
                                                        var IntValue = Math.Floor(QuantitySum / DiscountRule.Steps[i].Quantity);
                                                        ThisEnumerationCalculatedFreeProduct.Quantity *= IntValue;
                                                        ThisEnumerationCalculatedCoefficient = IntValue / RealValue;
                                                    }
                                                    else
                                                        ThisEnumerationCalculatedCoefficient = 1;
                                                }
                                            }
                                        }
                                    }
                                    if (ThisEnumerationCalculatedFreeProduct != null)
                                        EachStuffsFreeProducts = AddToFreeProducts(EachStuffsFreeProducts, ThisEnumerationCalculatedFreeProduct);
                                    else
                                        break;
                                    CalculatedCoefficient = CalculatedCoefficient + ThisEnumerationCalculatedCoefficient * (1 - CalculatedCoefficient);
                                }
                                ThisRuleArticle.StuffCalculatedCoefficient = CalculatedCoefficient;
                            }

                            CalculatedFreeProducts = AddToFreeProducts(CalculatedFreeProducts, TotalFreeProducts);
                            CalculatedFreeProducts = AddToFreeProducts(CalculatedFreeProducts, EachStuffsFreeProducts);

                            foreach (var item in CalculatedFreeProducts)
                                if (item.Quantity != 0 || SystemName == "Saman") {
                                    EffectOnRows = "سطرهای " + RowNumbers;
                                    TriedToGetDiscount = true;
                                }
                            #endregion
                            break;
                        case DiscountWay.CashSettlementPercent:
                            #region
                            #endregion
                            break;
                        default:
                            break;
                    }
                    foreach (var ThisRuleArticle in ThisRuleArticles)
                    {
                        if(RowCashDiscountIsSeperate)
                            ThisRuleArticle.CashDiscountPercent += CalculatedDiscountPercent;
                        else
                            ThisRuleArticle.DiscountPercent += CalculatedDiscountPercent;
                        ThisRuleArticle.FeeChange = -CalculatedCashPrise;
                        
                        if (CalculatedDiscountPercent != 0 || CalculatedCashPrise != 0 || CalculatedFreeProducts.Any(a => a.Quantity != 0) || (SystemName == "Saman" && TriedToGetDiscount)) {
                            ThisRuleArticle.AggregateOtherDiscounts_Percent = DiscountRule.AggregateOtherDiscounts_Percent;
                            ThisRuleArticle.AggregateOtherDiscounts_SpecificMoney = DiscountRule.AggregateOtherDiscounts_SpecificMoney;
                            ThisRuleArticle.AggregateOtherDiscounts_FreeProduct = DiscountRule.AggregateOtherDiscounts_FreeProduct;
                            ThisRuleArticle.AggregateOtherDiscounts_CashSettlement = DiscountRule.AggregateOtherDiscounts_CashSettlement;
                        }
                    }
                    Order.FreeProducts = AddToFreeProducts(Order.FreeProducts, CalculatedFreeProducts);
                    #endregion
                    break;
                case DiscountBasedOn.PriceOfAnArticle:
                    #region
                    ThisRuleArticles = new List<ArticleModel>();
                    RowNumbers = "";
                    for (var i = 0; i < Order.Articles.Length; i++)
                    {
                        if ((DiscountRule.Way == DiscountWay.Percent && Order.Articles[i].AggregateOtherDiscounts_Percent) ||
                                (DiscountRule.Way == DiscountWay.SpecificMoney && Order.Articles[i].AggregateOtherDiscounts_SpecificMoney) ||
                                (DiscountRule.Way == DiscountWay.FreeProduct && Order.Articles[i].AggregateOtherDiscounts_FreeProduct) ||
                                (DiscountRule.Way == DiscountWay.CashSettlementPercent && Order.Articles[i].AggregateOtherDiscounts_CashSettlement))
                            if (CheckRuleConditions(DiscountRule, Order, i))
                            {
                                ThisRuleArticles.Add(Order.Articles[i]);
                                RowNumbers = (RowNumbers == "" ? "" : "، ") + i;
                            }
                    }
                    CalculatedDiscountPercent = 0;
                    CalculatedCashPrise = 0;
                    CalculatedFreeProducts = new List<FreeProductModel>();

                    PriceSum = ThisRuleArticles.Any() ? ThisRuleArticles.Sum(a => a.Price) : 0;

                    switch (DiscountRule.Way)
                    {
                        case DiscountWay.Percent:
                            #region
                            for (var i = 0; i < DiscountRule.Steps.Length; i++)
                                if (PriceSum >= DiscountRule.Steps[i].Price) {
                                    CalculatedDiscountPercent = DiscountRule.Steps[i].Percent;
                                    if (CalculatedDiscountPercent != 0)
                                        EffectOnRows = "سطرهای " + RowNumbers;
                                }
                            #endregion
                            break;
                        case DiscountWay.SpecificMoney:
                            #region
                            CalculatedCoefficient = 0;
                            while (CalculatedCoefficient < 1)
                            {
                                decimal ThisPriceSum = PriceSum;
                                decimal ThisEnumerationCalculatedCashPrise = 0;
                                decimal ThisEnumerationCalculatedCoefficient = 0;
                                for (var i = 0; i < DiscountRule.Steps.Length; i++) {
                                    ThisPriceSum = Math.Round(ThisPriceSum * (1 - CalculatedCoefficient) * 100000) / 100000;
                                    if (ThisPriceSum >= DiscountRule.Steps[i].Price) {
                                        if (DiscountRule.Steps[i].AscendingCalculation) {
                                            var RealValue = ThisPriceSum / DiscountRule.Steps[i].Price;
                                            var IntValue = Math.Floor(ThisPriceSum / DiscountRule.Steps[i].Price);
                                            ThisEnumerationCalculatedCashPrise = DiscountRule.Steps[i].SpecificMoney; // * IntValue;
                                            ThisEnumerationCalculatedCoefficient = IntValue / RealValue;
                                        }
                                        else {
                                            ThisEnumerationCalculatedCashPrise = DiscountRule.Steps[i].SpecificMoney;
                                            ThisEnumerationCalculatedCoefficient = 1;
                                        }
                                    }
                                }
                                if (ThisEnumerationCalculatedCashPrise != 0) {
                                    CalculatedCashPrise += ThisEnumerationCalculatedCashPrise;
                                    if (CalculatedCashPrise != 0)
                                        EffectOnRows = "سطرهای " + RowNumbers;
                                }
                                else
                                    break;
                                CalculatedCoefficient = CalculatedCoefficient + ThisEnumerationCalculatedCoefficient * (1 - CalculatedCoefficient);
                            }
                            #endregion
                            break;
                        case DiscountWay.FreeProduct:
                            #region
                            CalculatedCoefficient = 0;
                            while (CalculatedCoefficient < 1)
                            {
                                //var ThisPriceSum = PriceSum;
                                decimal ThisPriceSum = Math.Round(PriceSum * (1 - CalculatedCoefficient) * 100000) / 100000;
                                FreeProductModel ThisEnumerationCalculatedFreeProduct = null;
                                decimal ThisEnumerationCalculatedCoefficient = 0;
                                for (var i = 0; i < DiscountRule.Steps.Length; i++) {
                                    //ThisPriceSum = Math.Round(ThisPriceSum * (1 - CalculatedCoefficient) * 100000) / 100000;
                                    if (ThisPriceSum >= DiscountRule.Steps[i].Price) {
                                        var Stuff = DiscountRule.Steps[i].SpecificStuffId.HasValue ? new FreeProductModel()
                                        {
                                            Stuff = AllSystemStuffs[DiscountRule.Steps[i].SpecificStuffId.Value],
                                            BatchNumber = null
                                        } : new FreeProductModel()
                                        {
                                            Stuff = ThisRuleArticles[0].Stuff,
                                            BatchNumber = ThisRuleArticles[0].BatchNumber
                                        };
                                        var Package = GetStuffPackage(Stuff.Stuff.Id, DiscountRule.Steps[i].UnitId);
                                        if (Package != null) {
                                            ThisEnumerationCalculatedFreeProduct = Stuff;
                                            ThisEnumerationCalculatedFreeProduct.Package = Package;
                                            ThisEnumerationCalculatedFreeProduct.Quantity = DiscountRule.Steps[i].FreeProductQuantity;
                                            
                                            if (DiscountRule.Steps[i].ContinueToCalculateOnThisRatio)
                                            {
                                                var RealValue = ThisPriceSum / DiscountRule.Steps[i].Price;
                                                var ShouldBeQuantityRounded = ThisEnumerationCalculatedFreeProduct.Quantity * RealValue;
                                                if (DiscountRule.Steps[i].RatioCalculationMethod == RatioCalculationMethod.Round)
                                                    ShouldBeQuantityRounded = Math.Floor(ShouldBeQuantityRounded);
                                                else
                                                    ShouldBeQuantityRounded = Math.Round(ShouldBeQuantityRounded);
                            
                                                ThisEnumerationCalculatedFreeProduct.Quantity = ShouldBeQuantityRounded;
                                                ThisEnumerationCalculatedCoefficient = 1;
                                            }
                                            else {
                                                if (DiscountRule.Steps[i].AscendingCalculation) {
                                                    var RealValue = ThisPriceSum / DiscountRule.Steps[i].Price;
                                                    var IntValue = Math.Floor(ThisPriceSum / DiscountRule.Steps[i].Price);
                                                    ThisEnumerationCalculatedFreeProduct.Quantity *= IntValue;
                                                    ThisEnumerationCalculatedCoefficient = IntValue / RealValue;
                                                }
                                                else
                                                    ThisEnumerationCalculatedCoefficient = 1;
                                            }
                                        }
                                    }
                                }
                                if (ThisEnumerationCalculatedFreeProduct != null) {
                                    CalculatedFreeProducts = AddToFreeProducts(CalculatedFreeProducts, ThisEnumerationCalculatedFreeProduct);
                                    if (CalculatedFreeProducts.Any(a => a.Quantity != 0))
                                        EffectOnRows = "سطرهای " + RowNumbers;
                                }
                                else
                                    break;
                                CalculatedCoefficient = CalculatedCoefficient + ThisEnumerationCalculatedCoefficient * (1 - CalculatedCoefficient);
                            }
                            #endregion
                            break;
                        case DiscountWay.CashSettlementPercent:
                            #region
                            #endregion
                            break;
                        default:
                            break;
                    }

                    foreach (var ThisRuleArticle in ThisRuleArticles)
                    {
                        if (RowCashDiscountIsSeperate)
                            ThisRuleArticle.CashDiscountPercent += CalculatedDiscountPercent;
                        else
                            ThisRuleArticle.DiscountPercent += CalculatedDiscountPercent;
                        //ThisRuleArticle.CashPrise += CalculatedCashPrise;
                        ThisRuleArticle.FeeChange = -CalculatedCashPrise;
                    }
                    Order.FreeProducts = AddToFreeProducts(Order.FreeProducts, CalculatedFreeProducts);

                    if (CalculatedDiscountPercent != 0 || CalculatedCashPrise != 0 || CalculatedFreeProducts.Any(a => a.Quantity != 0))
                    {
                        Order.AggregateOtherDiscounts_Percent = DiscountRule.AggregateOtherDiscounts_Percent;
                        Order.AggregateOtherDiscounts_SpecificMoney = DiscountRule.AggregateOtherDiscounts_SpecificMoney;
                        Order.AggregateOtherDiscounts_FreeProduct = DiscountRule.AggregateOtherDiscounts_FreeProduct;
                        Order.AggregateOtherDiscounts_CashSettlement = DiscountRule.AggregateOtherDiscounts_CashSettlement;
                        foreach (var ThisRuleArticle in ThisRuleArticles)
                        {
                            ThisRuleArticle.AggregateOtherDiscounts_Percent = DiscountRule.AggregateOtherDiscounts_Percent;
                            ThisRuleArticle.AggregateOtherDiscounts_SpecificMoney = DiscountRule.AggregateOtherDiscounts_SpecificMoney;
                            ThisRuleArticle.AggregateOtherDiscounts_FreeProduct = DiscountRule.AggregateOtherDiscounts_FreeProduct;
                            ThisRuleArticle.AggregateOtherDiscounts_CashSettlement = DiscountRule.AggregateOtherDiscounts_CashSettlement;
                        }
                    }
                    #endregion
                    break;
                case DiscountBasedOn.CountOfArticles:
                    #region
                    RowNumbers = "";
                    ThisRuleArticles = new List<ArticleModel>();
                    for (var i = 0; i < Order.Articles.Length; i++)
                        if ((DiscountRule.Way == DiscountWay.Percent && Order.Articles[i].AggregateOtherDiscounts_Percent) ||
                                (DiscountRule.Way == DiscountWay.SpecificMoney && Order.Articles[i].AggregateOtherDiscounts_SpecificMoney) ||
                                (DiscountRule.Way == DiscountWay.FreeProduct && Order.Articles[i].AggregateOtherDiscounts_FreeProduct) ||
                                (DiscountRule.Way == DiscountWay.CashSettlementPercent && Order.Articles[i].AggregateOtherDiscounts_CashSettlement)) {
                            if (CheckRuleConditions(DiscountRule, Order, i))
                                ThisRuleArticles.Add(Order.Articles[i]);
                                RowNumbers = (RowNumbers == "" ? "" : "، ") + i;
                            }
                    
                    TriedToGetDiscount = false;
                    CalculatedDiscountPercent = 0;
                    CalculatedCashPrise = 0;
                    CalculatedFreeProducts = new List<FreeProductModel>();

                    switch (DiscountRule.Way)
                    {
                        case DiscountWay.Percent:
                            #region
                            for (var i = 0; i < DiscountRule.Steps.Length; i++)
                            {
                                RowCount = ThisRuleArticles.Count;
                                if (RowCount >= DiscountRule.Steps[i].RowCount)
                                {
                                    CalculatedDiscountPercent = DiscountRule.Steps[i].Percent;
                                    if (DiscountRule.ShowDiscountInUnitFee)
                                        foreach (var ThisRuleArticle in ThisRuleArticles)
                                            ThisRuleArticle.ChangePercentToFeeChange = true;
                                        
                                    if (CalculatedDiscountPercent != 0 || SystemName == "Saman")
                                    {
                                        EffectOnRows = "سطرهای " + RowNumbers;
                                        TriedToGetDiscount = true;
                                    }
                                }
                            }
                            #endregion
                            break;
                        case DiscountWay.SpecificMoney:
                            #region
                            RowCount = ThisRuleArticles.Count;
                            for (var i = 0; i < DiscountRule.Steps.Length; i++)
                            {
                                if (RowCount >= DiscountRule.Steps[i].RowCount)
                                {
                                    CalculatedCashPrise = DiscountRule.Steps[i].SpecificMoney;
                                    EffectOnRows = "سطرهای " + RowNumbers;
                                    TriedToGetDiscount = true;
                                }
                            }
                            #endregion
                            break;
                        case DiscountWay.FreeProduct:
                            #region
                            RowCount = ThisRuleArticles.Count;
                            for (var i = 0; i < DiscountRule.Steps.Length; i++)
                            {
                                if (RowCount >= DiscountRule.Steps[i].RowCount)
                                {
                                    FreeProductModel Stuff = null;
                                    if (DiscountRule.Steps[i].SpecificStuffId.HasValue)
                                        Stuff = new FreeProductModel()
                                        {
                                            Stuff = AllSystemStuffs[DiscountRule.Steps[i].SpecificStuffId.Value],
                                            BatchNumber = null
                                        };
                                    else
                                    {
                                        ArticleModel TheMostCompetentArticle = null;
                                        decimal TheMostRemainingStuffQuantity = 0;
                                        foreach (var ThisRuleArticle in ThisRuleArticles)
                                            if ((1 - ThisRuleArticle.StuffCalculatedCoefficient) * ThisRuleArticle.SpecialStuffQuantity > TheMostRemainingStuffQuantity)
                                            {
                                                TheMostCompetentArticle = ThisRuleArticle;
                                                TheMostRemainingStuffQuantity = (1 - ThisRuleArticle.StuffCalculatedCoefficient) * ThisRuleArticle.SpecialStuffQuantity;
                                            }

                                        Stuff = new FreeProductModel()
                                        {
                                            Stuff = TheMostCompetentArticle.Stuff,
                                            BatchNumber = TheMostCompetentArticle.BatchNumber
                                        };
                                    }

                                    var Package = GetStuffPackage(Stuff.Stuff.Id, DiscountRule.Steps[i].UnitId);
                                    if (Package != null)
                                    {
                                        var CalculatedFreeProduct = Stuff;
                                        CalculatedFreeProduct.Package = Package;
                                        CalculatedFreeProduct.Quantity = DiscountRule.Steps[i].FreeProductQuantity;

                                        CalculatedFreeProducts = AddToFreeProducts(CalculatedFreeProducts, CalculatedFreeProduct);
                                        EffectOnRows = "سطرهای " + RowNumbers;
                                        TriedToGetDiscount = true;
                                    }
                                }
                            }
                        #endregion
                        break;
                        case DiscountWay.CashSettlementPercent:
                            #region
                            #endregion
                            break;
                        default:
                            break;
                    }

                    foreach (var ThisRuleArticle in ThisRuleArticles)
                    {
                        ThisRuleArticle.DiscountPercent += CalculatedDiscountPercent;
                        ThisRuleArticle.FeeChange = -CalculatedCashPrise;

                        if (CalculatedDiscountPercent != 0 || CalculatedCashPrise != 0 || CalculatedFreeProducts.Any(a => a.Quantity != 0) || (SystemName == "Saman" && TriedToGetDiscount))
                        {
                            ThisRuleArticle.AggregateOtherDiscounts_Percent = DiscountRule.AggregateOtherDiscounts_Percent;
                            ThisRuleArticle.AggregateOtherDiscounts_SpecificMoney = DiscountRule.AggregateOtherDiscounts_SpecificMoney;
                            ThisRuleArticle.AggregateOtherDiscounts_FreeProduct = DiscountRule.AggregateOtherDiscounts_FreeProduct;
                            ThisRuleArticle.AggregateOtherDiscounts_CashSettlement = DiscountRule.AggregateOtherDiscounts_CashSettlement;
                        }
                    }
                    Order.FreeProducts = AddToFreeProducts(Order.FreeProducts, CalculatedFreeProducts);
                    #endregion
                    break;
                case DiscountBasedOn.QuantityOfSomeArticles:
                    #region
                    RowNumbers = "";
                    ThisRuleArticles = new List<ArticleModel>();
                    for (var i = 0; i < Order.Articles.Length; i++)
                        if ((DiscountRule.Way == DiscountWay.Percent && Order.Articles[i].AggregateOtherDiscounts_Percent) ||
                                (DiscountRule.Way == DiscountWay.SpecificMoney && Order.Articles[i].AggregateOtherDiscounts_SpecificMoney) ||
                                (DiscountRule.Way == DiscountWay.FreeProduct && Order.Articles[i].AggregateOtherDiscounts_FreeProduct) ||
                                (DiscountRule.Way == DiscountWay.CashSettlementPercent && Order.Articles[i].AggregateOtherDiscounts_CashSettlement))
                            if (CheckRuleConditions(DiscountRule, Order, i)) {
                                ThisRuleArticles.Add(Order.Articles[i]);
                                RowNumbers = (RowNumbers == "" ? "" : "، ") + i;
                            }
                    TriedToGetDiscount = false;
                    CalculatedDiscountPercent = 0;
                    CalculatedCashPrise = 0;
                    CalculatedFreeProducts = new List<FreeProductModel>();

                    decimal MinimumOccuranceOfBasketQuantity = 1000000;
                    for (var c = 0; c < DiscountRule.StuffIds.Length; c++) {
                        var Condition_StuffId = DiscountRule.StuffIds[c];
                        var Condition_UnitId = DiscountRule.StuffConditionExtraParameters_UnitId[c];
                        var Condition_Package = GetStuffPackage(Condition_StuffId, Condition_UnitId);
                        var Condition_Coefficient = Condition_Package.Coefficient;
                        var Condition_Quantity = DiscountRule.StuffConditionExtraParameters_Quantity[c];
                        var Condition_StuffQuantity = Condition_Quantity * Condition_Coefficient;

                        decimal Condition_StuffQuantitySum = ThisRuleArticles.Any(a => a.Stuff.Id == Condition_StuffId) ? ThisRuleArticles.Where(a => a.Stuff.Id == Condition_StuffId).Sum(a => a.StuffQuantity) : 0;
                        
                        var Condition_Occurance = Condition_StuffQuantitySum / Condition_StuffQuantity;
                        if (Condition_Occurance < MinimumOccuranceOfBasketQuantity)
                            MinimumOccuranceOfBasketQuantity = Condition_Occurance.GetValueOrDefault(0);
                    }

                    switch (DiscountRule.Way)
                    {
                        case DiscountWay.Percent:
                            #region
                            for (var i = 0; i < DiscountRule.Steps.Length; i++)
                                if (MinimumOccuranceOfBasketQuantity >= DiscountRule.Steps[i].Quantity)
                                {
                                    CalculatedDiscountPercent = DiscountRule.Steps[i].Percent;
                                    if (DiscountRule.ShowDiscountInUnitFee)
                                        foreach (var ThisRuleArticle in ThisRuleArticles)
                                            ThisRuleArticle.ChangePercentToFeeChange = true;

                                    if (CalculatedDiscountPercent != 0 || SystemName == "Saman") {
                                        EffectOnRows = "سطرهای " + RowNumbers;
                                        TriedToGetDiscount = true;
                                    }
                                }
                            #endregion
                            break;
                        case DiscountWay.SpecificMoney:
                            #region
                            for (var i = 0; i < DiscountRule.Steps.Length; i++)
                                if (MinimumOccuranceOfBasketQuantity >= DiscountRule.Steps[i].Quantity)
                                {
                                    CalculatedCashPrise = DiscountRule.Steps[i].SpecificMoney;
                                    if (CalculatedCashPrise != 0 || SystemName == "Saman")
                                    {
                                        EffectOnRows = "سطرهای " + RowNumbers;
                                        TriedToGetDiscount = true;
                                    }
                                }
                            #endregion
                            break;
                        case DiscountWay.FreeProduct:
                            #region
                            for (var i = 0; i < DiscountRule.Steps.Length; i++)
                            {
                                if (MinimumOccuranceOfBasketQuantity >= DiscountRule.Steps[i].Quantity)
                                {
                                    FreeProductModel Stuff;
                                    if (DiscountRule.Steps[i].SpecificStuffId.HasValue)
                                        Stuff = new FreeProductModel()
                                        {
                                            Stuff = AllSystemStuffs[DiscountRule.Steps[i].SpecificStuffId.Value],
                                            BatchNumber = null
                                        };
                                    else
                                    {
                                        ArticleModel TheMostCompetentArticle = null;
                                        decimal TheMostStuffQuantity = 0;
                                        foreach (var ThisRuleArticle in ThisRuleArticles)
                                            if (ThisRuleArticle.StuffQuantity > TheMostStuffQuantity)
                                            {
                                                TheMostCompetentArticle = ThisRuleArticle;
                                                TheMostStuffQuantity = ThisRuleArticle.StuffQuantity;
                                            }

                                        Stuff = new FreeProductModel()
                                        {
                                            Stuff = TheMostCompetentArticle.Stuff,
                                            BatchNumber = TheMostCompetentArticle.BatchNumber
                                        };
                                    }

                                    var Package = GetStuffPackage(Stuff.Stuff.Id, DiscountRule.Steps[i].UnitId);
                                    if (Package != null)
                                    {
                                        CalculatedFreeProducts = new List<FreeProductModel>() {
                                            new FreeProductModel()
                                            {
                                                Stuff = Stuff.Stuff,
                                                BatchNumber = Stuff.BatchNumber,
                                                Package = Package,
                                                Quantity = DiscountRule.Steps[i].FreeProductQuantity
                                            }
                                        };

                                        if (DiscountRule.Steps[i].ContinueToCalculateOnThisRatio)
                                        {
                                            var RealValue = MinimumOccuranceOfBasketQuantity / DiscountRule.Steps[i].Quantity;
                                            var ShouldBeQuantityRounded = CalculatedFreeProducts[0].Quantity * RealValue;
                                            if (DiscountRule.Steps[i].RatioCalculationMethod == RatioCalculationMethod.Round)
                                                ShouldBeQuantityRounded = Math.Floor(ShouldBeQuantityRounded);
                                            else
                                                ShouldBeQuantityRounded = Math.Round(ShouldBeQuantityRounded);

                                            CalculatedFreeProducts[0].Quantity = ShouldBeQuantityRounded;
                                        }
                                        else if (DiscountRule.Steps[i].AscendingCalculation)
                                        {
                                            var RealValue = MinimumOccuranceOfBasketQuantity / DiscountRule.Steps[i].Quantity;
                                            var IntValue = Math.Floor(MinimumOccuranceOfBasketQuantity / DiscountRule.Steps[i].Quantity);
                                            CalculatedFreeProducts[0].Quantity *= IntValue;
                                        }
                                    }
                                }
                            }

                            if (CalculatedFreeProducts.Any(a => a.Quantity != 0 || SystemName == "Saman"))
                            {
                                EffectOnRows = "سطرهای " + RowNumbers;
                                TriedToGetDiscount = true;
                            }
                            #endregion
                            break;
                        case DiscountWay.CashSettlementPercent:
                            #region
                            #endregion
                            break;
                        default:
                            break;
                    }
                    
                    foreach (var ThisRuleArticle in ThisRuleArticles)
                    {
                        if (RowCashDiscountIsSeperate)
                            ThisRuleArticle.CashDiscountPercent += CalculatedDiscountPercent;
                        else
                            ThisRuleArticle.DiscountPercent += CalculatedDiscountPercent;
                        ThisRuleArticle.FeeChange = -CalculatedCashPrise;

                        if (CalculatedDiscountPercent != 0 || CalculatedCashPrise != 0 || CalculatedFreeProducts.Any(a => a.Quantity != 0) || (SystemName == "Saman" && TriedToGetDiscount)) {
                            ThisRuleArticle.AggregateOtherDiscounts_Percent = DiscountRule.AggregateOtherDiscounts_Percent;
                            ThisRuleArticle.AggregateOtherDiscounts_SpecificMoney = DiscountRule.AggregateOtherDiscounts_SpecificMoney;
                            ThisRuleArticle.AggregateOtherDiscounts_FreeProduct = DiscountRule.AggregateOtherDiscounts_FreeProduct;
                            ThisRuleArticle.AggregateOtherDiscounts_CashSettlement = DiscountRule.AggregateOtherDiscounts_CashSettlement;
                        }
                    }
                    Order.FreeProducts = AddToFreeProducts(Order.FreeProducts, CalculatedFreeProducts);
                    #endregion
                    break;
                case DiscountBasedOn.CombinedStuffBasket:
                    #region
                    RowNumbers = "";
                    var CombinedStuffBaskets_Bought = DiscountRule.CombinedStuffBaskets_Bought.Select(a => new KeyValuePair<DiscountRuleCombinedStuffBasket, List<ArticleModel>>(new DiscountRuleCombinedStuffBasket()
                    {
                        Id = a.Id,
                        Title = a.Title,
                        StuffGroupCodes = a.StuffGroupCodes,
                        StuffIds = a.StuffIds
                    }, new List<ArticleModel>())).ToList();
                    var CombinedStuffBaskets_Discount = DiscountRule.CombinedStuffBaskets_Discount.Select(a => new KeyValuePair<DiscountRuleCombinedStuffBasket, List<ArticleModel>>(new DiscountRuleCombinedStuffBasket()
                    {
                        Id = a.Id,
                        Title = a.Title,
                        StuffGroupCodes = a.StuffGroupCodes,
                        StuffIds = a.StuffIds
                    }, new List<ArticleModel>())).ToList();
                    
                    for (var i = 0; i < Order.Articles.Length; i++)
                    {
                        if (Order.Articles[i].Quantity > 0)
                        {
                            if ((DiscountRule.Way == DiscountWay.Percent && Order.Articles[i].AggregateOtherDiscounts_Percent) ||
                            (DiscountRule.Way == DiscountWay.SpecificMoney && Order.Articles[i].AggregateOtherDiscounts_SpecificMoney) ||
                            (DiscountRule.Way == DiscountWay.FreeProduct && Order.Articles[i].AggregateOtherDiscounts_FreeProduct) ||
                            (DiscountRule.Way == DiscountWay.CashSettlementPercent && Order.Articles[i].AggregateOtherDiscounts_CashSettlement))
                                if (CheckRuleConditions(DiscountRule, Order, i))
                                {
                                    for (var j = 0; j < CombinedStuffBaskets_Bought.Count; j++)
                                    {
                                        var ThisArticleIsInThisBasket = false;
                                        var StuffGroupCodes = CombinedStuffBaskets_Bought[j].Key.StuffGroupCodes;
                                        for (var k = 0; k < StuffGroupCodes.Length; k++)
                                        {
                                            if (Order.Articles[i].Stuff.GroupCode.StartsWith(StuffGroupCodes[k]))
                                            {
                                                ThisArticleIsInThisBasket = true;
                                                break;
                                            }
                                        }
                                        if (!ThisArticleIsInThisBasket)
                                        {
                                            var StuffIds = CombinedStuffBaskets_Bought[j].Key.StuffIds;
                                            for (var k = 0; k < StuffIds.Length; k++)
                                            {
                                                if (Order.Articles[i].Stuff.Id == StuffIds[k])
                                                {
                                                    ThisArticleIsInThisBasket = true;
                                                    break;
                                                }
                                            }
                                        }
                                        if (ThisArticleIsInThisBasket)
                                            CombinedStuffBaskets_Bought[j].Value.Add(Order.Articles[i]);
                                    }
                                    for (var j = 0; j < CombinedStuffBaskets_Discount.Count; j++)
                                    {
                                        var ThisArticleIsInThisBasket = false;
                                        var StuffGroupCodes = CombinedStuffBaskets_Discount[j].Key.StuffGroupCodes;
                                        for (var k = 0; k < StuffGroupCodes.Length; k++)
                                        {
                                            if (Order.Articles[i].Stuff.GroupCode.StartsWith(StuffGroupCodes[k]))
                                            {
                                                ThisArticleIsInThisBasket = true;
                                                break;
                                            }
                                        }
                                        if (!ThisArticleIsInThisBasket)
                                        {
                                            var StuffIds = CombinedStuffBaskets_Discount[j].Key.StuffIds;
                                            for (var k = 0; k < StuffIds.Length; k++)
                                            {
                                                if (Order.Articles[i].Stuff.Id == StuffIds[k])
                                                {
                                                    ThisArticleIsInThisBasket = true;
                                                    break;
                                                }
                                            }
                                        }
                                        if (ThisArticleIsInThisBasket)
                                            CombinedStuffBaskets_Discount[j].Value.Add(Order.Articles[i]);
                                    }

                                    RowNumbers = (RowNumbers == "" ? "" : "، ") + i;
                                }
                        }
                    }
                    
                    CalculatedDiscountPercent = 0;
                    var CalculatedDiscountPercentForArticles = new List<ArticleModel>();
                    
                    switch (DiscountRule.Way)
                    {
                        case DiscountWay.Percent:
                            #region
                            for (var i = 0; i < DiscountRule.Steps.Length; i++)
                            {
                                var StepConditionIsTrue = false;
                                if (DiscountRule.CombineStuffBaskets_SpecifyEachSoledBasket)
                                {
                                    StepConditionIsTrue = true;
                                    for (var j = 0; j < CombinedStuffBaskets_Bought.Count; j++)
                                    {
                                        var IsThisBasketShouldBeIncludedInOrder = false;
                                        for (var k = 0; k < DiscountRule.Steps[i].BoughtBasketIds.Length; k++)
                                        {
                                            if (DiscountRule.Steps[i].BoughtBasketIds[k] == CombinedStuffBaskets_Bought[j].Key.Id)
                                            {
                                                IsThisBasketShouldBeIncludedInOrder = true;
                                                break;
                                            }
                                        }
                                        if (IsThisBasketShouldBeIncludedInOrder && CombinedStuffBaskets_Bought[j].Value.Count == 0)
                                        {
                                            StepConditionIsTrue = false;
                                            break;
                                        }
                                    }
                                }
                                else {
                                    var BasketCount = 0;
                                    for (var j = 0; j < CombinedStuffBaskets_Bought.Count; j++)
                                        if (CombinedStuffBaskets_Bought[j].Value.Count > 0)
                                            BasketCount++;
                                    StepConditionIsTrue = BasketCount >= DiscountRule.Steps[i].Quantity;
                                }

                                if (StepConditionIsTrue)
                                {
                                    CalculatedDiscountPercent = DiscountRule.Steps[i].Percent;
                                    CalculatedDiscountPercentForArticles = new List<ArticleModel>();
                                    if (DiscountRule.CombineStuffBaskets_SpecifyEachBasketWhichGetDiscount)
                                    {
                                        for (var j = 0; j < CombinedStuffBaskets_Discount.Count; j++)
                                        {
                                            var IsThisBasketGetsDiscount = false;
                                            for (var k = 0; k < DiscountRule.Steps[i].DiscountBasketIds.Length; k++)
                                            {
                                                if (DiscountRule.Steps[i].DiscountBasketIds[k] == CombinedStuffBaskets_Discount[j].Key.Id)
                                                {
                                                    IsThisBasketGetsDiscount = true;
                                                    break;
                                                }
                                            }
                                            if (IsThisBasketGetsDiscount)
                                            {
                                                for (var k = 0; k < CombinedStuffBaskets_Discount[j].Value.Count; k++)
                                                    CalculatedDiscountPercentForArticles.Add(CombinedStuffBaskets_Discount[j].Value[k]);
                                            }
                                        }
                                    }
                                    else {
                                        for (var j = 0; j < CombinedStuffBaskets_Bought.Count; j++)
                                            for (var k = 0; k < CombinedStuffBaskets_Bought[j].Value.Count; k++)
                                                CalculatedDiscountPercentForArticles.Add(CombinedStuffBaskets_Bought[j].Value[k]);
                                    }
                                }
                            }
                            #endregion
                            break;
                        default:
                            break;
                    }

                    for (var i = 0; i < CalculatedDiscountPercentForArticles.Count; i++)
                    {
                        if (RowCashDiscountIsSeperate)
                            CalculatedDiscountPercentForArticles[i].CashDiscountPercent += CalculatedDiscountPercent;
                        else
                            CalculatedDiscountPercentForArticles[i].DiscountPercent += CalculatedDiscountPercent;
                        if (CalculatedDiscountPercent != 0)
                        {
                            CalculatedDiscountPercentForArticles[i].AggregateOtherDiscounts_Percent = DiscountRule.AggregateOtherDiscounts_Percent;
                            CalculatedDiscountPercentForArticles[i].AggregateOtherDiscounts_SpecificMoney = DiscountRule.AggregateOtherDiscounts_SpecificMoney;
                            CalculatedDiscountPercentForArticles[i].AggregateOtherDiscounts_FreeProduct = DiscountRule.AggregateOtherDiscounts_FreeProduct;
                            CalculatedDiscountPercentForArticles[i].AggregateOtherDiscounts_CashSettlement = DiscountRule.AggregateOtherDiscounts_CashSettlement;
                        }
                    }
                    #endregion
                    break;
                default:
                    break;
            }
            
            for (var i = 0; i < Order.Articles.Length; i++)
                if (Order.Articles[i].ChangePercentToFeeChange)
                {
                    Order.Articles[i].FeeChange = -Math.Round(Order.Articles[i].DiscountAmount / Order.Articles[i].StuffQuantity);
                    Order.Articles[i].CashDiscountPercent = 0;
                    Order.Articles[i].DiscountPercent = 0;
                }

            effectOnRows = EffectOnRows;
            return Order;
        }

        private Package GetStuffPackage(Guid StuffId, Guid? UnitId)
        {
            var Stuff = AllSystemStuffs.ContainsKey(StuffId) ? AllSystemStuffs[StuffId] : null;
            if (Stuff == null)
                Stuff = App.DB.GetStuff(StuffId);

            if (Stuff == null)
            {
                var AllStuffs = App.DB.conn.Table<Stuff>().ToArray();
                Stuff = AllStuffs.SingleOrDefault(a => a.Id == StuffId);
            }   

            if (UnitId.HasValue)
                return Stuff.Packages.SingleOrDefault(a => a.UnitId == UnitId && a.Enabled);
            else
                return Stuff.Packages.SingleOrDefault(a => a.Coefficient == 1 && a.Enabled);
        }

        private decimal ArticleQuantityInAUnit(ArticleModel Article, Guid UnitId)
        {
            var ArticleUnit = Article.Stuff.Packages.SingleOrDefault(a => a.UnitId == UnitId && a.Enabled);
            if (ArticleUnit != null)
                return ArticleUnit.Coefficient == 0 ? 0 : Article.StuffQuantity / ArticleUnit.Coefficient;
            return 0;
        }

        public bool CheckRuleConditions(RuleModel Rule, OrderModel Order, int? RowIndex)
        {
            if (!(Rule.BeginDate.Date <= Order.OrderInsertDate.Date && Order.OrderInsertDate.Date <= Rule.EndDate.Date))
                return false;

            if (Rule.ConditionOnPartner)
            {
                if (!Rule.PartnerIds.Any(a => a == Order.Partner.Id))
                    return false;
            }
            if (Rule.ConditionOnPartnerGroup)
            {
                if (!Rule.PartnerGroupIds.Any(a => Order.Partner.Groups.Any(b => b.Id == a)))
                    return false;
            }
            if (Rule.ConditionOnZone && !Order.Partner.GroupCode.StartsWith("0102"))
            {
                if (!Rule.ZoneGeneralCodes.Any(a => Order.Partner.GroupCode.StartsWith(a)))
                    return false;
            }
            if (Rule.ConditionOnVisitor)
            {
                if (!Rule.VisitorIds.Any(a => a == Order.VisitorId))
                    return false;
            }
            if (Rule.ConditionOnStuff)
            {
                if (!Rule.StuffIds.Any(a => a == Order.Articles[RowIndex.Value].Stuff.Id))
                    return false;
            }
            if (Rule.ConditionOnStuffGroup)
            {
                if (!Rule.StuffGroupCodes.Any(a => Order.Articles[RowIndex.Value].Stuff.GroupCode.StartsWith(a)))
                    return false;
            }
            if (Rule.ConditionOnStuffBasket)
            {
                if (!Rule.StuffBasketIds.Any(a => Order.Articles[RowIndex.Value].Stuff.Baskets.Any(b => b.Id == a)))
                    return false;
            }
            if (Rule.ConditionOnSettlementType)
            {
                if (!Rule.SettlementTypeIds.Any(a => a == Order.SettlementTypeId))
                    return false;
            }
            if (Rule.ConditionOnSettlementDay && Rule.SettlementDay.HasValue)
            {
                if (Order.SettlementDay > Rule.SettlementDay.Value)
                    return false;
            }

            return true;
        }

        private List<FreeProductModel> AddToFreeProducts(List<FreeProductModel> FreeProducts, FreeProductModel item)
        {
            if (item == null)
                return FreeProducts;

            var ExistingFreeProduct = FreeProducts.SingleOrDefault(a => a.Stuff.Id == item.Stuff.Id && a.Package.UnitId == item.Package.UnitId && (a.BatchNumber == null ? new Nullable<Guid>() : a.BatchNumber.BatchNumberId) == (item.BatchNumber == null ? new Nullable<Guid>() : item.BatchNumber.BatchNumberId));

            if (ExistingFreeProduct == null)
                FreeProducts.Add(item);
            else
                ExistingFreeProduct.Quantity += item.Quantity;

            return FreeProducts;
        }

        private List<FreeProductModel> AddToFreeProducts(List<FreeProductModel> FreeProducts, List<FreeProductModel> items)
        {
            foreach (var item in items)
                FreeProducts = AddToFreeProducts(FreeProducts, item);
            
            return FreeProducts;
        }
    }
}
