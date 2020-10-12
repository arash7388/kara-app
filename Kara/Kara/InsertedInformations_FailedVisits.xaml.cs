using Kara.Assets;
using Kara.CustomRenderer;
using Plugin.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Kara
{
    public class InsertedInformationsFailedVisitsCustomCell : ViewCell
    {
        public static readonly BindableProperty VisitIdProperty =
            BindableProperty.Create("VisitId", typeof(Guid), typeof(InsertedInformationsFailedVisitsCustomCell), Guid.Empty);
        public Guid VisitId
        {
            get { return (Guid)GetValue(VisitIdProperty); }
            set { SetValue(VisitIdProperty, value); }
        }

        public InsertedInformationsFailedVisitsCustomCell()
        {
            this.SetBinding(InsertedInformationsFailedVisitsCustomCell.VisitIdProperty, "Id");

            Grid GridWrapper = new Grid()
            {
                Padding = new Thickness(5, 0),
                RowSpacing = 1,
                ColumnSpacing = 1,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };
            GridWrapper.SetBinding(Grid.BackgroundColorProperty, "RowColor");

            GridWrapper.RowDefinitions = new RowDefinitionCollection()
            {
                new RowDefinition() { Height = 35 },
                new RowDefinition() { Height = 25 }
            };

            GridWrapper.ColumnDefinitions = new ColumnDefinitionCollection()
            {
                new ColumnDefinition() { Width = new GridLength(4, GridUnitType.Star) },
                new ColumnDefinition() { Width = new GridLength(4, GridUnitType.Star) },
                new ColumnDefinition() { Width = new GridLength(8, GridUnitType.Star) }
            };
            var CheckboxColumnDefinition = new ColumnDefinition() { };
            GridWrapper.ColumnDefinitions.Add(CheckboxColumnDefinition);

            Label PartnerNameLabel = null, DateLabel = null, TimeLabel = null, DescriptionLabel = null;
            MyCheckBox CheckBox = null;

            DateLabel = new Label() { LineBreakMode = LineBreakMode.TailTruncation, HorizontalOptions = LayoutOptions.Start, HorizontalTextAlignment = TextAlignment.Start, VerticalOptions = LayoutOptions.Center, FontSize = 16, TextColor = Color.FromHex("222222") };
            TimeLabel = new Label() { LineBreakMode = LineBreakMode.TailTruncation, HorizontalOptions = LayoutOptions.Start, HorizontalTextAlignment = TextAlignment.Start, VerticalOptions = LayoutOptions.Center, FontSize = 16, TextColor = Color.FromHex("222222") };
            PartnerNameLabel = new Label() { LineBreakMode = LineBreakMode.TailTruncation, HorizontalOptions = LayoutOptions.End, HorizontalTextAlignment = TextAlignment.End, VerticalOptions = LayoutOptions.Center, FontSize = 16, TextColor = Color.FromHex("222222") };
            DescriptionLabel = new Label() { LineBreakMode = LineBreakMode.TailTruncation, HorizontalOptions = LayoutOptions.End, HorizontalTextAlignment = TextAlignment.End, VerticalOptions = LayoutOptions.Center, FontSize = 13 };
            CheckBox = new MyCheckBox() { HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };

            GridWrapper.Children.Add(DateLabel, 0, 0);
            GridWrapper.Children.Add(TimeLabel, 1, 0);
            GridWrapper.Children.Add(PartnerNameLabel, 2, 0);
            GridWrapper.Children.Add(DescriptionLabel, 0, 1);
            Grid.SetColumnSpan(DescriptionLabel, 3);
            GridWrapper.Children.Add(CheckBox, 3, 0);
            Grid.SetRowSpan(CheckBox, 2);

            DateLabel.SetBinding(Label.TextProperty, "Date");
            TimeLabel.SetBinding(Label.TextProperty, "Time");
            PartnerNameLabel.SetBinding(Label.TextProperty, "PartnerName");
            DescriptionLabel.SetBinding(Label.TextProperty, "Description");
            DescriptionLabel.SetBinding(Label.TextColorProperty, "DescriptionColor");
            CheckboxColumnDefinition.SetBinding(ColumnDefinition.WidthProperty, "CheckBoxColumnWidth");
            CheckBox.SetBinding(MyCheckBox.CheckedProperty, "Selected", BindingMode.TwoWay);
            CheckBox.SetBinding(MyCheckBox.IsVisibleProperty, "CanBeSelectedInMultiselection");

            View = GridWrapper;
        }
    }

    public partial class InsertedInformations_FailedVisits : GradientContentPage
    {
        ObservableCollection<DBRepository.FailedVisitListModel> FailedVisitsList;
        private bool justToday = false;
        private bool justLocal = false;
        InsertedInformations InsertedInformations;
        private bool BackButton_Visible, ToolbarItem_SearchBar_Visible, ToolbarItem_Delete_Visible, ToolbarItem_SendToServer_Visible, ToolbarItem_Edit_Visible, ToolbarItem_Show_Visible, ToolbarItem_SelectAll_Visible;

        public InsertedInformations_FailedVisits(InsertedInformations InsertedInformations)
        {
            InitializeComponent();
            Title = "عدم سفارشات";
            this.InsertedInformations = InsertedInformations;

            BusyIndicatorContainder.BackgroundColor = Color.FromRgba(255, 255, 255, 70);
            BusyIndicator.Color = Color.FromRgba(80, 100, 150, 255);

            FailedVisitItems.HasUnevenRows = true;
            FailedVisitItems.SeparatorColor = Color.FromHex("A5ABB7");
            FailedVisitItems.ItemTemplate = new DataTemplate(typeof(InsertedInformationsFailedVisitsCustomCell));
            FailedVisitItems.ItemSelected += (sender, e) => {
                ((ListView)sender).SelectedItem = null;
            };
            FailedVisitItems.ItemTapped += (sender, e) => {
                var TappedItem = (DBRepository.FailedVisitListModel)e.Item;
                FailedVisitItems_ItemTapped(TappedItem);
            };
            FailedVisitItems.OnLongClick += FailedVisitItems_OnLongClick;

            VisitsSearchBar.TextChanged += async (sender, args) => {
                await FillFailedVisits(args.NewTextValue);
            };
            VisitsSearchBar.SearchButtonPressed += (sender, args) => {
                VisitsSearchBar.IsVisible = false;
                FiltersSection.IsVisible = true;
                RefreshToolbarItems();
            };

            justToday = new SettingField<bool>("FailedVisitsList_JustToday", false).Value;
            JustTodaySwitch.IsToggled = justToday;
            justLocal = new SettingField<bool>("FailedVisitsList_JustLocal", false).Value;
            JustLocalSwitch.IsToggled = justLocal;

            JustTodaySwitch.Toggled += FilterChanged;
            JustLocalSwitch.Toggled += FilterChanged;

            FillFailedVisits("");
        }
        
        public void SetCurrentPage()
        {
            InsertedInformations.ToolbarItem_SearchBar.Activated += ToolbarItem_SearchBar_Activated;
            InsertedInformations.ToolbarItem_SendToServer.Activated += ToolbarItem_SendToServer_Activated;
            InsertedInformations.ToolbarItem_Delete.Activated += ToolbarItem_Delete_Activated;
            InsertedInformations.ToolbarItem_Edit.Activated += ToolbarItem_Edit_Activated;
            InsertedInformations.ToolbarItem_Show.Activated += ToolbarItem_Show_Activated;
            InsertedInformations.ToolbarItem_SelectAll.Activated += ToolbarItem_SelectAll_Activated;
            RefreshToolbarItems();
        }
        public void UnsetCurrentPage()
        {
            InsertedInformations.ToolbarItem_SearchBar.Activated -= ToolbarItem_SearchBar_Activated;
            InsertedInformations.ToolbarItem_SendToServer.Activated -= ToolbarItem_SendToServer_Activated;
            InsertedInformations.ToolbarItem_Delete.Activated -= ToolbarItem_Delete_Activated;
            InsertedInformations.ToolbarItem_Edit.Activated -= ToolbarItem_Edit_Activated;
            InsertedInformations.ToolbarItem_Show.Activated -= ToolbarItem_Show_Activated;
            InsertedInformations.ToolbarItem_SelectAll.Activated -= ToolbarItem_SelectAll_Activated;
        }

        bool MultiSelectionMode = false;
        private async void ExitMultiSelectionMode()
        {
            foreach (var item in FailedVisitsList.Where(a => a.Selected))
                item.Selected = false;
            MultiSelectionMode = false;
            DBRepository.FailedVisitListModel.Multiselection = false;
            FailedVisitItems.ItemsSource = null;
            FailedVisitItems.ItemsSource = FailedVisitsList;
            RefreshToolbarItems();
        }
        private async void FailedVisitItems_OnLongClick(object sender, EventArgs e)
        {
            var Position = ((ListViewLongClickEventArgs)e).Position;
            var FailedVisit = FailedVisitsList[Position - 1];
            if (!FailedVisit.Sent && !MultiSelectionMode)
            {
                MultiSelectionMode = true;
                DBRepository.FailedVisitListModel.Multiselection = true;
                if (LastSelectedFailedVisit != null)
                {
                    LastSelectedFailedVisit.Selected = false;
                    LastSelectedFailedVisit = null;
                }
                FailedVisitItems.ItemsSource = null;
                FailedVisitItems.ItemsSource = FailedVisitsList;
                await Task.Delay(100);
                FailedVisitItems_ItemTapped(FailedVisit);
            }
        }

        protected override bool OnBackButtonPressed()
        {
            if (MultiSelectionMode)
            {
                ExitMultiSelectionMode();
                return true;
            }
            return base.OnBackButtonPressed();
        }

        DBRepository.FailedVisitListModel LastSelectedFailedVisit = null;
        bool? TappedItemSent;
        private void FailedVisitItems_ItemTapped(DBRepository.FailedVisitListModel TappedItem)
        {
            if (MultiSelectionMode)
            {
                LastSelectedFailedVisit = null;
                if (!TappedItem.Sent)
                    TappedItem.Selected = !TappedItem.Selected;
            }
            else
            {
                if (TappedItem == LastSelectedFailedVisit)
                {
                    LastSelectedFailedVisit = null;
                    TappedItem.Selected = false;
                    TappedItemSent = null;
                }
                else
                {
                    if (LastSelectedFailedVisit != null)
                        LastSelectedFailedVisit.Selected = false;
                    LastSelectedFailedVisit = TappedItem;
                    LastSelectedFailedVisit.Selected = true;

                    TappedItemSent = TappedItem.Sent;
                }
            }

            RefreshToolbarItems();
        }

        private void ToolbarItem_SelectAll_Activated(object sender, EventArgs e)
        {
            var AllUnSentOrdersSelected = FailedVisitsList.Where(a => !a.Sent).All(a => a.Selected);
            if (AllUnSentOrdersSelected)
            {
                foreach (var item in FailedVisitsList.Where(a => !a.Sent && a.Selected))
                    item.Selected = false;
                InsertedInformations.ToolbarItem_SelectAll.Icon = "SelectAll_Empty.png";
            }
            else
            {
                foreach (var item in FailedVisitsList.Where(a => !a.Sent && !a.Selected))
                    item.Selected = true;
                InsertedInformations.ToolbarItem_SelectAll.Icon = "SelectAll_Full.png";
            }
        }

        private void ToolbarItem_Show_Activated(object sender, EventArgs e)
        {
            var FailedOrderInsertForm = new FailedOrderInsertForm(null, LastSelectedFailedVisit.FailedVisitData.Id, null, null)
            {
                StartColor = Color.FromHex("E6EBEF"),
                EndColor = Color.FromHex("A6CFED")
            };
            this.Navigation.PushAsync(FailedOrderInsertForm);
        }

        private void ToolbarItem_Edit_Activated(object sender, EventArgs e)
        {
            var FailedOrderInsertForm = new FailedOrderInsertForm(null, LastSelectedFailedVisit.FailedVisitData.Id, this, null)
            {
                StartColor = Color.FromHex("E6EBEF"),
                EndColor = Color.FromHex("A6CFED")
            };
            this.Navigation.PushAsync(FailedOrderInsertForm);
        }

        private async void ToolbarItem_Delete_Activated(object sender, EventArgs e)
        {
            var SelectedFailedVisits = MultiSelectionMode ? FailedVisitsList.Where(a => a.Selected).ToArray() : new DBRepository.FailedVisitListModel[] { LastSelectedFailedVisit };
            if (SelectedFailedVisits.Length == 0)
            {
                App.ShowError("خطا", "هیچ عدم سفارشی انتخاب نشده است.", "خوب");
                return;
            }

            var answer = await DisplayAlert("حذف " + SelectedFailedVisits.Length + " عدم سفارش", "آیا مطمئنید؟", "بله", "خیر");
            if (answer)
            {
                WaitToggle(false);
                await Task.Delay(500);
                var result = await App.DB.DeleteFailedVisitsAsync(SelectedFailedVisits.Select(a => a.Id).ToArray());
                if (!result.Success)
                {
                    WaitToggle(false);
                    App.ShowError("خطا", result.Message, "خوب");
                }
                else
                {
                    ExitMultiSelectionMode();
                    WaitToggle(true);
                    FillFailedVisits(VisitsSearchBar.Text);
                }
            }
        }

        private async void ToolbarItem_SendToServer_Activated(object sender, EventArgs e)
        {
            var SelectedFailedVisits = MultiSelectionMode ? FailedVisitsList.Where(a => a.Selected).ToArray() : new DBRepository.FailedVisitListModel[] { LastSelectedFailedVisit };
            if (SelectedFailedVisits.Length == 0)
            {
                App.ShowError("خطا", "هیچ عدم سفارشی انتخاب نشده است.", "خوب");
                return;
            }

            WaitToggle(false);
            var submitResult = await Connectivity.SubmitFailedVisitsAsync(SelectedFailedVisits.Select(a => a.FailedVisitData).ToArray());
            if (!submitResult.Success)
            {
                WaitToggle(false);
                App.ShowError("خطا", submitResult.Message, "خوب");
                if (submitResult.Data != 0)
                {
                    ExitMultiSelectionMode();
                    FillFailedVisits(VisitsSearchBar.Text);
                    InsertedInformations.Partners.FillPartners();
                }
            }
            else
            {
                ExitMultiSelectionMode();
                WaitToggle(true);
                FillFailedVisits(VisitsSearchBar.Text);
                InsertedInformations.Partners.FillPartners();
            }
        }

        private void ToolbarItem_SearchBar_Activated(object sender, EventArgs e)
        {
            VisitsSearchBar.IsVisible = true;
            FiltersSection.IsVisible = false;
            RefreshToolbarItems();
        }

        public void FilterChanged(object sender, EventArgs e)
        {
            justToday = JustTodaySwitch.IsToggled;
            new SettingField<bool>("FailedVisitsList_JustToday", justToday).Value = justToday;
            justLocal = JustLocalSwitch.IsToggled;
            new SettingField<bool>("FailedVisitsList_JustLocal", justLocal).Value = justLocal;
            FillFailedVisits(VisitsSearchBar.Text);
        }

        public void WaitToggle(bool WithSuccess)
        {
            if (!BusyIndicatorContainder.IsVisible)
            {
                BusyIndicatorContainder.IsVisible = true;
            }
            else
            {
                BusyIndicatorContainder.IsVisible = false;
                if (!WithSuccess)
                {
                }
            }
            RefreshToolbarItems();
        }

        private async Task FillFailedVisits(string Filter)
        {
            FailedVisitItems.IsRefreshing = true;
            await Task.Delay(100);

            var FailedVisitsResult = await App.DB.GetFailedVisitsListAsync(justToday, justLocal, Filter.ReplacePersianDigits());
            if (!FailedVisitsResult.Success)
            {
                App.ShowError("خطا", "در نمایش لیست عدم سفارشات خطایی رخ داد.\n" + FailedVisitsResult.Message, "خوب");
                FailedVisitItems.IsRefreshing = false;
                return;
            }
            FailedVisitsList = new ObservableCollection<DBRepository.FailedVisitListModel>(FailedVisitsResult.Data);
            FailedVisitItems.ItemsSource = null;
            FailedVisitItems.ItemsSource = FailedVisitsList;

            RefreshToolbarItems();
            FailedVisitItems.IsRefreshing = false;
        }

        public void RefreshToolbarItems()
        {
            ToolbarItem_SendToServer_Visible = false;
            ToolbarItem_Delete_Visible = false;
            ToolbarItem_Edit_Visible = false;
            ToolbarItem_Show_Visible = false;
            ToolbarItem_SearchBar_Visible = false;
            ToolbarItem_SelectAll_Visible = false;
            BackButton_Visible = false;

            if (MultiSelectionMode)
            {
                ToolbarItem_SelectAll_Visible = true;
                var AllUnSentOrdersSelected = FailedVisitsList.Where(a => !a.Sent).All(a => a.Selected);
                InsertedInformations.ToolbarItem_SelectAll.Icon = AllUnSentOrdersSelected ? "SelectAll_Full.png" : "SelectAll_Empty.png";
            }
            else
                BackButton_Visible = true;

            var SelectedCount = FailedVisitsList != null ? FailedVisitsList.Count(a => a.Selected) : 0;
            if (SelectedCount == 0)
            {
                if (!MultiSelectionMode)
                    ToolbarItem_SearchBar_Visible = true;
            }
            else if (MultiSelectionMode)
            {
                ToolbarItem_SendToServer_Visible = true;
                ToolbarItem_Delete_Visible = true;
            }
            else
            {
                if (!FailedVisitsList.Single(a => a.Selected).Sent)
                {
                    ToolbarItem_SendToServer_Visible = true;
                    ToolbarItem_Delete_Visible = true;
                    ToolbarItem_Edit_Visible = true;
                }
                ToolbarItem_Show_Visible = true;
            }

            InsertedInformations.RefreshToolbarItems(BackButton_Visible, ToolbarItem_SearchBar_Visible, ToolbarItem_Delete_Visible, ToolbarItem_SendToServer_Visible, ToolbarItem_Edit_Visible, ToolbarItem_Show_Visible, ToolbarItem_SelectAll_Visible);
        }
    }
}
