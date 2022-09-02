using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Infrastructure.DataBase
{
    public partial class Event
    {
        public Guid Id { get; set; }
        [Column(TypeName = "jsonb")]
        public JsonDocument  Data { get; set; } = null!;
        [Column(TypeName = "jsonb")]
        public JsonDocument?  Meta { get; set; } = null!;
        public DateTime Created { get; set; }
        [JsonPropertyName("event_id")]
        public int EventId { get; set; }
    }
}
