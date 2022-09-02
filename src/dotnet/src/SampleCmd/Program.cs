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
global using Infrastructure;
global using Infrastructure.DataBase;
global using SampleCmd;
global using SampleCmd.DataBase;
global using Domain;

var builder = ConsoleApp.CreateBuilder(args);
builder.ConfigureServices((ctx,services) =>
{
    services.AddScoped<IEventInsert, EventInsert>();

    services.AddDbContext<SampleContext>(op => {
        op.UseNpgsql("Host=read_model;Port=5432;Username=postgres;Password=post;Database=sample");
    });

    services.AddDbContext<EventstoreContext>(op => {
        op.UseNpgsql("Host=write_model;Port=5432;Username=postgres;Password=post;Database=eventstore");
    });
});

var app = builder.Build();
app.AddCommands<Commands>();
app.Run();
