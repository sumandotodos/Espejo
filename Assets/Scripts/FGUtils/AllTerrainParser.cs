using UnityEngine;
using System.Collections;

public enum ParserMode { begin, end };

public class AllTerrainParser {


	/* properties */
	string theData;
	public int begin, end;
	ParserMode mode;

	public int dataSize() {
		return theData.Length;
	}

	public AllTerrainParser(string d) {

		theData = d;
		begin = end = 0;

	}

	public void setParserMode(ParserMode m) {

		mode = m;

	}

	public bool scanWhileNotCharInList(params char[] chars) {

		bool result = false;

		if (mode == ParserMode.begin) {
			char currentChar = theData [begin];
			bool res = false;
			for (int i = 0; i < chars.Length; ++i) {
				if (currentChar == chars [i]) {
					res = true;
					break;
				}
			}
			while ((begin < theData.Length) && !res) {
				++begin;
				currentChar = theData [begin];
				res = false;
				for (int i = 0; i < chars.Length; ++i) {
					if (currentChar == chars [i]) {
						res = true;
						break;
					}
				}
			}
			if (begin == theData.Length)
				result = false;

			if (end <= begin)
				end = begin + 1;
			
		} 

		else if(mode == ParserMode.end) {
			char currentChar = theData [end];
			bool res = false;
			for (int i = 0; i < chars.Length; ++i) {
				if (currentChar == chars [i]) {
					res = true;
					break;
				}
			}
			while ((end < theData.Length) && !res) {
				++end;
				currentChar = theData [end];
				res = false;
				for (int i = 0; i < chars.Length; ++i) {
					if (currentChar == chars [i]) {
						res = true;
						break;
					}
				}
			}
			if (end == theData.Length)
				result = false;


		}

		return result;

	}

	public bool scanUntilValidChar() {

		bool result = true;

		if (mode == ParserMode.begin) {

			while ((begin < theData.Length) && isValidCharacter(theData [begin])) {
				++begin;
			}
			if (begin == theData.Length)
				result = false;

			if (end <= begin)
				end = begin + 1;

		} else {

			while ((end < theData.Length) && isValidCharacter(theData [end])) {
				++end;
			}
			if (end == theData.Length)
				result = false;
			

		}

		return result;


	}

	public bool scanToNextValidChar() {

		bool result = true;

		if (mode == ParserMode.begin) {

			while ((begin < theData.Length) && !isValidCharacter(theData [begin])) {
				++begin;
			}
			if (begin == theData.Length)
				result = false;
			
			if (end <= begin)
				end = begin + 1;

		} else {

			while ((end < theData.Length) && !isValidCharacter(theData [end])) {
				++end;
			}
			if (end == theData.Length)
				result = false;

		}

		return result;


	}

	public bool scanPastChar(char c) {
		return scanToChar (c);
	}

	// Maybe this should be called scanPastChar
	public bool scanToChar(char c) {

		bool result = true;

		if (mode == ParserMode.begin) {

			while ((begin < theData.Length) && (c != theData [begin])) {
				++begin;
			}
			if (begin < theData.Length)
				++begin;
			else
				result = false;
			if (end <= begin)
				end = begin + 1;

		} else {

			while ((end < theData.Length) && (c != (theData [end]))) {
				++end;
			}
			if (begin < theData.Length)
				++end;
			else
				result = false;

		}

		return result;

	}

	public void addOffset(int off) {

		if (mode == ParserMode.begin) {

			begin += off;

		} else {

			end += off;

		}

	}

	public void rewindEndHead() {

		end = begin + 1;

	}

	public char charAtHead() {

		if (mode == ParserMode.begin) {
			char c = (char)0;

			if (begin > 0) {
				c = theData [begin - 1];
			}
			if (begin < theData.Length) {
				c = theData [begin + 1];
			}
			if(begin < theData.Length) c = theData [begin];
			return c;
		} else {
			char c = (char)0; 
			if (end > 0) {
				c = theData [end - 1];
			}
			if (end < theData.Length) {
				c = theData [end + 1];
			}
			if(end < theData.Length) c = theData [end];
			return c;
		}

	}

	public void scanWhileNotNextLine() {

		if (mode == ParserMode.begin) {

			while ((begin < theData.Length) && !isReturnChar (theData [begin])) {
				++begin;
			}

			if (end <= begin)
				end = begin + 1;

		} else {

			while ((end < theData.Length) && !isReturnChar (theData [end])) {
				++end;
			}

		}

	}

	public void scanToNextLine() {

		if (mode == ParserMode.begin) {

			while ((begin < theData.Length) && !isReturnChar (theData [begin])) {
				++begin;
			}
			if (begin < theData.Length)
				++begin;
			if (end <= begin)
				end = begin + 1;

		} else {

			while ((end < theData.Length) && !isReturnChar (theData [end])) {
				++end;
			}
			if (begin < theData.Length)
				++end;

		}

	}

	public bool isValidCharacter(char c) {

		if (c >= 'A' && c <= 'Z')
			return true;
		if (c >= 'a' && c <= 'z')
			return true;
		if (c >= '0' && c <= '9')
			return true;
		if (c == '_')
			return true;
		if (c == '-')
			return true;

		return false;

	}

	public void scanToNextDQuotes() {

		if (mode == ParserMode.begin) {

			while ((begin < theData.Length) && !isDQuote (theData [begin])) {
				++begin;
			}
			if (begin < theData.Length)
				++begin;
			if (end <= begin)
				end = begin + 1;

		} else {

			while ((end < theData.Length) && !isDQuote (theData [end])) {
				++end;
			}
			if (begin < theData.Length)
				++end;

		}

	}



	public string extract() {

		int jojor;
		jojor = theData.Length;

		if (end >= theData.Length)
			return "";
		return theData.Substring (begin, (end - begin));


	}

	bool isSeparator(char c) {

		if (c == ' ')
			return true;
		if (c == '\n')
			return true;

		return false;

	}

	bool isDQuote(char c) {

		return c == '\"';

	}

	bool isReturnChar(char c) {

		return c == '\n';

	}



}
