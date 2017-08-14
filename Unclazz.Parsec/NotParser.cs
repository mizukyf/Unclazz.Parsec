﻿using System;

namespace Unclazz.Parsec
{
    sealed class NotParser<T> : Parser<T>
    {
        internal NotParser(Parser<T> original)
        {
            _original = original ?? throw new ArgumentNullException(nameof(original));
        }

        readonly Parser<T> _original;
        
        public override ParseResult<T> Parse(ParserInput input)
        {
            input.Mark();
            var p = input.Position;
            var originalResult = _original.Parse(input);
            if (originalResult.Successful)
            {
                return ParseResult.OfFailure<T>(p, string.Format("parsing with {0} " +
                        "must be failed but actualy be successful.", _original));
            }
            else
            {
                input.Reset();
                input.Unmark();
                return ParseResult.OfSuccess(p, default(T));
            }
        }

        public override string ToString()
        {
            return string.Format("Not({0})", _original);
        }
    }
}