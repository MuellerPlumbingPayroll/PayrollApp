﻿using System;
namespace Timecard.Models
{
    public class CostCode
    {
        public static readonly string PlumbingCodeGroup = "PLUMBING";
        public static readonly string ServiceCodeGroup = "SERVICE";

        public string Id { get; set; }
        public string Code { get; set; }
        public string CodeGroup { get; set; }
        public string Description { get; set; }

        public CostCode()
        {
        }
    }
}
