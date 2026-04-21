namespace GexPlatform.Infrastructure.Exceptions;

/// <summary>
/// Exception thrown when there's an error fetching or parsing options data.
/// </summary>
public class OptionsDataException : Exception
{
    /// <summary>
    /// Initializes a new instance of the OptionsDataException class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public OptionsDataException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the OptionsDataException class with an inner exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public OptionsDataException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
