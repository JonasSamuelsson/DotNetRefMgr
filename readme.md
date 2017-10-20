# DotNetRefMgr
DotNetRefMgr is a tool for switching between package & project references within a project and/or solution.

## Initialize
In order to switch between kind of references the project file(s) must first be initialized using:
```
dotnetrefmgr.exe initialize <path>
```
That will update the project files and add the following item groups
```
<ItemGroup>
    <!-- <PackageReferences> -->
    <!-- </PackageReferences> -->
</ItemGroup>

<ItemGroup>
    <!-- <ProjectReferences> -->
    <!-- </ProjectReferences> -->
</ItemGroup>
```

The next step is to add package & project references to your project (manually or using nuget) and move them to their dedicated section.

## Configure

The syntax for switching reference type is
```
dotnetrefmgr.exe configure <path> --type package|project
```
