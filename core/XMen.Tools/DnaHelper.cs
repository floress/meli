using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace XMen.Tools
{
    public static class DnaHelper
    {
        /// <summary>
        /// Generate random square dna matrix of matrixSize and letters
        /// </summary>
        /// <param name="maxSizeOfMatrix">Size of matrix</param>
        /// <param name="letters">Letters</param>
        /// <returns></returns>
        public static string[] GenerateRandomDna(int maxSizeOfMatrix, params char[] letters)
        {
            var rnd = new Random(Guid.NewGuid().GetHashCode());
            var matrix = new string[maxSizeOfMatrix];

            for (var row = 0; row < maxSizeOfMatrix; row++)
            {
                for (var col = 0; col < maxSizeOfMatrix; col++)
                {
                    matrix[row] += letters[rnd.Next(0, letters.Length - 1)];
                }
            }

            return matrix;
        }

        /// <summary>
        /// Generate random square mutant dna matrix of matrixSize and letters
        /// </summary>
        /// <param name="maxSizeOfMatrix">Size of matrix</param>
        /// <param name="sequenceLength"></param>
        /// <param name="letters">Letters</param>
        /// <returns></returns>
        public static string[] GenerateRandomMutants(int maxSizeOfMatrix, int sequenceLength, params char[] letters)
        {
            if (sequenceLength > maxSizeOfMatrix)
                throw new ArgumentOutOfRangeException(nameof(sequenceLength));

            var rnd = new Random(Guid.NewGuid().GetHashCode());
            var matrix = new string[maxSizeOfMatrix];

            var letter1Index = rnd.Next(0, letters.Length - 1);
            var letter1 = letters[letter1Index];
            var letter2Index = rnd.Next(0, letters.Length - 1);
            var letter2 = letters[letter2Index];
            var colRandom = rnd.Next(0, maxSizeOfMatrix - 1);
            var rowRandom = rnd.Next(0, maxSizeOfMatrix - 1);
            var sequence1 = 0;
            var sequence2 = 0;

            for (var row = 0; row < maxSizeOfMatrix; row++)
            {
                for (var col = 0; col < maxSizeOfMatrix; col++)
                {
                    if (col == colRandom && sequence1 < sequenceLength)
                    {
                        matrix[row] += letters[letter1];
                        sequence1++;
                    }
                    if (row == rowRandom && sequence2 < sequenceLength)
                    {
                        matrix[row] += letters[letter2];
                        sequence2++;
                    }
                    matrix[row] += letters[rnd.Next(0, letters.Length - 1)];
                }
            }

            return matrix;
        }

        /// <summary>
        /// Generate million of random dna matrix with specific size
        /// </summary>
        /// <param name="maxSizeOfMatrix">Size of matrix</param>
        /// <param name="letters">Allowed letters in sequence</param>
        /// <returns></returns>
        public static ConcurrentBag<string[]> GenerateMillionRandomDna(int maxSizeOfMatrix, params char[] letters)
        {
            var dnas = new ConcurrentBag<string[]>();
            var rnd = new Random(Guid.NewGuid().GetHashCode());

            Parallel.For(0, 1000000, i =>
            {
                dnas.Add(GenerateRandomDna(rnd.Next(4, maxSizeOfMatrix), letters));
            });

            return dnas;
        }

        /// <summary>
        /// Generate million of random dna matrix with specific size
        /// </summary>
        /// <param name="maxSizeOfMatrix">Size of matrix</param>
        /// <param name="letters">Allowed letters in sequence</param>
        /// <returns></returns>
        public static ConcurrentBag<string[]> GenerateHundredRandomDna(int maxSizeOfMatrix, params char[] letters)
        {
            var dnas = new ConcurrentBag<string[]>();
            var rnd = new Random(Guid.NewGuid().GetHashCode());

            Parallel.For(0, 100, i =>
            {
                dnas.Add(GenerateRandomDna(rnd.Next(4, maxSizeOfMatrix), letters));
            });

            return dnas;
        }
    }
}