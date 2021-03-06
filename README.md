# BlockBot.io

A web application for building chat bots.

## Meeting Minutes

Project team meeting minutes can be found [here](https://github.com/uiowa-cs-5800-0001-fall-2018/blocklybot/wiki/Meeting-Minutes).

## Project Structure

This project is an ASP.NET Core MVC web application hosted on AWS Elastic Beanstalk. When a user sets up an integration and creates a project, specific code for interacting with that integration is created as an AWS Lambda function. A diagram of how the application works is [here](https://cloudcraft.co/view/83dae09e-8d89-45e3-a839-ce86165a1c0e?key=Ed73-LtpZYT6eClSgURPbQ). Below is a description of each part of this project.

* BlockBot.AwsServices/ - C# class library for Amazon Web Services helper classes
* BlockBot.Tests/ - MsTest unit tests
* BlockBot.Web/ - ASP.NET Core MVC web application
* [BlockBot.Web/closure-library/](https://github.com/google/closure-library) - Google's common JavaScript library, used by google-blockly
* [BlockBot.Web/google-blockly/](https://github.com/google/blockly) - Node.js library for a web-based visual programming editor
  
## First Time Install Instructions

This project assumes the developer is working on a Windows operating system, and ideally that the developer is working on Windows 10 or later.

1. Ensure you have the following software installed:
   * (Recommended) [Visual Studio Code](https://code.visualstudio.com/)
     * [Enable "Open with Code" functionality during install](https://thisdavej.com/right-click-on-windows-folder-and-open-with-visual-studio-code/)
   * [Visual Studio 2017](https://visualstudio.microsoft.com/thank-you-downloading-visual-studio/?sku=Community&rel=15#)
     * Install the following workloads (with all optional components): 
       * .NET Core cross-platform development
       * ASP.NET and web development
       * Data science and analytical applications
   * (Recommended) [Set up a Jetbrains student account](https://www.jetbrains.com/student/) and  install [Jetbrains ReSharper](https://www.jetbrains.com/resharper/)
   * [AWS Toolkit for Visual Studio](https://marketplace.visualstudio.com/items?itemName=AmazonWebServices.AWSToolkitforVisualStudio2017&refid=gs_card)
   * [Python 2](https://www.python.org/download/releases/2.7.8/)
   * [Node.js 8](https://nodejs.org/en/)
   * (Recommended) [Jetbrains Webstorm](https://www.jetbrains.com/webstorm/) (JavaScript IDE)
   * (Recommended) [Configure Windows to show file extensions](https://www.howtogeek.com/205086/beginner-how-to-make-windows-show-file-extensions/)
2. Clone this repo to your local machine
3. Navigate to `BlockBot.Web/closure-library` and run `npm install`
4. Navigate to `BlockBot.Web/google-blockly/` and run `npm install`
5. Open `FundamentalsChatbot.sln` in Visual Studio
6. Configure the application's database: 
   * Open the Package Manager Console: `Tools > NuGet Package Manager > Package Manager Console`
   * To delete an existing version of the database, run the `drop-database` command.
   * To creates a new, empty version of the database with the appropriate schema, run the `update-database` command.
7. Ensure the `BlockBot.Web` project is the startup project:
   * In the solution explorer, the current startup project has a bold project name
   * To set a project as the startup project, right-click the project in the solution explorer and select `Set as StartUp Project`
8. To start the project, click the green play icon in the top middle of the Visual Studio window. The first time this is done, Visual Studio will likely spend several minutes copying files and building the solution.
9. A browser should open and the site will open in a tab. This project uses HTTPS by default, so security errors may occur if your browser doesn't trust the certificate used for local development. Follow the appropriate steps for your browser to trust this certificate and visit the web page.

## Making Changes to this Project

This project does not allow people to push code to the develop or master branches. Instead, changes should be made by getting the latest version of develop, branching off of develop to a separate branch, making changes on the separate branch, and pushing the separate branch to this repository. Then, pull requests should be made to merge those changes into develop. Changes are merged from develop to master during software releases.

## Additional Resources

All additional documentation for this project is available under the project [wiki](https://github.com/uiowa-cs-5800-0001-fall-2018/blocklybot/wiki).

Resources for Google Blockly can be found on the [Blockly project page](https://developers.google.com/blockly/).

This project integrations with a number of web services, resources for which can be found below:
 * [Amazon Web Services](https://docs.aws.amazon.com)
 * Twilio: TODO
 * Facebook Messenger: TODO
 * Twitter: TODO
 * Reddit: TODO
 * Slack: TODO
 * Google Calendar: TODO
