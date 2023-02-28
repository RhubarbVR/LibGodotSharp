using GodotGame;
using LibGodotSharp;
using Org.Godotengine.Godot;

[Activity(
    Name = "com.godot.game.GodotApp" ,
    Label = "@string/godot_project_name_string",
    Theme = "@style/GodotAppSplashTheme",
    LaunchMode = Android.Content.PM.LaunchMode.SingleTask,
    ExcludeFromRecents = false,
    Exported = true,
    ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape,
    ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.ScreenSize | Android.Content.PM.ConfigChanges.Density | Android.Content.PM.ConfigChanges.Keyboard | Android.Content.PM.ConfigChanges.Navigation | Android.Content.PM.ConfigChanges.ScreenLayout | Android.Content.PM.ConfigChanges.UiMode,
    ResizeableActivity = false,
    MainLauncher = true)]
public class GodotApp : FullScreenGodotApp
{
    public unsafe override void OnCreate(Bundle? savedInstanceState)
    {
        Java.Lang.JavaSystem.LoadLibrary("godot_android");
        LibGodotManager.RunGodot(Array.Empty<string>(), TestLibGodotSharpExtensionEntry.EntryPoint, MainGodotGame.LoadScene, MainGodotGame.LoadProjectSettings, true);
        SetTheme(TestAndroid.Resource.Style.GodotAppMainTheme);
        base.OnCreate(savedInstanceState);
    }
}