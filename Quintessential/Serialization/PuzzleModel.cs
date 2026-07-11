using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Quintessential.Serialization;
[DataContract]
public class PuzzleModel {

    // display name, internal name, journal author
    [DataMember]
    public string Name, ID, Author;
    // the inputs
    [DataMember]
    public List<PuzzleIoM> Inputs = new();
    // the outputs
    [DataMember]
    public List<PuzzleIoM> Outputs = new();
    // output multiplier
    [DataMember(EmitDefaultValue = false)]
    public int OutputMultiplier = 1;
    // vanilla permission info
    [DataMember]
    public PuzzlePermissions PermissionFlags;
    // modded permisisons, can be used for parts, instructions, or anything else
    [DataMember]
    public HashSet<string> CustomPermissions = new();
    // set of highlighted hexes
    [DataMember(EmitDefaultValue = false)]
    public HashSet<HexIndexM> Highlights = new();
    // production-related stuff, or null for non-production puzzles
    [DataMember(EmitDefaultValue = false)]
    public ProductionInfoM ProductionInfo = null;
    [DataMember(EmitDefaultValue = false)]
    public List<ConduitM> Conduits = null;
    [DataMember(EmitDefaultValue = false)]
    public PayloadsM Payloads = null;

	public static PuzzleModel FromPuzzle(Puzzle puzzle) {
		PuzzleModel model = new(){
			ID = puzzle.puzzleId,
			PermissionFlags = puzzle.permissionFlags,
			Name = puzzle.puzzleName?.GetEnglish() ?? "Unnamed",
			Author = puzzle.journalAuthor.HasValue() ? puzzle.journalAuthor.GetValue() : "",
			CustomPermissions = ((patch_Puzzle)(object)puzzle).CustomPermissions,
			OutputMultiplier = puzzle.outputMultiplier
        };
		foreach(var @in in puzzle.inputs)
			model.Inputs.Add(new PuzzleIoM(@in));
		foreach(var @out in puzzle.outputs)
			model.Outputs.Add(new PuzzleIoM(@out));
		foreach(var item in puzzle.highlights)
			model.Highlights.Add(new HexIndexM(item));
		if(puzzle.productionInfo.HasValue())
			// if there's production cabinet info, use that
			model.ProductionInfo = new ProductionInfoM(puzzle.productionInfo.GetValue());
		else if (((patch_Puzzle)(object)puzzle).EngineConduits.HasValue())
		{
			// otherwise, populate the engine conduits
			model.Conduits = new();
			foreach (var conduit in ((patch_Puzzle)(object)puzzle).EngineConduits.GetValue())
			{
				model.Conduits.Add(new ConduitM(conduit));
			}
		}
		if (((patch_Puzzle)(object)puzzle).Payloads.HasValue())
		{
			model.Payloads = new PayloadsM(((patch_Puzzle)(object)puzzle).Payloads.GetValue());
		}

		return model;
	}

	public static Puzzle FromModel(PuzzleModel model) {
		Puzzle ret = new(){
            puzzleId = model.ID,
            puzzleName = Translations.Translate(model.Name),
            inputs = model.Inputs.Select(k => k.FromModel()).ToArray(),
            outputs = model.Outputs.Select(k => k.FromModel()).ToArray(),
            permissionFlags = model.PermissionFlags,
            journalAuthor = model.Author.Equals("") ? new Maybe<string>(false, null) : model.Author,
            highlights = model.Highlights.Select(k => k.FromModel()).ToArray(),
            outputMultiplier = model.OutputMultiplier
		};
		if(model.ProductionInfo != null) {
			if (model.ProductionInfo.Chambers.Count > 0)
			{
				ret.productionInfo = model.ProductionInfo.FromModel();
				// Calculate bounds
				ret.CalculateCabinetBounds();
			}
		}
		else if (model.Conduits != null)
		{
			// if it's not a cabinet, use these
			((patch_Puzzle)(object)ret).EngineConduits = model.Conduits.Select(c => c.FromModel()).ToArray();
		}
		((patch_Puzzle)(object)ret).CustomPermissions = model.CustomPermissions;

		if (model.Payloads != null) {
			((patch_Puzzle)(object)ret).Payloads = model.Payloads.FromModel();
		}

		return ret;
	}

    [DataContract]
    public class HexIndexM {
        [DataMember]
        public string Pos;

		public HexIndexM(HexIndex ind) {
			Pos = ind.Q + "," + ind.R;
		}

		public HexIndexM(){}

		public HexIndex FromModel() {
			return new(Q(), R());
		}

		public int Q() {
			return int.Parse(Pos.Split(',')[0]);
		}

		public int R() {
			return int.Parse(Pos.Split(',')[1]);
		}
	}

