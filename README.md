# RegistrationListenerService
Worker Service in .NET 5.0

Packages used: 
1. EntityFrameworkCore
2. RabbitMQ.Client
3. Automapper
4. Polly
5. CsvHelper
6. Serilog

Establishes a connection with a RabbitMQ message broker and listens for messages in a queue. Persists the messages in Database and CSV file. 
Maps the message record to a new object using Automapper and posts to two endpoints. Retry policies are applied for the post operations utilizing Polly.

Serilog is utilized to keep track of all actions.
