// See https://aka.ms/new-console-template for more information
using SqlSample;

Console.WriteLine("Hello, World!");

var database = new SampleDemoDatabase();
var items = database.GetItems();
foreach(var item in items)
{
    Console.WriteLine(item.BillDate);
}