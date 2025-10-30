using Godot;
using System;

public partial class EmptyGate : Gate, IUndoable
{
	[Export] private bool undoable = false;
	private bool unlocked = false;
	public void Unlock()
	{
		if (undoable)
		{
			Open();
			return;
		}

		if (!unlocked)
		{
			unlocked = true;
            Open();
        }
	}

	// Overrides so this gate in particular doesnt use checkpoint system (if not undoable)
	public override void AddLocalCheckpoint()
	{
		if (undoable)
			base.AddLocalCheckpoint();
	}
    public override void UndoLocalCheckpoint(bool nextCheckpointRequested)
	{
        if (undoable)
            base.UndoLocalCheckpoint(nextCheckpointRequested);
	}
    public override void ReplaceTopLocalCheckpoint()
	{
        if (undoable)
            base.ReplaceTopLocalCheckpoint();
	}
}
