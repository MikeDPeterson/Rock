﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using Rock;
using Rock.Attribute;
using Rock.Data;
using Rock.Model;
using Rock.Security;
using Rock.Web.Cache;
using Rock.Web.UI;
using Rock.Web.UI.Controls;
using church.ccv.Prayer.Model;

namespace RockWeb.Plugins.church_ccv.Prayer
{
    [DisplayName( "Campus Prayer Request Entry" )]
    [Category( "CCV > Prayer" )]
    [Description( "Allows prayer requests to be added via visitors on the website." )]

    // Category Selection
    [CategoryField( "Category Selection", "A top level category. This controls which categories the person can choose from when entering their prayer request.", false, "church.ccv.Prayer.Model.CampusPrayerRequest", "", "", false, "", "Category Selection", 1, "GroupCategoryId" )]
    [CategoryField( "Default Category", "If categories are not being shown, choose a default category to use for all new prayer requests.", false, "church.ccv.Prayer.Model.CampusPrayerRequest", "", "", false, "4B2D88F5-6E45-4B4B-8776-11118C8E8269", "Category Selection", 2, "DefaultCategory" )]

    // Features
    [BooleanField( "Enable Auto Approve", "If enabled, prayer requests are automatically approved; otherwise they must be approved by an admin before they can be seen by the prayer team.", true, "Features", 3 )]
    [IntegerField( "Expires After (Days)", "Number of days until the request will expire (only applies when auto-approved is enabled).", false, 14, "Features", 4, "ExpireDays" )]
    [BooleanField( "Default Allow Comments Setting", "This is the default setting for the 'Allow Comments' on prayer requests. If you enable the 'Comments Flag' below, the requestor can override this default setting.", true, "Features", 5 )]
    [BooleanField( "Enable Urgent Flag", "If enabled, requestors will be able to flag prayer requests as urgent.", false, "Features", 6 )]
    [BooleanField( "Enable Comments Flag", "If enabled, requestors will be able set whether or not they want to allow comments on their requests.", false, "Features", 7 )]
    [BooleanField( "Enable Public Display Flag", "If enabled, requestors will be able set whether or not they want their request displayed on the public website.", false, "Features", 8 )]
    [IntegerField( "Character Limit", "If set to something other than 0, this will limit the number of characters allowed when entering a new prayer request.", false, 250, "Features", 9 )]
    [BooleanField( "Require Last Name", "Require that a last name be entered", true, "Features", 10 )]
    [CampusField( "Default Campus", "The default campus for the request.", true, "", "Features", 15, "DefaultCampusId" )]

    // On Save Behavior
    [BooleanField( "Navigate To Parent On Save", "If enabled, on successful save control will redirect back to the parent page.", false, "On Save Behavior", 11 )]
    [CodeEditorField( "Save Success Text", "Text to display upon successful save. (Only applies if not navigating to parent page on save.) <span class='tip tip-lava'></span><span class='tip tip-html'></span>", CodeEditorMode.Html, CodeEditorTheme.Rock, 200, false, "<p>Thank you for allowing us to pray for you.</p>", "On Save Behavior", 12 )]
    [WorkflowTypeField( "Workflow", "An optional workflow to start when prayer request is created. The PrayerRequest will be set as the workflow 'Entity' attribute when processing is started.", false, false, "", "On Save Behavior", 13 )]
    [BooleanField( "Enable Debug", "Outputs the object graph to help create your liquid syntax.", false, "On Save Behavior", 14 )]    

    public partial class PrayerRequestEntry : RockBlock
    {
        #region Properties
        public int? PrayerRequestEntityTypeId { get; private set; }
        
        // note: the ascx uses these for rendering logic
        public bool EnableUrgentFlag { get; private set; }
        public bool EnableCommentsFlag { get; private set; }
        public bool EnablePublicDisplayFlag { get; private set; }

        #endregion

