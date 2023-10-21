// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

using System;
using System.Collections.Generic;
using DigitalRise.Mathematics;
using System.Security.Principal;
using System.Text;
using DigitalRise.UI.Rendering;
using FontStashSharp;
using FontStashSharp.RichText;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DigitalRise.UI.Controls
{
	/// <summary>
	/// Provides information during rendering of UI controls.
	/// </summary>
	public class UIRenderContext : IDisposable
	{
		// Copy from CullNone but with activate scissors test.
		private static readonly RasterizerState CullNoneWithScissors = new RasterizerState
		{
			Name = "UIRenderer.CullNoneWithScissors",
			CullMode = RasterizerState.CullNone.CullMode,
			DepthBias = RasterizerState.CullNone.DepthBias,
			FillMode = RasterizerState.CullNone.FillMode,
			MultiSampleAntiAlias = RasterizerState.CullNone.MultiSampleAntiAlias,
			SlopeScaleDepthBias = RasterizerState.CullNone.SlopeScaleDepthBias,
			ScissorTestEnable = true,
		};

		private static Texture2D _whiteTexture;

		private bool _batchIsActive;

		/// <summary>
		/// Gets a value indicating whether this instance has been disposed of.
		/// </summary>
		/// <value>
		/// <see langword="true"/> if this instance has been disposed of; otherwise, 
		/// <see langword="false"/>.
		/// </value>
		public bool IsDisposed { get; private set; }


		/// <summary>
		/// Gets or sets the size of the current time step.
		/// </summary>
		/// <value>The size of the current time step.</value>
		public TimeSpan DeltaTime { get; set; }


		/// <summary>
		/// Gets or sets the absolute opacity.
		/// </summary>
		/// <value>The absolute opacity. The default value is 1.0f.</value>
		public float Opacity { get; set; }


		/// <summary>
		/// Gets or sets the absolute render transformation.
		/// </summary>
		/// <value>
		/// The absolute render transformation. The default value is 
		/// <see cref="Rendering.RenderTransform.Identity"/>.
		/// </value>
		public RenderTransform RenderTransform { get; set; }

		/// <summary>
		/// Gets the sprite batch that is used to draw all images for the UI controls.
		/// </summary>
		/// <value>The sprite batch that is used to draw all images for the UI controls.</value>
		public SpriteBatch SpriteBatch { get; private set; }


		/// <summary>
		/// Gets a generic collection of name/value pairs which can be used to store custom data.
		/// </summary>
		/// <value>
		/// A generic collection of name/value pairs which can be used to store custom data.
		/// </value>
		public IDictionary<string, object> Data { get; private set; }

		/// <summary>
		/// Gets a white 1x1 texture.
		/// </summary>
		/// <value>A texture with a single white texel.</value>
		private Texture2D WhiteTexture
		{
			get
			{
				if (_whiteTexture == null)
				{
					_whiteTexture = CreateWhiteTexture(SpriteBatch.GraphicsDevice);
				}

				return _whiteTexture;
			}
		}

		private Vector2 Origin => RenderTransform.Origin;
		private Vector2 Scale => RenderTransform.Scale;
		private float Rotation => RenderTransform.Rotation;
		private Vector2 Translation => RenderTransform.Translation;


		/// <summary>
		/// Initializes a new instance of the <see cref="UIRenderContext"/> class.
		/// </summary>
		public UIRenderContext(GraphicsDevice graphicsDevice)
		{
			Opacity = 1.0f;
			RenderTransform = RenderTransform.Identity;
			Data = new Dictionary<string, object>();
			SpriteBatch = new SpriteBatch(graphicsDevice);
		}

		/// <summary>
		/// Releases all resources used by an instance of the <see cref="UIRenderer"/> class.
		/// </summary>
		/// <remarks>
		/// This method calls the virtual <see cref="Dispose(bool)"/> method, passing in 
		/// <see langword="true"/>, and then suppresses finalization of the instance.
		/// </remarks>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}


		/// <summary>
		/// Releases the unmanaged resources used by an instance of the <see cref="UIRenderer"/> class 
		/// and optionally releases the managed resources.
		/// </summary>
		/// <param name="disposing">
		/// <see langword="true"/> to release both managed and unmanaged resources; 
		/// <see langword="false"/> to release only unmanaged resources.
		/// </param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "_whiteTexture")]
		protected virtual void Dispose(bool disposing)
		{
			if (!IsDisposed)
			{
				if (disposing)
				{
					SpriteBatch.Dispose();
					_whiteTexture.SafeDispose();
				}

				IsDisposed = true;
			}
		}

		/// <summary>
		/// Calls the <see cref="Microsoft.Xna.Framework.Graphics.SpriteBatch.Begin"/> method of the
		/// <see cref="SpriteBatch"/> with default settings.
		/// </summary>
		/// <remarks>
		/// This method remembers if it was already called. Redundant calls of this method are safe.
		/// </remarks>
		public void BeginBatch()
		{
			if (!_batchIsActive)
			{
				SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp,
													DepthStencilState.None, CullNoneWithScissors);
				_batchIsActive = true;
			}
		}

		/// <summary>
		/// Calls <see cref="Microsoft.Xna.Framework.Graphics.SpriteBatch.End"/> method of the
		/// <see cref="SpriteBatch"/> to commit the current batch.
		/// </summary>
		public void EndBatch()
		{
			if (_batchIsActive)
			{
				SpriteBatch.End();
				_batchIsActive = false;
			}
		}

		/// <overloads>
		/// <summary>
		/// Transforms a sprite and adds it to a batch of sprites for rendering. 
		/// </summary>
		/// </overloads>
		/// 
		/// <summary>
		/// Transforms a sprite and adds it to a batch of sprites for rendering using the specified 
		/// texture, position and color. 
		/// </summary>
		/// <param name="spriteBatch">The sprite batch for rendering.</param>
		/// <param name="texture">The texture.</param>
		/// <param name="position">The location in screen coordinates to draw the sprite.</param>
		/// <param name="color">
		/// The color to tint a sprite. Use white for full color with no tinting.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="spriteBatch"/> or <paramref name="texture"/> is <see langword="null"/>.
		/// </exception>
		public void Draw(Texture2D texture, Vector2 position, Color color)
		{
			InternalDraw(texture, position, Vector2.One, null, color);
		}


		/// <summary>
		/// Transforms a sprite and adds it to a batch of sprites for rendering using the specified 
		/// texture, position, source rectangle and color. 
		/// </summary>
		/// <param name="spriteBatch">The sprite batch for rendering.</param>
		/// <param name="texture">The texture.</param>
		/// <param name="position">The location in screen coordinates to draw the sprite.</param>
		/// <param name="sourceRectangle">
		/// A rectangle that specifies (in texels) the source texels from a texture. Use 
		/// <see langword="null"/> to draw the entire texture.
		/// </param>
		/// <param name="color">
		/// The color to tint a sprite. Use white for full color with no tinting.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="spriteBatch"/> or <paramref name="texture"/> is <see langword="null"/>.
		/// </exception>
		public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color)
		{
			InternalDraw(texture, position, Vector2.One, sourceRectangle, color);
		}


		/// <summary>
		/// Transforms a sprite and adds it to a batch of sprites for rendering using the specified 
		/// texture, position, source rectangle and color. 
		/// </summary>
		/// <param name="spriteBatch">The sprite batch for rendering.</param>
		/// <param name="texture">The texture.</param>
		/// <param name="destinationRectangle">
		/// A rectangle that specifies (in screen coordinates) the destination for drawing the sprite. 
		/// If this rectangle is not the same size as the source rectangle, the sprite will be scaled to 
		/// fit.
		/// </param>
		/// <param name="sourceRectangle">
		/// A rectangle that specifies (in texels) the source texels from a texture. Use 
		/// <see langword="null"/> to draw the entire texture.
		/// </param>
		/// <param name="color">
		/// The color to tint a sprite. Use white for full color with no tinting.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="spriteBatch"/> or <paramref name="texture"/> is <see langword="null"/>.
		/// </exception>
		public void Draw(Texture2D texture, RectangleF destinationRectangle,
										 Rectangle? sourceRectangle, Color color)
		{
			if (texture == null)
				throw new ArgumentNullException("texture");

			Rectangle sourceRect = sourceRectangle ?? new Rectangle(0, 0, texture.Width, texture.Height);

			Vector2 scale = new Vector2(destinationRectangle.Width / sourceRect.Width,
																		destinationRectangle.Height / sourceRect.Height);

			InternalDraw(texture, destinationRectangle.Location, scale, sourceRectangle, color);
		}


		private void InternalDraw(Texture2D texture, Vector2 position, Vector2 scale, Rectangle? sourceRectangle, Color color)
		{
			if (SpriteBatch == null)
				throw new ArgumentNullException("spriteBatch");
			if (texture == null)
				throw new ArgumentNullException("texture");

			// The size of the sprite in texels.
			Vector2 spriteExtent = sourceRectangle.HasValue
												 ? new Vector2(sourceRectangle.Value.Width, sourceRectangle.Value.Height)
												 : new Vector2(texture.Width, texture.Height);

			if (Numeric.IsZero(spriteExtent.X) || Numeric.IsZero(spriteExtent.Y) || Numeric.IsZero(Scale.X) || Numeric.IsZero(Scale.Y))
				return;

			// ----- Convert origin from screen space to the local space of the sprite.
			// The render transform specifies the origin in screen coordinates, but for
			// the SpriteBatch the origin needs to be given in the local space of the sprite.

			// The final size of the sprite when drawn on screen (without render transform).
			Vector2 screenExtent = spriteExtent * scale;

			// The vector that points from the upper, left corner to the transform origin in screen 
			// coordinates (without render transform).
			Vector2 relativeOrigin = Origin - position;

			// Normalize this vector such that (0,0) is the upper, left corner and (1,1) the 
			// lower, right corner of the sprite.
			Vector2 normalizedOrigin = relativeOrigin / screenExtent;

			// Now we can compute the transform origin in the local space of the sprite.
			Vector2 spriteOrigin = normalizedOrigin * spriteExtent;

			// ----- Adjust sprite batch parameters.
			// Now we need to prepare the parameters for the SpriteBatch.Draw-call and apply the 
			// render transformation. (This is tricky because the XNA SpriteBatch is not documented 
			// well.)

			// The position that needs to he passed to the SpriteBatch is not the upper, left corner 
			// of the sprite. Instead it is the position where the origin of the sprite will be on
			// screen.

			// First compute the upper, left corner of the sprite considering the scale of the render
			// transform.
			position = Origin - relativeOrigin * Scale;

			// Now, let's find the point where the origin of the sprite will be on screen.
			position += normalizedOrigin * screenExtent * Scale;

			// Finally we need to apply the render transform.
			position += Translation;

			// scale is either (1, 1) or determined by the destinationRectangle.
			// We need to apply the render transform to the scale.
			scale *= Scale;

			// That's it - let's render.
			SpriteBatch.Draw(texture, (Vector2)position, sourceRectangle, color, Rotation, (Vector2)spriteOrigin, (Vector2)scale, SpriteEffects.None, 0.0f);
		}

		/// <overloads>
		/// <summary>
		/// Transforms a string and adds it to a batch of sprites for rendering.
		/// </summary>
		/// </overloads>
		/// 
		/// <summary>
		/// Transforms a string and adds it to a batch of sprites for rendering using the specified 
		/// font, text, position, and color.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch for rendering.</param>
		/// <param name="spriteFont">A font for displaying text.</param>
		/// <param name="text">The text string.</param>
		/// <param name="position">The location in screen coordinates to draw the sprite.</param>
		/// <param name="color">
		/// The color to tint a sprite. Use white for full color with no tinting.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="spriteBatch"/> or <paramref name="spriteFont"/> is <see langword="null"/>.
		/// </exception>
		public void DrawRichText(RichTextLayout richText, Vector2 position, Color color)
		{
			if (SpriteBatch == null)
				throw new ArgumentNullException("spriteBatch");

			if (Numeric.IsZero(Scale.X) || Numeric.IsZero(Scale.Y))
				return;

			// Adjust parameters for sprite batch.
			Vector2 origin = Origin - position;
			position = position + origin + Translation;

			if (RenderTransform == RenderTransform.Identity)
			{
				position.X = (int)position.X;
				position.Y = (int)position.Y;
				origin.X = (int)origin.X;
				origin.Y = (int)origin.Y;
			}

			richText.Draw(SpriteBatch, position, color, Rotation, origin, Scale);
		}


		/// <overloads>
		/// <summary>
		/// Transforms a string and adds it to a batch of sprites for rendering.
		/// </summary>
		/// </overloads>
		/// 
		/// <summary>
		/// Transforms a string and adds it to a batch of sprites for rendering using the specified 
		/// font, text, position, and color.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch for rendering.</param>
		/// <param name="spriteFont">A font for displaying text.</param>
		/// <param name="text">The text string.</param>
		/// <param name="position">The location in screen coordinates to draw the sprite.</param>
		/// <param name="color">
		/// The color to tint a sprite. Use white for full color with no tinting.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="spriteBatch"/> or <paramref name="spriteFont"/> is <see langword="null"/>.
		/// </exception>
		public void DrawString(SpriteFontBase spriteFont, string text, Vector2 position, Color color,
			TextStyle textStyle = TextStyle.None, FontSystemEffect effect = FontSystemEffect.None, int effectAmount = 0)
		{
			if (SpriteBatch == null)
				throw new ArgumentNullException("spriteBatch");

			if (Numeric.IsZero(Scale.X) || Numeric.IsZero(Scale.Y))
				return;

			// Adjust parameters for sprite batch.
			Vector2 origin = Origin - position;
			position = position + origin + Translation;

			if (RenderTransform == RenderTransform.Identity)
			{
				position.X = (int)position.X;
				position.Y = (int)position.Y;
				origin.X = (int)origin.X;
				origin.Y = (int)origin.Y;
			}

			SpriteBatch.DrawString(spriteFont, text, (Vector2)position, color, Rotation, origin, Scale, textStyle: textStyle, effect: effect, effectAmount: effectAmount);
		}


		/// <summary>
		/// Transforms a string and adds it to a batch of sprites for rendering using the specified 
		/// font, text, position, and color.
		/// </summary>
		/// <param name="spriteBatch">The sprite batch for rendering.</param>
		/// <param name="spriteFont">A font for displaying text.</param>
		/// <param name="text">The text string.</param>
		/// <param name="position">The location in screen coordinates to draw the sprite.</param>
		/// <param name="color">
		/// The color to tint a sprite. Use white for full color with no tinting.
		/// </param>
		public void DrawString(SpriteFontBase spriteFont, StringBuilder text, Vector2 position, Color color,
			TextStyle textStyle = TextStyle.None, FontSystemEffect effect = FontSystemEffect.None, int effectAmount = 0)
		{
			if (SpriteBatch == null)
				throw new ArgumentNullException("spriteBatch");

			if (Numeric.IsZero(Scale.X) || Numeric.IsZero(Scale.Y))
				return;

			// Adjust parameters for sprite batch.
			Vector2 origin = Origin - position;
			position = position + origin + Translation;

			if (RenderTransform == RenderTransform.Identity)
			{
				position.X = (int)position.X;
				position.Y = (int)position.Y;
				origin.X = (int)origin.X;
				origin.Y = (int)origin.Y;
			}

			SpriteBatch.DrawString(spriteFont, text, (Vector2)position, color, Rotation, origin, Scale, textStyle: textStyle, effect: effect, effectAmount: effectAmount);
		}


		public void FillRectangle(RectangleF rect, Color color)
		{
			Draw(WhiteTexture, rect, null, color);
		}

		/// <summary>
		/// Create a white 1x1 texture.
		/// </summary>
		/// <value>A texture with a single white texel.</value>
		public static Texture2D CreateWhiteTexture(GraphicsDevice graphicsDevice)
		{
			var result = new Texture2D(graphicsDevice, 1, 1, false, SurfaceFormat.Color);
			result.SetData(new[] { Color.White });

			return result;
		}
	}
}
