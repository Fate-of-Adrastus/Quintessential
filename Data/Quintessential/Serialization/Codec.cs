using System;

namespace Quintessential.Serialization;

/* This code uses -dynamic-!
 * Why?
 * Although dynamic is horrible all my research led me to believe that
 * generic lambda function can't be created. E.g.:
 * //> private Func<CodecMap<TData>, TData, T> Decoder<TData>;
 * We'd need such a type, sice TData isn't known when the Codec is being constructed.
 * TData comes from Encode/Decode.
 * In an ideal world such a generic lambda could be made.
 * This means that the type of the -dynamic- parameters is actually known,
 * it just has to be hidden from the compiler.
 * Still: // TODO: Remove dynamic somehow!
 */

public partial class Codec<T> {
    protected internal Codec() { }
    private Func<dynamic, T, dynamic> Encoder; // aguments of //> Func<CodecMap<TData>, T, TData>
    private Func<dynamic, dynamic, T> Decoder; // aguments of //> Func<CodecMap<TData>, TData, T>

    public virtual TData Encode<TData>(CodecMap<TData> map, T item) {
        return Encoder(map, item);
    }
    public virtual T Decode<TData>(CodecMap<TData> map, TData encoding) {
        return Decoder(map, encoding);
    }

    public SealedCodec<T, TTransform> Seal<TTransform>(string properityName, Func<TTransform, T> getter) =>
        new(this, properityName, getter);
    public SealedCodec<T, TTransform> Seal<TTransform>(string properityName, Func<TTransform, T> getter, T defautValue) =>
        new(this, properityName, getter, defautValue);
}
