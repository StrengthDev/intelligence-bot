# intelligence-bot
**(CURRENTLY GETTING PORTED TO PYTHON)**
My personal discord bot.

The goal of this project isn't anything too serious, just a Discord bot to be used in friend group servers.

# Features
(some features may have not been implemented yet)

 - **Math and RNG utilities** - The bot can solve math expressions, generate random numbers or random sequences in different ways, and calculate the expected number of tries for an arbitrary random event
 - **Information query** - General information may be queried, such as user info, the bot's current time, emote info, etc..
 - **Timers and reminders** - The bot can signal users after a specified duration or at a specified date and time
 - **Game library and sessions** - Users may add games they own to the bot's library, and then see common games among a specified group of users, have the bot pick a game for them, etc..
 - **Image generation** - The bot is capable of loading image generation AI models in the ONNX format and use them to generate and post random images (NOT IMPLEMENTED)
 - **Image manipulation** - Images posted in the chat can have filters applied to them or have text added on to them (NOT IMPLEMENTED)
 - **Conversation** - Users can "talk" to the bot (done by loading and using a transformer AI model) (NOT IMPLEMENTED)
 - **Miscellaneous** - Some other functions and commands are available, such as: saving and posting quotes (NOT IMPLEMENTED), "randomly" answering yes or no questions, 8ball command (NOT IMPLEMENTED), regex find and replace, reacting/replying to arbitrary messages (NOT IMPLEMENTED), etc..

# Configuration
To configure the token, the command prefix, commands like ask and 8ball, file locations for the game library, AI models and quotes, etc.. a "config.ini" file is used, located in the same directory as the executable.
