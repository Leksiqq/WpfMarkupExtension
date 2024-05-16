using System.Diagnostics;
using System.Globalization;

namespace TestBoolExpressionConverter;

public class Tests
{
    private BoolExpressionConverter _conv = new();
    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        Trace.Listeners.Add(new ConsoleTraceListener());
        Trace.AutoFlush = true;
        _conv.Verbose = true;
    }

    [Test]
    [TestCase(new object[] { true, false }, "@0 & @1", false, "@0 @1 &")]
    [TestCase(new object[] { false, true }, "@0 & @1", false, "@0 @1 &")]
    [TestCase(new object[] { false, false }, "@0 & @1", false, "@0 @1 &")]
    [TestCase(new object[] { true, true }, "@0 & @1", true, "@0 @1 &")]
    [TestCase(new object[] { true, false, true }, "(@0 | @1) & @2", true, "@0 @1 | @2 &")]
    [TestCase(new object[] { false, true, true }, "(@0 | @1) & @2", true, "@0 @1 | @2 &")]
    [TestCase(new object[] { false, false, true }, "(@0 | @1) & @2", false, "@0 @1 | @2 &")]
    [TestCase(new object[] { true, false, true, false }, "(@0 | @1) & @2|@3", true, "@0 @1 | @2 & @3 |")]
    [TestCase(new object[] { true }, "!@0", false, "@0 !")]
    [TestCase(new object[] { false, false, true }, "!(@0 | @1) & @2", true, "@0 @1 | ! @2 &")]
    public void Test1(object[] values, string expression, bool ans, string postfixRecord)
    {
        if(_conv.Convert(values, typeof(bool), expression, CultureInfo.InvariantCulture) is bool b)
        {
            Assert.That(b, Is.EqualTo(ans));
            Assert.That(_conv.PostfixRecord, Is.EqualTo(postfixRecord));
        }
        else
        {
            Assert.Fail();
        }
    }
}