
namespace ???
{
public class PreProcessorParser
{
	private List<string> code;
	private List<string> externalLibraries; //like stdio.h
	private List<KeyValuePair<string, string>> objectLikeMacros;
	
	public PreProcessorParser(List<string> inputCode)
	{
		code = inputCode;
		externalLibraries = new List<string>();
		objectLikeMacros = new List<KeyValuePair<string, string>>();
	}
	

	//returns code with parsed pre-processor directives
	public List<string> Parse()
	{
		RemoveComments();
		ImportIncludedFiles(); //other .h files from project
		GetLibrariesList(); //like stdio.h
		GetMacros();
		ApplyMacros();
	}
	
	private void RemoveComments()
	{
		RemoveOneLineComments();
		RemoveMultiLineComments();
	}
	
	private void RemoveOneLineComment()
	{
		for(int i = 0; i < code.Count; i++)
		{
			if(code[i].Contains(@"//")
			{
				code[i] = code[i].SubStr(0, code[i].FirstOf('//'));
			}
		}
	}
	
	private void RemoveMultiLineComments()
	{
		bool foundCommentStart = false;
		bool foundCommentEnd = false;
		int startLine, startChar;
		int endLine, endChar;
		for(int i = 0; i < code.Count; i++)
		{
			if(!foundCommentStart) //look for comment start only when it wasnt found before
			{
				if(code[i].Contains(@"/*") //if line contains comment stat
				{
					foundCommentStart = true;
					startLine = i;
					startChar = code[i].FirstOf(@"/*");
				}
			}
			
			if(foundCommentStart)  //NOT ELSE! - "foundCommentStart" can change it's value 
			{
				if(code[i].Contains(@"*/")
				{
					foundCommentStart = true;
					startLine = i;
					startChar = code[i].FirstOf(@"*/");
				}
			}
			
			if(foundCommentStart && foundCommentEnd)
			{
				if(startLine > endLine)
					throw new Exception("comment's start line: {0} is after comment's end line: {1}");
					
				if(startLine == endLine && startChar > endChar)
					throw new Exception("comment's start char: {0} is after comment's end char: {1} (they are in same line: {2}", startChar, endChar, startLine);
					
				
				{	//remove comment text
					if(startLine == endLine) //comment in 1 line
					{
						code[startLine] = code[startLine].Substr(startChar, endChar - startChar); //remove from /* to */ in same line
					}
					else //multi line comment
					{
						code[startLine] = code[startLine].Substr(startChar, code[startLine].Length - startChar); //remove from /* to end of line in start line of comment
						code[endline] = code[endLine].Substr(0, endChar); //remove from line start to */ in end lines
						for(int j = startLine + 2; j < endLine; j++) //remove startLine - endline - 2 lines betw start and 
						{
							code.removeAt(startLine + 1); //it's not a bug (i hope so :D)
						}
					}
				}
				
				{	//clean up
					foundCommentStart = false;
					fountCommentEnd = false;
					startLine = -1;
					endLine = -1;
					startChar = -1;
					endChar = -1;
				}
				
				i--; //there can be more comments in 1 line
			}
			
		}
	}

	//gets and removes macros definition from code
	private void GetMacros()
	{
		//TODO: make Function-like Macros
		for(int i = 0; i < code.Count; i++)
		{
			if(code[i].ToLowerCase().Contains(@"#define"))
			{
			
			}
		}
	}
}
}