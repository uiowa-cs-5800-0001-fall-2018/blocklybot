using System;

namespace BlockBot.Common.Functional
{
    /// <summary>
    ///     Functional data data to represent a discriminated
    ///     union of two possible types.
    /// </summary>
    /// <typeparam name="TL">Type of "Left" item.</typeparam>
    /// <typeparam name="TR">Type of "Right" item.</typeparam>
    public class Either<TL, TR>
    {
        private readonly bool _isLeft;
        private readonly TL _left;
        private readonly TR _right;

        public Either(TL left)
        {
            this._left = left;
            _isLeft = true;
        }

        public Either(TR right)
        {
            this._right = right;
            _isLeft = false;
        }

        public T Match<T>(Func<TL, T> leftFunc, Func<TR, T> rightFunc)
        {
            if (leftFunc == null)
            {
                throw new ArgumentNullException(nameof(leftFunc));
            }

            if (rightFunc == null)
            {
                throw new ArgumentNullException(nameof(rightFunc));
            }

            return _isLeft ? leftFunc(_left) : rightFunc(_right);
        }

        /// <summary>
        ///     If right value is assigned, execute an action on it.
        /// </summary>
        /// <param name="rightAction">Action to execute.</param>
        public void DoRight(Action<TR> rightAction)
        {
            if (rightAction == null)
            {
                throw new ArgumentNullException(nameof(rightAction));
            }

            if (!_isLeft)
            {
                rightAction(_right);
            }
        }

        public TL LeftOrDefault()
        {
            return Match(l => l, r => default(TL));
        }

        public TR RightOrDefault()
        {
            return Match(l => default(TR), r => r);
        }

        public static implicit operator Either<TL, TR>(TL left)
        {
            return new Either<TL, TR>(left);
        }

        public static implicit operator Either<TL, TR>(TR right)
        {
            return new Either<TL, TR>(right);
        }
    }
}