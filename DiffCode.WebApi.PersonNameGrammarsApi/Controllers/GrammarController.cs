using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DiffCode.PersonNameGrammars.Enums;
using DiffCode.PersonNameGrammars.Models;
using DiffCode.WebApi.PersonNameGrammarsApi.Data;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;







namespace DiffCode.WebApi.PersonNameGrammarsApi.Controllers
{
  /// <summary>
  /// Методы для работы с грамматиками.
  /// </summary>
  [Route("[controller]")]
  public class GrammarController : Controller
  {
    private readonly GrammarsContext _ctx;





    public GrammarController(GrammarsContext context) => _ctx = context;








    /// <summary>
    /// Возвращает список всех грамматик.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    /// Пример запроса, возвращающего все грамматики:
    /// 
    /// GET /Grammar
    /// </para>
    /// 
    /// <para>
    /// Пример запроса, возвращающего грамматики для личных имен:
    /// 
    /// GET /Grammar?part=first
    /// </para>
    /// 
    /// <para>
    /// Пример запроса, возвращающего грамматики для фамилий, заканчивающихся на "кий":
    /// 
    /// GET /Grammar?part=last&amp;ending=кий
    /// </para>
    /// </remarks>
    /// 
    /// <returns>Список всех имеющихся в базе данных грамматик.</returns>
    /// <response code="200">Возвращает список всех имеющихся в базе данных грамматик.</response>
    /// <response code="404">В случае, если списка грамматик не существует.</response>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<Grammar>>> Get([FromQuery]string part = null, [FromQuery]string ending = null)
    {
      var partParsedOk = Enum.TryParse<NamePart>(part, true, out NamePart partResult);

      var result = _ctx.Grammars
        .AsEnumerable()
        .Where(c => (partParsedOk ? c.For.ToString().ToLower().Equals(part.ToLower()) : true) && (!string.IsNullOrWhiteSpace(ending) ? c.NameEnding.ToLower() == ending.ToLower() : true))
        .ToList();

      if (result == null)
      {
        return NotFound();
      };

      return await Task.FromResult(result);
    }




    /// <summary>
    /// Возвращает грамматику по её идентификатору.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    /// Пример запроса:
    /// 
    /// GET /Grammar/5
    /// </para>
    /// </remarks>
    /// 
    /// <param name="id">Идентификатор грамматики.</param>
    /// <returns>Грамматика с указанным идентификатором.</returns>
    /// <response code="200">Возвращает грамматику с указанным идентификатором.</response>
    /// <response code="404">В случае, если грамматика с указанным идентификатором не найдена.</response>
    [HttpGet("{id}")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Grammar>> Get(int id)
    {
      var result = await _ctx.Grammars.FindAsync(id);
      if (result == null)
      {
        return NotFound();
      };
      return result;
    }















  }
}
