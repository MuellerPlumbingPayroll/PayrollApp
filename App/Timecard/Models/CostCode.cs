namespace Timecard.Models
{
    public class CostCode
    {
        public static readonly string PlumbingCodeGroup = "PLUMBING";
        public static readonly string ServiceCodeGroup = "SERVICE";

        public string Code { get; set; }
        public string CodeGroup { get; set; }
        public string Description { get; set; }

        public static CostCode DummyCostCode(string codeGroup)
        {
            return new CostCode
            {
                Code = "Not Listed",
                CodeGroup = codeGroup,
                Description = "Not Listed"
            };
        }

        public override bool Equals(object obj)
        {
            if (!(obj is CostCode other))
            {
                return false;
            }

            return Code == other.Code && 
                   CodeGroup == other.CodeGroup && 
                   Description == other.Description;
        }

        public override int GetHashCode()
        {
            int result = 17;

            result = 31 * result + Code.GetHashCode();
            result = 31 * result + CodeGroup.GetHashCode();
            result = 31 * result + Description.GetHashCode();

            return result;
        }
    }
}
