using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SuggestionTextBox
{
    /// <summary>
    /// Suggestion Text Box control.
    /// </summary>
    public class SuggestionTextBox : TextBox, IDisposable
    {
        #region Privates
        /// <summary>
        /// DataGridView to be used as drop down list for suggestions.
        /// </summary>
        private DataGridView _suggestionBox =  new DataGridView();
        /// <summary>
        /// DataGridView DataSource.
        /// </summary>
        private BindingList<Suggestion> _suggestionData = new BindingList<Suggestion>();
        /// <summary>
        /// Signals a character transfer after the focus moved from 
        /// the DataGridView to the TextBox.
        /// </summary>
        private bool _transferChar;
        /// <summary>
        /// DataGridView maximum height.
        /// </summary>
        private int _suggestionBoxHeight = 80;
        /// <summary>
        /// DataGridView background color.
        /// </summary>
        private Color _suggestionBoxBackColor = Color.White;
        /// <summary>
        /// DataGridView fore color.
        /// </summary>
        private Color _suggestionBoxForeColor = Color.Black;
        /// <summary>
        /// DataGridView selection background color.
        /// </summary>
        private Color _suggestionBoxSelectionColor = Color.Blue;
        /// <summary>
        /// Signals whether the SuggestionBox will show suggestions
        /// taking case sensitivity into account.
        /// </summary>
        private bool _isSuggestionCaseSensitive = true;
        /// <summary>
        /// DataGridView default row height.
        /// </summary>
        private int _suggestionRowHeight;
        /// <summary>
        /// Prevents a new search when a DataGridView element is selected.
        /// </summary>
        private bool _userSelectedSuggestion;
        /// <summary>
        /// Prevents a new search when the user changes
        /// the focus from the DataGridView to the TextBox.
        /// </summary>
        private bool _userFocusedTextBoxByUpKey;
        #endregion

        /// <summary>
        /// Class constructor.
        /// </summary>
        public SuggestionTextBox()
        {
            #region DataGridView/SuggestionBox setup
            // User cannot add columns to the DataGridView.
            _suggestionBox.AllowUserToAddRows = false;
            // Column header should not be visible.
            _suggestionBox.ColumnHeadersVisible = false;
            // Row headers should not be visible.
            _suggestionBox.RowHeadersVisible = false;
            // The only column must fill the width of the DataGridView.
            _suggestionBox.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            // Disable the clipboard on the DataGridView.
            _suggestionBox.ClipboardCopyMode = DataGridViewClipboardCopyMode.Disable;
            // User shuld not be able to resize rows.
            _suggestionBox.AllowUserToResizeRows = false;
            // User can select only one row at once.
            _suggestionBox.MultiSelect = false;
            // Cells must have no borders.
            _suggestionBox.CellBorderStyle = DataGridViewCellBorderStyle.None;
            // The whole row is selected.
            _suggestionBox.SelectionMode = DataGridViewSelectionMode.FullRowSelect;    
            // Suggestions are aligned to the left.
            _suggestionBox.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            // Cells text cannot be edited.
            _suggestionBox.ReadOnly = true;
            // DataGridView's style cannot be FixedSingle, or a black contour appears.
            _suggestionBox.BorderStyle = BorderStyle.Fixed3D;
            // Set the DataGridView text font to match the TextBox font.
            _suggestionBox.Font = Font;
            // Set the SuggestionBox position to match the TextBox position.
            _suggestionBox.Location = Location;
            // Set the SuggestionBox width to match the TextBox width.
            _suggestionBox.Width = Width;
            // DataGridView control cannot be selected with TAB.
            _suggestionBox.TabStop = false;
            // Set the default row height.
            _suggestionRowHeight = _suggestionBox.RowTemplate.Height;
            #endregion

            #region DataGridView events subscription
            // Register to the DataGridView's Resize event.
            _suggestionBox.Resize += SuggestionBox_Resize;
            // Register to the DataGridView's KeyDown event.
            _suggestionBox.KeyDown += SuggestionBox_KeyDown;
            // Register to the DataGridView's KeyPress event.
            _suggestionBox.KeyPress += SuggestionBox_KeyPress;
            // Register to the DataGridView's CellDoubleClick event.
            _suggestionBox.CellDoubleClick += SuggestionBox_CellDoubleClick;
            #endregion

            // Set a CustomSource for the TextBox.
            AutoCompleteSource = AutoCompleteSource.CustomSource;
            // Hide the DataGridView.
            HideSuggestionBox();
        }

        #region DataGridView event handlers
        /// <summary>
        /// Handles the KeyPress event of the DataGridView.
        /// If a character is pressed, it is redirected to the TextBox and
        /// the DataGridView is hidden.
        /// </summary>
        private void SuggestionBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // If there is no character to redirect, return immediately.
            if (!_transferChar) return;
            // Set the flag to signal a redirection is occurred.
            _transferChar = false;     
            // Store the character as string.
            var ch = e.KeyChar.ToString();
            // NOTE: https://msdn.microsoft.com/it-it/library/system.windows.forms.sendkeys.send(v=vs.110).aspx
            /* The plus sign (+), caret (^), percent sign (%), tilde (~), and parentheses () 
             * have special meanings to SendKeys. To specify one of these characters, 
             * enclose it within braces ({}). For example, to specify the plus sign, 
             * use "{+}". To specify brace characters, use "{{}" and "{}}". 
             * Brackets ([ ]) have no special meaning to SendKeys, but you must enclose 
             * them in braces. In other applications, brackets do have a special meaning 
             * that might be significant when dynamic data exchange (DDE) occurs. */
            switch (e.KeyChar)
            {
                case '+':
                case '^':
                case '%':
                case '~':
                case '(':
                case ')':
                case '[':
                case ']':
                case '{':
                case '}':
                    ch = "{" + ch + "}";
                    break;
            }
            // Send the character to the TextBox. Focus must be set on the TextBox
            // (this is done in the KeyDown event handler, that is called before KeyPress).
            SendKeys.Send(ch);
        }
        /// <summary>
        /// Handles the CellDoubleClick event of a DataGridView's cell.
        /// The cell text will be copied to the TextBox.
        /// </summary>
        private void SuggestionBox_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // Signals the user selected one of the suggestions.
            _userSelectedSuggestion = true;
            // Copies the cell's text to the TextBox.
            Text = _suggestionData[e.RowIndex].Value;
            // Hide the DataGridView.
            HideSuggestionBox();
        }
        /// <summary>
        /// Handles the KeyDown event of the DataGridView.
        /// 
        /// - UP: go up one cell on the DataGridView, but if the cell is the upper one
        ///   then move the focus on the TextBox, leaving the DataGridView visible.
        /// 
        /// - DOWN: go down one cell on the DatGridView.
        /// 
        /// - TAB, ESCAPE: hide the DataGridView and move the focus on the TextBox.
        /// 
        /// - ENTER: get the text from the currently selected cell and copies it to
        ///   the TextBox.
        /// 
        /// - DEFAULT: every other character needs to be redirected to the TextBox.
        /// </summary>
        private void SuggestionBox_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    // If this wasn't the top cell, just move to the cell above.
                    if (!_suggestionBox.Rows[0].Selected)
                        return;
                    // Signal the user focused the TextBox.
                    _userFocusedTextBoxByUpKey = true;
                    // If the TextBox is selected, remove every selection from the DataGridView.
                    _suggestionBox.ClearSelection();
                    // Move the focus on the TextBox.
                    Focus();
                    return;

                case Keys.Down:
                    return;

                case Keys.Tab:
                case Keys.Escape:
                    // Hide the DataGridView.
                    HideSuggestionBox();
                    // Signal the user focused the TextBox.
                    _userFocusedTextBoxByUpKey = true;
                    // Move the focus on the TextBox.
                    Focus();
                    return;

                case Keys.Enter:
                    // Signals the user selected a suggestion.
                    _userSelectedSuggestion = true;
                    // Get the currently selected cells (only one is allowed).
                    var cells = _suggestionBox.SelectedCells;
                    // No cells selected: short circuit and exit.
                    if (cells.Count == 0) return;
                    // Get the text from the cell and copy it to the TextBox.
                    Text = cells[0].Value.ToString();
                    // Hide the DataGridView.
                    HideSuggestionBox();
                    // Hack: prevent TextBox OnEnter handler to mess things up.
                    _userFocusedTextBoxByUpKey = true;
                    // Move the focus on the TextBox.
                    Focus();
                    return;

                default:
                    // Hide the DataGridView.
                    HideSuggestionBox();
                    // Set the flag to signal a redirection is occurring.
                    _transferChar = true;
                    // Move the focus on the TextBox.
                    Focus();
                    return;
            }
        }
        /// <summary>
        /// Handles the Resize event of the DataGridView.
        /// </summary>
        private void SuggestionBox_Resize(object sender, EventArgs e)
        {
            // Move the DataGridView below the TextBox.
            MoveSuggestionBox();
            // Match the TextBox width.
            _suggestionBox.Width = Width;
        }
        #endregion

        #region TextBox event handlers overrides
        /// <summary>
        /// Override the OnCreateControl event of the TextBox.
        /// User defined SuggestionTextBox properties are loaded here.
        /// </summary>
        protected override void OnCreateControl()
        {
            // Call the base handler.
            base.OnCreateControl();
            // Set the user defined properties for the SuggestionTextBox.
            _suggestionBox.ForeColor = _suggestionBoxForeColor;
            _suggestionBox.DefaultCellStyle.BackColor = _suggestionBoxBackColor;
            _suggestionBox.DefaultCellStyle.SelectionBackColor = _suggestionBoxSelectionColor;
            _suggestionBox.BackgroundColor = _suggestionBoxBackColor;
            _suggestionBox.Height = _suggestionBoxHeight;
            _isSuggestionCaseSensitive = IsSuggestionBoxCaseSensistive;
        }
        /// <summary>
        /// Override the ParentChanged event of the TextBox.
        /// If the TextBox parent is being changed, the DataGridView parent
        /// will also be changed.
        /// </summary>
        protected override void OnParentChanged(EventArgs e)
        {
            // Call the base handler.
            base.OnParentChanged(e);
            // Change the DataGridView's parent.
            _suggestionBox.Parent = Parent;
        }
        /// <summary>
        /// Override the OnLocationChanged event of the TextBox.
        /// If the TextBox location is being changed, the DataGridView location
        /// will also be changed.
        /// </summary>
        protected override void OnLocationChanged(EventArgs e)
        {
            // Call the base handler.
            base.OnLocationChanged(e);
            // Change the DataGridView's location.
            MoveSuggestionBox();
        }
        /// <summary>
        /// Override the OnResize event of the TextBox.
        /// If the TextBox size is being changed, the DataGridView width
        /// and position will also be changed.
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            // Call the base handler.
            base.OnResize(e);
            // Changes the width of the DataGridView's width to match the TextBox one.
            _suggestionBox.Width = Width;
            // Align the DataGridView with the TextBox.
            MoveSuggestionBox();
        }
        /// <summary>
        /// Override the OnTextChanged event of the TextBox.
        /// When the TextBox Text is being changed, load the 
        /// matching suggestions.
        /// </summary>
        protected override void OnTextChanged(EventArgs e)
        {
            // Call the base handler.
            base.OnTextChanged(e);
            // If the event is raised because the user seleted a suggestion,
            // rest the _userSelectedSuggestion flag and return immediately.
            if (_userSelectedSuggestion)
            {
                _userSelectedSuggestion = false;
                return;
            }
            // If there is no text in the TextBox, hide the DataGridView.
            if (string.IsNullOrEmpty(Text))
            {
                // Reload (or resets, in this case) the suggestions list.
                GetSuggestionBoxData();
                // Hide the DataGridView.
                HideSuggestionBox();
                return;
            }
            // Reload the suggestions list.
            GetSuggestionBoxData();
        }
        /// <summary>
        /// Override the OnKeyDown event of the TextBox.
        /// 
        /// - DOWN: if user presses DOWN and the DataGridView has some loaded suggestions,
        ///   then show the DatGridView, focus it, and select the top cell.
        /// 
        /// - ENTER: hide the DataGridView (user selected a suggestion).
        /// 
        /// - ESC: hide the DataGridView.
        /// </summary>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            // Call the base handler.
            base.OnKeyDown(e);

            switch (e.KeyCode)
            {
                case Keys.Down:
                    // There are no suggestions to show, return immediately.
                    if (_suggestionBox.Rows.Count == 0) return;
                    // The TextBox does not have focus (we're already focused on the DataGridView), 
                    // return immediately.
                    if (!Focused) return;
                    // If the DataGridView is not visible, show it.
                    if (!_suggestionBox.Visible)
                        ShowSuggestionBox();
                    // Move the focus on the DataGridView.
                    _suggestionBox.Focus();
                    // Set the first cell of the DataGridView as selected.
                    _suggestionBox.Rows[0].Selected = true;
                    break;

                case Keys.Enter:
                    // Hide the DataGridView.
                    HideSuggestionBox();
                    break;

                case Keys.Escape:
                    // Hide the DataGridView.
                    HideSuggestionBox();
                    break;
            }
        }
        /// <summary>
        /// Override the OnLeave event of the TextBox.
        /// When the TextBox loses the focus, hide the DatagGridView.
        /// </summary>
        protected override void OnLeave(EventArgs e)
        { 
            // Call the base handler.
            base.OnLeave(e);
            // If the TextBox loses the focus, hide the DatagGridView.
            if (!_suggestionBox.Focused)
                HideSuggestionBox();
        }
        /// <summary>
        /// Override the OnEnter event of the TextBox.
        /// When the TextBox gets the focus, load the DatagGridView with the suggestions.
        /// </summary>
        protected override void OnEnter(EventArgs e)
        { 
            // Call the base handler.
            base.OnEnter(e);
            // Reset the _userFocusedTextBoxByUpKey flag.
            if (_userFocusedTextBoxByUpKey)
            {
                _userFocusedTextBoxByUpKey = false;
                return;
            }
            // Load and show the DataGridView if some suggestions are available.
            GetSuggestionBoxData();
        }
        #endregion
        /// <summary>
        /// Align the DataGridView below the TextBox control.
        /// </summary>
        private void MoveSuggestionBox()
        {
            // Move the DataGridView.
            _suggestionBox.Location = new Point(Left, Top + Height);
        }
        /// <summary>
        /// Gets the filtered suggestions (CASE SENSITIVE).
        /// 
        /// NOTE: I thought about using regexes but, due special characters
        /// like '.', the code would be a little more complex. Feel free
        /// to use them though I don't think this would increase performances. 
        /// </summary>
        private void GetSuggestionBoxDataCS()
        {
            // If the suggestions list is empty, return immediately.
            if (AutoCompleteCustomSource.Count == 0) return;
            // Clears all the suggestions of the list.
            _suggestionData.Clear();
            // This speeds up a little the access to the textbox.
            var txt = Text;
            // Filter the suggestions.
            foreach (string item in AutoCompleteCustomSource)
            {
                // The item is null or empty: next item.
                if (string.IsNullOrEmpty(item)) continue;
                // The text to match is null or empty: break the foreach.
                if (string.IsNullOrEmpty(txt)) break;
                // Load the suggestion if there is a match.
                if(item.Contains(txt))
                    _suggestionData.Add(new Suggestion(item));
            }
        }
        /// <summary>
        /// Gets the filtered suggestions (CASE INSENSITIVE).
        /// 
        /// NOTE: I thought about using regexes but, due special characters
        /// like '.', the code would be a little more complex. Feel free
        /// to use them though I don't think this would increase performances. 
        /// </summary>
        private void GetSuggestionBoxDataCI()
        {
            // If the suggestions list is empty, return immediately.
            if (AutoCompleteCustomSource.Count == 0) return;
            // Clears all the suggestions of the list.
            _suggestionData.Clear();
            // This speeds up a little the access to the textbox.
            var txt = Text;
            // Filter the suggestions.
            foreach (string item in AutoCompleteCustomSource)
            {
                // The item is null or empty: next item.
                if (string.IsNullOrEmpty(item)) continue;
                // The text to match is null or empty: break the foreach.
                if (string.IsNullOrEmpty(txt)) break;
                // Load the suggestion if there is a match.
                if (item.ToLower().Contains(txt.ToLower()))
                    _suggestionData.Add(new Suggestion(item));
            }
        }
        /// <summary>
        /// Loads the suggestions list.
        /// </summary>
        private void GetSuggestionBoxData()
        {
            // Load the suggestions list.
            if (_isSuggestionCaseSensitive)
                GetSuggestionBoxDataCS();
            else
                GetSuggestionBoxDataCI();
            // If the suggestions list is empty, hide the DataGridView
            // and return immediately.
            if (_suggestionData.Count == 0)
            {
                HideSuggestionBox();
                return;
            }
            // Load the DataGridView with the suggestions list.
            FillSuggestionBox();
            // Show the DataGridView.
            ShowSuggestionBox();
        }
        /// <summary>
        /// Hides the DataGridView.
        /// </summary>
        private void HideSuggestionBox()
        {
            _suggestionBox.Hide();
        }
        /// <summary>
        /// Shows the DataGridView.
        /// </summary>
        private void ShowSuggestionBox()
        {
            // Mostra la SuggestionBox.
            _suggestionBox.Show();
            // Porta la SuggestionBox in primo piano.
            _suggestionBox.BringToFront();
            // Toglie qualsiasi selezione.
            _suggestionBox.ClearSelection();
        }
        /// <summary>
        /// Loads the DataGridView with the list of suggestions.
        /// </summary>
        private void FillSuggestionBox()
        {
            // If there are no suggestions to show, return immediately.
            if (_suggestionData.Count == 0) return;
            // Set the DataGridView DataSource.
            _suggestionBox.DataSource = _suggestionData;
            // Calculate the total height of the loaded rows.
            var totHeight = _suggestionData.Count * _suggestionRowHeight;
            // If the total height is below the maximum (user set it), 
            // resize it.
            if (totHeight < _suggestionBoxHeight)
                _suggestionBox.Height = totHeight;
            else
                _suggestionBox.Height = _suggestionBoxHeight;
        }
        /// <summary>
        /// Calls Dispose(true).
        /// </summary>
        public new void Dispose()
        {
            // Call the base class Dispose() method.
            base.Dispose();
            Dispose(true);
            // No finalization needed in this case.
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Executes the real disposal of the class resources.
        /// We don't have any native resource to release.
        /// </summary>
        protected new virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_suggestionBox != null)
                {
                    _suggestionBox.Rows.Clear();
                    _suggestionBox.Dispose();
                    _suggestionBox = null;
                }
                _suggestionData = null;
            }
        }

        #region Public Properties

        [Category("SuggestionBox Properties")]
        [Description("SuggestionBox maximum height.")]
        [DisplayName("SuggestionBox Height")]
        public int SuggestionBoxHeight
        {
            get { return _suggestionBoxHeight; }
            set { _suggestionBoxHeight = value; }
        }

        [Category("SuggestionBox Properties")]
        [Description("SuggestionBox background color.")]
        [DisplayName("SuggestionBox BackColor")]
        public Color SuggestionBoxBackColor
        {
            get { return _suggestionBoxBackColor; }
            set { _suggestionBoxBackColor = value; }
        }

        [Category("SuggestionBox Properties")]
        [Description("SuggestionBox text color.")]
        [DisplayName("SuggestionBox ForeColor")]
        public Color SuggestionBoxForeColor
        {
            get { return _suggestionBoxForeColor; }
            set { _suggestionBoxForeColor = value; }
        }

        [Category("SuggestionBox Properties")]
        [Description("SuggestionBox selected item color.")]
        [DisplayName("SuggestionBox SelectionColor")]
        public Color SuggestionBoxSelectionColor
        {
            get { return _suggestionBoxSelectionColor; }
            set { _suggestionBoxSelectionColor = value; }
        }

        [Category("SuggestionBox Properties")]
        [Description("Enables/disables case sensitivity when matching for suggestions.")]
        [DisplayName("SuggestionBox CaseSensitive")]
        public bool IsSuggestionBoxCaseSensistive
        {
            get { return _isSuggestionCaseSensitive; }
            set { _isSuggestionCaseSensitive = value; }
        }

        /* WARNING: the TextBox behavior is different when the Multiline property is true.
         * For example, our expected behavior when the user is pressing the down key is the
         * DataGridView to be focused but, due to the Multiline property being true, the TextBox 
         * "steals" the focus back immediately: this is because the down key is used to move to
         * the text row below the actual one.
         * 
         * Therefore, I'm disabling the use of the Multiline property.
         */
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Multiline cannot be set in a SuggestionTextBox control.", true)]
        public new bool Multiline { get; set; }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("AutoCompleteMode cannot be set in a SuggestionTextBox control: only CustomSource is allowed.", true)]
        public new AutoCompleteMode AutoCompleteMode { get { return AutoCompleteMode.None; } }
        #endregion
    }
}
