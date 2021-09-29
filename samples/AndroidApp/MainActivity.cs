using Android.App;
using Android.OS;
using AndroidX.AppCompat.App;
using P41.ViewBindingsGenerator;

namespace AndroidApp;

[Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
[GenerateBindings("activity_main.xml")]
public partial class MainActivity : AppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        SetContentView(Resource.Layout.activity_main);

        HelloText.Text = "Hello Generetor";
        ClickMe.Text = "Generated";
        Progress.Indeterminate = true;
    }
}
