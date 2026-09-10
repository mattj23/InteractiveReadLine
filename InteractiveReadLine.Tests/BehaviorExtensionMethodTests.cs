using System;
using InteractiveReadLine.KeyBehaviors;
using InteractiveReadLine.Tests.Fakes;
using Xunit;

namespace InteractiveReadLine.Tests
{
    public class BehaviorExtensionMethodTests
    {
        [Theory]
        [InlineData(false, false, false)]
        [InlineData(true, false, false)]
        [InlineData(false, true, false)]
        [InlineData(false, false, true)]
        [InlineData(true, true, true)]
        public void AddKeyBehavior_WithModifiers_RegistersThoseModifiers(bool control, bool alt, bool shift)
        {
            var config = ReadLineConfig.Empty
                .AddKeyBehavior(ConsoleKey.X, control, alt, shift, t => { });

            var key = Assert.Single(config.KeyBehaviors.Keys);
            Assert.Equal(ConsoleKey.X, key.Key);
            Assert.Equal(control, key.HasCtrl);
            Assert.Equal(alt, key.HasAlt);
            Assert.Equal(shift, key.HasShift);
        }

        [Fact]
        public void AddKeyBehavior_WithControlModifier_FiresOnTheModifiedKey()
        {
            var fired = false;
            var config = ReadLineConfig.Basic
                .AddKeyBehavior(ConsoleKey.X, true, false, false, t => fired = true);

            // KeyBuilder.Add takes (key, shift, alt, control)
            var keys = KeyBuilder.Create().Add(ConsoleKey.X, false, false, true).Enter().Keys;
            new ConsoleReadLine(new TestConsole(10, 80, keys)).ReadLine(config);

            Assert.True(fired);
        }

        /// <summary>
        /// Regression test for dropped modifiers: a Ctrl+X binding used to register as a plain X, so typing
        /// an unmodified "x" would run the behavior instead of inserting the character.
        /// </summary>
        [Fact]
        public void AddKeyBehavior_WithControlModifier_DoesNotFireOnTheUnmodifiedKey()
        {
            var fired = false;
            var config = ReadLineConfig.Basic
                .AddKeyBehavior(ConsoleKey.X, true, false, false, t => fired = true);

            var keys = KeyBuilder.Create().Add("x").Enter().Keys;
            var result = new ConsoleReadLine(new TestConsole(10, 80, keys)).ReadLine(config);

            Assert.False(fired);
            Assert.Equal("x", result);
        }

        [Fact]
        public void AddKeyBehavior_RebindingAKeyBoundByBasic_DoesNotThrow()
        {
            // ReadLineConfig.Basic already binds DownArrow through AddUpDownHistoryNavigation. This is the
            // "Custom Key Behaviors" example in the README, which used to throw an ArgumentException.
            var exception = Record.Exception(() =>
                ReadLineConfig.Basic.AddKeyBehavior(ConsoleKey.DownArrow, t => { }));

            Assert.Null(exception);
        }

        [Fact]
        public void AddKeyBehavior_RebindingAKey_ReplacesTheEarlierBehavior()
        {
            var firstFired = false;
            var secondFired = false;

            var config = ReadLineConfig.Empty
                .SetDefaultKeyBehavior(CommonKeyBehaviors.InsertCharacter)
                .AddEnterToFinish()
                .AddKeyBehavior(ConsoleKey.F1, t => firstFired = true)
                .AddKeyBehavior(ConsoleKey.F1, t => secondFired = true);

            var keys = KeyBuilder.Create().Add(ConsoleKey.F1, false, false, false).Enter().Keys;
            new ConsoleReadLine(new TestConsole(10, 80, keys)).ReadLine(config);

            Assert.False(firstFired);
            Assert.True(secondFired);

            // Rebinding replaced the entry, so only Enter and F1 are registered.
            Assert.Equal(2, config.KeyBehaviors.Count);
        }

        [Fact]
        public void AddKeyBehavior_OverridingADefaultBinding_TakesEffect()
        {
            var config = ReadLineConfig.Basic
                .AddKeyBehavior(ConsoleKey.DownArrow, CommonKeyBehaviors.InsertCharacter);

            var keys = KeyBuilder.Create().Add("ab").DownArrow().Enter().Keys;
            var result = new ConsoleReadLine(new TestConsole(10, 80, keys)).ReadLine(config);

            // DownArrow now runs InsertCharacter instead of navigating history. The key carries no character,
            // so the buffer is unchanged. History navigation therefore does not clear the buffer.
            Assert.Equal("ab", result);
        }

        [Fact]
        public void AddCtrlKeyBehavior_RegistersOnlyTheControlModifier()
        {
            var config = ReadLineConfig.Empty.AddCtrlKeyBehavior(ConsoleKey.W, t => { });

            var key = Assert.Single(config.KeyBehaviors.Keys);
            Assert.Equal(ConsoleKey.W, key.Key);
            Assert.True(key.HasCtrl);
            Assert.False(key.HasAlt);
            Assert.False(key.HasShift);
        }

        [Fact]
        public void AddKeyBehavior_ByConsoleKey_RegistersNoModifiers()
        {
            var config = ReadLineConfig.Empty.AddKeyBehavior(ConsoleKey.Home, t => { });

            var key = Assert.Single(config.KeyBehaviors.Keys);
            Assert.Equal(ConsoleKey.Home, key.Key);
            Assert.False(key.HasCtrl);
            Assert.False(key.HasAlt);
            Assert.False(key.HasShift);
        }

        [Fact]
        public void AddKeyBehavior_ByChar_RegistersTheCharacter()
        {
            var config = ReadLineConfig.Empty.AddKeyBehavior('q', t => { });

            var key = Assert.Single(config.KeyBehaviors.Keys);
            Assert.Equal('q', key.Char);
            Assert.Null(key.Key);
        }

        [Fact]
        public void AddTabAutoComplete_BindsTabAndShiftTabSeparately()
        {
            var config = ReadLineConfig.Empty.AddTabAutoComplete();

            Assert.Equal(2, config.KeyBehaviors.Count);
            Assert.True(config.KeyBehaviors.ContainsKey(new KeyId(ConsoleKey.Tab, false, false, false)));
            Assert.True(config.KeyBehaviors.ContainsKey(new KeyId(ConsoleKey.Tab, false, false, true)));
        }
    }
}
