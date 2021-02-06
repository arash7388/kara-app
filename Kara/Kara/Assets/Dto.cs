using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kara.Assets
{
    public class DtoEditSaleOrder
    {
        public DtoSaleOrder SaleOrder { get; set; }
        public int SaleOrderStuffsCount { get; set; }
        public List<DtoSaleOrderStuff> SaleOrderStuffs { get; set; }
        public List<DtoSaleOrderFreeProduct> SaleOrderFreeProducts { get; set; }
        public string Operation { get; set; }
        public List<DtoCashDiscountSteps> CashDiscountSteps { get; set; }
        public string DistributionReversionReasonName { get; set; }
        public string DistributionReversionReasonId { get; set; }
    }

    public class DtoCashDiscountSteps
    {
        public int Day { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalPrice { get; set; }

    }
    public class DtoSaleOrderFreeProduct
    {
        public string StuffName { get; set; }
        public Guid StuffId { get; set; }
        public string StuffCode { get; set; }
        public string BatchNumberId { get; set; }
        public string BatchNumber { get; set; }
        public string ExpirationDate { get; set; }
        public Guid PackageId { get; set; }
        public string PackageName { get; set; }
        public decimal PackageCoefficient { get; set; }
        public decimal FreeProductQuantity { get; set; }
        public decimal? FreeProduct_UnitPrice { get; set; }
        public List<string> StuffGroupNames { get; set; }
        public List<Guid> StuffGroupIds { get; set; }
    }

    public class DtoPersonnelHistoryInformation
    {
        public bool PersonnelIsVisitor { get; set; }
        public int? MaximumOrderPerPeriod { get; set; }
        public int? PeriodForMaximumOrder { get; set; }
        public List<KeyValuePair<string, int>> personnelOrdersHistory { get; set; }
    }

    public class DtoSaleOrder
    {
        public decimal TotalPackageQuntity { get; set; }
        public decimal? TotalStuffQuntity { get; set; }
        public decimal? TotalQuntityPrice { get; set; }
        public decimal? TotalDiscountAmount { get; set; }
        public decimal? TotalFinalPrice { get; set; }
        public Guid OrderId { get; set; }
        public int OrderPreCode { get; set; }
        public string OrderInsertDate { get; set; }
        public Guid PartnerId { get; set; }
        public string PartnerCode { get; set; }
        public string PartnerName { get; set; }
        public string PartnerAddress { get; set; }
        public string PartnerLegalName { get; set; }
        public string PartnerPhones { get; set; }
        public string partnerZoneCode { get; set; }
        public string partnerSpecialVisitorId { get; set; }
        public string[] PartnerDynamicGroupIds { get; set; }
        public string PartnerGroupCode { get; set; }
        public bool isPartnerAPersonnel { get; set; }
        public bool PartnerIsLegal { get; set; }
        public bool CalculateVATForThisPerson { get; set; }
        public int PartnerDistributionTotalReversionCount { get; set; }
        public Guid? PersonnelId { get; set; }
        public string PersonnelCode { get; set; }
        public string PersonnelName { get; set; }
        public string[] personnelZoneCodes { get; set; }
        public IEnumerable<string> VisitorDynamicGroupIds { get; set; }
        public bool PersonnelTerritories_PartnerGroupCheck { get; set; }
        public bool PersonnelTerritories_ZoneAndRouteCheck { get; set; }
        public bool PersonnelTerritories_OrCondition { get; set; }
        public string PersonnelTypeCode { get; set; }
        public string ChequeSettlementDay { get; set; }
        public string OrderDiscountCalculationFrom { get; set; }
        public string CostCenterName { get; set; }
        public Guid? CostCenterId { get; set; }
        public int EditingOrderState { get; set; }
        public string PartnerOwnOrderCode { get; set; }
        public string DefaultWarehouseName { get; set; }
        public string DefaultWarehouseId { get; set; }
        public Dictionary<string, int> PartnerSaleHistory { get; set; }
        public DtoPersonnelHistoryInformation personnelHistoryInformation { get; set; }
        public Guid SettlementTypeId { get; set; }
        public string SettlementName { get; set; }
        public decimal OrderDiscountPercent { get; set; }
        public decimal? OrderDiscountAmount { get; set; }
        public string OrderDescription { get; set; }
        public string OrderDescription2 { get; set; }
        public string InsertUserName { get; set; }
        public int OrderStuffCount { get; set; }
        public int DiscountPercentChanged { get; set; }
        public bool PriceEditable { get; set; }
        public bool DiscountEditable { get; set; }
        public bool VATEditable { get; set; }
        public decimal CashPriseAmount { get; set; }
        public decimal CashSettlementDiscountPercent { get; set; }
        public decimal? CashSettlementDiscountAmount { get; set; }
        public decimal? OrderFinalPrice { get; set; }
        public string OrdererName { get; set; }
        public string PartnerCity { get; set; }
        public string PartnerZone { get; set; }
        public string PartnerRoute { get; set; }
        public decimal StuffPricesSum { get; set; }
        public decimal? DiscountAmountsSum { get; set; }
        public decimal? OrderPriceSum { get; set; }
        public string DistributionReversionReasonName { get; set; }
        public string DistributionReversionReasonId { get; set; }
        public bool DeterminigWholeDistributionReversionForfeit { get; set; }
        public bool IncludesDistributionTotalReversionForfeit { get; set; }
        public string DistributionTotalReversionForfeit { get; set; }
        public bool OrderInTransportation { get; set; }
        public bool CalculateThePriceOnShippedOrders { get; set; }
        public bool CalculateTheDiscountOnShippedOrders { get; set; }
        public IEnumerable<AddSubPriceModel> AddedFields { get; set; }
        public IEnumerable<AddSubPriceModel> SubtractedFields { get; set; }
        public bool SubField1_IsVATDeduction { get; set; }
        public bool DecreaseVAT { get; set; }
        public Dictionary<int, IEnumerable<DtoSaleOrderOptionalDiscountRules>> OptinalDiscountRules { get; set; }
        
    }

    public class DtoSaleOrderOptionalDiscountRules
    {
        public bool Choosed { get; set; }
        public string RuleId { get; set; }
    }

    public class AddSubPriceModel
    {
        public string Title { get; set; }
        public decimal Price { get; set; }
        public Guid? SpecificCodingId { get; set; }
        public string SpecificCoding { get; set; }
        public Guid? EntityId { get; set; }
        public string Entity { get; set; }
    }

    public class DtoSaleOrderStuff
    {
        public Guid Id { get; set; }
        public int Index { get; set; }
        public Guid StuffId { get; set; }
        public string StuffName { get; set; }
        public List<string> CustomColumns { get; set; }
        public string BatchNumberId { get; set; }
        public string BatchNumber { get; set; }
        public string ExpirationDate { get; set; }
        public string StuffDescription { get; set; }
        public decimal StuffVATPercent { get; set; }
        public decimal LastBuyUnitPrice { get; set; }
        public string StuffCode { get; set; }
        public Guid DefaultPackageId { get; set; }
        public string DefaultPackageName { get; set; }
        public Guid PackageId { get; set; }
        public Guid UnitId { get; set; }
        public string PackageName { get; set; }
        public string PackageFlag { get; set; }
        public decimal Quantity { get; set; }
        public decimal PackageCoefficient { get; set; }
        public decimal? StuffQuantity { get; set; }
        public decimal SalePrice { get; set; }
        public decimal FeeChange { get; set; }
        public decimal? QuantityPrice { get; set; }
        public decimal DiscountPercent { get; set; }
        public decimal CashDiscountPercent { get; set; }
        public string DiscountCalculationFrom { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? ArticlePrice { get; set; }
        public string StuffGroupCode { get; set; }
        public List<Guid> StuffBasketIds { get; set; }
        public object Units { get; set; }
        public decimal PackageVolume { get; set; }
        public decimal PackageWeight { get; set; }
        public decimal VATPercent { get; set; }
        public decimal VATAmount { get; set; }
        public decimal MaxPackageCoefficient { get; set; }
        public bool NeedsToBeMeasuredBeforeShipment { get; set; }
        public bool MeasurementDone { get; set; }
        public List<string> StuffGroupNames { get; set; }
        public List<Guid> StuffGroupIds { get; set; }
    }

    public class GetOrderEditDataDto
    {
        public DtoSaleOrder SaleOrder { get; set; }
        public int SaleOrderStuffsCount { get; set; }
        public List<DtoSaleOrderStuff> SaleOrderStuffs { get; set; }
        public Guid WarehouseId { get; set; }
    }
}
