using Xamarin.Forms;

namespace Parallax
{
    [ContentProperty("Content")]
    public class ParallaxView : View
    {
        public static readonly BindableProperty ImageSourceProperty =
            BindableProperty.Create<ParallaxView, ImageSource>(p => p.ImageSource, null);

        public ImageSource ImageSource
        {
            get { return (ImageSource) GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        public static readonly BindableProperty ParallaxRateProperty =
            BindableProperty.Create<ParallaxView, float>(p => p.ParallaxRate, 2.5f);

        public float ParallaxRate
        {
            get { return (float)GetValue(ParallaxRateProperty); }
            set { SetValue(ParallaxRateProperty, value); }
        }

        public static readonly BindableProperty FadeProperty =
            BindableProperty.Create<ParallaxView, bool>(p => p.Fade, true);

        public bool Fade
        {
            get { return (bool)GetValue(FadeProperty); }
            set { SetValue(FadeProperty, value); }
        }

        private View _content;

        public View Content
        {
            get { return _content; }
            set
            {
                if (_content != null)
                {
                    _content.BindingContext = null;
                    OnChildRemoved(_content);
                }

                if (_content == value) return;

                _content = value;

                if (_content != null)
                {
                    _content.BindingContext = BindingContext;
                    OnChildAdded(_content);
                }

                OnPropertyChanged();
            }
        }

        protected override void OnBindingContextChanged()
        {
            if (_content != null)
            {
                OnChildRemoved(_content);
                _content.BindingContext = BindingContext;
                OnChildAdded(_content);
            }
        }
    }
}
