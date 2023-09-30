// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

using System;
using System.Runtime.Serialization;


namespace DigitalRise.Geometry
{

  /// <summary>
  /// The exception that is thrown when an error in the geometry library occurs.
  /// </summary>
  [Serializable]
  public class GeometryException : Exception
  {
    [NonSerialized]
    private object _context;


    /// <summary>
    /// Gets an object that provides additional information.
    /// </summary>
    /// <value>An object that provides additional information.</value>
    public object Context
    {
      get { return _context; }
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="GeometryException"/> class.
    /// </summary>
    public GeometryException()
      : this(null)
    {
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="GeometryException"/> class.
    /// </summary>
    /// <param name="context">An object that provides additional information.</param>
    public GeometryException(object context)
    {
      _context = context;
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="GeometryException"/> class with a specified 
    /// error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public GeometryException(string message) 
      : this(message, null)
    {
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="GeometryException"/> class with a specified 
    /// error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="context">An object that provides additional information.</param>
    public GeometryException(string message, object context)
      : base(message)
    {
      _context = context;
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="GeometryException"/> class with a specified 
    /// error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">
    /// The exception that is the cause of the current exception, or <see langword="null"/> if no 
    /// inner exception is specified.
    /// </param>
    public GeometryException(string message, Exception innerException) 
      : this(message, innerException, null)
    {
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="GeometryException"/> class with a specified 
    /// error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">
    /// The exception that is the cause of the current exception, or <see langword="null"/> if no 
    /// inner exception is specified.
    /// </param>
    /// <param name="context">An object that provides additional information.</param>
    public GeometryException(string message, Exception innerException, object context)
      : base(message, innerException)
    {
      _context = context;
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="GeometryException"/> class with serialized
    /// data.
    /// </summary>
    /// <param name="info">
    /// The <see cref="SerializationInfo"/> that holds the serialized object data about the
    /// exception being thrown.
    /// </param>
    /// <param name="context">
    /// The <see cref="StreamingContext"/> that contains contextual information about the source or
    /// destination.
    /// </param>
    /// <exception cref="SerializationException">
    /// The class name is <see langword="null"/> or <see cref="Exception.HResult"/> is zero (0).
    /// </exception>
    /// <exception cref="ArgumentNullException">
    /// The info parameter is <see langword="null"/>.
    /// </exception>
    protected GeometryException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
