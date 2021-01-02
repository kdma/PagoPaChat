using System.Collections.Generic;

namespace ChatServer.DI
{
  public interface IClientMessageCache
  {
    void Add(string message);
    List<string> GetAll();
  }
}