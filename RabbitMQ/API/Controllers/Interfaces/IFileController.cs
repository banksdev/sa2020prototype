using System;
using Microsoft.AspNetCore.Mvc;
using static API.Controllers.FileController;

public interface IFileController
{
    IActionResult Get(Guid id);
    IActionResult Post([FromBody] ContentWrapper filetext);
}