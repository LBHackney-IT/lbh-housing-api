FROM mcr.microsoft.com/dotnet/sdk:3.1

# disable microsoft telematry
ENV DOTNET_CLI_TELEMETRY_OPTOUT='true'

ARG LBHPACKAGESTOKEN
ENV LBHPACKAGESTOKEN=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

WORKDIR /app

# Copy csproj and nuget config and restore as distinct layers
COPY ./HousingRegisterApi.sln ./
COPY ./HousingRegisterApi/HousingRegisterApi.csproj ./HousingRegisterApi/
COPY ./HousingRegisterApi.Tests/HousingRegisterApi.Tests.csproj ./HousingRegisterApi.Tests/
COPY /NuGet.config /root/.nuget/NuGet/NuGet.Config

RUN dotnet restore ./HousingRegisterApi/HousingRegisterApi.csproj
RUN dotnet restore ./HousingRegisterApi.Tests/HousingRegisterApi.Tests.csproj

# Copy everything else and build
COPY . .

RUN dotnet build -c Release -o out ./HousingRegisterApi/HousingRegisterApi.csproj
RUN dotnet publish -c Release -o out ./HousingRegisterApi/HousingRegisterApi.csproj

EXPOSE ${PORT:-5000}
CMD ASPNETCORE_URLS=http://+:${PORT:-5000} dotnet ./out/HousingRegisterApi.dll
