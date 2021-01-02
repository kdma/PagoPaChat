using System;
using System.Threading;
using ChatServer;
using ChatServer.DI;

namespace ChatTest
{
  public class SingleClientTestPeer : INotifyPeers
  {
    private readonly ManualResetEvent _manualResetEvent;

    public SingleClientTestPeer(ManualResetEvent manualResetEvent)
    {
      _manualResetEvent = manualResetEvent;
    }

    public void SignalMessage()
    {
      _manualResetEvent.Set();
    }
  }
}