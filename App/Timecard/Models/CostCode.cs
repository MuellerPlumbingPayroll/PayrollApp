namespace Timecard.Models
{
    public class CostCode
    {
        public static readonly string PlumbingCodeGroup = "Plumbing";
        public static readonly string HeatingCodeGroup = "Heating";
        public static readonly string ServiceCodeGroup = "Service";
        public static readonly string ShopCodeGroup = "Shop";

        public string Code { get; set; }
        public string CodeGroup { get; set; }
        public string Description { get; set; }

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
