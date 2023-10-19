# RiffViewer
Welcome to RiffViewer repository! What you can find here is a final project of my Bachelor studies at [Brno University of Technology](https://www.vut.cz/en/). For a brief summary, it is a C# library that can help you load and analyze all kinds of [RIFF files](https://en.wikipedia.org/wiki/Resource_Interchange_File_Format).

## Main goals
There are some goals for this project that I would like to (and should if I want to graduate) achieve during my last year's studies. They are currently:

- Create a C# library for reading, writing and editing RIFF files
- Create a simple CLI tool that will work with this library
- Later, create a GUI with a nice presentation of the RIFF file

## Building and running the app
This app is built using .NET CORE 7, so you'll need to get that [here](https://dotnet.microsoft.com/en-us/download/dotnet/7.0). Once you download the SDK, simply go to the solution directory and run `dotnet build` command to build the app. You can also run the `dotnet run` command to run the app straight away.

To analyze a RIFF file through CLI tool, you'll need to specify a path to the file. Sample usage: `./RiffViewer.exe ./sound.wav`

## Contributing
As this is a project I need to finish for my Bachelor degree, I can't accept any kind of contributions from anyone, but once I finish my studies, I might continue working on this project and I will be able to accept contributions by then... anyways, if you're reading this, I can't accept your contributions!
