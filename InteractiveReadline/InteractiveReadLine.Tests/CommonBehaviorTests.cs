using System;
using System.Text;
using InteractiveReadLine.KeyBehaviors;
using Moq;
using Xunit;

namespace InteractiveReadLine.Tests
{
    public class CommonBehaviorTests
    {
        [Theory]
        [InlineData("test", 0, "est")]
        [InlineData("test", 1, "tst")]
        [InlineData("test", 2, "tet")]
        [InlineData("test", 3, "tes")]
        [InlineData("test", 4, "test")]
        public void DeleteAtCursor_Works(string text, int cursor, string expected)
        {
            var buffer = new StringBuilder(text);
            var mock = new Mock<IKeyBehaviorTarget>();
            mock.SetupGet(x => x.TextBuffer).Returns(buffer);
            mock.SetupProperty(x => x.CursorPosition);
            mock.Object.CursorPosition = cursor;

            CommonBehaviors.Delete(mock.Object);

            Assert.Equal(cursor, mock.Object.CursorPosition);
            Assert.Equal(expected, buffer.ToString());
        }


        [Theory]
        [InlineData("test", 0, "test", 0)]
        [InlineData("test", 1, "est", 0)]
        [InlineData("test", 2, "tst", 1)]
        [InlineData("test", 3, "tet", 2)]
        [InlineData("test", 4, "tes", 3)]
        public void BackspaceAtCursor_Works(string text, int cursor, string expected, int expectedCursor)
        {
            var buffer = new StringBuilder(text);
            var mock = new Mock<IKeyBehaviorTarget>();
            mock.SetupGet(x => x.TextBuffer).Returns(buffer);
            mock.SetupProperty(x => x.CursorPosition);
            mock.Object.CursorPosition = cursor;

            CommonBehaviors.Backspace(mock.Object);

            Assert.Equal(expectedCursor, mock.Object.CursorPosition);
            Assert.Equal(expected, buffer.ToString());
        }

        [Fact]
        public void CursorMoveToEnd_Works()
        {
            var buffer = new StringBuilder("this is a test");
            var mock = new Mock<IKeyBehaviorTarget>();
            mock.SetupGet(x => x.TextBuffer).Returns(buffer);
            mock.SetupProperty(x => x.CursorPosition);
            mock.Object.CursorPosition = 4;

            CommonBehaviors.MoveCursorToEnd(mock.Object);

            Assert.Equal(14, mock.Object.CursorPosition);
        }
        
        [Fact]
        public void CursorMoveToStart_Works()
        {
            var buffer = new StringBuilder("this is a test");
            var mock = new Mock<IKeyBehaviorTarget>();
            mock.SetupGet(x => x.TextBuffer).Returns(buffer);
            mock.SetupProperty(x => x.CursorPosition);
            mock.Object.CursorPosition = 14;

            CommonBehaviors.MoveCursorToStart(mock.Object);

            Assert.Equal(0, mock.Object.CursorPosition);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(4, 3)]
        [InlineData(2, 1)]
        public void MoveCursorLeft_Works(int cursor, int expected)
        {
            var buffer = new StringBuilder("test");
            var mock = new Mock<IKeyBehaviorTarget>();
            mock.SetupGet(x => x.TextBuffer).Returns(buffer);
            mock.SetupProperty(x => x.CursorPosition);
            mock.Object.CursorPosition = cursor;

            CommonBehaviors.MoveCursorLeft(mock.Object);

            Assert.Equal(expected, mock.Object.CursorPosition);
        }


        [Theory]
        [InlineData(0, 1)]
        [InlineData(4, 4)]
        [InlineData(2, 3)]
        public void MoveCursorRight_Works(int cursor, int expected)
        {
            var buffer = new StringBuilder("test");
            var mock = new Mock<IKeyBehaviorTarget>();
            mock.SetupGet(x => x.TextBuffer).Returns(buffer);
            mock.SetupProperty(x => x.CursorPosition);
            mock.Object.CursorPosition = cursor;

            CommonBehaviors.MoveCursorRight(mock.Object);

            Assert.Equal(expected, mock.Object.CursorPosition);
        }

