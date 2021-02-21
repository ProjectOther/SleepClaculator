using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CalculatorServer
{
    class Client
    {
        public string Id { get; set; }
        public string timeSleep { get; set; }
        public Server server;

        public string goodMorning { get; set; }
        public Socket socket;
        public Client(Socket socket, Server server)
        {
            this.server = server;
            this.Id = Guid.NewGuid().ToString();
            this.socket = socket;
        }
        protected internal void Process()
        {
            try
            {
                while (true)
                {   
                    server.ChekClient(Id, GetMsg());
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message); ;
            }
        }
        protected internal string GetMsg()
        {
            byte[] data = new byte[1024];
            string temp;
            StringBuilder stringBuilder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = socket.Receive(data);
                stringBuilder.Append(Encoding.Unicode.GetString(data, 0, bytes));

  
            } while (socket.Available>0);
            temp = stringBuilder.ToString();
            return temp;
        }
        
    }
}
