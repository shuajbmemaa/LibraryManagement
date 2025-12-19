namespace LMS.Application.DTO.AI
{
    public class QueryPlan
    {
        public string Entity { get; set; }
        public List<QueryFilter>? Filter { get; set; }
        public string? OrderBy { get; set; }
        public string? OrderDirection { get; set; }
        public List<string>? Select { get; set; }
        public int? Limit { get; set; }
        public ChatMessagePlan Chat { get; set; } = new();
    }
}