        #region Base Control Methods
        /// <summary>
        /// Handles the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit( EventArgs e )
        {
            base.OnInit( e );

            RockContext rockContext = new RockContext();

            this.EnableUrgentFlag = GetAttributeValue( "EnableUrgentFlag" ).AsBoolean();
            this.EnableCommentsFlag = GetAttributeValue( "EnableCommentsFlag" ).AsBoolean();
            this.EnablePublicDisplayFlag = GetAttributeValue( "EnablePublicDisplayFlag" ).AsBoolean();
            tbLastName.Required = GetAttributeValue( "RequireLastName" ).AsBooleanOrNull() ?? true;

            RockPage.AddScriptLink( Page, ResolveUrl( "~/Scripts/bootstrap-limit.js" ) );
            var categoryGuid = GetAttributeValue( "GroupCategoryId" );
            if ( ! string.IsNullOrEmpty( categoryGuid ) )
            {
                BindCategories( categoryGuid );

                // set the default category
                if ( !string.IsNullOrWhiteSpace( GetAttributeValue( "DefaultCategory" ) ) )
                {
                    Guid defaultCategoryGuid = GetAttributeValue( "DefaultCategory" ).AsGuid();
                    var defaultCategoryId = CategoryCache.Read( defaultCategoryGuid, rockContext ).Id;

                    bddlCategory.SetValue( defaultCategoryId );
                }
            }
            else
            {
                bddlCategory.Visible = false;
            }

            // bind Campus selection box
            BindCampuses();

            // Set campus to default Campus specified in block settings
            Guid defaultCampusGuid = GetAttributeValue( "DefaultCampusId" ).AsGuid();
            CampusCache campus = CampusCache.All().Where( p => p.Guid == defaultCampusGuid ).SingleOrDefault();

            if ( campus != null )
            {
                bddlCampus.SetValue( campus.Id );
            }
            
            Type type = new CampusPrayerRequest().GetType();
            this.PrayerRequestEntityTypeId = Rock.Web.Cache.EntityTypeCache.GetId( type.FullName );

            int charLimit = GetAttributeValue( "CharacterLimit" ).AsInteger();
            if ( charLimit > 0 )
            {
                dtbRequest.Placeholder = string.Format( "Please pray that... (up to {0} characters)", charLimit );
                string scriptFormat = @"
    function SetCharacterLimit() {{
        $('#{0}').limit({{maxChars: {1}, counter:'#{2}', normalClass:'badge', warningClass:'badge-warning', overLimitClass: 'badge-danger'}});

        $('#{0}').on('cross', function(){{
            $('#{3}').prop('disabled', true);
        }});
        $('#{0}').on('uncross', function(){{
            $('#{3}').prop('disabled', false);
        }});
    }};
    $(document).ready(function () {{ SetCharacterLimit(); }});
    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(SetCharacterLimit);
";
                string script = string.Format(scriptFormat , dtbRequest.ClientID, charLimit, lblCount.ClientID, lbSave.ClientID );
                ScriptManager.RegisterStartupScript( this.Page, this.GetType(), string.Format( "limit-{0}", this.ClientID ), script, true );
            }
        }

        /// <summary>
        /// Handles the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            base.OnLoad( e );

            if ( ! Page.IsPostBack )
            {
                if ( CurrentPerson != null )
                {
                    tbFirstName.Text = CurrentPerson.FirstName;
                    tbLastName.Text = CurrentPerson.LastName;
                    ebEmail.Text = CurrentPerson.Email;
                    bddlCampus.SetValue( CurrentPerson.GetCampus().Id );
                }

                dtbRequest.Text = PageParameter( "Request" );
            }
        }

        #endregion

        #region Events

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles the Click event to save the prayer request.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void btnSave_Click( object sender, EventArgs e )
        {
            if ( ! IsValid() )
            {
                return;
            }

            bool isAutoApproved = GetAttributeValue( "EnableAutoApprove" ).AsBoolean();
            bool defaultAllowComments = GetAttributeValue( "DefaultAllowCommentsSetting" ).AsBoolean();

            CampusPrayerRequest prayerRequest = new CampusPrayerRequest { Id = 0, IsActive = true, IsApproved = isAutoApproved, AllowComments = defaultAllowComments };

            var rockContext = new RockContext();
            Service<CampusPrayerRequest> prayerRequestService = new Service<CampusPrayerRequest>( rockContext );
            prayerRequestService.Add( prayerRequest );
            prayerRequest.EnteredDateTime = RockDateTime.Now;

            if ( isAutoApproved )
            {
                prayerRequest.ApprovedByPersonAliasId = CurrentPersonAliasId;
                prayerRequest.ApprovedOnDateTime = RockDateTime.Now;
                var expireDays = Convert.ToDouble( GetAttributeValue( "ExpireDays" ) );
                prayerRequest.ExpirationDate = RockDateTime.Now.AddDays( expireDays );
            }

            // Now record all the bits...
            // Make sure the Category is hydrated so it's included for any Lava processing
            Category category;
            int? categoryId = bddlCategory.SelectedValueAsInt();
            Guid defaultCategoryGuid = GetAttributeValue( "DefaultCategory" ).AsGuid();
            if ( categoryId == null && !defaultCategoryGuid.IsEmpty() )
            {
                category = new CategoryService( rockContext ).Get( defaultCategoryGuid );
                categoryId = category.Id;
            }
            else
            {
                category = new CategoryService( rockContext ).Get( categoryId.Value );
            }

            prayerRequest.CategoryId = categoryId;
            prayerRequest.Category = category;
            prayerRequest.RequestedByPersonAliasId = CurrentPersonAliasId;
            prayerRequest.FirstName = tbFirstName.Text;
            prayerRequest.LastName = tbLastName.Text;
            prayerRequest.Email = ebEmail.Text;
            prayerRequest.Text = dtbRequest.Text;
            prayerRequest.CampusId = bddlCampus.SelectedValueAsInt();

            if ( this.EnableUrgentFlag )
            {
                prayerRequest.IsUrgent = cbIsUrgent.Checked;
            }
            else
            {
                prayerRequest.IsUrgent = false;
            }

            if ( this.EnableCommentsFlag )
            {
                prayerRequest.AllowComments = cbAllowComments.Checked;
            }

            if ( this.EnablePublicDisplayFlag )
            {
                prayerRequest.IsPublic = cbAllowPublicDisplay.Checked;
            }
            else
            {
                prayerRequest.IsPublic = false;
            }

            if ( !Page.IsValid )
            {
                return;
            }

            if ( !prayerRequest.IsValid )
            {
                // field controls render error messages
                return;
            }

            rockContext.SaveChanges();

            StartWorkflow( prayerRequest, rockContext );

            bool isNavigateToParent = GetAttributeValue( "NavigateToParentOnSave" ).AsBoolean();

            if ( isNavigateToParent )
            {
                NavigateToParentPage();
            }
            else
            {
                pnlForm.Visible = false;
                pnlReceipt.Visible = true;

                // Build success text that is Lava capable
                var mergeFields = Rock.Lava.LavaHelper.GetCommonMergeFields( this.RockPage, this.CurrentPerson );
                mergeFields.Add( "PrayerRequest", prayerRequest );
                nbMessage.Text = GetAttributeValue( "SaveSuccessText" ).ResolveMergeFields( mergeFields );

                // Resolve any dynamic url references
                string appRoot = ResolveRockUrl( "~/" );
                string themeRoot = ResolveRockUrl( "~~/" );
                nbMessage.Text = nbMessage.Text.Replace( "~~/", themeRoot ).Replace( "~/", appRoot );

                // show liquid help for debug
                if ( GetAttributeValue( "EnableDebug" ).AsBoolean() && IsUserAuthorized( Authorization.EDIT ) )
                {
                    nbMessage.Text += mergeFields.lavaDebugInfo();
                }
            }
        }

