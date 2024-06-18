using AutreMachine.Common;
using AutreMachine.Common.Samples.PaginatedList;
using AutreMachine.Common.Samples.ServiceReponse;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using static System.Net.WebRequestMethods;
using static System.Runtime.InteropServices.JavaScript.JSType;

// ---------------
// ServiceResponse
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
var http = new HttpClient();
http.BaseAddress = new Uri("https://www.google.com");
var respGoogle = await APICaller<string>.Get(http, "/");
if (respGoogle.Succeeded && respGoogle.Content != null)
    Console.WriteLine($"Success : {respGoogle.Content}");
else
    Console.WriteLine($"Error : {respGoogle.Message}");

Console.WriteLine("\nLM Studio\n-----------");
var aiChain = new AIChain();
var resp = await aiChain.AskQuestion<string>(new AIMessage[] { new AIMessage { role = "user", content = "combien font 2+2 ?" } }, 0);
if (resp.Succeeded && resp.Content != null)
    Console.WriteLine($"Success : {resp.Content}");
else
    Console.WriteLine($"Error : {resp.Message}");


// PaginatedList
// -------------
Console.WriteLine("\nPaginatedList\n-----------");
var elementsQuery = from element in new TestElementContext(500) select element;
var indexPage = 3;
var pageSize = 15;
var page = PaginatedList<TestElement>.Create(elementsQuery, indexPage, pageSize);
Console.WriteLine($"Displaying page {indexPage} of size {pageSize} :");
foreach (var element in page)
    Console.WriteLine("- " + element.Name);

// PaginatedView
// -------------
// Difference 
Console.WriteLine("\nPaginated View\n-----------");
var pageVM = PaginatedVM<TestElement>.Create(elementsQuery, indexPage, pageSize);
Console.WriteLine($"Displaying page {indexPage} of size {pageSize} :");
foreach (var element in page)
    Console.WriteLine("- " + element.Name);
Console.WriteLine($"Total number of pages : {pageVM.TotalPages}");



return;
