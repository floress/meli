using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace XMen.Tools
{
    /// <summary>
    /// 
    /// </summary>
    public class MutantDetector : IMutantDetector
    {
        private const int PatternLen = 4;
        private const int PatternQty = 2;
        readonly Regex _regLetters;

        public MutantDetector()
        {
            // [^A^T^C^G] 
            _regLetters = new Regex("[^A^T^C^G]", RegexOptions.Compiled); // Compiled is faster
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dna"></param>
        public void Validate(string[] dna)
        {
            if (dna == null)
                throw new ArgumentNullException(nameof(dna), "Cannot be null.");

            var n = dna.Length;

            if (n < PatternLen)
                throw new ArgumentOutOfRangeException(nameof(dna), $"Matrix {n}x{n} is invalid.");

            if (dna.Any(s => s.Length != n))
                throw new ArgumentOutOfRangeException(nameof(dna), $"Matrix {n}x{n} is invalid.");

            if (dna.Any(s => _regLetters.IsMatch(s)))
                throw new ArgumentOutOfRangeException(nameof(dna), "Letters error.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dna"></param>
        /// <returns></returns>
        public bool IsMutant(string[] dna)
        {
            Validate(dna);

            var sequences = new int[3];

            Task.WaitAll(
                Task.Factory.StartNew(() => { sequences[0] = Horizontal(dna); }), 
                Task.Factory.StartNew(() => { sequences[1] = Vertical(dna); }), 
                Task.Factory.StartNew(() => { sequences[2] = Obliques(dna); }));

            return sequences[0] + sequences[1] + sequences[2] >= PatternQty;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="dna"></param>
        /// <returns></returns>
        private int Horizontal(string[] dna)
        {
            var n = dna.Length;
            var sequencesFound = 0;

            Parallel.For(0, n, rowIndex =>
            {
                var colIndex = 0;
                var consecutive = 1;
                while (colIndex < n - 1)
                {
                    var previous = dna[rowIndex][colIndex];
                    var next = dna[rowIndex][colIndex + 1];

                    if (previous == next)
                    {
                        consecutive++;
                        if (consecutive == PatternLen)
                        {
                            Interlocked.Increment(ref sequencesFound);
                            consecutive = 1;
                        }
                    }
                    else
                    {
                        consecutive = 1;
                    }

                    colIndex++;
                }
            });

            return sequencesFound;
        }

        /// <summary>
        /// Reads oblique rows of dna matrix 
        /// See ObliqueRowsMethod.pdf in docs folder
        /// </summary>
        /// <param name="dna"></param>
        /// <returns></returns>
        private int Obliques(string[] dna)
        {
            var sequencesFound = 0;

            // Step 1 Top-Left Corner to Bottom-Right Corner
            var n = dna.Length;
            var firstRow = n - PatternLen;

            var row = firstRow;
            while (row >= 0)
            {
                var colIndex = 0;
                var rowIndex = row;
                var consecutive = 1;
                while (colIndex < n - 1 && rowIndex < n - 1)
                {
                    var previous = dna[rowIndex][colIndex];
                    var next = dna[rowIndex + 1][colIndex + 1];

                    if (previous == next)
                    {
                        consecutive++;
                        if (consecutive == PatternLen) // Found sequence
                        {
                            sequencesFound++;
                            consecutive = 1;
                        }
                    }
                    else
                    {
                        consecutive = 1;
                    }

                    colIndex++;
                    rowIndex++;
                }

                row--;
            }

            // Step 2

            var firstLeftColumn = n - PatternLen;
            var col = firstLeftColumn;
            while (col >= 1 && sequencesFound < PatternQty)
            {
                var rowIndex = 0;
                var colIndex = col;
                var consecutive = 1;

                while (colIndex < n - 1 && rowIndex < n - 1)
                {
                    var previous = dna[rowIndex][colIndex];
                    var next = dna[rowIndex + 1][colIndex + 1];

                    if (previous == next)
                    {
                        consecutive++;
                        if (consecutive == PatternLen) // Found sequence
                        {
                            sequencesFound++;
                            consecutive = 1;
                        }
                    }
                    else
                    {
                        consecutive = 1;
                    }

                    colIndex++;
                    rowIndex++;
                }

                col--;
            }

            // Step 3 Top-Right Corner to Bottom-Left Corner 
            row = firstRow;
            while (row >= 0 && sequencesFound < PatternQty)
            {
                var colIndex = n - 1;
                var rowIndex = row;

                var consecutive = 1;
                while (colIndex > 0 && rowIndex < n - 1)
                {
                    var previous = dna[rowIndex][colIndex];
                    var next = dna[rowIndex + 1][colIndex - 1];

                    if (previous == next)
                    {
                        consecutive++;
                        if (consecutive == PatternLen) // Found sequence
                        {
                            sequencesFound++;
                            consecutive = 1;
                        }
                    }
                    else
                    {
                        consecutive = 1;
                    }

                    colIndex--;
                    rowIndex++;
                }

                row--;
            }

            // Step 4
            var firstRightColum = n - (n - PatternLen);
            col = firstRightColum;
            while (col < n && sequencesFound < PatternQty)
            {
                var rowIndex = 0;
                var colIndex = col - 1;
                var consecutive = 1;
                while (colIndex > 0 && rowIndex < n - 1)
                {
                    var previous = dna[rowIndex][colIndex];
                    var next = dna[rowIndex + 1][colIndex - 1];

                    if (previous == next)
                    {
                        consecutive++;
                        if (consecutive == PatternLen) // Found sequence
                        {
                            sequencesFound++;
                            consecutive = 1;
                        }
                    }
                    else
                    {
                        consecutive = 1;
                    }

                    colIndex--;
                    rowIndex++;
                }

                col++;
            }

            return sequencesFound;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dna"></param>
        /// <returns></returns>
        private int Vertical(string[] dna)
        {
            var n = dna.Length;
            var sequencesFound = 0;

            Parallel.For(0, n, colIndex =>
            {
                var rowIndex = 0;
                var consecutive = 1;

                while (rowIndex < n - 1)
                {
                    var previous = dna[rowIndex][colIndex];
                    var next = dna[rowIndex + 1][colIndex];

                    if (previous == next)
                    {
                        consecutive++;
                        if (consecutive == PatternLen) // Found sequence
                        {
                            Interlocked.Increment(ref sequencesFound);
                            consecutive = 1;
                        }
                    }
                    else
                    {
                        consecutive = 1;
                    }

                    rowIndex++;
                }
            });

            return sequencesFound;
        }
    }
}
