// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

using System;
using DigitalRise.Mathematics.Algebra;
using Microsoft.Xna.Framework;

namespace DigitalRise.Particles.Effectors
{
  /// <summary>
  /// Linearly interpolates a particle parameter of type <see cref="Vector3"/> between a start and 
  /// an end value.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Used particle parameters:
  /// <list type="table">
  /// <listheader><term>Particle Parameter</term><description>Description</description></listheader>
  /// <item>
  /// <term><see cref="ValueParameter"/></term>
  /// <description>
  /// The <see cref="Vector3"/> parameter that stores the result of the linear interpolation.
  /// </description>
  /// </item>
  /// <item>
  /// <term><see cref="StartParameter"/></term>
  /// <description>
  /// A <see cref="Vector3"/> parameter that defines the start value.
  /// </description>
  /// </item>
  /// <item>
  /// <term><see cref="EndParameter"/></term>
  /// <description>
  /// A <see cref="Vector3"/> parameter that defines the end value.
  /// </description>
  /// </item>
  /// <item>
  /// <term><see cref="FactorParameter"/></term>
  /// <description>
  /// A <see cref="float"/> parameter that defines the interpolation factor. If this factor is 0, 
  /// the interpolation results is equal to the start value. If this factor is 1, the interpolation 
  /// result is equal to the end value. Per default, the parameter "NormalizedAge" is used.
  /// </description>
  /// </item>
  /// </list>
  /// </para>
  /// </remarks>
  public class Vector3LerpEffector : ParticleEffector
  {
    //--------------------------------------------------------------
    #region Fields
    //--------------------------------------------------------------

    private IParticleParameter<Vector3> _startParameter;
    private IParticleParameter<Vector3> _endParameter;
    private IParticleParameter<Vector3> _valueParameter;
    private IParticleParameter<float> _factorParameter;
    #endregion


    //--------------------------------------------------------------
    #region Properties & Events
    //--------------------------------------------------------------

    /// <summary>
    /// Gets or sets the name of the parameter that stores the interpolation result.
    /// (A varying or uniform parameter of type <see cref="Vector3"/>.)
    /// </summary>
    /// <value>
    /// The name of the parameter that stores the interpolation result.
    /// (Parameter type: varying or uniform, value type: <see cref="Vector3"/>) <br/>
    /// The default value is <see langword="null"/>.
    /// </value>
    /// <remarks>
    /// This property needs to be set before the particle system is running. The particle effector 
    /// might ignore any changes that occur while the particle system is running.
    /// </remarks>
    [ParticleParameter(ParticleParameterUsage.Out)]
    public string ValueParameter { get; set; }


    /// <summary>
    /// Gets or sets the name of the parameter that defines the start value.
    /// (A varying or uniform parameter of type <see cref="Vector3"/>.)
    /// </summary>
    /// <value>
    /// The name of the parameter that defines the start value.
    /// (Parameter type: varying or uniform, value type: <see cref="Vector3"/>) <br/>
    /// The default value is <see langword="null"/>.
    /// </value>
    /// <remarks>
    /// This property needs to be set before the particle system is running. The particle effector 
    /// might ignore any changes that occur while the particle system is running.
    /// </remarks>
    [ParticleParameter(ParticleParameterUsage.In)]
    public string StartParameter { get; set; }


    /// <summary>
    /// Gets or sets the name of the parameter that defines the end value.
    /// (A varying or uniform parameter of type <see cref="Vector3"/>.)
    /// </summary>
    /// <value>
    /// The name of the parameter that defines the end value.
    /// (Parameter type: varying or uniform, value type: <see cref="Vector3"/>) <br/>
    /// The default value is <see langword="null"/>.
    /// </value>
    /// <remarks>
    /// This property needs to be set before the particle system is running. The particle effector 
    /// might ignore any changes that occur while the particle system is running.
    /// </remarks>
    [ParticleParameter(ParticleParameterUsage.In)]
    public string EndParameter { get; set; }


