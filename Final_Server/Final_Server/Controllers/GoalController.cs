﻿using Final_Server.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Final_Server.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class GoalController : ControllerBase
    {
        // GET: api/<GoalController>
        [HttpGet]
        public IEnumerable<Goal> Get() //get all goals
        {
            return Goal.ReadGoals();
        }


        // POST api/<GoalController>
        [HttpPost("/newGoal")]
        public IActionResult Post(int goalActive, [FromBody] string goalName) //insert new goal
        {
            int numEffected = Goal.InsertGoal(goalName, goalActive);
            if (numEffected != 0)
            {
                return Ok("goal succesfully inserted");
            }
            else
            {
                return NotFound("Error in insert this goal");
            }
        }


        [HttpPut("/UpdateGoalName")]
        public IActionResult PutGoalName( int goalNum, [FromBody] string goalName) //Update Goal Name
        {
            int numEffected = Goal.UpdateGoalName(goalNum, goalName);
            if (numEffected != 0)
            {
                return Ok("Goal succesfully updated");
            }
            else
            {
                return NotFound("We couldnt update the Goal");
            }
        }


        [HttpPut("/goalNum/goalActive")]
        public IActionResult PutGoalActive(int goalNum, int goalActive) //Update Goal Active
        {
            int numEffected = Goal.UpdateGoalActive(goalNum, goalActive);
            if (numEffected != 0)
            {
                return Ok("Goal succesfully updated");
            }
            else
            {
                return NotFound("We couldnt update the Goal");
            }
        }


    }
}
