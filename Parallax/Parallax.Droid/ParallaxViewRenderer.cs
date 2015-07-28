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
    public sealed class ParallaxViewRenderer : View, IVisualElementRenderer
    {
        private ImageView _imageView;
        private LinearLayout _contentView;
        private IVisualElementRenderer _renderer;

        public ParallaxViewRenderer() : this(Forms.Context)
        {
        }

        public ParallaxViewRenderer(Context context) : base(context)
        {
            Create();
        }

        void Create()
        {
            var inflatorservice = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
            var containerView = (FrameLayout)inflatorservice.Inflate(Resource.Layout.layout, null, false);

            ViewGroup = containerView;

            _imageView = containerView.FindViewById<ImageView>(Resource.Id.parallax_main_image);
            _contentView = containerView.FindViewById<LinearLayout>(Resource.Id.parallax_content_view);
        }

        private bool _init;
        private ViewGroup _viewGroup;

        public event EventHandler<VisualElementChangedEventArgs> ElementChanged;
            
        public void SetElement(VisualElement element)
        {
            var oldElement = this.Element; 
            if (oldElement != null) 
                oldElement.PropertyChanged -= HandlePropertyChanged; 

            Element = element; 

            if (Element != null)
            {
                UpdateContent();
                Element.PropertyChanged += HandlePropertyChanged;
            }

            if (!_init) 
            { 
                _init = true; 
                Tracker = new VisualElementTracker (this); 
            }

            if(ElementChanged != null) 
                ElementChanged (this, new VisualElementChangedEventArgs (oldElement, this.Element));
            
            Tracker.UpdateLayout ();
        }

        void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Content")
                UpdateContent();
        }

        void UpdateContent()
        {
            if (_viewGroup != null) 
                _contentView.RemoveView (_viewGroup); 

            var parallaxView = (ParallaxView)Element;

            _renderer = RendererFactory.GetRenderer (parallaxView.Content); 

            var type = Type.GetType("Xamarin.Forms.Platform.Android.Platform,Xamarin.Forms.Platform.Android");
            if (type != null)
            {
                var field = type.GetField("RendererProperty", BindingFlags.Static | BindingFlags.Public);

                if (field != null)
                    parallaxView.Content.SetValue((BindableProperty)field.GetValue(null), _renderer);
            }

            _viewGroup = _renderer.ViewGroup;

            _contentView.AddView (_viewGroup);
            SetTextViewLayout(_viewGroup); 
        }

        void SetTextViewLayout(ViewGroup view)
        {
            for (var i = 0; i < view.ChildCount; ++i)
            {
                var child = view.GetChildAt(i);
                var textView = child as TextView;
                if (textView != null)
                {
                    textView.LayoutParameters.Height = ViewGroup.LayoutParams.WrapContent;
                }
        
                var viewGroup = child as ViewGroup;
                if (viewGroup != null)
                    SetTextViewLayout(viewGroup);
            }
        }

        public SizeRequest GetDesiredSize(int widthConstraint, int heightConstraint)
        {
            Measure (widthConstraint, heightConstraint);
            return new SizeRequest (new Size (MeasuredWidth, MeasuredHeight)); 
        }

        public void UpdateLayout()
        {
            if (Tracker != null)
                Tracker.UpdateLayout ();
        }

        public VisualElementTracker Tracker
        {
            get;
            private set;
        }

        public ViewGroup ViewGroup
        {
            get;
            private set;
        }

        public VisualElement Element
        {
            get;
            private set;
        }


