#region [R# naming]

// ReSharper disable ArrangeTypeModifiers
// ReSharper disable UnusedMember.Local
// ReSharper disable FieldCanBeMadeReadOnly.Local
// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable InconsistentNaming

#endregion

using NUnit.Framework;
using Monad.Parsec;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Monad;

namespace Monad.Specs
{
    public class ParsecSuite
    {
        [Test]
        public void TestOR()
        {
            var p =
                (from fst in Prim.String("robert")
                    select fst)
                    .Or(from snd in Prim.String("jimmy")
                        select snd)
                    .Or(from thrd in Prim.String("john paul")
                        select thrd)
                    .Or(from fth in Prim.String("john")
                        select fth);

            var r = p.Parse("robert");
            (!r.IsFaulted && r.Value.Single().Item1.IsEqualTo("robert")).Should().BeTrue();
            r = p.Parse("jimmy");
            (!r.IsFaulted && r.Value.Single().Item1.IsEqualTo("jimmy")).Should().BeTrue();
            r = p.Parse("john paul");
            (!r.IsFaulted && r.Value.Single().Item1.IsEqualTo("john paul")).Should().BeTrue();
            r = p.Parse("john");
            (!r.IsFaulted && r.Value.Single().Item1.IsEqualTo("john")).Should().BeTrue();
        }

        [Test]
        public void TestBinding()
        {
            var p = from x in Prim.Item()
                from _ in Prim.Item()
                from y in Prim.Item()
                select new[] {x, y};

            var res = p.Parse("abcdef").Value.Single();

            (res.Item1.First().Value == 'a' &&
             res.Item1.Second().Value == 'c').Should().BeTrue();

            res.Item1.First().Location.Line.Should().Be(1);
            res.Item1.First().Location.Column.Should().Be(1);

            res.Item1.Second().Location.Line.Should().Be(1);
            res.Item1.Second().Location.Column.Should().Be(3);

            bool found = p.Parse("ab").Value.IsEmpty;

            found.Should().BeTrue();
        }

        [Test]
        public void TestInteger()
        {
            var p = Prim.Integer();

            (!p.Parse("123").IsFaulted && p.Parse("123").Value.Single().Item1 == 123).Should().BeTrue();
            (!p.Parse("-123").IsFaulted && p.Parse("-123").Value.Single().Item1 == -123).Should().BeTrue();
            (!p.Parse(int.MaxValue.ToString()).IsFaulted &&
             p.Parse(int.MaxValue.ToString()).Value.Single().Item1 == int.MaxValue).Should().BeTrue();

            // Bug here in both .NET and Mono, neither can parse an Int32.MinValue, overflow exception is thrown.
            //Assert.True(!p.Parse(int.MinValue.ToString()).IsFaulted && p.Parse(int.MinValue.ToString()).Value.Single().Item1 == int.MinValue);
        }

        [Test]
        public void TestDigitList()
        {
            var p = from open in Prim.Character('[')
                from d in Prim.Digit()
                from ds in
                    Prim.Many(
                        from comma in Prim.Character(',')
                        from digit in Prim.Digit()
                        select digit
                        )
                from close in Prim.Character(']')
                select d.Cons(ds);

            var r = p.Parse("[1,2,3,4]").Value.Single();

            (r.Item1.First().Value == '1').Should().BeTrue();
            (r.Item1.Skip(1).First().Value == '2').Should().BeTrue();
            (r.Item1.Skip(2).First().Value == '3').Should().BeTrue();
            (r.Item1.Skip(3).First().Value == '4').Should().BeTrue();

            var r2 = p.Parse("[1,2,3,4");
            r2.IsFaulted.Should().BeTrue();

            r2.Errors.First().Expected.Should().Be("']'");
            r2.Errors.First().Input.IsEmpty.Should().BeTrue();

            var r3 = p.Parse("[1,2,3,4*");

            r3.IsFaulted.Should().BeTrue();
            r3.Errors.First().Expected.Should().Be("']'");
            r3.Errors.First().Location.Line.Should().Be(1);
            r3.Errors.First().Location.Column.Should().Be(9);
        }

        [Test]
        public void TestString()
        {
            var r = Prim.String("he").Parse("hell").Value.Single();
            r.Item1.AsString().Should().Be("he");
            r.Item2.AsString().Should().Be("ll");

            r = Prim.String("hello").Parse("hello, world").Value.Single();
            r.Item1.AsString().Should().Be("hello");
            r.Item2.AsString().Should().Be(", world");
        }

        [Test]
        public void TestMany()
        {
            var r = Prim.Many(Prim.Character('a')).Parse("aaabcde").Value.Single();
            r.Item1.AsString().Should().Be("aaa");
            r.Item2.AsString().Should().Be("bcde");
        }

        [Test]
        public void TestMany1()
        {
            var r = Prim.Many1(Prim.Character('a')).Parse("aaabcde").Value.Single();
            r.Item1.AsString().Should().Be("aaa");
            r.Item2.AsString().Should().Be("bcde");

            var r2 = Prim.Many1(Prim.Character('a')).Parse("bcde");
            r2.Value.IsEmpty.Should().BeTrue();
        }

