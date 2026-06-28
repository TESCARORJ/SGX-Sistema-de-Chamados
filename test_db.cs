using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SGX.SistemaChamado.Infrastructure.Persistence;

var options = new DbContextOptionsBuilder<SGXSistemaChamadoDbContext>()
    .UseNpgsql("Host=localhost;Database=sgx_chamados;Username=postgres;Password=postgres")
    .Options;

using var db = new SGXSistemaChamadoDbContext(options);

var items = db.RoadmapChecklists
    .Where(x => x.RoadmapItemId == Guid.Parse("78787878-7878-7878-7878-000000000018"))
    .OrderBy(x => x.Ordem)
    .ToList();

Console.WriteLine($"Total active items: {items.Count(x => x.Ativo)}");
Console.WriteLine($"Total concluded items: {items.Count(x => x.Concluido)}");

foreach(var item in items)
{
    Console.WriteLine($"Ordem {item.Ordem}: Concluido = {item.Concluido}");
}