    [DataContract]
    public class PuzzleIoM {
        [DataMember]
        public MoleculeM Molecule;
        [DataMember(EmitDefaultValue = false)]
        public int AmountOverride = 0;

		public PuzzleIoM(PuzzleInputOutput io) {
			Molecule = new MoleculeM(io.molecule);
			AmountOverride = ((patch_PuzzleInputOutput)(object)io).AmountOverride;
		}

		public PuzzleIoM(){}

		public PuzzleInputOutput FromModel(){
			PuzzleInputOutput io = new PuzzleInputOutput(Molecule.FromModel());
			((patch_PuzzleInputOutput)(object)(io)).AmountOverride = AmountOverride;
			return io;
		}
	}

    [DataContract]
    public class MoleculeM {
        [DataMember]
        public List<AtomM> Atoms = new();
        [DataMember]
        public List<BondM> Bonds = new();
        [DataMember]
        public string Name = "";

		public MoleculeM(Molecule mol) {
			// Preserve the name
			Name = mol.displayName.GetOrDefault(null)?.GetEnglish() ?? "";
            // Clean molecule
            //mol = MoleculeEditorScreen.ImportRepeatingMolecules(mol); What does this line do??? ImportRepeatingMolecules was only present in previous possible bug!!!
            foreach (var atom in mol.GetAtoms())
				Atoms.Add(new AtomM(atom.Value, new HexIndexM(atom.Key)));
			foreach(var bond in mol.GetBonds())
				Bonds.Add(new BondM(bond));
		}

		public MoleculeM(){}

		public Molecule FromModel() {
			Molecule ret = new();
			foreach(var item in Atoms)
				ret.AddAtom(item.FromModel(), item.Position.FromModel());
			foreach(var item in Bonds)
				ret.AddBond((BondTypeEnum)item.BondBits(), item.A.FromModel(), item.B.FromModel());
			if(!Name.Equals(""))
				ret.displayName = Translations.Translate(Name);
			//return MoleculeEditorScreen.ExportRepeatingMolecules(ret, class_181.field_4474); Same as previous removed to run tests!!!!
			return ret;
        }
    }

    [DataContract]
    public class AtomM {
        [DataMember]
        public string AtomType;
        [DataMember]
        public HexIndexM Position;

		public AtomM(Atom atom, HexIndexM hex) {
			AtomType = ((patch_AtomType)(object)atom.atomType).QuintAtomType;
			Position = hex;
		}

		public AtomM(){}

		public Atom FromModel() {
			if(AtomType == null)
				throw new NullReferenceException("Missing atom type!");

			return new Atom(
				AtomTypes.atoms.FirstOrDefault(k => AtomType.Equals(((patch_AtomType)(object)k).QuintAtomType))
				?? throw new Exception($"Atom type \"{AtomType}\" does not exist!")
			);
		}
	}

    [DataContract]
    public class BondM {
        [DataMember]
        public HexIndexM A, B;
        [DataMember]
        public HashSet<string> BondTypes = new();

		public BondM(Bond bond) {
			A = new HexIndexM(bond.hexPos1);
			B = new HexIndexM(bond.hexPos2);
			if((bond.type & BondTypeEnum.Standard) == BondTypeEnum.Standard)
				BondTypes.Add("standard");
			if((bond.type & BondTypeEnum.Prisma0) == BondTypeEnum.Prisma0)
				BondTypes.Add("triplex_0");
			if((bond.type & BondTypeEnum.Prisma1) == BondTypeEnum.Prisma1)
				BondTypes.Add("triplex_1");
			if((bond.type & BondTypeEnum.Prisma2) == BondTypeEnum.Prisma2)
				BondTypes.Add("triplex_2");
		}

		public BondM(){}

		public byte BondBits() {
			byte bits = 0;
			if(BondTypes.Contains("standard"))
				bits |= (byte)BondTypeEnum.Standard;
			if(BondTypes.Contains("triplex_0"))
				bits |= (byte)BondTypeEnum.Prisma0;
			if(BondTypes.Contains("triplex_1"))
				bits |= (byte)BondTypeEnum.Prisma1;
			if(BondTypes.Contains("triplex_2"))
				bits |= (byte)BondTypeEnum.Prisma2;
			return bits;
		}
	}

    [DataContract]
    public class ProductionInfoM {
        [DataMember]
        public List<ChamberM> Chambers = new();
        [DataMember]
        public List<ConduitM> Conduits = new();
        [DataMember]
        public List<VialM> Vials = new();
        [DataMember]
        public bool Isolation = false, ShrinkLeft = false, ShrinkRight = false;

