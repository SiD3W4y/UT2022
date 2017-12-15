// Source: https://github.com/6A/UT2022
// ReSharper disable All

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using miniPokemon;

/// <summary>
///   A simple class used for unit tests.
/// </summary>
[DebuggerNonUserCode]
internal class Tests
{
    #region Tests
    // ==================================================================================================================>>
    // Insert your tests below ==========================================================================================>>
    // ==================================================================================================================>>

    [Test(Skip = true)]
    public static void Template()
    {
    }
    
    
    [Test]
    public static void TestGetterSetter()
    {
        string whoami = "I am an animal !\n";
        string describe = "My name is Epicat.\n";
        string name = "Epicat";
        
        Animal cat = new Animal("Epicat");

        string whoami_test = Output(() => { cat.WhoAmI(); });
        string describe_test = Output(() => { cat.Describe();});
        string rename_test = "Moumoune";
        
        Tests.Assert(whoami_test == whoami,"Got "+whoami_test+" instead of "+whoami);
        Tests.Assert(describe_test == describe,"Got "+describe_test+" instead of "+describe);

        cat.Rename("Moumoune");
        
        Tests.Assert(cat.Name == rename_test,"Got "+cat.Name+" instead of "+rename_test);
        

    }
  
    [Test]
    public static void TestPokemon()
    {
        Pokemon testy = new Pokemon("TEST",100,150,Pokemon.Poketype.ELECTRICK);

        string description_target = "My name is TEST I'm a pokemon of type ELECTRICK and I'm level 1\n";
        string description_res = Output(() => { testy.Describe(); });
        
        Tests.Assert(testy.IsKO == false,"Pokemon should be alive when init");
        Tests.Assert(description_target == description_res,"Got "+description_res+" instead of "+description_target);
        
        testy.GetHurt(5000);
        
        Tests.Assert(testy.IsKO == true,"Still alive after -5000 Hp / 100 ?!");
        Tests.Assert(testy.Life == 0,"Your pokemon has "+testy.Life+" after a 5000HP hit instead of 0");
        
        testy.Heal(1);
        
        Tests.Assert(testy.Life == 1,"Your Pokemon should have had 1HP instead of (healing 1HP from zero)"+testy.Life);
    }

    [Test]
    public static void TestTrain()
    {
        int age = 22;
        Trainer bob = new Trainer("Bob",age);

        string whoami_target = "I'm a pokemon Trainer !\n";
        string desc_target = "My name is Bob, I'm 22 and I have 0 Pokemon !\n";
        
        string whoami_res = Output(() => {bob.WhoAmI();});
        string desc_res = Output(() => { bob.Describe(); });

        string display_target = "My Pokemon are :\n- a\n- b\n";
        
        
        Tests.Assert(whoami_target == whoami_res,"Got ["+whoami_res+"] instead of "+whoami_target);
        Tests.Assert(desc_target == desc_res,"Got "+desc_res+ "instead of "+desc_target);
        
        bob.Birthday();
        
        Tests.Assert(bob.Age == age+1,"Got age "+bob.Age+" after birthday instead of "+age+1);
        
        bob.CatchAPokemon(new Pokemon("a",100,100,Pokemon.Poketype.FIRE));
        bob.CatchAPokemon(new Pokemon("b",100,100,Pokemon.Poketype.FIRE));
        
        string display_res = Output(() => { bob.MyPokemon(); });
        
        Tests.Assert(bob.NumberOfPokemon() == 2,"Got count of "+bob.NumberOfPokemon()+" instead of two");
        Tests.Assert(display_res == display_target,"Got "+display_res+" instead of "+display_target);
        

    }
    #endregion


        
    #region Performing tests and utilities
    /// <summary>
    ///   Returns the output of the given action in the standard output.
    /// </summary>
    public static string Output(Action action)
    {
        TextWriter originalWriter = Console.Out;
        
        using (StringWriter writer = new StringWriter())
        {
            Console.SetOut(writer);

            try
            {
                action();
            }
            finally
            {
                Console.SetOut(originalWriter);
            }

            return writer.ToString();
        }
    }

    /// <summary>
    ///   Returns the output of the given action in the standard output,
    ///   and sets <paramref name="value"/> to the return value of the action.
    /// </summary>
    public static string Output<T>(Func<T> action, out T value)
    {
        TextWriter originalWriter = Console.Out;
        
        using (StringWriter writer = new StringWriter())
        {
            Console.SetOut(writer);

            try
            {
                value = action();
            }
            finally
            {
                Console.SetOut(originalWriter);
            }

            return writer.ToString();
        }
    }

    private static void PrintSuccess()
    {
    	Console.ForegroundColor = ConsoleColor.Green;
    	Console.WriteLine("  Test successful.");
    	Console.WriteLine();
    }
    
