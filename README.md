# Labb3 Code-first application DBMS using MongoDB

A refactored version of Labb2 Dungeon Crawler. Using code-first with MongoDB the goal is to implement a save-function for the player to save their progress during gameplay. This is a team-effort done together with Robin Björkil: https://github.com/RobinBjorkil

SETUP:
Build the repository project in Visual Studio and go to "Manage user Secrets" to create a secrets.json class that contains the following key: ["ConnectionString"] - which points to your chosen server in MongoDB. Here's a template for how your secrets.json file should look like:

{ "ConnectionString": "mongodb://your_Server_Name/" }

Change "your_Server_Name" with your chosen server in MongoDB.

Run the project in Visual Studio.
See the following picture if you're unsure where to find "Manage User Secrets" image

User Secrets Guide: https://dontpaniclabs.com/blog/post/2023/03/02/how-to-set-up-user-secrets-for-net-core-projects-in-visual-studio/
