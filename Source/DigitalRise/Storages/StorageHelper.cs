// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

using System;
using System.IO;


namespace DigitalRise.Storages
{
  /// <summary>
  /// Provides conversion and extension methods for <see cref="DigitalRise.Storages"/>.
  /// </summary>
  internal static class StorageHelper
  {
    /// <summary>
    /// Gets the application installation folder.
    /// </summary>
    /// <value>The application installation folder.</value>
    internal static string BaseLocation
    {
      get
      {
        return AppDomain.CurrentDomain.BaseDirectory;
      }
    }


    /// <summary>
    /// Switches the directory separator character in the specified path.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <param name="directorySeparator">The desired directory separator character.</param>
    /// <returns>The path using only the specified directory separator.</returns>
    internal static string SwitchDirectorySeparator(string path, char directorySeparator)
    {
      switch (directorySeparator)
      {
        case '/':
          path = path.Replace('\\', '/');
          break;
        case '\\':
          path = path.Replace('/', '\\');
          break;
        default:
          path = path.Replace('\\', directorySeparator);
          path = path.Replace('/', directorySeparator);
          break;
      }

      return path;
    }


    /// <summary>
    /// Validates the mount point and normalizes the path.
    /// </summary>
    /// <param name="path">The mount point.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">
    /// <paramref name="path"/> is invalid.
    /// </exception>
    internal static string NormalizeMountPoint(string path)
    {
      const string root = ""; // Path that represents the root directory.
      const string message = "Invalid mount point. Mount point needs to be specified relative to root directory of the virtual files system in canonical form.";

      // null or "" are valid mount points, same as '/'.
      if (String.IsNullOrEmpty(path))
        return root;

      // Paths with "pathA/pathB/../pathC" are not supported.
      if (path.Contains(".."))
        throw new ArgumentException(message, "path");

      // Switch to forward slashes '/'.
      path = path.Replace('\\', '/');

      // Reduce "./path" to "path".
      while (path.StartsWith("./", StringComparison.Ordinal))
        path = path.Substring(2);

      // Reduce "pathA/./pathB" to "pathA/pathB".
      path = path.Replace("/./", "/");

      if (path.Length == 0)
        return root;

      // Trim leading '/'. All mount points are relative to root directory!
      if (path[0] == '/')
        path = path.Substring(1);

      if (path.Length == 0)
        return root;

      // Rooted path, such as "C:\path" or "\\server\path", are not supported.
      if (Path.IsPathRooted(path))
        throw new ArgumentException(message, "path");

      // Trim trailing '/'.
      while (path.Length > 0 && path[path.Length - 1] == '/')
        path = path.Substring(0, path.Length - 1);

      if (path.Length == 0)
        return root;

      return path;
    }


    /// <summary>
    /// Validates and normalizes the path of a file in a storage.
    /// </summary>
    /// <param name="path">The path the file.</param>
    /// <returns>The normalized path.</returns>
    internal static string NormalizePath(string path)
    {
      const string message = "Invalid path. The path needs to be specified relative to the root directory in canonical form.";

      if (path == null)
        throw new ArgumentNullException("path");
      if (path.Length == 0)
        throw new ArgumentException("Invalid path. Path must not be empty.", "path");

      // Paths with "pathA/pathB/../pathC" are not supported.
      if (path.Contains(".."))
        throw new ArgumentException(message, "path");

      // Switch to forward slashes '/'.
      path = SwitchDirectorySeparator(path, '/');

      // Reduce "./path" to "path".
      while (path.StartsWith("./", StringComparison.Ordinal))
        path = path.Substring(2);

      // Reduce "pathA/./pathB" to "pathA/pathB".
      path = path.Replace("/./", "/");

      if (path.Length == 0)
        throw new ArgumentException(message, "path");

      // Trim leading '/'. All mount points are relative to root directory!
      if (path[0] == '/')
        path = path.Substring(1);

      if (path.Length == 0)
        throw new ArgumentException(message, "path");

      // Absolute paths are not supported.
      if (Path.IsPathRooted(path))
        throw new ArgumentException(message, "path");

      // Trim trailing '/'.
      while (path.Length > 0 && path[path.Length - 1] == '/')
        path = path.Substring(0, path.Length - 1);

      if (path.Length == 0)
        throw new ArgumentException(message, "path");

      return path;
    }
  }
}
