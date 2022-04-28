#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/azure-functions/dotnet:4 AS base
WORKDIR /home/site/wwwroot
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["dotnet/FunctionApps/FundooNotesAPI.AzureFunctions.Notes/FundooNotesAPI.AzureFunctions.Notes.csproj", "dotnet/FunctionApps/FundooNotesAPI.AzureFunctions.Notes/"]
COPY ["dotnet/FundooNotesAPI.Shared/FundooNotesAPI.Shared.csproj", "dotnet/FundooNotesAPI.Shared/"]
COPY ["dotnet/FundooNotesAPI.Models/FundooNotesAPI.Models.csproj", "dotnet/FundooNotesAPI.Models/"]
RUN dotnet restore "dotnet/FunctionApps/FundooNotesAPI.AzureFunctions.Notes/FundooNotesAPI.AzureFunctions.Notes.csproj"
COPY . .
WORKDIR "/src/dotnet/FunctionApps/FundooNotesAPI.AzureFunctions.Notes"
RUN dotnet build "FundooNotesAPI.AzureFunctions.Notes.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FundooNotesAPI.AzureFunctions.Notes.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /home/site/wwwroot
COPY --from=publish /app/publish .
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true