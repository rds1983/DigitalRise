// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

using System;
using System.IO;


namespace DigitalRise.Storages
{
#pragma warning disable 1574

  // Workaround:
  // Using IStorage.OpenFile() in a VfsStorage may raise many FileNotFoundExceptions,
  // which may be reported as first chance exceptions. To avoid first chance exceptions
  // we internally use the TryOpenFile(string path).
  // Once all mobile platforms support some form of FileExists(), we can decide whether
  // to add TryOpenFile() or FileExists() to the API. For now, we keep the simple API.
  internal interface IStorageInternal
  {
    /// <summary>
    /// Opens the specified file for reading.
    /// </summary>
    /// <param name="path">The file to open.</param>
    /// <returns>
    /// A <see cref="Stream"/> object for reading the file. Returns <see langword="null"/> if file
    /// does not exist.
    /// </returns>
    Stream TryOpenFile(string path);
  }


  /// <summary>
  /// Provides access to files.
  /// </summary>
  /// <remarks>
  /// 
  /// <note type="important">
  /// <para>
  /// <see cref="DigitalRise.Storages"/> currently only provides read access to files! Full 
  /// read/write access, as well as support for additional platforms, will be added in a future
  /// version.
  /// </para>
  /// </note>
  /// 
  /// <para>
  /// The <see cref="IStorage"/> interface provides a common API to access files from different
  /// sources. The following implementations provide access to physical file systems on different
  /// platforms:
  /// </para>
  /// <list type="bullet">
  /// <item>
  /// <term><see cref="FileSystemStorage"/></term>
  /// <description>provides access to the file system of the operating system.</description>
  /// </item>
  /// <item>
  /// <term><see cref="T:DigitalRise.Storages.TitleStorage"/></term>
  /// <description>provides access to title storage on Xbox 360.</description>
  /// </item>
  /// <item>
  /// <term>(Not yet implemented) UserStorage</term>
  /// <description>provides access to user storage on Xbox 360.</description>
  /// </item>
  /// <item>
  /// <term>(Not yet implemented) IsolatedStorage</term>
  /// <description>provides access to isolated storage in Silverlight.</description>
  /// </item>
  /// <item>
  /// <term>(Not yet implemented) WindowsStorage</term>
  /// <description>provides access to storage folders in Windows Store apps.</description>
  /// </item>
  /// </list>
  /// <para>
  /// Some storages are built on top of other storages. For example:
  /// </para>
  /// <list type="bullet">
  /// <item>
  /// <term><see cref="VfsStorage"/></term>
  /// <description>
  /// maps existing storages into a virtual file system. Different storage devices and locations can
  /// be treated as one directory hierarchy.
  /// </description>
  /// </item>
  /// <item>
  /// <term><see cref="ZipStorage"/></term>
  /// <description>
  /// provides access to files stored in a ZIP archive. The ZIP archive can be read from any of the
  /// existing storages.
  /// </description>
  /// </item>
  /// </list>
  /// 
  /// <para>
  /// <strong>Case-Sensitivity:</strong><br/>
  /// File retrieval is case-sensitive if the storage provider (e.g. the platform OS) is
  /// case-sensitive. It is recommended to assume case-sensitivity to ensure that applications can
  /// be ported to non-Windows platforms.
  /// </para>
  /// <para>
  /// <strong>Directory Separator:</strong><br/>
  /// Storages accepts '\' and '/' as directory separators. Internally, paths are normalized to use
  /// '/'.
  /// </para>
  /// 
  /// <para>
  /// <strong>Possible Extensions:</strong><br/>
  /// The <see cref="IStorage"/> concept is highly extensible. Developers can provide custom 
  /// <see cref="IStorage"/> implementations to add support for new platforms or manipulate existing
  /// storages. Here are just a few features that could be implemented on top of 
  /// <see cref="IStorage"/>:
  /// </para>
  /// <list type="bullet">
  /// <item>
  /// <term>Access control</term>
  /// <description>
  /// A storage may wrap another storage and implement access control to restrict user access or
  /// filter certain files. For example, a "ReadOnlyStorage" may prevent write access to an existing
  /// location.
  /// </description>
  /// </item>
  /// <item>
  /// <term>Archives</term>
  /// <description>
  /// Storages can be added to support other package formats, such as 7-Zip, BZIP2, PAK.
  /// </description>
  /// </item>
  /// <item>
  /// <term>Caching ("CachedStorage")</term>
  /// <description>
  /// Files from another storage could be cached in memory for faster access.
  /// </description>
  /// </item>
  /// <item>
  /// <term>Cloud storage</term>
  /// <description>A storage may access data in the cloud, such as OneDrive.</description>
  /// </item>
  /// <item>
  /// <term>Encryption ("EncryptedStorage")</term>
  /// <description>
  /// Data could be encrypted and decrypted when accessing files in an existing storage.
  /// </description>
  /// </item>
  /// <item>
  /// <term>Mapping ("RedirectStorage")</term>
  /// <description>Directories can transparently be mapped to different location.</description>
  /// </item>
  /// <item>
  /// <term>Redundancy ("MirroredStorage")</term>
  /// <description>File access could be mirrored across different storages.</description>
  /// </item>
  /// <item>
  /// <term>Ad-hoc storage</term>
  /// <description>
  /// Instead of accessing files on an existing devices or locations, files can be stored in custom
  /// data structures (DBs, BLOBs, ...).
  /// </description>
  /// </item>
  /// </list>
  /// </remarks>
  public interface IStorage : IDisposable
#pragma warning restore 1574
  {
    // Implementation details:
    // The interface IStorage provides all fundamental file operations.
    // The StorageHelper provides extensions method for derived operations.
    // The abstract base class Storage provides virtual methods for derived methods
    // in case a derived type contains a more efficient implementation.

    // Example:
    // CopyDirectory() is not a fundamental operation and not directly supported by
    // all storage providers.
    // StorageHelper provides an extension method for CopyDirectory() which is built
    // on the methods in IStorage. It checks whether the storage provider is derived
    // from Storage, in which case Storage.CopyDirectory() is called.
    // The base class Storage provides a virtual method CopyDirectory(). The base
    // implementation calls the StorageHelper.CopyDirectoryInternal() method.
    // Derived storage providers can override CopyDirectory() if a more efficient
    // implementation exists.


    /// <summary>
    /// Gets the real path and name of the specified file.
    /// </summary>
    /// <param name="path">The file to check.</param>
    /// <returns>
    /// The path where the specified file is located. If the file is located inside an archive, the
    /// path and name of the archive is returned; otherwise, <see langword="null"/> if the file was
    /// not found.
    /// </returns>
    /// <remarks>
    /// <para>
    /// Storages, such as the <see cref="VfsStorage"/>, can be used to virtualize access to
    /// resources. File and directories from different location can be mapped into a virtual
    /// directory hierarchy. The method <see cref="GetRealPath"/> can be used to resolve the actual
    /// source of a file.
    /// </para>
    /// <para>
    /// This method can only be used to query for files, but not directories. (Multiple directories
    /// may be mapped to the same virtual path.)
    /// </para>
    /// <note type="warning">
    /// <para>
    /// Some storages hide the actual file location and may return <see langword="null"/> even if
    /// the file exists. The files inside the storage can still be opened with <see cref="OpenFile"/>
    /// but the real location is concealed. Therefore, <see cref="GetRealPath"/> cannot be used to
    /// check if a file exists inside a storage.
    /// </para>
    /// </note>
    /// </remarks>
    string GetRealPath(string path);


    /// <summary>
    /// Opens the specified file for reading.
    /// </summary>
    /// <param name="path">The file to open.</param>
    /// <returns>A <see cref="Stream"/> object for reading the file.</returns>
    Stream OpenFile(string path);
  }
}
