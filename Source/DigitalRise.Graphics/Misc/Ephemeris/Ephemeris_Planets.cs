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

using DigitalRise.Mathematics;


namespace DigitalRise.Graphics
{
  /// <summary>
  /// The planets which are visible to the naked eye.
  /// </summary>
  internal enum VisiblePlanets
  {
    Mercury,
    Venus,
    Earth,
    Mars,
    Jupiter,
    Saturn,
  };


  partial class Ephemeris
  {
    //--------------------------------------------------------------
    #region Nested Types
    //--------------------------------------------------------------

    // Stores the parameters required to uniquely identify a specific planet orbit.
    private struct OrbitalElements
    {
      public float Period;
      public float EpochLongitude;
      public float PerihelionLongitude;
      public float Eccentricity;
      public float SemiMajorAxis;
      public float Inclination;
      public float LongitudeAscendingNode;
      public float AngularDiameter;
      public float VisualMagnitude;


      public OrbitalElements(float period, float epochLongitude, float perihelionLongitude,
        float eccentricity, float semiMajorAxis, float inclination, float longitudeAscendingNode,
        float angularDiameter, float visualMagnitude)
      {
        Period = period;
        EpochLongitude = epochLongitude;
        PerihelionLongitude = perihelionLongitude;
        Eccentricity = eccentricity;
        SemiMajorAxis = semiMajorAxis;
        Inclination = inclination;
        LongitudeAscendingNode = longitudeAscendingNode;
        AngularDiameter = angularDiameter;
        VisualMagnitude = visualMagnitude;
      }
    };


    //// Stores the calculated position and brightness of a planet.
    //private struct PlanetData
    //{
    //  // Equatorial coordinates.
    //  public float RightAscension;
    //  public float Declination;

    //  // Measures the brightness of the planet as seen on earth. Lower values mean more brightness.
    //  // (I think this is the same as "apparent magnitude".)
    //  public float VisualMagnitude;
    //};
    #endregion


    //--------------------------------------------------------------
    #region Constants
    //--------------------------------------------------------------

    //private const int NumberOfPlanets = 6;

    // The orbits of the visible planets.
    private readonly OrbitalElements[] _planetElements =
    {
      // Mercury
      new OrbitalElements(0.240852f, MathHelper.ToRadians(60.750646f), MathHelper.ToRadians(77.299833f), 
        0.205633f, 0.387099f, MathHelper.ToRadians(7.004540f), MathHelper.ToRadians(48.212740f), 6.74f, -0.42f),

      // Venus
      new OrbitalElements(0.615211f, MathHelper.ToRadians(88.455855f), MathHelper.ToRadians(131.430236f), 
        0.006778f, 0.723332f, MathHelper.ToRadians(3.394535f), MathHelper.ToRadians(76.589820f), 16.92f, -4.40f),

      // Earth
      new OrbitalElements(1.00004f, MathHelper.ToRadians(99.403308f), MathHelper.ToRadians(102.768413f), 
        0.016713f, 1.00000f, 0, 0, 0, 0),

      // Mars
      new OrbitalElements(1.880932f, MathHelper.ToRadians(240.739474f), MathHelper.ToRadians(335.874939f), 
        0.093396f, 1.523688f, MathHelper.ToRadians(1.849736f), MathHelper.ToRadians(49.480308f), 9.36f, -1.52f),

      // Jupiter
      new OrbitalElements(11.863075f, MathHelper.ToRadians(90.638185f), MathHelper.ToRadians(14.170747f), 
        0.048482f, 5.202561f, MathHelper.ToRadians(1.303613f), MathHelper.ToRadians(100.353142f), 196.74f, -9.40f),

      // Saturn
      new OrbitalElements(29.471362f, MathHelper.ToRadians(287.690033f), MathHelper.ToRadians(92.861407f), 
        0.055581f, 9.554747f, MathHelper.ToRadians(2.488980f), MathHelper.ToRadians(113.576139f), 165.60f, -8.88f)
    };
    #endregion


    //--------------------------------------------------------------
    #region Fields
    //--------------------------------------------------------------

    //private readonly PlanetData[] _planetData = new PlanetData[NumberOfPlanets];
    #endregion


    //--------------------------------------------------------------
    #region Properties & Events
    //--------------------------------------------------------------
    #endregion


    //--------------------------------------------------------------
    #region Methods
    //--------------------------------------------------------------

    ///// <summary>
    ///// Computes the planet data of the specified planet.
    ///// </summary>
    ///// <param name="planet">The planet.</param>
    ///// <remarks>
    ///// <see cref="ComputeEarthPosition"/> must have been called before this method!
    ///// </remarks>
    //void ComputePlanetData(VisiblePlanets planet)
    //{
    //  int index = (int)planet;

    //  float Np = ((2.0 * ConstantsF.Pi) / 365.242191) * (_epoch1990Days / planetElements[index].Period);
    //  Np = InRange(Np);

    //  float Mp = Np + planetElements[index].EpochLongitude - planetElements[index].PerihelionLongitude;

    //  float heliocentricLongitude = Np + 2.0 * planetElements[index].Eccentricity * Math.Sin(Mp) +
    //                                 planetElements[index].EpochLongitude;
    //  heliocentricLongitude = InRange(heliocentricLongitude);

    //  float vp = heliocentricLongitude - planetElements[index].PerihelionLongitude;

