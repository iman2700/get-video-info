using Serilog;
using Serilog.Events;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Net;
using Infrastructure.Registry;
using Serilog.Formatting.Json;
using Microsoft.Extensions.Configuration;
using Ardalis.GuardClauses;

namespace Infrastructure.Log;

public class CustomSerilog : Application.Common.Interfaces.ILogger
{
    /// <summary>
    /// Key name of system logs
    /// </summary>

    private string? _hostAppName;

    private int _selfLogCounter;
    private int _failureLogCounter;
    private int _maxLogNumber;

    private Serilog.Core.Logger _seriLogger;

    public CustomSerilog(IConfiguration configuration)
    {
        _hostAppName = configuration.GetValue<string>("SeriLog:HostAppName");
        Guard.Against.NullOrEmpty(_hostAppName);

        _maxLogNumber = configuration.GetValue<int>("SeriLog:MaxLogNumber", -1);
        Guard.Against.NegativeOrZero(_maxLogNumber);

        int maxFailureLogNumber = configuration.GetValue<int>("SeriLog:MaxFailureLogNumber", -1);
        Guard.Against.NegativeOrZero(maxFailureLogNumber);
        
        string? logsDirectoryRegistryKeyName = configuration.GetValue<string>("SeriLog:LogsDirectoryRegistryKeyName");
        Guard.Against.NullOrEmpty(logsDirectoryRegistryKeyName);

        string? logFileExtension = configuration.GetValue<string>("SeriLog:LogFileExtension");
        Guard.Against.NullOrEmpty(logFileExtension);

        string? logOutputTemplate = configuration.GetValue<string>("SeriLog:LogOutputTemplate");
        Guard.Against.NullOrEmpty(logOutputTemplate);

        LogEventLevel? debugMinimumLevel = configuration.GetValue<LogEventLevel?>("SeriLog:DebugMinimumLevel", null);
        Guard.Against.Null(debugMinimumLevel);

        LogEventLevel? consoleMinimumLevel = configuration.GetValue<LogEventLevel?>("SeriLog:ConsoleMinimumLevel", null);
        Guard.Against.Null(consoleMinimumLevel);

        LogEventLevel? fileSaveMinimumLevel = configuration.GetValue<LogEventLevel?>("SeriLog:FileSaveMinimumLevel", null);
        Guard.Against.Null(fileSaveMinimumLevel);

        bool elasticSearchEnable = configuration.GetValue<bool>("SeriLog:ElasticSearchEnable", false);

        LogEventLevel? elasticsearchMinimumLevel = configuration.GetValue<LogEventLevel?>("SeriLog:ElasticsearchMinimumLevel", null);
        string? elasticSearchUrl = configuration.GetValue<string>("SeriLog:ElasticSearchUrl");
        string? elasticSearchUsername = configuration.GetValue<string>("SeriLog:ElasticSearchUsername");
        string? elasticSearchPassword = configuration.GetValue<string>("SeriLog:ElasticSearchPassword");

        if (elasticSearchEnable)
        {
            Guard.Against.Null(elasticsearchMinimumLevel);
            Guard.Against.NullOrEmpty(elasticSearchUrl);
            Guard.Against.NullOrEmpty(elasticSearchUsername);
            Guard.Against.NullOrEmpty(elasticSearchPassword);
        }

        _seriLogger = Initialize(maxFailureLogNumber, logsDirectoryRegistryKeyName,
            logFileExtension, logOutputTemplate, (LogEventLevel)debugMinimumLevel, (LogEventLevel)consoleMinimumLevel,
            (LogEventLevel)fileSaveMinimumLevel, elasticSearchEnable, (LogEventLevel)elasticsearchMinimumLevel,
            elasticSearchUrl, elasticSearchUsername, elasticSearchPassword);
    }

