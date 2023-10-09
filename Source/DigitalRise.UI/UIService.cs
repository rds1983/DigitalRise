// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

using System;
using DigitalRise.Input;
using DigitalRise.UI.Controls;
using Keys = Microsoft.Xna.Framework.Input.Keys;

#if !MONOGAME
using MouseCursor = System.Nullable<System.IntPtr>;
#endif

namespace DigitalRise.UI
{
  /// <summary>
  /// Provides the ability to configure and control the graphical user interface.
  /// </summary>
  /// <remarks>
  /// <see cref="UIScreen"/>s can be added to the <see cref="Screens"/> collection. Screens are 
  /// automatically updated. But screens are not automatically drawn! 
  /// <see cref="UIScreen.Draw(TimeSpan)"/> must be called manually.
  /// </remarks>
  public interface IUIService
  {
    /// <summary>
    /// Gets or sets the mouse cursor, overriding the default mouse cursor.
    /// </summary>
    /// <value>
    /// The mouse cursor, overriding the current mouse cursor. Set   to <see langword="null"/> to 
    /// use the default mouse cursor. 
    /// </value>
    /// <remarks>
    /// <para>
    /// Normally, the control under the mouse determines the current mouse cursor. If this property
    /// is set, the automatically determined cursor is overridden. This is useful, for example,
    /// if a Wait cursor should displayed.
    /// </para>
    /// </remarks>
    MouseCursor Cursor { get; set; }


    /// <summary>
    /// Gets the input service.
    /// </summary>
    /// <value>The input service.</value>
    IInputService InputService { get; }


    /// <summary>
    /// Gets or sets the key map that translates <see cref="Keys"/> to characters.
    /// </summary>
    /// <value>
    /// The key map. The default is a key map that is automatically chosen based on the current
    /// culture settings.
    /// </value>
    KeyMap KeyMap { get; set; }


    /// <summary>
    /// Gets the <see cref="UIScreen"/>s.
    /// </summary>
    /// <value>The <see cref="UIScreen"/>s.</value>
    UIScreenCollection Screens { get; }
  }
}
