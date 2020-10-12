using Android.Support.V4.App;
using Kara.Droid.Renderers;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(NavigationPage), typeof(CustomNavigationRenderer))]
namespace Kara.Droid.Renderers
{
    public class CustomNavigationRenderer : Xamarin.Forms.Platform.Android.AppCompat.NavigationPageRenderer
    {
        protected override void SetupPageTransition(FragmentTransaction transaction, bool isPush)
        {
            base.SetupPageTransition(transaction, isPush);
            transaction.SetCustomAnimations(isPush ? Resource.Drawable.abc_slide_in_right : Resource.Drawable.abc_slide_in_left, isPush ? Resource.Drawable.abc_slide_out_left : Resource.Drawable.abc_slide_out_right);
        }
    }

    //public static class Utilities
    //{
    //    private static async Task FakePopFragmentAsync(this FragmentManager FragmentManager)
    //    {
    //        if (FragmentManager.Fragments.Count > 1)
    //        {
    //            FragmentManager.
    //                BeginTransaction().
    //                SetCustomAnimations(Resource.Animation.FadeInSmall, Resource.Animation.SlideDown, Resource.Animation.SlideDown,
    //                Resource.Animation.FadeInSmall).
    //                Show(FragmentManager.Fragments[FragmentManager.Fragments.Count - 2]).
    //                Hide(FragmentManager.Fragments.Last()).Commit();
    //            await Task.Delay(500);
    //        }
    //    }
    //}
}
    