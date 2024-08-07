using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using WebScrapAPI.Repository.IRepository;

namespace WebScrapAPI.Repository
{
    public class RuleRepository : IRuleRepository
    {
        private readonly string _connectionString;

        public RuleRepository(IConfiguration iConfiguration)
        {
            _connectionString = iConfiguration.GetConnectionString("RuleDB");
        }

        public int AddRule(WebScrapAPI.Models.Rule rule)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                
                    try
                    {
                        // Insert Rule
                        SqlCommand cmd = new SqlCommand("AddRule", conn);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@RuleType", rule.RuleType);
                        cmd.Parameters.AddWithValue("@Date", rule.Date.HasValue ? (object)rule.Date.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@Title", rule.Title);
                        cmd.Parameters.AddWithValue("@Description", rule.Description);

                        //int ruleID = (int)cmd.ExecuteScalar();


                        //---------------------------------

                        // Define the output parameter
                        SqlParameter outputParam = new SqlParameter
                        {
                            ParameterName = "@NewRuleID",
                            SqlDbType = SqlDbType.Int,
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(outputParam);
                        cmd.ExecuteNonQuery();
                        // Retrieve the output parameter value
                        int newRuleID = (int)outputParam.Value;

                        //----------------------------------

                        // Insert Bullet Points
                        foreach (var bulletPoint in rule.BulletPoints)
                        {
                            SqlCommand bulletCmd = new SqlCommand("AddBulletPoint", conn);
                            bulletCmd.CommandType = CommandType.StoredProcedure;

                            bulletCmd.Parameters.AddWithValue("@RuleID", newRuleID);
                            bulletCmd.Parameters.AddWithValue("@BulletPoint", bulletPoint);

                            bulletCmd.ExecuteNonQuery();
                        }

                        return newRuleID;
                    }
                    catch
                    {
                        throw;
                    }
                
            }
        }


        public List<WebScrapAPI.Models.Rule> GetAllRules()
        {
            List<WebScrapAPI.Models.Rule> rules = new List<WebScrapAPI.Models.Rule>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("GetAllRules", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int ruleID = reader.GetInt32(reader.GetOrdinal("RuleID"));
                        WebScrapAPI.Models.Rule rule = rules.Find(r => r.RuleID == ruleID);

                        if (rule == null)
                        {
                            rule = new WebScrapAPI.Models.Rule
                            {
                                RuleID = ruleID,
                                RuleType = reader.GetString(reader.GetOrdinal("RuleType")),
                                Date = reader.IsDBNull(reader.GetOrdinal("Date")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("Date")),
                                Title = reader.GetString(reader.GetOrdinal("Title")),
                                Description = reader.GetString(reader.GetOrdinal("Description")),
                                BulletPoints = new List<string>()
                            };
                            rules.Add(rule);
                        }

                        if (!reader.IsDBNull(reader.GetOrdinal("BulletPoint")))
                        {
                            rule.BulletPoints.Add(reader.GetString(reader.GetOrdinal("BulletPoint")));
                        }
                    }
                }
            }

            return rules;
        }
    }
}
