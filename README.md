# AutreMachine.Common

A collection of several tools useful when creating Blazor projects.

## ServiceResponse
ServiceReponse<T> is a generic class that can pass a result with a Success bool (a bool indicating if the result is correct) and a Message string (to indicate the source of the problem in case the call is unsuccessful).
The idea came from Go, where exception does not exist ; everything is carried through errors passed with function calls.
Why not do the same in C# ? This notion is very elegant, and gets rid of lot of *try/catch* not always necessary.



