using FMSC.Sampling;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
namespace Sampling.Test
{
    
    
    /// <summary>
    ///This is a test class for BlockSelecterTest and is intended
    ///to contain all BlockSelecterTest Unit Tests
    ///</summary>
    [TestClass()]
    public class BlockSelecterTest
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
        #endregion


        [TestMethod()]
        public void TestBlockSelecter()
        {
            decimal tolarance = .01m;
            int freqency = 15;
            int iFrequency = 0;
            BlockSelecter selecter = new BlockSelecter(freqency, iFrequency);

            int numSamples = freqency * 2 * 5 * 1000;
            int[] results = new int[numSamples];
            int totalSamples = 0;
            int totalISamples = 0;

            for (int i = 0; i < numSamples; i++)
            {
                SampleItem item = selecter.NextItem();

                if (item != null && item.IsInsuranceItem)
                {
                    results[i] = 2;
                    totalISamples++;
                }
                else if (item != null && item.IsSelected)
                {
                    results[i] = 1;
                    totalSamples++;
                }
                else
                {
                    results[i] = 0;
                }
            }

            Assert.IsTrue(selecter.ITreeFrequency == iFrequency);

            this.TestContext.WriteLine(" numsamples  = {0}", numSamples.ToString());
            this.TestContext.WriteLine("total samples  = {0}", totalSamples.ToString());
            this.TestContext.WriteLine("total Isamples = {0}", totalISamples.ToString());

            decimal observedFreq = (totalSamples / (decimal)numSamples);
            decimal observediFreq = (totalISamples / (decimal)totalSamples);
            this.TestContext.WriteLine("Observed freq = {0}", observedFreq.ToString());
            this.TestContext.WriteLine("Observed iFreq = {0}", observediFreq.ToString());
            

            decimal dFreq = Math.Abs((1 / (decimal)freqency) - observedFreq);
            

            this.TestContext.WriteLine("delta freq  = {0}", dFreq.ToString());
            


            bool freqInTolarance = Math.Abs((1/(decimal)freqency) - observedFreq) <= tolarance;  
            
            Assert.IsTrue(freqInTolarance);


            if (iFrequency > 0)
            {
                decimal dIFreq = Math.Abs((1 / (decimal)iFrequency) - observediFreq);
                this.TestContext.WriteLine("delta iFreq = {0}", dIFreq.ToString());
                bool ifreqInTolarance = Math.Abs((1 / (decimal)iFrequency) - observediFreq) <= tolarance;
                Assert.IsTrue(ifreqInTolarance);
            }

        }
    }
}
