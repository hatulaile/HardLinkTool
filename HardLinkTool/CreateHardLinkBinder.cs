using System.CommandLine;
using System.CommandLine.Binding;

namespace HardLinkTool;

public class CreateHardLinkBinder : BinderBase<CreateHardLinkOption>
{
    private readonly Argument<string> _inputArgument;
    private readonly Option<string?> _outputOption;
    private readonly Option<long> _skipSizeOption;
    private readonly Option<bool> _overwriteOption;

    public CreateHardLinkBinder(Argument<string> inputArgument, Option<string?> outputOption,
        Option<long> skipSizeOption, Option<bool> overwriteOption)
    {
        _inputArgument = inputArgument;
        _outputOption = outputOption;
        _skipSizeOption = skipSizeOption;
        _overwriteOption = overwriteOption;
    }

    protected override CreateHardLinkOption GetBoundValue(BindingContext bindingContext)
    {
        return new CreateHardLinkOption
        {
            Input = bindingContext.ParseResult.GetValueForArgument(_inputArgument),
            Output = bindingContext.ParseResult.GetValueForOption(_outputOption),
            SkipSize = bindingContext.ParseResult.GetValueForOption(_skipSizeOption),
            IsOverwrite = bindingContext.ParseResult.GetValueForOption(_overwriteOption)
        };
    }
}