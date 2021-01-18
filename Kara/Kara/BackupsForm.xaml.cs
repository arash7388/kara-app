using Kara.Assets;
using Kara.CustomRenderer;
using Kara.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Kara
{
    public class BackupModel : INotifyPropertyChanged
    {
        private double _ProgressPercent = -1;
        public double ProgressPercent
        {
            get { return _ProgressPercent; }
            set
            {
                _ProgressPercent = value;
                OnPropertyChanged("ProgressPercent");
                OnPropertyChanged("ProgressBarVisible");
                OnPropertyChanged("ProgressBarNotVisible");
            }
        }
        public bool ProgressBarVisible { get { return _ProgressPercent != -1; } }
        public bool ProgressBarNotVisible { get { return _ProgressPercent == -1; } }
        public long DateTimeMiliseconds { get; set; }
        public DateTime DateTime { get { return new DateTime(1970, 1, 1).AddMilliseconds(Convert.ToDouble(DateTimeMiliseconds)); } }
        public string Date { get { return _ProgressPercent != -1 ? "---" : DateTime.ToShortStringForDate().ToPersianDigits(); } }
        public string Time { get { return _ProgressPercent != -1 ? "---" : DateTime.ToShortStringForTime().ToPersianDigits(); } }
        public long _Size { get; set; }
        public string Size { get { return _ProgressPercent != -1 ? "---" : (_Size < 1024 ? (_Size.ToString() + " B") : _Size < Math.Pow(1024, 2) ? ((_Size / 1024.0).ToString("###,##0.##") + " KB") : _Size < Math.Pow(1024, 3) ? ((_Size / Math.Pow(1024.0, 2)).ToString("###,##0.##") + " MB") : ((_Size / Math.Pow(1024.0, 3)).ToString("###,###,###,##0.##") + " GB")).ToPersianDigits(); } }
        public string FileName { get; set; }
        private bool _Selected;
        public bool Selected {
            get { return _Selected; }
            set
            {
                _Selected = value;
                OnPropertyChanged("Selected");
                OnPropertyChanged("RowColor");
            }
        }
        public string RowColor { get { return _Selected ? "#A4DEF5" : "#DCE6FA"; } }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class BakupListCell : ViewCell
    {
        public BakupListCell()
        {
            View = GetView(true);
        }

        public View GetView(bool WithBinding)
        {
            Grid GridWrapper = new Grid()
            {
                Padding = new Thickness(5, 0),
                RowSpacing = 1,
                ColumnSpacing = 1,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                RowDefinitions = new RowDefinitionCollection()
                {
                    new RowDefinition() { Height = 40 },
                    new RowDefinition() { Height = 10 }
                },
                ColumnDefinitions = new ColumnDefinitionCollection()
                {
                    new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star)},
                    new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star)},
                    new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star)}
                }
            };
            if (WithBinding)
                GridWrapper.SetBinding(Grid.BackgroundColorProperty, "RowColor");
            else
                GridWrapper.BackgroundColor = Color.FromHex("0062C4");

            Label DateLabel = null, TimeLabel = null, SizeLabel = null;
            ProgressBar UploadProgressBar;

            DateLabel = new Label() { LineBreakMode = LineBreakMode.WordWrap, TextColor = Color.FromHex(WithBinding ? "222" : "fff"), HorizontalOptions = LayoutOptions.Center, HorizontalTextAlignment = TextAlignment.Center, VerticalOptions = LayoutOptions.Center, FontSize = 16 };
            TimeLabel = new Label() { LineBreakMode = LineBreakMode.WordWrap, TextColor = Color.FromHex(WithBinding ? "222" : "fff"), HorizontalOptions = LayoutOptions.Center, HorizontalTextAlignment = TextAlignment.Center, VerticalOptions = LayoutOptions.Center, FontSize = 16 };
            SizeLabel = new Label() { LineBreakMode = LineBreakMode.WordWrap, TextColor = Color.FromHex(WithBinding ? "222" : "fff"), HorizontalOptions = LayoutOptions.Center, HorizontalTextAlignment = TextAlignment.Center, VerticalOptions = LayoutOptions.Center, FontSize = 16 };
            UploadProgressBar = new ProgressBar() { HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.EndAndExpand };

            GridWrapper.Children.Add(DateLabel, 2, 0);
            GridWrapper.Children.Add(TimeLabel, 1, 0);
            GridWrapper.Children.Add(SizeLabel, 0, 0);
            if (WithBinding)
            {
                GridWrapper.Children.Add(UploadProgressBar, 0, 1);
                Grid.SetColumnSpan(UploadProgressBar, 3);
            }

            if (WithBinding)
            {
                DateLabel.SetBinding(Label.TextProperty, "Date");
                TimeLabel.SetBinding(Label.TextProperty, "Time");
                SizeLabel.SetBinding(Label.TextProperty, "Size");
                UploadProgressBar.SetBinding(ProgressBar.ProgressProperty, "ProgressPercent");
                UploadProgressBar.SetBinding(ProgressBar.IsVisibleProperty, "ProgressBarVisible");
            }
            else
            {
                DateLabel.Text = "تاریخ";
                TimeLabel.Text = "ساعت";
                SizeLabel.Text = "سایز";
            }

            return GridWrapper;
        }
    }

    public partial class BackupsForm : GradientContentPage
    {
        private ToolbarItem ToolbarItem_Backup, ToolbarItem_Restore;
        ObservableCollection<BackupModel> Backups = new ObservableCollection<BackupModel>();

        public BackupsForm()
        {
            InitializeComponent();

            BackupsList.ItemTapped += BackupsList_ItemTapped;
            BackupsList.ItemSelected += BackupsList_ItemSelected;
            BackupsList.SeparatorColor = Color.FromHex("A5ABB7");
            BackupsList.HasUnevenRows = true;
            BackupsList.ItemTemplate = new DataTemplate(typeof(BakupListCell));
            BackupsList.IsPullToRefreshEnabled = true;
            BackupsList.Refreshing += BackupsList_Refreshing;
            BackupsListHeader.Children.Add(new BakupListCell().GetView(false));

            ToolbarItem_Backup = new ToolbarItem();
            ToolbarItem_Backup.Text = "تهیه پشتیبان";
            ToolbarItem_Backup.Icon = "Upload.png";
            ToolbarItem_Backup.Activated += ToolbarItem_Backup_Activated;
            ToolbarItem_Backup.Order = ToolbarItemOrder.Primary;
            ToolbarItem_Backup.Priority = 1;
            
            ToolbarItem_Restore = new ToolbarItem();
            ToolbarItem_Restore.Text = "بازیابی پشتیبان";
            ToolbarItem_Restore.Icon = "Download.png";
            ToolbarItem_Restore.Activated += ToolbarItem_Restore_Activated;
            ToolbarItem_Restore.Order = ToolbarItemOrder.Primary;
            ToolbarItem_Restore.Priority = 2;

            RefreshToolbarItems();

            BackupsList_Refreshing(null, null);
        }
        
        BackupModel SelectedItem;
        private void BackupsList_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var ThisSelectedItem = (BackupModel)e.Item;
            if(ThisSelectedItem == SelectedItem)
            {
                SelectedItem.Selected = false;
                SelectedItem = null;
            }
            else
            {
                if(SelectedItem != null)
                    SelectedItem.Selected = false;
                SelectedItem = ThisSelectedItem;
                SelectedItem.Selected = true;
            }

            RefreshToolbarItems();
        }
        bool OperationInProgress = false;
        void RefreshToolbarItems()
        {
            if(OperationInProgress)
            {
                if (this.ToolbarItems.Contains(ToolbarItem_Restore))
                    this.ToolbarItems.Remove(ToolbarItem_Restore);
                if (this.ToolbarItems.Contains(ToolbarItem_Backup))
                    this.ToolbarItems.Remove(ToolbarItem_Backup);
            }
            else if (SelectedItem == null)
            {
                if (this.ToolbarItems.Contains(ToolbarItem_Restore))
                    this.ToolbarItems.Remove(ToolbarItem_Restore);
                if (!this.ToolbarItems.Contains(ToolbarItem_Backup))
                    this.ToolbarItems.Add(ToolbarItem_Backup);
            }
            else
            {
                if (this.ToolbarItems.Contains(ToolbarItem_Backup))
                    this.ToolbarItems.Remove(ToolbarItem_Backup);
                if (!this.ToolbarItems.Contains(ToolbarItem_Restore))
                    this.ToolbarItems.Add(ToolbarItem_Restore);
            }
        }
        private void BackupsList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ((ListView)sender).SelectedItem = null;
        }

        BackupModel UploadingBackup;
        private async void ToolbarItem_Backup_Activated(object sender, EventArgs e)
        {
            OperationInProgress = true;
            RefreshToolbarItems();
            UploadingBackup = new BackupModel() { ProgressPercent = 0 };
            var newList = new List<BackupModel> { UploadingBackup };
            newList.AddRange(Backups.ToList());
            Backups = new ObservableCollection<BackupModel>(newList);
            BackupsList.ItemsSource = null;
            BackupsList.ItemsSource = Backups;

            var result = await Connectivity.UploadBackupToServerAsync(new UploadFileCompleted(UploadFileCompleted), new UploadProgressChanged(UploadProgressChanged));
            if (!result.Success)
                App.ShowError("خطا", result.Message, "خوب");
            
            Backups = new ObservableCollection<BackupModel>(Backups.ToList().Union(new BackupModel[] { UploadingBackup }));
            UploadingBackup = null;
            BackupsList.ItemsSource = null;
            BackupsList.ItemsSource = Backups;
            OperationInProgress = false;
            BackupsList_Refreshing(null, null);
            
            App.ToastMessageHandler.ShowMessage("پشتیبان اطلاعات با موفقیت به سرور ارسال شد.", ToastMessageDuration.Long);
        }
        void UploadFileCompleted()
        {
        }
        void UploadProgressChanged(int progressPercentage)
        {
            UploadingBackup.ProgressPercent = progressPercentage / 100.0;
        }

        private async void ToolbarItem_Restore_Activated(object sender, EventArgs e)
        {
            var answer = await DisplayAlert("بازیابی پایگاه داده", "آیا مطمئنید؟", "بله", "خیر");
            if (answer)
            {
                OperationInProgress = true;
                RefreshToolbarItems();
                SelectedItem.ProgressPercent = 0;

                var result = await Connectivity.DownloadBackupFromServerAsync(SelectedItem.FileName, new DownloadFileCompleted(DownloadFileCompleted), new DownloadProgressChanged(DownloadProgressChanged));
                if (!result.Success)
                    App.ShowError("خطا", result.Message, "خوب");
                
                SelectedItem.ProgressPercent = -1;
                OperationInProgress = false;
                RefreshToolbarItems();

                App.ToastMessageHandler.ShowMessage("فایل پشتیبان اطلاعات مورد نظر با موفقیت بازیابی شد.", ToastMessageDuration.Long);
            }
        }
        void DownloadFileCompleted()
        {
        }
        void DownloadProgressChanged(int progressPercentage)
        {
            SelectedItem.ProgressPercent = progressPercentage / 100.0;
        }

        private async void BackupsList_Refreshing(object sender, EventArgs e)
        {
            BackupsList.IsRefreshing = true;
            await Task.Delay(100);

            var Result = await Connectivity.GetBackupListFromServerAsync();
            if (!Result.Success)
            {
                App.ShowError("خطا", "در نمایش لیست پشتیبان ها خطایی رخ داد.\n" + Result.Message, "خوب");
                BackupsList.IsRefreshing = false;
                return;
            }

            Backups = new ObservableCollection<BackupModel>(Result.Data);
            BackupsList.ItemsSource = null;
            BackupsList.ItemsSource = Backups;
            SelectedItem = null;
            RefreshToolbarItems();

            await Task.Delay(100);
            BackupsList.IsRefreshing = false;
        }
    }
}
