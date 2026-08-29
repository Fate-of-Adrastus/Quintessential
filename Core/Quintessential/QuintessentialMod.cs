using System;

namespace Quintessential;

public abstract class QuintessentialMod {
	public abstract string ModId { get; }
	public virtual Type SettingsType => typeof(object);
    public virtual Identifier GetIdentifier(string name) => new(ModId, name);
	public virtual LocString Translate(string key = "") => key == "" ? Translations.Translate(ModId) : Translations.Translate(ModId + "." + key);

    public ModMeta Meta;
	public object Settings;

	public abstract void Load();

	public abstract void LoadContent();
    public abstract void LoadCompatContent();
    public abstract void FinaliseContent();

    public abstract void PostLoad();

	public abstract void Unload();

	public virtual void ApplySettings() {

	}
}
