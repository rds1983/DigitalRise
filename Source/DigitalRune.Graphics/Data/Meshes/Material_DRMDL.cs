using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using AssetManagementBase;
using DigitalRune.Graphics.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNAssets;

namespace DigitalRune.Graphics
{ 
	partial class Material
	{
		internal static Material FromXml(AssetManager assetManager, IGraphicsService graphicsService, string xml)
		{
			var material = new Material();

			// Parse XML file.
			var document = XDocument.Parse(xml);

			var materialElement = document.Root;
			if (materialElement == null || materialElement.Name != "Material")
			{
				string message = string.Format(CultureInfo.InvariantCulture, "Root element \"<Material>\" is missing in XML.");
				throw new Exception(message);
			}

			// Override material name, if attribute is set.
			material.Name = (string)materialElement.Attribute("Name") ?? material.Name;

			// Create effect bindings for all render passes.
			foreach (var passElement in materialElement.Elements("Pass"))
			{
				string pass = passElement.GetMandatoryAttribute("Name");
				if (material.Passes.Contains(pass))
				{
					string message = XmlHelper.GetExceptionMessage(passElement, "Duplicate entry. The pass \"{0}\" was already defined.", pass);
					throw new Exception(message);
				}

				// Skip this pass if the graphics profile does not match the actual target profile.
				string profile = (string)passElement.Attribute("Profile") ?? "ANY";
				string profileLower = profile.ToUpperInvariant();
				if (profileLower == "REACH")
				{
					throw new Exception("Reach profile isn't supported.");
				}
				else if (profileLower != "HIDEF" && profileLower != "ANY")
				{
					string message = XmlHelper.GetExceptionMessage(passElement, "Unknown profile: \"{0}\". Allowed values are \"HiDef\" or \"Any\"", profile);
					throw new Exception(message);
				}

				// ----- Parameters
				var opaqueData = new Dictionary<string, object>();
				foreach (var parameterElement in passElement.Elements("Parameter"))
				{
					string name = parameterElement.GetMandatoryAttribute("Name");
					if (opaqueData.ContainsKey(name))
					{
						string message = XmlHelper.GetExceptionMessage(parameterElement, "Duplicate entry. The parameter \"{0}\" was already defined.", name);
						throw new Exception(message);
					}

					object value = parameterElement.Attribute("Value").ToParameterValue(null);
					if (value != null)
					{
						opaqueData.Add(name, value);
					}
				}

				// ----- Textures
				foreach (var textureElement in passElement.Elements("Texture"))
				{
					string name = textureElement.GetMandatoryAttribute("Name");
					if (opaqueData.ContainsKey(name))
					{
						string message = XmlHelper.GetExceptionMessage(textureElement, "Duplicate entry. The texture \"{0}\" was already defined.", name);
						throw new Exception(message);
					}

					string fileName = textureElement.GetMandatoryAttribute("File");

					// Texture processor parameters.
					// TODO: Do something with those params
					/*
					var colorKeyAttribute = textureElement.Attribute("ColorKey");
					bool colorKeyEnabled = colorKeyAttribute != null;
					Color colorKeyColor = colorKeyAttribute.ToColor(Color.Magenta);
					bool generateMipmaps = (bool?)textureElement.Attribute("GenerateMipmaps") ?? true;
					float inputGamma = (float?)textureElement.Attribute("InputGamma") ?? 2.2f;
					float outputGamma = (float?)textureElement.Attribute("OutputGamma") ?? 2.2f;
					bool premultiplyAlpha = (bool?)textureElement.Attribute("PremultiplyAlpha") ?? true;
					bool resizeToPowerOfTwo = (bool?)textureElement.Attribute("ResizeToPowerOfTwo") ?? false;
					float referenceAlpha = (float?)textureElement.Attribute("ReferenceAlpha") ?? 0.9f;
					bool scaleAlphaToCoverage = (bool?)textureElement.Attribute("ScaleAlphaToCoverage") ?? false;*/

					var texture = assetManager.LoadTexture(graphicsService.GraphicsDevice, fileName);
					opaqueData[name] = texture;
				}

				// ----- Effect
				string effectName = passElement.GetMandatoryAttribute("Effect");

				EffectBinding binding;

				switch (effectName)
				{
					case "AlphaTestEffect":
						binding = new AlphaTestEffectBinding(graphicsService, opaqueData);
						break;
					case "BasicEffect":
						binding = new BasicEffectBinding(graphicsService, opaqueData);
						break;
					case "DualTextureEffect":
						binding = new DualTextureEffectBinding(graphicsService, opaqueData);
						break;
					case "EnvironmentMapEffect":
						binding = new EnvironmentMapEffectBinding(graphicsService, opaqueData);
						break;
					case "SkinnedEffect":
						binding = new SkinnedEffectBinding(graphicsService, opaqueData);
						break;
					default:
						Effect effect;
						if (effectName.StartsWith("DigitalRune/"))
						{
							effect = Resources.GetDREffect(graphicsService.GraphicsDevice, effectName);
						} else
						{
							var effectPath = "FNA";
							if (!string.IsNullOrEmpty(assetManager.CurrentFolder))
							{
								effectPath = "../" + effectPath + assetManager.CurrentFolder + "/";
							}
							effectPath += Path.ChangeExtension(effectName, "efb");
							effect = assetManager.LoadEffect(graphicsService.GraphicsDevice, effectPath);
						}
						binding = new EffectBinding(graphicsService, effect, opaqueData);
						break;
				}

				material[pass] = binding;
			}

			return material;
		}
	}
}
