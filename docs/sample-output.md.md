# Git History (Part 1)

Period: 2024-01-01 to 2024-03-31 Total commits in this file: 248

## 1a2b3c4d (2024-03-31)

Feature: Add new Excel ribbon customization

### Added:

#### SomeProject/ExcelRibbon.cs

```csharp
public class ExcelRibbon : Office.IRibbonExtensibility
{
    public string GetCustomUI(string ribbonId)
    {
        // Implementation details...
    }
}
```

#### Others

- SomeProject/References/Sample File.xlsx
- SomeProject/Assets/app.ico

### Deleted:

#### SomeProject/Class_Old.cs

```diff
-Public Class Constants
-
-    Public Const HeadingBlue As Long = 13998939
-    Public Const TableBodyLightBlue As Long = 1591752
```

#### Others

- SomeProject/Assets/app2.ico
- SomeProject/References/Outdated Sample.xlsx

### Modified/Renamed:

#### SomeProject/ThisAddIn.cs

```diff
+ using Microsoft.Office.Core;
- using Microsoft.Office.Interop.Excel;
  
  public partial class ThisAddIn
  {
+     private ExcelRibbon ribbon;
-     private bool initialized;
```

#### SomeProject/ClassRenamedAndModified.cs

```diff
similarity index 98%
rename from SomeProject/ClassBeforeRename.cs
rename to SomeProject/ClassRenamedAndModified.cs
@@ -1,4 +1,4 @@
-﻿Imports GraphicSchedule.Utilities.Extensions.Core
+﻿Imports GraphicSchedule.Common.Extensions.Core
 Public Class ExcelFormulaBuilder
```

#### Others

- SomePath/photo.png
- SomePath/renamed-excel.xlsx (from SomePath/before-renamed-excel.xlsx)

### Other Changes:
- {Change type} {Relative File Path}