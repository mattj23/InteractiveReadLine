using System.Linq;
using InteractiveReadLine.Tokenizing;
using Xunit;

namespace InteractiveReadLine.Tests
{
    public class TokenDistanceTests
    {
        [Fact]
        public void FirstNonEmpty_IdentifiesCorrectly()
        {
            var line = SimpleLine();

            Assert.Equal("this", line.FirstNonHidden.Text);
        }

        [Fact]
        public void ForwardDistance_WorksCorrectly()
        {
            var line = SimpleLine();
            var start = line.FirstNonHidden;
            var end = line.FirstOrDefault(x => x.Text == "testing");

            var distance = start.DistanceTo(end);
            Assert.Equal(4, distance);
        }

        [Fact]
        public void ForwardDistance_WorksCorrectly_WhileIgnoringHidden()
        {
            var line = SimpleLine();
            var start = line.FirstNonHidden;
            var end = line.FirstOrDefault(x => x.Text == "testing");

            var distance = start.DistanceTo(end, true);
            Assert.Equal(2, distance);
        }

        private TokenizedLine SimpleLine()
        {
            var line = new TokenizedLine();
            line.Add("   ", true);
            line.Add("this", false);
            line.Add(" ", true);
            line.Add("is", false);
            line.Add(" ", true);
            line.Add("testing", false);
            line.Add("   ", true);
            line.Add("driven", true);
            return line;
        }
    }
}