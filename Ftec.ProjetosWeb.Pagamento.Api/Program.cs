var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Pagamento & Frete API",
        Version = "v1",
        Description = "Microsserviço responsável por simulação de pagamento e cálculo de frete — Neurosky"
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("Neurosky", policy =>
    {
        policy.WithOrigins(
            "http://pedido.neurosky.com.br",
            "http://usuario.neurosky.com.br",
            "http://produto.neurosky.com.br",
            "http://categoria.neurosky.com.br",
            "http://avaliacao.neurosky.com.br",
            "http://estatistica.neurosky.com.br"
        )
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pagamento & Frete API v1");
    c.RoutePrefix = "swagger";
});

app.UseCors("Neurosky");

app.UseAuthorization();

app.MapControllers();

app.Run();
