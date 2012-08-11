using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Embedded_C_Parser
{
    public class PreProcessorParser
    {
        private List<string> code;
        private SortedSet<string> externalLibraries; //like stdio.h
        private List<string> possibleIncludeFilesPaths; //like headers from project
        private List<KeyValuePair<string, string>> objectLikeMacros;

        public PreProcessorParser(List<string> inputCode, List<string> includeFilesPaths = null)
        {
            code = inputCode;

            if (includeFilesPaths == null)
            {
                includeFilesPaths = new List<string>();
            }
            else
            {
                possibleIncludeFilesPaths = includeFilesPaths;
            }

            externalLibraries = new SortedSet<string>();
            objectLikeMacros = new List<KeyValuePair<string, string>>();
        }

        //returns code with parsed pre-processor directives
        public List<string> Parse()
        {
            RemoveComments();
            GetMacros(); //and apply header guarian (#ifndef #define #endif) //#include too
            //GetLibrariesList(); //libraries like stdio.h
            ApplyMacros();

            return code;
        }

        /// <summary>
        /// applies all known macros to code
        /// its slow like hell, but its not a problem for now
        /// </summary>
        private void ApplyMacros()
        {
            for (int i = 0; i < code.Count; i++)
            {
                while (objectLikeMacros.Any(item => code[i].Contains(item.Key))) //if any macro can be found
                {
                    foreach (var item in objectLikeMacros) //try to apply every macro
                    {
                        code[i].Replace(item.Key, item.Value);
                    }
                }
            }
        }

        private void GetLibrariesList()
        {
            throw new NotImplementedException();
        }

        private void RemoveComments()
        {
            RemoveOneLineComments();
            RemoveMultiLineComments();
        }

        private void RemoveOneLineComments()
        {
            for (int i = 0; i < code.Count; i++)
            {
                if (code[i].Contains(@"//"))
                {
                    code[i] = code[i].Substring(0, code[i].IndexOf("//"));
                }
            }
        }

        private void RemoveMultiLineComments()
        {
            bool foundCommentStart = false;
            bool foundCommentEnd = false;
            int startLine = -1;
            int startChar = -1;
            int endLine = -1;
            int endChar = -1;

            for (int i = 0; i < code.Count; i++)
            {
                if (!foundCommentStart) //look for comment start only when it wasnt found before
                {
                    if (code[i].Contains(@"/*")) //if line contains comment stat
                    {
                        foundCommentStart = true;
                        startLine = i;
                        startChar = code[i].IndexOf(@"/*");
                    }
                }

                if (foundCommentStart)  //NOT ELSE! - "foundCommentStart" can change it's value
                {
                    if (code[i].Contains(@"*/"))
                    {
                        foundCommentEnd = true;
                        endLine = i;
                        endChar = code[i].IndexOf(@"*/") + 2;
                    }
                }

                if (foundCommentStart && foundCommentEnd)
                {
                    RemoveTextBlock(startLine, startChar, endLine, endChar);

                    {	//clean up
                        foundCommentStart = false;
                        foundCommentEnd = false;
                        startLine = -1;
                        endLine = -1;
                        startChar = -1;
                        endChar = -1;
                    }

                    i--; //there can be more comments in 1 line
                }
            }
        }


        /// <summary>
        /// removes chars from 1st to char before last one
        /// </summary>
        /// <param name="startLine"></param>
        /// <param name="startChar"></param>
        /// <param name="endLine"></param>
        /// <param name="endChar"></param>
        private void RemoveTextBlock(int startLine, int startChar, int endLine, int endChar)
        {
            if (startLine > endLine)
                throw new Exception(String.Format(
                    "comment's start line: {0} is after comment's end line: {1}", startLine, endLine
                ));

            if (startLine == endLine && startChar > endChar)
                throw new Exception(String.Format(
                    "comment's start char: {0} is after comment's end char: {1} (they are in same line: {2}", startChar, endChar, startLine
                ));

            //remove comment text
            if (startLine == endLine) //comment in 1 line
            {
                code[startLine] = code[startLine].Remove(startChar, endChar - startChar); 
            }
            else //multi line comment
            {
                code[startLine] = code[startLine].Substring(0, startChar); 
                code[endLine] = code[endLine].Substring(endChar, code[endLine].Length - endChar); 
                for (int j = startLine + 1; j < endLine; j++) 
                {
                    code[j] = string.Empty;
                }
            }
        }

        //gets and removes macros definition from code
        private void GetMacros()
        {
            MergeMultiLineMacros();

            bool waitingForEndif = false;
            bool ifdefDeletringAgroo = false; //ifndef or ifdef with failed condition is on (so we are removing lines)
            for (int i = 0; i < code.Count; i++)
            {
                //#endif
                IfndefAndIfdefLineRemoverAndEndifActivator(ref waitingForEndif, ref ifdefDeletringAgroo, i);

                //#define
                //get object like macros (a) -> (b) and remove them from code
                GetObjectLineMacrosFromLine(i);

                //#ifndef  and  #ifdef
                ifndefAndIfdefActivator(ref waitingForEndif, ref ifdefDeletringAgroo, i);

               //#include < lib >
                LibrariesIncludeGetter(i);

               //#include " xxx "
                HeadersIncluder(i);
            }
            //TODO: make Function-like Macros
        }

        //not tested
        private void HeadersIncluder(int lineNo)
        {
            if (code[lineNo].ToLower().Trim().StartsWith("#include\""))
            {
                int startQuoteIndex = code[lineNo].IndexOf('"');
                int endQuoteIndex = code[lineNo].LastIndexOf('"');
                if (endQuoteIndex == -1)
                    throw new ApplicationException(String.Format(
                        "#include\" xxx \" without ending quote '\"' in line: {0}", lineNo
                    ));

                string include = code[lineNo].Substring(startQuoteIndex, endQuoteIndex - startQuoteIndex);
                string includePath;

                if ((includePath = possibleIncludeFilesPaths.Find(x => GetFileNameFromPath(x).Equals(include))) == String.Empty)
                    throw new ApplicationException(String.Format(
                        "There is an header: {0} in line: {1} which does not exist in possible includes list",
                        include,
                        lineNo
                    ));

                IncludeHeaderToCode(includePath, lineNo);

                code[lineNo] = String.Empty;
            }
        }

        private void IncludeHeaderToCode(string headerPath, int lineNo)
        {
            List<string> headerCode = GetCodeFromFile(headerPath);

            AddHeaderToCode(lineNo, headerCode);
        }

        private void AddHeaderToCode(int lineNo, List<string> headerCode)
        {
            List<string> tempCode = code.GetRange(lineNo, code.Count - lineNo);
            code.RemoveRange(lineNo, code.Count - lineNo);
            code.AddRange(headerCode);
            code.AddRange(tempCode);
        }

        private static List<string> GetCodeFromFile(string headerPath)
        {
            if (!File.Exists(headerPath))
                throw new ApplicationException(String.Format(
                    "file: '{0}' you want to include does not exist"
                ));

            List<string> codeFromFile = new List<string>();
            using (StreamReader sr = new StreamReader(headerPath))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    codeFromFile.Add(line);
                }
                sr.Close();
            }
            return codeFromFile;
        }

        private string GetFileNameFromPath(string path)
        {
            int lastSlashIndex = path.LastIndexOf('/');
            if(lastSlashIndex == -1)
            {
                lastSlashIndex = path.LastIndexOf('\\');
            }

            if(lastSlashIndex == -1)
                throw new ArgumentException(String.Format(
                    "Path: {0} is incorrect", path
                    ),
                path
                );

            return path.Substring(lastSlashIndex + 1);
        }

        private void LibrariesIncludeGetter(int lineNo)
        {
            if (code[lineNo].ToLower().Trim().StartsWith(@"#include<"))
            {
                int startBracketIndex = code[lineNo].IndexOf('<');
                int endBracketIndex = code[lineNo].IndexOf('>');
                if (endBracketIndex == -1)
                    throw new ApplicationException(String.Format(
                        "#include< xxx > without ending bracket '>' in line: {0}", lineNo
                    ));

                externalLibraries.Add(code[lineNo].Substring(startBracketIndex, endBracketIndex - startBracketIndex));
            }
        }

        private void IfndefAndIfdefLineRemoverAndEndifActivator(ref bool waitingForEndif, ref bool ifdefDeletringAgroo, int i)
        {
            if (code[i].ToLower().TrimStart(' ').StartsWith("#endif"))
            {
                if (ifdefDeletringAgroo)
                {
                    ifdefDeletringAgroo = false;
                    waitingForEndif = false;
                    code[i] = String.Empty;
                }
                else if (waitingForEndif)
                {
                    waitingForEndif = false;
                    code[i] = String.Empty;
                }
                else
                {
                    throw new ApplicationException(String.Format(
                        "Unmatched #endif in line: {0}", i
                    ));
                }
            }

            //deleting lines if agroo active
            if (ifdefDeletringAgroo)
            {
                code[i] = String.Empty;
            }
        }

        private void ifndefAndIfdefActivator(ref bool waitingForEndif, ref bool ifdefDeletringAgroo, int lineNo)
        {
            if (code[lineNo].ToLower().TrimStart(' ').StartsWith(@"#ifndef"))
            {
                string[] words = code[lineNo].Split(' ');

                if (words.Length != 2)
                    throw new Exception(String.Format(
                        "Wrong #ifndef directive (wrong no of words) in line: {0}", lineNo
                    ));

                if (objectLikeMacros.Any(item => item.Key == words[1])) //if needed define is not defined
                {
                    ifdefDeletringAgroo = true;
                }
                waitingForEndif = true;
                code[lineNo] = String.Empty;
            }

            if (code[lineNo].ToLower().TrimStart(' ').StartsWith(@"#ifdef"))
            {
                string[] words = code[lineNo].Split(' ');

                if (words.Length != 2)
                    throw new Exception(String.Format(
                        "Wrong #ifdef directive (wrong no of words) in line: {0}", lineNo
                    ));

                if (!objectLikeMacros.Any(item => item.Key == words[1])) //if needed define is not defined
                {
                    ifdefDeletringAgroo = true;
                }
                waitingForEndif = true;
                code[lineNo] = String.Empty;
            }
        }

        private void GetObjectLineMacrosFromLine(int lineNo)
        {
            if (code[lineNo].ToLower().TrimStart(' ').StartsWith(@"#define"))
            {
                string[] words = code[lineNo].Split(' ');

                if (words[0].ToLower() != "#define")
                    throw new Exception(String.Format(
                        "bad #define directive - err 1 in line: {0}", lineNo
                    ));

                if (words.Length < 2)
                    throw new Exception(String.Format(
                        "bad #define directive - err 2 in line: {0}", lineNo
                    ));

                if (objectLikeMacros.Any(item => item.Key == words[1]))
                    throw new Exception(String.Format(
                        "{0} from line {1} is already defined as object like macro",
                        words[0],
                        lineNo
                    ));

                string definition = code[lineNo].Substring(code[lineNo].IndexOf(words[1]) + words[1].Length).TrimStart();

                objectLikeMacros.Add(new KeyValuePair<string, string>(
                    words[1], //name
                    definition
                ));

                code[lineNo] = String.Empty;
            }
        }

        /// <summary>
        /// merge multi line defines to 1-line defines
        /// </summary>
        private void MergeMultiLineMacros()
        {
            for (int i = 0; i < code.Count; i++)
            {
                if (code[i].ToLower().Contains(@"#define"))
                {
                    int j = i;
                    while (code[j] != String.Empty && code[j].Trim().Last() == '\\') // if '\' is last char add next line to macro
                    {
                        code[j] = code[j].Remove(code[j].IndexOf('\\'), 1);
                        code[j] += code[j + 1];
                        code[j + 1] = String.Empty;
                        j++;
                    }
                }
            }
        }

    }
}