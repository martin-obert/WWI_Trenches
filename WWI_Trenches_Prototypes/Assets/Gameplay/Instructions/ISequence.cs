using System;
using System.Collections.Generic;
using Assets.Gameplay.Character.Interfaces;
using UnityEngine.Assertions.Must;

namespace Assets.Gameplay.Instructions
{
    public interface ISequence
    {
        void Chain(ISequence sequence);

        Type[] Orders { get; }

        ISequence Next { get; }
    }


    public interface ISequencer
    {
        void Execute<T>(IOrderArguments<T> arguments, ISequence sequence = null);
        ISequence CurrentSequence { get; set; }
    }

    public interface IConditionalSequence : ISequence
    {
        Type[] OnSuccess { get; }
        Type[] OnFailed { get; }
    }

    public class ConditionalSequence : IConditionalSequence
    {
        private readonly Func<bool> _predicate;

        public Type[] OnSuccess { get; }

        public Type[] OnFailed { get; }
        public Type[] Orders => _predicate() ? OnSuccess : OnFailed;
        public ISequence Next { get; private set; }

        public ConditionalSequence(Func<bool> predicate, Type[] onSuccess, Type[] onFailed)
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

        public Type[] Orders { get; }

        public BasicSequence(params Type[] order)
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
        private readonly Func<ISequencer, ISequence> _onSucccess;
        private readonly Func<ISequencer, ISequence> _onFailed;
        private readonly ISequencer _sequencer;
        public ForkSequence(Func<bool> predicate, ISequencer sequencer, Func<ISequencer, ISequence> onSuccess, Func<ISequencer, ISequence> onFailed)
        {
            _sequencer = sequencer;
            IsFulfilled = predicate;
            _onFailed = onFailed;
            _onSucccess = onSuccess;
        }


        public ISequence OnDone => _onSucccess(_sequencer);

        public ISequence OnFailed => _onFailed(_sequencer);

        public Func<bool> IsFulfilled { get; }
    }

    public static class BrainHelper
    {
        public static ISequencer Do(this ISequencer builder, params Type[] order)
        {
            if (order == null || order.Length == 0)
                return null;

            var result = new BasicSequence(order);
            builder.CurrentSequence = result;

            return builder;
        }

        public static ISequencer Then(this ISequencer sequence, params Type[] order)
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
        public static ISequencer If(this ISequencer sequence, bool predicate, Type[] onSuccess, Type[] onFail)
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
        public static ISequencer Decide(this ISequencer sequence, Func<bool> predicate, Type[] onSuccess, Type[] onFail)
        {
            if (onSuccess == null || onSuccess.Length == 0 && onFail == null || onFail.Length == 0) return sequence;
            sequence.CurrentSequence = new ConditionalSequence(predicate, onSuccess, onFail);
            return sequence;
        }

        //public static ISequencer Fork(this ISequencer sequencer, Func<bool> predicate,
        //    Func<ISequencer, ISequence> onSuccess, Func<ISequencer, ISequence> onFail)
        //{
        //    sequencer.CurrentSequence = new ForkSequence(predicate, sequencer, onSuccess, onFail);

        //    return sequencer;
        //}

        public static void Execute<T>(this ISequencer sequence, IOrderArguments<T> arguments)
        {
            sequence.Execute(arguments);
        }
    }


    /// <summary>
    /// Example
    /// </summary>
    public class Test : ISequencer
    {
        private ISequence _currentSequence;
        private ISequence _startingSequence;



        public void Execute<T>(IOrderArguments<T> arguments, ISequence sequence = null)
        {
            var current = sequence ?? _startingSequence;

            if (sequence == _startingSequence || sequence == null)
                return;

            var orders = current.Orders;

            if (current.Next != null)
                Execute(arguments, current.Next);
        }

        public ISequence CurrentSequence
        {
            get { return _currentSequence; }
            set
            {
                if (_startingSequence == null)
                    _startingSequence = value;

                _currentSequence?.Chain(value);
                _currentSequence = value;
            }
        }
    }
}
