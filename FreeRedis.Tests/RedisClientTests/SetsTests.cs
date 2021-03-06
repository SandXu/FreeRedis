using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Xunit;

namespace FreeRedis.Tests.RedisClientTests
{
    public class SetsTests : TestBase
    {
        [Fact]
        public void SAdd()
        {
            cli.Del("TestSAdd1");
            Assert.Equal(4, cli.SAdd("TestSAdd1", Null, Class, String, Bytes));
            Assert.Equal(Class.ToString(), cli.SPop<TestClass>("TestSAdd1")?.ToString());
            Assert.Equal(3, cli.SCard("TestSAdd1"));
        }

        [Fact]
        public void SCard()
        {
            cli.Del("TestSCard1");
            Assert.Equal(4, cli.SAdd("TestSCard1", Null, Class, String, Bytes));
            Assert.Equal(4, cli.SCard("TestSCard1"));
        }

        [Fact]
        public void SDiff()
        {
            cli.Del("TestSDiff1", "TestSDiff2");
            Assert.Equal(2, cli.SAdd("TestSDiff1", String, Class));
            Assert.Equal(2, cli.SAdd("TestSDiff2", Null, Bytes));
            Assert.Equal(2, cli.SDiff("TestSDiff1", "TestSDiff2").Length);
            Assert.Equal(String, cli.SDiff("TestSDiff1", "TestSDiff2")[0]);
            Assert.Equal(Class.ToString(), cli.SDiff("TestSDiff1", "TestSDiff2")[1]);
        }

        [Fact]
        public void SDiffStore()
        {
            cli.Del("TestSDiffStore1", "TestSDiffStore2", "TestSDiffStore3");
            Assert.Equal(2, cli.SAdd("TestSDiffStore1", String, Class));
            Assert.Equal(2, cli.SAdd("TestSDiffStore2", Null, Bytes));
            Assert.Equal(2, cli.SDiffStore("TestSDiffStore3", "TestSDiffStore1", "TestSDiffStore2"));
            Assert.Equal(2, cli.SCard("TestSDiffStore3"));
            Assert.Null(cli.SPop("TestSDiffStore3"));
            Assert.Equal(String, cli.SPop("TestSDiffStore3"));
        }

        [Fact]
        public void SInter()
        {
            cli.Del("TestSInter1", "TestSInter2");
            Assert.Equal(4, cli.SAdd("TestSInter1", Null, Class, String, Bytes));
            Assert.Equal(4, cli.SAdd("TestSInter2", Null, Null, String, String));
            Assert.Equal(2, cli.SInter("TestSInter1", "TestSInter2").Length);
            Assert.Equal(String, cli.SInter("TestSInter1", "TestSInter2")[0]);
            Assert.Null(cli.SInter("TestSInter1", "TestSInter2")[1]);
        }

        [Fact]
        public void SInterStore()
        {
            cli.Del("TestSInterStore1", "TestSInterStore2", "TestSInterStore3");
            Assert.Equal(4, cli.SAdd("TestSInterStore1", Null, Class, String, Bytes));
            Assert.Equal(4, cli.SAdd("TestSInterStore2", Null, Null, String, String));
            Assert.Equal(2, cli.SInterStore("TestSInterStore3", "TestSInterStore1", "TestSInterStore2"));
            Assert.Equal(2, cli.SCard("TestSInterStore3"));
            Assert.Null(cli.SPop("TestSInterStore3"));
            Assert.Equal(String, cli.SPop("TestSInterStore3"));
        }

        [Fact]
        public void SIsMember()
        {
            cli.Del("TestSIsMember1");
            Assert.Equal(4, cli.SAdd("TestSIsMember1", Null, Class, String, Bytes));
            Assert.True(cli.SIsMember("TestSIsMember1", Null));
            Assert.True(cli.SIsMember("TestSIsMember1", String));
            Assert.True(cli.SIsMember("TestSIsMember1", Bytes));
            Assert.True(cli.SIsMember("TestSIsMember1", Class));
            Assert.Equal(4, cli.SCard("TestSIsMember1"));
        }

        [Fact]
        public void SMeMembers()
        {
            cli.Del("TestSMeMembers1");
            Assert.Equal(4, cli.SAdd("TestSMeMembers1", Null, Class, String, Bytes));
            Assert.Null(cli.SMeMembers("TestSMeMembers1")[0]);
            Assert.Null(cli.SMeMembers("TestSMeMembers1")[1]);
            Assert.Equal(String, cli.SMeMembers("TestSMeMembers1")[2]);
            Assert.Equal(String, cli.SMeMembers("TestSMeMembers1")[3]);
            Assert.Equal(Encoding.UTF8.GetString(Bytes), cli.SMeMembers("TestSMeMembers1")[4]);
            Assert.Equal(Encoding.UTF8.GetString(Bytes), cli.SMeMembers("TestSMeMembers1")[5]);
            Assert.Equal(Class.ToString(), JsonConvert.DeserializeObject<TestClass>(cli.SMeMembers("TestSMeMembers1")[6]).ToString());
            Assert.Equal(Class.ToString(), JsonConvert.DeserializeObject<TestClass>(cli.SMeMembers("TestSMeMembers1")[7]).ToString());
        }

