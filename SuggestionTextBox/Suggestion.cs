namespace SuggestionTextBox
{
    /// <summary>
    /// This class describes the suggestion objects to be loaded in the 
    /// DataGridView. It's just a wrapper class for the string type.
    /// </summary>
    public class Suggestion
    {
        string _value;
        public Suggestion(string s)
        {
            _value = s;
        }
        public string Value { get { return _value; } set { _value = value; } }
    }
}
