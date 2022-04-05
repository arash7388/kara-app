using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using Kara.Models;
using System.Reflection;
using System.Linq.Expressions;
using System.ComponentModel;
using Xamarin.Forms;
using System.IO;
using Xamarin.Forms.Maps;
using System.Collections.ObjectModel;

namespace Kara.Assets
{
    public class DBRepository
    {
        public SQLiteConnection conn;

        public DBRepository(string dbPath)
        {
            try
            {
                conn = new SQLiteConnection(dbPath);
                //conn.CreateTable(typeof(TempTable));
            }
            catch (Exception err)
            {
                //App.File.Copy(App.DBFileName.Replace("karadb.db3", "karadbcopy.db3"), App.DBFileName);
                //conn = new SQLiteConnection(dbPath);
                //conn.CreateTable(typeof(TempTable));
            }
        }

        public async Task<ResultSuccess> CreateTablesAsync()
        {
            return await Task.Run(() =>
            {
                InitTransaction();
                try
                {
                    foreach (var table in ModelsInterface.AllTables)
                        conn.CreateTable(table.Key);

                    CommitTransaction();
                    return new ResultSuccess(true, "");
                }
                catch (Exception err)
                {
                    RollbackTransaction();
                    return new ResultSuccess(false, err.ProperMessage());
                }
            });
        }

        public async Task<ResultSuccess> CleanDataBaseAsync()
        {
            return await Task.Run(() =>
            {
                InitTransaction();
                try
                {
                    foreach (var table in ModelsInterface.AllTables)
                        conn.Execute("Delete from " + table.Value);

                    CommitTransaction();
                    return new ResultSuccess(true, "");
                }
                catch (Exception err)
                {
                    RollbackTransaction();
                    return new ResultSuccess(false, err.ProperMessage());
                }
            });
        }

        public ResultSuccess InitTransaction()
        {
            try
            {
                conn.BeginTransaction();
                return new ResultSuccess();
            }
            catch (Exception err)
            {
                return new ResultSuccess(false, err.ProperMessage());
            }
        }

        public ResultSuccess RollbackTransaction()
        {
            try
            {
                conn.Rollback();
                return new ResultSuccess();
            }
            catch (Exception err)
            {
                return new ResultSuccess(false, err.ProperMessage());
            }
        }

        public ResultSuccess CommitTransaction()
        {
            try
            {
                conn.Commit();
                return new ResultSuccess();
            }
            catch (Exception err)
            {
                return new ResultSuccess(false, err.ProperMessage());
            }
        }

        public async Task<ResultSuccess> InsertOrUpdateAllRecordsAsync<T>(IEnumerable<T> Records)
        {
            return await Task.Run(() =>
            {
                var IsInTransaction = conn.IsInTransaction;
                if (!IsInTransaction)
                    InitTransaction();
                try
                {
                    Type RecordType = typeof(T);
                    foreach (var Record in Records)
                        conn.InsertOrReplace(Record, RecordType);

                    if (!IsInTransaction)
                        CommitTransaction();
                    return new ResultSuccess(true, "");
                }
                catch (Exception err)
                {
                    if (!IsInTransaction)
                        RollbackTransaction();
                    return new ResultSuccess(false, err.ProperMessage());
                }
            });
        }

        public async Task<ResultSuccess> InsertAllRecordsAsync<T>(IEnumerable<T> Records)
        {
            return await Task.Run(() =>
            {
                var IsInTransaction = conn.IsInTransaction;
                if (!IsInTransaction)
                    InitTransaction();
                try
                {
                    Type RecordType = typeof(T);
                    foreach (var Record in Records)
                        conn.Insert(Record, RecordType);

                    if (!IsInTransaction)
                        CommitTransaction();
                    return new ResultSuccess(true, "");
                }
                catch (Exception err)
                {
                    if (!IsInTransaction)
                        RollbackTransaction();
                    return new ResultSuccess(false, err.ProperMessage());
                }
            });
        }

        public async Task<ResultSuccess> InsertRecordAsync<T>(T Record)
        {
            return await Task.Run(() =>
            {
                try
                {
                    conn.Insert(Record, typeof(T));

                    return new ResultSuccess(true, "");
                }
                catch (Exception err)
                {
                    return new ResultSuccess(false, err.ProperMessage());
                }
            });
        }

        public async Task<ResultSuccess> DeleteAllRecordsAsync<T>()
        {
            return await Task.Run(() =>
            {
                try
                {
                    conn.DeleteAll<T>();

                    return new ResultSuccess(true, "");
                }
                catch (Exception err)
                {
                    return new ResultSuccess(false, err.ProperMessage());
                }
            });
        }

        public async Task<ResultSuccess> SetEnabledForAllRecordsAsync<T>(bool Enabled)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var tsql = "Update " + typeof(T).Name + " Set Enabled = " + (Enabled ? 1 : 0);
                    conn.Execute(tsql);

