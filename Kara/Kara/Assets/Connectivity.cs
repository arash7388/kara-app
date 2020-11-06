using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Kara.Assets;
using Kara.Models;
using Plugin.Settings;
using Kara.Helpers;
using System.IO;
using Xamarin.Forms;

namespace Kara.Assets
{
    public static class Connectivity
    {
        private static string ServerRoot { get { return "http://" + App.ServerAddress + "/MobileApp/General/"; } }
        private static HttpClient _HttpClient;
        public static HttpClient HttpClient
        {
            get
            {
                if (_HttpClient == null)
                {
                    _HttpClient = new HttpClient();
                    _HttpClient.Timeout = new TimeSpan(0, 1, 0);
                    _HttpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                }
                return _HttpClient;
            }
        }

        public static async Task<ResultSuccess<LoginResult>> Login(string Username, string Password)
        {
            try
            {
                var resultTask = await HttpClient.GetStringAsyncForUnicode(ServerRoot + "Login?Username=" + Username + "&Password=" + Password);
                if (!resultTask.Success)
                    throw new Exception(resultTask.Message);
                var result = JsonConvert.DeserializeObject<ResultSuccess<LoginResult>>(resultTask.Data);
                return result;
            }
            catch (Exception err)
            {
                return new ResultSuccess<LoginResult>(false, err.ProperMessage());
            }
        }

        public static async Task<ResultSuccess> UpdateDB(UpdateItemModel UpdateItem)
        {
            var result1 = App.DB.InitTransaction();
            if (!result1.Success)
                return new ResultSuccess(false, result1.Message);
            try
            {
                PartialDBUpdater.HttpClient = HttpClient;
                PartialDBUpdater.ServerRoot = ServerRoot;

                var RequestId = Guid.NewGuid();
                var From = 1;
                var Count = UpdateItem.FetchRecordCountPerRequest;
                while (true)
                {
                    var PartialUpdateResultTask = UpdateItem.PartialDBUpdater.UpdateItemFromServer(RequestId, From, Count);
                    var PartialUpdateResult = await PartialUpdateResultTask;
                    if (PartialUpdateResultTask.Exception != null)
                    {
                        var result2 = App.DB.RollbackTransaction();
                        if (!result2.Success)
                            return new ResultSuccess(false, result2.Message);
                        return new ResultSuccess(false, PartialUpdateResultTask.Exception.ProperMessage());
                    }
                    if (!PartialUpdateResult.Success)
                    {
                        var result2 = App.DB.RollbackTransaction();
                        if (!result2.Success)
                            return new ResultSuccess(false, result2.Message);
                        return new ResultSuccess(false, PartialUpdateResult.Message);
                    }
                    if (From + Count > PartialUpdateResult.Data)
                    {
                        UpdateItem.Progress = 1;
                        var result3 = App.DB.CommitTransaction();
                        if (!result3.Success)
                            return new ResultSuccess(false, result3.Message);
                        return new ResultSuccess(true, "");
                    }

                    UpdateItem.Progress = From * 1.0 / PartialUpdateResult.Data;
                    From += Count;
                }

                if (UpdateItem.PartialDBUpdater.GetType().Equals(typeof(PartialUpdateDB_Stocks)))
                {
                    var Warehouses = (await App.DB.GetWarehousesAsync()).Data.ToArray();

                    if (Warehouses.Any())
                    {
                        if (Warehouses.Length == 1)
                            App.DefaultWarehouseId.Value = Warehouses.Single().WarehouseId.ToString();
                    }

                    if (!string.IsNullOrEmpty(App.DefaultWarehouseId.Value) && !Warehouses.Any(a => a.WarehouseId.ToString().ToLower() == App.DefaultWarehouseId.Value.ToLower()))
                    {
                        App.DefaultWarehouseId.Value = null;
                    }
                }
            }
            catch (Exception err)
            {
                var result4 = App.DB.RollbackTransaction();
                if (!result4.Success)
                    return new ResultSuccess(false, result4.Message);
                return new ResultSuccess(false, err.ProperMessage());
            }
        }

        public class PartnerCycleDetailModel
        {
            public DateTime Date { get; set; }
            public string Description { get; set; }
            public decimal Debtor { get; set; }
            public decimal Creditor { get; set; }
            public decimal Remainder { get; set; }
        }
        public class UncashedChequeModel
        {
            public string BackNumber { get; set; }
            public string Serial { get; set; }
            public DateTime MaturityDate { get; set; }
            public decimal Price { get; set; }
            public string State { get; set; }
            public string Description { get; set; }
        }
        public class ReturnedChequeModel
        {
            public string BackNumber { get; set; }
            public string Serial { get; set; }
            public DateTime MaturityDate { get; set; }
            public decimal Price { get; set; }
            public string Description { get; set; }
        }
        public class PartnerCycleInformationModel
        {
            public decimal Remainder { get; set; }
            public int UncashedChequesCount { get; set; }
            public decimal UncashedChequesPrice { get; set; }
            public int ReturnedChequesCount { get; set; }
            public decimal ReturnedChequesPrice { get; set; }
            public UncashedChequeModel[] UncashedChequesList { get; set; }
            public ReturnedChequeModel[] ReturnedChequesList { get; set; }
            public PartnerCycleDetailModel[] PartnerCycleDetail { get; set; }
        }
        static KeyValuePair<Guid, PartnerCycleInformationModel> LastFetchedData = new KeyValuePair<Guid, PartnerCycleInformationModel>(Guid.Empty, null);
        static bool GetPartnerCycleInformationFromServerAsyncWorking = false;
        public static async Task<ResultSuccess<PartnerCycleInformationModel>> GetPartnerCycleInformationFromServerAsync(Guid PartnerId, bool Refresh)
        {
            try
            {
                var Counter = 1;
                while (GetPartnerCycleInformationFromServerAsyncWorking)
                {
                    await Task.Delay(100);
                    Counter++;
                    if (Counter > 200)
                        break;
                }

                GetPartnerCycleInformationFromServerAsyncWorking = true;

                var CurrentData = !Refresh && LastFetchedData.Key == PartnerId ? LastFetchedData.Value : null;

                if (CurrentData == null)
                {
                    var resultTask = HttpClient.GetStringAsyncForUnicode(ServerRoot + "GetPartnerCycleInformation?PartnerId=" + PartnerId);
                    var result = await resultTask;
                    if (resultTask.Exception != null)
                    {
                        GetPartnerCycleInformationFromServerAsyncWorking = false;
                        return new ResultSuccess<PartnerCycleInformationModel>(false, resultTask.Exception.ProperMessage());
                    }

                    if (!result.Success)
                        throw new Exception(result.Message);

                    var resultDeserialized = JsonConvert.DeserializeObject<ResultSuccess<PartnerCycleInformationModel>>(result.Data);
                    if (!resultDeserialized.Success)
                    {
                        GetPartnerCycleInformationFromServerAsyncWorking = false;
                        return new ResultSuccess<PartnerCycleInformationModel>(false, resultDeserialized.Message);
                    }

                    CurrentData = resultDeserialized.Data;
                }

                LastFetchedData = new KeyValuePair<Guid, PartnerCycleInformationModel>(PartnerId, CurrentData);

                GetPartnerCycleInformationFromServerAsyncWorking = false;
                return new ResultSuccess<PartnerCycleInformationModel>(true, "", CurrentData);
            }
            catch (Exception err)
            {
                GetPartnerCycleInformationFromServerAsyncWorking = false;
                return new ResultSuccess<PartnerCycleInformationModel>(false, err.ProperMessage());
            }
        }

        public class UnSettledOrderModel : INotifyPropertyChanged
        {
            private bool _Selected;
            public bool Selected 
            { 
                get { 
                    return _Selected; 
                    }   

                set { 
                    _Selected = value; 
                    OnPropertyChanged("Selected"); 
                    OnPropertyChanged("RowColor"); 

                    //if (App.InsertedInformations_Partners != null) 
                    //    App.InsertedInformations_Partners.RefreshToolbarItems(); 
                    } 
            }

            public string RowColor
            {
                get
                {
                    //return ForChangedPartnersList ? Selected ? "#F5F5A4" : Sent ? "#B7E5BF" : "#DCE6FA" :
                    //Selected ? "#A4DEF5" : HasOrder ? "#B7E5BF" : HasFailedVisit ? "#E5B7BF" : "#DCE6FA";

                    return "#FFFFFF";
                }
            }
            public bool Sent { get; set; }
            public static bool Multiselection { get; set; }
            public bool CanBeSelectedInMultiselection { get { return true; /*Sent ? false : true;*/ } }
            public GridLength CheckBoxColumnWidth { get { return Multiselection ? 60 : 0; } }

            public Guid OrderId { get; set; }
            public string OrderCode { get; set; }
            public string TotalCode { get; set; }
            public string OrderPreCode { get; set; }
            public string OrderDate { get; set; }
            public string PartnerCode { get; set; }
            public string PartnerName { get; set; }
            public string DriverCode { get; set; }
            public string DriverName { get; set; }
            public string Settled_Reversion { get; set; }
            public string Settled_Cash { get; set; }
            public string Settled_Bank { get; set; }
            public string Settled_Cheque { get; set; }
            public string Settled_Total { get; set; }
            public int Settled_Cheque_Count { get; set; }
            public string VisitorCode { get; set; }
            public string VisitorName { get; set; }
            public string Remainder { get; set; }
            public string Price { get; set; }


