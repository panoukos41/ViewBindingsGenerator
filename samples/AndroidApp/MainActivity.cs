using Android.App;
using Android.OS;
using Android.Views;
using AndroidX.AppCompat.App;
using P41.ViewBindingsGenerator;
using Fragment = AndroidX.Fragment.App.Fragment;

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

[GenerateBindings("fragment_main.xml")]
public partial class MainFragment : Fragment
{
    public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
    {
        var view = inflater.Inflate(Resource.Layout.fragment_main, container, false)!;

        HelloText.Text = "Hello Generetor";
        ClickMe.Text = "Generated";
        Progress.Indeterminate = true;

        return view;
    }
}
