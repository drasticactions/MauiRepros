using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiHRBenchmarks;

public partial class CollectionViewTestPage : ContentPage
{
    public CollectionViewTestPage()
    {
        InitializeComponent();
        this.Persons = GeneratePersons();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        this.TestCollectionView.ItemsSource = this.Persons;
    }

    public List<Person> Persons { get; set; }

    private List<Person> GeneratePersons()
    {
        var persons = new List<Person>();
        for (int i = 0; i < 1000; i++)
        {
            persons.Add(new Person { Name = $"Person {i}", Age = $"{i}" });
        }
        return persons;
    }
    
    public class Person
    {
        public string Name { get; set; }
        public string Age { get; set; }
    }
}