                    return new ResultSuccess(true, "");
                }
                catch (Exception err)
                {
                    return new ResultSuccess(false, err.ProperMessage());
                }
            });
        }

        public async Task<ResultSuccess> InsertOrUpdateRecordAsync<T>(T Record)
        {
            return await Task.Run(() =>
            {
                try
                {
                    conn.InsertOrReplace(Record, typeof(T));

                    return new ResultSuccess(true, "");
                }
                catch (Exception err)
                {
                    return new ResultSuccess(false, err.ProperMessage());
                }
            });
        }

        public async Task<ResultSuccess<Partner>> GetPartnerAsync(Guid? Id, string Code)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var Partner = conn.Table<Partner>().SingleOrDefault(a => a.Id == Id || a.Code == Code);

                    if (Partner == null)
                        return new ResultSuccess<Partner>(false, "طرف حساب مورد نظر یافت نشد.");

                    return new ResultSuccess<Partner>(true, "", Partner);
                }
                catch (Exception err)
                {
                    return new ResultSuccess<Partner>(false, err.ProperMessage());
                }
            });
        }

        public async Task<ResultSuccess<Partner>> GetPartnerAsync(Guid? Id)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var Partner = conn.Table<Partner>().SingleOrDefault(a => a.Id == Id);

                    if (Partner == null)
                        return new ResultSuccess<Partner>(false, "طرف حساب مورد نظر یافت نشد.");

                    return new ResultSuccess<Partner>(true, "", Partner);
                }
                catch (Exception err)
                {
                    return new ResultSuccess<Partner>(false, err.ProperMessage());
                }
            });
        }

        public async Task<ResultSuccess<List<Partner>>> GetPartnersAsync()
        {
            return await Task.Run(() =>
            {
                try
                {
                    var Partners = conn.Table<Partner>().ToList();

                    return new ResultSuccess<List<Partner>>(true, "", Partners);
                }
                catch (Exception err)
                {
                    return new ResultSuccess<List<Partner>>(false, err.ProperMessage());
                }
            });
        }

        public class PartnerListModel : INotifyPropertyChanged
        {
            private bool _Selected;
            public Guid Id { get; set; }
            public string Code { get; set; }
            public string Name { get; set; }
            public string LegalName { get; set; }
            public string Group { get; set; }
            public string Zone { get; set; }
            public string Address { get; set; }
            public string Phone { get; set; }
            public bool HasOrder { get; set; }
            public bool HasFailedVisit { get; set; }
            public Partner PartnerData { get; set; }
            public int? DistanceFromMe { get; set; }
            public bool Sent { get; set; }
            public bool Selected { get { return _Selected; } set { _Selected = value; OnPropertyChanged("Selected"); OnPropertyChanged("RowColor"); if (App.InsertedInformations_Partners != null) App.InsertedInformations_Partners.RefreshToolbarItems(); } }
            public string RowColor
            {
                get
                {
                    return ForChangedPartnersList ? Selected ? "#F5F5A4" : Sent ? "#B7E5BF" : "#DCE6FA" :
Selected ? "#A4DEF5" : HasOrder ? "#B7E5BF" : HasFailedVisit ? "#E5B7BF" : "#DCE6FA";
                }
            }
            public static bool Multiselection { get; set; }
            public bool CanBeSelectedInMultiselection { get { return Sent ? false : true; } }
            public GridLength CheckBoxColumnWidth { get { return Multiselection ? 60 : 0; } }

            public bool ForChangedPartnersList { get; set; }

            public event PropertyChangedEventHandler PropertyChanged;
            public void OnPropertyChanged(string propertyName)
            {
                var handler = PropertyChanged;
                if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public class PartnerListModelEqualityComparer : IEqualityComparer<PartnerListModel>
        {
            public bool Equals(PartnerListModel partner1, PartnerListModel partner2)
            {
                return partner1.Code == partner2.Code;
            }
            public int GetHashCode(PartnerListModel partner)
            {
                return partner.Code.GetHashCode();
            }
        }
        public async Task<ResultSuccess<List<PartnerListModel>>> GetDayPartnersListAsync(int day, bool IncludeVisited, int? NearbyCustomers_Distance, string Filter)
        {
            Filter = Filter.ToLatinDigits();
            return await Task.Run(() =>
            {
                try
                {
                    var _day = DateTime.Now.AddHours((double)App.ShowSaleVisitProgramPartnersToVisitorHourShift.Value).Date.AddDays(day);
                    var _nextday = _day.AddDays(1);
                    var DayPartners = conn.Table<VisitProgramPartner>().Where(a => a.Date == _day).OrderBy(a => a.Indx).ToList();
                    var AllPartners = conn.Table<Partner>().Where(a => a.Enabled).ToList();

                    AllPartners = FilterPartners(AllPartners, Filter);

                    var Partners = DayPartners.Select(a => AllPartners.SingleOrDefault(b => a.PartnerId == b.Id)).Where(a => a != null).ToList();

                    var Routes = conn.Table<Zone>().Where(a => a.ZoneLevel == 3).ToList();
                    var Groups = (from Group in conn.Table<DynamicGroup>().ToList()
                                  join PartnerGroup in conn.Table<DynamicGroupPartner>().ToList() on Group.Id equals PartnerGroup.GroupId
                                  select new { PartnerId = PartnerGroup.PartnerId, GroupName = Group.Name })
                                 .GroupBy(a => a.PartnerId)
                                 .Select(a => new
                                 {
                                     PartnerId = a.Key,
                                     GroupNames = a.Select(b => b.GroupName).Aggregate((sum, x) => sum + ", " + x)
                                 }).ToList();
                    var Orders = conn.Table<SaleOrder>().Where(a => a.InsertDateTime >= _day && a.InsertDateTime < _nextday).ToList();
                    var FailedVisits = conn.Table<FailedVisit>().Where(a => a.VisitTime >= _day && a.VisitTime < _nextday).ToList();

                    Position CurrentLocation = new Position(0, 0);
                    if (App.LastLocation != null && App.LastLocation.Latitude.HasValue && App.LastLocation.Longitude.HasValue && App.LastLocation.DateTime > DateTime.Now.AddMinutes(-1))
                        CurrentLocation = new Position(App.LastLocation.Latitude.Value, App.LastLocation.Longitude.Value);

                    var PartnersWithData = (from Partner in Partners
                                            join Route in Routes on Partner.ZoneId equals Route.Id
                                            join Group in Groups on Partner.Id equals Group.PartnerId into PartnerGroupNames
                                            from PartnerGroupName in PartnerGroupNames.DefaultIfEmpty()
                                            join Order in Orders on Partner.Id equals Order.PartnerId into PartnerOrders
                                            from PartnerOrder in PartnerOrders.DefaultIfEmpty()
                                            join FailedVisit in FailedVisits on Partner.Id equals FailedVisit.PartnerId into PartnerFailedVisits
                                            from PartnerFailedVisit in PartnerFailedVisits.DefaultIfEmpty()
                                            select new PartnerListModel()
                                            {
                                                Id = Partner.Id,
                                                Code = Partner.Code,
                                                Name = Partner.Name,
                                                LegalName = Partner.LegalName,
                                                Group = PartnerGroupName != null ? PartnerGroupName.GroupNames : "بدون گروه",
                                                Zone = Route.CompleteName,
                                                Address = Partner.Address,
                                                Phone = new string[] { Partner.Phone1, Partner.Phone2, Partner.Mobile }.Any(ph => !string.IsNullOrEmpty(ph)) ? new string[] { Partner.Phone1, Partner.Phone2, Partner.Mobile }.Where(ph => !string.IsNullOrEmpty(ph)).Aggregate((sum, x) => sum + "\n" + x) : "",
                                                HasOrder = PartnerOrder != null,
                                                HasFailedVisit = PartnerFailedVisit != null,
                                                PartnerData = Partner,
                                                DistanceFromMe = CurrentLocation.Latitude == 0 ? new Nullable<int>() : !Partner.Latitude.HasValue || !Partner.Longitude.HasValue ? new Nullable<int>() :
                                                    (int)CurrentLocation.meterDistanceBetweenPoints(new Position(Partner.Latitude.Value, Partner.Longitude.Value))
                                            }).Distinct(new PartnerListModelEqualityComparer()).ToList();

                    if (NearbyCustomers_Distance.HasValue)
                    {
                        PartnersWithData = PartnersWithData
                            .Where(a => a.DistanceFromMe.HasValue)
                            .Where(a => a.DistanceFromMe <= NearbyCustomers_Distance.Value)
                            .OrderBy(a => a.DistanceFromMe)
                            .ToList();
                    }

                    var FinalList = PartnersWithData.Where(a => IncludeVisited || (!a.HasOrder && !a.HasFailedVisit)).ToList();
                    return new ResultSuccess<List<PartnerListModel>>(true, "", FinalList);
                }
                catch (Exception err)
                {
                    return new ResultSuccess<List<PartnerListModel>>(false, err.ProperMessage());
                }
            });
        }

        public async Task<ResultSuccess<List<PartnerListModel>>> GetAllPartnersListAsync(bool JustChanged, bool? JustToday, bool? JustLocal, int? NearbyCustomers_Distance, string Filter)
        {
            Filter = Filter.ToLatinDigits();
            return await Task.Run(() =>
            {
                try
                {
                    var Today = DateTime.Now.Date;
                    var Tomorrow = Today.AddDays(1);
                    var AllPartners = conn.Table<Partner>().Where(a => a.Enabled).ToList();

                    if (JustChanged)
                    {
                        AllPartners = AllPartners.Where(a => a.ChangeDate.HasValue).ToList();
                        if (JustToday.Value)
                            AllPartners = AllPartners.Where(a => a.ChangeDate.Value >= Today && a.ChangeDate.Value < Tomorrow).ToList();

                        if (JustLocal.Value)
                            AllPartners = AllPartners.Where(a => !a.Sent.GetValueOrDefault(false)).ToList();
                    }

                    AllPartners = FilterPartners(AllPartners, Filter);

                    var Cities = conn.Table<Zone>().Where(a => a.ZoneLevel == 1).ToList();
                    var Zones = conn.Table<Zone>().Where(a => a.ZoneLevel == 2).ToList();
                    var Routes = conn.Table<Zone>().Where(a => a.ZoneLevel == 3).ToList();
                    Routes.AddRange(Cities.Where(a => !Zones.Any(b => b.ParentId == a.Id)));
                    Routes.AddRange(Zones.Where(a => !Routes.Any(b => b.ParentId == a.Id)));

                    var Groups = (from Group in conn.Table<DynamicGroup>().ToList()
                                  join PartnerGroup in conn.Table<DynamicGroupPartner>().ToList() on Group.Id equals PartnerGroup.GroupId
                                  select new { PartnerId = PartnerGroup.PartnerId, GroupName = Group.Name })
                                 .GroupBy(a => a.PartnerId)
                                 .Select(a => new
                                 {
                                     PartnerId = a.Key,
                                     GroupNames = a.Select(b => b.GroupName).Aggregate((sum, x) => sum + ", " + x)
                                 }).ToList();
                    var Orders = conn.Table<SaleOrder>().Where(a => a.InsertDateTime >= Today && a.InsertDateTime < Tomorrow).ToList();
                    var FailedVisits = conn.Table<FailedVisit>().Where(a => a.VisitTime >= Today && a.VisitTime < Tomorrow).ToList();

                    Position CurrentLocation = new Position(0, 0);
                    if (App.LastLocation != null && App.LastLocation.Latitude.HasValue && App.LastLocation.Longitude.HasValue && App.LastLocation.DateTime > DateTime.Now.AddMinutes(-1))
                        CurrentLocation = new Position(App.LastLocation.Latitude.Value, App.LastLocation.Longitude.Value);

                    var PartnersWithData = (from Partner in AllPartners
                                                //join Route in Routes on Partner.ZoneId equals Route.Id
                                            join Route in Routes on Partner.ZoneId equals Route.Id into zones
                                            from z in zones.DefaultIfEmpty()
                                            join Group in Groups on Partner.Id equals Group.PartnerId into PartnerGroupNames
                                            from PartnerGroupName in PartnerGroupNames.DefaultIfEmpty()
                                            join Order in Orders on Partner.Id equals Order.PartnerId into PartnerOrders
                                            from PartnerOrder in PartnerOrders.DefaultIfEmpty()
                                            join FailedVisit in FailedVisits on Partner.Id equals FailedVisit.PartnerId into PartnerFailedVisits
                                            from PartnerFailedVisit in PartnerFailedVisits.DefaultIfEmpty()
                                            select new PartnerListModel()
                                            {
                                                Id = Partner.Id,
                                                Code = Partner.Code,
                                                Name = Partner.Name,
                                                LegalName = Partner.LegalName,
                                                Group = PartnerGroupName != null ? PartnerGroupName.GroupNames : "بدون گروه",
                                                //Zone = Route.CompleteName,
                                                Zone = z?.CompleteName,
                                                Address = Partner.Address,
                                                Phone = new string[] { Partner.Phone1, Partner.Phone2, Partner.Mobile }.Any(ph => !string.IsNullOrEmpty(ph)) ? new string[] { Partner.Phone1, Partner.Phone2, Partner.Mobile }.Where(ph => !string.IsNullOrEmpty(ph)).Aggregate((sum, x) => sum + "\n" + x) : "",
                                                HasOrder = PartnerOrder != null,
                                                HasFailedVisit = PartnerFailedVisit != null,
                                                Sent = Partner.Sent.GetValueOrDefault(false),
                                                ForChangedPartnersList = JustChanged,
                                                PartnerData = Partner,
                                                DistanceFromMe = CurrentLocation.Latitude == 0 ? new Nullable<int>() : !Partner.Latitude.HasValue || !Partner.Longitude.HasValue ? new Nullable<int>() :
                                                    (int)CurrentLocation.meterDistanceBetweenPoints(new Position(Partner.Latitude.Value, Partner.Longitude.Value))
                                            }).Distinct(new PartnerListModelEqualityComparer()).ToList();

                    if (NearbyCustomers_Distance.HasValue)
                    {
                        PartnersWithData = PartnersWithData
                            .Where(a => a.DistanceFromMe.HasValue)
                            .Where(a => a.DistanceFromMe <= NearbyCustomers_Distance.Value)
                            .OrderBy(a => a.DistanceFromMe)
                            .ToList();
                    }

                    return new ResultSuccess<List<PartnerListModel>>(true, "", PartnersWithData);
                }
                catch (Exception err)
                {
                    return new ResultSuccess<List<PartnerListModel>>(false, err.ProperMessage());
                }
            });
        }

        private List<Partner> FilterPartners(List<Partner> AllPartners, string Filter)
        {
            Filter = Filter.ToLatinDigits();
            if (!string.IsNullOrWhiteSpace(Filter))
            {
                var TotalPoints = AllPartners.Select(a => new KeyValuePair<Partner, int>(a, 0)).ToList();
                var FilterSegments = Filter.ToLower().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var FilterSegment in FilterSegments)
                {
                    var NewPoints = AllPartners.Select(a => new KeyValuePair<Partner, int>(a, FilterSegment.Length * (
                        (a.Name.ToLower() == FilterSegment ? 3 : a.Name.ToLower().StartsWith(FilterSegment) ? 2 : a.Name.ToLower().Contains(FilterSegment) ? 1 : 0) +
                        (a.LegalName.ToLower() == FilterSegment ? 3 : a.LegalName.ToLower().StartsWith(FilterSegment) ? 2 : a.LegalName.ToLower().Contains(FilterSegment) ? 1 : 0) +
                        (a.Phone1 == FilterSegment ? 10 : a.Phone1.StartsWith(FilterSegment) ? 2 : a.Phone1.Contains(FilterSegment) ? 1 : 0) +
                        (a.Phone2 == FilterSegment ? 10 : a.Phone2.StartsWith(FilterSegment) ? 2 : a.Phone2.Contains(FilterSegment) ? 1 : 0) +
                        (a.Mobile == FilterSegment ? 10 : a.Mobile.StartsWith(FilterSegment) ? 2 : a.Mobile.Contains(FilterSegment) ? 1 : 0) +
                        (a.Address.ToLower() == FilterSegment ? 10 : a.Address.ToLower().StartsWith(FilterSegment) ? 2 : a.Address.ToLower().Contains(FilterSegment) ? 1 : 0) +
                        (a.Code == FilterSegment ? 10 : a.Code.StartsWith(FilterSegment) ? 2 : a.Code.Contains(FilterSegment) ? 1 : 0)
                    ))).ToList();

                    TotalPoints = (
                        from TotalPoint in TotalPoints
                        join NewPoint in NewPoints on TotalPoint.Key.Id equals NewPoint.Key.Id
                        select new KeyValuePair<Partner, int>(TotalPoint.Key, TotalPoint.Value + NewPoint.Value)
                    ).ToList();
                }
                AllPartners = (
                    from TotalPoint in TotalPoints
                    join Partner in AllPartners on TotalPoint.Key.Id equals Partner.Id
                    where TotalPoint.Value != 0
                    orderby TotalPoint.Value descending, Partner.Code
                    select Partner
                ).ToList();
            }
            else
                AllPartners = AllPartners.OrderBy(a => a.Code).ToList();

            return AllPartners;
        }

        public static Dictionary<string, int> _StuffsSettlementDays;
        public async Task<ResultSuccess<Dictionary<string, int>>> GetStuffsSettlementDaysAsync()
        {
            return await Task.Run(() =>
            {
                try
                {
                    if (_StuffsSettlementDays == null)
                        _StuffsSettlementDays = conn.Table<StuffSettlementDay>().ToList().ToDictionary(a => a.Key.ToLower(), a => a.SettlementDay);

                    return new ResultSuccess<Dictionary<string, int>>(true, "", _StuffsSettlementDays);
                }
                catch (Exception err)
                {
                    return new ResultSuccess<Dictionary<string, int>>(false, err.ProperMessage());
                }
            });
        }

        public async Task<ResultSuccess<List<Stuff>>> GetStuffsAsync()
        {
            return await Task.Run(() =>
            {
                try
                {
                    var Stuffs = conn.Table<Stuff>().Where(a => a.Enabled).ToList();

                    return new ResultSuccess<List<Stuff>>(true, "", Stuffs);
                }
                catch (Exception err)
                {
                    return new ResultSuccess<List<Stuff>>(false, err.ProperMessage());
                }
            });
        }

        public async Task<ResultSuccess<List<StuffGroup>>> GetStuffGroupsAsync()
        {
            return await Task.Run(() =>
            {
                try
                {
                    var StuffGroups = conn.Table<StuffGroup>().Where(a => a.Enabled).ToList();

                    return new ResultSuccess<List<StuffGroup>>(true, "", StuffGroups);
                }
                catch (Exception err)
                {
                    return new ResultSuccess<List<StuffGroup>>(false, err.ProperMessage());
                }
            });
        }

        public async Task<ResultSuccess<List<Zone>>> GetZonesAsync()
        {
            return await Task.Run(() =>
            {
                try
                {
                    var Zones = conn.Table<Zone>().ToList();

                    return new ResultSuccess<List<Zone>>(true, "", Zones);
                }
                catch (Exception err)
                {
                    return new ResultSuccess<List<Zone>>(false, err.ProperMessage());
                }
            });
        }

        public async Task<ResultSuccess<List<Cash>>> GetCashesAsync()
        {
            return await Task.Run(() =>
            {
                try
                {
                    var Cashes = conn.Table<Cash>().ToList();

                    return new ResultSuccess<List<Cash>>(true, "", Cashes);
                }
                catch (Exception err)
                {
                    return new ResultSuccess<List<Cash>>(false, err.ProperMessage());
                }
            });
        }

        public async Task<ResultSuccess<List<Bank>>> GetBanksAsync()
        {
            return await Task.Run(() =>
            {
                try
                {
                    var Banks = conn.Table<Bank>().ToList();

                    return new ResultSuccess<List<Bank>>(true, "", Banks);
                }
                catch (Exception err)
                {
                    return new ResultSuccess<List<Bank>>(false, err.ProperMessage());
                }
            });
        }

        public async Task<ResultSuccess<List<BankAccount>>> GetBankAccountsAsync()
        {
            return await Task.Run(() =>
            {
                try
                {
                    var BankAccounts = conn.Table<BankAccount>().ToList();

                    return new ResultSuccess<List<BankAccount>>(true, "", BankAccounts);
                }
                catch (Exception err)
                {
                    return new ResultSuccess<List<BankAccount>>(false, err.ProperMessage());
                }
            });
        }

        public class AccessModel
        {
            public bool PartnerInsert { get; set; }
            public bool PartnerUpdate { get; set; }
            public bool PartnerSpecifics { get; set; }
            public int AccessToNextDay { get; set; }
            public int AccessToPreDay { get; set; }
            public bool AddDiscounts { get; set; }
            public bool AddFreeProducts { get; set; }
            public bool AccessToViewPartnerRemainder { get; set; }
        }
        public async Task<ResultSuccess> FetchUserAccessesAsync()
        {
            return await Task.Run(() =>
            {
                try
                {
                    var Accesses = conn.Table<Access>().ToArray();
                    var result = new AccessModel() { AccessToNextDay = 0, AccessToPreDay = 0 };

                    result.PartnerInsert = Accesses.Any(a => a.AccessName == "PartnerInsert");
                    result.PartnerUpdate = Accesses.Any(a => a.AccessName == "PartnerUpdate");
                    result.PartnerSpecifics = Accesses.Any(a => a.AccessName == "AccessToPartnerSpecifics");
                    result.AddDiscounts = Accesses.Any(a => a.AccessName == "AddDiscounts");
                    result.AddFreeProducts = Accesses.Any(a => a.AccessName == "AddFreeProducts");
                    result.AccessToViewPartnerRemainder = Accesses.Any(a => a.AccessName == "AccessToViewPartnerRemainder");

                    var AccessToNextDay = Accesses.SingleOrDefault(a => a.AccessName == "AccessToNextDay");
                    if (AccessToNextDay != null)
                    {
                        int Parameter = 0;
                        Int32.TryParse(AccessToNextDay.Parameter, out Parameter);
                        result.AccessToNextDay = Parameter;
                    }
                    var AccessToPreviouseDay = Accesses.SingleOrDefault(a => a.AccessName == "AccessToPreviouseDay");
                    if (AccessToPreviouseDay != null)
                    {
                        int Parameter = 0;
                        Int32.TryParse(AccessToPreviouseDay.Parameter, out Parameter);
                        result.AccessToPreDay = Parameter;
                    }

                    App.Accesses = result;
                    return new ResultSuccess();
                }
                catch (Exception err)
                {
                    return new ResultSuccess(false, err.ProperMessage());
                }
            });
        }

        public async Task<ResultSuccess<List<NotOrderReason>>> GetNotOrderReasonsAsync()
        {
            return await Task.Run(() =>
            {
                try
                {
                    var NotOrderReasons = conn.Table<NotOrderReason>().Where(a => a.Enabled && a.BDate <= DateTime.Today && DateTime.Today <= a.EDate).ToList();

                    return new ResultSuccess<List<NotOrderReason>>(true, "", NotOrderReasons);
                }
                catch (Exception err)
                {
                    return new ResultSuccess<List<NotOrderReason>>(false, err.ProperMessage());
                }
            });
        }

        public async Task<ResultSuccess<List<Credit>>> GetCreditsAsync()
        {
            return await Task.Run(() =>
            {
                try
                {
                    var Credits = conn.Table<Credit>().ToList();

                    return new ResultSuccess<List<Credit>>(true, "", Credits);
                }
                catch (Exception err)
                {
                    return new ResultSuccess<List<Credit>>(false, err.ProperMessage());
                }
            });
        }

        public async Task<ResultSuccess<List<PriceListVersion>>> GetPriceListVersionsAsync()
        {
            return await Task.Run(() =>
            {
                try
                {
                    var PriceListVersions = conn.Table<PriceListVersion>().ToList();

                    return new ResultSuccess<List<PriceListVersion>>(true, "", PriceListVersions);
                }
                catch (Exception err)
                {
                    return new ResultSuccess<List<PriceListVersion>>(false, err.ProperMessage());
                }
            });
        }

        public async Task<ResultSuccess<List<Warehouse>>> GetWarehousesAsync()
        {
            return await Task.Run(() =>
            {
                try
                {
                    var Warehouses = conn.Table<Warehouse>().ToList();

                    return new ResultSuccess<List<Warehouse>>(true, "", Warehouses);
                }
                catch (Exception err)
                {
                    return new ResultSuccess<List<Warehouse>>(false, err.ProperMessage());
                }
            });
        }

        public class OrderListModel : INotifyPropertyChanged
        {
            private bool _Selected;
            public int? _PreCode { get; set; }
            public DateTime _DateTime { get; set; }
            public decimal _Price { get; set; }

            public Guid Id { get; set; }
            public string PreCode { get { return _PreCode.HasValue ? _PreCode.Value.ToString().ToPersianDigits() : "---"; } }
            public string PartnerName { get; set; }
            public string Date { get { return _DateTime.Date.ToTooShortStringForDate().ToPersianDigits(); } }
            public string Time { get { return _DateTime.ToTooShortStringForTime().ToPersianDigits(); } }
            public string Price { get { return _Price.ToString("###,###,###,###,###,###,##0.").ToPersianDigits(); } }
            public string Description { get; set; }
            public string DescriptionColor { get { return "#21822D"; } }
            public bool Sent { get; set; }
            public bool Selected { get { return _Selected; } set { _Selected = value; OnPropertyChanged("Selected"); OnPropertyChanged("RowColor"); App.InsertedInformations_Orders.RefreshToolbarItems(); } }
            public string RowColor { get { return Selected ? "#F5F5A4" : Sent ? "#B7E5BF" : "#DCE6FA"; } }
            public static bool Multiselection { get; set; }
            public bool CanBeSelectedInMultiselection { get { return _PreCode.HasValue ? false : true; } }
            public GridLength CheckBoxColumnWidth { get { return Multiselection ? 60 : 0; } }
            public FailedVisit FailedVisitData { get; set; }
            public SaleOrder SaleOrderData { get; set; }


            public event PropertyChangedEventHandler PropertyChanged;
            public void OnPropertyChanged(string propertyName)
            {
                var handler = PropertyChanged;
                if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public async Task<ResultSuccess<List<OrderListModel>>> GetOrdersListAsync(bool justToday, bool justLocal, string Filter)
        {
            Filter = Filter.ToLatinDigits();
            return await Task.Run(() =>
            {
                try
                {
                    var SaleOrdersQueryable = conn.Table<SaleOrder>().AsQueryable();
                    var Partners = conn.Table<Partner>().ToList();

                    if (justToday)
                    {
                        var Today = DateTime.Now.Date;
                        var Tommorow = Today.AddDays(1);
                        SaleOrdersQueryable = SaleOrdersQueryable.Where(a => a.InsertDateTime >= Today && a.InsertDateTime < Tommorow);
                    }

                    if (justLocal)
                        SaleOrdersQueryable = SaleOrdersQueryable.AsEnumerable().Where(a => !a.PreCode.HasValue).AsQueryable();

                    var AllData = SaleOrdersQueryable.ToList().OrderByDescending(a => a.InsertDateTime).ToList();

                    var AllOrders = AllData.Select(a => new
                    {
                        SaleOrder = a,
                        Id = a.Id,
                        PreCode = a.PreCode,
                        DateTime = a.InsertDateTime,
                        PartnerId = a.PartnerId,
                        FinalPrice = a.OrderFinalPrice,
                        Description = a.Description,
                        Sent = a.PreCode.HasValue
                    }).ToList();

                    var Orders = (
                        from order in AllOrders
                        join partner in Partners on order.PartnerId equals partner.Id
                        select new OrderListModel()
                        {
                            Id = order.Id,
                            _PreCode = order.PreCode,
                            _DateTime = order.DateTime,
                            PartnerName = (!string.IsNullOrEmpty(partner.LegalName) ? (partner.LegalName + (!string.IsNullOrEmpty(partner.Name) ? ("(" + partner.Name + ")") : "")) : partner.Name).ToPersianDigits(),
                            _Price = order.FinalPrice,
                            Description = order.Description != null ? order.Description.Replace("\n", " ") : "",
                            Sent = order.Sent,
                            SaleOrderData = order.SaleOrder
                        }
                    ).ToList();

                    if (!string.IsNullOrWhiteSpace(Filter))
                    {
                        var TotalPoints = Orders.Select(a => new KeyValuePair<OrderListModel, int>(a, 0)).ToList();
                        var FilterSegments = Filter.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var FilterSegment in FilterSegments)
                        {
                            var NewPoints = Orders.Select(a => new KeyValuePair<OrderListModel, int>(a, FilterSegment.Length * (
                                (a.PartnerName == FilterSegment ? 10 : a.PartnerName.StartsWith(FilterSegment) ? 2 : a.PartnerName.Contains(FilterSegment) ? 1 : 0) +
                                (a.Description == FilterSegment ? 10 : a.Description.StartsWith(FilterSegment) ? 2 : a.Description.Contains(FilterSegment) ? 1 : 0) +
                                (a.PreCode == FilterSegment ? 10 : 0)
                            ))).ToList();

                            TotalPoints = (
                                from TotalPoint in TotalPoints
                                join NewPoint in NewPoints on TotalPoint.Key.Id equals NewPoint.Key.Id
                                select new KeyValuePair<OrderListModel, int>(TotalPoint.Key, TotalPoint.Value + NewPoint.Value)
                            ).ToList();
                        }
                        Orders = (
                            from TotalPoint in TotalPoints
                            join Order in Orders on TotalPoint.Key.Id equals Order.Id
                            where TotalPoint.Value != 0
                            orderby TotalPoint.Value
                            select Order
                        ).ToList();
                    }

                    return new ResultSuccess<List<OrderListModel>>(true, "", Orders);
                }
                catch (Exception err)
                {
                    return new ResultSuccess<List<OrderListModel>>(false, err.ProperMessage());
                }
            });
        }

        public class FailedVisitListModel : INotifyPropertyChanged
        {
            private bool _Selected;
            public DateTime _DateTime { get; set; }

            public Guid Id { get; set; }
            public string PartnerName { get; set; }
            public string Date
            {
                get
                {
                    return _DateTime.Date.ToTooShortStringForDate().ToPersianDigits();
                }
            }
            public string Time
            {
                get
                {
                    return _DateTime.ToTooShortStringForTime().ToPersianDigits();
                }
            }
            public string Description { get; set; }
            public string DescriptionColor { get { return "#CE4242"; } }
            public bool Sent { get; set; }
            public bool Selected { get { return _Selected; } set { _Selected = value; OnPropertyChanged("Selected"); OnPropertyChanged("RowColor"); App.InsertedInformations_FailedVisits.RefreshToolbarItems(); } }
            public string RowColor { get { return Selected ? "#F5F5A4" : Sent ? "#B7E5BF" : "#DCE6FA"; } }
            public static bool Multiselection { get; set; }
            public bool CanBeSelectedInMultiselection { get { return Sent ? false : true; } }
            public GridLength CheckBoxColumnWidth { get { return Multiselection ? 60 : 0; } }

            public FailedVisit FailedVisitData { get; set; }

            public event PropertyChangedEventHandler PropertyChanged;
            public void OnPropertyChanged(string propertyName)
            {
                var handler = PropertyChanged;
                if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public async Task<ResultSuccess<List<FailedVisitListModel>>> GetFailedVisitsListAsync(bool justToday, bool justLocal, string Filter)
        {
            Filter = Filter.ToLatinDigits();
            return await Task.Run(() =>
            {
                try
                {
                    var FailedVisitsQueryable = conn.Table<FailedVisit>();
                    var Partners = conn.Table<Partner>().ToList();
                    var FailedVisitReasons = conn.Table<NotOrderReason>().ToList();

                    if (justToday)
                    {
                        var Today = DateTime.Now.Date;
                        var Tommorow = Today.AddDays(1);
                        FailedVisitsQueryable = FailedVisitsQueryable.Where(a => a.VisitTime >= Today && a.VisitTime < Tommorow);
                    }

                    if (justLocal)
                        FailedVisitsQueryable = FailedVisitsQueryable.Where(a => !a.Sent);

                    var AllData = FailedVisitsQueryable.ToList().OrderByDescending(a => a.VisitTime).ToList();

                    var AllFailedVisits = AllData.Select(a => new
                    {
                        Id = a.Id,
                        DateTime = a.VisitTime,
                        PartnerId = a.PartnerId,
                        Description = FailedVisitReasons.Single(b => b.Id == a.ReasonId).Name + (!string.IsNullOrEmpty(a.Description) ? (" توضیحات: " + a.Description) : ""),
                        Sent = a.Sent,
                        FailedVisit = a
                    }).ToList();

                    var FailedVisits = (
                        from failedVisit in AllFailedVisits
                        join partner in Partners on failedVisit.PartnerId equals partner.Id
                        select new FailedVisitListModel()
                        {
                            Id = failedVisit.Id,
                            _DateTime = failedVisit.DateTime,
                            PartnerName = (!string.IsNullOrEmpty(partner.LegalName) ? (partner.LegalName + (!string.IsNullOrEmpty(partner.Name) ? ("(" + partner.Name + ")") : "")) : partner.Name).ToPersianDigits(),
                            Description = failedVisit.Description != null ? failedVisit.Description.Replace("\n", " ") : "",
                            Sent = failedVisit.Sent,
                            FailedVisitData = failedVisit.FailedVisit
                        }
                    ).ToList();

                    if (!string.IsNullOrWhiteSpace(Filter))
                    {
                        var TotalPoints = FailedVisits.Select(a => new KeyValuePair<FailedVisitListModel, int>(a, 0)).ToList();
                        var FilterSegments = Filter.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var FilterSegment in FilterSegments)
                        {
                            var NewPoints = FailedVisits.Select(a => new KeyValuePair<FailedVisitListModel, int>(a, FilterSegment.Length * (
                                (a.PartnerName == FilterSegment ? 10 : a.PartnerName.StartsWith(FilterSegment) ? 2 : a.PartnerName.Contains(FilterSegment) ? 1 : 0) +
                                (a.Description == FilterSegment ? 10 : a.Description.StartsWith(FilterSegment) ? 2 : a.Description.Contains(FilterSegment) ? 1 : 0)
                            ))).ToList();

                            TotalPoints = (
                                from TotalPoint in TotalPoints
                                join NewPoint in NewPoints on TotalPoint.Key.Id equals NewPoint.Key.Id
                                select new KeyValuePair<FailedVisitListModel, int>(TotalPoint.Key, TotalPoint.Value + NewPoint.Value)
                            ).ToList();
                        }
                        FailedVisits = (
                            from TotalPoint in TotalPoints
                            join failedVisit in FailedVisits on TotalPoint.Key.Id equals failedVisit.Id
                            where TotalPoint.Value != 0
                            orderby TotalPoint.Value
                            select failedVisit
                        ).ToList();
                    }

                    return new ResultSuccess<List<FailedVisitListModel>>(true, "", FailedVisits);
                }
                catch (Exception err)
                {
                    return new ResultSuccess<List<FailedVisitListModel>>(false, err.ProperMessage());
                }
            });
        }

        public async Task<ResultSuccess> DeletePartnerDynamicGroupsAsync(Guid PartnerId, Guid[] GroupIds)
        {
            return await Task.Run(() =>
            {
                try
                {
                    foreach (var GroupId in GroupIds)
                        conn.Execute("Delete from DynamicGroupPartner where PartnerId = '" + PartnerId.ToString() + "' and GroupId = '" + GroupId.ToString() + "'");
                    return new ResultSuccess(true, "");
                }
                catch (Exception err)
                {
                    return new ResultSuccess(false, err.ProperMessage());
                }
            });
        }

        public async Task<ResultSuccess> DeleteFailedVisitsAsync(Guid[] VisitIds)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var FailedVisits = conn.Table<FailedVisit>().ToArray().Where(a => VisitIds.Contains(a.Id));
                    var SentCount = FailedVisits.Count(a => a.Sent);
                    if (SentCount != 0)
                        return new ResultSuccess(false, (FailedVisits.Count() == 1 ? "عدم سفارش" : FailedVisits.Count() == SentCount ? "تمام عدم سفارشات" : "برخی از عدم سفارشات") + " انتخاب شده به سرور ارسال شده است و نمی توانید آن" + (FailedVisits.Count() > 1 ? " ها" : "") + " را حذف کنید.");

                    foreach (var FailedVisitId in VisitIds)
                        conn.Execute("Delete from FailedVisit where Id = '" + FailedVisitId.ToString() + "'");

                    return new ResultSuccess(true, "");
                }
                catch (Exception err)
                {
                    return new ResultSuccess(false, err.ProperMessage());
                }
            });
        }
        public async Task<ResultSuccess> DeletePartnersAsync(Guid[] PartnerIds)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var Partners = conn.Table<Partner>().ToArray().Where(a => PartnerIds.Contains(a.Id));
                    var SentCount = Partners.Count(a => !string.IsNullOrEmpty(a.Code));
                    if (SentCount != 0)
                        return new ResultSuccess(false, (Partners.Count() == 1 ? "مشتری" : Partners.Count() == SentCount ? "تمام مشتریان" : "برخی از مشتریان") + " انتخاب شده به سرور ارسال شده و نمی توانید آن" + (Partners.Count() > 1 ? " ها" : "") + " را حذف کنید.");

                    foreach (var PartnerId in PartnerIds)
                    {
                        conn.Execute("Delete from DynamicGroupPartner where PartnerId = '" + PartnerId.ToString() + "'");
                        conn.Execute("Delete from Partner where Id = '" + PartnerId.ToString() + "'");
                    }

                    return new ResultSuccess(true, "");
                }
                catch (Exception err)
                {
                    return new ResultSuccess(false, err.ProperMessage());
                }
            });
        }
        public async Task<ResultSuccess> DeleteSaleOrdersAsync(Guid[] OrderIds)
        {
            return await Task.Run(() =>
            {
                var IsInTransaction = conn.IsInTransaction;
                if (!IsInTransaction)
                    InitTransaction();
                try
                {
                    var SaleOrders = conn.Table<SaleOrder>().ToArray().Where(a => OrderIds.Contains(a.Id));
                    var SentCount = SaleOrders.Count(a => a.PreCode.HasValue);
                    if (SentCount != 0)
                    {
                        if (!IsInTransaction)
                            RollbackTransaction();
                        return new ResultSuccess(false, (SaleOrders.Count() == 1 ? "سفارش" : SaleOrders.Count() == SentCount ? "تمام سفارشات" : "برخی از سفارشات") + " انتخاب شده به سرور ارسال شده است و نمی توانید آن" + (SaleOrders.Count() > 1 ? " ها" : "") + " را حذف کنید.");
                    }

                    foreach (var OrderId in OrderIds)
                    {
                        conn.Execute("Delete from SaleOrderStuff where OrderId = '" + OrderId.ToString() + "'");
                        conn.Execute("Delete from CashDiscount where OrderId = '" + OrderId.ToString() + "'");
                        conn.Execute("Delete from SaleOrder where Id = '" + OrderId.ToString() + "'");
                    }

                    if (!IsInTransaction)
                        CommitTransaction();
                    return new ResultSuccess(true, "");
                }
                catch (Exception err)
                {
                    if (!IsInTransaction)
                        RollbackTransaction();
                    return new ResultSuccess(false, err.ProperMessage());
                }
            });
        }

        public async Task<ResultSuccess<SaleOrder>> GetSaleOrderAsync(Guid OrderId)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var result = conn.Table<SaleOrder>().Single(a => a.Id == OrderId);

                    return new ResultSuccess<SaleOrder>(true, "", result);
                }
                catch (Exception err)
                {
                    return new ResultSuccess<SaleOrder>(false, err.ProperMessage());
                }
            });
        }

        public async Task<ResultSuccess<List<SaleOrderStuff>>> GetSaleOrderStuffsAsync(Guid OrderId)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var result = conn.Table<SaleOrderStuff>().Where(a => a.OrderId == OrderId).ToList();

                    return new ResultSuccess<List<SaleOrderStuff>>(true, "", result);
                }
                catch (Exception err)
                {
                    return new ResultSuccess<List<SaleOrderStuff>>(false, err.ProperMessage());
                }
            });
        }

        public async Task<ResultSuccess<List<CashDiscount>>> GetSaleOrderCashDiscountsAsync(Guid OrderId)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var result = conn.Table<CashDiscount>().Where(a => a.OrderId == OrderId).ToList();

                    return new ResultSuccess<List<CashDiscount>>(true, "", result);
                }
                catch (Exception err)
                {
                    return new ResultSuccess<List<CashDiscount>>(false, err.ProperMessage());
                }
            });
        }

        public async Task<ResultSuccess<List<CashDiscount>>> GetAllSaleOrderCashDiscountsAsync()
        {
            return await Task.Run(() =>
            {
                try
                {
                    var result = conn.Table<CashDiscount>().ToList();

                    return new ResultSuccess<List<CashDiscount>>(true, "", result);
                }
                catch (Exception err)
                {
                    return new ResultSuccess<List<CashDiscount>>(false, err.ProperMessage());
                }
            });
        }

        public async Task<ResultSuccess<NotOrderReason>> GetNotOrderReasonAsync(Guid Id)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var NotOrderReason = conn.Table<NotOrderReason>().SingleOrDefault(a => a.Id == Id);

                    if (NotOrderReason == null)
                        return new ResultSuccess<NotOrderReason>(false, "دلیل عدم سفارش مورد نظر یافت نشد.");

                    return new ResultSuccess<NotOrderReason>(true, "", NotOrderReason);
                }
                catch (Exception err)
                {
                    return new ResultSuccess<NotOrderReason>(false, err.ProperMessage());
                }
            });
        }

        public class FailedVisitModel
        {
            public FailedVisit FailedVisit { get; set; }
            public Partner Partner { get; set; }
            public NotOrderReason NotOrderReason { get; set; }
        }
        public async Task<ResultSuccess<FailedVisitModel>> GetFailedVisitAsync(Guid VisitId)
        {
            return await Task.Run(async () =>
            {
                try
                {
                    var FailedVisit = conn.Table<FailedVisit>().SingleOrDefault(a => a.Id == VisitId);
                    if (FailedVisit == null)
                        return new ResultSuccess<FailedVisitModel>(false, "ویزیت مورد نظر در سیستم یافت نشد.");

                    var PartnerResult = await GetPartnerAsync(FailedVisit.PartnerId, null);
                    if (!PartnerResult.Success)
                        return new ResultSuccess<FailedVisitModel>(false, PartnerResult.Message);

                    var NotOrderReasonResult = await GetNotOrderReasonAsync(FailedVisit.ReasonId);
                    if (!NotOrderReasonResult.Success)
                        return new ResultSuccess<FailedVisitModel>(false, NotOrderReasonResult.Message);

                    return new ResultSuccess<FailedVisitModel>(true, "", new FailedVisitModel()
                    {
                        FailedVisit = FailedVisit,
                        Partner = PartnerResult.Data,
                        NotOrderReason = NotOrderReasonResult.Data
                    });
                }
                catch (Exception err)
                {
                    return new ResultSuccess<FailedVisitModel>(false, err.ProperMessage());
                }
            });
        }

        public class StuffListModelEqualityComparer : IEqualityComparer<StuffListModel>
        {
            public bool Equals(StuffListModel stuff1, StuffListModel stuff2)
            {
                return stuff1.Code == stuff2.Code && stuff1.GroupName == stuff2.GroupName;
            }
            public int GetHashCode(StuffListModel stuff)
            {
                return (stuff.Code + stuff.GroupName).GetHashCode();
            }
        }

        public class StuffListModelArray : INotifyPropertyChanged
        {
            private StuffListModel[] _Stuffs;
            public StuffListModel[] Stuffs
            {
                get { return _Stuffs; }
                set
                {
                    _Stuffs = value;
                    OnPropertyChanged("Stuffs");
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            public void OnPropertyChanged(string propertyName)
            {
                var handler = PropertyChanged;
                if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public class PackageQuantityModel
        {
            public Package Package { get; set; }
            public decimal Quantity { get; set; }
        }
        public class StuffListModel : INotifyPropertyChanged
        {
            public StuffListModel(PackageQuantityModel[] PackagesData)
            {
                this.PackagesData = PackagesData;
                if (PackagesData != null)
                    _SelectedPackage = PackagesData.Any(a => a.Package.DefaultPackage) ? PackagesData.First(a => a.Package.DefaultPackage).Package : PackagesData.Any(a => a.Package.Coefficient == 1) ? PackagesData.Single(a => a.Package.Coefficient == 1).Package : PackagesData.Single(a => a.Package.Coefficient == PackagesData.Min(b => b.Package.Coefficient)).Package;
            }
            private bool _SelectedInGallaryMode;
            public bool SelectedInGallaryMode
            {
                get
                {
                    return _SelectedInGallaryMode;
                }
                set
                {
                    _SelectedInGallaryMode = value;
                    OnPropertyChanged("SelectedInGallaryMode");
                }
            }

            private bool _Selected;
            public Guid StuffId { get; set; }
            public Guid? BatchNumberId { get; set; }
            public string Id { get { return StuffId.ToString() + "|" + (BatchNumberId.HasValue ? BatchNumberId.Value.ToString() : ""); } }
            public string Code { get { return StuffData == null ? null : StuffData.Code.ToPersianDigits(); } }
            public string BarCode { get { return StuffData == null ? null : StuffData.BarCode.ToPersianDigits(); } }
            public string Name { get { return StuffData == null ? null : StuffData.Name.ToPersianDigits(); } }
            public string ReportName { get { return StuffData == null ? null : StuffData.ReportName.ToPersianDigits(); } }
            public string GallaryTitle { get { return Code + " - " + Name; } }
            public string ExpirationDate { get { return BatchNumberData == null ? "---" : (BatchNumberData.ExpirationDatePresentation == (int)DatePresentation.Shamsi ? BatchNumberData.ExpirationDate.ToShortStringForDate() : BatchNumberData.ExpirationDate.ToString("yyyy/MM/dd")).ToPersianDigits(); } }
            public string BatchNumber { get { return BatchNumberData == null ? "---" : BatchNumberData.BatchNumber.ToPersianDigits(); } }
            private bool _QRScannedThisBatchNumber;
            public bool QRScannedThisBatchNumber { get { return _QRScannedThisBatchNumber; } set { _QRScannedThisBatchNumber = value; OnPropertyChanged("RowColor"); } }
            public string Description { get { return StuffData == null ? null : StuffData.Description.ToPersianDigits(); } }
            private string _GroupName;
            public string GroupName { get { return _GroupName == null ? null : _GroupName.ToPersianDigits(); } set { _GroupName = value; OnPropertyChanged("GroupName"); } }

            private string _GroupCode;
            public string GroupCode { get { return _GroupCode == null ? null : _GroupCode.ToPersianDigits(); } set { _GroupCode = value; OnPropertyChanged("GroupCode"); } }
            private int _GroupStuffsCount;
            public int GroupStuffsCount { get { return _GroupStuffsCount; } set { _GroupStuffsCount = value; OnPropertyChanged("DisplayGroupName"); } }
            public string DisplayGroupName { get { return (GroupName + " (" + _GroupStuffsCount + ")").ToPersianDigits(); } }
            public bool IsGroup { get; set; }
            private bool _IsGroupOpen;
            public bool IsGroupOpen { get { return _IsGroupOpen; } set { _IsGroupOpen = value; OnPropertyChanged("GroupButtonIcon"); } }
            public int? GroupNumber { get; set; }
            private string _GroupSummary;
            public string GroupSummary { get { return _GroupSummary == null ? null : _GroupSummary.ToPersianDigits(); } set { _GroupSummary = value; OnPropertyChanged("GroupSummary"); } }
            public int ListGroupRow1Height { get { return BatchNumberId.HasValue ? 0 : IsGroup ? 35 : 0; } }
            public int ListGroupRow2Height { get { return BatchNumberId.HasValue ? 0 : IsGroup ? 20 : 0; } }
            public int ListStuffRowHeight { get { return BatchNumberId.HasValue ? 0 : IsGroup ? 0 : 45; } }
            public int BatchNumberRowHeight { get { return BatchNumberId.HasValue ? 35 : 0; } }
            public ImageSource GroupButtonIcon { get { return IsGroup ? IsGroupOpen ? "UpArrow.png" : "DownArrow.png" : null; } }
            bool PointJustEntered;

            private decimal _quantity;
            public decimal Quantity
            {
                get
                {
                    //if (!ForSaleReversion)
                        return GetQuantity();

                    return _quantity;
                }
                set
                {
                    //if (!ForSaleReversion)
                        SetQuantity(value);
                    //else
                        //_quantity = value;
                }
            }

            private decimal GetQuantity()
            {
                return HasBatchNumbers ? StuffRow_BatchNumberRows.Sum(a => a.Quantity) : PackagesData == null ? 0 : PackagesData.Single(a => a.Package.Id == SelectedPackage.Id).Quantity;
            }

            public bool ForTotalSaleOrderEditing { get; set; }
            public bool ForSaleReversion { get; set; }

            private void SetQuantity(decimal value)
            {

                if (PackagesData != null && !HasBatchNumbers)
                {
                    //if (ForSaleReversion)
                    //{
                    //    PackagesData.Single(a => a.Package.Id == SelectedPackage.Id).Quantity += value;
                    //    return;
                    //}

                    var NewValue = value;
                    PointJustEntered = (double)(value % 1) == (double)0.354168413153848456;
                    if (PointJustEntered)
                        NewValue = Math.Floor(NewValue);
                    var ValueChange = NewValue - PackagesData.Single(a => a.Package.Id == SelectedPackage.Id).Quantity;

                    if (ValueChange * SelectedPackage.Coefficient > RemainedStock &&
                        !App.CheckForNegativeStocksOnOrderInsertion.Value)
                        ValueChange = RemainedStock / SelectedPackage.Coefficient;

                    ValueChange =
                        Convert.ToDecimal(
                            (Math.Floor(ValueChange / SelectedPackage.PossibleQuantityCoefficient) *
                             SelectedPackage.PossibleQuantityCoefficient).ToString("##############0.###"));

                    PackagesData.Single(a => a.Package.Id == SelectedPackage.Id).Quantity += ValueChange;
                    if (PackagesData.Single(a => a.Package.Id == SelectedPackage.Id).Quantity < 0)
                        PackagesData.Single(a => a.Package.Id == SelectedPackage.Id).Quantity = 0;

                    ChangeProperties();
                    if (BatchNumberId.HasValue)
                        BatchNumberRow_StuffParentRow.ChangeProperties();

                    //_quantity = value;
                }
            }

            private bool _QuantityFocused;
            public bool QuantityFocused
            {
                get
                {
                    return _QuantityFocused;
                }
                set
                {
                    _QuantityFocused = value;
                    OnPropertyChanged("QuantityFocused");
                    OnPropertyChanged("QuantityLabel");
                }
            }
            public string QuantityLabel { get { return Quantity.ToString().ToPersianDigits().Replace(".", "/") + (PointJustEntered ? "/" : "") + (_QuantityFocused ? "|" : ""); } }
            public decimal? _UnitPrice { get; set; }
            public decimal? _ConsumerUnitPrice { get; set; }
            private Package _SelectedPackage { get; set; }
            public Package SelectedPackage
            {
                get
                {
                    return BatchNumberId.HasValue ? BatchNumberRow_StuffParentRow.SelectedPackage : _SelectedPackage;
                }
                set
                {
                    if (!BatchNumberId.HasValue)
                    {
                        _SelectedPackage = value;
                        ChangeProperties();
                        if (StuffRow_BatchNumberRows != null)
                            foreach (var item in StuffRow_BatchNumberRows)
                                item.ChangeProperties();
                    }
                }
            }
            public string UnitName { get { return SelectedPackage == null ? "" : (SelectedPackage.Name + (SelectedPackage.Coefficient != 1 ? ("(" + SelectedPackage.Coefficient + ")") : "")).ToPersianDigits(); } }

            public string Fee
            {
                get { return !_UnitPrice.HasValue ? "" : (_UnitPrice.Value * (SelectedPackage == null ? 1 : SelectedPackage.Coefficient)).ToString("###,###,###,###,###,###,##0.").ToPersianDigits(); }
            }

            private string _reversionFee;
            public string ReversionFee
            {
                get { return _reversionFee.ToPersianDigits(); }
                set { _reversionFee = value.ToLatinDigits(); }
            }

            private string _reversionVATPercent;
            public string ReversionVATPercent
            {
                get { return _reversionVATPercent.ToPersianDigits(); }
                set
                {
                    if (ReversionIsFreeProduct)
                        _reversionVATPercent = "";
                    else
                        _reversionVATPercent = value.ToLatinDigits();
                }
            }

            private string _reversionVATAmount;
            public string ReversionVATAmount
            {
                get { return _reversionVATAmount.ToPersianDigits(); }
                set
                { _reversionVATAmount = value.ToLatinDigits(); }
            }

            private string _reversionDiscountPercent;
            public string ReversionDiscountPercent
            {
                get { return _reversionDiscountPercent.ToPersianDigits(); }
                set
                {
                    if (ReversionIsFreeProduct)
                        _reversionDiscountPercent = "";
                    else
                        _reversionDiscountPercent = value.ToLatinDigits();
                }
            }

            private bool _reversionIsFreeProduct;
            public bool ReversionIsFreeProduct
            {
                get { return _reversionIsFreeProduct; }
                set
                {
                    _reversionIsFreeProduct = value;
                    if (value)
                    {
                        ReversionDiscountPercent = "";
                        ReversionVATPercent = "";
                    }
                }
            }

            public string ConsumerFee { get { return "ق م: " + (!_ConsumerUnitPrice.HasValue ? "---" : (_ConsumerUnitPrice.Value * (SelectedPackage == null ? 1 : SelectedPackage.Coefficient)).ToString("###,###,###,###,###,###,##0.").ToPersianDigits()); } }
            public bool Selected { get { return _Selected; } set { _Selected = value; OnPropertyChanged("Selected"); OnPropertyChanged("RowColor"); } }
            public string RowColor { get { return IsGroup ? "#B4E8F5" : Selected ? "#A4DEF5" : OddRow ? "#DCE6FA" : _QRScannedThisBatchNumber ? "#94CEE5" : "#CCD6EA"; } }
            public ImageSource ImageSource { get; set; }
            public Stuff StuffData { get; set; }
            public StuffBatchNumber BatchNumberData { get; set; }
            public PackageQuantityModel[] PackagesData { get; set; }
            public decimal TotalStuffQuantity { get { return HasBatchNumbers ? StuffRow_BatchNumberRows.Sum(a => a.TotalStuffQuantity) : PackagesData != null ? PackagesData.Any() ? PackagesData.Sum(a => a.Quantity * a.Package.Coefficient) : 0 : 0; } }

            private decimal __UnitStock;
            public decimal _UnitStock
            {
                get
                {
                    return !HasBatchNumbers ? __UnitStock : StuffRow_BatchNumberRows.Any() ? StuffRow_BatchNumberRows.Sum(a => a._UnitStock) : 0;
                }
                set
                {
                    if (!HasBatchNumbers)
                        __UnitStock = value ;
                }
            }

            private decimal _remainedStock;
            public decimal RemainedStock
            {
                get
                {
                    if (HasBatchNumbers)
                        return StuffRow_BatchNumberRows.Sum(a => a.RemainedStock);

                    decimal res = _UnitStock - (PackagesData?.Sum(a => a.Quantity * a.Package.Coefficient) ?? 0);

                    return res;
                }
                //set
                //{
                //    //_remainedStock = value;
                //}
            }

            public string Stock => Math.Floor(RemainedStock / (SelectedPackage?.Coefficient ?? 1)).ToString("###,###,###,##0.").ToPersianDigits();


            public decimal? _Price => TotalStuffQuantity * _UnitPrice;
            public string Price => _Price.HasValue ? _Price.Value.ToString("###,###,###,###,###,###,##0.").ToPersianDigits() : "---";




            public bool HasBatchNumbers { get { return StuffRow_BatchNumberRows != null && StuffRow_BatchNumberRows.Any(); } }
            private bool _OddRow;
            public bool OddRow { get { return _OddRow; } set { _OddRow = value; OnPropertyChanged("RowColor"); } }

            public StuffListModel BatchNumberRow_StuffParentRow;
            public StuffListModel[] StuffRow_BatchNumberRows;

            public void ChangeProperties()
            {
                OnPropertyChanged("SelectedPackage");
                OnPropertyChanged("UnitName");
                OnPropertyChanged("Stock");
                OnPropertyChanged("Fee");
                OnPropertyChanged("ConsumerFee");
                OnPropertyChanged("Quantity");
                OnPropertyChanged("_Price");
                OnPropertyChanged("Price");
                OnPropertyChanged("QuantityLabel");
                OnPropertyChanged("ReversionIsFreeProduct");
            }

            public event PropertyChangedEventHandler PropertyChanged;
            public void OnPropertyChanged(string propertyName)
            {
                var handler = PropertyChanged;
                if (handler != null)
                    handler(this, new PropertyChangedEventArgs(propertyName));
            }

            public Guid ArticleId { get; set; }
        }

        public static StuffListModel CloneStuff(StuffListModel obj)
        {
            var result = new StuffListModel(obj.PackagesData.Select(a => new PackageQuantityModel() { Package = a.Package, Quantity = 0 }).ToArray())
            {
                StuffId = obj.StuffId,
                BatchNumberId = obj.BatchNumberId,
                GroupName = obj.GroupName,
                GroupCode = obj.GroupCode,
                IsGroup = obj.IsGroup,
                GroupNumber = obj.GroupNumber,
                _UnitPrice = obj._UnitPrice,
                _ConsumerUnitPrice = obj._ConsumerUnitPrice,
                ImageSource = obj.ImageSource,
                StuffData = obj.StuffData,
                BatchNumberData = obj.BatchNumberData,
                _UnitStock = obj._UnitStock
            };

            if (!obj.BatchNumberId.HasValue)
            {
                result.StuffRow_BatchNumberRows = obj.StuffRow_BatchNumberRows.Select(a => CloneStuff(a)).ToArray();
                foreach (var item in result.StuffRow_BatchNumberRows)
                    item.BatchNumberRow_StuffParentRow = result;
            }

            return result;
        }

        public static bool _AllStuffsDataInitialized = false;
        public static List<StuffListModel> _AllStuffsData;
        public static List<StuffListModel> _AllStuffsDataForReversion;
        public static List<StuffListModel> _AllStuffGroupsData;
        class GuidArrayEqualityComparer : IEqualityComparer<Guid[]>
        {
            public bool Equals(Guid[] X, Guid[] Y)
            {
                if (X == null && Y == null)
                    return true;
                else if (X == null | Y == null)
                    return false;
                else if (X.Any(x => !Y.Any(y => x == y)) || Y.Any(y => !X.Any(x => x == y)))
                    return false;
                else
                    return true;
            }

            public int GetHashCode(Guid[] obj)
            {
                return (obj == null ? "null" : !obj.Any() ? "" : obj.OrderBy(a => a).Select(a => a.ToString()).Aggregate((sum, x) => sum + "|" + x)).GetHashCode();
            }
        }
        public async Task<ResultSuccess> FetchStuffsListAsync()
        {
            return await Task.Run(() =>
            {
                try
                {
                    var AllStuffs_OrderLess = conn.Table<Stuff>().Where(a => a.Enabled).ToList();
                    var StuffOrders = conn.Table<StuffOrder>().ToList();

                    var AllStuffs = (
                        from stuff in AllStuffs_OrderLess
                        join order in StuffOrders on stuff.Id equals order.StuffId into stuffsWithOrder
                        from stuffWithOrder in stuffsWithOrder.DefaultIfEmpty()
                        orderby stuffWithOrder != null ? stuffWithOrder.OrderIndex : 1000000000, stuff.Code
                        select stuff
                    ).ToList();

                    var AllPackages = conn.Table<Package>().Where(a => a.Enabled).ToList();

                    var AllBatchNumbers = conn.Table<StuffBatchNumber>().Where(a => a.Enabled).ToList();

                    var StuffsGroups = AllStuffs.Select(a => new { GroupId = Guid.Empty, GroupCode = "", GroupName = "", StuffId = a.Id }).ToList();
                    if (App.StuffListGroupingMethod.Value == 1)
                    {
                        var StuffGroups = conn.Table<StuffGroup>().ToList();
                        var StuffGroupsWithParents = StuffGroups.Where(a => AllStuffs.Any(b => b.GroupId == a.Id)).Select(a => new
                        {
                            Group = a,
                            Parent = StuffGroups.SingleOrDefault(b => b.Id == a.ParentId)
                        }).Select(a => new
                        {
                            Group = a.Group,
                            Parent = a.Parent,
                            GrandParent = a.Parent == null ? null : StuffGroups.SingleOrDefault(b => b.Id == a.Parent.ParentId)
                        }).Select(a => new
                        {
                            Group = a.Group,
                            Parent = a.Parent,
                            GrandParent = a.GrandParent,
                            GrandGrandParent = a.GrandParent == null ? null : StuffGroups.SingleOrDefault(b => b.Id == a.GrandParent.ParentId)
                        }).Select(a => new
                        {
                            GroupId = a.Group.Id,
                            GroupCode = a.Group.Code,
                            GroupName = (a.Parent == null ? "" : ((a.GrandParent == null ? "" : ((a.GrandGrandParent == null ? "" : (a.GrandGrandParent.Name + " > ")) + a.GrandParent.Name + " > ")) + a.Parent.Name + " > ")) + a.Group.Name
                        });

                        StuffsGroups = StuffGroupsWithParents.SelectMany(a => AllStuffs.Where(b => b.GroupId == a.GroupId).Select(b => new { GroupId = a.GroupId, GroupCode = a.GroupCode, GroupName = a.GroupName, StuffId = b.Id })).ToList();
                    }
                    else if (App.StuffListGroupingMethod.Value == 2)
                    {
                        var StuffBaskets = conn.Table<StuffBasket>().ToList();
                        var StuffBasketStuffs = conn.Table<StuffBasketStuff>().ToList();

                        var MultipleBasketIdsStuffIds = StuffBasketStuffs.GroupBy(a => a.StuffId).Where(a => a.Count() > 1).ToArray();
                        var guidArrayEqualityComparer = new GuidArrayEqualityComparer();
                        var MultipleBasketIds = MultipleBasketIdsStuffIds.Select(a => a.Select(b => b.BasketId).OrderBy(b => b).ToArray()).Distinct(guidArrayEqualityComparer).ToArray();

                        var CompoundBaskets = MultipleBasketIds.Select(a => new KeyValuePair<Guid[], StuffBasket>(a, new StuffBasket()
                        {
                            Id = Guid.NewGuid(),
                            Name = a.Select(b => StuffBaskets.Single(c => c.Id == b).Name).Aggregate((sum, x) => sum + "، " + x)
                        })).ToArray();

                        var AddingStuffBaskets = CompoundBaskets.Select(a => a.Value).ToArray();

                        StuffBaskets.AddRange(AddingStuffBaskets);

                        var CompoundBasketsStuffs = MultipleBasketIdsStuffIds.Select(a => new StuffBasketStuff()
                        {
                            StuffId = a.Key,
                            BasketId = CompoundBaskets.Single(b => guidArrayEqualityComparer.Equals(a.Select(c => c.BasketId).ToArray(), b.Key)).Value.Id
                        }).ToArray();

                        var RemovingStuffBasketStuffs = StuffBasketStuffs.Where(a => MultipleBasketIdsStuffIds.Any(b => a.StuffId == b.Key)).ToArray();
                        foreach (var item in RemovingStuffBasketStuffs)
                            StuffBasketStuffs.Remove(item);
                        StuffBasketStuffs.AddRange(CompoundBasketsStuffs);

                        StuffsGroups = StuffBaskets.OrderBy(a => a.Name).SelectMany((a, index) => StuffBasketStuffs.Where(b => b.BasketId == a.Id).Select(b => new { GroupId = a.Id, GroupCode = index.ToString().PadLeft(3, '0'), GroupName = a.Name, StuffId = b.StuffId })).ToList();
                        var WithoutBasketStuffs = AllStuffs.Where(a => !StuffBasketStuffs.Any(b => b.StuffId == a.Id)).Select(a => new { GroupId = Guid.Empty, GroupCode = "99999999", GroupName = "سایر کالاها", StuffId = a.Id }).ToArray();
                        if (WithoutBasketStuffs.Any())
                            StuffsGroups.AddRange(WithoutBasketStuffs);
                    }

                    var StuffsWithBatchNumbers = (from stuff in AllStuffs
                                                  from batch in AllBatchNumbers.Where(a => a.StuffId == stuff.Id).DefaultIfEmpty()
                                                  group batch by stuff into batchs
                                                  select new
                                                  {
                                                      Stuff = batchs.Key,
                                                      BatchNumbers = batchs.Where(a => a != null).ToArray()
                                                  }).ToArray();

                    var StuffsWithData = (from Stuff in StuffsWithBatchNumbers
                                          join StuffGroup in StuffsGroups on Stuff.Stuff.Id equals StuffGroup.StuffId
                                          join Package in AllPackages on Stuff.Stuff.Id equals Package.StuffId
                                          group Package by new { Stuff, StuffGroup } into Packages
                                          select new StuffListModel(Packages.Select(a => new PackageQuantityModel() { Package = a, Quantity = 0 }).ToArray())
                                          {
                                              StuffId = Packages.Key.Stuff.Stuff.Id,
                                              GroupName = Packages.Key.StuffGroup.GroupName,
                                              GroupCode = Packages.Key.StuffGroup.GroupCode,
                                              _UnitStock = 0,
                                              _UnitPrice = null,
                                              _ConsumerUnitPrice = null,
                                              StuffData = Packages.Key.Stuff.Stuff,
                                              BatchNumberData = null,
                                              ImageSource = !string.IsNullOrWhiteSpace(Packages.Key.Stuff.Stuff.ImageFileExtension) && Packages.Key.Stuff.Stuff.SavedImageDate.HasValue ? ImageSource.FromFile(Path.Combine(App.imagesDirectory, Packages.Key.Stuff.Stuff.Id + "." + Packages.Key.Stuff.Stuff.ImageFileExtension)) : "NoImage.png",

                                              StuffRow_BatchNumberRows = !Packages.Key.Stuff.BatchNumbers.Any() ? new StuffListModel[] { } :
                                                  Packages.Key.Stuff.BatchNumbers.OrderBy(a => a.ExpirationDate).ThenBy(a => a.BatchNumber).Select(a => new StuffListModel(Packages.Select(b => new PackageQuantityModel() { Package = b, Quantity = 0 }).ToArray())
                                                  {
                                                      StuffId = Packages.Key.Stuff.Stuff.Id,
                                                      BatchNumberId = a.BatchNumberId,
                                                      BatchNumberData = a
                                                  }).ToArray().Union(new StuffListModel[] {
                                                      new StuffListModel(Packages.Select(b => new PackageQuantityModel() { Package = b, Quantity = 0 }).ToArray()) {
                                                          StuffId = Packages.Key.Stuff.Stuff.Id,
                                                          BatchNumberId = Guid.Empty,
                                                          BatchNumberData = null
                                                      }
                                                  }).ToArray()
                                          }).ToList();

                    foreach (var StuffRow in StuffsWithData)
                        foreach (var BatchNumberRow in StuffRow.StuffRow_BatchNumberRows)
                            BatchNumberRow.BatchNumberRow_StuffParentRow = StuffRow;

                    _AllStuffsData = StuffsWithData;
                    _AllStuffGroupsData = StuffsGroups.Any(a => a.GroupId != Guid.Empty) ? StuffsGroups.GroupBy(a => new { a.GroupId, a.GroupCode, a.GroupName }).OrderBy(a => a.Key.GroupCode).Select((a, index) => new StuffListModel(null) { StuffId = a.Key.GroupId, GroupCode = a.Key.GroupCode, GroupName = a.Key.GroupName, IsGroup = true, IsGroupOpen = false, GroupNumber = index + 1 }).ToList() : null;
                    _AllStuffsDataInitialized = true;

                    return new ResultSuccess(true, "");
                }
                catch (Exception err)
                {
                    return new ResultSuccess(false, err.ProperMessage());
                }
            });
        }

        public async Task<ResultSuccess> FetchStuffsListForReversionAsync()
        {
            return await Task.Run(() =>
            {
                try
                {
                    var AllStuffs_OrderLess = conn.Table<Stuff>().Where(a => a.Enabled).ToList();
                    var StuffOrders = conn.Table<StuffOrder>().ToList();

                    var AllStuffs = (
                        from stuff in AllStuffs_OrderLess
                        join order in StuffOrders on stuff.Id equals order.StuffId into stuffsWithOrder
                        from stuffWithOrder in stuffsWithOrder.DefaultIfEmpty()
                        orderby stuffWithOrder != null ? stuffWithOrder.OrderIndex : 1000000000, stuff.Code
                        select stuff
                    ).ToList();

                    var AllPackages = conn.Table<Package>().Where(a => a.Enabled).ToList();

                    var AllBatchNumbers = conn.Table<StuffBatchNumber>().Where(a => a.Enabled).ToList();

                    var StuffsGroups = AllStuffs.Select(a => new { GroupId = Guid.Empty, GroupCode = "", GroupName = "", StuffId = a.Id }).ToList();
                    if (App.StuffListGroupingMethod.Value == 1)
                    {
                        var StuffGroups = conn.Table<StuffGroup>().ToList();
                        var StuffGroupsWithParents = StuffGroups.Where(a => AllStuffs.Any(b => b.GroupId == a.Id)).Select(a => new
                        {
                            Group = a,
                            Parent = StuffGroups.SingleOrDefault(b => b.Id == a.ParentId)
                        }).Select(a => new
                        {
                            Group = a.Group,
                            Parent = a.Parent,
                            GrandParent = a.Parent == null ? null : StuffGroups.SingleOrDefault(b => b.Id == a.Parent.ParentId)
                        }).Select(a => new
                        {
                            Group = a.Group,
                            Parent = a.Parent,
                            GrandParent = a.GrandParent,
                            GrandGrandParent = a.GrandParent == null ? null : StuffGroups.SingleOrDefault(b => b.Id == a.GrandParent.ParentId)
                        }).Select(a => new
                        {
                            GroupId = a.Group.Id,
                            GroupCode = a.Group.Code,
                            GroupName = (a.Parent == null ? "" : ((a.GrandParent == null ? "" : ((a.GrandGrandParent == null ? "" : (a.GrandGrandParent.Name + " > ")) + a.GrandParent.Name + " > ")) + a.Parent.Name + " > ")) + a.Group.Name
                        });

                        StuffsGroups = StuffGroupsWithParents.SelectMany(a => AllStuffs.Where(b => b.GroupId == a.GroupId).Select(b => new { GroupId = a.GroupId, GroupCode = a.GroupCode, GroupName = a.GroupName, StuffId = b.Id })).ToList();
                    }
                    else if (App.StuffListGroupingMethod.Value == 2)
                    {
                        var StuffBaskets = conn.Table<StuffBasket>().ToList();
                        var StuffBasketStuffs = conn.Table<StuffBasketStuff>().ToList();

                        var MultipleBasketIdsStuffIds = StuffBasketStuffs.GroupBy(a => a.StuffId).Where(a => a.Count() > 1).ToArray();
                        var guidArrayEqualityComparer = new GuidArrayEqualityComparer();
                        var MultipleBasketIds = MultipleBasketIdsStuffIds.Select(a => a.Select(b => b.BasketId).OrderBy(b => b).ToArray()).Distinct(guidArrayEqualityComparer).ToArray();

                        var CompoundBaskets = MultipleBasketIds.Select(a => new KeyValuePair<Guid[], StuffBasket>(a, new StuffBasket()
                        {
                            Id = Guid.NewGuid(),
                            Name = a.Select(b => StuffBaskets.Single(c => c.Id == b).Name).Aggregate((sum, x) => sum + "، " + x)
                        })).ToArray();

                        var AddingStuffBaskets = CompoundBaskets.Select(a => a.Value).ToArray();

                        StuffBaskets.AddRange(AddingStuffBaskets);

                        var CompoundBasketsStuffs = MultipleBasketIdsStuffIds.Select(a => new StuffBasketStuff()
                        {
                            StuffId = a.Key,
                            BasketId = CompoundBaskets.Single(b => guidArrayEqualityComparer.Equals(a.Select(c => c.BasketId).ToArray(), b.Key)).Value.Id
                        }).ToArray();

                        var RemovingStuffBasketStuffs = StuffBasketStuffs.Where(a => MultipleBasketIdsStuffIds.Any(b => a.StuffId == b.Key)).ToArray();
                        foreach (var item in RemovingStuffBasketStuffs)
                            StuffBasketStuffs.Remove(item);
                        StuffBasketStuffs.AddRange(CompoundBasketsStuffs);

                        StuffsGroups = StuffBaskets.OrderBy(a => a.Name).SelectMany((a, index) => StuffBasketStuffs.Where(b => b.BasketId == a.Id).Select(b => new { GroupId = a.Id, GroupCode = index.ToString().PadLeft(3, '0'), GroupName = a.Name, StuffId = b.StuffId })).ToList();
                        var WithoutBasketStuffs = AllStuffs.Where(a => !StuffBasketStuffs.Any(b => b.StuffId == a.Id)).Select(a => new { GroupId = Guid.Empty, GroupCode = "99999999", GroupName = "سایر کالاها", StuffId = a.Id }).ToArray();
                        if (WithoutBasketStuffs.Any())
                            StuffsGroups.AddRange(WithoutBasketStuffs);
                    }

                    var StuffsWithBatchNumbers = (from stuff in AllStuffs
                                                  from batch in AllBatchNumbers.Where(a => a.StuffId == stuff.Id).DefaultIfEmpty()
                                                  group batch by stuff into batchs
                                                  select new
                                                  {
                                                      Stuff = batchs.Key,
                                                      BatchNumbers = batchs.Where(a => a != null).ToArray()
                                                  }).ToArray();

                    var StuffsWithData = (from Stuff in StuffsWithBatchNumbers
                                          join StuffGroup in StuffsGroups on Stuff.Stuff.Id equals StuffGroup.StuffId
                                          join Package in AllPackages on Stuff.Stuff.Id equals Package.StuffId
                                          group Package by new { Stuff, StuffGroup } into Packages
                                          select new StuffListModel(Packages.Select(a => new PackageQuantityModel() { Package = a, Quantity = 0 }).ToArray())
                                          {
                                              StuffId = Packages.Key.Stuff.Stuff.Id,
                                              GroupName = Packages.Key.StuffGroup.GroupName,
                                              GroupCode = Packages.Key.StuffGroup.GroupCode,
                                              _UnitStock = 0,
                                              _UnitPrice = null,
                                              _ConsumerUnitPrice = null,
                                              StuffData = Packages.Key.Stuff.Stuff,
                                              BatchNumberData = null,
                                              ImageSource = !string.IsNullOrWhiteSpace(Packages.Key.Stuff.Stuff.ImageFileExtension) && Packages.Key.Stuff.Stuff.SavedImageDate.HasValue ? ImageSource.FromFile(Path.Combine(App.imagesDirectory, Packages.Key.Stuff.Stuff.Id + "." + Packages.Key.Stuff.Stuff.ImageFileExtension)) : "NoImage.png",

                                              StuffRow_BatchNumberRows = !Packages.Key.Stuff.BatchNumbers.Any() ? new StuffListModel[] { } :
                                                  Packages.Key.Stuff.BatchNumbers.OrderBy(a => a.ExpirationDate).ThenBy(a => a.BatchNumber).Select(a => new StuffListModel(Packages.Select(b => new PackageQuantityModel() { Package = b, Quantity = 0 }).ToArray())
                                                  {
                                                      StuffId = Packages.Key.Stuff.Stuff.Id,
                                                      BatchNumberId = a.BatchNumberId,
                                                      BatchNumberData = a
                                                  }).ToArray().Union(new StuffListModel[] {
                                                      new StuffListModel(Packages.Select(b => new PackageQuantityModel() { Package = b, Quantity = 0 }).ToArray()) {
                                                          StuffId = Packages.Key.Stuff.Stuff.Id,
                                                          BatchNumberId = Guid.Empty,
                                                          BatchNumberData = null
                                                      }
                                                  }).ToArray()
                                          }).ToList();

                    foreach (var StuffRow in StuffsWithData)
                        foreach (var BatchNumberRow in StuffRow.StuffRow_BatchNumberRows)
                            BatchNumberRow.BatchNumberRow_StuffParentRow = StuffRow;

                    _AllStuffsDataForReversion = StuffsWithData;
                    _AllStuffGroupsData = StuffsGroups.Any(a => a.GroupId != Guid.Empty) ? StuffsGroups.GroupBy(a => new { a.GroupId, a.GroupCode, a.GroupName }).OrderBy(a => a.Key.GroupCode).Select((a, index) => new StuffListModel(null) { StuffId = a.Key.GroupId, GroupCode = a.Key.GroupCode, GroupName = a.Key.GroupName, IsGroup = true, IsGroupOpen = false, GroupNumber = index + 1 }).ToList() : null;

                    return new ResultSuccess(true, "");
                }
                catch (Exception err)
                {
                    return new ResultSuccess(false, err.ProperMessage());
                }
            });
        }

        public async Task GetZeroStockStuffAndAddToSourceAsync(Guid? PartnerId,
            Guid? EditingOrderId, bool WithoutGroups, Guid? WarehouseId, Guid stuffId, SaleOrder EditingOrder,
            List<StuffListModel> LastStuffsGroups,
            List<StuffListModel> AllStuffsData, List<StuffListModel> AllStuffGroupsData,
            Picker GallaryStuffGroupPicker,
            ObservableCollection<StuffListModel> _StuffsList, string Filter)
        {
            //get a single stuff
            var result = await DoGetAllStuffsListAsync(PartnerId, EditingOrderId, WithoutGroups, WarehouseId, stuffId);

            var NewStuffsData = result.Data[0];
            var NewStuffGroupsData = result.Data[1];

            AllStuffGroupsData = NewStuffGroupsData;

            var EditingSaleOrderStuffs = EditingOrder.SaleOrderStuffs.Where(a => !a.FreeProduct);

            foreach (var saleOrderStuff in EditingSaleOrderStuffs)
            {
                DBRepository.StuffListModel StuffInList = NewStuffsData.SingleOrDefault(a => a.StuffId == saleOrderStuff.Package.StuffId);

                if (StuffInList != null)
                {
                    StuffInList.ForTotalSaleOrderEditing = true;

                    if (StuffInList.HasBatchNumbers)
                        StuffInList = StuffInList.StuffRow_BatchNumberRows.SingleOrDefault(a => a.BatchNumberId == saleOrderStuff.BatchNumberId.GetValueOrDefault(Guid.Empty));
                    if (StuffInList != null)
                    {
                        if (StuffInList.BatchNumberId.HasValue)
                            StuffInList.BatchNumberRow_StuffParentRow.SelectedPackage = saleOrderStuff.Package;
                        else
                            StuffInList.SelectedPackage = saleOrderStuff.Package;

                        var selectedPackageId = StuffInList.SelectedPackage.Id;

                        if (StuffInList.PackagesData != null)
                            StuffInList.PackagesData.FirstOrDefault(p => p.Package.Id == selectedPackageId).Quantity = saleOrderStuff.Quantity;
                        StuffInList._UnitStock = saleOrderStuff.Quantity;
                    }

                    StuffInList.ForTotalSaleOrderEditing = false;

                    StuffInList.ArticleId = saleOrderStuff.Id;
                }
            }

            foreach (var item in NewStuffsData)
                if (item.TotalStuffQuantity > 0)
                    item.SelectedInGallaryMode = true;

            var FilteredStuffs = await App.DB.FilterStuffsAsync(NewStuffsData, Filter);

            if (EditingOrder != null)
                FilteredStuffs = FilteredStuffs.OrderBy(a => a.Quantity == 0).ToList();

            var StuffsWithGroupsData = FilteredStuffs.ToList();

            if (AllStuffGroupsData != null)
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

                //todo 1400/11/04 check with not null batch numbers

                var BatchNumbers = StuffsListTemp.Where(a => !a.IsGroup).SelectMany(a => a.StuffRow_BatchNumberRows).ToList();

                StuffsListTemp.AddRange(BatchNumbers);

                var toBeInsertedStuff = StuffsListTemp.FirstOrDefault();
                if (toBeInsertedStuff != null)
                {
                    toBeInsertedStuff.OddRow = _StuffsList.Count % 2 == 0;
                    _StuffsList.Add(toBeInsertedStuff);
                    AllStuffsData.Add(toBeInsertedStuff);
                }
            }
            catch (Exception err)
            {

            }
        }

        public async Task<ResultSuccess<List<StuffListModel>[]>> GetAllStuffsListAsync(Guid? PartnerId,
            Guid? EditingOrderId, bool WithoutGroups, Guid? WarehouseId)
        {
            return await DoGetAllStuffsListAsync(PartnerId, EditingOrderId, WithoutGroups, WarehouseId);
        }

        private async Task<ResultSuccess<List<StuffListModel>[]>> DoGetAllStuffsListAsync(Guid? PartnerId, Guid? EditingOrderId, bool WithoutGroups, Guid? WarehouseId, Guid? stuffId = null)
        {
            return await Task.Run(async () =>
            {
                try
                {
                    if (!_AllStuffsDataInitialized)
                    {
                        var result = await FetchStuffsListAsync();
                        if (!result.Success)
                            return new ResultSuccess<List<StuffListModel>[]>(false, result.Message);
                    }

                    var Stocks = conn.Table<Stock>().ToList().Select(a => new
                    {
                        WarehouseId = a.WarehouseId,
                        StuffId = a.StuffId,
                        BatchNumberId = a.BatchNumberId.GetValueOrDefault(Guid.Empty),
                        Stock = Math.Min(a.Real, a.Fake)
                    }).ToList();

                    var Orders = conn.Table<SaleOrder>().ToList().Where(a => !a.PreCode.HasValue && a.Id != EditingOrderId).ToList();

                    if (WarehouseId.HasValue)
                    {
                        Stocks = Stocks.Where(a => a.WarehouseId == WarehouseId).ToList();
                        Orders = Orders.Where(a => a.WarehouseId == WarehouseId).ToList();
                    }

                    var NotSubmittedOrderStuffs = (from order in Orders
                                                   from stuff in conn.Table<SaleOrderStuff>().ToList().Where(a => a.OrderId == order.Id)
                                                   from package in conn.Table<Package>().ToList().Where(a => a.Id == stuff.PackageId)
                                                   select new
                                                   {
                                                       StuffId = package.StuffId,
                                                       BatchNumberId = stuff.BatchNumberId.GetValueOrDefault(Guid.Empty),
                                                       Quantity = stuff.Quantity * package.Coefficient
                                                   }).GroupBy(a => new
                                                   {
                                                       a.StuffId,
                                                       a.BatchNumberId
                                                   }).Select(a => new
                                                   {
                                                       a.Key.StuffId,
                                                       a.Key.BatchNumberId,
                                                       Quantity = a.Sum(b => b.Quantity)
                                                   }).ToList();


                    var AllStocks = from stock in Stocks
                                    from ordered in NotSubmittedOrderStuffs.Where(a => a.StuffId == stock.StuffId && a.BatchNumberId == stock.BatchNumberId).DefaultIfEmpty()
                                    select new
                                    {
                                        stock.StuffId,
                                        stock.BatchNumberId,
                                        stock = stock.Stock - (ordered != null ? ordered.Quantity : 0)
                                    };

                    var Today = DateTime.Now.Date;
                    List<PriceListStuff> PriceListStuffs = new List<PriceListStuff>();
                    var PriceListStuffResult = await GetPartnerPriceListStuffsAsync(PartnerId, Today);
                    if (PriceListStuffResult.Success)
                        PriceListStuffs = PriceListStuffResult.Data;

                    var AllStuffsData = _AllStuffsData.Select(a => CloneStuff(a)).ToList().Distinct().ToList();

                    var PriceData = from StuffData in AllStuffsData
                                    from PriceListStuff in PriceListStuffs.Where(a => a.StuffId == StuffData.StuffId).DefaultIfEmpty()
                                    select new { StuffData, PriceListStuff };
                    foreach (var item in PriceData)
                        if (item.PriceListStuff != null)
                        {
                            item.StuffData._UnitPrice = item.PriceListStuff.SalePrice;
                            item.StuffData._ConsumerUnitPrice = item.PriceListStuff.ConsumerPrice;
                        }

                    var StuffsWithBatchNumbers = AllStuffsData.Where(a => !a.HasBatchNumbers).ToList();
                    StuffsWithBatchNumbers.AddRange(AllStuffsData.Where(a => a.HasBatchNumbers).SelectMany(a => a.StuffRow_BatchNumberRows));

                    var StuffsArray = StuffsWithBatchNumbers.Select(a => new
                    {
                        Id = a.StuffId.ToString() + "|" + a.BatchNumberId.GetValueOrDefault(Guid.Empty).ToString(),
                        Model = a
                    }).ToArray();
                    var StocksDictionary = AllStocks.ToDictionary(a => a.StuffId.ToString() + "|" + a.BatchNumberId.ToString(), a => a.stock);
                    foreach (var item in StuffsArray)
                        item.Model._UnitStock = StocksDictionary[item.Id];// StocksDictionary.ContainsKey(item.Id) ? StocksDictionary[item.Id] : 0;

                    if (stuffId != null)
                        AllStuffsData = AllStuffsData.Where(a => a.StuffData.Id == stuffId).ToList();
                    else if (!App.ShowNotAvailableStuffsOnOrderInsertion.Value)
                        AllStuffsData = AllStuffsData.Where(a => a._UnitStock > 0).ToList();

                    foreach (var item in AllStuffsData)
                    {
                        var ShouldBeDeletedBatchNumbers = item.StuffRow_BatchNumberRows.Where(a => a._UnitStock <= 0 && (a.BatchNumberId == Guid.Empty || !App.ShowNotAvailableStuffsOnOrderInsertion.Value)).ToArray();
                        if (ShouldBeDeletedBatchNumbers.Any())
                            item.StuffRow_BatchNumberRows = item.StuffRow_BatchNumberRows.Where(a => !ShouldBeDeletedBatchNumbers.Any(b => b.BatchNumberId == a.BatchNumberId)).ToArray();
                    }

                    if (_AllStuffGroupsData != null && !WithoutGroups)
                    {
                        var ThisStuffsGroups = _AllStuffGroupsData.Where(a => AllStuffsData.Any(b => b.GroupCode == a.GroupCode)).ToList();
                        return new ResultSuccess<List<StuffListModel>[]>(true, "", new List<StuffListModel>[] { AllStuffsData, ThisStuffsGroups });
                    }
                    else
                        return new ResultSuccess<List<StuffListModel>[]>(true, "", new List<StuffListModel>[] { AllStuffsData, null });
                }
                catch (Exception err)
                {
                    return new ResultSuccess<List<StuffListModel>[]>(false, err.ProperMessage());
                }
            });
        }

        public async Task<ResultSuccess<List<StuffListModel>[]>> GetAllStuffsListForReversionAsync()
        {
            return await Task.Run(async () =>
            {
                try
                {
                    var result = await FetchStuffsListForReversionAsync();

                    if (!result.Success)
                        return new ResultSuccess<List<StuffListModel>[]>(false, result.Message);

                    var AllStuffsData = _AllStuffsDataForReversion.Select(a => CloneStuff(a)).ToList().Distinct().ToList();

                    var StuffsWithBatchNumbers = AllStuffsData.Where(a => !a.HasBatchNumbers).ToList();
                    StuffsWithBatchNumbers.AddRange(AllStuffsData.Where(a => a.HasBatchNumbers).SelectMany(a => a.StuffRow_BatchNumberRows));

                    var StuffsArray = StuffsWithBatchNumbers.Select(a => new
                    {
                        Id = a.StuffId.ToString() + "|" + a.BatchNumberId.GetValueOrDefault(Guid.Empty).ToString(),
                        Model = a
                    }).ToArray();


                    foreach (var item in AllStuffsData)
                    {
                        var ShouldBeDeletedBatchNumbers = item.StuffRow_BatchNumberRows.Where(a => a._UnitStock <= 0 && (a.BatchNumberId == Guid.Empty || !App.ShowNotAvailableStuffsOnOrderInsertion.Value)).ToArray();
                        if (ShouldBeDeletedBatchNumbers.Any())
                            item.StuffRow_BatchNumberRows = item.StuffRow_BatchNumberRows.Where(a => !ShouldBeDeletedBatchNumbers.Any(b => b.BatchNumberId == a.BatchNumberId)).ToArray();
                    }

                    return new ResultSuccess<List<StuffListModel>[]>(true, "", new List<StuffListModel>[] { AllStuffsData, null });
                }
                catch (Exception err)
                {
                    return new ResultSuccess<List<StuffListModel>[]>(false, err.ProperMessage());
                }
            });
        }

        private bool DoesScannedCodeMatchThisQR(string Code, StuffListModel stuff)
        {
            try
            {
                if (stuff.Code == "52030001".ToPersianDigits())
                {
                    var wfweethrhrth = 0;
                }

                for (int i = 0; i < App.QRScannerInVisitorAppForSelectingStuffTemplates.Length; i++)
                {
                    if (App.QRScannerInVisitorAppForSelectingStuffTemplates[i].Contains("<BatchNo>") || App.QRScannerInVisitorAppForSelectingStuffTemplates[i].Contains("<ExpDate>"))
                    {
                        for (int j = 0; j < stuff.StuffRow_BatchNumberRows.Length; j++)
                        {
                            var TemplateWithActualValues = App.QRScannerInVisitorAppForSelectingStuffTemplates[i]
                                .Replace("<Code>", stuff.Code)
                                .Replace("<BarCode>", stuff.BarCode)
                                .Replace("<BatchNo>", stuff.StuffRow_BatchNumberRows[j].BatchNumber);

                            var AllDateFormationTemplates = (stuff.StuffRow_BatchNumberRows[j].ExpirationDate != "---" ? new string[] {
                                TemplateWithActualValues.Replace("<ExpDate>", stuff.StuffRow_BatchNumberRows[j].ExpirationDate),
                                TemplateWithActualValues.Replace("<ExpDate>", stuff.StuffRow_BatchNumberRows[j].ExpirationDate.Substring(2)),
                                TemplateWithActualValues.Replace("<ExpDate>", stuff.StuffRow_BatchNumberRows[j].ExpirationDate.Substring(0, 7)),
                                TemplateWithActualValues.Replace("<ExpDate>", stuff.StuffRow_BatchNumberRows[j].ExpirationDate.Substring(0, 8) + "00"),

                                TemplateWithActualValues.Replace("<ExpDate>", stuff.StuffRow_BatchNumberRows[j].ExpirationDate.Replace("/", "-")),
                                TemplateWithActualValues.Replace("<ExpDate>", stuff.StuffRow_BatchNumberRows[j].ExpirationDate.Substring(2).Replace("/", "-")),
                                TemplateWithActualValues.Replace("<ExpDate>", stuff.StuffRow_BatchNumberRows[j].ExpirationDate.Substring(0, 7).Replace("/", "-")),
                                TemplateWithActualValues.Replace("<ExpDate>", (stuff.StuffRow_BatchNumberRows[j].ExpirationDate.Substring(0, 8) + "00").Replace("/", "-")),

                                TemplateWithActualValues.Replace("<ExpDate>", stuff.StuffRow_BatchNumberRows[j].ExpirationDate.Replace("/", "")),
                                TemplateWithActualValues.Replace("<ExpDate>", stuff.StuffRow_BatchNumberRows[j].ExpirationDate.Substring(2).Replace("/", "")),
                                TemplateWithActualValues.Replace("<ExpDate>", stuff.StuffRow_BatchNumberRows[j].ExpirationDate.Substring(0, 7).Replace("/", "")),
                                TemplateWithActualValues.Replace("<ExpDate>", (stuff.StuffRow_BatchNumberRows[j].ExpirationDate.Substring(0, 8) + "00").Replace("/", ""))
                            } : new string[] {
                                TemplateWithActualValues
                            }).Select(a => a.Split(new string[] { "<*>" }, StringSplitOptions.RemoveEmptyEntries)).ToList();

                            if (AllDateFormationTemplates.Any(b => b.All(a => Code.Contains(a.ToPersianDigits()))))
                            {
                                stuff.StuffRow_BatchNumberRows[j].QRScannedThisBatchNumber = true;
                                return true;
                            }
                        }
                    }
                    else
                    {
                        var TemplateWithActualValues = App.QRScannerInVisitorAppForSelectingStuffTemplates[i]
                            .Replace("<Code>", stuff.Code)
                            .Replace("<BarCode>", stuff.BarCode)
                            .Split(new string[] { "<*>" }, StringSplitOptions.RemoveEmptyEntries).ToList();

                        if (TemplateWithActualValues.All(a => Code.Contains(a.ToPersianDigits())))
                            return true;
                    }
                }
            }
            catch (Exception err)
            {
            }

            return false;
        }

        public async Task<List<StuffListModel>> FilterStuffsAsync(List<StuffListModel> AllStuffs, string Filter)
        {
            Filter = Filter.ToPersianDigits();
            return await Task.Run(() =>
            {
                if (!string.IsNullOrWhiteSpace(Filter))
                {
                    var TotalPoints = AllStuffs.Select(a => new KeyValuePair<StuffListModel, int>(a, 0)).ToList();
                    if (Filter.StartsWith("ScannedBarcode:"))
                    {
                        var BarCode = Filter.Replace("ScannedBarcode:", "").Replace("\n", "");
                        TotalPoints = AllStuffs.Select(a => new KeyValuePair<StuffListModel, int>(a, a.BarCode == BarCode || DoesScannedCodeMatchThisQR(BarCode, a) ? 1 : 0)).ToList();
                    }
                    else
                    {
                        var FilterSegments = Filter.ToLower().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        var WordPower = 100;
                        foreach (var FilterSegment in FilterSegments)
                        {
                            WordPower = (int)Math.Round(WordPower * 0.9);
                            var NewPoints = AllStuffs.Select(a => new KeyValuePair<StuffListModel, int>(a, WordPower * FilterSegment.Length * (
                                (a.Name.ToLower().Split(' ').Sum(b => b == FilterSegment ? 3 : b.StartsWith(FilterSegment) ? 2 : b.Contains(FilterSegment) ? 1 : 0)) +
                                (a.Description.ToLower().Split(' ').Sum(b => b == FilterSegment ? 3 : b.StartsWith(FilterSegment) ? 2 : b.Contains(FilterSegment) ? 1 : 0)) +
                                (a.Code == FilterSegment ? 10 : a.Code.StartsWith(FilterSegment) ? 2 : a.Code.Contains(FilterSegment) ? 1 : 0)
                            ))).ToList();

                            TotalPoints = (
                                from TotalPoint in TotalPoints
                                join NewPoint in NewPoints on TotalPoint.Key.Id.ToString() + (TotalPoint.Key.GroupCode != null ? TotalPoint.Key.GroupCode : "") equals NewPoint.Key.Id.ToString() + (NewPoint.Key.GroupCode != null ? NewPoint.Key.GroupCode : "")
                                select new KeyValuePair<StuffListModel, int>(TotalPoint.Key, TotalPoint.Value + NewPoint.Value)
                            ).ToList();
                        }
                    }

                    AllStuffs = (
                        from TotalPoint in TotalPoints
                        join Stuff in AllStuffs on TotalPoint.Key.Id.ToString() + (TotalPoint.Key.GroupCode != null ? TotalPoint.Key.GroupCode : "") equals Stuff.Id.ToString() + (Stuff.GroupCode != null ? Stuff.GroupCode : "")
                        where TotalPoint.Value != 0
                        orderby TotalPoint.Value descending
                        select Stuff
                    ).ToList();
                }

                return AllStuffs;
            });
        }

        public async Task<ResultSuccess<List<PriceListStuff>>> GetPartnerPriceListStuffsAsync(Guid? PartnerId, DateTime Date)
        {
            return await Task.Run(async () =>
            {
                try
                {
                    var PriceListVersions = conn.Table<PriceListVersion>()
                        .Where(a => a.BeginDate <= Date && Date <= a.EndDate).ToList();
                    if (PartnerId.HasValue)
                    {
                        var Partner = (await GetPartnerAsync(PartnerId.Value, null)).Data;

                        var PriceListZones = conn.Table<PriceListZone>().ToList().Where(a => Partner.ZoneCompleteCode.StartsWith(a.ZoneCompleteCode)).ToList();
                        PriceListVersions = PriceListVersions.Where(a => PriceListZones.Any(b => b.PriceListId == a.PriceListId)).ToList();

                        var PartnerGroups = conn.Table<DynamicGroupPartner>().Where(a => a.PartnerId == PartnerId).ToList();
                        var AllPriceListDynamicPartnerGroups = conn.Table<PriceListDynamicPartnerGroup>().ToList();
                        var PriceListDynamicPartnerGroups = AllPriceListDynamicPartnerGroups.Where(a => PartnerGroups.Any(b => b.GroupId == a.DynamicPartnerGroupId)).ToList();
                        PriceListVersions = PriceListVersions.Where(a => !AllPriceListDynamicPartnerGroups.Any(b => b.PriceListId == a.PriceListId) || PriceListDynamicPartnerGroups.Any(b => b.PriceListId == a.PriceListId)).ToList();
                    }
                    else
                    {
                        var DefaultPriceListVersionId = !string.IsNullOrEmpty(App.DefaultPriceListVersionId.Value) ? new Guid(App.DefaultPriceListVersionId.Value) : new Nullable<Guid>();
                        PriceListVersions = PriceListVersions.Where(a => a.Id == DefaultPriceListVersionId).ToList();
                    }

                    var PriceListStuffsPredicate = PredicateBuilder.False<PriceListStuff>();
                    foreach (var PriceListVersion in PriceListVersions)
                    {
                        var arg = PriceListVersion.Id;
                        PriceListStuffsPredicate = PriceListStuffsPredicate.Or(a => a.VersionId == arg);
                    }
                    var PriceListStuffs = conn.Table<PriceListStuff>().ToList().AsQueryable()
                        .Where(PriceListStuffsPredicate).ToList()
                        .GroupBy(a => a.StuffId)
                        .Select(a => a.First()).ToList();

                    return new ResultSuccess<List<PriceListStuff>>(true, "", PriceListStuffs);
                }
                catch (Exception err)
                {
                    return new ResultSuccess<List<PriceListStuff>>(false, err.ProperMessage());
                }
            });
        }

        public Partner GetPartner(Guid Id)
        {
            return conn.Table<Partner>().SingleOrDefault(a => a.Id == Id);
        }
        public Package GetPackage(Guid Id)
        {
            return conn.Table<Package>().SingleOrDefault(a => a.Id == Id);
        }
        public Package[] GetStuffPackages(Guid StuffId)
        {
            return conn.Table<Package>().Where(a => a.StuffId == StuffId).ToArray();
        }
        public DynamicGroup[] GetPartnerGroups(Guid PartnerId)
        {
            var GroupIds = conn.Table<DynamicGroupPartner>().Where(a => a.PartnerId == PartnerId).ToList().Select(a => a.GroupId).ToArray();
            var AllGroups = conn.Table<DynamicGroup>().ToList();

            return AllGroups.Where(a => GroupIds.Contains(a.Id)).ToArray();
        }
        public DynamicGroup[] GetPartnerGroups()
        {
            return conn.Table<DynamicGroup>().ToArray();
        }
        public StuffBasket[] GetStuffBaskets(Guid StuffId)
        {
            var BasketIds = conn.Table<StuffBasketStuff>().ToArray().Where(a => a.StuffId == StuffId).Select(a => a.BasketId).ToArray();
            return conn.Table<StuffBasket>().ToList().Where(a => BasketIds.Contains(a.Id)).ToArray();
        }
        public Unit GetUnit(Guid Id)
        {
            return conn.Table<Unit>().SingleOrDefault(a => a.Id == Id);
        }
        public Stuff GetStuff(Guid Id)
        {
            return conn.Table<Stuff>().SingleOrDefault(a => a.Id == Id);
        }
        public StuffBatchNumber GetBatchNumber(Guid Id)
        {
            return conn.Table<StuffBatchNumber>().SingleOrDefault(a => a.BatchNumberId == Id);
        }
        public SettlementType GetSettlementType(Guid Id)
        {
            return conn.Table<SettlementType>().SingleOrDefault(a => a.Id == Id);
        }
        public SettlementType[] GetSettlementTypes()
        {
            return conn.Table<SettlementType>().Where(a => a.Enabled).ToArray();
        }
        public SaleOrderStuff[] GetSaleOrderStuffs(Guid OrderId)
        {
            return conn.Table<SaleOrderStuff>().ToArray().Where(a => a.OrderId == OrderId).ToArray();
        }
        public CashDiscount[] GetCashDiscounts(Guid OrderId)
        {
            return conn.Table<CashDiscount>().ToArray().Where(a => a.OrderId == OrderId).ToArray();
        }
        public SaleOrder GetSaleOrder(Guid Id)
        {
            return conn.Table<SaleOrder>().SingleOrDefault(a => a.Id == Id);
        }

        private class SeperateConsideringModel
        {
            public DiscountRule Rule { get; set; }
            public Guid? StuffId { get; set; }
            public string StuffGroupCode { get; set; }
            public Guid? StuffBasketId { get; set; }
            public int? ConditionIndex { get; set; }
        }
        public async Task<List<KeyValuePair<int, RuleModel[]>>> GetDiscountRulesInModelForCalculation()
        {
            try
            {
                var __AllStuffGroups = conn.Table<StuffGroup>().ToList();
                var __AllStuffBaskets = conn.Table<StuffBasket>().ToList();
                var __AllStuffBasketStuffs = conn.Table<StuffBasketStuff>().ToList();
                var __AllStuffBasketStuffsDic = __AllStuffBasketStuffs.GroupBy(a => a.StuffId).ToDictionary(a => a.Key, a => a.Select(b => b.BasketId).ToArray());
                var __AllStuffs = conn.Table<Stuff>().ToList();

                var AllStuffGroups = __AllStuffGroups.Select(a => new
                {
                    Id = a.Id,
                    Code = a.Code
                }).ToList();

                var AllStuffBaskets = __AllStuffBaskets.ToList();

                var AllStuffs = __AllStuffs.Select(a => new
                {
                    StuffId = a.Id,
                    StuffGroupCode = a.GroupCode,
                    StuffBasketIds = __AllStuffBasketStuffsDic.ContainsKey(a.Id) ? __AllStuffBasketStuffsDic[a.Id] : new Guid[] { }
                }).ToList();

                var AllZones = (await GetZonesAsync()).Data
                    .Select(a => new
                    {
                        Id = a.Id,
                        EntityGroupId = a.EntityGroupId,
                        Code = a.EntityGroupCode
                    }).AsEnumerable();

                var Rules = conn.Table<DiscountRule>().ToList().GroupBy(a => a.Prority).Select(a => new
                {
                    Priority = a.Key,
                    Rules = a.ToArray()
                }).ToList().ToList();

                var Conditions = conn.Table<DiscountRuleCondition>().ToList();
                var Steps = conn.Table<DiscountRuleStep>().ToList();
                var SaleDiscountRuleStuffBaskets = conn.Table<SaleDiscountRuleStuffBasket>().ToList();
                var SaleDiscountRuleStuffBasketDetails = conn.Table<SaleDiscountRuleStuffBasketDetail>().ToList();
                var SaleDiscountRuleStepStuffBaskets = conn.Table<SaleDiscountRuleStepStuffBasket>().ToList();

                var RulesWithSeperateConsiderations = Rules.Select(aaa => new
                {
                    Priority = aaa.Priority,
                    Rules = aaa.Rules.SelectMany(a =>
                        a.ConsiderEachStuffSeperately ?
                            (a.ConditionOnStuff ? Conditions.Where(b => b.RuleId == a.RuleId && b.ConditionType == (int)ConditionType.Stuff)
                            .Select((b, index) => new SeperateConsideringModel()
                            {
                                Rule = a,
                                StuffId = b.ConditionParameter,
                                ConditionIndex = index
                            }).ToArray() : a.ConditionOnStuffGroup ? Conditions.Where(b => b.RuleId == a.RuleId && b.ConditionType == (int)ConditionType.StuffGroup).SelectMany(b => AllStuffs.Where(c => c.StuffGroupCode.StartsWith(AllStuffGroups.Any(d => d.Id == b.ConditionParameter) ? AllStuffGroups.SingleOrDefault(d => d.Id == b.ConditionParameter).Code : "-")))
                            .Select((b, index) => new SeperateConsideringModel()
                            {
                                Rule = a,
                                StuffId = b.StuffId,
                                ConditionIndex = index
                            }).ToArray() : a.ConditionOnStuffBasket ? Conditions.Where(b => b.RuleId == a.RuleId && b.ConditionType == (int)ConditionType.StuffBasket).SelectMany(b => AllStuffs.Where(c => c.StuffBasketIds.Any(d => d == b.ConditionParameter)))
                            .Select((b, index) => new SeperateConsideringModel()
                            {
                                Rule = a,
                                StuffId = b.StuffId,
                                ConditionIndex = index
                            }).ToArray() : new SeperateConsideringModel[1] { new SeperateConsideringModel() { Rule = a } }) :
                        a.ConsiderEachStuffGroupSeperately ?
                            a.ConditionOnStuffGroup ? Conditions.Where(b => b.RuleId == a.RuleId && b.ConditionType == (int)ConditionType.StuffGroup)
                            .SelectMany(b => AllStuffs.Where(c => c.StuffGroupCode.StartsWith(AllStuffGroups.Any(d => d.Id == b.ConditionParameter) ? AllStuffGroups.Single(d => d.Id == b.ConditionParameter).Code : "-")).GroupBy(c => c.StuffGroupCode).Select(c => c.Key))
                            .Select((b, index) => new SeperateConsideringModel()
                            {
                                Rule = a,
                                StuffGroupCode = b,
                                ConditionIndex = index
                            }).ToArray() : new SeperateConsideringModel[1] { new SeperateConsideringModel() { Rule = a } } :
                        a.ConsiderEachStuffBasketSeperately ?
                            a.ConditionOnStuffBasket ? Conditions.Where(b => b.RuleId == a.RuleId && b.ConditionType == (int)ConditionType.StuffBasket)
                            .Select((b, index) => new SeperateConsideringModel()
                            {
                                Rule = a,
                                StuffBasketId = b.ConditionParameter,
                                ConditionIndex = index
                            }).ToArray() : new SeperateConsideringModel[1] { new SeperateConsideringModel() { Rule = a } } :
                        new SeperateConsideringModel[1] { new SeperateConsideringModel() { Rule = a } }
                    ).ToArray()
                }).ToArray();

                var _SaleDiscountRuleStepStuffBaskets =
                    (from stepStuffBasket in SaleDiscountRuleStepStuffBaskets
                     from stuffBasket in SaleDiscountRuleStuffBaskets
                     where stepStuffBasket.BasketId == stuffBasket.Id
                     select new
                     {
                         stuffBasket,
                         stepStuffBasket
                     }).ToList();

                var _Baskets = (from basket in SaleDiscountRuleStuffBaskets
                                from basketDetail in SaleDiscountRuleStuffBasketDetails.Where(detail => basket.Id == detail.BasketId)
                                from stuff in __AllStuffs.Where(s => s.Id == basketDetail.StuffId).DefaultIfEmpty()
                                from stuffGroup in __AllStuffGroups.Where(sg => sg.Id == basketDetail.StuffGroupId).DefaultIfEmpty()
                                select new
                                {
                                    basket,
                                    stuffGroup,
                                    stuff
                                }).ToList().GroupBy(a => a.basket).Select(a => new
                                {
                                    basket = a.Key,
                                    stuffGroups = a.Select(b => b.stuffGroup).Where(b => b != null),
                                    stuffs = a.Select(b => b.stuff).Where(b => b != null)
                                }).ToArray();

                var DiscountRules = RulesWithSeperateConsiderations.Select(aa => new KeyValuePair<int, RuleModel[]>(aa.Priority, aa.Rules.Select(a => new RuleModel()
                {
                    RuleId = a.ConditionIndex.HasValue ? new Guid(a.Rule.RuleId.ToString().Substring(0, 24) + a.ConditionIndex.ToString().PadLeft(12, '0')) : a.Rule.RuleId,
                    RuleDescription = a.Rule.Description,
                    Priority = a.Rule.Prority + (a.ConditionIndex.HasValue ? a.ConditionIndex.Value * 0.001 : 0),
                    BeginDate = a.Rule.BeginDate,
                    EndDate = a.Rule.EndDate,
                    AggregateOtherDiscounts_Percent = a.Rule.AggregateOtherDiscounts_Percent,
                    AggregateOtherDiscounts_SpecificMoney = a.Rule.AggregateOtherDiscounts_SpecificMoney,
                    AggregateOtherDiscounts_FreeProduct = a.Rule.AggregateOtherDiscounts_FreeProduct,
                    AggregateOtherDiscounts_CashSettlement = a.Rule.AggregateOtherDiscounts_CashSettlement,
                    BasedOn = (DiscountBasedOn)a.Rule.DiscountBasedOn,
                    Way = (DiscountWay)a.Rule.DiscountWay,
                    ConditionOnPartner = a.Rule.ConditionOnPartner,
                    PartnerIds = Conditions.Where(b => a.Rule.RuleId == b.RuleId && b.ConditionType == (int)ConditionType.Partner).Select(b => b.ConditionParameter).ToArray(),
                    ConditionOnPartnerGroup = a.Rule.ConditionOnPartnerGroup,
                    PartnerGroupIds = Conditions.Where(b => a.Rule.RuleId == b.RuleId && b.ConditionType == (int)ConditionType.PartnerGroup).Select(b => b.ConditionParameter).ToArray(),
                    ConditionOnZone = a.Rule.ConditionOnZone,
                    ZoneGeneralCodes = Conditions.Where(b => a.Rule.RuleId == b.RuleId && b.ConditionType == (int)ConditionType.Zone).Select(b => b.ConditionParameter == new Guid("00000000-0000-0000-0001-000000000001") ? "" : AllZones.Any(c => c.EntityGroupId == b.ConditionParameter) ? AllZones.Single(c => c.EntityGroupId == b.ConditionParameter).Code : "---").ToArray(),
                    ConditionOnVisitor = a.Rule.ConditionOnVisitor,
                    VisitorIds = Conditions.Where(b => a.Rule.RuleId == b.RuleId && b.ConditionType == (int)ConditionType.Visitor).Select(b => b.ConditionParameter).ToArray(),
                    ConditionOnStuff = a.Rule.ConditionOnStuff || (a.Rule.ConditionOnStuffGroup && a.Rule.ConsiderEachStuffSeperately) || (a.Rule.ConditionOnStuffBasket && a.Rule.ConsiderEachStuffSeperately),
                    StuffIds = a.StuffId.HasValue ? new Guid[1] { a.StuffId.Value } : Conditions.Where(b => a.Rule.RuleId == b.RuleId && b.ConditionType == (int)ConditionType.Stuff).Select(b => b.ConditionParameter).ToArray(),
                    StuffConditionExtraParameters_UnitId = Conditions.Where(b => a.Rule.RuleId == b.RuleId && b.ConditionType == (int)ConditionType.Stuff).Select(b => !string.IsNullOrEmpty(b.ConditionParameter2) ? new Guid(b.ConditionParameter2) : new Nullable<Guid>()).ToArray(),
                    StuffConditionExtraParameters_Quantity = Conditions.Where(b => a.Rule.RuleId == b.RuleId && b.ConditionType == (int)ConditionType.Stuff).Select(b => !string.IsNullOrEmpty(b.ConditionParameter3) ? Convert.ToDecimal(b.ConditionParameter3) : new Nullable<decimal>()).ToArray(),
                    ConditionOnStuffGroup = a.Rule.ConditionOnStuffGroup,
                    StuffGroupCodes = !string.IsNullOrEmpty(a.StuffGroupCode) ? new string[1] { a.StuffGroupCode } : Conditions.Where(b => a.Rule.RuleId == b.RuleId && b.ConditionType == (int)ConditionType.StuffGroup).Select(b => AllStuffGroups.Any(c => c.Id == b.ConditionParameter) ? AllStuffGroups.Single(c => c.Id == b.ConditionParameter).Code : "").Where(c => !string.IsNullOrEmpty(c)).ToArray(),
                    ConditionOnStuffBasket = a.Rule.ConditionOnStuffBasket,
                    StuffBasketIds = a.StuffBasketId.HasValue ? new Guid[1] { a.StuffBasketId.Value } : Conditions.Where(b => a.Rule.RuleId == b.RuleId && b.ConditionType == (int)ConditionType.StuffBasket).Select(b => b.ConditionParameter).ToArray(),
                    ConditionOnSettlementType = a.Rule.ConditionOnSettlementType,
                    SettlementTypeIds = Conditions.Where(b => a.Rule.RuleId == b.RuleId && b.ConditionType == (int)ConditionType.SettlementType).Select(b => b.ConditionParameter).ToArray(),
                    ConditionOnSettlementDay = a.Rule.ConditionOnSettlementDay,
                    SettlementDay = Conditions.Any(b => a.Rule.RuleId == b.RuleId && b.ConditionType == (int)ConditionType.SettlementDay) ? Convert.ToInt32(Conditions.Single(b => a.Rule.RuleId == b.RuleId && b.ConditionType == (int)ConditionType.SettlementDay).ConditionParameter2) : new Nullable<int>(),
                    CombineStuffBaskets_SpecifyEachSoledBasket = a.Rule.CombineStuffBaskets_SpecifyEachSoledBasket.GetValueOrDefault(false),
                    CombineStuffBaskets_SpecifyEachBasketWhichGetDiscount = a.Rule.CombineStuffBaskets_SpecifyEachBasketWhichGetDiscount.GetValueOrDefault(false),
                    CombinedStuffBaskets_Bought = _Baskets.Where(b => b.basket.RuleId == a.Rule.RuleId && b.basket.Saled)
                        .Select(b => new DiscountRuleCombinedStuffBasket()
                        {
                            Id = b.basket.Id,
                            Title = b.basket.Title,
                            StuffGroupCodes = b.stuffGroups.Select(c => c.Code).ToArray(),
                            StuffIds = b.stuffs.Select(c => c.Id).ToArray()
                        }).ToArray(),
                    CombinedStuffBaskets_Discount = _Baskets.Where(b => b.basket.RuleId == a.Rule.RuleId && !b.basket.Saled)
                        .Select(b => new DiscountRuleCombinedStuffBasket()
                        {
                            Id = b.basket.Id,
                            Title = b.basket.Title,
                            StuffGroupCodes = b.stuffGroups.Select(c => c.Code).ToArray(),
                            StuffIds = b.stuffs.Select(c => c.Id).ToArray()
                        }).ToArray(),
                    Steps = Steps.Where(b => b.RuleId == a.Rule.RuleId).Select(b => new
                    {
                        StepIndex = b.StepIndex,
                        SpecificUnitId = b.QuantityStep_SpecificUnitId,
                        RowCount = b.ArticleRowCount,
                        Quantity = b.QuantityStep_Quantity,
                        Price = b.TotalPrice,
                        Percent = b.DiscountPercent,
                        SpecificMoney = b.DiscountSpecificMoney,
                        AscendingCalculation = b.AcsendingCalculation,
                        CashSettlementDay = b.CashSettlementDay,
                        ContinueToCalculateOnThisRatio = b.ContinueToCalculateOnThisRatio,
                        RatioCalculationMethod = b.RatioCalculationMethod,
                        SpecificStuffId = b.FreeProduct_SpecificStuffId,
                        UnitId = b.FreeProduct_UnitId,
                        FreeProductQuantity = b.FreeProduct_Quantity,
                        BoughtBasketIds = _SaleDiscountRuleStepStuffBaskets.Where(c => c.stepStuffBasket.StepId == b.StepId && c.stuffBasket.Saled).Select(c => c.stuffBasket.Id).ToArray(),
                        DiscountBasketIds = _SaleDiscountRuleStepStuffBaskets.Where(c => c.stepStuffBasket.StepId == b.StepId && !c.stuffBasket.Saled).Select(c => c.stuffBasket.Id).ToArray()
                    })
                    .GroupBy(b => new
                    {
                        b.RowCount,
                        b.Quantity,
                        b.Price,
                        b.Percent,
                        b.SpecificMoney,
                        b.AscendingCalculation,
                        b.CashSettlementDay,
                        b.ContinueToCalculateOnThisRatio,
                        b.RatioCalculationMethod,
                        b.SpecificStuffId,
                        b.UnitId,
                        b.FreeProductQuantity,
                        b.BoughtBasketIds,
                        b.DiscountBasketIds
                    }).Select(b => new RuleStepModel()
                    {
                        StepIndex = b.First().StepIndex,
                        SpecificUnitIds = b.Select(c => c.SpecificUnitId).ToArray(),
                        RowCount = b.Key.RowCount.GetValueOrDefault(0),
                        Quantity = b.Key.Quantity.GetValueOrDefault(0),
                        Price = b.Key.Price.GetValueOrDefault(0),
                        Percent = b.Key.Percent.GetValueOrDefault(0),
                        SpecificMoney = b.Key.SpecificMoney.GetValueOrDefault(0),
                        AscendingCalculation = b.Key.AscendingCalculation.GetValueOrDefault(false),
                        CashSettlementDay = b.Key.CashSettlementDay.GetValueOrDefault(0),
                        ContinueToCalculateOnThisRatio = b.Key.ContinueToCalculateOnThisRatio.GetValueOrDefault(false),
                        RatioCalculationMethod = (RatioCalculationMethod)b.Key.RatioCalculationMethod.GetValueOrDefault(1),
                        SpecificStuffId = b.Key.SpecificStuffId,
                        UnitId = b.Key.UnitId,
                        FreeProductQuantity = b.Key.FreeProductQuantity.GetValueOrDefault(0),
                        BoughtBasketIds = b.Key.BoughtBasketIds,
                        DiscountBasketIds = b.Key.DiscountBasketIds
                    }).OrderBy(b => b.StepIndex).ToArray()
                }).OrderBy(a => a.Priority).ToArray()
                )).OrderBy(a => a.Key).ToList();

                return DiscountRules;
            }
            catch (Exception err)
            {
                var aaa = err;
                return null;
            }
        }
        /*
        public async Task<List<KeyValuePair<int, RuleModel[]>>> GetDiscountRulesInModelForCalculation()
        {
            try
            {
                var __AllStuffGroups = conn.Table<StuffGroup>().ToList();
                var __AllStuffBaskets = conn.Table<StuffBasket>().ToList();
                var __AllStuffBasketStuffs = conn.Table<StuffBasketStuff>().ToList();
                var __AllStuffBasketStuffsDic = __AllStuffBasketStuffs.GroupBy(a => a.StuffId).ToDictionary(a => a.Key, a => a.Select(b => b.BasketId).ToArray());
                var __AllStuffs = conn.Table<Stuff>().ToList();


                var AllStuffGroups = __AllStuffGroups.Select(a => new
                    {
                        Id = a.Id,
                        Code = a.Code
                    }).ToList();

                var AllStuffBaskets = __AllStuffBaskets.ToList();

                var _AllStuffs = __AllStuffs.Select(a => new
                {
                    StuffId = a.Id,
                    StuffGroupCode = a.GroupCode,
                    StuffBasketIds = __AllStuffBasketStuffsDic.ContainsKey(a.Id) ? __AllStuffBasketStuffsDic[a.Id] : new Guid[] { }
                }).ToList();
                
                var GroupStuffs = new List<KeyValuePair<string, Guid[]>>();
                foreach (var group in AllStuffGroups)
                {
                    var StuffIds = _AllStuffs.Where(a => a.StuffGroupCode.StartsWith(group.Code)).Select(a => a.StuffId).ToArray();
                    GroupStuffs.Add(new KeyValuePair<string, Guid[]>(group.Code, StuffIds));
                }
                var GroupStuffsDic = GroupStuffs.ToDictionary(a => a.Key, a => a.Value);
                
                var BasketStuffsDic = AllStuffBaskets.Select(a => new
                {
                    BasketId = a.Id,
                    StuffIds = _AllStuffs.Where(b => b.StuffBasketIds.Contains(a.Id)).Select(b => b.StuffId).ToArray()
                }).ToDictionary(a => a.BasketId, a => a.StuffIds);

                var AllZones = (await GetZonesAsync()).Data
                    .Select(a => new
                    {
                        Id = a.Id,
                        EntityGroupId = a.EntityGroupId,
                        Code = a.EntityGroupCode
                    }).AsEnumerable();

                var Rules = conn.Table<DiscountRule>().ToList().GroupBy(a => a.Prority).Select(a => new
                {
                    Priority = a.Key,
                    Rules = a.ToArray()
                }).ToList().ToList();

                var Conditions = conn.Table<DiscountRuleCondition>().ToList();
                
                var StuffConditions = Conditions.Where(a => a.ConditionType == (int)ConditionType.Stuff).Select(a => new
                {
                    RuleId = a.RuleId,
                    StuffId = a.ConditionParameter
                }).GroupBy(a => a.RuleId).ToDictionary(a => a.Key, a => a.ToArray());

                var StuffGroupConditions = Conditions.Where(a => a.ConditionType == (int)ConditionType.StuffGroup).Select(a => new
                {
                    RuleId = a.RuleId,
                    StuffGroupCode = AllStuffGroups.Any(b => b.Id == a.ConditionParameter) ? AllStuffGroups.SingleOrDefault(b => b.Id == a.ConditionParameter).Code : "-"
                }).GroupBy(a => a.RuleId).ToDictionary(a => a.Key, a => a.ToArray());

                var StuffBasketConditions = Conditions.Where(a => a.ConditionType == (int)ConditionType.StuffBasket).Select(a => new
                {
                    RuleId = a.RuleId,
                    BasketId = a.ConditionParameter
                }).GroupBy(a => a.RuleId).ToDictionary(a => a.Key, a => a.ToArray());

                var Steps = conn.Table<DiscountRuleStep>().ToList();

                var RulesWithSeperateConsiderations = Rules.Select(aaa => new
                {
                    Priority = aaa.Priority,
                    Rules = aaa.Rules.SelectMany(a =>
                        a.ConsiderEachStuffSeperately ? (
                            a.ConditionOnStuff && StuffConditions.ContainsKey(a.RuleId) ?
                                StuffConditions[a.RuleId]
                                .Select((b, index) => new SeperateConsideringModel()
                                {
                                    Rule = a,
                                    StuffId = b.StuffId,
                                    ConditionIndex = index
                                }).ToArray() :
                            a.ConditionOnStuffGroup && StuffGroupConditions.ContainsKey(a.RuleId) ?
                                StuffGroupConditions[a.RuleId].SelectMany(b => (GroupStuffsDic.ContainsKey(b.StuffGroupCode) ? GroupStuffsDic[b.StuffGroupCode] : new Guid[] { }))
                                .Select((b, index) => new SeperateConsideringModel()
                                {
                                    Rule = a,
                                    StuffId = b,
                                    ConditionIndex = index
                                }).ToArray() :
                            a.ConditionOnStuffBasket && StuffBasketConditions.ContainsKey(a.RuleId) ?
                                StuffBasketConditions[a.RuleId].SelectMany(b => (BasketStuffsDic.ContainsKey(b.BasketId) ? BasketStuffsDic[b.BasketId] : new Guid[] { }))
                                .Select((b, index) => new SeperateConsideringModel()
                                {
                                    Rule = a,
                                    StuffId = b,
                                    ConditionIndex = index
                                }).ToArray() :
                            new SeperateConsideringModel[1] { new SeperateConsideringModel() { Rule = a } }) :
                        a.ConsiderEachStuffGroupSeperately ?
                            a.ConditionOnStuffGroup && StuffGroupConditions.ContainsKey(a.RuleId) ?
                                StuffGroupConditions[a.RuleId].SelectMany(b => AllStuffGroups.Where(c => c.Code.StartsWith(b.StuffGroupCode))).GroupBy(b => b.Code).Select(b => b.Key)
                                .Select((b, index) => new SeperateConsideringModel()
                                {
                                    Rule = a,
                                    StuffGroupCode = b,
                                    ConditionIndex = index
                                }).ToArray() :
                            new SeperateConsideringModel[1] { new SeperateConsideringModel() { Rule = a } } :
                        a.ConsiderEachStuffBasketSeperately ?
                            a.ConditionOnStuffBasket && StuffBasketConditions.ContainsKey(a.RuleId) ?
                                StuffBasketConditions[a.RuleId]
                                .Select((b, index) => new SeperateConsideringModel()
                                {
                                    Rule = a,
                                    StuffBasketId = b.BasketId,
                                    ConditionIndex = index
                                }).ToArray() : new SeperateConsideringModel[1] { new SeperateConsideringModel() { Rule = a } } :
                        new SeperateConsideringModel[1] { new SeperateConsideringModel() { Rule = a } }
                    ).ToArray()
                }).ToArray();

                List<KeyValuePair<int, RuleModel[]>> DiscountRules = new List<KeyValuePair<int, RuleModel[]>>();
                foreach (var aa in RulesWithSeperateConsiderations)
                {
                    var item = new KeyValuePair<int, RuleModel[]>(aa.Priority, aa.Rules.Select(a => new RuleModel()
                    {
                        //RuleId = a.ConditionIndex.HasValue ? new Guid(a.Rule.RuleId.ToString().Substring(0, 24) + a.ConditionIndex.ToString().PadLeft(12, '0')) : a.Rule.RuleId,
                        //RuleDescription = a.Rule.Description,
                        //Priority = a.Rule.Prority + (a.ConditionIndex.HasValue ? a.ConditionIndex.Value * 0.001 : 0),
                        //BeginDate = a.Rule.BeginDate,
                        //EndDate = a.Rule.EndDate,
                        //AggregateOtherDiscounts_Percent = a.Rule.AggregateOtherDiscounts_Percent,
                        //AggregateOtherDiscounts_SpecificMoney = a.Rule.AggregateOtherDiscounts_SpecificMoney,
                        //AggregateOtherDiscounts_FreeProduct = a.Rule.AggregateOtherDiscounts_FreeProduct,
                        //AggregateOtherDiscounts_CashSettlement = a.Rule.AggregateOtherDiscounts_CashSettlement,
                        //BasedOn = (DiscountBasedOn)a.Rule.DiscountBasedOn,
                        //Way = (DiscountWay)a.Rule.DiscountWay,
                        //ConditionOnPartner = a.Rule.ConditionOnPartner,
                        //PartnerIds = Conditions.Where(b => a.Rule.RuleId == b.RuleId && b.ConditionType == (int)ConditionType.Partner).Select(b => b.ConditionParameter).ToArray(),
                        //ConditionOnPartnerGroup = a.Rule.ConditionOnPartnerGroup,
                        //PartnerGroupIds = Conditions.Where(b => a.Rule.RuleId == b.RuleId && b.ConditionType == (int)ConditionType.PartnerGroup).Select(b => b.ConditionParameter).ToArray(),
                        //ConditionOnZone = a.Rule.ConditionOnZone,
                        //ZoneGeneralCodes = Conditions.Where(b => a.Rule.RuleId == b.RuleId && b.ConditionType == (int)ConditionType.Zone).Select(b => b.ConditionParameter == new Guid("00000000-0000-0000-0001-000000000001") ? "" : AllZones.Any(c => c.EntityGroupId == b.ConditionParameter) ? AllZones.Single(c => c.EntityGroupId == b.ConditionParameter).Code : "---").ToArray(),
                        //ConditionOnVisitor = a.Rule.ConditionOnVisitor,
                        //VisitorIds = Conditions.Where(b => a.Rule.RuleId == b.RuleId && b.ConditionType == (int)ConditionType.Visitor).Select(b => b.ConditionParameter).ToArray(),
                        //ConditionOnStuff = a.Rule.ConditionOnStuff || (a.Rule.ConditionOnStuffGroup && a.Rule.ConsiderEachStuffSeperately) || (a.Rule.ConditionOnStuffBasket && a.Rule.ConsiderEachStuffSeperately),
                        //StuffIds = a.StuffId.HasValue ? new Guid[1] { a.StuffId.Value } : Conditions.Where(b => a.Rule.RuleId == b.RuleId && b.ConditionType == (int)ConditionType.Stuff).Select(b => b.ConditionParameter).ToArray(),
                        //StuffConditionExtraParameters_UnitId = Conditions.Where(b => a.Rule.RuleId == b.RuleId && b.ConditionType == (int)ConditionType.Stuff).Select(b => !string.IsNullOrEmpty(b.ConditionParameter2) ? new Guid(b.ConditionParameter2) : new Nullable<Guid>()).ToArray(),
                        //StuffConditionExtraParameters_Quantity = Conditions.Where(b => a.Rule.RuleId == b.RuleId && b.ConditionType == (int)ConditionType.Stuff).Select(b => !string.IsNullOrEmpty(b.ConditionParameter3) ? Convert.ToDecimal(b.ConditionParameter3) : new Nullable<decimal>()).ToArray(),
                        //ConditionOnStuffGroup = a.Rule.ConditionOnStuffGroup,
                        //StuffGroupCodes = !string.IsNullOrEmpty(a.StuffGroupCode) ? new string[1] { a.StuffGroupCode } : Conditions.Where(b => a.Rule.RuleId == b.RuleId && b.ConditionType == (int)ConditionType.StuffGroup).Select(b => AllStuffGroups.Any(c => c.Id == b.ConditionParameter) ? AllStuffGroups.Single(c => c.Id == b.ConditionParameter).Code : "").Where(c => !string.IsNullOrEmpty(c)).ToArray(),
                        //ConditionOnStuffBasket = a.Rule.ConditionOnStuffBasket,
                        //StuffBasketIds = a.StuffBasketId.HasValue ? new Guid[1] { a.StuffBasketId.Value } : Conditions.Where(b => a.Rule.RuleId == b.RuleId && b.ConditionType == (int)ConditionType.StuffBasket).Select(b => b.ConditionParameter).ToArray(),
                        //ConditionOnSettlementType = a.Rule.ConditionOnSettlementType,
                        //SettlementTypeIds = Conditions.Where(b => a.Rule.RuleId == b.RuleId && b.ConditionType == (int)ConditionType.SettlementType).Select(b => b.ConditionParameter).ToArray(),
                        //ConditionOnSettlementDay = a.Rule.ConditionOnSettlementDay,
                        //SettlementDay = Conditions.Any(b => a.Rule.RuleId == b.RuleId && b.ConditionType == (int)ConditionType.SettlementDay) ? Convert.ToInt32(Conditions.Single(b => a.Rule.RuleId == b.RuleId && b.ConditionType == (int)ConditionType.SettlementDay).ConditionParameter2) : new Nullable<int>(),
                        //Steps = Steps.Where(b => b.RuleId == a.Rule.RuleId).Select(b => new
                        //{
                        //    StepIndex = b.StepIndex,
                        //    SpecificUnitId = b.QuantityStep_SpecificUnitId,
                        //    RowCount = b.ArticleRowCount,
                        //    Quantity = b.QuantityStep_Quantity,
                        //    Price = b.TotalPrice,
                        //    Percent = b.DiscountPercent,
                        //    SpecificMoney = b.DiscountSpecificMoney,
                        //    AscendingCalculation = b.AcsendingCalculation,
                        //    CashSettlementDay = b.CashSettlementDay,
                        //    ContinueToCalculateOnThisRatio = b.ContinueToCalculateOnThisRatio,
                        //    RatioCalculationMethod = b.RatioCalculationMethod,
                        //    SpecificStuffId = b.FreeProduct_SpecificStuffId,
                        //    UnitId = b.FreeProduct_UnitId,
                        //    FreeProductQuantity = b.FreeProduct_Quantity
                        //})
                        //.GroupBy(b => new
                        //{
                        //    b.RowCount,
                        //    b.Quantity,
                        //    b.Price,
                        //    b.Percent,
                        //    b.SpecificMoney,
                        //    b.AscendingCalculation,
                        //    b.CashSettlementDay,
                        //    b.ContinueToCalculateOnThisRatio,
                        //    b.RatioCalculationMethod,
                        //    b.SpecificStuffId,
                        //    b.UnitId,
                        //    b.FreeProductQuantity
                        //}).Select(b => new RuleStepModel()
                        //{
                        //    StepIndex = b.First().StepIndex,
                        //    SpecificUnitIds = b.Select(c => c.SpecificUnitId).ToArray(),
                        //    RowCount = b.Key.RowCount.GetValueOrDefault(0),
                        //    Quantity = b.Key.Quantity.GetValueOrDefault(0),
                        //    Price = b.Key.Price.GetValueOrDefault(0),
                        //    Percent = b.Key.Percent.GetValueOrDefault(0),
                        //    SpecificMoney = b.Key.SpecificMoney.GetValueOrDefault(0),
                        //    AscendingCalculation = b.Key.AscendingCalculation.GetValueOrDefault(false),
                        //    CashSettlementDay = b.Key.CashSettlementDay.GetValueOrDefault(0),
                        //    ContinueToCalculateOnThisRatio = b.Key.ContinueToCalculateOnThisRatio.GetValueOrDefault(false),
                        //    RatioCalculationMethod = (RatioCalculationMethod)b.Key.RatioCalculationMethod.GetValueOrDefault(1),
                        //    SpecificStuffId = b.Key.SpecificStuffId,
                        //    UnitId = b.Key.UnitId,
                        //    FreeProductQuantity = b.Key.FreeProductQuantity.GetValueOrDefault(0)
                        //}).OrderBy(b => b.StepIndex).ToArray()
                    }).ToArray());
                    DiscountRules.Add(item);
                }

                DiscountRules = RulesWithSeperateConsiderations.Select(aa => new KeyValuePair<int, RuleModel[]>(aa.Priority, aa.Rules.Select(a => new RuleModel()
                {
                    RuleId = a.ConditionIndex.HasValue ? new Guid(a.Rule.RuleId.ToString().Substring(0, 24) + a.ConditionIndex.ToString().PadLeft(12, '0')) : a.Rule.RuleId,
                    RuleDescription = a.Rule.Description,
                    Priority = a.Rule.Prority + (a.ConditionIndex.HasValue ? a.ConditionIndex.Value * 0.001 : 0),
                    BeginDate = a.Rule.BeginDate,
                    EndDate = a.Rule.EndDate,
                    AggregateOtherDiscounts_Percent = a.Rule.AggregateOtherDiscounts_Percent,
                    AggregateOtherDiscounts_SpecificMoney = a.Rule.AggregateOtherDiscounts_SpecificMoney,
                    AggregateOtherDiscounts_FreeProduct = a.Rule.AggregateOtherDiscounts_FreeProduct,
                    AggregateOtherDiscounts_CashSettlement = a.Rule.AggregateOtherDiscounts_CashSettlement,
                    BasedOn = (DiscountBasedOn)a.Rule.DiscountBasedOn,
                    //Way = (DiscountWay)a.Rule.DiscountWay,
                    //ConditionOnPartner = a.Rule.ConditionOnPartner,
                    //PartnerIds = Conditions.Where(b => a.Rule.RuleId == b.RuleId && b.ConditionType == (int)ConditionType.Partner).Select(b => b.ConditionParameter).ToArray(),
                    //ConditionOnPartnerGroup = a.Rule.ConditionOnPartnerGroup,
                    //PartnerGroupIds = Conditions.Where(b => a.Rule.RuleId == b.RuleId && b.ConditionType == (int)ConditionType.PartnerGroup).Select(b => b.ConditionParameter).ToArray(),
                    //ConditionOnZone = a.Rule.ConditionOnZone,
                    //ZoneGeneralCodes = Conditions.Where(b => a.Rule.RuleId == b.RuleId && b.ConditionType == (int)ConditionType.Zone).Select(b => b.ConditionParameter == new Guid("00000000-0000-0000-0001-000000000001") ? "" : AllZones.Any(c => c.EntityGroupId == b.ConditionParameter) ? AllZones.Single(c => c.EntityGroupId == b.ConditionParameter).Code : "---").ToArray(),
                    //ConditionOnVisitor = a.Rule.ConditionOnVisitor,
                    //VisitorIds = Conditions.Where(b => a.Rule.RuleId == b.RuleId && b.ConditionType == (int)ConditionType.Visitor).Select(b => b.ConditionParameter).ToArray(),
                    //ConditionOnStuff = a.Rule.ConditionOnStuff || (a.Rule.ConditionOnStuffGroup && a.Rule.ConsiderEachStuffSeperately) || (a.Rule.ConditionOnStuffBasket && a.Rule.ConsiderEachStuffSeperately),
                    //StuffIds = a.StuffId.HasValue ? new Guid[1] { a.StuffId.Value } : Conditions.Where(b => a.Rule.RuleId == b.RuleId && b.ConditionType == (int)ConditionType.Stuff).Select(b => b.ConditionParameter).ToArray(),
                    //StuffConditionExtraParameters_UnitId = Conditions.Where(b => a.Rule.RuleId == b.RuleId && b.ConditionType == (int)ConditionType.Stuff).Select(b => !string.IsNullOrEmpty(b.ConditionParameter2) ? new Guid(b.ConditionParameter2) : new Nullable<Guid>()).ToArray(),
                    //StuffConditionExtraParameters_Quantity = Conditions.Where(b => a.Rule.RuleId == b.RuleId && b.ConditionType == (int)ConditionType.Stuff).Select(b => !string.IsNullOrEmpty(b.ConditionParameter3) ? Convert.ToDecimal(b.ConditionParameter3) : new Nullable<decimal>()).ToArray(),
                    //ConditionOnStuffGroup = a.Rule.ConditionOnStuffGroup,
                    //StuffGroupCodes = !string.IsNullOrEmpty(a.StuffGroupCode) ? new string[1] { a.StuffGroupCode } : Conditions.Where(b => a.Rule.RuleId == b.RuleId && b.ConditionType == (int)ConditionType.StuffGroup).Select(b => AllStuffGroups.Any(c => c.Id == b.ConditionParameter) ? AllStuffGroups.Single(c => c.Id == b.ConditionParameter).Code : "").Where(c => !string.IsNullOrEmpty(c)).ToArray(),
                    //ConditionOnStuffBasket = a.Rule.ConditionOnStuffBasket,
                    //StuffBasketIds = a.StuffBasketId.HasValue ? new Guid[1] { a.StuffBasketId.Value } : Conditions.Where(b => a.Rule.RuleId == b.RuleId && b.ConditionType == (int)ConditionType.StuffBasket).Select(b => b.ConditionParameter).ToArray(),
                    //ConditionOnSettlementType = a.Rule.ConditionOnSettlementType,
                    //SettlementTypeIds = Conditions.Where(b => a.Rule.RuleId == b.RuleId && b.ConditionType == (int)ConditionType.SettlementType).Select(b => b.ConditionParameter).ToArray(),
                    //ConditionOnSettlementDay = a.Rule.ConditionOnSettlementDay,
                    //SettlementDay = Conditions.Any(b => a.Rule.RuleId == b.RuleId && b.ConditionType == (int)ConditionType.SettlementDay) ? Convert.ToInt32(Conditions.Single(b => a.Rule.RuleId == b.RuleId && b.ConditionType == (int)ConditionType.SettlementDay).ConditionParameter2) : new Nullable<int>(),
                    //Steps = Steps.Where(b => b.RuleId == a.Rule.RuleId).Select(b => new
                    //{
                    //    StepIndex = b.StepIndex,
                    //    SpecificUnitId = b.QuantityStep_SpecificUnitId,
                    //    RowCount = b.ArticleRowCount,
                    //    Quantity = b.QuantityStep_Quantity,
                    //    Price = b.TotalPrice,
                    //    Percent = b.DiscountPercent,
                    //    SpecificMoney = b.DiscountSpecificMoney,
                    //    AscendingCalculation = b.AcsendingCalculation,
                    //    CashSettlementDay = b.CashSettlementDay,
                    //    ContinueToCalculateOnThisRatio = b.ContinueToCalculateOnThisRatio,
                    //    RatioCalculationMethod = b.RatioCalculationMethod,
                    //    SpecificStuffId = b.FreeProduct_SpecificStuffId,
                    //    UnitId = b.FreeProduct_UnitId,
                    //    FreeProductQuantity = b.FreeProduct_Quantity
                    //})
                    //.GroupBy(b => new
                    //{
                    //    b.RowCount,
                    //    b.Quantity,
                    //    b.Price,
                    //    b.Percent,
                    //    b.SpecificMoney,
                    //    b.AscendingCalculation,
                    //    b.CashSettlementDay,
                    //    b.ContinueToCalculateOnThisRatio,
                    //    b.RatioCalculationMethod,
                    //    b.SpecificStuffId,
                    //    b.UnitId,
                    //    b.FreeProductQuantity
                    //}).Select(b => new RuleStepModel()
                    //{
                    //    StepIndex = b.First().StepIndex,
                    //    SpecificUnitIds = b.Select(c => c.SpecificUnitId).ToArray(),
                    //    RowCount = b.Key.RowCount.GetValueOrDefault(0),
                    //    Quantity = b.Key.Quantity.GetValueOrDefault(0),
                    //    Price = b.Key.Price.GetValueOrDefault(0),
                    //    Percent = b.Key.Percent.GetValueOrDefault(0),
                    //    SpecificMoney = b.Key.SpecificMoney.GetValueOrDefault(0),
                    //    AscendingCalculation = b.Key.AscendingCalculation.GetValueOrDefault(false),
                    //    CashSettlementDay = b.Key.CashSettlementDay.GetValueOrDefault(0),
                    //    ContinueToCalculateOnThisRatio = b.Key.ContinueToCalculateOnThisRatio.GetValueOrDefault(false),
                    //    RatioCalculationMethod = (RatioCalculationMethod)b.Key.RatioCalculationMethod.GetValueOrDefault(1),
                    //    SpecificStuffId = b.Key.SpecificStuffId,
                    //    UnitId = b.Key.UnitId,
                    //    FreeProductQuantity = b.Key.FreeProductQuantity.GetValueOrDefault(0)
                    //}).OrderBy(b => b.StepIndex).ToArray()
                }).OrderBy(a => a.Priority).ToArray()
                )).OrderBy(a => a.Key).ToList();

                return DiscountRules;
            }
            catch (Exception err)
            {
                var aaa = err;
                return null;
            }
        }
        */

        public async Task<List<KeyValuePair<int, RuleModel[]>>> GeDiscountRulesInModelForCalculation_ForSaman()
        {
            try
            {
                var AllStuffs = (await GetStuffsAsync()).Data
                     .Select(a => new
                     {
                         StuffId = a.Id,
                         StuffGroupCode = a.GroupCode,
                         StuffBasketIds = a.Baskets.Select(b => b.Id)
                     }).AsEnumerable();
                var AllStuffGroups = (await GetStuffGroupsAsync()).Data
                    .Select(a => new
                    {
                        Id = a.Id,
                        Code = a.Code
                    }).AsEnumerable();
                var AllZones = (await GetZonesAsync()).Data
                    .Select(a => new
                    {
                        Id = a.Id,
                        EntityGroupId = a.EntityGroupId,
                        Code = a.EntityGroupCode
                    }).AsEnumerable();

                var Rules = conn.Table<DiscountRule>().ToList().GroupBy(a => a.Prority).Select(a => new
                {
                    Priority = a.Key,
                    Rules = a
                }).ToList();

                var Conditions = conn.Table<DiscountRuleCondition>().ToList();
                var Steps = conn.Table<DiscountRuleStep>().ToList();

                var RulesWithSeperateConsiderations = Rules.Select(aaa => new
                {
                    Priority = aaa.Priority,
                    Rules = aaa.Rules.SelectMany(a =>
                        new SeperateConsideringModel[1] { new SeperateConsideringModel() { Rule = a } }
                    )
                });

                var DiscountRules = RulesWithSeperateConsiderations.AsEnumerable().Select(aa => new KeyValuePair<int, RuleModel[]>(aa.Priority, aa.Rules.Select(a => new RuleModel()
                {
                    RuleId = a.ConditionIndex.HasValue ? new Guid(a.Rule.RuleId.ToString().Substring(0, 24) + a.ConditionIndex.ToString().PadLeft(12, '0')) : a.Rule.RuleId,
                    RuleDescription = a.Rule.Description,
                    Priority = a.Rule.Prority + (a.ConditionIndex.HasValue ? a.ConditionIndex.Value * 0.001 : 0),
                    BeginDate = a.Rule.BeginDate,
                    EndDate = a.Rule.EndDate,
                    AggregateOtherDiscounts_Percent = a.Rule.AggregateOtherDiscounts_Percent,
                    AggregateOtherDiscounts_SpecificMoney = a.Rule.AggregateOtherDiscounts_SpecificMoney,
                    AggregateOtherDiscounts_FreeProduct = a.Rule.AggregateOtherDiscounts_FreeProduct,
                    AggregateOtherDiscounts_CashSettlement = a.Rule.AggregateOtherDiscounts_CashSettlement,
                    BasedOn = (DiscountBasedOn)a.Rule.DiscountBasedOn,
                    Way = (DiscountWay)a.Rule.DiscountWay,
                    ConditionOnPartner = a.Rule.ConditionOnPartner,
                    PartnerIds = Conditions.Where(b => a.Rule.RuleId == b.RuleId && b.ConditionType == (int)ConditionType.Partner).Select(b => b.ConditionParameter).ToArray(),
                    ConditionOnPartnerGroup = a.Rule.ConditionOnPartnerGroup,
                    PartnerGroupIds = Conditions.Where(b => a.Rule.RuleId == b.RuleId && b.ConditionType == (int)ConditionType.PartnerGroup).Select(b => b.ConditionParameter).ToArray(),
                    ConditionOnZone = a.Rule.ConditionOnZone,
                    ZoneGeneralCodes = Conditions.Where(b => a.Rule.RuleId == b.RuleId && b.ConditionType == (int)ConditionType.Zone).Select(b => AllZones.Single(c => c.EntityGroupId == b.ConditionParameter).Code).ToArray(),
                    ConditionOnVisitor = a.Rule.ConditionOnVisitor,
                    VisitorIds = Conditions.Where(b => a.Rule.RuleId == b.RuleId && b.ConditionType == (int)ConditionType.Visitor).Select(b => b.ConditionParameter).ToArray(),
                    ConditionOnStuff = a.Rule.ConditionOnStuff || (a.Rule.ConditionOnStuffGroup && a.Rule.ConsiderEachStuffSeperately) || (a.Rule.ConditionOnStuffBasket && a.Rule.ConsiderEachStuffSeperately),
                    StuffIds = a.StuffId.HasValue ? new Guid[1] { a.StuffId.Value } : Conditions.Where(b => a.Rule.RuleId == b.RuleId && b.ConditionType == (int)ConditionType.Stuff).Select(b => b.ConditionParameter).ToArray(),
                    ConditionOnStuffGroup = a.Rule.ConditionOnStuffGroup,
                    StuffGroupCodes = !string.IsNullOrEmpty(a.StuffGroupCode) ? new string[1] { a.StuffGroupCode } : Conditions.Where(b => a.Rule.RuleId == b.RuleId && b.ConditionType == (int)ConditionType.StuffGroup).Select(b => AllStuffGroups.Single(c => c.Id == b.ConditionParameter).Code).ToArray(),
                    ConditionOnStuffBasket = a.Rule.ConditionOnStuffBasket,
                    StuffBasketIds = a.StuffBasketId.HasValue ? new Guid[1] { a.StuffBasketId.Value } : Conditions.Where(b => a.Rule.RuleId == b.RuleId && b.ConditionType == (int)ConditionType.StuffBasket).Select(b => b.ConditionParameter).ToArray(),
                    ConditionOnSettlementType = a.Rule.ConditionOnSettlementType,
                    SettlementTypeIds = Conditions.Where(b => a.Rule.RuleId == b.RuleId && b.ConditionType == (int)ConditionType.SettlementType).Select(b => b.ConditionParameter).ToArray(),
                    ConditionOnSettlementDay = a.Rule.ConditionOnSettlementDay,
                    SettlementDay = Conditions.Any(b => a.Rule.RuleId == b.RuleId && b.ConditionType == (int)ConditionType.SettlementDay) ? Convert.ToInt32(Conditions.Single(b => a.Rule.RuleId == b.RuleId && b.ConditionType == (int)ConditionType.SettlementDay).ConditionParameter2) : new Nullable<int>(),
                    ShowDiscountInUnitFee = a.Rule.ShowDiscountInUnitFee,
                    Steps = Steps.Where(b => b.RuleId == a.Rule.RuleId).Select(b => new
                    {
                        StepIndex = b.StepIndex,
                        SpecificUnitId = b.QuantityStep_SpecificUnitId,
                        RowCount = b.ArticleRowCount,
                        Quantity = b.QuantityStep_Quantity,
                        Price = b.TotalPrice,
                        Percent = b.DiscountPercent,
                        SpecificMoney = b.DiscountSpecificMoney,
                        AscendingCalculation = b.AcsendingCalculation,
                        CashSettlementDay = b.CashSettlementDay,
                        ContinueToCalculateOnThisRatio = b.ContinueToCalculateOnThisRatio,
                        RatioCalculationMethod = b.RatioCalculationMethod,
                        SpecificStuffId = b.FreeProduct_SpecificStuffId,
                        UnitId = b.FreeProduct_UnitId,
                        FreeProductQuantity = b.FreeProduct_Quantity
                    })
                    .GroupBy(b => new
                    {
                        b.RowCount,
                        b.Quantity,
                        b.Price,
                        b.Percent,
                        b.SpecificMoney,
                        b.AscendingCalculation,
                        b.CashSettlementDay,
                        b.ContinueToCalculateOnThisRatio,
                        b.RatioCalculationMethod,
                        b.SpecificStuffId,
                        b.UnitId,
                        b.FreeProductQuantity
                    }).Select(b => new RuleStepModel()
                    {
                        StepIndex = b.First().StepIndex,
                        SpecificUnitIds = b.Select(c => c.SpecificUnitId).ToArray(),
                        RowCount = b.Key.RowCount.GetValueOrDefault(0),
                        Quantity = b.Key.Quantity.GetValueOrDefault(0),
                        Price = b.Key.Price.GetValueOrDefault(0),
                        Percent = b.Key.Percent.GetValueOrDefault(0),
                        SpecificMoney = b.Key.SpecificMoney.GetValueOrDefault(0),
                        AscendingCalculation = b.Key.AscendingCalculation.GetValueOrDefault(false),
                        CashSettlementDay = b.Key.CashSettlementDay.GetValueOrDefault(0),
                        ContinueToCalculateOnThisRatio = b.Key.ContinueToCalculateOnThisRatio.GetValueOrDefault(false),
                        RatioCalculationMethod = (RatioCalculationMethod)b.Key.RatioCalculationMethod.GetValueOrDefault(1),
                        SpecificStuffId = b.Key.SpecificStuffId,
                        UnitId = b.Key.UnitId,
                        FreeProductQuantity = b.Key.FreeProductQuantity.GetValueOrDefault(0)
                    }).OrderBy(b => b.StepIndex).ToArray()
                }).OrderBy(a => a.Priority).ToArray()
                )).OrderBy(a => a.Key).ToList();

                return DiscountRules;
            }
            catch (Exception err)
            {
                var aaa = err;
                return null;
            }
        }

        public static Dictionary<int, Dictionary<string, List<RuleModel>>> FetchedDiscountRules = null;
        public void ClearFetchedDiscountRules()
        {
            FetchedDiscountRules = null;
        }
        public async Task<Dictionary<int, Dictionary<string, List<RuleModel>>>> GetDiscountRulesAsync()
        {
            //if (FetchedDiscountRules == null)
            //{
            var Rules = App.SystemName.Value == "Saman" ? await GeDiscountRulesInModelForCalculation_ForSaman() : await GetDiscountRulesInModelForCalculation();
            if (Rules == null)
                return null;

            FetchedDiscountRules = Rules.Select(a => new
            {
                key = a.Key,
                value = a.Value
                    .GroupBy(b => b.RuleId.ToString().Substring(0, 24))
                    .Select(b => new
                    {
                        RuleId = b.Key,
                        Rules = b.ToList()
                    }).ToDictionary(aa => aa.RuleId, aa => aa.Rules)
            }).ToDictionary(a => a.key, a => a.value);
            //}

            return FetchedDiscountRules;
        }

        public SaleOrder CalculateProporatedDiscount(SaleOrder SaleOrder)
        {
            var SaleOrderStuffs = SaleOrder.SaleOrderStuffs;

            var UsualDiscount = SaleOrder.DiscountAmount + SaleOrder.CashPrise;
            var TotalAfterReducingRowDiscounts = SaleOrder.StuffsPriceSum - SaleOrder.StuffsRowDiscountSum;
            decimal AfterRowDiscountReduction_Sum = 0;
            for (int i = 0; i < SaleOrderStuffs.Length; i++)
            {
                var Article = SaleOrderStuffs[i];
                Article.ProporatedDiscount = TotalAfterReducingRowDiscounts == 0 ? 0 : Math.Round(UsualDiscount * (Article.SalePriceQuantity - Article.DiscountAmount) / TotalAfterReducingRowDiscounts);
                AfterRowDiscountReduction_Sum += Article.SalePriceQuantity - Article.DiscountAmount - Article.ProporatedDiscount;
            }

            return SaleOrder;
        }


    }

    public class StockHelper
    {
        public Guid StuffId { get; set; }
        public Guid BatchNumberId { get; set; }
        public decimal stock { get; set; }
    }
}


