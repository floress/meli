using System.Text.Json.Serialization;

namespace XMen.WebApi.Models
{
    public class MutantInput
    {
        [JsonPropertyName("dna")]
        public string[] Dna { get; set; }
    }
}
