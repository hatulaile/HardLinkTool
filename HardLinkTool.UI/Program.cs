using Avalonia;
using System;
using System.Collections.Generic;
using Avalonia.Media;

namespace HardLinkTool.UI;

sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace()
            .With(new FontManagerOptions
            {
                DefaultFamilyName = "Source Han Sans CN",
                FontFamilyMappings = new Dictionary<string, FontFamily>
                {
                    { "Source Han Sans CN", new FontFamily(
                        "avares://HardLinkTool.UI/Assets/Fonts/SourceHanSansCN/SourceHanSansCN-Regular.otf#Source Han Sans CN") },
                
                    { "Source Han Sans CN Bold", new FontFamily(
                        "avares://HardLinkTool.UI/Assets/Fonts/SourceHanSansCN/SourceHanSansCN-Bold.otf#思源黑体 CN") },
                }
            });
}