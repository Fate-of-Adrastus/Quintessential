using System;
using System.Collections.Generic;

namespace Quintessential.Serialization;

public class SealedCodec<TReturn, T> {
    readonly string PropertyName;
    readonly Func<T, TReturn> Getter;
    readonly Codec<TReturn> Codec;
    bool HasDefautValue = false;
    TReturn DefautValue;

    public virtual KeyValuePair<string, TData> EncodeAsProperty<TData>(CodecMap<TData> map, T item) {
        var ret = Getter(item);
        if (HasDefautValue && ret.Equals(DefautValue)) return KeyValuePair.Create("", default(TData)); // Discarded value
        return KeyValuePair.Create(PropertyName, Codec.Encode(map, ret));
    }
    public virtual TReturn DecodeAsProperty<TData>(CodecMap<TData> map, Dictionary<string, TData> obj) {
        TData data;
        if (HasDefautValue) {
            if (!obj.TryGetValue(PropertyName, out data))
                return DefautValue;
        } else
            data = obj[PropertyName];
        return Codec.Decode(map, data);
    }

    internal SealedCodec(Codec<TReturn> codec, string propertyName, Func<T, TReturn> getter) {
        Getter = getter;
        PropertyName = propertyName;
        Codec = codec;
    }
    internal SealedCodec(Codec<TReturn> codec, string propertyName, Func<T, TReturn> getter, TReturn defautValue) {
        Getter = getter;
        PropertyName = propertyName;
        Codec = codec;
        HasDefautValue = true;
        DefautValue = defautValue;
    }
    public SealedCodec<TReturn, T> WithDefaut(TReturn defautValue) {
        if (HasDefautValue) throw new InvalidOperationException("The Codec already has a default value!");
        HasDefautValue = true;
        DefautValue = defautValue;
        return this;
    }
}