        /// <summary>
        /// Set up the form for another request from the same person.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        protected void btnAddAnother_Click( object sender, EventArgs e )
        {
            pnlForm.Visible = true;
            pnlReceipt.Visible = false;
            dtbRequest.Text = string.Empty;
            dtbRequest.Focus();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Bind the category selector to the correct set of categories.
        /// </summary>
        private void BindCategories( string categoryGuid )
        {
            Guid guid = new Guid( categoryGuid );

            bddlCategory.DataSource = new CategoryService( new RockContext() ).GetByEntityTypeId( this.PrayerRequestEntityTypeId ).Where( c => c.Guid == guid ||
                ( c.ParentCategory != null && c.ParentCategory.Guid == guid ) ).AsQueryable().ToList();
            bddlCategory.DataTextField = "Name";
            bddlCategory.DataValueField = "Id";
            bddlCategory.DataBind();
        }

        /// <summary>
        /// Bind the campus selector
        /// </summary>
        private void BindCampuses()
        {
            bddlCampus.DataSource = CampusCache.All();
            bddlCampus.DataTextField = "Name";
            bddlCampus.DataValueField = "Id";
            bddlCampus.DataBind();
        }

        /// <summary>
        /// Returns true if the form is valid; false otherwise.
        /// </summary>
        /// <returns></returns>
        private bool IsValid()
        {
            IEnumerable<string> errors = Enumerable.Empty<string>();

            // Check length in case the client side js didn't
            int charLimit = GetAttributeValue( "CharacterLimit" ).AsInteger();
            if ( charLimit > 0  && dtbRequest.Text.Length > charLimit )
            {
                errors = errors.Concat( new[] { string.Format( "Whoops. Would you mind reducing the length of your prayer request to {0} characters?", charLimit ) } );
            }

            if ( errors != null && errors.Count() > 0 )
            {
                nbWarningMessage.Visible = true;
                nbWarningMessage.Text = errors.Aggregate( new StringBuilder( "<ul>" ), ( sb, s ) => sb.AppendFormat( "<li>{0}</li>", s ) ).Append( "</ul>" ).ToString();
                return false;
            }
            else
            {
                nbWarningMessage.Visible = false;
                return true;
            }
        }

        /// <summary>
        /// Starts the workflow if one was defined in the block setting.
        /// </summary>
        /// <param name="prayerRequest">The prayer request.</param>
        /// <param name="rockContext">The rock context.</param>
        private void StartWorkflow( CampusPrayerRequest prayerRequest, RockContext rockContext )
        {
            WorkflowType workflowType = null;
            Guid? workflowTypeGuid = GetAttributeValue( "Workflow" ).AsGuidOrNull();
            if ( workflowTypeGuid.HasValue )
            {
                var workflowTypeService = new WorkflowTypeService( rockContext );
                workflowType = workflowTypeService.Get( workflowTypeGuid.Value );
                if ( workflowType != null )
                {
                    try
                    {
                        var workflow = Workflow.Activate( workflowType, prayerRequest.Name );
                        List<string> workflowErrors;
                        new WorkflowService( rockContext ).Process( workflow, prayerRequest, out workflowErrors );
                    }
                    catch ( Exception ex )
                    {
                        ExceptionLogService.LogException( ex, this.Context );
                    }
                }
            }
        }

        #endregion
    }
}