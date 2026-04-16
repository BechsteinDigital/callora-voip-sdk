namespace CalloraVoipSdk.Core.Application.Media;

/// <summary>
/// Connects media receivers to media senders with bounded in-memory backpressure.
/// </summary>
public sealed class MediaConnector
{
    private const int DefaultQueueCapacity = 256;

    internal MediaConnector()
    {
    }

    /// <summary>
    /// Creates a one-way media connection from receiver to sender.
    /// </summary>
    public IDisposable Connect(IMediaReceiver mediaReceiver, IMediaSender mediaSender) =>
        new MediaConnection(mediaReceiver, mediaSender, DefaultQueueCapacity);

    /// <summary>
    /// Creates a bi-directional media bridge between two endpoints.
    /// </summary>
    public IDisposable CrossConnect(
        IMediaReceiver leftReceiver,
        IMediaSender leftSender,
        IMediaReceiver rightReceiver,
        IMediaSender rightSender)
    {
        var first = Connect(leftReceiver, rightSender);
        var second = Connect(rightReceiver, leftSender);
        return new CompositeDisposable(first, second);
    }
}
