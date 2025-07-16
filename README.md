# HackerNewsDashboardCodeChallenge

The challenge:

1. Create an API (server) to communicate with the HackerNews API (think of a proxy).
    End-point authentication would be a plus (username/password)
    It must support searching the data source
    It should have support for a "rating" system
    This must support basic Create, Read, Update, Delete (CRUD) operations
    It should have support for a comment system
2. Create a User Interface (browser) that can display content from the data source.
    Again, authentication would be a plus (username/password)
    It should allow the user to search the data source
    It should allow the user to "rate" a data item (CRUD). Think of a star rating system
    It should allow the user to add comments (CRUD)

For the "It must support searching the data source": I was not certain what would qualify for that. Funnily enough, the actual Hacker News doesn't have a search page because of the amount of queries that would take with how they have their data set up. It would be totally possible, but it would be querying every story ever done (or just the 200/500 on their lists). To be anywhere near performant there would have to be caching, etc., and I figured that was out of the scope of the challenge.

My solution is a single server. It serves the frontend at the root paths, which is a webassembly blazor application. It also serves as an API which has the proxy at /api/proxy/..., as well as methods for the front-end to call at /api/... (for convenience, authentication, etc.).

I used Radzen.Blazor to make the client style. I have contributed to the open source project before, and generally like product and team.

I used SQLLite for the database, since it is easiest to put in the GitHub repository. It has a little bit of pre-loaded data on a user with Email: "johnDoe@gmail.com" and Password: "passwordA1$". Or you can register as new user of course, the emails are there just to help with login they never get interacted with.

No tests since the main failure points are the HTTP requests and UI Elements, and what isn't is pretty simple functions.

I also added basic support for machine learning (AI) based reccomendations. I don't have a dataset nearly large enough to get good results off of this, and it uses Matrix Factorization so a large amount of other users would also be needed. For this reason, clicking any item that has not been rated before by some user will make it unable to predict a possible rating. Nonetheless, a very similar process could be used for any other number of reccomendation processes (Ex: a ChatGPT api call, or a model analyzing text similarity between a post and the titles of stories that the current user has liked in the past).

There are tons of things that could be improved on this project if given a proper purpose, but alas it is currently just a coding challenge.

Dockerfile can be found here: https://hub.docker.com/r/mvoxland/hackernewsdashboard