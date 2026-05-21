using Microsoft.AspNetCore.Identity;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Reflection;

namespace TaskApi.Features.Auth.Domain;

[JsonConverter(typeof(UserJsonConverter))]
public class User : IdentityUser<int>
{
    #pragma warning disable 
    public User()
    {
    }
    #pragma warning restore
    public string Fullname { get; set; }
}

public class UserJsonConverter : JsonConverter<User>
{
    public override User Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var jsonDocument = JsonDocument.ParseValue(ref reader);
        var user = JsonSerializer.Deserialize<User>(jsonDocument.RootElement.GetRawText(), 
            new JsonSerializerOptions { Converters = { } });
        return user ?? new User();
    }

    public override void Write(Utf8JsonWriter writer, User value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        
        var userProperties = typeof(User).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        
        foreach (var prop in userProperties)
        {
            var propValue = prop.GetValue(value);
            var propertyName = options.PropertyNamingPolicy?.ConvertName(prop.Name) ?? prop.Name;
            
            writer.WritePropertyName(propertyName);
            JsonSerializer.Serialize(writer, propValue, prop.PropertyType, options);
        }
        
        writer.WriteEndObject();
    }
}