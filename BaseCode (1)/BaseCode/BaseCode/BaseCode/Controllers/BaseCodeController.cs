using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Text;
using BaseCode.Models.Requests;
using BaseCode.Models.Responses;
using BaseCode.Models;
using BaseCode.Models.Tables;

namespace BaseCode.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BaseCodeController : Controller
    {
        private DBContext db;
        private readonly IWebHostEnvironment hostingEnvironment;
        private IHttpContextAccessor _IPAccess;

        private static readonly string[] Summaries = new[]
       {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public BaseCodeController(DBContext context, IWebHostEnvironment environment, IHttpContextAccessor accessor)
        {
            _IPAccess = accessor;
            db = context;
            hostingEnvironment = environment;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpPost("CreateUser")]
        public IActionResult CreateUser([FromBody] CreateUserRequest r)
        {
            CreateUserResponse resp = new CreateUserResponse();

            if (string.IsNullOrEmpty(r.FirstName))
            {
                resp.Message = "Please specify Firstname.";
                return BadRequest(resp);
            }
            if (string.IsNullOrEmpty(r.LastName))
            {
                resp.Message = "Please specify lastname.";
                return BadRequest(resp);
            }
            if (string.IsNullOrEmpty(r.UserName))
            {
                resp.Message = "Please specify username.";
                return BadRequest(resp);
            }
            if (string.IsNullOrEmpty(r.Password))
            {
                resp.Message = "Please specify password.";
                return BadRequest(resp);
            }
            if (string.IsNullOrEmpty(r.PhoneNumber))
            {
                resp.Message = "Please specify phone number.";
                return BadRequest(resp);
            }
            if (string.IsNullOrEmpty(r.dep_id))
            {
                resp.Message = "Please specify department id.";
                return BadRequest(resp);
            }

            resp = db.CreateUserUsingSqlScript(r);

            if (resp.isSuccess)
                return Ok(resp);
            else
                return BadRequest(resp);
        }

        [HttpPost("LoginUser")]

        public IActionResult LoginUser([FromBody] GetUserListRequest r)
        {
            GetUserListResponse resp = new GetUserListResponse();

           
            if (string.IsNullOrEmpty(r.Password) && string.IsNullOrEmpty(r.UserName))
            {
                resp.Message = "Please specify Username and Password.";
                return BadRequest(resp);
            }
            else if (string.IsNullOrEmpty(r.Password))
            {
                resp.Message = "Please specify Password.";
                return BadRequest(resp);
            }
            else if (string.IsNullOrEmpty(r.UserName))
            {
                resp.Message = "Please specify Username.";
                return BadRequest(resp);
            } 

            resp = db.LoginUser(r);

            if (resp.isSuccess)
                return Ok(resp);
            else
                return BadRequest(resp);
        }

        [HttpPost("UpdateUser")]
        public IActionResult UpdateUser([FromBody] CreateUserRequest r)
        {
            CreateUserResponse resp = new CreateUserResponse();

            if (string.IsNullOrEmpty(r.UserId.ToString()))
            {
                resp.Message = "Please specify UserId.";
                return BadRequest(resp);
            }

            resp = db.UpdateUser(r);

            if (resp.isSuccess)
                return Ok(resp);
            else
                return BadRequest(resp);
        }
        [HttpPost("GetUserList")]
        public IActionResult GetUserList([FromBody] GetUserListRequest r)
        {
            GetUserListResponse resp = new GetUserListResponse();
       
            resp = db.GetUserList(r);

            if (resp.isSuccess)
                return Ok(resp);
            else
                return BadRequest(resp);
        }

        //DEPARTMENT //

        [HttpPost("CreateDep")]
        public IActionResult CreateDep([FromBody] CreateDepRequest r)
        {
            CreateDepResponse resp = new CreateDepResponse();

            if (string.IsNullOrEmpty(r.DepName))
            {
                resp.Message = "Please specify name.";
                return BadRequest(resp);
            }
            if (string.IsNullOrEmpty(r.DepDescription))
            {
                resp.Message = "Please specify description.";
                return BadRequest(resp);
            }

            resp = db.CreateDepUsingSqlScript(r);

            if (resp.isSuccess)
                return Ok(resp);
            else
                return BadRequest(resp);
        }

        [HttpPost("UpdateDep")]
        public IActionResult UpdateDep([FromBody] CreateDepRequest r)
        {
            CreateDepResponse resp = new CreateDepResponse();

            if (string.IsNullOrEmpty(r.DepId.ToString()))
            {
                resp.Message = "Please specify DepId.";
                return BadRequest(resp);
            }

            resp = db.UpdateDep(r);

            if (resp.isSuccess)
                return Ok(resp);
            else
                return BadRequest(resp);
        }
        [HttpPost("DeleteDep")]
        public IActionResult DeleteDep([FromBody] CreateDepRequest r)
        {
            CreateDepResponse resp = new CreateDepResponse();

            if (string.IsNullOrEmpty(r.DepId.ToString()))
            {
                resp.Message = "Please specify Department Id.";
                return BadRequest(resp);
            }

            resp = db.UpdateDep(r);

            if (resp.isSuccess)
                return Ok(resp);
            else
                return BadRequest(resp);
        }
        [HttpPost("GetDepList")]    
        public IActionResult GetDepList([FromBody] GetDepListRequest r)
        {
            GetDepListResponse resp = new GetDepListResponse();

            resp = db.GetDepList(r);

            if (resp.isSuccess)
                return Ok(resp);
            else
                return BadRequest(resp);
        }
    }
}
