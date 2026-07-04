using MonoMod;
using System;
using System.Collections.Generic;
using BondTexture = class_200;
using Texture = class_256;


namespace Quintessential.BondAPI {
    public class BondType {

        public readonly string bondId;

        /// <summary> Lower renderPriority bonds render later. </summary>
        public readonly int renderPriority;

        public readonly BondOverlapMode overlapMode;

        public BondTexture bondTexture;

        public Texture[] unbondAnim;

        public List<string> overlapIds;

        public BondType(string id, BondTexture textures, Texture[] _unbondAnim, List<string> _overlapIds, int _renderPriority = 0, BondOverlapMode _overlapMode = BondOverlapMode.OnlyListed) {
            bondId = id;
            bondTexture = textures;
            overlapIds = _overlapIds;
            renderPriority = _renderPriority;
            overlapMode = _overlapMode;
            unbondAnim = _unbondAnim;
        }

        /// <returns> True if the bond allows the other, or the other allows this. </returns>
        public bool CanOverlapBond(BondType bond) {
            return (overlapIds.Contains(bond.bondId) != (overlapMode == BondOverlapMode.AllExcept)) ||
                   (bond.overlapIds.Contains(bondId) != (bond.overlapMode == BondOverlapMode.AllExcept));
        }

        public static implicit operator BondType(string id) {
            return BondTypes.GetBondType(id);
        }
    }

    /// <summary> OnlyListed allows listed bonds, AllExcept allows all unlisted bonds to overlap </summary>
    public enum BondOverlapMode {
        OnlyListed,
        AllExcept,
    }


    //public struct BondTypeConverter {
    //    public readonly int origType;
    //    private BondTypeConverter(int _origType) {
    //        origType = _origType;
    //    }
    //    public static implicit operator BondTypeConverter(int a) {
    //        return new BondTypeConverter(a);
    //    }
    //    public static implicit operator enum_126(BondTypeConverter converter) {
    //        return (enum_126)converter.origType;
    //    }
    //}
    //public enum OrigBondType
}
