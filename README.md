# AutreMachine.Common

A collection of several tools useful when creating Blazor projects.

## ServiceResponse
When calling functions, there is always a question lurking : if my result is null, is it normal or is it because there was a problem in the function ?

How can I have the result of the function **and** an indicator on if it worked well (and eventually a well designed error message) ?

ServiceReponse<T> is a generic class that can pass a result with a Success bool (a bool indicating if the result is correct) and a Message string (to indicate the source of the problem in case the call is unsuccessful).

The idea came from Go, where exception does not exist ; everything is carried through errors passed with function calls.

Why not do the same in C# ? This notion is very elegant, and gets rid of lot of *try/catch* not always necessary.

One of the benefits : you can pass a meaningful message from the Business Logic layer and display it to the user.

The class is basically structured this way :

```
public class ServiceResponse<T>
{
    [JsonPropertyName("succeeded")]
    public bool Succeeded { get; set; }
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("content")]
    public T? Content { get; set; } = default(T);
...
```

*Note* : Content holds the result, and can be null (in case of a **Ko** response).

There are 2 helper methods : **Ok** and **Ko** that pass the Success state and the result.

### How can we use it ?

Quite simply, functions will return a ServiceResponse<T> instead of only T class.
And when the caller gets the response, it will test the **Succeeded** field to test the success.

For instance :
```
public ServiceResponse<string> GetLastName(string firstName)
{
    if (firstName == "Joe")
        return ServiceResponse<string>.Ok("Blogo"); // Pass the result to the Ok return function

    return ServiceResponse<string>.Ko($"Sorry, could not find the last name for '{firstName}... did you mean 'Joe' ?");

}
```

You can find more samples here :
[AutreMachine.Common samples](https://github.com/AutreMachine/AutreMachine.Common/tree/master/AutreMachine.Common.Samples)


## APICaller
*(to be completed)*

## PaginatedList
Provides a pagniated list, from a IQueryable data source(ideally a DBSet from a DBContext).
It is very useful to display list of items on Razor pages.
For instance you could use it as following :
```
 IOrderedQueryable<Message>? messages = null;
 var messagesQuery = repository.GetAllQuery().Where(x => x.UserId == userId);

messages = messagesQuery.OrderByDescending(x => x.DateCreated);

var results = await PaginatedList<Message>.CreateAsync(messages, pageNumber ?? 1, pageSize);
...
```