//        private ImageView _imageView;
//        private ExtendedScrollView _scrollView;
//        private LinearLayout _contentView;
//        private FrameLayout _imageFrame;
//        private IVisualElementRenderer _renderer;
//        private ViewGroup _nativeView;
//        private Xamarin.Forms.View _content;
//        private Bitmap _bitmap;
//
//        protected override async void OnElementChanged(ElementChangedEventArgs<ParallaxView> e)
//        {
//            base.OnElementChanged(e);
//
//            var inflatorservice = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
//            var containerView = (LinearLayout)inflatorservice.Inflate(Resource.Layout.layout, null, false);
//
//            SetNativeControl(containerView);
//
//            _imageView = containerView.FindViewById<ImageView>(Resource.Id.parallax_main_image);
//            _scrollView = containerView.FindViewById<ExtendedScrollView>(Resource.Id.parallax_scroll_view);
//            _contentView = containerView.FindViewById<LinearLayout>(Resource.Id.parallax_content_view);
//            _imageFrame = containerView.FindViewById<FrameLayout>(Resource.Id.parallax_main_image_frame);
//
//            _scrollView.Scrolled += (s, args) =>
//            {
//                SetParallax();
//                SetImageAlpha();
//            };
//            
//            _imageView.LayoutChange += (s, e1) => _contentView.SetPadding(0, _imageView.MeasuredHeight, 0, 0);
//
//            SetupContent();
//
//            await SetUpImage();
//        }
//
//        private void SetupContent()
//        {
//            if (_renderer != null)
//                _renderer.Dispose();
//
//            _content = Element.Content;
//
//            if (_nativeView != null)
//                _contentView.RemoveAllViews();
//
//            if (_content == null)
//                return;
//            
//            _renderer = RendererFactory.GetRenderer(_content);
//            _renderer.SetElement(_content);
//            
//            var type = Type.GetType("Xamarin.Forms.Platform.Android.Platform,Xamarin.Forms.Platform.Android");
//            if (type != null)
//            {
//                var field = type.GetField("RendererProperty", BindingFlags.Static | BindingFlags.Public);
//
//                if (field != null)
//                {
//                    _content.SetValue((BindableProperty) field.GetValue(null), _renderer);
//                }
//            }
//
//            _nativeView = _renderer.ViewGroup;
//
//            _contentView.AddView(_nativeView, LayoutParams.MatchParent, LayoutParams.WrapContent);
//
//            SetTextViewLayout(_nativeView);
//
//            if (Element.BackgroundColor == Color.Transparent)
//                _contentView.SetBackgroundColor(Color.Default.ToAndroid(Color.Default));
//            else
//                _contentView.SetBackgroundColor(Element.BackgroundColor.ToAndroid(Color.Default));
//        }
//
//        void SetTextViewLayout(ViewGroup view)
//        {
//            for (var i = 0; i < view.ChildCount; ++i)
//            {
//                var child = view.GetChildAt(i);
//                var textView = child as TextView;
//                if (textView != null)
//                {
//                    textView.LayoutParameters.Height = LayoutParams.WrapContent;
//                    textView.RequestLayout();
//                }
//
//                var viewGroup = child as ViewGroup;
//                if (viewGroup != null)
//                    SetTextViewLayout(viewGroup);
//            }
//        }
//
//        private void SetParallax()
//        {
//            if (Element.ParallaxRate <= 1)
//                _imageFrame.TranslationY = 0;
//            else
//                _imageFrame.TranslationY = -(int)(_scrollView.ScrollY / Element.ParallaxRate);
//        }
//
//        private void SetImageAlpha()
//        {
//            if (Element.Fade)
//                _imageView.Alpha = (((float)_contentView.MeasuredHeight - _scrollView.ScrollY) / _contentView.MeasuredHeight);
//            else
//                _imageView.Alpha = 1;
//        }
//        
//        protected override async void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
//        {
//            base.OnElementPropertyChanged(sender, e);
//
//            if (e.PropertyName == "Content")
//            {
//                SetupContent();
//                RequestLayout();
//            }
//
//            if (e.PropertyName == ParallaxView.FadeProperty.PropertyName)
//                SetImageAlpha();
//
//            if (e.PropertyName == ParallaxView.ParallaxRateProperty.PropertyName)
//                SetParallax();
//
//            if (e.PropertyName == ParallaxView.ImageSourceProperty.PropertyName)
//                await SetUpImage();
//        }
//
//        private async Task SetUpImage()
//        {
//            if (Element != null && Element.ImageSource != null)
//            {
//                _bitmap = await GetImageFromImageSourceAsync(Element.ImageSource);
//                _imageView.SetImageBitmap(_bitmap);
//            }
//        }
//
//        private Task<Bitmap> GetImageFromImageSourceAsync(ImageSource imageSource)
//        {
//            IImageSourceHandler handler;
//
//            if (imageSource is FileImageSource)
//            {
//                handler = new FileImageSourceHandler();
//            }
//            else if (imageSource is StreamImageSource)
//            {
//                handler = new StreamImagesourceHandler();
//            }
//            else if (imageSource is UriImageSource)
//            {
//                handler = new ImageLoaderSourceHandler();
//            }
//            else
//            {
//                throw new NotSupportedException();
//            }
//
//            return handler.LoadImageAsync(imageSource, Context);
//        }
//
//        public void OnLayoutChange(View v, int left, int top, int right, int bottom, int oldLeft, int oldTop, int oldRight, int oldBottom)
//        {
//        }
    }
}