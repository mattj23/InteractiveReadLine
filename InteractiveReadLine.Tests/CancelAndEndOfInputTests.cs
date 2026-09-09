using System;
using System.Collections.Generic;
using InteractiveReadLine.KeyBehaviors;
using InteractiveReadLine.Tests.Fakes;
using Xunit;

namespace InteractiveReadLine.Tests
{
    public class CancelAndEndOfInputTests
    {
        [Fact]
        public void Enter_ProducesALineResult()
        {
            var keys = KeyBuilder.Create().Add("some text").Enter().Keys;
            var result = new ConsoleReadLine(new TestConsole(10, 80, keys)).Read(ReadLineConfig.Basic);

            Assert.Equal(ReadLineResultKind.Line, result.Kind);
            Assert.True(result.IsLine);
            Assert.Equal("some text", result.Text);
        }

        [Fact]
        public void CtrlC_ProducesACancelledResult()
        {
            var keys = KeyBuilder.Create().Add("abandon me").CtrlC().Keys;
            var result = new ConsoleReadLine(new TestConsole(10, 80, keys)).Read(ReadLineConfig.Basic);

            Assert.Equal(ReadLineResultKind.Cancelled, result.Kind);
            Assert.False(result.IsLine);
            Assert.Null(result.Text);
        }

        /// <summary>
        /// The string API converts a canceled line to an empty string. In a read-evaluate loop, this behaves as
        /// a no-op and does not terminate the loop as a null result would.
        /// </summary>
        [Fact]
        public void CtrlC_FlattensToAnEmptyString()
        {
            var keys = KeyBuilder.Create().Add("abandon me").CtrlC().Keys;
            var result = new ConsoleReadLine(new TestConsole(10, 80, keys)).ReadLine(ReadLineConfig.Basic);

            Assert.Equal(string.Empty, result);
        }

        [Fact]
        public void CtrlC_DoesNotRecordAnythingInTheHistory()
        {
            var history = new List<string> {"existing entry"};
            var config = ReadLineConfig.Basic.SetUpdatingHistorySource(history);
            var keys = KeyBuilder.Create().Add("abandon me").CtrlC().Keys;

            new ConsoleReadLine(new TestConsole(10, 80, keys)).Read(config);

            Assert.Single(history);
        }

        [Fact]
        public void CtrlD_OnAnEmptyLine_ProducesAnEndOfInputResult()
        {
            var config = ReadLineConfig.Basic.AddCtrlNavKeys();
            var keys = KeyBuilder.Create().CtrlD().Keys;
            var result = new ConsoleReadLine(new TestConsole(10, 80, keys)).Read(config);

            Assert.Equal(ReadLineResultKind.EndOfInput, result.Kind);
            Assert.Null(result.Text);
        }

        /// <summary>
        /// The string API converts the end-of-input result to null, matching both GNU Readline and
        /// Console.ReadLine, so that a "while the line is not null" loop terminates on Ctrl+D.
        /// </summary>
        [Fact]
        public void CtrlD_OnAnEmptyLine_FlattensToNull()
        {
            var config = ReadLineConfig.Basic.AddCtrlNavKeys();
            var keys = KeyBuilder.Create().CtrlD().Keys;
            var result = new ConsoleReadLine(new TestConsole(10, 80, keys)).ReadLine(config);

            Assert.Null(result);
        }

        [Fact]
        public void CtrlD_OnAnEmptyLine_DoesNotRecordAnythingInTheHistory()
        {
            var history = new List<string> {"existing entry"};
            var config = ReadLineConfig.Basic.AddCtrlNavKeys().SetUpdatingHistorySource(history);
            var keys = KeyBuilder.Create().CtrlD().Keys;

            new ConsoleReadLine(new TestConsole(10, 80, keys)).Read(config);

            Assert.Single(history);
        }

        /// <summary>
        /// With text in the buffer, Ctrl+D deletes forward instead of ending the input, matching shell behavior.
        /// </summary>
        [Fact]
        public void CtrlD_WithTextInTheBuffer_DeletesForwardAndContinues()
        {
            var config = ReadLineConfig.Basic.AddCtrlNavKeys();
            var keys = KeyBuilder.Create().Add("abc").LeftArrow(3).CtrlD().Enter().Keys;
            var result = new ConsoleReadLine(new TestConsole(10, 80, keys)).Read(config);

            Assert.Equal(ReadLineResultKind.Line, result.Kind);
            Assert.Equal("bc", result.Text);
        }

        [Fact]
        public void BasicConfiguration_BindsCtrlC()
        {
            var config = ReadLineConfig.Basic;

            Assert.True(config.KeyBehaviors.ContainsKey(new KeyId(ConsoleKey.C, true, false, false)));
        }

        [Fact]
        public void Provider_EnablesTreatControlCAsInput_AndRestoresItOnDispose()
        {
            var console = new TestConsole(10, 80) {TreatControlCAsInput = false};
            var provider = new ConsoleReadLine(console);

            Assert.True(console.TreatControlCAsInput);

            provider.Dispose();

            Assert.False(console.TreatControlCAsInput);
        }

        [Fact]
        public void Provider_RestoresTreatControlCAsInput_WhenItWasAlreadyEnabled()
        {
            var console = new TestConsole(10, 80) {TreatControlCAsInput = true};
            var provider = new ConsoleReadLine(console);
            provider.Dispose();

            Assert.True(console.TreatControlCAsInput);
        }

        [Theory]
        [InlineData("")]
        [InlineData("some text")]
        public void ForLine_RoundTripsThroughToText(string text)
        {
            var result = ReadLineResult.ForLine(text);

            Assert.Equal(text, result.ToText());
            Assert.True(result.IsLine);
        }

        [Fact]
        public void ToText_MapsCancelledToEmptyAndEndOfInputToNull()
        {
            Assert.Equal(string.Empty, ReadLineResult.Cancelled.ToText());
            Assert.Null(ReadLineResult.EndOfInput.ToText());
        }

        /// <summary>
        /// The result type distinguishes an empty line that the user entered from a line that they abandoned.
        /// </summary>
        [Fact]
        public void AnEnteredEmptyLine_IsDistinctFromACancelledOne()
        {
            var entered = new ConsoleReadLine(new TestConsole(10, 80, KeyBuilder.Create().Enter().Keys))
                .Read(ReadLineConfig.Basic);
            var cancelled = new ConsoleReadLine(new TestConsole(10, 80, KeyBuilder.Create().CtrlC().Keys))
                .Read(ReadLineConfig.Basic);

            Assert.Equal(string.Empty, entered.ToText());
            Assert.Equal(string.Empty, cancelled.ToText());

            Assert.NotEqual(entered.Kind, cancelled.Kind);
            Assert.True(entered.IsLine);
            Assert.False(cancelled.IsLine);
        }
    }
}
