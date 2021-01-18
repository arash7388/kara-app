using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using Kara.CustomRenderer;
using System.ComponentModel;
using Android.Views;
using Android.Graphics.Drawables;
using Kara.Droid;
using System.Reflection;
using Java.Lang;
using System.Timers;
using Android.Widget;
using Android.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms.Maps.Android;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Content;
using Xamarin.Forms.Maps;
using Kara.Models;
using Kara;
using Kara.Assets;
using Android.Runtime;
using System.IO;

[assembly: ExportRenderer(typeof(FullRoundedLabel), typeof(FullRoundedLabelRenderer))]
[assembly: ExportRenderer(typeof(RightRoundedLabel), typeof(RightRoundedLabelRenderer))]
[assembly: ExportRenderer(typeof(LeftEntryCompanionLabel), typeof(LeftEntryCompanionLabelRenderer))]
[assembly: ExportRenderer(typeof(RightEntryCompanionLabel), typeof(RightEntryCompanionLabelRenderer))]
//[assembly: ExportRenderer(typeof(NewCarouselLayout), typeof(NewCarouselLayoutRenderer))]
[assembly: ExportRenderer(typeof(MyProgressBar), typeof(MyProgressBarRenderer))]
[assembly: ExportRenderer(typeof(GradientContentPage), typeof(GradientContentPageRenderer))]
[assembly: ExportRenderer(typeof(PrintPreviewPage), typeof(PrintPreviewPageRenderer))]
[assembly: ExportRenderer(typeof(PlaceholderEditor), typeof(Kara.Droid.PlaceholderEditorRenderer))]
[assembly: ExportRenderer(typeof(RoundButton), typeof(Kara.Droid.RoundButtonCustomRenderer))]
[assembly: ExportRenderer(typeof(EntryCompanionIcon), typeof(Kara.Droid.EntryCompanionIconCustomRenderer))]
[assembly: ExportRenderer(typeof(MyEntry), typeof(Kara.Droid.MyEntryRenderer))]
[assembly: ExportRenderer(typeof(MyLabel), typeof(Kara.Droid.MyLabelRenderer))]
[assembly: ExportRenderer(typeof(MyPicker), typeof(Kara.Droid.MyPickerRenderer))]
[assembly: ExportRenderer(typeof(CustomMap), typeof(Kara.Droid.CustomMapRenderer))]
[assembly: ExportRenderer(typeof(MyListView), typeof(Kara.Droid.MyListViewRenderer))]
[assembly: ExportRenderer(typeof(MyCheckBox), typeof(Kara.Droid.MyCheckBoxRenderer))]
[assembly: ExportRenderer(typeof(Image), typeof(ImageFixRenderer))]

