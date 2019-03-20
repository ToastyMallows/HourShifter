# HourShifter

Simple command-line utility to shift the DateTaken EXIF metadata (ID 36867) of images by a number of hours.

## Compiling

Currently targeting .NET Core 2.1.

* `> dotnet build .\HourShifter.sln`
* `> dotnet test .\HourShifter.sln`
* `> dotnet .\src\HourShifter\bin\netcoreapp2.1\HourShifter.dll --help`
* `> dotnet publish -c Release -r win-x64`

Note:  See list of runtime identifiers (RIDs) to replace `win-x64` here:  https://docs.microsoft.com/en-us/dotnet/core/rid-catalog 

## Command-line Arguments

* -h (hours)
  * Will shift all image DateTaken EXIF metadata found in the current directory and all subdirectories by a number of hours.  The default value is 12.
  * Ex:
    * -h 5
    * -h -2

* -c
  * Flag for only searching the current directory, NOT subdirectories.  The default is false (subdirectories WILL be searched.)
  
* --help
  * Displays help screen.

* --version
  * Displays version information.

## Why?

While importing pictures scanned with the [Epson FastFoto FF-640](https://amazon.com/dp/B01HR89FNK) to Google Photos, I noticed that the DateTaken EXIF metadata was defaulted to a time of 00:00, but that the Date was correct.  When uploaded to Google Photos, it thought that the Time Zone was Central, which meant that all of the photos were showing a taken date of the previous day.  Windows doesn't seem to let you modify the **time** of the DateTaken value, so this was my quick solution.

## Problems?

Submit an issue or pull request :)