            public event PropertyChangedEventHandler PropertyChanged;
            public void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public static async Task<ResultSuccess<List<UnSettledOrderModel>>> GetUnSettledOrdersFromServerAsync(string UserName, string Password, string CurrentVersionNumber, string PartnerCode, string BOrderInsertDate, string EOrderInsertDate)
        {
            try
            {
                var resultTask = HttpClient.GetStringAsyncForUnicode(ServerRoot + $"GetUnSettledOrders?UserName={UserName}&Password={Password}&CurrentVersionNumber={CurrentVersionNumber}&PartnerCode={PartnerCode}");
                var result = await resultTask;

                if (resultTask.Exception != null)
                {
                    return new ResultSuccess<List<UnSettledOrderModel>>(false, resultTask.Exception.ProperMessage());
                }

                if (!result.Success)
                    throw new Exception(result.Message);

                var resultDeserialized = JsonConvert.DeserializeObject<ResultSuccess<List<UnSettledOrderModel>>>(result.Data);
                
                if (!resultDeserialized.Success)
                {
                    return new ResultSuccess<List<UnSettledOrderModel>>(false, resultDeserialized.Message);
                }

                return new ResultSuccess<List<UnSettledOrderModel>>(true, "", resultDeserialized.Data);
            }
            catch (Exception ex)
            {
                return new ResultSuccess<List<UnSettledOrderModel>>(false, ex.ProperMessage());
            }
        }

