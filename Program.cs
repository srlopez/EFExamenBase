using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using static System.Console;

public static class RandomData
{
    static readonly Random rnd;


    // Static constructor is called at most one time, before any
    // instance constructor is invoked or member is accessed.
    static RandomData()
    {
        rnd = new Random();
    }
    static string[] pelo = { "rubio", "moreno", "castaño" };
    static int getRnd(int a, int b) => rnd.Next(a, b);
    public static int getEdad() => getRnd(15, 45);
    public static int getEfectivo() => getRnd(25, 300);
    public static int getCreditos() => getRnd(5, 30);
    public static int getCurso() => getRnd(1, 3);
    public static string getPelo() => pelo[getRnd(0, pelo.Length)];

}

public class InstitutoContext : DbContext
{
    public DbSet<Alumno> Alumnos { get; set; }
    public DbSet<Modulo> Modulos { get; set; }
    public DbSet<Matricula> Matriculas { get; set; }

    public string connString { get; private set; }

    public InstitutoContext()
    {
        connString = $"Server=185.60.40.210\\SQLEXPRESS,58015;Database=EF00Santi;User Id=sa;Password=Pa88word;MultipleActiveResultSets=true";
        //connString = $"Server=localhost;Database=EFPrueba;User Id=sa;Password=Pa88word;";
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlServer(connString);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Matricula>().HasIndex(m => new
        {
            m.AlumnoId,
            m.ModuloId
        }).IsUnique();
    }
}
public class Alumno
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int AlumnoId { get; set; }
    public string Nombre { get; set; }

    public int Edad { get; set; } = RandomData.getEdad();
    public int Efectivo { get; set; } = RandomData.getEfectivo();
    public string Pelo { get; set; } = RandomData.getPelo();

    public List<Matricula> Matriculas { get; } = new List<Matricula>();

    public override string ToString() => $"#{AlumnoId} {Nombre} {Edad}Y {Efectivo}€ ({Matriculas.Count} Matrículas, {Pelo})";
}
public class Modulo
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int ModuloId { get; set; }
    public string Titulo { get; set; }

    public int Créditos { get; set; } = RandomData.getCreditos();
    public int Curso { get; set; } = RandomData.getCurso();


    public List<Matricula> Matriculaciones { get; } = new List<Matricula>();


    public override string ToString() => $"#{ModuloId} {Curso}/{Titulo} ({Matriculaciones.Count} Alumnos, {Créditos} Créditos)";
}
public class Matricula
{
    public int MatriculaId { get; set; }
    public int AlumnoId { get; set; }
    public int ModuloId { get; set; }


    public Alumno Alumno { get; set; }
    public Modulo Modulo { get; set; }

