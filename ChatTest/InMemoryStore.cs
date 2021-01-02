using System;
using System.Collections.Generic;
using ChatServer;
using ChatServer.DI;

namespace ChatTest
{
  public class InMemoryStore : IMessageStore
  {
    private readonly INotifyPeers _peerNotifier;

    public InMemoryStore(INotifyPeers peerNotifier)
    {
      _peerNotifier = peerNotifier;
    }

    private readonly Dictionary<Guid, string> _msgHistory = new Dictionary<Guid, string>();

    public void AddMessage(Guid id, string message)
    {
      _msgHistory.Add(id, message);
      _peerNotifier.SignalMessage();
    }

    public Dictionary<Guid, string> Read()
    {
      return _msgHistory;
    }
  }
}