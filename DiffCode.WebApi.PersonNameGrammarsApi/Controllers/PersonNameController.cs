using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DiffCode.PersonNameGrammars.Enums;
using DiffCode.PersonNameGrammars.Models;
using DiffCode.WebApi.PersonNameGrammarsApi.Data;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;







namespace DiffCode.WebApi.PersonNameGrammarsApi.Controllers
{
  /// <summary>
  /// Методы для работы с объектами <see cref="PersonName"/>.
  /// </summary>
  [Route("[controller]")]
  public class PersonNameController : Controller
  {
    private readonly GrammarsContext _ctx;





    public PersonNameController(GrammarsContext grammarsContext)
    {
      _ctx = grammarsContext;
    }












    /// <summary>
    /// Возвращает список всех имеющихся объектов <see cref="PersonName"/>.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    /// Пример запроса, возвращающего все имена:
    /// 
    /// GET /PersonName
    /// </para>
    /// 
    /// <para>
    /// Пример запроса, возвращающего личные имена:
    /// 
    /// GET /PersonName?part=first
    /// </para>
    /// 
    /// <para>
    /// Пример запроса, возвращающего личные мужские имена:
    /// 
    /// GET /PersonName?part=first&amp;gender=m
    /// </para>
    /// 
    /// <para>
    /// Пример запроса, возвращающего женские варианты отчеств:
    /// 
    /// GET /PersonName?part=mid&amp;gender=f
    /// </para>
    /// </remarks>
    /// 
    /// <param name="part">Часть имени.</param>
    /// <param name="gender">Гендерная принадлежность имени.</param>
    /// <returns>Список всех имеющихся в базе данных объектов <see cref="PersonName"/>.</returns>
    /// <response code="200">Возвращает список всех имеющихся в базе данных объектов <see cref="PersonName"/>.</response>
    /// <response code="404">В случае, если списка объектов <see cref="PersonName"/> не существует.</response>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<PersonName>>> Get([FromQuery] string part = null, [FromQuery] string gender = null)
    {
      var partParsedOk = Enum.TryParse<NamePart>(part, true, out NamePart partResult);
      var genderParsedOk = Enum.TryParse<Gender>(gender, true, out Gender genderResult);

      var result = _ctx.PersonNames
        .AsEnumerable()
        .Where(c => (partParsedOk ? c.Part.ToString().ToLower().Equals(part.ToLower()) : true) && (genderParsedOk ? c.Gender.ToString().ToLower().Equals(gender.ToLower()) : true))
        .ToList();

      if(result == null)
      {
        return NotFound();
      };

      return await Task.FromResult(result);
    }




