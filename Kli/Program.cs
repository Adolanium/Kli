﻿using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace Kli
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                DisplayError("No command given.");
                return;
            }

            string token = "sk-xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";
            string userInput = string.Join(" ", args);
            string cwd = Environment.CurrentDirectory;

            try
            {
                using HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                string os = GetOperatingSystem();

                JObject requestBody = BuildRequestBody(userInput, cwd, os);
                HttpResponseMessage response = await client.PostAsync(
                    "https://api.openai.com/v1/chat/completions",
                    new StringContent(requestBody.ToString(), Encoding.UTF8, "application/json")
                ).ConfigureAwait(false);

                string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                JObject jsonResponse = JObject.Parse(responseBody);

                if (jsonResponse["error"] != null)
                {
                    DisplayError("Error from OpenAI API:");
                    Console.WriteLine(jsonResponse["error"]["message"]);
                    Console.WriteLine("Aborted.");
                    return;
                }

                string command = jsonResponse["choices"][0]["message"]["content"].ToString();
                DisplayCommand(command);

                if (UserConfirms())
                {
                    ExecuteCommand(command);
                }
                else
                {
                    DisplayError("Aborted.");
                }
            }
            catch (Exception ex)
            {
                DisplayError($"An error occurred: {ex.Message}");
            }
        }

        private static JObject BuildRequestBody(string userInput, string cwd, string os)
        {
            return new JObject(
                new JProperty("model", "gpt-3.5-turbo"),
                new JProperty("messages", new JArray(
                    new JObject(
                        new JProperty("role", "system"),
                        new JProperty("content", $"You are a helpful assistant. You will generate command-line commands for PowerShell, Bash, Zsh, Git, Node.js, and other relevant command-line tools based on user input. Your response should contain ONLY the command and NO explanation. Do NOT ever use newlines to separate commands, instead use ';' for PowerShell, Bash, and Zsh. The current working directory is '{cwd}', and the user's operating system is '{os}'.")
                    ),
                    new JObject(
                        new JProperty("role", "user"),
                        new JProperty("content", userInput)
                    )
                )),
                new JProperty("temperature", 0.0)
            );
        }

        private static void DisplayError(string message)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: {message}");
            Console.ResetColor();
        }

        private static void DisplayCommand(string command)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(command);
            Console.ResetColor();
        }

        private static bool UserConfirms()
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("Press any key to continue, or n to cancel: ");
            ConsoleKeyInfo keypress = Console.ReadKey();
            Console.ResetColor();

            return keypress.Key != ConsoleKey.N;
        }

        private static void ExecuteCommand(string command)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nExecuting command...");
            Console.ResetColor();

            System.Diagnostics.Process process = new System.Diagnostics.Process();
            string os = GetOperatingSystem();

            if (os == "Unix" || os == "MacOSX")
            {
                process.StartInfo.FileName = "/bin/bash";
                process.StartInfo.Arguments = $"-c \"{command}\"";
            }
            else
            {
                process.StartInfo.FileName = "powershell.exe";
                process.StartInfo.Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command {command}";
            }

            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            Console.WriteLine(output);
        }

        private static string GetOperatingSystem()
        {
            return Environment.OSVersion.Platform.ToString();
        }
    }
}
