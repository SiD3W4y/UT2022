# 🇬🇧󠁧󠁢󠁥󠁮󠁧󠁿 English
Unit tests for the programming exercises given to the 2022 class @ EPITA.

The [template](./Template.cs) file provides an easy way to create unit tests for a subject.

**Pull requests are welcome.**

# 🇫🇷 French
Unit tests pour les TPs donnés chaque semaine à la promo 2022 @ EPITA.

Le fichier [template](./Template.cs) déclare des classes qui permettent de facilement créer des tests pour un sujet.

**Les pull requests sont les bienvenues.**

# Ex(e|a)mple
#### `Program.cs`
```csharp
public class Program
{
    public static void Main()
    {
        Tests.Perform(new Program(),    // tests instance methods in Program
                      typeof(Program)); // tests static methods in Program
    }
    
    [Test(1, 1, 2), Test(2, 3, 5)]
    public static void Maths(int a, int b, int result)
    {
        Tests.Assert(a + b == result);
    }
    
    [Test(ShouldFail = true)]
    public void BadMaths()
    {
        Tests.Assert(1 + 2 == 4);
    }
    
    [Test]
    public void BadDivision()
    {
        Tests.AssertException<ArgumentOutOfRangeException>(() =>
        {
            int a = 5;
            int b = 0;

            Console.WriteLine(a / b);
        });
    }
}
```

#### Output:
```
Now testing Void BadMaths()
  Test successful.

Now testing Void BadDivision()
  Test failed.
  -> (Program.cs :: 30:13) Expected an exception of type ArgumentOutOfRangeException,
                           but got an exception of type DivideByZeroException.

Now testing Void Maths(Int32, Int32, Int32)
  Test successful.

Now testing Void Maths(Int32, Int32, Int32)
  Test successful.

Unit tests session over: 3/4 tests successful.
```
