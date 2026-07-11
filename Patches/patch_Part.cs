using MonoMod;

class patch_Part{
	// this part type
	[MonoModIgnore]
	public extern PartType GetType();
	// this IO index
	[MonoModIgnore]
	public extern int GetInputOutputIndex();
	// setter for output amount
	[MonoModIgnore]
	private extern void SetOutputCount(int requiredCount);
	
	// handle output count overrides
	public extern void orig_SetupInputOutputFromSolution(Solution solution, int inputOutputIndex);

	public void SetupInputOutputFromSolution(Solution solution, int inputOutputIndex) {
        orig_SetupInputOutputFromSolution(solution, inputOutputIndex);
		
		bool isPolymer = this.GetType().isRepOutput;
		if(!isPolymer){
			PuzzleInputOutput[] list = (!GetType().isInput ? solution.GetPuzzle().outputs : solution.GetPuzzle().inputs);
			if(list == null || list.Length <= GetInputOutputIndex())
				return;

			PuzzleInputOutput io = list[GetInputOutputIndex()];
			if(io == null)
				return;

			int amount = ((patch_PuzzleInputOutput)(object)io).AmountOverride;
			if(amount > 0)
                SetOutputCount(amount);
		}
	}

    [MonoModReplace]
	public bool IsNotConduit()
    {
        PartType t = GetType();
        return !(t.isConduit || ((patch_PartType)(object)t).IsForced);
    }
}