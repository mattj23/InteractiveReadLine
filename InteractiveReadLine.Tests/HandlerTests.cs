using System;
using InteractiveReadLine.Formatting;
using InteractiveReadLine.Tests.Fakes;
using InteractiveReadLine.Tokenizing;
using Moq;
using Xunit;

namespace InteractiveReadLine.Tests
{
    public class HandlerTests
    {
        [Fact]
        public void AutoComplete_Starts()
        {
            var config = ReadLineConfig.Basic
                .SetLexer(CommonLexers.SplitOnWhitespace)
                .SetAutoCompletion(l => new string[] {"option1", "option2"});
            
            var console = new TestConsole(300, 150);
            var provider = new ConsoleReadLine(console);
            var handler = new ReadLineHandler(provider, config);
            
            handler.AutoCompleteNext();
            
            Assert.Equal("option1", handler.TextBuffer.ToString());
        }
        
        [Fact]
        public void AutoComplete_Up2()
        {
            var config = ReadLineConfig.Basic
                .SetLexer(CommonLexers.SplitOnWhitespace)
                .SetAutoCompletion(l => new string[] {"option1", "option2"});
            
            var console = new TestConsole(300, 150);
            var provider = new ConsoleReadLine(console);
            var handler = new ReadLineHandler(provider, config);
            
            handler.AutoCompleteNext();
            handler.AutoCompleteNext();
            
            Assert.Equal("option2", handler.TextBuffer.ToString());
        }
        
        [Fact]
        public void AutoComplete_Up3()
        {
            var config = ReadLineConfig.Basic
                .SetLexer(CommonLexers.SplitOnWhitespace)
                .SetAutoCompletion(l => new string[] {"option1", "option2"});
            
            var console = new TestConsole(300, 150);
            var provider = new ConsoleReadLine(console);
            var handler = new ReadLineHandler(provider, config);
            
            handler.AutoCompleteNext();
            handler.AutoCompleteNext();
            handler.AutoCompleteNext();
            
            Assert.Equal("option1", handler.TextBuffer.ToString());
        }
        
        [Fact]
        public void AutoComplete_Down()
        {
            var config = ReadLineConfig.Basic
                .SetLexer(CommonLexers.SplitOnWhitespace)
                .SetAutoCompletion(l => new string[] {"option1", "option2"});
            
            var console = new TestConsole(300, 150);
            var provider = new ConsoleReadLine(console);
            var handler = new ReadLineHandler(provider, config);
            
            handler.AutoCompleteNext();
            handler.AutoCompletePrevious();
            
            Assert.Equal("option2", handler.TextBuffer.ToString());
        }
        
        [Fact]
        public void AutoComplete_Down2()
        {
            var config = ReadLineConfig.Basic
                .SetLexer(CommonLexers.SplitOnWhitespace)
                .SetAutoCompletion(l => new string[] {"option1", "option2"});
            
            var console = new TestConsole(300, 150);
            var provider = new ConsoleReadLine(console);
            var handler = new ReadLineHandler(provider, config);
            
            handler.AutoCompleteNext();
            handler.AutoCompletePrevious();
            handler.AutoCompletePrevious();
            
            Assert.Equal("option1", handler.TextBuffer.ToString());
        }
        
        [Fact]
        public void AutoComplete_LexerDoesntMatchText_Resets()
        {
            var config = ReadLineConfig.Basic
                .SetLexer(state =>
                {
                    var t = new TokenizedLine();
                    t.Add("test", false, 3);
                    return t;
                })
                .SetAutoCompletion(l => Array.Empty<string>());
            
            var console = new TestConsole(300, 150);
            var provider = new ConsoleReadLine(console);
            var handler = new ReadLineHandler(provider, config);
            
            handler.AutoCompleteNext();
            Assert.Equal("test", handler.TextBuffer.ToString());
        }

        [Fact]
        public void InsertText_CallsProvider()
        {
            string called = null;
            var mock = new Mock<IReadLineProvider>();
            mock.Setup(x => x.InsertText(It.IsAny<FormattedText>()))
                .Callback<FormattedText>(s => called = s.Text);
            var handler = new ReadLineHandler(mock.Object);
            
            handler.InsertText("test-text");
            
            mock.Verify(x => x.InsertText(It.IsAny<FormattedText>()), Times.Once);
            Assert.Equal("test-text", called);
        }
        
        [Fact]
        public void GetTokens_CallsLexer()
        {
            var config = ReadLineConfig.Basic
                .SetLexer(CommonLexers.SplitOnWhitespace);
            var mock = new Mock<IReadLineProvider>();
            var handler = new ReadLineHandler(mock.Object, config);
            handler.TextBuffer.Append("this test");

            var tokens = handler.GetTextTokens();
            
            Assert.Equal(3, tokens.Count);
            Assert.Equal("this", tokens.First.Text);
            Assert.Equal("test", tokens.Last.Text);
        }
    }
}