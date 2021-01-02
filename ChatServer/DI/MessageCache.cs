using System.Collections.Generic;

namespace ChatServer.DI
{
  public class MessageCache : IClientMessageCache
  {
    private readonly List<string> _messages = new List<string>();

    public void Add(string message)
    {
      _messages.Add(message);
    }

    public List<string> GetAll()
    {
      return _messages;
    }
  }
}