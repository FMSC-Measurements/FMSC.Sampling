﻿using FMSC.Sampling;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sampling.Test
{
    [TestClass()]
    public class ThreePSelecterTest
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

        [TestMethod()]
        public void TestThreePSelecter()
        {
            decimal tolarance = .01m;

            int iFrequency = 0;
            int aveKpi = 20;
            int popSize = 1000000;
            int numSamples = 2000;
            int numIsamples = (iFrequency > 0) ? numSamples / iFrequency : 0;

            int popVol = aveKpi * popSize;
            int kz = popVol / numSamples;
            int maxKPI = kz * 2;

            var selecter = new ThreePSelecter(kz, maxKPI, iFrequency);

            int sampleCounter = 0;
            int iSampleCounter = 0;

            for (int i = 0; i < popSize; i++)
            {
                var result = selecter.NextItem() as ThreePItem;
                if (result != null && aveKpi > result.KPI)
                {
                    if (selecter.IsSelectingITrees && selecter.InsuranceCounter.Next())
                    {
                        iSampleCounter++;
                    }
                    else
                    {
                        sampleCounter++;
                    }
                }
            }

            var sampleDiff = sampleCounter - numSamples;
            decimal sampleErr = (decimal)Math.Abs(sampleDiff) / (decimal)numSamples;

            this.TestContext.WriteLine($"sampleDiff = {sampleDiff}");
            this.TestContext.WriteLine($"sampleErr = {sampleErr}");

            if (iFrequency > 0)
            {
                var iSampleDiff = iSampleCounter - numIsamples;
                decimal iSampleErr = (decimal)Math.Abs(iSampleDiff) / (decimal)numIsamples;

                this.TestContext.WriteLine($"iSampleDiff = {iSampleDiff}");
                this.TestContext.WriteLine($"iSampleErr = {iSampleErr}");

                Assert.IsTrue(iSampleErr <= tolarance);
            }

            Assert.IsTrue(sampleErr <= tolarance);
        }
    }
}