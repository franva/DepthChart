# DepthChart

## Branches

1. the Main branch is for the simple version which does not support Multi sports and teams.
2. feature/sport-team-support branch adds the functionality for supporting the Multiple sports and teams.

## How to run the code

1. Git clone this repository
2. Open this solution in Visual Studio 2022, make sure you have the latest .NET 8.0 installed,
3. Choose Https in the debug options

![Debug Options](Contents/debugOptions.png)

4. Press F5 to run the project

![Swagger](Contents/swagger.png)

5. First, use the `seedData` endpoint to generate some sample data, then you can start to play around.

![alt text](Contents/seedData.png)

### The project can also run inside Docker (Optional)
1. In the debug options, choose Container(Dockerfile)
2. Make sure you have the Docker Application installed and running
3. Press F5 to run the project

Once the project is launched, go this Swagger UI link
https://localhost:7087/swagger/index.html

Make sure you run the `seedData` endpoint first to generate some data and then you can play around with other endpoints.


## Assumptions

1. I assume that the coding challenge does not require front-end,
2. I assume that the methods mentioned in the document for adding, remove etc. players do not consider the difference sections of the Depth Table, e.g. the Offense, Defense Special and Reserve sections.
3. Positions are all upper case.

## Further Thoughts

We can use Kubernette to scale up and down our micro-services, also using CosmosDB will help in improving availability and geo-duplications.
In production, the in-memory cache can be replaced by Redis if there is a need for this.
