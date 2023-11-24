using RedLockNet.SERedis.Configuration;
using RedLockNet.SERedis;
using RedLockNet;
using StackExchange.Redis;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionMultiplexer = ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("RedisConnectionString"));
builder.Services.AddSingleton<IConnectionMultiplexer>(connectionMultiplexer);
builder.Services.AddScoped(ctx => ctx.GetRequiredService<IConnectionMultiplexer>().GetDatabase());

var multiplexers = new List<RedLockMultiplexer> {connectionMultiplexer};

var redLockFactory = RedLockFactory.Create(multiplexers);

builder.Services.AddSingleton<IDistributedLockFactory>(redLockFactory);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
