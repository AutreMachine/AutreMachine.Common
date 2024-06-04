﻿using AutreMachine.Common;
using AutreMachine.Common.Samples.APICaller;
using AutreMachine.Common.Samples.ServiceReponse;

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
/*var messages = new AIMessage[2]
            {
                new AIMessage {role="system", content="You are an helpful assistant who tries to extract every technical skill and every soft skill required in the prompts.You will output a JSON file with a Technical_skills list and a Soft_skills list."},
                new AIMessage {role="user", content="J'ai de nombreuses qualités"}
            };

var answer = await apiCallerLocalTest.AskQuestion(messages, 0);
if (answer.Succeeded && answer.Content != null)
    Console.WriteLine($"Success : {answer.Content}");
else
    Console.WriteLine($"Error : {answer.Message}");
*/

// Test on a local server
Console.WriteLine("\nLocal API\n-----------");
var joe = await apiCallerLocalTest.AnswerName("Joe");
if (joe.Succeeded && joe.Content != null)
    Console.WriteLine($"Success : {joe.Content}");
else
    Console.WriteLine($"Error : {joe.Message}");
