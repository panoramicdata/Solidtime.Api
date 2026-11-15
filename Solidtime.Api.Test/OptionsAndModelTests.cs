using Solidtime.Api.Exceptions;
using System.Net;
using System.Reflection;
using System.Text.Json;

namespace Solidtime.Api.Test;

/// <summary>
/// Offline tests for option validation, the client surface, exceptions and model serialisation
/// </summary>
public class OptionsAndModelTests
{
	private static readonly JsonSerializerOptions _json = new() { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };

	/// <summary>
	/// Invalid option values are rejected with a message naming the problem
	/// </summary>
	[Theory]
	[InlineData("", "https://x", 30, "ApiToken")]
	[InlineData("t", " ", 30, "BaseUrl is required")]
	[InlineData("t", "https://x", 0, "TimeoutSeconds")]
	[InlineData("t", "ftp://x", 30, "not a valid")]
	[InlineData("t", "relative/path", 30, "not a valid")]
	public void Validate_InvalidOptions_Throw(string token, string baseUrl, int timeout, string expected)
	{
		var options = new SolidtimeClientOptions { ApiToken = token, BaseUrl = baseUrl, TimeoutSeconds = timeout };

		new Action(options.Validate).Should().Throw<InvalidOperationException>().WithMessage($"*{expected}*");
	}

	/// <summary>
	/// The defaults are valid
	/// </summary>
	[Fact]
	public void Validate_Defaults_Pass()
	{
		var options = new SolidtimeClientOptions { ApiToken = "t" };

		options.Validate();

		options.BaseUrl.Should().Be("https://app.solidtime.io/api");
		options.TimeoutSeconds.Should().Be(30);
		options.JsonUnmappedMemberHandling.Should().Be(System.Text.Json.Serialization.JsonUnmappedMemberHandling.Skip);
	}

	/// <summary>
	/// A client exposes every endpoint group and can be disposed twice
	/// </summary>
	[Fact]
	public void Client_ExposesEndpointsAndDisposesTwice()
	{
		var client = new SolidtimeClient(new SolidtimeClientOptions { ApiToken = "t" });

		new object?[]
		{
			client.ApiTokens, client.Me, client.Organizations, client.Clients, client.Projects, client.Tags,
			client.Tasks, client.TimeEntries, client.ProjectMembers, client.Members, client.Reports,
			client.Charts, client.Imports
		}.Should().OnlyContain(endpoint => endpoint != null);

		client.Dispose();
		new Action(client.Dispose).Should().NotThrow();
	}

	/// <summary>
	/// A null options object or invalid options stop the client being built
	/// </summary>
	[Fact]
	public void Client_InvalidOptions_Throw()
	{
		new Action(() => _ = new SolidtimeClient(null!)).Should().Throw<ArgumentNullException>();
		new Action(() => _ = new SolidtimeClient(new SolidtimeClientOptions { ApiToken = "" })).Should().Throw<InvalidOperationException>();
	}

	/// <summary>
	/// The exception constructors keep what they are given
	/// </summary>
	[Fact]
	public void ApiException_KeepsMessageInnerAndStatus()
	{
		var inner = new InvalidOperationException("inner");

		new SolidtimeApiException().StatusCode.Should().BeNull();
		new SolidtimeApiException("m").Message.Should().Be("m");
		new SolidtimeApiException("m", inner).InnerException.Should().BeSameAs(inner);
		new SolidtimeApiException("m", HttpStatusCode.NotFound).StatusCode.Should().Be(HttpStatusCode.NotFound);
	}

	/// <summary>
	/// The model type names to round-trip
	/// </summary>
	public static TheoryData<string> ModelNames()
	{
		var data = new TheoryData<string>();
		foreach (var type in ModelTypes())
		{
			data.Add(type.Name);
		}

		return data;
	}

	/// <summary>
	/// Every model survives a serialise, deserialise, serialise round trip with every property set
	/// </summary>
	[Theory]
	[MemberData(nameof(ModelNames))]
	public void Model_RoundTrips(string name)
	{
		var type = ModelTypes().Single(t => t.Name == name);
		var instance = Populate(type);

		var first = JsonSerializer.Serialize(instance, type, _json);
		var second = JsonSerializer.Serialize(JsonSerializer.Deserialize(first, type, _json), type, _json);

		second.Should().Be(first);
		first.Should().NotBe("{}");
	}

	/// <summary>
	/// The generic envelopes read the wire format
	/// </summary>
	[Fact]
	public void Envelopes_ReadWireFormat()
	{
		var single = JsonSerializer.Deserialize<DataWrapper<Tag>>("{\"data\":{\"id\":\"1\",\"name\":\"n\"}}", _json)!;
		var page = JsonSerializer.Deserialize<PaginatedResponse<Tag>>("{\"data\":[{\"id\":\"1\",\"name\":\"n\"}],\"meta\":{\"current_page\":2,\"total\":1}}", _json)!;

		single.Data.Name.Should().Be("n");
		page.Data.Should().ContainSingle().Which.Id.Should().Be("1");
		page.Meta!.CurrentPage.Should().Be(2);
		page.Links.Should().BeNull();
	}

	private static List<Type> ModelTypes()
		=> [.. typeof(SolidtimeClient).Assembly.GetTypes()
			.Where(t => t.Namespace == "Solidtime.Api.Models" && t.IsClass && !t.IsAbstract && !t.IsGenericTypeDefinition && t.IsPublic)
			.OrderBy(t => t.Name)];

	private static object? Value(Type type)
	{
		var target = Nullable.GetUnderlyingType(type) ?? type;
		if (target == typeof(string))
		{
			return "x";
		}

		if (target == typeof(int))
		{
			return 1;
		}

		if (target == typeof(bool))
		{
			return true;
		}

		if (target == typeof(DateTimeOffset))
		{
			return new DateTimeOffset(2024, 1, 2, 3, 4, 5, TimeSpan.Zero);
		}

		if (target.IsGenericType && target.GetGenericTypeDefinition() == typeof(List<>))
		{
			var list = (System.Collections.IList)Activator.CreateInstance(target)!;
			list.Add(Value(target.GetGenericArguments()[0]));
			return list;
		}

		return target.IsClass && !target.IsGenericType ? Populate(target) : null;
	}

	private static object Populate(Type type)
	{
		var instance = Activator.CreateInstance(type)!;
		foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanWrite))
		{
			property.SetValue(instance, Value(property.PropertyType));
		}

		return instance;
	}
}
