using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DMS.Models;
using DMS.Utils;

namespace DMS.Controllers
{

    [ApiController]
    [Route("api")]
    public class DMSController : ControllerBase
    {

        [HttpGet("helloworld")]
        public async Task<IActionResult> helloworld()
        {
            return Ok("Hello World");
        }

        [HttpPost("login")]
        public async Task<IActionResult> login([FromBody] UserCreds creds)
        {
            var authentication = await AuthHelper.AuthFromCreds(creds.Username, creds.Password);

            if (authentication.Message == "Login successful")
            {
                return Ok(authentication);
            }
            else
            {
                return Unauthorized(authentication);
            }
        }
    }
}