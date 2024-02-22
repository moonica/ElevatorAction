# ElevatorAction
 Jonica Brown DVT Elevator Coding Challenge Feb 2024

This project models the operations of an elevator or set of elevators in a building, as a simple console line application. It was created in response to a technical challenge by software firm DVT as part of a recruitment process.
https://www.dvt.co.za/

This repo also includes 
- SQL scripts to generate the necessary DB and populate it with sample data
- Metadata files such as a decision register and diagrams
- The project management Trello board for this project can be found at:
https://trello.com/b/34r3ZIEA/elevator-action-tasks

##Technologies:
- C#.Net
- MS SQL Server

##Tests:
- Unit tests have been included as a project in the solution using MSTest
- Integration tests have not been included in this version

##Usage
1. The database connection string should be set in the App.config file prior to execution. Other settings are also configurable in the App.config file, such as the number of times to allow an unrecognized command before exiting.
2. The project is run by executing the executable (.exe) file.
3. Interaction is through the console via text commands. Input is not case sensitive.
4. A list of commands can be retrieved at any time (other than in the middle of an operation sequence) by entering the command "help".
5. Exit the application at any time (other than in the middle of an operation sequence) by entering the command "exit". NOTE that state will be lost upon exit (elevator definitions are saved to database but state information such as their position, direction of travel and load will be lost).

##Logging
Not implemented in this version

##Contact
Jonica Brown
jonicabrown@viewcentre.co.uk
https://github.com/moonica
https://www.linkedin.com/in/jonica-brown/
Pronunciation guide: "Jo" as in YOU, "ni" as in NIck, "ca" as in CUp. Like a unicorn.

##License
MIT License
Copyright (c) 2024 Jonica Brown
More details can be found in this project by opening the "LICENSE" file
