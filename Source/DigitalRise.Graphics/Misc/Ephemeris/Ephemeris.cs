// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

#region ----- Credits -----
// The ephemeris model is based on:
//
//    "Physically-Based Outdoor Scene Lighting", by Frank Kane (Founder of Sundog Software, LLC),
//    Game Engine Gems 1.
//
//    Copyright (c) 2004-2008  Sundog Software, LLC. All rights reserved worldwide.
//
// Code is used with permission from Frank Kane.
#endregion

using System;
using DigitalRise.Mathematics;
using DigitalRise.Mathematics.Algebra;
using Microsoft.Xna.Framework;
using MathHelper = DigitalRise.Mathematics.MathHelper;

namespace DigitalRise.Graphics
{
  /// <summary>
  /// Computes the physically-based properties of sky objects like the sun and the moon.
  /// </summary>
  /// <remarks>
  /// <para>
  /// In astronomy and celestial navigation, an ephemeris (plural: ephemerides; from the Greek word
  /// ἐφημερίς ephēmeris "diary", "journal") gives the positions of astronomical objects in the sky
  /// at a given time or times. The class <see cref="Ephemeris"/> can be used to retrieve the 
  /// positions of the sun and the moon. It also computes transformations which can be used to
  /// convert between different astronomical coordinate system. Further, the light contributions of
  /// the sun and the moon are estimated.
  /// </para>
  /// <para>
  /// The input for all computations are the position (specified using <see cref="Latitude"/>,
  /// <see cref="Longitude"/> and <see cref="Altitude"/>) and the current <see cref="Time"/>. All
  /// derived values are computed when <see cref="Update"/> is called. That means, 
  /// <see cref="Update"/> must be called every time the input properties are changed. It is not
  /// called automatically, so <see cref="Update"/> must be called at least once.
  /// </para>
  /// <para>
  /// Following coordinate systems are used. All coordinate system are right-handed and can be used
  /// with cartesian coordinates (X, Y, Z) or polar coordinates (latitude, longitude).
  /// </para>
  /// <para>
  /// <strong>Ecliptic Coordinate System:</strong><br/>
  /// This coordinate system is relative to the plane defined by the path of the sun or (which is
  /// the same) the plane in which the earth moves around the sun.  That means, in the ecliptic
  /// system the latitude of the sun or the earth is always 0.<br/>
  /// Latitude and longitude are 0 at the vernal equinox. Latitude in this space is also called
  /// declination. Longitude is also called right ascension.<br/>
  /// Regarding Cartesian coordinates, the x and y axes are in the plane of the earth orbit. x is
  /// the axis where latitude and longitude are 0, which is equal to the vernal equinox. +z points
  /// north. The origin of the coordinate system can be the sun (heliocentric) or the earth
  /// (geocentric).
  /// </para>
  /// <para>
  /// <strong>Equatorial Coordinate System:</strong><br/>
  /// This coordinate system is relative to the plane defined by the earth's equator. Latitude and
  /// longitude are 0 at the vernal equinox.<br/>
  /// Regarding Cartesian coordinates, the x and y axes are in the plane of the equator. x is the
  /// axis where latitude and longitude are 0, which is equal to the vernal equinox. +y points east.
  /// +z points north. The origin of the coordinate system can be the sun (heliocentric) or the
  /// earth (geocentric).
  /// </para>
  /// <para>
  /// <strong>Geographic Coordinate System:</strong><br/>
  /// This coordinate system is relative to the plane defined by the earth's equator. This system is
  /// like the Equatorial system but the longitude is 0 at Greenwich. This means, the difference to
  /// the Equatorial system is a constant longitude offset. This coordinate system is well known
  /// from school and globes. The properties <see cref="Latitude"/>, <see cref="Longitude"/> are
  /// relative to the Geographic Coordinate System.<br/>
  /// Regarding Cartesian coordinates, the x and y axes are in the plane of the equator. x is the
  /// axis where latitude and longitude are 0, which is in the line of Greenwich. +y points east. +z
  /// points north.
  /// </para>
  /// <para>
  /// <strong>World Space:</strong><br/>
  /// This coordinate system is relative to a place on the earth. Computer game levels use this
  /// coordinate system. It is also known as "Horizontal Coordinate System" or "Horizon Coordinates".
  /// The origin of this space is defined by <see cref="Latitude"/>, <see cref="Longitude"/> (in the
  /// Geographic coordinate system) and <see cref="Altitude"/>.<br/>
  /// Regarding Cartesian coordinates, +x points east, +y points up, -z points north.
  /// </para>
  /// </remarks>
  public partial class Ephemeris
  {
    // Notes:
    // References: 
    // - "Physically-Based Outdoor Scene Lighting", Game Engine Gems 1.
    //   which is based on the paper "A physically based night sky model" and others.
    //
    // ----- Misc
    // Lat/Long/Altitude could be encapsulated in a struct GeographicLocation.
    //
    // ----- TODOs:
    // - Add unit tests.


