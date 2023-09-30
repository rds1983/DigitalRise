using NUnit.Framework;


namespace DigitalRise.Animation.Tests
{
  [TestFixture]
  public class ReadOnlyAnimationInstanceCollectionTest
  {
    [Test]
    public void ShouldThrowOnInsert()
    {
      var collection = ReadOnlyAnimationInstanceCollection.Instance;
      Assert.That(() => collection.Add(null), Throws.InvalidOperationException);
      Assert.That(() => collection[0] = null, Throws.Exception);
    }
  }
}
