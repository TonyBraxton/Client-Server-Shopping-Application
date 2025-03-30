# Client-Server Shopping Application
# 🛒 Client-Server Shopping Application

## Project Overview
This project implements a simple **client-server shopping application** where:
- The **server** manages a list of products and client accounts.
- Multiple **clients** can connect to the server to:
  - View available products.
  - Make purchases.
  - View their order history.

The project demonstrates fundamental concepts of client-server architecture, TCP-based network communication, concurrency management, and GUI design.

---

## 📁 Project Structure
##  Key Features
### **Server Side**
The server is a console application that:
- **Initializes** predefined products with random quantities and creates three user accounts.
- **Listens** for client connections and processes commands sent by clients using TCP.

### Supported Commands
1. **CONNECT:**  
   Validates the client's account number and establishes a connection.
2. **GET_PRODUCTS:**  
   Sends the list of available products to the client.
3. **PURCHASE:**  
   Handles the purchase request, updates product inventory, and confirms the transaction.
4. **GET_ORDERS:**  
   Sends the client their purchase history.
5. **DISCONNECT:**  
   Ends the client session and releases resources.

The server handles multiple clients simultaneously through **multi-threading**, ensuring that all clients can interact independently.

---

###  **Client Side**
The client is a **Windows Forms** application with two forms:
- **Form1 (Login Form):**  
  - Allows the user to enter the hostname (default: `localhost`) and account number.
  - If successful, hides `Form1` and opens `Form2`.

- **Form2 (Main Shopping Form):**  
  - Displays available products.
  - Allows the user to:
    - Make purchases.
    - View their order history.
    - Disconnect gracefully from the server.

---

## **Communication Protocol**
Communication between the client and server uses **TCP** with `NetworkStream` for reliable data transfer.
- **Client Communication Methods:**
  - `SendMessage()` – Sends commands to the server.
  - `ReceiveMessage()` – Receives and interprets server responses.
- **Server Processing:**
  - Listens for client commands.
  - Processes requests and sends appropriate responses.

---

## **Concurrency Management**
- The server uses **multi-threading** to handle multiple clients simultaneously.
- Each client connection is processed in a separate thread, ensuring that multiple clients can interact without blocking each other.

---

##  How to Run
### **Step 1: Start the Server**
1. Open the `Server` project in Visual Studio.
2. Run `Program.cs` to start the server.
3. The server will listen for incoming client connections.

###  **Step 2: Run the Client Application**
1. Open the `Client` project in Visual Studio.
2. Run `Program.cs` to launch the Windows Forms application.
3. Enter the hostname (default is `localhost`) and account number to connect.
4. Interact with the available options on `Form2` after a successful login.

---

##  Usage Instructions
1. **Login:**
   - Open the client and enter valid account credentials.
2. **View Products:**
   - Click the “Get Products” button to see available products.
3. **Purchase Products:**
   - Select a product and click “Purchase” to complete the transaction.
4. **View Orders:**
   - Click “Get Orders” to view the list of past purchases.
5. **Disconnect:**
   - Click “Disconnect” to end the session gracefully.

---

## Configuration Details
- **Default Hostname:** `localhost`
- **Port:** 5000 (modifiable in `Program.cs` if needed)
- **Account Numbers:** Predefined on the server.

---

## Code Highlights
### Server Initialization
```csharp
// Initialize predefined products and accounts
ProductManager.InitializeProducts();
ProductManager.CreateDefaultAccounts();
Multi-threading for Client Handling
csharp
Copy
Edit
while (true)
{
    TcpClient client = server.AcceptTcpClient();
    Thread clientThread = new Thread(() => HandleClient(client));
    clientThread.Start();
}
🛒 Purchase Logic
csharp
Copy
Edit
if (ProductManager.PurchaseProduct(accountNumber, productId, quantity))
{
    SendResponse("PURCHASE_SUCCESS");
}
else
{
    SendResponse("PURCHASE_FAILED");
}
 
