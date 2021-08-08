#tool nuget:?package=NUnit.ConsoleRunner&version=3.12.0
#tool nuget:?package=OpenCover&version=4.7.1221
#tool "nuget:?package=ReportGenerator&version=4.8.12"
#addin nuget:?package=Cake.Coverlet&version=2.5.4
using System.IO;

const string solutionFile = "./HourShifter.sln";
const string projectFile = "./src/HourShifter/HourShifter.csproj";
const string coverageFolder = "./coverage/";
const string publishFolder = "./publish/";
const string testProjectGlob = "test/**/*.csproj";
const string DEBUG = "Debug";
const string RELEASE = "Release";
const string windowsRID = "win-x64";

string target = Argument("target", "Default");
string configuration = Argument("configuration", DEBUG);
string runtimeIdentifier = Argument("rid", windowsRID);

Setup(context =>
{
	Information($"Building in: {configuration}");
});

Task("Clean")
	.Does(() =>
{
	Information("Cleaning in Debug and Release!");

	DotNetCoreClean(solutionFile, new DotNetCoreCleanSettings{
		NoLogo = true,
		Configuration = DEBUG,
		ArgumentCustomization = (builder) => {
			if (runtimeIdentifier == windowsRID) {
				return builder.Append($"/p:RID={runtimeIdentifier}");
			}
			else
			{
				return builder.Append($"-p:RID={runtimeIdentifier}");
			}
		}
	});

	DotNetCoreClean(solutionFile, new DotNetCoreCleanSettings{
		NoLogo = true,
		Configuration = RELEASE,
		ArgumentCustomization = (builder) => {
			if (runtimeIdentifier == windowsRID) {
				return builder.Append($"/p:RID={runtimeIdentifier}");
			}
			else
			{
				return builder.Append($"-p:RID={runtimeIdentifier}");
			}
		}
	});

	if (DirectoryExists(publishFolder))
	{
		DeleteDirectory(publishFolder, new DeleteDirectorySettings{
			Recursive = true,
		});
	}

	if (DirectoryExists(coverageFolder))
	{
		DeleteDirectory(coverageFolder, new DeleteDirectorySettings{
			Recursive = true,
		});
	}
});

Task("Restore")
	.Does(() => 
{
	DotNetCoreRestore(solutionFile, new DotNetCoreRestoreSettings{
		ArgumentCustomization = (builder) => {
			if (runtimeIdentifier == windowsRID) {
				return builder.Append($"/p:RID={runtimeIdentifier}");
			}
			else
			{
				return builder.Append($"-p:RID={runtimeIdentifier}");
			}
		}
	});
});

Task("Build")
	.IsDependentOn("Restore")
	.Does(() =>
{
	DotNetCoreBuild(solutionFile, new DotNetCoreBuildSettings{
		NoLogo = true,
		NoRestore = true,
		Configuration = configuration,
		ArgumentCustomization = (builder) => {
			if (runtimeIdentifier == windowsRID) {
				return builder.Append($"/p:RID={runtimeIdentifier}");
			}
			else
			{
				return builder.Append($"-p:RID={runtimeIdentifier}");
			}
		}
	});
});

Task("Test")
	.IsDependentOn("Build")
	.Does(() => 
{
	bool success = true;

	foreach (FilePath item in GetFiles(testProjectGlob))
	{
		try
		{
			DotNetCoreTest(
				item.ToString(),
				new DotNetCoreTestSettings {
					NoBuild = true,
					NoRestore = true,
					NoLogo = true,
					Verbosity = DotNetCoreVerbosity.Normal,
					Configuration = configuration,
					ArgumentCustomization = (builder) => {
						if (runtimeIdentifier == windowsRID) {
							return builder.Append($"/p:RID={runtimeIdentifier}");
						}
						else
						{
							return builder.Append($"-p:RID={runtimeIdentifier}");
						}
					}
				},
				new CoverletSettings {
					CollectCoverage = true,
					CoverletOutputDirectory = Directory(coverageFolder),
					CoverletOutputFormat = CoverletOutputFormat.opencover,
					CoverletOutputName =  $"results",
					Threshold = 100,
					ThresholdType = ThresholdType.Line | ThresholdType.Branch | ThresholdType.Method
				}
			);
		}
		catch
		{
			success = false;
		}
	}

	if (!success)
	{
		throw new Exception("Tests failed or coverage check failed.  See output.");
	}
});

Task("GenerateReport")
	.IsDependentOn("Test")
	.Does(() =>
{
	ReportGenerator((FilePath)"./coverage/results.opencover.xml", "./coverage/report");
});

Task("Publish")
	.IsDependentOn("Test")
	.Does(() =>
{
	DotNetCorePublish(projectFile, new DotNetCorePublishSettings{
		NoLogo = true,
		Configuration = configuration,
		ArgumentCustomization = (builder) => {
			if (runtimeIdentifier == windowsRID) {
				return builder.Append($"/p:RID={runtimeIdentifier}");
			}
			else
			{
				return builder.Append($"-p:RID={runtimeIdentifier}");
			}
		},
		OutputDirectory = publishFolder,
	});
});
Task("Default")
	.IsDependentOn("Test");

RunTarget(target);
