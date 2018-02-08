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
        if (args == null || !args.Any())
        {
          Console.WriteLine(GetHelp());
          return;
        }

        if (args.Length < 2)
        {
          WriteError("You did not provide the required amount of arguments.");
          Console.WriteLine(Environment.NewLine);
          Console.WriteLine(GetHelp());
          return;
        }

        var fileInfo = new FileInfo(args[1]);
        if (!fileInfo.Exists)
        {
          WriteError($"The file {fileInfo.FullName} cannot be found. Please ensure it exists and that you have specified the correct path.");
          return;
        }

        if (!_regexAlphaNum.IsMatch(args[0]))
        {
          WriteError($"The input string {args[0]} is not alphanumeric.");
          return;
        }

        var streamReader = new StreamReader(fileInfo.FullName);
        try
        {
          var text = streamReader.ReadToEnd();
          if (string.IsNullOrEmpty(text))
          {
            WriteError("No match found. File does not contain any text.");
            return;
          }

          var lines = text.Split(Environment.NewLine);
          if (lines == null || !lines.Any())
          {
            WriteError("No match found. File only contains whitespace.");
            return;
          }

          var inputCharList = args[0].ToCharArray().ToList();
          inputCharList.Sort();

          int initialLineLength = lines.First().Length;
          if (inputCharList.Count != initialLineLength)
            WriteError("Your input string is not necessarily the same length as all the lines in your file.");

          bool fileLengthFlagged = false;
          var sb = new StringBuilder();
          foreach (var line in lines)
          {
            if (!fileLengthFlagged && line.Length != initialLineLength)
            {
              WriteError("Not all the lines in your file have the same length.");
              fileLengthFlagged = true;
            }

            if (!_regexAlphaNum.IsMatch(line))
              WriteError($"The line {line} in {fileInfo.FullName} is not alphanumeric.");

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
      sb.Append(Environment.NewLine);
      sb.AppendLine("Usage:\t\tStringMatcher yourstring filepath");
      sb.Append(Environment.NewLine);
      sb.AppendLine("Example:\tStringMatcher ABCDEF C:\\Temp\\MyFile.txt");

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
