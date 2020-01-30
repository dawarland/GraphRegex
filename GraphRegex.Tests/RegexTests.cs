using System;
using NUnit.Framework;

namespace GraphRegex.Tests
{
    [TestFixture]
    public class RegexTests
    {
        [TestCase("a", true)]
        [TestCase("ab", true)]
        [TestCase("a*", true)]
        [TestCase("a|b", true)]
        [TestCase("((ab)*|c)def*|x|y*(g|hi)", true)]
        [TestCase("((ab)*|c)def*|x|y*(g|hi)*)", false)]
        [TestCase("*a", false)]
        [TestCase("*", false)]
        [TestCase("|", false)]
        public void RegexMatchWorks(string regex, bool expected)
        {
            StringCursor stringCursor = new StringCursor(regex);
            NodeAST node = Regex.MatchRegex(stringCursor);
            Assert.That(node.Type != NodeASTType.Error, Is.EqualTo(expected));
        }

        [TestCase("a", "'a'")]
        [TestCase("ab", "(+ 'a' 'b')")]
        [TestCase("a|b", "(| 'a' 'b')")]
        [TestCase("ab|bc", "(| (+ 'a' 'b') (+ 'b' 'c'))")]
        [TestCase("(ab|c)*(de*|f)(g|h*i)", "(+ (* (| (+ 'a' 'b') 'c')) (| (+ 'd' (* 'e')) 'f') (| 'g' (+ (* 'h') 'i')))")]
        [TestCase("((ab)*|c)def*|x|y*", "(| (+ (| (* (+ 'a' 'b')) 'c') 'd' 'e' (* 'f')) 'x' (* 'y'))")]
        public void buildingWorks(string regex, string expected)
        {
            StringCursor stringCursor = new StringCursor(regex);
            NodeAST node = Regex.MatchRegex(stringCursor);
            Assert.That(node.ToString(), Is.EqualTo(expected));
        }

        [Test]
        public void tree()
        {
            NodeAST tree = new NodeAST(NodeASTType.Alternative, new NodeAST[]
            {
                new NodeAST(NodeASTType.Concatenation, new NodeAST []
                    {
                        new NodeAST(NodeASTType.Constant, 'd'),
                        new NodeAST(NodeASTType.Constant, 'e'),
                        new NodeAST(NodeASTType.Star, new NodeAST[]
                            {
                                new NodeAST(NodeASTType.Constant, 'f')
                            }
                        )
                    }
                ),
                new NodeAST(NodeASTType.Constant, 'x'),
                new NodeAST(NodeASTType.Star, new NodeAST[]
                    {
                        new NodeAST(NodeASTType.Constant, 'y')
                    }
                )
            });

            Assert.That(tree.ToString(), Is.EqualTo("(| (+ 'd' 'e' (* 'f')) 'x' (* 'y'))"));
        }

    }
}