    //--------------------------------------------------------------
    #region Fields
    //--------------------------------------------------------------

    // The fractional number of centuries elapsed since January 1, 2000 GMT, terrestrial time
    // (which does not account for leap seconds).
    private float _epoch2000Centuries;

    // The fractional number of days elapsed since January 1, 1990 GMT.
    private float _epoch1990Days;

    // Heliocentric radius of earth.
    //private float _earthRadius;

    // Longitude of earth in ecliptic coordinates. (Ecliptic lat is always 0.)
    private float _earthEclipticLongitude;

    // Longitude of sun in ecliptic coordinates. (Ecliptic lat is always 0.)
    private float _sunEclipticLongitude;

    // Obliquity of the ecliptic = tilt between ecliptic and equatorial plane.
    private float _e;

    // Conversion from Equatorial to World ignoring precession.
    private Matrix44F _equatorialToWorldNoPrecession;
    #endregion


    //--------------------------------------------------------------
    #region Properties & Events
    //--------------------------------------------------------------

    /// <summary>
    /// Gets or sets the latitude of the world space origin (using the Geographic coordinate space).
    /// </summary>
    /// <value>
    /// The latitude in degrees in the range [-90, 90]. (90° is at the north pole.)
    /// </value>
    public float Latitude
    {
      get { return _latitude; }
      set
      {
        if (value < -90 || value > 90)
          throw new ArgumentOutOfRangeException("value");

        _latitude = value;
      }
    }
    private float _latitude;


    /// <summary>
    /// Gets or sets the longitude of the world space origin (using the Geographic coordinate space).
    /// </summary>
    /// <value>
    /// The longitude in degrees in the range [-180, 180]. (East is positive.)
    /// </value>
    public float Longitude
    {
      get { return _longitude; }
      set
      {
        if (value < -180 || value > 180)
          throw new ArgumentOutOfRangeException("value");

        _longitude = value;
      }
    }
    private float _longitude;


    /// <summary>
    /// Gets or sets the altitude (elevation) in meters above the mean sea level.
    /// </summary>
    /// <value>The altitude (elevation) in meters above the mean sea level.</value>
    public float Altitude { get; set; }


    /// <summary>
    /// Gets or sets the date and time relative to Coordinated Universal Time (UTC).
    /// </summary>
    /// <value>
    /// The date and time relative to Coordinated Universal Time (UTC). The property is initialized
    /// with <see cref="DateTimeOffset.UtcNow"/>.
    /// </value>
    public DateTimeOffset Time { get; set; }


    /// <summary>
    /// Gets the sun position in world space in meters.
    /// </summary>
    /// <value>The sun position in world space in meters.</value>
    public Vector3 SunPosition { get; private set; }


    /// <summary>
    /// Gets the direction to the sun as seen from within the atmosphere considering optical 
    /// refraction.
    /// </summary>
    /// <value>
    /// The direction to the sun as seen from within the atmosphere considering optical refraction.
    /// </value>
    public Vector3 SunDirectionRefracted { get; private set; }


    /// <summary>
    /// Gets the moon position in world space.
    /// </summary>
    /// <value>The moon position in world space in meters.</value>
    public Vector3 MoonPosition { get; private set; }


