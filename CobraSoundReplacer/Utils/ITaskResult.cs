namespace CobraSoundReplacer.Utils;

public interface ITaskResult<in T>
{
    public void SetResult(T result);
}