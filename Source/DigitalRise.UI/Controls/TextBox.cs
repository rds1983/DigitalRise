// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using DigitalRise.GameBase;
using DigitalRise.Input;
using DigitalRise.Mathematics;
using Microsoft.Xna.Framework;
using Keys = Microsoft.Xna.Framework.Input.Keys;
using MathHelper = DigitalRise.Mathematics.MathHelper;
using MouseButtons = DigitalRise.Input.MouseButtons;


namespace DigitalRise.UI.Controls
{
	/// <summary>
	/// Represents a control that can be used to display or edit unformatted text.
	/// </summary>
	/// <remarks>
	/// The text box can be a multi-line text box (if <see cref="MinLines"/> or <see cref="MaxLines"/>
	/// is greater than 1). It supports vertical scrolling but not horizontal scrolling. On Windows, 
	/// text can be entered using the keyboard. On Windows Phone 7, the software keyboard is opened 
	/// when the text box is clicked. On Xbox 360, the software keyboard is opened when the text box
	/// is focused and A on the gamepad is clicked.
	/// </remarks>
	/// <example>
	/// The following example creates a multi-line text box with a context menu.
	/// <code lang="csharp">
	/// <![CDATA[
	/// // Create a multi-line text box.
	/// var textBox = new TextBox
	/// {
	///   Margin = new Vector4(4),
	///   Text = "Lorem ipsum dolor sit ...",
	///   MaxLines = 5,   // Show max 5 lines of text.
	///   HorizontalAlignment = HorizontalAlignment.Stretch,
	/// };
	/// 
	/// // Add a context menu (Cut, Copy, Paste) to the text box.
	/// var contextMenu = new ContextMenu();
	/// var cut = new MenuItem { Content = new TextBlock { Text = "Cut" } };
	/// var copy = new MenuItem { Content = new TextBlock { Text = "Copy" } };
	/// var paste = new MenuItem { Content = new TextBlock { Text = "Paste" } };
	/// cut.Click += (s, e) => textBox.Cut();
	/// copy.Click += (s, e) => textBox.Copy();
	/// paste.Click += (s, e) => textBox.Paste();
	/// contextMenu.Items.Add(cut);
	/// contextMenu.Items.Add(copy);
	/// contextMenu.Items.Add(paste);
	/// textBox.ContextMenu = contextMenu;
	/// 
	/// // To show the text box, add it to an existing content control or panel.
	/// panel.Children.Add(textBox);
	/// ]]>
	/// </code>
	/// </example>
	public partial class TextBox : UIControl
	{
		//--------------------------------------------------------------
		#region Fields
		//--------------------------------------------------------------

		// For platforms which do not have an OS clipboard mechanism:
		// Create a clipboard replacement which is shared by all text boxes and the console.
		internal static string ClipboardData;

		// Visual children.
		private ScrollBar _verticalScrollBar;

		// The indices in Text where new lines start. Only used for multi-line text box.
		// A number can be negative to indicate that the line ends because of a \n by the user
		// and not because of text wrapping. (When the number is positive, a \n must be inserted 
		// for text wrapping.)
		private List<int> _lineStarts;

		// True if the caret should be brought into the view. This flag is set when the 
		// CaretIndex is changed.
		private bool _bringCaretIntoView;

		// Text selection goes from _selectionStart to CaretIndex.
		// (-1 indicates that nothing is selected.)
		private int _selectionStart;

		// Mouse interactions.
		private const float MinDragDistanceSquared = 4;
		private Vector2 _mouseDownPosition;
		private bool _isDraggingSelection;
		private bool _isPressed;

		#endregion


		//--------------------------------------------------------------
		#region Properties & Events
		//--------------------------------------------------------------