    /// <summary>
    /// Gets the moon phase angle.
    /// </summary>
    /// <value>
    /// The moon phase angle in radians in the range [0, 2π]. A new moon has a phase angle of
    /// 0. A full moon has a phase angle of π. 
    /// </value>
    public float MoonPhaseAngle { get; set; }


    /// <summary>
    /// Gets the moon phase as a relative value.
    /// </summary>
    /// <value>The moon phase in the range [0, 1], where 0 is new moon and 1 is full moon.</value>
    public float MoonPhaseRelative
    {
      get
      {
        // The moon phase in the range [0, 1], where 0 is new moon and 1 is full moon.
        return 0.5f * (1.0f - MathF.Cos(MoonPhaseAngle));
      }
    }


    /// <summary>
    /// Gets the rotation matrix which converts directions from the ecliptic coordinate system to
    /// the equatorial coordinate system.
    /// </summary>
    /// <value>
    /// The rotation matrix which converts directions from the ecliptic coordinate system to the
    /// equatorial coordinate system.
    /// </value>
    public Matrix33F EclipticToEquatorial { get; private set; }


    //public Matrix44F EclipticToWorld { get; private set; }


    /// <summary>
    /// Gets the transformation matrix which converts directions from the equatorial coordinate
    /// system to the world space.
    /// </summary>
    /// <value>
    /// The transformation matrix which converts directions from the equatorial coordinate system to
    /// the world space.
    /// </value>
    public Matrix44F EquatorialToWorld { get; private set; }


    /// <summary>
    /// Gets the rotation matrix which converts directions from the equatorial coordinate system to
    /// the geographic coordinate system.
    /// </summary>
    /// <value>
    /// The rotation matrix which converts directions from the equatorial coordinate system to the
    /// geographic coordinate system.
    /// </value>
    public Matrix33F EquatorialToGeographic { get; private set; }


    //private Matrix33F WorldToGeographic { get; set; }
    #endregion


    //--------------------------------------------------------------
    #region Creation & Cleanup
    //--------------------------------------------------------------

    /// <summary>
    /// Initializes a new instance of the <see cref="Ephemeris"/> class.
    /// </summary>
    public Ephemeris()
    {
      // Seattle, Washington
      Latitude = 47;
      Longitude = 122;
      Altitude = 100;
      Time = DateTimeOffset.UtcNow;
    }
    #endregion


    //--------------------------------------------------------------
    #region Methods
    //--------------------------------------------------------------

