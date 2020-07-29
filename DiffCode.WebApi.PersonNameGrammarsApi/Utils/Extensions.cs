using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;





namespace DiffCode.WebApi.PersonNameGrammarsApi.Utils
{
  public static class Extensions
  {



    public static string CapitalizeFirstChar(this string str) => !string.IsNullOrWhiteSpace(str) ? $"{str[0].ToString().ToUpper()}{str.Substring(1)}" : "";



  }
}
