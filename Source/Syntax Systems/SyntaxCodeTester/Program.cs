using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyntaxCodeTester
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("SYNTAX CODE TESTER");

            // word capitalization test


            Console.WriteLine("WORD CAPITALIZATION TEST..");
            string word = "wordtocap";

            Console.WriteLine(PermissionSystem.Messages.PermissionSystemMessage.CapitalizeWord(word));
            Console.ReadLine();
        }
    }
}
