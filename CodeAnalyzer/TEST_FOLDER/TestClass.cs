using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dummyclass
{
    class TestClass
    {
        delegate void Del(string str);
        struct a
        {
        }
        enum b
        {         
        }

        static void Main(string[] args)
        {
            int a = 1;
            int b = 2;
            //To test braceless scope
            if (a > b)
                Console.WriteLine("Implementing braceless scope");
        }
    }
}
