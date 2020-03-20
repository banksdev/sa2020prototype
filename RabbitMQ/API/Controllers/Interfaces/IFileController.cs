public interface IFileController : ControllerBase
{
    IActionResult Get(Guid id);
    IActionResult Post([FromBody] ContentWrapper filetext);
}