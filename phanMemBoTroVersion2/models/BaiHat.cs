using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace phanMemBoTroVersion2.models
{
    public class BaiHat
    {
        [JsonPropertyName("title")]
        public string title { get; set; }

        [JsonPropertyName("fileVoice")]
        public string fileVoice { get; set; }

        [JsonPropertyName("fileInst")]
        public string fileInst { get; set; }

        [JsonPropertyName("fileLrc")]
        public string fileLrc { get; set; }

        [JsonPropertyName("author")]
        public string author { get; set; }

        [JsonPropertyName("authorBirthYear")]
        public int? authorBirthYear { get; set; }

        [JsonPropertyName("authorDeathYear")]
        public int? authorDeathYear { get; set; }

        [JsonPropertyName("songYear")]
        public int? songYear { get; set; }

        [JsonPropertyName("authorImage")]
        public string authorImage { get; set; }

        [JsonPropertyName("description")]
        public List<string> description { get; set; }

        [JsonPropertyName("authorInfo")]
        public List<string> authorInfo { get; set; }
    }
}
