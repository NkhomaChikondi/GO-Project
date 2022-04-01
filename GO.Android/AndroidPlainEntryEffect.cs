using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GO.Droid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ResolutionGroupName("PlainEntryGroup")]
[assembly: ExportEffect(typeof(AndroidPlainEntryEffect), "PlainEntryEffect")]



namespace GO.Droid
{
    public class AndroidPlainEntryEffect : PlatformEffect
    {
        public AndroidPlainEntryEffect()
        {

        }

        protected override void OnAttached()
        {
            try
            {
                if (Control != null)
                {
                    Android.Graphics.Color entryLineColor = Android.Graphics.Color.White;
                    Control.BackgroundTintList = ColorStateList.ValueOf(entryLineColor);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error... Unable to set property on attached control", ex.Message);
            }
        }
        protected override void OnDetached()
        {
        }
        protected override void OnElementPropertyChanged(PropertyChangedEventArgs args)
        {
            base.OnElementPropertyChanged(args);
        }
    }


}