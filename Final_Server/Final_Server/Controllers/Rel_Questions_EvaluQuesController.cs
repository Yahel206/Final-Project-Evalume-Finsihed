﻿using Final_Server.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Dynamic;
using System.Text.Json;
using System.Text.Json.Serialization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Final_Server.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class Rel_Questions_EvaluQuesController : ControllerBase
    {

        [HttpGet("/quesType/roleGroup_Type")]
        public IEnumerable<Object> GetEvaluQuesByType(bool quesType, int roleGroup_Type) //gets all the EvaluQues that fit the QuesType and RoleType
        {
            try
            {
                return Rel_Questions_EvaluQues.ReadEvaluQuesByType(quesType, roleGroup_Type);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet("/questionnaireNum")]
        public Object GetQuesByEvaluId(int questionnaireNum) //gets the questions that part of this corrent evaluQues
        {
            try
            {
                return Rel_Questions_EvaluQues.ReadQuesByEvaluId(questionnaireNum);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        //// POST api/<Rel_Questions_EvaluQuesController>
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        [HttpGet("/userNum")]
        public Object GetEvaluQuesByUserId(int userNum) //get the appropriate EvaluQues for the current employee
        {
            try
            {
                return Rel_Questions_EvaluQues.ReadEvaluQuesByUserId(userNum);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // GET api/<Rel_Questions_EvaluQuesController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<Rel_Questions_EvaluQuesController>
        [HttpPost]
        public IActionResult Post([FromBody] JsonElement data) // post a new evaluation form to the database
        {
            try
            {
                JsonDocument document = JsonDocument.Parse(data.ToString());
                JsonElement checkedArry = document.RootElement.GetProperty("myCheckedArray");
                List<int> questionList = new List<int>();
                foreach (JsonElement item in checkedArry.EnumerateArray())
                {
                    int[] numbers = item.GetProperty("questionNum").EnumerateArray().Select(x => x.GetInt32()).ToArray();
                    foreach (int number in numbers)
                    {
                        questionList.Add(number);
                    }
                }

                dynamic newForm = new ExpandoObject();
                newForm.roleType = Convert.ToInt32(data.GetProperty("myFormTypes").GetProperty("roleType").GetInt32());
                newForm.groupType = Convert.ToInt32(data.GetProperty("myFormTypes").GetProperty("groupType").GetInt32());
                newForm.questions = questionList.ToArray();

                int numEffected = Rel_Questions_EvaluQues.insertNewForm(newForm);

                if (numEffected != 0)
                {
                    return Ok("Evaluation succesfully inserted");
                }
                else
                {
                    return NotFound("Error in insert this Evaluation");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }



        [HttpPost("/EvaluationAnswers")]
        public IActionResult PostEmployeeAnswers([FromBody] JsonElement data) // post a new evaluation form by employee\manager the database
        {
            try
            {
                JsonDocument document = JsonDocument.Parse(data.ToString());
                JsonElement answers = document.RootElement.GetProperty("answers");


                int userNum = Convert.ToInt32(data.GetProperty("userNum").GetInt32());
                int evalu_Part_Type = Convert.ToInt32(data.GetProperty("evalu_Part_Type").GetInt32());
                int questionnaireNum = Convert.ToInt32(data.GetProperty("questionnaireNum").GetInt32());

                List<(int questionNum, int numericAnswer, string verbalAnswer)> answersList = new List<(int, int, string)>();

                //JsonElement answersElement = JsonElement.ParseValue(data.GetProperty("answers"));

                 foreach (JsonElement answersElement in answers.EnumerateArray())
                 {
                    int questionNum = Convert.ToInt32(answersElement.GetProperty("questionNum").GetInt32());
                    int numericAnswer = Convert.ToInt32(answersElement.GetProperty("numericAnswer").GetString());
                    string verbalAnswer = (answersElement.GetProperty("verbalAnswer").GetString()).ToString();
                    
                    answersList.Add((questionNum, numericAnswer, verbalAnswer));

                 };

                int numEffected = Rel_Questions_EvaluQues.insertNewAnswers(userNum, evalu_Part_Type, questionnaireNum, answersList);

                if (numEffected != 0)
                {
                    return Ok("Answers succesfully inserted");
                }
                else
                {
                    return NotFound("Error in insert this Answers");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        //[HttpPost("/EvaluationManagerAnswers")]
        //public IActionResult PostManagerAnswers([FromBody] JsonElement data) // post a evaluation form by manager to the database
        //{
        //    try
        //    {
        //        JsonDocument document = JsonDocument.Parse(data.ToString());
        //        JsonElement answers = document.RootElement.GetProperty("answers");


        //        int userNum = Convert.ToInt32(data.GetProperty("userNum").GetInt32());
        //        int evalu_Part_Type = Convert.ToInt32(data.GetProperty("evalu_Part_Type").GetInt32());
        //        int questionnaireNum = Convert.ToInt32(data.GetProperty("questionnaireNum").GetInt32());

        //        List<(int questionNum, int numericAnswer, string verbalAnswer)> answersList = new List<(int, int, string)>();

        //        //JsonElement answersElement = JsonElement.ParseValue(data.GetProperty("answers"));

        //        foreach (JsonElement answersElement in answers.EnumerateArray())
        //        {
        //            int questionNum = Convert.ToInt32(answersElement.GetProperty("questionNum").GetInt32());
        //            int numericAnswer = Convert.ToInt32(answersElement.GetProperty("numericAnswer").GetString());
        //            string verbalAnswer = (answersElement.GetProperty("verbalAnswer").GetString()).ToString();

        //            answersList.Add((questionNum, numericAnswer, verbalAnswer));

        //        };

        //        int numEffected = Rel_Questions_EvaluQues.insertNewManagerAnswers(userNum, evalu_Part_Type, questionnaireNum, answersList);

        //        if (numEffected != 0)
        //        {
        //            return Ok("Answers succesfully inserted");
        //        }
        //        else
        //        {
        //            return NotFound("Error in insert this Answers");
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        [HttpPost("/EvaluationSummeryAnswers")]
        public IActionResult PostSummaryAnswers([FromBody] JsonElement data) // post a evaluation summery ques by manager and employee
        {
            try
            {
                JsonDocument document = JsonDocument.Parse(data.ToString());
                JsonElement allGoals = document.RootElement.GetProperty("allGoals");


                int userNum = Convert.ToInt32(data.GetProperty("userNum").GetInt32());
                int evalu_Part_Type = Convert.ToInt32(data.GetProperty("evalu_Part_Type").GetInt32());
                int questionnaireNum = Convert.ToInt32(data.GetProperty("questionnaireNum").GetInt32());
                string managerOpinion = (data.GetProperty("managerOpinion")).ToString();
                string employeeOpinion = (data.GetProperty("employeeOpinion")).ToString();

                List <int> goalsList = new List<int>();

                //JsonElement answersElement = JsonElement.ParseValue(data.GetProperty("answers"));

                foreach (JsonElement answersElement in allGoals.EnumerateArray())
                {
                    int goalNum = Convert.ToInt32(answersElement.GetProperty("goalNum").GetInt32());


                    goalsList.Add(goalNum);
                };

                int numEffected = Rel_Questions_EvaluQues.insertNewSummeryAnswers(userNum, evalu_Part_Type, questionnaireNum, managerOpinion, employeeOpinion, goalsList);

                if (numEffected != 0)
                {
                    return Ok("Answers succesfully inserted");
                }
                else
                {
                    return NotFound("Error in insert this Answers");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        //[HttpPost]
        //public IActionResult PostSummaryAnswers([FromBody] Rel_EmployeeGoal rel_EmployeeGoal) // post a evaluation summery ques by manager and employee
        //{
        //    try
        //    {
        //        int numEffected = rel_EmployeeGoal.InsertEmployeeGoal();
        //        if (numEffected != 0)
        //        {
        //            return Ok("goal succesfully inserted");
        //        }
        //        else
        //        {
        //            return NotFound("Error in insert this goal");
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
        //// PUT api/<Rel_Questions_EvaluQuesController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/<Rel_Questions_EvaluQuesController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
