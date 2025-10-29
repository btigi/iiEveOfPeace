iiEighthSolitude
=========

iiEveOfPeace is a C# library supporting the modification of files relating to 7th Legion, the 1997 RTS game developed by Vision Software.

| Name       | Read | Write | Comment
|------------|:----:|-------|--------
| COLOURS    | ✗   |   ✗   | LUA
| CON        | ✗   |   ✗   | LUA
| DDS        | ✗   |   ✗   | Standard DDS
| FDA        | ✗   |   ✗   | 
| LUA        | ✗   |   ✗   | LUA
| RAT        | ✗   |   ✗   | 
| RSH        | ✗   |   ✗   | 
| SCREEN     | ✗   |   ✗   | LUA
| SGA        | ✔   |   ✗   | 
| TGA        | ✗   |   ✗   | Standard TGA
| UCS        | ✗   |   ✗   | Plain text (unicode)
| WHE        | ✗   |   ✗   | 
| WHM        | ✗   |   ✗   | 
| WTP        | ✗   |   ✗   | 


## Usage

Instantiate the relevant class and call the `Read` method passing the filename.

```csharp
var sgaProcessor = new SgaProcessor();
var files = sgaProcessor.Read(@"D:\SteamLibrary\steamapps\common\Dawn of War Gold\Engine\Engine.sga");

foreach (var file in files)
{
    File.WriteAllBytes(Path.Combine(@"D:\data\DawnOfWar", file.filename), file.data);
}
```


## Compiling

To clone and run this application, you'll need [Git](https://git-scm.com) and [.NET](https://dotnet.microsoft.com/) installed on your computer. From your command line:

```
# Clone this repository
$ git clone https://github.com/btigi/iiEveOfPeace

# Go into the repository
$ cd src

# Build  the app
$ dotnet build
```

## Licencing

iiEveOfPeace is licenced under the MIT License. Full licence details are available in licence.md

Thanks to [vojcermak](https://github.com/vojcermak/open-compote) for information on the SGA format.