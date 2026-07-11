using System;
using System.Collections;
using System.Collections.Generic;

namespace Quintessential.BondAPI {
    public static class BondTypes {

        private static List<string> bondIds = new();
        private static Dictionary<string, BondType> bondTypes = new();

        public static void RegisterBondType(BondType bondType) {
            if (bondTypes.ContainsKey(bondType.bondId)) {
                throw new OpusMagnumException("BondId ( " + bondType.bondId + " ) was attempted to be registered multiple times!");
            }
            if (bondType.bondId == "#replaceID") throw new OpusMagnumException("Bond id #replaceID is reserved for internal use.");
            if (bondIds.Count < 9) throw new OpusMagnumException("Can't add bondTypes before init.");

            bondTypes.Add(bondType.bondId, bondType);

            if (bondIds.Count == 9) {
                for (int i = 3; i < 8; i++) {
                    if (bondIds[i] == "#replaceID") {
                        bondIds[i] = bondType.bondId;
                        return;
                    }
                }
            }
            bondIds.Add(bondType.bondId);
        }

        public static IReadOnlyDictionary<string, BondType> GetTypes() {
            return bondTypes;
        }
        public static int GetBondIndex(string id) {
            return bondIds.IndexOf(id);
        }
        public static string GetBondId(int index) {
            return bondIds[index];
        }
        public static BondType GetBondType(string id) {
            return bondTypes[id];
        }
        public static BondType GetBondType(int index) {
            return bondTypes[bondIds[index]];
        }
        public static BondType GetBondType(BondTypeEnum type) {
            return bondTypes[bondIds[(int)type]];
        }


        public static void InitBonds() {
            bondTypes.Add("none", null);
            bondTypes.Add("standard", new BondType(
                "standard",
                BondTextures.standard,
                Assets.textures.field_83.field_154,
                new List<string>()
            ));
            bondTypes.Add("prisma0", new BondType(
                "prisma0",
                BondTextures.prisma0,
                Assets.textures.field_83.field_156,
                new List<string>() { "prisma1", "prisma2" },
                - 10
            ));
            bondTypes.Add("prisma1", new BondType(
                "prisma1",
                BondTextures.prisma1,
                Assets.textures.field_83.field_156,
                new List<string>() { "prisma0", "prisma2" },
                10
            ));
            bondTypes.Add("prisma2", new BondType(
                "prisma2",
                BondTextures.prisma2,
                Assets.textures.field_83.field_156,
                new List<string>() { "prisma0", "prisma1" }
            ));
            bondIds.Add("none");
            bondIds.Add("standard");
            bondIds.Add("prisma0");
            bondIds.Add("#replaceID");
            bondIds.Add("prisma1");
            bondIds.Add("#replaceID");
            bondIds.Add("#replaceID");
            bondIds.Add("#replaceID");
            bondIds.Add("prisma2");
        }
    }
}
