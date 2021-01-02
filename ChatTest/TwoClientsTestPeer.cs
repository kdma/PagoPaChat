using System.Threading;
using ChatServer.DI;

namespace ChatTest
{
  public class TwoClientsTestPeer : INotifyPeers
  {
    private readonly ManualResetEvent _manualResetEvent;
    private int _count = 0;

    public TwoClientsTestPeer(ManualResetEvent manualResetEvent)
    {
      _manualResetEvent = manualResetEvent;
    }

    public void SignalMessage()
    {
      _count++;
      if (_count == 2)
      {
        _manualResetEvent.Set();
      }
    }
  }
}