    /// <summary>
    /// Computes the derived values, like sun/moon positions, transformation matrices and light
    /// intensities. This method must be called when the location or time has changed.
    /// </summary>
    /// <remarks>
    /// This method must be called when the input properties <see cref="Latitude"/>,
    /// <see cref="Longitude"/>, <see cref="Altitude"/>, or <see cref="Time"/>) have changed.
    /// </remarks>
    public void Update()
    {
      _epoch2000Centuries = ToEpoch2000Centuries(Time, true);
      _epoch1990Days = ToEpoch1990Days(Time, false);

      // To transform from ecliptic to equatorial, we rotate by the obliquity of the ecliptic.
      _e = 0.409093f - 0.000227f * _epoch2000Centuries;
      EclipticToEquatorial = Matrix33F.CreateRotationX(_e);

      // GMST = Greenwich mean sidereal time (mittlere Greenwich-Sternzeit) in radians.
      float gmst = 4.894961f + 230121.675315f * ToEpoch2000Centuries(Time, false);
      EquatorialToGeographic = Matrix33F.CreateRotationZ(-gmst);

      // The earth axis slowly changes over time (precession). The precession movement repeats
      // itself approx. all 26000 years. When we move from to horizontal or geographics,
      // we need to apply the precession.

      // In Game Engine Gems:
      //var Rx = Matrix33F.CreateRotationX(0.1118 * _epoch2000Centuries);
      //var Ry = Matrix33F.CreateRotationY(-0.00972 * _epoch2000Centuries);
      //var Rz = Matrix33F.CreateRotationZ(0.01118 * _epoch2000Centuries);
      //var precession = Rz * (Ry * Rx);

      // In original article:
      var Ry = Matrix33F.CreateRotationY(-0.00972f * _epoch2000Centuries);
      var Rz = Matrix33F.CreateRotationZ(0.01118f * _epoch2000Centuries);
      var precession = Rz * Ry * Rz;

      // In game engine gems precession is applied in EclipticToWorld and in
      // EquatorialToWorld. This makes no sense since precession cannot be valid for both
      // coordinate systems. --> We assume the precession is given in equatorial space.
      //EclipticToWorld = rLat * rLong * EclipticToEquatorial * precession;

      // Latitude rotation
      var rLat = Matrix33F.CreateRotationY(MathHelper.ToRadians(Latitude) - ConstantsF.PiOver2);

      // Longitude rotation
      // LMST = Local mean sidereal time (mittlere Ortssternzeit) in radians.
      float lmst = gmst + MathHelper.ToRadians(Longitude);
      var rLong = Matrix33F.CreateRotationZ(-lmst);

      // Earth radius at the equator. (We assume a perfect sphere. We do not support geodetic 
      // systems with imperfect earth spheres.)
      const float earthRadius = 6378.137f * 1000;
      var equatorialToHorizontalTranslation = new Vector3(0, -earthRadius - Altitude, 0);

      // Switching of the coordinate axes between Equatorial (z up) and Horizontal (y up).
      var axisSwitch = new Matrix33F(0, 1, 0,
                                     0, 0, 1,
                                     1, 0, 0);

      EquatorialToWorld = new Matrix44F(axisSwitch * rLat * rLong * precession,
                                        equatorialToHorizontalTranslation);

      _equatorialToWorldNoPrecession = new Matrix44F(axisSwitch * rLat * rLong,
                                                     equatorialToHorizontalTranslation);

      //WorldToGeographic = EquatorialToGeographic * EquatorialToWorld.Minor.Transposed;

      ComputeSunPosition();
      ComputeMoonPosition();
      ComputeEarthPosition();

      //for (int i = 0; i < NumberOfPlanets; i++)
      //{
      //  var planet = (VisiblePlanets)i;
      //  if (planet != VisiblePlanets.Earth)
      //    ComputePlanetData(planet);
      //}
    }


    private void ComputeSunPosition()
    {
      // See http://en.wikipedia.org/wiki/Position_of_the_Sun. (But these formulas seem to be a bit
      // more precise.)

      float meanAnomaly = 6.24f + 628.302f * _epoch2000Centuries;

      // Ecliptic longitude.
      _sunEclipticLongitude = 4.895048f + 628.331951f * _epoch2000Centuries + (0.033417f - 0.000084f * _epoch2000Centuries) * MathF.Sin(meanAnomaly)
                            + 0.000351f * MathF.Sin(2.0f * meanAnomaly);

      // Distance from earth in astronomical units.
      float geocentricDistance = 1.000140f - (0.016708f - 0.000042f * _epoch2000Centuries) * MathF.Cos(meanAnomaly)
                                  - 0.000141f * MathF.Cos(2.0f * meanAnomaly);

      // Sun position.
      Vector3 sunPositionEcliptic = ToCartesian(geocentricDistance, 0, _sunEclipticLongitude);
      Vector3 sunPositionEquatorial = EclipticToEquatorial * sunPositionEcliptic;

      // Note: The sun formula is already corrected by precession.
      SunPosition = _equatorialToWorldNoPrecession.TransformDirection(sunPositionEquatorial);
      Vector3 sunDirection = SunPosition.Normalized();

      // Convert from astronomical units to meters.
      const float au = 149597870700f; // 1 au = 149,597,870,700 m
      SunPosition *= au;

      // Account for atmospheric refraction.
      float elevation = MathF.Asin(sunDirection.Y);
      elevation = Refract(elevation);
      sunDirection.Y = MathF.Sin(elevation);
      sunDirection.Normalize();
      SunDirectionRefracted = sunDirection;
    }


