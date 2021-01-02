using System;
using System.Collections.Generic;

namespace ChatServer.DI
{
  public interface IMessageStore
  {
    void AddMessage(Guid id, string message);
    Dictionary<Guid, string> Read();
  }
}