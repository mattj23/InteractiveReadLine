using System;
using InteractiveReadLine.Tests.Fakes;
using Xunit;

namespace InteractiveReadLine.Tests
{
    /// <summary>
    /// These tests validate that the TestConsole fake works the way it's expected to
    /// </summary>
    public class FakeConsoleValidationTests
    {
        [Fact]
        public void Buffer_StartsEmpty()
        {
            var height = 5;
            var test = new TestConsole(height, 10);

            for (int i = 0; i < height; i++)
            {
                Assert.Equal(string.Empty, test.GetRow(i));
            }
        }

        [Fact]
        public void SimpleWriteOneLine_MatchesExpected()
        {
            var test = new TestConsole(5, 10);
            test.Write("hello");

            Assert.Equal("hello", test.GetRow(0));
        }
        
        [Fact]
        public void WriteWithOverflow_MatchesExpected()
        {
            var test = new TestConsole(5, 10);
            test.Write("0123456789abcdefg");

            Assert.Equal("0123456789", test.GetRow(0));
            Assert.Equal("abcdefg", test.GetRow(1));
        }
        
        [Fact]
        public void WriteLineWithOverflow_MatchesExpected()
        {
            var test = new TestConsole(5, 10);
            test.WriteLine("0123456789abcdefg");
            test.Write("hello");

            Assert.Equal("0123456789", test.GetRow(0));
            Assert.Equal("abcdefg", test.GetRow(1));
            Assert.Equal("hello", test.GetRow(2));
        }

        [Fact]
        public void WriteManyLines_WithHeightOverflow_ShiftsRows()
        {
            var test = new TestConsole(5, 30);
            test.WriteLine("this is the first row");
            test.WriteLine("And this is the second");
            test.WriteLine("here is the third");
            test.WriteLine("and the fourth");
            test.WriteLine("but this is the last row");

            Assert.Equal("And this is the second", test.GetRow(0));
            Assert.Equal("here is the third", test.GetRow(1));
            Assert.Equal("and the fourth", test.GetRow(2));
            Assert.Equal("but this is the last row", test.GetRow(3));
            Assert.Equal(string.Empty, test.GetRow(4));
        }

        [Fact]
        public void FakeKeyBuilder_BuildsFromText_Simple()
        {
            var keys = KeyBuilder.Create().Add("aA3 _").Keys;
            
            Assert.Equal(new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false), keys[0]);
            Assert.Equal(new ConsoleKeyInfo('A', ConsoleKey.A, true, false, false), keys[1]);
            Assert.Equal(new ConsoleKeyInfo('3', ConsoleKey.D3, false, false, false), keys[2]);
            Assert.Equal(new ConsoleKeyInfo(' ', ConsoleKey.Spacebar, false, false, false), keys[3]);
            Assert.Equal(new ConsoleKeyInfo('_', ConsoleKey.Sleep, false, false, false), keys[4]);
        }

        [Fact]
        public void FakeKeyBuilder_BuildsFromText_WithSpecial()
        {
            var keys = KeyBuilder.Create()
                .Add("aA")
                .Add(ConsoleKey.W, false, false, true)
                .Add(ConsoleKey.Enter, false, true, false)
                .Keys;
            
            Assert.Equal(new ConsoleKeyInfo('a', ConsoleKey.A, false, false, false), keys[0]);
            Assert.Equal(new ConsoleKeyInfo('A', ConsoleKey.A, true, false, false), keys[1]);
            Assert.Equal(new ConsoleKeyInfo(char.MinValue, ConsoleKey.W, false, false, true), keys[2]);
            Assert.Equal(new ConsoleKeyInfo(char.MinValue, ConsoleKey.Enter, false, true, false), keys[3]);
        }


    }
}
