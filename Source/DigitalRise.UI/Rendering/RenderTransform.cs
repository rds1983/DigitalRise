// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

using System;
using System.Globalization;
using System.Text;
using DigitalRise.Mathematics;
using DigitalRise.Mathematics.Algebra;
using FontStashSharp;
using FontStashSharp.RichText;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace DigitalRise.UI.Rendering
{
  /// <summary>
  /// Represents a render transformation.
  /// </summary>
  /// <remarks>
  /// <para>
  /// The render transformation defines a 2D SRT transformation: Scaling followed by a rotation 
  /// followed by a translation. The center of the scaling and rotation can be set explicitly.
  /// </para>
  /// <para>
  /// Note that a render transformation cannot be inverted. The inverse is not an SRT 
  /// transformation. But you can convert the render transformation to a 3x3 matrix.
  /// </para>
  /// <para>
  /// Render transformations can be combined using multiplication. The order of the transformations 
  /// is right-to-left - the same as when using matrices (<see cref="Matrix22F"/>, etc.). 
  /// </para>
  /// <para>
  /// <strong>SpriteBatch Rendering:</strong> The render transformation also provides wrapper 
  /// methods for the Draw-routines of the <see cref="SpriteBatch"/>. The render transformation will
  /// be automatically applied to all sprites and strings that are drawn.
  /// </para>
  /// <para>
  /// <strong>Important:</strong> The combination of rotations and non-uniform scaling can be 
  /// problematic. Non-uniform scaling should only be used if the non-uniform scale is set by the
  /// last render transform in a hierarchy. (E.g. if an element in a visual tree has a rotation and
  /// the parent element has a non-uniform scaling, the child element won't be rendered correctly.)
  /// </para>
  /// </remarks>
  public struct RenderTransform : IEquatable<RenderTransform>
  {
    //--------------------------------------------------------------
    #region Constants
    //--------------------------------------------------------------

    /// <summary>
    /// The identity transform.
    /// </summary>
    public static readonly RenderTransform Identity = new RenderTransform(Vector2.Zero, Vector2.One, 0, Vector2.Zero);
    #endregion


    //--------------------------------------------------------------
    #region Fields
    //--------------------------------------------------------------

    /// <summary>
    /// The origin of the render transformations in screen coordinates.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
    public Vector2 Origin;


    /// <summary>
    /// The scale.
    /// </summary>
    /// <remarks>
    /// Note that some combinations of non-uniform scalings and rotations are not supported. (See
    /// class description of <see cref="RenderTransform"/>.)
    /// </remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
    public Vector2 Scale;


    /// <summary>
    /// The rotation given as the angle in radians.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
    public float Rotation;


    /// <summary>
    /// The translation.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
    public Vector2 Translation;
    #endregion


    //--------------------------------------------------------------
    #region Properties & Events
    //--------------------------------------------------------------
    #endregion


    //--------------------------------------------------------------
    #region Creation & Cleanup
    //--------------------------------------------------------------

    /// <overloads>
    /// <summary>
    /// Initializes a new instance of the <see cref="RenderTransform"/> struct.
    /// </summary>
    /// </overloads>
    /// 
    /// <summary>
    /// Initializes a new instance of the <see cref="RenderTransform"/> struct.
    /// </summary>
    /// <param name="origin">The origin of the transformations in screen coordinates.</param>
    /// <param name="scale">The scale factors.</param>
    /// <param name="rotation">The rotation angle in radians.</param>
    /// <param name="translation">The translation vector.</param>
    public RenderTransform(Vector2 origin, Vector2 scale, float rotation, Vector2 translation)
    {
      Origin = origin;
      Scale = scale;
      Rotation = rotation;
      Translation = translation;
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="RenderTransform"/> struct.
    /// </summary>
    /// <param name="position">
    /// The upper, left corner of the current element in screen coordinates.
    /// </param>
    /// <param name="width">The width of the element.</param>
    /// <param name="height">The height of the element.</param>
    /// <param name="relativeOrigin">
    /// The relative origin of the render transformation. (0, 0) represents the upper, left corner
    /// and (1, 1) represents the lower, right corner of the element.
    /// </param>
    /// <param name="scale">The scale factors.</param>
    /// <param name="rotation">The rotation angle in radians.</param>
    /// <param name="translation">The translation vector.</param>
    public RenderTransform(Vector2 position, float width, float height, Vector2 relativeOrigin, Vector2 scale, float rotation, Vector2 translation)
    {
      Origin.X = position.X + width * relativeOrigin.X;
      Origin.Y = position.Y + height * relativeOrigin.Y;
      Scale = scale;
      Rotation = rotation;
      Translation = translation;
    }
    #endregion


    //--------------------------------------------------------------
    #region Methods
    //--------------------------------------------------------------

    /// <summary>
    /// Returns the hash code for this instance.
    /// </summary>
    /// <returns>
    /// A 32-bit signed integer that is the hash code for this instance.
    /// </returns>
    public override int GetHashCode()
    {
      // ReSharper disable NonReadonlyFieldInGetHashCode
      unchecked
      {
        int hashCode = Origin.GetHashCode();
        hashCode = (hashCode * 397) ^ Scale.GetHashCode();
        hashCode = (hashCode * 397) ^ Rotation.GetHashCode();
        hashCode = (hashCode * 397) ^ Translation.GetHashCode();
        return hashCode;
      }
      // ReSharper restore NonReadonlyFieldInGetHashCode
    }


    /// <overloads>
    /// <summary>
    /// Indicates whether the current object is equal to another object.
    /// </summary>
    /// </overloads>
    /// 
    /// <summary>
    /// Indicates whether this instance and a specified object are equal.
    /// </summary>
    /// <param name="obj">Another object to compare to.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="obj"/> and this instance are the same type and
    /// represent the same value; otherwise, <see langword="false"/>.
    /// </returns>
    public override bool Equals(object obj)
    {
      return obj is RenderTransform && this == (RenderTransform)obj;
    }


    /// <summary>
    /// Indicates whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns>
    /// <see langword="true"/> if the current object is equal to the other parameter; otherwise, 
    /// <see langword="false"/>.
    /// </returns>
    public bool Equals(RenderTransform other)
    {
      return this == other;
    }


    /// <summary>
    /// Compares two <see cref="RenderTransform"/>s to determine whether they are the same.
    /// </summary>
    /// <param name="transform1">The first render transform.</param>
    /// <param name="transform2">The second render transform.</param>
    /// <returns>
    /// <see langword="true"/> if the render transforms are equal; otherwise 
    /// <see langword="false"/>.
    /// </returns>
    public static bool operator ==(RenderTransform transform1, RenderTransform transform2)
    {
      return transform1.Origin == transform2.Origin
             && transform1.Scale == transform2.Scale
             && transform1.Rotation == transform2.Rotation
             && transform1.Translation == transform2.Translation;
    }


    /// <summary>
    /// Compares two <see cref="RenderTransform"/>s to determine whether they are the different.
    /// </summary>
    /// <param name="transform1">The first render transform.</param>
    /// <param name="transform2">The second render transform.</param>
    /// <returns>
    /// <see langword="true"/> if the render transforms are different; otherwise 
    /// <see langword="false"/>.
    /// </returns>
    public static bool operator !=(RenderTransform transform1, RenderTransform transform2)
    {
      return !(transform1 == transform2);
    }


    /// <summary>
    /// Transforms a position.
    /// </summary>
    /// <param name="position">The position in screen coordinates.</param>
    /// <returns>The transformed position in screen coordinates.</returns>
    public Vector2 ToRenderPosition(Vector2 position)
    {
      // Given: 
      //   p .... point in screen space
      //   p' ... transformed point in screen space
      //   o .... the center of the scaling and rotation in screen space
      //   S .... scaling matrix
      //   R .... rotation matrix
      //   t .... translation vector
      //
      // The transformation is:
      //   p' = R S (p - o) + o + t

      position -= Origin;
      position *= Scale;
      position = Matrix22F.CreateRotation(Rotation) * position;
      position += Origin;
      position += Translation;
      return position;

      // ----- Alternatively, using a 3x3 matrix:
      //Vector3 p = new Vector3(point.X, point.Y, 1);
      //p = ToMatrix33F() * p;
      //return new Vector2(p.X, p.Y);
    }


    /// <summary>
    /// Transforms a position by the inverse of this render transformation.
    /// </summary>
    /// <param name="position">The transformed position in screen coordinates.</param>
    /// <returns>The position in screen coordinates.</returns>
    public Vector2 FromRenderPosition(Vector2 position)
    {
      if (Numeric.IsZero(Scale.X) || Numeric.IsZero(Scale.Y))
        return new Vector2(float.NaN);

      Vector3 p = new Vector3(position.X, position.Y, 1);
      p = ToMatrix33F().Inverse * p;
      return new Vector2(p.X, p.Y);
    }


    /// <summary>
    /// Transforms a direction.
    /// </summary>
    /// <param name="direction">The direction.</param>
    /// <returns>The transformed direction.</returns>
    public Vector2 ToRenderDirection(Vector2 direction)
    {
      direction *= Scale;
      direction = Matrix22F.CreateRotation(Rotation) * direction;
      return direction;
    }


    /// <summary>
    /// Transforms a direction by the inverse of this render transformation.
    /// </summary>
    /// <param name="direction">The transformed direction.</param>
    /// <returns>The direction.</returns>
    public Vector2 FromRenderDirection(Vector2 direction)
    {
      if (Numeric.IsZero(Scale.X) || Numeric.IsZero(Scale.Y))
        return new Vector2(float.NaN);

      return ToMatrix22F().Inverse * direction;
    }


    /// <summary>
    /// Transforms the specified rectangle. (Does not work with rotations!)
    /// </summary>
    /// <param name="rectangle">The rectangle.</param>
    /// <returns>The transformed rectangle.</returns>
    public RectangleF Transform(RectangleF rectangle)
    {
      Vector2 location = rectangle.Location;
      Vector2 size = rectangle.Size;
      location = ToRenderPosition(location);
      size = ToRenderDirection(size);
      return new RectangleF(location.X, location.Y, size.X, size.Y + 0.5f);
    }


    /// <summary>
    /// Multiplies two render transformation.
    /// </summary>
    /// <param name="transform2">The first render transformation.</param>
    /// <param name="transform1">The second render transformation.</param>
    /// <returns>
    /// The product <paramref name="transform2"/> * <paramref name="transform1"/>.
    /// </returns>
    public static RenderTransform operator *(RenderTransform transform2, RenderTransform transform1)
    {
      // Concatenation of transformations:
      //
      //   p' = R1 S1 (p - o1) + o1 + t1
      //   p'' = R2 S2 (p' - o2) + o2 + t2
      //       = R2 S2 (R1 S1 (p - o1) + o1 + t1 - o2) + o2 + t2
      //       = R2 S2 R1 S1 (p - o1) + R2 S2 o1 + R2 S2 t1 - R2 S2 o2 + o2 + t2
      //
      //   Assuming only UNIFORM SCALINGS we can reorder the matrix product:
      //       = R2 R1 S2 S1 (p - o1) + R2 S2 o1 + R2 S2 t1 - R2 S2 o2 + o2 + t2
      //
      //   We can add o1 - o1. This allows to bring the equation in the standard form:
      //       = R2 R1 S2 S1 (p - o1) + R2 S2 o1 + R2 S2 t1 - R2 S2 o2 + o2 + t2 + o1 - o1
      //       = R2 R1  S2 S1  (p - o1)  +  o1  + R2 S2 o1 + R2 S2 t1 - R2 S2 o2 + o2 + t2 - o1
      //       =   R      S    (p - o)   +  o   +                t
      //   => S = S2 S1
      //      R = R2 R1
      //      o = o1
      //      t = R2 S2 o1 + R2 S2 t1 - R2 S2 o2 + o2 + t2 - o1 = 
      //        = -o1 + o2 + t2 + R2 S2 (o1 + t1 - o2) 

      // Note: In the following we treat the scalings as uniform scaling. In practice, 
      // combining non-uniform scalings with rotations creates an error. We simply ignore 
      // these errors. (See also remarks in class description.)

      RenderTransform result;

      var s1 = transform1.Scale;
      var s2 = transform2.Scale;
      var θ1 = transform1.Rotation;
      var θ2 = transform2.Rotation;
      var o1 = transform1.Origin;
      var o2 = transform2.Origin;
      var t1 = transform1.Translation;
      var t2 = transform2.Translation;

      // S = S2 S1
      // Scalings are combined by multiplying the scale factors.
      result.Scale = s2 * s1;

      // R = R2 R1
      // Rotations in 2D are combined by adding the rotation angles.
      result.Rotation = θ2 + θ1;

      // o = o1
      result.Origin = o1;

      // t = -o1 + o2 + t2 + R2 S2 (o1 + t1 - o2)
      result.Translation = -o1 + o2 + t2 + Matrix22F.CreateRotation(θ2) * (s2 * (o1 + t1 - o2));

      return result;
    }


    /// <summary>
    /// Multiplies two render transformation.
    /// </summary>
    /// <param name="transform2">The first render transformation.</param>
    /// <param name="transform1">The second render transformation.</param>
    /// <returns>
    /// The product <paramref name="transform2"/> * <paramref name="transform1"/>.
    /// </returns>
    public static RenderTransform Multiply(RenderTransform transform2, RenderTransform transform1)
    {
      return transform2 * transform1;
    }


    /// <summary>
    /// Converts this render transformation to a 3x3 matrix.
    /// </summary>
    /// <returns>
    /// A 3x3-matrix that represents the same transformation.
    /// </returns>
    public Matrix33F ToMatrix33F()
    {
      Matrix33F t = new Matrix33F(1, 0, Translation.X,
                                  0, 1, Translation.Y,
                                  0, 0, 1);
      Matrix33F o = new Matrix33F(1, 0, Origin.X,
                                  0, 1, Origin.Y,
                                  0, 0, 1);
      Matrix33F s = new Matrix33F(Scale.X, 0, 0,
                                  0, Scale.Y, 0,
                                  0, 0, 1);
      Matrix33F r = Matrix33F.CreateRotationZ(Rotation);
      Matrix33F minusO = new Matrix33F(1, 0, -Origin.X,
                                       0, 1, -Origin.Y,
                                       0, 0, 1);
      return t * o * r * s * minusO;
    }


    /// <summary>
    /// Converts this render transformation to a 2x2 matrix that only represents the scaling and the
    /// rotation (no translation).
    /// </summary>
    /// <returns>
    /// A 2x2-matrix that represents the scaling and the rotation (no translation).
    /// </returns>
    public Matrix22F ToMatrix22F()
    {
      Matrix22F s = new Matrix22F(Scale.X, 0,
                                  0, Scale.Y);
      Matrix22F r = Matrix22F.CreateRotation(Rotation);
      return r * s;
    }


    /// <summary>
    /// Converts a render transformation to a 3x3 matrix.
    /// </summary>
    /// <param name="transform">The render transformation.</param>
    /// <returns>
    /// A 3x3-matrix that represents the same transformation.
    /// </returns>
    public static implicit operator Matrix33F(RenderTransform transform)
    {
      return transform.ToMatrix33F();
    }


    /// <summary>
    /// Converts a render transformation to a 2x2 matrix that only represents the scaling and the
    /// rotation (no translation).
    /// </summary>
    /// <param name="transform">The render transformation.</param>
    /// <returns>
    /// A 2x2-matrix that represents the scaling and the rotation (no translation).
    /// </returns>
    public static implicit operator Matrix22F(RenderTransform transform)
    {
      return transform.ToMatrix22F();
    }


    /// <overloads>
    /// <summary>
    /// Returns the string representation of this render transform.
    /// </summary>
    /// </overloads>
    /// 
    /// <summary>
    /// Returns the string representation of this render transform.
    /// </summary>
    /// <returns>The string representation of this render transform.</returns>
    public override string ToString()
    {
      return ToString(CultureInfo.InvariantCulture);
    }


    /// <summary>
    /// Returns the string representation of this pose using the specified culture-specific format
    /// information.
    /// </summary>
    /// <param name="provider">
    /// An <see cref="IFormatProvider"/> that supplies culture-specific formatting information
    /// </param>
    /// <returns>The string representation of this pose.</returns>
    public string ToString(IFormatProvider provider)
    {
      return string.Format(
        provider,
        "RenderTransform {{ Origin = {0}, Scale = {1}, Rotation = {2}, Translation = {3} }}",
        Origin, Scale, Rotation, Translation);
    }
    #endregion
  }
}
