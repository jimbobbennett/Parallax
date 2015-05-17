using Xamarin.Forms;

namespace Parallax
{
    [ContentProperty("ScrollContent")]
    public class ParallalPage : ContentPage
    {
        private readonly Image _image;
        private readonly ContentView _contentView;
        private readonly ScrollView _scrollView;
        private readonly Grid _imageGrid;

        public ParallalPage()
        {
            var grid = new Grid
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill
            };
        
            _image = new Image
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Start,
                Aspect = Aspect.AspectFit
            };
         
            _imageGrid = new Grid
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Start,
                BackgroundColor = Color.Black
            };

            _imageGrid.Children.Add(_image);

            _scrollView = new ScrollView
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.FillAndExpand,
                IsClippedToBounds = true
            };

            var stackLayout = new StackLayout
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill,
                Orientation = StackOrientation.Vertical
            };

            var paddingGrid = new Grid
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Start
            };
            
            _contentView = new ContentView
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = BackgroundColor
            };

            stackLayout.Children.Add(paddingGrid);
            stackLayout.Children.Add(_contentView);

            _scrollView.Content = stackLayout;

            _image.SizeChanged += (s, e) =>
            {
                paddingGrid.HeightRequest = _image.Height;
                _imageGrid.HeightRequest = _image.Height;
            };

            _scrollView.Scrolled += (s, e) =>
            {
                SetScroll();
                
                _image.Opacity = System.Math.Max(0, (_image.Height - _scrollView.ScrollY) / _image.Height);
            };
                    
            grid.Children.Add(_imageGrid);
            grid.Children.Add(_scrollView);

            Content = grid;

            PropertyChanged += (s, e) =>
            {
                switch (e.PropertyName)
                {
                    case "BackgroundColor":
                        _contentView.BackgroundColor = BackgroundColor;
                        break;
                }
            };
        }

        private void SetScroll()
        {
            var scroll = _scrollView.ScrollY / ParallaxRate;
            _imageGrid.Padding = new Thickness(0, System.Math.Min(0, -1 * scroll), 0, 0);
        }

        public static readonly BindableProperty ImageSourceProperty =  BindableProperty.Create<ParallalPage, ImageSource>(p => p.ImageSource, null, 
            propertyChanged:OnImageSourceChanged);

        private static void OnImageSourceChanged(BindableObject bindable, ImageSource oldvalue, ImageSource newvalue)
        {
            ((ParallalPage) bindable)._image.Source = newvalue;
        }

        public ImageSource ImageSource
        {
            get { return (ImageSource) GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        public static readonly BindableProperty ParallaxRateProperty =  BindableProperty.Create<ParallalPage, float>(p => p.ParallaxRate, 2.5f, 
            propertyChanged:OnParallaxRateChanged);

        private static void OnParallaxRateChanged(BindableObject bindable, float oldvalue, float newvalue)
        {
            ((ParallalPage) bindable).SetScroll();
        }

        public float ParallaxRate
        {
            get { return (float) GetValue(ParallaxRateProperty); }
            set { SetValue(ParallaxRateProperty, value); }
        }

        private View _scrollContent;

        public View ScrollContent
        {
            get { return _scrollContent; }
            set
            {
                if (_scrollContent == value) return;

                _scrollContent = value;
                _contentView.Content = _scrollContent;
                _scrollContent.VerticalOptions = LayoutOptions.FillAndExpand;

                OnPropertyChanged();
            }
        }
    }
}
