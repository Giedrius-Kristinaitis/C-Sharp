/*
 * Author: Giedrius Kristinaitis
 */

using System;

/// <summary>
/// Exception that occurs when there is a problem with a file the user created
/// </summary>
public class FileException : ApplicationException {

    // description about what caused the exception
    public string Cause { get; set; } 

    // a possible fix for the problem
    public string Fix { get; set; }

    /// <summary>
    /// Empty class constructor with a default message
    /// </summary>
    public FileException() : base("Blogas failo vardas arba formatas") { }

    /// <summary>
    /// Class constructor with custom exception message
    /// </summary>
    /// <param name="message">message to be passed to the parent class</param>
    public FileException(string message) : base(message) { }

    /// <summary>
    /// Class constructor with custom exception message and exception object
    /// </summary>
    /// <param name="message">message to be passed to the parent class</param>
    /// <param name="ex">custom exception to be passed to the parent class</param>
    public FileException(string message, Exception ex) : base(message, ex) { }

    /// <summary>
    /// Class constructor with custom exception message, cause of the exception and a fix
    /// </summary>
    /// <param name="message">message to be passed to the parent class</param>
    /// <param name="cause">what caused the exception</param>
    /// <param name="fix">a possible fix for the problem that is causing the exception</param>
    public FileException(string message, string cause, string fix) : base(message) {
        Cause = cause;
        Fix = fix;
    }

    /// <summary>
    /// Class constructor with custom exception message, cause of the exception, a fix
    /// and a custom exception object
    /// </summary>
    /// <param name="message">message to be passed to the parent class</param>
    /// <param name="cause">what caused the exception</param>
    /// <param name="fix">a possible fix for the problem that is causing the exception</param>
    /// <param name="ex">custom exception object to be passed to the parent class</param>
    public FileException(string message, string cause, string fix, Exception ex) : base(message, ex) {
        Cause = cause;
        Fix = fix;
    }
}
