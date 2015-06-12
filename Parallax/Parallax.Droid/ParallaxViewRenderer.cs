using System;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;
using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using Parallax;
using Parallax.Droid;
using Parallax.Droid.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Color = Xamarin.Forms.Color;
using View = Android.Views.View;

[assembly: ExportRenderer(typeof(ParallaxView), typeof(ParallaxViewRenderer))]

namespace Parallax.Droid
{
    public class ParallaxViewRenderer : ViewRenderer<ParallaxView, View>, View.IOnLayoutChangeListener
    {
        private ImageView _imageView;
        private ExtendedScrollView _scrollView;
        private FrameLayout _contentView;
        private FrameLayout _imageFrame;
        private IVisualElementRenderer _renderer;
        private View _nativeView;
        private Xamarin.Forms.View _content;
        private Bitmap _bitmap;

        protected override async void OnElementChanged(ElementChangedEventArgs<ParallaxView> e)
        {
            base.OnElementChanged(e);

            var inflatorservice = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
            var containerView = inflatorservice.Inflate(Resource.Layout.layout, null, false);

            SetNativeControl(containerView);

            _imageView = containerView.FindViewById<ImageView>(Resource.Id.parallax_main_image);
            _scrollView = containerView.FindViewById<ExtendedScrollView>(Resource.Id.parallax_scroll_view);
            _contentView = containerView.FindViewById<FrameLayout>(Resource.Id.parallax_content_view);
            _imageFrame = containerView.FindViewById<FrameLayout>(Resource.Id.parallax_main_image_frame);

            _scrollView.Scrolled += (s, args) =>
            {
                SetParallax();
                SetImageAlpha();
            };

            SetupContent();

            await SetUpImage();
        }

        private void SetupContent()
        {
            if (_renderer != null)
                _renderer.Dispose();

            _content = Element.Content;

            if (_nativeView != null)
                _contentView.RemoveAllViews();

            if (_content == null)
                return;

            _renderer = RendererFactory.GetRenderer(_content);
            _renderer.SetElement(_content);
            _nativeView = _renderer.ViewGroup;
            _contentView.AddView(_nativeView);
            
            var type = Type.GetType("Xamarin.Forms.Platform.Android.Platform,Xamarin.Forms.Platform.Android");
            if (type != null)
            {
                var field = type.GetField("RendererProperty", BindingFlags.Static | BindingFlags.Public);

                if (field != null)
                {
                    _content.SetValue((BindableProperty) field.GetValue(null), _renderer);
                }
            }

            LayoutControls();
        }
        
        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            //LayoutControls();
            base.OnLayout(changed, l, t, r, b);
        }

        private void LayoutControls()
        {
            if (Element.BackgroundColor == Color.Transparent)
                _contentView.SetBackgroundColor(Color.Default.ToAndroid(Color.Default));
            else
                _contentView.SetBackgroundColor(Element.BackgroundColor.ToAndroid(Color.Default));

            var width = Width;
            var rendererSize = _renderer.GetDesiredSize(width, int.MaxValue);
            var size = _content.GetSizeRequest(width, double.PositiveInfinity);
            
            var height = Math.Min(size.Request.Height, rendererSize.Request.Height);
            //height = Context.ToPixels(height);
            var i = Convert.ToInt32(height) + _contentView.PaddingTop;
            _contentView.LayoutParameters.Height =  Convert.ToInt32(Context.ToPixels(i));
            //_contentView.SetMinimumHeight(i);
        }

        private void SetParallax()
        {
            if (Element.ParallaxRate <= 1)
                _imageFrame.TranslationY = 0;
            else
                _imageFrame.TranslationY = -(int)(_scrollView.ScrollY / Element.ParallaxRate);
        }

        private void SetImageAlpha()
        {
            if (Element.Fade)
                _imageView.Alpha = (((float)_contentView.MeasuredHeight - _scrollView.ScrollY) / _contentView.MeasuredHeight);
            else
                _imageView.Alpha = 1;
        }
        
        protected override async void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == "Content")
                SetupContent();

            if (e.PropertyName == ParallaxView.FadeProperty.PropertyName)
                SetImageAlpha();

            if (e.PropertyName == ParallaxView.ParallaxRateProperty.PropertyName)
                SetParallax();

            if (e.PropertyName == ParallaxView.ImageSourceProperty.PropertyName)
                await SetUpImage();
        }

        private async Task SetUpImage()
        {
            if (Element != null && Element.ImageSource != null)
            {
                _bitmap = await GetImageFromImageSourceAsync(Element.ImageSource);
                _imageView.SetImageBitmap(_bitmap);

                _contentView.SetPadding(0, Convert.ToInt32(_bitmap.Height * (Width / Convert.ToDouble(_bitmap.Width))), 0, 0);
                LayoutControls();
            }
        }

        private Task<Bitmap> GetImageFromImageSourceAsync(ImageSource imageSource)
        {
            IImageSourceHandler handler;

            if (imageSource is FileImageSource)
            {
                handler = new FileImageSourceHandler();
            }
            else if (imageSource is StreamImageSource)
            {
                handler = new StreamImagesourceHandler();
            }
            else if (imageSource is UriImageSource)
            {
                handler = new ImageLoaderSourceHandler();
            }
            else
            {
                throw new NotSupportedException();
            }

            return handler.LoadImageAsync(imageSource, Context);
        }

        public void OnLayoutChange(View v, int left, int top, int right, int bottom, int oldLeft, int oldTop, int oldRight, int oldBottom)
        {
        }
    }
}