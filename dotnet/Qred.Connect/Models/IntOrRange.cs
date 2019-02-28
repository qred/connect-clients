using System;
using Newtonsoft.Json;

namespace Qred.Connect
{

  [JsonConverter(typeof(IntOrRangeConverter))]
  public abstract class IntOrRange
  {
    private class Int : IntOrRange
    {
      private long value;

      public Int(long value)
      {
        this.value = value;
      }

      public override T Match<T>(Func<long, T> onValue, Func<long, long?, T> onRange)
      {
        return onValue(value);
      }

      public override void Match(Action<long> onValue, Action<long, long?> onRange)
      {
        onValue(value);
      }
      public override string ToString() => value.ToString();
      public override int GetHashCode() => value.GetHashCode();
      public override bool Equals(Object other)
      {
        if (ReferenceEquals(null, other))
        {
          return false;
        }
        if (other is Int)
        {
          var o = (Int)other;
          return Equals(value, o.value);
        }
        return false;
      }
    }
    private class IntRange : IntOrRange
    {
      private long from;
      private long? to;

      public IntRange(long from, long? to)
      {
        this.from = from;
        this.to = to;
      }
      public override string ToString() => $"[{from}..{to?.ToString() ?? "."}]";
      public override int GetHashCode() => from.GetHashCode() + to?.GetHashCode() ?? 0;
      public override bool Equals(Object other)
      {
        if (ReferenceEquals(null, other))
        {
          return false;
        }
        if (other is IntRange)
        {
          var o = (IntRange)other;
          return Equals(from, o.from) && Equals(to, o.to);
        }
        return false;
      }

      public override T Match<T>(Func<long, T> onValue, Func<long, long?, T> onRange)
      {
        return onRange(from, to);
      }

      public override void Match(Action<long> onValue, Action<long, long?> onRange)
      {
        onRange(from, to);
      }
    }
    public static IntOrRange Create(long value)
    {
      return new Int(value);
    }
    public static IntOrRange Create(long from, long? to)
    {
      return new IntRange(from, to);
    }
    public abstract T Match<T>(Func<long, T> onValue, Func<long, long?, T> onRange);
    public abstract void Match(Action<long> onValue, Action<long, long?> onRange);

 
  }
}