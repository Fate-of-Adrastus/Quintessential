//using MonoMod;
//using Quintessential.BondAPI;
//using System.Collections.Generic;

//[MonoModPatch("class_191")]
//class patch_PartTypes {
//    [MonoModReplace]
//    public static void method_496() {
//        // This has not been modified:
//        class_191.field_1760 = new class_139 {
//            field_1528 = "input",
//            field_1529 = class_134.method_253("Reagent", string.Empty),
//            field_1530 = class_134.method_253("An input for this alchemical machine.", string.Empty),
//            field_1547 = class_238.field_1989.field_90.field_212,
//            field_1539 = true,
//            field_1541 = true,
//            field_1551 = enum_149.None
//        };
//        class_191.field_1761 = new class_139 {
//            field_1528 = "out-std",
//            field_1529 = class_134.method_253("Product", string.Empty),
//            field_1530 = class_134.method_253("The desired output of this alchemical machine.", string.Empty),
//            field_1547 = class_238.field_1989.field_90.field_212,
//            field_1539 = true,
//            field_1553 = true,
//            field_1551 = enum_149.None
//        };
//        class_191.field_1762 = new class_139 {
//            field_1528 = "out-rep",
//            field_1529 = class_134.method_253("Product", string.Empty),
//            field_1530 = class_134.method_253("The desired output of this alchemical machine.", string.Empty),
//            field_1547 = class_238.field_1989.field_90.field_212,
//            field_1539 = true,
//            field_1554 = true,
//            field_1551 = enum_149.None
//        };
//        class_191.field_1763 = new class_139 {
//            field_1528 = "pipe",
//            field_1529 = class_134.method_253("Conduit", string.Empty),
//            field_1530 = class_134.method_253("Moves an atom, or group of atoms, to a different chamber of the transmutation engine.", string.Empty),
//            field_1547 = class_238.field_1989.field_71,
//            field_1539 = true,
//            field_1543 = true,
//            field_1551 = enum_149.None
//        };
//        class_191.field_1764 = new class_139 {
//            field_1528 = "arm1",
//            field_1529 = class_134.method_253("Fixed-Length Arm", string.Empty),
//            field_1530 = class_134.method_253("A single arm and gripper.", string.Empty),
//            field_1547 = class_238.field_1989.field_90.field_245.field_313,
//            field_1548 = class_238.field_1989.field_90.field_245.field_314,
//            field_1531 = 20,
//            field_1532 = (enum_2)1,
//            field_1533 = true,
//            field_1534 = new HexRotation[] { HexRotation.R0 },
//            field_1536 = true,
//            field_1551 = enum_149.SimpleArm
//        };
//        class_191.field_1765 = new class_139 {
//            field_1528 = "arm2",
//            field_1529 = class_134.method_253("Fixed-Length Arm", string.Empty),
//            field_1530 = class_134.method_253("An assembly of two arms and two grippers.", string.Empty),
//            field_1547 = class_238.field_1989.field_90.field_245.field_329,
//            field_1548 = class_238.field_1989.field_90.field_245.field_330,
//            field_1531 = 30,
//            field_1532 = (enum_2)1,
//            field_1533 = true,
//            field_1534 = new HexRotation[]
//            {
//                HexRotation.R0,
//                HexRotation.R180
//            },
//            field_1536 = true,
//            field_1551 = enum_149.MultiArms
//        };
//        class_191.field_1766 = new class_139 {
//            field_1528 = "arm3",
//            field_1529 = class_134.method_253("Fixed-Length Arm", string.Empty),
//            field_1530 = class_134.method_253("An assembly of three arms and three grippers.", string.Empty),
//            field_1547 = class_238.field_1989.field_90.field_245.field_323,
//            field_1548 = class_238.field_1989.field_90.field_245.field_324,
//            field_1531 = 30,
//            field_1532 = (enum_2)1,
//            field_1533 = true,
//            field_1534 = new HexRotation[]
//            {
//                HexRotation.R0,
//                HexRotation.R120,
//                HexRotation.R240
//            },
//            field_1536 = true,
//            field_1551 = enum_149.MultiArms
//        };
//        class_191.field_1767 = new class_139 {
//            field_1528 = "arm6",
//            field_1529 = class_134.method_253("Fixed-Length Arm", string.Empty),
//            field_1530 = class_134.method_253("An assembly of six arms and six grippers.", string.Empty),
//            field_1547 = class_238.field_1989.field_90.field_245.field_321,
//            field_1548 = class_238.field_1989.field_90.field_245.field_322,
//            field_1531 = 30,
//            field_1532 = (enum_2)1,
//            field_1533 = true,
//            field_1534 = new HexRotation[]
//            {
//                HexRotation.R0,
//                HexRotation.R60,
//                HexRotation.R120,
//                HexRotation.R180,
//                HexRotation.R240,
//                HexRotation.R300
//            },
//            field_1536 = true,
//            field_1551 = enum_149.MultiArms
//        };
//        class_191.field_1768 = new class_139 {
//            field_1528 = "piston",
//            field_1529 = class_134.method_253("Piston Arm", string.Empty),
//            field_1530 = class_134.method_253("A single arm and gripper that can extend and retract.", string.Empty),
//            field_1547 = class_238.field_1989.field_90.field_245.field_315,
//            field_1548 = class_238.field_1989.field_90.field_245.field_316,
//            field_1531 = 40,
//            field_1532 = (enum_2)1,
//            field_1533 = true,
//            field_1535 = true,
//            field_1534 = new HexRotation[] { HexRotation.R0 },
//            field_1536 = true,
//            field_1551 = enum_149.PistonArm
//        };
//        class_191.field_1769 = new class_139 {
//            field_1528 = "claw-pivot",
//            field_1529 = class_134.method_253("Gripper", string.Empty),
//            field_1532 = (enum_2)1,
//            field_1537 = true,
//            field_1545 = true,
//            field_1551 = enum_149.None
//        };
//        class_191.field_1770 = new class_139 {
//            field_1528 = "track",
//            field_1529 = class_134.method_253("Track", string.Empty),
//            field_1530 = class_134.method_253("Arms can be placed on tracks and instructed to move along them.", string.Empty),
//            field_1547 = class_238.field_1989.field_90.field_245.field_325,
//            field_1548 = class_238.field_1989.field_90.field_245.field_326,
//            field_1531 = 5,
//            field_1539 = true,
//            field_1532 = (enum_2)2,
//            field_1542 = true,
//            field_1551 = enum_149.Track
//        };
//        class_191.field_1771 = new class_139 {
//            field_1528 = "baron",
//            field_1529 = class_134.method_253("Van Berlo's Wheel", string.Empty),
//            field_1530 = class_134.method_253("By using Van Berlo's wheel with the glyph of duplication, neutral salt can be turned into any of the four cardinal elements.", string.Empty),
//            field_1547 = class_238.field_1989.field_90.field_245.field_335,
//            field_1548 = class_238.field_1989.field_90.field_245.field_336,
//            field_1531 = 30,
//            field_1552 = true,
//            field_1532 = (enum_2)1,
//            field_1533 = true,
//            field_1544 = new Dictionary<HexIndex, AtomType>
//            {
//                {
//                    new HexIndex(-1, 0),
//                    class_175.field_1675
//                },
//                {
//                    new HexIndex(1, 0),
//                    class_175.field_1675
//                },
//                {
//                    new HexIndex(-1, 1),
//                    class_175.field_1676
//                },
//                {
//                    new HexIndex(0, 1),
//                    class_175.field_1679
//                },
//                {
//                    new HexIndex(0, -1),
//                    class_175.field_1678
//                },
//                {
//                    new HexIndex(1, -1),
//                    class_175.field_1677
//                }
//            },
//            field_1536 = true,
//            field_1551 = enum_149.BaronWheel
//        };
//        // Bonders ! have been modified:
//        class_191.field_1772 = new class_139 {
//            field_1528 = "bonder",
//			field_1529 = class_134.method_253("Glyph of Bonding", string.Empty),
//			field_1530 = class_134.method_253("The glyph of bonding creates a bond between two atoms, after which they will move as a group.", string.Empty),
//			field_1547 = class_238.field_1989.field_90.field_245.field_297,
//			field_1548 = class_238.field_1989.field_90.field_245.field_298,
//			field_1531 = 10,
//			field_1539 = true,
//			field_1549 = class_238.field_1989.field_97.field_374,
//			field_1550 = class_238.field_1989.field_97.field_375,
//			field_1538 = new class_222[]
//            {
//                (class_222)(object)new patch_BonderInfo(new List<BondType>(){ "standard" },false, new HexIndex(0, 0), new HexIndex(1, 0))
//            },
//			field_1540 = new HexIndex[]
//            {
//                new HexIndex(0, 0),
//                new HexIndex(1, 0)
//            },
//			field_1551 = enum_149.Bonder

