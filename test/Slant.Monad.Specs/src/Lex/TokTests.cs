#region [R# naming]
// ReSharper disable ArrangeTypeModifiers
// ReSharper disable UnusedMember.Local
// ReSharper disable FieldCanBeMadeReadOnly.Local
// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable InconsistentNaming
#endregion
using FluentAssertions;
using Monad;
using Monad.Parsec;
using Monad.Parsec.Language;
using NUnit.Framework;

namespace Slant.Monad.Specs.Lex
{
    public class TokTests
    {
        [Test]
        public void LexemeTest()
        {
            var lex = from l in Tok.Lexeme<ParserChar>(Prim.Character('A'))
                               select l;

            var res = lex.Parse("A");
            res.IsFaulted.Should().BeFalse();

            res = lex.Parse("A   ");
            res.IsFaulted.Should().BeFalse();
        }

        [Test]
        public void SymbolTest()
        {
            var sym = from s in Tok.Symbol("***")
                               select s;

            var res = sym.Parse("***   ");

            res.IsFaulted.Should().BeFalse();
        }

        [Test]
        public void OneLineComment()
        {
            var p = from v in Tok.OneLineComment(new HaskellDef())
                    select v;

            var res = p.Parse("-- This whole line is a comment");

            res.IsFaulted.Should().BeFalse();
        }


        [Test]
        public void MultiLineComment()
        {
            var p = from v in Tok.MultiLineComment(new HaskellDef())
                    select v;

            var res = p.Parse(
                @"{- This whole {- line is a comment
                     and so is -} this one with nested comments
                     this too -}  let x=1");

            var left = res.Value.Head().Item2.AsString();

            (!res.IsFaulted &&  left == "  let x=1").Should().BeTrue();
        }

        [Test]
        public void WhiteSpaceTest()
        {
            var p = from v in Tok.WhiteSpace(new HaskellDef())
                    select v;

            var res = p.Parse(
                @"                              {- This whole {- line is a comment
                                                and so is -} this one with nested comments
                                                this too -}  let x=1");

            var left = res.Value.Head().Item2.AsString();

            (!res.IsFaulted && left == "let x=1").Should().BeTrue();
        }

        [Test]
        public void CharLiteralTest()
        {
            var p = from v in Tok.Chars.CharLiteral()
                    select v;

            var res = p.Parse("'a'  abc");
            var left = res.Value.Head().Item2.AsString();

            (!res.IsFaulted && left == "abc").Should().BeTrue();
            (res.Value.Head().Item1.Value.Value == 'a').Should().BeTrue();

            res = p.Parse("'\\n'  abc");
            (res.Value.Head().Item1.Value.Value == '\n').Should().BeTrue();
        }

        [Test]
        public void StringLiteralTest()
        {
            var p = from v in Tok.Strings.StringLiteral()
                    select v;

            var res = p.Parse("\"abc\"  def");
            var left = res.Value.Head().Item2.AsString();

            (!res.IsFaulted && left == "def").Should().BeTrue();
            (res.Value.Head().Item1.Value.AsString() == "abc").Should().BeTrue();

            res = p.Parse("\"ab\\t\\nc\"  def");
            (res.Value.Head().Item1.Value.AsString() == "ab\t\nc").Should().BeTrue();
        }


        [Test]
        public void NumbersIntegerTest()
        {
            var p = from v in Tok.Numbers.Integer()
                    select v;

            var res = p.Parse("1234  def");
            var left = res.Value.Head().Item2.AsString();

            (!res.IsFaulted && left == "def").Should().BeTrue();
            (res.Value.Head().Item1.Value == 1234).Should().BeTrue();
        }

        [Test]
        public void NumbersHexTest()
        {
            var p = from v in Tok.Numbers.Hexadecimal()
                    select v;

            var res = p.Parse("xAB34");
            var left = res.Value.Head().Item2.AsString();

            (!res.IsFaulted).Should().BeTrue();
            (res.Value.Head().Item1.Value == 0xAB34).Should().BeTrue();
        }

        [Test]
        public void NumbersOctalTest()
        {
            var p = from v in Tok.Numbers.Octal()
                    select v;

            var res = p.Parse("o777");
            var left = res.Value.Head().Item2.AsString();

            (!res.IsFaulted).Should().BeTrue();
            (res.Value.Head().Item1.Value == 511).Should().BeTrue();
        }

        [Test]
        public void TestParens()
        {
            var r = Tok.Bracketing.Parens(Tok.Chars.CharLiteral()).Parse("( 'a' )");
            (!r.IsFaulted).Should().BeTrue();
            (r.Value.Head().Item1.Value.Value == 'a').Should().BeTrue();
        }

        [Test]
        public void TestBraces()
        {
            var r = Tok.Bracketing.Braces(Tok.Chars.CharLiteral()).Parse("{ 'a' }");
            (!r.IsFaulted).Should().BeTrue();
            (r.Value.Head().Item1.Value.Value == 'a').Should().BeTrue();
        }

        [Test]
        public void TestBrackets()
        {
            var r = Tok.Bracketing.Brackets(Tok.Chars.CharLiteral()).Parse("[ 'a' ]");
            (!r.IsFaulted).Should().BeTrue();
            (r.Value.Head().Item1.Value.Value == 'a').Should().BeTrue();
        }

        [Test]
        public void TestAngles()
        {
            var r = Tok.Bracketing.Angles(Tok.Chars.CharLiteral()).Parse("< 'a' >");
            (!r.IsFaulted).Should().BeTrue();
            (r.Value.Head().Item1.Value.Value == 'a').Should().BeTrue();
        }

        [Test]
        public void TestIdentifier()
        {
            var r = Tok.Id.Identifier(new HaskellDef()).Parse("onetWothree123  ");
            (!r.IsFaulted).Should().BeTrue();
            (r.Value.Head().Item1.Value.AsString() == "onetWothree123").Should().BeTrue();
        }

        [Test]
        public void TestReserved()
        {
            var def = new HaskellDef();
            var r = Tok.Id.Reserved(def.ReservedNames.Head(), def).Parse(def.ReservedNames.Head() + "  ");
            (!r.IsFaulted).Should().BeTrue();
            (r.Value.Head().Item1.Value.AsString() == def.ReservedNames.Head()).Should().BeTrue();
        }

        [Test]
        public void TestOperator()
        {
            var def = new HaskellDef();
            var r = Tok.Ops.Operator(def).Parse("&&*  ");
            (!r.IsFaulted).Should().BeTrue();
            (r.Value.Head().Item1.Value.AsString() == "&&*").Should().BeTrue();
        }

        [Test]
        public void TestReservedOperator()
        {
            var def = new HaskellDef();
            var r = Tok.Ops.ReservedOp("=>",def).Parse("=>  ");
            (!r.IsFaulted).Should().BeTrue();
            (r.Value.Head().Item1.Value.AsString() == "=>").Should().BeTrue();
        }
    }
}

