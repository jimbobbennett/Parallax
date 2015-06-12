using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Parallax.Annotations;
using Xamarin.Forms;

namespace Parallax
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private bool _fade = true;
        private decimal _rate = 2.5m;

        public MainViewModel()
        {
            Text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et " +
                   "dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip " +
                   "ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore " +
                   "eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia " +
                   "deserunt mollit anim id est laborum.\n" +
                   "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et " +
                   "dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip " +
                   "ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore " +
                   "eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia " +
                   "deserunt mollit anim id est laborum.\n" +
                   "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et " +
                   "dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip " +
                   "ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore " +
                   "eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia " +
                   "deserunt mollit anim id est laborum.";
        }

        public string Text { get; set; }

        public bool Fade
        {
            get { return _fade; }
            set
            {
                if (_fade == value) return;
                _fade = value;
                OnPropertyChanged();
            }
        }

        public decimal Rate
        {
            get { return _rate; }
            set
            {
                if (value.Equals(_rate)) return;
                _rate = value;
                OnPropertyChanged();
                OnPropertyChanged("FloatRate");
            }
        }

        public float FloatRate { get { return Convert.ToSingle(Rate); } }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            Padding = new Thickness(0, Device.OnPlatform(20, 0, 0), 0, 0);

            BindingContext = new MainViewModel();
        }
    }
}
