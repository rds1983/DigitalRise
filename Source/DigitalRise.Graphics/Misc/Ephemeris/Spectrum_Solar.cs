﻿// DigitalRune Engine - Copyright (C) DigitalRune GmbH
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


namespace DigitalRise.Graphics
{
	partial class Spectrum
	{
		// References: 
		// - "Physically-Based Outdoor Scene Lighting", Game Engine Gems 1.


		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional")]
		private static readonly double[,] rawNasaData =
		{
	  {379.5, 1.00E+00}, {380.5, 1.29E+00}, {381.5, 1.10E+00}, {382.5, 7.33E-01}, {383.5, 6.84E-01},
	  {384.5, 1.03E+00}, {385.5, 9.54E-01}, {386.5, 1.07E+00}, {387.5, 9.66E-01}, {388.5, 9.12E-01},
	  {389.5, 1.23E+00}, {390.5, 1.22E+00}, {391.5, 1.40E+00}, {392.5, 9.55E-01}, {393.5, 4.89E-01},
	  {394.5, 1.10E+00}, {395.5, 1.38E+00}, {396.5, 6.50E-01}, {397.5, 1.04E+00}, {398.5, 1.54E+00},
	  {399.5, 1.66E+00}, {400.5, 1.65E+00}, {401.5, 1.80E+00}, {402.5, 1.80E+00}, {403.5, 1.66E+00},
	  {404.5, 1.60E+00}, {405.5, 1.67E+00}, {406.5, 1.62E+00}, {407.5, 1.55E+00}, {408.5, 1.82E+00},
	  {409.5, 1.71E+00}, {410.5, 1.50E+00}, {411.5, 1.82E+00}, {412.5, 1.79E+00}, {413.5, 1.76E+00},
	  {414.5, 1.74E+00}, {415.5, 1.74E+00}, {416.5, 1.84E+00}, {417.5, 1.67E+00}, {418.5, 1.69E+00},
	  {419.5, 1.70E+00}, {420.5, 1.76E+00}, {421.5, 1.80E+00}, {422.5, 1.58E+00}, {423.5, 1.71E+00},
	  {424.5, 1.77E+00}, {425.5, 1.70E+00}, {426.5, 1.70E+00}, {427.5, 1.57E+00}, {428.5, 1.59E+00},
	  {429.5, 1.48E+00}, {430.5, 1.14E+00}, {431.5, 1.69E+00}, {432.5, 1.65E+00}, {433.5, 1.73E+00},
	  {434.5, 1.67E+00}, {435.5, 1.73E+00}, {436.5, 1.93E+00}, {437.5, 1.81E+00}, {438.5, 1.57E+00},
	  {439.5, 1.83E+00}, {440.5, 1.72E+00}, {441.5, 1.93E+00}, {442.5, 1.98E+00}, {443.5, 1.91E+00},
	  {444.5, 1.98E+00}, {445.5, 1.82E+00}, {446.5, 1.89E+00}, {447.5, 2.08E+00}, {448.5, 1.98E+00},
	  {449.5, 2.03E+00}, {450.5, 2.15E+00}, {451.5, 2.11E+00}, {452.5, 1.94E+00}, {453.5, 1.97E+00},
	  {454.5, 1.98E+00}, {455.5, 2.04E+00}, {456.5, 2.08E+00}, {457.5, 2.10E+00}, {458.5, 1.97E+00},
	  {459.5, 2.01E+00}, {460.5, 2.04E+00}, {461.5, 2.06E+00}, {462.5, 2.11E+00}, {463.5, 2.04E+00},
	  {464.5, 1.98E+00}, {465.5, 2.04E+00}, {466.5, 1.92E+00}, {467.5, 2.02E+00}, {468.5, 2.00E+00},
	  {469.5, 1.99E+00}, {470.5, 1.88E+00}, {471.5, 2.02E+00}, {472.5, 2.04E+00}, {473.5, 1.99E+00},
	  {474.5, 2.05E+00}, {475.5, 2.02E+00}, {476.5, 1.96E+00}, {477.5, 2.08E+00}, {478.5, 2.01E+00},
	  {479.5, 2.08E+00}, {480.5, 2.04E+00}, {481.5, 2.09E+00}, {482.5, 2.03E+00}, {483.5, 2.02E+00},
	  {484.5, 1.97E+00}, {485.5, 1.83E+00}, {486.5, 1.63E+00}, {487.5, 1.83E+00}, {488.5, 1.92E+00},
	  {489.5, 1.96E+00}, {490.5, 2.01E+00}, {491.5, 1.90E+00}, {492.5, 1.90E+00}, {493.5, 1.89E+00},
	  {494.5, 2.06E+00}, {495.5, 1.93E+00}, {496.5, 2.02E+00}, {497.5, 2.02E+00}, {498.5, 1.87E+00},
	  {499.5, 1.97E+00}, {500.5, 1.86E+00}, {501.5, 1.81E+00}, {502.5, 1.90E+00}, {503.5, 1.94E+00},
	  {504.5, 1.87E+00}, {505.5, 2.00E+00}, {506.5, 1.96E+00}, {507.5, 1.91E+00}, {508.5, 1.92E+00},
	  {509.5, 1.92E+00}, {510.5, 1.95E+00}, {511.5, 2.00E+00}, {512.5, 1.87E+00}, {513.5, 1.86E+00},
	  {514.5, 1.88E+00}, {515.5, 1.90E+00}, {516.5, 1.67E+00}, {517.5, 1.73E+00}, {518.5, 1.66E+00},
	  {519.5, 1.83E+00}, {520.5, 1.83E+00}, {521.5, 1.91E+00}, {522.5, 1.83E+00}, {523.5, 1.90E+00},
	  {524.5, 1.96E+00}, {525.5, 1.93E+00}, {526.5, 1.68E+00}, {527.5, 1.83E+00}, {528.5, 1.90E+00},
	  {529.5, 1.92E+00}, {530.5, 1.95E+00}, {531.5, 1.97E+00}, {532.5, 1.77E+00}, {533.5, 1.93E+00},
	  {534.5, 1.86E+00}, {535.5, 1.99E+00}, {536.5, 1.87E+00}, {537.5, 1.88E+00}, {538.5, 1.91E+00},
	  {539.5, 1.83E+00}, {540.5, 1.77E+00}, {541.5, 1.88E+00}, {542.5, 1.83E+00}, {543.5, 1.88E+00},
	  {544.5, 1.88E+00}, {545.5, 1.90E+00}, {546.5, 1.88E+00}, {547.5, 1.84E+00}, {548.5, 1.87E+00},
	  {549.5, 1.90E+00}, {550.5, 1.86E+00}, {551.5, 1.87E+00}, {552.5, 1.85E+00}, {553.5, 1.88E+00},
	  {554.5, 1.90E+00}, {555.5, 1.90E+00}, {556.5, 1.82E+00}, {557.5, 1.85E+00}, {558.5, 1.79E+00},
	  {559.5, 1.81E+00}, {560.5, 1.85E+00}, {561.5, 1.83E+00}, {562.5, 1.85E+00}, {563.5, 1.86E+00},
	  {564.5, 1.86E+00}, {565.5, 1.80E+00}, {566.5, 1.83E+00}, {567.5, 1.89E+00}, {568.5, 1.81E+00},
	  {569.5, 1.86E+00}, {570.5, 1.77E+00}, {571.5, 1.83E+00}, {572.5, 1.89E+00}, {573.5, 1.88E+00},
	  {574.5, 1.87E+00}, {575.5, 1.83E+00}, {576.5, 1.85E+00}, {577.5, 1.86E+00}, {578.5, 1.79E+00},
	  {579.5, 1.83E+00}, {580.5, 1.84E+00}, {581.5, 1.86E+00}, {582.5, 1.88E+00}, {583.5, 1.86E+00},
	  {584.5, 1.86E+00}, {585.5, 1.79E+00}, {586.5, 1.83E+00}, {587.5, 1.85E+00}, {588.5, 1.75E+00},
	  {589.5, 1.61E+00}, {590.5, 1.82E+00}, {591.5, 1.79E+00}, {592.5, 1.81E+00}, {593.5, 1.80E+00},
	  {594.5, 1.78E+00}, {595.5, 1.79E+00}, {596.5, 1.81E+00}, {597.5, 1.78E+00}, {598.5, 1.76E+00},
	  {599.5, 1.78E+00}, {600.5, 1.75E+00}, {601.5, 1.75E+00}, {602.5, 1.72E+00}, {603.5, 1.79E+00},
	  {604.5, 1.78E+00}, {605.5, 1.77E+00}, {606.5, 1.76E+00}, {607.5, 1.76E+00}, {608.5, 1.75E+00},
	  {609.5, 1.75E+00}, {610.5, 1.71E+00}, {611.5, 1.75E+00}, {612.5, 1.71E+00}, {613.5, 1.69E+00},
	  {614.5, 1.72E+00}, {615.5, 1.72E+00}, {616.5, 1.61E+00}, {617.5, 1.71E+00}, {618.5, 1.73E+00},
	  {619.5, 1.71E+00}, {620.5, 1.74E+00}, {621.5, 1.69E+00}, {622.5, 1.72E+00}, {623.5, 1.67E+00},
	  {624.5, 1.66E+00}, {625.5, 1.63E+00}, {626.5, 1.70E+00}, {627.5, 1.70E+00}, {628.5, 1.70E+00},
	  {629.5, 1.68E+00}, {631,   1.64E+00}, {633,   1.65E+00}, {635,   1.66E+00}, {637,   1.66E+00},
	  {639,   1.65E+00}, {641,   1.62E+00}, {643,   1.62E+00}, {645,   1.63E+00}, {647,   1.61E+00},
	  {649,   1.56E+00}, {651,   1.61E+00}, {653,   1.60E+00}, {655,   1.53E+00}, {657,   1.39E+00},
	  {659,   1.55E+00}, {661,   1.57E+00}, {663,   1.56E+00}, {665,   1.56E+00}, {667,   1.54E+00},
	  {669,   1.55E+00}, {671,   1.52E+00}, {673,   1.52E+00}, {675,   1.51E+00}, {677,   1.51E+00},
	  {679,   1.50E+00}, {681,   1.49E+00}, {683,   1.48E+00}, {685,   1.46E+00}, {687,   1.47E+00},
	  {689,   1.46E+00}, {691,   1.45E+00}, {693,   1.45E+00}, {695,   1.44E+00}, {697,   1.42E+00},
	  {699,   1.43E+00}, {701,   1.39E+00}, {703,   1.39E+00}, {705,   1.42E+00}, {707,   1.40E+00},
	  {709,   1.39E+00}, {711,   1.39E+00}, {713,   1.38E+00}, {715,   1.37E+00}, {717,   1.36E+00},
	  {719,   1.33E+00}, {721,   1.33E+00}, {723,   1.35E+00}, {725,   1.35E+00}, {727,   1.35E+00},
	  {729,   1.32E+00}, {731,   1.33E+00}, {733,   1.32E+00}, {735,   1.31E+00}, {737,   1.31E+00},
	  {739,   1.28E+00}, {741,   1.26E+00}, {743,   1.29E+00}, {745,   1.28E+00}, {747,   1.28E+00},
	  {749,   1.27E+00}, {751,   1.26E+00}, {753,   1.26E+00}, {755,   1.26E+00}, {757,   1.25E+00},
	  {759,   1.24E+00}, {761,   1.24E+00}, {763,   1.24E+00}, {765,   1.22E+00}, {767,   1.19E+00},
	  {769,   1.20E+00}, {771,   1.21E+00}, {773,   1.21E+00}, {775,   1.19E+00}, {777,   1.20E+00},
	  {779,   1.19E+00}, {781,   1.19E+00}
	};


		/// <summary>
		/// Sets this spectrum to the extraterrestrial solar spectrum, based on NASA data.
		/// </summary>
		public void SetSolarSpectrum()
		{
			int numberOfElements = rawNasaData.GetLength(0);
			int element = 1;
			double wavelength = 380;
			double prevWavelength = rawNasaData[0, 0];

			for (int i = 0; i < NumberOfSamples; i++)
			{
				double sampleTotal = 0;
				double nextWavelength = wavelength + SampleWidth;

				while (element < (numberOfElements - 1) && rawNasaData[element, 0] <= nextWavelength)
				{
					sampleTotal += rawNasaData[element, 1] * (rawNasaData[element, 0] - prevWavelength);
					prevWavelength = rawNasaData[element, 0];
					element++;
				}

				sampleTotal += rawNasaData[element, 1] * (nextWavelength - prevWavelength);
				Powers[i] = (float)sampleTotal;
				wavelength += SampleWidth;
			}
		}
	}
}
