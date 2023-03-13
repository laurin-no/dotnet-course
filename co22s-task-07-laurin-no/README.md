# Task 07: Blazor WASM

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

- to learn to make a Blazor WebAssembly app
- to learn to use external web api in a Blazor app
- to learn about basic controls in Blazor

## Material for the task

> **Following material will help with the task.**

It is recommended that you will check the material before begin coding.

1. [ASP.NET Core Blazor](https://docs.microsoft.com/en-us/aspnet/core/blazor/?view=aspnetcore-6.0)
2. [Build a Blazor todo list app](https://docs.microsoft.com/en-us/aspnet/core/blazor/tutorials/build-a-blazor-app?view=aspnetcore-6.0&pivots=webassembly)
3. [Call a web API from ASP.NET Core Blazor](https://docs.microsoft.com/en-us/aspnet/core/blazor/call-web-api?view=aspnetcore-6.0&pivots=webassembly)
4. [ASP.NET Core Blazor routing and navigation](https://docs.microsoft.com/en-us/aspnet/core/blazor/fundamentals/routing?view=aspnetcore-6.0)
5. [ASP.NET Core Blazor forms and validation](https://docs.microsoft.com/en-us/aspnet/core/blazor/forms-validation?view=aspnetcore-6.0)

## The Task

Create a Blazor WebAssembly (Blazor WASM) app to handle person and address data. Create UI for all people from the API and UIs for adding new persons, addresses and joining an address to a person. The BlazorWASM template (BlazorPeople), API and data model are given. The API uses Sqlite as database but the data model is simplified with no database relations with different types of data.

Reference the PeopleLib class library project in the Blazor app so you have access to the model classes.

Use the given API endpoints from the Blazor WASM app. The Blazor app handles joining the data so that proper data is shown (i.e. only addresses linked to the person is shown for the person). Use async calls to the API and handle them properly in the client app.

> Note! `HttpClient` is the only configured external dependency that the Blazor app pages can use. Other usable default dependencies are `NavigationManager` and `ILogger<T>`.

`src/BlazorPeople` is the BlazorWASM app. This needs to be edited!
`src/PeopleApi` is the web API. Note that the API is very simplified and its usage requires data handling in the client app.
`src/PeopleLib` is a class library containing the data models used by both the Blazor app and the web API.

> **Note! Remember to start the PeopleApi!** Instructions in src/README.md file.

### Evaluation points

The URLs in the following evaluation points are URLs in the client app. The API URLs must be reasoned from the intended functionality.

1. The app root (Index.razor) (/) loads people from api and displays their first name, last name and title in a table. The table's 4th column contains a link to the selected person's details page (/person/details/[id]), where the [id] is the selected person's id. The page must handle fetching the required data from the API.
2. A page (PersonDetails.razor) to show person details (/person/details/[id]). Shows all of a person's info and all addresses (`Address` class data) related to the selected person with possible contact info's info text (`ContactInfo` class data). The address data is shown in a table element with columns for each of the textual properties in `Address` class and a column for the possible info text. The details page contains also a link to edit page (/person/edit/[id]). The page must handle fetching the required data from the API.
3. A page (PersonEdit.razor) (/person/edit/[id]) to edit the selected person's textual (`string`) data. Use `EditForm` control to edit the data and use HTTP PUT to submit the changes to the API. Add only a single submit button to the page. The submit button should have text value **"Save changes"** (without the quotes). Clicking the submit button causes the data to be submitted to the API. When the edit succeeds then the user is redirected to the details page for the selected person (i.e. the details page is shown). The page must handle fetching the required data from the API.
4. A page (AddressCreate.razor) to create new address data (/address/create). The page contains a form to input data for a new address entity (`Address` class). Use `EditForm` control. The Id property of Address class must not have a field in the form. Use `InputText` or `InputNumber` components for the rest of the Address class' properties. Add the edit fields in the same order as the properties are listed in the Address class. Add a submit button with textual value **"Add new"** (without the quotes) that will submit the properly inputted data when clicked. The data is submitted via HTTP POST request to the API.
5. A page (ContactInfoPage.razor) for joining an address to a person (/contactinfo). In the page there is a selection for an address and a selection for the person and an additional textual field for info text. Use `InputSelect` components for selections and `InputTextArea` for the info text. Add an `id` attribute to the select fields so that the field for selecting address has an id="foraddress" and the field for selecting person has an id="forperson". Add a submit button with textual value **"Join"** (without the quotes) that will submit the properly inputted data when clicked. The controls are in `EditForm` control. The data is saved via POST request to the API. The page must handle fetching the required data from the API.

> Note! Read the task description and the evaluation points to get the task's specification (what is required to make the app complete).

## Task evaluation

Evaluation points for the task are described above. An evaluation point either works or it does not work there is no "it kind of works" step inbetween. Be sure to test your work. All working evaluation points are added to the task total and will count towards the course total. The task is worth five (5) points. Each evaluation point is checked individually and each will provide one (1) point so there is five checkpoints. Checkpoints are designed so that they may require additional code, that is not checked or tested, to function correctly.

## DevOps

There is a DevOps pipeline added to this task. The pipeline will build the solution and run automated tests on it. The pipeline triggers when a commit is pushed to GitHub on main branch. So remember to `git commit` `git push` when you are ready with the task. The automation uses GitHub Actions and some task runners. The automation is in folder named .github.

> **DO NOT modify the contents of .github or test folders.**