    /// <summary>
    /// Initializes the <see cref="Log.Logger"/>.
    /// </summary>
    /// <param name="directory">
    /// Directory of the local log file.
    /// </param>
    /// <param name="fileName">
    /// Bare name of the local log file (without directory and extension).
    /// <para />
    /// If this parameter is not specified, the default name for the local log file will be used.
    /// </param>
    /// <returns>
    /// The log file path.
    /// </returns>
    /// <exception cref="Exception">
    /// The log file directory cannot be created.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// The specified <paramref name="directory"/> is empty or cotains only white space, or
    /// the specified <paramref name="fileName"/> is empty or cotains only white space.
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// The specified <paramref name="directory"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="DirectoryNotFoundException">
    /// The specified <paramref name="directory"/> cannot be found on this machine.
    /// </exception>
    /// <remarks>
    /// For more information about log levels refer to:
    /// https://github.com/serilog/serilog/wiki/Configuration-Basics#minimum-level
    /// </remarks>
    private Serilog.Core.Logger Initialize(int maxFailureLogNumber, string logsDirectoryRegistryKeyName,
        string logFileExtension, string logOutputTemplate, LogEventLevel debugMinimumLevel, LogEventLevel consoleMinimumLevel,
        LogEventLevel fileSaveMinimumLevel, bool elasticSearchEnable, LogEventLevel elasticsearchMinimumLevel,
        string elasticSearchUrl, string elasticSearchUsername, string elasticSearchPassword)
    {
        string directory = InitializeLoggerMainDirectory(logsDirectoryRegistryKeyName);

        if (string.IsNullOrWhiteSpace(directory))
            throw new ArgumentException("The specified string is empty or contains only whitespace charachters.",
                nameof(directory));

        string fileName = $"{_hostAppName} - ";

        if (!Directory.Exists(directory))
            throw new DirectoryNotFoundException("The specified directory was not found.");


        // Creates the local file path
        string logFilePath = Path.Combine(directory, fileName + logFileExtension);

        // Creates the logger configuration object
        LoggerConfiguration loggerConfiguration = new LoggerConfiguration();

        // Sets the global minimum level of logging
        loggerConfiguration.MinimumLevel.Verbose();

        loggerConfiguration.Enrich.WithMachineName();
        loggerConfiguration.Enrich.WithEnvironmentUserName();
        loggerConfiguration.Enrich.WithProperty("IPs", Dns.GetHostAddresses(Dns.GetHostName()));

        //TODO Add Version of assembly

        loggerConfiguration.Enrich.WithProperty("Product", _hostAppName);


        // Configures the Visual Studio debug logging
        loggerConfiguration.WriteTo.Debug(restrictedToMinimumLevel: debugMinimumLevel,
            outputTemplate: logOutputTemplate);

        // Configures the console logging
        loggerConfiguration.WriteTo.Console(restrictedToMinimumLevel: consoleMinimumLevel,
            outputTemplate: logOutputTemplate);

        // Configures the local file logging
        // Ref.: https://github.com/serilog/serilog/wiki/Configuration-Basics#enrichers
        loggerConfiguration.WriteTo.File(logFilePath,
            restrictedToMinimumLevel: fileSaveMinimumLevel,
            rollingInterval: RollingInterval.Month,
            outputTemplate: logOutputTemplate,
            retainedFileCountLimit: 10,
            rollOnFileSizeLimit: true,
            fileSizeLimitBytes: 24575000,
            shared: true);

        if (elasticSearchEnable)
            ConfigureElasticSearch(loggerConfiguration, directory, elasticSearchUrl, 
                elasticSearchUsername, elasticSearchPassword, elasticsearchMinimumLevel,
                logFileExtension, maxFailureLogNumber);

        // Adds the self-log action
        Serilog.Debugging.SelfLog.Enable(LogSerilogSelfLog);

        // Creates 'Logger' from the configuration
        var logger = loggerConfiguration.CreateLogger();

        Console.WriteLine();
        Console.WriteLine(@"Logger is initialized.");
        Console.WriteLine($@"Local log file path: {logFilePath}");

        System.Diagnostics.Debug.WriteLine(
            $"Log file path:{logFilePath}\n loggerConfiguration: {loggerConfiguration}");
        return logger;
    }

