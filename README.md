# PasswordChecker

# Task 1 (Required)
1.	Create a Web API that calculates and returns the password strength. To calculate the password strength, feel free to suggest the best way to do it and return a value to be used by the application.
2.	Create a console app to interact with your Web API. Create a console app that allows entering a password and displays the password strength by calling the service described above.
3.	Create a Test project and implement the relevant test cases.

# Task 2 (Not required but you would have extra fun and points if delivered)
4.	Update the Web API so that it also checks whether the password has appeared in data breaches and provide the number of times the password has appeared in those breaches. To check if the password has appeared in data breaches, you need to use the Have I Been Pwnd API described here https://haveibeenpwned.com/API/v3#PwnedPasswords and here https://www.troyhunt.com/pwned-passwords-version-5. You must protect the password being searched for by using the k-anonymity model provided by the API. IMPORTANT: the Pwned Passwords API version 5 is still free as described here https://www.troyhunt.com/pwned-passwords-version-5. You DO NOT NEED TO PAY to access this API. 
5.	Update the console app described in the previous task to interact with your updated Web API. Now the console app must also return the number of times the password has appeared in data breaches.
6.	Update the Test project described in the previous task to implement the relevant test cases to cover the new requirements. 

# Tasks completed
Both Task 1 and 2 have been implemented. 

# Solution components
1. .Net Core Console App
2. .Net Core Web API
3. xUnit Test project

# Build and Run instructions
Open the .sln file in Visual Studio 
Set the Console App and the Web API as Startup project
Run the solution 

# Running the Unit test cases
Open the Test explorer Click Run All Tests to run the unit tests
