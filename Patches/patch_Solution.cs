using MonoMod;
using Quintessential;
using System;

public class patch_Solution
{
    [MonoModIgnore]
    [PatchSolutionInitializer]
    public static extern Solution FromPuzzle(Puzzle puzzle, string solutionName);

    public static bool GetConduits(Puzzle puzzle, out PlacedConduit[] conduits)
    {
        if (puzzle.productionInfo.GetOrDefault(out ProductionInfo cabinetInfo))  {
            conduits = cabinetInfo.conduits;
            return true;
        }
        return ((patch_Puzzle)(object)puzzle).EngineConduits.GetOrDefault(out conduits);
    } 

    public static void ApplyChanges(Puzzle puzzle, Solution solution)
    {
        if (((patch_Puzzle)(object)puzzle).Payloads.GetOrDefault(out Payloads payloads)) {
            foreach (Payloads.Payload p in payloads.SolutionInitialization)
            {
                foreach (var handler in QApi.SolutionPayloadHandler)
                {
                    if (p.Address.Equals(handler.Left))
                    {
                        handler.Right(solution, p.Data);
                    }
                }
            }
        }
    }

}
