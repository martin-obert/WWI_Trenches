using System;
using Assets.Gameplay.Character.Interfaces;

namespace Assets.Gameplay.Instructions
{
    public interface ISequence
    {
        Type Evaluate();
    }

    public interface ISequenceBuilder<T>
    {
        void Execute(IOrderArguments<T> arguments);

        void AppendSequence(ISequence sequence);

        void Terminate();
    }

    public class BasicSequence : ISequence
    {
        private readonly Type _order;

        public BasicSequence(Type order)
        {
            _order = order;
        }

        public Type Evaluate()
        {
            return _order;
        }
    }

    public class ForkSequence : ISequence
    {
        private readonly Func<bool> _predicate;
        private readonly ISequence _onSuccess;
        private readonly ISequence _onFail;

        public ForkSequence(Func<bool> predicate, ISequence onSuccess, ISequence onFail)
        {
            _predicate = predicate;
            _onSuccess = onSuccess;
            _onFail = onFail;
        }

        public virtual Type Evaluate()
        {
            return _predicate() ? _onSuccess.Evaluate() : _onFail.Evaluate();
        }
    }

    public class ConditionalForkSequence : ForkSequence
    {
        private readonly Func<bool> _predicate;

        public override Type Evaluate()
        {
            return _predicate() ? base.Evaluate() : null;
        }

        public ConditionalForkSequence(Func<bool> predicate, Func<bool> forkPredicate, ISequence onSuccess, ISequence onFail) : base(forkPredicate, onSuccess, onFail)
        {
            _predicate = predicate;
        }
    }

    public class ConditionalSequence : ISequence
    {
        private readonly Func<bool> _predicate;

        private readonly Type _successOrder;

        public ConditionalSequence(Func<bool> predicate, Type successOrder)
        {
            _predicate = predicate;
            _successOrder = successOrder;
        }


        public Type Evaluate()
        {
            return _predicate() ? _successOrder : null;
        }
    }

    public static class BrainHelper
    {
        public static ISequenceBuilder<T> Do<T>(this ISequenceBuilder<T> builder, Type order)
        {
            builder.AppendSequence(new BasicSequence(order));

            return builder;
        }

        public static ISequenceBuilder<T> DoIf<T>(this ISequenceBuilder<T> builder, Func<bool> predicate,
            Type successOder = null)
        {
            builder.AppendSequence(new ConditionalSequence(predicate, successOder));

            return builder;
        }

        public static ISequenceBuilder<T> Fork<T>(this ISequenceBuilder<T> builder, Func<bool> predicate,
            Type onsuccess, Type onfail)
        {
            builder.AppendSequence(new ForkSequence(predicate, new BasicSequence(onsuccess), new BasicSequence(onfail)));

            return builder;
        }

        public static ISequenceBuilder<T> ForkIf<T>(this ISequenceBuilder<T> builder, Func<bool> ifPredicate, Func<bool> forkPredicate,
            Type onsuccess = null, Type onfail = null)
        {
            builder.AppendSequence(new ConditionalForkSequence(ifPredicate, forkPredicate, new BasicSequence(onsuccess), new BasicSequence(onfail)));

            return builder;
        }
    }



}