    /// <summary>
    /// Возвращает объект <see cref="PersonName"/> по его идентификатору.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    /// Пример запроса:
    /// 
    /// GET /PersonName/5
    /// </para>
    /// </remarks>
    /// 
    /// <param name="id">Идентификатор объекта <see cref="PersonName"/>.</param>
    /// <returns>Объект <see cref="PersonName"/> с указанным идентификатором.</returns>
    /// <response code="200">Возвращает объект <see cref="PersonName"/> с указанным идентификатором.</response>
    /// <response code="404">В случае, если объект <see cref="PersonName"/> с указанным идентификатором не найден.</response>
    [HttpGet("{id}")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PersonName>> Get(int id)
    {
      var result = await _ctx.PersonNames.FindAsync(id);
      if(result == null)
      {
        return NotFound();
      };
      return result;
    }




    /// <summary>
    /// Возвращает объект <see cref="PersonName"/> по его имени.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    /// Пример запроса:
    /// 
    /// GET /PersonName/ByName/Иван
    /// </para>
    /// </remarks>
    /// 
    /// <param name="name">Имя.</param>
    /// <returns>Объект <see cref="PersonName"/> с указанным именем.</returns>
    /// <response code="200">Возвращает объект <see cref="PersonName"/> с указанным именем.</response>
    /// <response code="404">В случае, если объект <see cref="PersonName"/> с указанным именем не найден.</response>
    [HttpGet("ByName/{name}")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PersonName>> GetByName(string name)
    {
      var result = await _ctx.PersonNames.FirstOrDefaultAsync(f => f.Name == name);
      if (result == null)
      {
        return NotFound();
      };
      return result;
    }




    /// <summary>
    /// Возвращает производные объекты <see cref="PersonName"/> по идентификатору их родительского объекта <see cref="PersonName"/>.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    /// Пример запроса:
    /// 
    /// GET /PersonName/ByBaseId/5
    /// </para>
    /// </remarks>
    /// 
    /// <param name="baseId">Идентификатор родительского объекта <see cref="PersonName"/>.</param>
    /// <returns>Объекты <see cref="PersonName"/>.</returns>
    /// <response code="200">Возвращает объекты <see cref="PersonName"/> по идентификатору их родительского объекта.</response>
    [HttpGet("ByBaseId/{baseId}")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PersonName>>> GetByBaseId(int baseId)
    {
      var result = await _ctx.PersonNames.Where(w => w.BaseId == baseId).ToListAsync();
      return result;
    }




    /// <summary>
    /// Возвращает производные объекты <see cref="PersonName"/> по имени их родительского объекта <see cref="PersonName"/>.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    /// Пример запроса:
    /// 
    /// GET /PersonName/ByBaseName/Иван
    /// </para>
    /// </remarks>
    /// 
    /// <param name="baseName">Имя родительского объекта <see cref="PersonName"/>.</param>
    /// <returns>Объекты <see cref="PersonName"/>.</returns>
    /// <response code="200">Возвращает объекты <see cref="PersonName"/> по идентификатору их родительского объекта.</response>
    [HttpGet("ByBaseName/{baseName}")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PersonName>>> GetByBaseName(string baseName)
    {
      var result = await _ctx.PersonNames.Where(w => w.BaseName != null && w.BaseName.Name == baseName).ToListAsync();
      return result;
    }




    /// <summary>
    /// Создает новый объект <see cref="PersonName"/>, имеющий указанное имя <paramref name="name"/> и, опционально, производные от него мужское и женское отчество.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    /// В случае, если указаны все три параметра, то имя обрабатывается как мужское
    /// (свойство <see cref="PersonName.Gender"/> автоматически устанавливается равным <see cref="Gender.M"/>,
    /// в список производных имен <see cref="PersonName.Derived"/> добавляются автоматически сгенерированные
    /// объекты <see cref="PersonName"/> для мужского и женского варианта отчества, производного от указанного имени).
    /// </para>
    /// 
    /// <para>
    /// В случае, если хотя бы один из параметров <paramref name="malemidname"/>/<paramref name="femalemidname"/> 
    /// не указан, либо оба они не указаны, то имя обрабатывается как женское 
    /// (свойство <see cref="PersonName.Gender"/> автоматически устанавливается равным <see cref="Gender.F"/>).
    /// </para>
    /// 
    /// <para>
    /// Пример запроса для создания мужского имени 
    /// (должны быть указаны все производные от имени отчества - мужское и женское).
    /// 
    /// POST /PersonName
    /// {
    ///   "name": "Иван",
    ///   "malemidname": "Иванович",
    ///   "femalemidname": "Ивановна"
    /// }
    /// </para>
    /// 
    /// <para>
    /// Пример запроса для создания женского имени
    /// (отсутствие параметров malemidname и femalemidname указывает на то, что это женское имя).
    /// 
    /// POST /PersonName
    /// {
    ///   "name": "Наталья"
    /// }
    /// </para>
    /// </remarks>
    /// 
    /// <param name="name">Имя.</param>
    /// <param name="malemidname">Мужское отчество, производное от указанного имени (если имя мужское), либо null, если имя женское.</param>
    /// <param name="femalemidname">Женское отчество, производное от указанного имени (если имя мужское), либо null, если имя женское.</param>
    /// <returns>Успешно созданный объект PersonName.</returns>
    /// <response code="201">Возвращает успешно созданный объект <see cref="PersonName"/>.</response>
    /// <response code="400">В случае, если объект <see cref="PersonName"/> не удалось создать (параметр запроса name содержал пустую строку или null).</response>
    /// <response code="409">В случае, если в базе данных уже содержится объект <see cref="PersonName"/> с указанным в параметре name именем.</response>
    [HttpPost]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PersonName>> Post(string name, string malemidname = null, string femalemidname = null)
    {
      if (!string.IsNullOrWhiteSpace(name) && _ctx.PersonNameExists(name))
      {
        return Conflict();
      }
      else
      {
        if (!string.IsNullOrWhiteSpace(name))
        {
          var gender = string.IsNullOrWhiteSpace(malemidname) || string.IsNullOrWhiteSpace(femalemidname) ? Gender.F : Gender.M;

          var item = new PersonName
          {
            Name = name,
            Gender = gender,
            Part = NamePart.FIRST
          };

          if (gender == Gender.M && !string.IsNullOrWhiteSpace(malemidname) && !string.IsNullOrWhiteSpace(femalemidname))
          {
            item.Derived.Add(new PersonName { Name = malemidname, Gender = Gender.M, Part = NamePart.MID, BaseName = item });
            item.Derived.Add(new PersonName { Name = femalemidname, Gender = Gender.F, Part = NamePart.MID, BaseName = item });
          };

          _ctx.PersonNames.Add(item);
          await _ctx.SaveChangesAsync();

          return CreatedAtAction("Get", new { Id = item.Id }, item);
        }
        else
        {
          return BadRequest();
        };
      };
    }




    /// <summary>
    /// PUT /PersonName/5
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PersonName>> Put(int id, string name)
    {
      var entity = _ctx.PersonNames.FirstOrDefault(f => f.Id == id);

      if(entity != null)
      {
        entity.Name = name;
        await _ctx.SaveChangesAsync();
        return Ok();
      }
      else
      {
        return NotFound();
      };
    }




    /// <summary>
    /// Удаляет объект <see cref="PersonName"/> по его идентификатору.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    /// Удаляемый объект должен представлять собой личное мужское/женское имя.
    /// В этом случае удаление производных от него мужского/женского отчества будет произведено автоматически.
    /// В случае, если найденный объект представляет собой отчество, его удаление не производится.
    /// </para>
    /// 
    /// <para>
    /// Пример запроса:
    /// 
    /// DELETE /PersonName/5
    /// </para>
    /// </remarks>
    /// 
    /// <param name="id">Идентификатор объекта <see cref="PersonName"/>.</param>
    /// <response code="200">В случае успешного удаления объекта <see cref="PersonName"/> с его возможными производными.</response>
    /// <response code="400">В случае, если найденный по идентификатору объект <see cref="PersonName"/> не является личным мужским/женским именем.</response>
    /// <response code="404">В случае, если в базе данных не найден объект <see cref="PersonName"/> с указанным идентификатором.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
      var entity = _ctx.PersonNames.FindAsync(id);

      if(entity == null)
      {
        return NotFound();
      }
      else
      {
        if (!entity.Result.Part.Equals(NamePart.FIRST))
        {
          return BadRequest();
        }
        else
        {
          _ctx.PersonNames.RemoveRange(entity.Result.Derived);
          _ctx.PersonNames.Remove(entity.Result);

          await _ctx.SaveChangesAsync();
          return Ok();
        };
      };
    }






    /// <summary>
    /// Возвращает количество объектов <see cref="PersonName"/>, соответствующих переданным параметрам запроса.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    /// Пример запроса количества всех объектов <see cref="PersonName"/>:
    /// 
    /// GET /PersonName/Count
    /// </para>
    /// 
    /// <para>
    /// Пример запроса количества объектов <see cref="PersonName"/>, представляющих собой личное имя:
    /// 
    /// GET /PersonName/Count?part=first
    /// </para>
    /// 
    /// <para>
    /// Пример запроса количества объектов <see cref="PersonName"/>, представляющих собой женский вариант отчества:
    /// 
    /// GET /PersonName/Count?part=mid&amp;gender=f
    /// </para>
    /// </remarks>
    /// 
    /// <param name="part">Часть имени.</param>
    /// <param name="gender">Гендерная принадлежность имени.</param>
    /// <returns></returns>
    /// <response code="200">Возвращается по умолчанию. В случае ошибки в переданных параметрах будет возвращен весь список имен.</response>
    [HttpGet("Count")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<int>> GetCount([FromQuery]string part = null, [FromQuery]string gender = null)
    {
      var partParsedOk = Enum.TryParse<NamePart>(part, true, out NamePart partResult);
      var genderParsedOk = Enum.TryParse<Gender>(gender, true, out Gender genderResult);

      var result = _ctx.PersonNames
        .AsEnumerable()
        .Count(c => (partParsedOk ? c.Part.ToString().ToLower().Equals(part.ToLower()) : true) && (genderParsedOk ? c.Gender.ToString().ToLower().Equals(gender.ToLower()) : true));

      return await Task.FromResult(result);
    }














  }
}
