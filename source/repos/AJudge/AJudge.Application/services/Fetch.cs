using AJudge.Application.DtO.ProblemsDTO;
using System;
using System.Net.Http;
using System.Collections.Generic;
using HtmlAgilityPack;
using AJudge.Domain.Entities;
using AJudge.Application.services;

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

        public Task<Submission> Submit(string url, string code, string language, int problemId)
        {
            throw new NotImplementedException();
        }
    }
}
