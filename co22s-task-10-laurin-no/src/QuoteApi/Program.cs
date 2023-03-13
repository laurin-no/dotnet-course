using Microsoft.EntityFrameworkCore;
using QuoteApi.Data;

namespace QuoteApi;

// DO NOT remove the Program class declaration or the Main method. These are needed for the tests.

// DO edit the content of Main method to handle the task requirements.
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        builder.Services.AddDbContext<QuoteContext>(opt =>
            opt.UseSqlite(builder.Configuration.GetConnectionString("QuoteDb")));

        // Add "allow all" CORS as the default CORS policy. This is needed so that client apps in web browsers can access this API.
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(p =>
            {
                p.AllowAnyHeader();
                p.AllowAnyMethod();
                p.AllowAnyOrigin();
            });
        });

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        using (var serviceScope = app.Services.CreateScope())
        {
            var services = serviceScope.ServiceProvider;
            InitializeTheDatabase(app.Environment, services.GetRequiredService<QuoteContext>());
        }


        app.UseHttpsRedirection();

        app.UseCors();
        
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }

    private static void InitializeTheDatabase(IHostEnvironment env, QuoteContext db)
    {
        if (env.IsDevelopment())
        {
            db.Database.EnsureCreated();
            if (db.Quotes.Count() == 0)
            {
                db.Quotes.Add(new Quote
                {
                    Id = 1,
                    QuoteCreateDate = DateTime.Now,
                    QuoteCreator = "Pekka",
                    QuoteCreatorNormalized = "PEKKA",
                    TheQuote = "Get to da choppa",
                    WhenWasSaid = DateTime.Now,
                    WhoSaid = "Arnold Schwarzenegger"
                });
                db.Quotes.Add(new Quote
                {
                    Id = 2,
                    QuoteCreateDate = DateTime.Now,
                    QuoteCreator = "Pekka",
                    QuoteCreatorNormalized = "PEKKA",
                    TheQuote = "Get to da choppa",
                    WhenWasSaid = DateTime.Now,
                    WhoSaid = "Arnold Schwarzenegger"
                });
                db.Quotes.Add(new Quote
                {
                    Id = 3,
                    QuoteCreateDate = DateTime.Now,
                    QuoteCreator = "Erkki",
                    QuoteCreatorNormalized = "ERKKI",
                    TheQuote = "Come with me if you want to lift",
                    WhenWasSaid = DateTime.Now,
                    WhoSaid = "Arnold Schwarzenegger"
                });
                db.Quotes.Add(new Quote
                {
                    Id = 4,
                    QuoteCreateDate = DateTime.Now,
                    QuoteCreator = "Pekka",
                    QuoteCreatorNormalized = "PEKKA",
                    TheQuote = "Get to da choppa",
                    WhenWasSaid = DateTime.Now,
                    WhoSaid = "Arnold Schwarzenegger"
                });
                db.Quotes.Add(new Quote
                {
                    Id = 5,
                    QuoteCreateDate = DateTime.Now,
                    QuoteCreator = "Jonne",
                    QuoteCreatorNormalized = "JONNE",
                    TheQuote = "ES goes brrrrrrr",
                    WhenWasSaid = DateTime.Now,
                    WhoSaid = "Jonne Jokamies"
                });
                db.Quotes.Add(new Quote
                {
                    Id = 6,
                    QuoteCreateDate = DateTime.Now,
                    QuoteCreator = "Timo",
                    QuoteCreatorNormalized = "TIMO",
                    TheQuote = "Black round pirelli",
                    WhenWasSaid = DateTime.Now,
                    WhoSaid = "Marcus G"
                });

                db.SaveChanges();
            }
        }
    }
}