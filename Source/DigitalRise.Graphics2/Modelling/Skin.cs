﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace DigitalRise.Modelling
{
	public class Skin: ItemWithId
	{
		public List<ModelNode> JointNodes { get; } = new List<ModelNode>();
		public Matrix[] Transforms { get; set; }
	}
}