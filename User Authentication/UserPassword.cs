namespace UserAuthentication.cs
{
    public class UserPassword : ILogin, IRegister // Class for using passwords
    {
        private Dictionary<string, string> _userCredentials; // Dictionary to store the user credentials
        private readonly string _filePath = "credentials.txt"; // File path for the user credentials

        public UserPassword() // Constructor for the UserPassword class
        {
            _userCredentials = new Dictionary<string, string>(); // Initialize the dictionary
            LoadCredentials(); // Load the user credentials from the file
        }

        private void LoadCredentials() // Method to load the user credentials from the file
        {
            try
            {
                // Create file if it doesn't exist
                if (!File.Exists(_filePath)) // Check if the file exists
                {
                    File.Create(_filePath).Close(); // Create the file
                    return;
                }

                // Read all lines from file
                string[] lines = File.ReadAllLines(_filePath); // Read all lines from the file
                foreach (string line in lines) // Loop through each line
                {
                    string[] parts = line.Split(','); // Split the line into parts
                    if (parts.Length == 2) // Check if the line has two parts
                    {
                        _userCredentials[parts[0]] = parts[1]; // Add the username and password to the dictionary
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading credentials: {ex.Message}"); // Log the error
            }
        }

        private void SaveCredentials() // Method to save the user credentials to the file
        {
            try
            {
                // Convert dictionary to lines of text
                List<string> lines = _userCredentials // Convert the dictionary to a list of strings
                    .Select(kvp => $"{kvp.Key},{kvp.Value}") // Convert each key-value pair to a string
                    .ToList(); // Convert the list of strings to a list

                // Write all lines to file
                File.WriteAllLines(_filePath, lines); // Write all lines to the file
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving credentials: {ex.Message}"); // Log the error
            }
        }

        private (string username, string password) GetUserInput() // Method to get the user input
        {
            Console.WriteLine("Enter username: "); // Log the username input
            string? username = Console.ReadLine(); // Get the username input
            
            Console.WriteLine("Enter password: "); // Log the password input
            string? password = Console.ReadLine(); // Get the password input

            return (username ?? string.Empty, password ?? string.Empty); // Return the username and password
        }

        public bool Register() // Method to register the user
        {
            var (username, password) = GetUserInput(); // Get the username and password

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password)) // Check if the username or password is empty
            {
                Console.WriteLine("Username and password cannot be empty"); // Log the empty username or password
                return false; // Return false to indicate the registration failed
            }

            if (_userCredentials.ContainsKey(username)) // Check if the username exists
            {
                Console.WriteLine("Username already exists"); // Log the username already exists
                return false; // Return false to indicate the registration failed
            }

            _userCredentials.Add(username, password); // Add the new user to the dictionary
            SaveCredentials(); // Save the user credentials to the file
            Console.WriteLine("Registration successful"); // Log the successful registration
            return true; // Return true to indicate the registration was successful
        }

        public bool Login() // Method to login the user
        {
            var (username, password) = GetUserInput(); // Get the username and password

            if (_userCredentials.ContainsKey(username)) // Check if the username exists
            {
                if (password == _userCredentials[username])
                {
                    Console.WriteLine("Login successful"); // Log the successful login
                    return true; // Return true to indicate the login was successful
                }
                Console.WriteLine("Invalid password"); // Log the invalid password
                return false; // Return false to indicate the login failed
            }
            Console.WriteLine("Invalid username"); // Log the invalid username
            return false; // Return false to indicate the login failed
        }
    }
}