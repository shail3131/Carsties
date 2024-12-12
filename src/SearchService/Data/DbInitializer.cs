

namespace SearchService.Data;

using System.Text.Json;
using System;
using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Models;
using SearchService.Services;

public class DbInitializer
{

  public static async Task InitDb(WebApplication app)
  {
    await DB.InitAsync("SearchDb", MongoClientSettings
    .FromConnectionString(app.Configuration.GetConnectionString("MongoDbConnection")));

    await DB.Index<Item>()
   .Key(x => x.Make, KeyType.Text)
   .Key(x => x.Model, KeyType.Text)
   .Key(x => x.Color, KeyType.Text)
   .CreateAsync();

    var count = await DB.CountAsync<Item>();

    Console.WriteLine("No data - will attempt to seed");

    using var scope = app.Services.CreateScope();

    var httpClient = scope.ServiceProvider.GetService<AuctionSvcHttpClient>();

    var items = await httpClient.GetItemsForSearchDb();

    Console.WriteLine(items.Count + " returened from the auction service");

    if (items.Count > 0) await DB.SaveAsync(items);


  }

}
