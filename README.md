# Market Gossip Chat

### A chat app build with .Net 6 and React

### Available features and flow:

- New users can sign up
- This app uses .Net Identity
- Registered users can log in and talk with other users in a chatroom and then log out
- Users can post messages as commands into the chatroom with the following format
``/stock=stock_code ``
- When a user post a valid command, a decoupled bot will call the [Stooq API]( https://stooq.com/ "Stooq") using the received _stock_code_ to get a CSV with quote informations
- The Bot will parse the received CSV file and then post a message to a RabbitMQ queue with the quote info.
- Then RabbitMQ will send the message to the subscriber( chat server) and the chat server will build a message and send to the chatroom using the following format: ``“{stock} quote is ${close value} per share”``. This message owner will be the bot.
- Chat messages are limited to the last 50 messages. 

### Possible future features:
- Multiple chat rooms and an option to choose a room or create a new room
- Save room and chat data on DB (encrypt messages)
- Show online users on screen

### Known issues:
- UX and UI are not the best on the market
- The final message is hardcoded with the dollar $ icon, but Japanese stocks are in Yen (eg 1452.JP)
- Lack of unit/integration tests

### To run this project, follow these steps :

## Backend services

A RabbitMQ instance is necessary to run the backend services. It is possible to easy set one with docker using the following command:

 __docker run -d --rm -it --hostname my-rabbit -p 15672:15672 -p 5672:5672 --name mkt-gossip-rabbit rabbitmq:3-management__

*If your RabbitMq config is different from the command above, you'll need to update the connection info in the appsettings.json on both ChatApp and Worker

### How to run:

***Before runs make sure you have [.Net 6](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) installed on your machine***

1. Clone this repository and navigate to the backend directory
2. For the ChatApp run:

  __dotnet run -p .\Services\ChatApp\MarketGossip.ChatApp\MarketGossip.ChatApp.csproj__

3. For the Integration Worker run:

  __dotnet run -p .\Services\Workers\MarketGossip.IntegrationWorkers\MarketGossip.IntegrationWorkers.csproj__

This app uses SQLite. The [database file is already in this project](https://github.com/leo2d/market-gossip-dot-net/blob/main/backend/Services/ChatApp/MarketGossip.ChatApp/market-gossip.db). So as long as you use this Db File there's no need to run migrations.
There are already 2 users in this database, but you can register with new users using the sigin up form.

Existing users: 

username: maryjane
password: MaryPass123!

username: jacktrader
password: mypass123A!

## Frontend

### Into the stack:

This project was bootstrapped with [Create React App](https://github.com/facebook/create-react-app) using __React Js__ and Hooks

### How to run:

***Before runs make sure you have Node Js installed on your machine***

1. Clone this repository and navigate to the frontend directory
2. Then run __yarn__ or __npm i__
3. Setup the connection to the api
    1. Open the file **serverURL.js** in _src/serverURL.js_
    2. Change the value of **serverURL** with your server url info:  
         For Example:
        ```javascript

        //serverURL.js
        
       const serverURL = 'http://localhost:7004'; //your url here

       export default serverURL;

        
        ```
4. So you can run __yarn start__ or __npm run start__ to start the application
