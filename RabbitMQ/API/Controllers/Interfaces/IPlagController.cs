using System;
using Microsoft.AspNetCore.Mvc;
using static API.Controllers.FileController;

public interface IPlagController
{
    IActionResult Post([FromBody] string filetext);
}