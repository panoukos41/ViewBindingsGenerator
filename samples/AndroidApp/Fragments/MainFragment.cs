using Android.OS;
using Android.Views;
using P41.ViewBindingsGenerator;
using Fragment = AndroidX.Fragment.App.Fragment;

namespace AndroidApp.Fragments;

[AndroidBinding("fragment_main.xml")]
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