    //  float radius = (planetElements[index].SemiMajorAxis
    //              * (1.0 - planetElements[index].Eccentricity * planetElements[index].Eccentricity))
    //           / (1.0 + planetElements[index].Eccentricity * Math.Cos(vp));

    //  float heliocentricLatitude = Math.Asin(Math.Sin(heliocentricLongitude -
    //                                         planetElements[index].LongitudeAscendingNode) * Math.Sin(planetElements[index].Inclination));
    //  heliocentricLatitude = InRange(heliocentricLatitude);

    //  float y = Math.Sin(heliocentricLongitude - planetElements[index].LongitudeAscendingNode) *
    //             Math.Cos(planetElements[index].Inclination);
    //  float x = Math.Cos(heliocentricLongitude - planetElements[index].LongitudeAscendingNode);

    //  float projectedHeliocentricLongitude = Math.Atan2(y, x) + planetElements[index].LongitudeAscendingNode;

    //  float projectedRadius = radius * Math.Cos(heliocentricLatitude);

    //  float eclipticLongitude;

    //  if (index > (int)VisiblePlanets.Earth)
    //  {
    //    eclipticLongitude = Math.Atan((_earthRadius * Math.Sin(projectedHeliocentricLongitude - _earthEclipticLongitude))
    //                              / (projectedRadius - _earthRadius * Math.Cos(projectedHeliocentricLongitude - _earthEclipticLongitude)))
    //                        + projectedHeliocentricLongitude;
    //  }
    //  else
    //  {
    //    eclipticLongitude = ConstantsF.Pi + _earthEclipticLongitude + Math.Atan((projectedRadius * Math.Sin(_earthEclipticLongitude - projectedHeliocentricLongitude))
    //                                       / (_earthRadius - projectedRadius * Math.Cos(_earthEclipticLongitude - projectedHeliocentricLongitude)));
    //  }

    //  eclipticLongitude = InRange(eclipticLongitude);

    //  float eclipticLatitude = Math.Atan((projectedRadius * Math.Tan(heliocentricLatitude)
    //                                   * Math.Sin(eclipticLongitude - projectedHeliocentricLongitude))
    //                                  / (_earthRadius * Math.Sin(projectedHeliocentricLongitude - _earthEclipticLongitude)));

    //  float ra = Math.Atan2((Math.Sin(eclipticLongitude) * Math.Cos(_e) - Math.Tan(eclipticLatitude) * Math.Sin(_e))
    //                     , Math.Cos(eclipticLongitude));

    //  float dec = Math.Asin(Math.Sin(eclipticLatitude) * Math.Cos(_e) + Math.Cos(eclipticLatitude) * Math.Sin(_e)
    //                    * Math.Sin(eclipticLongitude));

    //  float dist2 = _earthRadius * _earthRadius + radius * radius - 2 * _earthRadius * radius * Math.Cos(heliocentricLongitude - _earthEclipticLongitude);
    //  float dist = Math.Sqrt(dist2);

    //  float d = eclipticLongitude - heliocentricLongitude;
    //  float phase = 0.5 * (1.0 + Math.Cos(d));

    //  float visualMagnitude;

    //  if (index == (int)VisiblePlanets.Venus)
    //  {
    //    d *= MathHelper.ToDegrees(d);
    //    visualMagnitude = -4.34 + 5.0 * Math.Log10(radius * dist) + 0.013 * d + 4.2E-7 * d * d * d;
    //  }
    //  else
    //  {
    //    visualMagnitude = 5.0 * Math.Log10((radius * dist) / Math.Sqrt(phase)) + planetElements[index].VisualMagnitude;
    //  }

    //  _planetData[index].RightAscension = ra;
    //  _planetData[index].Declination = dec;
    //  _planetData[index].VisualMagnitude = visualMagnitude;
    //}


    ///// <summary>
    ///// Gets the planet position.
    ///// </summary>
    ///// <param name="planet">The planet.</param>
    ///// <param name="rightAscension">The right ascension of .</param>
    ///// <param name="declination">The declination.</param>
    ///// <param name="visualMagnitude">The visual magnitude (lower values are brighter).</param>
    //public void GetPlanetPosition(VisiblePlanets planet, out float rightAscension, out float declination, out float visualMagnitude)
    //{
    //  if (planet == VisiblePlanets.Earth)
    //    throw new ArgumentException("This method must not be called for the earth.");

    //  var planetData = _planetData[(int)planet];
    //  rightAscension = planetData.RightAscension;
    //  declination = planetData.Declination;
    //  visualMagnitude = planetData.VisualMagnitude;

    //  // Note: To convert rightAscension and declination to horizontal coordinates:
    //  // Convert to cartesian coordinates. Apply EquatorialToWorld matrix.
    //  // To convert visualMagnitude to illuminance:
    //  // Use the formula of paper "A Physically-Based Night Sky Model" to compute irradiance.
    //  // When rendering as a sprite you must probably convert from irradiance to radiance using
    //  // the angular size of the sprite (solid angle).
    //  // Or use http://zfx.info/viewtopic.php?f=5&t=1298#p15341 to compute the "surface brightness".
    //  // To convert from radiometric units to photometric units multiply with 683 for green
    //  // (and smaller values for mixed spectrums).
    //}
    #endregion
  }
}
