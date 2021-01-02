using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using ChatServer.DI;

namespace ChatServer
{
  public class ChatClient
  {
    public Guid Id;
    private readonly TcpClient _tcpClient;
    private readonly IClientMessageCache _messageCache;
    private readonly ClientActions _clientActions;

    public ChatClient(Guid id, TcpClient tcpClient, IClientMessageCache messageCache, ClientActions clientActions)
    {
      Id = id;
      _tcpClient = tcpClient;
      _messageCache = messageCache;
      _clientActions = clientActions;
    }

    public void ReadLoop()
    {
      try
      {
        while (_tcpClient.Connected)
        {
          NetworkStream stream = _tcpClient.GetStream();

          var sr = new StreamReader(stream, Encoding.UTF8);
          string payload = sr.ReadLine();
          if (string.IsNullOrEmpty(payload)) continue;

          var message = CreateMessage(payload);
          SendFeedbackToUser(message, stream);

          _clientActions.OnMessageCreated(Id, message);
        }

        Release();
      }
      catch (Exception e)
      {
        Console.WriteLine(e);
      }
      finally
      {
        Release();
      }
    }

    private void Release()
    {
      _clientActions.OnDisconnect(Id);
      _tcpClient.Close();
    }

    private void SendFeedbackToUser(string message, NetworkStream stream)
    {
      var yourMessage = MessageForYou(message);
      stream.Write(yourMessage, 0, yourMessage.Length);
    }

    private static string CreateMessage(string payload)
    {
      var messageBuilder = new StringBuilder(payload);
      messageBuilder.Append(Environment.NewLine);
      return messageBuilder.ToString();
    }

    private byte[] MessageForYou(string message)
    {
      var humanReadableMessage = $"You: {message}";
      return Encode(humanReadableMessage);
    }

    private byte[] MessageFromOthers(Guid id, string message)
    {
      var humanReadableMessage = $"{id} says: {message}";
      return Encode(humanReadableMessage);
    }

    private static byte[] Encode(string humanReadableMessage)
    {
      return Encoding.UTF8.GetBytes(humanReadableMessage);
    }

    public void Write(Guid sender, string message)
    {
      _messageCache.Add(message);
      var msg = MessageFromOthers(sender, message);
      _tcpClient.GetStream().Write(msg, 0, msg.Length);
    }

    public IClientMessageCache GetMessageCache()
    {
      return _messageCache;
    }
  }

  public class ClientActions
  {
    public  Action<Guid> OnDisconnect { get; set; }
    public  Action<Guid,string> OnMessageCreated { get; set; }
  }
}