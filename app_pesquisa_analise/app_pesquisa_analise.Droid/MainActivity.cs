using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using Android.Support.Design.Widget;
using app_pesquisa_analise.Droid.Utils;

namespace app_pesquisa_analise.Droid
{
    //[Activity(Label = "app_pesquisa_analise", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    //[Activity(Label = "Pesquisa Analise App", Theme = "@android:style/Theme.Holo.Light", Icon = "@android:color/transparent", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    [Activity(Label = "EZQUEST Análise", Theme = "@android:style/Theme.Holo.Light", Icon = "@drawable/icon_pesquisa_analise", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            
            AndroidUtils.SetMainActicity(this);
            
            global::Xamarin.Forms.Forms.Init(this, bundle);

            LoadApplication(new App());
        }
        
    }
}

