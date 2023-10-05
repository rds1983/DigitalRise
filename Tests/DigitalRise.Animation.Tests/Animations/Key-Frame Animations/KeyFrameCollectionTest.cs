using System.Linq;
using Microsoft.Xna.Framework;
using NUnit.Framework;


namespace DigitalRise.Animation.Tests
{
  [TestFixture]
  public class KeyFrameCollectionTest
  {
    [Test]
    public void GetEnumeratorTest()
    {
      var collection = new KeyFrameCollection<Quaternion>();
      var keyFrame = new KeyFrame<Quaternion>();
      var keyFrame2 = new KeyFrame<Quaternion>();
      collection.Add(keyFrame);
      collection.Add(keyFrame2);

      foreach (var k in collection)
      { }

      Assert.AreEqual(2, collection.Count());
      Assert.Contains(keyFrame, collection);
      Assert.Contains(keyFrame2, collection);
    }
  }
}
