//using MonoMod;
//using Quintessential.BondAPI;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using static Quintessential.Serialization.PuzzleModel;

//[MonoModPatch("Sim")]
//class patch_Sim2 {


//    [MonoModReplace]
//    private static bool method_1844(Molecule param_5379, Molecule param_5380) {
//        if (param_5379.method_1100().Count != param_5380.method_1100().Count || param_5379.method_1101().Count != param_5380.method_1101().Count) {
//            return false;
//        }
//        foreach (KeyValuePair<HexIndex, Atom> keyValuePair in param_5379.method_1100()) {
//            Atom atom;
//            if (!param_5380.method_1100().TryGetValue(keyValuePair.Key, out atom) || keyValuePair.Value.field_2275 != atom.field_2275) {
//                return false;
//            }
//        }
//        using (IEnumerator<class_277> enumerator2 = param_5379.method_1101().GetEnumerator()) {
//            while (enumerator2.MoveNext()) {
//                class_277 currentBond = enumerator2.Current;
//                if (!param_5380.method_1101().Any<class_277>(bond => {
//                    bool sameLocation = ((bond.field_2187 == currentBond.field_2187 && bond.field_2188 == currentBond.field_2188) || (bond.field_2187 == currentBond.field_2188 && bond.field_2188 == currentBond.field_2187));
//                    return sameLocation && (((patch_Bond)(object)currentBond).bondTypes.FindAll(bondType => !((patch_Bond)(object)bond).bondTypes.Contains(bondType)).Count == 0);
//                })) {
//                    return false;
//                }
//            }
//        }
//        return true;
//    }

