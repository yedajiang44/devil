using CommandDotNet;

new AppRunner<AppCommand>().UseDefaultMiddleware().Run(args);