        [Fact]
        public void SMove()
        {
            cli.Del("TestSMove1", "TestSMove2");
            Assert.Equal(4, cli.SAdd("TestSMove1", Null, Class, String, Bytes));
            Assert.True(cli.SMove("TestSMove1", "TestSMove2", Class));
            Assert.False(cli.SMove("TestSMove1", "TestSMove2", "123123123xxxdx123"));
            Assert.Equal(1, cli.SCard("TestSMove2"));
            Assert.Equal(Class.ToString(), cli.SPop<TestClass>("TestSMove2").ToString());
        }

        [Fact]
        public void SPop()
        {
            cli.Del("TestSPop1");
            Assert.Equal(4, cli.SAdd("TestSPop1", Null, Class, String, Bytes));
            Assert.Equal(Class.ToString(), cli.SPop<TestClass>("TestSPop1")?.ToString());
            Assert.Equal(Class.ToString(), cli.SPop<TestClass>("TestSPop1")?.ToString());
            Assert.Equal(Encoding.UTF8.GetString(Bytes), Encoding.UTF8.GetString(cli.SPop<byte[]>("TestSPop1")));
            Assert.Equal(Encoding.UTF8.GetString(Bytes), Encoding.UTF8.GetString(cli.SPop<byte[]>("TestSPop1")));
            Assert.Equal(String, cli.SPop("TestSPop1"));
            Assert.Equal(String, cli.SPop("TestSPop1"));
            Assert.Null(cli.SPop("TestSPop1"));
            Assert.Null(cli.SPop("TestSPop1"));
        }

        [Fact]
        public void SRandMember()
        {
            cli.Del("TestSRandMember1");
            Assert.Equal(3, cli.SAdd("TestSRandMember1", String, String, Bytes, Bytes, Class, Class));
            Assert.NotNull(cli.SPop("TestSRandMember1"));
            Assert.NotNull(cli.SPop("TestSRandMember1"));
            Assert.NotNull(cli.SPop("TestSRandMember1"));
            Assert.Null(cli.SPop("TestSRandMember1"));
        }

        [Fact]
        public void SRem()
        {
            cli.Del("TestSRem1");
            Assert.Equal(4, cli.SAdd("TestSRem1", Null, Class, String, Bytes));
            Assert.Equal(4, cli.SRem("TestSRem1", Null, String, Bytes, Class));
            Assert.Null(cli.SPop("TestSRem1"));
        }

        [Fact]
        public void SScan()
        {
            cli.Del("TestSScan1");
            Assert.Equal(4, cli.SAdd("TestSScan1", Null, Class, String, Bytes));
            Assert.Equal(4, cli.SScan("TestSScan1", 0, "*", 10).items.Length);
        }

        [Fact]
        public void SUnion()
        {
            cli.Del("TestSUnion1", "TestSUnion2");
            Assert.Equal(2, cli.SAdd("TestSUnion1", Bytes, Bytes, Class, Class));
            Assert.Equal(2, cli.SAdd("TestSUnion2", Null, Null, String, String));
            Assert.Equal(4, cli.SUnion("TestSUnion1", "TestSUnion2").Length);
            Assert.Equal(Encoding.UTF8.GetString(Bytes), cli.SUnion("TestSUnion1", "TestSUnion2")[0]);
            Assert.Equal(Encoding.UTF8.GetString(Bytes), cli.SUnion("TestSUnion1", "TestSUnion2")[1]);
            Assert.Equal(Class.ToString(), cli.SUnion("TestSUnion1", "TestSUnion2")[2]);
            Assert.Equal(Class.ToString(), cli.SUnion("TestSUnion1", "TestSUnion2")[3]);
            Assert.Null(cli.SUnion("TestSUnion1", "TestSUnion2")[4]);
            Assert.Null(cli.SUnion("TestSUnion1", "TestSUnion2")[5]);
            Assert.Equal(String, cli.SUnion("TestSUnion1", "TestSUnion2")[6]);
            Assert.Equal(String, cli.SUnion("TestSUnion1", "TestSUnion2")[7]);
        }

        [Fact]
        public void SUnionStore()
        {
            cli.Del("TestSUnionStore1", "TestSUnionStore2", "TestSUnionStore3");
            Assert.Equal(2, cli.SAdd("TestSUnionStore1", Bytes, Bytes, Class, Class));
            Assert.Equal(2, cli.SAdd("TestSUnionStore2", Null, Null, String, String));
            Assert.Equal(4, cli.SUnionStore("TestSUnionStore3", "TestSUnionStore1", "TestSUnionStore2"));
            Assert.Equal(4, cli.SCard("TestSUnionStore3"));
            Assert.Equal(String, cli.SPop("TestSUnionStore3"));
            Assert.Equal(String, cli.SPop("TestSUnionStore3"));
            Assert.Null(cli.SPop("TestSUnionStore3"));
            Assert.Null(cli.SPop("TestSUnionStore3"));
            Assert.Equal(Class.ToString(), cli.SPop("TestSUnionStore3"));
            Assert.Equal(Class.ToString(), cli.SPop("TestSUnionStore3"));
            Assert.Equal(Encoding.UTF8.GetString(Bytes), cli.SPop("TestSUnionStore3"));
            Assert.Equal(Encoding.UTF8.GetString(Bytes), cli.SPop("TestSUnionStore3"));
        }

    }
}
