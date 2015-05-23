using System;
using System.Reflection;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using Xamarin.Forms;
using System.Threading.Tasks;
using CoreGraphics;
using Parallax;

[assembly: ExportRenderer(typeof(ParallaxPage), typeof(Parallax.iOS.ParallaxPageRenderer))]

namespace Parallax.iOS
{
    public sealed class ParallaxPageRenderer : PageRenderer
    {
        private readonly UIView _view;
        private readonly UIImageView _imageView;
        private readonly UIScrollView _scrollView;
        private readonly UIView _imageBackground;
        private readonly UIView _scrollContent;
        private UIView _nativeView;
        private readonly UIView _statusBarView;
        private View _content;
        private IVisualElementRenderer _renderer;

        public ParallaxPageRenderer()
        {
            _imageView = new UIImageView();
            _imageBackground = new UIView{ BackgroundColor = UIColor.Black };
            _scrollView = new UIScrollView
            {
                ScrollEnabled = true,
                ShowsVerticalScrollIndicator = true,
                ShowsHorizontalScrollIndicator = false,
                AlwaysBounceVertical = true,
                AlwaysBounceHorizontal = false
            };

            _statusBarView = new UIView();
            
            _view = new UIView();
            _scrollContent = new UIView();

            _scrollView.Scrolled += ScrollViewScrolled;
            _scrollView.Add(_scrollContent);

            _view.Add(_imageBackground);
            _view.Add(_imageView);
            _view.Add(_statusBarView);
            _view.Add(_scrollView);

            View = _view;
        }

        void ScrollViewScrolled(object sender, EventArgs e)
        {
            var offSet = (float)Math.Max(0f, (_scrollView.ContentOffset.Y));

            var opacity = ((float) _imageView.Frame.Height - (offSet/2f))/_imageView.Frame.Height;
            _imageView.Alpha = (float)Math.Min(Math.Max(0, opacity), 1);

            var top = ParallaxPage.Padding.Top - (offSet / ParallaxPage.ParallaxRate);
            _imageView.Frame = new CGRect(new CGPoint(0, top), 
                _imageView.Frame.Size);

            _imageBackground.Frame = _imageView.Frame;
        }

        public override async void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();

            await LayoutViewsAsync();
        }

        private ParallaxPage ParallaxPage { get; set; }

        protected override async void OnElementChanged(VisualElementChangedEventArgs e)
        {
            if (e.OldElement != null)
            {
                e.OldElement.PropertyChanged -= OnPropertyChanged;

                if (_nativeView != null)
                    _nativeView.RemoveFromSuperview();
            }

            base.OnElementChanged(e);

            ParallaxPage = e.NewElement as ParallaxPage;

            if (ParallaxPage != null)
            {
                ParallaxPage.PropertyChanged += OnPropertyChanged;

                _content = ParallaxPage.Content;

                AddContentRenderer();

                await LayoutViewsAsync();
            }
        }

        private void AddContentRenderer()
        {
            _renderer = RendererFactory.GetRenderer(_content);

            if (_nativeView != null)
                _nativeView.RemoveFromSuperview();

            if (_content == null)
                return;
            
            _nativeView = _renderer.NativeView;
            _scrollContent.Add(_nativeView);

            var type = Type.GetType("Xamarin.Forms.Platform.iOS.Platform,Xamarin.Forms.Platform.iOS");
            if (type != null)
            {
                var field = type.GetField("RendererProperty", BindingFlags.Static | BindingFlags.Public);

                if (field != null)
                {
                    _content.SetValue((BindableProperty)field.GetValue(null), _renderer);
                }
            }

            LayoutContent();
        }

        private async void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Page.PaddingProperty.PropertyName ||
                e.PropertyName == ParallaxPage.ImageSourceProperty.PropertyName ||
                e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
            {
                await LayoutViewsAsync();
            }

            if (e.PropertyName == "Content")
            {
                _content = ParallaxPage.Content;
                
                AddContentRenderer();
            }
        }

        private void LayoutContent()
        {
            if (_nativeView == null)
                return;

            var width = View.Frame.Width;

            if (width == 0)
                return;
            
            var size = _renderer.GetDesiredSize(View.Frame.Width, 0);

            _nativeView.Frame = new CGRect(new CGPoint(0, 0), new CGSize(width, size.Request.Height));
            _content.Layout(_nativeView.Bounds.ToRectangle());

            _scrollContent.Frame = new CGRect(new CGPoint(0, _imageView.Frame.Height),
                new CGSize(width, _nativeView.Frame.Height));

            _scrollView.ContentSize = new CGSize(width, _scrollContent.Frame.Bottom + ParallaxPage.Padding.Top);
        }

        private async Task LayoutViewsAsync()
        {
            if (ParallaxPage == null)
                return;

            _statusBarView.BackgroundColor = ParallaxPage.BackgroundColor.ToUIColor();
            _scrollContent.BackgroundColor = ParallaxPage.BackgroundColor.ToUIColor();

            if (ParallaxPage.BackgroundColor == Color.Transparent)
                _statusBarView.BackgroundColor = UIColor.White;

            _statusBarView.Frame = new CGRect(new CGPoint(0, 0),
                new CGSize(_view.Frame.Width, ParallaxPage.Padding.Top));

            _imageView.Image = await GetImageFromImageSourceAsync(ParallaxPage.ImageSource);

            var width = View.Frame.Width;
            var scaleFactor = width / _imageView.Image.Size.Width;

            var topPoint = new CGPoint(0, ParallaxPage.Padding.Top);

            _imageView.Frame = new CGRect(topPoint, 
                new CGSize(width, _imageView.Image.Size.Height * scaleFactor));
            
            _imageBackground.Frame = _imageView.Frame;
                        
            _scrollView.Frame = new CGRect(topPoint, new CGSize(width, View.Frame.Height));

            LayoutContent();
           
            ScrollViewScrolled(null, null);
        }

        private static Task<UIImage> GetImageFromImageSourceAsync(ImageSource imageSource)
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

            return handler.LoadImageAsync(imageSource);
        }
    }
}

