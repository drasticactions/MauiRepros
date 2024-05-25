using System.Text.Json.Serialization;

var builder = WebApplication.CreateSlimBuilder(args);
builder.Services.ConfigureHttpJsonOptions(options =>
{ 
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

var app = builder.Build();
app.MapGet("/hi", () => "hi");
app.MapPost("send-money", (SendMoneyRequest request) =>
{
    var receipt = new Receipt($"{request.From.FirstName} {request.From.LastName}",
        $"{request.To.FirstName} {request.To.LastName}",
        request.Amount,
        DateTime.Now,
        $"{request.To.Address.Street}, {request.To.Address.City}, {request.To.Address.State}, {request.To.Address.Zip}");
    return receipt;
});

app.Run();

[JsonSerializable(typeof(Receipt))]
[JsonSerializable(typeof(SendMoneyRequest))]
[JsonSerializable(typeof(AccountHolder))]
[JsonSerializable(typeof(Address))]
[JsonSerializable(typeof(State))]
partial class AppJsonSerializerContext : JsonSerializerContext
{
}

record AccountHolder(Guid Id, string FirstName, string LastName, Address Address, string Email);
record Address(string Street, string City, [property: JsonConverter(typeof(JsonStringEnumConverter<State>))] State State, string Zip);
record SendMoneyRequest(AccountHolder From, AccountHolder To, decimal Amount, DateTime SendOn);
record Receipt(string FromAccount, string ToAccount, decimal Amount, DateTime CreatedOn, string ToAddress);

[JsonConverter(typeof(JsonStringEnumConverter<State>))]
enum State { CA, NY, WA, TX, FL, IL, PA, OH, GA, MI, NC, NJ, VA }
