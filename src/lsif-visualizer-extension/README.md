# Extension to run a language server from a Language Server Index Format file

The extension allows to browse the content of a LSIF dump stored in a file using LSIF line json format. To open a dump use the command Open LSIF Database.

The extension is currently not published to the market place due to its use of native node modules. You therefore need to run it out of source or generate our own platform dependent VSIX file using the vsce tool.

# Running the extension

1. Open this directory in VSCode
2. In the `Run and Debug` tab (Ctrl + Shift + D) select `Launch Client` and hit the green arrow.
3. Open the command pallete (Ctrl + Shift + P) and type `Open LSIF Database`
    - *Note: you can only visualize "linked" LSIF files. Utilize the dotnet tool `lsif-debug` (also in this repository) to link LSIF files*
4. Select the LSIF file you're trying to visualize

You can now navigate the LSIF dump in the folder view tab and interact with files as if you were in the browser (can hover, go-to-definition etc.).

![gif of visualizing an LSIF dump](https://i.imgur.com/oVA7rgz.gif)