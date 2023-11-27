namespace AndroidLinkerTest;

public class MyGenericItem<T>
{
    public string Label { get;}
    public T Data { get; }
    public MyGenericItem(T data, string label)
    {
        Data = data;
        Label = label;
    }
}