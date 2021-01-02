using System;
using System.Collections.Generic;

namespace ChatServer.DI
{
  public class NoStore : IMessageStore
  {
    public void AddMessage(Guid id, string message)
    {
      ;
    }

    public Dictionary<Guid, string> Read()
    {
      return new Dictionary<Guid, string>();
    }
  }
}