		/// <summary>
		/// Gets or sets the position of the caret.
		/// </summary>
		/// <value>The position of the caret (zero-based index).</value>
		public int CaretIndex
		{
			get { return _caretIndex; }
			set
			{
				if (_caretIndex == value)
					return;

				_caretIndex = value;

				// Limit cursor position to allowed range.
				if (_caretIndex < 0)
				{
					_caretIndex = 0;
				}
				else
				{
					string text = Text ?? string.Empty;
					if (_caretIndex > text.Length)
						_caretIndex = text.Length;
				}

				// When the cursor is moved, the text box should scroll to the cursor position.
				_bringCaretIntoView = true;

				InvalidateArrange();
			}
		}
		private int _caretIndex;


		/// <summary>
		/// Gets a value indicating whether this text box is a multi-line text box that accepts ENTER
		/// keys to create new lines.
		/// </summary>
		/// <value>
		/// <see langword="true"/> if this text box is multi-line text box that accepts ENTER keys to
		/// create new lines; otherwise, <see langword="false"/>.
		/// </value>
		/// <remarks>
		/// A text box is a multi-line text box if <see cref="MaxLines"/> is greater than 1.
		/// </remarks>
		public bool IsMultiline { get { return MinLines > 1 || MaxLines > 1; } }


		/// <summary>
		/// Gets the content of the current selection in the text box.
		/// </summary>
		/// <value>
		/// The content of the current selection in the text box. Returns <see cref="string.Empty"/> if
		/// nothing is selected.
		/// </value>
		public string SelectedText
		{
			get
			{
				if (_selectionStart < 0)
					return string.Empty;

				string text = Text ?? string.Empty;

				// Sort selection indices.
				int start, end;
				if (_selectionStart < _caretIndex)
				{
					start = _selectionStart;
					end = _caretIndex;
				}
				else
				{
					start = _caretIndex;
					end = _selectionStart;
				}

				return text.Substring(start, end - start);
			}
		}
		#endregion


		//--------------------------------------------------------------
		#region Game Object Properties & Events
		//--------------------------------------------------------------

		/// <summary> 
		/// The ID of the <see cref="Text"/> game object property.
		/// </summary>
		[Browsable(false)]
		public static readonly GamePropertyInfo<string> TextProperty = CreateProperty(
			typeof(TextBox), "Text", GamePropertyCategories.Common, null, string.Empty,
			UIPropertyOptions.AffectsMeasure);

		/// <summary>
		/// Gets or sets the text. 
		/// This is a game object property.
		/// </summary>
		/// <value>The text.</value>
		public string Text
		{
			get => TextProperty.GetValue(this);
			set => TextProperty.SetValue(this, value);
		}


		/// <summary> 
		/// The ID of the <see cref="GuideTitle"/> game object property.
		/// </summary>
		[Browsable(false)]
		public static readonly GamePropertyInfo<string> GuideTitleProperty = CreateProperty(
			typeof(TextBox), "GuideTitle", GamePropertyCategories.Default, null, (string)null,
			UIPropertyOptions.None);

		/// <summary>
		/// Gets or sets the title that is displayed with the software keyboard.
		/// This is a game object property.
		/// </summary>
		/// <value>The title that is displayed with the software keyboard.</value>
		public string GuideTitle
		{
			get => GuideTitleProperty.GetValue(this);
			set => GuideTitleProperty.SetValue(this, value);
		}


		/// <summary> 
		/// The ID of the <see cref="GuideDescription"/> game object property.
		/// </summary>
		[Browsable(false)]
		public static readonly GamePropertyInfo<string> GuideDescriptionProperty = CreateProperty(
			typeof(TextBox), "GuideDescription", GamePropertyCategories.Default, null, (string)null,
			UIPropertyOptions.None);

		/// <summary>
		/// Gets or sets the description that is displayed with the software keyboard.
		/// This is a game object property.
		/// </summary>
		/// <value>The description that is displayed with the software keyboard.</value>
		public string GuideDescription
		{
			get => GuideDescriptionProperty.GetValue(this);
			set => GuideDescriptionProperty.SetValue(this, value);
		}



