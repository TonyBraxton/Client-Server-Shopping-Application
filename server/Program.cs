using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace server
{
    internal class Program
    {
        static List<ClientHandler> clients = new List<ClientHandler>();
        static Dictionary<string, int> products = new Dictionary<string, int>();
        static Dictionary<string, string> accounts = new Dictionary<string, string>();
        static List<string> orders = new List<string>();
        static object lockObj = new object();
        static Random random = new Random();
        static void Main(string[] args)
        {
            InitializeServer();

            TcpListener server = new TcpListener(IPAddress.Any, 8080);
            server.Start();
            Console.WriteLine("Server started...");

            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                ClientHandler handler = new ClientHandler(client);
                Thread clientThread = new Thread(new ThreadStart(handler.Run));
                clientThread.Start();
                lock (lockObj)
                {
                    clients.Add(handler);
                }
            }
        }
        static void InitializeServer()
        {
            products.Add("belt", random.Next(1, 4));
            products.Add("bag", random.Next(1, 4));
            products.Add("computer", random.Next(1, 4));
            products.Add("calculator", random.Next(1, 4));
            products.Add("pencil", random.Next(1, 4));

            accounts.Add("1001", "Tony");
            accounts.Add("1002", "Braxton");
            accounts.Add("1003", "Tchio");
        }
        public class ClientHandler
        {
            private TcpClient client;
            private string userName = string.Empty;

            public ClientHandler(TcpClient client)
            {
                this.client = client;
            }

            public void Run()
            {
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[1024];
                int bytesRead;

                try
                {
                    while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        Console.WriteLine("Received: " + message);

                        string response = HandleMessage(message);
                        byte[] responseData = Encoding.UTF8.GetBytes(response);
                        stream.Write(responseData, 0, responseData.Length);
                        Console.WriteLine("Sent: " + response);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
                finally
                {
                    lock (lockObj)
                    {
                        clients.Remove(this);
                    }
                    stream.Close();
                    client.Close();
                }
            }

            private string HandleMessage(string message)
            {
                string[] parts = message.Split(':');
                string command = parts[0];

                switch (command)
                {
                    case "CONNECT":
                        return HandleConnect(parts[1]);
                    case "GET_PRODUCTS":
                        return HandleGetProducts();
                    case "PURCHASE":
                        return HandlePurchase(parts[1]);
                    case "GET_ORDERS":
                        return HandleGetOrders();
                    case "DISCONNECT":
                        return HandleDisconnect();
                    default:
                        return "UNKNOWN_COMMAND";
                }
            }

            private string HandleConnect(string accountNumber)
            {
                if (accounts.ContainsKey(accountNumber))
                {
                    userName = accounts[accountNumber];
                    return $"CONNECTED:{userName}";
                }
                return "CONNECT_ERROR";
            }

            private string HandleGetProducts()
            {
                if (string.IsNullOrEmpty(userName))
                    return "NOT_CONNECTED";

                StringBuilder response = new StringBuilder("PRODUCTS:");
                foreach (var product in products)
                {
                    response.Append($"{product.Key},{product.Value}|");
                }
                return response.ToString().TrimEnd('|');
            }

            private string HandlePurchase(string productName)
            {
                if (string.IsNullOrEmpty(userName))
                    return "NOT_CONNECTED";

                lock (lockObj)
                {
                    if (products.ContainsKey(productName))
                    {
                        if (products[productName] > 0)
                        {
                            products[productName]--;
                            orders.Add($"{productName},1,{userName}");
                            return $"PURCHASED:{productName}";
                        }
                        else
                        {
                            return "NOT_AVAILABLE";
                        }
                    }
                    else
                    {
                        return "NOT_VALID";
                    }
                }
            }

            private string HandleGetOrders()
            {
                if (string.IsNullOrEmpty(userName))
                    return "NOT_CONNECTED";

                StringBuilder response = new StringBuilder("ORDERS:");
                foreach (var order in orders)
                {
                    response.Append($"{order}|");
                }
                return response.ToString().TrimEnd('|');
            }

            private string HandleDisconnect()
            {
                lock (lockObj)
                {
                    clients.Remove(this);
                }
                return string.Empty;
            }
        }
    }
}

