namespace LanguageLearningApp.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<LanguageAppContext>(
                options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("LanguageAppConnection")));

            builder.Services.AddControllers();

            builder.Services.AddScoped<UserService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseHttpsRedirection();

            app.MapControllers();


            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                    options.RoutePrefix = "swagger";
                    options.DocumentTitle = "LanguageApp Swagger";
                });
            }

            app.Run();
        }
    }
}
