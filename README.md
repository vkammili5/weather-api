# weather-api

This is a C# solution to the Weather API.

The Base URL is `https://localhost:7230/`

The API has the following endpoints:

| Action  | Endpoint                                | What it does                                                                    | API Documentation |
| ------- | --------------------------------------- | ------------------------------------------------------------------------------- | ----------------- |
| **GET** | `api/v1/weather/{cityName}`             | **Get** today's weather for city: `{cityName}`                                  | [Click Here](#)   |
| **GET** | `api/v1/weather/{latitude}/{longitude}` | **Get** today's weather for geo coordinates near `{latitude}` and `{longitude}` | [Click Here](#)   |

Here we have 3 folders:

1. The `WeatherAPI` folder contains the C# solution to the challenge
2. The `WeatherAPI.Tests` folder contains the unit tests for the solution
3. The `diagrams` folder contains diagrams related to the solution

# Instructions

**Prerequisite**: The machine running the application should have [.NET 6.0](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) (or above), and [MySQL](https://www.mysql.com/) installed.

...

# UML Diagram

![UML Diagram](diagrams/UML/WeatherAPI.png)

# API Documentation

## `Weather` endpoints

## `City` endpoints

### Get All Cities

[[Back To Top]](#weather-api)

#### Request

**GET** `api/v1/city`

#### Request samples

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

### Get City By City Name

[[Back To Top]](#weather-api)

#### Request

**GET** `api/v1/city/{cityName}`

For example `api/v1/city/Manchester`

#### Response samples

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

```
No geocoding found for Manchestersss, please do POST request to /api/v1/city endpoint to add new city.
```

### Add City

[[Back To Top]](#weather-api)

#### Request

**POST** `api/v1/city`

with request body

```json
{
  "name": "London",
  "latitude": 51.50853,
  "longitude": -0.12574
}
```

#### Request samples

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

### Update City

[[Back To Top]](#weather-api)

#### Request

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

#### Request samples

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

### Delete City

[[Back To Top]](#weather-api)

#### Request

**DELETE** `api/v1/city/{cityName}`

For example, `api/v1/city/London`

#### Request samples

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
