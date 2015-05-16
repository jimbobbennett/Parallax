using Xamarin.Forms;

namespace Parallax
{
    [ContentProperty("ScrollContent")]
    public class ParallalPage : ContentPage
    {
        private readonly Grid _grid;
        private readonly Image _image;
        private readonly ScrollView _scrollView;
        private readonly ContentView _contentView;

        public ParallalPage()
        {
            _grid = new Grid
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

            _scrollView = new ScrollView
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.FillAndExpand,
                IsClippedToBounds = true
            };

            _contentView = new ContentView
            {
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            _scrollView.Content = _contentView;

            _image.SizeChanged += (s, e) =>
            {
                _contentView.Padding = new Thickness(0, _image.Height, 0, 0);
            };

            _grid.Children.Add(_image);
            _grid.Children.Add(_scrollView);

            Content = _grid;
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
