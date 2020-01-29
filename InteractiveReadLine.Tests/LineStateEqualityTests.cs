using System.Data.SqlTypes;
using Xunit;

namespace InteractiveReadLine.Tests
{
    public class LineStateEqualityTests
    {
        [Fact]
        public void LineState_MatchingText_Equals()
        {
            var s1 = new LineState("this is some text", 5);
            var s2 = new LineState("this is some text", 5);
            
            Assert.Equal(s1, s2);
        }

        [Fact]
        public void LineState_MatchingText_WrongCursor_False()
        {
            var s1 = new LineState("this is some text", 5);
            var s2 = new LineState("this is some text", 6);
            
            Assert.NotEqual(s1, s2);
        }
        
        [Fact]
        public void LineState_WrongText_MatchingCursor_False()
        {
            var s1 = new LineState("this is some text", 5);
            var s2 = new LineState("this is som text", 5);
            
            Assert.NotEqual(s1, s2);
        }
        
        [Fact]
        public void LineState_MatchingText_HashCode()
        {
            var s1 = new LineState("this is some text", 5);
            var s2 = new LineState("this is some text", 5);
            
            Assert.Equal(s1.GetHashCode(), s2.GetHashCode());
        }

        [Fact]
        public void LineState_MatchingText_WrongCursor_HashCode()
        {
            var s1 = new LineState("this is some text", 5);
            var s2 = new LineState("this is some text", 6);
            
            Assert.NotEqual(s1.GetHashCode(), s2.GetHashCode());
        }
        
        [Fact]
        public void LineState_WrongText_MatchingCursor_HashCode()
        {
            var s1 = new LineState("this is some text", 5);
            var s2 = new LineState("this is som text", 5);
            
            Assert.NotEqual(s1.GetHashCode(), s2.GetHashCode());
        }
        [Fact]
        public void LineState_ReferenceEquals()
        {
            var s1 = new LineState("this is some text", 5);
            
            Assert.Equal(s1, s1);
        }

        [Fact]
        public void LineState_ReferenceEquals_FalseAgainstNull()
        {
            var s1 = new LineState("this is some text", 5);
            
            Assert.NotEqual(null, s1);
        }

        [Fact]
        public void LineState_NullEquals()
        {
            var s1 = new LineState("this is some text", 5);
            
            Assert.False(s1.Equals(null));
        }
        
        [Fact]
        public void LineState_Object_RefEquals()
        {
            var s1 = new LineState("this is some text", 5);
            
            Assert.True(s1.Equals(s1));
        }
        
        [Fact]
        public void LineState_Object_MismatchedTypes()
        {
            var s1 = new LineState("this is some text", 5);
            var i = 10;
            Assert.False(s1.Equals(i));
        }
        
        [Fact]
        public void LineState_Object_Compare()
        {
            var s1 = new LineState("this is some text", 5);
            var s2 = new LineState("this is some text", 5);
            var o = (object) s2;
            
            Assert.True(s1.Equals(o));
        }
    }
}