//    [MonoModReplace]
//    public void RunCycleGlyphsMain(bool isCycleStart) {
//        List<Part> list = new List<Part>();
//        foreach (Part part in this.method_1817().field_3919) {
//            foreach (Part part2 in part.field_2696) {
//                if (this.field_3821[part2].field_2729.method_1085()) {
//                    list.Add(part2);
//                    this.method_1842(part2);
//                }
//            }
//        }
//        this.method_1843();
//        using (List<Part>.Enumerator enumerator = this.method_1817().field_3919.GetEnumerator()) {
//            while (enumerator.MoveNext()) {
//                Sim.class_402 class_ = new Sim.class_402();
//                class_.field_3841 = enumerator.Current;
//                PartSimState partSimState = this.field_3821[class_.field_3841];
//                if (class_.field_3841.method_1159() == class_191.field_1776) {
//                    AtomReference atomReference;
//                    if (this.method_1850(class_.field_3841, new HexIndex(0, 0), list, false).method_99<AtomReference>(out atomReference) && atomReference.field_2280.field_2293) {
//                        atomReference.field_2277.method_1106(class_175.field_1675, atomReference.field_2278);
//                        atomReference.field_2279.field_2276 = new class_168(this.field_3818, (enum_7)0, (enum_132)0, atomReference.field_2280, class_238.field_1989.field_81.field_611, 30f);
//                        Vector2 vector = class_187.field_1742.method_492(class_.field_3841.method_1161());
//                        this.field_3818.field_3935.Add(new class_228(this.field_3818, (enum_7)1, vector, class_238.field_1989.field_90.field_233, 30f, Vector2.Zero, 0f));
//                        this.method_1856(class_238.field_1991.field_1840);
//                    }
//                } else if (class_.field_3841.method_1159() == class_191.field_1777) {
//                    AtomReference atomReference2;
//                    AtomReference atomReference3;
//                    if (this.method_1850(class_.field_3841, new HexIndex(0, 0), list, true).method_99<AtomReference>(out atomReference2) && atomReference2.field_2280.field_2293 && this.method_1850(class_.field_3841, new HexIndex(1, 0), list, false).method_99<AtomReference>(out atomReference3) && atomReference3.field_2280 == class_175.field_1675) {
//                        atomReference3.field_2277.method_1106(atomReference2.field_2280, atomReference3.field_2278);
//                        atomReference3.field_2279.field_2276 = new class_168(this.field_3818, (enum_7)0, (enum_132)0, class_175.field_1675, class_238.field_1989.field_81.field_612, 30f);
//                        Vector2 vector2 = class_187.field_1742.method_492(class_.field_3841.method_1161() + new HexIndex(1, 0).Rotated(class_.field_3841.method_1163()));
//                        this.field_3818.field_3935.Add(new class_228(this.field_3818, (enum_7)1, vector2, class_238.field_1989.field_90.field_256, 30f, Vector2.Zero, class_.field_3841.method_1163().ToRadians()));
//                        this.method_1856(class_238.field_1991.field_1843);
//                    }
//                } else if (class_.field_3841.method_1159() == class_191.field_1778) {
//                    AtomReference atomReference4;
//                    AtomReference atomReference5;
//                    AtomType atomType;
//                    if (this.method_1850(class_.field_3841, new HexIndex(1, 0), list, false).method_99<AtomReference>(out atomReference4) && this.method_1850(class_.field_3841, new HexIndex(0, 0), list, false).method_99<AtomReference>(out atomReference5) && atomReference4.field_2280.field_2294 && atomReference4.field_2280.field_2297.method_99<AtomType>(out atomType) && atomReference5.field_2280.field_2295 && !atomReference5.field_2281 && !atomReference5.field_2282) {
//                        atomReference4.field_2277.method_1106(atomType, atomReference4.field_2278);
//                        atomReference5.field_2277.method_1107(atomReference5.field_2278);
//                        this.field_3818.field_3937.Add(new class_286(this.field_3818, atomReference5.field_2278, class_175.field_1680));
//                        atomReference4.field_2279.field_2276 = new class_168(this.field_3818, (enum_7)0, (enum_132)1, atomReference4.field_2280, class_238.field_1989.field_81.field_614, 30f);
//                        Vector2 vector3 = class_187.field_1742.method_492(class_.field_3841.method_1161() + new HexIndex(1, 0).Rotated(class_.field_3841.method_1163()));
//                        this.field_3818.field_3935.Add(new class_228(this.field_3818, (enum_7)1, vector3, class_238.field_1989.field_90.field_256, 30f, Vector2.Zero, class_.field_3841.method_1163().ToRadians()));
//                        this.method_1856(class_238.field_1991.field_1844);
//                    }
//                } else if (class_.field_3841.method_1159() == class_191.field_1779) {
//                    if (!partSimState.field_2743) {
//                        HexIndex hexIndex = new HexIndex(0, 1);
//                        AtomReference atomReference6;
//                        AtomReference atomReference7;
//                        if (isCycleStart && this.method_1850(class_.field_3841, new HexIndex(0, 0), list, false).method_99<AtomReference>(out atomReference6) && this.method_1850(class_.field_3841, new HexIndex(1, 0), list, false).method_99<AtomReference>(out atomReference7) && !this.method_1850(class_.field_3841, hexIndex, list, false).method_1085() && atomReference6.field_2280 == atomReference7.field_2280 && atomReference6.field_2280.field_2294 && atomReference6.field_2280.field_2297.method_1085() && !atomReference6.field_2281 && !atomReference6.field_2282 && !atomReference7.field_2281 && !atomReference7.field_2282) {
//                            AtomType field_2 = atomReference6.field_2280;
//                            atomReference6.field_2277.method_1107(atomReference6.field_2278);
//                            atomReference7.field_2277.method_1107(atomReference7.field_2278);
//                            partSimState.field_2743 = true;
//                            partSimState.field_2744 = new AtomType[] { field_2 };
//                            this.method_1856(class_238.field_1991.field_1845);
//                            this.field_3818.field_3937.Add(new class_286(this.field_3818, atomReference6.field_2278, field_2));
//                            this.field_3818.field_3937.Add(new class_286(this.field_3818, atomReference7.field_2278, field_2));
//                            Vector2 vector4 = class_187.field_1742.method_491(class_.field_3841.method_1184(hexIndex), Vector2.Zero);
//                            this.field_3826.Add(new Sim.struct_122 {
//                                field_3850 = (Sim.enum_190)0,
//                                field_3851 = vector4,
//                                field_3852 = field_3832
//                            });
//                        }
//                    } else {
//                        AtomType atomType2 = partSimState.field_2744[0].field_2297.method_1087();
//                        Molecule molecule = new Molecule();
//                        molecule.method_1105(new Atom(atomType2), class_.field_3841.method_1184(new HexIndex(0, 1)));
//                        this.field_3823.Add(molecule);
//                    }
//                } else if (class_.field_3841.method_1159() == class_191.field_1780) {
//                    if (!partSimState.field_2743) {
//                        HexIndex hexIndex2 = new HexIndex(0, 1);
//                        HexIndex hexIndex3 = new HexIndex(1, -1);
//                        AtomReference atomReference8;
//                        AtomReference atomReference9;
//                        if (isCycleStart && this.method_1850(class_.field_3841, new HexIndex(0, 0), list, false).method_99<AtomReference>(out atomReference8) && this.method_1850(class_.field_3841, new HexIndex(1, 0), list, false).method_99<AtomReference>(out atomReference9) && !this.method_1850(class_.field_3841, hexIndex2, list, false).method_1085() && !this.method_1850(class_.field_3841, hexIndex3, list, false).method_1085() && atomReference8.field_2280 == class_175.field_1675 && !atomReference8.field_2281 && !atomReference8.field_2282 && atomReference9.field_2280 == class_175.field_1675 && !atomReference9.field_2281 && !atomReference9.field_2282) {
//                            atomReference8.field_2277.method_1107(atomReference8.field_2278);
//                            atomReference9.field_2277.method_1107(atomReference9.field_2278);
//                            partSimState.field_2743 = true;
//                            this.method_1856(class_238.field_1991.field_1838);
//                            for (int j = 0; j < 2; j++) {
//                                HexIndex hexIndex4 = ((j == 0) ? hexIndex2 : hexIndex3);
//                                Vector2 vector5 = class_187.field_1742.method_491(class_.field_3841.method_1184(hexIndex4), Vector2.Zero);
//                                this.field_3826.Add(new Sim.struct_122 {
//                                    field_3850 = (Sim.enum_190)0,
//                                    field_3851 = vector5,
//                                    field_3852 = field_3832
//                                });
//                            }
//                        }
//                    } else {
//                        Molecule molecule2 = new Molecule();
//                        molecule2.method_1105(new Atom(class_175.field_1687), class_.field_3841.method_1184(new HexIndex(0, 1)));
//                        this.field_3823.Add(molecule2);
//                        Molecule molecule3 = new Molecule();
//                        molecule3.method_1105(new Atom(class_175.field_1688), class_.field_3841.method_1184(new HexIndex(1, -1)));
//                        this.field_3823.Add(molecule3);
//                    }
//                } else if (class_.field_3841.method_1159() == class_191.field_1783) {
//                    if (!partSimState.field_2743) {
//                        HexIndex hexIndex5 = new HexIndex(0, 0);
//                        AtomReference[] array = new AtomReference[4];
//                        if (isCycleStart && this.method_1850(class_.field_3841, new HexIndex(-1, 1), list, false).method_99<AtomReference>(out array[0]) && this.method_1850(class_.field_3841, new HexIndex(0, 1), list, false).method_99<AtomReference>(out array[1]) && this.method_1850(class_.field_3841, new HexIndex(0, -1), list, false).method_99<AtomReference>(out array[2]) && this.method_1850(class_.field_3841, new HexIndex(1, -1), list, false).method_99<AtomReference>(out array[3]) && !this.method_1850(class_.field_3841, hexIndex5, list, false).method_1085() && !array[0].field_2281 && !array[0].field_2282 && !array[1].field_2281 && !array[1].field_2282 && !array[2].field_2281 && !array[2].field_2282 && !array[3].field_2281 && !array[3].field_2282) {
//                            if (array.Count<AtomReference>(atomref => atomref.field_2280 == class_175.field_1678) == 1) {
//                                if (array.Count<AtomReference>(atomref => atomref.field_2280 == class_175.field_1679) == 1) {
//                                    if (array.Count<AtomReference>(atomref => atomref.field_2280 == class_175.field_1677) == 1) {
//                                        if (array.Count<AtomReference>(atomref => atomref.field_2280 == class_175.field_1676) == 1) {
//                                            for (int k = 0; k < 4; k++) {
//                                                array[k].field_2277.method_1107(array[k].field_2278);
//                                            }
//                                            partSimState.field_2743 = true;
//                                            partSimState.field_2744 = array.Select<AtomReference, AtomType>(atomref => atomref.field_2280).ToArray<AtomType>();
//                                            this.method_1856(class_238.field_1991.field_1850);
//                                            Vector2 vector6 = class_187.field_1742.method_491(class_.field_3841.method_1184(hexIndex5), Vector2.Zero);
//                                            this.field_3826.Add(new Sim.struct_122 {
//                                                field_3850 = (Sim.enum_190)0,
//                                                field_3851 = vector6,
//                                                field_3852 = field_3832
//                                            });
//                                        }
//                                    }
//                                }
//                            }
//                        }
//                    } else {
//                        Molecule molecule4 = new Molecule();
//                        molecule4.method_1105(new Atom(class_175.field_1690), class_.field_3841.method_1184(new HexIndex(0, 0)));
//                        this.field_3823.Add(molecule4);
//                    }
//                } else if (class_.field_3841.method_1159() == class_191.field_1784) {
//                    if (!partSimState.field_2743) {
//                        HexIndex[] array2 = new HexIndex[]
//                        {
//                            new HexIndex(-1, 0),
//                            new HexIndex(0, -1),
//                            new HexIndex(1, -1),
//                            new HexIndex(1, 0)
//                        };
//                        AtomReference atomReference10;
//                        if (isCycleStart && this.method_1850(class_.field_3841, new HexIndex(0, 0), list, false).method_99<AtomReference>(out atomReference10) && !this.method_1850(class_.field_3841, array2[0], list, false).method_1085() && !this.method_1850(class_.field_3841, array2[1], list, false).method_1085() && !this.method_1850(class_.field_3841, array2[2], list, false).method_1085() && !this.method_1850(class_.field_3841, array2[3], list, false).method_1085() && atomReference10.field_2280 == class_175.field_1690 && !atomReference10.field_2281 && !atomReference10.field_2282) {
//                            atomReference10.field_2277.method_1107(atomReference10.field_2278);
//                            partSimState.field_2743 = true;
//                            this.method_1856(class_238.field_1991.field_1841);
//                            for (int l = 0; l < 4; l++) {
//                                Vector2 vector7 = class_187.field_1742.method_491(class_.field_3841.method_1184(array2[l]), Vector2.Zero);
//                                this.field_3826.Add(new Sim.struct_122 {
//                                    field_3850 = (Sim.enum_190)0,
//                                    field_3851 = vector7,
//                                    field_3852 = field_3832
//                                });
//                            }
//                        }
//                    } else {
//                        Molecule molecule5 = new Molecule();
//                        molecule5.method_1105(new Atom(class_175.field_1676), class_.field_3841.method_1184(new HexIndex(-1, 0)));
//                        this.field_3823.Add(molecule5);
//                        molecule5 = new Molecule();
//                        molecule5.method_1105(new Atom(class_175.field_1678), class_.field_3841.method_1184(new HexIndex(0, -1)));
//                        this.field_3823.Add(molecule5);
//                        molecule5 = new Molecule();
//                        molecule5.method_1105(new Atom(class_175.field_1679), class_.field_3841.method_1184(new HexIndex(1, -1)));
//                        this.field_3823.Add(molecule5);
//                        molecule5 = new Molecule();
//                        molecule5.method_1105(new Atom(class_175.field_1677), class_.field_3841.method_1184(new HexIndex(1, 0)));
//                        this.field_3823.Add(molecule5);
//                    }
//                } else if (class_.field_3841.method_1159() == class_191.field_1781) {
//                    AtomReference atomReference11;
//                    if (this.method_1850(class_.field_3841, new HexIndex(0, 0), list, false).method_99<AtomReference>(out atomReference11) && !atomReference11.field_2281 && !atomReference11.field_2282) {
//                        atomReference11.field_2277.method_1107(atomReference11.field_2278);
//                        this.method_1856(class_238.field_1991.field_1842);
//                        this.field_3818.field_3935.Add(new class_228(this.field_3818, (enum_7)1, class_187.field_1742.method_492(class_.field_3841.method_1161()) + new Vector2(147f, 47f), class_238.field_1989.field_90.field_242, 30f, Vector2.Zero, 0f));
//                        this.field_3818.field_3936.Add(new class_228(this.field_3818, (enum_7)1, class_187.field_1742.method_492(class_.field_3841.method_1161()) + new Vector2(80f, 0f), class_238.field_1989.field_90.field_240, 30f, Vector2.Zero, 0f));
//                    }
//                } else if (class_.field_3841.method_1159().field_1543) {
//                    PartSimState partSimState2 = this.field_3821[class_.field_3841];
//                    if (isCycleStart) {
//                        HashSet<HexIndex> hashSet = class_.field_3841.method_1186(this.method_1817());
//                        foreach (Molecule molecule6 in this.field_3828.ToArray()) {
//                            if (this.method_1834(hashSet, molecule6) && !this.method_1833(molecule6, list)) {
//                                IEnumerable<Part> field_3 = this.method_1817().field_3919;
//                                Func<Part, bool> func;
//                                if ((func = class_.field_3842) == null) {
//                                    func = (class_.field_3842 = new Func<Part, bool>(part => {
//                                        return part != class_.field_3841 && part.method_1159().field_1543 && part.field_2703 == class_.field_3841.field_2703;
//                                    }));
//                                }
//                                Part part3 = field_3.Where<Part>(func).First<Part>();
//                                this.field_3823.Remove(molecule6);
//                                Molecule molecule7 = molecule6.method_1104();
//                                molecule6.method_1116(class_.field_3841.method_1161(), part3.method_1163() - class_.field_3841.method_1163());
//                                molecule6.method_1118(part3.method_1161() - class_.field_3841.method_1161());
//                                PartSimState partSimState3 = this.field_3821[part3];
//                                partSimState2.field_2732.Add(molecule7);
//                                partSimState3.field_2733.Add(molecule6);
//                                foreach (HexIndex hexIndex6 in molecule6.method_1100().Keys) {
//                                    Vector2 vector8 = class_187.field_1742.method_491(hexIndex6, Vector2.Zero);
//                                    this.field_3826.Add(new Sim.struct_122 {
//                                        field_3850 = (Sim.enum_190)0,
//                                        field_3851 = vector8,
//                                        field_3852 = field_3832
//                                    });
//                                }
//                                this.field_3828.Remove(molecule6);
//                            }
//                        }
//                    } else if (!isCycleStart) {
//                        foreach (Molecule molecule8 in partSimState2.field_2733) {
//                            this.field_3823.Add(molecule8);
//                        }
//                        partSimState2.field_2732.Clear();
//                        partSimState2.field_2733.Clear();
//                    }
//                }
//                if (class_.field_3841.method_1159().field_1538.Length != 0) {                               // This Whole section has been rewritten
//                    foreach (class_222 class_2 in class_.field_3841.method_1159().field_1538) {
//                        patch_BonderInfo bonderInfo = (patch_BonderInfo)(object)class_2;
//                        AtomReference atomReference12;
//                        AtomReference atomReference13;
//                        if (this.method_1850(class_.field_3841, bonderInfo.field_1920, list, false).method_99<AtomReference>(out atomReference12) && this.method_1850(class_.field_3841, class_2.field_1921, list, false).method_99<AtomReference>(out atomReference13) &&
//                            (bonderInfo.uniqueAtoms.Count == 0 || (bonderInfo.uniqueAtoms.Contains(atomReference12.field_2280) && bonderInfo.uniqueAtoms.Contains(atomReference13.field_2280)))) {
//                            HexIndex hexIndex7 = class_.field_3841.method_1184(bonderInfo.field_1920);
//                            HexIndex hexIndex8 = class_.field_3841.method_1184(bonderInfo.field_1921);
//                            if (bonderInfo.isUnbond) {
//                                if (atomReference12.field_2277 == atomReference13.field_2277) {
//                                    Molecule molecule = atomReference12.field_2277;
//                                    patch_Molecule2 moleculeP = (patch_Molecule2)(object)molecule;
//                                    List<BondType> bondTypes = moleculeP.method_1113(hexIndex7, hexIndex8);
//                                    bool wasRemoved = false;
//                                    List<BondType> removedTypes = new();
//                                    foreach (var bondType in bonderInfo.bondTypes) {
//                                        if (bondTypes.Contains(bondType)) {
//                                            wasRemoved = true;
//                                            bondTypes.Remove(bondType);
//                                            removedTypes.Add(bondType);
//                                        }
//                                    }
//                                    if (bondTypes.Count == 0) molecule.method_1114(hexIndex7, hexIndex8);
//                                    if (wasRemoved) {
//                                        Vector2 vector9 = class_162.method_413(class_187.field_1742.method_492(hexIndex7), class_187.field_1742.method_492(hexIndex8), 0.5f);
//                                        foreach (var type in removedTypes) {
//                                            class_256[] array4 = type.unbondAnim;
//                                            this.field_3818.field_3935.Add(new class_228(this.field_3818, (enum_7)1, vector9, array4, 75f, new Vector2(1.5f, -5f), class_187.field_1742.method_492(hexIndex8 - hexIndex7).Angle()));
//                                            this.method_1856(class_238.field_1991.field_1849);
//                                        }
//                                    }
//                                }
//                            } else {
//                                Molecule molecule9 = atomReference12.field_2277;
//                                if (atomReference12.field_2277 != atomReference13.field_2277) {
//                                    this.field_3823.Remove(atomReference12.field_2277);
//                                    this.field_3823.Remove(atomReference13.field_2277);
//                                    molecule9 = molecule9.method_1119(atomReference13.field_2277);
//                                    this.field_3823.Add(molecule9);
//                                }
//                                foreach (var bondType in bonderInfo.bondTypes) {
//                                    class_200 bondTexture = bondType.bondTexture;
//                                    BondEffect bondEffect = new BondEffect(this.field_3818, (enum_7)1, bondTexture.field_1817, 60f, bondTexture.field_1818);
//                                    patch_Molecule2 moleculeP = (patch_Molecule2)(object)molecule9;
//                                    if (moleculeP.method_1112(bondType, hexIndex7, hexIndex8, bondEffect)) {
//                                        Vector2 vector10 = class_162.method_413(class_187.field_1742.method_492(hexIndex7), class_187.field_1742.method_492(hexIndex8), 0.5f);
//                                        this.field_3818.field_3935.Add(new class_228(this.field_3818, (enum_7)1, vector10, bondTexture.field_1819, 30f, Vector2.Zero, class_187.field_1742.method_492(hexIndex8 - hexIndex7).Angle()));
//                                        this.method_1856(bondTexture.field_1820);
//                                    }
//                                }
//                            }
//                        }
//                    }
//                }
//            }
//        }
//        List<Molecule> list2 = new List<Molecule>();
//        foreach (Molecule molecule10 in this.field_3823) {
//            if (molecule10.field_2638) {
//                HashSet<HexIndex> hashSet2 = new HashSet<HexIndex>(molecule10.method_1100().Keys);
//                Queue<HexIndex> queue = new Queue<HexIndex>();
//                while (hashSet2.Count > 0) {
//                    if (queue.Count == 0) {
//                        HexIndex hexIndex9 = hashSet2.First<HexIndex>();
//                        hashSet2.Remove(hexIndex9);
//                        queue.Enqueue(hexIndex9);
//                        list2.Add(new Molecule());
//                        list2.Last<Molecule>().method_1105(molecule10.method_1100()[hexIndex9], hexIndex9);
//                    }
//                    HexIndex hexIndex10 = queue.Dequeue();
//                    foreach (class_277 class_4 in molecule10.method_1101()) {
//                        Maybe<HexIndex> maybe = struct_18.field_1431;
//                        if (class_4.field_2187 == hexIndex10) {
//                            maybe = class_4.field_2188;
//                        } else if (class_4.field_2188 == hexIndex10) {
//                            maybe = class_4.field_2187;
//                        }
//                        if (maybe.method_1085() && hashSet2.Contains(maybe.method_1087())) {
//                            hashSet2.Remove(maybe.method_1087());
//                            queue.Enqueue(maybe.method_1087());
//                            list2.Last<Molecule>().method_1105(molecule10.method_1100()[maybe.method_1087()], maybe.method_1087());
//                        }
//                    }
//                }
//                foreach (class_277 class_5 in molecule10.method_1101()) {
//                    foreach (Molecule molecule11 in list2) {
//                        if (molecule11.method_1100().ContainsKey(class_5.field_2187)) {
//                            ((patch_Molecule2)(object)molecule11).SetBond(class_5, class_5.field_2187, class_5.field_2188);                              // Here is a change !
//                            break;
//                        }
//                    }
//                }
//            }
//        }
//        this.field_3823.RemoveAll(new Predicate<Molecule>(molecule => molecule.field_2638));
//        this.field_3823.AddRange(list2);
//        foreach (Part part4 in this.method_1817().field_3919.Where(part => part.method_1159().field_1553)) {
//            PartSimState partSimState4 = this.field_3821[part4];
//            if (partSimState4.field_2743) {
//                partSimState4.field_2743 = false;
//                partSimState4.field_2731 = this.field_3818.method_510();
//            }
//        }
//        foreach (Part part5 in this.method_1817().field_3919.Where(part => part.method_1159().method_309())) {
//            PartSimState partSimState5 = this.field_3821[part5];
//            HexIndex hexIndex11 = part5.method_1161();
//            HexRotation hexRotation = part5.method_1163();
//            Molecule molecule12 = part5.method_1185(this.method_1817());
//            if (part5.method_1159().field_1553) {
//                Maybe<Molecule> maybe2 = this.method_1848(hexIndex11 + molecule12.method_1100().Keys.First<HexIndex>().Rotated(hexRotation));
//                if (maybe2.method_1085() && !this.method_1833(maybe2.method_1087(), list) && method_1845(hexIndex11, hexRotation, molecule12, maybe2.method_1087())) {
//                    this.field_3823.Remove(maybe2.method_1087());
//                    partSimState5.field_2730 = Math.Min(partSimState5.field_2730 + 1, part5.method_1169());
//                    partSimState5.field_2743 = true;
//                    this.method_1856(class_238.field_1991.field_1868);
//                }
//            } else {
//                foreach (Molecule molecule13 in this.field_3823) {
//                    partSimState5.field_2730 = class_162.method_404(method_1846(hexIndex11, molecule12, molecule13), partSimState5.field_2730, part5.method_1169());
//                }
//            }
//        }
//        foreach (Part part6 in list) {
//            this.method_1841(part6);
//        }
//    }

