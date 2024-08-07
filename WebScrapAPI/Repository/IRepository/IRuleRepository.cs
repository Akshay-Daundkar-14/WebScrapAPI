namespace WebScrapAPI.Repository.IRepository
{
    public interface IRuleRepository
    {
        public int AddRule(WebScrapAPI.Models.Rule rule);

        public List<WebScrapAPI.Models.Rule> GetAllRules();
    }
}
