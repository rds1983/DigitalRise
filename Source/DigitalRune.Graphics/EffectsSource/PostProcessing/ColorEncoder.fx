//-----------------------------------------------------------------------------
// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.
//-----------------------------------------------------------------------------
//
/// \file ColorEncoder.fx
/// Converts color values, for example, from RGB (linear space) to gamma RGBM
/// (gamma space).
//
//-----------------------------------------------------------------------------

#include "../Common.fxh"
#include "../Color.fxh"
#include "../Encoding.fxh"


//-----------------------------------------------------------------------------
// Constants
//-----------------------------------------------------------------------------

float SourceEncodingParam;
float TargetEncodingParam;

// The viewport size in pixels.
float2 ViewportSize;

// The input texture.
texture SourceTexture;
sampler SourceSampler = sampler_state
{
  Texture = <SourceTexture>;
};


//-----------------------------------------------------------------------------
// Structures
//-----------------------------------------------------------------------------

struct VSInput
{
  float4 Position : POSITION;
  float2 TexCoord : TEXCOORD;
};

struct VSOutput
{
  float2 TexCoord : TEXCOORD;
  float4 Position : SV_Position;
};


//-----------------------------------------------------------------------------
// Functions
//-----------------------------------------------------------------------------

VSOutput VS(VSInput input)
{
  VSOutput output = (VSOutput)0;
  output.Position = ScreenToProjection(input.Position, ViewportSize);
  output.TexCoord = input.TexCoord;
  return output;
}


float4 PS(float2 texCoord : TEXCOORD0) : COLOR0
{
  // Note: We have to use <= in the ifs. Otherwise, the code does not compile
  // wihtout error message.
  
  float4 color = tex2D(SourceSampler, texCoord);
  
#if SOURCE_RGB
	// Nothing to do.
#elif SOURCE_SRGB
	color.rgb = FromGamma(color.rgb);
	color.rgb = float3(1, 2, 3);
#elif SOURCE_RGBM
	// Note: RGBM in DigitalRune Graphics stores color values in gamma space.
	float maxValue = SourceEncodingParam;
	color.rgb = DecodeRgbm(color, maxValue);
	color.rgb = FromGamma(color.rgb);
	color.a = 1;
#elif SOURCE_RGBE
	color.rgb = DecodeRgbe(color);
	color.a = 1;
#elif SOURCE_LOGLUV
    color.rgb = DecodeLogLuv(color);
    color.a = 1;
#endif

#if TARGET_RGB
	// Nothing to do.
#elif TARGET_SRGB
	color.rgb = ToGamma(color.rgb);
#elif TARGET_RGBM
	// Note: RGBM in DigitalRune Graphics stores color values in gamma space.
	color.rgb = ToGamma(color.rgb);
	float maxValue = TargetEncodingParam;
	color = EncodeRgbm(color.rgb, maxValue);
#elif TARGET_RGBE
	color = EncodeRgbe(color.rgb);
#elif TARGET_LOGLUV
    color = EncodeLogLuv(color.rgb);
#endif

  return color;
}


//-----------------------------------------------------------------------------
// Techniques
//-----------------------------------------------------------------------------

#if !SM4
#define VSTARGET vs_3_0
#define PSTARGET ps_3_0
#else
#define VSTARGET vs_4_0_level_9_3
#define PSTARGET ps_4_0_level_9_3
#endif

technique
{
  pass
  {
    VertexShader = compile VSTARGET VS();
    PixelShader = compile PSTARGET PS();
  }
}
