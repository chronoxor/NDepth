﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace NDepth.Examples.Common.LINQToObjectsExamples
{
    partial class Program
    {
        [Category("Grouping Operators")]
        [Description("This example uses group by to partition a list of numbers by their remainder when divided by 5.")]
        static void LinqGroupBySimple1()
        {
            Console.WriteLine("=== " + MethodInfo.GetCurrentMethod().Name + " ===");

            int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };

            var numberGroups =
                from n in numbers
                group n by n % 5 into g
                select new { Remainder = g.Key, Numbers = g };

            // Fluent expression equivalent.
            // var numberGroups = numbers.GroupBy(n => n % 5, (k, g) => new { Remainder = k, Numbers = g });

            foreach (var g in numberGroups)
            {
                Console.WriteLine("Numbers with a remainder of {0} when divided by 5:", g.Remainder);
                foreach (var n in g.Numbers)
                {
                    Console.WriteLine(n);
                }
            } 
        }
        
        [Category("Grouping Operators")]
        [Description("This example uses group by to partition a list of words by their first letter.")]
        static void LinqGroupBySimple2()
        {
            Console.WriteLine("=== " + MethodInfo.GetCurrentMethod().Name + " ===");

            string[] words = { "blueberry", "chimpanzee", "abacus", "banana", "apple", "cheese" };

            var wordGroups =
                from w in words
                group w by w[0] into g
                select new { FirstLetter = g.Key, Words = g };

            // Fluent expression equivalent.
            // var wordGroups = words.GroupBy(w => w[0], (k, g) => new { FirstLetter = k, Words = g });

            foreach (var g in wordGroups)
            {
                Console.WriteLine("Words that start with the letter '{0}':", g.FirstLetter);
                foreach (var w in g.Words)
                {
                    Console.WriteLine(w);
                }
            }
        }
        
        [Category("Grouping Operators")]
        [Description("This example uses group by to partition a list of products by category.")]
        static void LinqGroupBySimple3(CustomersStorage storage)
        {
            Console.WriteLine("=== " + MethodInfo.GetCurrentMethod().Name + " ===");

            var orderGroups =
                from p in storage.Products
                group p by p.Category into g
                select new { Category = g.Key, Products = g };

            // Fluent expression equivalent.
            // var orderGroups = storage.Products.GroupBy(p => p.Category, (k, g) => new { Category = k, Products = g });

            ObjectDumper.Write(orderGroups, 1);
        }
        
        [Category("Grouping Operators")]
        [Description("This example uses group by to partition a list of each customer's orders, " +
                     "first by year, and then by month.")]
        static void LinqGroupByNested(CustomersStorage storage)
        {
            Console.WriteLine("=== " + MethodInfo.GetCurrentMethod().Name + " ===");

            var customerOrderGroups =
                from c in storage.Customers
                select
                    new
                    {
                        c.CompanyName,
                        YearGroups =
                            from o in c.Orders
                            group o by o.OrderDate.Year into yg
                            select
                                new
                                {
                                    Year = yg.Key,
                                    MonthGroups =
                                        from o in yg
                                        group o by o.OrderDate.Month into mg
                                        select new { Month = mg.Key, Orders = mg }
                                }
                    };

            // Fluent expression equivalent.
            /*
            var customerOrderGroups = storage.Customers.Select(c => 
                new
                { 
                    c.CompanyName, 
                    YearGroups = c.Orders.GroupBy(
                        yo => yo.OrderDate.Year, 
                        (yk, yg) => 
                            new
                            {
                                Year = yk, 
                                MonthGroups = yg.GroupBy(
                                    mo => mo.OrderDate.Month, 
                                    (mk, mg) => new { Month = mk, Orders = mg })
                            }) 
                });
            */ 

            ObjectDumper.Write(customerOrderGroups, 3); 
        }

        private class AnagramEqualityComparer : IEqualityComparer<string>
        {
            public bool Equals(string x, string y)
            {
                return GetCanonicalString(x) == GetCanonicalString(y);
            }

            public int GetHashCode(string obj)
            {
                return GetCanonicalString(obj).GetHashCode();
            }

            private string GetCanonicalString(string word)
            {
                var wordChars = word.ToCharArray();
                Array.Sort(wordChars);
                return new string(wordChars);
            }
        }

        [Category("Grouping Operators")]
        [Description("This example uses GroupBy to partition trimmed elements of an array using " +
                     "a custom comparer that matches words that are anagrams of each other.")]
        static void LinqGroupByComparer()
        {
            Console.WriteLine("=== " + MethodInfo.GetCurrentMethod().Name + " ===");

            string[] anagrams = { "from   ", " salt", " earn ", "  last   ", " near ", " form  " };

            // Fluent expression only.
            var orderGroups = anagrams.GroupBy(w => w.Trim(), new AnagramEqualityComparer());

            ObjectDumper.Write(orderGroups, 1);
        }
        
        [Category("Grouping Operators")]
        [Description("This example uses GroupBy to partition trimmed elements of an array using " +
                     "a custom comparer that matches words that are anagrams of each other, " +
                     "and then converts the results to uppercase.")]
        static void LinqGroupByComparerMapped()
        {
            Console.WriteLine("=== " + MethodInfo.GetCurrentMethod().Name + " ===");

            string[] anagrams = { "from   ", " salt", " earn ", "  last   ", " near ", " form  " };
            
            // Fluent expression only.
            var orderGroups = anagrams.GroupBy(w => w.Trim(), a => a.ToUpper(), new AnagramEqualityComparer());

            ObjectDumper.Write(orderGroups, 1); 
        }
    }
}
