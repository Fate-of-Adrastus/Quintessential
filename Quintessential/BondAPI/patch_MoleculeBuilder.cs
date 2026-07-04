//using MonoMod;
//using Quintessential.BondAPI;
//using System.Collections.Generic;
//using System.Linq;
//using BondTypeEnum = enum_126;
//using Bond = class_277;

//[MonoModPatch("class_160")]
//class class_160 {

//    private Molecule field_1623;
//    private Stack<HexIndex> field_1624;
//    public class_160 AddBond(BondType bondType, int param_3655, int param_3656) {
//        HexIndex hexIndex = new HexIndex(param_3655, param_3656);
//        this.field_1623.method_1111(bondType, this.field_1624.Peek(), this.field_1624.Peek() + hexIndex);
//        this.field_1624.Push(this.field_1624.Peek() + hexIndex);
//        return this;
//    }
//    public class_160 AddBond(List<BondType> bondTypes, int param_3655, int param_3656) {
//        HexIndex hexIndex = new HexIndex(param_3655, param_3656);
//        foreach (var bondType in bondTypes) {

//            this.field_1623.method_1111(bondType, this.field_1624.Peek(), this.field_1624.Peek() + hexIndex);
//        }
//        this.field_1624.Push(this.field_1624.Peek() + hexIndex);
//        return this;
//    }
//    //public class_160 method_399() {
//    //    foreach (KeyValuePair<HexIndex, Atom> keyValuePair in this.field_1623.method_1100().ToArray()) {
//    //        if (!(keyValuePair.Key == new HexIndex(0, 0))) {
//    //            this.field_1623.method_1105(keyValuePair.Value, keyValuePair.Key.Rotated(HexRotation.R60));
//    //            this.field_1623.method_1105(keyValuePair.Value, keyValuePair.Key.Rotated(HexRotation.R120));
//    //            this.field_1623.method_1105(keyValuePair.Value, keyValuePair.Key.Rotated(HexRotation.R180));
//    //            this.field_1623.method_1105(keyValuePair.Value, keyValuePair.Key.Rotated(HexRotation.R240));
//    //            this.field_1623.method_1105(keyValuePair.Value, keyValuePair.Key.Rotated(HexRotation.R300));
//    //        }
//    //    }
//    //    foreach (Bond class_ in this.field_1623.method_1101().ToArray()) {
//    //        this.field_1623.method_1111((patch_Bond)(object)class_).bondTypes, class_.field_2187.Rotated(HexRotation.R60), class_.field_2188.Rotated(HexRotation.R60));
//    //        this.field_1623.method_1111(((patch_Bond)(object)class_).bondTypes, class_.field_2187.Rotated(HexRotation.R120), class_.field_2188.Rotated(HexRotation.R120));
//    //        this.field_1623.method_1111(((patch_Bond)(object)class_).bondTypes, class_.field_2187.Rotated(HexRotation.R180), class_.field_2188.Rotated(HexRotation.R180));
//    //        this.field_1623.method_1111(((patch_Bond)(object)class_).bondTypes, class_.field_2187.Rotated(HexRotation.R240), class_.field_2188.Rotated(HexRotation.R240));
//    //        this.field_1623.method_1111(((patch_Bond)(object)class_).bondTypes, class_.field_2187.Rotated(HexRotation.R300), class_.field_2188.Rotated(HexRotation.R300));
//    //    }
//    //    return this;
//    //}

//    //[MonoModReplace]
//    //[Obsolete("This method shouldn't be used by mods, use AddBond.")]
//    //public class_160 method_395(int param_3655, int param_3656) {
//    //    HexIndex hexIndex = new HexIndex(param_3655, param_3656);
//    //    this.field_1623.method_1111(1, this.field_1624.Peek(), this.field_1624.Peek() + hexIndex);
//    //    this.field_1624.Push(this.field_1624.Peek() + hexIndex);
//    //    return this;
//    //}
//    //[MonoModReplace]
//    //[Obsolete("This method shouldn't be used by mods, use AddBond.")]
//    //public class_160 method_396(int param_3657, int param_3658) {
//    //    HexIndex hexIndex = new HexIndex(param_3657, param_3658);
//    //    this.field_1623.method_1111(2, this.field_1624.Peek(), this.field_1624.Peek() + hexIndex);
//    //    this.field_1623.method_1111(4, this.field_1624.Peek(), this.field_1624.Peek() + hexIndex);
//    //    this.field_1623.method_1111(8, this.field_1624.Peek(), this.field_1624.Peek() + hexIndex);
//    //    this.field_1624.Push(this.field_1624.Peek() + hexIndex);
//    //    return this;
//    //}
//}
