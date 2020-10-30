﻿using Kara.Assets;
using Kara.CustomRenderer;
using Kara.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Kara.Assets.Connectivity;

namespace Kara
{
    public class TahsildarCustomCell : ViewCell
    {
        public static readonly BindableProperty OrderIdProperty = BindableProperty.Create("OrderId", typeof(Guid), typeof(TahsildarCustomCell), Guid.Empty);
        public Guid OrderId
        {
            get { return (Guid)GetValue(OrderIdProperty); }
            set { SetValue(OrderIdProperty, value); }
        }

        public TahsildarCustomCell()
        {
            this.SetBinding(OrderIdProperty, "Id");
        }
    }

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TahsildarForm : GradientContentPage
    {

        private bool? TappedItemSent;
        private UnSettledOrderModel LastSelectedOrder = null;
        private bool MultiSelectionMode = false;
        private Partner _BeforeSelectedPartner;
        private Partner _SelectedPartner;
        public Partner SelectedPartner
        {
            get { return _SelectedPartner; }
            set
            {
                _BeforeSelectedPartner = _SelectedPartner;
                _SelectedPartner = value;
                 PartnerSelected();
            }
        }
        public ObservableCollection<UnSettledOrderModel> FactorsObservableCollection { get; private set; }

        LeftEntryCompanionLabel PartnerChangeButton;
        RightRoundedLabel PartnerLabel;
        bool TapEventHandlingInProgress = false;

        public TahsildarForm()
        {
            InitializeComponent();
            FactorsView.ItemTapped += FactorsView_ItemTapped;
            FactorsView.OnLongClick += FactorsView_OnLongClick;
            
            BusyIndicatorContainder.BackgroundColor = Color.FromRgba(255, 255, 255, 70);
            BusyIndicator.Color = Color.FromRgba(80, 100, 150, 255);

            PartnerChangeButton = new LeftEntryCompanionLabel() { VerticalOptions = LayoutOptions.FillAndExpand, VerticalTextAlignment = TextAlignment.Center, HorizontalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.Center, FontSize = 18, Text = "🔍" };
            
            PartnerLabel = new RightRoundedLabel() { VerticalOptions = LayoutOptions.FillAndExpand, VerticalTextAlignment = TextAlignment.Center, HorizontalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.End, Text = "مشتری", Padding = new Thickness(0, 0, 50, 0) };
            
            PartnerLabel.FontSize *= 1.5;
            
            if (SelectedPartner == null)
            {
                PartnerChangeButton.IsEnabled = true;
                App.UniversalLineInApp = 875234038;
            }
            else
            {
                PartnerChangeButton.IsEnabled = false;
                App.UniversalLineInApp = 875234040;
            }

            var PartnerChangeButtonTapGestureRecognizer = new TapGestureRecognizer();
            PartnerChangeButtonTapGestureRecognizer.Tapped += (sender, e) => {
                if (!TapEventHandlingInProgress)
                {
                    TapEventHandlingInProgress = true;

                    try
                    {
                        //App.UniversalLineInApp = 875234043;
                        if (PartnerChangeButton.IsEnabled)
                        {
                            App.UniversalLineInApp = 875234044;
                            //FocusedQuantityTextBoxId = null;
                            //App.UniversalLineInApp = 875234045;
                            var PartnerListForm1 = new PartnerListForm();
                            //App.UniversalLineInApp = 875234046;
                            PartnerListForm1.TahsildarForm = this;  
                            //App.UniversalLineInApp = 875234047;
                            Navigation.PushAsync(PartnerListForm1);
                            //App.UniversalLineInApp = 875234048;
                        }
                        //App.UniversalLineInApp = 875234049;
                    }
                    catch (Exception)
                    { }

                    TapEventHandlingInProgress = false;
                }
            };

            App.UniversalLineInApp = 875234050;
            PartnerChangeButtonTapGestureRecognizer.SetBinding(TapGestureRecognizer.CommandParameterProperty, "Id");
            PartnerChangeButton.GestureRecognizers.Add(PartnerChangeButtonTapGestureRecognizer);
            PartnerChangeButton.WidthRequest = 150;

           
        }

        private void FactorsView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var TappedItem = (UnSettledOrderModel)e.Item;
            FactorsView_ItemTapped(TappedItem);
        }

