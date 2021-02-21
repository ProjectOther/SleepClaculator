using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CalculatorServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server();
            server.Connect();
            //server.ChekClient("asdaasdsd", "asdasd ,asdasd");


        }
    }
}
