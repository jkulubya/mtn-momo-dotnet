using System;
using Xunit;
using MomoApi.NET;

namespace Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var x = new Momo();
            var y = x.Collections;
        }
    }
}