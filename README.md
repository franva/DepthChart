# DepthChart

## How to run the code

1. Git clone this repository
2. Open this solution in Visual Studio 2022, make sure you have the latest .NET 8.0 installed,
3. Choose Https in the debug options

![Debug Options](Contents/image.png)

4. Press F5 to run the project

### You can also run the project inside Docker (Optional)
1. In the debug options, choose Container(Dockerfile)
2. Make sure you have the Docker Application installed and running
3. Press F5 to run the project

Once the project is launched, go this Swagger UI link
https://localhost:7087/swagger/index.html

Make sure you run the `seedData` endpoint first to generate some data and then you can play around with other endpoints.


## Assumptions

1. I assume that the coding challenge does not require front-end,
2. I assume that the methods mentioned in the document for adding, remove etc. players do not consider the difference sections of the Depth Table, e.g. the Offense, Defense Special and Reserve sections.
