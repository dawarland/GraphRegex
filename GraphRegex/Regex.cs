using System;
using System.Collections.Generic;
using System.Text;

namespace GraphRegex
{
    public class Regex
    {
        // Algo réalisé à l'aide de la grammaire formelle
        public static NodeAST MatchRegex(StringCursor stringCursor)
        {
            NodeAST node = MatchAlternative(stringCursor);
            if (node.Type == NodeASTType.Error)
                return node;
            return stringCursor.Match('\0') ? node : NodeAST.CreateError();
        }

        public static NodeAST MatchAlternative(StringCursor stringCursor)
        {
            NodeAST node = MatchConcatenation(stringCursor);
            if (node.Type == NodeASTType.Error)
                return node;
            List<NodeAST> children = new List<NodeAST>();
            children.Add(node);
            while (stringCursor.Match('|'))
            {
                stringCursor.Next();
                node = MatchConcatenation(stringCursor);
                if (node.Type == NodeASTType.Error)
                    return node;
                children.Add(node);
            }
            
            return (children.Count==1) ? children[0] : new NodeAST(NodeASTType.Alternative, children.ToArray()) ;
        }

        public static NodeAST MatchConcatenation(StringCursor stringCursor)
        {
            List<NodeAST> children = new List<NodeAST>();
            do
            {
                NodeAST node = MatchStar(stringCursor);
                if (node.Type == NodeASTType.Error)
                    return node;
                children.Add(node);
            } while (stringCursor.Match('(') || char.IsLetterOrDigit(stringCursor.Current));
            return (children.Count == 1) ? children[0] : new NodeAST(NodeASTType.Concatenation, children.ToArray());
        }

        //Expression starable qui contient, ou pas, une étoile
        public static NodeAST MatchStar(StringCursor stringCursor)
        {
            NodeAST node = MatchStarable(stringCursor);
            if (node.Type == NodeASTType.Error)
                return node;
            if (stringCursor.Match('*'))
            {
                stringCursor.Next();
                return new NodeAST(NodeASTType.Star, new NodeAST[] { node });
            }
            return node;
        }

        //Expression que l'on a le droit de mettre devant une etoile
        public static NodeAST MatchStarable(StringCursor stringCursor)
        {
            if (stringCursor.Match('(')) return MatchParenthesis(stringCursor);
            return MatchConstant(stringCursor);
        }

        public static NodeAST MatchParenthesis(StringCursor stringCursor)
        {
            if (!stringCursor.Match('('))
                return NodeAST.CreateError();
            stringCursor.Next();
            NodeAST node = MatchAlternative(stringCursor);
            if (node.Type == NodeASTType.Error)
                return node;
            if (!stringCursor.Match(')'))
                return NodeAST.CreateError();
            stringCursor.Next();
            return node;

        }

        public static NodeAST MatchConstant(StringCursor stringCursor)
        {
            if (!char.IsLetterOrDigit(stringCursor.Current)) return NodeAST.CreateError();
            NodeAST node = new NodeAST(NodeASTType.Constant, stringCursor.Current);
            stringCursor.Next();
            return node;
        }
    }

    public class StringCursor
    {
        readonly string _input;
        int _idx;

        public StringCursor(string input)
        {
            _input = input;
            _idx = 0;

        }

        public void Next()
        {
            _idx++;
        }

        public char Current
        {
            get
            {
                if (_idx < _input.Length)
                    return _input[_idx];
                return '\0';
            }
        }

        public bool Match(char c)
        {
            return c == Current;
        }
    }

    public class NodeAST
    {
        readonly NodeASTType _type;
        readonly char _value;
        readonly NodeAST[] _children;

        public static NodeAST CreateError()
        {
            return new NodeAST(NodeASTType.Error, null);
        }
        public NodeAST(NodeASTType type, char value)
           : this(type, value, null)
        {
        }
        public NodeAST(NodeASTType type, NodeAST[] children)
            : this(type, default, children)
        {
        }

        public NodeAST(NodeASTType type, char value, NodeAST[] children)
        {
            _type = type;
            _value = value;
            _children = children ?? new NodeAST[0];
        }

        public NodeASTType Type => _type;
        public char Value => _value;
        public IEnumerable<NodeAST> Children => _children;

        public override string ToString()
        {
            StringBuilder myStringBuilder = new StringBuilder();
            ToString(myStringBuilder);
            return myStringBuilder.ToString();
        }

        void ToString(StringBuilder sb)
        {
            if (_type == NodeASTType.Constant)
            {
                sb.AppendFormat("'{0}'", this._value);
                return;
            }

            sb.Append("(");
            if (_type == NodeASTType.Alternative)
                sb.Append("|");
            else if (_type == NodeASTType.Concatenation)
                sb.Append("+");
            else if (_type == NodeASTType.Star)
                sb.Append("*");

            foreach (NodeAST child in _children)
            {
                sb.Append(" ");
                child.ToString(sb);
            }

            sb.Append(")");
        }
    }

    public enum NodeASTType
    {
        None = 0,
        Error,
        Alternative,
        Concatenation,
        Star,
        Constant
    }

}
