using System;
using Stateless;

namespace BugPro
{
    public enum State { New, Analysis, Deferred, Fixing, Closed, Reopened }
    public enum Trigger { Assign, Defer, Resolve, Verify, Reject, Reopen }

    public class Bug
    {
        public State CurrentState => _machine.State;
        private readonly StateMachine<State, Trigger> _machine;

        public Bug()
        {
            _machine = new StateMachine<State, Trigger>(State.New);

            _machine.Configure(State.New).Permit(Trigger.Assign, State.Analysis);

            _machine.Configure(State.Analysis)
                .Permit(Trigger.Defer, State.Deferred)
                .Permit(Trigger.Resolve, State.Fixing)
                .Permit(Trigger.Reject, State.Closed);

            _machine.Configure(State.Deferred).Permit(Trigger.Assign, State.Analysis);

            _machine.Configure(State.Fixing)
                .Permit(Trigger.Verify, State.Closed)
                .Permit(Trigger.Reject, State.Analysis);

            _machine.Configure(State.Closed).Permit(Trigger.Reopen, State.Reopened);

            _machine.Configure(State.Reopened).Permit(Trigger.Assign, State.Analysis);
        }

        public void Fire(Trigger trigger) => _machine.Fire(trigger);
        public bool CanFire(Trigger trigger) => _machine.CanFire(trigger);
    }

    class Program
    {
        static void Main(string[] args)
        {
            var bug = new Bug();
            Console.WriteLine($"[LOG] Статус: {bug.CurrentState}");
            bug.Fire(Trigger.Assign);
            Console.WriteLine($"[LOG] Статус: {bug.CurrentState}");
        }
    }
}
