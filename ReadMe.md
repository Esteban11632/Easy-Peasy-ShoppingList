# Instructions to run the code

First of all, if you have GitHub Desktop installed, it makes the process a lot easier, but it is not required to run the code.

Before running the code, make sure you have Git installed on your machine. And have .NET SDK installed on your machine.

To Check if you have Git installed, you can go to your terminal and run the following command:

```
git --version
```

To Check if you have .NET SDK installed, you can go to your terminal and run the following command:

```
dotnet --version
```

To download .NET SDK, you can go to this link: https://dotnet.microsoft.com/en-us/download

Make sure to download the latest version.

After having everything installed, to run the application, follow these steps:

Its also very important to note that if the application GitHub Desktop is installed, it makes the process a lot easier.

1. **Clone the Repository**:
   Clone the repository to your local machine using the following command:

   ```
   git clone https://github.com/Esteban11632/Easy-Peasy-ShoppingList
   ```

2. **Navigate to the Project Directory**:
   Change your directory to the project folder:

   ```
   cd Easy-Peasy-ShoppingList
   ```

3. **Install Dependencies**:
   Make sure you have the .NET SDK installed. Run the following command to restore the project dependencies:

   ```
   dotnet restore
   ```

4. **Update the Database**:
   Run the following command to update / connect to the database:

   ```
   dotnet ef database update
   ```

5. **Build the Project**:
   Build the project to ensure everything is set up correctly:

   ```
   dotnet build
   ```

6. **Run the Application**:
   Start the application using the following command:

   ```
   dotnet run
   ```

7. **Access the Application**:
   Open your web browser and navigate to `http://localhost:5000`

Lastly, because the buttons to redirect the user to another page are not working, the main way you can change between pages is manually copy the URL and paste it into your browser.

This is the URL for the login page: `http://localhost:5000/login`

This is the URL for the register page: `http://localhost:5000/register`

Also to note, these last two pages cannot be accessed unless you are logged in.

This is the URL for the shopping list page: `http://localhost:5000/shopping-list`

This is the URL for the task manager page: `http://localhost:5000/task-manager`
