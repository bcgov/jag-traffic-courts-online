﻿using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TrafficCourts.Citizen.Service.Validators;

namespace TrafficCourts.Citizen.Service.Models.Dispute
{
    public class DisputedCount
    {
        /// <summary>
        /// Represents the dispuant plea on count.
        /// </summary>
        [JsonPropertyName("plea")]
        public Plea Plea { get; set; }

        /// <summary>
        /// The count number. Must be unique within an individual dispute.
        /// </summary>
        [JsonPropertyName("count")]
        [Range(1, 3)]
        public int Count { get; set; }

        /// <summary>
        /// The disputant is requesting time to pay the ticketed amount.
        /// </summary>
        [JsonPropertyName("request_time_to_pay")]
        [RequiredIf("Plea", Plea.Guilty)]
        public bool? RequestTimeToPay { get; set; }

        /// <summary>
        /// The disputant is requesting a reduction of the ticketed amount.
        /// </summary>
        [JsonPropertyName("request_reduction")]
        [RequiredIf("Plea", Plea.Guilty)]
        public bool? RequestReduction { get; set; }

        /// <summary>
        /// Does the want to appear in court?
        /// </summary>
        [JsonPropertyName("appear_in_court")]
        [RequiredIf("Plea", Plea.Guilty)]
        public bool? AppearInCourt { get; set; } = true;
    }

}
