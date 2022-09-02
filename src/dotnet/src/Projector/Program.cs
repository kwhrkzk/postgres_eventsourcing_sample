global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Text;
global using System.Text.Json;
global using System.Threading.Tasks;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.DependencyInjection;
global using ConsoleAppFramework;
global using Npgsql;
global using Projector;
global using Projector.DataBase;
global using Infrastructure;
global using Infrastructure.DataBase;
global using Domain;

var builder = ConsoleApp.CreateBuilder(args);
builder.ConfigureServices((ctx,services) =>
{
    services.AddDbContext<SampleDBContext>(op => {
        op.UseNpgsql("Host=read_model;Port=5432;Username=postgres;Password=post;Database=sample");
    });

    services.AddDbContext<EventstoreContext>(op => {
        op.UseNpgsql("Host=write_model;Port=5432;Username=postgres;Password=post;Database=eventstore");
    });
});

var app = builder.Build();
app.AddCommands<Commands>();
app.Run();
