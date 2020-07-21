using System.Collections.Concurrent;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XMen.Tools;

namespace XMen.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMutantOriginal()
        {
            var detector = new MutantDetector();
            var dna = new [] { "ATGCGA", "CAGTGC", "TTATGT", "AGAAGG", "CCCCTA", "TCACTG" };
            Assert.IsTrue(detector.IsMutant(dna));
        }

        /// <summary>
        /// Horizontal and vertical
        /// </summary>
        [TestMethod]
        public void TestMutant1()
        {
            var detector = new MutantDetector();
            var dna = new [] { 
                "A T G C G A",
                "C A G T G C",
                "T T G T G T",
                "A G A A G G",
                "C C C C T A",
                "T C A C T G" };

            Assert.IsTrue(detector.IsMutant(Normalize(dna)));
        }

        /// <summary>
        /// Horizontal and oblique
        /// </summary>
        [TestMethod]
        public void TestMutant2()
        {
            var detector = new MutantDetector();
            var dna = new [] { 
                "A T G C G A",
                "C G G T G C",
                "T T A T G T",
                "A G A A G G",
                "C G C C A A",
                "T C A C T A" };

            Assert.IsTrue(detector.IsMutant(Normalize(dna)));
        }

        /// <summary>
        /// Vertical G
        /// </summary>
        [TestMethod]
        public void TestHuman1()
        {
            var detector = new MutantDetector();
            var dna = new [] { 
                "A T G C G A",
                "C A G T G C",
                "T T G T G T",
                "A G A A G G",
                "C C T C T A",
                "T C A C T G" };

            Assert.IsFalse(detector.IsMutant(Normalize(dna)));
        }

        /// <summary>
        /// Random one hundred matrix
        /// </summary>
        [TestMethod]
        public void OneHundredTest()
        {
            var detector = new MutantDetector();
            var dnas = DnaHelper.GenerateHundredRandomDna(8, 'A', 'T', 'C', 'G');

            var mutants = new ConcurrentBag<string[]>();
            var humans = new ConcurrentBag<string[]>();

            dnas.AsParallel().ForAll(d =>
            {
                if (detector.IsMutant(d))
                {
                    mutants.Add(d);
                }
                else
                {
                    humans.Add(d);
                }
            });

            Assert.IsTrue(mutants.Count + humans.Count == 100);
        }

        private string[] Normalize(string[] dna) => dna.Select(s => s.Replace(" ", string.Empty)).ToArray();
    }
}