		/// <summary> 
		/// The ID of the <see cref="IsReadOnly"/> game object property.
		/// </summary>
		[Browsable(false)]
		public static readonly GamePropertyInfo<bool> IsReadOnlyProperty = CreateProperty(
			typeof(TextBox), "IsReadOnly", GamePropertyCategories.Behavior, null, false,
			UIPropertyOptions.AffectsArrange);

		/// <summary>
		/// Gets or sets a value indicating whether this text box is read-only. 
		/// This is a game object property.
		/// </summary>
		/// <value>
		/// <see langword="true"/> if this text box is read-only; otherwise, <see langword="false"/>.
		/// </value>
		[Browsable(false)]
		public bool IsReadOnly
		{
			get => IsReadOnlyProperty.GetValue(this);
			set => IsReadOnlyProperty.SetValue(this, value);
		}


		/// <summary> 
		/// The ID of the <see cref="IsPassword"/> game object property.
		/// </summary>
		[Browsable(false)]
		public static readonly GamePropertyInfo<bool> IsPasswordProperty = CreateProperty(
			typeof(TextBox), "IsPassword", GamePropertyCategories.Behavior, null, false,
			UIPropertyOptions.AffectsMeasure);

		/// <summary>
		/// Gets or sets a value indicating whether this text box is a password box that displays
		/// <see cref="PasswordCharacter"/>s instead of the real characters. This is a game object 
		/// property.
		/// </summary>
		/// <value>
		/// <see langword="true"/> if this text box is a password box that displays
		/// <see cref="PasswordCharacter"/>s instead of the real characters; otherwise, 
		/// <see langword="false"/>. Only single-line text boxes (where <see cref="MaxLines"/> is 1) can
		/// be password boxes.
		/// </value>
		public bool IsPassword
		{
			get => IsPasswordProperty.GetValue(this);
			set => IsPasswordProperty.SetValue(this, value);
		}


		/// <summary> 
		/// The ID of the <see cref="PasswordCharacter"/> game object property.
		/// </summary>
		[Browsable(false)]
		public static readonly GamePropertyInfo<char> PasswordCharacterProperty = CreateProperty(
			typeof(TextBox), "PasswordCharacter", GamePropertyCategories.Behavior, null, '•',
			UIPropertyOptions.AffectsMeasure);

		/// <summary>
		/// Gets or sets the character that is used instead of normal character if 
		/// <see cref="IsPassword"/> is set. This is a game object property.
		/// </summary>
		/// <value>
		/// The character that is used instead of normal character if <see cref="IsPassword"/> is set.
		/// </value>
		/// <remarks>
		/// Note: The password character must be included in the used SpriteFontBase (see 
		/// <see cref="UIControl.Font"/>)!
		/// </remarks>
		public char PasswordCharacter
		{
			get => PasswordCharacterProperty.GetValue(this);
			set => PasswordCharacterProperty.SetValue(this, value);
		}


		/// <summary> 
		/// The ID of the <see cref="MaxLength"/> game object property.
		/// </summary>
		[Browsable(false)]
		public static readonly GamePropertyInfo<int> MaxLengthProperty = CreateProperty(
			typeof(TextBox), "MaxLength", GamePropertyCategories.Default, null, int.MaxValue,
			UIPropertyOptions.None);

		/// <summary>
		/// Gets or sets the maximal number of characters in the <see cref="Text"/>. 
		/// This is a game object property.
		/// </summary>
		/// <value>The maximal number of characters in the <see cref="Text"/>.</value>
		public int MaxLength
		{
			get => MaxLengthProperty.GetValue(this);
			set => MaxLengthProperty.SetValue(this, value);
		}


		/// <summary> 
		/// The ID of the <see cref="MinLines"/> game object property.
		/// </summary>
		[Browsable(false)]
		public static readonly GamePropertyInfo<int> MinLinesProperty = CreateProperty(
			typeof(TextBox), "MinLines", GamePropertyCategories.Default, null, 1,
			UIPropertyOptions.AffectsMeasure);

