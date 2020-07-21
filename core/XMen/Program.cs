using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using XMen.Tools;

namespace XMen
{
    class Program
    {
        static void Main(string[] args)
        {
            var mut = new MutantDetector();

            switch (args.Length)
            {
                case 0: // Test of the million
                    var dnas = DnaHelper.GenerateMillionRandomDna(8, 'A', 'T', 'C', 'G');

                    Console.WriteLine($"{dnas.Count} random DNAs generated");
                    Console.WriteLine("Press enter to analyze");
                    Console.ReadLine();

                    var sw = new Stopwatch();
                    sw.Start();

                    var mutants = new ConcurrentBag<string[]>();
                    var humans = new ConcurrentBag<string[]>();
                    dnas.AsParallel().ForAll(d =>
                    {
                        if (mut.IsMutant(d))
                        {
                            mutants.Add(d);
                        }
                        else
                        {
                            humans.Add(d);
                        }
                    });

                    sw.Stop();

                    Console.WriteLine($"{dnas.Count} proccesed in {sw.ElapsedMilliseconds} ms. humans: {humans.Count} mutants: {mutants.Count}\n");

                    using (var writerHumans = new StreamWriter("humans.txt", false))
                    {
                        using var writerMutants = new StreamWriter("mutants.txt", false);

                        foreach (var mutant in mutants)
                        {
                            foreach (var rowMutant in mutant)
                            {
                                var formattedRowMutant = string.Join(" ", rowMutant.ToCharArray());
                                writerMutants.WriteLine(formattedRowMutant);
                            }
                            writerMutants.WriteLine();
                        }

                        foreach (var human in humans)
                        {
                            foreach (var humanRow in human)
                            {
                                var formattedHumanRow = string.Join(" ", humanRow.ToCharArray());
                                writerHumans.WriteLine(formattedHumanRow);
                            }
                            writerHumans.WriteLine();
                        }
                    }

                    break;
                default: //dna array
                    try
                    {
                        var result = mut.IsMutant(args);
                        Console.WriteLine(result ? "It's mutant" : "It's human");
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine($"\nException: {ex.Message}");
                        Console.WriteLine("\nUsages:");
                        Console.WriteLine("\n\tWithout parameters executes one million test: XMen.exe ");
                        Console.WriteLine("\tWith parameters as an dna array: XMen.exe ATGCGA CAGTGC TTATGT AGAAGG CCCCTA TCACTG");
                    }
                    break;
            }
        }
    }
}
