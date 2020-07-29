using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

using DiffCode.WebApi.PersonNameGrammarsApi.Data;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;






namespace DiffCode.WebApi.PersonNameGrammarsApi.Controllers
{
  /// <summary>
  /// 
  /// </summary>
  [Route("[controller]")]
  public class UtilitiesController : Controller
  {
    private readonly GrammarsContext _ctx;




    public UtilitiesController(GrammarsContext grammarsContext)
    {
      _ctx = grammarsContext;
    }









    
    //[AcceptVerbs("GET")]
    




    //[AcceptVerbs("POST")]
    [Route("UploadFile")]
    [HttpPost]
    public async Task<IActionResult> OnPostUploadAsync(IFormFile file)
    {
      var filePath = Path.GetTempFileName();
      using (var stream = System.IO.File.Create(filePath))
      {
        await file.CopyToAsync(stream);
      };

      var filecontent = XDocument.Load(filePath);
      if(filecontent != null)
      {
        _ctx.ProcessXmlData(filecontent);
        System.IO.File.Delete(filePath);

        return Ok(filecontent);
      }
      else
      {
        return BadRequest();
      };
    }








  }
}
