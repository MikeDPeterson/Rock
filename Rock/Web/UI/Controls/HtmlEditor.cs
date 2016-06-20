﻿// <copyright>
// Copyright by the Spark Development Network
//
// Licensed under the Rock Community License (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.rockrms.com/license
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Rock.Data;
using Rock.Security;
using Rock.Web.Cache;

namespace Rock.Web.UI.Controls
{
    /// <summary>
    /// Control for rendering an html editor
    /// </summary>
    [ToolboxData( "<{0}:HtmlEditor runat=server></{0}:HtmlEditor>" )]
    public class HtmlEditor : TextBox, IRockControl
    {
        #region IRockControl implementation

        /// <summary>
        /// Gets or sets the label text.
        /// </summary>
        /// <value>
        /// The label text.
        /// </value>
        [
        Bindable( true ),
        Category( "Appearance" ),
        DefaultValue( "" ),
        Description( "The text for the label." )
        ]
        public string Label
        {
            get { return ViewState["Label"] as string ?? string.Empty; }
            set { ViewState["Label"] = value; }
        }

        /// <summary>
        /// Gets or sets the form group class.
        /// </summary>
        /// <value>
        /// The form group class.
        /// </value>
        [
        Bindable( true ),
        Category( "Appearance" ),
        Description( "The CSS class to add to the form-group div." )
        ]
        public string FormGroupCssClass
        {
            get { return ViewState["FormGroupCssClass"] as string ?? string.Empty; }
            set { ViewState["FormGroupCssClass"] = value; }
        }