    private void ComputeEarthPosition()
    {
      float Np = ((2.0f * ConstantsF.Pi) / 365.242191f) * (_epoch1990Days / _planetElements[(int)VisiblePlanets.Earth].Period);
      Np = InRange(Np);

      float Mp = Np + _planetElements[(int)VisiblePlanets.Earth].EpochLongitude - _planetElements[(int)VisiblePlanets.Earth].PerihelionLongitude;

      _earthEclipticLongitude = Np + 2.0f * _planetElements[(int)VisiblePlanets.Earth].Eccentricity * MathF.Sin(Mp)
                                + _planetElements[(int)VisiblePlanets.Earth].EpochLongitude;

      _earthEclipticLongitude = InRange(_earthEclipticLongitude);

      //float vp = _earthEclipticLongitude - planetElements[(int)VisiblePlanets.Earth].PerihelionLongitude;
      //_earthRadius = (planetElements[(int)VisiblePlanets.Earth].SemiMajorAxis 
      //                 * (1.0 - planetElements[(int)VisiblePlanets.Earth].Eccentricity * planetElements[(int)VisiblePlanets.Earth].Eccentricity)) 
      //               / (1.0 + planetElements[(int)VisiblePlanets.Earth].Eccentricity * MathF.Cos(vp));
    }


    private void ComputeMoonPosition()
    {
      float lp = 3.8104f + 8399.7091f * _epoch2000Centuries;
      float m = 6.2300f + 628.3019f * _epoch2000Centuries;
      float f = 1.6280f + 8433.4663f * _epoch2000Centuries;
      float mp = 2.3554f + 8328.6911f * _epoch2000Centuries;
      float d = 5.1985f + 7771.3772f * _epoch2000Centuries;

      float longitude =
          lp
          + 0.1098f * MathF.Sin(mp)
          + 0.0222f * MathF.Sin(2 * d - mp)
          + 0.0115f * MathF.Sin(2 * d)
          + 0.0037f * MathF.Sin(2 * mp)
          - 0.0032f * MathF.Sin(m)
          - 0.0020f * MathF.Sin(2 * f)
          + 0.0010f * MathF.Sin(2 * d - 2 * mp)
          + 0.0010f * MathF.Sin(2 * d - m - mp)
          + 0.0009f * MathF.Sin(2 * d + mp)
          + 0.0008f * MathF.Sin(2 * d - m)
          + 0.0007f * MathF.Sin(mp - m)
          - 0.0006f * MathF.Sin(d)
          - 0.0005f * MathF.Sin(m + mp);

      float latitude =
          +0.0895f * MathF.Sin(f)
          + 0.0049f * MathF.Sin(mp + f)
          + 0.0048f * MathF.Sin(mp - f)
          + 0.0030f * MathF.Sin(2 * d - f)
          + 0.0010f * MathF.Sin(2 * d + f - mp)
          + 0.0008f * MathF.Sin(2 * d - f - mp)
          + 0.0006f * MathF.Sin(2 * d + f);

      longitude = InRange(longitude);
      _sunEclipticLongitude = InRange(_sunEclipticLongitude);
      MoonPhaseAngle = MathF.Abs(longitude - _sunEclipticLongitude);
      MoonPhaseAngle = InRange(MoonPhaseAngle);

      float pip =
          +0.016593f
          + 0.000904f * MathF.Cos(mp)
          + 0.000166f * MathF.Cos(2 * d - mp)
          + 0.000137f * MathF.Cos(2 * d)
          + 0.000049f * MathF.Cos(2 * mp)
          + 0.000015f * MathF.Cos(2 * d + mp)
          + 0.000009f * MathF.Cos(2 * d - m);

      float dMoon = 1.0f / pip; // Earth radii

      // Moon position in Cartesian coordinates of the ecliptic coordinates system.
      Vector3 moonPositionEcliptic = ToCartesian(dMoon, latitude, longitude);

      // Moon position in Cartesian coordinates of the equatorial coordinates system.
      Vector3 moonPositionEquatorial = EclipticToEquatorial * moonPositionEcliptic;

      // To [m].
      moonPositionEquatorial *= 6378.137f * 1000;

      // Note: The moon formula is already corrected by precession.
      MoonPosition = _equatorialToWorldNoPrecession.TransformPosition(moonPositionEquatorial);
    }
    #endregion
  }
}
