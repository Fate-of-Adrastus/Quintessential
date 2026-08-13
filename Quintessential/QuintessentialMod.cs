using System;

namespace Quintessential;

public abstract class QuintessentialMod {
	public abstract string ModId { get; }
	public virtual Type SettingsType => typeof(object);

	public ModMeta Meta;
	public object Settings;

	public abstract void Load();

	public abstract void PostLoad();

	public abstract void Unload();

	public virtual void LoadPuzzleContent() {

	}

	public virtual void ApplySettings() {

	}
}