		/// <summary>
		/// Gets or sets the minimal number of visible lines. 
		/// This is a game object property.
		/// </summary>
		/// <value>The minimal number of visible lines.</value>
		public int MinLines
		{
			get => MinLinesProperty.GetValue(this);
			set => MinLinesProperty.SetValue(this, value);
		}


		/// <summary> 
		/// The ID of the <see cref="MaxLines"/> game object property.
		/// </summary>
		[Browsable(false)]
		public static readonly GamePropertyInfo<int> MaxLinesProperty = CreateProperty(
			typeof(TextBox), "MaxLines", GamePropertyCategories.Default, null, int.MaxValue,
			UIPropertyOptions.AffectsMeasure);

		/// <summary>
		/// Gets or sets the maximal number of visible lines. 
		/// This is a game object property.
		/// </summary>
		/// <value>
		/// The maximal number of visible lines. The default is 1. If this property is greater than 1,
		/// the text box is a multi-line text box (<see cref="IsMultiline"/>).
		/// </value>
		public int MaxLines
		{
			get => MaxLinesProperty.GetValue(this);
			set => MaxLinesProperty.SetValue(this, value);
		}


		/// <summary> 
		/// The ID of the <see cref="SelectionColor"/> game object property.
		/// </summary>
		[Browsable(false)]
		public static readonly GamePropertyInfo<Color> SelectionColorProperty = CreateProperty(
			typeof(TextBox), "SelectionColor", GamePropertyCategories.Appearance, null,
			new Color(51, 153, 255, 168), UIPropertyOptions.None);

		/// <summary>
		/// Gets or sets the background color used for text selections. 
		/// This is a game object property.
		/// </summary>
		/// <value>
		/// The background color used for text selections.
		/// </value>
		public Color SelectionColor
		{
			get => SelectionColorProperty.GetValue(this);
			set => SelectionColorProperty.SetValue(this, value);
		}


		/// <summary> 
		/// The ID of the <see cref="VerticalScrollBarVisibility"/> game object property.
		/// </summary>
		[Browsable(false)]
		public static readonly GamePropertyInfo<ScrollBarVisibility> VerticalScrollBarVisibilityProperty = CreateProperty(
			typeof(TextBox), "VerticalScrollBarVisibility", GamePropertyCategories.Behavior, null,
			ScrollBarVisibility.Auto, UIPropertyOptions.AffectsMeasure);

		/// <summary>
		/// Gets or sets the <see cref="ScrollBarVisibility"/> of the vertical <see cref="ScrollBar"/>. 
		/// This is a game object property.
		/// </summary>
		/// <value>
		/// The <see cref="ScrollBarVisibility"/> of the vertical <see cref="ScrollBar"/>.
		/// </value>
		public ScrollBarVisibility VerticalScrollBarVisibility
		{
			get => VerticalScrollBarVisibilityProperty.GetValue(this);
			set => VerticalScrollBarVisibilityProperty.SetValue(this, value);
		}


		/// <summary> 
		/// The ID of the <see cref="VerticalScrollBarStyle"/> game object property.
		/// </summary>
		[Browsable(false)]
		public static readonly GamePropertyInfo<string> VerticalScrollBarStyleProperty = CreateProperty(
			typeof(TextBox), "VerticalScrollBarStyle", GamePropertyCategories.Style, null,
			"ScrollBarVertical", UIPropertyOptions.None);

		/// <summary>
		/// Gets or sets the style that is applied to the vertical <see cref="ScrollBar"/>. 
		/// This is a game object property.
		/// </summary>
		/// <value>
		/// The style that is applied to the vertical <see cref="ScrollBar"/>. Can be 
		/// <see langword="null"/> or an empty string to hide the scroll bar.
		/// </value>
		public string VerticalScrollBarStyle
		{
			get => VerticalScrollBarStyleProperty.GetValue(this);
			set => VerticalScrollBarStyleProperty.SetValue(this, value);
		}
		#endregion


