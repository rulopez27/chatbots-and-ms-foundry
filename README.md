# chatbots-and-ms-foundry
This project has been made learning on how to use  Microsoft Bot Framework SDK to create chatbots of many kinds. Here will be all my notes.

## Contents
- [Basics and Key Concepts](#basics-and-key-concepts)
    - [Azure Bot Serivce](#azure-bot-service)
    - [Bot Framework](#bot-framework)
    - [Bot Framework SDK](#bot-framework-sdk)
        - [Bot Adapter](#bot-adapter)
        - [Activities](#activities)
        - [Turns](#turns)
- [R2D2 Chatbot](#rd2d-chatbot)
    - [Startup.cs](#startupcs)
    - [DialogAndWelcomeBot.cs](#dialogandwelcomebotcs)

## Basics and Key Concepts
### Azure Bot Service
A bot is an app that users interacts with in a conversational way, using text, graphics (such as cards or images), or speech. Azure Bot Service is a clouc platform. It hosts bots and makes them available to *channels*, such as Microsoft Teams, Facebook or Slack.

#### Bot Framework
The Bot Framework Service, which is a component of Azure IA Bot Service, sends information between the user's bot-connected app and the bot. Each channel can include additional information in the activities they send.

The Bot Framework Service sends a *conversation update* when a party joins the conversation. For example, on starting a conversation with the Bot Framework Emulator, you might see two conversation update activities (on for the user joining the conversation and one for the bot joining).

#### Bot Framework SDK
The Bot Framework SDK allows you to build bots that can be hosted on the Azure AI Bot Service. This service defines a REST API and an activity protocol for how your bot and channels or users can interact. A bot interaction involves the exchange of ***activities***, which are handled in ***turns***.

### Bot Adapter
An adapter class that implements the Bot Framework Protocol an can be hosted in different cloud environments. Inherits from **Microsoft.Bot.Builder.Integration.AspNet.Core.CloudAdapter**. The adapter:
- Provides a method for handling requests from and methods for generating requests to the user's channel.
- Includes middleware pipeline which include turn processing outside your bot's turn handler.
- Calls the bot's turn handler and catches errors not otherwise handled in the turn handler.

### Activities
Every interaction between the user (or a channel) and the bot is represented as an *activity*. Activities can represent human text of speech, app-to-app notifications, reactions and other messages, and so on. Please read [Activity Schema](https://github.com/Microsoft/botframework-sdk/blob/main/specs/botframework-activity/botframework-activity.md) for further details.

### Turns
In Bot Framework SDK, a *turn* consists of the user's incoming activity to the bot and any activity the bot sends back to the user as an inmediate response. For example if the user whants to perform a certain task, the bot might respond with a question to get more details about the task.

## RD2D Chatbot
This bot has been created using .NET 6.0 Microsoft Bot Framework Core Bot template which includes a functional flight scheduled bot using LUIS with unit tests. However it has been modified to do something different in order to learn Bot Framework capabilities. Here I'm describing what I have done and found through the code so far.

Starting from the begining, this is a .NET 8.0 ASP.NET Core Web API application at its finest. Just one controller which is used by the bot itself. However there are many components which I'm describing as I'm understanding them. I will try to list them as they are required on the application to run and interact in order of execution.

### Startup.cs
This class basically defines how the ASP.NET Core application will work by setting injection dependency on **ConfigureServices** void.
- Adds HttpClient, Controllers and NewtonsoftJson.
- Adds a Singleton instance of **Microsoft.Bot.Connector.Authentication.BotFrameworkAuthentication** and **Microsoft.Bot.Builder.Integration.AspNet.Core.ConfigurationBotFrameworkAuthentication** which are going to be used with the **Bot Adapter**.
- Adds a Singleton instance of an implementation of **Microsoft.Bot.Builder.IStorage** to store user and conversation state.
- Adds a Singleton instance of **Microsoft.Bot.Builder.UserState** which is used in this bot's Dialog implementations.
- Adds a Singleton instance of **Microsoft.Bot.Builder.ConversationState** which is used by the Dialog system itself.
- Adds a Singleton implementation of **Microsoft.Bot.Builder.IRecognizer** to use Language Understanding Intelligent Service (LUIS). This is represented by [FlightBookingRecognizer.cs](#flightbookingrecognizercs) class.
- Registers singleton instances of [BookingDialog.cs](#bookingdialogcs) and [MainDialog.cs](#maindialogcs) dialogs used by the bot.
- Creates the bot as a transient by adding an implementation of **Microsoft.Bot.Builder.IBot**. In this case the [BotController.cs](#botcontrollercs) is expencting an IBot implementation which is [DialogAndWelcomeBot.cs](#dialogandwelcomebotcs) of type [MainDialog.cs](#maindialogcs).

### DialogAndWelcomeBot.cs
This class does is the bot itself. It inherits from **Microsoft.Bot.Builder.IBot** and it is injected by **Microsoft.Bot.Builder.ConversationState**, **Microsoft.Bot.Builder.UserState** and *generic dialogs*. Its main behavior is an async task ***OnMembersAddedAsync*** which initializes the conversation to every member by sending them an ***Adaptive Card*** as a result of private [CreateAdaptiveCardAttachment](#createadaptivecardattachment) Attachment function.

##### CreateAdaptiveCardAttachment
Private **Attachment** function that returns an attachment set on a card resource definition from **welcomeCard.json**. This card attachment is used to provide more options to the user. You can define the style of the card. Here's an example:

```json
{
  "$schema": "http://adaptivecards.io/schemas/adaptive-card.json",
  "type": "AdaptiveCard",
  "version": "1.0",
  "body": [
    {
      "type": "Image",
      "url": "https://freepngimg.com/save/104620-photos-r2-d2-free-transparent-image-hq/840x771",//I have changed card image from default Bot Framework icon to a picture of RD2D.
      "size": "stretch"
    },
    {
      "type": "TextBlock",
      "spacing": "medium",
      "size": "default",
      "weight": "bolder",
      "text": "Welcome to R2D2!",//I have change welcome text here too.
      "wrap": true,
      "maxLines": 0
    },
    {
      "type": "TextBlock",
      "size": "default",
      "isSubtle": true,
      "text": "Now that you have successfully run your bot, follow the links in this Adaptive Card to expand your knowledge of Bot Framework.",
      "wrap": true,
      "maxLines": 0
    }
  ],
  "actions": [ //Here is some interesting stuff, you can show options to the user. Every option has its own action to do.
    {
      "type": "Action.OpenUrl",
      "title": "Get an overview",
      "url": "https://docs.microsoft.com/en-us/azure/bot-service/?view=azure-bot-service-4.0"
    },
    {
      "type": "Action.OpenUrl",
      "title": "Ask a question",
      "url": "https://stackoverflow.com/questions/tagged/botframework"
    },
    {
      "type": "Action.OpenUrl",
      "title": "Learn how to deploy",
      "url": "https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-howto-deploy-azure?view=azure-bot-service-4.0"
    }
  ]
}
```