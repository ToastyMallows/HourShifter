#tool nuget:?package=NUnit.ConsoleRunner&version=3.11.1

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

const string solutionFile = "./HourShifter.sln";
const string DEBUG = "Debug";
const string RELEASE = "Release";

string target = Argument("target", "Default");
string configuration = Argument("configuration", DEBUG);

Setup(context => {
	Information($"Building in: {configuration}");
});

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
	.Does(() => {
		Information("Cleaning in Debug and Release!");

		DotNetCoreClean(solutionFile, new DotNetCoreCleanSettings(){
			Configuration = DEBUG
		});

		DotNetCoreClean(solutionFile, new DotNetCoreCleanSettings(){
			Configuration = RELEASE
		});
	});

Task("Restore")
	.Does(() => {
		DotNetCoreRestore(solutionFile);
	});

Task("Build")
	.IsDependentOn("Restore")
	.Does(() => {
		DotNetCoreBuild(solutionFile, new DotNetCoreBuildSettings() {
			Configuration = configuration
		});
	});

Task("Test")
	.IsDependentOn("Build")
	.Does(() => {
		DotNetCoreTest(solutionFile, new DotNetCoreTestSettings() {
			Configuration = configuration
		});
	});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
	.IsDependentOn("Test");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
