var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // ← Swashbuckle

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();     // ← Swashbuckle
    app.UseSwaggerUI();   // ← Swashbuckle
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();