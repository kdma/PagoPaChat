using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ChatServer;
using ChatServer.DI;
using ChatTest;
using NUnit.Framework;

namespace Tests
{
  public class Tests
  {
    [Test]
    public void ServerIsListening()
    {
      var listener = new Server();
      Task.Run(() => listener.Start());

      Assert.Throws<SocketException>(() =>
      {
        var clone = new Server();
        clone.Start();
      });

    }

    [Test]
    public void AClientCanConnect()
    {
      var clientPool = new ClientPool();
      var server = new Server(new NoStore(), clientPool);
      Task.Run(() => server.Start());

      var aClient = new TcpClient();
      aClient.Connect(IPAddress.Parse("127.0.0.1"), 10000);
      Assert.That(clientPool.GetConnectedClients(), Has.Count.EqualTo(1));
    }

    [Test]
    public void MultipleClientsCanConnect()
    {
      var connectionWait = new ManualResetEvent(false);
      var clientPool = new TestTwoClientPool(new ClientPool(), connectionWait);
      var server = new Server(new NoStore(), clientPool);
      Task.Run(() => server.Start());

      var aClient = new TcpClient();
      aClient.Connect(IPAddress.Parse("127.0.0.1"), 10000);

      var anotherClient = new TcpClient();
      anotherClient.Connect(IPAddress.Parse("127.0.0.1"), 10000);

      connectionWait.WaitOne();

      Assert.That(clientPool.GetConnectedClients(), Has.Count.EqualTo(2));
    }

    [Test]
    public void AClientCanSendAMessage()
    {
      var msgWait = new ManualResetEvent(false);

      var inMemoryStore = new InMemoryStore(new SingleClientTestPeer(msgWait));
      var clientPool = new ClientPool();

      var server = new Server(inMemoryStore, clientPool);
      Task.Run(() => server.Start());

      var aClient = new TcpClient();
      aClient.Connect(IPAddress.Parse("127.0.0.1"), 10000);

      Assert.That(aClient.GetStream().CanWrite);

      var message = $"Hello, World!{Environment.NewLine}";
      var msgBuffer = Encoding.ASCII.GetBytes(message);
      aClient.GetStream().Write(msgBuffer, 0, msgBuffer.Length);

      msgWait.WaitOne();

      StringAssert.AreEqualIgnoringCase(message, inMemoryStore.Read().First().Value);
    }

    [Test]
    public void TwoClientsCanCommunicate()
    {
      var msgWait = new ManualResetEvent(false);
      var inMemoryStore = new InMemoryStore(new TwoClientsTestPeer(msgWait));

      var connectionWait = new ManualResetEvent(false);
      var clientPool = new TestTwoClientPool(new ClientPool(), connectionWait);

      var server = new Server(inMemoryStore, clientPool);
      Task.Run(() => server.Start());

      var aTcpClient = new TcpClient();
      aTcpClient.Connect(IPAddress.Parse("127.0.0.1"), 10000);

      var anotherTcpClient = new TcpClient();
      anotherTcpClient.Connect(IPAddress.Parse("127.0.0.1"), 10000);

      connectionWait.WaitOne();

      var aClient = clientPool.GetConnectedClients().First();
      var anotherClient = clientPool.GetConnectedClients().Last();

      var message = $"Hello, World!{Environment.NewLine}";
      var msgBuffer = Encoding.UTF8.GetBytes(message);
      aTcpClient.GetStream().Write(msgBuffer, 0, msgBuffer.Length);

      var anotherMessage = $"Have you heard about our lord and savior?{Environment.NewLine}";
      var otherMsgBuffer = Encoding.UTF8.GetBytes(anotherMessage);
      anotherTcpClient.GetStream().Write(otherMsgBuffer, 0, otherMsgBuffer.Length);

      msgWait.WaitOne();

      var firstClientMessagesReceived = aClient.GetMessageCache();
      var secondClientMessagesReceived = anotherClient.GetMessageCache();

      StringAssert.AreEqualIgnoringCase(anotherMessage, firstClientMessagesReceived.GetAll().First());
      StringAssert.AreEqualIgnoringCase(message, secondClientMessagesReceived.GetAll().First());
    }

  }
}