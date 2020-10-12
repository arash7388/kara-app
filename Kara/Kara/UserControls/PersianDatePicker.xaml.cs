using Kara.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kara.UserControls
{
    [ContentProperty("ContainerContent")]
    public partial class PersianDatePicker : ContentView
    {
        private DateTime _Value;
        public DateTime Value
        {
            get
            {
                return _Value;
            }
            internal set
            {
                _Value = value;
                PersianDateLabel.Text = value.ToShortStringForDate();
            }
        }

        private PopUpDialogView PopUpDialog;

        public PersianDatePicker(PopUpDialogView PopUpDialog)
        {
            InitializeComponent();

            this.PopUpDialog = PopUpDialog;

            var PersianDateLabelTapGestureRecognizer = new TapGestureRecognizer();
            PersianDateLabelTapGestureRecognizer.Tapped += (sender, e) =>
            {
                var PersianCalendarGUI = new PersianCalendarGUI(Value, PopUpDialog);
                PopUpDialog.DialogContent = PersianCalendarGUI;
                PopUpDialog.DialogClosed += PopUpDialogClosed;
                PopUpDialog.ShowDialog();
                PopUpDialog.DialogShow += async (sender2, e2) =>
                {
                    await Task.Delay(20);
                    PersianCalendarGUI.Initialize();
                };
            };
            PersianDateLabel.GestureRecognizers.Add(PersianDateLabelTapGestureRecognizer);
        }

        public void PopUpDialogClosed (object sender, EventArgs e)
        {
            if(e != null && e.GetType().Equals(typeof(PopupClosedEventArgs)))
            {
                var NewValue = ((PopupClosedEventArgs)e).result;
                if (NewValue != null)
                    Value = ((DateTime)NewValue);
            }
            PopUpDialog.DialogClosed -= PopUpDialogClosed;
        }
    }
}