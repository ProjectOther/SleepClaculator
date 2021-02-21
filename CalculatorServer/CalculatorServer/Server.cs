using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CalculatorServer
{
    class Server
    {
        SqlConnection connection;
        SqlCommand command;
        SqlDataReader reader;
        string connectionString = @"Data Source=10.0.1.200;Initial Catalog=Sleep_DB;User Id=student";
        List<Client> clients = new List<Client>();

        TcpListener tcpListener;
        Socket server;

        static int port = 8000;
        static IPEndPoint ipPoint;
        public Server()
        {
            connection = new SqlConnection(connectionString);
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(ipPoint);
            server.Listen(5);
        }
        protected internal void BroadcastMsg(string msg, string id)
        {
            byte[] data = Encoding.Unicode.GetBytes(msg);
            for (int i = 0; i < clients.Count; i++)
            {
                if (clients[i].Id == id)
                    clients[i].socket.Send(data);
            }
        }
        protected internal void AddConnection(Client client)
        {
            clients.Add(client);
        }
        protected internal void ProcessingRequest(string message, string id)
        {
            
        }
        protected internal void ChekClient(string id_client,string message)
        {
            if (String.IsNullOrEmpty(id_client)&&String.IsNullOrEmpty(message))
                return;

            string[] getMessage = message.Split(',');

            try
            {
                string time_sleep = String.Empty;
                string time_wake = String.Empty;
                connection.Open();
                using (command = new SqlCommand($"SELECT [id_client] FROM [Sleep] WHERE [id_client] = \'{id_client}\';", connection))
                {
                    reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        reader.Close();
                        command.CommandText = $"SELECT * FROM [Sleep] WHERE [id_client] = \'{id_client}\';";
                        reader = command.ExecuteReader();

                        
                        while (reader.Read())
                        {
                             time_sleep = reader.GetValue(2).ToString();
                             time_wake = reader.GetValue(3).ToString();
                        }
                    }
                    else
                    {
                        reader.Close();
                        command.CommandText = $"INSERT INTO Sleep (id_client,sleep_time,wake_up) VALUES('{id_client}','{getMessage[0]}','{getMessage[1]}')";
                        command.ExecuteNonQuery();
                        time_sleep = getMessage[0];
                        time_wake = getMessage[1];
                    }

                    BroadcastMsg(Calculate(time_sleep,time_wake), id_client);


                }
                connection.Close();
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
            
        }
        protected internal string Calculate(string time_sleep,string time_wake)
        {
            string temp="";
            int sleep = int.Parse(time_sleep);
            int wake = int.Parse(time_wake);
            DateTime time = DateTime.Now;

            int res = 24 - time.Hour+(sleep-wake);
            temp = res.ToString();
            //try
            //{

            //    connection.Open();
            //    using (command = new SqlCommand("SELECT * FROM Sleep", connection))
            //    {

            //        reader = command.ExecuteReader();
            //        if (reader.HasRows)
            //        {
            //            while (reader.Read())
            //            {
            //                Console.WriteLine(reader.GetValue(0));
            //                Console.WriteLine(reader.GetValue(1));
            //            }
            //        }
            //    }
            //    connection.Close();
            //    //BroadcastMsg();
            //}
            //catch (Exception ex)
            //{

            //    Console.WriteLine(ex.Message);
            //}
            return temp;
        }
        public void Connect()
        {
            try
            {
                Socket socket = server.Accept();
                Client client = new Client(socket, this);
                AddConnection(client);
                Thread thread = new Thread(new ThreadStart(client.Process));
                thread.Start();

                //StringBuilder stringBuilder = new StringBuilder();
                //int bytes = 0;
                //byte[] data = new byte[10240];
                //do
                //{
                //    bytes = socket.Receive(data);
                //    stringBuilder.Append(Encoding.Unicode.GetString(data, 0, bytes));

                //} while (socket.Available>0);

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
        }

    }
}
