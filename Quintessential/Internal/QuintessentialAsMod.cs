using System;

namespace Quintessential.Internal;

public class QuintessentialAsMod : QuintessentialMod {

	public override Type SettingsType => typeof(QuintessentialSettings);

	public override void Load() { }

    public override void LoadPuzzleContent()
    {
        QApi.AddSolutionPayloadHandler("Quintessential:place", (solution, data) =>
        {
            String[] parameters = data.Split(',');
            if (parameters.Length != 4)
            {
                return;
            }
            HexIndex position = new(int.Parse(parameters[0]), int.Parse(parameters[1]));
            HexRotation rotation = new(int.Parse(parameters[2]));

            if (!PartTypes.GetById(parameters[3]).GetOrDefault(out PartType partType))
            {
                partType = PartTypes.equilibriumGlyph;
            }

            Part part = new(partType, false);
            solution.RepositionPart(part, position);
            part.RotateBy(solution, rotation);
        });
    }

    public override void PostLoad() { }

	public override void Unload() { }
}
