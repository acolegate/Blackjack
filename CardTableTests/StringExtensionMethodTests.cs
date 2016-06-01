using System;

using CardTable;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using MSTestExtensions;
//https://github.com/bbraithwaite/mstestextensions

namespace CardTableTests
{
    [TestClass]
    public class StringExtensionMethodTests : BaseTest
    {
        [TestMethod]
        public void StringExtensionMethodTests_Overwrite_VariousAndEdgeCases()
        {
            // act/assert
            Assert.AreEqual("456def", "abcdef".Overwrite(-3, "123456"), "Unexpected value returned");
            Assert.AreEqual("123def", "abcdef".Overwrite(0, "123"), "Unexpected value returned");
            Assert.AreEqual("ab123f", "abcdef".Overwrite(2, "123"), "Unexpected value returned");
            Assert.AreEqual("abc123", "abcdef".Overwrite(3, "123"), "Unexpected value returned");
            Assert.AreEqual("abc123", "abcdef".Overwrite(3, "123456"), "Unexpected value returned");
            Assert.AreEqual("123456", "abcdef".Overwrite(0, "123456"), "Unexpected value returned");
            Assert.AreEqual("456789", "abcdef".Overwrite(-3, "123456789"), "Unexpected value returned");
            Assert.AreEqual("123456", "abcdef".Overwrite(0, "123456789"), "Unexpected value returned");
            Assert.AreEqual("456789", "abcdef".Overwrite(-3, "123456789012"), "Unexpected value returned");
            Assert.AreEqual("abcdef", "abcdef".Overwrite(-3, "123"), "Unexpected value returned");
            Assert.AreEqual("abcdef", "abcdef".Overwrite(6, "123"), "Unexpected value returned");
            Assert.AreEqual("abcdef", "abcdef".Overwrite(-4, "123"), "Unexpected value returned");
            Assert.AreEqual("abcdef", "abcdef".Overwrite(7, "123"), "Unexpected value returned");

            Assert.AreEqual("abcdef", "abcdef".Overwrite(0, ""), "Unexpected value returned");
            Assert.AreEqual("abcdef", "abcdef".Overwrite(3, ""), "Unexpected value returned");
            Assert.AreEqual("abcdef", "abcdef".Overwrite(6, ""), "Unexpected value returned");
            Assert.AreEqual("abcdef", "abcdef".Overwrite(-3, ""), "Unexpected value returned");
            Assert.AreEqual("abcdef", "abcdef".Overwrite(7, ""), "Unexpected value returned");

            Assert.AreEqual("", "".Overwrite(0, "abcdef"), "Unexpected value returned");
            Assert.AreEqual("", "".Overwrite(-3, "abcdef"), "Unexpected value returned");
            Assert.AreEqual("", "".Overwrite(6, "abcdef"), "Unexpected value returned");
            Assert.AreEqual("", "".Overwrite(-8, "abcdef"), "Unexpected value returned");
            Assert.AreEqual("", "".Overwrite(8, "abcdef"), "Unexpected value returned");


            Assert.Throws<ArgumentException>(() => ((string)null).Overwrite(0, "123"), "Destination must not be null");
            Assert.Throws<ArgumentException>(() => "abcdef".Overwrite(0, null), "Substring must not be null");
        }
    }
}