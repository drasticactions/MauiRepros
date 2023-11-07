// See https://aka.ms/new-console-template for more information
using System.Globalization;

Console.WriteLine(DateTime.Now.Date.ToString("MMM-dd-yyyy", CultureInfo.GetCultureInfo("en-CA")));