    /// <summary>
    /// Gets or sets the name of the parameter that defines the interpolation factor.
    /// (A varying or uniform parameter of type <see cref="Vector3"/>.)
    /// </summary>
    /// <value>
    /// The name of the parameter that defines the interpolation factor.
    /// (Parameter type: varying or uniform, value type: <see cref="Vector3"/>) <br/>
    /// The default value is "NormalizedAge".
    /// </value>
    /// <remarks>
    /// This property needs to be set before the particle system is running. The particle effector 
    /// might ignore any changes that occur while the particle system is running.
    /// </remarks>
    [ParticleParameter(ParticleParameterUsage.In)]
    public string FactorParameter { get; set; }
    #endregion


    //--------------------------------------------------------------
    #region Creation & Cleanup
    //--------------------------------------------------------------

    /// <summary>
    /// Initializes a new instance of the <see cref="Vector3LerpEffector"/> class.
    /// </summary>
    public Vector3LerpEffector()
    {
      FactorParameter = ParticleParameterNames.NormalizedAge;
    }
    #endregion


    //--------------------------------------------------------------
    #region Methods
    //--------------------------------------------------------------

    /// <inheritdoc/>
    protected override ParticleEffector CreateInstanceCore()
    {
      return new Vector3LerpEffector();
    }


    /// <inheritdoc/>
    protected override void CloneCore(ParticleEffector source)
    {
      // Clone ParticleEffector properties.
      base.CloneCore(source);

      // Clone Vector3LerpEffector properties.
      var sourceTyped = (Vector3LerpEffector)source;
      ValueParameter = sourceTyped.ValueParameter;
      StartParameter = sourceTyped.StartParameter;
      EndParameter = sourceTyped.EndParameter;
      FactorParameter = sourceTyped.FactorParameter;
    }


    /// <inheritdoc/>
    protected override void OnRequeryParameters()
    {
      _valueParameter = ParticleSystem.Parameters.Get<Vector3>(ValueParameter);
      _startParameter = ParticleSystem.Parameters.Get<Vector3>(StartParameter);
      _endParameter = ParticleSystem.Parameters.Get<Vector3>(EndParameter);
      _factorParameter = ParticleSystem.Parameters.Get<float>(FactorParameter);
    }


    /// <inheritdoc/>
    protected override void OnUninitialize()
    {
      _valueParameter = null;
      _startParameter = null;
      _endParameter = null;
      _factorParameter = null;
    }


    /// <inheritdoc/>
    protected override void OnBeginUpdate(TimeSpan deltaTime)
    {
      if (_valueParameter == null
          || _startParameter == null
          || _endParameter == null
          || _factorParameter == null)
      {
        return;
      }

      Vector3[] values = _valueParameter.Values;
      if (values == null)
      {
        // Value is a uniform parameter.
        var f = _factorParameter.DefaultValue;
        _valueParameter.DefaultValue = (1 - f) * _startParameter.DefaultValue + f * _endParameter.DefaultValue;
      }
    }


    /// <inheritdoc/>
    protected override void OnUpdateParticles(TimeSpan deltaTime, int startIndex, int count)
    {
      if (_valueParameter == null 
          || _startParameter == null
          || _endParameter == null
          || _factorParameter == null)
      {
        return;
      }

      Vector3[] values = _valueParameter.Values;
      if (values == null)
      {
        // Value is a uniform parameter. Uniform parameters are handled in OnBeginUpdate().
        return;
      }

      // Value is a varying parameter.
      Vector3[] starts = _startParameter.Values;
      Vector3[] ends = _endParameter.Values;
      float[] factors = _factorParameter.Values;

      if (starts != null && ends != null && factors != null)
      {
        // Optimized case: Start, End, and Factor are varying parameters.
        for (int i = startIndex; i < startIndex + count; i++)
        {
          var f = factors[i];
          values[i] = (1 - f) * starts[i] + f * ends[i];
        }
      }
      else if (starts == null && ends == null && factors != null)
      {
        // Optimized case: Start and End are uniform parameters, Factor is varying parameter.
        Vector3 startValue = _startParameter.DefaultValue;
        Vector3 endValue = _endParameter.DefaultValue;
        for (int i = startIndex; i < startIndex + count; i++)
        {
          var f = factors[i];
          values[i] = (1 - f) * startValue + f * endValue;
        }
      }
      else
      {
        // General case:
        for (int i = startIndex; i < startIndex + count; i++)
        {
          var f = _factorParameter.GetValue(i);
          values[i] = (1 - f) * _startParameter.GetValue(i) + f * _endParameter.GetValue(i);
        }
      }
    }
    #endregion
  }
}
