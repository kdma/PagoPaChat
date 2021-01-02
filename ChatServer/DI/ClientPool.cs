using System;
using System.Collections.Generic;
using System.Linq;

namespace ChatServer.DI
{
  public class ClientPool : IClientPool
  {
    private readonly List<ChatClient> _connectedClients = new List<ChatClient>();

    public List<ChatClient> GetConnectedClients()
    {
      return _connectedClients;
    }

    public void Add(ChatClient chatClient)
    {
      _connectedClients.Add(chatClient);
    }

    public void Broadcast(Guid sender, string message)
    {
      foreach (var otherClient in _connectedClients.Where(c => c.Id != sender))
      {
        otherClient.Write(sender, message);
      }
    }

    public void Remove(Guid guid)
    {
      Console.WriteLine($"{guid} disconnected");
      _connectedClients.Remove(_connectedClients.FirstOrDefault(c => c.Id == guid));
    }
  }
}