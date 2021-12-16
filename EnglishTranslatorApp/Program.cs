using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<EnglishTranslationDbContext>(options => options.UseInMemoryDatabase(builder.Configuration.GetValue<string>("DbName")));
var app = builder.Build();

app.MapPost("/Translator/", async (EnglishTranslateEntity translateEntity, EnglishTranslationDbContext db) => {
    db.TranslatedEntities.Add(translateEntity);
    await db.SaveChangesAsync();
});

app.MapGet("Translator/English/{translationRequest}", async (TranslationRequest translationRequest, EnglishTranslationDbContext db) =>
{
    return await db.TranslatedEntities.SingleOrDefaultAsync(x => x.Word == translationRequest.Word && x.TranslatedWordLanguage == translationRequest.Language);
});

app.Run();


public class EnglishTranslateEntity
{
    public int Id { get; set; }
    public string Word { get; set; }
    public string TranslatedWord { get; set; }
    public string TranslatedWordLanguage { get; set; }

}

class EnglishTranslationDbContext: DbContext
{
    public EnglishTranslationDbContext(DbContextOptions<EnglishTranslationDbContext> options) : base(options) { }

    public DbSet<EnglishTranslateEntity> TranslatedEntities => Set<EnglishTranslateEntity>();
}

public record TranslationRequest(string Word, string Language)
{
    public static bool TryParse(string input, out TranslationRequest? request)
    {
        request = default;
        var splittedInput = input.Split(',');
        if(splittedInput.Length > 2)
            return false;

        request = new TranslationRequest(splittedInput[0], splittedInput[1]);
        return true;
    }
}