using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class Example
{
    public static HashSet<string> allMails = new HashSet<string>();
    public static int nombreIterations = 0;

    public static void Main()
    {
        Console.WriteLine("==================== Test 2 : Siren Validity");
        Console.WriteLine(CheckSirenValidity("126782")); //False
        Console.WriteLine(CheckSirenValidity("1267826546876")); //False
        Console.WriteLine(CheckSirenValidity("123456782")); //True
        Console.WriteLine(CheckSirenValidity("432567857")); //True
        Console.WriteLine(CheckSirenValidity("563457639")); //True
        Console.WriteLine(CheckSirenValidity("654367852")); //False

        Console.WriteLine("==================== Test 3 : Web Crawler de Mail");

        var response = GetEmailsInPageAndChildPages("www.test.com", 0);
        foreach (var item in response)
        {
            Console.WriteLine(item);

            //mailto:loin@mozilla.org
            //mailto:nullepart@mozilla.org
        }
    }

    //Test 3 : Web Crawler de Mail
    public static HashSet<string> GetEmailsInPageAndChildPages(string url, int maximumDepth)
    {
        if (nombreIterations <= maximumDepth)
        {
            nombreIterations++;

            var htmlContent = GetHtml(url);
            if (htmlContent == null)
            {
                return null;
            }

            var mails = ExtractMails(url);
            allMails.UnionWith(mails);

            var pageUrls = ExtractUrls(htmlContent);

            foreach (var u in pageUrls)
            {
                var result = GetEmailsInPageAndChildPages(u, maximumDepth);
                if (result == null)
                {
                    continue;
                }
            }
        }

        return allMails;
    }

    //Test 2 : Siren Validity
    public static bool CheckSirenValidity(string siren)
    {
        var sirenPattern = @"(\d{9}|\d{3}\d{3}\d{3})";

        Regex sirenRegx = new Regex(sirenPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);

        if (!sirenRegx.IsMatch(siren))
        {
            return false;
        }

        char[] ch = siren.ToCharArray();

        int sum = 0;
        for (var i = ch.Length - 1; 0 < i + 1; i--)
        {
            int strTmp = 0;

            if ((i + 1) % 2 == 0)
            {
                strTmp = int.Parse(ch[i].ToString()) * 2;
                strTmp = strTmp > 9 ? strTmp - 9 : strTmp;
            }
            else
            {
                strTmp = int.Parse(ch[i].ToString());
            }
            sum += strTmp;
        }

        return sum % 10 == 0;
    }

    private static HashSet<string> ExtractMails(string url)
    {
        const string emailPattern = @"href=""mailto:" + "[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}";
        var mails = new HashSet<string>();

        var codeSourceHtml = GetHtml(url);

        Regex mailRegx = new Regex(emailPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);

        MatchCollection matches = mailRegx.Matches(codeSourceHtml);

        foreach (Match match in matches)
        {
            mails.Add(match.Value.ToString().Substring(13));
        }

        return mails;
    }

    private static HashSet<string> ExtractUrls(string html)
    {
        const string utlPattern = "(?:href|src)=\"[^mail](.*?)[\"|'|>]";
        var urls = new HashSet<string>();

        Regex urlRegex = new Regex(utlPattern, RegexOptions.Singleline | RegexOptions.CultureInvariant);

        if (urlRegex.IsMatch(html))
        {
            foreach (Match match in urlRegex.Matches(html))
            {
                urls.Add(match.Groups[1].Value);
            }
        }

        return urls;
    }

    private static string GetHtml(string url)
    {
        //On suppose qu'à ce niveau on fait une récupéation du code html de la pag à partir de l'url passé en paramétre

        var text = @"<html>
                    <h1>CHILD2</h1>
                    <a href=""./index.html"">index</a>
                    <a href=""mailto:loin@mozilla.org"">Envoyer l'email loin</a>
                    <a href=""mailto:nullepart@mozilla.org"">Envoyer l'email nulle part</a>
                    ";
        return text;
    }
}