//        };
//        class_191.field_1773 = new class_139 {
//            field_1528 = "unbonder",
//            field_1529 = class_134.method_253("Glyph of Unbonding", string.Empty),
//            field_1530 = class_134.method_253("The glyph of unbonding eliminates the bond between two atoms.", string.Empty),
//            field_1547 = class_238.field_1989.field_90.field_245.field_331,
//            field_1548 = class_238.field_1989.field_90.field_245.field_332,
//            field_1531 = 10,
//            field_1539 = true,
//            field_1549 = class_238.field_1989.field_97.field_374,
//            field_1550 = class_238.field_1989.field_97.field_375,
//            field_1538 = new class_222[]
//            {
//                (class_222)(object)new patch_BonderInfo(new List<BondType>(){ "standard", "prisma0", "prisma1", "prisma2" }, true, new HexIndex(0, 0), new HexIndex(1, 0))
//            },
//            field_1540 = new HexIndex[]
//            {
//                new HexIndex(0, 0),
//                new HexIndex(1, 0)
//            },
//            field_1551 = enum_149.Unbonder
//        };
//        class_191.field_1774 = new class_139 {
//            field_1528 = "bonder-speed",
//            field_1529 = class_134.method_253("Glyph of Multi-bonding", string.Empty),
//            field_1530 = class_134.method_253("The glyph of multi-bonding can create up to three bonds at once.", string.Empty),
//            field_1547 = class_238.field_1989.field_90.field_245.field_311,
//            field_1548 = class_238.field_1989.field_90.field_245.field_312,
//            field_1531 = 30,
//            field_1539 = true,
//            field_1549 = class_238.field_1989.field_97.field_384,
//            field_1550 = class_238.field_1989.field_97.field_385,
//            field_1538 = new class_222[]
//            {
//                (class_222)(object)new patch_BonderInfo(new List<BondType>(){ "standard" },false, new HexIndex(0, 0), new HexIndex(-1, 1)),
//                (class_222)(object)new patch_BonderInfo(new List<BondType>(){ "standard" },false, new HexIndex(0, 0), new HexIndex(1, 0)),
//                (class_222)(object)new patch_BonderInfo(new List<BondType>(){ "standard" },false, new HexIndex(0, 0), new HexIndex(0, -1))
//            },
//            field_1540 = new HexIndex[]
//            {
//                new HexIndex(-1, 1),
//                new HexIndex(0, 0),
//                new HexIndex(1, 0),
//                new HexIndex(0, -1)
//            },
//            field_1551 = enum_149.SpeedBonder
//        };
//        class_191.field_1775 = new class_139 {
//            field_1528 = "bonder-prisma",
//            field_1529 = class_134.method_253("Glyph of Triplex-bonding", string.Empty),
//            field_1530 = class_134.method_253("The glyph of triplex-bonding creates three separate types of bonds between fire atoms that, when overlaid, become a triplex bond.", string.Empty),
//            field_1547 = class_238.field_1989.field_90.field_245.field_327,
//            field_1548 = class_238.field_1989.field_90.field_245.field_328,
//            field_1531 = 20,
//            field_1539 = true,
//            field_1549 = class_238.field_1989.field_97.field_386,
//            field_1550 = class_238.field_1989.field_97.field_387,
//            field_1538 = new class_222[]
//            {
//                (class_222)(object)new patch_BonderInfo(new List<BondType>(){ "prisma0" },false, new List<AtomType> { class_175.field_1678, class_175.field_1688 }, new HexIndex(0, 1), new HexIndex(1, 0)),
//                (class_222)(object)new patch_BonderInfo(new List<BondType>(){ "prisma1" },false, new List<AtomType> { class_175.field_1678, class_175.field_1688 }, new HexIndex(0, 0), new HexIndex(1, 0)),
//                (class_222)(object)new patch_BonderInfo(new List<BondType>(){ "prisma2" },false, new List<AtomType> { class_175.field_1678, class_175.field_1688 }, new HexIndex(0, 0), new HexIndex(0, 1))
//            },
//            field_1540 = new HexIndex[]
//            {
//                new HexIndex(0, 0),
//                new HexIndex(1, 0),
//                new HexIndex(0, 1)
//            },
//            field_1551 = enum_149.PrismaBonder
//        };
//        // This has not been modified:
//        class_191.field_1776 = new class_139 {
//            field_1528 = "glyph-calcification",
//            field_1529 = class_134.method_253("Glyph of Calcification", string.Empty),
//            field_1530 = class_134.method_253("The glyph of calcification transmutes any of the four cardinal elements-- air, fire, water, and earth-- into neutral salt.", string.Empty),
//            field_1547 = class_238.field_1989.field_90.field_245.field_299,
//            field_1548 = class_238.field_1989.field_90.field_245.field_300,
//            field_1531 = 10,
//            field_1539 = true,
//            field_1549 = class_238.field_1989.field_97.field_382,
//            field_1550 = class_238.field_1989.field_97.field_383,
//            field_1540 = new HexIndex[]
//            {
//                new HexIndex(0, 0)
//            },
//            field_1551 = enum_149.Calcification
//        };
//        class_191.field_1777 = new class_139 {
//            field_1528 = "glyph-duplication",
//            field_1529 = class_134.method_253("Glyph of Duplication", string.Empty),
//            field_1530 = class_134.method_253("The glyph of duplication transmutes a salt atom into one of the four cardinal elements by imbuing it with the essence of an existing atom.", string.Empty),
//            field_1547 = class_238.field_1989.field_90.field_245.field_305,
//            field_1548 = class_238.field_1989.field_90.field_245.field_306,
//            field_1531 = 20,
//            field_1539 = true,
//            field_1549 = class_238.field_1989.field_97.field_374,
//            field_1550 = class_238.field_1989.field_97.field_375,
//            field_1540 = new HexIndex[]
//            {
//                new HexIndex(0, 0),
//                new HexIndex(1, 0)
//            },
//            field_1551 = enum_149.Duplication
//        };
//        class_191.field_1778 = new class_139 {
//            field_1528 = "glyph-projection",
//            field_1529 = class_134.method_253("Glyph of Projection", string.Empty),
//            field_1530 = class_134.method_253("The glyph of projection consumes an atom of quicksilver and promotes an atom of metal to its next higher form.", string.Empty),
//            field_1547 = class_238.field_1989.field_90.field_245.field_317,
//            field_1548 = class_238.field_1989.field_90.field_245.field_318,
//            field_1531 = 20,
//            field_1539 = true,
//            field_1549 = class_238.field_1989.field_97.field_374,
//            field_1550 = class_238.field_1989.field_97.field_375,
//            field_1540 = new HexIndex[]
//            {
//                new HexIndex(0, 0),
//                new HexIndex(1, 0)
//            },
//            field_1551 = enum_149.Projection
//        };
//        class_191.field_1779 = new class_139 {
//            field_1528 = "glyph-purification",
//            field_1529 = class_134.method_253("Glyph of Purification", string.Empty),
//            field_1530 = class_134.method_253("The glyph of purification transmutes two atoms of the same metal into a single atom of their next higher form.", string.Empty),
//            field_1547 = class_238.field_1989.field_90.field_245.field_319,
//            field_1548 = class_238.field_1989.field_90.field_245.field_320,
//            field_1531 = 20,
//            field_1539 = true,
//            field_1549 = class_238.field_1989.field_97.field_386,
//            field_1550 = class_238.field_1989.field_97.field_387,
//            field_1540 = new HexIndex[]
//            {
//                new HexIndex(0, 0),
//                new HexIndex(1, 0),
//                new HexIndex(0, 1)
//            },
//            field_1551 = enum_149.Purification
//        };
//        class_191.field_1780 = new class_139 {
//            field_1528 = "glyph-life-and-death",
//            field_1529 = class_134.method_253("Glyph of Animismus", string.Empty),
//            field_1530 = class_134.method_253("The glyph of animismus transmutes two atoms of neutral salt into one atom of vitae and one atom of mors.", string.Empty),
//            field_1547 = class_238.field_1989.field_90.field_245.field_296,
//            field_1548 = class_238.field_1989.field_90.field_245.field_295,
//            field_1531 = 20,
//            field_1539 = true,
//            field_1549 = class_238.field_1989.field_97.field_368,
//            field_1550 = class_238.field_1989.field_97.field_369,
//            field_1540 = new HexIndex[]
//            {
//                new HexIndex(0, 1),
//                new HexIndex(0, 0),
//                new HexIndex(1, 0),
//                new HexIndex(1, -1)
//            },
//            field_1551 = enum_149.LifeAndDeath
//        };
//        class_191.field_1781 = new class_139 {
//            field_1528 = "glyph-disposal",
//            field_1529 = class_134.method_253("Glyph of Disposal", string.Empty),
//            field_1530 = class_134.method_253("The glyph of disposal removes unneeded atoms from the board.", string.Empty),
//            field_1547 = class_238.field_1989.field_90.field_245.field_303,
//            field_1548 = class_238.field_1989.field_90.field_245.field_304,
//            field_1539 = true,
//            field_1546 = false,
//            field_1549 = class_238.field_1989.field_97.field_372,
//            field_1550 = class_238.field_1989.field_97.field_373,
//            field_1552 = true,
//            field_1540 = new HexIndex[]
//            {
//                new HexIndex(0, 0),
//                new HexIndex(1, 0),
//                new HexIndex(0, 1),
//                new HexIndex(-1, 1),
//                new HexIndex(-1, 0),
//                new HexIndex(0, -1),
//                new HexIndex(1, -1)
//            },
//            field_1551 = enum_149.Disposal
//        };
//        class_191.field_1782 = new class_139 {
//            field_1528 = "glyph-marker",
//            field_1529 = class_134.method_253("The Glyph of Equilibrium", string.Empty),
//            field_1530 = class_134.method_253("Perfectly balanced in all aspects, the glyph of equilibrium does nothing but remain where it is placed. Some alchemists use it as a convenient marker, or for aesthetic considerations.", string.Empty),
//            field_1547 = class_238.field_1989.field_90.field_245.field_309,
//            field_1548 = class_238.field_1989.field_90.field_245.field_310,
//            field_1539 = true,
//            field_1546 = false,
//            field_1549 = class_238.field_1989.field_97.field_382,
//            field_1550 = class_238.field_1989.field_97.field_383,
//            field_1540 = new HexIndex[]
//            {
//                new HexIndex(0, 0)
//            },
//            field_1551 = enum_149.MultiArms
//        };
//        class_191.field_1783 = new class_139 {
//            field_1528 = "glyph-unification",
//            field_1529 = class_134.method_253("Glyph of Unification", string.Empty),
//            field_1530 = class_134.method_253("The glyph of unification transmutes one atom of each of the four cardinal elements into one atom of quintessence.", string.Empty),
//            field_1547 = class_238.field_1989.field_90.field_245.field_333,
//            field_1548 = class_238.field_1989.field_90.field_245.field_334,
//            field_1531 = 20,
//            field_1539 = true,
//            field_1549 = class_238.field_1989.field_97.field_388,
//            field_1550 = class_238.field_1989.field_97.field_389,
//            field_1540 = new HexIndex[]
//            {
//                new HexIndex(-1, 1),
//                new HexIndex(0, 1),
//                new HexIndex(0, 0),
//                new HexIndex(0, -1),
//                new HexIndex(1, -1)
//            },
//            field_1551 = enum_149.Quintessence
//        };
//        class_191.field_1784 = new class_139 {
//            field_1528 = "glyph-dispersion",
//            field_1529 = class_134.method_253("Glyph of Dispersion", string.Empty),
//            field_1530 = class_134.method_253("The glyph of dispersion transmutes one atom of quintessence into one atom of each of the four cardinal elements.", string.Empty),
//            field_1547 = class_238.field_1989.field_90.field_245.field_301,
//            field_1548 = class_238.field_1989.field_90.field_245.field_302,
//            field_1531 = 20,
//            field_1539 = true,
//            field_1549 = class_238.field_1989.field_97.field_370,
//            field_1550 = class_238.field_1989.field_97.field_371,
//            field_1540 = new HexIndex[]
//            {
//                new HexIndex(-1, 0),
//                new HexIndex(0, -1),
//                new HexIndex(0, 0),
//                new HexIndex(1, -1),
//                new HexIndex(1, 0)
//            },
//            field_1551 = enum_149.Quintessence
//        };
//        class_191.field_1785 = new class_139[]
//        {
//            class_191.field_1760,
//            class_191.field_1761,
//            class_191.field_1762,
//            class_191.field_1763,
//            class_191.field_1764,
//            class_191.field_1765,
//            class_191.field_1766,
//            class_191.field_1767,
//            class_191.field_1768,
//            class_191.field_1769,
//            class_191.field_1770,
//            class_191.field_1771,
//            class_191.field_1772,
//            class_191.field_1773,
//            class_191.field_1774,
//            class_191.field_1775,
//            class_191.field_1776,
//            class_191.field_1777,
//            class_191.field_1778,
//            class_191.field_1779,
//            class_191.field_1780,
//            class_191.field_1781,
//            class_191.field_1782,
//            class_191.field_1783,
//            class_191.field_1784
//        };
//        method_497();
//    }

//    private static void method_497() { orig_method_497(); }
//    private extern static void orig_method_497();
//}
