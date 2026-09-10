using static Quintessential.PartCycleDelegate;

namespace Quintessential;

public class PartCycleDelegate(CycleExecutionType executionType, RecipeCallDelegate @delegate) {

    public readonly CycleExecutionType ExecutionType = executionType;
    public readonly RecipeCallDelegate Delegate = @delegate;
    
    public enum CycleExecutionType {
        None = 0,
        Normal,
        AfterBonder,
    }
    public delegate void RecipeCallDelegate(patch_Sim sim, Part part, PartSimState simState, GlyphRecipe recipe, bool isCycleStart);
}