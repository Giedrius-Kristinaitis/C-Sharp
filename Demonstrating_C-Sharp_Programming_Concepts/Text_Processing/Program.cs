/*
 * Author: Giedrius Kristinaitis
 * 
 * For the sake of simplicity all classes are kept in the same source file
 */


using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Text {

	/// <summary>
	/// main program class
	/// </summary>
	class Program {

		// array containing all symbols that are considered word seperators in the text
		static char[] SEPERATORS = new char[] { ',', '.', ' ', '!', '?', '-', '/', '(', ')', ':', ';' };

		/// <summary>
		/// entry point of the program
		/// </summary>
		/// <param name="args">arguments for the program</param>
		static void Main(string[] args) {
			string[] lines = File.ReadAllLines("Knyga.txt");

			List<Word> words = new List<Word>();
			SplitTextIntoWords(lines, words);

			AnalyseText(lines, words);
			EditText(lines, words);
		}

		/// <summary>
		/// this method edits original text - aligns words of each line in the correct positions,
		/// which is later saved to another text file
		/// </summary>
		/// <param name="lines">original text split into lines</param>
		/// <param name="words">all words of the text</param>
		static void EditText(string[] lines, List<Word> words) {
			int[] wordWidths = new int[FindHighestWordInLineCount(lines.Length, words)];

			for (int i = 0; i < wordWidths.Length; i++) {
				for (int j = 0; j < lines.Length; j++) {
					int wordWidth = FindWidth(lines[j], j, i, words);

					if (wordWidth > wordWidths[i]) {
						wordWidths[i] = wordWidth;
					}
				}
			}

			string formattedText = FormatText(lines, words, wordWidths);
			CreateFormattedTextFile("ManoKnyga.txt", formattedText);
		}

		/// <summary>
		/// creates a new text file and writes the edited text to it
		/// </summary>
		/// <param name="fileName">name of the new file</param>
		/// <param name="formattedText">edited text to be written to the file</param>
		static void CreateFormattedTextFile(string fileName, string formattedText) {
			using (StreamWriter writer = new StreamWriter(fileName)) {
				writer.Write(formattedText);
			}
		}

        /// <summary>
        /// formats text by aligning words in the correct positions
        /// </summary>
        /// <param name="lines">text split into lines</param>
        /// <param name="words">list with all words of the text</param>
        /// <param name="wordWidths">array containing required widths of each word
        /// wordWidths[0] - length of the first word
        /// wordWidths[1] - length of the second word
        /// ...</param>
        /// <returns>a new formatted version of original text</returns>
		static string FormatText(string[] lines, List<Word> words, int[] wordWidths) {
			StringBuilder newText = new StringBuilder();

			for (int i = 0; i < lines.Length; i++) {
				newText.Append(" ");

				for (int j = 0; j < wordWidths.Length; j++) {
					if (wordWidths[j] == 0) {
						break;
					}

					newText.Append(MakeCorrectLengthString(lines[i],
						FindWordStartIndex(words, i, j), FindWordStartIndex(words, i, j + 1), wordWidths[j]));
				}

				if (i < lines.Length - 1) {
					newText.AppendLine();
				}
			}

			return newText.ToString();
		}

		/// <summary>
		/// makes a shortened string to be the correct length by adding spaces
		/// if needed
		/// </summary>
		/// <param name="line">line of text containing the shortened string</param>
		/// <param name="startIndex">starting index of the string</param>
		/// <param name="endIndex">ending index of the string</param>
		/// <param name="width">new length of the string</param>
		/// <returns>new version of shortened string with the length of parameter 'width'</returns>
		static string MakeCorrectLengthString(string line, int startIndex, int endIndex, int width) {
			if (startIndex == -1) {
				return "";
			}

			StringBuilder builder = new StringBuilder();

			string shortenedString = (endIndex != -1) ?
				ShortenString(line.Substring(startIndex, endIndex - startIndex)) :
				ShortenString(line.Substring(startIndex));

			builder.Append(shortenedString);

			if (shortenedString.Length <= width) {
				int spacesToInsert = width - shortenedString.Length + 1;

				for (int i = 0; i < spacesToInsert; i++) {
					builder.Append(" ");
				}
			}

			return builder.ToString();
		}

		/// <summary>
		/// finds the length of a shortened word string in a line
		/// </summary>
		/// <param name="line">line of text containing the word string</param>
		/// <param name="lineNumber">numbe of the line</param>
		/// <param name="wordNumber">number of the word in text line</param>
		/// <param name="words">list containing words of the text</param>
		/// <returns>length of the shortened word string</returns>
		static int FindWidth(string line, int lineNumber, int wordNumber, List<Word> words) {
			int wordStartIndex = FindWordStartIndex(words, lineNumber, wordNumber);
			int nextWordStartIndex = FindWordStartIndex(words, lineNumber, wordNumber + 1);

			if (wordStartIndex != -1) {
				if (nextWordStartIndex != -1) {
					return ShortenString(line.Substring(wordStartIndex, nextWordStartIndex - wordStartIndex))
						.Length;
				} else {
					return ShortenString(line.Substring(wordStartIndex)).Length;
				}
			}

			return 0;
		}

		/// <summary>
		/// shortens a string 
		/// shortening is done by removing mathing seperators if they are next to each other and
		/// keeping only one of them
		/// </summary>
		/// <param name="inputString">string to shorten</param>
		/// <returns>new shortened string</returns>
		static string ShortenString(string inputString) {
			StringBuilder builder = new StringBuilder();
			char lastCharacter = ' ';

			for (int i = 0; i < inputString.Length; i++) {
				char character = inputString[i];

				if (!IsSeperator(character)) {
					builder.Append(character);
				} else {
					if (character != lastCharacter) {
						builder.Append(character);
					}
				}

				lastCharacter = character;
			}

			return builder.ToString();
		}

		/// <summary>
		/// finds the starting index of a specific word in the text line
		/// </summary>
		/// <param name="words">all words of the text</param>
		/// <param name="lineNumber">line number in which the word is</param>
		/// <param name="wordNumber">number of the word in text line (starting from 0)</param>
		/// <returns></returns>
		static int FindWordStartIndex(List<Word> words, int lineNumber, int wordNumber) {
			int wordIndex = 0;

			foreach (Word word in words) {
				if (word.Line > lineNumber) {
					break;
				}

				if (word.Line == lineNumber) {
					if (wordIndex == wordNumber) {
						return word.StartIndex;
					}

					wordIndex++;
				}
			}

			return -1;
		}

		/// <summary>
		/// finds the number of words in the line with most words
		/// </summary>
		/// <param name="lineCount">number of text lines</param>
		/// <param name="words">all words of the text</param>
		/// <returns>number of words in the longest (in words) line</returns>
		static int FindHighestWordInLineCount(int lineCount, List<Word> words) {
			int[] counts = new int[lineCount];

			foreach (Word word in words) {
				counts[word.Line]++;
			}

			return FindBiggestArrayElement(counts);
		}

		/// <summary>
		/// finds biggest integer in an array
		/// </summary>
		/// <param name="array">array to search for integer</param>
		/// <returns>value of the biggest integer in the array</returns>
		static int FindBiggestArrayElement(int[] array) {
			int max = int.MinValue;

			foreach (int element in array) {
				if (element > max) {
					max = element;
				}
			}

			return max;
		}

		/// <summary>
		/// this method analyses text - finds word fragments, 
		/// where the last word character
		/// matches the first character of the next word,
		/// number of words containing only digits and sum of those numbers
		/// </summary>
		/// <param name="lines">text split into lines</param>
		/// <param name="words">all words of the text</param>
		static void AnalyseText(string[] lines, List<Word> words) {
			if (words.Count == 0) {
				CreateAnalysisFile("Analizė.txt", null, 0, 0);
				return;
			}

			List<WordFragment> fragments = FindAllWordFragments(lines, words);
			SortWordFragments(fragments);
			KeepOnlyLongestFragments(fragments);
			List<string> fragmentStrings = CreateFragmentStrings(lines, fragments);

			int numberCount = 0;
			int numberSum = 0;
			FindNumberInfo(lines, words, ref numberCount, ref numberSum);

			CreateAnalysisFile("Analizė.txt", fragmentStrings, numberCount, numberSum);
		}

		/// <summary>
		/// creates a text analysis file and writes results to it
		/// </summary>
		/// <param name="fileName">name of the new file</param>
		/// <param name="fragments">list containing all fragment strings</param>
		/// <param name="numberCount">count of words containing only digits</param>
		/// <param name="numberSum">sum of numbers/words containing only digits</param>
		static void CreateAnalysisFile(string fileName, List<string> fragments, int numberCount, int numberSum) {
			using (StreamWriter writer = new StreamWriter(fileName)) {
				if (fragments == null || fragments.Count == 0) {
					writer.WriteLine("Tekste nėra tokių žodžių fragmentų, sudarytų iš žodžių, kur žodžio paskutinė raidė sutampa su kito žodžio pirmąja raide.");
				} else {
					writer.WriteLine("Ilgiausi žodžių fragmentai, sudaryti iš žodžių, kur žodžio paskutinė raidė sutampa su kito žodžio pirmąja raide: ");

					foreach (string fragment in fragments) {
						writer.WriteLine(fragment);
					}
				}

				if (numberCount == 0) {
					writer.WriteLine("Tekste nėra žodžių, sudarytų tik iš skaitmenų.");
				} else {
					writer.WriteLine("Tekste yra {0} žodžių, sudarytų tik iš skaitmenų.", numberCount);
					writer.WriteLine("Skaičių bendra suma: {0}", numberSum);
				}
			}
		}

		/// <summary>
		/// removes all fragments from a list, except the ones with the highest length
		/// </summary>
		/// <param name="fragments">list to remove fragments from</param>
		static void KeepOnlyLongestFragments(List<WordFragment> fragments) {
			if (fragments.Count == 0) {
				return;
			}

			WordFragment longestFragment = fragments[0];

			for (int i = 1; i < fragments.Count; i++) {
				if (fragments[i].Length < longestFragment.Length) {
					fragments.RemoveAt(i--);
				}
			}
		}

		/// <summary>
		/// forms strings (see FormFragmentString(string[], WordFragment) mehthod)
		/// of all word fragments in the text and stores it in a list
		/// </summary>
		/// <param name="lines">text split into lines</param>
		/// <param name="fragments">list containing all word fragments</param>
		/// <returns></returns>
		static List<string> CreateFragmentStrings(string[] lines, List<WordFragment> fragments) {
			List<string> fragmentStrings = new List<string>();

			foreach (WordFragment fragment in fragments) {
				fragmentStrings.Add(FormFragmentString(lines, fragment));
			}

			return fragmentStrings;
		}

		/// <summary>
		/// finds how many words in the text contain only digits (which means that a word is a number)
		/// and calculates the sum of these numbers
		/// </summary>
		/// <param name="lines">text split into lines</param>
		/// <param name="words">all words of the text</param>
		/// <param name="numberCount">reference to a such word count integer</param>
		/// <param name="numberSum">reference to a number sum integer</param>
		static void FindNumberInfo(string[] lines, List<Word> words, ref int numberCount, ref int numberSum) {
			foreach (Word word in words) {
				string regex = @"\D";
				string wordString = lines[word.Line].Substring(word.StartIndex, word.EndIndex - word.StartIndex + 1);

				if (!Regex.IsMatch(wordString, regex)) {
					numberCount++;
					numberSum += int.Parse(wordString);
				}
			}
		}

		/// <summary>
		/// sorts word fragments by length in descending order
		/// </summary>
		/// <param name="fragments">list of fragments to sort</param>
		static void SortWordFragments(List<WordFragment> fragments) {
			for (int i = 0; i < fragments.Count - 1; i++) {
				for (int j = i + 1; j < fragments.Count; j++) {
					if (fragments[j] > fragments[i]) {
						WordFragment temp = fragments[i];
						fragments[i] = fragments[j];
						fragments[j] = temp;
					}
				}
			}
		}

		/// <summary>
		/// find all word fragments, where the last word character
		/// matches the first character of the next word, in the text
		/// </summary>
		/// <param name="lines">text split into lines</param>
		/// <param name="words">all words of the text</param>
		/// <returns>List containing all word fragments</returns>
		static List<WordFragment> FindAllWordFragments(string[] lines, List<Word> words) {
			List<WordFragment> fragments = new List<WordFragment>();
			bool inFragment = false;
			WordFragment fragment = null;
			Word lastWord = words[0];

			for (int i = 1; i < words.Count; i++) {
				Word word = words[i];

				if (!inFragment && Char.ToLower(lastWord.LastCharacter) == Char.ToLower(word.FirstCharacter)) {
					inFragment = true;
					fragment = new WordFragment(lastWord.Line, lastWord.StartIndex);
				}

				if (inFragment && Char.ToLower(lastWord.LastCharacter) != Char.ToLower(word.FirstCharacter)) {
					inFragment = false;
					CompleteFragmentAndAddToList(fragments, fragment, lastWord.Line, lastWord.EndIndex, lines);
				} else if (inFragment && i == words.Count - 1) {
					inFragment = false;
					CompleteFragmentAndAddToList(fragments, fragment, word.Line, word.EndIndex, lines);
				}

				lastWord = word;
			}

			return fragments;
		}

		/// <summary>
		/// completes (fills with correct information) 
		/// a WordFragment object and adds it to a list containing all fragments
		/// </summary>
		/// <param name="fragments">list to store the fragment in</param>
		/// <param name="fragment">word fragment to be completed</param>
		/// <param name="endLine">ending line of the fragment</param>
		/// <param name="endIndex">ending index of the fragment</param>
		/// <param name="lines">text containing the fragment split into lines</param>
		static void CompleteFragmentAndAddToList(List<WordFragment> fragments, WordFragment fragment,
			int endLine, int endIndex, string[] lines) {
			fragment.EndLine = endLine;
			fragment.EndIndex = endIndex;
			CalculateFragmentLength(lines, fragment);
			fragments.Add(fragment);
		}

		/// <summary>
		/// calculates the length (in characters) of word fragment
		/// </summary>
		/// <param name="lines">lines of text containing the fragment (only used if the
		/// fragment occupies more than one two lines)</param>
		/// <param name="fragment">word fragment to calculate length of</param>
		static void CalculateFragmentLength(string[] lines, WordFragment fragment) {
			for (int i = fragment.StartLine; i <= fragment.EndLine; i++) {
				if (i == fragment.StartLine) {
					if (fragment.StartLine == fragment.EndLine) {
						fragment.Length += fragment.EndIndex - fragment.StartIndex + 1;
					} else {
						fragment.Length += lines[i].Length - fragment.StartIndex;
					}
				} else if (i == fragment.EndLine) {
					fragment.Length += fragment.EndIndex + 1;
				} else {
					fragment.Length += lines[i].Length;
				}
			}
		}

		/// <summary>
		/// forms a string containing text of a given word fragment and
		/// information about which lines the fragment occupies in the text
		/// </summary>
		/// <param name="lines">text split into lines containing the fragment</param>
		/// <param name="fragment">word fragment to take information from (start position,
		/// end position)</param>
		/// <returns>a new string with the word fragment content and information</returns>
		static string FormFragmentString(string[] lines, WordFragment fragment) {
			StringBuilder builder = new StringBuilder();

			for (int i = fragment.StartLine; i <= fragment.EndLine; i++) {
				if (i == fragment.StartLine) {
					if (fragment.StartLine == fragment.EndLine) {
						builder.Append(lines[i].Substring(fragment.StartIndex, fragment.Length));
					} else {
						builder.Append(lines[i].Substring(fragment.StartIndex));
					}
				} else if (i == fragment.EndLine) {
					builder.Append(lines[i].Substring(0, fragment.EndIndex + 1));
				} else {
					builder.Append(lines[i]);
				}

				builder.AppendLine();
			}

			builder.Append(string.Format("Fragmentas yra eilutėse {0} - {1}.",
					fragment.StartLine + 1, fragment.EndLine + 1));
			builder.AppendLine();

			return builder.ToString();
		}

		/// <summary>
		/// splits text into words
		/// 
		/// splitting is done by looping through each character in the text,
		/// checking if it is a part of a word
		/// </summary>
		/// <param name="lines">text split into lines</param>
		/// <param name="words">list to store words in</param>
		static void SplitTextIntoWords(string[] lines, List<Word> words) {
			for (int i = 0; i < lines.Length; i++) {
				bool inWord = false;
				Word word = null;

				for (int j = 0; j < lines[i].Length; j++) {
					char character = lines[i][j];

					if (!inWord && !IsSeperator(character)) {
						inWord = true;
						word = CreateWordObject(j, i, lines[i][j]);
					}

					if (inWord && IsSeperator(character)) {
						inWord = false;
						CompleteWordAndAddToList(words, word, j - 1, lines[i][j - 1]);
					} else if (inWord && j == lines[i].Length - 1) {
						inWord = false;
						CompleteWordAndAddToList(words, word, j, lines[i][j]);
					}
				}
			}
		}

		/// <summary>
		/// creates a new word object and sets initial information
		/// 
		/// this method was created to add more logic in creating a word if needed
		/// without making code more complicated elsewhere
		/// </summary>
		/// <param name="startIndex">starting index of the word</param>
		/// <param name="line">line index of the word</param>
		/// <param name="firstCharacter">first character of the word</param>
		/// <returns>a newly created Word object</returns>
		static Word CreateWordObject(int startIndex, int line, char firstCharacter) {
			Word word = new Word(line, startIndex, firstCharacter);
			return word;
		}

		/// <summary>
		/// completes a word object - sets it's properties to correct values
		/// and adds the word to the list containing all words of the text
		/// 
		/// this method is only used to make the code more clear and to avoid repetition
		/// </summary>
		/// <param name="words">list contaning all words of the text</param>
		/// <param name="word">word object to be completed</param>
		/// <param name="endIndex">ending index of the word</param>
		/// <param name="lastCharacter">last character of the word</param>
		static void CompleteWordAndAddToList(List<Word> words, Word word, int endIndex, char lastCharacter) {
			word.LastCharacter = lastCharacter;
			word.EndIndex = endIndex;
			words.Add(word);
		}

		/// <summary>
		/// checks if a given character is considered a word seperator
		/// </summary>
		/// <param name="character">character to be checked</param>
		/// <returns>true if the character is a seperator</returns>
		static bool IsSeperator(char character) {
			foreach (char c in SEPERATORS) {
				if (c == character) {
					return true;
				}
			}

			return false;
		}
	}


	/// <summary>
	/// class containing all required information about a word in the text
	/// </summary>
	class Word {

		public int Line { get; set; } // line in which the word is
		public int StartIndex { get; set; } // index of the first word character
		public int EndIndex { get; set; } // index of the last word character
		public char FirstCharacter { get; set; } // first word character
		public char LastCharacter { get; set; } // last word character

		/// <summary>
		/// class constructor
		/// </summary>
		/// <param name="line">index of line containing the word</param>
		/// <param name="startIndex">word starting index</param>
		/// <param name="firstCharacter">first character of the word</param>
		public Word(int line, int startIndex, char firstCharacter) {
			Line = line;
			StartIndex = startIndex;
			FirstCharacter = firstCharacter;
		}
	}


	/// <summary>
	/// class containing all required information about a word fragment where the last word character
	/// matches the first character of the next word
	/// </summary>
	class WordFragment {

		public int StartLine { get; set; } // line in which the word fragment starts
		public int EndLine { get; set; } // line in which the word fragment ends
		public int StartIndex { get; set; } // index of the first fragment word in the text line
		public int EndIndex { get; set; } // index of the last fragment word's last character in the text line
		public int Length { get; set; } // length of the fragment (how many characters)

		/// <summary>
		/// class constructor
		/// </summary>
		/// <param name="startLine">fragment starting line</param>
		/// <param name="startIndex">fragment starting index in the line</param>
		public WordFragment(int startLine, int startIndex) {
			StartLine = startLine;
			StartIndex = startIndex;
		}

		/// <summary>
		/// greater than operator which is used to sort fragments
		/// </summary>
		/// <param name="lhs">left hand side of the comparing operation</param>
		/// <param name="rhs">right hand side of the comparing operation</param>
		/// <returns>true if the first fragment is longer than the second</returns>
		public static bool operator > (WordFragment lhs, WordFragment rhs) {
			return lhs.Length > rhs.Length;
		}

		/// <summary>
		/// less than operator which is used to sort fragments
		/// </summary>
		/// <param name="lhs">left hand side of the comparing operation</param>
		/// <param name="rhs">right hand side of the comparing operation</param>
		/// <returns>true if the first fragment is shorter than the second</returns>
		public static bool operator < (WordFragment lhs, WordFragment rhs) {
			return lhs.Length < rhs.Length;
		}
	}
}
