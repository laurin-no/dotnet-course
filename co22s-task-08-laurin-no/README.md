# Task 08: SignalR

<img alt="points bar" align="right" height="36" src="../../blob/badges/.github/badges/points-bar.svg" />

![GitHub Classroom Workflow](../../workflows/GitHub%20Classroom%20Workflow/badge.svg?branch=main)

***

## Student info

> Write your name, your estimation on how many points you will get and an estimate on how long it took to make the answer

- Student name: Laurin Novak
- Estimated points: 5
- Estimated time (hours): 4

***

## Purpose of this task

The purposes of this task are:

- to learn to create real-time apps
- to learn to use SignalR
- to learn to create a Blazor client app for SignalR communication

## Material for the task

> **Following material will help with the task.**

It is recommended that you will check the material before begin coding.

1. [Overview of ASP.NET Core SignalR](https://docs.microsoft.com/en-gb/aspnet/core/signalr/introduction?view=aspnetcore-6.0)
2. [Use ASP.NET Core SignalR with Blazor](https://docs.microsoft.com/en-gb/aspnet/core/blazor/tutorials/signalr-blazor?view=aspnetcore-6.0&tabs=visual-studio&pivots=webassembly)
3. [Built-in form components](https://docs.microsoft.com/en-us/aspnet/core/blazor/forms-validation?view=aspnetcore-6.0#built-in-form-components)

## The Task

Create a SignalR application with Blazor WebAssembly client. The template with some initial data is given in BlazorSignalRApp. Your task is to complete the app. The app has two SignalR hubs. One for chat messages and one for weather observation data. Some of the needed model classes are given and some needs to be implemented. The app also has a web api controller for reading the saved data from the in-memory database.

The application uses in-memory database for storing the chat messages and the weather observations. The database context and configuration is allready done.

What is given:

- ChatMessage class
- ChatMessageNotification class
- WeatherForecast class
- HubUrls class
- Chat.razor file, which
  - has the div id="messagelist"
  - has the url
- Weather.razor file
- In-memory partial data context (AppDataContext) for chat messages and weather observations

What needs to be done (and this is a long long list...) is described in detail in the following five steps in the evaluation points chapter. In short you will need to implement an endpoint to the web api, SignalR hubs to the server, user interface for the hubs as Blazor pages in the client and complete the data context class.

> Note! The src/BlazorSignalRApp folder contains a hosted BlazorWASM app. To run the whole app run the Server project. That will start both the server and the client. More info in src/README.md file.

### Evaluation points

1. Create a hub named `ChatHub` for chat messages. When a message is submitted to ChatHub it is saved to db and then transmitted to **all** clients.

    The ChatHub:

    - Has a method named `PostMessage` which takes only one parameter of type `ChatMessage` as input
    - The incomming message is saved to in-memory database as type `ChatMessageNotification`
    - The PostMessage method transmits the received message to all clients as type `ChatMessageNotification` by calling `MessageNotification` method on clients.
    - Is in `BlazorSignalRApp.Server.Hubs` namespace
    - Has only one constructor, which takes the AppDataContext as a parameter
      ```csharp
        public ChatHub(AppDataContext db)
        {
          // ...
        }
      ```
    - Is accessible from URI: `/chatter`

2. Create a client for the ChatHub to the Client app as Blazor page named `Chat.razor`.

    The Chat page:

    - Has url `/chat` (i.e. the page is accessible from url /chat)
    - Has inputs for user's name (id="username") and the message (id="themessage"). Use the given id values for the input fields.
    - Has only one button to submit the message. Use id="submitthemessage" for the button. Clicking the button sends the message via SignalR to the ChatHub.
    - Lists received messages as div elements with css classes: alert alert-success messagenotification. The inner div element contains the message's MessageTime, User and Message properties' values. The div elements are listed in a div element with id="messagelist".
    - Use value from injected HubUrls class' ChatHubUrl property as the hub's url

3. Add a DbSet property to `AppDataContext` class in folder Server/Data to support saving weather observations. Use the name `WeatherObservations` for the DbSet property. Complete the implementation of web api endpoints for app data on Server/Controllers/AppDataController class.

    The WeatherObservation class:

    - Create a weather observation class named `WeatherObservation`, which:
       - Inherits WeatherForecast class
       - Is in namespace `BlazorSignalRApp.Shared`
       - Has additional properties:
         - Guid Id (i.e. property name is Id and its type is Guid)
         - string? ObservationText

4. Create a hub named `WeatherHub` for weather observations. When an observation is submitted to WeatherHub it is saved to db and then transmitted to all **other** clients.

    The WeatherHub:

    - Has a method named `SubmitObservation` to submit weather observations as `WeatherObservation` class instance
    - The incomming observation is save to in-memory database
    - The SubmitObservation method transmits the observation to all other clients as `WeatherForecast` class instance by calling `Forecast` method on the clients.
    - Is in `BlazorSignalRApp.Server.Hubs` namespace
    - Has only one constructor, which takes the AppDataContext as a parameter
      ```csharp
        public WeatherHub(AppDataContext db)
        {
          // ...
        }
      ```
    - Is accessible from URI: `/observations`

5. Create a client for the WeatherHub to the Client app as Blazor page named `Weather.razor`.

    The Weather page:

    - Has url `/weather`
    - Has inputs for:
      - Date, use InputDate control
      - TemperatureC, use InputNumber control
      - Summary, use InputText control with attribute id="summary"
      - ObservationText, use InputText control with attribute id="observationtext"
    - Has only one button to submit the observation (id="submit"). Clicking the button sends the message via SignalR to the WeatherHub.
    - Lists all received forecasts as rows in a table element where WeatherForecast class' properties' Date, TemperatureC, TemperatureF and Summary values are in their own columns

> Note! Read the task description and the evaluation points to get the task's specification (what is required to make the app complete).

## Task evaluation

Evaluation points for the task are described above. An evaluation point either works or it does not work there is no "it kind of works" step inbetween. Be sure to test your work. All working evaluation points are added to the task total and will count towards the course total. The task is worth five (5) points. Each evaluation point is checked individually and each will provide one (1) point so there is five checkpoints. Checkpoints are designed so that they may require additional code, that is not checked or tested, to function correctly.

## DevOps

There is a DevOps pipeline added to this task. The pipeline will build the solution and run automated tests on it. The pipeline triggers when a commit is pushed to GitHub on main branch. So remember to `git commit` `git push` when you are ready with the task. The automation uses GitHub Actions and some task runners. The automation is in folder named .github.

> **DO NOT modify the contents of .github or test folders.**
