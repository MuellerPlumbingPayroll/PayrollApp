using System;
namespace Timecard.Models
{
    public static class JobType
    {
        public const string Construction = "Construction";
        public const string Service = "Service";
        public const string Other = "Other";
        public static readonly string[] Types =
        {
            Construction,
            Service,
            Other
        };
    }
}
