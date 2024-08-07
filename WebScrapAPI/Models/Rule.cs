namespace WebScrapAPI.Models
{
    public class Rule
    {
        public int RuleID { get; set; }
        public string RuleType{ get; set; }

        public DateTime? Date{ get; set; }

        public string Title { get; set; }
        public string Description { get; set; }

        public List<string> BulletPoints{ get; set; }
    }
}
