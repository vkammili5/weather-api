# weather-api

This is a C# solution to the Weather API.

This API consumes 2 **public APIs**:

1. [Weather data by Open-meteo.com](https://open-meteo.com/) for getting today's weather at a specified coordinate (latitude, longitude)
2. [Geocoding data by Open-meteo.com](https://open-meteo.com/en/docs/geocoding-api) for getting the coordinate (latitude, longitude) at a specified city name.

This API also allows users to specify their own "custom city" (with city `name`, `latitude`, `longitude`) so that the API can recognise new cities not found by the Geocoding public API.

The Base URL of the Weather API is `https://localhost:7123/`, and has the following endpoints:

| Action     | Endpoint                                | What it does                                                                                                    | API Documentation                                |
| ---------- | --------------------------------------- | --------------------------------------------------------------------------------------------------------------- | ------------------------------------------------ |
| **GET**    | `api/v1/weather/{cityName}`             | **Get** today's weather for city: `{cityName}`                                                                  | [Click Here](#get-weather-by-city-name)          |
| **GET**    | `api/v1/weather/{latitude}/{longitude}` | **Get** today's weather for geo coordinates near `{latitude}` and `{longitude}`                                 | [Click Here](#get-weather-by-latitude-longitude) |
| **GET**    | `api/v1/city`                           | **Get** all cities (`name`, `latitude`, `longitude`) in local MySql database                                    | [Click Here](#get-all-cities)                    |
| **GET**    | `api/v1/city/{cityName}`                | **Get** city (`name`, `latitude`, `longitude`) with `name` matching `{cityName}`                                | [Click Here](#get-city-by-city-name)             |
| **POST**   | `api/v1/city`                           | **Create** a new city (`name`, `latitude`, `longitude`) into local MySql database                               | [Click Here](#add-city)                          |
| **PUT**    | `api/v1/city/{cityName}`                | **Update** an existing city (`name`, `latitude`, `longitude`) in local MySql database, matching by `{cityName}` | [Click Here](#update-city)                       |
| **DELETE** | `api/v1/city/{cityName}`                | **Delete** an existing city (`name`, `latitude`, `longitude`) in local MySql database, matching by `{cityName}` | [Click Here](#delete-city)                       |

Here we have 3 folders:

1. The `WeatherAPI` folder contains the C# solution to the API
2. The `WeatherAPI.Tests` folder contains the unit tests for the solution
3. The `diagrams` folder contains diagrams related to the solution

# Instructions

**Prerequisite**: The machine running the application should have [.NET 6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) (or above), and [MySQL](https://www.mysql.com/) installed.

Clone the repository to your computer.

Have MySQL running in the background, using Task Manager or otherwise.

Setup local MySQL server to have user with appropriate privileges. For example, create a new MySQL user with the following commands in command line:

```
mysql -u root

CREATE USER 'cityweatherapi'@'localhost' IDENTIFIED BY 'apiuser123';

GRANT ALL PRIVILEGES ON cityweather.* TO 'cityweatherapi'@'localhost';

exit
```

Then modify the content of `WeatherAPI/appsettings.Development.json` so that it contains the appropriate `ConnectionStrings` to the MySql server with appropriate `user` and `password`.

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "cityweatherapi": "server=localhost; database=cityweather; user=cityweatherapi; password=apiuser123"
  }
}
```

Then navigate to the `WeatherAPI` folder (with cd command or otherwise).

Then execute the **migration commands** to create a new database `cityweather` with a table `Cities` in your MySQL server:

```
dotnet ef database update
```

Then run the application:

```
dotnet run
```

So now you can go to [https://localhost:7123/swagger/index.html](https://localhost:7123/swagger/index.html) to see the available endpoints.

# UML Diagram

![UML Diagram](diagrams/UML/WeatherAPI.png)

# API Documentation

## Get Weather By City Name

[[Back To Top]](#weather-api)

### Request

**GET** `api/v1/weather/{cityName}`

For example `api/v1/weather/Berlin`

### Response samples

Status Code: `200 OK`

Content type: `application/json`

```json
{
  "latitude": 52.52,
  "longitude": 13.419998,
  "weatherCode": "ClearSky",
  "whatToPrepare": "bring coat"
}
```

If `{cityName}` does not match any city name known by the API, then the response status code would be `404 Not Found`, with response body:

```json
{
  "message": "No geocoding found for RandomCity, please do POST request to /api/v1/city endpoint to add new city."
}
```

## Get Weather By Latitude Longitude

[[Back To Top]](#weather-api)

### Request

**GET** `api/v1/weather/{latitude}/{longitude}`

For example `api/v1/weather/52.52/13.41`

### Response samples

Status Code: `200 OK`

Content type: `application/json`

```json
{
  "latitude": 52.52,
  "longitude": 13.419998,
  "weatherCode": "ClearSky",
  "whatToPrepare": "bring coat"
}
```

If `{latitude}/{longitude}` are not in the range known by the API, then the response status code would be `400 Bad Request`, with response body:

```json
{
  "message": "Latitude must be in range of -90 to 90°. Given: 199.0."
}
```

## Get All Cities

[[Back To Top]](#weather-api)

### Request

**GET** `api/v1/city`

### Request samples

Status Code: `200 OK`

Content type: `application/json`

```json
[
  {
    "id": 1,
    "name": "Manchester",
    "latitude": 53.48095,
    "longitude": -2.23743
  },
  {
    "id": 2,
    "name": "Berlin",
    "latitude": 52.52437,
    "longitude": 13.41053
  }
]
```

## Get City By City Name

[[Back To Top]](#weather-api)

### Request

**GET** `api/v1/city/{cityName}`

For example `api/v1/city/Manchester`

### Response samples

Status Code: `200 OK`

Content type: `application/json`

```json
{
  "id": 1,
  "name": "Manchester",
  "latitude": 53.48095,
  "longitude": -2.23743
}
```

If `{cityName}` does not match any city name known by the API, then the response status code would be `404 Not Found`, with response body:

```json
{
  "message": "No geocoding found for RandomCity, please do POST request to /api/v1/city endpoint to add new city."
}
```

## Add City

[[Back To Top]](#weather-api)

### Request

**POST** `api/v1/city`

with request body

```json
{
  "name": "London",
  "latitude": 51.50853,
  "longitude": -0.12574
}
```

### Request samples

Status Code: `201 Created`

Content type: `application/json`

```json
{
  "id": 3,
  "name": "London",
  "latitude": 51.50853,
  "longitude": -0.12574
}
```

If the request body's city has a `name` matching an already existing city in the collection, then the web API cannot create another city with the same `name`, so it'll respond with status code `409 Conflict`.

Status Code: `409 Conflict`

Content type: `application/json`

```json
{
  "message": "City with city name London already exists."
}
```

If the request body's city has a `latitude` or `longitude` outside of their valid ranges:

$$-90 \degree \leq \textrm{latitude} \leq 90 \degree$$

$$-180 \degree \leq \textrm{longitude} \leq 180 \degree$$

then the web API cannot create city with invalid latitude longitude, so it'll respond with status code `400 Bad Request`.

Status Code: `400 Bad Request`

Content type: `application/json`

```json
{
  "message": "City New City should be in range -90 and 90 for latitude & -180 and 180 for longitude "
}
```

## Update City

[[Back To Top]](#weather-api)

### Request

**PUT** `api/v1/city/{cityName}`

For example, `api/v1/city/London`

with request body

```json
{
  "name": "London",
  "latitude": 12.34,
  "longitude": 3.33
}
```

### Request samples

Status Code: `200 OK`

Content type: `application/json`

```json
{
  "id": 3,
  "name": "London",
  "latitude": 12.34,
  "longitude": 3.33
}
```

If the endpoint's `{cityName}` does not match with the request body's city's `name`, then the API will respond with status code `400 Bad Request`.

Status Code: `400 Bad Request`

Content type: `application/json`

```json
{
  "message": "CityName New York should match city.name London"
}
```

If the endpoint's `{cityName}` does not match any city's name in collection, then the API will respond with status code `404 Not Found`.

Status Code: `404 Not Found`

Content type: `application/json`

```json
{
  "message": "CityName stringg not found in collection"
}
```

## Delete City

[[Back To Top]](#weather-api)

### Request

**DELETE** `api/v1/city/{cityName}`

For example, `api/v1/city/London`

### Request samples

Status Code: `204 No Content`

If the response status code is `204 No Content`, then the city with `{cityName}` is deleted

If the endpoint's `{cityName}` does not match with the request body's city's `name`, then the API will respond with status code `404 Not Found`.

Status Code: `404 Not Found`

Content type: `application/json`

```json
{
  "message": "CityName London not found in collection"
}
```

# `/health` endpoint

There's a special endpoint `/health` that shows the health of the application.

You can access it by sending a **GET** request to `https://localhost:7123/health`.

The typical response is:

```json
{
  "status": "Healthy",
  "totalDuration": "00:00:00.0620730",
  "entries": {
    "Open-meteo Forecast Public API": {
      "data": {},
      "duration": "00:00:00.0343575",
      "status": "Healthy",
      "tags": []
    },
    "Open-meteo Geocoding Public API": {
      "data": {},
      "duration": "00:00:00.0419530",
      "status": "Healthy",
      "tags": []
    },
    "City Database": {
      "data": {},
      "duration": "00:00:00.0620093",
      "status": "Healthy",
      "tags": []
    }
  }
}
```

which indicates that:

1. [Open-meteo Forecast Public API](https://open-meteo.com/en) is **healthy** (functional)
2. [Open-meteo Geocoding Public API](https://open-meteo.com/en/docs/geocoding-api) is **healthy** (functional)
3. City Database (MySQL local database) is **healthy** (successfully connected)

If any of the above 3 dependencies isn't healthy, then the `"status"` property will reflect that.

For example, if the MySQL server isn't running, then the response would look like this:

```json
{
  "status": "Unhealthy",
  "totalDuration": "00:00:04.0881357",
  "entries": {
    ...

    "City Database": {
      "data": {},
      "description": "Unable to connect to any of the specified MySQL hosts.",
      "duration": "00:00:04.0828675",
      "exception": "Unable to connect to any of the specified MySQL hosts.",
      "status": "Unhealthy",
      "tags": []
    }
  }
}
```
