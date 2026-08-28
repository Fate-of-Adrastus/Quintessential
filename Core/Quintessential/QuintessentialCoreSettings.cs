using Quintessential.Internal;
using Quintessential.Settings;

namespace Quintessential;

public class QuintessentialCoreSettings {

	public static QuintessentialCoreSettings Instance => QuintessentialCore.Instance.Settings as QuintessentialCoreSettings;

	[SettingsLabel("Hot Reload Campaigns")]
	public Keybinding HotReloadCampaigns = new("F11");

	[SettingsLabel("Enable Campaign Switcher")]
	public bool EnableCustomCampaigns = true;

	[SettingsLabel("Campaign Switcher Options:")]
	public CampaignSwitcherSettings SwitcherSettings = new();

	public class CampaignSwitcherSettings : SettingsGroup {

		public override bool Enabled => Instance.EnableCustomCampaigns;

		[SettingsLabel("Switch Campaign Left")]
		public Keybinding SwitchCampaignLeft = new() { Key = "K", Control = true };
		
		[SettingsLabel("Switch Campaign Right")]
		public Keybinding SwitchCampaignRight = new() { Key = "L", Control = true };
	}
}
