using Kara.CustomRenderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace Kara
{
    public partial class SettingsForm_GallaryStuffCount : GradientContentPage
    {
        Button[][] Buttons;
        Color UnselectedColor, SelectedColor;
        SettingsForm SettingsForm;

        public SettingsForm_GallaryStuffCount(SettingsForm SettingsForm)
        {
            InitializeComponent();

            this.SettingsForm = SettingsForm;

            UnselectedColor = Color.Transparent;
            SelectedColor = Color.FromHex("82B7ED");
            
            RefreshCellsGrid();
            
            RefreshGallaryStuffCountPresentation();
        }

        private void RefreshGallaryStuffCountPresentation()
        {
            var IsHorizontal = width > height;
            for (int i = 0; i < (IsHorizontal ? 3 : 4); i++)
                for (int j = 0; j < (IsHorizontal ? 4 : 3); j++)
                    Buttons[i][j].BackgroundColor = i < (IsHorizontal ? App.GallaryStuffCount[0] : App.GallaryStuffCount[1]) && j < (IsHorizontal ? App.GallaryStuffCount[1] : App.GallaryStuffCount[0]) ? SelectedColor : UnselectedColor;
        }

        double width, height;
        protected override async void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            if (this.width != width && this.height != height)
            {
                this.width = width;
                this.height = height;

                RefreshCellsGrid();
                RefreshGallaryStuffCountPresentation();
            }
        }

        private void RefreshCellsGrid()
        {
            var IsHorizontal = width > height;
            CellsGrid.RowDefinitions.Clear();
            CellsGrid.ColumnDefinitions.Clear();
            if (Buttons != null)
            {
                foreach (var row in Buttons)
                    foreach (var cell in row)
                        CellsGrid.Children.Remove(cell);
            }
            for (int i = 0; i < (IsHorizontal ? 3 : 4); i++)
                CellsGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Star });
            for (int i = 0; i < (IsHorizontal ? 4 : 3); i++)
                CellsGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Star });
            
            Buttons = new Button[IsHorizontal ? 3 : 4][];
            for (int i = 0; i < (IsHorizontal ? 3 : 4); i++)
            {
                Buttons[i] = new Button[IsHorizontal ? 4 : 3];
                for (int j = 0; j < (IsHorizontal ? 4 : 3); j++)
                {
                    Buttons[i][j] = new Button() { Text = "", HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand, BackgroundColor = UnselectedColor, ClassId = i + "_" + j, Margin = 0, BorderRadius = 10, BorderColor = Color.Red, BorderWidth = 1 };
                    Buttons[i][j].Clicked += SettingsForm_GallaryStuffCount_Clicked;
                    CellsGrid.Children.Add(Buttons[i][j], (IsHorizontal ? 3 : 2) - j, i);
                }
            }
        }

        private void SettingsForm_GallaryStuffCount_Clicked(object sender, EventArgs e)
        {
            var Class = ((Button)sender).ClassId;
            var IsHorizontal = width > height;
            var i = Convert.ToInt32(Class.Split('_')[0]) + 1;
            var j = Convert.ToInt32(Class.Split('_')[1]) + 1;

            App.GallaryStuffCount = new int[] {
                IsHorizontal ? i : j,
                IsHorizontal ? j : i
            };
            SettingsForm.RefreshGallryStuffCount();
            RefreshGallaryStuffCountPresentation();
        }
    }
}
