using System;
using System.Collections.Generic;

namespace ChatServer.DI
{
  public interface IClientPool
  {
    List<ChatClient> GetConnectedClients();
    void Add(ChatClient chatClient);
    void Broadcast(Guid sender, string message);
    void Remove(Guid guid);
  }
}