//    public SolutionEditorBase field_3818;
//    public Dictionary<Part, PartSimState> field_3821;
//    public List<Molecule> field_3823;
//    public List<Sim.struct_122> field_3826;
//    private List<Molecule> field_3828;
//    private static readonly float field_3832;

//    [MonoModReplace]
//    private Solution method_1817() { return this.field_3818.method_502(); }
//    private bool method_1833(Molecule param_5370, List<Part> param_5371) { return orig_method_1833(param_5370, param_5371); }
//    private bool method_1834(HashSet<HexIndex> param_5372, Molecule param_5373) { return orig_method_1834(param_5372, param_5373); }
//    private void method_1841(Part param_5377) { orig_method_1841(param_5377); }
//    private void method_1842(Part param_5378) { orig_method_1842(param_5378); }
//    private void method_1843() { orig_method_1843(); }
//    private static bool method_1845(HexIndex param_5381, HexRotation param_5382, Molecule param_5383, Molecule param_5384) { return orig_method_1845(param_5381, param_5382, param_5383, param_5384); }
//    private static int method_1846(HexIndex param_5385, Molecule param_5386, Molecule param_5387) { return orig_method_1846(param_5385, param_5386, param_5387); }
//    public Maybe<Molecule> method_1848(HexIndex param_5389) { return this.orig_method_1848(param_5389); }
//    private Maybe<AtomReference> method_1850(Part param_5391, HexIndex param_5392, List<Part> param_5393, bool param_5394) { return this.orig_method_1850(param_5391, param_5392, param_5393, param_5394); }
//    private void method_1856(Sound param_5408) { this.orig_method_1856(param_5408); }

//    private extern bool orig_method_1833(Molecule param_5370, List<Part> param_5371);
//    private extern bool orig_method_1834(HashSet<HexIndex> param_5372, Molecule param_5373);
//    private extern void orig_method_1841(Part param_5377);
//    private extern void orig_method_1842(Part param_5378);
//    private extern void orig_method_1843();
//    private static extern bool orig_method_1845(HexIndex param_5381, HexRotation param_5382, Molecule param_5383, Molecule param_5384);
//    private static extern int orig_method_1846(HexIndex param_5385, Molecule param_5386, Molecule param_5387);
//    public extern Maybe<Molecule> orig_method_1848(HexIndex param_5389);
//    private extern Maybe<AtomReference> orig_method_1850(Part param_5391, HexIndex param_5392, List<Part> param_5393, bool param_5394);
//    private extern void orig_method_1856(Sound param_5408);
//}