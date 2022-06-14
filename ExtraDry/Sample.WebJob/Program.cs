﻿// See https://aka.ms/new-console-template for more information
using ExtraDry.Server.DataWarehouse;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sample.Data;
using Sample.Shared;
using System.Text.Json;

var options = new JsonSerializerOptions() { WriteIndented = true };
//var json = JsonSerializer.Serialize(warehouse, options);
//Console.WriteLine(json);
//Console.WriteLine(warehouse.GenerateSql());

var services = new ServiceCollection();
services.AddLogging(configure => {
    configure.AddConsole();
    configure.SetMinimumLevel(LogLevel.Trace);
});
services.AddScoped<WarehouseModelBuilder>();
var provider = services.BuildServiceProvider();
var dataFactoryLogger = provider.GetRequiredService<ILogger<DataFactory>>();
var warehouseLogger = provider.GetRequiredService<ILogger<WarehouseModelBuilder>>();

var builder = provider.GetRequiredService<WarehouseModelBuilder>();// new WarehouseModelBuilder(warehouseLogger);
builder.LoadSchema<SampleContext>();

builder.Fact<Company>().Measure(e => e.AnnualRevenue).HasName("Big Bucks");
builder.Dimension<Date>().HasDateGenerator(options => {
        options.StartDate = new DateOnly(2020, 1, 1);
        options.EndDate = new DateOnly(DateTime.UtcNow.Year, 12, 31);
        options.FiscalYearEndingMonth = 6;
    });
builder.Dimension<Date>().Attribute(e => e.DayOfWeekName).IsIncluded(false);
builder.Dimension<Time>().HasTimeGenerator();


var model = builder.Build();
var compareJson = JsonSerializer.Serialize(model, options);


var connectionString = @"Server=(localdb)\mssqllocaldb;Database=ExtraDrySample;Trusted_Connection=True;";
var dbOptionsBuilder = new DbContextOptionsBuilder<SampleContext>().UseSqlServer(connectionString);
var databaseContext = new SampleContext(dbOptionsBuilder.Options);


var warehouseConnectionString = @"Server=(localdb)\mssqllocaldb;Database=ExtraDryWarehouse;Trusted_Connection=True;";
var warehouseOptionsBuilder = new DbContextOptionsBuilder<WarehouseContext>().UseSqlServer(warehouseConnectionString);
var warehouseContext = new WarehouseContext(warehouseOptionsBuilder.Options);


var factoryOptions = new DataFactoryOptions() {
    BatchSize = 10,
    AutoMigrations = true,
};


var factory = new DataFactory(model, databaseContext, warehouseContext, dataFactoryLogger, factoryOptions);
while(await factory.ProcessBatchesAsync() > 0) ;
