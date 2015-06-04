﻿using System;
using System.ComponentModel;
using System.Reflection;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using Xamarin.Forms;
using System.Threading.Tasks;
using CoreGraphics;
using Parallax;

[assembly: ExportRenderer(typeof(ParallaxView), typeof(Parallax.iOS.ParallaxViewRenderer))]

namespace Parallax.iOS
{
    public sealed class ParallaxViewRenderer : ViewRenderer<ParallaxView, UIView>
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

        public ParallaxViewRenderer()
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

            _view = new UIView { ClipsToBounds = true };
            _scrollContent = new UIView();

            _scrollView.Scrolled += ScrollViewScrolled;
            _scrollView.Add(_scrollContent);

            _view.Add(_imageBackground);
            _view.Add(_imageView);
            _view.Add(_statusBarView);
            _view.Add(_scrollView);
        }

        void ScrollViewScrolled(object sender, EventArgs e)
        {
            SetImageAlpha();

            var offSet = (float)Math.Max(0f, (_scrollView.ContentOffset.Y));
            var top = -1 * (offSet/Element.ParallaxRate);

            _imageView.Frame = new CGRect(new CGPoint(0, top),_imageView.Frame.Size);
            _imageBackground.Frame = _imageView.Frame;
        }

        private void SetImageAlpha()
        {
            var offSet = (float) Math.Max(0f, (_scrollView.ContentOffset.Y));

            var opacity = ((float) _imageView.Frame.Height - (offSet/2f))/_imageView.Frame.Height;

            if (Element.Fade)
                _imageView.Alpha = (float) Math.Min(Math.Max(0, opacity), 1);
            else
                _imageView.Alpha = 1f;
        }

        public override async void LayoutSubviews()
        {
            base.LayoutSubviews();

            await LayoutViewsAsync();
        }

        protected override async void OnElementChanged(ElementChangedEventArgs<ParallaxView> e)
        {
            base.OnElementChanged(e);

            _content = Element.Content;

            AddContentRenderer();

            await LayoutViewsAsync();
        }

        private void AddContentRenderer()
        {
            if (_renderer != null)
                _renderer.Dispose();

            _renderer = RendererFactory.GetRenderer(_content);
            
            if (_nativeView != null)
                _nativeView.RemoveFromSuperview();

            if (_content == null)
                return;

            _nativeView = _renderer.NativeView;
            SetNativeControl(_view);
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

        protected override async void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == Page.PaddingProperty.PropertyName ||
                e.PropertyName == ParallaxView.ImageSourceProperty.PropertyName ||
                e.PropertyName == VisualElement.BackgroundColorProperty.PropertyName)
            {
                await LayoutViewsAsync();
            }

            if (e.PropertyName == "Content")
            {
                _content = Element.Content;
                AddContentRenderer();
            }

            if (e.PropertyName == "Fade")
                SetImageAlpha();
        }

        private void LayoutContent()
        {
            if (_nativeView == null)
                return;

            var width = _view.Frame.Width;

            if (width == 0)
                return;

            var rendererSize = _renderer.GetDesiredSize(width, double.PositiveInfinity);
            var size = _content.GetSizeRequest(width, double.PositiveInfinity);

            _nativeView.Frame = new CGRect(new CGPoint(0, 0), new CGSize(width, Math.Max(size.Request.Height, rendererSize.Request.Height)));
            _content.Layout(_nativeView.Bounds.ToRectangle());

            _scrollContent.Frame = new CGRect(new CGPoint(0, _imageView.Frame.Height),
                new CGSize(width, _nativeView.Frame.Height));

            _scrollView.ContentSize = new CGSize(width, _scrollContent.Frame.Bottom);
        }

        private async Task LayoutViewsAsync()
        {
            if (Element == null)
                return;

            _statusBarView.BackgroundColor = Element.BackgroundColor.ToUIColor();
            _scrollContent.BackgroundColor = Element.BackgroundColor.ToUIColor();

            if (Element.BackgroundColor == Color.Transparent)
                _statusBarView.BackgroundColor = UIColor.White;

            _statusBarView.Frame = new CGRect(new CGPoint(0, 0),
                new CGSize(_view.Frame.Width, 0));

            _imageView.Image = await GetImageFromImageSourceAsync(Element.ImageSource);

            var width = Control.Frame.Width;
            var scaleFactor = _imageView == null || _imageView.Image == null ? 0 : width / _imageView.Image.Size.Width;

            var topPoint = new CGPoint(0, 0);

            _imageView.Frame = new CGRect(topPoint,
                new CGSize(width, _imageView == null || _imageView.Image == null ? 0 : _imageView.Image.Size.Height * scaleFactor));
            
            _imageBackground.Frame = _imageView.Frame;
                        
            _scrollView.Frame = new CGRect(topPoint, new CGSize(width, Control.Frame.Height));

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

