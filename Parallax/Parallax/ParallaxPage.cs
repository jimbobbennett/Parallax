using Xamarin.Forms;

namespace Parallax
{
    [ContentProperty("Content")]
    public class ParallaxPage : Page
    {
        public static readonly BindableProperty ImageSourceProperty =  
            BindableProperty.Create<ParallaxPage, ImageSource>(p => p.ImageSource, null);

        public ImageSource ImageSource
        {
            get { return (ImageSource) GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        public static readonly BindableProperty ParallaxRateProperty =  
            BindableProperty.Create<ParallaxPage, float>(p => p.ParallaxRate, 2.5f);

        public float ParallaxRate
        {
            get { return (float) GetValue(ParallaxRateProperty); }
            set { SetValue(ParallaxRateProperty, value); }
        }

        private View _content;

        public View Content
        {
            get { return _content; }
            set
            {
                if (_content == value) return;

                _content = value;

                OnPropertyChanged();
            }
        }
    }
}
