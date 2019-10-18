using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIPushCSharpSample
{
    class Person
    {

        public long Id { get; set; }

        public string Name { get; set; }



        public int Age { get; set; }



        public string Sex { get; set; }



        public override string ToString()

        {

            return $"Id={Id} Name={Name} Age={Age} Sex={Sex}";

        }

    }
}
