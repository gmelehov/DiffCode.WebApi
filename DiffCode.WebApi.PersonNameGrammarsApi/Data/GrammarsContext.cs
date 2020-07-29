using DiffCode.PersonNameGrammars.Enums;
using DiffCode.PersonNameGrammars.Models;
using DiffCode.WebApi.PersonNameGrammarsApi.Utils;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;







namespace DiffCode.WebApi.PersonNameGrammarsApi.Data
{
  /// <summary>
  /// 
  /// </summary>
  public class GrammarsContext : DbContext
  {
    protected string _connString;
    protected string _srvName = @"WS";
    protected string _dbName = @"PersonNameGrammars";




    public GrammarsContext() : base()
    {
      _connString = $"Data Source={_srvName};Initial Catalog={_dbName};Integrated Security=True;";
      Database.EnsureCreated();
    }
    public GrammarsContext(string serverName, string dbName) : base()
    {
      _srvName = serverName;
      _dbName = dbName;
      _connString = $"Data Source={_srvName};Initial Catalog={_dbName};Integrated Security=True;";
      Database.EnsureCreated();
    }
    public GrammarsContext(DbContextOptions<GrammarsContext> options, string serverName, string dbName) : base(options)
    {
      _srvName = serverName;
      _dbName = dbName;
      _connString = $"Data Source={_srvName};Initial Catalog={_dbName};Integrated Security=True;";
      Database.EnsureCreated();
    }








    public DbSet<PersonName> PersonNames { get; set; }
    public DbSet<GrammarCase> GrammarCases { get; set; }
    public DbSet<Grammar> Grammars { get; set; }










    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      if (!optionsBuilder.IsConfigured)
      {
        optionsBuilder.UseLazyLoadingProxies().UseSqlServer(_connString);
      };
      optionsBuilder.EnableSensitiveDataLogging();
    }
    protected override void OnModelCreating(ModelBuilder mb)
    {
      mb.Entity<Grammar>(ent =>
      {
        ent.Property(e => e.For).HasConversion(new EnumToStringConverter<NamePart>());
      });
      mb.Entity<PersonName>(ent =>
      {
        ent.Property(e => e.Gender).HasConversion(new EnumToStringConverter<Gender>());
        ent.Property(e => e.Part).HasConversion(new EnumToStringConverter<NamePart>());
        ent.HasOne(e => e.Grammar).WithMany(w => w.Names).HasForeignKey(f => f.GrammarId).OnDelete(DeleteBehavior.ClientSetNull);
        ent.HasOne(e => e.BaseName).WithMany(w => w.Derived).HasForeignKey(f => f.BaseId).OnDelete(DeleteBehavior.ClientSetNull);
      });
      mb.Entity<GrammarCase>(ent =>
      {
        ent.Property(e => e.Case).HasConversion(new EnumToStringConverter<Case>());
        ent.HasOne(e => e.Grammar).WithMany(w => w.GrammarCases).HasForeignKey(f => f.GrammarId).OnDelete(DeleteBehavior.Cascade);
      });
    }








    public PersonName FindPersonName(string name) => PersonNames.FirstOrDefault(f => f.Name == name);
    public bool PersonNameExists(string name) => PersonNames.Any(a => a.Name == name);





    /// <summary>
    /// Читает определения частей имен из файла по указанному пути,
    /// создает и возвращает коллекцию объектов <see cref="PersonName"/>
    /// </summary>
    /// <param name="xmlpath">Путь к xml-файлу, содержащему определения частей имен.</param>
    /// <returns></returns>
    public IList<PersonName> ReadPersonNamesFromXML(string xmlpath)
    {
      var xdoc = XDocument.Load(xmlpath);
      return xdoc.Root.Element(XName.Get("Names", "Grammars")).Elements().Select(s => new PersonName(s)).ToList();
    }
    public IList<PersonName> ReadPersonNamesFromXML(XDocument xdoc) => xdoc.Root.Element(XName.Get("Names", "Grammars")).Elements().Select(s => new PersonName(s)).ToList();

