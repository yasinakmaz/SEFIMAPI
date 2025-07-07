var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.SEFIMAPI>("sefimapi");

builder.Build().Run();
