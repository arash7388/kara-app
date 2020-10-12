using Microsoft.AppCenter.Analytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Kara
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MessageForm : ContentPage
    {
        public MessageForm()
        {
            InitializeComponent();
        }

        private async void RetryButton_Clicked(object sender, EventArgs e)
        {
            var loc = await App.CheckGps();
            if (loc != null)
            {
                //Navigation.RemovePage(this);

                if (Navigation.NavigationStack.Any(a => a is MainMenu))
                    await Navigation.PopToRootAsync();
                else
                {
                    string pushedPages = "";
                    foreach(var n in Navigation.NavigationStack)
                    {
                        pushedPages += n.Title + ",";
                    }
                    Analytics.TrackEvent($"RetryButton_Clicked-> Navigation.NavigationStack does not have MainMenu!,pushedPages:{pushedPages}");
                    await Navigation.PushAsync(new MainMenu());
                }

            }
               
            
        }
    }
}