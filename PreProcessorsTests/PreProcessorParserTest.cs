using System;
using System.Collections.Generic;
using Embedded_C_Parser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace PreProcessorsTests
{
    /// <summary>
    ///This is a test class for PreProcessorParserTest and is intended
    ///to contain all PreProcessorParserTest Unit Tests
    ///</summary>
    [TestClass()]
    public class PreProcessorParserTest
    {
        private TestContext testContextInstance;
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        /// <summary>
        ///A test for RemoveMultiLineComments
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Embedded_C_Parser.exe")]
        public void RemoveMultiLineCommentsTest()
        {
            var inputCode = new List<string>() {
                @"dsadsa /* fdsfjdis // iropew",
                @"dsadsa */ gdkops"
            };
            PreProcessorParser target = new PreProcessorParser(inputCode);
            PrivateObject po = new PrivateObject(target);
            po.Invoke("RemoveMultiLineComments", new Type[]{}, new Object[]{});

            List<string> actual = (List<string>) po.GetField("code");

            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual(@"dsadsa ", actual[0]);
            Assert.AreEqual(@" gdkops", actual[1]);
        }
        /// <summary>
        ///A test for RemoveOneLineComments
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Embedded_C_Parser.exe")]
        public void RemoveOneLineCommentsTest()
        {
            var inputCode = new List<string>() {
                @"dsadsa /* fdsfjdis // iropew",
                @"dsadsa */ gdkops"
            };
            PreProcessorParser target = new PreProcessorParser(inputCode);
            PrivateObject po = new PrivateObject(target);
            po.Invoke("RemoveOneLineComments", new Type[] { }, new Object[] { });

            List<string> actual = (List<string>)po.GetField("code");

            Assert.AreEqual(2, actual.Count);
            Assert.AreEqual(@"dsadsa /* fdsfjdis ", actual[0]);
            Assert.AreEqual(@"dsadsa */ gdkops", actual[1]);
        }

        /// <summary>
        ///A test for RemoveTextBlock
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Embedded_C_Parser.exe")]
        public void RemoveTextBlockTestFromOneLine()
        {
            List<string> inputCode = new List<string>()
            {
                "12345"
            };
            PreProcessorParser target = new PreProcessorParser(inputCode);
            PrivateObject po = new PrivateObject(target);
            int startLine = 0;
            int startChar = 1;
            int endLine = 0;
            int endChar = 3;
            po.Invoke(
                "RemoveTextBlock",
                new Type[] { typeof(int), typeof(int), typeof(int), typeof(int) },
                new object[] { startLine, startChar, endLine, endChar }
            );

            List<string> expected = new List<string>()
            {
                "145",
            };

            List<string> actual = (List<string>)po.GetField("code");

            Assert.AreEqual(expected.Count, actual.Count);
            for (int i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i], actual[i]);
            }
        }

        /// <summary>
        ///A test for RemoveTextBlock
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Embedded_C_Parser.exe")]
        public void RemoveTextBlockTestFromManyLines()
        {
            List<string> inputCode = new List<string>()
            {
                "12345",
                "12345",
                "12345",
                "12345"
            };
            PreProcessorParser target = new PreProcessorParser(inputCode);
            PrivateObject po = new PrivateObject(target);
            int startLine = 1;
            int startChar = 2;
            int endLine = 3;
            int endChar = 3;
            po.Invoke(
                "RemoveTextBlock",
                new Type[] { typeof(int), typeof(int), typeof(int), typeof(int) },
                new object[] { startLine, startChar, endLine, endChar }
            );

            string a = "23";
            
            List<string> expected = new List<string>()
            {
                "12345",
                "12",
                "",
                "45"
            };

            List<string> actual = (List<string>)po.GetField("code");

            Assert.AreEqual(expected.Count, actual.Count);
            for (int i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i], actual[i]);
            }
        }

        [TestMethod()]
        [DeploymentItem("Embedded_C_Parser.exe")]
        public void ifndefendifTest()
        {
            List<string> inputCode = new List<string>()
            {
                "#ifndef dupa",
                "tekst1",
                "#endif",
                "#ifdef dupa",
                "tekst2",
                "#endif",
                "#define dupa",
                "#ifndef dupa",
                "tekst3",
                "#endif",
                "#ifdef dupa",
                "tekst4",
                "#endif"
            };
            PreProcessorParser target = new PreProcessorParser(inputCode);
            PrivateObject po = new PrivateObject(target);
            po.Invoke("GetMacros");

            List<string> expected = new List<string>()
            {
                "",
                "tekst1",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                "",
                "tekst4",
                ""
            };

            List<string> actual = (List<string>)po.GetField("code");

            Assert.AreEqual(expected.Count, actual.Count);
            for (int i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i], actual[i]);
            }
        }

        [TestMethod()]
        [DeploymentItem("Embedded_C_Parser.exe")]
        public void definitionsGatheringTest()
        {
            List<string> inputCode = new List<string>()
            {
                "#define dupa 2dupy\\",
                " a nawet 3 dupy"
            };
            PreProcessorParser target = new PreProcessorParser(inputCode);
            PrivateObject po = new PrivateObject(target);
            po.Invoke("GetMacros");

            List<KeyValuePair<string, string>> actual = (List<KeyValuePair<string, string>>)po.GetField("objectLikeMacros");

            Assert.AreEqual(1, actual.Count);
            Assert.AreEqual("dupa", actual[0].Key);
            Assert.AreEqual("2dupy a nawet 3 dupy", actual[0].Value);
        }

        [TestMethod()]
        [DeploymentItem("Embedded_C_Parser.exe")]
        public void StubsTest()
        {
            List<string> expected = new List<string>()
            {
                "12345",
                "1",
                "45"
            };

            var mStubParser = MockRepository.GenerateStub<PreProcessorParser>(new Object[] { new List<string>() { "asd" } });
            mStubParser.Stub(x => x.Parse()).Return(new List<string>(){
                "12345",
                "1",
                "45"
            });

            List<string> actual = mStubParser.Parse();

            Assert.AreEqual(expected.Count, actual.Count);
            for (int i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i], actual[i]);
            }

            var mMockParer = MockRepository.GenerateMock<PreProcessorParser>(new Object[] { new List<string>() { "asd" } });
            mMockParer.Parse();
            mMockParer.AssertWasCalled(x => x.Parse());
            mStubParser.AssertWasCalled(x => x.Parse());
        }
        #region Additional test attributes

        //
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //

        #endregion Additional test attributes
    }
}