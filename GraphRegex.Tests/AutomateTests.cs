using System;
using NUnit.Framework;

namespace GraphRegex.Tests
{
    [TestFixture]
    public class RegexTests
    {
        [TestCase("ab", true)]
        [TestCase("abab", true)]
        [TestCase("aaaa", false)]
        [TestCase("aba", false)]
        [TestCase("ba", false)]
        public void AutomateMatchWorks(string candidate, bool expected)
        {
            //Automate pour le regex (ab)*
            NodeDFA s0 = new NodeDFA(true);
            NodeDFA s1 = new NodeDFA(false);
            NodeDFA s2 = new NodeDFA(false);
            NodeDFA s3 = new NodeDFA(false);
            s0.Add('a', s1);
            s0.Add('b', s3);
            s1.Add('a', s3);
            s1.Add('b', s2);
            s2.Add('a', s1);
            s2.Add('b', s3);
            s3.Add('a', s3);
            s3.Add('b', s3);

            Automate dfa = new Automate(s0);

            Assert.That(dfa.Match(candidate), Is.EqualTo(expected));
        }
    }
}
