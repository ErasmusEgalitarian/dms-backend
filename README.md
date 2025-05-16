# DMS API

**To run**
Install requirements 
```dotnet add package MongoDB.Driver```
Create a file Secrets.cs in the root folder
```cs
namespace DMS.Secrets
{
    public static class Secrets
    {
        public static string DBUsername = "<Your DB username>";
        public static string DBPassword = "<Your DB password>";
    }
}

```
And run in the terminal with 
```
dotnet run
```

