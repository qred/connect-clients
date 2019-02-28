using System;
using Newtonsoft.Json;
using Qred.Connect;
using Xunit;

namespace Tests
{
  public class IntOrRangeTests
  {
    public class Wrapper
    {
      public IntOrRange V { get; set; }
      public override string ToString() => V.ToString();
    }
    [Fact]
    public void cant_parse_invalid()=>
      Assert.Throws<JsonReaderException>(() =>
        JsonConvert.DeserializeObject<Wrapper>("{\"v\":\"4e4e6f13-8bef-4322-8413-bfaa49400d6c\"}"));
    
    [Fact]
    public void can_parse_int() =>
      Assert.Equal("1", JsonConvert.DeserializeObject<Wrapper>("{\"v\":1}").ToString());
    [Fact]
    public void can_parse_range() => 
      Assert.Equal("[0..10]", JsonConvert.DeserializeObject<Wrapper>("{\"v\":[0,10]}").ToString());
    [Fact]
    public void can_parse_from_range()=>
      Assert.Equal("[10...]", JsonConvert.DeserializeObject<Wrapper>("{\"v\":[10,null]}").ToString());

    [Fact]
    public void can_serialize_int()=>
      Assert.Equal("1", JsonConvert.SerializeObject(IntOrRange.Create(1)));
    [Fact]
    public void can_serialize_range()=>
      Assert.Equal("[0,10]", JsonConvert.SerializeObject(IntOrRange.Create(0, 10)));

    [Fact]
    public void can_serialize_from_range() => 
      Assert.Equal("[10,null]", JsonConvert.SerializeObject(IntOrRange.Create(10, null)));
  }
}
