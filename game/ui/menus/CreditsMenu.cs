using Godot;

namespace ProjectGJ.Ui.Menus;

public partial class CreditsMenu : PanelContainer
{
	public override void _Ready()
	{
		var text = GetNode<RichTextLabel>("%Credits");
		var back = GetNode<Button>("%Back");

		text.MetaClicked += OnMetaClicked;
		back.Pressed += () => Visible = false;
	}

	private void OnMetaClicked(Variant meta)
	{
		OS.ShellOpen(meta.AsString());
	}
}
