using DigitalRune.Mathematics.Algebra;

namespace DigitalRune.Character.Skeleton_Animations
{
	/// <summary>
	/// TODO: Make internal after merging Animation and Graphics
	/// </summary>
	public class SkeletonKeyFrameAnimationData
	{
		public float[] Times;
		public Vector3F[] Translations;
		public QuaternionF[] Rotations;
		public Vector3F[] Scales;
	}
}
