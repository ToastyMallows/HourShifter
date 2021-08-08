# HourShifter

Simple command-line utility to shift the DateTaken EXIF metadata (ID 36867) of images by a number of hours.

## Compiling

Currently targeting .NET 5.

### Windows

* `> .\build.ps1`

### Linux/Mac

* `> ./build.sh`

## Publishing

Create a self-contained release for a certain RID:

### Windows

* `> dotnet publish -c Release -r win-x64`

### Linux

* `> dotnet publish -c Release -r linux-x64`

### MacOS

* `> dotnet publish -c Release -r osx-x64`

Note:  See the full list of runtime identifiers (RIDs) here:  https://docs.microsoft.com/en-us/dotnet/core/rid-catalog

Check Releases on Github for the pre-compiled release for your operating system.  If your specific release isn't pre-built, pull down the code and build it locally.

NOTE: I'm unable to test that the Mac OSX and Linux version work, please try them and report back if they don't! 🙂

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