    /// <summary>
    /// Загружает коллекцию объектов <see cref="PersonName"/>, созданную 
    /// из определений частей имен, найденных в xml-файле по указанному пути,
    /// и сохраняет ее в базу данных.
    /// </summary>
    /// <param name="xmlpath">Путь к xml-файлу, содержащему определения частей имен.</param>
    public void LoadPersonNamesFromXML(string xmlpath)
    {
      var pnames = ReadPersonNamesFromXML(xmlpath);
      PersonNames.AddRange(pnames.Where(w => !PersonNames.Any(a => a.Name == w.Name)));
      SaveChanges();
    }
    public void LoadPersonNamesFromXML(XDocument xdoc)
    {
      var pnames = ReadPersonNamesFromXML(xdoc);
      PersonNames.AddRange(pnames.Where(w => !PersonNames.Any(a => a.Name == w.Name)));
      SaveChanges();
    }





    /// <summary>
    /// Читает определения грамматик из файла по указанному пути,
    /// создает и возвращает коллекцию объектов <see cref="Grammar"/>
    /// </summary>
    /// <param name="xmlpath">Путь к xml-файлу, содержащему определения грамматик.</param>
    /// <returns></returns>
    public IList<Grammar> ReadGrammarsFromXML(string xmlpath)
    {
      var xdoc = XDocument.Load(xmlpath);
      return xdoc.Root.Element(XName.Get("Grammars", "Grammars")).Elements(XName.Get("Grammar", "Grammars")).Select(s => new Grammar(s)).ToList();
    }
    public IList<Grammar> ReadGrammarsFromXML(XDocument xdoc) => xdoc.Root.Element(XName.Get("Grammars", "Grammars")).Elements(XName.Get("Grammar", "Grammars")).Select(s => new Grammar(s)).ToList();

    /// <summary>
    /// Загружает коллекцию объектов <see cref="Grammar"/>, созданную 
    /// из определений грамматик, найденных в xml-файле по указанному пути,
    /// и сохраняет ее в базу данных.
    /// </summary>
    /// <param name="xmlpath">Путь к xml-файлу, содержащему определения грамматик.</param>
    public void LoadGrammarsFromXML(string xmlpath)
    {
      var grammars = ReadGrammarsFromXML(xmlpath);
      Grammars.AddRange(grammars.Where(w => !Grammars.Any(a => a.For.Equals(w.For) && a.BaseEnding == w.BaseEnding)));
      SaveChanges();
    }
    public void LoadGrammarsFromXML(XDocument xdoc)
    {
      var grammars = ReadGrammarsFromXML(xdoc);
      Grammars.AddRange(grammars.Where(w => !Grammars.Any(a => a.For.Equals(w.For) && a.BaseEnding == w.BaseEnding)));
      SaveChanges();
    }





    public void CalculatePersonNamesGrammars()
    {
      Grammars.Include(i => i.Names).Include(i => i.GrammarCases).Load();
      Grammars.ToList().ForEach(gr =>
      {
        var nomcase = gr.GrammarCases.FirstOrDefault(f => f.Case == Case.NOM);
        var names = PersonNames.AsEnumerable().Where(w => Regex.IsMatch(w.Name, $".?{gr.Suffix}{(w.Gender == Gender.M ? nomcase.MForm : nomcase.FForm)}$") && gr.BaseEnding == (w.Gender == Gender.M ? nomcase.MForm : nomcase.FForm) && gr.For == w.Part);
        foreach (var n in names)
        {
          n.Grammar = gr;
          n.GrammarId = gr.Id;
        };
      });

      SaveChanges();
    }
    public PersonName CalculatePersonNameGrammar(PersonName pname)
    {
      if (pname != null)
      {
        Grammars.Include(i => i.GrammarCases).Load();
        var grammar = Grammars.AsEnumerable().FirstOrDefault(f => Regex.IsMatch(pname.Name, $".?{f.Suffix}{(pname.Gender == Gender.M ? f.MForm : f.FForm)}$") && f.BaseEnding == (pname.Gender == Gender.M ? f.MForm : f.FForm) && f.For == pname.Part);
        if (grammar != null)
        {
          pname.Grammar = grammar;
          pname.GrammarId = grammar.Id;
        };
      };

      return pname;
    }






    public void ProcessXmlData(string xmlpath)
    {
      LoadGrammarsFromXML(xmlpath);
      LoadPersonNamesFromXML(xmlpath);

      CalculatePersonNamesGrammars();
    }
    public void ProcessXmlData(XDocument xdoc)
    {
      LoadGrammarsFromXML(xdoc);
      LoadPersonNamesFromXML(xdoc);

      CalculatePersonNamesGrammars();
    }









