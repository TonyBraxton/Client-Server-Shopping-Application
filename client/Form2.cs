using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace client
{
    public partial class Form2 : Form
    {
        private TcpClient client;
        private NetworkStream stream;
        private Dictionary<string, int> products = new Dictionary<string, int>();
        public Form2(TcpClient client, NetworkStream stream)
        {
            InitializeComponent();
            this.client = client;
            this.stream = stream;

            LoadProducts();
            LoadOrders();

        }
        private void LoadProducts()
        {
            SendMessage("GET_PRODUCTS");
            string response = ReceiveMessage();

            if (response.StartsWith("PRODUCTS:"))
            {
                products.Clear();
                string[] productEntries = response.Substring(9).Split('|');
                foreach (var entry in productEntries)
                {
                    string[] parts = entry.Split(',');
                    string name = parts[0];
                    int quantity = int.Parse(parts[1]);
                    products[name] = quantity;
                }
                UpdateProductList();
            }
            else
            {
                MessageBox.Show("Failed to retrieve products.");
            }
        }
        private void LoadOrders()
        {
            try
            {
                SendMessage("GET_ORDERS");
                string response = ReceiveMessage();

                if (response.StartsWith("ORDERS:"))
                {
                    lstOrders.Items.Clear();
                    string[] orderEntries = response.Substring(7).Split('|');
                    foreach (var entry in orderEntries)
                    {
                        lstOrders.Items.Add(entry);
                    }
                }
                else if (response == "NOT_CONNECTED")
                {
                    MessageBox.Show("Not connected to the server.");
                }
                else
                {
                    MessageBox.Show("Failed to retrieve orders.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving orders: {ex.Message}");
            }
        }
        private void UpdateProductList()
        {
            lstProducts.Items.Clear();
            foreach (var product in products)
            {
                lstProducts.Items.Add($"{product.Key} - Quantity: {product.Value}");
            }
        }

        private void btnPurchase_Click(object sender, EventArgs e)
        {
            if (lstProducts.SelectedItem == null)
            {
                MessageBox.Show("Please select a product to purchase.");
                return;
            }

            string selectedProduct = lstProducts.SelectedItem.ToString().Split('-')[0].Trim();
            SendMessage($"PURCHASE:{selectedProduct}");

            string response = ReceiveMessage();
            if (response == $"PURCHASED:{selectedProduct}")
            {
                MessageBox.Show("Purchase successful!");
                LoadProducts();
                LoadOrders();
            }
            else if (response == "NOT_AVAILABLE")
            {
                MessageBox.Show("Product is no longer available.");
            }
            else if (response == "NOT_VALID")
            {
                MessageBox.Show("Invalid product.");
            }
            else if (response == "NOT_CONNECTED")
            {
                MessageBox.Show("Not connected to the server.");
            }
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            SendMessage("DISCONNECT");
            Close();
        }
        private void Form2_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (client != null)
            {
                SendMessage("DISCONNECT");
                client.Close();
            }
        }
        private void SendMessage(string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            stream.Write(data, 0, data.Length);
        }
        private string ReceiveMessage()
        {
            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            return Encoding.UTF8.GetString(buffer, 0, bytesRead);
        }
    }
}
