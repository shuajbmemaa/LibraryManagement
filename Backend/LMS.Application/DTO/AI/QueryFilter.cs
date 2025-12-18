namespace LMS.Application.DTO.AI
{
    public class QueryFilter
    {
        public string Field { get; set; }
        public string Operator { get; set; } = "eq"; // eq, contains, startswith, endswith, gt, lt, gte, lte, in
        public object Value { get; set; }
    }
}
