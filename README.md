# HourShifter

Simple command-line utility to shift the DateTaken EXIF metadata (ID 36867) of images by a number of hours.

## Dependencies

### Windows

None

### Unix

* libgdiplus

### MacOS

None? Using [runtime.osx.10.10-x64.CoreCompat.System.Drawing](https://www.nuget.org/packages/runtime.osx.10.10-x64.CoreCompat.System.Drawing)

## Compiling

Currently targeting .NET 5.

### Windows

* `> .\build.ps1` (RID "win-x64" is the default)

### Unix

* `> ./build.sh --rid="linux-x64"`

### MacOS

* `> ./build.sh --rid="osx-x64"`

## Publishing

Create a self-contained release for a certain RID:

### Windows

* `> .\build.ps1 --target=Publish` (RID "win-x64" is the default)

### Linux

* `> ./build.sh --target=Publish --rid="linux-x64"`

### MacOS

* `> ./build.sh --target=Publish --rid="osx-x64"`

NOTE:  See the full list of runtime identifiers (RIDs) here:  https://docs.microsoft.com/en-us/dotnet/core/rid-catalog

Check Releases on Github for the pre-compiled release for your operating system.  If your specific release isn't pre-built, pull down the code and build it locally.

## Command-line Arguments

* -h, --hours
  * Will shift all image DateTaken EXIF metadata found in the current directory and all subdirectories by a number of hours.  The default value is 12.
  * Ex:
    * -h 5
    * -h -2

* -c, --currentDirectoryOnly
  * Flag for only searching the current directory, NOT subdirectories.  The default is false (subdirectories WILL be searched.)

* -l, --logLevel
  * Flag to control the log level of the application.  Default: Info.  Acceptable values: Debug, Info, Warn, Error

* -q, --quiet
  * Flag for showing any output or waiting to press any key to exit.  Program will run and exit immediately without any output.  Log level is ignored.
  
* --help
  * Displays help screen.

* --version
  * Displays version information.

## Why?

While importing pictures scanned with the [Epson FastFoto FF-640](https://amazon.com/dp/B01HR89FNK) to Google Photos, I noticed that the DateTaken EXIF metadata was defaulted to a time of 00:00, but that the Date was correct.  When uploaded to Google Photos, it thought that the Time Zone was Central, which meant that all of the photos were showing a taken date of the previous day.  Windows doesn't seem to let you modify the **time** of the DateTaken value, so this was my quick solution.

## Problems?

Submit an issue or pull request 😃
