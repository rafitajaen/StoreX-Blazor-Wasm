using FSH.BlazorWebAssembly.Client.Infrastructure.Preferences;
using FSH.BlazorWebAssembly.Client.Infrastructure.Theme;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace FSH.BlazorWebAssembly.Client.Shared;

public partial class BaseLayout
{
    private ClientPreference? _themePreference;
    private MudTheme _currentTheme = new LightTheme();
    private bool _themeDrawerOpen;
    private bool _rightToLeft;

    protected override async Task OnInitializedAsync()
    {
        _themePreference = await ClientPreferences.GetPreference() as ClientPreference;
        if (_themePreference == null) _themePreference = new ClientPreference();
        SetCurrentTheme(_themePreference);

        Snackbar.Add("Like this app? ", Severity.Normal, config =>
        {
            config.BackgroundBlurred = true;
            config.Icon = Icons.Custom.Brands.GitHub;
            config.Action = "Get in touch on LinkedIn!";
            config.ActionColor = Color.Primary;
            config.Onclick = snackbar =>
            {
                Navigation.NavigateTo("https://www.linkedin.com/in/rafitajaen/");
                return Task.CompletedTask;
            };
        });
    }

    private async Task ThemePreferenceChanged(ClientPreference themePreference)
    {
        SetCurrentTheme(themePreference);
        await ClientPreferences.SetPreference(themePreference);
    }

    private void SetCurrentTheme(ClientPreference themePreference)
    {
        _currentTheme = themePreference.IsDarkMode ? new DarkTheme() : new LightTheme();
        _currentTheme.Palette.Primary = themePreference.PrimaryColor;
        _currentTheme.Palette.Secondary = themePreference.SecondaryColor;
        _currentTheme.LayoutProperties.DefaultBorderRadius = $"{themePreference.BorderRadius}px";
        _currentTheme.LayoutProperties.DefaultBorderRadius = $"{themePreference.BorderRadius}px";
        _rightToLeft = themePreference.IsRTL;
    }
}