using System;
using System.Collections.Generic;
using System.Text;

namespace GraphRegex
{
    public class Automate
    {
        readonly NodeDFA _start;

        public Automate(NodeDFA start)
        {
            _start = start;
        }

        public bool Match(string candidate)
        {
            NodeDFA currentState = _start;
            foreach (char c in candidate)
            {
                currentState = currentState.getSuccessor(c);
            }

            return (currentState.IsFinal);
        }
    }

    public class NodeDFA
    {
        readonly bool _isFinal;
        readonly Dictionary<char,NodeDFA> _successors;

        public NodeDFA(bool isFinal)
        {
            _isFinal = isFinal;
        }

        public void Add(char label, NodeDFA successor)
        {
            _successors.Add(label, successor);
        }

        public NodeDFA getSuccessor(char c)
        {
            return _successors[c];
        }

        public bool IsFinal => _isFinal;

        //public override string ToString()
        //{
        //    StringBuilder myStringBuilder = new StringBuilder();
        //    ToString(myStringBuilder);
        //    return myStringBuilder.ToString();
        //}

        //void ToString(StringBuilder sb)
        //{
        //    if (_type == NodeType.Constant)
        //    {
        //        sb.AppendFormat("'{0}'", this._value);
        //        return;
        //    }

        //    sb.Append("(");
        //    if (_type == NodeType.Alternative)
        //        sb.Append("|");
        //    else if (_type == NodeType.Concatenation)
        //        sb.Append("+");
        //    else if (_type == NodeType.Star)
        //        sb.Append("*");

        //    foreach (Node child in _children)
        //    {
        //        sb.Append(" ");
        //        child.ToString(sb);
        //    }

        //    sb.Append(")");
        //}
    }

    public enum NodeDFAStatut
    {
        None = 0,
        Error,
        Debut,
        Transit,
        Final
    }

}