		public ProductionInfoM(ProductionInfo info) {
			foreach(var chamber in info.chambers)
				Chambers.Add(new ChamberM(chamber));
			foreach(PlacedConduit conduit in info.conduits)
				Conduits.Add(new ConduitM(conduit));
			foreach(PlacedVial vial in info.vials)
				Vials.Add(new VialM(vial));
			ShrinkLeft = info.tightLeftBound;
			ShrinkRight = info.tightRightBound;
			Isolation = info.requireIsolation;
		}

		public ProductionInfoM(){}

		public ProductionInfo FromModel() {
			ProductionInfo ret = new(){
                chambers = Chambers.Select(k => k.FromModel()).ToArray(),
                conduits = Conduits.Select(k => k.FromModel()).ToArray(),
                vials = Vials.Select(k => k.FromModel()).ToArray(),
                tightLeftBound = ShrinkLeft,
                tightRightBound = ShrinkRight,
                requireIsolation = Isolation
			};
			return ret;
		}
	}

    [DataContract]
    public class ChamberM {
        [DataMember]
        public string ChamberType;
        [DataMember]
        public HexIndexM Position;

		public ChamberM(PlacedChamber chamber) {
			ChamberType = chamber.chamber.name;
			Position = new HexIndexM(chamber.hexPos);
		}

		public ChamberM(){}

		public PlacedChamber FromModel() {
			return new(Position.Q(), Position.R(), Puzzles.prodChambers.First(k => k.name.Equals(ChamberType)));
		}
	}

    [DataContract]
    public class ConduitM {
        [DataMember]
        public HexIndexM PosA, PosB;
        [DataMember]
        public List<HexIndexM> Shape = new();

		public ConduitM(){}

		public ConduitM(PlacedConduit c) {
			foreach(HexIndex hex in c.conduitHexes)
				Shape.Add(new HexIndexM(hex));
			// TODO: when are there ever more than two?
			PosA = new HexIndexM(c.conduitTransforms[0].translation);
			PosB = new HexIndexM(c.conduitTransforms[1].translation);
		}

		public PlacedConduit FromModel() {
			return new PlacedConduit(PosA.Q(), PosA.R(), PosB.Q(), PosB.R(), Shape.Select(k => k.FromModel()).ToArray());
		}
	}

    [DataContract]
    public class VialM {
        [DataMember]
        public HexIndexM Position;
        [DataMember]
        public bool Top;
        [DataMember]
        public List<Pair<string, string>> Sprites = new();

		public VialM(){}

		public VialM(PlacedVial v) {
			Position = new HexIndexM(v.hexPos);
			Top = v.isTopConnected;
			foreach(Tuple<Texture, Texture> sprites in v.textures)
				Sprites.Add(new(CleanName(sprites.Item1), CleanName(sprites.Item2)));
		}

		public PlacedVial FromModel() {
			return new PlacedVial(Position.Q(), Position.R(), Top,
				Sprites.Select(xs => Tuple.Create(AssetLoaderHelper.LoadTexture(xs.Left), AssetLoaderHelper.LoadTexture(xs.Right))).ToArray());
		}

		private static string CleanName(Texture texture){
			string name = texture.sourceFile.GetValue();
			if(name.StartsWith("Content/") || name.StartsWith("Content\\"))
				name = name.Substring("Content/".Length);
			return name;
		}
	}

    [DataContract]
    public class PayloadsM
	{
        // change puzzle behaviour at runtime
        //public List<PayloadM> PuzzleInitialization = new();
        // changes new solutions
        [DataMember]
        public List<PayloadM> SolutionInitialization = new();

		public PayloadsM(){}

		public PayloadsM(Payloads p)
		{
			/*
			foreach (Payloads.Payload pl in p.PuzzleInitialization)
			{
				PuzzleInitialization.Add(new(pl));
			}
			*/
			foreach (Payloads.Payload pl in p.SolutionInitialization)
			{
				SolutionInitialization.Add(new(pl));
			}
		}

        public Payloads FromModel()
        {
            Payloads ret = new();
			/*
			foreach (PayloadM pl in PuzzleInitialization)
			{
				ret.PuzzleInitialization.Add(pl.FromModel());
			}
			*/
			foreach (PayloadM pl in SolutionInitialization)
			{
				ret.SolutionInitialization.Add(pl.FromModel());
			}

			return ret;
        }
    }
    [DataContract]
    public class PayloadM {
        [DataMember]
        public string Address;
        [DataMember]
        public string Data;

		public PayloadM(){}
        public PayloadM(Payloads.Payload pl)
        {
			Address = pl.Address;
			Data = pl.Data;
        }

        public Payloads.Payload FromModel()
        {
			if (!QApi.SolutionPayloadHandler.Exists(sph => sph.Left == Address))
			{
	           throw new Exception("No payload handler for address \"" + Address + "\"");
			}
			return new(Address, Data);
        }
    }
}
