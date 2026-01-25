using System.Threading.Tasks;
using HardLinkTool.Library.Modules;

namespace HardLinkTool.UI.Services;

public interface IDialogService
{
    Task<string> OpenEditable(HardLinkEntry entry);
}