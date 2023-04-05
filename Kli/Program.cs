using Newtonsoft.Json.Linq;
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

                JObject requestBody = BuildRequestBody(userInput, cwd);
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

        private static JObject BuildRequestBody(string userInput, string cwd)
        {
            return new JObject(
                new JProperty("model", "gpt-3.5-turbo"),
                new JProperty("messages", new JArray(
                    new JObject(
                        new JProperty("role", "system"),
                        new JProperty("content", $"You are a helpful assistant. You will generate PowerShell, Git, Node.js, and other relevant command-line commands based on user input. Your response should contain ONLY the command and NO explanation. Do NOT ever use newlines to separate commands, instead use ';'. The current working directory is '{cwd}'.")
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

            System.Diagnostics.Process process = new System.Diagnostics.Process
            {
                StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = "-NoProfile -ExecutionPolicy Bypass -Command " + command,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            Console.WriteLine(output);
        }
    }
}
