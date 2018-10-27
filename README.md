# blocklybot

A web application for building chat bots.

## Project Structure
* BlockBot.AwsServices - C# class library for Amazon Web Services helper classes
* BlockBot.Tests - MsTest unit tests
* BlockBot.Web - ASP.NET Core MVC web application
  * closure-library - Google's common JavaScript library, used by google-blockly (https://github.com/google/closure-library)
  * google-blockly - Node.js library for a web-based visual programming editor (https://github.com/google/blockly)
  
## First Time Install Instructions

1. Ensure you have the following software installed:
   * (Optional) [Visual Studio Code](https://code.visualstudio.com/)
     * (Recommended) [Enable "Open with Code" functionality during install](https://thisdavej.com/right-click-on-windows-folder-and-open-with-visual-studio-code/)
   * [Visual Studio 2017](https://visualstudio.microsoft.com/thank-you-downloading-visual-studio/?sku=Community&rel=15#)
     * Install the following workloads (with all optional components): .NET Core cross-platform development, ASP.NET and web development, and Data science and analytical applications
   * (Optional) [Set up a Jetbrains student account](https://www.jetbrains.com/student/) and  install [Jetbrains ReSharper](https://www.jetbrains.com/resharper/)
   * [AWS Toolkit for Visual Studio](https://marketplace.visualstudio.com/items?itemName=AmazonWebServices.AWSToolkitforVisualStudio2017&refid=gs_card)
   * [Python 2](https://www.python.org/download/releases/2.7.8/)
   * [Node.js 8](https://nodejs.org/en/)
   * (Optional) [Jetbrains Webstorm](https://www.jetbrains.com/webstorm/) (javascript IDE)
   * (Optional) [Configure Windows to show file extensions](https://www.howtogeek.com/205086/beginner-how-to-make-windows-show-file-extensions/)
2. Clone this repo to your local machine
3. Navigate to BlockBot.Web/closure-library/ and run `npm install`
4. Navigate to BlockBot.Web/google-blockly/ and run `npm install`
5. 
