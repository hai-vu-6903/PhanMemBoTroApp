using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace phanMemBoTroVersion2.models
{
    public class DieuVu
    {
        [JsonPropertyName("title")]
        public string TenBai { get; set; }

        [JsonPropertyName("file")]
        public string AudioPath { get; set; }

        [JsonPropertyName("video")]
        public string VideoPath { get; set; }
    }
}
