//using MonoMod;
//using Quintessential.BondAPI;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Linq;
//using System.Runtime.CompilerServices;
//using System.Runtime.InteropServices;
//using static Quintessential.Serialization.PuzzleModel;
//using Bond = class_277;
//using BondTypeEnum = enum_126;
//using OpusMagnumException = class_266;

//[MonoModPatch("class_277")]
//class patch_Bond {

//    public List<BondType> bondTypes;

//    [Obsolete]
//    public BondTypeEnum field_2186;
//    public HexIndex field_2187;
//    public HexIndex field_2188;
//    public List<BondEffect> field_2189;


//    [Obsolete]
//    [MonoModReplace]
//    [MonoModConstructor]
//    public patch_Bond(BondTypeEnum _type, HexIndex _index1, HexIndex _index2) {
//        field_2189 = new List<BondEffect>();
//        field_2186 = _type;
//        field_2187 = _index1;
//        field_2188 = _index2;
//        bondTypes = new List<BondType>() { };
//        if ((_type & BondTypeEnum.Standard) == BondTypeEnum.Standard) bondTypes.Add("standard");
//        if ((_type & BondTypeEnum.Prisma0) == BondTypeEnum.Prisma0) bondTypes.Add("prisma0");
//        if ((_type & BondTypeEnum.Prisma1) == BondTypeEnum.Prisma1) bondTypes.Add("prisma1");
//        if ((_type & BondTypeEnum.Prisma2) == BondTypeEnum.Prisma2) bondTypes.Add("prisma2");
//    }

//    [MonoModConstructor]
//    public patch_Bond(BondType bondType, HexIndex _index1, HexIndex _index2) {
//        field_2189 = new List<BondEffect>();
//        field_2186 = (BondTypeEnum)BondTypes.GetBondIndex(bondType.bondId);
//        field_2187 = _index1;
//        field_2188 = _index2;
//        bondTypes = new List<BondType>() { bondType };
//    }

//    // Returns true if it was added
//    public bool AddTypeSafe(BondType type) {
//        if (bondTypes.Contains(type)) return false;
//        int i = 0;
//        while (bondTypes.Count > i && bondTypes[i].renderPriority > type.renderPriority) i++;
//        bondTypes.Insert(i, type);
//        field_2186 = field_2186 | (BondTypeEnum)BondTypes.GetBondIndex(type.bondId);
//        return true;
//    }
//}