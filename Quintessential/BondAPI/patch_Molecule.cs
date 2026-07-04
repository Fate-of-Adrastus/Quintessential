//using MonoMod;
//using Quintessential.BondAPI;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Bond = class_277;
//using BondTypeEnum = enum_126;
//using OpusMagnumException = class_266;

//[MonoModPatch("Molecule")]
//class patch_Molecule2 {

//    private List<Bond> field_2643;
//    private Dictionary<HexIndex, Atom> field_2642;

//    [MonoModReplace]
//    public void method_1110() {
//        foreach (KeyValuePair<HexIndex, Atom> keyValuePair in this.field_2642.Where(pair => pair.Value.field_2275.field_2298.method_1085()).ToArray()) {
//            HexIndex key = keyValuePair.Key;
//            Atom value = keyValuePair.Value;
//            this.field_2642.Remove(key);
//            Molecule molecule = value.field_2275.field_2298.method_1087();
//            foreach (KeyValuePair<HexIndex, Atom> keyValuePair2 in molecule.method_1100()) {
//                ((Molecule)(object)this).method_1105(keyValuePair2.Value.method_804(), keyValuePair2.Key + key);
//            }
//            foreach (Bond bond in molecule.method_1101()) {
//                this.SetBond(bond, bond.field_2187 + key, bond.field_2188 + key);
//            }
//        }
//    }
//    public void SetBond(Bond bond, HexIndex index1, HexIndex index2) {
//        if (HexIndex.Distance(index1, index2) != 1) {
//            throw new OpusMagnumException("Invalid distance between ends of bond");
//        }
//        Maybe<Bond> thisBond = field_2643.Where(bond => {
//            return (bond.field_2187 == index1 && bond.field_2188 == index2) ||
//            (bond.field_2187 == index2 && bond.field_2188 == index1);
//        }).method_430();
//        if (!thisBond.method_1085()) {
//            field_2643.Add(bond);
//            return;
//        }
//        Bond bond2 = thisBond.method_1087();
//        bond2 = bond.method_754();
//    }
//    public bool method_1111(BondType type, HexIndex index1, HexIndex index2) {
//        return this.method_1112(type, index1, index2, struct_18.field_1431);
//    }
//    public bool method_1112(BondType type, HexIndex index1, HexIndex index2, Maybe<BondEffect> effects) {
//        if (HexIndex.Distance(index1, index2) != 1) {
//            throw new OpusMagnumException("Invalid distance between ends of bond");
//        }
//        Maybe<Bond> thisBond = field_2643.Where(bond => {
//            return (bond.field_2187 == index1 && bond.field_2188 == index2) ||
//            (bond.field_2187 == index2 && bond.field_2188 == index1);
//        }).method_430();
//        if (!thisBond.method_1085()) {
//            Bond bond = (Bond)(object)new patch_Bond(type, index1, index2);
//            effects.method_1093(effect => { bond.field_2189.Add(effect); });
//            field_2643.Add(bond);
//            return true;
//        }

//        patch_Bond bond2 = (patch_Bond)(object)thisBond.method_1087();
//        if (!bond2.bondTypes.TrueForAll(bondType => { return bondType.CanOverlapBond(type); })) return false;
//        if (bond2.AddTypeSafe(type)) {
//            effects.method_1093(effect => { bond2.field_2189.Add(effect); });
//            return true;
//        }
//        return false;
//    }

//    [MonoModReplace]
//    public static Molecule method_1122(AtomType param_4848, AtomType param_4849) {
//        Molecule molecule = new Molecule();
//        molecule.method_1105(new Atom(param_4848), new HexIndex(0, 0));
//        molecule.method_1105(new Atom(param_4849), new HexIndex(1, 0));
//        ((patch_Molecule2)(object)molecule).method_1111(BondTypes.GetBondType("standard"), new HexIndex(0, 0), new HexIndex(1, 0));
//        return molecule;
//    }
//    public List<BondType> method_1113(HexIndex param_4838, HexIndex param_4839) {
//        foreach (class_277 class_ in ((Molecule)(object)this).method_1101()) {
//            if ((class_.field_2187 == param_4838 && class_.field_2188 == param_4839) || (class_.field_2187 == param_4839 && class_.field_2188 == param_4838)) {
//                return ((patch_Bond)(object)class_).bondTypes;
//            }
//        }
//        return new List<BondType>();
//    }

//    //[MonoModRemove]
//    //[Obsolete]
//    //public enum_126 method_1113(HexIndex param_4838, HexIndex param_4839) {
//    //    foreach (class_277 class_ in this.method_1101()) {
//    //        if ((class_.field_2187 == param_4838 && class_.field_2188 == param_4839) || (class_.field_2187 == param_4839 && class_.field_2188 == param_4838)) {
//    //            return class_.field_2186;
//    //        }
//    //    }
//    //    return enum_126.None;
//    //}
//    //[MonoModRemove]
//    //[Obsolete]
//    //public bool method_1111(enum_126 param_4831, HexIndex param_4832, HexIndex param_4833) {
//    //    return this.method_1112(param_4831, param_4832, param_4833, struct_18.field_1431);
//    //}
//    //[MonoModRemove]
//    //[Obsolete]
//    //public bool method_1112(BondTypeEnum type, HexIndex index1, HexIndex index2, Maybe<BondEffect> effects) {
//    //    if (HexIndex.Distance(index1, index2) != 1) {
//    //        throw new OpusMagnumException("Invalid distance between ends of bond");
//    //    }
//    //    Maybe<Bond> thisBond = field_2643.Where(bond => {
//    //        return (bond.field_2187 == index1 && bond.field_2188 == index2) ||
//    //        (bond.field_2187 == index2 && bond.field_2188 == index1);
//    //    }).method_430();
//    //    if (!thisBond.method_1085()) {
//    //        Bond bond = new Bond(type, index1, index2);
//    //        effects.method_1093(effect => { bond.field_2189.Add(effect); });
//    //        field_2643.Add(bond);
//    //        return true;
//    //    }
//    //    Bond bond2 = thisBond.method_1087();
//    //    BondTypeEnum type2 = bond2.field_2186;
//    //    if ((type2 & type) == type) {
//    //        return false;
//    //    }
//    //    if (type == BondTypeEnum.Standard && ((type2 & BondTypeEnum.Prisma0) == BondTypeEnum.Prisma0 || (type2 & BondTypeEnum.Prisma1) == BondTypeEnum.Prisma1 || (type2 & BondTypeEnum.Prisma2) == BondTypeEnum.Prisma2)) {
//    //        return false;
//    //    }
//    //    if ((type == BondTypeEnum.Prisma0 || type == BondTypeEnum.Prisma1 || type == BondTypeEnum.Prisma2) && (type2 & BondTypeEnum.Standard) == BondTypeEnum.Standard) {
//    //        return false;
//    //    }
//    //    bond2.field_2186 |= type;
//    //    effects.method_1093(effect => { bond2.field_2189.Add(effect); });
//    //    return true;
//    //}
//}
