using System;
using static Quintessential.CycleEvent;

namespace Quintessential;

public class CycleEvent(CycleEventExecutionType executionType, CycleEventDelegate @delegate) {

    public readonly CycleEventExecutionType ExecutionType = executionType;
    public readonly CycleEventDelegate Delegate = @delegate;

    [Flags]
    public enum CycleEventExecutionType {
        None = 0,
        First = 1,
        BeforeEarlyGlyphs = 2,
        AfterEarlyGlyphs = 4,
        AfterInstructions = 8,
        BeforeLateGlyphs = 16,
        AfterLateGlyphs = 32,
        Last = 64,
    }
    public delegate void CycleEventDelegate(patch_Sim sim, CycleEventExecutionType executionType);
}