FROM mcr.microsoft.com/dotnet/core/sdk:3.1

# disable microsoft telematry
ENV DOTNET_CLI_TELEMETRY_OPTOUT='true'

ENV DynamoDb_LocalMode='true'

WORKDIR /app

# Copy csproj and restore as distinct layers
COPY ./HousingRegisterApi.sln ./
COPY ./HousingRegisterApi/HousingRegisterApi.csproj ./HousingRegisterApi/
COPY ./HousingRegisterApi.Tests/HousingRegisterApi.Tests.csproj ./HousingRegisterApi.Tests/

RUN dotnet restore ./HousingRegisterApi/HousingRegisterApi.csproj
RUN dotnet restore ./HousingRegisterApi.Tests/HousingRegisterApi.Tests.csproj

# Copy everything else and build
COPY . .

RUN dotnet build -c debug -o out HousingRegisterApi.Tests/HousingRegisterApi.Tests.csproj

CMD dotnet test
