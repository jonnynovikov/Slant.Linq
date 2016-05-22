#region [R# naming]

// ReSharper disable ArrangeTypeModifiers
// ReSharper disable UnusedMember.Local
// ReSharper disable FieldCanBeMadeReadOnly.Local
// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable InconsistentNaming

#endregion

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Monad;
using Monad.Utility;

namespace Monad.Specs
{
    public class OptionSuite
    {
        [Test]
        public void TestBinding()
        {
            Option<int> option = () => 1000.ToOption();
            Option<int> option2 = () => 2000.ToOption();

            var result = from o in option
                select o;

            (result.HasValue() && result.Value() == 1000).Should().BeTrue();
            (result.Match(Just: () => true, Nothing: () => false)()).Should().BeTrue();
            (result.Match(Just: () => true, Nothing: false)()).Should().BeTrue();

            result = from o in option
                from o2 in option2
                select o2;

            (result.HasValue() && result.Value() == 2000).Should().BeTrue();
            (result.Match(Just: () => true, Nothing: () => false)()).Should().BeTrue();
            (result.Match(Just: () => true, Nothing: false)()).Should().BeTrue();

            result = from o in option
                from o2 in Nothing()
                select o2;

            result.HasValue().Should().BeFalse();
        }

        [Test]
        public void TestEquals()
        {
            OptionStrict<int> option = 1000.ToOptionStrict();
            OptionStrict<int> option2 = 1000.ToOptionStrict();

            option.Should().Be(option2);
        }

        public Option<int> Nothing()
        {
            return Option.Nothing<int>();
        }
    }
}