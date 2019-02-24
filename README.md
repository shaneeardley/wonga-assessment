# Rabit MQ Assessment - Shane Eardley

## Preface
In order to fulfil the requirements of dispatching and reading messages, a pair of dotnetcore console applications, using the RabbitMQ messaging service, were created.

## Prerequisites
* [RabbitMQ](https://www.rabbitmq.com/download.html) Installed and running
* [.NET Core SDK](https://dotnet.microsoft.com/download) Installed

## How to Use
* Clone the project locally 
* Navigate to the ['Shane.MQ'](./Shane.MQ) directory
* Run the file [startup.cmd](./Shane.MQ/startup.cmd). This will run a dotnet restore, build the project, and launch both the sender and receiver locally
* Alternatively, open the Shane.MQ.sln sollution file, and use Visual Studio to build and run the projects
