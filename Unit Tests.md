# Unit Tests

## Organisation

Ist mir selbst auch eingefallen, aber in [Zen Coding: Structuring Unit Tests](http://zendeveloper.blogspot.co.at/2012/01/structuring-unit-tests.html) ist es schon passend beschrieben.  
Anstatt der nested classes werden hier Namespaces und top-level classes verwendet, damit es übersichtlicher ist. Organisatorisch heißt das, dass (Unter-) Ordner eingeführt werden. Z.B.  
```
Tests.csproj
+ Services
  + ProcessorTests
    | MethodA.cs
    | MethodB.cs
    | ...
  + RepositoryTests
    | Base.cs (common SetUp-Code)
    | MethodA.cs
    | MethodB.cs
    | ...
    + MethodC
      | StateA.cs
      | StateB.cs
      | ...
```

## Benennung

Das Schema von [Naming standards for unit tests - Osherove](http://osherove.com/blog/2005/4/3/naming-standards-for-unit-tests.html) fand und finde ich ganz passend, nur mit einer Modifikation die obige Orginasation berücksichtigt und mMn leserlicher ist.

`State_under_Test___expected_Behavior` 

Es entspricht also diesem Regex-Pattern: `^([^_]+_)*[^_]+_{3}([^_]+_)*[^_]+$`

Anmerkung: wenn wie oben gezeigt die States in Ordner aufgebrochen werden, so kann sich die Benennung zu `expected_Bahavior` verkürzen, da der State bereits im Ordner-Namen enthalten ist.