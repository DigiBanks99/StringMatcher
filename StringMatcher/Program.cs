using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace StringMatcher
{
  class Program
  {
    private static readonly Regex _regexAlphaNum = new Regex("^[a-zA-Z0-9]+$");

    protected Program()
    {

    }

    static void Main(string[] args)
    {
      try
      {
        Console.WriteLine(GetHelp());

        Console.Write("Please provide the search string:\t");
        var searchLine = Console.ReadLine();
        Console.Write("Please provide the file path:\t\t");
        var filePath = Console.ReadLine();

        var fileInfo = new FileInfo(filePath);
        if (!fileInfo.Exists)
        {
          WriteError($"The file {fileInfo.FullName} cannot be found. Please ensure it exists and that you have specified the correct path.{Environment.NewLine}");
          return;
        }

        if (!_regexAlphaNum.IsMatch(searchLine))
        {
          WriteError($"The input string {searchLine} is not alphanumeric.{Environment.NewLine}");
          return;
        }

        var streamReader = new StreamReader(fileInfo.FullName);
        try
        {
          var text = streamReader.ReadToEnd();
          if (string.IsNullOrEmpty(text))
          {
            WriteError($"No match found. File does not contain any text.{Environment.NewLine}");
            return;
          }

          var lines = text.Split(Environment.NewLine);
          if (lines == null || !lines.Any())
          {
            WriteError($"No match found. File only contains whitespace.{Environment.NewLine}");
            return;
          }

          var inputCharList = searchLine.ToCharArray().ToList();
          inputCharList.Sort();

          int initialLineLength = lines.First().Length;
          if (inputCharList.Count != initialLineLength)
            WriteError($"Your input string is not necessarily the same length as all the lines in your file.{Environment.NewLine}");

          bool fileLengthFlagged = false;
          var sb = new StringBuilder();
          foreach (var line in lines)
          {
            if (!fileLengthFlagged && line.Length != initialLineLength)
            {
              WriteError($"Not all the lines in your file have the same length.{Environment.NewLine}");
              fileLengthFlagged = true;
            }

            if (!_regexAlphaNum.IsMatch(line))
              WriteError($"The line {line} in {fileInfo.FullName} is not alphanumeric.{Environment.NewLine}");

            var lineCharList = line.ToCharArray().ToList();
            lineCharList.Sort();

            if (string.Join("", lineCharList) == string.Join("", inputCharList))
              sb.AppendLine(line);
          }

          if (sb.Length == 0)
          {
            WriteError("No match found. No strings in the file are equivalent.");
            return;
          }

          Console.WriteLine("The following equivalent strings were found:");
          Console.WriteLine(sb.ToString());
        }
        catch (IOException ex)
        {
          WriteError(ex);
        }
        finally
        {
          streamReader.Close();
        }
      }
      catch (Exception ex)
      {
        WriteError(ex);
        throw;
      }
      finally
      {
        Console.Write("Press any key to continue...");
        Console.ReadKey();
      }
    }

    private static string GetHelp()
    {
      var sb = new StringBuilder();

      sb.AppendLine("Takes a string and a file path as an input and checks if the file contains a string equivalent to the input.");
      sb.AppendLine("A file is said to contain an equivalent string if a string exists within the file for which all the characters are also present in the input string.");

      return sb.ToString();
    }

    private static void WriteError(Exception ex)
    {
      WriteError(ex.Message);
    }

    private static void WriteError(string message)
    {
      Console.Error.WriteLine($"ERROR: {message}");
    }
  }
}
