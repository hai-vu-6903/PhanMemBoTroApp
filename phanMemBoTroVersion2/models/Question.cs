using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace phanMemBoTroVersion2.models
{
    public class Question
    {
        [JsonPropertyName("question")]
        public string question { get; set; }

        [JsonPropertyName("options")]
        public List<string> options { get; set; }

        [JsonPropertyName("answer")]
        public int answer { get; set; }

        [JsonIgnore]
        public int SelectedAnswer { get; set; } = -1;

        public bool IsCorrect => SelectedAnswer == answer;

        public bool IsShuffled { get; set; } = false;

    }
}
