using System.Xml.Serialization;

namespace RTXLauncherAUI.Models;

// This is our pure data model. It can be easily serialized to XML.
public class SettingsData
{
	[XmlAttribute] public bool UseCustomResolution { get; set; } = false;
	[XmlAttribute] public int Width { get; set; } = 1920;
	[XmlAttribute] public int Height { get; set; } = 1080;
	[XmlAttribute] public bool LoadWorkshopAddons { get; set; } = true;
	[XmlAttribute] public bool DisableChromium { get; set; } = false;
	[XmlAttribute] public bool ConsoleEnabled { get; set; } = true;
	[XmlAttribute] public bool DeveloperMode { get; set; } = false;
	[XmlAttribute] public bool ToolsMode { get; set; } = false;
	[XmlAttribute] public int DXLevel { get; set; } = 90;
	[XmlAttribute] public string CustomLaunchOptions { get; set; } = "";
	[XmlAttribute] public string ManuallySpecifiedInstallPath { get; set; } = "";

	// Launcher settings
	[XmlAttribute] public bool CheckForUpdatesOnLaunch { get; set; } = true;
	[XmlAttribute] public string Theme { get; set; } = "Simple";
}