        public class UncashedChequeListModel : INotifyPropertyChanged
        {
            string _BackNumber;
            public string BackNumber { get { return _BackNumber.ReplaceLatinDigits(); } set { _BackNumber = value; } }
            string _Serial;
            public string Serial { get { return _Serial.ReplaceLatinDigits(); } set { _Serial = value; } }
            public DateTime _MaturityDate { private get; set; }
            public string MaturityDate { get { return _MaturityDate.ToShortStringForDate().Substring(2).ReplaceLatinDigits(); } }
            public decimal _Price { private get; set; }
            public string Price { get { return _Price.ToString("###,###,###,###,###,##0.").ReplaceLatinDigits(); } }
            string _State;
            public string State { get { return _State.ReplaceLatinDigits(); } set { _State = value; } }
            string _Description;
            public string Description { get { return _Description.ReplaceLatinDigits(); } set { _Description = value; } }
            public event PropertyChangedEventHandler PropertyChanged;
            public void OnPropertyChanged(string propertyName)
            {
                var handler = PropertyChanged;
                if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public static async Task<ResultSuccess<List<UncashedChequeListModel>>> GetUncashedChequesListAsync(Guid PartnerId, bool Refresh)
        {
            return await Task.Run(async () =>
            {
                try
                {
                    var Data = await GetPartnerCycleInformationFromServerAsync(PartnerId, Refresh);
                    if (!Data.Success)
                        return new ResultSuccess<List<UncashedChequeListModel>>(false, Data.Message);

                    var ReportData = Data.Data.UncashedChequesList.Select(a => new UncashedChequeListModel()
                    {
                        BackNumber = a.BackNumber,
                        Serial = a.Serial,
                        _MaturityDate = a.MaturityDate,
                        _Price = a.Price,
                        State = a.State,
                        Description = a.Description
                    }).ToList();

                    return new ResultSuccess<List<UncashedChequeListModel>>(true, "", ReportData);
                }
                catch (Exception err)
                {
                    return new ResultSuccess<List<UncashedChequeListModel>>(false, err.ProperMessage());
                }
            });
        }

        public class ReturnedChequeListModel : INotifyPropertyChanged
        {
            string _BackNumber;
            public string BackNumber { get { return _BackNumber.ReplaceLatinDigits(); } set { _BackNumber = value; } }
            string _Serial;
            public string Serial { get { return _Serial.ReplaceLatinDigits(); } set { _Serial = value; } }
            public DateTime _MaturityDate { private get; set; }
            public string MaturityDate { get { return _MaturityDate.ToShortStringForDate().Substring(2).ReplaceLatinDigits(); } }
            public decimal _Price { private get; set; }
            public string Price { get { return _Price.ToString("###,###,###,###,###,##0.").ReplaceLatinDigits(); } }
            string _Description;
            public string Description { get { return _Description.ReplaceLatinDigits(); } set { _Description = value; } }
            public event PropertyChangedEventHandler PropertyChanged;
            public void OnPropertyChanged(string propertyName)
            {
                var handler = PropertyChanged;
                if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public static async Task<ResultSuccess<List<ReturnedChequeListModel>>> GetReturnedChequesListAsync(Guid PartnerId, bool Refresh)
        {
            return await Task.Run(async () =>
            {
                try
                {
                    var Data = await GetPartnerCycleInformationFromServerAsync(PartnerId, Refresh);
                    if (!Data.Success)
                        return new ResultSuccess<List<ReturnedChequeListModel>>(false, Data.Message);

                    var ReportData = Data.Data.ReturnedChequesList.Select(a => new ReturnedChequeListModel()
                    {
                        BackNumber = a.BackNumber,
                        Serial = a.Serial,
                        _MaturityDate = a.MaturityDate,
                        _Price = a.Price,
                        Description = a.Description
                    }).ToList();

                    return new ResultSuccess<List<ReturnedChequeListModel>>(true, "", ReportData);
                }
                catch (Exception err)
                {
                    return new ResultSuccess<List<ReturnedChequeListModel>>(false, err.ProperMessage());
                }
            });
        }

        public class CycleDataListModel : INotifyPropertyChanged
        {
            public DateTime _Date { private get; set; }
            public string Date { get { return _Date.ToShortStringForDate().Substring(2).ReplaceLatinDigits(); } }
            string _Description;
            public string Description { get { return _Description.ReplaceLatinDigits(); } set { _Description = value; } }
            public decimal _Debtor { private get; set; }
            public string Debtor { get { return _Debtor.ToString("###,###,###,###,###,##0.").ReplaceLatinDigits(); } }
            public decimal _Creditor { private get; set; }
            public string Creditor { get { return _Creditor.ToString("###,###,###,###,###,##0.").ReplaceLatinDigits(); } }
            public decimal _Remainder { get; set; }
            public string Remainder { get { return (_Remainder < 0 ? "(" : "") + Math.Abs(_Remainder).ToString("###,###,###,###,###,##0.").ReplaceLatinDigits() + (_Remainder < 0 ? ")" : ""); } }

            public event PropertyChangedEventHandler PropertyChanged;
            public void OnPropertyChanged(string propertyName)
            {
                var handler = PropertyChanged;
                if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public static async Task<ResultSuccess<List<CycleDataListModel>>> GetCycleDatasListAsync(Guid PartnerId, bool Refresh)
        {
            return await Task.Run(async () =>
            {
                try
                {
                    var Data = await GetPartnerCycleInformationFromServerAsync(PartnerId, Refresh);
                    if (!Data.Success)
                        return new ResultSuccess<List<CycleDataListModel>>(false, Data.Message);

                    var ReportData = Data.Data.PartnerCycleDetail.Select(a => new CycleDataListModel()
                    {
                        _Date = a.Date,
                        Description = a.Description,
                        _Debtor = a.Debtor,
                        _Creditor = a.Creditor,
                        _Remainder = a.Remainder
                    }).ToList();

                    return new ResultSuccess<List<CycleDataListModel>>(true, "", ReportData);
                }
                catch (Exception err)
                {
                    return new ResultSuccess<List<CycleDataListModel>>(false, err.ProperMessage());
                }
            });
        }

        public class ReportGeneralModel
        {
            public string _Column1 { get; set; }
            public string Column1 { get { return (_Column1 != null ? _Column1 : "").ReplaceLatinDigits(); } }
            public string _Column2 { get; set; }
            public string Column2 { get { return (_Column2 != null ? _Column2 : "").ReplaceLatinDigits(); } }
            public string _Column3 { get; set; }
            public string Column3 { get { return (_Column3 != null ? _Column3 : "").ReplaceLatinDigits(); } }
            public string _Column4 { get; set; }
            public string Column4 { get { return (_Column4 != null ? _Column4 : "").ReplaceLatinDigits(); } }
            public string _Column5 { get; set; }
            public string Column5 { get { return (_Column5 != null ? _Column5 : "").ReplaceLatinDigits(); } }
        }
        public static async Task<ResultSuccess<ReportGeneralModel[]>> GetReportDataAsync(string ReportType, DateTime BDate, DateTime EDate)
        {
            return await Task.Run(async () =>
            {
                try
                {
                    var resultTask = HttpClient.GetStringAsyncForUnicode(ServerRoot + "GetReport?UserName=" + App.Username.Value + "&Password=" + App.Password.Value + "&CurrentVersionNumber=" + App.CurrentVersionNumber + "&ReportType=" + ReportType + "&BDate=" + BDate.ToShortStringForDate() + "&EDate=" + EDate.ToShortStringForDate());
                    var result = await resultTask;
                    if (resultTask.Exception != null)
                        return new ResultSuccess<ReportGeneralModel[]>(false, resultTask.Exception.ProperMessage());

                    if (!result.Success)
                        return new ResultSuccess<ReportGeneralModel[]>(false, result.Message);

                    var resultDeserialized = JsonConvert.DeserializeObject<ResultSuccess<ReportGeneralModel[]>>(result.Data);
                    if (!resultDeserialized.Success)
                        return new ResultSuccess<ReportGeneralModel[]>(false, resultDeserialized.Message);


                    return new ResultSuccess<ReportGeneralModel[]>(true, "", resultDeserialized.Data);
                }
                catch (Exception err)
                {
                    return new ResultSuccess<ReportGeneralModel[]>(false, err.ProperMessage());
                }
            });
        }


        public static async Task<ResultSuccess<BackupModel[]>> GetBackupListFromServerAsync()
        {
            try
            {
                var resultTask = HttpClient.GetStringAsyncForUnicode(ServerRoot + "GetBackupsList?UserName=" + App.Username.Value + "&Password=" + App.Password.Value + "&CurrentVersionNumber=" + App.CurrentVersionNumber);
                var result = await resultTask;
                if (resultTask.Exception != null)
                    return new ResultSuccess<BackupModel[]>(false, resultTask.Exception.ProperMessage());

                if (!result.Success)
                    return new ResultSuccess<BackupModel[]>(false, result.Message);

                var resultDeserialized = JsonConvert.DeserializeObject<ResultSuccess<BackupModel[]>>(result.Data);
                if (!resultDeserialized.Success)
                    return new ResultSuccess<BackupModel[]>(false, resultDeserialized.Message);

                var BackupsList = resultDeserialized.Data;

                return new ResultSuccess<BackupModel[]>(true, "", BackupsList);
            }
            catch (Exception err)
            {
                return new ResultSuccess<BackupModel[]>(false, err.ProperMessage());
            }
        }

        public static async Task<ResultSuccess> UploadBackupToServerAsync(UploadFileCompleted OnUploadFileCompleted, UploadProgressChanged OnUploadProgressChanged)
        {
            try
            {
                var URL = ServerRoot + "SubmitBackup?UserName=" + App.Username.Value + "&Password=" + App.Password.Value + "&CurrentVersionNumber=" + App.CurrentVersionNumber;
                var FileName = App.DBFileName;
                var uploadResult = await App.Uploader.UploadFile(URL, FileName, "", OnUploadFileCompleted, OnUploadProgressChanged);
                if (!uploadResult.Success)
                    return new ResultSuccess(false, uploadResult.Message);

                return new ResultSuccess(true, "");
            }
            catch (Exception err)
            {
                return new ResultSuccess(false, err.ProperMessage());
            }
        }

        public static async Task<ResultSuccess> DownloadBackupFromServerAsync(string FileName, DownloadFileCompleted OnDownloadFileCompleted, DownloadProgressChanged OnDownloadProgressChanged)
        {
            try
            {
                App.File.Copy(App.DBFileName, App.DBFileName.Replace("karadb.db3", "karadbcopy.db3"));

                var downloadResult = await App.Downloader.DownloadFile(ServerRoot.Replace("/MobileApp/General/", "") + "/Temp/Mobile/", FileName, App.DBFileName.Replace("karadb.db3", ""), "karadb.db3", OnDownloadFileCompleted, OnDownloadProgressChanged);
                if (!downloadResult.Success)
                {
                    App.File.Copy(App.DBFileName.Replace("karadb.db3", "karadbcopy.db3"), App.DBFileName);
                    return new ResultSuccess(false, downloadResult.Message);
                }

                App.File.Delete(App.DBFileName.Replace("karadb.db3", "karadbcopy.db3"));
                return new ResultSuccess(true, "");
            }
            catch (Exception err)
            {
                return new ResultSuccess(false, err.ProperMessage());
            }
        }

        public class PartnerSubmitResultDataModel
        {
            public string PartnerCode { get; set; }
        }
        public static async Task<ResultSuccess<int>> SubmitPartnersAsync(Partner[] Partners)
        {
            var SentCount = 0;
            try
            {
                foreach (var Partner in Partners)
                {
                    var Data = new[]
                    {
                        new KeyValuePair<string, string>("Id", Partner.Id.ToString().ReplacePersianDigits()),
                        new KeyValuePair<string, string>("FirstName", Partner.FirstName.ReplacePersianDigits()),
                        new KeyValuePair<string, string>("LastName", Partner.LastName.ReplacePersianDigits()),
                        new KeyValuePair<string, string>("LegalName", Partner.LegalName.ReplacePersianDigits()),
                        new KeyValuePair<string, string>("ZoneId", Partner.ZoneId.ToString()),
                        new KeyValuePair<string, string>("Phone1", Partner.Phone1.ReplacePersianDigits()),
                        new KeyValuePair<string, string>("Phone2", Partner.Phone2.ReplacePersianDigits()),
                        new KeyValuePair<string, string>("Mobile", Partner.Mobile.ReplacePersianDigits()),
                        new KeyValuePair<string, string>("Fax", Partner.Fax.ReplacePersianDigits()),
                        new KeyValuePair<string, string>("Address", Partner.Address.ReplacePersianDigits()),
                        new KeyValuePair<string, string>("IsLegal", Partner.IsLegal.ToString()),
                        new KeyValuePair<string, string>("CalculateVATForThisPerson", Partner.CalculateVATForThisPerson.ToString()),
                        new KeyValuePair<string, string>("CreditId", Partner.CreditId.ToString()),
                        new KeyValuePair<string, string>("GroupIds", Partner.Groups.Any() ? Partner.Groups.Select(a => a.Id.ToString()).Aggregate((sum, x) => sum + "|" + x) : "")
                    };
                    HttpContent Content = new FormUrlEncodedContent(Data);

                    var resultTask = HttpClient.PostAsyncForUnicode(ServerRoot + "SubmitPartner?UserName=" + App.Username.Value + "&Password=" + App.Password.Value + "&CurrentVersionNumber=" + App.CurrentVersionNumber, Content);
                    var result = await resultTask;
                    if (resultTask.Exception != null)
                        throw new Exception(resultTask.Exception.ProperMessage());
                    if (!result.Success)
                        throw new Exception(result.Message);

                    var resultDeserialized = JsonConvert.DeserializeObject<ResultSuccess<PartnerSubmitResultDataModel>>(result.Data);
                    if (!resultDeserialized.Success)
                        throw new Exception(resultDeserialized.Message);

                    Partner.Sent = true;
                    Partner.Code = resultDeserialized.Data.PartnerCode;
                    var updateResult = await App.DB.InsertOrUpdateRecordAsync<Partner>(Partner);

                    SentCount++;
                }

                return new ResultSuccess<int>(true, "", SentCount);
            }
            catch (Exception err)
            {
                var Message = (Partners.Count() == 1 ? "" : SentCount == 0 ? "هیچ مشتری به سرور ارسال نشد." : "تعداد " + SentCount + " مشتری به سرور ارسال شد، اما در ادامه مشکلی رخ داد.") + err.ProperMessage();
                return new ResultSuccess<int>(false, Message, SentCount);
            }
        }



        public static async Task<ResultSuccess<int>> SubmitReceiptPecuniaryAsync(FinancialTransactionDocument[] documents)
        {
            var SentCount = 0;
            try
            {
                foreach (var document in documents)
                {
                    var Data = new[]
                    {
                        new KeyValuePair<string, string>("DocumentId", document.DocumentId.ToString()),
                        new KeyValuePair<string, string>("Price", document.InputPrice.ToString()),
                        new KeyValuePair<string, string>("CollectorId", document.CollectorId.ToString()),
                        new KeyValuePair<string, string>("TransactionType", document.TransactionType.ToString()),
                        new KeyValuePair<string, string>("PartnerId", document.PartnerId.ToString()),
                        new KeyValuePair<string, string>("CashAccountId", document.CashAccountId.ToString()),
                        //new KeyValuePair<string, string>("InputPrice", document.InputPrice.ToString()),
                        //new KeyValuePair<string, string>("OutputPrice", document.OutputPrice.ToString()),
                        //new KeyValuePair<string, string>("DocumentCode", document.DocumentCode.ToString()),
                        //new KeyValuePair<string, string>("DocumentState", document.DocumentState.ToString()),
                        new KeyValuePair<string, string>("PersianDocumentDate", document.PersianDocumentDate.ToString()),
                        //new KeyValuePair<string, string>("DocumentUserId", document.DocumentUserId.ToString()),
                        new KeyValuePair<string, string>("DocumentDescription", document.DocumentDescription.ToString()),
                        //new KeyValuePair<string, string>("ChequeCode", document.ChequeCode.ToString()),
                        //new KeyValuePair<string, string>("BranchName", document.BranchName.ToString()),
                        //new KeyValuePair<string, string>("BranchCode", document.BranchCode.ToString()),
                        //new KeyValuePair<string, string>("Delivery", document.Delivery.ToString()),
                        //new KeyValuePair<string, string>("Issuance", document.Issuance.ToString()),
                        //new KeyValuePair<string, string>("BankTransferCode", document.BankTransferCode.ToString()),
                    };

                    HttpContent Content = new FormUrlEncodedContent(Data);

                    var resultTask = HttpClient.PostAsyncForUnicode(ServerRoot + "InsertReceiptPrcuniary?UserName=" + App.Username.Value + "&Password=" + App.Password.Value + "&CurrentVersionNumber=" + App.CurrentVersionNumber, Content);
                    var result = await resultTask;
                    if (resultTask.Exception != null)
                        throw new Exception(resultTask.Exception.ProperMessage());
                    if (!result.Success)
                        throw new Exception(result.Message);

                    var resultDeserialized = JsonConvert.DeserializeObject<ResultSuccess<PartnerSubmitResultDataModel>>(result.Data);
                    if (!resultDeserialized.Success)
                        throw new Exception(resultDeserialized.Message);

                    //Partner.Sent = true;
                    //Partner.Code = resultDeserialized.Data.PartnerCode;
                    var updateResult = await App.DB.InsertOrUpdateRecordAsync<FinancialTransactionDocument>(document);

                    SentCount++;
                }

                return new ResultSuccess<int>(true, "", SentCount);
            }
            catch (Exception err)
            {
                var Message = (documents.Count() == 1 ? "" : SentCount == 0 ? "هیچ سندی به سرور ارسال نشد." : "تعداد " + SentCount + " سند به سرور ارسال شد، اما در ادامه مشکلی رخ داد.") + err.ProperMessage();
                return new ResultSuccess<int>(false, Message, SentCount);
            }
        }


        public static async Task<ResultSuccess<int>> SubmitFailedVisitsAsync(FailedVisit[] FailedVisits)
        {
            var SentCount = 0;
            try
            {
                foreach (var FailedVisit in FailedVisits)
                {
                    if (FailedVisit.Partner.ChangeDate.HasValue && !FailedVisit.Partner.Sent.GetValueOrDefault(false))
                    {
                        var partnerSubmitResultTask = SubmitPartnersAsync(new Partner[] { FailedVisit.Partner });
                        var partnerSubmitResult = await partnerSubmitResultTask;
                        if (partnerSubmitResultTask.Exception != null)
                            throw new Exception(partnerSubmitResultTask.Exception.ProperMessage());
                        if (!partnerSubmitResult.Success)
                            throw new Exception(partnerSubmitResult.Message);
                    }

                    var Data = new[]
                    {
                        new KeyValuePair<string, string>("Description", FailedVisit.Description.ReplacePersianDigits()),
                        new KeyValuePair<string, string>("GeoLocationAccuracy", FailedVisit.GeoLocationAccuracy.HasValue ? FailedVisit.GeoLocationAccuracy.Value.ToString() : ""),
                        new KeyValuePair<string, string>("GeoLocationLat", FailedVisit.GeoLocationLat.HasValue ? FailedVisit.GeoLocationLat.Value.ToString() : "" ),
                        new KeyValuePair<string, string>("GeoLocationLong", FailedVisit.GeoLocationLong.HasValue ? FailedVisit.GeoLocationLong.Value.ToString() : ""),
                        new KeyValuePair<string, string>("Id", FailedVisit.Id.ToString()),
                        new KeyValuePair<string, string>("PartnerId", FailedVisit.PartnerId.ToString()),
                        new KeyValuePair<string, string>("ReasonId", FailedVisit.ReasonId.ToString()),
                        new KeyValuePair<string, string>("VisitTime", FailedVisit.VisitTime.ToString("yyyy-MM-dd HH-mm-ss"))
                    };
                    HttpContent Content = new FormUrlEncodedContent(Data);

                    var resultTask = HttpClient.PostAsyncForUnicode(ServerRoot + "SubmitFailedVisit?UserName=" + App.Username.Value + "&Password=" + App.Password.Value + "&CurrentVersionNumber=" + App.CurrentVersionNumber, Content);
                    var result = await resultTask;
                    if (resultTask.Exception != null)
                        throw new Exception(resultTask.Exception.ProperMessage());
                    if (!result.Success)
                        throw new Exception(result.Message);

                    var resultDeserialized = JsonConvert.DeserializeObject<ResultSuccess>(result.Data);
                    if (!resultDeserialized.Success)
                        throw new Exception(resultDeserialized.Message);

                    FailedVisit.Sent = true;
                    var updateResult = await App.DB.InsertOrUpdateRecordAsync<FailedVisit>(FailedVisit);

                    SentCount++;
                }

                return new ResultSuccess<int>(true, "", SentCount);
            }
            catch (Exception err)
            {
                var Message = (FailedVisits.Count() == 1 ? "" : SentCount == 0 ? "هیچ عدم سفارشی به سرور ارسال نشد." : "تعداد " + SentCount + " عدم سفارش به سرور ارسال شد، اما در ادامه مشکلی رخ داد.") + err.ProperMessage();
                return new ResultSuccess<int>(false, Message, SentCount);
            }
        }

        public static async Task<ResultSuccess> SubmitDeviceSettingChange()
        {
            try
            {
                var DeviceSettingChanges = App.DB.conn.Table<DeviceSettingChange>().Where(a => !a.Sent).ToList().Take(5).ToList();
                if (DeviceSettingChanges.Any())
                {
                    var Data = DeviceSettingChanges.SelectMany((a, index) => new[] {
                        new KeyValuePair<string, string>("DeviceSettingChange_" + index + "_DateTime", a.DateTime.ToString("yyyy-MM-dd HH-mm-ss")),
                        new KeyValuePair<string, string>("DeviceSettingChange_" + index + "_Type", a.Type.ToString())
                    }).ToArray();
                    HttpContent Content = new FormUrlEncodedContent(Data);

                    var resultTask = HttpClient.PostAsyncForUnicode(ServerRoot + "SubmitDeviceSettingChanges?UserName=" + App.Username.Value + "&Password=" + App.Password.Value + "&CurrentVersionNumber=" + App.CurrentVersionNumber, Content);
                    var result = await resultTask;
                    if (resultTask.Exception != null)
                        throw new Exception(resultTask.Exception.ProperMessage());
                    if (!result.Success)
                        throw new Exception(result.Message);

                    var resultDeserialized = JsonConvert.DeserializeObject<ResultSuccess>(result.Data);
                    if (!resultDeserialized.Success)
                        throw new Exception(resultDeserialized.Message);

                    foreach (var item in DeviceSettingChanges)
                    {
                        item.Sent = true;
                        var updateResult = await App.DB.InsertOrUpdateRecordAsync(item);
                    }
                }

                return new ResultSuccess(true, "");
            }
            catch (Exception err)
            {
                return new ResultSuccess(false, err.ProperMessage());
            }
        }

        public static async Task<ResultSuccess> SubmitExceptionsLog(string log)
        {
            try
            {
                var Data = new[] {
                    new KeyValuePair<string, string>("Log", System.Net.WebUtility.UrlEncode(log))
                }.ToArray();
                HttpContent Content = new FormUrlEncodedContent(Data);

                var resultTask = HttpClient.PostAsyncForUnicode("http://89.165.11.211:8000/MobileApp/General/SubmitExceptionsLog?UserName=" + App.Username.Value + "&Password=" + App.Password.Value + "&CurrentVersionNumber=" + App.CurrentVersionNumber + "&Server=" + App.AllServerAddresses.Value, Content);
                var result = await resultTask;
                if (resultTask.Exception != null)
                    throw new Exception(resultTask.Exception.ProperMessage());
                if (!result.Success)
                    throw new Exception(result.Message);

                var resultDeserialized = JsonConvert.DeserializeObject<ResultSuccess>(result.Data);
                if (!resultDeserialized.Success)
                    throw new Exception(resultDeserialized.Message);

                return new ResultSuccess(true, "");
            }
            catch (Exception err)
            {
                return new ResultSuccess(false, err.ProperMessage());
            }
        }

        public class SaleOrderSubmitResultDataModel
        {
            public int OrderPreCode { get; set; }
            public string InsertDateTime { get; set; }
            public Stock[] RefreshedStocks { get; set; }
        }
        public static async Task<ResultSuccess<int>> SubmitSaleOrdersAsync(SaleOrder[] SaleOrders)
        {
            var SentCount = 0;
            try
            {
                foreach (var SaleOrder in SaleOrders)
                {
                    if (SaleOrder.Partner.ChangeDate.HasValue && !SaleOrder.Partner.Sent.GetValueOrDefault(false))
                    {
                        var partnerSubmitResultTask = SubmitPartnersAsync(new Partner[] { SaleOrder.Partner });
                        var partnerSubmitResult = await partnerSubmitResultTask;
                        if (partnerSubmitResultTask.Exception != null)
                            throw new Exception(partnerSubmitResultTask.Exception.ProperMessage());
                        if (!partnerSubmitResult.Success)
                            throw new Exception(partnerSubmitResult.Message);
                    }

                    var SaleOrderStuffs = (await App.DB.GetSaleOrderStuffsAsync(SaleOrder.Id)).Data;

                    //temp
                    var allDiscounts = (await App.DB.GetAllSaleOrderCashDiscountsAsync()).Data;

                    var CashDiscounts = (await App.DB.GetSaleOrderCashDiscountsAsync(SaleOrder.Id)).Data;

                    var Data = new[]
                    {
                        new KeyValuePair<string, string>("Id", SaleOrder.Id.ToString()),
                        new KeyValuePair<string, string>("WarehouseId", SaleOrder.WarehouseId.ToString()),
                        new KeyValuePair<string, string>("PartnerId", SaleOrder.PartnerId.ToString()),
                        new KeyValuePair<string, string>("SettlementTypeId", SaleOrder.SettlementTypeId.ToString()),
                        new KeyValuePair<string, string>("SettlementDay", SaleOrder.SettlementDay.ToString()),
                        new KeyValuePair<string, string>("Description", SaleOrder.Description),
                        new KeyValuePair<string, string>("VisitTime", SaleOrder.InsertDateTime.ToString("yyyy-MM-dd HH-mm-ss")),
                        new KeyValuePair<string, string>("DiscountPercent", SaleOrder.DiscountPercent.ToString()),
                        new KeyValuePair<string, string>("AddedDiscountPercent", SaleOrder.AddedDiscountPercent.ToString()),
                        new KeyValuePair<string, string>("CashPrise", SaleOrder.CashPrise.ToString()),
                        new KeyValuePair<string, string>("GeoLocationAccuracy", SaleOrder.GeoLocation_Accuracy.HasValue ? SaleOrder.GeoLocation_Accuracy.ToString() : ""),
                        new KeyValuePair<string, string>("GeoLocationLat", SaleOrder.GeoLocation_Latitude.HasValue?SaleOrder.GeoLocation_Latitude.ToString() : ""),
                        new KeyValuePair<string, string>("GeoLocationLong", SaleOrder.GeoLocation_Longitude.HasValue?SaleOrder.GeoLocation_Longitude.ToString() : "")
                    }.ToList();
                    var index = 0;
                    foreach (var SaleOrderStuff in SaleOrderStuffs)
                    {
                        index++;
                        Data.Add(new KeyValuePair<string, string>("Article_" + index + "_Index", index.ToString()));
                        Data.Add(new KeyValuePair<string, string>("Article_" + index + "_Id", SaleOrderStuff.Id.ToString()));
                        Data.Add(new KeyValuePair<string, string>("Article_" + index + "_PackageId", SaleOrderStuff.PackageId.ToString()));
                        Data.Add(new KeyValuePair<string, string>("Article_" + index + "_BatchNumberId", SaleOrderStuff.BatchNumberId.HasValue ? SaleOrderStuff.BatchNumberId.Value.ToString() : ""));
                        Data.Add(new KeyValuePair<string, string>("Article_" + index + "_Quantity", SaleOrderStuff.Quantity.ToString()));
                        Data.Add(new KeyValuePair<string, string>("Article_" + index + "_VATPercent", SaleOrderStuff.VATPercent.ToString()));
                        Data.Add(new KeyValuePair<string, string>("Article_" + index + "_DiscountPercent", SaleOrderStuff.DiscountPercent.ToString()));
                        Data.Add(new KeyValuePair<string, string>("Article_" + index + "_CashDiscountPercent", SaleOrderStuff.CashDiscountPercent.ToString()));
                        Data.Add(new KeyValuePair<string, string>("Article_" + index + "_AddedDiscountPercent", SaleOrderStuff.AddedDiscountPercent.ToString()));
                        Data.Add(new KeyValuePair<string, string>("Article_" + index + "_SalePrice", SaleOrderStuff.SalePrice.ToString()));
                        Data.Add(new KeyValuePair<string, string>("Article_" + index + "_FreeProduct", SaleOrderStuff.FreeProduct.ToString()));
                        Data.Add(new KeyValuePair<string, string>("Article_" + index + "_FreeProductAddedQuantity", SaleOrderStuff.FreeProductAddedQuantity.ToString()));
                        Data.Add(new KeyValuePair<string, string>("Article_" + index + "_FreeProductUnitPrice", SaleOrderStuff.FreeProduct_UnitPrice.HasValue ? SaleOrderStuff.FreeProduct_UnitPrice.Value.ToString() : ""));
                        Data.Add(new KeyValuePair<string, string>("Article_" + index + "_StuffSettlementDay", SaleOrderStuff.StuffSettlementDay.HasValue ? SaleOrderStuff.StuffSettlementDay.Value.ToString() : ""));
                    }
                    index = 0;
                    foreach (var CashDiscount in CashDiscounts)
                    {
                        index++;
                        Data.Add(new KeyValuePair<string, string>("CashDiscount_" + index + "_Index", index.ToString()));
                        Data.Add(new KeyValuePair<string, string>("CashDiscount_" + index + "_Day", CashDiscount.Day.ToString()));
                        Data.Add(new KeyValuePair<string, string>("CashDiscount_" + index + "_Percent", CashDiscount.Percent.ToString()));
                    }

                    HttpContent Content = new FormUrlEncodedContent(Data);

                    var resultTask = HttpClient.PostAsyncForUnicode(ServerRoot + "SubmitSaleOrder?UserName=" + App.Username.Value + "&Password=" + App.Password.Value + "&CurrentVersionNumber=" + App.CurrentVersionNumber + "&LastPriceListVersion=" + App.LastPriceListVersion.Value + "&LastDiscountRuleVersion=" + App.LastDiscountRuleVersion.Value, Content);
                    var result = await resultTask;
                    if (resultTask.Exception != null)
                        throw new Exception(resultTask.Exception.ProperMessage());
                    if (!result.Success)
                        throw new Exception(result.Message);

                    var resultDeserialized = JsonConvert.DeserializeObject<ResultSuccess<SaleOrderSubmitResultDataModel>>(result.Data);
                    if (!resultDeserialized.Success)
                        throw new Exception(resultDeserialized.Message);

                    SaleOrder.PreCode = resultDeserialized.Data.OrderPreCode;
                    SaleOrder.InsertDateTime = new DateTime
                    (
                        Convert.ToInt32(resultDeserialized.Data.InsertDateTime.Substring(0, 4)),
                        Convert.ToInt32(resultDeserialized.Data.InsertDateTime.Substring(5, 2)),
                        Convert.ToInt32(resultDeserialized.Data.InsertDateTime.Substring(8, 2)),
                        Convert.ToInt32(resultDeserialized.Data.InsertDateTime.Substring(11, 2)),
                        Convert.ToInt32(resultDeserialized.Data.InsertDateTime.Substring(14, 2)),
                        Convert.ToInt32(resultDeserialized.Data.InsertDateTime.Substring(17, 2))
                    );
                    var updateResult = await App.DB.InsertOrUpdateRecordAsync<SaleOrder>(SaleOrder);

                    updateResult = await App.DB.DeleteAllRecordsAsync<Stock>();
                    updateResult = await App.DB.InsertAllRecordsAsync(resultDeserialized.Data.RefreshedStocks);

                    SentCount++;
                }

                return new ResultSuccess<int>(true, "", SentCount);
            }
            catch (Exception err)
            {
                var Message = (SaleOrders.Count() == 1 ? "" : SentCount == 0 ? "هیچ سفارشی به سرور ارسال نشد." : "تعداد " + SentCount + " فاکتور به سرور ارسال شد، اما در ادامه مشکلی رخ داد.") + err.ProperMessage();
                return new ResultSuccess<int>(false, Message, SentCount);
            }
        }

        public static async Task<ResultSuccess> SubmitLocationsAsync(IEnumerable<LocationModel> Locations)
        {
            try
            {
                await SubmitDeviceSettingChange();

                var SendingLocations = Locations.Where(a => !a.SentToApplication).ToArray();
                if (SendingLocations.Any())
                    return await SendPointsViaTCP(SendingLocations);

                return new ResultSuccess();


                //var Data = new KeyValuePair<string, string>[] { }.ToList();
                //var index = 0;
                //foreach (var Location in Locations)
                //{
                //    if (!Location.SentToApplication)
                //    {
                //        index++;
                //        Data.Add(new KeyValuePair<string, string>("Point_" + index + "_Index", index.ToString()));
                //        Data.Add(new KeyValuePair<string, string>("Point_" + index + "_TimeStamp", Location.Timestamp.ToString()));
                //        Data.Add(new KeyValuePair<string, string>("Point_" + index + "_State", Location.DeviceState.ToString()));
                //        Data.Add(new KeyValuePair<string, string>("Point_" + index + "_Lat", Location.Latitude.HasValue ? Location.Latitude.Value.ToString("###.#########") : ""));
                //        Data.Add(new KeyValuePair<string, string>("Point_" + index + "_Long", Location.Longitude.HasValue ? Location.Longitude.Value.ToString("###.#########") : ""));
                //        Data.Add(new KeyValuePair<string, string>("Point_" + index + "_Accuracy", Location.Accuracy.HasValue ? Location.Accuracy.Value.ToString("###.#########") : ""));
                //    }
                //}

                //HttpContent Content = new FormUrlEncodedContent(Data);
                //
                //var resultTask = HttpClient.PostAsyncForUnicode(ServerRoot + "SubmitLocations?UserName=" + App.Username.Value + "&Password=" + App.Password.Value + "&CurrentVersionNumber=" + App.CurrentVersionNumber, Content);

                //var result = await resultTask;
                //if (resultTask.Exception != null)
                //    return new ResultSuccess(false, resultTask.Exception.ProperMessage());
                //if (!result.Success)
                //    return new ResultSuccess(false, result.Message);

                //var resultDeserialized = JsonConvert.DeserializeObject<ResultSuccess>(result.Data);
                //if (!resultDeserialized.Success)
                //    return new ResultSuccess(false, resultDeserialized.Message);
            }
            catch (Exception err)
            {
                return new ResultSuccess(false, err.ProperMessage());
            }
        }

        static Task<ResultSuccess> SendPointsViaTCP(LocationModel[] Points)
        {
            var IP = App.ServerAddress.Contains(":") ? App.ServerAddress.Split(':')[0] : App.ServerAddress;
            int Port = 11328;

            var SubmitData = new ILocationSubmitModel()
            {
                UserName = App.Username.Value,
                Password = App.Password.Value,
                CurrentVersionNumber = App.CurrentVersionNumber,
                Points = Points.Select((a, index) => new ISubmittingLocationModel()
                {
                    Index = index + 1,
                    TimeStamp = a.Timestamp,
                    State = a.DeviceState,
                    Lat = a.Latitude,
                    Long = a.Longitude,
                    Accuracy = a.Accuracy
                }).ToArray()
            };

            return App.TCPClient.SubmitLocations(IP, Port, SubmitData);
        }



        //TODO
        //static bool SubmitLocationScheduleIsRunning = false;
        //public static async Task SubmitLocationSchedule()
        //{
        //    if(!SubmitLocationScheduleIsRunning)
        //    {
        //        while (true)
        //        {
        //            try
        //            {
        //                await Task.Delay(App.SendLocationsToServerPeriod * 1000);

        //                var UnsentLocations = App.DB.conn.Table<NewLocationModel>().Where(a => !a.SentToApplication).ToArray();
        //                var tak100 = UnsentLocations.OrderBy(a => a.Timestamp).Take(100).ToList();
        //                //if (tak100.Count >= 20 || tak100.Any(a => a.DateTime < DateTime.Now.AddMinutes(-5)))
        //                //{
        //                var result = await SubmitLocationsAsync(tak100);
        //                if (result.Success)
        //                {
        //                    foreach (var location in tak100)
        //                        App.DB.conn.Execute("Delete from ServiceTrackingPoint where Timestamp = " + location.Timestamp.ToString());
        //                }
        //                    //else
        //                    //    App.ToastMessageHandler.ShowMessage("Erro Sending Locations: " + result.Message, Helpers.ToastMessageDuration.Long);
        //                //}
        //            }
        //            catch (Exception)
        //            {
        //            }
        //        }
        //    }
        //}
    }

    public class UpdateItemModel : INotifyPropertyChanged
    {
        private static int ItemsCount = 0;
        private static Random rnd = new Random();

        public event PropertyChangedEventHandler PropertyChanged;

        private bool _UpdateOperationNotStarted;
        public bool UpdateOperationNotStarted { get { return _UpdateOperationNotStarted; } set { _UpdateOperationNotStarted = value; OnPropertyChanged("UpdateOperationNotStarted"); } }
        public Guid Id { get; set; }
        public int Index { get; set; }
        public bool Selected { get; set; }
        private bool _IsUpdating;
        public bool IsUpdating { get { return _IsUpdating; } set { _IsUpdating = value; OnPropertyChanged("IsUpdating"); } }
        public string FinishedImageSource { get { return progress == 1 ? "OK.png" : null; } }
        public string Name { get; set; }
        public string Title { get; set; }
        private double progress { get; set; }
        public double Progress
        {
            get { return progress; }
            set
            {
                if (progress == value)
                    return;

                progress = value;
                OnPropertyChanged("Progress");
                OnPropertyChanged("ProgressPercent");
                OnPropertyChanged("FinishedImageSource");
            }
        }
        public string ProgressPercent
        {
            get { return " " + ((int)Math.Round(progress * 100)).ToString().ReplaceLatinDigits() + " %"; }
        }

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public PartialDBUpdater PartialDBUpdater { get; set; }
        public int FetchRecordCountPerRequest { get; set; }
        public UpdateItemModel(string Name, string Title, PartialDBUpdater PartialDBUpdater, int FetchRecordCountPerRequest)
        {
            ItemsCount++;
            this.Name = Name;
            this.Title = Title;
            Index = ItemsCount;
            Selected = true;
            Progress = 0;
            Id = Guid.NewGuid();
            this.PartialDBUpdater = PartialDBUpdater;
            this.FetchRecordCountPerRequest = FetchRecordCountPerRequest;
            this.UpdateOperationNotStarted = true;
        }
    }
    public class PartialDBUpdater
    {
        public static HttpClient HttpClient;
        public static string ServerRoot;
        public virtual async Task<ResultSuccess<int>> UpdateItemFromServer(Guid RequestId, int From, int Count)
        {
            throw new NotImplementedException();
        }
    }

    public class PartialUpdateDB_Stuffs : PartialDBUpdater
    {
        private static List<Stuff> InitialStuffsData;
        private static List<StuffGroup> InitialStuffGroupsData;
        public override async Task<ResultSuccess<int>> UpdateItemFromServer(Guid RequestId, int From, int Count)
        {
            try
            {
                if (From == 1)
                {
                    InitialStuffsData = (await App.DB.GetStuffsAsync()).Data;
                    InitialStuffGroupsData = (await App.DB.GetStuffGroupsAsync()).Data;

                    await App.DB.SetEnabledForAllRecordsAsync<StuffGroup>(false);
                    await App.DB.SetEnabledForAllRecordsAsync<Stuff>(false);
                    await App.DB.SetEnabledForAllRecordsAsync<StuffBatchNumber>(false);
                    await App.DB.SetEnabledForAllRecordsAsync<Package>(false);

                    await App.DB.DeleteAllRecordsAsync<Unit>();
                    await App.DB.DeleteAllRecordsAsync<StuffBasket>();
                    await App.DB.DeleteAllRecordsAsync<StuffBasketStuff>();
                    await App.DB.DeleteAllRecordsAsync<StuffOrder>();
                    await App.DB.DeleteAllRecordsAsync<StuffSettlementDay>();
                }

                var resultTask = HttpClient.GetStringAsyncForUnicode(ServerRoot + "GetStuffs?RequestId=" + RequestId.ToString() + "&UserName=" + App.Username.Value + "&Password=" + App.Password.Value + "&From=" + From + "&Count=" + Count + "&CurrentVersionNumber=" + App.CurrentVersionNumber);
                var result = await resultTask;
                if (resultTask.Exception != null)
                    return new ResultSuccess<int>(false, resultTask.Exception.ProperMessage());

                if (!result.Success)
                    return new ResultSuccess<int>(false, result.Message);

                var resultDeserialized = JsonConvert.DeserializeObject<ResultSuccess<UpdateDB_StuffBatchModel>>(result.Data);
                if (!resultDeserialized.Success)
                    return new ResultSuccess<int>(false, resultDeserialized.Message);

                var StuffGroups = resultDeserialized.Data.StuffGroups;
                var Units = resultDeserialized.Data.Units;
                var Stuffs = resultDeserialized.Data.Stuffs;
                var StuffOrders = resultDeserialized.Data.StuffOrders;
                var StuffBatchNumbers = resultDeserialized.Data.StuffBatchNumbers;
                var StuffBaskets = resultDeserialized.Data.StuffBaskets;
                var StuffBasketStuffs = resultDeserialized.Data.StuffBasketStuffs;
                var StuffSettlementDays = resultDeserialized.Data.StuffSettlementDays;
                var Packages = resultDeserialized.Data.Packages;

                foreach (var Stuff in Stuffs)
                {
                    var BeforeStuff = InitialStuffsData.SingleOrDefault(a => a.Id == Stuff.Id);
                    if (BeforeStuff != null)
                        Stuff.SavedImageDate = BeforeStuff.SavedImageDate;
                }
                foreach (var StuffGroup in StuffGroups)
                {
                    var BeforeStuffGroup = InitialStuffGroupsData.SingleOrDefault(a => a.Id == StuffGroup.Id);
                    if (BeforeStuffGroup != null)
                        StuffGroup.SavedImageDate = BeforeStuffGroup.SavedImageDate;
                }

                var result1 = await App.DB.InsertOrUpdateAllRecordsAsync<StuffGroup>(StuffGroups);
                if (!result1.Success)
                    return new ResultSuccess<int>(false, result1.Message);
                var result2 = await App.DB.InsertAllRecordsAsync<Unit>(Units);
                if (!result2.Success)
                    return new ResultSuccess<int>(false, result2.Message);
                var result3 = await App.DB.InsertOrUpdateAllRecordsAsync<Stuff>(Stuffs);
                if (!result3.Success)
                    return new ResultSuccess<int>(false, result3.Message);
                var result8 = await App.DB.InsertAllRecordsAsync<StuffOrder>(StuffOrders);
                if (!result8.Success)
                    return new ResultSuccess<int>(false, result8.Message);
                var result4 = await App.DB.InsertOrUpdateAllRecordsAsync<StuffBatchNumber>(StuffBatchNumbers);
                if (!result4.Success)
                    return new ResultSuccess<int>(false, result4.Message);
                var result5 = await App.DB.InsertAllRecordsAsync<StuffBasket>(StuffBaskets);
                if (!result5.Success)
                    return new ResultSuccess<int>(false, result5.Message);
                var result6 = await App.DB.InsertAllRecordsAsync<StuffBasketStuff>(StuffBasketStuffs);
                if (!result6.Success)
                    return new ResultSuccess<int>(false, result6.Message);
                var result7 = await App.DB.InsertOrUpdateAllRecordsAsync<Package>(Packages);
                if (!result7.Success)
                    return new ResultSuccess<int>(false, result7.Message);
                var result9 = await App.DB.InsertOrUpdateAllRecordsAsync<StuffSettlementDay>(StuffSettlementDays);
                if (!result9.Success)
                    return new ResultSuccess<int>(false, result9.Message);

                return new ResultSuccess<int>(true, "", resultDeserialized.Data.TotalCount);
            }
            catch (Exception err)
            {
                return new ResultSuccess<int>(false, err.ProperMessage());
            }
        }
    }

    public class PartialUpdateDB_StuffImages : PartialDBUpdater
    {
        private static Stuff[] ShouldBeDownloadedStuffsData = new Stuff[] { };
        private static StuffGroup[] ShouldBeDownloadedStuffGroupsData = new StuffGroup[] { };
        public override async Task<ResultSuccess<int>> UpdateItemFromServer(Guid RequestId, int From, int Count)
        {
            try
            {
                if (From == 1)
                {
                    ShouldBeDownloadedStuffsData =
                        (await App.DB.GetStuffsAsync())
                        .Data.Where(a => a.ServerImageDate.HasValue).ToList()
                        .Where(a => a.ServerImageDate.Value > a.SavedImageDate.GetValueOrDefault(DateTime.MinValue)).ToArray();
                    ShouldBeDownloadedStuffGroupsData =
                        (await App.DB.GetStuffGroupsAsync())
                        .Data.Where(a => a.ServerImageDate.HasValue).ToList()
                        .Where(a => a.ServerImageDate.Value > a.SavedImageDate.GetValueOrDefault(DateTime.MinValue)).ToArray();
                    if (!ShouldBeDownloadedStuffsData.Any() && !ShouldBeDownloadedStuffGroupsData.Any())
                        return new ResultSuccess<int>(true, "", 1);
                }

                Stuff ShouldBeDownloadedStuff = null;
                StuffGroup ShouldBeDownloadedStuffGroup = null;

                ShouldBeDownloadedStuff = ShouldBeDownloadedStuffsData.Skip(From - 1).FirstOrDefault();
                if (ShouldBeDownloadedStuff == null)
                    ShouldBeDownloadedStuffGroup = ShouldBeDownloadedStuffGroupsData.Skip(From - ShouldBeDownloadedStuffsData.Count() - 1).FirstOrDefault();

                var URL = "http://" + App.ServerAddress + "/Uploads/" + (ShouldBeDownloadedStuff != null ? "StuffPictures" : "StuffGroupPictures") + "/";
                var FileName = ShouldBeDownloadedStuff != null ? (ShouldBeDownloadedStuff.Id + "." + ShouldBeDownloadedStuff.ImageFileExtension) : (ShouldBeDownloadedStuffGroup.Id + "." + ShouldBeDownloadedStuffGroup.ImageFileExtension);
                var downloadResult = await App.Downloader.DownloadFile(URL, FileName, "StuffsGallery", null, null, null);
                if (!downloadResult.Success)
                    return new ResultSuccess<int>(false, downloadResult.Message);

                ResultSuccess updateDBResult = new ResultSuccess();
                if (ShouldBeDownloadedStuff != null)
                {
                    ShouldBeDownloadedStuff.SavedImageDate = ShouldBeDownloadedStuff.ServerImageDate;
                    updateDBResult = await App.DB.InsertOrUpdateRecordAsync<Stuff>(ShouldBeDownloadedStuff);
                }
                if (ShouldBeDownloadedStuffGroup != null)
                {
                    ShouldBeDownloadedStuffGroup.SavedImageDate = ShouldBeDownloadedStuffGroup.ServerImageDate;
                    updateDBResult = await App.DB.InsertOrUpdateRecordAsync<StuffGroup>(ShouldBeDownloadedStuffGroup);
                }
                if (!updateDBResult.Success)
                    return new ResultSuccess<int>(false, updateDBResult.Message);

                return new ResultSuccess<int>(true, "", ShouldBeDownloadedStuffsData.Count() + ShouldBeDownloadedStuffGroupsData.Count());
            }
            catch (Exception err)
            {
                return new ResultSuccess<int>(false, err.ProperMessage());
            }
        }
    }

    public class PartialUpdateDB_Partners : PartialDBUpdater
    {
        public override async Task<ResultSuccess<int>> UpdateItemFromServer(Guid RequestId, int From, int Count)
        {
            try
            {
                if (From == 1)
                {
                    await App.DB.SetEnabledForAllRecordsAsync<Partner>(false);

                    await App.DB.DeleteAllRecordsAsync<Zone>();
                    await App.DB.DeleteAllRecordsAsync<Credit>();
                    await App.DB.DeleteAllRecordsAsync<DynamicGroup>();
                    await App.DB.DeleteAllRecordsAsync<DynamicGroupPartner>();
                    await App.DB.DeleteAllRecordsAsync<VisitProgramPartner>();
                }

                var resultTask = HttpClient.GetStringAsyncForUnicode(ServerRoot + "GetPartners?RequestId=" + RequestId.ToString() + "&UserName=" + App.Username.Value + "&Password=" + App.Password.Value + "&From=" + From + "&Count=" + Count + "&CurrentVersionNumber=" + App.CurrentVersionNumber);
                var result = await resultTask;
                if (resultTask.Exception != null)
                    return new ResultSuccess<int>(false, resultTask.Exception.ProperMessage());

                if (!result.Success)
                    return new ResultSuccess<int>(false, result.Message);

                var resultDeserialized = JsonConvert.DeserializeObject<ResultSuccess<UpdateDB_PartnerBatchModel>>(result.Data);
                if (!resultDeserialized.Success)
                    return new ResultSuccess<int>(false, resultDeserialized.Message);

                var Cities = resultDeserialized.Data.Cities;
                var Zones = resultDeserialized.Data.Zones;
                var Routes = resultDeserialized.Data.Routes;
                var Credits = resultDeserialized.Data.Credits;
                var Partners = resultDeserialized.Data.Partners;
                var DynamicGroups = resultDeserialized.Data.DynamicGroups;
                var DynamicGroupPartners = resultDeserialized.Data.DynamicGroupPartners;
                var VisitProgramPartners = resultDeserialized.Data.VisitProgramPartners;

                var result1 = await App.DB.InsertAllRecordsAsync<Zone>(Cities);
                if (!result1.Success)
                    return new ResultSuccess<int>(false, result1.Message);
                var result2 = await App.DB.InsertAllRecordsAsync<Zone>(Zones);
                if (!result2.Success)
                    return new ResultSuccess<int>(false, result2.Message);
                var result3 = await App.DB.InsertAllRecordsAsync<Zone>(Routes);
                if (!result3.Success)
                    return new ResultSuccess<int>(false, result3.Message);
                var result4 = await App.DB.InsertAllRecordsAsync<Credit>(Credits);
                if (!result4.Success)
                    return new ResultSuccess<int>(false, result4.Message);
                var result5 = await App.DB.InsertOrUpdateAllRecordsAsync<Partner>(Partners);
                if (!result5.Success)
                    return new ResultSuccess<int>(false, result5.Message);
                var result6 = await App.DB.InsertAllRecordsAsync<DynamicGroup>(DynamicGroups);
                if (!result6.Success)
                    return new ResultSuccess<int>(false, result6.Message);
                var result7 = await App.DB.InsertAllRecordsAsync<DynamicGroupPartner>(DynamicGroupPartners);
                if (!result7.Success)
                    return new ResultSuccess<int>(false, result7.Message);
                var result8 = await App.DB.InsertAllRecordsAsync<VisitProgramPartner>(VisitProgramPartners);
                if (!result8.Success)
                    return new ResultSuccess<int>(false, result8.Message);

                return new ResultSuccess<int>(true, "", resultDeserialized.Data.TotalCount);
            }
            catch (Exception err)
            {
                return new ResultSuccess<int>(false, err.ProperMessage());
            }
        }
    }

    public class PartialUpdateDB_Stocks : PartialDBUpdater
    {
        public override async Task<ResultSuccess<int>> UpdateItemFromServer(Guid RequestId, int From, int Count)
        {
            try
            {
                if (From == 1)
                {
                    await App.DB.DeleteAllRecordsAsync<Stock>();
                    await App.DB.DeleteAllRecordsAsync<Warehouse>();
                }

                var resultTask = HttpClient.GetStringAsyncForUnicode(ServerRoot + "GetStocks?RequestId=" + RequestId.ToString() + "&UserName=" + App.Username.Value + "&Password=" + App.Password.Value + "&From=" + From + "&Count=" + Count + "&CurrentVersionNumber=" + App.CurrentVersionNumber);
                var result = await resultTask;
                if (resultTask.Exception != null)
                    return new ResultSuccess<int>(false, resultTask.Exception.ProperMessage());

                if (!result.Success)
                    return new ResultSuccess<int>(false, result.Message);

                var resultDeserialized = JsonConvert.DeserializeObject<ResultSuccess<UpdateDB_StockBatchModel>>(result.Data);
                if (!resultDeserialized.Success)
                    return new ResultSuccess<int>(false, resultDeserialized.Message);

                var Stocks = resultDeserialized.Data.Stocks;
                var Warehouses = resultDeserialized.Data.Warehouses;

                var result1 = await App.DB.InsertAllRecordsAsync(Stocks);
                if (!result1.Success)
                    return new ResultSuccess<int>(false, result1.Message);

                var result2 = await App.DB.InsertAllRecordsAsync(Warehouses);
                if (!result2.Success)
                    return new ResultSuccess<int>(false, result2.Message);

                return new ResultSuccess<int>(true, "", resultDeserialized.Data.TotalCount);
            }
            catch (Exception err)
            {
                return new ResultSuccess<int>(false, err.ProperMessage());
            }
        }
    }

    public class PartialUpdateDB_PriceLists : PartialDBUpdater
    {
        public override async Task<ResultSuccess<int>> UpdateItemFromServer(Guid RequestId, int From, int Count)
        {
            try
            {
                if (From == 1)
                {
                    await App.DB.DeleteAllRecordsAsync<PriceListZone>();
                    await App.DB.DeleteAllRecordsAsync<PriceListDynamicPartnerGroup>();
                    await App.DB.DeleteAllRecordsAsync<PriceListVersion>();
                    await App.DB.DeleteAllRecordsAsync<PriceListStuff>();
                }

                var resultTask = HttpClient.GetStringAsyncForUnicode(ServerRoot + "GetPriceLists?RequestId=" + RequestId.ToString() + "&UserName=" + App.Username.Value + "&Password=" + App.Password.Value + "&From=" + From + "&Count=" + Count + "&CurrentVersionNumber=" + App.CurrentVersionNumber);
                var result = await resultTask;
                if (resultTask.Exception != null)
                    return new ResultSuccess<int>(false, resultTask.Exception.ProperMessage());

                if (!result.Success)
                    return new ResultSuccess<int>(false, result.Message);

                var resultDeserialized = JsonConvert.DeserializeObject<ResultSuccess<UpdateDB_PriceListBatchModel>>(result.Data);
                if (!resultDeserialized.Success)
                    return new ResultSuccess<int>(false, resultDeserialized.Message);

                var PriceListZones = resultDeserialized.Data.PriceListZones;
                var PriceListDynamicPartnerGroups = resultDeserialized.Data.PriceListDynamicPartnerGroups;
                var PriceListVersions = resultDeserialized.Data.PriceListVersions;
                var PriceListStuffs = resultDeserialized.Data.PriceListStuffs;

                var result1 = await App.DB.InsertAllRecordsAsync<PriceListZone>(PriceListZones);
                if (!result1.Success)
                    return new ResultSuccess<int>(false, result1.Message);
                var result2 = await App.DB.InsertAllRecordsAsync<PriceListDynamicPartnerGroup>(PriceListDynamicPartnerGroups);
                if (!result2.Success)
                    return new ResultSuccess<int>(false, result2.Message);
                var result3 = await App.DB.InsertAllRecordsAsync<PriceListVersion>(PriceListVersions);
                if (!result3.Success)
                    return new ResultSuccess<int>(false, result3.Message);
                var result4 = await App.DB.InsertAllRecordsAsync<PriceListStuff>(PriceListStuffs);
                if (!result4.Success)
                    return new ResultSuccess<int>(false, result4.Message);

                App.LastPriceListOrDiscountRuleVersionChanged = App.LastPriceListOrDiscountRuleVersionChanged || App.LastPriceListVersion.Value != resultDeserialized.Data.LastPriceListVersion;
                App.LastPriceListVersion.Value = resultDeserialized.Data.LastPriceListVersion;

                return new ResultSuccess<int>(true, "", resultDeserialized.Data.TotalCount);
            }
            catch (Exception err)
            {
                return new ResultSuccess<int>(false, err.ProperMessage());
            }
        }
    }

    public class PartialUpdateDB_DiscountRules : PartialDBUpdater
    {
        public override async Task<ResultSuccess<int>> UpdateItemFromServer(Guid RequestId, int From, int Count)
        {
            try
            {
                if (From == 1)
                {
                    await App.DB.DeleteAllRecordsAsync<DiscountRule>();
                    await App.DB.DeleteAllRecordsAsync<DiscountRuleCondition>();
                    await App.DB.DeleteAllRecordsAsync<DiscountRuleStep>();
                    await App.DB.DeleteAllRecordsAsync<SaleDiscountRuleStuffBasket>();
                    await App.DB.DeleteAllRecordsAsync<SaleDiscountRuleStuffBasketDetail>();
                    await App.DB.DeleteAllRecordsAsync<SaleDiscountRuleStepStuffBasket>();
                }

                var resultTask = HttpClient.GetStringAsyncForUnicode(ServerRoot + "GetDiscountRules?RequestId=" + RequestId.ToString() + "&UserName=" + App.Username.Value + "&Password=" + App.Password.Value + "&From=" + From + "&Count=" + Count + "&CurrentVersionNumber=" + App.CurrentVersionNumber);
                var result = await resultTask;
                if (resultTask.Exception != null)
                    return new ResultSuccess<int>(false, resultTask.Exception.ProperMessage());

                if (!result.Success)
                    return new ResultSuccess<int>(false, result.Message);

                var resultDeserialized = JsonConvert.DeserializeObject<ResultSuccess<UpdateDB_DiscountRuleBatchModel>>(result.Data);
                if (!resultDeserialized.Success)
                    return new ResultSuccess<int>(false, resultDeserialized.Message);

                var DiscountRules = resultDeserialized.Data.DiscountRules;
                foreach (var item in DiscountRules)
                {
                    item.BeginDate = new DateTime(1970, 1, 1).AddMilliseconds(Convert.ToDouble(item.BeginDateMiliseconds));
                    item.EndDate = new DateTime(1970, 1, 1).AddMilliseconds(Convert.ToDouble(item.EndDateMiliseconds));
                }
                var DiscountRuleConditions = resultDeserialized.Data.DiscountRuleConditions;
                var DiscountRuleSteps = resultDeserialized.Data.DiscountRuleSteps;
                var DiscountRuleStuffBaskets = resultDeserialized.Data.DiscountRuleStuffBaskets;
                var DiscountRuleStuffBasketDetails = resultDeserialized.Data.DiscountRuleStuffBasketDetails;
                var DiscountRuleStepStuffBaskets = resultDeserialized.Data.DiscountRuleStepStuffBaskets;

                var result1 = await App.DB.InsertAllRecordsAsync<DiscountRule>(DiscountRules);
                if (!result1.Success)
                    return new ResultSuccess<int>(false, result1.Message);

                var result2 = await App.DB.InsertAllRecordsAsync<DiscountRuleCondition>(DiscountRuleConditions);
                if (!result2.Success)
                    return new ResultSuccess<int>(false, result2.Message);

                var result3 = await App.DB.InsertAllRecordsAsync<DiscountRuleStep>(DiscountRuleSteps);
                if (!result3.Success)
                    return new ResultSuccess<int>(false, result3.Message);

                var result4 = await App.DB.InsertAllRecordsAsync(DiscountRuleStuffBaskets);
                if (!result4.Success)
                    return new ResultSuccess<int>(false, result4.Message);

                var result5 = await App.DB.InsertAllRecordsAsync(DiscountRuleStuffBasketDetails);
                if (!result5.Success)
                    return new ResultSuccess<int>(false, result5.Message);

                var result6 = await App.DB.InsertAllRecordsAsync(DiscountRuleStepStuffBaskets);
                if (!result6.Success)
                    return new ResultSuccess<int>(false, result6.Message);

                App.LastPriceListOrDiscountRuleVersionChanged = App.LastPriceListOrDiscountRuleVersionChanged || App.LastDiscountRuleVersion.Value != resultDeserialized.Data.LastDiscountRuleVersion;
                App.LastDiscountRuleVersion.Value = resultDeserialized.Data.LastDiscountRuleVersion;

                return new ResultSuccess<int>(true, "", resultDeserialized.Data.TotalCount);
            }
            catch (Exception err)
            {
                return new ResultSuccess<int>(false, err.ProperMessage());
            }
        }
    }

    public class PartialUpdateDB_OtherInformations : PartialDBUpdater
    {
        public override async Task<ResultSuccess<int>> UpdateItemFromServer(Guid RequestId, int From, int Count)
        {
            try
            {
                if (From == 1)
                {
                    await App.DB.SetEnabledForAllRecordsAsync<NotOrderReason>(false);
                    await App.DB.SetEnabledForAllRecordsAsync<SettlementType>(false);

                    await App.DB.DeleteAllRecordsAsync<Access>();
                }

                var resultTask = HttpClient.GetStringAsyncForUnicode(ServerRoot + "GetOtherInformations?RequestId=" + RequestId.ToString() + "&UserName=" + App.Username.Value + "&Password=" + App.Password.Value + "&From=" + From + "&Count=" + Count + "&CurrentVersionNumber=" + App.CurrentVersionNumber);
                var result = await resultTask;
                if (resultTask.Exception != null)
                    return new ResultSuccess<int>(false, resultTask.Exception.ProperMessage());

                if (!result.Success)
                    return new ResultSuccess<int>(false, result.Message);

                var resultDeserialized = JsonConvert.DeserializeObject<ResultSuccess<UpdateDB_OtherInformationBatchModel>>(result.Data);
                if (!resultDeserialized.Success)
                    return new ResultSuccess<int>(false, resultDeserialized.Message);

                var NotOrderReasons = resultDeserialized.Data.NotOrderReasons;
                var SettlementTypes = resultDeserialized.Data.SettlementTypes;
                var Accesses = resultDeserialized.Data.Accesses;
                var AppSettings = resultDeserialized.Data.AppSettings;

                var result1 = await App.DB.InsertOrUpdateAllRecordsAsync<NotOrderReason>(NotOrderReasons);
                if (!result1.Success)
                    return new ResultSuccess<int>(false, result1.Message);
                var result2 = await App.DB.InsertOrUpdateAllRecordsAsync<SettlementType>(SettlementTypes);
                if (!result2.Success)
                    return new ResultSuccess<int>(false, result2.Message);
                var result3 = await App.DB.InsertAllRecordsAsync<Access>(Accesses);
                if (!result3.Success)
                    return new ResultSuccess<int>(false, result3.Message);

                if (AppSettings.Length == 1)
                {
                    App.VATPercent.Value = AppSettings[0].VATPercent;
                    App.CheckForNegativeStocksOnOrderInsertion.Value = AppSettings[0].CheckForNegativeStocksOnOrderInsertion;
                    App.OrderPrintShowMainUnitFee.Value = AppSettings[0].OrderPrintShowMainUnitFee;
                    App.OrderPrintShowSmallUnitFee.Value = AppSettings[0].OrderPrintShowSmallUnitFee;
                    App.UseBatchNumberAndExpirationDate.Value = AppSettings[0].UseBatchNumberAndExpirationDate;
                    App.UseVisitProgram.Value = AppSettings[0].UseVisitProgram;
                    App.AllowOptionalDiscountRules_MultiSelection.Value = AppSettings[0].AllowOptionalDiscountRules_MultiSelection;
                    App.SystemName.Value = AppSettings[0].SystemName;

                    App.VisitorBeginWorkTime.Value = AppSettings[0].VisitorBeginWorkTime;
                    App.VisitorEndWorkTime.Value = AppSettings[0].VisitorEndWorkTime;
                    App.GPSShouldBeTurnedOnDuringWorkTime.Value = AppSettings[0].GPSShouldBeTurnedOnDuringWorkTime;
                    App.InternetShouldBeConnectedDuringWorkTime.Value = AppSettings[0].InternetShouldBeConnectedDuringWorkTime;

                    App.ShowSaleVisitProgramPartnersToVisitorHourShift.Value = AppSettings[0].ShowSaleVisitProgramPartnersToVisitorHourShift;
                    App.DayStartTime.Value = AppSettings[0].DayStartTime;
                    App.DayEndTime.Value = AppSettings[0].DayEndTime;
                    App.CompanyNameForPrint.Value = AppSettings[0].CompanyNameForPrint;
                    App.CompanyLogoForPrint.Value = AppSettings[0].CompanyLogoForPrint;
                    if (!string.IsNullOrEmpty(AppSettings[0].CompanyLogoForPrint))
                    {
                        var URL = "http://" + App.ServerAddress + "/Uploads/CompanyLogo/";
                        var FileName = AppSettings[0].CompanyLogoForPrint;
                        var downloadResult = await App.Downloader.DownloadFile(URL, FileName, "StuffsGallery", null, null, null);
                        if (!downloadResult.Success)
                            return new ResultSuccess<int>(false, "در دانلود فایل لوگوی شرکت برای چاپ خطایی رخ داد. " + downloadResult.Message);
                    }
                    App.PrintTitle.Value = AppSettings[0].PrintTitle;
                    App.EndOfPrintDescription.Value = AppSettings[0].EndOfPrintDescription;
                    App.HasPrintingOption.Value = AppSettings[0].HasPrintingOption;
                    App.DefineWarehouseForSaleAndBuy.Value = AppSettings[0].DefineWarehouseForSaleAndBuy;
                    App.UseBarcodeScannerInVisitorAppToSelectStuff.Value = AppSettings[0].UseBarcodeScannerInVisitorAppToSelectStuff;
                    App.UseQRScannerInVisitorAppToSelectStuff.Value = AppSettings[0].UseQRScannerInVisitorAppToSelectStuff;
                    App.QRScannerInVisitorAppForSelectingStuffTemplates = AppSettings[0].QRScannerInVisitorAppForSelectingStuffTemplates;
                    App.CalculateStuffsSettlementDaysBasedOn.Value = AppSettings[0].CalculateStuffsSettlementDaysBasedOn;
                }

                var AccessesResult = await App.DB.FetchUserAccessesAsync();
                if (!AccessesResult.Success)
                    return new ResultSuccess<int>(false, AccessesResult.Message);

                return new ResultSuccess<int>(true, "", resultDeserialized.Data.TotalCount);
            }
            catch (Exception err)
            {
                return new ResultSuccess<int>(false, err.ProperMessage());
            }
        }
    }

}