    /// <summary>
    /// Creates a log with the <b>Verbose</b> level.
    /// </summary>
    /// <param name="text">
    /// The text associated with this log.
    /// </param>
    /// <param name="currentMethod">
    /// A <see cref="MethodBase"/> representing the method that is calling this method.
    /// <para>
    /// This can be: <c>System.Reflection.MethodBase.GetCurrentMethod()</c>
    /// </para>
    /// </param>
    /// <param name="exception">
    /// The <see cref="Exception"/> associated with this log.
    /// </param>
    /// <param name="lineNumber">
    /// Specifies in which line this method is called.
    /// <para>
    /// Do not specify this parameter!
    /// </para>
    /// </param>
    /// <returns>
    /// <see langword="true"/>, if the log is created successfully, otherwise <see langword="false"/>.
    /// </returns>
    public bool Verbose(string? text, MethodBase? currentMethod, Exception? exception = null,
        [CallerLineNumber] int lineNumber = -1)
    {
        try
        {
            string messageTemplate = CreateLogMessage(currentMethod, text, exception, lineNumber);

            _seriLogger.Verbose(messageTemplate);

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Creates a log with the <b>Debug</b> level.
    /// </summary>
    /// <param name="text">
    /// The text associated with this log.
    /// </param>
    /// <param name="currentMethod">
    /// A <see cref="MethodBase"/> representing the method that is calling this method.
    /// <para>
    /// This can be: <c>System.Reflection.MethodBase.GetCurrentMethod()</c>
    /// </para>
    /// </param>
    /// <param name="exception">
    /// The <see cref="Exception"/> associated with this log.
    /// </param>
    /// <param name="lineNumber">
    /// Specifies in which line this method is called.
    /// <para>
    /// Do not specify this parameter!
    /// </para>
    /// </param>
    /// <returns>
    /// <see langword="true"/>, if the log is created successfully, otherwise <see langword="false"/>.
    /// </returns>
    public bool Debug(string? text, MethodBase? currentMethod, Exception? exception = null,
        [CallerLineNumber] int lineNumber = -1)
    {
        try
        {
            string messageTemplate = CreateLogMessage(currentMethod, text, exception, lineNumber);

            _seriLogger.Debug(messageTemplate);

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Creates a log with the <b>Information</b> level.
    /// </summary>
    /// <param name="text">
    /// The text associated with this log.
    /// </param>
    /// <param name="currentMethod">
    /// A <see cref="MethodBase"/> representing the method that is calling this method.
    /// <para>
    /// This can be: <c>System.Reflection.MethodBase.GetCurrentMethod()</c>
    /// </para>
    /// </param>
    /// <param name="exception">
    /// The <see cref="Exception"/> associated with this log.
    /// </param>
    /// <param name="lineNumber">
    /// Specifies in which line this method is called.
    /// <para>
    /// Do not specify this parameter!
    /// </para>
    /// </param>
    /// <returns>
    /// <see langword="true"/>, if the log is created successfully, otherwise <see langword="false"/>.
    /// </returns>
    public bool Information(string? text, MethodBase? currentMethod, Exception? exception = null,
        [CallerLineNumber] int lineNumber = -1)
    {
        try
        {
            string messageTemplate = CreateLogMessage(currentMethod, text, exception, lineNumber);

            _seriLogger.Information(messageTemplate);

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Creates a log with the <b>Warning</b> level.
    /// </summary>
    /// <param name="text">
    /// The text associated with this log.
    /// </param>
    /// <param name="currentMethod">
    /// A <see cref="MethodBase"/> representing the method that is calling this method.
    /// <para>
    /// This can be: <c>System.Reflection.MethodBase.GetCurrentMethod()</c>
    /// </para>
    /// </param>
    /// <param name="exception">
    /// The <see cref="Exception"/> associated with this log.
    /// </param>
    /// <param name="lineNumber">
    /// Specifies in which line this method is called.
    /// <para>
    /// Do not specify this parameter!
    /// </para>
    /// </param>
    /// <returns>
    /// <see langword="true"/>, if the log is created successfully, otherwise <see langword="false"/>.
    /// </returns>
    public bool Warning(string? text, MethodBase? currentMethod, Exception? exception = null,
        [CallerLineNumber] int lineNumber = -1)
    {
        try
        {
            string messageTemplate = CreateLogMessage(currentMethod, text, exception, lineNumber);

            _seriLogger.Warning(messageTemplate);

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Creates a log with the <b>Error</b> level.
    /// </summary>
    /// <param name="text">
    /// The text associated with this log.
    /// </param>
    /// <param name="currentMethod">
    /// A <see cref="MethodBase"/> representing the method that is calling this method.
    /// <para>
    /// This can be: <c>System.Reflection.MethodBase.GetCurrentMethod()</c>
    /// </para>
    /// </param>
    /// <param name="lineNumber">
    /// Specifies in which line this method is called.
    /// <para>
    /// Do not specify this parameter!
    /// </para>
    /// </param>
    /// <returns>
    /// <see langword="true"/>, if the log is created successfully, otherwise <see langword="false"/>.
    /// </returns>
    public bool Error(string? text, MethodBase? currentMethod, [CallerLineNumber] int lineNumber = -1)
    {
        try
        {
            string messageTemplate = CreateLogMessage(currentMethod, text, null, lineNumber);

            _seriLogger.Error(messageTemplate);

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Creates a log with the <b>Error</b> level.
    /// </summary>
    /// <param name="text">
    /// The text associated with this log.
    /// </param>
    /// <param name="currentMethod">
    /// A <see cref="MethodBase"/> representing the method that is calling this method.
    /// <para>
    /// This can be: <c>System.Reflection.MethodBase.GetCurrentMethod()</c>
    /// </para>
    /// </param>
    /// <param name="exception">
    /// The <see cref="Exception"/> associated with this log.
    /// </param>
    /// <param name="lineNumber">
    /// Specifies in which line this method is called.
    /// <para>
    /// Do not specify this parameter!
    /// </para>
    /// </param>
    /// <returns>
    /// <see langword="true"/>, if the log is created successfully, otherwise <see langword="false"/>.
    /// </returns>
    public bool Error(Exception? exception, MethodBase? currentMethod, string? text = null,
        [CallerLineNumber] int lineNumber = -1)
    {
        try
        {

            string messageTemplate = CreateLogMessage(currentMethod, text, exception, lineNumber);

            _seriLogger.Error(messageTemplate);

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Creates a log with the <b>Fatal</b> level.
    /// </summary>
    /// <param name="text">
    /// The text associated with this log.
    /// </param>
    /// <param name="currentMethod">
    /// A <see cref="MethodBase"/> representing the method that is calling this method.
    /// <para>
    /// This can be: <c>System.Reflection.MethodBase.GetCurrentMethod()</c>
    /// </para>
    /// </param>
    /// <param name="lineNumber">
    /// Specifies in which line this method is called.
    /// <para>
    /// Do not specify this parameter!
    /// </para>
    /// </param>
    /// <returns>
    /// <see langword="true"/>, if the log is created successfully, otherwise <see langword="false"/>.
    /// </returns>
    public bool Fatal(string? text, MethodBase? currentMethod, [CallerLineNumber] int lineNumber = -1)
    {
        try
        {
            string messageTemplate = CreateLogMessage(currentMethod, text, null, lineNumber);

            _seriLogger.Fatal(messageTemplate);

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Creates a log with the <b>Fatal</b> level.
    /// </summary>
    /// <param name="text">
    /// The text associated with this log.
    /// </param>
    /// <param name="currentMethod">
    /// A <see cref="MethodBase"/> representing the method that is calling this method.
    /// <para>
    /// This can be: <c>System.Reflection.MethodBase.GetCurrentMethod()</c>
    /// </para>
    /// </param>
    /// <param name="exception">
    /// The <see cref="Exception"/> associated with this log.
    /// </param>
    /// <param name="lineNumber">
    /// Specifies in which line this method is called.
    /// <para>
    /// Do not specify this parameter!
    /// </para>
    /// </param>
    /// <returns>
    /// <see langword="true"/>, if the log is created successfully, otherwise <see langword="false"/>.
    /// </returns>
    public bool Fatal(Exception? exception, MethodBase? currentMethod, string? text = null,
        [CallerLineNumber] int lineNumber = -1)
    {
        try
        {
            string messageTemplate = CreateLogMessage(currentMethod, text, exception, lineNumber);

            _seriLogger.Fatal(messageTemplate);

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }



    /// <summary>
    /// Find directory of logs from registry or set it as default
    /// </summary>
    /// <returns></returns>
    public string InitializeLoggerMainDirectory(string logsDirectoryRegistryKeyName)
    {
        try
        {
            object directoryValueNullable = RegistryOperations.GetValue(new RegistryItem(new[] { logsDirectoryRegistryKeyName }));

            //log directory from registry
            string logsDir = directoryValueNullable.ToString() ?? throw new InvalidOperationException();

            if (!File.Exists(logsDir))
                Directory.CreateDirectory(logsDir);
            return logsDir;
        }
        catch (Exception ex)
        {
            //error reading from registry
            Console.WriteLine($"\n\n----error reading from registry---\n{ex}\n\n-------\n");
            try
            {
                FileInfo fileInfo = new FileInfo(Assembly.GetExecutingAssembly().Location);
                return fileInfo.DirectoryName ?? throw new InvalidOperationException("Directory name of the log files can not be null!");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }

    /// <summary>
    /// Creates the log message from the specified inputs.
    /// </summary>
    /// <param name="methodBase">
    /// A <see cref="MethodBase"/> representing the method that is calling the log method.
    /// </param>
    /// <param name="text">
    /// The text associated with the log.
    /// </param>
    /// <param name="exception">
    /// The <see cref="Exception"/> associated with the log.
    /// </param>
    /// <param name="lineNumber">
    /// Specifies in which line the log method is called.
    /// </param>
    /// <returns>
    /// The log message.
    /// </returns>
    private string CreateLogMessage(MethodBase? methodBase, string? text, Exception? exception, int lineNumber = -1)
    {
        // Creates the header
        string message = "\n" + CreateLogHeader(methodBase, lineNumber);

        // Adds the specified message
        if (!string.IsNullOrWhiteSpace(text))
            message += "\n\n" + text;

        // Adds the exception description
        if (exception is not null)
            message += "\n\n" + GetExceptionDescription(exception, methodBase);

        return message;
    }

    /// <summary>
    /// Creates the header of the log message.
    /// </summary>
    /// <param name="methodBase">
    /// A <see cref="MethodBase"/> representing the method that is calling the log method.
    /// </param>
    /// <param name="lineNumber">
    /// Specifies in which line the log method is called.
    /// </param>
    /// <returns>
    /// The header of the log message.
    /// </returns>
    private string CreateLogHeader(MethodBase? methodBase, int lineNumber = -1)
    {
        if (methodBase is null && lineNumber < 0)
            return string.Empty;
        else if (methodBase is null)
            return $"[Line: {lineNumber}]: ";
        else if (lineNumber < 0)
            return $"From {GetDescription(methodBase)}: ";
        else
            return $"From {GetDescription(methodBase)} [Line: {lineNumber}]: ";
    }

    /// <summary>
    /// Produces a text corresponding to the specified <paramref name="exception"/>.
    /// </summary>
    /// <param name="exception">
    /// The <see cref="Exception"/> to be described.
    /// </param>
    /// <param name="methodBase">
    /// A <see cref="MethodBase"/> representing the method that is calling the log method.
    /// </param>
    /// <returns>
    /// The description of the specified <paramref name="exception"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// The specified <paramref name="exception"/> is <see langword="null"/>.
    /// </exception>
    private string GetExceptionDescription(Exception? exception, MethodBase? methodBase = null)
    {
        if (exception is null)
            throw new ArgumentNullException(nameof(exception));

        string message = string.Empty;

        string standardIndent = "  ";

        if (methodBase is null)
            message += "An exception is thrown:";
        else
            message += $"An exception is thrown in {GetDescription(methodBase)}:";

        message += $"\n\n{standardIndent}Type: {exception.GetType().FullName}";

        if (!string.IsNullOrEmpty(exception.Message))
            message += string.Format("\n\n{0}Message:\n{0}{1}", standardIndent, exception.Message);

        if (!string.IsNullOrEmpty(exception.Source))
            message += $"\n\n{standardIndent}Source: {exception.Source}";

        if (GetDescription(exception.TargetSite) is string tagetSite)
            message += string.Format("\n\n{0}Target Site:\n{0}{1}", standardIndent, tagetSite);

        if (!string.IsNullOrEmpty(exception.StackTrace))
        {
            message += $"\n\n{standardIndent}Stack Trace:";

            foreach (string line in exception.StackTrace.Split(new string[] { "\r\n" },
                         StringSplitOptions.RemoveEmptyEntries))
                message += " \n" + standardIndent + line;
        }

        if (!string.IsNullOrEmpty(exception.HelpLink))
            message += $"\n\n{standardIndent}Help link:\n{exception.HelpLink}";

        if (exception.InnerException is Exception innerException)
            message += GetInnerExceptionDescription(innerException);

        return message;
    }

    /// <summary>
    /// Produces a text corresponding to the specified inner <paramref name="exception"/>
    /// of a parent <see cref="Exception"/>.
    /// </summary>
    /// <param name="exception">
    /// The inner <see cref="Exception"/> to be described.
    /// </param>
    /// <param name="indentIndex">
    /// Index of text indent corresponding to the specified <see cref="Exception"/>.
    /// <para>
    /// This value must be a positive number.
    /// </para>
    /// </param>
    /// <returns>
    /// The description of the specified inner <paramref name="exception"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// The specified <paramref name="exception"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// The specified indentIndex is not a positive number.
    /// </exception>
    private string GetInnerExceptionDescription(Exception exception, int indentIndex = 0)
    {
        if (exception is null)
            throw new ArgumentNullException(nameof(exception));

        if (indentIndex < 0)
            throw new ArgumentOutOfRangeException(nameof(indentIndex));

        string message = string.Empty;

        string parentIndent = string.Empty;

        for (int i = 0; i < indentIndex + 1; i++)
            parentIndent += "  ";

        string indent = string.Empty;

        for (int i = 0; i < indentIndex + 2; i++)
            indent += "  ";

        message += $"\n\n{parentIndent}Inner exception:";

        message += $"\n\n{indent}Type: {exception.GetType().FullName}";

        if (!string.IsNullOrEmpty(exception.Message))
            message += string.Format("\n\n{0}Message:\n{0}{1}", indent, exception.Message);

        if (!string.IsNullOrEmpty(exception.Source))
            message += $"\n\n{indent}Source: {exception.Source}";

        if (GetDescription(exception.TargetSite) is string tagetSite)
            message += string.Format("\n\n{0}Target Site:\n{0}{1}", indent, tagetSite);

        if (!string.IsNullOrEmpty(exception.StackTrace))
        {
            message += $"\n\n{indent}Stack Trace:";

            foreach (string line in exception.StackTrace.Split(new string[] { "\r\n" },
                         StringSplitOptions.RemoveEmptyEntries))
                message += " \n" + indent + line;
        }

        if (!string.IsNullOrEmpty(exception.HelpLink))
            message += $"\n\n{indent}Help link:\n{exception.HelpLink}";

        if (exception.InnerException is Exception innerException)
            message += GetInnerExceptionDescription(innerException, ++indentIndex);

        return message;
    }

    /// <summary>
    /// Retrieves the description of the specified <see cref="MethodBase"/>.
    /// </summary>
    /// <param name="methodBase">
    /// <see cref="MethodBase"/>.
    /// </param>
    /// <returns>
    /// A <see cref="string"/> representing the specified <see cref="MethodBase"/>.
    /// If the specified <see cref="MethodBase"/> is <see langword="null"/>,
    /// then returns <see langword="null"/>.
    /// </returns>
    private string? GetDescription(MethodBase? methodBase)
    {
        if (methodBase is null)
            return null;

        BindingFlags flags = BindingFlags.Default;

        // Selects all available flags
        foreach (BindingFlags flag in Enum.GetValues(typeof(BindingFlags)))
            flags |= flag;

        MethodInfo? methodInfo = null;

        try
        {
            // Retrieve the MethodInfo
            if (methodBase.ReflectedType != null)
                methodInfo = methodBase.ReflectedType.GetMethod(methodBase.Name, flags) ??
                             throw new InvalidOperationException();
        }
        catch (Exception)
        {
            methodInfo = null;
        }

        string parameters = string.Empty;

        // Retrieves the method arguments
        foreach (ParameterInfo parameter in methodBase.GetParameters())
            parameters += string.IsNullOrEmpty(parameters)
                ? $"{parameter.ParameterType.Name} {parameter.Name}"
                : $", {parameter.ParameterType.Name} {parameter.Name}";

        if (methodInfo is null)
        // Returns the caller address without its return type
        {
            if (methodBase.ReflectedType != null)
                return $"{methodBase.ReflectedType.FullName}.{methodBase.Name}({parameters})";
        }
        else
        // Returns the caller address and its return type
        {
            System.Diagnostics.Debug.Assert(methodBase.ReflectedType != null, "methodBase.ReflectedType != null");
            return
                $"({methodInfo.ReturnType.Name}) {methodBase.ReflectedType.FullName}.{methodInfo.Name}({parameters})";
        }
        return null;
    }

    /// <summary>
    /// Logs the Serilog self-log message.
    /// </summary>
    /// <param name="message">
    /// Serilog self-log message.
    /// </param>
    private void LogSerilogSelfLog(string message)
    {
        if (_selfLogCounter >= _maxLogNumber)
            return;

        Error(message + $"\nCounter: {_selfLogCounter}", MethodBase.GetCurrentMethod());
        _selfLogCounter++;
    }

    public void LogEventFailed(LogEvent logEvent)
    {
        string message = "Logger failed to submit the log event to the database.";
        System.Diagnostics.Debug.WriteLine(logEvent.ToString());

        if (logEvent.Exception is Exception exception)
            message += "\n\n" + exception.Message;

        System.Diagnostics.Debug.WriteLine($"ELASTICSEARCH ERROR \n {message}");
    }

    /// <summary>
    /// Configures the elastic search.
    /// </summary>
    /// <param name="loggerConfiguration">
    /// Logger configuration.
    /// </param>
    /// <param name="failureSinkLogDirectory">
    /// Directory of the failure sink log.
    /// </param>
    private void ConfigureElasticSearch(LoggerConfiguration loggerConfiguration,
        string failureSinkLogDirectory,string elasticSearchUrl,string elasticSearchUsername,string elasticSearchPassword,
        LogEventLevel elasticsearchMinimumLevel,string logFileExtension, int maxFailureLogNumber)
    {
        string defaultElasticSearchFailureLogFileName = $"{_hostAppName}FailureLog";
        // Configures the elasticsearch database logging.
        Serilog.Sinks.Elasticsearch.ElasticsearchSinkOptions elasticsearchSinkOptions =
            new(new Uri(elasticSearchUrl))
            {
                MinimumLogEventLevel = elasticsearchMinimumLevel,
                //AutoRegisterTemplate = true,
                AutoRegisterTemplateVersion = Serilog.Sinks.Elasticsearch.AutoRegisterTemplateVersion.ESv7,
                IndexFormat = $"{_hostAppName.ToLower()}-{DateTime.UtcNow:yyyy-MM}",
                FailureCallback = LogEventFailed,
                EmitEventFailure = Serilog.Sinks.Elasticsearch.EmitEventFailureHandling.WriteToSelfLog
                                   | Serilog.Sinks.Elasticsearch.EmitEventFailureHandling.WriteToFailureSink
                                   | Serilog.Sinks.Elasticsearch.EmitEventFailureHandling.RaiseCallback,
                ModifyConnectionSettings = x =>
                    x.BasicAuthentication(
                        elasticSearchUsername, elasticSearchPassword)

                //BufferCleanPayload = BufferCleanPayload,
                //TypeName = "aux-log"
                //TODO How does it work with proxy?
            };
        try
        {
            if (_failureLogCounter < maxFailureLogNumber)
            {
#pragma warning disable CS0618 // Type or member is obsolete
                elasticsearchSinkOptions.FailureSink = new Serilog.Sinks.File.FileSink(
                    Path.Combine(failureSinkLogDirectory,
                        defaultElasticSearchFailureLogFileName + logFileExtension),
                    new JsonFormatter(), null);
#pragma warning restore CS0618 // Type or member is obsolete
                _failureLogCounter++;
            }
        }
        catch (Exception)
        {
        }

        loggerConfiguration.WriteTo.Elasticsearch(elasticsearchSinkOptions);
    }
}
