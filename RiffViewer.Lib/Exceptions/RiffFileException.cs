namespace RiffViewer.Lib.Exceptions;

/// <summary>
/// Exception thrown when there is an error with a RIFF file.
/// </summary>
public class RiffFileException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RiffFileException"/> class.
    /// </summary>
    public RiffFileException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RiffFileException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error</param>
    public RiffFileException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RiffFileException"/> class with a specified error message
    /// and a reference to the inner exception that is the cause of this exception
    /// </summary>
    /// <param name="message">The message that describes the error</param>
    /// <param name="inner">The exception that is the cause of the current exception</param> 
    public RiffFileException(string message, Exception inner) : base(message, inner)
    {
    }
}