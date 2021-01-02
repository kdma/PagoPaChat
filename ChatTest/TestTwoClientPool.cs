using System;
using System.Collections.Generic;
using System.Threading;
using ChatServer;
using ChatServer.DI;

namespace ChatTest
{
  public class TestTwoClientPool : IClientPool
  {
    private readonly ClientPool _clientPool;
    private readonly ManualResetEvent _manualResetEvent;
    
    public TestTwoClientPool(ClientPool clientPool, ManualResetEvent manualResetEvent)
    {
      _clientPool = clientPool;
      _manualResetEvent = manualResetEvent;
    }

    public List<ChatClient> GetConnectedClients()
    {
      return _clientPool.GetConnectedClients();
    }

    public void Add(ChatClient chatClient)
    {
      _clientPool.Add(chatClient);
      if (_clientPool.GetConnectedClients().Count == 2)
      {
        _manualResetEvent.Set();
      }
    }

    public void Broadcast(Guid sender, string message)
    {
      _clientPool.Broadcast(sender, message);
    }

    public void Remove(Guid guid)
    {
      _clientPool.Remove(guid);
    }
  }
}