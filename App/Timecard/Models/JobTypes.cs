using System;
namespace Timecard.Models
{
    public static class JobTypes
    {
        public const string Construction = "Construction";
        public const string Service = "Service";
        public const string Other = "Other";
        public static readonly string[] Types =
        {
            JobTypes.Construction,
            JobTypes.Service,
            JobTypes.Other
        };
    }
}
