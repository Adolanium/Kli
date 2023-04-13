# Kli - Command Line Assistant

Kli is a versatile command-line assistant that generates commands for PowerShell, Bash, Zsh, Git, Node.js, and other command-line tools based on user input. It utilizes OpenAI's GPT-3.5-turbo to provide intelligent command suggestions. With Kli, you can easily and quickly generate commands without the need to memorize them.

## Features

- Generates commands for PowerShell, Bash, Zsh, Git, Node.js, and other command-line tools
- Cross-platform support for Windows, Linux, and macOS
- Utilizes OpenAI's GPT-3.5-turbo for intelligent command suggestions
- User confirmation before command execution
- Easy-to-use and lightweight
- Local caching of recent responses to reduce API calls and improve performance

## Getting Started

### Prerequisites

- .NET 6.0 or higher
- An OpenAI API key (sign up at https://beta.openai.com/signup/)

### Installation

1. Clone the repository:
```
git clone https://github.com/Adolanium/Kli.git
```

2. Navigate to the project directory:
```
cd Kli
```

3. Open `Program.cs` and replace `sk-xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx` with your OpenAI API key.

### Usage

1. Build the project:
```
dotnet build
```
2. Run the application with your input as command-line arguments:
```Kli "command"```

So, for example:

1. ```Kli show all files in current folder```
2. ```Kli create new folder named Test```
3. ```Kli soft undo last 3 git commits```
4. ```Kli create a js file that prints "Hello World", then run it```


## Contributing

Contributions are welcome! Please feel free to submit issues, feature requests, or pull requests.

## Acknowledgments

- [OpenAI](https://openai.com/) for their GPT-3.5-turbo model, which powers the command suggestions in Kli.
