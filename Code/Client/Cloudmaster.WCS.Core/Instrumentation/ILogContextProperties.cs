namespace WCS.Core.Instrumentation
{
    public interface ILogContextProperties
    {
        object this[string key] { get; set; }
        void Remove(string key);
        void Clear();
    }

}
