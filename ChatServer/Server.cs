using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ChatServer.DI;

namespace ChatServer
{
  public class Server
  {
    private TcpListener _listener;

    private readonly object _syncRoot = new object();
    private readonly IMessageStore _store;
    private readonly IClientPool _clientPool;

    public Server() : this(new NoStore(), new ClientPool())
    {
    }

    public Server(IMessageStore store, IClientPool clientPool)
    {
      _store = store;
      _clientPool = clientPool;
    }

    public void Start()
    {
      try
      {
        _listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 10000);
        _listener.Start();
        Console.WriteLine("Server up");

        while (true)
        {
          var acceptTcpClient = _listener.AcceptTcpClient();

          var chatClient = new ChatClient(Guid.NewGuid(), acceptTcpClient, new MessageCache(), new ClientActions()
          {
            OnDisconnect = (id) => _clientPool.Remove(id),
            OnMessageCreated = OnMessageCreated
          });

          _clientPool.Add(chatClient);

          Console.WriteLine("{0} connected to server", chatClient.Id);
          Task.Run(() => chatClient.ReadLoop());
        }
      }
      catch (SocketException e)
      {
        Console.WriteLine("SocketException: {0}", e);
        throw;
      }
      finally
      {
        _listener.Stop();
      }
    }

    private void OnMessageCreated(Guid sender, string message)
    {
      Console.WriteLine("{0} says {1}", sender, message);
      lock (_syncRoot)
      {
        _clientPool.Broadcast(sender, message);
        _store.AddMessage(sender, message);
      }
    }
  }
}