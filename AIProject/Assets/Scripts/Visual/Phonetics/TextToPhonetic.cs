using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class TextToPhonetic : MonoBehaviour
{
	static readonly Regex regex = new Regex("[^a-zA-Z _.,?!]");
	public string EnglishToPhonetic(string input)
	{
		//Null check
		if (input is null) return "Null String";
		if (input.Length == 0) return "Empty String";

		//Remove non-alphabetic characters
		input = regex.Replace(input, string.Empty);

		//Initial letter exceptions
		input = Regex.Replace(input, "kn", "n", RegexOptions.IgnoreCase);
		input = Regex.Replace(input, "gn", "n", RegexOptions.IgnoreCase);
		input = Regex.Replace(input, "pn", "n", RegexOptions.IgnoreCase);
		input = Regex.Replace(input, "ae", "e", RegexOptions.IgnoreCase);
		input = Regex.Replace(input, "wr", "r", RegexOptions.IgnoreCase);
		input = Regex.Replace(input, " x", "s", RegexOptions.IgnoreCase);
		input = Regex.Replace(input, " wh", "w", RegexOptions.IgnoreCase);


		//Check if string is 0
		if (input.Length == 0) return "Empty String";

		string[] arrayOfWords = input.Split(' ');

		//First step, remove double letters
		string firstStepString = "";
		//Run through all words in sentence
		foreach (string word in arrayOfWords)
		{
			//Work out if character needs to go
			foreach (char character in word)
			{
				if (firstStepString.Length > 0)
				{
					//If the character is not the same as the previous character, add it to array
					if(firstStepString[firstStepString.Length - 1] != character || (character == 'c' || character == 'C'))
					{
						firstStepString += character;
					}
				}
				else 
				{
					firstStepString += character;
				}
			}
			firstStepString += " ";
		}
		//Apply temp string to input
		input = firstStepString;

		//B Rule
		input = Regex.Replace(input, "mb ", "b ", RegexOptions.IgnoreCase);

		//C Rule
		input = Regex.Replace(input, "sc", "sk", RegexOptions.IgnoreCase);
		//C -> X rule
		input = Regex.Replace(input, "cia", "xia", RegexOptions.IgnoreCase);
		input = Regex.Replace(input, "ch", "xh", RegexOptions.IgnoreCase);
		//C -> S rule
		input = Regex.Replace(input, "ce", "se", RegexOptions.IgnoreCase);
		input = Regex.Replace(input, "ci", "si", RegexOptions.IgnoreCase);
		input = Regex.Replace(input, "cy", "sy", RegexOptions.IgnoreCase);

		//Rest of C -> k
		input = Regex.Replace(input, "c", "k", RegexOptions.IgnoreCase);

		//D -> G rule && G -> drop
		input = Regex.Replace(input, "d?ge", "je", RegexOptions.IgnoreCase);
		input = Regex.Replace(input, "d?gi", "ji", RegexOptions.IgnoreCase);
		input = Regex.Replace(input, "d?gy", "jy", RegexOptions.IgnoreCase);

		//D -> T rule
		input = Regex.Replace(input, "d", "t", RegexOptions.IgnoreCase);

		//G Rules
		input = Regex.Replace(input, "gn", "n", RegexOptions.IgnoreCase);
		input = Regex.Replace(input, "gh ", "h ", RegexOptions.IgnoreCase);
		//Fix this
		input = Regex.Replace(input, "gh=*[bcdfghjklmnpqrstvwxz]*", "h", RegexOptions.IgnoreCase);
		input = Regex.Replace(input, "g", "k", RegexOptions.IgnoreCase);

		//H rules
		//H rules can wait

		//K rules
		input = Regex.Replace(input, "ck", "c", RegexOptions.IgnoreCase);

		//P rules
		input = Regex.Replace(input, "ph", "f", RegexOptions.IgnoreCase);

		//Q rules
		input = Regex.Replace(input, "q", "k", RegexOptions.IgnoreCase);

		//S and T rules
		input = Regex.Replace(input, "sh", "xh", RegexOptions.IgnoreCase);
		input = Regex.Replace(input, "(s|t)ia", "xia", RegexOptions.IgnoreCase);
		input = Regex.Replace(input, "(s|t)io", "xio", RegexOptions.IgnoreCase);

		//T rule
		input = Regex.Replace(input, "th", "0", RegexOptions.IgnoreCase);
		input = Regex.Replace(input, "tch", "ch", RegexOptions.IgnoreCase);

		//V rules
		input = Regex.Replace(input, "v", "f", RegexOptions.IgnoreCase);

		//W rules
		//Do this please

		//Y rules too please

		//Z rules
		input = Regex.Replace(input, "z", "s", RegexOptions.IgnoreCase);






		return input;
	}
}
