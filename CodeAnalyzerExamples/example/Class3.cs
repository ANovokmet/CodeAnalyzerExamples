using System;
using System.Collections.Generic;
using System.Text;

namespace SummaryAddApp
{
    /// <summary>
    /// My class 1
    /// </summary>
    public class Class1
    {
        /// <summary>
        /// My main method
        /// Test
        /// </summary>
        /// <param name="args"></param>
        public void Main(string[] args)
        {
            Console.WriteLine("Hello, world!");
        }
    }

    public class Class2
    {
        public void Do(string[] args, string despoa)
        {
            Console.WriteLine("Hello, world!");
        }

        public string This { get; set; }

        public string That { get; set; }
    }
}
