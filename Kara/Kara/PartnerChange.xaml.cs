using Kara.Assets;
using Kara.CustomRenderer;
using Kara.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Kara
{
    public partial class PartnerChange : GradientContentPage
    {
        Partner EditingPartner;
        PartnerListForm PartnerListForm;
        InsertedInformations_Partners InsertedInformations_Partners;
        bool JustShow;
        private ToolbarItem ToolbarItem_LocalSave, ToolbarItem_SendToServer;
        Label FirstName_Label,
            LastName_Label,
            LegalName_Label,
            City_Label,
            Zone_Label,
            Route_Label,
            Phone1_Label,
            Phone2_Label,
            Cell_Label,
            Fax_Label,
            Address_Label,
            Credit_Label,
            IsPartnerLegal_Label,
            CalculateVATForThisPerson_Label,
            PartnerGroup_Label,
            NationalCode_Label;
        Entry FirstName,
            LastName,
            LegalName,
            Phone1,
            Phone2,
            Cell,
            Fax,
            NationalCode;

        PlaceholderEditor Address;
        Switch IsPartnerLegal, CalculateVATForThisPerson;
        Picker CityPicker, ZonePicker, RoutePicker, CreditPicker;
        Label City, CityChangeButton,
            Zone, ZoneChangeButton,
            Route, RouteChangeButton,
            Credit, CreditChangeButton;
        Zone[] Cities, Zones, Routes;
        Credit[] Credits;
        DynamicGroup[] PartnerGroups;
        KeyValuePair<Label, Switch>[] PartnerGroupSwitchs;

        
        public PartnerChange(Partner Partner, PartnerListForm PartnerListForm, InsertedInformations_Partners InsertedInformations_Partners, bool JustShow)
        {
            InitializeComponent();

            this.PartnerListForm = PartnerListForm;
            this.InsertedInformations_Partners = InsertedInformations_Partners;
            this.JustShow = JustShow;
            this.Padding = new Thickness(5, 10);

            EditingPartner = Partner;

            //این 2 گروه در گروه مشتری ها نمی آیند
            //مشتریان بدون گروه
            //پرسنل شرکت
            PartnerGroups = App.DB.GetPartnerGroups().Where(a => a.Id != new Guid("00000000-0000-0000-0000-FFFFFFFFFFFF") && a.Id != new Guid("00000000-0000-0000-0000-EEEEEEEEEEEE")).ToArray();
            PartnerGroupSwitchs = new KeyValuePair<Label, Switch>[PartnerGroups.Length];
            for (int i = 0; i < PartnerGroups.Length; i++)
                PartnerGroupSwitchs[i] = new KeyValuePair<Label, Switch>(new Label() { Text = PartnerGroups[i].Name, HorizontalOptions = LayoutOptions.EndAndExpand }, new Switch() { HorizontalOptions = LayoutOptions.End });

            City_Label = new Label() { Text = "شهر:  ", LineBreakMode = LineBreakMode.NoWrap, HorizontalTextAlignment = TextAlignment.End, VerticalTextAlignment = TextAlignment.Center, HorizontalOptions = LayoutOptions.FillAndExpand };
            Zone_Label = new Label() { Text = "منطقه:  ", LineBreakMode = LineBreakMode.NoWrap, HorizontalTextAlignment = TextAlignment.End, VerticalTextAlignment = TextAlignment.Center, HorizontalOptions = LayoutOptions.FillAndExpand };
            Route_Label = new Label() { Text = "مسیر:  ", LineBreakMode = LineBreakMode.NoWrap, HorizontalTextAlignment = TextAlignment.End, VerticalTextAlignment = TextAlignment.Center, HorizontalOptions = LayoutOptions.FillAndExpand };
            FirstName_Label = new Label() { Text = "نام:  ", LineBreakMode = LineBreakMode.NoWrap, HorizontalTextAlignment = TextAlignment.End, VerticalTextAlignment = TextAlignment.Center, HorizontalOptions = LayoutOptions.FillAndExpand };
            LastName_Label = new Label() { Text = "نام خانوادگی:  ", LineBreakMode = LineBreakMode.NoWrap, HorizontalTextAlignment = TextAlignment.End, VerticalTextAlignment = TextAlignment.Center, HorizontalOptions = LayoutOptions.FillAndExpand };
            NationalCode_Label = new Label() { Text = "کد ملی:  ", LineBreakMode = LineBreakMode.NoWrap, HorizontalTextAlignment = TextAlignment.End, VerticalTextAlignment = TextAlignment.Center, HorizontalOptions = LayoutOptions.FillAndExpand };
            LegalName_Label = new Label() { Text = "نام حقوقی:  ", LineBreakMode = LineBreakMode.NoWrap, HorizontalTextAlignment = TextAlignment.End, VerticalTextAlignment = TextAlignment.Center, HorizontalOptions = LayoutOptions.FillAndExpand };
            Phone1_Label = new Label() { Text = "تلفن 1:  ", LineBreakMode = LineBreakMode.NoWrap, HorizontalTextAlignment = TextAlignment.End, VerticalTextAlignment = TextAlignment.Center, HorizontalOptions = LayoutOptions.FillAndExpand };
            Phone2_Label = new Label() { Text = "تلفن 2:  ", LineBreakMode = LineBreakMode.NoWrap, HorizontalTextAlignment = TextAlignment.End, VerticalTextAlignment = TextAlignment.Center, HorizontalOptions = LayoutOptions.FillAndExpand };
            Cell_Label = new Label() { Text = "تلفن همراه:  ", LineBreakMode = LineBreakMode.NoWrap, HorizontalTextAlignment = TextAlignment.End, VerticalTextAlignment = TextAlignment.Center, HorizontalOptions = LayoutOptions.FillAndExpand };
            Fax_Label = new Label() { Text = "فکس:  ", LineBreakMode = LineBreakMode.NoWrap, HorizontalTextAlignment = TextAlignment.End, VerticalTextAlignment = TextAlignment.Center, HorizontalOptions = LayoutOptions.FillAndExpand };
            Address_Label = new Label() { Text = "آدرس:  ", LineBreakMode = LineBreakMode.NoWrap, HorizontalTextAlignment = TextAlignment.End, VerticalTextAlignment = TextAlignment.Center, HorizontalOptions = LayoutOptions.FillAndExpand };
            Credit_Label = new Label() { Text = "اعتبار:  ", LineBreakMode = LineBreakMode.NoWrap, HorizontalTextAlignment = TextAlignment.End, VerticalTextAlignment = TextAlignment.Center, HorizontalOptions = LayoutOptions.FillAndExpand };
            IsPartnerLegal_Label = new Label() { Text = "مشتری حقوقی است.", LineBreakMode = LineBreakMode.NoWrap, HorizontalTextAlignment = TextAlignment.End, VerticalTextAlignment = TextAlignment.Center, HorizontalOptions = LayoutOptions.EndAndExpand };
            CalculateVATForThisPerson_Label = new Label() { Text = "محاسبه مالیات ا.ا. برای این مشتری", LineBreakMode = LineBreakMode.NoWrap, HorizontalTextAlignment = TextAlignment.End, VerticalTextAlignment = TextAlignment.Center, HorizontalOptions = LayoutOptions.EndAndExpand };
            PartnerGroup_Label = new Label() { Text = "گروه مشتری:  ", LineBreakMode = LineBreakMode.NoWrap, HorizontalTextAlignment = TextAlignment.End, VerticalTextAlignment = TextAlignment.Start, HorizontalOptions = LayoutOptions.FillAndExpand };

            City = new RightRoundedLabel() { Text = "", HorizontalTextAlignment = TextAlignment.End, HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand, FontSize = 18 };
            CityPicker = new Picker() { };
            CityChangeButton = new LeftEntryCompanionLabel() { VerticalOptions = LayoutOptions.FillAndExpand, HorizontalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.Center, FontSize = 18, Text = "..." };

            Zone = new RightRoundedLabel() { Text = "", HorizontalTextAlignment = TextAlignment.End, HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand, FontSize = 18 };
            ZonePicker = new Picker() { };
            ZoneChangeButton = new LeftEntryCompanionLabel() { VerticalOptions = LayoutOptions.FillAndExpand, HorizontalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.Center, FontSize = 18, Text = "..." };

            Route = new RightRoundedLabel() { Text = "", HorizontalTextAlignment = TextAlignment.End, HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand, FontSize = 18 };
            RoutePicker = new Picker() { };
            RouteChangeButton = new LeftEntryCompanionLabel() { VerticalOptions = LayoutOptions.FillAndExpand, HorizontalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.Center, FontSize = 18, Text = "..." };

            FillZones(EditingPartner != null ? EditingPartner.ZoneId : new Nullable<Guid>(), EditingPartner != null ? EditingPartner.Groups.Select(a => a.Id).ToArray() : new Guid[] { });

            FirstName = new MyEntry() { Text = EditingPartner != null ? EditingPartner.FirstName : "", HorizontalTextAlignment = TextAlignment.End, HorizontalOptions = LayoutOptions.FillAndExpand, RightRounded = true, LeftRounded = true, Padding = new Thickness(30, 10) };
            LastName = new MyEntry() { Text = EditingPartner != null ? EditingPartner.LastName : "", HorizontalTextAlignment = TextAlignment.End, HorizontalOptions = LayoutOptions.FillAndExpand, RightRounded = true, LeftRounded = true, Padding = new Thickness(30, 10) };
            NationalCode = new MyEntry() { Text = EditingPartner != null ? EditingPartner.NationalCode : "", HorizontalTextAlignment = TextAlignment.End, HorizontalOptions = LayoutOptions.FillAndExpand, RightRounded = true, LeftRounded = true, Padding = new Thickness(30, 10) };
            LegalName = new MyEntry() { Text = EditingPartner != null ? EditingPartner.LegalName : "", HorizontalTextAlignment = TextAlignment.End, HorizontalOptions = LayoutOptions.FillAndExpand, RightRounded = true, LeftRounded = true, Padding = new Thickness(30, 10) };
            Phone1 = new MyEntry() { Text = EditingPartner != null ? EditingPartner.Phone1 : "", HorizontalTextAlignment = TextAlignment.End, HorizontalOptions = LayoutOptions.FillAndExpand, RightRounded = true, LeftRounded = true, Padding = new Thickness(30, 10), Keyboard = Keyboard.Telephone };
            Phone2 = new MyEntry() { Text = EditingPartner != null ? EditingPartner.Phone2 : "", HorizontalTextAlignment = TextAlignment.End, HorizontalOptions = LayoutOptions.FillAndExpand, RightRounded = true, LeftRounded = true, Padding = new Thickness(30, 10), Keyboard = Keyboard.Telephone };
            Cell = new MyEntry() { Text = EditingPartner != null ? EditingPartner.Mobile : "", HorizontalTextAlignment = TextAlignment.End, HorizontalOptions = LayoutOptions.FillAndExpand, RightRounded = true, LeftRounded = true, Padding = new Thickness(30, 10), Keyboard = Keyboard.Telephone };
            Fax = new MyEntry() { Text = EditingPartner != null ? EditingPartner.Fax : "", HorizontalTextAlignment = TextAlignment.End, HorizontalOptions = LayoutOptions.FillAndExpand, RightRounded = true, LeftRounded = true, Padding = new Thickness(30, 10), Keyboard = Keyboard.Telephone };
            Address = new PlaceholderEditor() { Text = EditingPartner != null ? EditingPartner.Address : "", HorizontalOptions = LayoutOptions.FillAndExpand, Padding = new Thickness(30, 10) };

            Credit = new RightRoundedLabel() { Text = "", HorizontalTextAlignment = TextAlignment.End, HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand, FontSize = 18 };
            CreditPicker = new Picker() { };
            CreditChangeButton = new LeftEntryCompanionLabel() { VerticalOptions = LayoutOptions.FillAndExpand, HorizontalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.Center, FontSize = 18, Text = "..." };

            FillCredits(EditingPartner != null ? EditingPartner.CreditId : new Nullable<Guid>());

            IsPartnerLegal = new Switch() { IsToggled = EditingPartner != null ? EditingPartner.IsLegal : false, HorizontalOptions = LayoutOptions.End };
            CalculateVATForThisPerson = new Switch() { IsToggled = EditingPartner != null ? EditingPartner.CalculateVATForThisPerson : false, HorizontalOptions = LayoutOptions.End };

            ToolbarItem_LocalSave = new ToolbarItem();
            ToolbarItem_LocalSave.Text = "ذخیره محلی";
            ToolbarItem_LocalSave.Icon = "Save.png";
            ToolbarItem_LocalSave.Clicked += SubmitPartnerToStorage;
            ToolbarItem_LocalSave.Order = ToolbarItemOrder.Primary;
            ToolbarItem_LocalSave.Priority = 0;
            if (!JustShow)
                this.ToolbarItems.Add(ToolbarItem_LocalSave);

            ToolbarItem_SendToServer = new ToolbarItem();
            ToolbarItem_SendToServer.Text = "ذخیره محلی";
            ToolbarItem_SendToServer.Icon = "Upload.png";
            ToolbarItem_SendToServer.Activated += SubmitPartnerToServer;
            ToolbarItem_SendToServer.Order = ToolbarItemOrder.Primary;
            ToolbarItem_SendToServer.Priority = 0;
            if (!JustShow)
                this.ToolbarItems.Add(ToolbarItem_SendToServer);

            BusyIndicatorContainder.BackgroundColor = Color.FromRgba(255, 255, 255, 70);
            BusyIndicator.Color = Color.FromRgba(80, 100, 150, 255);
        }

        Guid LastSizeAllocationId = Guid.NewGuid();
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
            try
            {


                if (LastWidth != width || LastHeight != height)
                {
                    LastWidth = width;
                    LastHeight = height;

                    if (LastWidth > LastHeight)
                    {
                        PartnerDataLayoutGrid.RowDefinitions = new RowDefinitionCollection();
                        for (int i = 0; i < 10; i++)
                            PartnerDataLayoutGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(i == 9 ? 1 : 40, i == 9 ? GridUnitType.Auto : GridUnitType.Absolute) });

                        PartnerDataLayoutGrid.ColumnDefinitions = new ColumnDefinitionCollection();
                        PartnerDataLayoutGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(40, GridUnitType.Absolute) });
                        PartnerDataLayoutGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(99, GridUnitType.Star) });
                        PartnerDataLayoutGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
                        PartnerDataLayoutGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(10, GridUnitType.Absolute) });
                        PartnerDataLayoutGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(40, GridUnitType.Absolute) });
                        PartnerDataLayoutGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(99, GridUnitType.Star) });
                        PartnerDataLayoutGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });

                        PartnerDataLayoutGrid.Children.Clear();

                        var RowIndex = 0;
                        PartnerDataLayoutGrid.Children.Add(City_Label, 6, RowIndex);
                        PartnerDataLayoutGrid.Children.Add(City, 5, RowIndex);
                        PartnerDataLayoutGrid.Children.Add(CityPicker, 4, RowIndex);
                        PartnerDataLayoutGrid.Children.Add(CityChangeButton, 4, RowIndex);

                        PartnerDataLayoutGrid.Children.Add(FirstName_Label, 2, RowIndex);
                        PartnerDataLayoutGrid.Children.Add(FirstName, 0, RowIndex);
                        Grid.SetColumnSpan(FirstName, 2);

                        RowIndex++;
                        PartnerDataLayoutGrid.Children.Add(Zone_Label, 6, RowIndex);
                        PartnerDataLayoutGrid.Children.Add(Zone, 5, RowIndex);
                        PartnerDataLayoutGrid.Children.Add(ZonePicker, 4, RowIndex);
                        PartnerDataLayoutGrid.Children.Add(ZoneChangeButton, 4, RowIndex);

                        PartnerDataLayoutGrid.Children.Add(LastName_Label, 2, RowIndex);
                        PartnerDataLayoutGrid.Children.Add(LastName, 0, RowIndex);
                        Grid.SetColumnSpan(LastName, 2);

                        RowIndex++;
                        PartnerDataLayoutGrid.Children.Add(NationalCode_Label, 2, RowIndex);
                        PartnerDataLayoutGrid.Children.Add(NationalCode, 0, RowIndex);
                        Grid.SetColumnSpan(NationalCode, 2);

                        RowIndex++;
                        PartnerDataLayoutGrid.Children.Add(NationalCode_Label, 2, RowIndex);
                        PartnerDataLayoutGrid.Children.Add(NationalCode, 0, RowIndex);
                        Grid.SetColumnSpan(NationalCode, 2);

                        RowIndex++;
                        PartnerDataLayoutGrid.Children.Add(Route_Label, 6, RowIndex);
                        PartnerDataLayoutGrid.Children.Add(Route, 5, RowIndex);
                        PartnerDataLayoutGrid.Children.Add(RoutePicker, 4, RowIndex);
                        PartnerDataLayoutGrid.Children.Add(RouteChangeButton, 4, RowIndex);

                        PartnerDataLayoutGrid.Children.Add(LegalName_Label, 2, RowIndex);
                        PartnerDataLayoutGrid.Children.Add(LegalName, 0, RowIndex);
                        Grid.SetColumnSpan(LegalName, 2);

                        RowIndex++;
                        PartnerDataLayoutGrid.Children.Add(Phone1_Label, 6, RowIndex);
                        PartnerDataLayoutGrid.Children.Add(Phone1, 4, RowIndex);
                        Grid.SetColumnSpan(Phone1, 2);

                        PartnerDataLayoutGrid.Children.Add(Phone2_Label, 2, RowIndex);
                        PartnerDataLayoutGrid.Children.Add(Phone2, 0, RowIndex);
                        Grid.SetColumnSpan(Phone2, 2);

                        RowIndex++;
                        PartnerDataLayoutGrid.Children.Add(Cell_Label, 6, RowIndex);
                        PartnerDataLayoutGrid.Children.Add(Cell, 4, RowIndex);
                        Grid.SetColumnSpan(Cell, 2);

                        PartnerDataLayoutGrid.Children.Add(Fax_Label, 2, RowIndex);
                        PartnerDataLayoutGrid.Children.Add(Fax, 0, RowIndex);
                        Grid.SetColumnSpan(Fax, 2);

                        RowIndex++;
                        PartnerDataLayoutGrid.Children.Add(Address_Label, 6, RowIndex);
                        PartnerDataLayoutGrid.Children.Add(Address, 0, RowIndex);
                        Grid.SetColumnSpan(Address, 6);

                        RowIndex++;
                        PartnerDataLayoutGrid.Children.Add(Credit_Label, 6, RowIndex);
                        PartnerDataLayoutGrid.Children.Add(Credit, 5, RowIndex);
                        PartnerDataLayoutGrid.Children.Add(CreditPicker, 4, RowIndex);
                        PartnerDataLayoutGrid.Children.Add(CreditChangeButton, 4, RowIndex);

                        RowIndex++;
                        var IsPartnerLegalStack = new StackLayout()
                        {
                            Orientation = StackOrientation.Horizontal,
                            Children = { IsPartnerLegal_Label, IsPartnerLegal }
                        };
                        PartnerDataLayoutGrid.Children.Add(IsPartnerLegalStack, 4, RowIndex);
                        Grid.SetColumnSpan(IsPartnerLegalStack, 3);

                        var CalculateVATForThisPersonStack = new StackLayout()
                        {
                            Orientation = StackOrientation.Horizontal,
                            Children = { CalculateVATForThisPerson_Label, CalculateVATForThisPerson }
                        };
                        PartnerDataLayoutGrid.Children.Add(CalculateVATForThisPersonStack, 0, RowIndex);
                        Grid.SetColumnSpan(CalculateVATForThisPersonStack, 3);

                        RowIndex++;
                        PartnerDataLayoutGrid.Children.Add(PartnerGroup_Label, 6, RowIndex);
                        var PartnerGroupsStack = new StackLayout() { Orientation = StackOrientation.Vertical };
                        for (int i = 0; i < PartnerGroups.Length; i++)
                        {
                            PartnerGroupsStack.Children.Add(new StackLayout()
                            {
                                Orientation = StackOrientation.Horizontal,
                                Children = { PartnerGroupSwitchs[i].Key, PartnerGroupSwitchs[i].Value }
                            });
                        }
                        PartnerDataLayoutGrid.Children.Add(PartnerGroupsStack, 0, RowIndex);
                        Grid.SetColumnSpan(PartnerGroupsStack, 6);
                    }
                    else
                    {
                        PartnerDataLayoutGrid.RowDefinitions = new RowDefinitionCollection();
                        for (int i = 0; i < 16; i++)
                            PartnerDataLayoutGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(i == 10 ? 80 : i == 15 ? 1 : 40, i == 15 ? GridUnitType.Auto : GridUnitType.Absolute) });

                        PartnerDataLayoutGrid.ColumnDefinitions = new ColumnDefinitionCollection();
                        PartnerDataLayoutGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(40, GridUnitType.Absolute) });
                        PartnerDataLayoutGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(99, GridUnitType.Star) });
                        PartnerDataLayoutGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });

                        PartnerDataLayoutGrid.Children.Clear();

                        var RowIndex = 0;
                        PartnerDataLayoutGrid.Children.Add(City_Label, 2, RowIndex);
                        PartnerDataLayoutGrid.Children.Add(City, 1, RowIndex);
                        PartnerDataLayoutGrid.Children.Add(CityPicker, 0, RowIndex);
                        PartnerDataLayoutGrid.Children.Add(CityChangeButton, 0, RowIndex);

                        RowIndex++;
                        PartnerDataLayoutGrid.Children.Add(Zone_Label, 2, RowIndex);
                        PartnerDataLayoutGrid.Children.Add(Zone, 1, RowIndex);
                        PartnerDataLayoutGrid.Children.Add(ZonePicker, 0, RowIndex);
                        PartnerDataLayoutGrid.Children.Add(ZoneChangeButton, 0, RowIndex);

                        RowIndex++;
                        PartnerDataLayoutGrid.Children.Add(Route_Label, 2, RowIndex);
                        PartnerDataLayoutGrid.Children.Add(Route, 1, RowIndex);
                        PartnerDataLayoutGrid.Children.Add(RoutePicker, 0, RowIndex);
                        PartnerDataLayoutGrid.Children.Add(RouteChangeButton, 0, RowIndex);

                        RowIndex++;
                        PartnerDataLayoutGrid.Children.Add(FirstName_Label, 2, RowIndex);
                        PartnerDataLayoutGrid.Children.Add(FirstName, 0, RowIndex);
                        Grid.SetColumnSpan(FirstName, 2);

                        RowIndex++;
                        PartnerDataLayoutGrid.Children.Add(LastName_Label, 2, RowIndex);
                        PartnerDataLayoutGrid.Children.Add(LastName, 0, RowIndex);
                        Grid.SetColumnSpan(LastName, 2);

                        RowIndex++;
                        PartnerDataLayoutGrid.Children.Add(NationalCode_Label, 2, RowIndex);
                        PartnerDataLayoutGrid.Children.Add(NationalCode, 0, RowIndex);
                        Grid.SetColumnSpan(NationalCode, 2);

                        RowIndex++;
                        PartnerDataLayoutGrid.Children.Add(LegalName_Label, 2, RowIndex);
                        PartnerDataLayoutGrid.Children.Add(LegalName, 0, RowIndex);
                        Grid.SetColumnSpan(LegalName, 2);

                        RowIndex++;
                        PartnerDataLayoutGrid.Children.Add(Phone1_Label, 2, RowIndex);
                        PartnerDataLayoutGrid.Children.Add(Phone1, 0, RowIndex);
                        Grid.SetColumnSpan(Phone1, 2);

                        RowIndex++;
                        PartnerDataLayoutGrid.Children.Add(Phone2_Label, 2, RowIndex);
                        PartnerDataLayoutGrid.Children.Add(Phone2, 0, RowIndex);
                        Grid.SetColumnSpan(Phone2, 2);

                        RowIndex++;
                        PartnerDataLayoutGrid.Children.Add(Cell_Label, 2, RowIndex);
                        PartnerDataLayoutGrid.Children.Add(Cell, 0, RowIndex);
                        Grid.SetColumnSpan(Cell, 2);

                        RowIndex++;
                        PartnerDataLayoutGrid.Children.Add(Fax_Label, 2, RowIndex);
                        PartnerDataLayoutGrid.Children.Add(Fax, 0, RowIndex);
                        Grid.SetColumnSpan(Fax, 2);

                        RowIndex++;
                        PartnerDataLayoutGrid.Children.Add(Address_Label, 2, RowIndex);
                        PartnerDataLayoutGrid.Children.Add(Address, 0, RowIndex);
                        Grid.SetColumnSpan(Address, 2);

                        RowIndex++;
                        PartnerDataLayoutGrid.Children.Add(Credit_Label, 2, RowIndex);
                        PartnerDataLayoutGrid.Children.Add(Credit, 1, RowIndex);
                        PartnerDataLayoutGrid.Children.Add(CreditPicker, 0, RowIndex);
                        PartnerDataLayoutGrid.Children.Add(CreditChangeButton, 0, RowIndex);

                        RowIndex++;
                        var IsPartnerLegalStack = new StackLayout()
                        {
                            Orientation = StackOrientation.Horizontal,
                            Children = { IsPartnerLegal_Label, IsPartnerLegal }
                        };
                        PartnerDataLayoutGrid.Children.Add(IsPartnerLegalStack, 0, RowIndex);
                        Grid.SetColumnSpan(IsPartnerLegalStack, 3);

                        RowIndex++;
                        var CalculateVATForThisPersonStack = new StackLayout()
                        {
                            Orientation = StackOrientation.Horizontal,
                            Children = { CalculateVATForThisPerson_Label, CalculateVATForThisPerson }
                        };
                        PartnerDataLayoutGrid.Children.Add(CalculateVATForThisPersonStack, 0, RowIndex);
                        Grid.SetColumnSpan(CalculateVATForThisPersonStack, 3);

                        RowIndex++;
                        PartnerDataLayoutGrid.Children.Add(PartnerGroup_Label, 2, RowIndex);
                        var PartnerGroupsStack = new StackLayout() { Orientation = StackOrientation.Vertical };
                        for (int i = 0; i < PartnerGroups.Length; i++)
                        {
                            PartnerGroupsStack.Children.Add(new StackLayout()
                            {
                                Orientation = StackOrientation.Horizontal,
                                Children = { PartnerGroupSwitchs[i].Key, PartnerGroupSwitchs[i].Value }
                            });
                        }
                        PartnerDataLayoutGrid.Children.Add(PartnerGroupsStack, 0, RowIndex);
                        Grid.SetColumnSpan(PartnerGroupsStack, 2);
                    }
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async void FillZones(Guid? RouteId, Guid[] GroupIds)
        {
            for (int i = 0; i < PartnerGroups.Length; i++)
                if (GroupIds.Any(a => a == PartnerGroups[i].Id))
                    PartnerGroupSwitchs[i].Value.IsToggled = true;
                else
                    PartnerGroupSwitchs[i].Value.IsToggled = false;

            var AllZones = (await App.DB.GetZonesAsync()).Data;
            Cities = AllZones.Where(a => a.ZoneLevel == 1).ToArray();
            Zones = AllZones.Where(a => a.ZoneLevel == 2).ToArray();
            Routes = AllZones.Where(a => a.ZoneLevel == 3).ToArray();

            foreach (var City in Cities)
                CityPicker.Items.Add(City.Name);

            var SelectedRoute = Routes.SingleOrDefault(a => a.Id == RouteId);
            if (SelectedRoute != null)
            {
                var SelectedZone = Zones.SingleOrDefault(a => a.Id == SelectedRoute.ParentId);
                var SelectedCity = Cities.SingleOrDefault(a => a.Id == SelectedZone.ParentId);

                for (int i = 0; i < Cities.Length; i++)
                    if (Cities[i].Id == SelectedCity.Id)
                    {
                        CityPicker.SelectedIndex = i;
                        City.Text = Cities[i].Name;
                        break;
                    }

                foreach (var Zone in Zones.Where(a => a.ParentId == SelectedCity.Id))
                    ZonePicker.Items.Add(Zone.Name);
                for (int i = 0; i < Zones.Length; i++)
                    if (Zones[i].Id == SelectedZone.Id)
                    {
                        ZonePicker.SelectedIndex = i;
                        Zone.Text = Zones[i].Name;
                        break;
                    }

                foreach (var Route in Routes.Where(a => a.ParentId == SelectedZone.Id))
                    RoutePicker.Items.Add(Route.Name);
                for (int i = 0; i < Routes.Length; i++)
                    if (Routes[i].Id == SelectedRoute.Id)
                    {
                        RoutePicker.SelectedIndex = i;
                        Route.Text = Routes[i].Name;
                        break;
                    }
            }

            CityPicker.SelectedIndexChanged += (sender, e) => {
                City.Text = Cities[CityPicker.SelectedIndex].Name;
                ZonePicker.Items.Clear();
                foreach (var Zone in Zones.Where(a => a.ParentId == Cities[CityPicker.SelectedIndex].Id))
                    ZonePicker.Items.Add(Zone.Name);
                ZonePicker.SelectedIndex = -1;
            };
            ZonePicker.SelectedIndexChanged += (sender, e) => {
                if (ZonePicker.SelectedIndex == -1)
                    Zone.Text = "";
                else
                {
                    Zone.Text = Zones.Where(a => a.ParentId == Cities[CityPicker.SelectedIndex].Id).ToArray()[ZonePicker.SelectedIndex].Name;
                    RoutePicker.Items.Clear();
                    foreach (var Route in Routes.Where(a => a.ParentId == Zones.Where(b => b.ParentId == Cities[CityPicker.SelectedIndex].Id).ToArray()[ZonePicker.SelectedIndex].Id))
                        RoutePicker.Items.Add(Route.Name);
                    RoutePicker.SelectedIndex = -1;
                }
            };
            RoutePicker.SelectedIndexChanged += (sender, e) => {
                if (RoutePicker.SelectedIndex == -1)
                    Route.Text = "";
                else
                {
                    Route.Text = Routes.Where(a => a.ParentId == Zones.Where(b => b.ParentId == Cities[CityPicker.SelectedIndex].Id).ToArray()[ZonePicker.SelectedIndex].Id).ToArray()[RoutePicker.SelectedIndex].Name;
                }
            };
        }

        public async void FillCredits(Guid? CreditId)
        {
            Credits = (await App.DB.GetCreditsAsync()).Data.ToArray();

            foreach (var Credit in Credits)
                CreditPicker.Items.Add(Credit.Name);

            var SelectedCredit = Credits.SingleOrDefault(a => a.Id == CreditId);
            if (SelectedCredit != null)
            {
                for (int i = 0; i < Credits.Length; i++)
                    if (Credits[i].Id == SelectedCredit.Id)
                    {
                        CreditPicker.SelectedIndex = i;
                        Credit.Text = Credits[i].Name;
                        break;
                    }
            }

            CreditPicker.SelectedIndexChanged += (sender, e) => {
                if (CreditPicker.SelectedIndex == -1)
                    Credit.Text = "";
                else
                {
                    Credit.Text = Credits[CreditPicker.SelectedIndex].Name;
                }
            };
        }

        public async void SubmitPartnerToServer(object sender, EventArgs e)
        {
            WaitToggle(false);
            await Task.Delay(100);
            var SaveResult = await SavePartner();

            if (!SaveResult.Success)
            {
                WaitToggle(false);
                App.ShowError("خطا", SaveResult.Message, "خوب");
            }
            else
            {
                var submitResult = await Connectivity.SubmitPartnersAsync(new Partner[] { SaveResult.Data });

                if (!submitResult.Success)
                {
                    WaitToggle(false);
                    App.ShowError("خطا", "اطلاعات مشتری به صورت محلی ثبت شد اما در ارسال اطلاعات به سرور خطایی رخ داده است: " + submitResult.Message, "خوب");
                }
                else
                {
                    WaitToggle(true);
                    App.ToastMessageHandler.ShowMessage("اطلاعات مشتری با موفقیت به سرور ارسال شد.", Helpers.ToastMessageDuration.Long);
                    try { await Navigation.PopAsync(); } catch (Exception) { }
                }
            }
        }

        public async void SubmitPartnerToStorage(object sender, EventArgs e)
        {
            WaitToggle(false);
            await Task.Delay(100);
            var SaveResult = await SavePartner();

            if (!SaveResult.Success)
            {
                WaitToggle(false);
                App.ShowError("خطا", SaveResult.Message, "خوب");
            }
            else
            {
                WaitToggle(true);
                App.ToastMessageHandler.ShowMessage("اطلاعات مشتری با موفقیت به صورت محلی ثبت شد.", Helpers.ToastMessageDuration.Long);
                try { await Navigation.PopAsync(); } catch (Exception) { }
            }
        }

        public void WaitToggle(bool FormWorkFinished)
        {
            if (!BusyIndicatorContainder.IsVisible)
            {
                BusyIndicatorContainder.IsVisible = true;
                this.ToolbarItems.Remove(ToolbarItem_LocalSave);
                this.ToolbarItems.Remove(ToolbarItem_SendToServer);
            }
            else
            {
                BusyIndicatorContainder.IsVisible = false;
                if (!FormWorkFinished && !JustShow)
                {
                    this.ToolbarItems.Add(ToolbarItem_LocalSave);
                    this.ToolbarItems.Add(ToolbarItem_SendToServer);
                }
            }
        }

        private async Task<ResultSuccess<Partner>> SavePartner()
        {
            try
            {
                if(!App.GpsEnabled)
                    return new ResultSuccess<Partner>(false, "لطفا مکان یاب را فعال نمایید");

                if (CityPicker.SelectedIndex == -1)
                    return new ResultSuccess<Partner>(false, "شهر انتخاب نشده است.");

                if (ZonePicker.SelectedIndex == -1)
                    return new ResultSuccess<Partner>(false, "منطقه انتخاب نشده است.");

                if (RoutePicker.SelectedIndex == -1)
                    return new ResultSuccess<Partner>(false, "مسیر انتخاب نشده است.");

                if (string.IsNullOrWhiteSpace(FirstName.Text) && string.IsNullOrWhiteSpace(LastName.Text))
                    return new ResultSuccess<Partner>(false, "نام و نام خانوادگی وارد نشده است.");

                if (CreditPicker.SelectedIndex == -1)
                    return new ResultSuccess<Partner>(false, "اعتبار انتخاب نشده است.");

                var CityId = Cities[CityPicker.SelectedIndex].Id;
                var ZoneId = Zones.Where(a => a.ParentId == CityId).ToArray()[ZonePicker.SelectedIndex].Id;
                var Route = Routes.Where(a => a.ParentId == ZoneId).ToArray()[RoutePicker.SelectedIndex];

                var DynamicGroups = new DynamicGroupPartner[] { };
                if (EditingPartner == null)
                {
                    EditingPartner = new Partner()
                    {
                        Id = Guid.NewGuid(),
                        Code = "--------" + new Random().Next(0, 999999999).ToString().PadLeft(9, '0'),
                        FirstName = FirstName.Text,
                        LastName = LastName.Text,
                        Name = FirstName.Text + " " + LastName.Text,
                        LegalName = LegalName.Text,
                        ZoneId = Route.Id,
                        GroupCode = Route.EntityGroupCode,
                        ZoneCompleteCode = Route.CompleteCode,
                        Phone1 = Phone1.Text,
                        Phone2 = Phone2.Text,
                        Mobile = Cell.Text,
                        Fax = Fax.Text,
                        Address = Address.Text,
                        IsLegal = IsPartnerLegal.IsToggled,
                        CalculateVATForThisPerson = CalculateVATForThisPerson.IsToggled,
                        Enabled = true,
                        ChangeDate = DateTime.Now,
                        Sent = false,
                        CreditId = Credits[CreditPicker.SelectedIndex].Id,
                        NationalCode = NationalCode.Text
                    };

                    DynamicGroups = PartnerGroupSwitchs.Select((a, index) => new { a, index }).ToArray().Where(a => a.a.Value.IsToggled).Select(a => new DynamicGroupPartner()
                    {
                        GroupId = PartnerGroups[a.index].Id,
                        PartnerId = EditingPartner.Id
                    }).ToArray();

                    if (!DynamicGroups.Any())
                        return new ResultSuccess<Partner>(false, "هیچ گروهی انتخاب نشده است.");

                    //if (!DynamicGroups.Any())
                    //    DynamicGroups = new DynamicGroupPartner[1] { new DynamicGroupPartner()
                    //    {
                    //        GroupId = new Guid("00000000-0000-0000-0000-EEEEEEEEEEEE"),
                    //        PartnerId = EditingPartner.Id
                    //    } };

                    var result = await App.DB.InsertOrUpdateRecordAsync<Partner>(EditingPartner);
                    if (!result.Success)
                        return new ResultSuccess<Partner>(false, result.Message);

                    result = await App.DB.InsertOrUpdateAllRecordsAsync<DynamicGroupPartner>(DynamicGroups);
                    if (!result.Success)
                        return new ResultSuccess<Partner>(false, result.Message);
                }
                else
                {
                    EditingPartner.FirstName = FirstName.Text;
                    EditingPartner.LastName = LastName.Text;
                    EditingPartner.NationalCode = NationalCode.Text;
                    EditingPartner.Name = FirstName.Text + " " + LastName.Text;
                    EditingPartner.LegalName = LegalName.Text;
                    EditingPartner.ZoneId = Route.Id;
                    EditingPartner.GroupCode = Route.EntityGroupCode;
                    EditingPartner.ZoneCompleteCode = Route.CompleteCode;
                    EditingPartner.Phone1 = Phone1.Text;
                    EditingPartner.Phone2 = Phone2.Text;
                    EditingPartner.Mobile = Cell.Text;
                    EditingPartner.Fax = Fax.Text;
                    EditingPartner.Address = Address.Text;
                    EditingPartner.IsLegal = IsPartnerLegal.IsToggled;
                    EditingPartner.CalculateVATForThisPerson = CalculateVATForThisPerson.IsToggled;
                    EditingPartner.ChangeDate = DateTime.Now;
                    EditingPartner.Sent = false;
                    EditingPartner.CreditId = Credits[CreditPicker.SelectedIndex].Id;

                    DynamicGroups = PartnerGroupSwitchs.Select((a, index) => new { a, index }).ToArray().Where(a => a.a.Value.IsToggled).Select(a => new DynamicGroupPartner()
                    {
                        GroupId = PartnerGroups[a.index].Id,
                        PartnerId = EditingPartner.Id
                    }).ToArray();

                    if (!DynamicGroups.Any())
                        return new ResultSuccess<Partner>(false, "هیچ گروهی انتخاب نشده است.");

                    //if (!DynamicGroups.Any())
                    //    DynamicGroups = new DynamicGroupPartner[1] { new DynamicGroupPartner()
                    //    {
                    //        GroupId = new Guid("00000000-0000-0000-0000-EEEEEEEEEEEE"),
                    //        PartnerId = EditingPartner.Id
                    //    } };

                    var result = await App.DB.InsertOrUpdateRecordAsync<Partner>(EditingPartner);
                    if (!result.Success)
                        return new ResultSuccess<Partner>(false, result.Message);

                    result = await App.DB.DeletePartnerDynamicGroupsAsync(EditingPartner.Id, EditingPartner.Groups.Select(a => a.Id).ToArray());
                    if (!result.Success)
                        return new ResultSuccess<Partner>(false, result.Message);

                    result = await App.DB.InsertAllRecordsAsync<DynamicGroupPartner>(DynamicGroups);
                    if (!result.Success)
                        return new ResultSuccess<Partner>(false, result.Message);
                }

                if (PartnerListForm != null)
                    await PartnerListForm.FillPartners();
                if (InsertedInformations_Partners != null)
                    await InsertedInformations_Partners.FillPartners();

                return new ResultSuccess<Partner>(true, "", EditingPartner);
            }
            catch (Exception err)
            {
                return new ResultSuccess<Partner>(false, err.ProperMessage());
            }
        }
    }
}
