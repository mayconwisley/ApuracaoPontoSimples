namespace ApuracaoPontoSimples.Domain.ValueObjects;

public sealed class TimeInterval
{
    public TimeSpan? Start { get; set; }
    public TimeSpan? End { get; set; }

    public TimeSpan Duration => (Start.HasValue && End.HasValue) ? End.Value - Start.Value : TimeSpan.Zero;

    public TimeInterval(TimeSpan? start, TimeSpan? end)
    {
        Start = start;
        End = end;
    }

    public TimeInterval() { }
}
