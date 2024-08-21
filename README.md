# Language Learning App

Welcome to the Language Learning App, powered by OpenAI's GPT. This application facilitates language acquisition through an interactive web platform.

## Installation

### Database Setup

1. Set the default project to `LanguageLearningApp.Data`.
2. Navigate to the API directory:
    ```bash
    cd LanguageLearningApp.API
    ```
3. Update the database with Entity Framework:
    ```bash
    dotnet ef database update
    ```

### Configuration

1. Obtain an OpenAI API key.
2. Configure the API key in the `appsettings.json` file.

## Usage

1. **Register/Login:** Create an account or log in using your username and email.
2. **Select Language:** Choose the language you wish to learn.
3. **Choose Topic:** Pick a topic and start exploring the associated lessons.
4. **Add Content:** You can manually add new topics and lessons by modifying the API code.
