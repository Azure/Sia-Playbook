# Playbook Readme

## Configuration

Configuration structure can be found in usersecrets.template.json.

To create your own configuration, copy usersecrets.template.json to your userSecrets.

[Official documentation about user secrets.](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?tabs=visual-studio)

[More information on setting up secrets is available on the Gateway README.](../Sia-Gateway#service-architecture)

Playbooks will pull from a remote GitHub repository using the provided Token and repository identifiers.

If no GitHub Token is provided, Playbooks will use the Local configuration to load from the local file system.

