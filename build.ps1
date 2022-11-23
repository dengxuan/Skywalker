Remove-Item nupkgs\*

#Extensions
dotnet pack Skywalker.sln -c Release -p:SymbolPackageFormat=snupkg  -p:IncludeSymbols=true --output nupkgs
