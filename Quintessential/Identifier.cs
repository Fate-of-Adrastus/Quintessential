using OpusMagnumException = class_266;

namespace Quintessential;
struct Identifier {
    string namespc, name;

    public Identifier(string _namespc, string _name) {
        namespc = _namespc;
        name = _name;
    }
    public Identifier(string identifier) {
        var sections = identifier.Split(new char[] { ':' });
        if (sections.Length != 2) throw new OpusMagnumException("Identifier constructor contains invalid number of ':'");

        namespc = sections[0];
        name = sections[1];
    }

    public static implicit operator Identifier(string identifier) {
        return new Identifier(identifier);
    }
    public static implicit operator string(Identifier identifier) {
        return identifier.ToString();
    }

    public readonly override string ToString() {
        return namespc + ":" + name;
    }
    public static bool operator ==(Identifier i1, Identifier i2) {
        return i1.name == i2.name && i1.namespc == i2.namespc;
    }
    public static bool operator !=(Identifier i1, Identifier i2) {
        return i1.name != i2.name || i1.namespc != i2.namespc;
    }
    public readonly override bool Equals(object obj) {
        if (obj is Identifier i) {
            return name == i.name && namespc == i.namespc;
        }
        return false;
    }
    public readonly override int GetHashCode() {
        return ToString().GetHashCode();
    }
}