# darwin-api-template-dotnet
The API Template is a boilerplate for a .NET 6 API Microservice, to speed up the process of creating a new microservice.

## How to run 

You can run the project using Docker, this project contains a docker-compose.yml file which builds the project based on .net6.0 SDK. 

Run the command below: 

```shell
docker compose up
```

After that the project should be running on [http://localhost:5001/swagger] and the MongoDB Database should be running on [http://localhost:27017].

## Architecture
The API Template project follows the Onion Architecture, keeping the projects split in layers based on their responsibility, keeping it maintainable and testable in both layers, Unit ( without external integration, like DB, to test the logic ), and Integration ( with external integration, to test the full workflow ).

The database used in the application is MongoDB.

The application runs in a docker machine that is automatically started by Visual Studio when debugging.

## Project Structure
![Project Structure](https://github.com/wiley/darwin-api-template-dotnet/blob/master/ProjectStructure.png)

### Authorization.API
This is the most external layer of the application, it is here that we have all the controllers for the Microservice API. This layer is only responsible for routing the API endpoints to services, it should not have any logic except for Authentication ( if necessary ) and Logging.

The DI is applied at this layer, by the Startup class.

### Authorization.Domain
This layer is the base layer of the project, it does not reference any other layer and it is where we have all the domain models of the application.

### Authorization.Infrastructure
This layer is where we have all the external dependencies ( Database for example ). It is here that we define what are the external connections we may need and implement them, like the DB Context. It may have knowledge of the Domain layer to map models to DBs.

### Authorization.Services
This is the business logic layer. All the logic of the application is put inside services that are called by the API layer ( since this is well split, we can test them independently and mocking DB contexts ).

A service can reference another one through DI and Interfaces, so, we can easily maintain and test them independently as well as together by mocking or not the referenced services.

Each service should have a single responsibility and not mix things up, following SOLID principles.




### DB Connection
The MongoDB connection / repository also uses a generic interface / class, IMongoRepository. This interface implements all the methods a mongo repository needs, like, CRUD operations.

To use that, we just need to DI this interface / class into our services, specifying the mongo DB class we will be mapping into.

The domain model should inherit from the interface IDocument and have the properties mapping the collection document. We can use annotations like BsonElement to map the property to the name of the field in the document as well as the BsonCollection for the class to define which collection will be used, and the BsonIgnoreExtraElements so that we don't need to have all the properties from the document mapped into our class ( if we don't add this annotation, we will have an exception in case the Mongo Document have more properties that is not mapped in the Domain Model ).

