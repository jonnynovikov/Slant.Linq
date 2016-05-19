using Monad.Parsec;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Monad;
using Monad.Utility;


namespace Slant.Monad.Tests
{
    public class ImmutableListTests
    {
        [Fact]
        public void EnumeratorTest1()
        {
            var list1 = new ImmutableList<int>(new int[] { 0, 1, 2 });

            int index = 0;
            foreach (var item in list1)
            {
                item.Should().Be(index);
                index++;
            }
            index.Should().Be(3);
        }

        [Fact]
        public void EnumeratorTest2()
        {
            var list1 = new ImmutableList<int>(new int[] { 0, 1, 2 });
            var list2 = new ImmutableList<int>(new int[] { 3, 4, 5 });

            var list = list1.Concat(list2);

            int index = 0;
            foreach (var item in list)
            {
                item.Should().Be(index);
                index++;
            }

            index.Should().Be(6);
        }

        [Fact]
        public void EnumeratorTest3()
        {
            var list1 = new ImmutableList<int>(new int[] { 1, 2, 3 });
            var list2 = new ImmutableList<int>(new int[] { 4, 5, 6 });

            var list = 0.Cons( list1.Concat(list2) );

            int index = 0;
            foreach (var item in list)
            {
                item.Should().Be(index);
                index++;
            }

            index.Should().Be(7);
        }

        [Fact]
        public void EnumeratorLengthTest1()
        {
            var list1 = new ImmutableList<int>(new int[] { 0, 1, 2 });
            var list2 = new ImmutableList<int>(new int[] { 3, 4, 5 });

            var list = list1.Concat(list2);

            list.Should().HaveCount(6);

            list = list.Tail();
            list.Should().HaveCount(5);

            list = list.Tail();
            list = list.Tail();
            list = list.Tail();
            list = list.Tail();

            list.Should().HaveCount(1);
            list.IsAlmostEmpty.Should().BeTrue();

            list = list.Tail();
            list.IsEmpty.Should().BeTrue();

            Action act = () => list.Tail();
            act.ShouldThrow<ParserException>();
        }
    }
}
