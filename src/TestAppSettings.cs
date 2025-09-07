namespace Xunit.Microsoft.DependencyInjection;

/// <summary>
/// Represents a test configuration file to be loaded into the fixture configuration root.
/// </summary>
public class TestAppSettings
{
	/// <summary>
	/// The JSON file name (relative or absolute) to load.
	/// </summary>
	public string? Filename { get; set; }

	/// <summary>
	/// Indicates whether the file is optional. When true missing files do not cause an exception.
	/// </summary>
	public bool IsOptional { get; set; } = false;
}
