public interface IUndoable
{
    public void AddLocalCheckpoint();
    public void UndoLocalCheckpoint(bool nextCheckpointRequested);
    public void ReplaceTopLocalCheckpoint();
}