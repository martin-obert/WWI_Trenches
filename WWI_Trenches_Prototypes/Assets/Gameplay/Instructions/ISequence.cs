using System;
using Assets.Gameplay.Character;

namespace Assets.Gameplay.Instructions
{
    public interface ISequence
    {
        void Chain(ISequence sequence);

        IOrder[] Orders { get; }

        ISequence Next { get; }
    }


    public interface ISequenceExecutor
    {
        ISequence CurrentSequence { get; set; }
        void Execute<T>(IOrderArguments<T> arguments, ISequence sequence = null);
    }

    public interface IConditionalSequence : ISequence
    {
        IOrder[] OnSuccess { get; }
        IOrder[] OnFailed { get; }
    }

    public class ConditionalSequence : IConditionalSequence
    {
        private readonly Func<bool> _predicate;

        public IOrder[] OnSuccess { get; }

        public IOrder[] OnFailed { get; }

        public IOrder[] Orders => _predicate() ? OnSuccess : OnFailed;

        public ISequence Next { get; private set; }

        public ConditionalSequence(Func<bool> predicate, IOrder[] onSuccess, IOrder[] onFailed)
        {
            _predicate = predicate;
            OnSuccess = onSuccess;
            OnFailed = onFailed;
        }

        public void Chain(ISequence sequence)
        {
            Next = sequence;
        }


    }

    public class BasicSequence : ISequence
    {
        public void Chain(ISequence sequence)
        {
            Next = sequence;
        }

        public IOrder[] Orders { get; }

        public BasicSequence(params IOrder[] order)
        {
            Orders = order;
        }

        public ISequence Next { get; private set; }
    }

    public interface IForkSequence
    {
        ISequence OnDone { get; }

        ISequence OnFailed { get; }

        Func<bool> IsFulfilled { get; }
    }

    public class ForkSequence : IForkSequence
    {
        private readonly Func<ISequenceExecutor, ISequence> _onSucccess;

        private readonly Func<ISequenceExecutor, ISequence> _onFailed;

        private readonly ISequenceExecutor _sequenceExecutor;

        public ForkSequence(Func<bool> predicate, ISequenceExecutor sequenceExecutor, Func<ISequenceExecutor, ISequence> onSuccess, Func<ISequenceExecutor, ISequence> onFailed)
        {
            _sequenceExecutor = sequenceExecutor;
            IsFulfilled = predicate;
            _onFailed = onFailed;
            _onSucccess = onSuccess;
        }


        public ISequence OnDone => _onSucccess(_sequenceExecutor);

        public ISequence OnFailed => _onFailed(_sequenceExecutor);

        public Func<bool> IsFulfilled { get; }
    }

    public static class BrainHelper
    {
        public static ISequenceExecutor Do(this ISequenceExecutor builder, params IOrder[] order)
        {
            if (order == null || order.Length == 0)
                return null;

            var result = new BasicSequence(order);
            builder.CurrentSequence = result;

            return builder;
        }

        public static ISequenceExecutor Then(this ISequenceExecutor sequence, params IOrder[] order)
        {
            if (order == null || order.Length == 0)
                return null;

            sequence.CurrentSequence = new BasicSequence(order);


            return sequence;
        }

        /// <summary>
        /// Evaluates condition immediately
        /// </summary>
        /// <param name="sequence"></param>
        /// <param name="predicate"></param>
        /// <param name="onSuccess"></param>
        /// <param name="onFail"></param>
        /// <returns></returns>
        public static ISequenceExecutor If(this ISequenceExecutor sequence, bool predicate, IOrder[] onSuccess, IOrder[] onFail)
        {
            if (onSuccess == null || onSuccess.Length == 0 && onFail == null || onFail.Length == 0) return sequence;

            if (predicate)
            {
                return Then(sequence, onSuccess);
            }

            return Then(sequence, onFail);
        }

        /// <summary>
        /// Evaluates condition at time of sequence execution
        /// </summary>
        /// <param name="sequence"></param>
        /// <param name="predicate"></param>
        /// <param name="onSuccess"></param>
        /// <param name="onFail"></param>
        /// <returns></returns>
        public static ISequenceExecutor Decide(this ISequenceExecutor sequence, Func<bool> predicate, IOrder[] onSuccess, IOrder[] onFail)
        {
            if (onSuccess == null || onSuccess.Length == 0 && onFail == null || onFail.Length == 0) return sequence;
            sequence.CurrentSequence = new ConditionalSequence(predicate, onSuccess, onFail);
            return sequence;
        }
    }
}