    public PersonName CreatePersonName(string name, Gender gender, string malemidname = null, string femalemidname = null)
    {
      if (!string.IsNullOrWhiteSpace(name) && !PersonNameExists(name))
      {
        PersonName result = new PersonName
        {
          Name = name,
          Gender = gender,
          Part = NamePart.FIRST
        };
        if (gender == Gender.M && !string.IsNullOrWhiteSpace(malemidname) && !string.IsNullOrWhiteSpace(femalemidname))
        {
          result.Derived.Add(new PersonName { Name = malemidname, Gender = Gender.M, Part = NamePart.MID, BaseName = result });
          result.Derived.Add(new PersonName { Name = femalemidname, Gender = Gender.F, Part = NamePart.MID, BaseName = result });
        };

        return CalculatePersonNameGrammar(result);
      }
      else
      {
        return null;
      }
    }





    public PersonData ParsePersonData(string text)
    {
      Grammars.Include(i => i.Names).Include(i => i.GrammarCases).Load();
      return ParseSinglePersonData(text);
    }
    public IEnumerable<PersonData> ParsePersonDatas(IEnumerable<string> texts)
    {
      Grammars.Include(i => i.Names).Include(i => i.GrammarCases).Load();
      return texts.Select(s => ParseSinglePersonData(s)).ToList();
    }
    public IEnumerable<PersonData> ParsePersonDatas(params string[] texts) => ParsePersonDatas(texts.AsEnumerable());





    private PersonData ParseSinglePersonData(string text)
    {
      var result = new PersonData();
      string firstName = "";
      string midName = "";
      string lastName = "";
      Gender gender = Gender.N;

      var list = text.ToLower().Trim(' ').Split(' ').Select(s => s.CapitalizeFirstChar()).ToList();

      string foundName = PersonNames.AsEnumerable().Select(s => s.Name).Intersect(list).FirstOrDefault();
      PersonName foundPersonName = FindPersonName(foundName);


      if (foundPersonName != null)
      {
        switch (foundPersonName.Part)
        {
          case NamePart.FIRST:
            firstName = foundName;
            result.FirstName = foundName;
            result.FirstNameGrammar = foundPersonName.Grammar;
            break;
          case NamePart.MID:
            midName = foundName;
            result.MidName = foundName;
            result.MidNameGrammar = foundPersonName.Grammar;
            break;

          default:
            break;
        };

        gender = foundPersonName.Gender;
        result.Gender = foundPersonName.Gender;
        list.Remove(foundName);
      };


      foundName = PersonNames.AsEnumerable().Select(s => s.Name).Intersect(list).FirstOrDefault();
      foundPersonName = FindPersonName(foundName);


      if (foundPersonName != null)
      {
        switch (foundPersonName.Part)
        {
          case NamePart.FIRST:
            firstName = foundName;
            result.FirstName = foundName;
            result.FirstNameGrammar = foundPersonName.Grammar;
            break;
          case NamePart.MID:
            midName = foundName;
            result.MidName = foundName;
            result.MidNameGrammar = foundPersonName.Grammar;
            break;

          default:
            break;
        };

        list.Remove(foundName);
      };


      if (list.Count > 0)
      {
        for (var i = 0; i < list.Count; i++)
        {
          var found = Grammars.AsEnumerable().FirstOrDefault(f => f.For == NamePart.LAST && Regex.IsMatch(list[i], @$".?{(gender.Equals(Gender.M) ? f.MForm : f.FForm)}$"));
          if (found != null)
          {
            lastName = list[i];
            result.LastName = list[i];
            result.LastNameGrammar = found;
          }
        };
      };


      if (string.IsNullOrWhiteSpace(lastName))
      {
        lastName = String.Join(" ", list).Trim(' ');
        var found = Grammars.AsEnumerable().FirstOrDefault(f => f.For == NamePart.LAST && Regex.IsMatch(lastName, @$".?{(gender.Equals(Gender.M) ? f.MForm : f.FForm)}$"));
        if (found != null)
        {
          result.LastName = lastName;
          result.LastNameGrammar = found;
        }
      };


      return result;
    }






    



  }
}
