/*
    Hangman - A hangman game written in C#.

    This could have been a very simple one-method, one-file solution.
    But that is no fun.
    I went overboard and added a lot of features.
    -Dependency Injection using Autofac
    -Command line parsing using System.CommandLine
    -Async/await
    All these features are complete overkill.
    Their purpose is to toy with some design principles.
    On my wishlist: add Docker support.
*/

// NET6.0 so there is no Main. 
// That's really weird, but I'll get used to it. Hopefully?
// Notice the absence of namespaces.
// That feels really strange.
// Everything in my brain screams 'This is missing information'.

using Autofac;
// All we do here is wire up the DI container.
// We'll evaluate the args later on.
// It should be noted that System.CommandLine does offer it's own DI container,
// but I have no idea how to use it with nested dependencies.
var container = ContainerConfiguration.Configure();
using (var scope = container.BeginLifetimeScope())
{
    var app = scope.Resolve<IApplication>();
    await app.RunAsync(args);
    Console.WriteLine("Press any key to exit...");
    Console.Read();
}