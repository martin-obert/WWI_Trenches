using System;
using Assets.Gameplay.Character;

namespace Assets.Gameplay.Instructions
{
    public interface ISequence
    {
        void Chain(ISequence sequence);

        Character.ISequence[] Orders { get; }

        ISequence Next { get; }
    }


    public interface ISequenceExecutor
    {
        ISequence CurrentSequence { get; set; }
        void Execute<T>(IOrderArguments<T> arguments, ISequence sequence = null);
    }

    public interface IConditionalSequence : ISequence
    {
        Character.ISequence[] OnSuccess { get; }
        Character.ISequence[] OnFailed { get; }
    }

    public class ConditionalSequence : IConditionalSequence
    {
        private readonly Func<bool> _predicate;

        public Character.ISequence[] OnSuccess { get; }

        public Character.ISequence[] OnFailed { get; }

        public Character.ISequence[] Orders => _predicate() ? OnSuccess : OnFailed;

        public ISequence Next { get; private set; }

        public ConditionalSequence(Func<bool> predicate, Character.ISequence[] onSuccess, Character.ISequence[] onFailed)
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

        public Character.ISequence[] Orders { get; }

        public BasicSequence(params Character.ISequence[] order)
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
        public static ISequenceExecutor Do(this ISequenceExecutor builder, params Character.ISequence[] order)
        {
            if (order == null || order.Length == 0)
                return null;

            var result = new BasicSequence(order);
            builder.CurrentSequence = result;

            return builder;
        }

        public static ISequenceExecutor Then(this ISequenceExecutor sequence, params Character.ISequence[] order)
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
        public static ISequenceExecutor If(this ISequenceExecutor sequence, bool predicate, Character.ISequence[] onSuccess, Character.ISequence[] onFail)
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
        public static ISequenceExecutor Decide(this ISequenceExecutor sequence, Func<bool> predicate, Character.ISequence[] onSuccess, Character.ISequence[] onFail)
        {
            if (onSuccess == null || onSuccess.Length == 0 && onFail == null || onFail.Length == 0) return sequence;
            sequence.CurrentSequence = new ConditionalSequence(predicate, onSuccess, onFail);
            return sequence;
        }
    }
}
