# Pruebas EF

### Agregados
Sum()
var suma = creditos.*Sum*();
Average()
var media = db.Alumnos.*Average*(a => a.Edad);

### Filtros y Paginación
Where(*lambda*)
db.Alumnos.Where(a => a.Edad > 25)
Skip()
Take()

Find
First
Last

### Inclusion de datos con Navegación
Include(*"MiembrosNavegables"*)
var al3 = db.Alumnos.Include("Matriculas").ToList();

### Clasificación
OrderBy(*lambda*)
var al3 = db.Alumnos.Include("Matriculas").OrderBy(a => a.AlumnoId).Skip(2).First();

OrderByDescending(*lambda*)
ThenBy(*lambda*)

### Mapeado de datos
Select(*lambda*)
SelectMany(*lambda*)

### Agrupaciones + Select
.GroupBy(a => a.pelo)
.Select(g => new
{
    pelo = g.Key,
    total = g.Sum(a => a.creditos)
})

.GroupBy(a => new { a.rico, a.pelo })
.Select(g => new
{
    rico = g.Key.rico,
    pelo = g.Key.pelo,
    total = g.Sum(a => a.creditos)
})