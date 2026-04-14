FROM mcr.microsoft.com/dotnet/sdk:8.0

# disable microsoft telematry
ENV DOTNET_CLI_TELEMETRY_OPTOUT='true'

ENV DynamoDb_LocalMode='true'

ARG LBHPACKAGESTOKEN
ENV LBHPACKAGESTOKEN=$LBHPACKAGESTOKEN

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

RUN dotnet build -c debug HousingRegisterApi.Tests/HousingRegisterApi.Tests.csproj

CMD dotnet test HousingRegisterApi.Tests/HousingRegisterApi.Tests.csproj -c Debug \
    --logger "console;verbosity=minimal" \
    --logger "junit;LogFilePath=/app/test_results/junit.xml" \
    /p:CollectCoverage=true \
    /p:CoverletOutput=/app/coverage/ \
    /p:CoverletOutputFormat=opencover \
    /p:CoverletOutputFile=coverage.opencover.xml
