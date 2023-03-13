# Task 09: ASP.NET MVC app with Authentication

<img alt="points bar" align="right" height="36" src="../../blob/badges/.github/badges/points-bar.svg" />

![GitHub Classroom Workflow](../../workflows/GitHub%20Classroom%20Workflow/badge.svg?branch=main)

***

## Student info

> Write your name, your estimation on how many points you will get and an estimate on how long it took to make the answer

- Student name: Laurin Novak
- Estimated points: 20
- Estimated time (hours): 10

***

## Purpose of this task

The purposes of this task are:

- to learn basic authentication scenario with ASP.NET MVC app
- to learn about Areas in ASP.NET MVC app
- to handle logged in user's data properly
- to learn about ASP.NET Individual User Account authentication
- to learn about Authentication (who the user is) and Authorization (what the user is allowed to do)

## Material for the task

> **Following material will help with the task.**

It is recommended that you will check the material before begin coding.

1. [Get started with ASP.NET Core MVC](https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-mvc-app/start-mvc?view=aspnetcore-6.0)
2. [ASP.NET Core security topics](https://docs.microsoft.com/en-us/aspnet/core/security)
3. [Overview of ASP.NET Core authentication](https://docs.microsoft.com/en-us/aspnet/core/security/authentication)
4. [Articles based on ASP.NET Core projects created with individual user accounts](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/individual)
5. [Introduction to authorization in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/security/authorization/introduction)
6. [Role-based authorization in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/security/authorization/roles)
7. [Identity model customization in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/customize-identity-model)
8. [Areas in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/areas)
9. [Creating and configuring a model](https://docs.microsoft.com/en-us/ef/core/modeling/)

## The Task

Create an ASP.NET MVC web application that uses Individual user accounts (i.e. the user accounts are handled and persisted in the app's database). The src folder contains a project named DeviceManager that was created with dotnet CLI command `dotnet new mvc -au Individual`. The created template has only been changed to enable testing and to show the three (3) users in the database at Home/Index view. The app.db database contains the three users and the required roles for the app. The DB also has some devices linked to the users.

Complete the app to support user roles for normal users and admin users. The normal users has access only to the user's own devices and can add new, edit, list and delete those. The users in admin role has access to all user accounts and to all devices from all users. The admin can list a selected user's devices, add new, edit and delete devices from the user.

The admin users' functionality is separated in an Area named `Administration` and must be accessible from url https://localhost:<port>/administration. The normal users functionality is implemented without areas.

Implement the normal users' functionality so that there is no way for a user to be able to access someone else's devices. All actions for normal users must have code that prevents access to other user's data.

Implement admin users' functionality so that only users in the role `Admins` can access that functionality. All of the code must use properly configured `[Authorize]` attributes to allow or deny access.

Implement `AppUser` class in `DeviceManager.Data` namespace in Data folder. That class must be configured to be used as the default identity type instead of the template's default IdentityUser class. The app user has an additional property named `Devices` for the user's devices. Use proper type for the property. Other properties are inherited from IdentityUser class. Change the template's ApplicationDbContext class to use the created AppUser class as the IdentityDbContext type.

Implement `Device` class in `DeviceManager.Data` namespace in Data folder. The Device class contains all information about a device and models the Devices table in the db. The Device class also has a virtual property to the AppUser class (i.e. the user object who's device the class' instance is). Add the devices table to the ApplicationDbContext as a DbSet named `Devices` and model the Device entity so that the database's restrictions for data fields and relations are implemented. Model the restrictions with attributes.

The Device class could be used as a view model for the views that interacts with device data but when the Device class described above is properly configured for the database then it's usage as a model class for views is difficult. It is recommended to create another class to be used as a view model for the device data and handle data mapping between classes in the controller code. So create a `DeviceViewModel` class in `DeviceManager.Models` namespace in Models folder. Implement the properties that are needed to handle all CRUD operations on a device. Use the same property names for the implemented properties as the Device class has.

The Devices table is modelled from the following T-SQL (a MS SQL flavor of SQL-language) and transformed to fit SQLite database.

```sql
CREATE TABLE [Devices] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [Name] nvarchar(50) NOT NULL,
    [Description] nvarchar(max),
    [DateAdded] datetime(7),
    CONSTRAINT [PK_Devices] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Devices_AspNetUsers_Id] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
```

### Evaluation points

1. The Device class is properly implemented as described in The Task chapter.
2. ApplicationDbContext class with necessary other classes are properly implemented as described in The Task chapter.
3. Administration area
   
    All the content in the Administration area is accessible only to users who are in Admins role. The area's main page (/administration) must be handled in the area's HomeController's Index action method. The rendered view contains a table element with all the users (AppUser class instances) in the application's database. The table has columns for a user's id and email fields and a column for link element (a) to the user's devices in the area (i.e. the Index action of UserDevicesController). Use Authorize attribute with proper parameters to all controller classes in the Administration area.

    The admin's controls for a user's devices is modelled in `UserDevicesController`. The controller has Index, Details, Create, Edit and Delete actions for the selected user's devices. The class is in namespace `DeviceManager.Areas.Admin.Controllers`. The selected user's id is passed to action methods in query parameter named userId (i.e. the url ../userdevices/details/X?userId=Y must render user's with id  Y device's with id X details).

    **Index**: lists the selected user's devices in a table element. The table has header row and columns for Id, UserId, Name, Description and DateAdded values and a column for Details, Edit and Delete action links to the device.

    **Details**: shows the selected user's selected device's data with links to Edit and Delete actions for the device.

    **Create**: renders a html form to create a new device. Only Name and Description property values can be set by the admin user on the form. Other values are set by the controller action (i.e. by the server).

    **Edit**: renders a html form to edit the selected device. The form must have a input field for all of the device's properties but only Name and Description should be editable by the user. Use html `readonly` attribute to other inputs to indicate that the user should not edit those.

    **Delete**: shows the selected user's selected device's data. Renders a html form and submit button to confirm the deletion. Clicking the submit button posts (HTML POST) the form and the server handles the device deletion. The form must contain hidden input fields for the device id (Id) and the user id (UserId).

4. Normal user's functionality

    Implement `DevicesController` class that has all the functionality for a normal user. The class is in namespace `DeviceManager.Controllers`. Implement the class so that anonymous users cannot access any of the action methods. Use properly configured Authorize attribute with the DevicesController class. Url https://localhost:<port>/devices must be handled by Index action method in the DeviceController class.

    **Index**: lists the loggedin user's devices in a table element. The table has header row and columns for Id, UserId, Name, Description and DateAdded values and a column for Details, Edit and Delete action links to the device.

    **Details**: shows the loggedin user's selected device's data with links to Edit and Delete actions for the device. Must be accessible from url ../devices/details/[id] where [id] is the selected device's id. Must only show the details if the device with id is actually the loggedin user's device.

    **Create**: renders a html form to create a new device. Only Name and Description property values can be set by the user on the form. Other values are set by the controller action (i.e. by the server). Note! the device's owner is also set by the action method in the server.

    **Edit**: renders a html form to edit the selected device. The form must have a input field for all of the device's properties but only Name and Description should be editable by the user. Use html `readonly` attribute to other inputs to indicate that the user should not edit those.

    **Delete**: shows the loggedin user's selected device's data. Renders a html form and submit button to confirm the deletion. Clicking the submit button posts (HTML POST) the form and the server handles the device deletion.

    The normal user must not be able to access any of the functionality for Admins.

5. Cases
    
    Case 1: 
    A device with id D belongs to a user with id X. A normal user with id Y logs in to the application and types the url https://localhost:<port>/devices/details/D (i.e. tries to access to a device belonging to another user). This must not be allowed. 

    Case 2:
    A device with id D belongs to a user with id X. A normal user with id Z logs in to the application and edits the form content to make a HTTP POST request to the url https://localhost:<port>/devices/delete/D (i.e. tries to POST a delete request to a device belonging to another user). This must not be allowed.

    The app must not cause 500 errors in any of these cases (InternalServerError).

    In all cases where a user tries to CRUD (Create, Read, Update or Delete) another user's device. The action must not be allowed to complete (i.e. the data is not shown to the wrong user and changes are not persisted to the database). The fraudulent user must instead be redirected to the Error action on the HomeController.

> Note! Read the task description and the evaluation points to get the task's specification (what is required to make the app complete).

## Task evaluation

Evaluation points for the task are described above. An evaluation point either works or it does not work there is no "it kind of works" step inbetween. Be sure to test your work. All working evaluation points are added to the task total and will count towards the course total. The task is worth twenty (20) points. Each evaluation point is checked individually and each will provide four (4) points so there is five checkpoints. Checkpoints are designed so that they may require additional code, that is not checked or tested, to function correctly.

## DevOps

There is a DevOps pipeline added to this task. The pipeline will build the solution and run automated tests on it. The pipeline triggers when a commit is pushed to GitHub on main branch. So remember to `git commit` `git push` when you are ready with the task. The automation uses GitHub Actions and some task runners. The automation is in folder named .github.

> **DO NOT modify the contents of .github or test folders.**
