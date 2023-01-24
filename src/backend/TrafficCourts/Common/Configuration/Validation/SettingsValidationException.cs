using System.Configuration;

namespace TrafficCourts.Common.Configuration.Validation;

/// <summary>
/// A utility <see cref="Exception"/> that indicates a strong typed configuration model was not configured correctly
/// </summary>
public class SettingsValidationException : ConfigurationErrorsException
{
    private const string ErrorMessageSuffix = "Check that your configuration has been loaded correctly, and all necessary values are set in the configuration files.";

    public static SettingsValidationException RequiredSectionNotFound(string sectionName)
    {
        return new SettingsValidationException($"Required configuration section not found {sectionName}");
    }

    /// <summary>
    /// Creates a new instance of the <see cref="SettingsValidationException"/>
    /// </summary>
    /// <param name="className">The name of the class being validated</param>
    /// <param name="propertyName">The property of the instance that was invalid</param>
    /// <param name="error">A description of the configuration error</param>
    public SettingsValidationException(string className, string propertyName, string error)
        : this(GetMessage(className, propertyName, error))
    {
    }

    private static string GetMessage(string className, string propertyName, string message)
    {
        return $@"Settings were invalid: {className}.{propertyName} {message}. {ErrorMessageSuffix}";
    }

    /// <summary>
    /// Creates a new instance of the <see cref="SettingsValidationException"/>
    /// </summary>
    /// <param name="errorMessage">A description of the configuration error</param>
    private SettingsValidationException(string errorMessage) : base(errorMessage)
    {
    }
}