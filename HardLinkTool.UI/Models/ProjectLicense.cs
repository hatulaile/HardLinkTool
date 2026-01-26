namespace HardLinkTool.UI.Models;

public sealed class ProjectLicense
{
    public string ProjectName { get; }

    public string ProjectUrl { get; }

    public string? LicensesUrl { get; }

    public string LicensesRaw { get; }

    public ProjectLicense(string projectName, string projectUrl, string licensesRaw, string? licensesUrl = null)
    {
        ProjectName = projectName;
        ProjectUrl = projectUrl;
        LicensesRaw = licensesRaw;
        LicensesUrl = licensesUrl;
    }
}