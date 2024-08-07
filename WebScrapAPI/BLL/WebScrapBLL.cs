using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using WebScrapAPI.Models;
using WebScrapAPI.BLL.IBLL;

namespace WebScrapAPI.BLL
{
    public class WebScrapBLL : IWebScrapBLL
    {
        public List<Rule> GetWebScrap()
        {
            List<Rule> rules = new List<Rule>();
            using (var driver = new ChromeDriver())
            {
                try
                {
                    driver.Url = "https://www.consumerfinance.gov/rules-policy/final-rules/";

                    var titles = driver.FindElements(By.XPath("//*[@id=\"content__main\"]/div[4]/div/section/article"));

                    if (titles == null) return rules;

                    foreach (var title in titles)
                    {
                        var ruleType = title.FindElement(By.ClassName("m-meta-header__item")).Text;
                        var date = title.FindElement(By.ClassName("datetime")).Text;
                        var _title = title.FindElement(By.ClassName("o-post-preview__title")).Text;
                        var description = title.FindElement(By.ClassName("o-post-preview__description")).Text;

                        var bullets = title.FindElements(By.ClassName("m-tags__tag"));


                        Rule rule = new Rule()
                        {
                            RuleType = ruleType,
                            Date = Convert.ToDateTime(date),
                            Title = _title,
                            Description = description,
                        };

                        List<string> _bulletPoints = new List<string>();

                        foreach (var bullet in bullets)
                        {
                            string cleanedInput = bullet.Text.Replace("•\r\n", "").Trim();
                            _bulletPoints.Add(cleanedInput);
                        }

                        rule.BulletPoints = _bulletPoints;

                        rules.Add(rule);
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("EXCEPTION", ex);
                }
                finally
                {
                    driver.Quit();
                }
                return rules;
            }
        }
    }
}