    /// <summary>
    ///   Performs all tests declared in this class and in the additional classes that have been specified.
    /// </summary>
    /// <param name="objects">
    ///   A list of objects to test.
    ///   If the object is a <see cref="Type"/>, then all of its static methods will be tested.
    ///   If it isn't, then all of its instance methods will be tested.
    /// </param>
    /// <remarks>
    ///   The methods that are to be tested must be marked with <see cref="TestAttribute"/>.
    /// </remarks>
    /// <example>
    ///   Tests.Perform(typeof(Program), // for the static methods
    ///                 new Program());  // for the instance methods
    /// </example>
    public static void Perform(params object[] objects)
    {
        const BindingFlags ALL = BindingFlags.NonPublic | BindingFlags.Public;

        // execute them one by one
        int nbfails = 0,
            nbtests = 0;

        foreach (object obj in objects.Concat(new[] {typeof(Tests)}))
        {
            Type type = obj as Type ?? obj?.GetType();

            if (type == null)
                continue;

            foreach (MethodInfo method in type.GetMethods(ALL | (obj is Type ? BindingFlags.Static : BindingFlags.Instance)))
            foreach (TestAttribute attribute in method.GetCustomAttributes<TestAttribute>())
            {
                if (attribute.Skip)
                    continue;

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Now testing {0}", method);

                nbtests++;

                object instance = method.IsStatic ? null : obj;
                bool forcedFail = false;

                try
                {
                    method.Invoke(instance, attribute.Arguments);

                    if (attribute.ShouldFail)
                    {
                        forcedFail = true;
                        throw new AssertionException("Excepted test to fail, but it succeeded.");
                    }

                    PrintSuccess();

                    continue;
                }
                catch when (attribute.ShouldFail && !forcedFail)
                {
                    PrintSuccess();

                    continue;
                }
                catch (Exception e)
                {
                    while (e is TargetInvocationException)
                        e = (e as TargetInvocationException).InnerException;

                    if (e is AssertionException)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.WriteLine("  Test failed.");

                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.WriteLine("  -> {0}", e);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("  Unexpected error in test.");

                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.WriteLine("  -> {0}", e.Message);
                    }
                }

                nbfails++;

                if (attribute.IsFatal)
                    break;

                Console.WriteLine();
            }
        }

        Console.ForegroundColor = ConsoleColor.Blue;
        Console.Write("Unit tests session over: ");

        string s = nbtests - nbfails > 1 ? "s" : "";

        if (nbfails == 0)
            Console.WriteLine("all test{0} successful.", s);
        else if (nbtests != 0)
            Console.WriteLine("{0}/{1} test{2} successful.", nbtests - nbfails, nbtests, s);

        Console.ResetColor();
    }

    /// <summary>
    ///   Asserts that the specified condition is <see langword="true"/>.
    ///   If not, an <see cref="AssertionException"/> will be thrown.
    /// </summary>
    /// <example>
    ///   // no error:
    ///   Tests.Assert(1 + 1 == 2, "Basic arithmetics should be supported.");
    /// 
    ///   // error:
    ///   Tests.Assert(new int[] { 1, 1 } == new int[] { 1, 1 }, "Arrays should be compared element-wise.");
    /// </example>
    public static void Assert(bool assertion, string message = null)
    {
        if (!assertion)
            throw new AssertionException(message ?? "Assertion failed.") { Frame = new StackFrame(1, true) };
    }

    /// <summary>
    ///   Asserts that the given action will throw an <see cref="Exception"/> of type <see cref="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the exception to catch.</typeparam>
    /// <example>
    ///   // no error:
    ///   Tests.AssertException{ArgumentNullException}(() => throw new ArgumentNullException());
    ///
    ///   // error:
    ///   Tests.AssertException{ArgumentNullException}(() => throw new Exception());
    /// </example>
    public static void AssertException<T>(Action action, string message = null) where T : Exception
    {
        try
        {
            action();

            throw new AssertionException(message ?? "Expected action to fail, but it did not.") { Frame = new StackFrame(1, true) };
        }
        catch (T)
        {
            // Caught the right exception
        }
        catch (Exception e)
        {
            throw new AssertionException(message ?? $"Expected an exception of type {typeof(T).Name}, but got an exception of type {e.GetType().Name}.", e) { Frame = new StackFrame(1, true) };
        }
    }

    /// <summary>
    ///   Asserts that the given action will throw an <see cref="Exception"/>.
    /// </summary>
    /// <example>
    ///   // no error:
    ///   Tests.AssertException(() => 0 / 0);
    /// 
    ///   // error:
    ///   Tests.AssertException(() => { });
    /// </example>
    public static void AssertException(Action action, string message = null)
    {
        try
        {
            action();

            throw new AssertionException(message ?? "Expected action to fail, but it did not.") { Frame = new StackFrame(1, true) };
        }
        catch
        {
            // Caught the right exception
        }
    }
    #endregion
}

#region Classes
/// <summary>
///   Indicates that the marked method will be tested during unit tests.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class TestAttribute : Attribute
{
    /// <summary>
    ///   If <see langword="true" /> and the method fails, all following tests will be skipped,
    ///   and an error will be immediately shown.
    /// </summary>
    public bool IsFatal { get; set; } = false;

    /// <summary>
    ///   If <see langword="true" />, the test will be considered failed unless it throws an exception.
    /// </summary>
    public bool ShouldFail { get; set; } = false;

    /// <summary>
    ///   If <see langword="true"/>, the test will be skipped.
    /// </summary>
    public bool Skip { get; set; } = false;

    /// <summary>
    ///   Gets the list of all the arguments to pass to the function when invoking it.
    /// </summary>
    public object[] Arguments { get; }

    /// <summary>
    ///   Indicates that the marked method will be tested during unit tests, providing the
    ///   specified arguments.
    /// </summary>
    public TestAttribute(params object[] arguments)
    {
        Arguments = arguments;
    }
}

/// <summary>
///   Represents an exception encountered when an assertion proves to be false.
/// </summary>
public sealed class AssertionException : Exception
{
    /// <summary>
    ///   Gets or sets the stack frame of the exception.
    /// </summary>
    public StackFrame Frame { get; set; }

    /// <summary>
    ///   Creates a new <see cref="AssertionException"/>, given an optional message and inner exception.
    /// </summary>
    public AssertionException(string message = null, Exception inner = null) : base(message, inner)
    {
    }

    public override string ToString()
    {
        if (Frame == null)
            return Message;

        string filename = Path.GetFileName(Frame.GetFileName()) ?? "<unknown file>";

        return $"({filename} :: {Frame.GetFileLineNumber()}:{Frame.GetFileColumnNumber()}) {Message}";
    }
}
#endregion