namespace Kara.Droid
{
    public class MyCheckBoxRenderer : ButtonRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Button> e)
        {
            var control = new CheckBox(this.Context);
            control.Checked = ((MyCheckBox)this.Element).Checked;
            control.CheckedChange += (sender, eArg) => {
                if (((MyCheckBox)this.Element).Checked != control.Checked)
                    ((MyCheckBox)this.Element).Checked = control.Checked;
            };
            this.SetNativeControl(control);

            var element = (MyCheckBox)this.Element;

            if(element.Padding.HasValue)
                this.Control.SetPadding(
                    (int)element.Padding.Value.Left,
                    (int)element.Padding.Value.Top,
                    (int)element.Padding.Value.Right,
                    (int)element.Padding.Value.Bottom
                );

            base.OnElementChanged(e);
        }
        
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Checked")
                if (((CheckBox)this.Control).Checked != ((MyCheckBox)this.Element).Checked)
                    ((CheckBox)this.Control).Checked = ((MyCheckBox)this.Element).Checked;
            base.OnElementPropertyChanged(sender, e);
        }
    }
    
    public class PrintPreviewPageRenderer : PageRenderer
    {   
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == PrintPreviewPage.ScanCommandProperty.PropertyName)
            {
                var printPreviewPage = (PrintPreviewPage)this.Element;
                try
                {
                    var bitmap = getBitmapFromView(this.RootView);
                    printPreviewPage.ScanBitmap = bitmap;
                }
                catch (System.Exception err)
                {
                    printPreviewPage.ScanException = err;
                }
            }
            else if (e.PropertyName == PrintPreviewPage.MergeCommandProperty.PropertyName)
            {
                var printPreviewPage = (PrintPreviewPage)this.Element;
                try
                {
                    printPreviewPage.MergedBitmap = MergeBitmap(printPreviewPage.MergingBitmaps.Select(a => new KeyValuePair<int, Bitmap>(a.Key, (Bitmap)a.Value)).ToArray());
                }
                catch (System.Exception err)
                {
                    printPreviewPage.MergeException = err;
                }
            }
        }

        public Bitmap getBitmapFromView(Android.Views.View v)
        {
            v.RootView.Measure(
                    Android.Views.View.MeasureSpec.MakeMeasureSpec(v.LayoutParameters.Width, Android.Views.MeasureSpecMode.Exactly),
                    Android.Views.View.MeasureSpec.MakeMeasureSpec(v.LayoutParameters.Height, Android.Views.MeasureSpecMode.Exactly));

            //v.Layout(0, 0, v.MeasuredWidth, v.MeasuredHeight);

            var Width = v.Width;
            var Height = v.Height;

            Bitmap b = Bitmap.CreateBitmap(Width, Height, Bitmap.Config.Argb8888);
            Canvas c = new Canvas(b);
            v.Draw(c);

            var top = 0;
            try
            {
                while (b.GetPixel(0, top) != -10063246)
                    top++;
            }
            catch (System.Exception e)
            {
                top = 0;
            }
            
            var bottom = Height - 1;
            try
            {
                while (b.GetPixel(0, bottom) != -10063246)
                    bottom--;
            }
            catch (System.Exception e)
            {
                bottom = b.Height;
            }
            
            var left = 0;
            try
            {
                while (b.GetPixel(left, top) == -10063246)
                    left++;
            }
            catch (System.Exception e)
            {
                left = 0;
            }
            
            var right = Resources.DisplayMetrics.WidthPixels - 1;

            try
            {
                while (b.GetPixel(right, top) == -10063246)
                    right--;
            }
            catch (System.Exception e)
            {
                right = b.Width;
            }

            top = 0;
            bottom = b.Height;
            left = 0;
            right = b.Width;

            var result = Bitmap.CreateBitmap(right - left + 1, bottom - top + 1, b.GetConfig());
            new Canvas(result).DrawBitmap(b, new Rect(left, top, right, bottom), new Rect(0, 0, right - left + 1, bottom - top + 1), null);

            return result;
        }

        public Bitmap MergeBitmap(KeyValuePair<int, Bitmap>[] Bitmaps)
        {
            var PrintPreviewWidth = ((PrintPreviewPage)this.Element).PrintPreviewWidth;
            var PrintPreviewHeight = ((PrintPreviewPage)this.Element).PrintPreviewHeight;
            
            var resultBitmap = Bitmap.CreateBitmap(PrintPreviewWidth, Bitmaps.Length == 1 ? Bitmaps[0].Value.Height : Bitmaps.Last().Key + PrintPreviewHeight, Bitmaps.Last().Value.GetConfig());
            Canvas resultCanvas = new Canvas(resultBitmap);
            
            for (int i = 0; i < Bitmaps.Length; i++)
            {
                var SourceRect = new Rect(0, i < Bitmaps.Length - 1 ? 0 : (Bitmaps.Length - 1) * PrintPreviewHeight - Bitmaps[i].Key, Bitmaps[i].Value.Width, PrintPreviewHeight);
                var DestinationRect = new Rect(0, i * PrintPreviewHeight, PrintPreviewWidth, Bitmaps[i].Key + PrintPreviewHeight);
                resultCanvas.DrawBitmap(Bitmaps[i].Value, SourceRect, DestinationRect, null);
            }
            
            return resultBitmap;
        }
    }

    public class GradientContentPageRenderer : PageRenderer
    {
        protected override void OnVisibilityChanged(Android.Views.View changedView, ViewStates visibility)
        {
            base.OnVisibilityChanged(changedView, visibility);
            SetBackground();
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Page> page)
        {
            base.OnElementChanged(page);
            SetBackground();
        }

        private void SetBackground()
        {
            var startColor = ((GradientContentPage)this.Element).StartColor.ToAndroid();
            var endColor = ((GradientContentPage)this.Element).EndColor.ToAndroid();

            var colors = new int[] { startColor, endColor };

            Background = new GradientDrawable(GradientDrawable.Orientation.TopBottom, colors);
            //Background = this.Resources.GetDrawable(Resource.Drawable.TileBackgroundForPrint);
        }
    }

    public class MyEntryRenderer : Xamarin.Forms.Platform.Android.EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (this.Control == null) return;

            var element = (MyEntry)this.Element;

            this.Control.Background = this.Resources.GetDrawable(
                element.RightRounded && element.LeftRounded ? Resource.Drawable.MyEntryEditTextFullRounded :
                element.RightRounded && !element.LeftRounded ? Resource.Drawable.MyEntryEditTextRightRounded :
                !element.RightRounded && element.LeftRounded ? Resource.Drawable.MyEntryEditTextLeftRounded :
                Resource.Drawable.MyEntryEditTextNotRounded
            );
            this.Control.SetPadding(
                element.Padding.HasValue ? (int)element.Padding.Value.Left : 20,
                element.Padding.HasValue ? (int)element.Padding.Value.Top : 10,
                element.Padding.HasValue ? (int)element.Padding.Value.Right : 10,
                element.Padding.HasValue ? (int)element.Padding.Value.Bottom : 10
            );
        }
    }

    public class RightRoundedLabelRenderer : Xamarin.Forms.Platform.Android.LabelRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

            if (this.Control == null) return;

            this.Control.Background = this.Resources.GetDrawable(Resource.Drawable.RightRoundedLabel);
            this.Control.SetPadding(10, 10, 10, 10);

            var element = (RightRoundedLabel)this.Element;

            this.Control.SetPadding(
                element.Padding.HasValue ? (int)element.Padding.Value.Left : 20,
                element.Padding.HasValue ? (int)element.Padding.Value.Top : 10,
                element.Padding.HasValue ? (int)element.Padding.Value.Right : 10,
                element.Padding.HasValue ? (int)element.Padding.Value.Bottom : 10
            );
        }
    }

    public class FullRoundedLabelRenderer : Xamarin.Forms.Platform.Android.LabelRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

            if (this.Control == null) return;

            this.Control.Background = this.Resources.GetDrawable(Resource.Drawable.FullRoundedLable);
            this.Control.SetPadding(10, 10, 10, 10);
        }
    }

    public class MyLabelRenderer : LabelRenderer
    {
        static Drawable bg;
        static Drawable _GallaryBG, _GallaryTopBG, _GallaryBottomBG;
        private Drawable GetDrawable(string type)
        {
            switch (type)
            {
                case "GallaryBG":
                    if (_GallaryBG == null)
                        _GallaryBG = this.Resources.GetDrawable(Resource.Drawable.GallaryBG);
                    return _GallaryBG;
                    break;
                case "GallaryTopBG":
                    if (_GallaryTopBG == null)
                        _GallaryTopBG = this.Resources.GetDrawable(Resource.Drawable.GallaryTopBG);
                    return _GallaryTopBG;
                    break;
                case "GallaryBottomBG":
                    if (_GallaryBottomBG == null)
                        _GallaryBottomBG = this.Resources.GetDrawable(Resource.Drawable.GallaryBottomBG);
                    return _GallaryBottomBG;
                    break;
                default:
                    break;
            }
            return null;
        }

        bool BGAssigned = false;
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

            if (this.Control == null) return;

            var myLabel = (MyLabel)this.Element;

            if(!BGAssigned)
            {
                if (myLabel.IsGallaryBG)
                    this.Control.Background = GetDrawable("GallaryBG");
                if (myLabel.IsGallaryTopBG)
                    this.Control.Background = GetDrawable("GallaryTopBG");
                if (myLabel.IsGallaryBottomBG)
                    this.Control.Background = GetDrawable("GallaryBottomBG");
                BGAssigned = true;
            }
            
            this.Control.SetPadding((int)myLabel.Padding.Left, (int)myLabel.Padding.Top, (int)myLabel.Padding.Right, (int)myLabel.Padding.Bottom);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (this.Control == null) return;

            var myLabel = (MyLabel)this.Element;
            
            if(!myLabel.IsPrintGrayBackground && !myLabel.IsGallaryBG && !myLabel.IsGallaryTopBG && !myLabel.IsGallaryBottomBG && (e.PropertyName == Label.WidthProperty.PropertyName || e.PropertyName == Label.HeightProperty.PropertyName))
            {
                if (myLabel.Width != 0 && myLabel.Height != 0)
                {
                    this.Control.Background = new MyLabelBackgroundDrawable(Android.Graphics.Color.Argb((int)(myLabel.MyBackgroundColor.A * 255), (int)(myLabel.MyBackgroundColor.R * 255), (int)(myLabel.MyBackgroundColor.G * 255), (int)(myLabel.MyBackgroundColor.B * 255)), myLabel.Width * 2, myLabel.Height * 2, myLabel.BorderRadius) { };
                }
            }
        }
    }

    public class MyLabelBackgroundDrawable : Drawable
    {
        Android.Graphics.Color c;
        double Width, Height;
        int[] BorderRadius;
        public MyLabelBackgroundDrawable(Android.Graphics.Color c, double Width, double Height, int[] BorderRadius)
        {
            this.c = c;
            this.Width = Width;
            this.Height = Height;
            this.BorderRadius = BorderRadius;
        }

        public override int Opacity
        {
            get
            {
                return 1;
            }
        }

        public override void Draw(Canvas canvas)
        {
            var Paint = new Paint();
            Paint.Color = c;
            RectF r = new RectF(0, 0, (float)Width, (float)Height);
            canvas.DrawRoundRect(r, BorderRadius[0], BorderRadius[1], Paint);
        }

        public override void SetAlpha(int alpha)
        {

        }

        public override void SetColorFilter(ColorFilter colorFilter)
        {

        }
    }
    
    public class LeftEntryCompanionLabelRenderer : LabelRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);
            if (this.Control == null) return;

            var leftEntryCompanionLabel = (LeftEntryCompanionLabel)this.Element;
            if (leftEntryCompanionLabel.Disabled)
            {
                this.Control.Background = this.Resources.GetDrawable(Resource.Drawable.LeftEntryCompanionLableDisabled);
                this.Control.SetTextColor(Android.Graphics.Color.Gray);
            }
            else
            {
                this.Control.Background = this.Resources.GetDrawable(Resource.Drawable.LeftEntryCompanionLable);
                this.Control.SetTextColor(Android.Graphics.Color.Black);
            }
            
            this.Control.SetPadding(10, 10, 10, 10);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (this.Control == null) return;

            var leftEntryCompanionLabel = (LeftEntryCompanionLabel)this.Element;
            if (e.PropertyName == LeftEntryCompanionLabel.DisabledProperty.PropertyName)
            {
                if (leftEntryCompanionLabel.Disabled)
                {
                    this.Control.Background = this.Resources.GetDrawable(Resource.Drawable.LeftEntryCompanionLableDisabled);
                    this.Control.SetTextColor(Android.Graphics.Color.Gray);
                }
                else
                {
                    this.Control.Background = this.Resources.GetDrawable(Resource.Drawable.LeftEntryCompanionLable);
                    this.Control.SetTextColor(Android.Graphics.Color.Black);
                }
            }
        }
    }

    public class RightEntryCompanionLabelRenderer : LabelRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);
            if (this.Control == null) return;
            
            var rightEntryCompanionLabel = (RightEntryCompanionLabel)this.Element;
            if (rightEntryCompanionLabel.Disabled)
            {
                this.Control.Background = this.Resources.GetDrawable(Resource.Drawable.RightEntryCompanionLableDisabled);
                this.Control.SetTextColor(Android.Graphics.Color.Gray);
            }
            else
            {
                this.Control.Background = this.Resources.GetDrawable(Resource.Drawable.RightEntryCompanionLable);
                this.Control.SetTextColor(Android.Graphics.Color.Black);
            }

            this.Control.SetPadding(10, 10, 10, 10);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (this.Control == null) return;

            var rightEntryCompanionLabel = (RightEntryCompanionLabel)this.Element;
            if (e.PropertyName == RightEntryCompanionLabel.DisabledProperty.PropertyName)
            {
                if (rightEntryCompanionLabel.Disabled)
                {
                    this.Control.Background = this.Resources.GetDrawable(Resource.Drawable.RightEntryCompanionLableDisabled);
                    this.Control.SetTextColor(Android.Graphics.Color.Gray);
                }
                else
                {
                    this.Control.Background = this.Resources.GetDrawable(Resource.Drawable.RightEntryCompanionLable);
                    this.Control.SetTextColor(Android.Graphics.Color.Black);
                }
            }
        }
    }

    public class EntryCompanionIconCustomRenderer : Xamarin.Forms.Platform.Android.ImageRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Image> e)
        {
            base.OnElementChanged(e);
            if (this.Control == null) return;

            this.Control.Background = this.Resources.GetDrawable(Resource.Drawable.EntryCompanionIcon);
            this.Control.SetPadding(12, 2, 12, 2);
        }
    }

    public class RoundButtonCustomRenderer : Xamarin.Forms.Platform.Android.ButtonRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Button> e)
        {
            base.OnElementChanged(e);
            if (this.Control == null) return;

            this.Control.Background = this.Resources.GetDrawable(Resource.Drawable.RoundButton);
            this.Control.SetPadding(10, -5, 10, -5);
            this.Control.SetTextColor(Android.Graphics.Color.White);
            this.Control.SetIncludeFontPadding(false);
        }
    }

    public class PlaceholderEditorRenderer : EditorRenderer
    {
        public PlaceholderEditorRenderer() { }

        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                var element = e.NewElement as PlaceholderEditor;

                this.Control.Background = Resources.GetDrawable(Resource.Drawable.placeholderEditorStyle);

                this.Control.Hint = element.Placeholder;

                this.Control.SetPadding(
                    element.Padding.HasValue ? (int)element.Padding.Value.Left : 20,
                    element.Padding.HasValue ? (int)element.Padding.Value.Top : 10,
                    element.Padding.HasValue ? (int)element.Padding.Value.Right : 10,
                    element.Padding.HasValue ? (int)element.Padding.Value.Bottom : 10
                );
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == PlaceholderEditor.PlaceholderProperty.PropertyName)
            {
                var element = this.Element as PlaceholderEditor;
                this.Control.Hint = element.Placeholder;
            }
        }
    }

    public class MyPickerRenderer : PickerRenderer
    {
        public MyPickerRenderer() { }

        protected override void OnElementChanged(ElementChangedEventArgs<Picker> e)
        {
            base.OnElementChanged(e);
            if (this.Control == null) return;

            this.Control.Background = this.Resources.GetDrawable(Resource.Drawable.MyPicker);
            this.Control.SetTextColor(Android.Graphics.Color.White);
            this.Control.SetHintTextColor(Android.Graphics.Color.WhiteSmoke);
        }
    }

    //public class NewCarouselLayoutRenderer : ScrollViewRenderer
    //{
    //    HorizontalScrollView _scrollView;
    //    int _prevScrollX;
    //    int _deltaX;
    //    bool _motionDown;
    //    Timer _deltaXResetTimer;
    //    Timer _scrollStopTimer;

    //    protected override void OnElementChanged(VisualElementChangedEventArgs e)
    //    {
    //        base.OnElementChanged(e);
    //        if (e.NewElement == null) return;

    //        e.NewElement.PropertyChanged += ElementPropertyChanged;

    //        _deltaXResetTimer = new Timer(100) { AutoReset = false };
    //        _deltaXResetTimer.Elapsed += (object sender, ElapsedEventArgs args) => _deltaX = 0;

    //        _scrollStopTimer = new Timer(200) { AutoReset = false };
    //        _scrollStopTimer.Elapsed += (object sender, ElapsedEventArgs args2) => UpdateSelectedIndex();
    //    }

    //    void ElementPropertyChanged(object sender, PropertyChangedEventArgs e)
    //    {
    //        if (e.PropertyName == "Renderer")
    //        {
    //            _scrollView = (HorizontalScrollView)typeof(ScrollViewRenderer)
    //                .GetField("_hScrollView", BindingFlags.NonPublic | BindingFlags.Instance)
    //                .GetValue(this);

    //            _scrollView.HorizontalScrollBarEnabled = false;
    //            _scrollView.Touch += HScrollViewTouch;
    //        }
    //        if (e.PropertyName == NewCarouselLayout.SelectedIndexProperty.PropertyName && !_motionDown)
    //        {
    //            ScrollToIndex(((NewCarouselLayout)this.Element).SelectedIndex);
    //        }
    //    }

    //    void HScrollViewTouch(object sender, TouchEventArgs e)
    //    {
    //        e.Handled = false;

    //        switch (e.Event.Action)
    //        {
    //            case MotionEventActions.Move:
    //                _deltaXResetTimer.Stop();
    //                _deltaX = _scrollView.ScrollX - _prevScrollX;
    //                _prevScrollX = _scrollView.ScrollX;

    //                UpdateSelectedIndex();

    //                _deltaXResetTimer.Start();
    //                break;
    //            case MotionEventActions.Down:
    //                _motionDown = true;
    //                _scrollStopTimer.Stop();
    //                break;
    //            case MotionEventActions.Up:
    //                _motionDown = false;
    //                SnapScroll();
    //                _scrollStopTimer.Start();
    //                break;
    //        }
    //    }

    //    void UpdateSelectedIndex()
    //    {
    //        var center = _scrollView.ScrollX + (_scrollView.Width / 2);
    //        var mycarouselLayout = (NewCarouselLayout)this.Element;
    //        mycarouselLayout.SelectedIndex = (center / _scrollView.Width);
    //    }

    //    void SnapScroll()
    //    {
    //        var roughIndex = (float)_scrollView.ScrollX / _scrollView.Width;

    //        var targetIndex =
    //            _deltaX < 0 ? Java.Lang.Math.Floor(roughIndex)
    //            : _deltaX > 0 ? Java.Lang.Math.Ceil(roughIndex)
    //            : Java.Lang.Math.Round(roughIndex);

    //        ScrollToIndex((int)targetIndex);
    //    }

    //    void ScrollToIndex(int targetIndex)
    //    {
    //        var targetX = targetIndex * _scrollView.Width;
    //        _scrollView.Post(new Runnable(() =>
    //        {
    //            _scrollView.SmoothScrollTo(targetX, 0);
    //        }));
    //    }
    //}

    public class MyProgressBarRenderer : ProgressBarRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.ProgressBar> e)
        {
            base.OnElementChanged(e);

            var color = ((MyProgressBar)this.Element).Color;
            Control.ProgressDrawable.SetColorFilter(color.ToAndroid(), Android.Graphics.PorterDuff.Mode.SrcIn);
            Control.ProgressTintList = Android.Content.Res.ColorStateList.ValueOf(color.ToAndroid());
        }
    }
    
    public class ImageFixRenderer : ImageRenderer
    {
        public ImageFixRenderer()
        {
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                base.Dispose(disposing);
            }
            catch (Java.Lang.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }
    }

    public class CustomMapRenderer : MapRenderer, GoogleMap.IInfoWindowAdapter, IOnMapReadyCallback
    {
        GoogleMap map;
        bool isDrawn;
        public static Dictionary<MapIcon, BitmapDescriptor> MapIcons;
        CustomMap formsMap;

        protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Map> e)
        {
            try
            {
                base.OnElementChanged(e);

                if (e.OldElement != null)
                {
                    map.InfoWindowClick -= OnInfoWindowClick;
                }

                if (e.NewElement != null)
                {
                    formsMap = (CustomMap)e.NewElement;
                    ((MapView)Control).GetMapAsync(this);
                }
            }
            catch (System.Exception)
            {
            }
        }
        
        public void OnMapReady(GoogleMap googleMap)
        {
            map = googleMap;
            map.InfoWindowClick += OnInfoWindowClick;
            map.SetInfoWindowAdapter(this);
            map.MarkerClick += Map_MarkerClick;
            map.MapClick += Map_MapClick;

            InvokeOnMapReadyBaseClassHack(googleMap);
        }

        private void InvokeOnMapReadyBaseClassHack(GoogleMap googleMap)
        {
            System.Reflection.MethodInfo onMapReadyMethodInfo = null;

            Type baseType = typeof(MapRenderer);
            foreach (var currentMethod in baseType.GetMethods(System.Reflection.BindingFlags.NonPublic |
                                                             System.Reflection.BindingFlags.Instance |
                                                              System.Reflection.BindingFlags.DeclaredOnly))
            {

                if (currentMethod.IsFinal && currentMethod.IsPrivate)
                {
                    if (string.Equals(currentMethod.Name, "OnMapReady", StringComparison.Ordinal))
                    {
                        onMapReadyMethodInfo = currentMethod;

                        break;
                    }

                    if (currentMethod.Name.EndsWith(".OnMapReady", StringComparison.Ordinal))
                    {
                        onMapReadyMethodInfo = currentMethod;

                        break;
                    }
                }
            }

            if (onMapReadyMethodInfo != null)
            {
                try
                {
                    onMapReadyMethodInfo.Invoke(this, new[] { googleMap });
                }
                catch (Java.Lang.Exception)
                {
                }
                catch (System.Exception)
                {
                }
            }
        }

        private void Map_MapClick(object sender, GoogleMap.MapClickEventArgs e)
        {
            if(formsMap.ShowingPin != null && ((DBRepository.PartnerListModel)formsMap.ShowingPin.Data).Selected)
            {
                ((Marker)formsMap.ShowingPin.Marker).HideInfoWindow();
                ((DBRepository.PartnerListModel)formsMap.ShowingPin.Data).Selected = false;
                formsMap.NotifyChangeShowingPin();
                formsMap.OnShowingPinChanged(null);
            }
        }

        private void Map_MarkerClick(object sender, GoogleMap.MarkerClickEventArgs e)
        {
            var CustomPin = GetCustomPin(e.Marker);
            if (!((DBRepository.PartnerListModel)CustomPin.Data).Selected)
            {
                if (formsMap.ShowingPin != null && ((DBRepository.PartnerListModel)formsMap.ShowingPin.Data).Selected)
                    ((DBRepository.PartnerListModel)formsMap.ShowingPin.Data).Selected = false;
                e.Marker.ShowInfoWindow();
            }
            else
                e.Marker.HideInfoWindow();
            ((DBRepository.PartnerListModel)CustomPin.Data).Selected = !((DBRepository.PartnerListModel)CustomPin.Data).Selected;
            
            formsMap.ShowingPin = CustomPin;
            formsMap.NotifyChangeShowingPin();
            formsMap.OnShowingPinChanged(null);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            
            if (e.PropertyName.Equals("CustomPins"))
            {
                if (map != null)
                {
                    map.Clear();

                    foreach (var pin in formsMap.CustomPins)
                    {
                        var markerOptions = new MarkerOptions();
                        markerOptions.SetPosition(new LatLng(pin.Pin.Position.Latitude, pin.Pin.Position.Longitude));
                        markerOptions.SetIcon(MapIcons[pin.Icon]);
                        pin.Marker = map.AddMarker(markerOptions);
                        if (((DBRepository.PartnerListModel)pin.Data).Selected)
                        {
                            //((Marker)pin.Marker).ShowInfoWindow();
                            formsMap.ShowingPin = pin;
                            formsMap.NotifyChangeShowingPin();
                            formsMap.OnShowingPinChanged(null);
                        }
                    }
                    isDrawn = true;
                }
            }
            else if(e.PropertyName.Equals("ShowingPin"))
            {
                var ShowingPin = formsMap.ShowingPin;
                if (ShowingPin != null)
                {
                    if (ShowingPin.Marker != null)
                    {
                        if (((DBRepository.PartnerListModel)ShowingPin.Data).Selected)
                        {
                            ((Marker)ShowingPin.Marker).ShowInfoWindow();
                        }
                        else
                        {
                            ((Marker)ShowingPin.Marker).HideInfoWindow();
                        }
                    }
                }
            }
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            try
            {
                base.OnLayout(changed, l, t, r, b);

                if (changed)
                {
                    isDrawn = false;
                }
            }
            catch (System.Exception)
            {
            }
        }

        void OnInfoWindowClick(object sender, GoogleMap.InfoWindowClickEventArgs e)
        {
            //var customPin = GetCustomPin(e.Marker);
            //if (customPin == null)
            //    throw new System.Exception("Custom pin not found");
        }

        public Android.Views.View GetInfoContents(Marker marker)
        {
            var inflater = Android.App.Application.Context.GetSystemService(Context.LayoutInflaterService) as Android.Views.LayoutInflater;
            if (inflater != null)
            {
                Android.Views.View view;

                var customPin = GetCustomPin(marker);
                if (customPin == null)
                {
                    throw new System.Exception("Custom pin not found");
                }
                
                view = inflater.Inflate(Resource.Layout.MapInfoWindow, null);

                var CustomerName = view.FindViewById<TextView>(Resource.Id.CustomerName);
                var CustomerCode = view.FindViewById<TextView>(Resource.Id.CustomerCode);
                var CustomerGroup = view.FindViewById<TextView>(Resource.Id.CustomerGroup);
                var CustomerZone = view.FindViewById<TextView>(Resource.Id.CustomerZone);
                var CustomerAddress = view.FindViewById<TextView>(Resource.Id.CustomerAddress);
                var CustomerPhone = view.FindViewById<TextView>(Resource.Id.CustomerPhone);
                var DistanceFromMe = view.FindViewById<TextView>(Resource.Id.DistanceFromMe);

                var Partner = (DBRepository.PartnerListModel)customPin.Data;
                
                if (CustomerName != null)
                {
                    CustomerName.Text = (!string.IsNullOrWhiteSpace(Partner.PartnerData.Name) ? (Partner.PartnerData.Name + (!string.IsNullOrWhiteSpace(Partner.PartnerData.LegalName) ? (" (" + Partner.PartnerData.LegalName + ")") : "")) : Partner.PartnerData.LegalName).ToPersianDigits();
                    CustomerName.Typeface = MainActivity.IranSansFont;
                }
                if (CustomerCode != null)
                {
                    CustomerCode.Text = "کد: " + (Partner.PartnerData.Code).ToPersianDigits();
                    CustomerCode.Typeface = MainActivity.IranSansFont;
                }
                if (CustomerGroup != null)
                {
                    CustomerGroup.Text = "گروه: " + (Partner.PartnerData.Groups.Any() ? Partner.PartnerData.Groups.Select(a => a.Name).Aggregate((sum, x) => sum + "، " + x) : "---").ToPersianDigits();
                    CustomerGroup.Typeface = MainActivity.IranSansFont;
                }
                if (CustomerZone != null)
                {
                    CustomerZone.Text = "منطقه/مسیر: ---";// + (Partner.PartnerData.ZoneName).ReplaceLatinDigits();
                    CustomerZone.Typeface = MainActivity.IranSansFont;
                }
                if (CustomerAddress != null)
                {
                    CustomerAddress.Text = "آدرس: " + (Partner.PartnerData.Address).ToPersianDigits();
                    CustomerAddress.Typeface = MainActivity.IranSansFont;
                }
                if (CustomerPhone != null)
                {
                    var PartnerPhones = new string[] { Partner.PartnerData.Phone1, Partner.PartnerData.Phone2, Partner.PartnerData.Mobile }.Where(a => !string.IsNullOrWhiteSpace(a));
                    CustomerPhone.Text = "تلفن ها: " + (PartnerPhones.Any() ? PartnerPhones.Aggregate((sum, x) => sum.Replace(" و ", "، ") + " و " + x) : "---").ToPersianDigits();
                    CustomerPhone.Typeface = MainActivity.IranSansFont;
                }
                if (DistanceFromMe != null)
                {
                    var PartnerPhones = new string[] { Partner.PartnerData.Phone1, Partner.PartnerData.Phone2, Partner.PartnerData.Mobile }.Where(a => !string.IsNullOrWhiteSpace(a));
                    DistanceFromMe.Text = "فاصله از من: " + (!Partner.DistanceFromMe.HasValue ? "---" : (Partner.DistanceFromMe.Value < 1000 ? (Partner.DistanceFromMe.Value.ToString() + " متر") : ((Partner.DistanceFromMe.Value / 1000.0).ToString("##0.#") + " کیلومتر"))).ToPersianDigits();
                    DistanceFromMe.Typeface = MainActivity.IranSansFont;
                }

                return view;
            }
            return null;
        }

        public Android.Views.View GetInfoWindow(Marker marker)
        {
            return null;
        }

        CustomPin GetCustomPin(Marker annotation)
        {
            var position = new Position(annotation.Position.Latitude, annotation.Position.Longitude);
            foreach (var pin in formsMap.CustomPins)
            {
                if (pin.Pin.Position == position)
                {
                    return pin;
                }
            }
            return null;
        }
    }

    public class MyListViewRenderer : Xamarin.Forms.Platform.Android.ListViewRenderer
    {
        public MyListViewRenderer()
        { }

        public MyListViewRenderer(IntPtr javaReference, JniHandleOwnership transfer)
        { }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.ListView> e)
        {
            base.OnElementChanged(e);
            if (this.Control == null) return;

            LongClickable = true;
            this.Control.ItemLongClick += (sender, eArg) => {
                ((MyListView)this.Element).LongClick(eArg.Position);
            };
        }
    }

}