using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using WebScrapAPI.Models;
using WebScrapAPI.BLL;
using WebScrapAPI.Repository.IRepository;
using WebScrapAPI.BLL.IBLL;

namespace WebScrapAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebScrapsController : ControllerBase
    {

        private readonly IRuleRepository _ruleRepository;
        private readonly IWebScrapBLL _webScrapBLL;

        public WebScrapsController(IRuleRepository ruleRepository, IWebScrapBLL webScrapBLL)
        {
            _ruleRepository = ruleRepository;
            _webScrapBLL = webScrapBLL;
        }


        [HttpGet]
        async public Task<IActionResult> GetWebScraps()
        {

            // Getting records from database
            var _dbRules = _ruleRepository.GetAllRules();

            if (_dbRules == null || _dbRules.Count == 0)
            {
                // Getting records from web scrapping 
                List<Rule> rules = _webScrapBLL.GetWebScrap();
                if (rules == null)
                {
                    return StatusCode(500, "Something went wrong");
                }

                // Adding data in database
                foreach (var rule in rules)
                {
                    _ruleRepository.AddRule(rule);
                }
               // return StatusCode(500, "Something went wrong");
            }

            return Ok(_dbRules);

        }


    }
}
