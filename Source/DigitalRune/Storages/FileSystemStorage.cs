// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

using System;
using System.IO;


namespace DigitalRune.Storages
{
  /// <summary>
  /// Provides access to the file system of the operating system.
  /// (Not supported on these platforms: Windows Phone, Windows Store, Xbox 360)
  /// </summary>
  /// <remarks>
  /// <para>
  /// This type is not available on the following platforms: Windows Phone, Windows Store, Xbox 360
  /// </para>
  /// All file access is relative to the root directory specified in the constructor.
  /// </remarks>
  public class FileSystemStorage : Storage, IStorageInternal
  {
    //--------------------------------------------------------------
    #region Fields
    //--------------------------------------------------------------
    #endregion


    //--------------------------------------------------------------
    #region Properties & Events
    //--------------------------------------------------------------

    /// <inheritdoc/>
    protected override char DirectorySeparator
    {
      get { return Path.DirectorySeparatorChar; }
    }


    /// <summary>
    /// Gets the absolute path to the root directory.
    /// </summary>
    /// <value>The absolute path to the root directory.</value>
    /// <remarks>
    /// All file access is relative to this root directory.
    /// </remarks>
    public string RootDirectory { get; private set; }
    #endregion


    //--------------------------------------------------------------
    #region Creation & Cleanup
    //--------------------------------------------------------------

    /// <overloads>
    /// <summary>
    /// Initializes a new instance of the <see cref="FileSystemStorage"/> class.
    /// </summary>
    /// </overloads>
    /// 
    /// <summary>
    /// Initializes a new instance of the <see cref="FileSystemStorage"/> class using the current
    /// working directory as the root directory.
    /// </summary>
    public FileSystemStorage()
      : this(null)
    {
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="FileSystemStorage"/> class using the specified
    /// directory as the root directory.
    /// </summary>
    /// <param name="rootDirectory">
    /// The root directory specified relative to the current working directory or as an absolute
    /// path. Can be <see langword="null"/> or "" to select the current working directory. The path
    /// is evaluated in the constructor, i.e. changing the current working directory later on does
    /// not affect the <see cref="FileSystemStorage"/>.
    /// </param>
    public FileSystemStorage(string rootDirectory)
    {
      if (string.IsNullOrEmpty(rootDirectory))
        RootDirectory = Directory.GetCurrentDirectory();
      else
        RootDirectory = Path.GetFullPath(rootDirectory);
    }
    #endregion


    //--------------------------------------------------------------
    #region Methods
    //--------------------------------------------------------------

    /// <inheritdoc/>
    public override string GetRealPath(string path)
    {
      path = Path.Combine(RootDirectory, path);
      if (File.Exists(path))
        return StorageHelper.SwitchDirectorySeparator(path, DirectorySeparator);

      return null;
    }


    /// <inheritdoc/>
    public override Stream OpenFile(string path)
    {
      path = Path.Combine(RootDirectory, path);
      return File.OpenRead(path);
    }


    /// <inheritdoc/>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
    Stream IStorageInternal.TryOpenFile(string path)
    {
      path = Path.Combine(RootDirectory, path);
      return File.Exists(path) ? File.OpenRead(path) : null;
    }
    #endregion
  }
}
