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
    public partial class Form1 : Form
    {
        private TcpClient client;
        private NetworkStream stream;
        public Form1()
        {
            InitializeComponent();
            txtHostName.Text = "localhost";
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                client = new TcpClient(txtHostName.Text, 8080);
                stream = client.GetStream();

                string accountNumber = txtAccNum.Text.Trim();
                SendMessage($"CONNECT:{accountNumber}");

                string response = ReceiveMessage();

                if (response.StartsWith("CONNECTED:"))
                {
                    string userName = response.Split(':')[1];
                    MessageBox.Show($"Welcome, {userName}!");

                    Form2 mainForm = new Form2(client, stream);
                    this.Hide();
                    mainForm.ShowDialog();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Failed to connect. Please check your account number.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
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
