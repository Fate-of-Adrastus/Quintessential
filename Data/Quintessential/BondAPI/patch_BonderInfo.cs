//using MonoMod;
//using Quintessential.BondAPI;
//using System;
//using System.Collections.Generic;
//using Bond = class_277;
//using BondTypeEnum = enum_126;
//using OpusMagnumException = class_266;

//[MonoModPatch("class_222")]
//class patch_BonderInfo {

//    public List<BondType> bondTypes;
//    public List<AtomType> uniqueAtoms;
//    public bool isUnbond = false;

//    [MonoModConstructor]
//    public patch_BonderInfo(List<BondType> _bondTypes, bool _isUnbond, HexIndex index1, HexIndex index2) {
//        this.field_1920 = index1;
//        this.field_1921 = index2;
//        this.field_1922 = (BondTypeEnum)BondTypes.GetBondIndex(_bondTypes[0].bondId);
//        this.field_1923 = struct_18.field_1431;
//        bondTypes = _bondTypes;
//        isUnbond = _isUnbond;
//        uniqueAtoms = new List<AtomType>();
//    }

//    [MonoModConstructor]
//    public patch_BonderInfo(List<BondType> _bondTypes, bool _isUnbond,List<AtomType> _uniqueAtoms, HexIndex index1, HexIndex index2) {
//        this.field_1920 = index1;
//        this.field_1921 = index2;
//        this.field_1922 = (BondTypeEnum)BondTypes.GetBondIndex(_bondTypes[0].bondId);
//        this.field_1923 = _uniqueAtoms[0];
//        bondTypes = _bondTypes;
//        isUnbond = _isUnbond;
//        uniqueAtoms = _uniqueAtoms;
//    }

//    [Obsolete]
//    [MonoModRemove]
//    [MonoModConstructor]
//    public patch_BonderInfo(HexIndex param_3903, HexIndex param_3904, BondTypeEnum _type, Maybe<AtomType> param_3905) { }

//    public readonly HexIndex field_1920;
//    public readonly HexIndex field_1921;
//    [Obsolete]
//    public readonly BondTypeEnum field_1922;
//    [Obsolete]
//    public Maybe<AtomType> field_1923;
//}