        [Test]
        public void TestSkipMany1()
        {
            var p = Prim.SkipMany1(Prim.Character('*'));

            var r = p.Parse("****hello, world");
            r.IsFaulted.Should().BeFalse();

            var after = r.Value.Head().Item2.AsString();
            after.Should().Be("hello, world");

            r = p.Parse("*hello, world");
            r.IsFaulted.Should().BeFalse();

            after = r.Value.Head().Item2.AsString();
            after.Should().Be("hello, world");

            r = p.Parse("hello, world");
            r.IsFaulted.Should().BeTrue();
        }

        [Test]
        public void TestSkipMany()
        {
            var p = Prim.SkipMany(Prim.Character('*'));

            var r = p.Parse("****hello, world");
            r.IsFaulted.Should().BeFalse();

            var after = r.Value.Head().Item2.AsString();
            after.Should().Be("hello, world");

            r = p.Parse("*hello, world");
            r.IsFaulted.Should().BeFalse();

            after = r.Value.Head().Item2.AsString();
            after.Should().Be("hello, world");

            r = p.Parse("hello, world");
            r.IsFaulted.Should().BeFalse();

            after = r.Value.Head().Item2.AsString();
            after.Should().Be("hello, world");
        }

        [Test]
        public void TestOneOf()
        {
            var p = Prim.OneOf("xyz");
            var r = p.Parse("zzz");
            (!r.IsFaulted && r.Value.Head().Item1.Value == 'z').Should().BeTrue();
            r = p.Parse("xxx");
            (!r.IsFaulted && r.Value.Head().Item1.Value == 'x').Should().BeTrue();
            r = p.Parse("www");
            (r.IsFaulted).Should().BeTrue();
        }

        [Test]
        public void TestNoneOf()
        {
            var p = Prim.NoneOf("xyz");
            var r = p.Parse("zzz");
            (r.IsFaulted).Should().BeTrue();
            r = p.Parse("xxx");
            (r.IsFaulted).Should().BeTrue();
            r = p.Parse("www");
            (!r.IsFaulted && r.Value.Head().Item1.Value == 'w' && r.Value.Head().Item2.AsString() == "ww").Should()
                .BeTrue();
        }

        [Test]
        public void TestDigit()
        {
            var r = Prim.Digit().Parse("1").Value.Single();
            r.Item1.Value.Should().Be('1');
        }

        [Test]
        public void TestChar()
        {
            var r = Prim.Character('X').Parse("X").Value.Single();
            r.Item1.Value.Should().Be('X');
        }

        [Test]
        public void TestSatisfy()
        {
            var r = Prim.Satisfy(c => c == 'x', "'x'").Parse("xbxcxdxe").Value.Single();
            r.Item1.Value.Should().Be('x');
            r.Item2.AsString().Should().Be("bxcxdxe");
        }

        [Test]
        public void TestItem()
        {
            Prim.Item().Parse("").Value.IsEmpty.Should().BeTrue();

            var r = Prim.Item().Parse("abc").Value.Single();
            (r.Item1.Value == 'a' &&
             r.Item2.AsString() == "bc").Should().BeTrue();
        }

        [Test]
        public void TestFailure()
        {
            var inp = "abc".ToParserChar();

            var parser = Prim.Failure<bool>(ParserError.Create("failed because...", inp));

            var result = parser.Parse(inp);

            result.Value.IsEmpty.Should().BeTrue();
        }

        [Test]
        public void TestReturn()
        {
            var r = Prim.Return(1).Parse("abc").Value.Single();
            (r.Item1 == 1 &&
             r.Item2.AsString() == "abc").Should().BeTrue();
        }

        [Test]
        public void TestChoice()
        {
            var r = Prim.Choice(Prim.Item(), Prim.Return(Prim.ParserChar('d'))).Parse("abc").Value.Single();
            (r.Item1.Value == 'a' &&
             r.Item2.AsString() == "bc").Should().BeTrue();

            var inp = "abc".ToParserChar();

            var parser = Prim.Choice(
                Prim.Failure<ParserChar>(ParserError.Create("failed because...", inp)),
                Prim.Return(Prim.ParserChar('d'))
                )
                .Parse(inp);

            r = parser.Value.Single();

            (r.Item1.Value == 'd' &&
             r.Item2.AsString() == "abc").Should().BeTrue();
        }

        [Test]
        public void TestWhiteSpace()
        {
            var r = Prim.WhiteSpace().Parse(" ");
            (r.IsFaulted).Should().BeFalse();
            (r.Value.Count() == 1).Should().BeTrue();
        }

        [Test]
        public void TestWhiteSpace2()
        {
            var r = Prim.WhiteSpace().Parse("a");
            r.IsFaulted.Should().BeFalse();

            r.Value.Should().HaveCount(1);

            var empty = r.Value.Count() == 1;
            empty.Should().BeTrue();
            r.Value.Single().Item2.AsString().Should().Be("a");
        }

        [Test]
        public void TestBetween()
        {
            var r = Prim.Between(Prim.Character('['), Prim.Character(']'), Prim.String("abc")).Parse("[abc]");
            (!r.IsFaulted).Should().BeTrue();
            (r.Value.First().Item1.AsString() == "abc").Should().BeTrue();
        }
    }
}