using System.Text.Json.Serialization;

namespace XMen.Data.Models
{
    public class Stats
    {
        [JsonPropertyName("count_mutant_dna")]
        public long Mutants { get; set; }

        [JsonPropertyName("count_human_dna")]
        public long Humans { get; set; }

        [JsonPropertyName("ratio")]
        public decimal? Ratio => Humans ==  0 && Humans == 0 ? 0 : (decimal?)Mutants / (Mutants + Humans);
    }
}
