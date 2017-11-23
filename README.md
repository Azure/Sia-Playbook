## This repository is part of the SIA project that can be found at Azure/Sia-Root

# Starting the project
* `git submodule init`
* `git submodule update --remote`
* Right click on the src/Sia.Playbook project in Visual Studio and select 'manage user secrets'
* Copy the src/Sia.Playbook/usersecrets.template.json file into this window
* Replace with your Azure Active Directory instance and playbook git repository

# Usage Notes

Event Types:  Source Data Object Options

* Event (0): data that may change for each instance of the thing happening (e.g., "this escalation went to $team", "this notification was sent to $distribution", etc.).
    
* Ticket (1): data that will only change for different incidents but not different events within one timeline (e.g., "ticket 100 has severity 1", "ticket 101 has severity 2", etc.).
    
* EventType (2): data that will be the same for every event of a given type.
    
* Engagement (3): data that will reflect the current user (which should be relatively rare).
  
    
  
# Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.microsoft.com.

When you submit a pull request, a CLA-bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., label, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

