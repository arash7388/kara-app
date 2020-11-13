using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using System.Reflection;
using Kara.Assets;

namespace Kara.Models
{
    public class ModelsInterface
    {
        public enum TransactionType
        {
            CashDocumentIn = 1,         //دریافت نقدی
            CashDocumentOut = 2,        //پرداخت نقدی
            BankDocumentIn = 3,         //حواله دریافتنی
            BankDocumentOut = 4,        //حواله پرداختنی
            ChequeDocumentIn = 5,       //دریافت چک
            ChequeDocumentOut = 6,      //پرداخت چک
            CashCharge = 10,            //شارژ صندوق
            BankCharge = 11,            //شارژ بانک
        }

        public static KeyValuePair<Type, string>[] AllTables = new KeyValuePair<Type, string>[]
        {
            new KeyValuePair<Type, string>(typeof(Zone), "Zone"),
            new KeyValuePair<Type, string>(typeof(DynamicGroup), "DynamicGroup"),
            new KeyValuePair<Type, string>(typeof(Credit), "Credit"),
            new KeyValuePair<Type, string>(typeof(Partner), "Partner"),
            new KeyValuePair<Type, string>(typeof(DynamicGroupPartner), "DynamicGroupPartner"),
            new KeyValuePair<Type, string>(typeof(StuffGroup), "StuffGroup"),
            new KeyValuePair<Type, string>(typeof(StuffOrder), "StuffOrder"),
            new KeyValuePair<Type, string>(typeof(Stuff), "Stuff"),
            new KeyValuePair<Type, string>(typeof(PriceListVersion), "PriceListVersion"),
            new KeyValuePair<Type, string>(typeof(PriceListStuff), "PriceListStuff"),
            new KeyValuePair<Type, string>(typeof(PriceListZone), "PriceListZone"),
            new KeyValuePair<Type, string>(typeof(PriceListDynamicPartnerGroup), "PriceListDynamicPartnerGroup"),
            new KeyValuePair<Type, string>(typeof(Unit), "Unit"),
            new KeyValuePair<Type, string>(typeof(Package), "Package"),
            new KeyValuePair<Type, string>(typeof(StuffBasket), "StuffBasket"),
            new KeyValuePair<Type, string>(typeof(StuffBasketStuff), "StuffBasketStuff"),
            new KeyValuePair<Type, string>(typeof(StuffBatchNumber), "StuffBatchNumber"),
            new KeyValuePair<Type, string>(typeof(StuffSettlementDay), "StuffSettlementDay"),
            new KeyValuePair<Type, string>(typeof(LocationModel), "LocationModel"),
            new KeyValuePair<Type, string>(typeof(SettlementType), "SettlementType"),
            new KeyValuePair<Type, string>(typeof(Warehouse), "Warehouse"),
            new KeyValuePair<Type, string>(typeof(Stock), "Stock"),
            new KeyValuePair<Type, string>(typeof(VisitProgramPartner), "VisitProgramPartner"),
            new KeyValuePair<Type, string>(typeof(NotOrderReason), "NotOrderReason"),
            new KeyValuePair<Type, string>(typeof(FailedVisit), "FailedVisit"),
            new KeyValuePair<Type, string>(typeof(DiscountRule), "DiscountRule"),
            new KeyValuePair<Type, string>(typeof(DiscountRuleCondition), "DiscountRuleCondition"),
            new KeyValuePair<Type, string>(typeof(SaleDiscountRuleStuffBasket), "SaleDiscountRuleStuffBasket"),
            new KeyValuePair<Type, string>(typeof(SaleDiscountRuleStuffBasketDetail), "SaleDiscountRuleStuffBasketDetail"),
            new KeyValuePair<Type, string>(typeof(DiscountRuleStep), "DiscountRuleStep"),
            new KeyValuePair<Type, string>(typeof(SaleDiscountRuleStepStuffBasket), "SaleDiscountRuleStepStuffBasket"),
            new KeyValuePair<Type, string>(typeof(Access), "Access"),
            new KeyValuePair<Type, string>(typeof(SaleOrder), "SaleOrder"),
            new KeyValuePair<Type, string>(typeof(SaleOrderStuff), "SaleOrderStuff"),
            new KeyValuePair<Type, string>(typeof(CashDiscount), "CashDiscount"),
            new KeyValuePair<Type, string>(typeof(DeviceSettingChange), "DeviceSettingChange"),
            new KeyValuePair<Type, string>(typeof(FinancialTransactionDocument), "FinancialTransactionDocument"),
            new KeyValuePair<Type, string>(typeof(Cash), "Cash"),
            new KeyValuePair<Type, string>(typeof(Bank), "Bank"),
            new KeyValuePair<Type, string>(typeof(BankAccount), "BankAccount")
        };

