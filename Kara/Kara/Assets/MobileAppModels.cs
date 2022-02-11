using System;
using Kara.Models;

namespace Kara.Assets
{
    public class LoginResult
    {
        public Guid UserId { get; set; }
        public Guid PersonnelId { get; set; }
        public Guid EntityId { get; set; }
        public string RealName { get; set; }
        public string EntityCode { get; set; }
    }

    public class UpdateDB_StuffBatchModel
    {
        public int TotalCount { get; set; }
        public int From { get; set; }
        public StuffGroup[] StuffGroups { get; set; }
        public Stuff[] Stuffs { get; set; }
        public Unit[] Units { get; set; }
        public Package[] Packages { get; set; }
        public StuffBasket[] StuffBaskets { get; set; }
        public StuffBasketStuff[] StuffBasketStuffs { get; set; }
        public StuffOrder[] StuffOrders { get; set; }
        public StuffBatchNumber[] StuffBatchNumbers { get; set; }
        public StuffSettlementDay[] StuffSettlementDays { get; set; }
    }

    public class UpdateDB_StockBatchModel
    {
        public int TotalCount { get; set; }
        public int From { get; set; }
        public Stock[] Stocks { get; set; }
        public Warehouse[] Warehouses { get; set; }
    }

    public class UpdateDB_PartnerBatchModel
    {
        public int TotalCount { get; set; }
        public int From { get; set; }
        public Zone[] Cities { get; set; }
        public Zone[] Zones { get; set; }
        public Zone[] Routes { get; set; }
        public Partner[] Partners { get; set; }
        public DynamicGroup[] DynamicGroups { get; set; }
        public DynamicGroupPartner[] DynamicGroupPartners { get; set; }
        public Credit[] Credits { get; set; }
        public VisitProgramPartner[] VisitProgramPartners { get; set; }
    }

    public class UpdateDB_PriceListBatchModel
    {
        public int TotalCount { get; set; }
        public int From { get; set; }
        public PriceListZone[] PriceListZones { get; set; }
        public PriceListDynamicPartnerGroup[] PriceListDynamicPartnerGroups { get; set; }
        public PriceListVersion[] PriceListVersions { get; set; }
        public PriceListStuff[] PriceListStuffs { get; set; }
        public int LastPriceListVersion { get; set; }
    }

    public class AppSettingsModel
    {
        public string SystemName { get; set; }
        public decimal VATPercent { get; set; }
        public bool UseBatchNumberAndExpirationDate { get; set; }
        public bool CheckForNegativeStocksOnOrderInsertion { get; set; }
        public bool OrderPrintShowMainUnitFee { get; set; }
        public bool OrderPrintShowSmallUnitFee { get; set; }
        public bool UseVisitProgram { get; set; }
        public long? _VisitorBeginWorkTime { get; set; }
        public TimeSpan? VisitorBeginWorkTime
        {
            get
            {
                return _VisitorBeginWorkTime.HasValue ? new TimeSpan(_VisitorBeginWorkTime.Value) : new TimeSpan?();
            }
            set
            {
                _VisitorBeginWorkTime = value.HasValue ? value.Value.Ticks : new long?();
            }
        }
        public long? _VisitorEndWorkTime { get; set; }
        public TimeSpan? VisitorEndWorkTime
        {
            get
            {
                return _VisitorEndWorkTime.HasValue ? new TimeSpan(_VisitorEndWorkTime.Value) : new TimeSpan?();
            }
            set
            {
                _VisitorEndWorkTime = value.HasValue ? value.Value.Ticks : new long?();
            }
        }
        public bool GPSShouldBeTurnedOnDuringWorkTime { get; set; }
        public bool InternetShouldBeConnectedDuringWorkTime { get; set; }
        public decimal ShowSaleVisitProgramPartnersToVisitorHourShift { get; set; }
        public decimal DayStartTime { get; set; }
        public decimal DayEndTime { get; set; }
        public bool AllowOptionalDiscountRules_MultiSelection { get; set; }
        public bool AllowOptionalDiscountRules { get; set; }
        public string CompanyNameForPrint { get; set; }
        public string CompanyLogoForPrint { get; set; }
        public string PrintTitle { get; set; }
        public string EndOfPrintDescription { get; set; }
        public bool HasPrintingOption { get; set; }
        public bool UseBarcodeScannerInVisitorAppToSelectStuff { get; set; }
        public bool UseQRScannerInVisitorAppToSelectStuff { get; set; }
        public string[] QRScannerInVisitorAppForSelectingStuffTemplates { get; set; }
        public int CalculateStuffsSettlementDaysBasedOn { get; set; }
        public bool DefineWarehouseForSaleAndBuy { get; set; }
        public bool UseCollectorAndroidApplication { get; set; }//تحصیلدار
        public bool UseVisitorsNadroidApplication { get; set; } 
        public bool UseDistributerAndroidApplication { get; set; }//موزع
        public int WarnIfSalePriceIsLessThanTheLastBuyPrice { get; set; }
    }
    public class UpdateDB_OtherInformationBatchModel
    {
        public int TotalCount { get; set; }
        public int From { get; set; }
        public SettlementType[] SettlementTypes { get; set; }
        public NotOrderReason[] NotOrderReasons { get; set; }
        public Access[] Accesses { get; set; }
        public AppSettingsModel[] AppSettings { get; set; }
    }

    public class UpdateDB_DiscountRuleBatchModel
    {
        public int TotalCount { get; set; }
        public int From { get; set; }
        public DiscountRule[] DiscountRules { get; set; }
        public DiscountRuleCondition[] DiscountRuleConditions { get; set; }
        public DiscountRuleStep[] DiscountRuleSteps { get; set; }
        public SaleDiscountRuleStuffBasket[] DiscountRuleStuffBaskets { get; set; }
        public SaleDiscountRuleStuffBasketDetail[] DiscountRuleStuffBasketDetails { get; set; }
        public SaleDiscountRuleStepStuffBasket[] DiscountRuleStepStuffBaskets { get; set; }
        public int LastDiscountRuleVersion { get; set; }
    }

    public class UpdateDB_CashBatchModel
    {
        public int TotalCount { get; set; }
        public int From { get; set; }
        public Cash[] Cashes { get; set; }
    }

    public class UpdateDB_BankBatchModel
    {
        public int TotalCount { get; set; }
        public int From { get; set; }
        public Bank[] Banks { get; set; }
    }

    public class UpdateDB_BankAccountBatchModel
    {
        public int TotalCount { get; set; }
        public int From { get; set; }
        public BankAccount[] BankAccounts { get; set; }
    }


}
