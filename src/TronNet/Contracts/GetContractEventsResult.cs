using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TronNet.Contracts
{
    public class GetContractEventsResult
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }
        [JsonPropertyName("data")]
        public List<GetContractEventsResultItem> Data { get; set; }

        [JsonPropertyName("meta")]
        public GetContractEventsResultMetaData Meta { get; set; }
    }

    public class GetContractEventsResultItem
    {
        [JsonPropertyName("block_number")]
        public long BlockNumber { get; set; }
        [JsonPropertyName("block_timestamp")]
        public long BlockTimestamp { get; set; }
        [JsonPropertyName("contract_address")]
        public string ContractAddress { get; set; }
        [JsonPropertyName("event_name")]
        public string EventName { get; set; }

        [JsonPropertyName("result")]
        public GetContractEventsResultTransaction Result { get; set; }
        [JsonPropertyName("transaction_id")]
        public string TransactionId { get; set; }
    }

    public class GetContractEventsResultTransaction
    {
        [JsonPropertyName("from")]
        public string From { get; set; }
        [JsonPropertyName("to")]
        public string To { get; set; }
        [JsonPropertyName("value")]
        public string Value { internal get; set; }

        public decimal Amount { get; internal set; }
    }

    public class GetContractEventsResultMetaData
    {
        [JsonPropertyName("fingerprint")]
        public string Fingerprint { get; set; }

        [JsonPropertyName("page_size")]
        public int PageSize { get; set; }
    }

}