        private void FactorsView_ItemTapped(UnSettledOrderModel TappedItem)
        {
            if (MultiSelectionMode)
            {
                LastSelectedOrder = null;
                TappedItem.Selected = !TappedItem.Selected;
            }
            else
            {
                if (TappedItem == LastSelectedOrder)
                {
                    LastSelectedOrder = null;
                    TappedItem.Selected = false;
                    TappedItemSent = null;
                }
                else
                {
                    if (LastSelectedOrder != null)
                        LastSelectedOrder.Selected = false;
                    LastSelectedOrder = TappedItem;
                    LastSelectedOrder.Selected = true;

                    TappedItemSent = TappedItem.Sent;
                }
            }

            RefreshToolbarItems();
        }

        private async void FactorsView_OnLongClick(object sender, EventArgs e)
        {
            var Position = ((ListViewLongClickEventArgs)e).Position;
            var Order = FactorsObservableCollection[Position - 1];

            if (!MultiSelectionMode)
            {
                MultiSelectionMode = true;
                UnSettledOrderModel.Multiselection = true;

                if (LastSelectedOrder != null)
                {
                    LastSelectedOrder.Selected = false;
                    LastSelectedOrder = null;
                }

                FactorsView.ItemsSource = null;
                FactorsView.ItemsSource = FactorsObservableCollection;

                await Task.Delay(100);
                FactorsView_ItemTapped(Order);
            }
        }
                
