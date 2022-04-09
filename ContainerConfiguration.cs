using Autofac;
internal class ContainerConfiguration
{
    public static IContainer Configure()
    {
        var builder = new ContainerBuilder();
        builder.RegisterType<GameOptions>()
            .As<IGameOptions>()
            .SingleInstance();
        builder.RegisterType<Game>()
            .As<IGame>();
        builder.RegisterType<Application>()
            .As<IApplication>();
        return builder.Build();
    }
}