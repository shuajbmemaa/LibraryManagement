namespace LMS.Application.DTO.AI
{
    public class RawQueryPlan
    {
        public string? Entity { get; set; }
        public object? Filter { get; set; }
        public object? Select { get; set; }
        public object? Limit { get; set; }
        public string? OrderBy { get; set; }
        public string? OrderDirection { get; set; }
        public ChatMessagePlan? Chat { get; set; }
    }
}
