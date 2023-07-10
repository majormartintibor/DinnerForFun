using MudBlazor;

namespace DFF.BlazorServer.Shared;

public partial class MainLayout
{
    private bool _drawerOpen = false;
    private bool _isDarkMode = true;
    private MudTheme MyCustomTheme { get; set; } = new()
    {
        Palette = new PaletteLight()
        {
            Primary = Colors.Blue.Default,
            Secondary = Colors.Green.Accent4,
            AppbarBackground = Colors.Red.Default,
        },
        PaletteDark = new PaletteDark()
        {
            Primary = Colors.Blue.Lighten1
        },

        LayoutProperties = new LayoutProperties()
        {
            DrawerWidthLeft = "260px",
            DrawerWidthRight = "300px"
        }
    };

    void DrawerToggle()
    {
        _drawerOpen = !_drawerOpen;
    }
}