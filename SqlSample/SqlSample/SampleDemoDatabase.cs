using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlSample
{
    public class SampleDemoDatabase
    {
        static object locker = new object();
        SQLiteConnection database;
        public SampleDemoDatabase(string appDirectory = "")
        {
            string dbPath = System.IO.Path.Combine(appDirectory, "mydatabase3.db");
            SQLiteConnection connection = new SQLiteConnection(dbPath);
            database = connection;
            // Creating the table
            database.CreateTable<OrderItem>();

            // Inserting items into table
            database.Query<OrderItem>("INSERT INTO OrderItem(BillDate) values('2023-06-12')");

        }

        public IEnumerable<OrderItem> GetItems()
        {
            lock (locker)
            {
                var table = from i in database.Table<OrderItem>() select i;
                return table;
            }
        }

    }

    public class OrderItem
    {
        public DateTime BillDate { get; set; }

    }
}
