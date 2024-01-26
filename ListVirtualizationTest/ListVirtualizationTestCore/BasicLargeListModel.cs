namespace ListVirtualizationTestCore;

public class BasicLargeListModel
{
    public List<string> Items { get; }

    public BasicLargeListModel(int count = 10000) {
        Items = new List<string>();
        for (int i = 0; i < count; i++)
        {
            this.Items.Add(i.ToString());
        }
    }
}
