using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using Parallax;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using View = Android.Views.View;

[assembly: ExportRenderer(typeof(ParallaxView), typeof(Parallax.Droid.ParallaxViewRenderer))]

namespace Parallax.Droid
{
    public class ParallaxViewRenderer : ViewRenderer<ParallaxView, View>
    {
    }
}