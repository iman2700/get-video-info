using System.Reflection;
using System.Runtime.CompilerServices;

namespace Application.Common.Interfaces;

public interface ILogger
{
    public bool Verbose(string? text, MethodBase? currentMethod, Exception? exception = null,
        [CallerLineNumber] int lineNumber = -1);

    public bool Debug(string? text, MethodBase? currentMethod, Exception? exception = null,
        [CallerLineNumber] int lineNumber = -1);

    public bool Information(string? text, MethodBase? currentMethod, Exception? exception = null,
        [CallerLineNumber] int lineNumber = -1);

    public bool Warning(string? text, MethodBase? currentMethod, Exception? exception = null,
        [CallerLineNumber] int lineNumber = -1);

    public bool Error(string? text, MethodBase? currentMethod, [CallerLineNumber] int lineNumber = -1);

    public bool Error(Exception? exception, MethodBase? currentMethod, string? text = null,
        [CallerLineNumber] int lineNumber = -1);

    public bool Fatal(string? text, MethodBase? currentMethod, [CallerLineNumber] int lineNumber = -1);

    public bool Fatal(Exception? exception, MethodBase? currentMethod, string? text = null,
        [CallerLineNumber] int lineNumber = -1);
}
