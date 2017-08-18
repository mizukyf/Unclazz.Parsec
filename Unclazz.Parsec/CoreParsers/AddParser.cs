﻿using System;
using System.Collections.Generic;

namespace Unclazz.Parsec.CoreParsers
{
    sealed class AddParser<T> : Parser<IEnumerable<T>>
    {
        internal AddParser(Parser<T> left, Parser<T> right)
        {
            _left = left ?? throw new ArgumentNullException(nameof(left));
            _right = right ?? throw new ArgumentNullException(nameof(right));
        }

        readonly Parser<T> _left;
        readonly Parser<T> _right;

        public override ParseResult<IEnumerable<T>> Parse(ParserInput input)
        {
            var p = input.Position;
            var leftResult = _left.Parse(input);
            if (leftResult.Successful)
            {
                var rightResult = _right.Parse(input);
                var q = new Queue<T>();
                var canBacktrack = leftResult.CanBacktrack && rightResult.CanBacktrack;
                leftResult.IfSuccessful(c => c.IfHasValue(q.Enqueue));
                rightResult.IfSuccessful(c => c.IfHasValue(q.Enqueue));
                return rightResult.Successful
                    ? Success(p, new Capture<IEnumerable<T>>(q), canBacktrack)
                    : Failure(p, rightResult.Message, canBacktrack);
            }
            return Failure(p, leftResult.Message, leftResult.CanBacktrack);
        }
        public override string ToString()
        {
            return string.Format("Add({0}, {1})", _left, _right);
        }
    }
}
