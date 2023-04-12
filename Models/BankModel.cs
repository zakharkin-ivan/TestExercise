using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TestExercise.Models
{
    internal class BankModel
    {
        public BankModel(int id,
                    Guid uid,
                    long accountNumber,
                    string iban,
                    string bankName,
                    long routingNumber,
                    string swiftBic)
        {
            Id = id;
            Uid = uid;
            AccountNumber = accountNumber;
            Iban = iban ?? throw new ArgumentNullException(nameof(iban));
            BankName = bankName ?? throw new ArgumentNullException(nameof(bankName));
            RoutingNumber = routingNumber;
            SwiftBic = swiftBic ?? throw new ArgumentNullException(nameof(swiftBic));
        }
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("uid")]
        public Guid Uid { get; set; }
        [JsonPropertyName("account_number")]
        public long AccountNumber { get; set; }
        [JsonPropertyName("iban")]
        public string Iban { get; set; } = string.Empty;
        [JsonPropertyName("bank_name")]
        public string BankName { get; set; } = string.Empty;
        [JsonPropertyName("routing_number")]
        public long RoutingNumber { get; set; }
        [JsonPropertyName("swift_bic")]
        public string SwiftBic { get; set; } = string.Empty;
    }
}
