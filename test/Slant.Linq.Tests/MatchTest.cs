using NUnit.Framework;
using static Slant.API;

namespace Slant.Linq.Tests
{
    [TestFixture]
    public class MatchTest
    {
        [Test]
        public void ShouldThrowIfNotMatching()
        {
            Match(new object()).of(
                    Case(ignored => false, o => null)
            );
        }

    }
}