		//--------------------------------------------------------------
		#region Creation & Cleanup
		//--------------------------------------------------------------

		/// <summary>
		/// Initializes static members of the <see cref="TextBox"/> class.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
		static TextBox()
		{
			// Focusable by default.
			OverrideDefaultValue(typeof(TextBox), FocusableProperty.Id, true);
			OverrideDefaultValue(typeof(TextBox), MaxLinesProperty.Id, 1);
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="TextBox"/> class.
		/// </summary>
		public TextBox()
		{
			Style = "TextBox";
			VisualText = new StringBuilder();

			_selectionStart = -1;
			VisualSelectionBounds = new List<RectangleF>();

			// When IsReadOnly is changed, we must update the cursor because the I beam 
			// cursor should not be used over read-only text boxes.
			var isReadOnlyProperty = IsReadOnlyProperty.Get(this);
			isReadOnlyProperty.Changed += (s, e) => UpdateCursor();

			// Clear selection when text is updated.
			var textProperty = TextProperty.Get(this);
			textProperty.Changed += (s, e) => ClearSelection();
		}
		#endregion


		//--------------------------------------------------------------
		#region Methods
		//--------------------------------------------------------------

		/// <inheritdoc/>
		protected override void OnLoad()
		{
			base.OnLoad();

			// If the control is writable, it uses an I beam cursor; otherwise, the default cursor.
			UpdateCursor();

			var verticalScrollBarStyle = VerticalScrollBarStyle;
			if (!string.IsNullOrEmpty(verticalScrollBarStyle))
			{
				_verticalScrollBar = new ScrollBar
				{
					Style = verticalScrollBarStyle,
				};
				VisualChildren.Add(_verticalScrollBar);

				if (Numeric.IsNaN(_verticalScrollBar.Width))
					_verticalScrollBar.Width = 16;

				// The scroll bar value changes the VisualOffset value.
				var scrollBarValue = RangeBase.ValueProperty.Get(_verticalScrollBar);
				scrollBarValue.Changed += (s, e) => VisualOffset = _verticalScrollBar.Value;

				// The I beam cursor should not be visible over the scroll bar. Therefore, 
				// the scroll bar sets its desired cursor to the default cursor to override 
				// the I beam of the text box.
				_verticalScrollBar.Cursor = Screen.Renderer.GetCursor(null);
			}
		}


		/// <inheritdoc/>
		protected override void OnUnload()
		{
			VisualChildren.Remove(_verticalScrollBar);
			_verticalScrollBar = null;

			base.OnUnload();
		}


		private void UpdateCursor()
		{
			Cursor = (IsReadOnly || Screen == null) ? null : Screen.Renderer.GetCursor("IBeam");
		}


		/// <inheritdoc/>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods")]
		protected override void OnHandleInput(InputContext context)
		{
			ContinueDraggingSelection(context);

			base.OnHandleInput(context);

			if (!IsLoaded)
				return;

			var inputService = InputService;

#if !ANDROID && !IOS
			var screen = Screen;
			bool isMouseOver = IsMouseOver;
			if (isMouseOver && !inputService.IsMouseOrTouchHandled)
			{
				if (inputService.IsDoubleClick(MouseButtons.Left)
						&& !_mouseDownPosition.IsNaN()
						&& (_mouseDownPosition - context.MousePosition).LengthSquared() < MinDragDistanceSquared)
				{
					// Double-click with left mouse button --> Select word or white-space.
					inputService.IsMouseOrTouchHandled = true;
					int index = GetIndex(context.MousePosition, screen);
					SelectWordOrWhiteSpace(index);
					StartDraggingSelection(context);
				}
				else if (inputService.IsPressed(MouseButtons.Left, false))
				{
					// Left mouse button pressed --> Position caret.
					inputService.IsMouseOrTouchHandled = true;
					int index = GetIndex(context.MousePosition, screen);
					_selectionStart = index;
					CaretIndex = index;
					StartDraggingSelection(context);
				}
				else
				{
					// Check for other mouse interactions.
					_isDraggingSelection = false;
					if (inputService.MouseWheelDelta != 0 && IsMultiline)
					{
						// Mouse wheel over the text box --> Scroll vertically.
						inputService.IsMouseOrTouchHandled = true;

						float delta = inputService.MouseWheelDelta / screen.MouseWheelScrollDelta * screen.MouseWheelScrollLines;
						delta *= _verticalScrollBar.SmallChange;
						VisualOffset = MathHelper.Clamp(VisualOffset - delta, 0, _verticalScrollBar.Maximum);
						_verticalScrollBar.Value = VisualOffset;
						InvalidateArrange();
					}
				}
			}

			if (!_isDraggingSelection && IsFocusWithin)
				HandleKeyboardInput();
#else
      {
        // Windows phone: The guide is shown when the touch is released over the box.
        bool isMouseOver = IsMouseOver;
        if (inputService.IsMouseOrTouchHandled)
        {
          _isPressed = false;
        }
        else if (_isPressed && isMouseOver && inputService.IsReleased(MouseButtons.Left))
        {
          ShowGuide(inputService.GetLogicalPlayer(context.AllowedPlayer));
          inputService.IsMouseOrTouchHandled = true;
          _isPressed = false;
        }
        else if (_isPressed && (!isMouseOver || inputService.IsUp(MouseButtons.Left)))
        {
          _isPressed = false;
        }
        else if (isMouseOver && inputService.IsPressed(MouseButtons.Left, false))
        {
          _isPressed = true;
          inputService.IsMouseOrTouchHandled = true;
        }
      }
#endif
		}


		private void StartDraggingSelection(InputContext context)
		{
			if (_selectionStart < 0)
			{
				// Start index of selection is not set.
				return;
			}

			_isDraggingSelection = true;
			_mouseDownPosition = context.MousePosition;
		}


		private void ContinueDraggingSelection(InputContext context)
		{
			if (!_isDraggingSelection)
				return;

			if (!IsLoaded)
				return;

			var screen = Screen;
			var inputService = InputService;

			// The user is dragging the caret (end of selection).
			if (!inputService.IsMouseOrTouchHandled)
			{
				inputService.IsMouseOrTouchHandled = true;
				if (inputService.IsDown(MouseButtons.Left))
				{
					// Only update the caret position if the mouse has moved.
					// (This check is necessary because we don't want to clear a selection
					// created by a double-click.)
					if ((_mouseDownPosition - context.MousePosition).LengthSquared() > MinDragDistanceSquared)
					{
						// Update the caret index (= end of selection).
						CaretIndex = GetIndex(context.MousePosition, screen);
					}
				}
				else
				{
					// The user has released the mouse button.
					StopDraggingSelection();
				}
			}
			else
			{
				// Mouse input has been intercepted.
				StopDraggingSelection();
			}
		}


		private void StopDraggingSelection()
		{
			_isDraggingSelection = false;

			// Clear selection if empty.
			if (_selectionStart == CaretIndex)
			{
				_selectionStart = -1;
				InvalidateArrange();
			}
		}


		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
		private void HandleKeyboardInput()
		{
			var screen = Screen;
			var inputService = InputService;

			if (inputService.IsKeyboardHandled || inputService.PressedKeys.Count <= 0)
				return;

			// Coerce caret index. (This can be necessary if the TextBox.Text property 
			// was changed by the user directly.)
			string text = Text ?? string.Empty;
			if (CaretIndex > text.Length)
				CaretIndex = text.Length;

			// Check for keyboard shortcuts.
			if ((inputService.ModifierKeys & ModifierKeys.Control) == ModifierKeys.Control)
			{
				if (inputService.IsPressed(Keys.A, true))
				{
					// ----- Select All
					inputService.IsKeyboardHandled = true;
					SelectAll();
				}
				if (inputService.IsPressed(Keys.V, true))
				{
					// ----- Paste
					inputService.IsKeyboardHandled = true;
					Paste();
				}
				else if (inputService.IsPressed(Keys.C, true))
				{
					// ----- Copy
					inputService.IsKeyboardHandled = true;
					Copy();
				}
				else if (inputService.IsPressed(Keys.X, true))
				{
					// ----- Cut
					inputService.IsKeyboardHandled = true;
					Cut();
				}
			}

			// ----- Backspace
			if (inputService.IsPressed(Keys.Back, true))
			{
				inputService.IsKeyboardHandled = true;
				Backspace();
			}

			// ----- Delete
			if (inputService.IsPressed(Keys.Delete, true))
			{
				inputService.IsKeyboardHandled = true;
				Delete();
			}

			// ----- Left
			if (inputService.IsPressed(Keys.Left, true))
			{
				if (_caretIndex != 0)                               // Don't handle key to allow control focus change.
				{
					inputService.IsKeyboardHandled = true;
					MoveLeft(inputService.ModifierKeys);
				}
			}

			// ----- Right
			if (inputService.IsPressed(Keys.Right, true))
			{
				if (_caretIndex != text.Length)                     // Don't handle key to allow control focus change.
				{
					inputService.IsKeyboardHandled = true;
					MoveRight(inputService.ModifierKeys);
				}
			}

			// ----- Up
			if (IsMultiline && inputService.IsPressed(Keys.Up, true))
			{
				if (GetLine(_caretIndex) != 0)                      // Don't handle key to allow control focus change.
				{
					inputService.IsKeyboardHandled = true;
					MoveUp(inputService.ModifierKeys);
				}
			}

			// ----- Down
			if (IsMultiline && inputService.IsPressed(Keys.Down, true))
			{
				if (GetLine(_caretIndex) != GetLine(text.Length))   // Don't handle key to allow control focus change.
				{
					inputService.IsKeyboardHandled = true;
					MoveDown(inputService.ModifierKeys);
				}
			}

			// ----- Home
			if (inputService.IsPressed(Keys.Home, true))
			{
				inputService.IsKeyboardHandled = true;
				Home(inputService.ModifierKeys);
			}

			// ----- End
			if (inputService.IsPressed(Keys.End, true))
			{
				inputService.IsKeyboardHandled = true;
				End(inputService.ModifierKeys);
			}

			// ----- PageUp
			if (IsMultiline && inputService.IsPressed(Keys.PageUp, true))
			{
				inputService.IsKeyboardHandled = true;
				PageUp(inputService.ModifierKeys);
			}

			// ----- PageDown
			if (IsMultiline && inputService.IsPressed(Keys.PageDown, true))
			{
				inputService.IsKeyboardHandled = true;
				PageDown(inputService.ModifierKeys);
			}

			// ----- Enter
			if (IsMultiline && inputService.IsPressed(Keys.Enter, true))
			{
				inputService.IsKeyboardHandled = true;
				Enter();
			}

			// Other keys enter text if they have a readable character equivalent in the 
			// current key map.
			var modifierKeys = inputService.ModifierKeys;
			var uiService = screen.UIService;
			for (int i = 0; i < inputService.PressedKeys.Count; i++)
			{
				var key = inputService.PressedKeys[i];
				text = Text ?? string.Empty;
				if (text.Length >= MaxLength)
					break;

				char c = uiService.KeyMap[key, modifierKeys];
				if (c == 0 || char.IsControl(c))
					continue;

				inputService.IsKeyboardHandled = true;

				if (IsReadOnly)
					break;

				if (DeleteSelection())
					text = Text ?? string.Empty;

				Text = text.Insert(CaretIndex, c.ToString());
				CaretIndex++;
			}
		}

		#endregion
	}
}