        /// <summary>
        /// Gets or sets the help text.
        /// </summary>
        /// <value>
        /// The help text.
        /// </value>
        [
        Bindable( true ),
        Category( "Appearance" ),
        DefaultValue( "" ),
        Description( "The help block." )
        ]
        public string Help
        {
            get
            {
                return HelpBlock != null ? HelpBlock.Text : string.Empty;
            }

            set
            {
                if ( HelpBlock != null )
                {
                    HelpBlock.Text = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the warning text.
        /// </summary>
        /// <value>
        /// The warning text.
        /// </value>
        [
        Bindable( true ),
        Category( "Appearance" ),
        DefaultValue( "" ),
        Description( "The warning block." )
        ]
        public string Warning
        {
            get
            {
                return WarningBlock != null ? WarningBlock.Text : string.Empty;
            }

            set
            {
                if ( WarningBlock != null )
                {
                    WarningBlock.Text = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="RockTextBox"/> is required.
        /// </summary>
        /// <value>
        ///   <c>true</c> if required; otherwise, <c>false</c>.
        /// </value>
        [
        Bindable( true ),
        Category( "Behavior" ),
        DefaultValue( "false" ),
        Description( "Is the value required?" )
        ]
        public bool Required
        {
            get
            {
                return ViewState["Required"] as bool? ?? false;
            }

            set
            {
                ViewState["Required"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the required error message.  If blank, the LabelName name will be used
        /// </summary>
        /// <value>
        /// The required error message.
        /// </value>
        public string RequiredErrorMessage
        {
            get
            {
                return RequiredFieldValidator != null ? RequiredFieldValidator.ErrorMessage : string.Empty;
            }

            set
            {
                if ( RequiredFieldValidator != null )
                {
                    RequiredFieldValidator.ErrorMessage = value;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is valid.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsValid
        {
            get
            {
                return !Required || RequiredFieldValidator == null || RequiredFieldValidator.IsValid;
            }
        }

        /// <summary>
        /// Gets or sets the help block.
        /// </summary>
        /// <value>
        /// The help block.
        /// </value>
        public HelpBlock HelpBlock { get; set; }

        /// <summary>
        /// Gets or sets the warning block.
        /// </summary>
        /// <value>
        /// The warning block.
        /// </value>
        public WarningBlock WarningBlock { get; set; }

        /// <summary>
        /// Gets or sets the required field validator.
        /// </summary>
        /// <value>
        /// The required field validator.
        /// </value>
        public RequiredFieldValidator RequiredFieldValidator { get; set; }

        /// <summary>
        /// Gets or sets the group of controls for which the <see cref="T:System.Web.UI.WebControls.TextBox" /> control causes validation when it posts back to the server.
        /// </summary>
        /// <returns>The group of controls for which the <see cref="T:System.Web.UI.WebControls.TextBox" /> control causes validation when it posts back to the server. The default value is an empty string ("").</returns>
        public override string ValidationGroup
        {
            get
            {
                return base.ValidationGroup;
            }

            set
            {
                base.ValidationGroup = value;
                RequiredFieldValidator.ValidationGroup = value;
            }
        }

        #endregion

        private HiddenField _hfDisableVrm;
        private HiddenFieldWithClass _hfInCodeEditorMode;
        private CodeEditor _ceEditor;

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public enum ToolbarConfig
        {
            /// <summary>
            /// A lighter more airy view
            /// </summary>
            Light,

            /// <summary>
            /// The full monty
            /// </summary>
            Full
        }

        /// <summary>
        /// Gets or sets the toolbar.
        /// </summary>
        /// <value>
        /// The toolbar.
        /// </value>
        public ToolbarConfig Toolbar
        {
            get
            {
                object toolbarObj = ViewState["Toolbar"];
                if ( toolbarObj != null )
                {
                    return (ToolbarConfig)toolbarObj;
                }
                else
                {
                    return ToolbarConfig.Light;
                }
            }

            set
            {
                ViewState["Toolbar"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum width of the resize.
        /// </summary>
        /// <value>
        /// The maximum width of the resize.
        /// </value>
        public int? ResizeMaxWidth
        {
            get
            {
                return ViewState["ResizeMaxWidth"] as int?;
            }

            set
            {
                ViewState["ResizeMaxWidth"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the custom javascript that will get executed when the editor 'on change' event occurs
        /// </summary>
        /// <value>
        /// The custom on change press script.
        /// </value>
        public string OnChangeScript
        {
            get
            {
                return ViewState["OnChangeScript"] as string;
            }

            set
            {
                ViewState["OnChangeScript"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the document folder root.
        /// Defaults to ~/Content
        /// </summary>
        /// <value>
        /// The document folder root.
        /// </value>
        public string DocumentFolderRoot
        {
            get
            {
                var result = ViewState["DocumentFolderRoot"] as string;
                if ( string.IsNullOrWhiteSpace( result ) )
                {
                    result = "~/Content";
                }

                return result;
            }

            set
            {
                ViewState["DocumentFolderRoot"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the image folder root.
        /// Defaults to ~/Content
        /// </summary>
        /// <value>
        /// The image folder root.
        /// </value>
        public string ImageFolderRoot
        {
            get
            {
                var result = ViewState["ImageFolderRoot"] as string;
                if ( string.IsNullOrWhiteSpace( result ) )
                {
                    result = "~/Content";
                }

                return result;
            }

            set
            {
                ViewState["ImageFolderRoot"] = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether root folder should be specific to user.  If true
        /// a folder name equal to the the current user's login will be added to the root path.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [user specific root]; otherwise, <c>false</c>.
        /// </value>
        public bool UserSpecificRoot
        {
            get
            {
                return ViewState["UserSpecificRoot"] as bool? ?? false;
            }

            set
            {
                ViewState["UserSpecificRoot"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the merge fields to make available.  This should include either a list of
        /// entity type names (full name), or other non-object string values
        /// </summary>
        /// <remarks>
        /// Format should be one of the following formats
        ///     "FieldName"                     - Label will be a case delimited version of FieldName (i.e. "Field Name")
        ///     "FieldName|LabelName"
        ///     "FieldName^EntityType           - Will evaluate the entity type and add a navigable tree for the objects 
        ///                                       properties and attributes. Label will be a case delimited version of 
        ///                                       FieldName (i.e. "Field Name")
        ///     "FieldName^EntityType|LabelName - Will evaluate the entity type and add a navigable tree for the objects 
        ///                                       properties and attributes.    
        ///                                  
        /// Supports the following "special" field names
        ///     "GlobalAttribute"               - Provides navigable list of global attributes
        ///     "Campuses"                      - Will return an array of all campuses
        ///     "Date"                          - Will return lava syntax for displaying current date
        ///     "Time"                          - Will return lava syntax for displaying current time
        ///     "DayOfWeek"                     - Will return lava syntax for displaying the current day of the week
        ///     "PageParameter"                 - Will return lava synax and support for rendering any page parameter 
        ///                                       (query string and/or route parameter value)
        /// </remarks>
        /// <value>
        /// The merge fields.
        /// </value>
        public List<string> MergeFields
        {
            get
            {
                var mergeFields = ViewState["MergeFields"] as List<string>;
                if ( mergeFields == null )
                {
                    mergeFields = new List<string>();
                    ViewState["MergeFields"] = mergeFields;
                }

                return mergeFields;
            }

            set
            {
                ViewState["MergeFields"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the additional configurations.
        /// </summary>
        /// <value>
        /// The additional configurations.
        /// </value>
        [Obsolete( "Doesn't do anything anymore" )]
        public string AdditionalConfigurations
        {
            get { return ViewState["AdditionalConfigurations"] as string ?? string.Empty; }
            set { ViewState["AdditionalConfigurations"] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [start in code editor mode].
        /// </summary>
        /// <value>
        /// <c>true</c> if [start in code editor mode]; otherwise, <c>false</c>.
        /// </value>
        public bool StartInCodeEditorMode
        {
            get
            {
                return ViewState["StartInCodeEditorMode"] as bool? ?? false;
            }

            set
            {
                ViewState["StartInCodeEditorMode"] = value;
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlEditor"/> class.
        /// </summary>
        public HtmlEditor()
            : base()
        {
            RequiredFieldValidator = new RequiredFieldValidator();
            RequiredFieldValidator.ValidationGroup = this.ValidationGroup;
            HelpBlock = new HelpBlock();
            WarningBlock = new WarningBlock();

            TextMode = TextBoxMode.MultiLine;
            Rows = 10;
            Columns = 80;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit( System.EventArgs e )
        {
            base.OnInit( e );

            if ( this.Visible && !ScriptManager.GetCurrent( this.Page ).IsInAsyncPostBack )
            {
                RockPage.AddScriptLink( Page, ResolveUrl( "~/Scripts/summernote/plugins/RockFileBrowser.js" ), true );
                RockPage.AddScriptLink( Page, ResolveUrl( "~/Scripts/summernote/plugins/RockImageBrowser.js" ), true );
                RockPage.AddScriptLink( Page, ResolveUrl( "~/Scripts/summernote/plugins/RockMergeField.js" ), true );
                RockPage.AddScriptLink( Page, ResolveUrl( "~/Scripts/summernote/plugins/RockCodeEditor.js" ), true );
            }

            EnsureChildControls();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            // set this textbox hidden until we can run the js to attach summernote to it
            this.Style[HtmlTextWriterStyle.Display] = "none";

            base.OnLoad( e );

            if ( this.Page.IsPostBack )
            {
                if ( _hfInCodeEditorMode.Value.AsBoolean() )
                {
                    this.Text = _ceEditor.Text;
                }
            }
        }

        /// <summary>
        /// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
        /// </summary>
        protected override void CreateChildControls()
        {
            base.CreateChildControls();
            Controls.Clear();
            RockControlHelper.CreateChildControls( this, Controls );

            _hfDisableVrm = new HiddenField();
            _hfDisableVrm.ID = this.ID + "_dvrm";
            _hfDisableVrm.Value = "True";
            Controls.Add( _hfDisableVrm );

            _hfInCodeEditorMode = new HiddenFieldWithClass();
            _hfInCodeEditorMode.CssClass = "js-incodeeditormode";
            _hfInCodeEditorMode.ID = this.ID + "_hfInCodeEditorMode";
            Controls.Add( _hfInCodeEditorMode );

            _ceEditor = new CodeEditor();
            _ceEditor.ID = this.ID + "_codeEditor";
            _ceEditor.EditorMode = CodeEditorMode.Html;
            Controls.Add( _ceEditor );
        }

        /// <summary>
        /// Outputs server control content to a provided <see cref="T:System.Web.UI.HtmlTextWriter" /> object and stores tracing information about the control if tracing is enabled.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter" /> object that receives the control content.</param>
        public override void RenderControl( HtmlTextWriter writer )
        {
            if ( this.Visible )
            {
                RockControlHelper.RenderControl( this, writer );
                _hfDisableVrm.RenderControl( writer );
                _hfInCodeEditorMode.RenderControl( writer );
                _ceEditor.RenderControl( writer );
            }
        }

        /// <summary>
        /// Renders the base control.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public void RenderBaseControl( HtmlTextWriter writer )
        {
            string summernoteInitScriptFormat = @"
// ensure that summernote.js link is added to page
if (!$('#summernoteJsLib').length) {{
    // by default, jquery adds a cache-busting parameter on dynamically added script tags. set the ajaxSetup cache:true to prevent this
    $.ajaxSetup({{ cache: true }});
    $('head').prepend(""<script id='summernoteJsLib' src='{1}' />"");
}}

var summerNoteEditor = $('#{0}').summernote({{
    height: '{2}', //set editable area's height
    toolbar: Rock.htmlEditor.toolbar_RockCustomConfig{11},

    callbacks: {{
       {12} 
    }},

    buttons: {{
        rockfilebrowser: RockFileBrowser,
        rockimagebrowser: RockImageBrowser, 
        rockmergefield: RockMergeField,
        rockcodeeditor: RockCodeEditor
    }},

    rockFileBrowserOptions: {{ 
        enabled: {3},
        documentFolderRoot: '{4}', 
        imageFolderRoot: '{5}',
        imageFileTypeWhiteList: '{6}',
        fileTypeBlackList: '{7}'
    }},

    rockMergeFieldOptions: {{ 
        enabled: {9},
        mergeFields: '{8}' 
    }},
    rockTheme: '{10}',

    codeEditorOptions: {{
        controlId: '{13}',
        inCodeEditorModeHiddenFieldId: '{14}'
    }},
}});

if ({15} && RockCodeEditor) {{
    RockCodeEditor(summerNoteEditor.data('summernote')).click();
}}
";

            string summernoteJsLib = ( (RockPage)this.Page ).ResolveRockUrl( "~/Scripts/summernote/summernote.min.js", true );
            bool rockMergeFieldEnabled = MergeFields.Any();
            bool rockFileBrowserEnabled = false;

            // only show the File/Image plugin if they have Auth to the file browser page
            var fileBrowserPage = new Rock.Model.PageService( new RockContext() ).Get( Rock.SystemGuid.Page.HTMLEDITOR_ROCKFILEBROWSER_PLUGIN_FRAME.AsGuid() );
            if ( fileBrowserPage != null )
            {
                var currentPerson = this.RockBlock().CurrentPerson;
                if ( currentPerson != null )
                {
                    if ( fileBrowserPage.IsAuthorized( Authorization.VIEW, currentPerson ) )
                    {
                        rockFileBrowserEnabled = true;
                    }
                }
            }

            var globalAttributesCache = GlobalAttributesCache.Read();

            string imageFileTypeWhiteList = globalAttributesCache.GetValue( "ContentImageFiletypeWhitelist" );
            string fileTypeBlackList = globalAttributesCache.GetValue( "ContentFiletypeBlacklist" );

            string documentFolderRoot = this.DocumentFolderRoot;
            string imageFolderRoot = this.ImageFolderRoot;
            if ( this.UserSpecificRoot )
            {
                var currentUser = this.RockBlock().CurrentUser;
                if ( currentUser != null )
                {
                    documentFolderRoot = System.Web.VirtualPathUtility.Combine( documentFolderRoot.EnsureTrailingBackslash(), currentUser.UserName.ToString() );
                    imageFolderRoot = System.Web.VirtualPathUtility.Combine( imageFolderRoot.EnsureTrailingBackslash(), currentUser.UserName.ToString() );
                }
            }

            string callbacksOption = null;
            if ( !string.IsNullOrEmpty( this.OnChangeScript ) )
            {
                callbacksOption = string.Format(
@" onKeyup: function() {{  
    {0}  
}}",
   this.OnChangeScript );
            }

            string summernoteInitScript = string.Format( 
                summernoteInitScriptFormat,
                this.ClientID,   // {0}
                summernoteJsLib, // {1}
                this.Height, // {2}
                rockFileBrowserEnabled.ToTrueFalse().ToLower(),                 // {3}
                Rock.Security.Encryption.EncryptString( documentFolderRoot ),   // {4} encrypt the folders so the folder can only be configured on the server
                Rock.Security.Encryption.EncryptString( imageFolderRoot ),      // {5}
                imageFileTypeWhiteList,                                         // {6}
                fileTypeBlackList,                                              // {7}
                this.MergeFields.AsDelimited( "," ),                            // {8}
                rockMergeFieldEnabled.ToTrueFalse().ToLower(),                  // {9} 
                ( (RockPage)this.Page ).Site.Theme,                             // {10}
                this.Toolbar.ConvertToString(),                                 // {11} 
                callbacksOption,                                                // {12}
                _ceEditor.ClientID,                                             // {13}
                _hfInCodeEditorMode.ClientID,                                   // {14}
                StartInCodeEditorMode.ToTrueFalse().ToLower()                   // {15}
                );                                                

            ScriptManager.RegisterStartupScript( this, this.GetType(), "summernote_init_script_" + this.ClientID, summernoteInitScript, true );

            // add ace.js on demand only when there will be a codeeditor rendered
            if ( ScriptManager.GetCurrent( this.Page ).IsInAsyncPostBack )
            {
                ScriptManager.RegisterClientScriptInclude( this.Page, this.Page.GetType(), "rock-file-browser-plugin", ( (RockPage)this.Page ).ResolveRockUrl( "~/Scripts/summernote/plugins/RockFileBrowser.js", true ) );
                ScriptManager.RegisterClientScriptInclude( this.Page, this.Page.GetType(), "rock-image-browser-plugin", ( (RockPage)this.Page ).ResolveRockUrl( "~/Scripts/summernote/plugins/RockImageBrowser.js", true ) );
                ScriptManager.RegisterClientScriptInclude( this.Page, this.Page.GetType(), "rock-mergefield-plugin", ( (RockPage)this.Page ).ResolveRockUrl( "~/Scripts/summernote/plugins/RockMergeField.js", true ) );
                ScriptManager.RegisterClientScriptInclude( this.Page, this.Page.GetType(), "rock-codeeditor-plugin", ( (RockPage)this.Page ).ResolveRockUrl( "~/Scripts/summernote/plugins/RockCodeEditor.js", true ) );
            }

            base.RenderControl( writer );
        }
    }
}