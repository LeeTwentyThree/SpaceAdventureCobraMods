namespace CobraSoundReplacer.Utils;

public class TaskResult<T> : ITaskResult<T>
{
    public bool Success { get; private set; }
    
    private T _result;

    public T GetResult()
    {
        if (!Success)
        {
            Plugin.Logger.LogWarning("Attempting to access the result from an uncompleted task!");
        }
        return _result;
    }

    public void SetResult(T result)
    {
        _result = result;
        Success = true;
    }
}