    public override string ToString() => $"{AlumnoId}x{ModuloId}";
}
class Program
{
    static  void CrearBD(){
        using (var db = new InstitutoContext())
        {
            bool deleted = db.Database.EnsureDeleted();
            WriteLine($"Database deleted: {deleted}");
            bool created = db.Database.EnsureCreated();
            WriteLine($"Database created: {created}");
        }
    }
    static void GenerarDatos()
    {
        using (var db = new InstitutoContext())
        {

            db.Alumnos.RemoveRange(db.Alumnos);
            db.Modulos.RemoveRange(db.Modulos);
            db.SaveChanges();

            var a1 = new Alumno { AlumnoId = 1, Nombre = "Pedro" }; db.Alumnos.Add(a1);
            var a2 = new Alumno { AlumnoId = 2, Nombre = "Luis" }; db.Alumnos.Add(a2);
            var a3 = new Alumno { AlumnoId = 3, Nombre = "Antonio" }; db.Alumnos.Add(a3);
            var a4 = new Alumno { AlumnoId = 4, Nombre = "Marcos" }; db.Alumnos.Add(a4);
            var a5 = new Alumno { AlumnoId = 5, Nombre = "Aroa" }; db.Alumnos.Add(a5);
            var a6 = new Alumno { AlumnoId = 6, Nombre = "Anne" }; db.Alumnos.Add(a6);
            var a7 = new Alumno { AlumnoId = 7, Nombre = "Yosune" }; db.Alumnos.Add(a7);
            db.SaveChanges();

            var m1 = new Modulo { ModuloId = 100, Titulo = "MOD100" }; db.Modulos.Add(m1);
            var m2 = new Modulo { ModuloId = 200, Titulo = "MOD200" }; db.Modulos.Add(m2);
            var m3 = new Modulo { ModuloId = 300, Titulo = "MOD300" }; db.Modulos.Add(m3);
            var m4 = new Modulo { ModuloId = 400, Titulo = "MOD400" }; db.Modulos.Add(m4);
            var m5 = new Modulo { ModuloId = 500, Titulo = "MOD500" }; db.Modulos.Add(m5);
            var m6 = new Modulo { ModuloId = 600, Titulo = "MOD600" }; db.Modulos.Add(m6);
            var m7 = new Modulo { ModuloId = 700, Titulo = "MOD700" }; db.Modulos.Add(m7);
            db.SaveChanges();

        }
    }
    static void BorrarDatos()
    {
        using (var db = new InstitutoContext())
        {
            // No recuerdo como
            foreach (var a in db.Alumnos)
            {
                //.ToList() 'There is already an open DataReader associated with this Connection which must be closed first.'
                // MultipleActiveResultSets=true
                var ms = 0;
                foreach (var m in db.Modulos)
                {
                    if (ms < 4)
                    {
                        if (a.Edad % 2 == m.Créditos % 2)
                        {
                            db.Matriculas.Add(new Matricula { Alumno = a, Modulo = m });
                            ms++;
                        }
                    }
                    else
                    {
                        if (a.Efectivo > 175) db.Matriculas.Add(new Matricula { Alumno = a, Modulo = m });
                    }
                }
            }

            // asi borro 1
            foreach (var m in db.Matriculas)
                if (m.ModuloId % 4 == 0) db.Matriculas.Remove(m);
            // asi borro muchas
            db.Matriculas.RemoveRange(db.Matriculas
                .Where(m => m.MatriculaId % 8 == 0));

            db.SaveChanges();
        }
    }
    static void ShowData()
    {
        using (var db = new InstitutoContext())
        {
            foreach (var a in db.Alumnos)
                WriteLine(a);
            WriteLine();

            foreach (var a in db.Alumnos.Include("Matriculas"))
            {
                Console.WriteLine(a);
                var mids = a.Matriculas.Select(m => m.MatriculaId);
                foreach (var mo in db.Matriculas.Where(ma => mids.Contains(ma.MatriculaId)).Select(ma => ma.Modulo))
                {
                    Console.WriteLine($"   {mo}");
                }
            }
            WriteLine();
        }
    }
    static void RealizarQuery2()
    {
        using (var db = new InstitutoContext())
        {

            // Suma de los créditos de los modulos de los alumnos con Edad>10
            // SELECT SUM(M.Créditos)
            // FROM ALUMNOS AS A, MATRICULAS AS T, MODULOS AS M
            // WHERE A.AlumnoId = T.AlumnoId 
            //     AND T.ModuloId = M.ModuloId
            //     AND A.Edad > 10
            // GO
            var sc = db.Alumnos.Where(a => a.Edad > 10)
                    .SelectMany(a => a.Matriculas)
                    .Select(mt => mt.Modulo.Créditos)
                    .Sum();
            Console.WriteLine(sc);
            Console.WriteLine();

            // Suma de los créditos de los modulos de Aroa
            // SELECT SUM(M.Créditos)
            //     FROM ALUMNOS AS A, MATRICULAS AS T, MODULOS AS M
            //     WHERE A.AlumnoId = T.AlumnoId 
            //         AND T.ModuloId = M.ModuloId
            //         AND A.Nombre = 'Aroa'
            // GO                
            var aroa = db.Alumnos
                .Where(a => a.Nombre == "Aroa")
                .First();

            sc = db.Alumnos
                    .Where(a => a.Nombre == "Aroa")
                    .SelectMany(a => a.Matriculas)
                    .Select(mt => mt.Modulo.Créditos)
                    .Sum();
            Console.WriteLine($"{aroa} {sc}");
            Console.WriteLine();

            // suma de creditos por color del pelo y si es rico 
            var sumaCreditos0 = db.Alumnos
                    .Where(al => true) //Todos (lo dejo por si es filtro)
                    .SelectMany(al => al.Matriculas, (a, m) => new
                    {
                        mat = m.MatriculaId,
                        rico = a.Efectivo >= 150,
                        pelo = a.Pelo,
                        creditos = m.Modulo.Créditos
                    })
                    .ToList();

            var sumaCreditos1 = sumaCreditos0
                    .GroupBy(a => a.pelo)
                    .Select(g => new
                    {
                        pelo = g.Key,
                        total = g.Sum(a => a.creditos)
                    })
                    .OrderBy(x => x.total)
                    .ToList();
            foreach (var x in sumaCreditos1) Console.WriteLine(x);
            Console.WriteLine();

            var sumaCreditosF = db.Alumnos
                    .Where(al => true) //Todos (lo dejo por si es filtro)
                    .SelectMany(al => al.Matriculas, (a, m) => new
                    {
                        rico = a.Efectivo >= 150,
                        pelo = a.Pelo,
                        creditos = m.Modulo.Créditos
                    })
                    .ToList()
                    .GroupBy(a => new { a.rico, a.pelo })
                    .Select(g => new
                    {
                        rico = g.Key.rico,
                        pelo = g.Key.pelo,
                        total = g.Sum(a => a.creditos)
                    })
                    .OrderByDescending(x => x.rico)
                    .ThenBy(x => x.pelo)
                    .ToList();
            foreach (var x in sumaCreditosF) Console.WriteLine(x);
            // /*
            // SELECT T.MatriculaId,
            //     Pelo, 
            //        CASE WHEN Efectivo>=150  THEN 1 ELSE 0 END  AS RICO,
            //        M.Créditos
            //        FROM ALUMNOS AS A, MATRICULAS AS T, MODULOS AS M
            //        WHERE A.AlumnoId = T.AlumnoId AND T.ModuloId = M.ModuloId
            // GO
            // */
            // SELECT  
            //        CASE WHEN Efectivo>=150  THEN 1 ELSE 0 END AS RICO,
            //        PELO,
            //        SUM(M.Créditos)
            //        FROM ALUMNOS AS A, MATRICULAS AS T, MODULOS AS M
            //        WHERE A.AlumnoId = T.AlumnoId AND T.ModuloId = M.ModuloId
            //        GROUP BY PELO, CASE WHEN Efectivo>=150 THEN 1 ELSE 0 END
            // GO
        }
    }
    static void RealizarQuery()
    {
        using (var db = new InstitutoContext())
        {
            // 1 - Calcular la media de Edad de los alumnos
            var media = db.Alumnos.Average(a => a.Edad);
            WriteLine($"1.- La media de Edad es {media}");

            // 2 - Calcular la media de Edad con Pelo rubio
            var mediaH = db.Alumnos.Where(a => a.Pelo == "rubio").Average(a => a.Edad);
            WriteLine($"2.- La media de Edad de rubios es {mediaH}");

            // 3 - Numero de matriculaciones del 3er Alumno
            var al3 = db.Alumnos.Include("Matriculas").OrderBy(a => a.AlumnoId).Skip(2).First();
            var nMatriculas = al3.Matriculas.ToList().Count;
            WriteLine($"3.- El alumno {al3} está matriculado en {nMatriculas}");

            // 4 - Suma de los Cŕeditos del 3er Alumno
            var modulosIds = al3.Matriculas.Select(m => m.ModuloId);
            var creditos = db.Modulos.Where(m => modulosIds.Contains(m.ModuloId)).Select(m => m.Créditos);
            var suma = creditos.Sum();
            WriteLine($"4.- El alumno {al3} suma {suma} créditos");

            // 5 - Suma de los Cŕeditos de todos los alumnos de Edad > 25 y pelo castaño
            suma = db.Alumnos
                //.Include("Matriculas")
                .Where(a => a.Edad > 25)
                .Where(a => a.Pelo == "castaño")
                .SelectMany(f => f.Matriculas.Select(m => m.Modulo.Créditos)).Sum();
            //creditos = matriculas2.Select(m => m.Modulo.Créditos);
            //suma = creditos.Sum();
            WriteLine($"5.- Los castaños de >25 suman {suma}");

            // 6 - Los modulos ordenados por numero de matriculaciones
            var modulos = db.Modulos.Select(m => new
            {
                modulo = m.Titulo,
                matriculaciones = m.Matriculaciones.Count
            }).OrderByDescending(m => m.matriculaciones).ToList();
            foreach (var m in modulos) WriteLine($"6.- {m.modulo} {m.matriculaciones}");

            // 7 - Suma de efectivo de los matriculados en el modulo primero
            var mod2 = db.Modulos.Include("Matriculaciones").OrderBy(a => a.ModuloId).First();
            var mat2 = db.Matriculas.Include("Alumno").Where(m => m.ModuloId == mod2.ModuloId).ToList();
            suma = mat2.Select(m => m.Alumno.Efectivo).Sum();
            WriteLine($"7.- Los matriculados en {mod2} tienen {suma}€");

            // 8 -

            // 9 -

            // 10 - suma de creditos agrupados por color del pelo y si tienen mas de 150€
            var sumaCreditosF = db.Alumnos
                    .Where(al => true) //Todos (lo dejo por si filtro)
                    .SelectMany(al => al.Matriculas, (a, m) => new
                    {
                        rico = a.Efectivo >= 150,
                        pelo = a.Pelo,
                        creditos = m.Modulo.Créditos
                    })
                    .ToList()
                    .GroupBy(a => new { a.rico, a.pelo })
                    .Select(g => new
                    {
                        rico = g.Key.rico,
                        pelo = g.Key.pelo,
                        total = g.Sum(a => a.creditos)
                    })
                    .OrderByDescending(x => x.rico)
                    .ThenBy(x => x.pelo)
                    .ToList();

            foreach (var x in sumaCreditosF)
                Console.WriteLine(x);

            // Un diccionario
            var sumaCreditosDic = db.Alumnos
                   .SelectMany(al => al.Matriculas, (a, m) => new
                   {
                       pelo = a.Pelo,
                       creditos = m.Modulo.Créditos
                   })
                   .ToList()
                   .GroupBy(a => a.pelo)
                   .Select(g => new
                   {
                       pelo = g.Key,
                       total = g.Sum(a => a.creditos)
                   })
                   .OrderByDescending(x => x.pelo)
                   .ThenBy(x => x.pelo)
                   .ToDictionary(x => x.pelo, x => x.total);

            foreach (var k in sumaCreditosDic.Keys)
                Console.WriteLine($"{k}  {sumaCreditosDic[k]}");
        }
    }

    static void Main(string[] args)
    {
        CrearBD();
        GenerarDatos();
        BorrarDatos();
        ShowData();
        RealizarQuery();
    }

}