using Godot;
using System;

public partial class EmptyGate : Gate, IUndoable
{
	private bool unlocked = false;
	public void Unlock()
	{
		if (!unlocked)
		{
			unlocked = true;
            Open();
        }
	}

	// Overrides so this gate in particular doesnt use checkpoint system
	public override void AddLocalCheckpoint() { }
    public override void UndoLocalCheckpoint(bool nextCheckpointRequested) { }
    public override void ReplaceTopLocalCheckpoint() { }
}
