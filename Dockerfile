FROM microsoft/dotnet:2.0-sdk AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY ./TrafficDataImportGUI/TrafficDataImportGUI.csproj ./TrafficDataImportGUI/
RUN dotnet restore ./TrafficDataImportGUI/TrafficDataImportGUI.csproj

# Copy everything else and build
COPY . ./
WORKDIR /app/TrafficDataImportGUI
RUN dotnet publish -c Release -o out
WORKDIR ./out
EXPOSE 5577
ENTRYPOINT ["dotnet", "TrafficDataImportGUI.dll"]
# Build runtime image
#FROM microsoft/dotnet:aspnetcore-runtime
#WORKDIR /app
#EXPOSE 5577
#COPY --from=build-env /app/TrafficDataImportGUI/out .
#RUN ls