        Guid LastSizeAllocationId;
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
            if (LastWidth != width || LastHeight != height)
            {
                LastWidth = width;
                LastHeight = height;

                RefreshPartnerSection(LastWidth > LastHeight);
            }
        }

        private void RefreshPartnerSection(bool Horizental)
        {
            PartnerSection.RowDefinitions = new RowDefinitionCollection();
            PartnerSection.ColumnDefinitions = new ColumnDefinitionCollection();
            PartnerSection.Children.Clear();

            if (Horizental)
            {
                PartnerSection.RowDefinitions.Add(new RowDefinition() { Height = 50 });
                
                PartnerSection.ColumnDefinitions.Add(new ColumnDefinition() { Width = 50 });
                PartnerSection.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
                var GallaryStuffGroupOffset = 0;
                
                PartnerSection.Children.Add(PartnerChangeButton, 0 + GallaryStuffGroupOffset, 0);
                PartnerSection.Children.Add(PartnerLabel, 1 + GallaryStuffGroupOffset, 0);
            }
            else
            {
                PartnerSection.RowDefinitions.Add(new RowDefinition() { Height = 50 });
                
                PartnerSection.ColumnDefinitions.Add(new ColumnDefinition() { Width = 50 });
                PartnerSection.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });

                PartnerSection.Children.Add(PartnerChangeButton, 0, 0);
                PartnerSection.Children.Add(PartnerLabel, 1, 0);
            }
        }

        private async Task PartnerSelected()
        {
            PartnerLabel.Text = SelectedPartner == null ? "مشتری" :
                !string.IsNullOrEmpty(SelectedPartner.LegalName) ? (SelectedPartner.LegalName + (!string.IsNullOrEmpty(SelectedPartner.Name) ? (" (" + SelectedPartner.Name + ")") : "")) : (SelectedPartner.Name);

            var BeforeSelectedPartnerId = _BeforeSelectedPartner == null ? Guid.NewGuid() : _BeforeSelectedPartner.Id;
            var NewSelectedPartnerId = SelectedPartner == null ? Guid.NewGuid() : SelectedPartner.Id;
            if (SelectedPartner != null && BeforeSelectedPartnerId != NewSelectedPartnerId)
            {
                BusyIndicatorContainder.IsVisible = true;
                await FillFactors(SelectedPartner.Code);
                BusyIndicatorContainder.IsVisible = false;
            }
        }

        private async Task FillFactors(string partnerCode)
        {
            //var getUnsetteledFactorsResult = await Connectivity.GetUnSettledOrdersFromServerAsync(App.Username.Value, App.Password.Value, App.CurrentVersionNumber, partnerCode, "", "");
            //if (getUnsetteledFactorsResult.Success)
            //{
            //    FactorsView.IsVisible = true;
            //    FactorsObservableCollection = new ObservableCollection<UnSettledOrderModel>(getUnsetteledFactorsResult.Data);
            //    FactorsView.ItemsSource = FactorsObservableCollection;
            //}

            var testResult = new List<UnSettledOrderModel>
                        {
                            new UnSettledOrderModel
                            {
                                DriverName = "رضا محمودی",
                                OrderCode = "1001",
                                OrderDate = "",
                                //OrderId = new Guid(),
                                Remainder = 10000.ToString(),
                                Price = 14000.ToString()
                            },
                             new UnSettledOrderModel
                            {
                                DriverName = "علی محمودی",
                                OrderCode = "1002",
                                OrderId = new Guid(),
                                Remainder = 20000.ToString(),
                                Price = 24000.ToString()
                            },
                              new UnSettledOrderModel
                            {
                                DriverName = "آرش سیبسی",
                                OrderCode = "1003",
                                OrderId = new Guid(),
                                Remainder = 30000.ToString(),
                                Price = 34000.ToString()
                            }
                        };

                FactorsView.IsVisible = true;
                FactorsObservableCollection = new ObservableCollection<UnSettledOrderModel>(testResult);
                FactorsView.ItemsSource = FactorsObservableCollection;
        }

        public void RefreshToolbarItems()
        {
            //ToolbarItem_SendToServer_Visible = false;
            //ToolbarItem_Delete_Visible = false;
            //ToolbarItem_Edit_Visible = false;
            //ToolbarItem_Show_Visible = false;
            //ToolbarItem_SearchBar_Visible = false;
            //ToolbarItem_SelectAll_Visible = false;
            //BackButton_Visible = false;

            //if (MultiSelectionMode)
            //{
            //    ToolbarItem_SelectAll_Visible = true;
            //    var AllUnSentOrdersSelected = PartnersList.Where(a => !a.Sent).All(a => a.Selected);
            //    InsertedInformations.ToolbarItem_SelectAll.Icon = AllUnSentOrdersSelected ? "SelectAll_Full.png" : "SelectAll_Empty.png";
            //}
            //else
            //    BackButton_Visible = true;

            //var SelectedCount = PartnersList != null ? PartnersList.Count(a => a.Selected) : 0;
            //if (SelectedCount == 0)
            //{
            //    if (!MultiSelectionMode)
            //        ToolbarItem_SearchBar_Visible = true;
            //}
            //else if (MultiSelectionMode)
            //{
            //    ToolbarItem_SendToServer_Visible = true;
            //    ToolbarItem_Delete_Visible = true;
            //}
            //else
            //{
            //    if (!PartnersList.Single(a => a.Selected).Sent)
            //    {
            //        ToolbarItem_SendToServer_Visible = true;
            //        ToolbarItem_Delete_Visible = true;
            //        ToolbarItem_Edit_Visible = true;
            //    }
            //    ToolbarItem_Show_Visible = true;
            //}

            //InsertedInformations.RefreshToolbarItems(BackButton_Visible, ToolbarItem_SearchBar_Visible, ToolbarItem_Delete_Visible, ToolbarItem_SendToServer_Visible, ToolbarItem_Edit_Visible, ToolbarItem_Show_Visible, ToolbarItem_SelectAll_Visible);
        }
    }
}