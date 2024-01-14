# For a clean setup of the process, run the scripts in order:

1. setup1.sh
   - This script will prompt you for values that will be stored in environment variables.
   - The application reads certain values from the code, so accurate naming and values are crucial.

   * Check the enviroment variables before proceeding to the setup2
   * Close the terminal where you executed setup 1 open a new one for setup2.

2. runall.sh
   - This script will build and run the main API project.
   - Running this project will automatically create two tables using the seeder.
   - Ensure you have SQL Server and MSSQL installed on your machine, and create a Windows account.
   - The connection string is straightforward and creates the database in Windows accounts.
   - Runs the entire application the blazor client, prxy api and main api