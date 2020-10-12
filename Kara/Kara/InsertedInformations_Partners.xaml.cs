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
    public class InsertedInformationsPartnersCustomCell : ViewCell
    {
        public static readonly BindableProperty PartnerIdProperty =
            BindableProperty.Create("PartnerId", typeof(Guid), typeof(InsertedInformationsPartnersCustomCell), Guid.Empty);
        public Guid PartnerId
        {
            get { return (Guid)GetValue(PartnerIdProperty); }
            set { SetValue(PartnerIdProperty, value); }
        }

        public InsertedInformationsPartnersCustomCell()
        {
            this.SetBinding(InsertedInformationsPartnersCustomCell.PartnerIdProperty, "Id");

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
                new ColumnDefinition() { Width = new GridLength(6, GridUnitType.Star) },
                new ColumnDefinition() { Width = new GridLength(5, GridUnitType.Star) },
                new ColumnDefinition() { Width = new GridLength(10, GridUnitType.Star) },
                new ColumnDefinition() { Width = new GridLength(4, GridUnitType.Star) }
            };
            var CheckboxColumnDefinition = new ColumnDefinition() { };
            GridWrapper.ColumnDefinitions.Add(CheckboxColumnDefinition);
            
            Label PhoneLabel = null, GroupLabel = null, NameLabel = null, CodeLabel = null, AddressLabel = null;
            MyCheckBox CheckBox = null;

            PhoneLabel = new Label() { LineBreakMode = LineBreakMode.TailTruncation, HorizontalOptions = LayoutOptions.Start, HorizontalTextAlignment = TextAlignment.Start, VerticalOptions = LayoutOptions.Center, FontSize = 16, TextColor = Color.FromHex("222222") };
            GroupLabel = new Label() { LineBreakMode = LineBreakMode.TailTruncation, HorizontalOptions = LayoutOptions.Start, HorizontalTextAlignment = TextAlignment.Start, VerticalOptions = LayoutOptions.Center, FontSize = 16, TextColor = Color.FromHex("222222") };
            NameLabel = new Label() { LineBreakMode = LineBreakMode.TailTruncation, HorizontalOptions = LayoutOptions.End, HorizontalTextAlignment = TextAlignment.End, VerticalOptions = LayoutOptions.Center, FontSize = 16, TextColor = Color.FromHex("222222") };
            CodeLabel = new Label() { LineBreakMode = LineBreakMode.TailTruncation, HorizontalOptions = LayoutOptions.End, HorizontalTextAlignment = TextAlignment.End, VerticalOptions = LayoutOptions.Center, FontSize = 16, TextColor = Color.FromHex("222222") };
            AddressLabel = new Label() { LineBreakMode = LineBreakMode.TailTruncation, HorizontalOptions = LayoutOptions.End, HorizontalTextAlignment = TextAlignment.End, VerticalOptions = LayoutOptions.Center, FontSize = 13 };
            CheckBox = new MyCheckBox() { HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };

            GridWrapper.Children.Add(PhoneLabel, 0, 0);
            GridWrapper.Children.Add(GroupLabel, 1, 0);
            GridWrapper.Children.Add(NameLabel, 2, 0);
            GridWrapper.Children.Add(CodeLabel, 3, 0);
            GridWrapper.Children.Add(AddressLabel, 0, 1);
            Grid.SetColumnSpan(AddressLabel, 4);
            GridWrapper.Children.Add(CheckBox, 4, 0);
            Grid.SetRowSpan(CheckBox, 2);

            PhoneLabel.SetBinding(Label.TextProperty, "Phone");
            GroupLabel.SetBinding(Label.TextProperty, "Group");
            NameLabel.SetBinding(Label.TextProperty, "Name");
            CodeLabel.SetBinding(Label.TextProperty, "Code");
            AddressLabel.SetBinding(Label.TextProperty, "Address");
            AddressLabel.SetBinding(Label.TextColorProperty, "DescriptionColor");
            CheckboxColumnDefinition.SetBinding(ColumnDefinition.WidthProperty, "CheckBoxColumnWidth");
            CheckBox.SetBinding(MyCheckBox.CheckedProperty, "Selected", BindingMode.TwoWay);
            CheckBox.SetBinding(MyCheckBox.IsVisibleProperty, "CanBeSelectedInMultiselection");

            View = GridWrapper;
        }
    }

    public partial class InsertedInformations_Partners : GradientContentPage
    {
        ObservableCollection<DBRepository.PartnerListModel> PartnersList;
        private bool justToday = false;
        private bool justLocal = false;
        InsertedInformations InsertedInformations;
        private bool BackButton_Visible, ToolbarItem_SearchBar_Visible, ToolbarItem_Delete_Visible, ToolbarItem_SendToServer_Visible, ToolbarItem_Edit_Visible, ToolbarItem_Show_Visible, ToolbarItem_SelectAll_Visible;

        public InsertedInformations_Partners(InsertedInformations InsertedInformations)
        {
            InitializeComponent();
            Title = "مشتریان";
            this.InsertedInformations = InsertedInformations;
            
            BusyIndicatorContainder.BackgroundColor = Color.FromRgba(255, 255, 255, 70);
            BusyIndicator.Color = Color.FromRgba(80, 100, 150, 255);

            PartnerItems.HasUnevenRows = true;
            PartnerItems.SeparatorColor = Color.FromHex("A5ABB7");
            PartnerItems.ItemTemplate = new DataTemplate(typeof(InsertedInformationsPartnersCustomCell));
            PartnerItems.ItemSelected += (sender, e) => {
                ((ListView)sender).SelectedItem = null;
            };
            PartnerItems.ItemTapped += (sender, e) => {
                var TappedItem = (DBRepository.PartnerListModel)e.Item;
                PartnerItems_ItemTapped(TappedItem);
            };
            PartnerItems.OnLongClick += PartnerItems_OnLongClick;

            VisitsSearchBar.TextChanged += async (sender, args) => {
                await FillPartners(args.NewTextValue);
            };
            VisitsSearchBar.SearchButtonPressed += (sender, args) => {
                VisitsSearchBar.IsVisible = false;
                FiltersSection.IsVisible = true;
                RefreshToolbarItems();
            };

            justToday = new SettingField<bool>("PartnersList_JustToday", false).Value;
            JustTodaySwitch.IsToggled = justToday;
            justLocal = new SettingField<bool>("PartnersList_JustLocal", false).Value;
            JustLocalSwitch.IsToggled = justLocal;

            JustTodaySwitch.Toggled += FilterChanged;
            JustLocalSwitch.Toggled += FilterChanged;

            FillPartners("");
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
            foreach (var item in PartnersList.Where(a => a.Selected))
                item.Selected = false;
            MultiSelectionMode = false;
            DBRepository.PartnerListModel.Multiselection = false;
            PartnerItems.ItemsSource = null;
            PartnerItems.ItemsSource = PartnersList;
            RefreshToolbarItems();
        }
        private async void PartnerItems_OnLongClick(object sender, EventArgs e)
        {
            var Position = ((ListViewLongClickEventArgs)e).Position;
            var Partner = PartnersList[Position - 1];
            if (!Partner.Sent && !MultiSelectionMode)
            {
                MultiSelectionMode = true;
                DBRepository.PartnerListModel.Multiselection = true;
                if (LastSelectedPartner != null)
                {
                    LastSelectedPartner.Selected = false;
                    LastSelectedPartner = null;
                }
                PartnerItems.ItemsSource = null;
                PartnerItems.ItemsSource = PartnersList;
                await Task.Delay(100);
                PartnerItems_ItemTapped(Partner);
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

        DBRepository.PartnerListModel LastSelectedPartner = null;
        bool? TappedItemSent;
        private void PartnerItems_ItemTapped(DBRepository.PartnerListModel TappedItem)
        {
            if (MultiSelectionMode)
            {
                LastSelectedPartner = null;
                if (!TappedItem.Sent)
                    TappedItem.Selected = !TappedItem.Selected;
            }
            else
            {
                if (TappedItem == LastSelectedPartner)
                {
                    LastSelectedPartner = null;
                    TappedItem.Selected = false;
                    TappedItemSent = null;
                }
                else
                {
                    if (LastSelectedPartner != null)
                        LastSelectedPartner.Selected = false;
                    LastSelectedPartner = TappedItem;
                    LastSelectedPartner.Selected = true;

                    TappedItemSent = TappedItem.Sent;
                }
            }

            RefreshToolbarItems();
        }

        private void ToolbarItem_SelectAll_Activated(object sender, EventArgs e)
        {
            var AllUnSentOrdersSelected = PartnersList.Where(a => !a.Sent).All(a => a.Selected);
            if (AllUnSentOrdersSelected)
            {
                foreach (var item in PartnersList.Where(a => !a.Sent && a.Selected))
                    item.Selected = false;
                InsertedInformations.ToolbarItem_SelectAll.Icon = "SelectAll_Empty.png";
            }
            else
            {
                foreach (var item in PartnersList.Where(a => !a.Sent && !a.Selected))
                    item.Selected = true;
                InsertedInformations.ToolbarItem_SelectAll.Icon = "SelectAll_Full.png";
            }
        }

        private void ToolbarItem_Show_Activated(object sender, EventArgs e)
        {
            var PartnerChange = new PartnerChange(LastSelectedPartner.PartnerData, null, this, true)
            {
                StartColor = Color.FromHex("E6EBEF"),
                EndColor = Color.FromHex("A6CFED")
            };
            this.Navigation.PushAsync(PartnerChange);
        }

        private void ToolbarItem_Edit_Activated(object sender, EventArgs e)
        {
            var PartnerChange = new PartnerChange(LastSelectedPartner.PartnerData, null, this, false)
            {
                StartColor = Color.FromHex("E6EBEF"),
                EndColor = Color.FromHex("A6CFED")
            };
            this.Navigation.PushAsync(PartnerChange);
        }

        private async void ToolbarItem_Delete_Activated(object sender, EventArgs e)
        {
            var SelectedPartners = MultiSelectionMode ? PartnersList.Where(a => a.Selected).ToArray() : new DBRepository.PartnerListModel[] { LastSelectedPartner };
            if (SelectedPartners.Length == 0)
            {
                App.ShowError("خطا", "هیچ مشتری انتخاب نشده است.", "خوب");
                return;
            }

            var answer = await DisplayAlert("حذف " + SelectedPartners.Length + " مشتری", "آیا مطمئنید؟", "بله", "خیر");
            if (answer)
            {
                WaitToggle(false);
                await Task.Delay(500);
                var result = await App.DB.DeletePartnersAsync(SelectedPartners.Select(a => a.Id).ToArray());
                if (!result.Success)
                {
                    WaitToggle(false);
                    App.ShowError("خطا", result.Message, "خوب");
                }
                else
                {
                    ExitMultiSelectionMode();
                    WaitToggle(true);
                    FillPartners(VisitsSearchBar.Text);
                }
            }
        }

        private async void ToolbarItem_SendToServer_Activated(object sender, EventArgs e)
        {
            var SelectedPartners = MultiSelectionMode ? PartnersList.Where(a => a.Selected).ToArray() : new DBRepository.PartnerListModel[] { LastSelectedPartner };
            if (SelectedPartners.Length == 0)
            {
                App.ShowError("خطا", "هیچ مشتری انتخاب نشده است.", "خوب");
                return;
            }

            WaitToggle(false);
            var submitResult = await Connectivity.SubmitPartnersAsync(SelectedPartners.Select(a => a.PartnerData).ToArray());
            if (!submitResult.Success)
            {
                WaitToggle(false);
                App.ShowError("خطا", submitResult.Message, "خوب");
                if (submitResult.Data != 0)
                {
                    ExitMultiSelectionMode();
                    FillPartners(VisitsSearchBar.Text);
                    InsertedInformations.Partners.FillPartners();
                }
            }
            else
            {
                ExitMultiSelectionMode();
                WaitToggle(true);
                FillPartners(VisitsSearchBar.Text);
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
            new SettingField<bool>("PartnersList_JustToday", justToday).Value = justToday;
            justLocal = JustLocalSwitch.IsToggled;
            new SettingField<bool>("PartnersList_JustLocal", justLocal).Value = justLocal;
            FillPartners(VisitsSearchBar.Text);
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
        public async Task FillPartners()
        {
            FillPartners(VisitsSearchBar.Text);
        }
        public async Task FillPartners(string Filter)
        {
            PartnerItems.IsRefreshing = true;
            await Task.Delay(100);

            var PartnersResult = await App.DB.GetAllPartnersListAsync(true, justToday, justLocal, null, Filter.ReplacePersianDigits());
            if (!PartnersResult.Success)
            {
                App.ShowError("خطا", "در نمایش لیست عدم سفارشات خطایی رخ داد.\n" + PartnersResult.Message, "خوب");
                PartnerItems.IsRefreshing = false;
                return;
            }

            PartnersList = new ObservableCollection<DBRepository.PartnerListModel>(PartnersResult.Data.Select(a => new DBRepository.PartnerListModel()
            {
                Id = a.Id,
                Code = a.Code.StartsWith("-") ? "---" : a.Code.ReplaceLatinDigits(),
                Name = a.Name.ReplaceLatinDigits(),
                Group = !string.IsNullOrWhiteSpace(a.Group) ? a.Group.ReplaceLatinDigits() : "---",
                Zone = a.Zone.ReplaceLatinDigits(),
                Address = (a.Zone + " - " + a.Address).ReplaceLatinDigits(),
                Phone = a.Phone.ReplaceLatinDigits(),
                HasOrder = a.HasOrder,
                HasFailedVisit = a.HasFailedVisit,
                Sent = a.Sent,
                ForChangedPartnersList = a.ForChangedPartnersList,
                PartnerData = a.PartnerData
            }));
            PartnerItems.ItemsSource = null;
            PartnerItems.ItemsSource = PartnersList;

            RefreshToolbarItems();

            PartnerItems.IsRefreshing = false;
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
                var AllUnSentOrdersSelected = PartnersList.Where(a => !a.Sent).All(a => a.Selected);
                InsertedInformations.ToolbarItem_SelectAll.Icon = AllUnSentOrdersSelected ? "SelectAll_Full.png" : "SelectAll_Empty.png";
            }
            else
                BackButton_Visible = true;

            var SelectedCount = PartnersList != null ? PartnersList.Count(a => a.Selected) : 0;
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
                if (!PartnersList.Single(a => a.Selected).Sent)
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
