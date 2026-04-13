using Microsoft.VisualStudio.TestTools.UnitTesting;
using BugPro;
using System;

namespace BugTests
{
    [TestClass]
    public class BugWorkflowTests
    {
        private Bug _bug = default!;

        [TestInitialize]
        public void Setup() => _bug = new Bug();

        [TestMethod] public void T1_InitialIsNew() => Assert.AreEqual(State.New, _bug.CurrentState);
        [TestMethod] public void T2_CanAssign() { _bug.Fire(Trigger.Assign); Assert.AreEqual(State.Analysis, _bug.CurrentState); }
        [TestMethod] public void T3_CanDefer() { _bug.Fire(Trigger.Assign); _bug.Fire(Trigger.Defer); Assert.AreEqual(State.Deferred, _bug.CurrentState); }
        [TestMethod] public void T4_CanResolve() { _bug.Fire(Trigger.Assign); _bug.Fire(Trigger.Resolve); Assert.AreEqual(State.Fixing, _bug.CurrentState); }
        [TestMethod] public void T5_CanReject() { _bug.Fire(Trigger.Assign); _bug.Fire(Trigger.Reject); Assert.AreEqual(State.Closed, _bug.CurrentState); }
        [TestMethod] public void T6_CanVerify() { _bug.Fire(Trigger.Assign); _bug.Fire(Trigger.Resolve); _bug.Fire(Trigger.Verify); Assert.AreEqual(State.Closed, _bug.CurrentState); }
        [TestMethod] public void T7_CanReturnFromFix() { _bug.Fire(Trigger.Assign); _bug.Fire(Trigger.Resolve); _bug.Fire(Trigger.Reject); Assert.AreEqual(State.Analysis, _bug.CurrentState); }
        [TestMethod] public void T8_CanReopen() { _bug.Fire(Trigger.Assign); _bug.Fire(Trigger.Reject); _bug.Fire(Trigger.Reopen); Assert.AreEqual(State.Reopened, _bug.CurrentState); }

        [DataTestMethod]
        [DataRow(Trigger.Defer)] [DataRow(Trigger.Resolve)] [DataRow(Trigger.Verify)] [DataRow(Trigger.Reopen)]
        [ExpectedException(typeof(InvalidOperationException))]
        public void T9_12_InvalidFromNew(Trigger t) => _bug.Fire(t);

        [DataTestMethod]
        [DataRow(Trigger.Assign)] [DataRow(Trigger.Verify)] [DataRow(Trigger.Reopen)]
        [ExpectedException(typeof(InvalidOperationException))]
        public void T13_15_InvalidFromAnalysis(Trigger t) { _bug.Fire(Trigger.Assign); _bug.Fire(t); }

        [DataTestMethod]
        [DataRow(Trigger.Resolve)] [DataRow(Trigger.Verify)] [DataRow(Trigger.Reject)] [DataRow(Trigger.Defer)] [DataRow(Trigger.Assign)]
        [ExpectedException(typeof(InvalidOperationException))]
        public void T16_20_InvalidFromClosed(Trigger t) { _bug.Fire(Trigger.Assign); _bug.Fire(Trigger.Reject); _bug.Fire(t); }
        
        [TestMethod] public void T21_CheckCanFire() => Assert.IsFalse(_bug.CanFire(Trigger.Verify));
    }
}
