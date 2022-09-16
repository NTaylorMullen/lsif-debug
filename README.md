# LSIF Debug Tool

This repository contains a dotnet tool that allows you to better debug / understand LSIF output.

# How to install

*Note: You must have the [.NET 6.0 SDK](https://dotnet.microsoft.com/en-us/download) installed*

```powershell
dotnet tool install lsif-debug --prerelease --global
```

# Usage

```
Description:
  Tool to allow you to better debug / understand LSIF output.

Usage:
  lsif-debug [command] [options]

Options:
  --version       Show version information
  -?, -h, --help  Show help and usage information

Commands:
  link <lsif> <source>  Link a *.lsif file to a given source repository.
  visualize <lsif>      Visualize a linked LSIF file in VSCode.
```

## Link

The `link` command is used to merge vital information from a source repository into an LSIF file to make it easier to visualize.

Once linked dumps can be visualized on any machine.

### Linking a single LSIF file

```powershell
lsif-debug link C:/Users/JohnDoe/Downloads/Example.lsif C:/GitHub/RepoThatGendLSIFFolder
```

**Output:** `C:/Users/JohnDoe/Downloads/Example.linked.lsif`

### Linking multiple LSIF files

Sometimes LSIF is separated on a per-project / scope basis resulting in multiple LSIF files representing a single dump. In this situation `lsif-debug` can combine linked information of all LSIF files into one final linked output.

```powershell
lsif-debug link C:/Users/JohnDoe/Downloads/FolderWithLotsOfLSIF C:/GitHub/RepoThatGendLSIF
```

**Output:** `C:/Users/JohnDoe/Downloads/FolderWithLotsOfLSIF.linked.lsif`

## Visualize

The `visualize` command installs a VSCode extension to visualize LSIF and then launches VSCode with the given linked LSIF path.

```powershell
lsif-debug visualize C:/Users/JohnDoe/Downloads/Example.linked.lsif
```

Once in VSCode you can interact with the LSIF dump as if you were in your IDE (hover, definition, references etc.):

![gif showing visualization](https://i.imgur.com/4zixIzc.gif)

# Contributing

Contributions are welcome! For instructions on how to build / debug the source see below.

## lsif-debug

Open solution in Visual Studio and done!

*Note: Packaging lsif-debug uses a pre-built version of lsif-visualizer-extension (one I hacked together). Contributions making this more automated would be super welcome*.

## lsif-visualizer-extension

See its dedicated [README.md](src/lsif-visualizer-extension/README.md)

## Testing

lol
