// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

using System;

namespace DigitalRune
{
  /// <exclude/>
  [Obsolete("Use GlobalSettings instead of Global. (Reason: The name Global may conflict with the global keyword in languages other than C#. Therefore the class Global has been renamed to GlobalSettings.)")]
  public static class Global
  {
    /// <inheritdoc cref="GlobalSettings.ValidationLevel"/>
    public static int ValidationLevel
    {
      get { return GlobalSettings.ValidationLevel; }
      set { GlobalSettings.ValidationLevel = value; }
    }
  }


  /// <summary>
  /// Defines global settings and information for all DigitalRune libraries.
  /// </summary>
  public static class GlobalSettings
  {
    internal const int ValidationLevelNone = 0;
    internal const int ValidationLevelUserHighCheap = 1 << 0;      // Relevant for customer, high priority, low performance impact.
    internal const int ValidationLevelUserHighExpensive = 1 << 1;  // Relevant for customer, high priority, medium performance impact.
    internal const int ValidationLevelUser = 0xff;                 // All checks which are relevant for customers.
    internal const int ValidationLevelDev = 0xff00;                // All checks which are relevant for the DigitalRune team during library development.
    internal const int ValidationLevelDevBasic = 1 << 8;           // Basic validation of algorithms.
    internal const int ValidationLevelDebug = 0xffff;              // All user and dev checks.


    /// <summary>
    /// Gets or sets the validation level for all DigitalRune libraries, used to enable additional
    /// input validation and other checks.
    /// </summary>
    /// <value>
    /// The validation level for all DigitalRune libraries.
    /// </value>
    /// <remarks>
    /// <para>
    /// The default validation level for release builds is 0. Setting a validation level greater
    /// than 0, enables additional checks in the DigitalRune libraries, e.g. more detailed input
    /// validation. These checks are usually turned off (using <see cref="ValidationLevel"/> = 0)
    /// to avoid a performance impact in release builds. During development and for debugging, 
    /// validation can be enabled. Set <see cref="ValidationLevel"/> to 0xff (=255) to enable
    /// all checks which are relevant for DigitalRune customers. Validation levels > 255 are 
    /// reserved for internal development.
    /// </para>
    /// </remarks>
    public static int ValidationLevel
    {
      get { return ValidationLevelInternal; }
      set { ValidationLevelInternal = value; }
    }
#if DEBUG
    internal static int ValidationLevelInternal = ValidationLevelDebug;
#else
    internal static int ValidationLevelInternal = ValidationLevelNone;
#endif
  }
}
