# SuggestionTextBox
Winforms TextBox with suggestion drop down list control

*** Introduction ***

Problem: the standard WinForms TextBox allows an autocompletion list showing only strings STARTING with the user's inputed string. Instead, we want a list of possible suggestions CONTAINING the user inputed text.

*** Example ***:

if the user inputs "B" on the standard TextBox and the autocompletion list is loaded with the string "ABC", the autocomplete drop down list will NOT be shown, as the user input is "B" and the list doesn't contain any string starting with this string.
I needed what I'm going to call a "Suggestion Text Box": the drop down list of the TextBox must show every string *containing* the inputed one. So, in the above example , the drop down list should pop and show "ABC" as a suggestion.
The first thing I tried to achieve this was to find an AutoCompleteMode property value that could fit my needing, but there is none. At the moment I thought "how is this possible, they forgot about this..." but later I come to the conclusion that the behavior of the standard WinForms TextBox is perfectly correct, as the meaning of the autocompletion is different.

*** Background ***

I tought I had to create a new UserControl but, to be honest, I don't think I have enough skills (nor the time) to create one of them from scratch, so, after a lot of searching for a solution, I ended up reading this post on StackOverflow:

https://stackoverflow.com/questions/4470140/sub-class-of-control-that-combines-multiple-controls

This solution seemed clever enough to me, so I decided to try this way: I created a SuggestionTextBox class that inherits from the standard TextBox control class and implements/overrides the necessary logic to achieve the wanted behavior explained in the introduction. The "drop down list" is a DataGridView control.

*** Using the code ***

The attached archive contains the solution file and 2 projects: the DLL project for the control (SuggestionBox.csproj) and a test WinForms project (Test.csproj).
The projects are compiled targeting version 4.6 of the .NET Framework in both Debug and Release mode (I'm using Visual Studio Community 2017).
The Suggestion Text Box has it's own properties section. Every property there can be set *ONLY* in design mode (for now, at least):

The available properties are:

- BackColor: the background color of the whole drop down list of suggestions;

- CaseSensitive: enables/disables case sensitivity when matching for suggestions;

- ForeColor:  the suggestion text color;

- Height: the maximum height of the drop down list;

- SelectionColor: the color of a selected suggestion.

NOTE: the Multiline property cannot be set (this also happens with the standard TextBox control, no drop down is allowed if the control is multiline).

The standard WinForms TextBox behavior if different when the Multiline property is set to true. For example, the SuggestionTextBox expected behavior when the user is pressing the down key is the DataGridView to be focused and the first cell to be selected but, due to the Multiline property being true, the TextBox "steals" the focus back immediately: I suppose this is because the down key is used by the TextBox to move the cursor to the text row below the actual one.
Therefore, I disabled the Multiline property for the SuggestionTextBox.

*** Points of Interest ***
At the beginning, I thought the ListBox control would have been the best control to fake a drop down list but, due to some buggy behavior (I couldn't deselect a selected item before moving the focus), I eventually decided to use a DataGridView (much more flexible, imho).

