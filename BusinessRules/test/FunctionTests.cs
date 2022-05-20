using CSIPKeyInGUI.BusinessRules;
using NUnit.Framework;

//2021/03/22_Ares_Stanley-Add test
[TestFixture]
public class FunctionTests
{
    [Test]
    public void GetNum_ExpectedBehavior()
    {
        string strSource = "100";
        var result = BRCommon.GetNum(strSource);
        Assert.AreEqual("100000", result);

        strSource = "abc";
        result = BRCommon.GetNum(strSource);
        Assert.AreEqual("0", result);

        strSource = "100c";
        result = BRCommon.GetNum(strSource);
        Assert.AreEqual("0", result);
    }

    //此測試需再確認
    [Test]
    public void InsertAppointString_ExpectedBehavior()
    {
        string strValue = "abcde";
        int startIndex = 2;
        string strInsertValue = "|";
        var result = BRCommon.InsertAppointString(strValue, startIndex, strInsertValue);
        Assert.AreEqual("ab||cde", result);
    }

    [Test]
    public void GetPadLeftString_ExpectedBehavior()
    {
        string strValue = "abcde";
        int intPadCount = 7;
        char chType = '@';
        var result = BRCommon.GetPadLeftString(strValue, intPadCount, chType);
        Assert.AreEqual("@@abcde", result);

        strValue = "";
        intPadCount = 2;
        chType = '@';
        result = BRCommon.GetPadLeftString(strValue, intPadCount, chType);
        Assert.AreEqual("", result);

        strValue = "X";
        intPadCount = 2;
        chType = '@';
        result = BRCommon.GetPadLeftString(strValue, intPadCount, chType);
        Assert.AreEqual("X", result);
    }

    [Test]
    public void GetPadRightString_ExpectedBehavior()
    {
        string strValue = "abcde";
        int intPadCount = 7;
        char chType = '@';
        var result = BRCommon.GetPadRightString(strValue, intPadCount, chType);
        Assert.AreEqual("abcde@@", result);

        strValue = "";
        intPadCount = 2;
        chType = '@';
        result = BRCommon.GetPadRightString(strValue, intPadCount, chType);
        Assert.AreEqual("", result);

        strValue = "X";
        intPadCount = 2;
        chType = '@';
        result = BRCommon.GetPadRightString(strValue, intPadCount, chType);
        Assert.AreEqual("X", result);
    }

    [Test]
    public void ChangeToSBC_ExpectedBehavior()
    {
        string strInput = "";
        for (int i = 33; i < 127; i++)
        {
            strInput += (char)i;
        }
        var result = BRCommon.ChangeToSBC(strInput);
        Assert.AreEqual("！＂＃＄％＆＇（）＊＋，－．／０１２３４５６７８９：；＜＝＞？＠ＡＢＣＤＥＦＧＨＩＪＫＬＭＮＯＰＱＲＳＴＵＶＷＸＹＺ［＼］＾＿｀ａｂｃｄｅｆｇｈｉｊｋｌｍｎｏｐｑｒｓｔｕｖｗｘｙｚ｛｜｝～", result);
    }
}
