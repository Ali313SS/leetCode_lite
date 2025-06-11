using AJudge.Application.DtO.ProblemsDTO;
using System;
using System.Net.Http;
using System.Collections.Generic;
using HtmlAgilityPack;
using AJudge.Domain.Entities;
using AJudge.Application.services;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace AJudge.Application.Services
{
    public class FetchFromCses : IFetchServices
    {
        public  Task<Problem> FetchFrom(string url)
        {
            return Task.Run(() =>
            {
                using var client = new HttpClient();
                var response = client.GetAsync(url).Result;
                if (!response.IsSuccessStatusCode)
                    throw new Exception($"Failed to fetch data from URL: {response.StatusCode}");

                string htmlContent = response.Content.ReadAsStringAsync().Result;
                var doc = new HtmlDocument();
                doc.LoadHtml(htmlContent);

                // Problem Name
                var h1Title = doc.DocumentNode.SelectSingleNode("//h1[not(@id)]");
                var problemName = h1Title != null ? HtmlEntity.DeEntitize(h1Title.InnerText).Trim() : "Unknown";

                // Full content div
                var mdDiv = doc.DocumentNode.SelectSingleNode("//div[contains(@class,'md')]");

                // Description: all nodes before Input section
                var description = "No description available.";
                if (mdDiv != null)
                {
                    var sb = new System.Text.StringBuilder();
                    foreach (var node in mdDiv.ChildNodes)
                    {
                        if (node.Name.Equals("h1", StringComparison.OrdinalIgnoreCase) &&
                            node.GetAttributeValue("id", "").Equals("input", StringComparison.OrdinalIgnoreCase))
                            break;

                        if (node.Name == "p" || node.Name == "ul" || node.Name == "ol")
                            sb.AppendLine(HtmlEntity.DeEntitize(node.InnerText.Trim()));
                    }
                    var descText = sb.ToString().Trim();
                    if (!string.IsNullOrEmpty(descText))
                        description = descText;
                }

                // Input and Output formats
                var inputFormat = ExtractSection(doc, "input");
                var outputFormat = ExtractSection(doc, "output");

                // Test cases (sample)
                var testCases = ExtractTestCases(doc);

                return new Problem
                {
                    ProblemName = problemName,
                    Description = description,
                    InputFormat = inputFormat,
                    OutputFormat = outputFormat,
                    ProblemSource = "CSES",
                    ProblemLink = url,
                    TestCases = testCases
                };
            });
        }

        private static string ExtractSection(HtmlDocument doc, string sectionId)
        {
            var header = doc.DocumentNode.SelectSingleNode($"//h1[@id='{sectionId}']");
            if (header == null)
                return $"No {sectionId} section provided.";

            var sb = new System.Text.StringBuilder();
            for (var node = header.NextSibling; node != null; node = node.NextSibling)
            {
                if (node.Name.Equals("h1", StringComparison.OrdinalIgnoreCase))
                    break;

                if (node.Name.Equals("p", StringComparison.OrdinalIgnoreCase) ||
                    node.Name.Equals("ul", StringComparison.OrdinalIgnoreCase) ||
                    node.Name.Equals("ol", StringComparison.OrdinalIgnoreCase))
                {
                    sb.AppendLine(HtmlEntity.DeEntitize(node.InnerText.Trim()));
                }
            }

            var text = sb.ToString().Trim();
            return string.IsNullOrEmpty(text)
                ? $"No {sectionId} section provided."
                : text;
        }

        private static List<TestCase> ExtractTestCases(HtmlDocument doc)
        {
            var examplesHeader = doc.DocumentNode.SelectSingleNode("//h1[@id='example']");
            var cases = new List<TestCase>();
            if (examplesHeader != null)
            {
                // sample inputs and outputs are in the first two <pre> after the header
                var pres = examplesHeader.SelectNodes("following-sibling::pre");
                if (pres != null && pres.Count >= 2)
                {
                    var inputSample = HtmlEntity.DeEntitize(pres[0].InnerText).Trim();
                    var outputSample = HtmlEntity.DeEntitize(pres[1].InnerText).Trim();
                    cases.Add(new TestCase { Input = inputSample, Output = outputSample });
                }
            }
            return cases;
        }

        public async Task<string> Submit(string url,
       string code,
       string language,     
       int problemId)
        {
            var init = this.Init().Result;
            return await Submit(url, code, language, problemId, init.Key, init.Value);

        }
        protected async Task<string> Submit(
        string url,
        string code,
        string language,      
        int problemId,
        string phpSessionId,
        string csrfToken)
        {


            
            var taskid=url.Trim('/').Split('/').Last();
            Console.WriteLine($"Task ID: {taskid}");

            // Boundary must match the one in the raw request
            var boundary = "----WebKitFormBoundaryaAZOaIsZIiq4Xc2X";
            var crlf = "\r\n";

            // Build each form part as a string template
            string BuildPart(string name, string value)
            {
                // Note the empty line after Content-Disposition
                return
                    $"--{boundary}{crlf}" +
                    $"Content-Disposition: form-data; name=\"{name}\"{crlf}" +
                    $"{crlf}" +              // <-- empty line here
                    $"{value}{crlf}";
            }

            // Special case for the file part
            string BuildFilePart(string fieldName, string fileName, string fileContent)
            {
                return
                    $"--{boundary}{crlf}" +
                    $"Content-Disposition: form-data; name=\"{fieldName}\"; filename=\"{fileName}\"{crlf}" +
                    $"Content-Type: text/plain{crlf}" +
                    $"{crlf}" +              // <-- empty line before file data
                    $"{fileContent}{crlf}";
            }

            // Assemble the body
            var sb = new StringBuilder();
            sb.Append(BuildPart("csrf_token", $"{csrfToken}"));
            sb.Append(BuildPart("task", $"{taskid}"));
            sb.Append(BuildFilePart("file", "main.cpp", $"{code}"));
            sb.Append(BuildPart("lang", "C++"));
            sb.Append(BuildPart("option", "C++17"));
            sb.Append(BuildPart("type", "course"));
            sb.Append(BuildPart("target", "problemset"));
            // Closing boundary
            sb.Append($"--{boundary}--{crlf}");

            // Convert to byte[] (UTF-8)
            byte[] bodyBytes = Encoding.UTF8.GetBytes(sb.ToString());

            // Create the request
            var req = (HttpWebRequest)WebRequest.Create("https://cses.fi/course/send.php");
            req.Method = "POST";
            req.ContentType = $"multipart/form-data; boundary={boundary}";
            req.ContentLength = bodyBytes.Length;

            // Standard headers from your raw request
            req.Headers.Add("Cookie", $"PHPSESSID={phpSessionId}");
            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/136.0.0.0 Safari/537.36";
            req.Referer = $"{url}";
            req.Headers.Add("Origin", "https://cses.fi");
            req.Headers.Add("Upgrade-Insecure-Requests", "1");
            req.Headers.Add("Cache-Control", "max-age=0");
            req.Headers.Add("Accept-Language", "en-US,en;q=0.9");
            req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";

            // Write the body
            using (var reqStream = req.GetRequestStream())
            {
                reqStream.Write(bodyBytes, 0, bodyBytes.Length);
            }

            // Send and read the response
            using (var resp = (HttpWebResponse)req.GetResponse())
            using (var reader = new StreamReader(resp.GetResponseStream()))
            {
                Console.WriteLine($"Status: {(int)resp.StatusCode} {resp.StatusCode}");
                Console.WriteLine("Response body:");
                Console.WriteLine(reader.ReadToEnd());
            }
            return "XXXX";
        }




        protected async Task<KeyValuePair<string, string>> Init()
        {
            // First Request: GET /problemset/task/1617
            using var handler = new HttpClientHandler { UseCookies = false };
            using var client = new HttpClient(handler);

            var firstRequest = new HttpRequestMessage(HttpMethod.Get, "https://cses.fi/problemset/task/1617");
            firstRequest.Headers.Add("Host", "cses.fi");
            firstRequest.Headers.Add("Cache-Control", "max-age=0");
            firstRequest.Headers.Add("Sec-Ch-Ua", "\"Not.A/Brand\";v=\"99\", \"Chromium\";v=\"136\"");
            firstRequest.Headers.Add("Sec-Ch-Ua-Mobile", "?0");
            firstRequest.Headers.Add("Sec-Ch-Ua-Platform", "\"Windows\"");
            firstRequest.Headers.Add("Accept-Language", "en-US,en;q=0.9");
            firstRequest.Headers.Add("Upgrade-Insecure-Requests", "1");
            firstRequest.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/136.0.0.0 Safari/537.36");
            firstRequest.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
            firstRequest.Headers.Add("Sec-Fetch-Site", "same-origin");
            firstRequest.Headers.Add("Sec-Fetch-Mode", "navigate");
            firstRequest.Headers.Add("Sec-Fetch-User", "?1");
            firstRequest.Headers.Add("Sec-Fetch-Dest", "document");
            firstRequest.Headers.Add("Referer", "https://cses.fi/problemset/");
            firstRequest.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            firstRequest.Headers.Add("Priority", "u=0, i");
            firstRequest.Headers.Add("Connection", "keep-alive");

            var firstResponse = await client.SendAsync(firstRequest);
            firstResponse.EnsureSuccessStatusCode();

            // Extract PHPSESSID from set-cookie header
            string phpsessId = null;
            if (firstResponse.Headers.TryGetValues("Set-Cookie", out var cookies))
            {
                foreach (var cookie in cookies)
                {
                    if (cookie.Contains("PHPSESSID"))
                    {
                        var parts = cookie.Split(';');
                        foreach (var part in parts)
                        {
                            if (part.Trim().StartsWith("PHPSESSID="))
                            {
                                phpsessId = part.Trim().Substring("PHPSESSID=".Length);
                                break;
                            }
                        }
                    }
                    if (phpsessId != null)
                        break;
                }
            }

            if (string.IsNullOrEmpty(phpsessId))
                throw new Exception("PHPSESSID not found in first response.");
            Console.WriteLine($"PHPSESSID: {phpsessId}");

            // Second Request: GET /login with PHPSESSID cookie using automatic decompression
            using var handler2 = new HttpClientHandler
            {
                AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate | System.Net.DecompressionMethods.Brotli
            };
            using var client2 = new HttpClient(handler2);
            var secondRequest = new HttpRequestMessage(HttpMethod.Get, "https://cses.fi/login");
            secondRequest.Headers.Add("Host", "cses.fi");
            secondRequest.Headers.Add("Cookie", $"PHPSESSID={phpsessId}");
            secondRequest.Headers.Add("Sec-Ch-Ua", "\"Not.A/Brand\";v=\"99\", \"Chromium\";v=\"136\"");
            secondRequest.Headers.Add("Sec-Ch-Ua-Mobile", "?0");
            secondRequest.Headers.Add("Sec-Ch-Ua-Platform", "\"Windows\"");
            secondRequest.Headers.Add("Accept-Language", "en-US,en;q=0.9");
            secondRequest.Headers.Add("Upgrade-Insecure-Requests", "1");
            secondRequest.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/136.0.0.0 Safari/537.36");
            secondRequest.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
            secondRequest.Headers.Add("Sec-Fetch-Site", "same-origin");
            secondRequest.Headers.Add("Sec-Fetch-Mode", "navigate");
            secondRequest.Headers.Add("Sec-Fetch-User", "?1");
            secondRequest.Headers.Add("Sec-Fetch-Dest", "document");
            secondRequest.Headers.Add("Referer", "https://cses.fi/problemset/task/1617");
            // Removing the manual Accept-Encoding header as HttpClientHandler handles it
            secondRequest.Headers.Add("Priority", "u=0, i");
            secondRequest.Headers.Add("Connection", "keep-alive");

            var secondResponse = await client2.SendAsync(secondRequest);
            secondResponse.EnsureSuccessStatusCode();

            // The response content will now be automatically decompressed
            var secondHtml = await secondResponse.Content.ReadAsStringAsync();
            Console.WriteLine("secondResponse content:");
            Console.WriteLine(secondHtml);

            // Parse the html to extract csrf_token using HtmlAgilityPack
            var htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.LoadHtml(secondHtml);

            var csrfTokenNode = htmlDoc.DocumentNode.SelectSingleNode("//input[@name='csrf_token']");
            if (csrfTokenNode == null)
                throw new Exception("csrf_token not found in login page HTML.");

            var csrfToken = csrfTokenNode.GetAttributeValue("value", string.Empty);
            if (string.IsNullOrEmpty(csrfToken))
                throw new Exception("csrf_token has no value.");
            Console.WriteLine($"csrf_token: {csrfToken}");
            await SendLoginRequest(phpsessId, csrfToken); // Call the SendLoginRequest method with the extracted values.
            return new KeyValuePair<string, string>(phpsessId, csrfToken);

        }
        // Third Request: POST /login using csrf_token and PHPSESSID cookie
        // Pseudocode:
        // 1. Create a new HttpRequestMessage for the POST request using the URL "https://cses.fi/login".
        // 2. Add the request headers as provided (Host, Cookie, Cache-Control, etc.).
        // 3. Create the form data content containing csrf_token, nick, and pass.
        // 4. Assign the FormUrlEncodedContent to the request.Content.
        // 5. Use an HttpClient to send the request asynchronously.
        // 6. Ensure the response has a successful status code and (optionally) read the response content.



            protected async Task<string> SendLoginRequest(string phpsessionId, string csrfToken)
            {
                using var client = new HttpClient();
                var postRequest = new HttpRequestMessage(HttpMethod.Post, "https://cses.fi/login");
                postRequest.Headers.Add("Host", "cses.fi");
                postRequest.Headers.Add("Cookie", $"PHPSESSID={phpsessionId}");
                postRequest.Headers.Add("Cache-Control", "max-age=0");
                postRequest.Headers.Add("Sec-Ch-Ua", "\"Not.A/Brand\";v=\"99\", \"Chromium\";v=\"136\"");
                postRequest.Headers.Add("Sec-Ch-Ua-Mobile", "?0");
                postRequest.Headers.Add("Sec-Ch-Ua-Platform", "\"Windows\"");
                postRequest.Headers.Add("Accept-Language", "en-US,en;q=0.9");
                postRequest.Headers.Add("Origin", "https://cses.fi");
                // The Content-Type header will be set automatically by FormUrlEncodedContent.
                postRequest.Headers.Add("Upgrade-Insecure-Requests", "1");
                postRequest.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/136.0.0.0 Safari/537.36");
                postRequest.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
                postRequest.Headers.Add("Sec-Fetch-Site", "same-origin");
                postRequest.Headers.Add("Sec-Fetch-Mode", "navigate");
                postRequest.Headers.Add("Sec-Fetch-User", "?1");
                postRequest.Headers.Add("Sec-Fetch-Dest", "document");
                postRequest.Headers.Add("Referer", "https://cses.fi/login");
                postRequest.Headers.Add("Accept-Encoding", "gzip, deflate, br");
                postRequest.Headers.Add("Priority", "u=0, i");
                postRequest.Headers.Add("Connection", "keep-alive");

                var postData = new List<KeyValuePair<string, string>>
                        {
                            new KeyValuePair<string, string>("csrf_token", csrfToken),
                            new KeyValuePair<string, string>("nick", "Alice313"),
                            new KeyValuePair<string, string>("pass", "XYZBOB")
                        };
                postRequest.Content = new FormUrlEncodedContent(postData);

                var response = await client.SendAsync(postRequest);
                response.EnsureSuccessStatusCode();

                // Optionally, return or process the response content.
                return await response.Content.ReadAsStringAsync();
            }
        }
    }
        