        private static Dictionary<Type, List<PropertyInfo>> _PrimaryKeys;
        public static Dictionary<Type, List<PropertyInfo>> PrimaryKeys
        { get
            {
                if (_PrimaryKeys == null)
                    _PrimaryKeys = AllTables.Select(t => new KeyValuePair<Type, List<PropertyInfo>>(t.Key, t.Key.GetRuntimeProperties().Where(prop => prop.GetCustomAttribute<PrimaryKeyAttribute>() != null).ToList())).ToDictionary(a => a.Key, a => a.Value);
                
                return _PrimaryKeys;
            }
        }
    }

    [Table("TempTable")]
    public class TempTable
    {
        [PrimaryKey]
        public int Id { get; set; }
    }

    [Table("Zone")]
    public class Zone
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        public Guid EntityGroupId { get; set; }
        [MaxLength(10)]
        public string Code { get; set; }
        [MaxLength(100)]
        public string Name { get; set; }
        [MaxLength(10)]
        public string CompleteCode { get; set; }
        [MaxLength(250)]
        public string CompleteName { get; set; }
        public Guid? ParentId { get; set; }
        public int ZoneLevel { get; set; }
        [MaxLength(30)]
        public string EntityGroupCode { get; set; }
    }
    
    [Table("DynamicGroup")]
    public class DynamicGroup
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        [MaxLength(200)]
        public string Name { get; set; }
    }

    //TODO: This is just a name for now
    //Should add credit parameteres after finalizing the topic in main kara applicaiton
    [Table("Credit")]
    public class Credit
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        [MaxLength(200)]
        public string Name { get; set; }
    }

    [Table("Partner")]
    public class Partner
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        [MaxLength(20), Unique]
        public string Code { get; set; }
        [MaxLength(150)]
        public string Name { get; set; }
        [MaxLength(50)]
        public string FirstName { get; set; }
        [MaxLength(100)]
        public string LastName { get; set; }
        [MaxLength(30)]
        public string GroupCode { get; set; }
        [MaxLength(250)]
        public string LegalName { get; set; }
        [MaxLength(100)]
        public string Phone1 { get; set; }
        [MaxLength(100)]
        public string Phone2 { get; set; }
        [MaxLength(100)]
        public string Mobile { get; set; }
        [MaxLength(100)]
        public string Fax { get; set; }
        [MaxLength(300)]
        public string Address { get; set; }
        public Guid ZoneId { get; set; }
        [MaxLength(10)]
        public string ZoneCompleteCode { get; set; }
        public bool IsLegal { get; set; }
        public bool CalculateVATForThisPerson { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public bool Enabled { get; set; }
        public DateTime? ChangeDate { get; set; }
        public bool? Sent { get; set; }
        public Guid CreditId { get; set; }
        [Ignore]
        private DynamicGroup[] _Groups { get; set; }
        [Ignore]
        public DynamicGroup[] Groups { get { if (_Groups == null || !_Groups.Any()) _Groups = App.DB.GetPartnerGroups(Id); return _Groups; } }
    }

    [Table("FinancialTransactionDocument")]
    public class FinancialTransactionDocument
    {
        public System.Guid DocumentId{get;set;}

        public System.Guid YearId{get;set;}

        public int TransactionType{get;set;}

        public System.Nullable<System.Guid> PartnerId{get;set;}

        public System.Nullable<System.Guid> PersonnelId{get;set;}

        public System.Nullable<System.Guid> CashAccountId{get;set;}

        public System.Nullable<System.Guid> BankAccountId{get;set;}

        public System.Nullable<System.Guid> CreditAccountId{get;set;}

        public System.Nullable<System.Guid> OrderId{get;set;}

        public decimal InputPrice{get;set;}

        public decimal OutputPrice{get;set;}

        public string DocumentCode{get;set;}

        public int DocumentState{get;set;}

        public System.DateTime DocumentDate{get;set;}
        public string PersianDocumentDate{get;set;}

        public System.Nullable<System.DateTime> TransactionDate{get;set;}

        public System.Nullable<System.DateTime> MaturityDate{get;set;}

        public System.Guid DocumentUserId{get;set;}

        public string DocumentDescription{get;set;}

        public string ChequeCode{get;set;}

        public string BranchName{get;set;}

        public string BranchCode{get;set;}

        public string Delivery{get;set;}

        public string Issuance{get;set;}

        public string BankTransferCode{get;set;}

        public System.Nullable<int> ChequeType{get;set;}

        public System.Nullable<System.Guid> ChequeDefinitionId{get;set;}

        public System.Nullable<System.Guid> PartnerChequeId{get;set;}

        public System.Nullable<System.Guid> RevokedChequeId{get;set;}

        public System.Nullable<int> PaymentBankType{get;set;}

        public System.Nullable<int> CashChargeType{get;set;}

        public System.Nullable<System.Guid> FCashAccountId{get;set;}

        public System.Nullable<System.Guid> ChequeBankId{get;set;}

        public System.Nullable<System.Guid> DocumentTypeId{get;set;}

        public bool IsInitialChequeOperation{get;set;}

        public System.Nullable<System.Guid> FBankAccountId{get;set;}

        public System.Nullable<System.Guid> BatchDocumentId{get;set;}

        public System.Nullable<bool> AccountingConfirmed{get;set;}

        public System.Nullable<System.Guid> SpecificCodingId{get;set;}

        public string ReceivedChequeBankAccountNumber{get;set;}

        public System.Nullable<System.Guid> TotalId{get;set;}

        public System.Nullable<System.Guid> CollectorId{get;set;}

        public System.Nullable<System.Guid> ReferenceId{get;set;}

        public System.Nullable<System.Guid> Cashier{get;set;}

        public System.Nullable<decimal> BankFee{get;set;}

        [Ignore]
        private DynamicGroup[] _Groups { get; set; }
        
    }

    [Table("DynamicGroupPartner")]
    public class DynamicGroupPartner
    {
        [PrimaryKey, AutoIncrement]
        public int AutoId { get; set; }
        public Guid GroupId { get; set; }
        public Guid PartnerId { get; set; }
    }

    [Table("StuffGroup")]
    public class StuffGroup
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        [MaxLength(150)]
        public string Name { get; set; }
        [MaxLength(50)]
        public string Code { get; set; }
        public Guid? ParentId { get; set; }
        public bool Enabled { get; set; }
        public DateTime? ServerImageDate { get; set; }
        public DateTime? SavedImageDate { get; set; }
        [MaxLength(250)]
        public string ImageFileExtension { get; set; }
    }

    [Table("StuffOrder")]
    public class StuffOrder
    {
        [PrimaryKey, AutoIncrement]
        public int AutoId { get; set; }
        public Guid StuffId { get; set; }
        public int OrderIndex { get; set; }
    }

    [Table("Stuff")]
    public class Stuff
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        public Guid GroupId { get; set; }
        [MaxLength(10)]
        public string Code { get; set; }
        [MaxLength(50)]
        public string BarCode { get; set; }
        [MaxLength(150)]
        public string Name { get; set; }
        public bool Enabled { get; set; }
        [MaxLength(250)]
        public string ReportName { get; set; }
        [MaxLength(250)]
        public string Description { get; set; }
        [MaxLength(30)]
        public string GroupCode { get; set; }
        public bool HasVAT { get; set; }
        public DateTime? ServerImageDate { get; set; }
        public DateTime? SavedImageDate { get; set; }
        [MaxLength(250)]
        public string ImageFileExtension { get; set; }
        [Ignore]
        private Package[] _Packages { get; set; }
        [Ignore]
        public Package[] Packages { get { if (_Packages == null) _Packages = App.DB.GetStuffPackages(Id); return _Packages; } }
        [Ignore]
        private StuffBasket[] _Baskets { get; set; }
        [Ignore]
        public StuffBasket[] Baskets { get { if (_Baskets == null) _Baskets = App.DB.GetStuffBaskets(Id); return _Baskets; } }
        public int SaleCoefficient { get; set; }
        public int MinForSale { get; set; }
    }

    [Table("PriceListVersion")]
    public class PriceListVersion
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        public Guid PriceListId { get; set; }
        public string PriceListName { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    [Table("PriceListStuff")]
    public class PriceListStuff
    {
        [PrimaryKey, AutoIncrement]
        public int AutoId { get; set; }
        public Guid VersionId { get; set; }
        public Guid StuffId { get; set; }
        public decimal SalePrice { get; set; }
        public decimal ConsumerPrice { get; set; }
    }

    [Table("PriceListZone")]
    public class PriceListZone
    {
        [PrimaryKey, AutoIncrement]
        public int AutoId { get; set; }
        public Guid PriceListId { get; set; }
        [MaxLength(10)]
        public string ZoneCompleteCode { get; set; }
    }

    [Table("PriceListDynamicPartnerGroup")]
    public class PriceListDynamicPartnerGroup
    {
        [PrimaryKey, AutoIncrement]
        public int AutoId { get; set; }
        public Guid PriceListId { get; set; }
        public Guid DynamicPartnerGroupId { get; set; }
    }

    [Table("Unit")]
    public class Unit
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }
    }
    
    [Table("Package")]
    public class Package
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        public Guid StuffId { get; set; }
        [Ignore]
        private Stuff _Stuff { get; set; }
        [Ignore]
        public Stuff Stuff { get { if (_Stuff == null) _Stuff = App.DB.GetStuff(StuffId); return _Stuff; } }
        public Guid UnitId { get; set; }
        [Ignore]
        private Unit _Unit { get; set; }
        [Ignore]
        public Unit Unit { get { if (_Unit == null) _Unit = App.DB.GetUnit(UnitId); return _Unit; } }
        [MaxLength(50)]
        public string Name { get; set; }
        public decimal Coefficient { get; set; }
        public bool DefaultPackage { get; set; }
        public bool Enabled { get; set; }
        public decimal PossibleQuantityCoefficient { get; set; }
    }

    [Table("StuffBasket")]
    public class StuffBasket
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }
    }

    [Table("StuffBasketStuff")]
    public class StuffBasketStuff
    {
        [PrimaryKey, AutoIncrement]
        public int AutoId { get; set; }
        public Guid BasketId { get; set; }
        public Guid StuffId { get; set; }
    }

    [Table("StuffBatchNumber")]
    public class StuffBatchNumber
    {
        [PrimaryKey]
        public Guid BatchNumberId { get; set; }
        public Guid StuffId { get; set; }
        [MaxLength(100)]
        public string BatchNumber { get; set; }
        public DateTime ExpirationDate { get; set; }
        public int ExpirationDatePresentation { get; set; }
        public bool Enabled { get; set; }
    }

    [Table("StuffSettlementDay")]
    public class StuffSettlementDay
    {
        [PrimaryKey]
        public string Key { get; set; }
        public int SettlementDay { get; set; }
    }
    
    [Table("LocationModel")]
    public class LocationModel
    {
        [PrimaryKey]
        public long Timestamp { get; set; }
        [Ignore]
        public DateTime DateTime { get { return Timestamp.ToDateForTimeStamp(); } }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? Accuracy { get; set; }
        public int DeviceState { get; set; }
        public bool SentToApplication { get; set; }
        public override string ToString()
        {
            try
            {
                var ret = Timestamp + "|";
                ret = ret + DateTime.ToShortStringForDate() + "|";
                ret = ret + (Latitude.HasValue ? Latitude.Value.ToString() : "") + "|";
                ret = ret + (Longitude.HasValue ? Longitude.Value.ToString() : "") + "|";
                ret = ret + (Accuracy.HasValue ? Accuracy.Value.ToString() : "") + "|";
                ret = ret + DeviceState + "|";
                ret = ret + SentToApplication;
                return ret;
            }
            catch (Exception err)
            {
                return null;
            }
        }

        public static LocationModel FromString(string Str)
        {
            var sections = Str.Split('|');
            var location = new LocationModel()
            {
                Timestamp = Convert.ToInt64(sections[0]),
                Latitude = sections[2] != "" ? Convert.ToDouble(sections[2]) : new double?(),
                Longitude = sections[3] != "" ? Convert.ToDouble(sections[3]) : new double?(),
                Accuracy = sections[4] != "" ? Convert.ToDouble(sections[4]) : new double?(),
                DeviceState = Convert.ToInt32(sections[5]),
                SentToApplication = Convert.ToBoolean(sections[6])
            };
            return location;
        }
    }

    [Table("SettlementType")]
    public class SettlementType
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }
        public int Day { get; set; }
        public bool Enabled { get; set; }
    }
    
    [Table("Warehouse")]
    public class Warehouse
    {
        [PrimaryKey]
        public Guid WarehouseId { get; set; }
        public string WarehouseName { get; set; }
    }

    [Table("Stock")]
    public class Stock
    {
        [PrimaryKey, AutoIncrement]
        public int AutoId { get; set; }
        public Guid? WarehouseId { get; set; }
        public Guid StuffId { get; set; }
        public Guid? BatchNumberId { get; set; }
        public decimal Real { get; set; }
        public decimal Fake { get; set; }
    }

    [Table("VisitProgramPartner")]
    public class VisitProgramPartner
    {
        [PrimaryKey, AutoIncrement]
        public int AutoId { get; set; }
        public Guid PartnerId { get; set; }
        public DateTime Date { get; set; }
        public int Indx { get; set; }
    }

    [Table("NotOrderReason")]
    public class NotOrderReason
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        [MaxLength(1000)]
        public string Name { get; set; }
        public DateTime BDate { get; set; }
        public DateTime EDate { get; set; }
        public bool Enabled { get; set; }
    }

    [Table("FailedVisit")]
    public class FailedVisit
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        public Guid PartnerId { get; set; }
        [Ignore]
        private Partner _Partner { get; set; }
        [Ignore]
        public Partner Partner { get { if (_Partner == null) _Partner = App.DB.GetPartner(PartnerId); return _Partner; } }
        public DateTime VisitTime { get; set; }
        public decimal? GeoLocationLat { get; set; }
        public decimal? GeoLocationLong { get; set; }
        public decimal? GeoLocationAccuracy { get; set; }
        public Guid ReasonId { get; set; }
        public string Description { get; set; }
        public bool Sent { get; set; }
    }

    [Table("DiscountRule")]
    public class DiscountRule
    {
        [PrimaryKey]
        public Guid RuleId { get; set; }
        public int Prority { get; set; }
        [Ignore]
        public long BeginDateMiliseconds { get; set; }
        [Ignore]
        public long EndDateMiliseconds { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        [MaxLength(256)]
        public string Description { get; set; }
        public int DiscountBasedOn { get; set; }
        public int DiscountWay { get; set; }
        public bool ConditionOnPartner { get; set; }
        public bool ConditionOnPartnerGroup { get; set; }
        public bool ConditionOnZone { get; set; }
        public bool ConditionOnVisitor { get; set; }
        public bool ConditionOnStuff { get; set; }
        public bool ConditionOnStuffGroup { get; set; }
        public bool ConditionOnStuffBasket { get; set; }
        public bool ConditionOnSettlementType { get; set; }
        public bool ConditionOnSettlementDay { get; set; }
        public bool AggregateOtherDiscounts_Percent { get; set; }
        public bool AggregateOtherDiscounts_SpecificMoney { get; set; }
        public bool AggregateOtherDiscounts_FreeProduct { get; set; }
        public bool AggregateOtherDiscounts_CashSettlement { get; set; }
        public bool ConsiderEachStuffSeperately { get; set; }
        public bool ConsiderEachStuffGroupSeperately { get; set; }
        public bool ConsiderEachStuffBasketSeperately { get; set; }
        public bool AllowOptionalDiscounts { get; set; }
        public bool ShowDiscountInUnitFee { get; set; }
        public bool? CombineStuffBaskets_SpecifyEachSoledBasket { get; set; }
        public bool? CombineStuffBaskets_SpecifyEachBasketWhichGetDiscount { get; set; }
    }
    [Table("SaleDiscountRuleStuffBasket")]
    public class SaleDiscountRuleStuffBasket
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        public Guid RuleId { get; set; }
        public string Title { get; set; }
        public bool Saled { get; set; }
        public int RowIndex { get; set; }
    }
    [Table("SaleDiscountRuleStuffBasketDetail")]
    public class SaleDiscountRuleStuffBasketDetail
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        public Guid BasketId { get; set; }
        public Guid? StuffId { get; set; }
        public Guid? StuffGroupId { get; set; }
        public int RowIndex { get; set; }
    }
    [Table("DiscountRuleCondition")]
    public class DiscountRuleCondition
    {
        [PrimaryKey, AutoIncrement]
        public int AutoId { get; set; }
        public Guid RuleId { get; set; }
        public Guid ConditionParameter { get; set; }
        public int ConditionType { get; set; }
        [MaxLength(100)]
        public string ConditionParameter2 { get; set; }
        [MaxLength(100)]
        public string ConditionParameter3 { get; set; }
    }
    [Table("DiscountRuleStep")]
    public class DiscountRuleStep
    {
        [PrimaryKey]
        public Guid StepId { get; set; }
        public Guid RuleId { get; set; }
        public decimal? TotalPrice { get; set; }
        public Guid? QuantityStep_SpecificUnitId { get; set; }
        public int? QuantityStep_Quantity { get; set; }
        public decimal? DiscountPercent { get; set; }
        public decimal? DiscountSpecificMoney { get; set; }
	    public bool? AcsendingCalculation { get; set; }
	    public int? CashSettlementDay { get; set; }
	    public bool? FreeProduct_BoughtStuff { get; set; }
	    public Guid? FreeProduct_SpecificStuffId { get; set; }
	    public Guid? FreeProduct_UnitId { get; set; }
	    public int? FreeProduct_Quantity { get; set; }
	    public int StepIndex { get; set; }
	    public bool? ContinueToCalculateOnThisRatio { get; set; }
	    public int? RatioCalculationMethod { get; set; }
	    public int? ArticleRowCount { get; set; }
    }
    [Table("SaleDiscountRuleStepStuffBasket")]
    public class SaleDiscountRuleStepStuffBasket
    {
        [PrimaryKey, AutoIncrement]
        public int AutoId { get; set; }
        public Guid StepId { get; set; }
        public Guid BasketId { get; set; }
    }
    [Table("Access")]
    public class Access
    {
        [PrimaryKey, MaxLength(100)]
        public string AccessName { get; set; }
        [MaxLength(100)]
        public string Parameter { get; set; }
    }

    [Table("SaleOrder")]
    public class SaleOrder
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        public int? PreCode { get; set; }
        public Guid PartnerId { get; set; }
        [Ignore]
        private Partner _Partner { get; set; }
        [Ignore]
        public Partner Partner { get { if (_Partner == null) _Partner = App.DB.GetPartner(PartnerId); return _Partner; } }
        public DateTime InsertDateTime { get; set; }
        [MaxLength(1000)]
        public string Description { get; set; }
        public decimal DiscountPercent { get; set; }
        [Ignore]
        public decimal DiscountAmount { get { return Math.Round((StuffsPriceSum - StuffsRowDiscountSum) * DiscountPercent / 100); } }
        public decimal AddedDiscountPercent { get; set; }
        [Ignore]
        public decimal AddedDiscountAmount { get { return Math.Round((StuffsPriceSum - StuffsRowDiscountSum) * AddedDiscountPercent / 100); } }
        public decimal CashPrise { get; set; }
        public Guid SettlementTypeId { get; set; }
        [Ignore]
        private SettlementType _SettlementType { get; set; }
        [Ignore]
        public SettlementType SettlementType { get { if (_SettlementType == null) _SettlementType = App.DB.GetSettlementType(SettlementTypeId); return _SettlementType; } }
        public int SettlementDay { get; set; }
        public decimal? GeoLocation_Latitude  { get; set; }
        public decimal? GeoLocation_Longitude { get; set; }
        public decimal? GeoLocation_Accuracy { get; set; }
        [Ignore]
        private SaleOrderStuff[] _SaleOrderStuffs { get; set; }
        [Ignore]
        public SaleOrderStuff[] SaleOrderStuffs { get { if (_SaleOrderStuffs == null) _SaleOrderStuffs = App.DB.GetSaleOrderStuffs(Id);  return _SaleOrderStuffs; } set { _SaleOrderStuffs = value; } }
        [Ignore]
        private CashDiscount[] _CashDiscounts { get; set; }
        [Ignore]
        public CashDiscount[] CashDiscounts { get { if (_CashDiscounts == null) _CashDiscounts = App.DB.GetCashDiscounts(Id); return _CashDiscounts; } set { _CashDiscounts = value; } }
        [Ignore]
        public decimal StuffsQuantitySum { get { return SaleOrderStuffs.Any() ? SaleOrderStuffs.Sum(a => a.Quantity) : 0; } }
        [Ignore]
        public decimal StuffsStuffQuantitySum { get { return SaleOrderStuffs.Any() ? SaleOrderStuffs.Sum(a => a.StuffQuantity) : 0; } }
        [Ignore]
        public decimal StuffsPriceSum { get { return SaleOrderStuffs.Any() ? SaleOrderStuffs.Sum(a => a.SalePriceQuantity) : 0; } }
        [Ignore]
        public decimal StuffsRowDiscountSum { get { return SaleOrderStuffs.Any() ? SaleOrderStuffs.Sum(a => a.DiscountAmount + a.CashDiscountAmount) : 0; } }
        [Ignore]
        public decimal StuffsVATSum { get { return SaleOrderStuffs.Any() ? SaleOrderStuffs.Sum(a => a.VATAmount) : 0; } }
        [Ignore]
        public decimal OrderFinalPrice { get { return StuffsPriceSum - StuffsRowDiscountSum - DiscountAmount - CashPrise + StuffsVATSum; } }
        public Guid? WarehouseId { get; set; }
    }

    [Table("SaleOrderStuff")]
    public class SaleOrderStuff
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        [Ignore]
        private SaleOrder _SaleOrder { get; set; }
        [Ignore]
        public SaleOrder SaleOrder { get { if (_SaleOrder == null) _SaleOrder = App.DB.GetSaleOrder(OrderId); return _SaleOrder; } set { _SaleOrder = value; } }
        public int ArticleIndex { get; set; }
        public Guid PackageId { get; set; }
        [Ignore]
        public Package _Package { get; set; }
        [Ignore]
        public Package Package { get { if (_Package == null) _Package = App.DB.GetPackage(PackageId); return _Package; } }
        public decimal Quantity { get; set; }
        [Ignore]
        public decimal StuffQuantity { get { return Quantity * Package.Coefficient; } }
        public decimal SalePrice { get; set; }
        [Ignore]
        public decimal SalePriceQuantity { get { return Quantity * SalePrice; } }
        public decimal DiscountPercent { get; set; }
        public decimal CashDiscountPercent { get; set; }
        [Ignore]
        public decimal DiscountAmount { get { return Math.Round(SalePriceQuantity * DiscountPercent / 100); } }
        public decimal CashDiscountAmount { get { return Math.Round((SalePriceQuantity - DiscountAmount) * CashDiscountPercent / 100); } }
        public decimal AddedDiscountPercent { get; set; }
        [Ignore]
        public decimal AddedDiscountAmount { get { return Math.Round(SalePriceQuantity * AddedDiscountPercent / 100); } }
        public decimal ProporatedDiscount { get; set; }
        public decimal VATPercent { get; set; }
        [Ignore]
        public decimal VATAmount { get { return Math.Round((SalePriceQuantity - DiscountAmount - CashDiscountAmount - ProporatedDiscount) * VATPercent / 100); } }
        public bool FreeProduct { get; set; }
        public decimal FreeProductAddedQuantity { get; set; }
        public decimal? FreeProduct_UnitPrice { get; set; }
        public Guid? BatchNumberId { get; set; }
        [Ignore]
        private StuffBatchNumber _BatchNumber { get; set; }
        [Ignore]
        public StuffBatchNumber BatchNumber { get { if (_BatchNumber == null && BatchNumberId.HasValue) _BatchNumber = App.DB.GetBatchNumber(BatchNumberId.Value); return _BatchNumber; } }
        [Ignore]
        public decimal FinalPrice { get { return SalePriceQuantity - DiscountAmount - CashDiscountAmount - ProporatedDiscount + VATAmount; } }
        public int? StuffSettlementDay { get; set; }
    }
    
    [Table("CashDiscount")]
    public class CashDiscount
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        [Ignore]
        private SaleOrder _SaleOrder { get; set; }
        [Ignore]
        public SaleOrder SaleOrder { get { if (_SaleOrder == null) _SaleOrder = App.DB.GetSaleOrder(OrderId); return _SaleOrder; } set { _SaleOrder = value; } }
        public int Day { get; set; }
        public decimal Percent { get; set; }
        [Ignore]
        public decimal DiscountAmount { get { return Math.Round(Percent * (SaleOrder.StuffsPriceSum - SaleOrder.StuffsRowDiscountSum - SaleOrder.DiscountAmount) / 100); } }
        [Ignore]
        public decimal OrderFinalPrice { get { return SaleOrder.SaleOrderStuffs.Any() ? SaleOrder.SaleOrderStuffs.Sum(a => Math.Round(Math.Round((a.FinalPrice - a.VATAmount) * (100 - Percent) / 100) * (100 + a.VATPercent) / 100)) : 0; } }
    }
    
    [Table("DeviceSettingChange")]
    public class DeviceSettingChange
    {
        [PrimaryKey]
        public DateTime DateTime { get; set; }
        public int Type { get; set; }
        public bool Sent { get; set; }
    }

    [Table("Cash")]  //صندوق
    public class Cash
    {
        [PrimaryKey]
        public Guid EntityId { get; set; }
        public bool IsEnable { get; set; }
        public string EntityCode { get; set; }
        public string EntityName { get; set; }
    }

    [Table("Bank")]  
    public class Bank
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }

    [Table("BankAccount")]
    public class BankAccount
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
