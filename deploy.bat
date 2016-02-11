del Org.Kevoree.Group.WSGroup.1.1.0-rc1.nupkg
nuget pack kevoree-dotnet-group-ws.nuspec
nuget push Org.Kevoree.Group.WSGroup.1.1.0-rc1.nupkg Admin:Admin -Source http://localhost:81/nuget/Default
