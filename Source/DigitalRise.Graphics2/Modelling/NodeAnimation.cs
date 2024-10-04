﻿using Microsoft.Xna.Framework;
using System;

namespace DigitalRise.Modelling
{
	public class NodeAnimation
	{
		public ModelNode Node { get; }
		public AnimationTransforms<Vector3> Translations { get; } = new AnimationTransformsVector3();
		public AnimationTransforms<Vector3> Scales { get; } = new AnimationTransformsVector3();
		public AnimationTransforms<Quaternion> Rotations { get; } = new AnimationTransformsQuaternion();

		public NodeAnimation(ModelNode node)
		{
			Node = node ?? throw new ArgumentNullException(nameof(node));
		}
	}
}
