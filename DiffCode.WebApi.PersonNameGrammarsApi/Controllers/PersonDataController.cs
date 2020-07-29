using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;

using DiffCode.PersonNameGrammars.Models;
using DiffCode.WebApi.PersonNameGrammarsApi.Data;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;







namespace DiffCode.WebApi.PersonNameGrammarsApi.Controllers
{
  /// <summary>
  /// Методы для работы с личными данными.
  /// </summary>
  [Route("[controller]")]
  public class PersonDataController : Controller
  {
    private readonly GrammarsContext _ctx;





    public PersonDataController(GrammarsContext context) => _ctx = context;










    /// <summary>
    /// Возвращает личные данные (ФИО, пол) со списком склонений по падежам.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    /// В качестве параметра допустимы строки в любом регистре, содержащие все три части имени в именительном падеже,
    /// разделенные пробелом, в формате Фамилия Имя Отчество, либо Имя Отчество Фамилия.
    /// </para>
    /// 
    /// <para>
    /// Примеры запроса:
    /// 
    /// GET /PersonData/иван%20васильевич%20грозный
    /// GET /PersonData/Шульц%20НАТАЛЬЯ%20мурадовна
    /// </para>
    /// </remarks>
    /// 
    /// <param name="text"></param>
    /// <returns></returns>
    /// <response code="200">В случае успешного разбора входной строки и создания соответствующих личных данных.</response>
    /// <response code="400">В случае, если входная строка была пустой, либо равна null.</response>
    /// <response code="404">В случае неудачного разбора входной строки (не найдено личное имя/отчество/фамилия).</response>
    [HttpGet("{text}")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<PersonData> Get(string text)
    {
      if (string.IsNullOrWhiteSpace(text))
      {
        return BadRequest();
      }
      else
      {
        var result = _ctx.ParsePersonData(text);

        if (!result.IsCorrect())
        {
          return NotFound();
        }
        else
        {
          return Ok(result);
        };
      };
    }




    /// <summary>
    /// Возвращает коллекцию личных данных (ФИО, пол, список склонений по падежам).
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    /// В качестве параметра допустимы строки в любом регистре, содержащие все три части имени в именительном падеже,
    /// разделенные пробелом, в формате Фамилия Имя Отчество, либо Имя Отчество Фамилия.
    /// </para>
    /// 
    /// <para>
    /// Пример запроса:
    /// 
    /// POST /PersonData
    /// [
    ///   "иван васильевич грозный",
    ///   "шульц евгения ильинична",
    ///   "куусинен лев самуилович"
    /// ]
    /// </para>
    /// </remarks>
    /// 
    /// <param name="texts"></param>
    /// <returns></returns>
    /// <response code="200">В случае успешного разбора входной строки и создания соответствующих личных данных.</response>
    /// <response code="400">В случае, если входная строка была пустой, либо равна null.</response>
    /// <response code="404">В случае неудачного разбора входной строки (не найдено личное имя/отчество/фамилия).</response>
    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<IEnumerable<PersonData>> Post([FromBody]IEnumerable<string> texts)
    {
      if (texts == null || texts.Count() == 0)
      {
        return BadRequest();
      }
      else
      {
        var result = _ctx.ParsePersonDatas(texts).Where(w => w.IsCorrect()).ToList();

        if (result.Count == 0)
        {
          return NotFound();
        }
        else
        {
          return Ok(result);
        };
      };
    }








  }
}
