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
            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> login([FromBody] UserCreds creds)
        {
            // Retrieve user credentials from the database
            var authentication = await AuthHelper.AuthFromCreds(creds.Username, creds.Password);

            // Respond with correct status code depending on the authentication result
            if (authentication.Message == "Login successful")
            {
                return Ok(authentication);
            }
            else
            {
                return Unauthorized(authentication);
            }
        }

        [HttpPost("status")]
        public async Task<IActionResult> status([FromBody] Status status)
        {
            // Authenticate with token
            var token = Request.Headers["Authorization"].ToString();
            var authentication = await AuthHelper.AuthFromToken(token);
            if (!authentication)
            {
                return Unauthorized();
            }


            // Get scale ID from token
            string scaleId = await DBHelper.Token2ID("PLACEHOLDER_TOKEN");

            // Get datetime
            DateTime time = DateTime.UtcNow;

            // Add status to the database
            bool DBStatus = await DBHelper.AddStatus(scaleId, status.Version, time);

            // Craft and send response
            var response = new DefaultResponse(); 
            if (DBStatus == true)
            {
                response.Message = "Status added";
                return Ok(response);
            }
            else
            {
                return StatusCode(500);
            }
        }

        [HttpPost("registerWeight")]
        public async Task<IActionResult> registerWeight([FromBody] WeightReq weightBody)
        {
            // Authenticate with token
           var token = Request.Headers["Authorization"].ToString();
           var authentication = await AuthHelper.AuthFromToken(token);
           if (!authentication)
            {
                return Unauthorized();
            }

            // Get datetime 
            DateTime time = DateTime.UtcNow;

            bool DBStatus = await DBHelper.AddWeight(weightBody.WorkerID, weightBody.Type, weightBody.Weight, weightBody.Period, time);

            // Craft and send response
            var response = new DefaultResponse(); 
            if (DBStatus == true)
            {
                response.Message = "Weight registered";
                return Ok(response);
            }
            else
            {
                return StatusCode(500);
            }

        }
    }
}