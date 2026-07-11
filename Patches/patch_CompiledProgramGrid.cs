using MonoMod;

using System.Collections.Generic;
using System.Linq;

#pragma warning disable CS0626 // Method, operator, or accessor is marked external and has no attributes on it

public class patch_CompiledProgramGrid
{
    [MonoModIgnore]
	Dictionary<Part, CompiledProgram> programDict;

	public extern int orig_GetLoopedCycle(int instructionIndex);

	public int GetLoopedCycle(int instructionIndex)
	{
		if (this.programDict.Count == 0) return 0;

		int num = this.programDict.Values.First().instructions.Length;
		if (num == 0) return 0;
		
		return instructionIndex % num;
	}
}