        [Fact]
        public void ClearBuffer_Works()
        {
            var buffer = new StringBuilder("test");
            var mock = new Mock<IKeyBehaviorTarget>();
            mock.SetupGet(x => x.TextBuffer).Returns(buffer);
            mock.SetupProperty(x => x.CursorPosition);
            mock.Object.CursorPosition = 4;

            CommonBehaviors.ClearAll(mock.Object);

            Assert.Equal("", buffer.ToString());
            Assert.Equal(0, mock.Object.CursorPosition);
        }

        [Theory]
        [InlineData('A', 0, "Atest")]
        [InlineData('9', 2, "te9st")]
        [InlineData('0', 4, "test0")]
        [InlineData(' ', 0, " test")]
        [InlineData(' ', 2, "te st")]
        [InlineData(' ', 4, "test ")]
        [InlineData('.', 0, ".test")]
        [InlineData(';', 2, "te;st")]
        [InlineData(',', 4, "test,")]
        [InlineData('*', 0, "*test")]
        [InlineData('%', 2, "te%st")]
        [InlineData('$', 4, "test$")]
        public void InsertCharacters_Works(char c, int cursor, string expected)
        {
            var buffer = new StringBuilder("test");
            var mock = new Mock<IKeyBehaviorTarget>();
            mock.SetupGet(x => x.TextBuffer).Returns(buffer);
            mock.SetupProperty(x => x.CursorPosition);
            mock.SetupGet(x => x.ReceivedKey).Returns(new ConsoleKeyInfo(c, ConsoleKey.Oem1, false, false, false));
            mock.Object.CursorPosition = cursor;

            CommonBehaviors.InsertCharacter(mock.Object);

            Assert.Equal(expected, buffer.ToString());
            Assert.Equal(cursor + 1, mock.Object.CursorPosition);
        }

        [Theory]
        [InlineData(0, "")]
        [InlineData(3, "tes")]
        [InlineData(8, "test dat")]
        [InlineData(9, "test data")]
        public void CutToEnd_Works(int cursor, string expected)
        {
            //                              0123456789
            var buffer = new StringBuilder("test data");
            var mock = new Mock<IKeyBehaviorTarget>();
            mock.SetupGet(x => x.TextBuffer).Returns(buffer);
            mock.SetupProperty(x => x.CursorPosition);
            mock.Object.CursorPosition = cursor;

            CommonBehaviors.CutToEnd(mock.Object);

            Assert.Equal(expected, buffer.ToString());
            Assert.Equal(cursor, mock.Object.CursorPosition);
        }

        [Theory]
        [InlineData(0, "test data")]
        [InlineData(3, "t data")]
        [InlineData(8, "a")]
        [InlineData(9, "")]
        public void CutToStart_Works(int cursor, string expected)
        {
            //                              0123456789
            var buffer = new StringBuilder("test data");
            var mock = new Mock<IKeyBehaviorTarget>();
            mock.SetupGet(x => x.TextBuffer).Returns(buffer);
            mock.SetupProperty(x => x.CursorPosition);
            mock.Object.CursorPosition = cursor;

            CommonBehaviors.CutToStart(mock.Object);

            Assert.Equal(expected, buffer.ToString());
            Assert.Equal(0, mock.Object.CursorPosition);
        }

        [Theory]
        [InlineData(0, "test data here ", 0)]
        [InlineData(2, "st data here ", 0)]
        [InlineData(4, " data here ", 0)]
        [InlineData(5, "data here ", 0)]
        [InlineData(7, "test ta here ", 5)]
        [InlineData(10, "test here ", 5)]
        [InlineData(15, "test data ", 10)]
        public void CutPreviousWord_WithNoTokens_Works(int cursor, string expected, int expectedCursor)
        {
            //                              0123456789012345
            var buffer = new StringBuilder("test data here ");
            var mock = new Mock<IKeyBehaviorTarget>();
            mock.SetupGet(x => x.TextBuffer).Returns(buffer);
            mock.SetupProperty(x => x.CursorPosition);
            mock.Object.CursorPosition = cursor;

            CommonBehaviors.CutPreviousWord(mock.Object);

            Assert.Equal(expected, buffer.ToString());
            Assert.Equal(expectedCursor, mock.Object.CursorPosition);
        }
    }
}