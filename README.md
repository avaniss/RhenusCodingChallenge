# Betting API

## Overview

This repository contains an implementation of a **Betting API** which the users can use to play the betting game.

### *Game Rules*
   - This is a game of chance in which a random number between 0 - 9 is to be generated.
   - A player will predict the random number.
   - Upon registering, the player will have a starting account of 10,000 points and can wager them on a prediction which
     they will either win or lose.
   - Any number of points can be wagered.
   - If the player is correct, they win 9 times their stake.
   - If the player is incorrect, they lose their stake.

The application is implemented in C# .net core using [FastEndpoints](https://fast-endpoints.com/) for the REST APIs.

## Prerequisites

1. .NET SDK `v8.0.1`

## Installation

1. Clone the repository: `git clone https://github.com/avaniss/RhenusCodingChallenge.git`
2. Open the solution in visual studio or any other compatible IDE.
3. Build the project.


## Solution architecture

![alt text](./Resources/Architecture.png)


## Dependencies

1. [FastEndpoints](https://fast-endpoints.com/): provides a developer friendly and convenient way of implementing minimal APIs in c# .net using REPR design pattern.
2. [FastEndpoints Security](https://fast-endpoints.com/docs/security#introduction): is used to enable endpoint authentication and authorization.
3. [MediatR](https://github.com/jbogard/MediatR): is used to facilitate synchronous/asynchronous communication between endpoints and commands.
4. [FluentValidation](https://docs.fluentvalidation.net/en/latest/#): is used to add validation rules for user requests.
5. [EntityFramework](https://learn.microsoft.com/en-us/ef/core/): provides a convenient way to work with a database using .net objects.
6. [Moq.EntityFramework](https://github.com/MichalJankowskii/Moq.EntityFrameworkCore): is used to mock database context.
7. [FastEndpoints Testing](https://fast-endpoints.com/docs/integration-unit-testing#integration-testing): provides functions to work with test API server.

## Database

This application supports both `SQL Server` database and `In-Memory [default]` database.
The selection of database type is made configuration via [`UseInMemoryDatabase`](./API/appsettings.Development.json) property.
If it is set to `true`, the in memory database will be used. If it is set to `false`, then the user must define `ConnectionStrings`.

Database consists the following tables:

1. `Users`: Persists user details.
2. `Bets`: Persists bets placed by users. It has a reference to the `User` table.

## Usage/Features

- Run the application from the IDE or via CLI: 
    1. `cd path[path where the repository is stored]`
    2. `dotnet run`

- Upon successful deployment, the swagger UI becomes accessible at [https://localhost:7225/swagger/index.html](https://localhost:7225/swagger/index.html)

- The following API endpoints have been implemented:

    1. `/register`: Users can register themselves by creating new account.
         <details>
          <summary>View steps</summary>

          1. Navigate to https://localhost:7225/swagger/index.html
          2. Expand `User` API endpoint group.
          3. Fill in the details, see example below:
             * Name
             * Username
             * Password
          3. Verify your details and click `Execute`
          

          Sample request:
          
            ```
                curl -X 'POST' \
                  'https://localhost:7225/register' \
                  -H 'accept: */*' \
                  -H 'Content-Type: application/json' \
                  -d '{
                  "name": "Foo Bar",
                  "username": "FooBar",
                  "password": "StrongPass123!"
                }'
            ```

          Sample response:

            ```
                {
                  "username": "FooBar",
                  "password": "StrongPass123!",
                  "name": "Foo Bar",
                  "balance": 10000,
                  "id": 4
                }
            ```
        </details>

    2. `/authenticate`: Users can authenticate themselves to generate an authentication token.
         <details>
          <summary>View steps</summary>

          1. Navigate to https://localhost:7225/swagger/index.html
          2. Expand `Auth` API endpoint group.
          3. Fill in the details, see example below:
             * Username
             * Password
          3. Verify your details and click `Execute`
          4. You can use [jwt.io](https://jwt.io/) to verify the token received in the response.
          

          Sample request:
          
            ```
                curl -X 'POST' \
                  'https://localhost:7225/authenticate' \
                  -H 'accept: */*' \
                  -H 'Content-Type: application/json' \
                  -d '{
                  "username": "FooBar",
                  "password": "StrongPass123!"
                }'
            ```

          Sample response:

            ```
                {
                  "token": "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IkZvb0JhciIsImF1ZCI6IkludGVybmFsIFVzZXJzIiwiaXNzIjoiQXZhbmkgUGF0ZWwiLCJleHAiOjE3MDkzMjM0MDAsImlhdCI6MTcwOTIzNzAwMCwibmJmIjoxNzA5MjM3MDAwfQ.e1vN0GLW5VZYr9hVoXqWzZ1atd4EgpwOdhxy2uxj4Ag",
                  "expiry": "2024-03-01T20:03:20.7621558Z"
                }
            ```
        </details>

    3. `/bet`: Users can place bet using this endpoint and in response they will receive the results of the bet.
    User identification is important to place a bet and hence, this endpoint is only accessible with a valid token that was generated using `/authenticate` endpoint.
         <details>
          <summary>View steps</summary>

          1. Navigate to https://localhost:7225/swagger/index.html
          2. Expand `Bet` API endpoint group.
          3. Fill in the details, see example below:
             * Number
             * Points
          3. Verify your details and click `Execute`
          

          Sample request:
          
            ```
                curl -X 'POST' \
                  'https://localhost:7225/bet' \
                  -H 'accept: */*' \
                  -H 'Authorization: Bearer eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6InJvb3QiLCJhdWQiOiJJbnRlcm5hbCBVc2VycyIsImlzcyI6IkF2YW5pIFBhdGVsIiwiZXhwIjoxNzA5MzIxODA3LCJpYXQiOjE3MDkyMzU0MDcsIm5iZiI6MTcwOTIzNTQwN30.zjEdorkwrWQjGRHMhhcrKyInf2yOykYQdwYIzIMAaS0' \
                  -H 'Content-Type: application/json' \
                  -d '{
                  "number": 1,
                  "points": 100
                }'
            ```

          Sample response:

            ```
                {
                  "balance": 9900,
                  "status": "LOST",
                  "points": "-100"
                }
            ```
        </details>
## Test

This application uses MSTest plugin to add the test cases. All the command and endpoint classes are fully unit tested.

## Improvements

- The current version of the API implementation provides only a single endpoint for betting, more features and endpoints can be added as enhancements in the future:

1. **Performance monitoring**:

   We have `PerformanceBehaviour` class that adds a warning log message when an endpoint takes more than `500 ms` to execute. 
   In future, this implementation can be extended to push performance metric to some Time-Series database like `Prometheus` and build dashboards using it to 
   have alerts automatically sent when an API doesn't meet its Key Performance Indicators(KPI).

2. **Code coverage**:

   For the initial implementation, keeping in mind the availability of time in hand, 
   unit tests are written only for the most ciritcal part of the application.

   In future, code coverage should ideally remain in around 95% if not 100% for the entire codebase.
   And unit tests must also be backed by additional testing methods like, integration tests, user acceptance tests, 
   performance & load tests, etc.

3. **Code symentics & design**:
    
    This is an area of improvement that never goes out of focus. As more can contribute to this codebase
    in future, it is highly probable that the code becomes messy and doesn't follow a common programming practise.

    To address this, additional efforts must be put in writing a contribution guide listing out 
    a set of rules that must be followed for contributing to this codebase.

4. **Continous Integration,Improvement / Continous Deployment, Delivery**:

    Today many software engineering teams follow agile practises. It enables engineers to 
    adapt to changes and deliver at a faster pace. And in this competitive world, deliverying a 
    first is of atmost importance. 

    In future, there must be a CI/CD pipeline to build, test and deploy application at each iteration.
    To be able to run automated tests and to ensure that the quality of code remains intact.

    Without having a proper CI/CD pipelines, an application would definitely struggle in delivering continous releases.

## Summary

There were several new things learned while working on this application. And several other that needs 
to be learned. With a mindset of continous learning, I see following next steps:

 1. Write code contribution guide & define conding standards.
 2. Code Coverage -> bring it close to 95%, and then continously improve with every new change. 
 3. CI/CD pipelint having build/test/release steps.
 4. Learning about how to publish performance numbers of an API to some database like Prometheus, 
 and creating dashboads using tools like (Grafana).
 5. Learning about setting up an alerting system that notifies engineers when the application malfunctions.

## Contact

- Author Name: Avani Patel
- Email: avani.shah1807@gmail.com

Feel free to reach out if you have any questions or suggestions!
