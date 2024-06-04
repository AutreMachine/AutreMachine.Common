using AutreMachine.Common;
using AutreMachine.Common.Samples.APICaller;
using AutreMachine.Common.Samples.ServiceReponse;
using System.Text.Json;
using System.Text.Json.Serialization;

// ---------------
// ServiceRepsonse
// ---------------

var test = new TestClass();

// Let's test a simple Scalar result
// ---------------------------------
Console.WriteLine("Scalar\n-----------");
var res1 = test.GetLastName("Joe");
if (res1.Succeeded)
    Console.WriteLine($"Success : last name is {res1.Content}");
else
    Console.WriteLine($"Error : {res1.Message}");

Console.WriteLine();
res1 = test.GetLastName("Jimmy");
if (res1.Succeeded)
    Console.WriteLine($"Success : last name is {res1.Content}");
else
    Console.WriteLine($"Error : {res1.Message}");

// Now for something a bit more complicated
// ----------------------------------------
Console.WriteLine("\nClass\n-----------");
var res2 = await test.GetSimpleReturnClass("1234");
if (res2.Succeeded && res2.Content != null)
    Console.WriteLine($"Success : location is '{res2.Content.Location}'");
else
    Console.WriteLine($"Error : {res2.Message}");

// And now again more complexity
// -----------------------------
Console.WriteLine("\nTuple\n-----------");
var userIds = new List<string> { "1234", "0000", "7898" };
var res3 = await test.GetAllUsers(userIds);
if (res3.Succeeded)
{
    Console.WriteLine("Users found :");
    foreach (var user in res3.Content.results)
        Console.WriteLine($"User #{user.UserId} : {user.Location}");

    Console.WriteLine();
    Console.WriteLine("Errors found :");
    foreach (var error in res3.Content.errors)
        Console.WriteLine($"  - {error}");
}
else
    Console.WriteLine($"Error : {res3.Message}");


// APICALLER
// ---------
Console.WriteLine("\nAPICaller\n-----------");

Console.WriteLine("\nLM Studio\n-----------");
var apiCallerLocalTest = new APICallerLocalTest();
var resp = await apiCallerLocalTest.AskQuestion(new AIMessage[] { new AIMessage { role = "user", content = "combien font 2+2 ?" } }, 0);
if (resp.Succeeded && resp.Content != null)
    Console.WriteLine($"Success : {resp.Content}");
else
    Console.WriteLine($"Error : {resp.Message}");

// Test on a local server
Console.WriteLine("\nLocal API\n-----------");
var joe = await apiCallerLocalTest.AnswerName("Joe");
if (joe.Succeeded && joe.Content != null)
    Console.WriteLine($"Success : {joe.Content}");
else
    Console.WriteLine($"Error : {joe.Message}");

// Test on a local server
Console.WriteLine("\nLocal API 2\n-----------");
var ask = await apiCallerLocalTest.AnswerClass(new AskClass { first="joe", last = "blogo", age = 42});
if (joe.Succeeded && joe.Content != null)
    Console.WriteLine($"Success : {JsonSerializer.Serialize(ask.Content)}");
else
    Console.WriteLine($"Error : {ask.Message}");

return;